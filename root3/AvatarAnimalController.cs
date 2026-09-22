using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000BC RID: 188
[Preserve]
public class AvatarAnimalController : AvatarController
{
	// Token: 0x060003AC RID: 940 RVA: 0x0001B0E8 File Offset: 0x000192E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.modelT = EModelBase.FindModel(base.transform);
		if (EntityClass.list[this.entity.entityClass].PainResistPerHit >= 0f)
		{
			this.hitLayerIndex = 1;
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0001B134 File Offset: 0x00019334
	[PublicizedFrom(EAccessModifier.Private)]
	public void assignBodyParts()
	{
		Transform transform = this.bipedT.FindInChilds("Hips", false);
		if (!transform)
		{
			return;
		}
		this.head = this.bipedT.FindInChilds("Head", false);
		this.leftUpperLeg = this.bipedT.FindInChilds("LeftUpLeg", false);
		this.rightUpperLeg = this.bipedT.FindInChilds("RightUpLeg", false);
		this.leftUpperArm = this.bipedT.FindInChilds("LeftArm", false);
		this.rightUpperArm = this.bipedT.FindInChilds("RightArm", false);
		this.limbScale = (this.head.position - transform.position).magnitude * 1.32f / this.head.lossyScale.x;
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0001B20C File Offset: 0x0001940C
	[PublicizedFrom(EAccessModifier.Protected)]
	public Transform SpawnLimbGore(Transform limbT, bool isLeft, string path, bool restoreState)
	{
		if (limbT == null)
		{
			return null;
		}
		CharacterJoint[] componentsInChildren = limbT.GetComponentsInChildren<CharacterJoint>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i]);
		}
		Rigidbody[] componentsInChildren2 = limbT.GetComponentsInChildren<Rigidbody>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			UnityEngine.Object.Destroy(componentsInChildren2[j]);
		}
		Collider[] componentsInChildren3 = limbT.GetComponentsInChildren<Collider>();
		for (int k = 0; k < componentsInChildren3.Length; k++)
		{
			UnityEngine.Object.Destroy(componentsInChildren3[k]);
		}
		Transform parent = limbT.parent;
		Transform transform = null;
		if (parent != null)
		{
			transform = this.SpawnLimbGore(parent, path, false);
			transform.localPosition = new Vector3(limbT.localPosition.x * 0.5f, limbT.localPosition.y, limbT.localPosition.z);
			transform.localRotation = limbT.localRotation;
			float num = this.limbScale;
			if (limbT != this.head)
			{
				num *= 0.7f;
				Vector3 vector = new Vector3(0f, 0f, 90f);
				if (isLeft)
				{
					vector *= -1f;
				}
				vector += new Vector3(175f, 270f, 45f);
				transform.localEulerAngles += vector;
			}
			else
			{
				transform.localPosition = limbT.localPosition * 0.63f;
				Transform transform2 = new GameObject("scaleTarget").transform;
				transform2.position = limbT.position;
				for (int l = 0; l < limbT.childCount; l++)
				{
					limbT.GetChild(l).SetParent(transform2);
				}
				transform2.SetParent(limbT.parent);
				limbT.SetParent(transform2);
				transform2.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			}
			transform.localScale = limbT.localScale * num;
		}
		limbT.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		return transform;
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0001B424 File Offset: 0x00019624
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform SpawnLimbGore(Transform parent, string path, bool restoreState)
	{
		if (parent)
		{
			GameObject gameObject = (GameObject)Resources.Load(path);
			if (!gameObject)
			{
				Log.Out(this.entity.EntityName + " SpawnLimbGore prefab not found in resource. path: {0}", new object[]
				{
					path
				});
				string assetBundlePath = DismembermentManager.GetAssetBundlePath(path);
				GameObject gameObject2 = DataLoader.LoadAsset<GameObject>(assetBundlePath, false);
				if (!gameObject2)
				{
					Log.Warning(this.entity.EntityName + " SpawnLimbGore prefab not found in asset bundle. path: {0}", new object[]
					{
						assetBundlePath
					});
					return null;
				}
				gameObject = gameObject2;
			}
			GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(gameObject, parent);
			GorePrefab component = gameObject3.GetComponent<GorePrefab>();
			if (component)
			{
				component.restoreState = restoreState;
			}
			return gameObject3.transform;
		}
		return null;
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x0001B4D8 File Offset: 0x000196D8
	public override void RemoveLimb(BodyDamage _bodyDamage, bool restoreState)
	{
		EnumBodyPartHit bodyPartHit = _bodyDamage.bodyPartHit;
		EnumDamageTypes damageType = _bodyDamage.damageType;
		if (!this.headDismembered && (bodyPartHit & EnumBodyPartHit.Head) > EnumBodyPartHit.None)
		{
			this.MakeDismemberedPart(1U, damageType, this.head, false, restoreState);
			this.headDismembered = true;
		}
		if ((!this.leftUpperLegDismembered && (bodyPartHit & EnumBodyPartHit.LeftUpperLeg) > EnumBodyPartHit.None) || (bodyPartHit & EnumBodyPartHit.LeftLowerLeg) > EnumBodyPartHit.None)
		{
			this.MakeDismemberedPart(32U, damageType, this.leftUpperLeg, true, restoreState);
			this.leftUpperLegDismembered = true;
		}
		if ((!this.rightUpperLegDismembered && (bodyPartHit & EnumBodyPartHit.RightUpperLeg) > EnumBodyPartHit.None) || (bodyPartHit & EnumBodyPartHit.RightLowerLeg) > EnumBodyPartHit.None)
		{
			this.MakeDismemberedPart(128U, damageType, this.rightUpperLeg, false, restoreState);
			this.rightUpperLegDismembered = true;
		}
		if ((!this.leftUpperArmDismembered && (bodyPartHit & EnumBodyPartHit.LeftUpperArm) > EnumBodyPartHit.None) || (bodyPartHit & EnumBodyPartHit.LeftLowerArm) > EnumBodyPartHit.None)
		{
			this.MakeDismemberedPart(2U, damageType, this.leftUpperArm, true, restoreState);
			this.leftUpperArmDismembered = true;
		}
		if ((!this.rightUpperArmDismembered && (bodyPartHit & EnumBodyPartHit.RightUpperArm) > EnumBodyPartHit.None) || (bodyPartHit & EnumBodyPartHit.RightLowerArm) > EnumBodyPartHit.None)
		{
			this.MakeDismemberedPart(8U, damageType, this.rightUpperArm, false, restoreState);
			this.rightUpperArmDismembered = true;
		}
		if (this.entity.IsAlive())
		{
			int num = 0;
			if (this.leftUpperLegDismembered)
			{
				num++;
			}
			if (this.rightUpperLegDismembered)
			{
				num++;
			}
			if (this.leftUpperArmDismembered)
			{
				num++;
			}
			if (this.rightUpperArmDismembered)
			{
				num++;
			}
			if (this.missingMotorLimbs != num)
			{
				this.missingMotorLimbs = num;
			}
			float num2 = Mathf.Max(this.entity.moveSpeedAggroMax, this.entity.moveSpeedPanicMax) * (1f - (float)num / 5f);
			if (this.missingMotorLimbs >= 3)
			{
				num2 = 0f;
				this.entity.lastDamageResponse.Fatal = true;
				this.entity.Kill(this.entity.lastDamageResponse);
			}
			this.entity.moveSpeed = num2;
			this.entity.moveSpeedAggro = num2;
			this.entity.moveSpeedPanic = num2;
		}
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x0001B6B0 File Offset: 0x000198B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void MakeDismemberedPart(uint bodyDamageFlag, EnumDamageTypes damageType, Transform partT, bool isLeft, bool restoreState)
	{
		DismemberedPartData dismemberedPartData = DismembermentManager.DismemberPart(bodyDamageFlag, damageType, this.entity, true, false);
		if (dismemberedPartData == null)
		{
			return;
		}
		if (partT)
		{
			this.ProcDismemberedPart(partT, partT, dismemberedPartData);
		}
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0001B6E4 File Offset: 0x000198E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcDismemberedPart(Transform t, Transform partT, DismemberedPartData part)
	{
		if (part.particlePaths != null)
		{
			for (int i = 0; i < part.particlePaths.Length; i++)
			{
				string text = part.particlePaths[i];
				if (!string.IsNullOrEmpty(text))
				{
					DismembermentManager.SpawnParticleEffect(new ParticleEffect(text, t.position + Origin.position, Quaternion.identity, 1f, Color.white), -1);
				}
			}
			return;
		}
		DismembermentManager.SpawnParticleEffect(new ParticleEffect("blood_impact", t.position + Origin.position, Quaternion.identity, 1f, Color.white), -1);
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0001B778 File Offset: 0x00019978
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		if (!this.bipedT)
		{
			this.bipedT = this.entity.emodel.GetModelTransform();
			this.rightHandT = this.FindTransform(this.entity.GetRightHandTransformName());
			this.assignBodyParts();
			this.assignStates();
			base.SetAnimator(this.bipedT);
			if (this.entity.RootMotion)
			{
				AvatarRootMotion avatarRootMotion = this.bipedT.GetComponent<AvatarRootMotion>();
				if (avatarRootMotion == null)
				{
					avatarRootMotion = this.bipedT.gameObject.AddComponent<AvatarRootMotion>();
				}
				avatarRootMotion.Init(this, this.anim);
			}
		}
		this.SetWalkType(this.entity.GetWalkType(), false);
		this._setBool(AvatarController.isDeadHash, this.entity.IsDead(), true);
		this._setBool(AvatarController.isAliveHash, this.entity.IsAlive(), true);
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x0001B85A File Offset: 0x00019A5A
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform FindTransform(string _name)
	{
		return this.bipedT.FindInChildren(_name);
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x0001B868 File Offset: 0x00019A68
	public override Transform GetRightHandTransform()
	{
		return this.rightHandT;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x0001B870 File Offset: 0x00019A70
	public override AvatarController.ActionState GetActionState()
	{
		if (this.attackPlayingTime > 0f)
		{
			return AvatarController.ActionState.Active;
		}
		int tagHash = this.baseStateInfo.tagHash;
		if (tagHash == AvatarController.attackStartHash || this.actionTimeActive > 0f)
		{
			return AvatarController.ActionState.Start;
		}
		if (tagHash == AvatarController.attackReadyHash)
		{
			return AvatarController.ActionState.Ready;
		}
		if (tagHash == AvatarController.attackHash)
		{
			return AvatarController.ActionState.Active;
		}
		return AvatarController.ActionState.None;
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x0001B8C4 File Offset: 0x00019AC4
	public override bool IsActionActive()
	{
		return this.GetActionState() > AvatarController.ActionState.None;
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x0001B8CF File Offset: 0x00019ACF
	public override void StartAction(int _animType)
	{
		if (_animType < 3000)
		{
			this.StartAnimationAttack();
			return;
		}
		this.idleTime = 0f;
		this._setInt(AvatarController.attackHash, _animType, true);
		this._setTrigger(AvatarController.attackTriggerHash, true);
		this.actionTimeActive = 0.2f;
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x0001B90F File Offset: 0x00019B0F
	public override bool IsAnimationAttackPlaying()
	{
		return this.attackPlayingTime > 0f || (!this.anim.IsInTransition(0) && this.baseStateInfo.tagHash == AvatarController.attackHash);
	}

	// Token: 0x060003BA RID: 954 RVA: 0x0001B942 File Offset: 0x00019B42
	public override void StartAnimationAttack()
	{
		if (!this.entity.isEntityRemote)
		{
			this.attackPlayingTime = 0.5f;
		}
		this.idleTime = 0f;
		this._setTrigger(AvatarController.attackTriggerHash, true);
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0001B973 File Offset: 0x00019B73
	public override bool IsAnimationSpecialAttackPlaying()
	{
		return this.IsAnimationAttackPlaying();
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0001B973 File Offset: 0x00019B73
	public override bool IsAnimationSpecialAttack2Playing()
	{
		return this.IsAnimationAttackPlaying();
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0001B973 File Offset: 0x00019B73
	public override bool IsAnimationRagingPlaying()
	{
		return this.IsAnimationAttackPlaying();
	}

	// Token: 0x060003BE RID: 958 RVA: 0x0001B97B File Offset: 0x00019B7B
	public override void SetAlive()
	{
		this._setBool(AvatarController.isAliveHash, true, true);
		this._setBool(AvatarController.isDeadHash, false, true);
		this._setTrigger(AvatarController.triggerAliveHash, true);
	}

	// Token: 0x060003BF RID: 959 RVA: 0x0001B9A4 File Offset: 0x00019BA4
	public override void SetVisible(bool _b)
	{
		if (this.m_bVisible != _b || !this.visInit)
		{
			this.m_bVisible = _b;
			this.visInit = true;
			Transform transform = this.bipedT;
			if (transform != null)
			{
				Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = _b;
				}
			}
		}
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x0001BA00 File Offset: 0x00019C00
	public override void StartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float _random, float _duration)
	{
		if (!base.CheckHit(_duration))
		{
			this.SetDataFloat(AvatarController.DataTypes.HitDuration, _duration, true);
			return;
		}
		this.idleTime = 0f;
		this._setInt(AvatarController.bodyPartHitHash, (int)_bodyPart, true);
		this._setInt(AvatarController.hitDirectionHash, _dir, true);
		this._setInt(AvatarController.hitDamageHash, _hitDamage, true);
		this._setBool(AvatarController.criticalHitHash, _criticalHit, true);
		this._setInt(AvatarController.movementStateHash, _movementState, true);
		this._setInt(AvatarController.randomHash, Mathf.FloorToInt(_random * 100f), true);
		this.SetDataFloat(AvatarController.DataTypes.HitDuration, _duration, true);
		this._setTrigger(AvatarController.painTriggerHash, true);
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x0001BAA0 File Offset: 0x00019CA0
	public override bool IsAnimationHitRunning()
	{
		if (this.hitLayerIndex < 0)
		{
			return false;
		}
		AnimatorStateInfo currentAnimatorStateInfo = this.anim.GetCurrentAnimatorStateInfo(this.hitLayerIndex);
		return (currentAnimatorStateInfo.tagHash == AvatarController.hitHash && currentAnimatorStateInfo.normalizedTime < 0.55f) || this.anim.IsInTransition(this.hitLayerIndex);
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x0001BAFC File Offset: 0x00019CFC
	public override void StartAnimationJump(AnimJumpMode jumpMode)
	{
		this.idleTime = 0f;
		if (this.bipedT == null || !this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim != null)
		{
			if (jumpMode == AnimJumpMode.Start)
			{
				this._setTrigger(AvatarController.jumpStartHash, true);
				return;
			}
			this._setTrigger(AvatarController.jumpLandHash, true);
		}
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x0001BB5F File Offset: 0x00019D5F
	public override void StartEating()
	{
		if (!this.isEating)
		{
			this._setTrigger(AvatarController.beginCorpseEatHash, true);
			this.isEating = true;
		}
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0001BB7C File Offset: 0x00019D7C
	public override void StopEating()
	{
		if (this.isEating)
		{
			this._setTrigger(AvatarController.endCorpseEatHash, true);
			this.isEating = false;
		}
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0001BB99 File Offset: 0x00019D99
	public override void SetSwim(bool _enable)
	{
		this._setBool(AvatarController.isSwimHash, _enable, true);
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x0001BBA8 File Offset: 0x00019DA8
	public override void StartDeathAnimation(EnumBodyPartHit _bodyPart, int _movementState, float _random)
	{
		this.isInDeathAnim = true;
		this._setBool(AvatarController.isAliveHash, false, true);
		this._setBool(AvatarController.isDeadHash, true, true);
		this._setInt(AvatarController.randomHash, Mathf.FloorToInt(_random * 100f), true);
		this.idleTime = 0f;
		this._resetTrigger(AvatarController.attackTriggerHash, true);
		this._resetTrigger(AvatarController.painTriggerHash, true);
		this._setTrigger(AvatarController.deathTriggerHash, true);
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x0001BC1D File Offset: 0x00019E1D
	public override Transform GetActiveModelRoot()
	{
		if (!this.modelT)
		{
			return this.bipedT;
		}
		return this.modelT;
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x0001BC3C File Offset: 0x00019E3C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		float deltaTime = Time.deltaTime;
		if (this.actionTimeActive > 0f)
		{
			this.actionTimeActive -= deltaTime;
		}
		if (this.electrocuteTime > 0.3f && !this.entity.emodel.IsRagdollActive)
		{
			this._setTrigger(AvatarController.electrocuteTriggerHash, true);
		}
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x0001BC9C File Offset: 0x00019E9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void LateUpdate()
	{
		if (!this.m_bVisible && (!this.entity || this.entity.isEntityRemote))
		{
			return;
		}
		if (!this.bipedT || !this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!this.anim || !this.anim.enabled)
		{
			return;
		}
		this.attackPlayingTime -= Time.deltaTime;
		this.UpdateLayerStateInfo();
		float num = 0f;
		float num2 = 0f;
		if (!this.entity.IsFlyMode.Value)
		{
			num = this.entity.speedForward;
			num2 = this.entity.speedStrafe;
		}
		float num3 = num2;
		if (num3 >= 1234f)
		{
			num3 = 0f;
		}
		this._setFloat(AvatarController.forwardHash, num, false);
		this._setFloat(AvatarController.strafeHash, num3, false);
		if (!this.entity.IsDead())
		{
			float num4 = num * num + num3 * num3;
			this._setInt(AvatarController.movementStateHash, (num4 > this.entity.moveSpeedAggro * this.entity.moveSpeedAggro) ? 3 : ((num4 > this.entity.moveSpeed * this.entity.moveSpeed) ? 2 : ((num4 > 0.001f) ? 1 : 0)), false);
		}
		if (base.IsMoving(num, num2))
		{
			this.idleTime = 0f;
			this._setBool(AvatarController.isMovingHash, true, false);
		}
		else
		{
			this._setBool(AvatarController.isMovingHash, false, false);
		}
		if (this.isInDeathAnim && this.baseStateInfo.tagHash == AvatarController.deathHash && this.baseStateInfo.normalizedTime >= 1f && !this.anim.IsInTransition(0))
		{
			this.isInDeathAnim = false;
			if (this.entity.HasDeathAnim)
			{
				EModelBase emodel = this.entity.emodel;
				DamageResponse damageResponse = DamageResponse.New(true);
				emodel.DoRagdoll(damageResponse, EModelBase.RagdollMode.Default, 999999f);
			}
		}
		this._setFloat(AvatarController.idleTimeHash, this.idleTime, false);
		this.idleTime += Time.deltaTime;
	}

	// Token: 0x060003CA RID: 970 RVA: 0x0001BEA9 File Offset: 0x0001A0A9
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLayerStateInfo()
	{
		this.baseStateInfo = this.anim.GetCurrentAnimatorStateInfo(0);
	}

	// Token: 0x0400044B RID: 1099
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool visInit;

	// Token: 0x0400044C RID: 1100
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_bVisible;

	// Token: 0x0400044D RID: 1101
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform modelT;

	// Token: 0x0400044E RID: 1102
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform bipedT;

	// Token: 0x0400044F RID: 1103
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform rightHandT;

	// Token: 0x04000450 RID: 1104
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform head;

	// Token: 0x04000451 RID: 1105
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform leftUpperArm;

	// Token: 0x04000452 RID: 1106
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform rightUpperArm;

	// Token: 0x04000453 RID: 1107
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform leftUpperLeg;

	// Token: 0x04000454 RID: 1108
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform rightUpperLeg;

	// Token: 0x04000455 RID: 1109
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float limbScale;

	// Token: 0x04000456 RID: 1110
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo baseStateInfo;

	// Token: 0x04000457 RID: 1111
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float idleTime;

	// Token: 0x04000458 RID: 1112
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float actionTimeActive;

	// Token: 0x04000459 RID: 1113
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float attackPlayingTime;

	// Token: 0x0400045A RID: 1114
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool headDismembered;

	// Token: 0x0400045B RID: 1115
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool leftUpperArmDismembered;

	// Token: 0x0400045C RID: 1116
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool rightUpperArmDismembered;

	// Token: 0x0400045D RID: 1117
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool leftUpperLegDismembered;

	// Token: 0x0400045E RID: 1118
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool rightUpperLegDismembered;

	// Token: 0x0400045F RID: 1119
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isInDeathAnim;

	// Token: 0x04000460 RID: 1120
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int missingMotorLimbs;

	// Token: 0x04000461 RID: 1121
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isEating;
}
