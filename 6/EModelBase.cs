using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Assets.DuckType.Jiggle;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

// Token: 0x0200050F RID: 1295
[Preserve]
public abstract class EModelBase : MonoBehaviour
{
	// Token: 0x1700049D RID: 1181
	// (get) Token: 0x06002A7B RID: 10875 RVA: 0x0010C5F2 File Offset: 0x0010A7F2
	// (set) Token: 0x06002A7C RID: 10876 RVA: 0x0010C5FA File Offset: 0x0010A7FA
	public bool IsFPV { get; set; }

	// Token: 0x1700049E RID: 1182
	// (get) Token: 0x06002A7D RID: 10877 RVA: 0x0010C603 File Offset: 0x0010A803
	public virtual Transform NeckTransform
	{
		get
		{
			return this.neckTransform;
		}
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x0010C60C File Offset: 0x0010A80C
	public virtual void Init(World _world, Entity _entity, EModelInstanceAssets _eModelAssets)
	{
		EntityClass entityClass = EntityClass.list[_entity.entityClass];
		this.assets = _eModelAssets;
		this.visible = false;
		this.entity = _entity;
		this.ragdollChance = entityClass.RagdollOnDeathChance;
		this.bHasRagdoll = entityClass.HasRagdoll;
		this.modelTransformParent = EModelBase.FindModel(base.transform);
		this.createModel(_world, entityClass);
		bool flag = this.entity.RootMotion || this.bHasRagdoll;
		if (GameManager.IsDedicatedServer && !flag)
		{
			if (entityClass.Properties.GetString(EntityClass.PropAvatarController).Length > 0)
			{
				this.avatarController = base.gameObject.AddComponent<AvatarControllerDummy>();
				Animator[] componentsInChildren = base.transform.GetComponentsInChildren<Animator>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = false;
				}
			}
		}
		else
		{
			this.createAvatarController(entityClass);
			if (GameManager.IsDedicatedServer && this.avatarController && flag)
			{
				this.avatarController.SetVisible(true);
			}
		}
		this.gazeController = base.transform.GetComponentInChildren<CharacterGazeController>();
		this.InitCommon();
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x0010C724 File Offset: 0x0010A924
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitCommon()
	{
		this.LookAtInit();
		if (this.modelTransformParent)
		{
			this.SwitchModelAndView(false, EntityClass.list[this.entity.entityClass].bIsMale);
		}
		else
		{
			this.modelTransformParent = base.transform;
			this.headTransform = base.transform;
		}
		this.InitRigidBodies();
		this.JiggleInit();
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x0010C78C File Offset: 0x0010A98C
	public void InitRigidBodies()
	{
		if (!this.bipedRootTransform)
		{
			return;
		}
		List<Rigidbody> list = new List<Rigidbody>();
		this.bipedRootTransform.GetComponentsInChildren<Rigidbody>(list);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (list[i].gameObject.CompareTag("AudioRigidBody"))
			{
				list.RemoveAt(i);
			}
		}
		float num = EntityClass.list[this.entity.entityClass].MassKg / (float)list.Count;
		if (list.Count == 11)
		{
			num *= 1.1224489f;
			int j = 0;
			while (j < list.Count)
			{
				Rigidbody rigidbody = list[j];
				float num2 = 1f;
				bool flag = false;
				string name = rigidbody.name;
				uint num3 = <PrivateImplementationDetails>.ComputeStringHash(name);
				if (num3 <= 2248339218U)
				{
					if (num3 <= 977255260U)
					{
						if (num3 != 819656463U)
						{
							if (num3 != 928972447U)
							{
								if (num3 != 977255260U)
								{
									goto IL_2C9;
								}
								if (!(name == "RightUpLeg"))
								{
									goto IL_2C9;
								}
							}
							else
							{
								if (!(name == "RightForeArm"))
								{
									goto IL_2C9;
								}
								goto IL_295;
							}
						}
						else if (!(name == "LeftUpLeg"))
						{
							goto IL_2C9;
						}
						num2 = 1f;
					}
					else
					{
						if (num3 != 1413449684U)
						{
							if (num3 != 2231561599U)
							{
								if (num3 != 2248339218U)
								{
									goto IL_2C9;
								}
								if (!(name == "Spine2"))
								{
									goto IL_2C9;
								}
							}
							else if (!(name == "Spine1"))
							{
								goto IL_2C9;
							}
						}
						else if (!(name == "Spine"))
						{
							goto IL_2C9;
						}
						num2 = 2f;
						flag = true;
					}
				}
				else
				{
					if (num3 <= 2996251363U)
					{
						if (num3 != 2449536012U)
						{
							if (num3 != 2925602325U)
							{
								if (num3 != 2996251363U)
								{
									goto IL_2C9;
								}
								if (!(name == "Head"))
								{
									goto IL_2C9;
								}
								num2 = 0.8f;
								goto IL_2C9;
							}
							else if (!(name == "RightArm"))
							{
								goto IL_2C9;
							}
						}
						else
						{
							if (!(name == "LeftLeg"))
							{
								goto IL_2C9;
							}
							goto IL_2BF;
						}
					}
					else if (num3 <= 3119934960U)
					{
						if (num3 != 3001187991U)
						{
							if (num3 != 3119934960U)
							{
								goto IL_2C9;
							}
							if (!(name == "LeftForeArm"))
							{
								goto IL_2C9;
							}
							goto IL_295;
						}
						else
						{
							if (!(name == "RightLeg"))
							{
								goto IL_2C9;
							}
							goto IL_2BF;
						}
					}
					else if (num3 != 3537553655U)
					{
						if (num3 != 4018002826U)
						{
							goto IL_2C9;
						}
						if (!(name == "LeftArm"))
						{
							goto IL_2C9;
						}
					}
					else
					{
						if (!(name == "Hips"))
						{
							goto IL_2C9;
						}
						num2 = 2f;
						goto IL_2C9;
					}
					num2 = 0.5f;
					goto IL_2C9;
					IL_2BF:
					num2 = 0.5f;
					flag = true;
				}
				IL_2C9:
				rigidbody.mass = num * num2;
				if (flag && !this.entity.isEntityRemote)
				{
					rigidbody.gameObject.GetOrAddComponent<CollisionCallForward>().Entity = this.entity;
				}
				if (rigidbody.drag <= 0f)
				{
					rigidbody.drag = 0.25f;
				}
				j++;
				continue;
				IL_295:
				num2 = 0.5f;
				flag = true;
				goto IL_2C9;
			}
			return;
		}
		for (int k = 0; k < list.Count; k++)
		{
			list[k].mass = num;
		}
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void PostInit()
	{
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x0010CAE4 File Offset: 0x0010ACE4
	public static Transform FindModel(Transform _t)
	{
		Transform transform = _t.Find("Graphics/Model");
		if (transform)
		{
			return transform;
		}
		return _t;
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnUnload()
	{
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x0010CB08 File Offset: 0x0010AD08
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		this.assets.Release();
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x0010CB15 File Offset: 0x0010AD15
	public void OriginChanged(Vector3 _deltaPos)
	{
		this.ragdollPosePelvisPos += _deltaPos;
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x0010CB2C File Offset: 0x0010AD2C
	public virtual Vector3 GetHeadPosition()
	{
		if (this.headTransform == null)
		{
			return this.entity.position + Vector3.up * this.entity.GetEyeHeight();
		}
		return this.headTransform.position + Origin.position;
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x0010CB82 File Offset: 0x0010AD82
	public virtual Vector3 GetNavObjectPosition()
	{
		if (this.NavObjectTransform == null)
		{
			return this.GetHeadPosition();
		}
		return this.NavObjectTransform.position + Origin.position;
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x0010CBB0 File Offset: 0x0010ADB0
	public virtual Vector3 GetHipPosition()
	{
		if (this.bipedPelvisTransform == null)
		{
			return this.entity.position + Vector3.up * (this.entity.height * 0.5f);
		}
		return this.bipedPelvisTransform.position + Origin.position;
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x0010CC0C File Offset: 0x0010AE0C
	public virtual Quaternion GetFlatHipRotation()
	{
		Vector3 vector;
		if (this.bipedPelvisTransform == null)
		{
			vector = this.entity.rotation;
		}
		else
		{
			vector = this.bipedPelvisTransform.eulerAngles;
		}
		vector = new Vector3(0f, vector.y, 0f);
		return Quaternion.Euler(vector);
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x0010CC60 File Offset: 0x0010AE60
	public virtual Vector3 GetChestPosition()
	{
		if (this.bipedPelvisTransform == null || this.headTransform == null)
		{
			return Vector3.Lerp(this.GetHipPosition(), this.GetHeadPosition(), 0.4f);
		}
		return Vector3.Lerp(this.bipedPelvisTransform.position, this.headTransform.position, 0.6f) + Origin.position;
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x0010CCCC File Offset: 0x0010AECC
	public virtual Vector3 GetBellyPosition()
	{
		if (this.bipedPelvisTransform == null || this.headTransform == null)
		{
			return Vector3.Lerp(this.GetHipPosition(), this.GetHeadPosition(), 0.2f);
		}
		return Vector3.Lerp(this.bipedPelvisTransform.position, this.headTransform.position, 0.2f) + Origin.position;
	}

	// Token: 0x06002A8C RID: 10892 RVA: 0x0010CD38 File Offset: 0x0010AF38
	public IKController AddIKController()
	{
		IKController ikcontroller = null;
		Transform transform = this.GetModelTransform();
		if (transform)
		{
			Animator componentInChildren = transform.GetComponentInChildren<Animator>();
			if (componentInChildren)
			{
				ikcontroller = componentInChildren.GetComponent<IKController>();
				if (!ikcontroller)
				{
					ikcontroller = componentInChildren.gameObject.AddComponent<IKController>();
				}
			}
		}
		return ikcontroller;
	}

	// Token: 0x06002A8D RID: 10893 RVA: 0x0010CD84 File Offset: 0x0010AF84
	public void RemoveIKController()
	{
		Transform transform = this.GetModelTransform();
		if (transform)
		{
			IKController componentInChildren = transform.GetComponentInChildren<IKController>();
			if (componentInChildren)
			{
				componentInChildren.Cleanup();
				UnityEngine.Object.Destroy(componentInChildren);
			}
		}
	}

	// Token: 0x06002A8E RID: 10894 RVA: 0x0010CDBC File Offset: 0x0010AFBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CrouchUpdate(EntityAlive _ea)
	{
		Transform transform = this.neckParentTransform;
		Transform parent = transform.parent;
		float crouchBendPer = _ea.crouchBendPer;
		Quaternion rhs = Quaternion.Euler(28f * crouchBendPer, 0f, 0f);
		transform.localRotation *= rhs;
		parent.localRotation *= rhs;
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x0010CE18 File Offset: 0x0010B018
	[PublicizedFrom(EAccessModifier.Protected)]
	public void LookAtInit()
	{
		EntityClass entityClass = EntityClass.list[this.entity.entityClass];
		this.lookAtMaxAngle = entityClass.LookAtAngle;
		this.lookAtEnabled = (this.lookAtMaxAngle > 0f);
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x0010CE5A File Offset: 0x0010B05A
	public void ClearLookAt()
	{
		this.lookAtBlendPerTarget = 0f;
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x0010CE67 File Offset: 0x0010B067
	public void SetLookAt(Vector3 _pos)
	{
		this.lookAtPos = _pos;
		this.lookAtBlendPerTarget = this.lookAtFullBlendPer;
		this.lookAtIsPos = true;
	}

	// Token: 0x06002A92 RID: 10898 RVA: 0x0010CE83 File Offset: 0x0010B083
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetLookAt()
	{
		this.lookAtBlendPer = 0f;
		this.lookAtBlendPerTarget = 0f;
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x0010CE9C File Offset: 0x0010B09C
	[PublicizedFrom(EAccessModifier.Private)]
	public void LookAtUpdate(EntityAlive e)
	{
		if (this.gazeController != null)
		{
			return;
		}
		EnumEntityStunType currentStun = e.bodyDamage.CurrentStun;
		float deltaTime = Time.deltaTime;
		if (e.IsDead() || (currentStun != EnumEntityStunType.None && currentStun != EnumEntityStunType.Getup))
		{
			this.lookAtBlendPerTarget = 0f;
		}
		else if (!this.lookAtIsPos)
		{
			this.lookAtBlendPerTarget -= deltaTime;
			Vector3 vector;
			if (e.GetHeadLookTarget(out vector))
			{
				this.lookAtPos = vector;
				this.lookAtBlendPerTarget = this.lookAtFullBlendPer;
			}
		}
		if (this.lookAtBlendPer <= 0f && this.lookAtBlendPerTarget <= 0f)
		{
			return;
		}
		this.lookAtFullChangeTime -= deltaTime;
		if (this.lookAtFullChangeTime <= 0f)
		{
			this.lookAtFullChangeTime = 1.3f + 2.7f * e.rand.RandomFloat;
			this.lookAtFullBlendPer = 0.2f + 1.5f * e.rand.RandomFloat;
			if (this.lookAtFullBlendPer > 1f)
			{
				this.lookAtFullBlendPer = 1f;
			}
		}
		this.lookAtBlendPer = Mathf.MoveTowards(this.lookAtBlendPer, this.lookAtBlendPerTarget, deltaTime * 1.5f);
		Quaternion rotation = this.neckParentTransform.rotation;
		Transform transform = this.headTransform;
		Vector3 upwards = rotation * Vector3.up;
		Quaternion quaternion;
		if (this.entity is EntityNPC && !(this.avatarController is AvatarSDCSController))
		{
			quaternion = Quaternion.LookRotation(this.lookAtPos - Origin.position - transform.position);
			quaternion *= Quaternion.AngleAxis(-90f, Vector3.forward);
		}
		else
		{
			quaternion = Quaternion.LookRotation(this.lookAtPos - Origin.position - transform.position, upwards);
			quaternion *= Quaternion.Slerp(Quaternion.identity, transform.localRotation, 0.5f);
		}
		Quaternion b = Quaternion.RotateTowards(rotation, quaternion, this.lookAtMaxAngle);
		this.lookAtRot = Quaternion.Slerp(this.lookAtRot, b, 0.16f);
		float num = this.lookAtBlendPer;
		this.neckTransform.rotation = Quaternion.Slerp(this.neckTransform.rotation, this.lookAtRot, num * 0.4f);
		Quaternion rotation2 = transform.rotation;
		transform.rotation = Quaternion.Slerp(rotation2, this.lookAtRot, num);
	}

	// Token: 0x06002A94 RID: 10900 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void FixedUpdate()
	{
	}

	// Token: 0x06002A95 RID: 10901 RVA: 0x0010D0EA File Offset: 0x0010B2EA
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		this.FrameUpdateRagdoll();
		if (this.modelTransformParent != this.headTransform)
		{
			this.UpdateHeadState();
		}
	}

	// Token: 0x06002A96 RID: 10902 RVA: 0x0010D10C File Offset: 0x0010B30C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void LateUpdate()
	{
		if (this.ragdollIsBlending)
		{
			this.BlendRagdoll();
		}
		EntityAlive entityAlive = this.entity as EntityAlive;
		if (entityAlive != null && !this.IsRagdollActive)
		{
			if (entityAlive.crouchType > 0)
			{
				this.CrouchUpdate(entityAlive);
			}
			if (this.lookAtEnabled)
			{
				this.LookAtUpdate(entityAlive);
			}
		}
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x0010D15D File Offset: 0x0010B35D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Transform GetModelTransform()
	{
		if (!this.modelTransform)
		{
			return this.modelTransformParent;
		}
		return this.modelTransform;
	}

	// Token: 0x06002A98 RID: 10904 RVA: 0x0010D179 File Offset: 0x0010B379
	public Transform GetModelTransformParent()
	{
		return this.modelTransformParent;
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x0010D184 File Offset: 0x0010B384
	public virtual Transform GetHitTransform(DamageSource _damageSource)
	{
		string hitTransformName = _damageSource.getHitTransformName();
		if (hitTransformName != null && this.bipedRootTransform)
		{
			return this.bipedRootTransform.FindInChilds(hitTransformName, false);
		}
		return null;
	}

	// Token: 0x06002A9A RID: 10906 RVA: 0x0010D1B8 File Offset: 0x0010B3B8
	public virtual Transform GetHitTransform(BodyPrimaryHit _primary)
	{
		if (this.physicsBody != null)
		{
			string tag;
			switch (_primary)
			{
			case BodyPrimaryHit.Torso:
				tag = "E_BP_Body";
				break;
			case BodyPrimaryHit.Head:
				tag = "E_BP_Head";
				break;
			case BodyPrimaryHit.LeftUpperArm:
				tag = "E_BP_LArm";
				break;
			case BodyPrimaryHit.RightUpperArm:
				tag = "E_BP_RArm";
				break;
			case BodyPrimaryHit.LeftUpperLeg:
				tag = "E_BP_LLeg";
				break;
			case BodyPrimaryHit.RightUpperLeg:
				tag = "E_BP_RLeg";
				break;
			case BodyPrimaryHit.LeftLowerArm:
				tag = "E_BP_LLowerArm";
				break;
			case BodyPrimaryHit.RightLowerArm:
				tag = "E_BP_RLowerArm";
				break;
			case BodyPrimaryHit.LeftLowerLeg:
				tag = "E_BP_LLowerLeg";
				break;
			case BodyPrimaryHit.RightLowerLeg:
				tag = "E_BP_RLowerLeg";
				break;
			default:
				return null;
			}
			return this.physicsBody.GetTransformForColliderTag(tag);
		}
		return null;
	}

	// Token: 0x06002A9B RID: 10907 RVA: 0x0010D262 File Offset: 0x0010B462
	public virtual Transform GetHeadTransform()
	{
		if (!this.headTransform)
		{
			return base.transform;
		}
		return this.headTransform;
	}

	// Token: 0x06002A9C RID: 10908 RVA: 0x0010D27E File Offset: 0x0010B47E
	public virtual Transform GetPelvisTransform()
	{
		return this.bipedPelvisTransform;
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x0010D286 File Offset: 0x0010B486
	public virtual Transform GetThirdPersonCameraTransform()
	{
		return base.transform;
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x0010D290 File Offset: 0x0010B490
	public virtual void OnDeath(DamageResponse _dmResponse, ChunkCluster _cc)
	{
		EntityAlive entityAlive = this.entity as EntityAlive;
		bool flag = entityAlive && entityAlive.bodyDamage.CurrentStun > EnumEntityStunType.None;
		bool flag2 = true;
		if (this.HasRagdoll() && (flag2 || !this.entity.HasDeathAnim || entityAlive.IsSleeper || entityAlive.GetWalkType() == 21 || flag || _dmResponse.Random < this.ragdollChance))
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.DoRagdoll(_dmResponse, EModelBase.RagdollMode.Default, 999999f);
				return;
			}
			if (!entityAlive.IsSpawned())
			{
				this.SpawnWithRagdoll();
				return;
			}
		}
		else if (this.avatarController != null)
		{
			this.avatarController.StartDeathAnimation(_dmResponse.HitBodyPart, _dmResponse.MovementState, _dmResponse.Random);
			if (this.entity is EntityPlayer && this.bipedRootTransform)
			{
				Transform transform = this.bipedRootTransform.Find("Spine1");
				if (transform)
				{
					RagdollWhenHit ragdollWhenHit;
					if (transform.TryGetComponent<RagdollWhenHit>(out ragdollWhenHit))
					{
						ragdollWhenHit.enabled = true;
						return;
					}
					transform.gameObject.AddComponent<RagdollWhenHit>();
				}
			}
		}
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void restoreTPose(PhysicsBodyInstance physicsBody)
	{
	}

	// Token: 0x06002AA0 RID: 10912 RVA: 0x0010D3AE File Offset: 0x0010B5AE
	public virtual bool HasRagdoll()
	{
		return this.bHasRagdoll && this.physicsBody != null;
	}

	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x06002AA1 RID: 10913 RVA: 0x0010D3C3 File Offset: 0x0010B5C3
	public bool IsRagdollActive
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.ragdollState > EModelBase.ERagdollState.Off;
		}
	}

	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x06002AA2 RID: 10914 RVA: 0x0010D3CE File Offset: 0x0010B5CE
	public bool IsRagdollMovement
	{
		get
		{
			return this.ragdollState != EModelBase.ERagdollState.Off && this.ragdollState != EModelBase.ERagdollState.StandCollide;
		}
	}

	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x06002AA3 RID: 10915 RVA: 0x0010D3E6 File Offset: 0x0010B5E6
	public bool IsRagdollOn
	{
		get
		{
			return this.ragdollState == EModelBase.ERagdollState.On || this.ragdollState == EModelBase.ERagdollState.Dead;
		}
	}

	// Token: 0x170004A2 RID: 1186
	// (get) Token: 0x06002AA4 RID: 10916 RVA: 0x0010D3FC File Offset: 0x0010B5FC
	public bool IsRagdollDead
	{
		get
		{
			return this.ragdollState == EModelBase.ERagdollState.Dead;
		}
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x0010D408 File Offset: 0x0010B608
	public void DoRagdoll(EModelBase.RagdollMode _mode, float stunTime, EnumBodyPartHit bodyPart, Vector3 forceVec, Vector3 forceWorldPos, bool isRemote)
	{
		if (this.entity.IsFlyMode.Value || this.entity.AttachedToEntity)
		{
			return;
		}
		if (!isRemote && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.entity.isEntityRemote)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityRagdoll>().Setup(this.entity, (byte)_mode, stunTime, bodyPart, forceVec, forceWorldPos), false);
			return;
		}
		bool flag = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.entity.isEntityRemote;
		if (stunTime == 0f)
		{
			if (!flag)
			{
				this.entity.PhysicsPush(forceVec, forceWorldPos, false);
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.entity.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entity.entityId, -1, NetPackageManager.GetPackage<NetPackageEntityRagdoll>().Setup(this.entity, (byte)_mode, 0f, EnumBodyPartHit.Torso, forceVec, forceWorldPos), false);
			}
			return;
		}
		if (!this.StartRagdoll(stunTime))
		{
			return;
		}
		if (forceVec.sqrMagnitude > 0f && !this.entity.isEntityRemote)
		{
			Vector3 vector = forceVec;
			if (bodyPart == EnumBodyPartHit.None)
			{
				float num = -10f;
				if (vector.y < num)
				{
					vector.y = num;
				}
				this.SetRagdollVelocity(vector);
			}
			else
			{
				BodyPrimaryHit primary = bodyPart.ToPrimary();
				Transform hitTransform = this.GetHitTransform(primary);
				if (hitTransform)
				{
					Rigidbody component = hitTransform.GetComponent<Rigidbody>();
					if (component)
					{
						if (_mode != EModelBase.RagdollMode.FullForce)
						{
							this.ClampForceForMass(ref vector);
						}
						if (forceWorldPos.sqrMagnitude > 0f)
						{
							component.AddForceAtPosition(vector, forceWorldPos - Origin.position, ForceMode.Impulse);
						}
						else
						{
							component.AddForce(vector, ForceMode.Impulse);
						}
					}
				}
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.entity.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entity.entityId, -1, NetPackageManager.GetPackage<NetPackageEntityRagdoll>().Setup(this.entity, (byte)_mode, stunTime, bodyPart, forceVec, forceWorldPos), false);
		}
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x0010D5F8 File Offset: 0x0010B7F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClampForceForMass(ref Vector3 _force)
	{
		float num = EntityClass.list[this.entity.entityClass].MassKg * 8f;
		float num2 = _force.sqrMagnitude + 0.001f;
		if (num2 > num * num)
		{
			_force *= num / Mathf.Sqrt(num2);
		}
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x0010D654 File Offset: 0x0010B854
	public void DoRagdoll(in DamageResponse dr, EModelBase.RagdollMode _mode = EModelBase.RagdollMode.Default, float stunTime = 999999f)
	{
		EntityAlive entityAlive = this.entity as EntityAlive;
		if (entityAlive && entityAlive.isDisintegrated)
		{
			return;
		}
		float num = (float)dr.Strength;
		DamageSource source = dr.Source;
		if (num > 0f && source != null)
		{
			Vector3 vector = source.getDirection();
			EnumDamageTypes damageType = source.GetDamageType();
			if (damageType != EnumDamageTypes.Falling && damageType != EnumDamageTypes.Crushing)
			{
				float num2;
				if (dr.HitBodyPart == EnumBodyPartHit.None)
				{
					num2 = this.entity.rand.RandomRange(5f, 25f);
				}
				else
				{
					float min = -10f;
					if (stunTime == 0f)
					{
						min = 5f;
					}
					num2 = this.entity.rand.RandomRange(min, 40f);
					num *= 0.5f;
					if (source.damageType == EnumDamageTypes.Bashing)
					{
						num *= 2.5f;
					}
					if (dr.Critical)
					{
						num2 += 25f;
						num *= 2f;
					}
					if ((dr.HitBodyPart & EnumBodyPartHit.Head) > EnumBodyPartHit.None)
					{
						num *= 0.45f;
					}
					num = Utils.FastMin(20f + num, 500f);
					vector *= num;
				}
				Vector3 axis = Vector3.Cross(vector.normalized, Vector3.up);
				vector = Quaternion.AngleAxis(num2, axis) * vector;
			}
			this.DoRagdoll(_mode, stunTime, dr.HitBodyPart, vector, source.getHitTransformPosition(), false);
			return;
		}
		this.DoRagdoll(_mode, stunTime, dr.HitBodyPart, Vector3.zero, Vector3.zero, false);
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x0010D7C9 File Offset: 0x0010B9C9
	public void SetRagdollState(int newState)
	{
		if (this.ragdollState == EModelBase.ERagdollState.On && newState == 2)
		{
			this.ragdollTime = 9999f;
		}
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x0010D7E3 File Offset: 0x0010B9E3
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnWithRagdoll()
	{
		if (this.ragdollState == EModelBase.ERagdollState.Off)
		{
			this.ragdollState = EModelBase.ERagdollState.SpawnWait;
		}
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x0010D7F4 File Offset: 0x0010B9F4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool StartRagdoll(float stunTime)
	{
		if (!this.HasRagdoll())
		{
			return false;
		}
		if (this.ragdollState == EModelBase.ERagdollState.Dead)
		{
			return true;
		}
		bool flag = this.entity.IsDead();
		if (!flag && this.ragdollState == EModelBase.ERagdollState.On)
		{
			this.ragdollDuration = Utils.FastMax(this.ragdollDuration, stunTime);
			return true;
		}
		if (this.entity.IsMarkedForUnload())
		{
			return false;
		}
		this.ragdollAnimator = this.avatarController.GetAnimator();
		if (!this.ragdollAnimator)
		{
			return false;
		}
		bool flag2 = this.ragdollState > EModelBase.ERagdollState.Off;
		this.ragdollState = EModelBase.ERagdollState.On;
		this.ragdollTime = 0f;
		this.entity.OnRagdoll(true);
		this.ragdollIsPlayer = (this.entity is EntityPlayer);
		this.ragdollAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		this.ragdollAnimator.keepAnimatorStateOnDisable = true;
		this.ragdollAnimator.enabled = false;
		Animation component = this.GetModelTransform().GetComponent<Animation>();
		if (component)
		{
			component.cullingType = AnimationCullingType.AlwaysAnimate;
			component.enabled = false;
		}
		this.CaptureRagdollBones();
		this.CaptureRagdollZeroBones();
		if (flag)
		{
			stunTime = 0.3f;
			this.SetRagdollDead();
		}
		if (this.physicsBody != null)
		{
			this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.Ragdoll);
		}
		this.entity.PhysicsPause();
		EntityAlive entityAlive = this.entity as EntityAlive;
		entityAlive.SetStun(EnumEntityStunType.Prone);
		entityAlive.bodyDamage.StunDuration = 1f;
		entityAlive.SetCVar("ragdoll", 1f);
		if (!this.ragdollIsPlayer)
		{
			this.entity.PhysicsTransform.gameObject.SetActive(false);
		}
		this.ragdollTime = 0f;
		this.ragdollDuration = stunTime;
		this.ragdollRotY = this.entity.rotation.y;
		if (this.ragdollIsPlayer)
		{
			this.ragdollDuration *= 0.5f;
		}
		if (flag2)
		{
			this.ragdollIsBlending = false;
			this.ragdollAdjustPosDelay = 0f;
		}
		this.ResetLookAt();
		return true;
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x0010D9DC File Offset: 0x0010BBDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetRagdollDead()
	{
		this.ragdollState = EModelBase.ERagdollState.Dead;
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			Rigidbody rb = this.ragdollPoses[i].rb;
			if (rb)
			{
				rb.maxDepenetrationVelocity = 2f;
				rb.maxAngularVelocity = 1f;
			}
		}
	}

	// Token: 0x06002AAC RID: 10924 RVA: 0x0010DA38 File Offset: 0x0010BC38
	public void DisableRagdoll(bool isSetAlive)
	{
		if (this.ragdollState == EModelBase.ERagdollState.Off)
		{
			return;
		}
		if (this.physicsBody != null)
		{
			this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.Collision);
		}
		if (this.bipedRootTransform)
		{
			RagdollWhenHit[] componentsInChildren = this.bipedRootTransform.GetComponentsInChildren<RagdollWhenHit>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i]);
			}
		}
		EntityAlive entityAlive = this.entity as EntityAlive;
		if (!isSetAlive && !this.entity.IsDead())
		{
			Vector3 vector = this.headTransform.position - this.bipedPelvisTransform.position;
			float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			this.ragdollIsFacingUp = false;
			this.ragdollIsAnimal = EntityClass.list[entityAlive.entityClass].bIsAnimalEntity;
			string stateName;
			if (this.ragdollIsAnimal)
			{
				stateName = "Knockdown";
			}
			else
			{
				stateName = "Knockdown - Chest";
				if (this.bipedPelvisTransform.forward.y > 0f)
				{
					this.ragdollIsFacingUp = true;
					stateName = "Knockdown - Back";
					num += 180f;
				}
			}
			this.ragdollRotY = num;
			this.CopyRagdollRot();
			Animation component = this.GetModelTransform().GetComponent<Animation>();
			if (component != null)
			{
				component.cullingType = AnimationCullingType.AlwaysAnimate;
				component.enabled = true;
			}
			this.avatarController.ResetAnimations();
			this.ragdollAnimator.keepAnimatorStateOnDisable = true;
			int layer = this.ragdollIsPlayer ? 5 : 0;
			this.ragdollAnimator.CrossFadeInFixedTime(stateName, 0.25f, layer, 2f, 0f);
			this.ragdollAdjustPosDelay = 0.05f;
			this.ragdollState = EModelBase.ERagdollState.BlendOutGround;
			this.ragdollTime = 0f;
			this.ragdollIsBlending = true;
			entityAlive.SetStun(EnumEntityStunType.Getup);
			return;
		}
		this.bipedPelvisTransform.localPosition = this.ragdollPosePelvisLocalPos;
		this.RestoreRagdollStartRot();
		this.SetRagdollOff();
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x0010DC14 File Offset: 0x0010BE14
	[PublicizedFrom(EAccessModifier.Private)]
	public void CaptureRagdollBones()
	{
		if (this.ragdollPoses.Count > 0)
		{
			return;
		}
		Animator animator = this.avatarController.GetAnimator();
		if (!animator)
		{
			return;
		}
		this.ragdollPosePelvisLocalPos = this.bipedPelvisTransform.localPosition;
		animator.GetComponentsInChildren<Rigidbody>(EModelBase.ragdollTempRBs);
		for (int i = 0; i < EModelBase.ragdollTempRBs.Count; i++)
		{
			Rigidbody rigidbody = EModelBase.ragdollTempRBs[i];
			GameObject gameObject = rigidbody.gameObject;
			if (!gameObject.CompareTag("Item") && !gameObject.CompareTag("AudioRigidBody"))
			{
				Transform transform = rigidbody.transform;
				EModelBase.RagdollPose item;
				item.t = transform;
				item.rb = rigidbody;
				item.rot = Quaternion.identity;
				item.startRot = transform.localRotation;
				this.ragdollPoses.Add(item);
			}
		}
		EModelBase.ragdollTempRBs.Clear();
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x0010DCF0 File Offset: 0x0010BEF0
	public void CaptureRagdollPositions(List<Vector3> positionList)
	{
		this.CaptureRagdollBones();
		positionList.Clear();
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			positionList.Add(this.ragdollPoses[i].t.position);
		}
	}

	// Token: 0x06002AAF RID: 10927 RVA: 0x0010DD3C File Offset: 0x0010BF3C
	public void ApplyRagdollVelocities(List<Vector3> velocities)
	{
		if (velocities.Count == this.ragdollPoses.Count)
		{
			for (int i = 0; i < this.ragdollPoses.Count; i++)
			{
				Rigidbody rb = this.ragdollPoses[i].rb;
				if (rb)
				{
					rb.velocity = velocities[i];
				}
			}
		}
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x0010DD9C File Offset: 0x0010BF9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetRagdollVelocity(Vector3 _vel)
	{
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			this.ragdollPoses[i].rb.velocity = _vel;
		}
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x0010DDD8 File Offset: 0x0010BFD8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CopyRagdollRot()
	{
		this.ragdollPosePelvisPos = this.bipedPelvisTransform.position;
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			EModelBase.RagdollPose ragdollPose = this.ragdollPoses[i];
			if (ragdollPose.t == this.bipedPelvisTransform)
			{
				ragdollPose.rot = ragdollPose.t.rotation;
			}
			else
			{
				ragdollPose.rot = ragdollPose.t.localRotation;
			}
			this.ragdollPoses[i] = ragdollPose;
		}
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x0010DE60 File Offset: 0x0010C060
	[PublicizedFrom(EAccessModifier.Private)]
	public void RestoreRagdollStartRot()
	{
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			this.ragdollPoses[i].t.localRotation = this.ragdollPoses[i].startRot;
		}
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x0010DEAC File Offset: 0x0010C0AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CaptureRagdollZeroBones()
	{
		if (this.ragdollZeroBones != null)
		{
			return;
		}
		Transform parent = this.headTransform;
		if (!parent)
		{
			return;
		}
		this.ragdollZeroTime = 0.33f;
		this.ragdollZeroBones = new List<Transform>();
		while ((parent = parent.parent) && !(parent == this.bipedPelvisTransform))
		{
			if (!parent.GetComponent<Rigidbody>())
			{
				GameObject gameObject = parent.gameObject;
				if (!gameObject.CompareTag("Item") && !gameObject.CompareTag("AudioRigidBody"))
				{
					this.ragdollZeroBones.Add(parent);
				}
			}
		}
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x0010DF44 File Offset: 0x0010C144
	[PublicizedFrom(EAccessModifier.Private)]
	public void BlendRagdollZeroBones()
	{
		if (this.ragdollZeroTime > 0f)
		{
			float deltaTime = Time.deltaTime;
			this.ragdollZeroTime -= deltaTime;
			for (int i = 0; i < this.ragdollZeroBones.Count; i++)
			{
				Transform transform = this.ragdollZeroBones[i];
				transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, 120f * deltaTime);
			}
		}
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x0010DFB0 File Offset: 0x0010C1B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BlendRagdoll()
	{
		if (this.ragdollAdjustPosDelay > 0f)
		{
			this.ragdollAdjustPosDelay -= Time.deltaTime;
			if (this.ragdollAdjustPosDelay <= 0f)
			{
				Vector3 vector = this.bipedPelvisTransform.position - this.modelTransform.position;
				Vector3 vector2 = this.ragdollPosePelvisPos;
				vector2.x -= vector.x;
				vector2.z -= vector.z;
				if (this.entity.isEntityRemote)
				{
					vector2 = Vector3.Lerp(vector2, this.entity.targetPos - Origin.position, Time.fixedDeltaTime * 10f);
				}
				int num = 0;
				RaycastHit raycastHit;
				while (num < 5 && Physics.Raycast(vector2, Vector3.down, out raycastHit, 3f, -538750989))
				{
					RootTransformRefEntity component = raycastHit.transform.GetComponent<RootTransformRefEntity>();
					if (!component || component.RootTransform != this.entity.transform)
					{
						vector2.y = raycastHit.point.y + 0.02f;
						break;
					}
					vector2.y = raycastHit.point.y - 0.01f;
					num++;
				}
				this.entity.PhysicsResume(vector2 + Origin.position, this.ragdollRotY);
			}
		}
		float num2 = this.ragdollTime / 0.7f;
		float num3 = num2;
		if (!this.ragdollIsAnimal)
		{
			num3 = (num2 - 0.2f) / 0.8f;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
		}
		this.bipedPelvisTransform.position = Vector3.Lerp(this.ragdollPosePelvisPos, this.bipedPelvisTransform.position, num3);
		for (int i = 0; i < this.ragdollPoses.Count; i++)
		{
			Transform t = this.ragdollPoses[i].t;
			if (t == this.bipedPelvisTransform)
			{
				t.rotation = Quaternion.Slerp(this.ragdollPoses[i].rot, t.rotation, num3);
			}
			else
			{
				t.localRotation = Quaternion.Slerp(this.ragdollPoses[i].rot, t.localRotation, num2);
			}
		}
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x0010E1FC File Offset: 0x0010C3FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void FrameUpdateRagdoll()
	{
		if (this.ragdollState == EModelBase.ERagdollState.Off)
		{
			return;
		}
		if (this.ragdollState == EModelBase.ERagdollState.SpawnWait)
		{
			Chunk chunk = (Chunk)this.entity.world.GetChunkFromWorldPos(this.entity.GetBlockPosition());
			if (chunk != null && chunk.IsCollisionMeshGenerated && chunk.IsDisplayed)
			{
				this.ragdollState = EModelBase.ERagdollState.Off;
				this.StartRagdoll(float.MaxValue);
			}
			return;
		}
		bool flag = this.entity.IsDead();
		if (this.pelvisRB && this.IsRagdollMovement)
		{
			if (!flag && this.entity.isEntityRemote)
			{
				Vector3 position = this.pelvisRB.position;
				Vector3 b = this.entity.targetPos - Origin.position;
				this.pelvisRB.AddForce(-EModelBase.serverPosSpringForce * (position - b) - EModelBase.serverPosSpringDamping * this.pelvisRB.velocity, ForceMode.Acceleration);
			}
			if (!(this.entity is EntityPlayer))
			{
				this.entity.SetPosition(this.pelvisRB.position + Origin.position, false);
			}
			else if (!this.entity.isEntityRemote)
			{
				this.entity.SetPosition(this.pelvisRB.position + Origin.position, false);
			}
		}
		this.entity.SetRotationAndStopTurning(new Vector3(0f, this.ragdollRotY, 0f));
		this.ragdollTime += Time.deltaTime;
		switch (this.ragdollState)
		{
		case EModelBase.ERagdollState.On:
			this.BlendRagdollZeroBones();
			if (this.ragdollTime >= this.ragdollDuration && (this.pelvisRB.velocity.sqrMagnitude <= 0.25f || this.ragdollTime > 10f))
			{
				this.DisableRagdoll(false);
				if (!this.entity.isEntityRemote)
				{
					NetPackageEntityRagdoll package = NetPackageManager.GetPackage<NetPackageEntityRagdoll>().Setup(this.entity, (byte)this.ragdollState);
					if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
					{
						this.entity.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entity.entityId, -1, package, false);
						return;
					}
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
					return;
				}
			}
			break;
		case EModelBase.ERagdollState.BlendOutGround:
			this.RestoreRagdollStartRot();
			if (this.ragdollTime >= 0.25f)
			{
				string stateName = "GetUpChest";
				if (this.ragdollIsFacingUp)
				{
					stateName = "GetUpBack";
				}
				if ((this.entity as EntityAlive).IsWalkTypeACrawl())
				{
					stateName = "CrawlerGetUpChest";
				}
				int layer = this.ragdollIsPlayer ? 5 : 0;
				this.ragdollAnimator.CrossFade(stateName, 0.3f, layer);
				this.ragdollState = EModelBase.ERagdollState.BlendOutStand;
				return;
			}
			break;
		case EModelBase.ERagdollState.BlendOutStand:
			this.RestoreRagdollStartRot();
			if (this.ragdollTime >= 0.7f)
			{
				this.ragdollIsBlending = false;
				this.ragdollState = EModelBase.ERagdollState.Stand;
				this.ragdollTime = 0f;
				return;
			}
			break;
		case EModelBase.ERagdollState.Stand:
			if (this.ragdollTime >= 0.8f)
			{
				if (!this.ragdollIsPlayer)
				{
					this.entity.PhysicsTransform.gameObject.SetActive(true);
				}
				this.ragdollState = EModelBase.ERagdollState.StandCollide;
				return;
			}
			break;
		case EModelBase.ERagdollState.StandCollide:
			if (this.ragdollTime >= 1.7f)
			{
				this.SetRagdollOff();
				this.entity.OnRagdoll(false);
				return;
			}
			break;
		case EModelBase.ERagdollState.SpawnWait:
			break;
		case EModelBase.ERagdollState.Dead:
			this.BlendRagdollZeroBones();
			if (this.ragdollTime >= this.ragdollDuration)
			{
				this.ragdollDuration = float.MaxValue;
				if (this.physicsBody != null)
				{
					this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.RagdollDead);
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x0010E590 File Offset: 0x0010C790
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetRagdollOff()
	{
		this.ragdollState = EModelBase.ERagdollState.Off;
		EntityAlive entityAlive = this.entity as EntityAlive;
		entityAlive.SetCVar("ragdoll", 0f);
		entityAlive.ClearStun();
		if (!this.ragdollIsPlayer)
		{
			this.entity.PhysicsTransform.gameObject.SetActive(true);
		}
		this.ragdollPoses.Clear();
		this.ragdollZeroBones = null;
		this.CheckAnimFreeze();
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x0010E5FA File Offset: 0x0010C7FA
	public string GetRagdollDebugInfo()
	{
		return string.Format("{0:0.#}/{1:0.#} {2}", this.ragdollTime.ToCultureInvariantString(), this.ragdollDuration.ToCultureInvariantString(), this.ragdollState.ToStringCached<EModelBase.ERagdollState>());
	}

	// Token: 0x06002AB9 RID: 10937 RVA: 0x0010E628 File Offset: 0x0010C828
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ClothSimInit()
	{
		this.clothSim = base.GetComponentsInChildren<Cloth>();
		if (this.clothSim != null)
		{
			for (int i = this.clothSim.Length - 1; i >= 0; i--)
			{
				this.clothSim[i].gameObject.SetActive(false);
			}
			if (GameManager.IsDedicatedServer)
			{
				this.clothSim = null;
			}
			this.isClothSimOn = false;
		}
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x0010E688 File Offset: 0x0010C888
	public void ClothSimOn(bool _on, bool _force = false)
	{
		if (this.clothSim == null || (this.isClothSimOn == _on && !_force))
		{
			return;
		}
		bool flag = this.entity is EntityPlayerLocal;
		this.isClothSimOn = _on;
		for (int i = this.clothSim.Length - 1; i >= 0; i--)
		{
			Cloth cloth = this.clothSim[i];
			cloth.gameObject.SetActive(_on);
			if (_on && flag)
			{
				cloth.worldAccelerationScale = 0.3f;
			}
		}
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x0010E6FC File Offset: 0x0010C8FC
	public void ClearClothMotion()
	{
		for (int i = this.clothSim.Length - 1; i >= 0; i--)
		{
			this.clothSim[i].ClearTransformMotion();
		}
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x0010E72C File Offset: 0x0010C92C
	[PublicizedFrom(EAccessModifier.Private)]
	public void JiggleInit()
	{
		this.jiggles = base.GetComponentsInChildren<Jiggle>();
		if (GameManager.IsDedicatedServer && this.jiggles != null)
		{
			for (int i = this.jiggles.Length - 1; i >= 0; i--)
			{
				this.jiggles[i].gameObject.SetActive(false);
			}
			this.jiggles = null;
		}
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x0010E784 File Offset: 0x0010C984
	public void JiggleOn(bool _on)
	{
		if (this.jiggles == null || this.isJiggleOn == _on)
		{
			return;
		}
		this.isJiggleOn = _on;
		for (int i = this.jiggles.Length - 1; i >= 0; i--)
		{
			this.jiggles[i].gameObject.SetActive(_on);
		}
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x0010E7D4 File Offset: 0x0010C9D4
	public void LogJiggles()
	{
		if (this.jiggles == null || this.jiggles.Length == 0)
		{
			return;
		}
		Log.Out(string.Format("{0}_{1} jiggle count: {2}", this.entity.GetDebugName(), this.entity.entityId, this.jiggles.Length));
		for (int i = 0; i < this.jiggles.Length; i++)
		{
			Jiggle jiggle = this.jiggles[i];
			Log.Out(string.Format("\t{0} {1}", i, jiggle.DebugString()));
		}
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x0010E864 File Offset: 0x0010CA64
	public virtual void SwitchModelAndView(bool _bFPV, bool _bMale)
	{
		if (this.GetModelTransform() != null)
		{
			Animator component = this.GetModelTransform().GetComponent<Animator>();
			if (component != null)
			{
				component.enabled = !_bFPV;
			}
		}
		this.IsFPV = _bFPV;
		if (this.modelName != null && this.modelTransformParent != null)
		{
			this.modelTransform = this.modelTransformParent.Find(this.modelName);
			this.meshTransform = GameUtils.FindTagInDirectChilds(this.modelTransform, "E_Mesh");
			if (!this.meshTransform)
			{
				this.meshTransform = this.modelTransform.Find("LOD0");
				if (!this.meshTransform)
				{
					Renderer componentInChildren = this.modelTransform.GetComponentInChildren<Renderer>();
					if (componentInChildren)
					{
						this.meshTransform = componentInChildren.transform;
					}
				}
			}
		}
		Transform transform = this.GetModelTransform();
		if (this.avatarController)
		{
			this.avatarController.SwitchModelAndView(this.modelName, _bFPV, _bMale);
		}
		else if (transform)
		{
			transform.gameObject.SetActive(true);
		}
		this.headTransform = GameUtils.FindTagInChilds(transform, "E_BP_Head");
		if (this.headTransform)
		{
			this.neckTransform = this.headTransform.parent;
			this.neckParentTransform = this.neckTransform.parent;
		}
		this.bipedRootTransform = GameUtils.FindTagInChilds(transform, "E_BP_BipedRoot");
		if (this.bipedRootTransform == null)
		{
			foreach (string text in EModelBase.commonBips)
			{
				if ((this.bipedRootTransform = transform.Find(text)) != null || (this.bipedRootTransform = transform.FindInChilds(text, false)) != null)
				{
					break;
				}
			}
		}
		if (this.bipedRootTransform != null)
		{
			if (this.bipedRootTransform.name != "pelvis" && this.bipedRootTransform.name != "Hips")
			{
				this.bipedPelvisTransform = GameUtils.FindChildWithPartialName(this.bipedRootTransform, new string[]
				{
					"pelvis"
				});
				if (this.bipedPelvisTransform == null)
				{
					this.bipedPelvisTransform = GameUtils.FindChildWithPartialName(this.bipedRootTransform, new string[]
					{
						"hips"
					});
					if (this.bipedPelvisTransform == null)
					{
						this.bipedPelvisTransform = GameUtils.FindChildWithPartialName(this.bipedRootTransform, new string[]
						{
							"hip"
						});
					}
				}
			}
			else
			{
				this.bipedPelvisTransform = this.bipedRootTransform;
			}
		}
		if (this.bipedPelvisTransform)
		{
			this.pelvisRB = this.bipedPelvisTransform.GetComponent<Rigidbody>();
			if (!(this.entity is EntityPlayer))
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.modelTransformParent.GetComponentsInChildren<SkinnedMeshRenderer>(true))
				{
					if (skinnedMeshRenderer.quality == SkinQuality.Auto)
					{
						skinnedMeshRenderer.quality = SkinQuality.Bone2;
					}
					Bounds localBounds = skinnedMeshRenderer.localBounds;
					if (skinnedMeshRenderer.CompareTag("E_BP_Eye"))
					{
						if (this.headTransform)
						{
							skinnedMeshRenderer.rootBone = this.headTransform;
							localBounds.center = Vector3.zero;
							localBounds.extents = new Vector3(0.25f, 0.25f, 0.25f);
						}
						skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
					}
					else if (skinnedMeshRenderer.rootBone != this.bipedPelvisTransform)
					{
						skinnedMeshRenderer.rootBone = this.bipedPelvisTransform;
						localBounds.center += -this.bipedPelvisTransform.localPosition;
					}
					skinnedMeshRenderer.localBounds = localBounds;
				}
			}
		}
		EntityAlive entityAlive = this.entity as EntityAlive;
		if (entityAlive && entityAlive.inventory != null)
		{
			if (this.entity.isEntityRemote)
			{
				entityAlive.inventory.ForceHoldingItemUpdate();
			}
			else if (this.entity is EntityPlayerLocal && (this.entity as EntityPlayerLocal).PlayerUI != null && !(this.entity as EntityPlayerLocal).PlayerUI.windowManager.IsWindowOpen("character"))
			{
				entityAlive.inventory.ForceHoldingItemUpdate();
			}
		}
		if (this.GetRightHandTransform() != null)
		{
			SkinnedMeshRenderer[] componentsInChildren2 = this.GetRightHandTransform().GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int k = 0; k < componentsInChildren2.Length; k++)
			{
				componentsInChildren2[k].updateWhenOffscreen = true;
			}
		}
		this.NavObjectTransform = base.transform.FindInChilds("IconTag", false);
		PhysicsBodyLayout physicsBodyLayout = EntityClass.list[this.entity.entityClass].PhysicsBody;
		if (physicsBodyLayout != null && this.bipedRootTransform != null)
		{
			if (this.physicsBody != null)
			{
				this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.Disabled);
			}
			this.physicsBody = new PhysicsBodyInstance(this.bipedRootTransform, physicsBodyLayout, EnumColliderMode.Collision);
			this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.Collision);
			RagdollWhenHit[] componentsInChildren3 = this.bipedRootTransform.GetComponentsInChildren<RagdollWhenHit>();
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				componentsInChildren3[i].enabled = false;
			}
		}
		else
		{
			this.physicsBody = null;
		}
		if (transform != null)
		{
			Animator component2 = transform.GetComponent<Animator>();
			if (component2 != null)
			{
				component2.enabled = !this.IsFPV;
			}
			this.SetColliderLayers(transform, 0);
		}
		this.CheckAnimFreeze();
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x0010EDC4 File Offset: 0x0010CFC4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetColliderLayers(Transform modelT, int layer)
	{
		CapsuleCollider[] componentsInChildren = modelT.GetComponentsInChildren<CapsuleCollider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			GameObject gameObject = componentsInChildren[i].gameObject;
			if (!gameObject.CompareTag("LargeEntityBlocker") && !gameObject.CompareTag("Physics"))
			{
				gameObject.layer = layer;
			}
		}
		BoxCollider[] componentsInChildren2 = modelT.GetComponentsInChildren<BoxCollider>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].gameObject.layer = layer;
		}
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x0010EE38 File Offset: 0x0010D038
	public virtual void SetInRightHand(Transform _transform)
	{
		if (this.avatarController != null)
		{
			this.avatarController.SetInRightHand(_transform);
		}
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x0010EE54 File Offset: 0x0010D054
	public virtual void SetAlive()
	{
		this.DisableRagdoll(true);
		Transform transform = this.GetModelTransform();
		if (transform)
		{
			Utils.MoveTaggedToLayer(transform.gameObject, "LargeEntityBlocker", 19);
			Animator component = transform.GetComponent<Animator>();
			if (component != null)
			{
				component.enabled = !this.IsFPV;
			}
		}
		if (this.avatarController != null)
		{
			this.avatarController.SetAlive();
		}
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x0010EEC4 File Offset: 0x0010D0C4
	public virtual void SetDead()
	{
		Transform transform = this.GetModelTransform();
		if (transform)
		{
			Utils.MoveTaggedToLayer(transform.gameObject, "LargeEntityBlocker", 17);
		}
		if (this.ragdollState == EModelBase.ERagdollState.On)
		{
			this.SetRagdollDead();
			if (this.physicsBody != null && this.physicsBody.Mode != EnumColliderMode.RagdollDead)
			{
				this.physicsBody.SetColliderMode(EnumColliderType.All, EnumColliderMode.RagdollDead);
			}
		}
	}

	// Token: 0x170004A3 RID: 1187
	// (get) Token: 0x06002AC4 RID: 10948 RVA: 0x0010EF24 File Offset: 0x0010D124
	// (set) Token: 0x06002AC5 RID: 10949 RVA: 0x0010EF2C File Offset: 0x0010D12C
	public bool visible { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06002AC6 RID: 10950 RVA: 0x0010EF38 File Offset: 0x0010D138
	public virtual void SetVisible(bool _bVisible, bool _isKeepColliders = false)
	{
		this.visible = _bVisible;
		if (_isKeepColliders)
		{
			this.modelTransformParent.GetComponentsInChildren<Renderer>(EModelBase.rendererList);
			for (int i = 0; i < EModelBase.rendererList.Count; i++)
			{
				EModelBase.rendererList[i].enabled = _bVisible;
			}
			EModelBase.rendererList.Clear();
			if (!_bVisible)
			{
				return;
			}
		}
		if (this.avatarController != null)
		{
			this.avatarController.SetVisible(_bVisible);
		}
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x0010EFB0 File Offset: 0x0010D1B0
	public void SetFade(float _fade)
	{
		if (this.matPropBlock == null)
		{
			this.matPropBlock = new MaterialPropertyBlock();
		}
		this.modelTransformParent.GetComponentsInChildren<Renderer>(EModelBase.rendererList);
		for (int i = 0; i < EModelBase.rendererList.Count; i++)
		{
			Renderer renderer = EModelBase.rendererList[i];
			bool flag = false;
			Material material = renderer.material;
			string name = material.shader.name;
			if (material.HasProperty("_Fade") && name.Contains("Game/Character"))
			{
				flag = true;
			}
			if (renderer.gameObject.CompareTag("LOD") || renderer.gameObject.CompareTag("E_Mesh") || flag)
			{
				this.matPropBlock.SetFloat(EModelBase.fadeId, _fade);
				renderer.SetPropertyBlock(this.matPropBlock);
			}
		}
		EModelBase.rendererList.Clear();
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x0010F086 File Offset: 0x0010D286
	public virtual Transform GetRightHandTransform()
	{
		if (this.avatarController != null)
		{
			return this.avatarController.GetRightHandTransform();
		}
		return null;
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetSkinTexture(string _textureName)
	{
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x0010F0A4 File Offset: 0x0010D2A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void createModel(World _world, EntityClass _ec)
	{
		if (this.modelTransformParent == null)
		{
			return;
		}
		Transform transform = null;
		if (_ec.IsPrefabCombined && this.modelTransformParent.childCount > 0)
		{
			transform = this.modelTransformParent.GetChild(0);
		}
		if (this.assets.Mesh)
		{
			transform = UnityEngine.Object.Instantiate<Transform>(this.assets.Mesh, this.modelTransformParent, false);
			transform.name = this.assets.Mesh.name;
			Vector3 localPosition = transform.localPosition;
			if ((double)localPosition.z < -0.5 || (double)localPosition.z > 0.5)
			{
				Log.Warning("createModel mesh moved {0} {1} {2}", new object[]
				{
					transform.name,
					localPosition.ToString("f3"),
					transform.localRotation
				});
			}
			localPosition.x = 0f;
			localPosition.y = 0f;
			transform.localPosition = localPosition;
		}
		if (transform)
		{
			transform.gameObject.SetActive(true);
			this.modelName = transform.name;
			if (_ec.particleOnSpawn.fileName != null)
			{
				ParticleSystem particleSystem = DataLoader.LoadAsset<ParticleSystem>(_ec.particleOnSpawn.fileName, false);
				if (particleSystem != null)
				{
					ParticleSystem particleSystem2 = UnityEngine.Object.Instantiate<ParticleSystem>(particleSystem);
					particleSystem2.transform.parent = this.modelTransformParent;
					if (_ec.particleOnSpawn.shapeMesh != null && _ec.particleOnSpawn.shapeMesh.Length > 0)
					{
						SkinnedMeshRenderer[] componentsInChildren = base.GetComponentsInChildren<SkinnedMeshRenderer>();
						ParticleSystem.ShapeModule shape = particleSystem2.shape;
						shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
						string text = _ec.particleOnSpawn.shapeMesh.ToLower();
						if (text.Contains("setshapetomesh"))
						{
							text = text.Replace("setshapetomesh", "");
							int num = int.Parse(text);
							if (num >= 0 && num < componentsInChildren.Length)
							{
								shape.skinnedMeshRenderer = componentsInChildren[num];
								ParticleSystem[] componentsInChildren2 = particleSystem2.transform.GetComponentsInChildren<ParticleSystem>();
								if (componentsInChildren2 != null)
								{
									for (int i = 0; i < componentsInChildren2.Length; i++)
									{
										shape = componentsInChildren2[i].shape;
										shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
										shape.skinnedMeshRenderer = componentsInChildren[num];
									}
								}
							}
						}
					}
				}
			}
			bool flag = false;
			this.AltMaterial = this.assets.AltMaterial;
			if (this.AltMaterial != null)
			{
				flag = true;
			}
			Color value = Color.black;
			string @string = _ec.Properties.GetString(EntityClass.PropMatColor);
			if (@string.Length > 0)
			{
				value = EntityClass.sColors[@string];
				flag = true;
			}
			if (flag)
			{
				base.GetComponentsInChildren<SkinnedMeshRenderer>(true, EModelBase.skinnedRendererList);
				Material material = this.AltMaterial;
				if (@string.Length > 0)
				{
					if (!material)
					{
						for (int j = 0; j < EModelBase.skinnedRendererList.Count; j++)
						{
							SkinnedMeshRenderer skinnedMeshRenderer = EModelBase.skinnedRendererList[j];
							if (skinnedMeshRenderer.CompareTag("LOD"))
							{
								material = skinnedMeshRenderer.sharedMaterials[0];
								break;
							}
						}
					}
					if (@string.Length > 0)
					{
						material = UnityEngine.Object.Instantiate<Material>(material);
						material.SetColor("_EmissiveColor", value);
					}
					this.AltMaterial = material;
				}
				for (int k = 0; k < EModelBase.skinnedRendererList.Count; k++)
				{
					SkinnedMeshRenderer skinnedMeshRenderer2 = EModelBase.skinnedRendererList[k];
					if (skinnedMeshRenderer2.CompareTag("LOD"))
					{
						Material[] sharedMaterials = skinnedMeshRenderer2.sharedMaterials;
						sharedMaterials[0] = material;
						skinnedMeshRenderer2.materials = sharedMaterials;
					}
				}
				EModelBase.skinnedRendererList.Clear();
			}
			if (_ec.MatSwap != null)
			{
				foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>(true))
				{
					if (renderer.CompareTag("LOD"))
					{
						Material[] sharedMaterials2 = renderer.sharedMaterials;
						int num2 = Utils.FastMin(sharedMaterials2.Length, _ec.MatSwap.Length);
						for (int m = 0; m < num2; m++)
						{
							string text2 = _ec.MatSwap[m];
							if (text2 != null && text2.Length > 0)
							{
								Material material2 = DataLoader.LoadAsset<Material>(text2, false);
								if (material2)
								{
									sharedMaterials2[m] = material2;
								}
							}
						}
						renderer.materials = sharedMaterials2;
					}
				}
			}
		}
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x0010F4E0 File Offset: 0x0010D6E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void createAvatarController(EntityClass _ec)
	{
		string text = (this.entity is EntityPlayerLocal) ? EntityClass.PropLocalAvatarController : EntityClass.PropAvatarController;
		text = _ec.Properties.GetString(text);
		if (text.Length > 0)
		{
			Type type = Type.GetType(text);
			this.avatarController = (base.gameObject.GetComponent(type) as AvatarController);
			if (!this.avatarController)
			{
				this.avatarController = (base.gameObject.AddComponent(type) as AvatarController);
			}
		}
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x0010F560 File Offset: 0x0010D760
	public virtual void Detach()
	{
		for (int i = 0; i < this.modelTransformParent.childCount; i++)
		{
			UnityEngine.Object.Destroy(this.modelTransformParent.GetChild(i).gameObject);
		}
		UnityEngine.Object.Destroy(this.avatarController);
		UnityEngine.Object.Destroy(this);
		this.avatarController = null;
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x0010F5B4 File Offset: 0x0010D7B4
	[Conditional("DEBUG_RAGDOLL")]
	public void LogRagdoll(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} Ragdoll {1}, id{2}, {3}, {4}", new object[]
		{
			GameManager.frameCount,
			this.entity.GetDebugName(),
			this.entity.entityId,
			this.ragdollState,
			_format
		});
		Log.Warning(_format, _args);
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x0010F61C File Offset: 0x0010D81C
	[Conditional("DEBUG_RAGDOLLDO")]
	public void LogRagdollDo(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} Ragdoll Do {1}, id{2}, {3}, time {4}, {5}", new object[]
		{
			GameManager.frameCount,
			this.entity.GetDebugName(),
			this.entity.entityId,
			this.ragdollState,
			this.ragdollTime,
			_format
		});
		Log.Warning(_format, _args);
	}

	// Token: 0x06002ACF RID: 10959 RVA: 0x0010F690 File Offset: 0x0010D890
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckAnimFreeze()
	{
		if (EAIManager.isAnimFreeze)
		{
			EntityAlive entityAlive = this.entity as EntityAlive;
			if (entityAlive && entityAlive.aiManager != null && this.avatarController)
			{
				Animator animator = this.avatarController.GetAnimator();
				if (animator)
				{
					animator.enabled = false;
				}
			}
		}
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x0010F6E8 File Offset: 0x0010D8E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateHeadState()
	{
		if (this.headTransform == null)
		{
			return;
		}
		switch (this.HeadState)
		{
		case EModelBase.HeadStates.Standard:
			if (this.headTransform.localScale.x != this.HeadStandardSize)
			{
				this.headTransform.localScale = new Vector3(this.HeadStandardSize, this.HeadStandardSize, this.HeadStandardSize);
				return;
			}
			break;
		case EModelBase.HeadStates.Growing:
		{
			float num = this.headTransform.localScale.y + this.headScaleSpeed * Time.deltaTime;
			if (num >= this.HeadBigSize)
			{
				num = this.HeadBigSize;
				this.HeadState = EModelBase.HeadStates.BigHead;
				EntityAlive entityAlive = this.entity as EntityAlive;
				if (entityAlive != null)
				{
					entityAlive.CurrentHeadState = this.HeadState;
				}
			}
			this.headTransform.localScale = new Vector3(num, num, num);
			return;
		}
		case EModelBase.HeadStates.BigHead:
			if (this.headTransform.localScale.x != this.HeadBigSize)
			{
				this.headTransform.localScale = new Vector3(this.HeadBigSize, this.HeadBigSize, this.HeadBigSize);
			}
			break;
		case EModelBase.HeadStates.Shrinking:
		{
			float num2 = this.headTransform.localScale.y - this.headScaleSpeed * Time.deltaTime;
			if (num2 <= this.HeadStandardSize)
			{
				num2 = this.HeadStandardSize;
				this.HeadState = EModelBase.HeadStates.Standard;
				EntityAlive entityAlive2 = this.entity as EntityAlive;
				if (entityAlive2 != null)
				{
					entityAlive2.CurrentHeadState = this.HeadState;
				}
			}
			this.headTransform.localScale = new Vector3(num2, num2, num2);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x0010F870 File Offset: 0x0010DA70
	public void ForceHeadState(EModelBase.HeadStates headState)
	{
		if (this.headTransform == null)
		{
			return;
		}
		this.HeadState = headState;
		if (headState != EModelBase.HeadStates.Standard)
		{
			if (headState == EModelBase.HeadStates.BigHead)
			{
				float headBigSize = this.HeadBigSize;
				this.headTransform.localScale = new Vector3(headBigSize, headBigSize, headBigSize);
				return;
			}
		}
		else
		{
			float headStandardSize = this.HeadStandardSize;
			this.headTransform.localScale = new Vector3(headStandardSize, headStandardSize, headStandardSize);
		}
	}

	// Token: 0x06002AD2 RID: 10962 RVA: 0x0010F8CF File Offset: 0x0010DACF
	public void SetHeadScale(float standard)
	{
		this.HeadStandardSize = standard;
		this.HeadBigSize = Mathf.Min(4.5f, standard * 3f);
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x0010F8F0 File Offset: 0x0010DAF0
	[PublicizedFrom(EAccessModifier.Protected)]
	public EModelBase()
	{
	}

	// Token: 0x04002064 RID: 8292
	public const string ExtFirstPerson = "_FP";

	// Token: 0x04002065 RID: 8293
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cEmissiveColor = "_EmissiveColor";

	// Token: 0x04002066 RID: 8294
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EModelInstanceAssets assets;

	// Token: 0x04002067 RID: 8295
	public AvatarController avatarController;

	// Token: 0x04002068 RID: 8296
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform modelTransformParent;

	// Token: 0x04002069 RID: 8297
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform modelTransform;

	// Token: 0x0400206A RID: 8298
	public Transform meshTransform;

	// Token: 0x0400206B RID: 8299
	public Transform bipedRootTransform;

	// Token: 0x0400206C RID: 8300
	public Transform bipedPelvisTransform;

	// Token: 0x0400206D RID: 8301
	public Rigidbody pelvisRB;

	// Token: 0x0400206E RID: 8302
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform headTransform;

	// Token: 0x0400206F RID: 8303
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform neckTransform;

	// Token: 0x04002070 RID: 8304
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform neckParentTransform;

	// Token: 0x04002071 RID: 8305
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CharacterGazeController gazeController;

	// Token: 0x04002072 RID: 8306
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public PhysicsBodyInstance physicsBody;

	// Token: 0x04002073 RID: 8307
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string modelName;

	// Token: 0x04002074 RID: 8308
	public Material AltMaterial;

	// Token: 0x04002075 RID: 8309
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float ragdollChance;

	// Token: 0x04002076 RID: 8310
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bHasRagdoll;

	// Token: 0x04002077 RID: 8311
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Cloth[] clothSim;

	// Token: 0x04002078 RID: 8312
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isClothSimOn = true;

	// Token: 0x04002079 RID: 8313
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Jiggle[] jiggles;

	// Token: 0x0400207A RID: 8314
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isJiggleOn = true;

	// Token: 0x0400207C RID: 8316
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Entity entity;

	// Token: 0x0400207D RID: 8317
	public EModelBase.HeadStates HeadState;

	// Token: 0x0400207E RID: 8318
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float headScaleSpeed = 2f;

	// Token: 0x0400207F RID: 8319
	public float HeadStandardSize = 1f;

	// Token: 0x04002080 RID: 8320
	public float HeadBigSize = 3f;

	// Token: 0x04002081 RID: 8321
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform NavObjectTransform;

	// Token: 0x04002082 RID: 8322
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float ragdollAlignmentForce = 25f;

	// Token: 0x04002083 RID: 8323
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float ragdollAlingmentDistance = 0.2f;

	// Token: 0x04002084 RID: 8324
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float ragdollBlendPositionSpeed = 10f;

	// Token: 0x04002085 RID: 8325
	public static float serverPosSpringForce = 50f;

	// Token: 0x04002086 RID: 8326
	public static float serverPosSpringDamping = 0.1f;

	// Token: 0x04002087 RID: 8327
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Renderer> rendererList = new List<Renderer>();

	// Token: 0x04002088 RID: 8328
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<SkinnedMeshRenderer> skinnedRendererList = new List<SkinnedMeshRenderer>();

	// Token: 0x04002089 RID: 8329
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLookAtSlerpPer = 0.16f;

	// Token: 0x0400208A RID: 8330
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLookAtAnimBlend = 0.5f;

	// Token: 0x0400208B RID: 8331
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool lookAtEnabled;

	// Token: 0x0400208C RID: 8332
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtMaxAngle;

	// Token: 0x0400208D RID: 8333
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lookAtIsPos;

	// Token: 0x0400208E RID: 8334
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lookAtPos;

	// Token: 0x0400208F RID: 8335
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion lookAtRot;

	// Token: 0x04002090 RID: 8336
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtBlendPer;

	// Token: 0x04002091 RID: 8337
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtBlendPerTarget;

	// Token: 0x04002092 RID: 8338
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtFullChangeTime;

	// Token: 0x04002093 RID: 8339
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lookAtFullBlendPer = 1f;

	// Token: 0x04002094 RID: 8340
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRadgollBlendOutGroundTime = 0.25f;

	// Token: 0x04002095 RID: 8341
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRadgollBlendOutTime = 0.7f;

	// Token: 0x04002096 RID: 8342
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRagdollMinDisableVel = 0.5f;

	// Token: 0x04002097 RID: 8343
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRagdollDeadMaxDepentrationVel = 2f;

	// Token: 0x04002098 RID: 8344
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cRagdollDeadMaxAngularVel = 1f;

	// Token: 0x04002099 RID: 8345
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cRadgollPlayerAnimLayer = 5;

	// Token: 0x0400209A RID: 8346
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EModelBase.ERagdollState ragdollState;

	// Token: 0x0400209B RID: 8347
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollTime;

	// Token: 0x0400209C RID: 8348
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollDuration;

	// Token: 0x0400209D RID: 8349
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator ragdollAnimator;

	// Token: 0x0400209E RID: 8350
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollRotY;

	// Token: 0x0400209F RID: 8351
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ragdollIsBlending;

	// Token: 0x040020A0 RID: 8352
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollAdjustPosDelay;

	// Token: 0x040020A1 RID: 8353
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ragdollIsPlayer;

	// Token: 0x040020A2 RID: 8354
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ragdollIsAnimal;

	// Token: 0x040020A3 RID: 8355
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ragdollIsFacingUp;

	// Token: 0x040020A4 RID: 8356
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<EModelBase.RagdollPose> ragdollPoses = new List<EModelBase.RagdollPose>();

	// Token: 0x040020A5 RID: 8357
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 ragdollPosePelvisPos;

	// Token: 0x040020A6 RID: 8358
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 ragdollPosePelvisLocalPos;

	// Token: 0x040020A7 RID: 8359
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<Rigidbody> ragdollTempRBs = new List<Rigidbody>();

	// Token: 0x040020A8 RID: 8360
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ragdollZeroTime;

	// Token: 0x040020A9 RID: 8361
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Transform> ragdollZeroBones;

	// Token: 0x040020AA RID: 8362
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string[] commonBips = new string[]
	{
		"Bip001",
		"Bip01"
	};

	// Token: 0x040020AC RID: 8364
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MaterialPropertyBlock matPropBlock;

	// Token: 0x040020AD RID: 8365
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int fadeId = Shader.PropertyToID("_Fade");

	// Token: 0x02000510 RID: 1296
	public enum HeadStates
	{
		// Token: 0x040020AF RID: 8367
		Standard,
		// Token: 0x040020B0 RID: 8368
		Growing,
		// Token: 0x040020B1 RID: 8369
		BigHead,
		// Token: 0x040020B2 RID: 8370
		Shrinking
	}

	// Token: 0x02000511 RID: 1297
	public enum RagdollMode
	{
		// Token: 0x040020B4 RID: 8372
		Default,
		// Token: 0x040020B5 RID: 8373
		FullForce
	}

	// Token: 0x02000512 RID: 1298
	[PublicizedFrom(EAccessModifier.Private)]
	public enum ERagdollState
	{
		// Token: 0x040020B7 RID: 8375
		Off,
		// Token: 0x040020B8 RID: 8376
		On,
		// Token: 0x040020B9 RID: 8377
		BlendOutGround,
		// Token: 0x040020BA RID: 8378
		BlendOutStand,
		// Token: 0x040020BB RID: 8379
		Stand,
		// Token: 0x040020BC RID: 8380
		StandCollide,
		// Token: 0x040020BD RID: 8381
		SpawnWait,
		// Token: 0x040020BE RID: 8382
		Dead
	}

	// Token: 0x02000513 RID: 1299
	public struct RagdollPose
	{
		// Token: 0x040020BF RID: 8383
		public Transform t;

		// Token: 0x040020C0 RID: 8384
		public Rigidbody rb;

		// Token: 0x040020C1 RID: 8385
		public Quaternion startRot;

		// Token: 0x040020C2 RID: 8386
		public Quaternion rot;
	}
}
