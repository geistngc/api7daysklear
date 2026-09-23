using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Audio;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003F5 RID: 1013
[Preserve]
public class Entity : MonoBehaviour, ILockTarget
{
	// Token: 0x170003AA RID: 938
	// (get) Token: 0x06001E97 RID: 7831 RVA: 0x000B9A54 File Offset: 0x000B7C54
	public FastTags<TagGroup.Global> EntityTags
	{
		get
		{
			return this.cachedTags;
		}
	}

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x06001E98 RID: 7832 RVA: 0x000B9A5C File Offset: 0x000B7C5C
	public EntityClass EntityClass
	{
		get
		{
			EntityClass result;
			EntityClass.list.TryGetValue(this.entityClass, out result);
			return result;
		}
	}

	// Token: 0x170003AC RID: 940
	// (get) Token: 0x06001E99 RID: 7833 RVA: 0x000B9A7D File Offset: 0x000B7C7D
	public virtual string LocalizedEntityName
	{
		get
		{
			return Localization.Get(EntityClass.list[this.entityClass].entityClassName, false, null);
		}
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x06001E9A RID: 7834 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual Entity.EnumPositionUpdateMovementType positionUpdateMovementType
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return Entity.EnumPositionUpdateMovementType.Lerp;
		}
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x000B9A9C File Offset: 0x000B7C9C
	public static bool CheckDistance(int entityID_A, int entityID_B)
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		Entity entity = GameManager.Instance.World.GetEntity(entityID_A);
		if (entity == null)
		{
			return false;
		}
		Entity entity2 = GameManager.Instance.World.GetEntity(entityID_B);
		return !(entity2 == null) && Entity.CheckDistance(entity, entity2);
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x000B9B05 File Offset: 0x000B7D05
	public static bool CheckDistance(Entity entityB, int entityID_A)
	{
		return Entity.CheckDistance(entityID_A, entityB);
	}

	// Token: 0x06001E9D RID: 7837 RVA: 0x000B9B10 File Offset: 0x000B7D10
	public static bool CheckDistance(int entityID_A, Entity entityB)
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		if (entityB == null)
		{
			return false;
		}
		Entity entity = GameManager.Instance.World.GetEntity(entityID_A);
		return !(entity == null) && Entity.CheckDistance(entity, entityB);
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x000B9B68 File Offset: 0x000B7D68
	public static bool CheckDistance(Entity A, Vector3 B)
	{
		return !(A == null) && Entity.CheckDistance(A.transform.position, B);
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x000B9B86 File Offset: 0x000B7D86
	public static bool CheckDistance(Vector3 A, Entity B)
	{
		return !(B == null) && Entity.CheckDistance(A, B.transform.position);
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x000B9BA4 File Offset: 0x000B7DA4
	public static bool CheckDistance(Vector3 A, int entityID_B)
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		Entity entity = GameManager.Instance.World.GetEntity(entityID_B);
		return !(entity == null) && Entity.CheckDistance(A - Origin.position, entity.transform.position);
	}

	// Token: 0x06001EA1 RID: 7841 RVA: 0x000B9C08 File Offset: 0x000B7E08
	public static bool CheckDistance(Vector3 A, Vector3 B)
	{
		return (A - B).magnitude < 256f;
	}

	// Token: 0x06001EA2 RID: 7842 RVA: 0x000B9C2B File Offset: 0x000B7E2B
	public static bool CheckDistance(Entity listenerEntity, Entity sourceEntity)
	{
		return Entity.CheckDistance(sourceEntity.transform.position, listenerEntity.transform.position);
	}

	// Token: 0x06001EA3 RID: 7843 RVA: 0x000B9C48 File Offset: 0x000B7E48
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		Entity.InstanceCount++;
		this.world = GameManager.Instance.World;
		this.isEntityRemote = !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		this.WorldTimeBorn = this.world.worldTime;
		this.rand = this.world.GetGameRandom();
		this.SetupBounds();
	}

	// Token: 0x06001EA4 RID: 7844 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x000B9CAC File Offset: 0x000B7EAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~Entity()
	{
		Entity.InstanceCount--;
	}

	// Token: 0x06001EA6 RID: 7846 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnXMLChanged()
	{
	}

	// Token: 0x06001EA7 RID: 7847 RVA: 0x000B9CE0 File Offset: 0x000B7EE0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetupBounds()
	{
		BoxCollider boxCollider;
		if (base.TryGetComponent<BoxCollider>(out boxCollider))
		{
			this.nativeCollider = boxCollider;
			Vector3 localScale = base.transform.localScale;
			this.scaledExtent = Vector3.Scale(boxCollider.size, localScale) * 0.5f;
			Vector3 b = Vector3.Scale(boxCollider.center, localScale);
			this.boundingBox = BoundsUtils.BoundsForMinMax(-this.scaledExtent, this.scaledExtent);
			this.boundingBox.center = this.boundingBox.center + b;
			if (this.isDetailedHeadBodyColliders())
			{
				boxCollider.enabled = false;
				return;
			}
		}
		else
		{
			CharacterController characterController;
			if (base.TryGetComponent<CharacterController>(out characterController))
			{
				Vector3 localScale2 = base.transform.localScale;
				float radius = characterController.radius;
				this.scaledExtent = new Vector3(radius * localScale2.x, characterController.height * localScale2.y * 0.5f, radius * localScale2.z);
				this.boundingBox = BoundsUtils.BoundsForMinMax(-this.scaledExtent, this.scaledExtent);
				return;
			}
			this.boundingBox = BoundsUtils.BoundsForMinMax(Vector3.zero, Vector3.one);
		}
	}

	// Token: 0x06001EA8 RID: 7848 RVA: 0x000B9E00 File Offset: 0x000B8000
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		this.bWasDead = this.IsDead();
		this.animateYaw();
		if (this.physicsMasterTargetTime > 0f)
		{
			this.PhysicsMasterTargetFrameUpdate();
		}
		else
		{
			this.updateTransform();
		}
		if (this.bIsChunkObserver && !this.isEntityRemote)
		{
			if (this.movableChunkObserver == null)
			{
				this.movableChunkObserver = new MovableSharedChunkObserver(this.world.m_SharedChunkObserverCache);
			}
			this.movableChunkObserver.SetPosition(this.position);
		}
		else if (!this.bIsChunkObserver && this.movableChunkObserver != null)
		{
			this.movableChunkObserver.Dispose();
			this.movableChunkObserver = null;
		}
		if (this.animatorAudioMonitoringDictionary.Count > 0)
		{
			List<Entity.StopAnimatorAudioType> list = new List<Entity.StopAnimatorAudioType>();
			foreach (KeyValuePair<Entity.StopAnimatorAudioType, Handle> keyValuePair in this.animatorAudioMonitoringDictionary)
			{
				if (!keyValuePair.Value.IsPlaying())
				{
					keyValuePair.Value.Stop(this.entityId);
					list.Add(keyValuePair.Key);
				}
			}
			foreach (Entity.StopAnimatorAudioType key in list)
			{
				this.animatorAudioMonitoringDictionary.Remove(key);
			}
		}
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x000B9F68 File Offset: 0x000B8168
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateTransform()
	{
		if (this.AttachedToEntity != null)
		{
			return;
		}
		this.ApplyFixedUpdate();
		if (!this.emodel || !this.emodel.IsRagdollOn)
		{
			float y;
			if (this.physicsRB)
			{
				Vector3 b = this.physicsRBT.position - this.physicsBasePos;
				Vector3 vector = Vector3.Lerp(base.transform.position, b, this.physicsPosMoveDistance * Time.deltaTime / Time.fixedDeltaTime);
				base.transform.position = vector;
				y = this.physicsRBT.eulerAngles.y;
			}
			else
			{
				Vector3 b2 = this.position - Origin.position;
				base.transform.position = Vector3.Lerp(base.transform.position, b2, Time.deltaTime * Entity.updatePositionLerpTimeScale);
				y = this.rotation.y;
			}
			if (this.isRotateToGround)
			{
				Vector3 vector2 = this.groundSurface.normal;
				float num = Vector3.Dot(vector2, Vector3.up);
				if (this.IsRotateToGroundFlat)
				{
					num = 1f;
				}
				if (num > 0.99f || num < 0.7f)
				{
					vector2 = Vector3.up;
				}
				Vector3 vector3 = Quaternion.AngleAxis(-y, Vector3.up) * vector2;
				float target = 90f - Mathf.Atan2(vector3.y, vector3.z) * 57.29578f;
				this.rotateToGroundPitchVel *= 0.86f;
				this.rotateToGroundPitchVel += Mathf.DeltaAngle(this.rotateToGroundPitch, target) * 0.8f * Time.deltaTime;
				this.rotateToGroundPitch += this.rotateToGroundPitchVel;
				base.transform.eulerAngles = new Vector3(this.rotateToGroundPitch, y, 0f);
			}
			else
			{
				base.transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(base.transform.eulerAngles.y, y, Time.deltaTime * Entity.updateRotationLerpTimeScale), 0f);
			}
		}
		if (this.isEntityRemote && this.PhysicsTransform != null)
		{
			this.PhysicsTransform.position = Vector3.Lerp(this.PhysicsTransform.position, this.position - Origin.position, Time.deltaTime * Entity.updateRotationLerpTimeScale);
		}
	}

	// Token: 0x06001EAA RID: 7850 RVA: 0x000BA1C8 File Offset: 0x000B83C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdate()
	{
		this.ApplyFixedUpdate();
		this.wasFixedUpdate = true;
		if (this.physicsRB)
		{
			this.physicsRB.velocity *= 0.9f;
			this.physicsRB.angularVelocity *= 0.9f;
			Transform transform = this.physicsRBT;
			Vector3 b = this.physicsTargetPos + this.physicsBasePos;
			Vector3 vector = Vector3.Lerp(transform.position, b, 0.4f);
			this.physicsPos = vector;
			this.physicsRot = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, this.rotation.y, 0f), 0.3f);
			transform.SetPositionAndRotation(vector, this.physicsRot);
			if (this.physicsCapsuleCollider)
			{
				EntityAlive entityAlive = this as EntityAlive;
				if (entityAlive)
				{
					entityAlive.CrouchHeightFixedUpdate();
				}
			}
		}
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x000BA2BC File Offset: 0x000B84BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyFixedUpdate()
	{
		if (!this.wasFixedUpdate)
		{
			return;
		}
		this.wasFixedUpdate = false;
		if (this.physicsRB)
		{
			Transform transform = this.physicsRBT;
			Vector3 a = transform.position;
			if ((a - this.physicsPos).sqrMagnitude > 0.0001f)
			{
				Vector3 vector = this.position;
				Vector3 a2 = a - this.physicsBasePos;
				this.physicsPos = a;
				this.SetPosition(a2 + Origin.position, false);
				this.PhysicsTransform.position = a2;
			}
			this.physicsPosMoveDistance = Vector3.Distance(this.physicsPos, base.transform.position);
			if (Mathf.Abs(Quaternion.Angle(transform.rotation, this.physicsRot)) > 0.1f)
			{
				Quaternion quaternion = transform.rotation;
				this.physicsRot = quaternion;
				this.rotation = quaternion.eulerAngles;
				this.qrotation = quaternion;
			}
		}
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x000BA3A9 File Offset: 0x000B85A9
	public virtual void OriginChanged(Vector3 _deltaPos)
	{
		this.physicsPos += _deltaPos;
		this.physicsTargetPos += _deltaPos;
		if (this.emodel)
		{
			this.emodel.OriginChanged(_deltaPos);
		}
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x000BA3E8 File Offset: 0x000B85E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AddCharacterController()
	{
		if (!this.PhysicsTransform)
		{
			return;
		}
		float num = 0.08f;
		Vector3 center = Vector3.zero;
		bool flag = false;
		GameObject gameObject = this.PhysicsTransform.gameObject;
		float num2;
		float num3;
		if (this is EntityPlayer)
		{
			CharacterController component = gameObject.GetComponent<CharacterController>();
			if (!component)
			{
				Log.Error("Player !cc");
				return;
			}
			center = component.center;
			num2 = component.height;
			num3 = component.radius;
			this.m_characterController = new CharacterControllerUnity(component);
			if (!this.isEntityRemote)
			{
				gameObject.AddComponent<ColliderHitCallForward>().Entity = this;
			}
			BoxCollider boxCollider = this.nativeCollider as BoxCollider;
			if (boxCollider)
			{
				num2 = Utils.FastMax(boxCollider.size.y - num, this.stepHeight);
				center = boxCollider.center;
				center.y = num2 * 0.5f;
				if (boxCollider.size.x > boxCollider.size.y)
				{
					center.y += (boxCollider.size.x - boxCollider.size.y) * 0.5f;
				}
				num3 = boxCollider.size.x * 0.5f - num;
				flag = true;
			}
		}
		else
		{
			flag = true;
			CapsuleCollider component2 = gameObject.GetComponent<CapsuleCollider>();
			if (component2)
			{
				center = component2.center;
				num2 = component2.height;
				num3 = component2.radius;
			}
			else
			{
				gameObject.AddComponent<CapsuleCollider>();
				center.y = 0.9f;
				num2 = 1.8f;
				num3 = 0.3f;
			}
			if (this.physicsCapsuleCollider)
			{
				num2 = this.physicsBaseHeight;
				center.y = num2 * 0.5f;
			}
			CharacterController characterController;
			if (gameObject.TryGetComponent<CharacterController>(out characterController))
			{
				center = characterController.center;
				num2 = characterController.height;
				num3 = characterController.radius;
				UnityEngine.Object.Destroy(characterController);
				Log.Warning("{0} has old CC", new object[]
				{
					this.ToString()
				});
			}
			this.m_characterController = new CharacterControllerKinematic(this);
		}
		if (num2 <= 0f)
		{
			return;
		}
		if (flag)
		{
			center.y /= this.physicsHeightScale;
			this.m_characterController.SetSize(center, num2 / this.physicsHeightScale, num3);
			this.physicsBaseHeight = num2;
			this.physicsHeight = num2;
			if (this.physicsCapsuleCollider)
			{
				this.PhysicsSetHeight(num2);
			}
		}
		this.m_characterController.SetStepOffset(this.stepHeight);
		Vector3 localScale = base.transform.localScale;
		this.scaledExtent = new Vector3(num3 * localScale.x, num2 * localScale.y * 0.5f, num3 * localScale.z);
		this.boundingBox = BoundsUtils.BoundsForMinMax(-this.scaledExtent, this.scaledExtent);
		if (this.nativeCollider)
		{
			this.nativeCollider.enabled = false;
		}
	}

	// Token: 0x06001EAE RID: 7854 RVA: 0x000BA6D4 File Offset: 0x000B88D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetCCScale(float scale)
	{
		CharacterControllerAbstract characterController = this.m_characterController;
		if (characterController == null)
		{
			return;
		}
		this.PhysicsTransform.localScale = Vector3.one;
		Vector3 center = characterController.GetCenter() * scale;
		float num = characterController.GetHeight() * scale;
		if (num < 2.2f && num > 1.89f)
		{
			num = 1.89f;
			center.y = num * 0.5f;
		}
		float num2 = Utils.FastMax(scale, 1f);
		characterController.SetSize(center, num, characterController.GetRadius() * num2);
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x000BA752 File Offset: 0x000B8952
	public virtual void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		this.entityClass = _entityClass;
		this.assets = _assets;
		this.InitCommon();
		this.InitEModel(_eModelAssets);
		this.PhysicsInit();
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x000BA778 File Offset: 0x000B8978
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void InitCommon()
	{
		EntityClass entityClass = EntityClass.list[this.entityClass];
		this.cachedTags = entityClass.Tags;
		this.bIsChunkObserver = entityClass.bIsChunkObserver;
		this.CopyPropertiesFromEntityClass();
		this.InitializeBagFromLootList();
		if (this.PhysicsTransform)
		{
			this.PhysicsTransform.gameObject.tag = "Physics";
		}
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000BA7DC File Offset: 0x000B89DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitializeBagFromLootList()
	{
		if (this.bag != null)
		{
			return;
		}
		string text = this.GetLootList();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		LootContainer lootContainer = LootContainer.GetLootContainer(text, false);
		if (lootContainer == null)
		{
			return;
		}
		Vector2i size = lootContainer.size;
		if (size.x <= 0 || size.y <= 0)
		{
			return;
		}
		this.bag = new Bag(size.x * size.y);
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x000BA840 File Offset: 0x000B8A40
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitEModel(EModelInstanceAssets _assets)
	{
		Type modelType = EntityClass.list[this.entityClass].modelType;
		this.emodel = (base.gameObject.AddComponent(modelType) as EModelBase);
		this.emodel.Init(this.world, this, _assets);
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x000BA88D File Offset: 0x000B8A8D
	public virtual void PostInit()
	{
		if (this.emodel != null)
		{
			this.emodel.PostInit();
			this.HandleNavObject();
		}
		this.fallLastY = this.position.y;
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x000BA8C0 File Offset: 0x000B8AC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsInit()
	{
		Transform transform = GameUtils.FindTagInChilds(this.ModelTransform, "Physics");
		if (transform)
		{
			this.PhysicsTransform = this.RootTransform.Find("Physics");
			if (this.PhysicsTransform)
			{
				UnityEngine.Object.Destroy(this.PhysicsTransform.gameObject);
				Log.Warning("{0} has old Physics", new object[]
				{
					this.ToString()
				});
			}
			this.PhysicsTransform = transform;
			transform.SetParent(this.RootTransform, false);
		}
		else if (!this.PhysicsTransform)
		{
			this.PhysicsTransform = this.RootTransform.Find("Physics");
		}
		this.physicsRBT = GameUtils.FindTagInChilds(this.RootTransform, "LargeEntityBlocker");
		if (this.physicsRBT)
		{
			Transform transform2 = base.transform;
			Transform parent = this.physicsRBT.parent;
			this.physicsPos = this.physicsRBT.position;
			this.physicsRot = transform2.rotation;
			if (parent != transform2.parent)
			{
				Vector3 vector = this.physicsRBT.localPosition;
				float x = parent.lossyScale.x;
				vector += parent.localPosition * (1f / x);
				Collider[] componentsInChildren = this.physicsRBT.GetComponentsInChildren<Collider>();
				for (int i = componentsInChildren.Length - 1; i >= 0; i--)
				{
					Collider collider = componentsInChildren[i];
					CapsuleCollider capsuleCollider;
					BoxCollider boxCollider;
					SphereCollider sphereCollider;
					if (capsuleCollider = (collider as CapsuleCollider))
					{
						capsuleCollider.center = (capsuleCollider.center + vector) * x;
						capsuleCollider.height *= x;
						capsuleCollider.radius *= x;
					}
					else if (boxCollider = (collider as BoxCollider))
					{
						boxCollider.center = (boxCollider.center + vector) * x;
						boxCollider.size *= x;
					}
					else if (sphereCollider = (collider as SphereCollider))
					{
						sphereCollider.center = (sphereCollider.center + vector) * x;
						sphereCollider.radius *= x;
					}
				}
				this.physicsBasePos = Vector3.zero;
				this.physicsRBT.SetParent(transform2.parent, true);
				this.physicsRBT.localScale = Vector3.one;
			}
			else
			{
				this.physicsBasePos = Vector3.Scale(this.physicsRBT.localPosition, parent.lossyScale);
			}
			this.physicsRB = this.physicsRBT.gameObject.AddComponent<Rigidbody>();
			this.physicsRB.useGravity = false;
			float v = EntityClass.list[this.entityClass].MassKg * 0.6f;
			this.physicsRB.mass = Utils.FastMax(30f, v);
			this.physicsRB.constraints = (RigidbodyConstraints)80;
			this.physicsRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			this.physicsTargetPos = this.physicsPos;
			CapsuleCollider component = this.physicsRBT.GetComponent<CapsuleCollider>();
			if (component && component.direction == 1)
			{
				this.physicsCapsuleCollider = component;
				this.physicsColliderRadius = component.radius;
				this.physicsHeightScale = 1.09f;
				float height = component.height;
				float y = component.center.y;
				float num = y + height * 0.5f;
				this.physicsBaseHeight = num * this.physicsHeightScale;
				this.physicsColliderLowerY = y - height * 0.5f;
				if ((double)this.physicsBaseHeight > 1.95)
				{
					this.physicsBaseHeight = 1.95f;
				}
			}
		}
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x000BAC74 File Offset: 0x000B8E74
	public void PhysicsSetRB(Rigidbody rb)
	{
		this.physicsRB = rb;
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x000BAC7D File Offset: 0x000B8E7D
	public void PhysicsPause()
	{
		if (this.physicsRBT)
		{
			this.physicsRBT.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x000BACA0 File Offset: 0x000B8EA0
	public virtual void PhysicsResume(Vector3 pos, float rotY)
	{
		this.rotation = new Vector3(0f, rotY, 0f);
		if (this.physicsRBT)
		{
			this.physicsRBT.gameObject.SetActive(true);
			this.physicsRBT.eulerAngles = this.rotation;
			this.physicsPosMoveDistance = 0f;
		}
		this.SetPosition(pos, true);
		base.transform.SetPositionAndRotation(pos - Origin.position, Quaternion.Euler(this.rotation));
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x000BAD28 File Offset: 0x000B8F28
	public virtual void PhysicsPush(Vector3 forceVec, Vector3 forceWorldPos, bool affectLocalPlayerController = false)
	{
		if (forceVec.sqrMagnitude > 0f)
		{
			Rigidbody rigidbody = this.physicsRB;
			if (rigidbody)
			{
				if (!this.emodel.IsRagdollActive)
				{
					forceVec *= 5f;
				}
				if (forceWorldPos.sqrMagnitude > 0f)
				{
					rigidbody.AddForceAtPosition(forceVec, forceWorldPos - Origin.position, ForceMode.Impulse);
					return;
				}
				rigidbody.AddForce(forceVec, ForceMode.Impulse);
			}
		}
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x000BAD98 File Offset: 0x000B8F98
	public void PhysicsSetHeight(float _height)
	{
		this.physicsHeight = _height;
		float num = this.physicsColliderLowerY;
		if (_height - num < this.physicsColliderRadius)
		{
			num = _height - this.physicsColliderRadius;
			if (num < 0f)
			{
				num = 0f;
			}
		}
		this.physicsCapsuleCollider.height = _height - num;
		Vector3 center = this.physicsCapsuleCollider.center;
		center.y = (_height + num) * 0.5f;
		if (center.y < this.physicsColliderRadius)
		{
			center.y = this.physicsColliderRadius;
		}
		this.physicsCapsuleCollider.center = center;
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000BAE28 File Offset: 0x000B9028
	public virtual void PhysicsMasterBecome()
	{
		this.isPhysicsMaster = true;
		this.physicsMasterTargetTime = 0f;
		this.SetPosition(this.physicsMasterTargetPos, false);
		this.qrotation = this.physicsMasterTargetRot;
		if (this.physicsRB)
		{
			this.physicsRB.position = this.position - Origin.position;
			this.physicsRB.rotation = this.qrotation;
			this.physicsRB.velocity = this.physicsVel;
			this.physicsRB.angularVelocity = this.physicsAngVel;
		}
	}

	// Token: 0x06001EBB RID: 7867 RVA: 0x000BAEBC File Offset: 0x000B90BC
	public NetPackageEntityPhysics PhysicsMasterSetupBroadcast()
	{
		if ((this.position - this.physicsMasterSendPos).sqrMagnitude < 0.0025000002f && Quaternion.Angle(this.qrotation, this.physicsMasterSendRot) < 1f)
		{
			return null;
		}
		this.physicsMasterSendPos = this.position;
		this.physicsMasterSendRot = this.qrotation;
		return NetPackageManager.GetPackage<NetPackageEntityPhysics>().Setup(this);
	}

	// Token: 0x06001EBC RID: 7868 RVA: 0x000BAF28 File Offset: 0x000B9128
	public void PhysicsMasterSendToServer(Transform t)
	{
		if (this.clientEntityId != 0)
		{
			return;
		}
		this.position = t.position + Origin.position;
		this.qrotation = t.rotation;
		if (this.GetVelocityPerSecond().sqrMagnitude < 0.16000001f)
		{
			this.isPhysicsMaster = false;
		}
		NetPackageEntityPhysics package = NetPackageManager.GetPackage<NetPackageEntityPhysics>().Setup(this);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x000BAF94 File Offset: 0x000B9194
	public Vector3 PhysicsMasterGetFinalPosition()
	{
		if (this.physicsMasterTargetTime > 0f)
		{
			return this.physicsMasterTargetPos;
		}
		return this.position;
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x000BAFB0 File Offset: 0x000B91B0
	public void PhysicsMasterSetTargetOrientation(Vector3 pos, Quaternion rot)
	{
		this.physicsMasterFromPos = this.position;
		this.physicsMasterFromRot = this.qrotation;
		this.physicsMasterTargetElapsed = 0f;
		this.physicsMasterTargetTime = 0.1f;
		this.physicsMasterTargetPos = pos;
		this.physicsMasterTargetRot = rot;
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x000BAFF0 File Offset: 0x000B91F0
	public void PhysicsMasterTargetFrameUpdate()
	{
		this.physicsMasterTargetElapsed += Time.deltaTime;
		float t = this.physicsMasterTargetElapsed / this.physicsMasterTargetTime;
		Vector3 vector = Vector3.Lerp(this.physicsMasterFromPos, this.physicsMasterTargetPos, t);
		this.SetPosition(vector, true);
		Quaternion quaternion = Quaternion.Lerp(this.physicsMasterFromRot, this.physicsMasterTargetRot, t);
		this.qrotation = quaternion;
		this.physicsRB.position = vector - Origin.position;
		this.physicsRB.rotation = quaternion;
		if (this.physicsMasterTargetElapsed >= this.physicsMasterTargetTime)
		{
			this.physicsMasterTargetTime = 0f;
		}
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x000BB08D File Offset: 0x000B928D
	public void SetHeight(float _height)
	{
		this.m_characterController.SetHeight(_height / this.physicsHeightScale);
		this.PhysicsSetHeight(_height);
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x000BB0AC File Offset: 0x000B92AC
	public void SetMaxHeight(float _maxHeight)
	{
		this.physicsBaseHeight = _maxHeight;
		if (this.m_characterController != null)
		{
			this.m_characterController.SetHeight(_maxHeight / this.physicsHeightScale);
		}
		if (this.physicsCapsuleCollider)
		{
			this.PhysicsSetHeight(_maxHeight);
			float y = this.physicsCapsuleCollider.center.y;
			float num = this.physicsCapsuleCollider.height * 0.5f;
			this.physicsBaseHeight = y + num;
			this.physicsColliderLowerY = y - num;
		}
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x000BB124 File Offset: 0x000B9324
	public void SetScale(float scale)
	{
		Vector3 localScale = new Vector3(scale, scale, scale);
		this.ModelTransform.localScale = localScale;
		foreach (CharacterJoint characterJoint in this.ModelTransform.GetComponentsInChildren<CharacterJoint>())
		{
			if (characterJoint.autoConfigureConnectedAnchor)
			{
				characterJoint.autoConfigureConnectedAnchor = false;
				characterJoint.autoConfigureConnectedAnchor = true;
			}
		}
		if (this.physicsRBT)
		{
			this.physicsBaseHeight *= scale;
			this.physicsHeight *= scale;
			this.physicsColliderLowerY *= scale;
			Collider[] componentsInChildren2 = this.physicsRBT.GetComponentsInChildren<Collider>();
			for (int j = componentsInChildren2.Length - 1; j >= 0; j--)
			{
				Collider collider = componentsInChildren2[j];
				CapsuleCollider capsuleCollider = collider as CapsuleCollider;
				if (capsuleCollider != null)
				{
					capsuleCollider.center *= scale;
					capsuleCollider.height *= scale;
					capsuleCollider.radius *= scale;
				}
				else
				{
					BoxCollider boxCollider = collider as BoxCollider;
					if (boxCollider != null)
					{
						boxCollider.center *= scale;
						boxCollider.size *= scale;
					}
					else
					{
						SphereCollider sphereCollider = collider as SphereCollider;
						if (sphereCollider != null)
						{
							sphereCollider.center *= scale;
							sphereCollider.radius *= scale;
						}
					}
				}
			}
		}
		this.SetCCScale(scale);
	}

	// Token: 0x06001EC3 RID: 7875 RVA: 0x000BB290 File Offset: 0x000B9490
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ReplicateSpeeds()
	{
		int num = this.speedSentTicks - 1;
		this.speedSentTicks = num;
		if (num > 0)
		{
			return;
		}
		float num2 = this.speedForward - this.speedForwardSent;
		float num3 = this.speedStrafe - this.speedStrafeSent;
		if (num2 * num2 + num3 * num3 >= 4.0000004E-06f)
		{
			this.speedSentTicks = 3;
			this.speedForwardSent = this.speedForward;
			this.speedStrafeSent = this.speedStrafe;
			if (this.world.IsRemote())
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntitySpeeds>().Setup(this), false);
				return;
			}
			this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, this.entityId, NetPackageManager.GetPackage<NetPackageEntitySpeeds>().Setup(this), false);
		}
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetMovementState()
	{
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000BB348 File Offset: 0x000B9548
	[PublicizedFrom(EAccessModifier.Private)]
	public void animateYaw()
	{
		if (this.yawSeekTimeMax <= 1E-45f)
		{
			if (this.yawSeekTimeMax > 0f)
			{
				this.rotation.y = this.yawSeekAngleEnd;
				this.yawSeekTimeMax = 0f;
			}
			return;
		}
		this.yawSeekTime += Time.deltaTime;
		float t = Mathf.Clamp01(this.yawSeekTime / this.yawSeekTimeMax);
		if (this.yawSeekTime < this.yawSeekTimeMax)
		{
			this.rotation.y = Mathf.Lerp(this.yawSeekAngle, this.yawSeekAngleEnd, t);
			return;
		}
		this.yawSeekTimeMax = 0f;
		this.rotation.y = this.yawSeekAngleEnd;
	}

	// Token: 0x06001EC6 RID: 7878 RVA: 0x000BB3F9 File Offset: 0x000B95F9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsSeekYaw()
	{
		return this.yawSeekTimeMax > 0f;
	}

	// Token: 0x06001EC7 RID: 7879 RVA: 0x000BB408 File Offset: 0x000B9608
	public void SeekYawToPos(Vector3 _pos, float _yawSlowAt)
	{
		float num = _pos.x - this.position.x;
		float num2 = _pos.z - this.position.z;
		if (num * num + num2 * num2 > 0.0001f)
		{
			float yaw = Mathf.Atan2(num, num2) * 57.29578f;
			this.SeekYaw(yaw, 0f, _yawSlowAt);
		}
	}

	// Token: 0x06001EC8 RID: 7880 RVA: 0x000BB468 File Offset: 0x000B9668
	public float SeekYaw(float yaw, float _, float yawSlowAt)
	{
		if (yaw < 0f)
		{
			yaw += 360f;
		}
		if (yaw > 360f)
		{
			yaw -= 360f;
		}
		if (this.rotation.y < 0f)
		{
			this.rotation.y = this.rotation.y + 360f;
		}
		if (this.rotation.y > 360f)
		{
			this.rotation.y = this.rotation.y - 360f;
		}
		float num = EntityClass.list[this.entityClass].MaxTurnSpeed;
		if (this.inWaterPercent > 0.3f)
		{
			num *= 1f - this.inWaterPercent * 0.5f;
		}
		if (num > 0f)
		{
			float num2 = yaw - this.rotation.y;
			if (num2 != 0f)
			{
				if (num2 < -180f)
				{
					num2 += 360f;
				}
				if (num2 > 180f)
				{
					num2 -= 360f;
				}
				float num3 = Utils.FastAbs(num2);
				if (num3 < yawSlowAt)
				{
					float num4 = num3 / yawSlowAt;
					num = num * num4 * num4;
					num = Utils.FastMax(num, 20f);
				}
				this.yawSeekTime = 0f;
				this.yawSeekTimeMax = num3 / num;
				this.yawSeekAngle = this.rotation.y;
				this.yawSeekAngleEnd = this.rotation.y + num2;
				return num2;
			}
		}
		this.rotation.y = yaw;
		this.yawSeekTimeMax = 0f;
		return 0f;
	}

	// Token: 0x06001EC9 RID: 7881 RVA: 0x000BB5D3 File Offset: 0x000B97D3
	public virtual void KillLootContainer()
	{
		this.Kill(DamageResponse.New(true));
	}

	// Token: 0x06001ECA RID: 7882 RVA: 0x000BB5E4 File Offset: 0x000B97E4
	public virtual void Kill(DamageResponse _dmResponse)
	{
		this.SetDead();
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity != null)
				{
					entity.Kill(_dmResponse);
					entity.Detach();
				}
			}
		}
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x000BB634 File Offset: 0x000B9834
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickInWater()
	{
		this.inWaterLevel = this.CalcWaterLevel();
		this.inWaterPercent = this.inWaterLevel / (this.GetHeight() * 1.1f);
		this.isInWater = (this.inWaterPercent >= 0.25f);
		bool flag = this.isSwimming;
		this.isSwimming = this.CalcIfSwimming();
		if (this.isSwimming != flag)
		{
			this.SwimChanged();
		}
		bool flag2 = this.isHeadUnderwater;
		this.isHeadUnderwater = this.IsHeadUnderwater();
		if (this.isHeadUnderwater != flag2)
		{
			this.OnHeadUnderwaterStateChanged(this.isHeadUnderwater);
		}
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x000BB6C8 File Offset: 0x000B98C8
	public float CalcWaterLevel()
	{
		float num = this.GetHeight() * 1.1f;
		int num2 = Utils.Fastfloor(this.position.y + num);
		int num3 = Utils.Fastfloor(this.position.y);
		int num4 = num2 - num3 + 1;
		int num5 = Utils.Fastfloor(this.position.x);
		int num6 = Utils.Fastfloor(this.position.z);
		int i = -2;
		while (i < 6)
		{
			Vector3i vector3i;
			if (i < 0)
			{
				vector3i.x = num5;
				vector3i.z = num6;
				goto IL_E4;
			}
			vector3i.x = Utils.Fastfloor(this.position.x + Entity.waterLevelDirOffsets[i] * 0.28f);
			vector3i.z = Utils.Fastfloor(this.position.z + Entity.waterLevelDirOffsets[i + 1] * 0.28f);
			if (vector3i.x != num5 || vector3i.z != num6)
			{
				goto IL_E4;
			}
			IL_184:
			i += 2;
			continue;
			IL_E4:
			vector3i.y = num2;
			int num7 = num4;
			float num8;
			for (;;)
			{
				num8 = this.world.GetWaterPercent(vector3i);
				if (num8 > 0f)
				{
					break;
				}
				vector3i.y--;
				if (--num7 <= 0)
				{
					goto IL_184;
				}
			}
			if (num7 == num4)
			{
				vector3i.y++;
				if (this.world.GetWaterPercent(vector3i) == 0f)
				{
					num8 = 0.6f;
				}
				vector3i.y--;
			}
			else
			{
				num8 = 0.6f;
			}
			return Mathf.Clamp((float)vector3i.y + num8 - this.position.y, 0f, num);
		}
		return 0f;
	}

	// Token: 0x06001ECD RID: 7885 RVA: 0x000BB86C File Offset: 0x000B9A6C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CalcIfSwimming()
	{
		return this.inWaterPercent >= 0.5f;
	}

	// Token: 0x06001ECE RID: 7886 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SwimChanged()
	{
	}

	// Token: 0x06001ECF RID: 7887 RVA: 0x000BB87E File Offset: 0x000B9A7E
	public virtual bool IsHeadUnderwater()
	{
		return this.inWaterPercent >= 0.9f;
	}

	// Token: 0x06001ED0 RID: 7888 RVA: 0x000BB890 File Offset: 0x000B9A90
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnHeadUnderwaterStateChanged(bool _bUnderwater)
	{
		if (!_bUnderwater)
		{
			Manager.Play(this, "water_emerge", 1f, false);
		}
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnCollisionForward(Transform t, Collision collision, bool isStay)
	{
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x000BB8A8 File Offset: 0x000B9AA8
	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.normal.y > 0.707f && hit.normal.y > this.groundSurface.normal.y && hit.moveDirection.y < 0f)
		{
			if ((double)(hit.point - this.groundSurface.lastHitPoint).sqrMagnitude > 0.001 || this.groundSurface.lastNormal == Vector3.zero)
			{
				this.groundSurface.normal = hit.normal;
			}
			else
			{
				this.groundSurface.normal = this.groundSurface.lastNormal;
			}
			this.groundSurface.hitPoint = hit.point;
		}
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x000BB977 File Offset: 0x000B9B77
	[PublicizedFrom(EAccessModifier.Private)]
	public void ccEntityCollision(Vector3 _vel)
	{
		this.canCCMove = true;
		this.ccEntityCollisionStart(_vel);
		if (!this.isCCDelayed)
		{
			this.ccEntityCollisionResults();
		}
	}

	// Token: 0x06001ED4 RID: 7892 RVA: 0x000BB998 File Offset: 0x000B9B98
	[PublicizedFrom(EAccessModifier.Private)]
	public void ccEntityCollisionStart(Vector3 _vel)
	{
		this.groundSurface.lastHitPoint = this.groundSurface.hitPoint;
		this.groundSurface.lastNormal = this.groundSurface.normal;
		this.groundSurface.normal = Vector3.up;
		this.ySize *= this.ConditionalScalePhysicsMulConstant(0.4f);
		if (this.isMotionSlowedDown)
		{
			this.isMotionSlowedDown = false;
			_vel.x *= this.motionMultiplier;
			if (!this.isCollidedVertically)
			{
				_vel.y *= this.motionMultiplier;
			}
			_vel.z *= this.motionMultiplier;
		}
		this.hitMove = _vel;
		this.collisionFlags = CollisionFlags.None;
		if (this.IsStuck)
		{
			this.PhysicsTransform.position += this.hitMove;
			return;
		}
		this.collisionFlags = this.m_characterController.Move(this.hitMove);
	}

	// Token: 0x06001ED5 RID: 7893 RVA: 0x000BBA90 File Offset: 0x000B9C90
	[PublicizedFrom(EAccessModifier.Private)]
	public void ccEntityCollisionResults()
	{
		Vector3 vector = this.PhysicsTransform.position;
		this.physicsTargetPos = vector;
		vector += Origin.position;
		Vector3 vector2 = vector - this.position;
		this.position = vector;
		this.boundingBox.center = this.boundingBox.center + vector2;
		Vector2 vector3 = new Vector2(this.motion.x, this.motion.z);
		this.projectedMove = 0f;
		float sqrMagnitude = vector3.sqrMagnitude;
		if (sqrMagnitude > 1E-15f)
		{
			Vector2 lhs = new Vector2(vector2.x, vector2.z);
			this.projectedMove = Utils.FastClamp01(Vector2.Dot(lhs, vector3) / sqrMagnitude);
			vector3 *= this.projectedMove;
		}
		this.motion.x = vector3.x;
		this.motion.z = vector3.y;
		if (this.motion.y > 0f)
		{
			if (vector2.y >= 0f && vector2.y < this.motion.y * 0.95f)
			{
				this.motion.y = 0f;
			}
			else
			{
				this.motion.y = Utils.FastClamp(vector2.y, 0f, this.motion.y);
			}
		}
		else
		{
			this.motion.y = Utils.FastClamp(vector2.y, this.motion.y, 0f);
		}
		this.isCollidedHorizontally = ((this.collisionFlags & CollisionFlags.Sides) > CollisionFlags.None);
		this.isCollidedVertically = ((this.collisionFlags & (CollisionFlags)6) > CollisionFlags.None);
		bool flag = this.onGround;
		this.onGround = this.m_characterController.IsGrounded();
		this.wasOnGround = (flag && !this.onGround);
		this.wasOnGround |= this.isHeadUnderwater;
		if (this.wasOnGround)
		{
			this.fallLastY = vector.y - vector2.y;
		}
		if (this.onGround)
		{
			this.groundSurface.normal = this.m_characterController.GroundNormal;
		}
		this.world.CheckEntityCollisionWithBlocks(this);
		this.UpdateFall(this.hitMove.y);
	}

	// Token: 0x06001ED6 RID: 7894 RVA: 0x000BBCC8 File Offset: 0x000B9EC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void aabbEntityCollision(Vector3 _vel)
	{
		this.ySize *= 0.4f;
		if (this.isMotionSlowedDown)
		{
			this.isMotionSlowedDown = false;
			_vel.x *= this.motionMultiplier;
			if (!this.isCollidedVertically)
			{
				_vel.y *= this.motionMultiplier;
			}
			_vel.z *= this.motionMultiplier;
			this.motion = Vector3.zero;
		}
		Vector3 vector = _vel;
		Bounds bounds = this.boundingBox;
		if (Math.Abs(_vel.x) <= 0.0001f)
		{
			Math.Abs(_vel.z);
		}
		this.collAABB.Clear();
		Bounds aabb = BoundsUtils.ExpandDirectional(this.boundingBox, vector);
		this.world.GetCollidingBounds(this, aabb, this.collAABB);
		Vector3 vector2 = BoundsUtils.ClipBoundsMove(this.boundingBox, vector, this.collAABB, this.collAABB.Count);
		this.boundingBox.center = this.boundingBox.center + vector2;
		bool flag = this.onGround || (vector.y != vector2.y && vector.y < 0f);
		if (this.stepHeight > 0f && flag && this.ySize < 0.05f && (vector.x != vector2.x || vector.z != vector2.z))
		{
			Vector3 vector3 = vector2;
			vector2 = vector;
			vector2.y = this.stepHeight;
			Bounds bounds2 = this.boundingBox;
			this.boundingBox = bounds;
			this.collAABB.Clear();
			aabb = BoundsUtils.ExpandDirectional(this.boundingBox, new Vector3(vector2.x, 0f, vector2.z));
			this.world.GetCollidingBounds(this, aabb, this.collAABB);
			vector2 = BoundsUtils.ClipBoundsMove(this.boundingBox, vector2, this.collAABB, this.collAABB.Count);
			this.boundingBox.center = this.boundingBox.center + vector2;
			float y = BoundsUtils.ClipBoundsMoveY(this.boundingBox.min, this.boundingBox.max, -this.stepHeight, this.collAABB, this.collAABB.Count);
			this.boundingBox.center = this.boundingBox.center + new Vector3(0f, y, 0f);
			vector2.y = y;
			if (vector3.x * vector3.x + vector3.z * vector3.z >= vector2.x * vector2.x + vector2.z * vector2.z)
			{
				vector2 = vector3;
				this.boundingBox = bounds2;
			}
			else if (this.boundingBox.min.y - (float)((int)this.boundingBox.min.y) > 0f)
			{
				this.ySize += this.boundingBox.min.y - bounds2.min.y;
			}
		}
		Vector3 center = this.boundingBox.center;
		this.position.x = center.x;
		this.position.y = this.boundingBox.min.y + this.yOffset - this.ySize;
		this.position.z = center.z;
		if (this.PhysicsTransform != null && (this.PhysicsTransform.position - (this.position - Origin.position)).sqrMagnitude > 0.0001f)
		{
			this.PhysicsTransform.position = this.position - Origin.position;
		}
		this.isCollidedHorizontally = (vector.x != vector2.x || vector.z != vector2.z);
		this.isCollidedVertically = (vector.y != vector2.y);
		this.onGround = (vector.y != vector2.y && vector.y < 0f);
		this.world.CheckEntityCollisionWithBlocks(this);
		this.UpdateFall(vector2.y);
		if (vector.x != vector2.x)
		{
			this.motion.x = 0f;
		}
		if (vector.y != vector2.y)
		{
			this.motion.y = 0f;
		}
		if (vector.z != vector2.z)
		{
			this.motion.z = 0f;
		}
	}

	// Token: 0x06001ED7 RID: 7895 RVA: 0x000BC15B File Offset: 0x000BA35B
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CalcFixedUpdateTimeScaleConstants()
	{
		this.kAddFixedUpdateTimeScale = Time.deltaTime / 0.05f;
	}

	// Token: 0x06001ED8 RID: 7896 RVA: 0x000BC16E File Offset: 0x000BA36E
	public float ScalePhysicsMulConstant(float tickMulDelta)
	{
		return Mathf.Pow(tickMulDelta, this.kAddFixedUpdateTimeScale);
	}

	// Token: 0x06001ED9 RID: 7897 RVA: 0x000BC17C File Offset: 0x000BA37C
	public float ScalePhysicsAddConstant(float tickAddDelta)
	{
		return this.kAddFixedUpdateTimeScale * tickAddDelta;
	}

	// Token: 0x06001EDA RID: 7898 RVA: 0x000149AE File Offset: 0x00012BAE
	public float ConditionalScalePhysicsMulConstant(float tickMulDelta)
	{
		return tickMulDelta;
	}

	// Token: 0x06001EDB RID: 7899 RVA: 0x000149AE File Offset: 0x00012BAE
	public float ConditionalScalePhysicsAddConstant(float tickAddDelta)
	{
		return tickAddDelta;
	}

	// Token: 0x06001EDC RID: 7900 RVA: 0x000BC188 File Offset: 0x000BA388
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void entityCollision(Vector3 _motion)
	{
		if (this.emodel.IsRagdollMovement)
		{
			if (this.emodel.pelvisRB)
			{
				float num = this.emodel.bipedPelvisTransform.position.y + Origin.position.y;
				Vector3 velocity = this.emodel.pelvisRB.velocity;
				if (velocity.y < -1f)
				{
					this.fallVelY = Utils.FastMin(this.fallVelY, velocity.y);
					float num2 = this.fallLastY - num;
					if (num2 > 0f)
					{
						this.fallDistance += num2;
					}
				}
				else if (this.fallDistance > 0f)
				{
					this.fallLastMotion.y = this.fallVelY * 0.05f;
					this.fallLastMotion.y = this.fallLastMotion.y * 1.5f;
					this.onGround = true;
					this.UpdateFall(0f);
				}
				this.fallLastY = num;
			}
			return;
		}
		this.ApplyFixedUpdate();
		if (this.m_characterController != null)
		{
			this.ccEntityCollision(_motion);
			return;
		}
		this.aabbEntityCollision(_motion);
	}

	// Token: 0x06001EDD RID: 7901 RVA: 0x000BC2A0 File Offset: 0x000BA4A0
	public virtual void SetMotionMultiplier(float _motionMultiplier)
	{
		this.isMotionSlowedDown = true;
		this.motionMultiplier = _motionMultiplier;
		if (this.motionMultiplier < 0.5f)
		{
			this.fallDistance = 0f;
		}
	}

	// Token: 0x06001EDE RID: 7902 RVA: 0x000BC2C8 File Offset: 0x000BA4C8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetDistance(Entity _other)
	{
		return (this.position - _other.position).magnitude;
	}

	// Token: 0x06001EDF RID: 7903 RVA: 0x000BC2F0 File Offset: 0x000BA4F0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetDistanceSq(Entity _other)
	{
		return (this.position - _other.position).sqrMagnitude;
	}

	// Token: 0x06001EE0 RID: 7904 RVA: 0x000BC318 File Offset: 0x000BA518
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetDistanceSq(Vector3 _pos)
	{
		return (this.position - _pos).sqrMagnitude;
	}

	// Token: 0x06001EE1 RID: 7905 RVA: 0x000BC33C File Offset: 0x000BA53C
	public float GetSoundTravelTime(Vector3 _otherPos)
	{
		return (this.position - _otherPos).magnitude / 343f;
	}

	// Token: 0x06001EE2 RID: 7906 RVA: 0x000BC363 File Offset: 0x000BA563
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInWater()
	{
		return this.isInWater;
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000BC36B File Offset: 0x000BA56B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsSwimming()
	{
		return this.isSwimming;
	}

	// Token: 0x06001EE4 RID: 7908 RVA: 0x000BC373 File Offset: 0x000BA573
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsInElevator()
	{
		return this.bInElevator;
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000BC37B File Offset: 0x000BA57B
	public void SetInElevator(bool _b)
	{
		this.bInElevator = _b;
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000BC384 File Offset: 0x000BA584
	public virtual bool IsAirBorne()
	{
		return this.bAirBorne || !this.onGround;
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000BC399 File Offset: 0x000BA599
	public void SetAirBorne(bool _b)
	{
		this.bAirBorne = _b;
	}

	// Token: 0x170003AE RID: 942
	// (get) Token: 0x06001EE8 RID: 7912 RVA: 0x000BC3A2 File Offset: 0x000BA5A2
	public float width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.scaledExtent.x * 2f;
		}
	}

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x06001EE9 RID: 7913 RVA: 0x000BC3B5 File Offset: 0x000BA5B5
	public float depth
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.scaledExtent.z * 2f;
		}
	}

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x06001EEA RID: 7914 RVA: 0x000BC3C8 File Offset: 0x000BA5C8
	public float height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.scaledExtent.y * 2f;
		}
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
	public virtual float GetEyeHeight()
	{
		return 0f;
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x000BC3DB File Offset: 0x000BA5DB
	public virtual float GetHeight()
	{
		if (this.m_characterController != null)
		{
			return this.m_characterController.GetHeight();
		}
		return this.height;
	}

	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x06001EED RID: 7917 RVA: 0x000BC3F7 File Offset: 0x000BA5F7
	public float radius
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.scaledExtent.x;
		}
	}

	// Token: 0x06001EEE RID: 7918 RVA: 0x000BC404 File Offset: 0x000BA604
	public virtual void Move(Vector3 _direction, bool _isDirAbsolute, float _velocity, float _maxVelocity)
	{
		if (!this.IsClientControlled() && (GamePrefs.GetBool(EnumGamePrefs.DebugStopEnemiesMoving) || GameStats.GetInt(EnumGameStats.GameState) == 2))
		{
			return;
		}
		float y = _direction.y;
		_direction.y = 0f;
		_direction.Normalize();
		if (_isDirAbsolute)
		{
			float num = Mathf.Clamp(_maxVelocity - Mathf.Max(0f, Vector3.Dot(this.motion, _direction)), 0f, _velocity);
			this.motion.x = this.motion.x + this.ConditionalScalePhysicsAddConstant(_direction.x * num);
			this.motion.y = this.motion.y + this.ConditionalScalePhysicsAddConstant(_direction.y * _velocity);
			this.motion.z = this.motion.z + this.ConditionalScalePhysicsAddConstant(_direction.z * num);
			return;
		}
		Vector3 rhs = base.transform.forward * _direction.z + base.transform.right * _direction.x;
		rhs.Normalize();
		float num2 = Mathf.Clamp(_maxVelocity - Mathf.Max(0f, Vector3.Dot(this.motion, rhs)), 0f, _velocity);
		this.motion += base.transform.forward * this.ConditionalScalePhysicsAddConstant(_direction.z * num2) + base.transform.right * this.ConditionalScalePhysicsAddConstant(_direction.x * num2) + base.transform.up * this.ConditionalScalePhysicsAddConstant(y * _velocity);
	}

	// Token: 0x06001EEF RID: 7919 RVA: 0x000BC594 File Offset: 0x000BA794
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsAlive()
	{
		return !this.IsDead();
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x000BC59F File Offset: 0x000BA79F
	public bool WasAlive()
	{
		return !this.WasDead();
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x000BC5AA File Offset: 0x000BA7AA
	public virtual bool IsDead()
	{
		return this.bDead;
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x000BC5B2 File Offset: 0x000BA7B2
	public bool WasDead()
	{
		return this.bWasDead;
	}

	// Token: 0x06001EF3 RID: 7923 RVA: 0x000BC5BC File Offset: 0x000BA7BC
	public virtual void SetDead()
	{
		this.bDead = true;
		Manager.DestroySoundsForEntity(this.entityId);
		if (this.m_marker != null)
		{
			this.m_marker.Release();
			this.m_marker = null;
		}
		if (this.PhysicsTransform != null)
		{
			if (this.emodel.HasRagdoll())
			{
				this.PhysicsTransform.gameObject.layer = 17;
			}
			else
			{
				this.PhysicsTransform.gameObject.layer = 14;
			}
		}
		if (this.physicsRBT)
		{
			this.physicsRBT.gameObject.SetActive(false);
		}
		if (this.emodel != null)
		{
			this.emodel.SetDead();
		}
	}

	// Token: 0x06001EF4 RID: 7924 RVA: 0x000BC670 File Offset: 0x000BA870
	public virtual void SetAlive()
	{
		this.bDead = false;
		if (this.PhysicsTransform != null)
		{
			if (this is EntityPlayerLocal)
			{
				this.PhysicsTransform.gameObject.layer = 20;
				return;
			}
			if (ConsoleCmdCCPhysics.EnableCCPhysicsChanges && this is EntityPlayer)
			{
				this.PhysicsTransform.gameObject.layer = 3;
				return;
			}
			this.PhysicsTransform.gameObject.layer = 15;
		}
	}

	// Token: 0x06001EF5 RID: 7925 RVA: 0x000BC6E0 File Offset: 0x000BA8E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateFall(float mY)
	{
		if (this.onGround)
		{
			if (this.fallDistance > 0f)
			{
				this.fallHitGround(this.fallDistance, this.fallLastMotion);
				this.fallDistance = 0f;
				return;
			}
		}
		else if (mY < 0f)
		{
			float num = this.fallLastY - this.position.y;
			this.fallLastY = this.position.y;
			this.fallLastMotion = this.motion;
			this.fallDistance += num;
		}
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void fallHitGround(float _v, Vector3 _fallMotion)
	{
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x000BC768 File Offset: 0x000BA968
	public virtual void OnRagdoll(bool isActive)
	{
		if (isActive && this.emodel.bipedPelvisTransform)
		{
			this.fallLastY = this.emodel.bipedPelvisTransform.position.y + Origin.position.y;
			this.fallVelY = 0f;
		}
	}

	// Token: 0x06001EF8 RID: 7928 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool CanDamageEntity(int _sourceEntityId)
	{
		return true;
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x000BC7BB File Offset: 0x000BA9BB
	public virtual int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale = 1f)
	{
		this.setBeenAttacked();
		return 0;
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void setBeenAttacked()
	{
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void FireAttackedEvents(DamageResponse dmResponse)
	{
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void ProcessDamageResponse(DamageResponse _dmResponse)
	{
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x000BC7C4 File Offset: 0x000BA9C4
	public Bounds getBoundingBox()
	{
		return this.boundingBox;
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnDamagedByExplosion()
	{
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x000BC7CC File Offset: 0x000BA9CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnPushEntity(Entity _entity)
	{
		Vector3 vector = _entity.position - this.position;
		float num = Utils.FastMax(Mathf.Abs(vector.x), Mathf.Abs(vector.z));
		if (num >= 0.01f)
		{
			num = Mathf.Sqrt(num);
			float num2 = 1f / num;
			vector.x *= num2;
			vector.z *= num2;
			if (num2 < 1f)
			{
				vector.x *= num2;
				vector.z *= num2;
			}
			float num3 = 0.05f * (1f - this.entityCollisionReduction);
			num3 *= Utils.FastMin(_entity.GetWeight(), this.GetWeight()) / Utils.FastMax(_entity.GetWeight(), this.GetWeight());
			vector.x *= num3;
			vector.z *= num3;
			this.AddVelocity(new Vector3(-vector.x, 0f, -vector.z));
			if (_entity.CanBePushed())
			{
				_entity.AddVelocity(new Vector3(vector.x, 0f, vector.z));
			}
		}
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x000BC8EC File Offset: 0x000BAAEC
	public virtual void AddVelocity(Vector3 _vel)
	{
		this.motion += _vel;
		this.SetAirBorne(true);
	}

	// Token: 0x06001F01 RID: 7937 RVA: 0x000BC908 File Offset: 0x000BAB08
	public virtual Vector3 GetVelocityPerSecond()
	{
		if (this.AttachedToEntity)
		{
			return this.AttachedToEntity.GetVelocityPerSecond();
		}
		if (this.physicsRB)
		{
			return this.physicsRB.velocity;
		}
		return this.motion * 20f;
	}

	// Token: 0x06001F02 RID: 7938 RVA: 0x000BC957 File Offset: 0x000BAB57
	public virtual Vector3 GetAngularVelocityPerSecond()
	{
		if (this.AttachedToEntity)
		{
			return this.AttachedToEntity.GetAngularVelocityPerSecond();
		}
		if (this.physicsRB)
		{
			return this.physicsRB.angularVelocity;
		}
		return Vector3.zero;
	}

	// Token: 0x06001F03 RID: 7939 RVA: 0x000BC990 File Offset: 0x000BAB90
	public virtual void SetVelocityPerSecond(Vector3 vel, Vector3 angularVel)
	{
		if (this.AttachedToEntity)
		{
			this.AttachedToEntity.SetVelocityPerSecond(vel, angularVel);
			return;
		}
		this.physicsVel = vel;
		this.physicsAngVel = angularVel;
		if (this.isPhysicsMaster && this.physicsRB)
		{
			this.physicsRB.velocity = vel;
			this.physicsRB.angularVelocity = angularVel;
		}
		this.motion = vel * 0.05f;
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool CanBePushed()
	{
		return false;
	}

	// Token: 0x06001F05 RID: 7941 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual float GetPushBoundsVertical()
	{
		return 0f;
	}

	// Token: 0x06001F06 RID: 7942 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool CanCollideWith(Entity _other)
	{
		return true;
	}

	// Token: 0x06001F07 RID: 7943 RVA: 0x000BCA04 File Offset: 0x000BAC04
	public virtual void OnUpdatePosition(float _partialTicks)
	{
		this.ticksExisted++;
		this.prevPos = this.position;
		this.prevRotation = this.rotation;
		if (this.isUpdatePosition)
		{
			if (this.AttachedToEntity || (this.emodel && this.emodel.IsRagdollOn))
			{
				this.isUpdatePosition = false;
			}
			else
			{
				switch (this.positionUpdateMovementType)
				{
				case Entity.EnumPositionUpdateMovementType.Lerp:
					this.SetPosition(Vector3.Lerp(this.position, this.targetPos, Time.deltaTime / Time.fixedDeltaTime * Entity.tickPositionLerpMultiplier), false);
					goto IL_D6;
				case Entity.EnumPositionUpdateMovementType.MoveTowards:
					this.SetPosition(Vector3.MoveTowards(this.position, this.targetPos, Entity.tickPositionMoveTowardsMaxDistance), false);
					goto IL_D6;
				}
				this.SetPosition(this.targetPos, false);
				IL_D6:
				if (this.position == this.targetPos)
				{
					this.isUpdatePosition = false;
				}
				if (this.PhysicsTransform != null)
				{
					this.physicsTargetPos = this.position - Origin.position;
					this.PhysicsTransform.position = this.physicsTargetPos;
				}
			}
		}
		if (this.interpolateTargetQRot > 0)
		{
			this.qrotation = Quaternion.Lerp(this.qrotation, this.targetQRot, 1f / (float)this.interpolateTargetQRot);
			this.interpolateTargetQRot--;
		}
		if (this.interpolateTargetRot > 0)
		{
			float t = 1f / (float)this.interpolateTargetRot;
			this.SetRotation(new Vector3(Mathf.LerpAngle(this.rotation.x, this.targetRot.x, t), Mathf.LerpAngle(this.rotation.y, this.targetRot.y, t), Mathf.LerpAngle(this.rotation.z, this.targetRot.z, t)));
			this.interpolateTargetRot--;
		}
		if (!this.isEntityRemote && !this.IsDead() && !this.IsClientControlled() && this.position.y < 0f && this.IsDeadIfOutOfWorld())
		{
			EntityDrone entityDrone = this as EntityDrone;
			if (entityDrone)
			{
				entityDrone.NotifyOffTheWorld();
				return;
			}
			Log.Warning(string.Concat(new string[]
			{
				"Entity ",
				(this != null) ? this.ToString() : null,
				" fell off the world, id=",
				this.entityId.ToString(),
				" pos=",
				this.position.ToCultureInvariantString()
			}));
			this.MarkToUnload();
		}
	}

	// Token: 0x06001F08 RID: 7944 RVA: 0x000BCCA0 File Offset: 0x000BAEA0
	public virtual void CheckPosition()
	{
		if (float.IsNaN(this.position.x) || float.IsInfinity(this.position.x))
		{
			this.position.x = this.lastTickPos[0].x;
		}
		if (float.IsNaN(this.position.y) || float.IsInfinity(this.position.y))
		{
			this.position.y = this.lastTickPos[0].y;
		}
		if (float.IsNaN(this.position.z) || float.IsInfinity(this.position.z))
		{
			this.position.z = this.lastTickPos[0].z;
		}
		if (float.IsNaN(this.rotation.x) || float.IsInfinity(this.rotation.x))
		{
			this.rotation.x = this.prevRotation.x;
		}
		if (float.IsNaN(this.rotation.y) || float.IsInfinity(this.rotation.y))
		{
			this.rotation.y = this.prevRotation.y;
		}
		if (float.IsNaN(this.rotation.z) || float.IsInfinity(this.rotation.z))
		{
			this.rotation.z = this.prevRotation.z;
		}
	}

	// Token: 0x06001F09 RID: 7945 RVA: 0x000BCE1C File Offset: 0x000BB01C
	public virtual void OnUpdateEntity()
	{
		bool flag = this.isInWater;
		if (!this.isEntityStatic())
		{
			this.TickInWater();
		}
		if (this.isEntityRemote)
		{
			return;
		}
		if (this.isInWater)
		{
			if (!flag && !this.firstUpdate && this.fallDistance > 1f)
			{
				this.PlayOneShot("waterfallinginto", false, false, false, null, 1f);
			}
			this.fallDistance = 0f;
		}
		if (!this.RootMotion && !this.IsDead() && this.CanBePushed())
		{
			List<Entity> entitiesInBounds = this.world.GetEntitiesInBounds(this, BoundsUtils.ExpandBounds(this.boundingBox, 0.2f, this.GetPushBoundsVertical(), 0.2f));
			if (entitiesInBounds != null && entitiesInBounds.Count > 0)
			{
				for (int i = 0; i < entitiesInBounds.Count; i++)
				{
					Entity entity = entitiesInBounds[i];
					this.OnPushEntity(entity);
				}
			}
		}
		this.firstUpdate = false;
	}

	// Token: 0x06001F0A RID: 7946 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnAddedToWorld()
	{
	}

	// Token: 0x06001F0B RID: 7947 RVA: 0x000BCEFC File Offset: 0x000BB0FC
	public virtual void OnEntityUnload()
	{
		if (this.isUnloaded)
		{
			Log.Warning("OnEntityUnload already unloaded {0} ", new object[]
			{
				this.GetDebugName()
			});
			return;
		}
		this.isUnloaded = true;
		Manager.DestroySoundsForEntity(this.entityId);
		if (this.movableChunkObserver != null)
		{
			this.movableChunkObserver.Dispose();
			this.movableChunkObserver = null;
		}
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity != null)
				{
					entity.Detach();
				}
			}
		}
		if (this.AttachedToEntity != null)
		{
			this.Detach();
		}
		if (this.emodel != null)
		{
			this.emodel.OnUnload();
		}
		try
		{
			UnityEngine.Object.Destroy(this.RootTransform.gameObject);
		}
		catch (Exception e)
		{
			Log.Error("OnEntityUnload: {0}", new object[]
			{
				this.GetDebugName()
			});
			Log.Exception(e);
		}
	}

	// Token: 0x06001F0C RID: 7948 RVA: 0x000BCFFC File Offset: 0x000BB1FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		this.assets.Release();
	}

	// Token: 0x06001F0D RID: 7949 RVA: 0x000BD00C File Offset: 0x000BB20C
	public virtual float GetLightBrightness()
	{
		Vector3i blockPosition = this.GetBlockPosition();
		Vector3i blockPos = blockPosition;
		blockPos.y += Mathf.RoundToInt(this.height + 0.5f);
		return Utils.FastMax(this.world.GetLightBrightness(blockPosition), this.world.GetLightBrightness(blockPos));
	}

	// Token: 0x06001F0E RID: 7950 RVA: 0x000BD05B File Offset: 0x000BB25B
	public Vector3i GetBlockPosition()
	{
		return World.worldToBlockPos(this.position);
	}

	// Token: 0x06001F0F RID: 7951 RVA: 0x000BD068 File Offset: 0x000BB268
	public virtual void InitLocation(Vector3 _pos, Vector3 _rot)
	{
		this.serverPos = NetEntityDistributionEntry.EncodePos(_pos);
		this.SetPosition(_pos, true);
		this.SetRotation(_rot);
		this.ResetLastTickPos(_pos);
		base.transform.SetPositionAndRotation(this.position - Origin.position, Quaternion.Euler(this.rotation));
	}

	// Token: 0x06001F10 RID: 7952 RVA: 0x000BD0BD File Offset: 0x000BB2BD
	public Vector3 GetPosition()
	{
		return this.position;
	}

	// Token: 0x06001F11 RID: 7953 RVA: 0x000BD0C8 File Offset: 0x000BB2C8
	public virtual void SetPosition(Vector3 _pos, bool _bUpdatePhysics = true)
	{
		this.position = _pos;
		float num = this.width * 0.5f;
		float num2 = this.depth * 0.5f;
		float num3 = _pos.y - this.yOffset + this.ySize;
		this.boundingBox = BoundsUtils.BoundsForMinMax(_pos.x - num, num3, _pos.z - num2, _pos.x + num, num3 + this.height, _pos.z + num2);
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity != null)
				{
					entity.SetPosition(_pos, false);
				}
			}
		}
		if (_bUpdatePhysics && this.PhysicsTransform != null)
		{
			this.PhysicsTransform.position = _pos - Origin.position;
			if (this.physicsRBT)
			{
				this.physicsPos = _pos - Origin.position + this.physicsBasePos;
				this.physicsRBT.position = this.physicsPos;
				this.physicsTargetPos = this.PhysicsTransform.position;
			}
		}
	}

	// Token: 0x06001F12 RID: 7954 RVA: 0x000BD1E9 File Offset: 0x000BB3E9
	public void SetRotationAndStopTurning(Vector3 _rot)
	{
		this.SetRotation(_rot);
		this.yawSeekTimeMax = 0f;
		this.interpolateTargetQRot = 0;
		this.interpolateTargetRot = 0;
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x000BD20B File Offset: 0x000BB40B
	public virtual void SetRotation(Vector3 _rot)
	{
		this.rotation = _rot;
		this.qrotation = Quaternion.Euler(_rot);
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x000BD220 File Offset: 0x000BB420
	public void SetPosAndRotFromNetwork(Vector3 _pos, Vector3 _rot, int _steps)
	{
		this.targetPos = _pos;
		this.targetRot = _rot;
		this.isUpdatePosition = true;
		this.interpolateTargetRot = _steps;
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x000BD23E File Offset: 0x000BB43E
	public void SetPosAndQRotFromNetwork(Vector3 _pos, Quaternion _rot, int _steps)
	{
		this.targetPos = _pos;
		this.targetQRot = _rot;
		this.isUpdatePosition = true;
		this.interpolateTargetQRot = _steps;
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x000BD25C File Offset: 0x000BB45C
	public void SetRotFromNetwork(Vector3 _rot, int _steps)
	{
		this.targetRot = _rot;
		this.interpolateTargetRot = _steps;
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x000BD26C File Offset: 0x000BB46C
	public void SetQRotFromNetwork(Quaternion _qrot, int _steps)
	{
		this.targetQRot = _qrot;
		this.interpolateTargetQRot = _steps;
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x000BD27C File Offset: 0x000BB47C
	public float GetBrightness(float _t)
	{
		int num = Utils.Fastfloor(this.position.x);
		int num2 = Utils.Fastfloor(this.position.z);
		if (this.world.GetChunkSync(World.toChunkXZ(num), World.toChunkXZ(num2)) != null)
		{
			float num3 = (this.boundingBox.max.y - this.boundingBox.min.y) * 0.66f;
			int y = Utils.Fastfloor((double)this.position.y - (double)this.yOffset + (double)num3);
			return this.world.GetLightBrightness(new Vector3i(num, y, num2));
		}
		return 0f;
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void VisiblityCheck(float _distanceSqr, bool _masterIsZooming)
	{
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x000BD322 File Offset: 0x000BB522
	public void SetIgnoredByAI(bool ignore)
	{
		this.isIgnoredByAI = ignore;
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x000BD32B File Offset: 0x000BB52B
	public virtual bool IsIgnoredByAI()
	{
		return this.isIgnoredByAI;
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x000BD333 File Offset: 0x000BB533
	public virtual Vector3 getHeadPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight(), 0f);
		}
		return this.emodel.GetHeadPosition();
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x000BD36F File Offset: 0x000BB56F
	public virtual Vector3 getNavObjectPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight(), 0f);
		}
		return this.emodel.GetNavObjectPosition();
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x000BD3AC File Offset: 0x000BB5AC
	public virtual Vector3 getBellyPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight() / 2f, 0f);
		}
		return this.emodel.GetBellyPosition();
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x000BD3FC File Offset: 0x000BB5FC
	public virtual Vector3 getHipPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight() / 2f, 0f);
		}
		return this.emodel.GetHipPosition();
	}

	// Token: 0x06001F20 RID: 7968 RVA: 0x000BD44C File Offset: 0x000BB64C
	public virtual Vector3 getChestPosition()
	{
		if (this.emodel == null)
		{
			return this.position + new Vector3(0f, this.GetEyeHeight() / 2.4f, 0f);
		}
		return this.emodel.GetChestPosition();
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x000BD499 File Offset: 0x000BB699
	public void SetVelocity(Vector3 _vel)
	{
		this.motion = _vel;
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x00040FA0 File Offset: 0x0003F1A0
	public virtual float GetWeight()
	{
		return 1f;
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x00040FA0 File Offset: 0x0003F1A0
	public virtual float GetPushFactor()
	{
		return 1f;
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x00040FA0 File Offset: 0x0003F1A0
	public virtual float GetSightDetectionScale()
	{
		return 1f;
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x000BD4A2 File Offset: 0x000BB6A2
	public virtual void OnLoadedFromEntityCache(EntityCreationData _ed)
	{
		if (this.bIsChunkObserver && !this.isEntityRemote)
		{
			this.movableChunkObserver = new MovableSharedChunkObserver(this.world.m_SharedChunkObserverCache);
			this.movableChunkObserver.SetPosition(this.position);
		}
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool IsSavedToNetwork()
	{
		return true;
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000BD4DB File Offset: 0x000BB6DB
	public virtual bool IsSavedToFile()
	{
		return !this.world.IsEditor() || !GameManager.Instance.GetDynamicPrefabDecorator().IsEntityInPrefab(this.entityId);
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetEntityName(string _name)
	{
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x000BD504 File Offset: 0x000BB704
	public virtual void CopyPropertiesFromEntityClass()
	{
		EntityClass entityClass = EntityClass.list[this.entityClass];
		this.RootMotion = entityClass.RootMotion;
		this.HasDeathAnim = entityClass.HasDeathAnim;
		this.entityFlags = entityClass.entityFlags;
		this.entityType = EntityType.Unknown;
		entityClass.Properties.ParseEnum<EntityType>(EntityClass.PropEntityType, ref this.entityType);
		entityClass.Properties.ParseFloat(EntityClass.PropLootDropProb, ref this.lootDropProb);
		entityClass.Properties.ParseString(EntityClass.PropLootList, ref this.lootList);
		entityClass.Properties.ParseString(EntityClass.PropMapIcon, ref this.mapIcon);
		entityClass.Properties.ParseString(EntityClass.PropCompassIcon, ref this.compassIcon);
		entityClass.Properties.ParseString(EntityClass.PropCompassUpIcon, ref this.compassUpIcon);
		entityClass.Properties.ParseString(EntityClass.PropCompassDownIcon, ref this.compassDownIcon);
		entityClass.Properties.ParseString(EntityClass.PropTrackerIcon, ref this.trackerIcon);
		entityClass.Properties.ParseBool(EntityClass.PropRotateToGround, ref this.isRotateToGround);
		int num = 0;
		int num2 = 1;
		while (num2 <= 10 && entityClass.Properties.Values.ContainsKey(string.Format("{0}{1}", EntityClass.PropCustomCommandName, num2)))
		{
			num++;
			num2++;
		}
		this.customCmds = new EntityActivationCommand[num];
		if (num > 0)
		{
			for (int i = 1; i <= num; i++)
			{
				if (entityClass.Properties.Values.ContainsKey(string.Format("{0}{1}", EntityClass.PropCustomCommandName, i)))
				{
					EntityActivationCommand entityActivationCommand = default(EntityActivationCommand);
					entityActivationCommand.commandId = entityClass.Properties.Values[string.Format("{0}{1}", EntityClass.PropCustomCommandName, i)];
					entityActivationCommand.icon = entityClass.Properties.Values[string.Format("{0}{1}", EntityClass.PropCustomCommandIcon, i)];
					entityActivationCommand.eventName = entityClass.Properties.Values[string.Format("{0}{1}", EntityClass.PropCustomCommandEvent, i)];
					string key = string.Format("{0}{1}", EntityClass.PropCustomCommandIconColor, i);
					if (this.EntityClass.Properties.Values.ContainsKey(key))
					{
						entityActivationCommand.iconColor = StringParsers.ParseHexColor(this.EntityClass.Properties.Values[key]);
					}
					else
					{
						entityActivationCommand.iconColor = Color.white;
					}
					key = string.Format("{0}{1}", EntityClass.PropCustomCommandActivateTime, i);
					if (entityClass.Properties.Values.ContainsKey(key))
					{
						entityActivationCommand.activateTime = StringParsers.ParseFloat(entityClass.Properties.Values[key], 0, -1, NumberStyles.Any);
					}
					else
					{
						entityActivationCommand.activateTime = -1f;
					}
					entityActivationCommand.enabled = true;
					this.customCmds[i - 1] = entityActivationCommand;
				}
			}
		}
		this.activationCommands = null;
		this.lastUpdateFrameOfActivationCommands = -1;
		this.lastUpdateActivationCommandsPlayerId = -1;
		this.lastUpdateHadEnabledActivationCommands = false;
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x000BD81B File Offset: 0x000BBA1B
	public virtual string GetLootList()
	{
		return this.lootList;
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x000BD823 File Offset: 0x000BBA23
	public virtual void MarkToUnload()
	{
		this.markedForUnload = true;
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x000BD82C File Offset: 0x000BBA2C
	public virtual bool IsMarkedForUnload()
	{
		return this.markedForUnload || this.IsDead();
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool IsSpawned()
	{
		return true;
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x000BD840 File Offset: 0x000BBA40
	public void ResetLastTickPos(Vector3 _pos)
	{
		for (int i = 0; i < this.lastTickPos.Length; i++)
		{
			this.lastTickPos[i] = _pos;
		}
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x000BD870 File Offset: 0x000BBA70
	public void SetLastTickPos(Vector3 _pos)
	{
		for (int i = this.lastTickPos.Length - 1; i > 0; i--)
		{
			this.lastTickPos[i] = this.lastTickPos[i - 1];
		}
		this.lastTickPos[0] = _pos;
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isDetailedHeadBodyColliders()
	{
		return false;
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual Transform GetModelTransform()
	{
		return null;
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x000BD8B9 File Offset: 0x000BBAB9
	public virtual Vector3 GetMapIconScale()
	{
		return new Vector3(1f, 1f, 1f);
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x000BD8CF File Offset: 0x000BBACF
	public virtual string GetMapIcon()
	{
		return this.mapIcon;
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x000BD8D7 File Offset: 0x000BBAD7
	public virtual string GetCompassIcon()
	{
		if (this.compassIcon == null)
		{
			return this.mapIcon;
		}
		return this.compassIcon;
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x000BD8EE File Offset: 0x000BBAEE
	public virtual string GetCompassUpIcon()
	{
		return this.compassUpIcon;
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x000BD8F6 File Offset: 0x000BBAF6
	public virtual string GetCompassDownIcon()
	{
		return this.compassDownIcon;
	}

	// Token: 0x06001F37 RID: 7991 RVA: 0x000BD8FE File Offset: 0x000BBAFE
	public virtual string GetTrackerIcon()
	{
		return this.trackerIcon;
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x000BD906 File Offset: 0x000BBB06
	public virtual bool HasUIIcon()
	{
		return this.mapIcon != null || this.trackerIcon != null || this.compassIcon != null;
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual EnumMapObjectType GetMapObjectType()
	{
		return EnumMapObjectType.Entity;
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsMapIconBlinking()
	{
		return false;
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x000BD923 File Offset: 0x000BBB23
	public virtual bool IsDrawMapIcon()
	{
		return this.IsSpawned();
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x000BD92B File Offset: 0x000BBB2B
	public virtual Color GetMapIconColor()
	{
		return Color.white;
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool CanMapIconBeSelected()
	{
		return false;
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x0002F184 File Offset: 0x0002D384
	public virtual int GetLayerForMapIcon()
	{
		return 2;
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x000BD932 File Offset: 0x000BBB32
	public virtual bool IsClientControlled()
	{
		return this.attachedEntities != null && this.attachedEntities.Length != 0 && this.attachedEntities[0] != null;
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool IsDeadIfOutOfWorld()
	{
		return true;
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool CanCollideWithBlocks()
	{
		return true;
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x000BD955 File Offset: 0x000BBB55
	public void SetSpawnerSource(EnumSpawnerSource _spawnerSource)
	{
		this.SetSpawnerSource(_spawnerSource, 0L, 0);
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x000BD961 File Offset: 0x000BBB61
	public void SetSpawnerSource(EnumSpawnerSource _spawnerSource, long _chunkKey, int _biomeIdHash)
	{
		this.spawnerSource = _spawnerSource;
		this.spawnerSourceChunkKey = _chunkKey;
		this.spawnerSourceBiomeIdHash = _biomeIdHash;
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x000BD978 File Offset: 0x000BBB78
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EnumSpawnerSource GetSpawnerSource()
	{
		return this.spawnerSource;
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x000BD980 File Offset: 0x000BBB80
	public long GetSpawnerSourceChunkKey()
	{
		return this.spawnerSourceChunkKey;
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x000BD988 File Offset: 0x000BBB88
	public int GetSpawnerSourceBiomeIdHash()
	{
		return this.spawnerSourceBiomeIdHash;
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
	public float CalculateAudioOcclusion()
	{
		return 0f;
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x000BD990 File Offset: 0x000BBB90
	public virtual void PlayOneShot(string clipName, bool sound_in_head = false, bool serverSignalOnly = false, bool isUnique = false, AnimationEvent _animEvent = null, float volumeScale = 1f)
	{
		if (!sound_in_head)
		{
			if (!serverSignalOnly)
			{
				Manager.BroadcastPlay(this, clipName, serverSignalOnly, volumeScale);
				return;
			}
			Handle handle = Manager.Play(this, clipName, volumeScale, true);
			if (_animEvent != null && handle != null)
			{
				int intParameter = _animEvent.intParameter;
				if (intParameter > 0)
				{
					this.addAnimatorAudioToMonitor((Entity.StopAnimatorAudioType)intParameter, handle);
					return;
				}
			}
		}
		else
		{
			Manager.PlayInsidePlayerHead(clipName, -1, 0f, false, isUnique);
		}
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x000BD9E4 File Offset: 0x000BBBE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void addAnimatorAudioToMonitor(Entity.StopAnimatorAudioType _sat, Handle _handle)
	{
		Handle handle;
		if (this.animatorAudioMonitoringDictionary.TryGetValue(_sat, out handle))
		{
			handle.Stop(this.entityId);
		}
		this.animatorAudioMonitoringDictionary[_sat] = _handle;
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x000BDA1C File Offset: 0x000BBC1C
	public void StopAnimatorAudio(Entity.StopAnimatorAudioType _sat)
	{
		Handle handle;
		if (this.animatorAudioMonitoringDictionary.TryGetValue(_sat, out handle))
		{
			handle.Stop(this.entityId);
			this.animatorAudioMonitoringDictionary.Remove(_sat);
		}
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x000BDA52 File Offset: 0x000BBC52
	public void StopOneShot(string clipName)
	{
		Manager.BroadcastStop(this.entityId, clipName);
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x000BDA60 File Offset: 0x000BBC60
	public bool HasEnabledActivationCommands(EntityPlayerLocal _playerFocusing)
	{
		return this.UpdateActivationCommands(_playerFocusing);
	}

	// Token: 0x06001F4D RID: 8013 RVA: 0x000BDA6C File Offset: 0x000BBC6C
	public EntityActivationCommand[] GetActivationCommands()
	{
		if (this.activationCommands != null)
		{
			return this.activationCommands;
		}
		List<EntityActivationCommand> commands = new List<EntityActivationCommand>();
		this.InitLocalActivationCommands(delegate(EntityActivationCommand command)
		{
			commands.Add(command);
		});
		if (this.customCmds != null)
		{
			for (int i = 0; i < this.customCmds.Length; i++)
			{
				commands.Add(this.customCmds[i]);
			}
		}
		this.ReorderActivationCommands(commands);
		this.activationCommands = commands.ToArray();
		return this.activationCommands;
	}

	// Token: 0x06001F4E RID: 8014 RVA: 0x000BDB00 File Offset: 0x000BBD00
	public bool UpdateActivationCommands(EntityPlayerLocal _playerFocusing)
	{
		int num = (_playerFocusing != null) ? _playerFocusing.entityId : -1;
		int frameCount = Time.frameCount;
		if (frameCount == this.lastUpdateFrameOfActivationCommands && num == this.lastUpdateActivationCommandsPlayerId)
		{
			return this.lastUpdateHadEnabledActivationCommands;
		}
		EntityActivationCommand[] array = this.GetActivationCommands();
		if (array == null || array.Length == 0)
		{
			this.lastUpdateFrameOfActivationCommands = frameCount;
			this.lastUpdateActivationCommandsPlayerId = num;
			this.lastUpdateHadEnabledActivationCommands = false;
			return false;
		}
		if (_playerFocusing == null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				EntityActivationCommand entityActivationCommand = array[i];
				entityActivationCommand.enabled = true;
				array[i] = entityActivationCommand;
			}
			this.lastUpdateFrameOfActivationCommands = frameCount;
			this.lastUpdateActivationCommandsPlayerId = num;
			this.lastUpdateHadEnabledActivationCommands = true;
			return true;
		}
		bool flag = false;
		for (int j = 0; j < array.Length; j++)
		{
			EntityActivationCommand entityActivationCommand2 = array[j];
			entityActivationCommand2.enabled = this.AllowActivationCommand(entityActivationCommand2.commandId, _playerFocusing);
			array[j] = entityActivationCommand2;
			flag |= entityActivationCommand2.enabled;
		}
		this.lastUpdateFrameOfActivationCommands = frameCount;
		this.lastUpdateActivationCommandsPlayerId = num;
		this.lastUpdateHadEnabledActivationCommands = flag;
		return flag;
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void InitLocalActivationCommands(Action<EntityActivationCommand> _addCallback)
	{
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void ReorderActivationCommands(List<EntityActivationCommand> _commands)
	{
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x000BDC14 File Offset: 0x000BBE14
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void MoveActivationCommandAfter(List<EntityActivationCommand> _commands, string _commandToMove, string _afterCommand)
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < _commands.Count; i++)
		{
			if (_commands[i].commandId == _commandToMove)
			{
				num = i;
			}
			if (_commands[i].commandId == _afterCommand)
			{
				num2 = i;
			}
		}
		if (num < 0 || num2 < 0 || num == num2)
		{
			return;
		}
		EntityActivationCommand item = _commands[num];
		_commands.RemoveAt(num);
		int index = (num < num2) ? num2 : (num2 + 1);
		_commands.Insert(index, item);
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x000BDC9C File Offset: 0x000BBE9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void MoveActivationCommandBefore(List<EntityActivationCommand> _commands, string _commandToMove, string _beforeCommand)
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < _commands.Count; i++)
		{
			if (_commands[i].commandId == _commandToMove)
			{
				num = i;
			}
			if (_commands[i].commandId == _beforeCommand)
			{
				num2 = i;
			}
		}
		if (num < 0 || num2 < 0 || num == num2)
		{
			return;
		}
		EntityActivationCommand item = _commands[num];
		_commands.RemoveAt(num);
		int index = (num < num2) ? (num2 - 1) : num2;
		_commands.Insert(index, item);
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x0004E558 File Offset: 0x0004C758
	public virtual string GetActivationText()
	{
		return string.Empty;
	}

	// Token: 0x06001F54 RID: 8020 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool AllowActivationCommand(ReadOnlySpan<char> _commandName, EntityPlayerLocal _playerFocusing)
	{
		return true;
	}

	// Token: 0x06001F55 RID: 8021 RVA: 0x000BDD24 File Offset: 0x000BBF24
	public void ActivateEntityCommand(EntityActivationCommand _command, EntityPlayerLocal _playerFocusing)
	{
		this.OnEntityActivated(_command, _playerFocusing);
		if (!string.IsNullOrEmpty(_command.eventName))
		{
			EntityClass ec = EntityClass.list[this.entityClass];
			if (_command.activateTime > 0f)
			{
				TimerEventData timerEventData2 = new TimerEventData();
				timerEventData2.Data = null;
				timerEventData2.CloseOnHit = true;
				timerEventData2.FullTimeFinishEvent += delegate(TimerEventData timerEventData)
				{
					GameEventManager.Current.HandleAction(ec.onActivateEvent, _playerFocusing, this, false, "", "", false, true, "", null);
				};
				XUiC_Timer.OpenTimer(_playerFocusing.PlayerUI.xui, _command.activateTime, timerEventData2, -1f, "", true);
				return;
			}
			if (ec.onActivateEvent != "")
			{
				GameEventManager.Current.HandleAction(ec.onActivateEvent, _playerFocusing, this, false, "", "", false, true, "", null);
			}
		}
	}

	// Token: 0x06001F56 RID: 8022 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEntityActivated(EntityActivationCommand _command, EntityPlayerLocal _playerFocusing)
	{
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x00045D81 File Offset: 0x00043F81
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool CommandIs(ReadOnlySpan<char> _givenCommand, string _compareCommand)
	{
		return _givenCommand.Equals(_compareCommand, StringComparison.Ordinal);
	}

	// Token: 0x06001F58 RID: 8024 RVA: 0x000BDE1A File Offset: 0x000BC01A
	public void Collect(int _playerId)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(this.entityId, _playerId), false);
			return;
		}
		this.OnCollectLocal(_playerId);
		this.OnCollectServer(_playerId);
	}

	// Token: 0x06001F59 RID: 8025 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnCollectServer(int _playerId)
	{
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnCollectLocal(int _playerId)
	{
	}

	// Token: 0x170003B2 RID: 946
	// (get) Token: 0x06001F5B RID: 8027 RVA: 0x0002F184 File Offset: 0x0002D384
	public LockTargetType LockTargetType
	{
		get
		{
			return LockTargetType.Entity;
		}
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsSharedLock(ushort _channel)
	{
		return false;
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x000BDE54 File Offset: 0x000BC054
	public virtual bool CanLockOnServer(int _lockingPlayerID, ILockContext _context, ushort _channel)
	{
		if (this.bag == null)
		{
			return false;
		}
		if (this.spawnById > 0 && this.spawnById != _lockingPlayerID)
		{
			if (TwitchManager.Current.StealingCrateEvent != "")
			{
				EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(_lockingPlayerID) as EntityPlayer;
				if (entityPlayer != null)
				{
					GameEventManager.Current.HandleAction(TwitchManager.Current.StealingCrateEvent, entityPlayer, entityPlayer, false, "", "", false, true, "", null);
				}
			}
			if (!this.spawnByAllowShare)
			{
				if (TwitchManager.Current.DeniedCrateEvent != "")
				{
					EntityPlayer entityPlayer2 = GameManager.Instance.World.GetEntity(_lockingPlayerID) as EntityPlayer;
					if (entityPlayer2 != null)
					{
						GameEventManager.Current.HandleAction(TwitchManager.Current.DeniedCrateEvent, entityPlayer2, entityPlayer2, false, "", "", false, true, "", null);
					}
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x000BDF40 File Offset: 0x000BC140
	public virtual bool CanLockLocally(ILockContext _context, ushort _channel)
	{
		bool flag;
		if (this.bag == null)
		{
			Entity.EntityLockContext entityLockContext = _context as Entity.EntityLockContext;
			flag = (entityLockContext != null && entityLockContext.Bag != null);
		}
		else
		{
			flag = true;
		}
		return flag && LocalPlayerUI.GetUIForPrimaryPlayer() != null;
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x000BDF80 File Offset: 0x000BC180
	public virtual void OnLockedServer(bool _success, int _lockingPlayerID, ILockContext _context, ushort _channel)
	{
		if (!_success || this.bag == null)
		{
			return;
		}
		Entity.EntityLockContext entityLockContext = _context as Entity.EntityLockContext;
		if (entityLockContext != null)
		{
			entityLockContext.FirstTimeTouched = !this.bag.Touched;
			entityLockContext.Bag = this.bag;
		}
		if (!this.bag.Touched)
		{
			GameManager.Instance.lootManager.LootBagOpened(this.bag, this, _lockingPlayerID);
		}
		this.bag.Touched = true;
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x000BDFF4 File Offset: 0x000BC1F4
	public virtual void OnLockedLocal(bool _success, ILockContext _context, ushort _channel)
	{
		Entity.EntityLockContext entityLockContext = _context as Entity.EntityLockContext;
		if (entityLockContext != null && entityLockContext.Bag != null)
		{
			this.bag = entityLockContext.Bag.Clone();
		}
		LocalPlayerUI playerUI = LocalPlayerUI.GetUIForPrimaryPlayer();
		if (this.bag == null || !_success || playerUI == null)
		{
			if (playerUI != null)
			{
				GameManager.ShowTooltip(playerUI.entityPlayer, Localization.Get("ttNoInteractItem", false, null), string.Empty, "ui_denied", null, false, false, 0f);
			}
			return;
		}
		LootContainer lootContainer = LootContainer.GetLootContainer(this.GetLootList(), true);
		XUi xui = playerUI.xui;
		Bag bag = this.bag;
		LootContainer lootContainer2 = lootContainer;
		string containerName = Localization.Get(this.LocalizedEntityName, false, null);
		Action onModified = new Action(this.OnBagModified);
		Action onClose = delegate()
		{
			LockManager.Instance.UnlockRequestLocal();
		};
		Func<bool> interactionCheck = () => this.GetDistanceSq(playerUI.entityPlayer) <= Constants.cPlayerInteractDistance * Constants.cPlayerInteractDistance;
		Entity.EntityLockContext entityLockContext2 = _context as Entity.EntityLockContext;
		XUiC_BagStorageWindowGroup.Open(xui, this, bag, lootContainer2, containerName, onModified, onClose, interactionCheck, entityLockContext2 != null && entityLockContext2.FirstTimeTouched);
		if (lootContainer != null && playerUI.entityPlayer != null)
		{
			lootContainer.ExecuteBuffActions(this.entityId, playerUI.entityPlayer);
		}
		this.bag.Touched = true;
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x000BE148 File Offset: 0x000BC348
	public virtual void OnUnlockedServer(int _unlockingPlayerId, ushort _channel)
	{
		if (this.bag == null)
		{
			return;
		}
		LootContainer lootContainer = LootContainer.GetLootContainer(this.GetLootList(), true);
		if (lootContainer == null)
		{
			return;
		}
		LootContainer.DestroyOnClose destroyOnClose = lootContainer.destroyOnClose;
		if (destroyOnClose != LootContainer.DestroyOnClose.True && (destroyOnClose != LootContainer.DestroyOnClose.Empty || !this.bag.IsEmpty()))
		{
			return;
		}
		if (!this.bag.IsEmpty())
		{
			this.DropBagServer();
			this.bag.Clear();
		}
		this.KillLootContainer();
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x000BE1BC File Offset: 0x000BC3BC
	public void DropBagServer()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (this is EntityLootContainer)
		{
			return;
		}
		Vector3 transformPos = this.GetPosition();
		transformPos.y += 0.9f;
		if (this.lootDropProb != 0f)
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			if (entityClass.lootDrops != null)
			{
				EntityLootContainer entityLootContainer = EntityFactory.CreateEntity(entityClass.LootDropPick(this.rand), transformPos, Vector3.zero) as EntityLootContainer;
				GameManager.Instance.World.SpawnEntityInWorld(entityLootContainer);
				entityLootContainer.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
				Manager.BroadcastPlay(transformPos, "zpack_spawn", 0f);
				return;
			}
		}
		if (this.bag == null || this.bag.IsEmpty())
		{
			return;
		}
		EntityLootContainer entityLootContainer2 = EntityFactory.CreateEntity("DroppedLootContainer".GetHashCode(), transformPos, Vector3.zero) as EntityLootContainer;
		if (entityLootContainer2 == null)
		{
			return;
		}
		entityLootContainer2.OverrideLootList = this.GetLootList();
		entityLootContainer2.SetContent(ItemStack.Clone(this.bag.GetSlots()), this.bag.SlotCount);
		entityLootContainer2.bag.Touched = this.bag.Touched;
		GameManager.Instance.World.SpawnEntityInWorld(entityLootContainer2);
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x000BE308 File Offset: 0x000BC508
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnBagModified()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			NetPackageBag package = NetPackageManager.GetPackage<NetPackageBag>().Setup(this.entityId, this.bag);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
		}
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x000BE344 File Offset: 0x000BC544
	public void SetAttachMaxCount(int maxCount)
	{
		if (this.attachedEntities != null)
		{
			if (this.attachedEntities.Length == maxCount)
			{
				return;
			}
			for (int i = maxCount; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity)
				{
					entity.Detach();
				}
			}
		}
		Entity[] array = this.attachedEntities;
		this.attachedEntities = null;
		if (maxCount > 0)
		{
			this.attachedEntities = new Entity[maxCount];
			if (array != null)
			{
				int num = Utils.FastMin(array.Length, maxCount);
				for (int j = 0; j < num; j++)
				{
					this.attachedEntities[j] = array[j];
				}
			}
		}
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x000BE3D7 File Offset: 0x000BC5D7
	public int GetAttachMaxCount()
	{
		if (this.attachedEntities != null)
		{
			return (int)((byte)this.attachedEntities.Length);
		}
		return 0;
	}

	// Token: 0x06001F66 RID: 8038 RVA: 0x000BE3EC File Offset: 0x000BC5EC
	public int GetAttachFreeCount()
	{
		int num = 0;
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				if (this.attachedEntities[i] == null)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x000BE42B File Offset: 0x000BC62B
	public Entity GetAttached(int slot)
	{
		if (this.attachedEntities != null && slot < this.attachedEntities.Length)
		{
			return this.attachedEntities[slot];
		}
		return null;
	}

	// Token: 0x170003B3 RID: 947
	// (get) Token: 0x06001F68 RID: 8040 RVA: 0x000BE44A File Offset: 0x000BC64A
	public Entity AttachedMainEntity
	{
		get
		{
			if (this.attachedEntities == null)
			{
				return null;
			}
			return this.attachedEntities[0];
		}
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x000BE460 File Offset: 0x000BC660
	public Entity GetFirstAttached()
	{
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				Entity entity = this.attachedEntities[i];
				if (entity)
				{
					return entity;
				}
			}
		}
		return null;
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x000BE49C File Offset: 0x000BC69C
	public EntityPlayerLocal GetAttachedPlayerLocal()
	{
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				EntityPlayerLocal entityPlayerLocal = this.attachedEntities[i] as EntityPlayerLocal;
				if (entityPlayerLocal)
				{
					return entityPlayerLocal;
				}
			}
		}
		return null;
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x000BE4DD File Offset: 0x000BC6DD
	public bool CanAttach(Entity _entity)
	{
		return this.FindAttachSlot(_entity) < 0 && this.FindAttachSlot(null) >= 0;
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x000BE4F8 File Offset: 0x000BC6F8
	public int FindAttachSlot(Entity _entity)
	{
		if (this.attachedEntities != null)
		{
			for (int i = 0; i < this.attachedEntities.Length; i++)
			{
				if (this.attachedEntities[i] == _entity)
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x000BE533 File Offset: 0x000BC733
	public bool IsAttached(Entity _entity)
	{
		return this.FindAttachSlot(_entity) >= 0;
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x000BE542 File Offset: 0x000BC742
	public bool IsDriven()
	{
		return this.attachedEntities != null && this.attachedEntities[0];
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x000BE55C File Offset: 0x000BC75C
	public virtual int AttachEntityToSelf(Entity _other, int slot)
	{
		int num = this.FindAttachSlot(_other);
		if (num >= 0)
		{
			if (slot < 0 || slot == num)
			{
				return num;
			}
			this.DetachEntity(_other);
		}
		if (slot < 0)
		{
			slot = this.FindAttachSlot(null);
			if (slot < 0)
			{
				return -1;
			}
		}
		if (slot >= this.attachedEntities.Length)
		{
			return -1;
		}
		if (slot == 0)
		{
			this.serverPos = NetEntityDistributionEntry.EncodePos(this.position);
			this.isEntityRemote = _other.isEntityRemote;
		}
		this.attachedEntities[slot] = _other;
		return slot;
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x000BE5D0 File Offset: 0x000BC7D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void DetachEntity(Entity _other)
	{
		int num = this.FindAttachSlot(_other);
		if (num < 0)
		{
			return;
		}
		if (num == 0)
		{
			this.isEntityRemote = this.world.IsRemote();
		}
		this.attachedEntities[num] = null;
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x000BE608 File Offset: 0x000BC808
	public virtual void StartAttachToEntity(Entity _other, int slot = -1)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.AttachServer, this.entityId, _other.entityId, slot), false);
			return;
		}
		slot = this.AttachToEntity(_other, slot);
		if (slot >= 0)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.AttachClient, this.entityId, _other.entityId, slot), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06001F72 RID: 8050 RVA: 0x000BE688 File Offset: 0x000BC888
	public virtual int AttachToEntity(Entity _other, int slot = -1)
	{
		if (_other.IsAttached(this))
		{
			return -1;
		}
		slot = _other.AttachEntityToSelf(this, slot);
		if (slot < 0)
		{
			return slot;
		}
		AttachedToEntitySlotInfo attachedToInfo = _other.GetAttachedToInfo(slot);
		this.RootTransform.SetParent(attachedToInfo.enterParentTransform, false);
		this.RootTransform.localPosition = Vector3.zero;
		this.RootTransform.localEulerAngles = Vector3.zero;
		this.ModelTransform.localPosition = attachedToInfo.enterPosition;
		this.ModelTransform.localEulerAngles = attachedToInfo.enterRotation;
		this.rotation = attachedToInfo.enterRotation;
		if (this.isEntityRemote && !attachedToInfo.bKeep3rdPersonModelVisible)
		{
			this.emodel.SetVisible(false, false);
		}
		this.AttachedToEntity = _other;
		return slot;
	}

	// Token: 0x06001F73 RID: 8051 RVA: 0x000BE740 File Offset: 0x000BC940
	public void SendDetach()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.DetachServer, this.entityId, -1, -1), false);
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAttach>().Setup(NetPackageEntityAttach.AttachType.DetachClient, this.entityId, -1, -1), false, -1, -1, -1, null, 192, false);
		}
		this.Detach();
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x000BE7B0 File Offset: 0x000BC9B0
	public virtual void Detach()
	{
		this.RootTransform.parent = EntityFactory.ParentNameToTransform[EntityClass.list[this.entityClass].parentGameObjectName];
		if (this.AttachedToEntity == null)
		{
			return;
		}
		int num = this.AttachedToEntity.FindAttachSlot(this);
		if (num < 0)
		{
			num = 0;
		}
		AttachedToEntitySlotInfo attachedToInfo = this.AttachedToEntity.GetAttachedToInfo(num);
		AttachedToEntitySlotExit attachedToEntitySlotExit = this.FindValidExitPosition(attachedToInfo.exits);
		Entity attachedToEntity = this.AttachedToEntity;
		this.AttachedToEntity = null;
		this.isUpdatePosition = false;
		if (attachedToEntitySlotExit.position != Vector3.zero)
		{
			this.SetPosition(attachedToEntitySlotExit.position, true);
			this.SetRotation(attachedToEntitySlotExit.rotation);
		}
		this.ResetLastTickPos(base.transform.position + Origin.position);
		attachedToEntity.DetachEntity(this);
		EntityVehicle entityVehicle = attachedToEntity as EntityVehicle;
		if (entityVehicle != null && entityVehicle.HornActivation != null)
		{
			XUiC_InteractionPrompt.SetText(LocalPlayerUI.GetUIForPrimaryPlayer(), "");
		}
		if (this.isEntityRemote && !attachedToInfo.bKeep3rdPersonModelVisible)
		{
			this.emodel.SetVisible(true, false);
		}
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void MoveByAttachedEntity(EntityPlayerLocal _player)
	{
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000BE8C8 File Offset: 0x000BCAC8
	public virtual AttachedToEntitySlotExit FindValidExitPosition(List<AttachedToEntitySlotExit> candidatePositions)
	{
		AttachedToEntitySlotExit attachedToEntitySlotExit;
		attachedToEntitySlotExit.position = Vector3.zero;
		attachedToEntitySlotExit.rotation = Vector3.zero;
		if (this.m_characterController == null)
		{
			return attachedToEntitySlotExit;
		}
		this.AttachedToEntity.SetPhysicsCollidersLayer(14);
		float radius = this.m_characterController.GetRadius();
		float num = this.m_characterController.GetHeight() - radius * 2f;
		Vector3 vector = base.transform.position + this.m_characterController.GetCenter();
		vector.y -= num * 0.5f;
		for (int i = 0; i < candidatePositions.Count; i++)
		{
			for (float num2 = 0f; num2 < 0.75f; num2 += 0.24f)
			{
				Vector3 vector2 = vector;
				vector2.y += num2;
				attachedToEntitySlotExit = candidatePositions[i];
				attachedToEntitySlotExit.position.y = attachedToEntitySlotExit.position.y + num2;
				Vector3 vector3 = attachedToEntitySlotExit.position - Origin.position - vector2;
				vector3.y += radius;
				Vector3 normalized = vector3.normalized;
				float num3 = vector3.magnitude;
				if (normalized.y < 0f)
				{
					float num4 = normalized.y;
					if (num4 < -0.707f)
					{
						break;
					}
					num4 *= -1.6f;
					num3 += num4;
					attachedToEntitySlotExit.position += normalized * num4;
				}
				bool flag = false;
				Vector3 origin = vector2;
				for (float num5 = -radius * 0.5f; num5 < num; num5 += 0.2f)
				{
					origin.y = vector2.y + num5;
					flag = Physics.Raycast(origin, normalized, num3, 1084817408);
					if (flag)
					{
						break;
					}
				}
				Vector3 vector4 = vector2 - normalized * 0.1f;
				Vector3 point = vector4;
				point.y += num;
				if (!flag && !Physics.CapsuleCast(vector4, point, radius, normalized, num3, 1084817408))
				{
					this.AttachedToEntity.SetPhysicsCollidersLayer(21);
					return attachedToEntitySlotExit;
				}
			}
		}
		this.AttachedToEntity.SetPhysicsCollidersLayer(21);
		attachedToEntitySlotExit.position = Vector3.zero;
		return attachedToEntitySlotExit;
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x000BEAF8 File Offset: 0x000BCCF8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetPhysicsCollidersLayer(int layer)
	{
		Collider[] componentsInChildren = this.PhysicsTransform.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layer;
		}
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DebugCapsuleCast()
	{
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual AttachedToEntitySlotInfo GetAttachedToInfo(int _slotIdx)
	{
		return null;
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x000BEB30 File Offset: 0x000BCD30
	public virtual bool CanUpdateEntity()
	{
		Vector3i vector3i = World.worldToBlockPos(this.position);
		IChunk chunkFromWorldPos = this.world.GetChunkFromWorldPos(vector3i.x, vector3i.y, vector3i.z);
		if (chunkFromWorldPos == null || !chunkFromWorldPos.GetAvailable())
		{
			return false;
		}
		for (int i = 0; i < this.adjacentPositions.Length; i++)
		{
			int num = World.toChunkXZ(vector3i.x + this.adjacentPositions[i].x);
			int num2 = World.toChunkXZ(vector3i.z + this.adjacentPositions[i].z);
			if (num != chunkFromWorldPos.X || num2 != chunkFromWorldPos.Z)
			{
				IChunk chunkSync = this.world.GetChunkSync(num, num2);
				if (chunkSync == null || !chunkSync.GetAvailable())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x000BEBF7 File Offset: 0x000BCDF7
	public virtual Transform GetThirdPersonCameraTransform()
	{
		return this.emodel.GetThirdPersonCameraTransform();
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x000BEC04 File Offset: 0x000BCE04
	public virtual void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		_bw.Write((byte)this.spawnerSource);
		if (this.spawnerSource == EnumSpawnerSource.Biome)
		{
			_bw.Write(this.spawnerSourceBiomeIdHash);
			_bw.Write(this.spawnerSourceChunkKey);
		}
		_bw.Write(this.WorldTimeBorn);
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x000BEC40 File Offset: 0x000BCE40
	public virtual void Read(byte _version, BinaryReader _br)
	{
		if (_version >= 11)
		{
			this.spawnerSource = (EnumSpawnerSource)_br.ReadByte();
			if (this.spawnerSource == EnumSpawnerSource.Biome)
			{
				if (_version >= 28)
				{
					this.spawnerSourceBiomeIdHash = _br.ReadInt32();
				}
				else
				{
					_br.ReadString();
					this.spawnerSource = EnumSpawnerSource.Delete;
				}
				this.spawnerSourceChunkKey = _br.ReadInt64();
			}
		}
		if (_version >= 15)
		{
			this.WorldTimeBorn = _br.ReadUInt64();
		}
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isEntityStatic()
	{
		return false;
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void AddUIHarvestingItem(ItemStack _is, bool _bAddOnlyIfNotExisting = false)
	{
	}

	// Token: 0x06001F80 RID: 8064 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsQRotationUsed()
	{
		return false;
	}

	// Token: 0x06001F81 RID: 8065 RVA: 0x000BECA5 File Offset: 0x000BCEA5
	public bool HasAnyTags(FastTags<TagGroup.Global> tags)
	{
		return this.cachedTags.Test_AnySet(tags);
	}

	// Token: 0x06001F82 RID: 8066 RVA: 0x000BECB3 File Offset: 0x000BCEB3
	public bool HasAllTags(FastTags<TagGroup.Global> tags)
	{
		return this.cachedTags.Test_AllSet(tags);
	}

	// Token: 0x06001F83 RID: 8067 RVA: 0x000BECC4 File Offset: 0x000BCEC4
	public void SetTransformActive(string partName, bool active)
	{
		Transform transform = base.transform.FindInChilds(partName, false);
		if (transform != null)
		{
			transform.gameObject.SetActive(active);
		}
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x000BECF4 File Offset: 0x000BCEF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleNavObject()
	{
		if (EntityClass.list[this.entityClass].NavObject != "")
		{
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(EntityClass.list[this.entityClass].NavObject, this, "", false);
		}
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x000BED50 File Offset: 0x000BCF50
	public void AddNavObject(string navObjectName, string overrideSprite, string overrideText)
	{
		if (this.NavObject == null)
		{
			NavObjectManager.Instance.RegisterNavObject(navObjectName, this, overrideSprite, false).name = overrideText;
			return;
		}
		NavObjectClass navObjectClass = NavObjectClass.GetNavObjectClass(navObjectName);
		this.NavObject.name = overrideText;
		this.NavObject.AddNavObjectClass(navObjectClass);
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x000BED9C File Offset: 0x000BCF9C
	public void RemoveNavObject(string navObjectName)
	{
		NavObjectClass navObjectClass = NavObjectClass.GetNavObjectClass(navObjectName);
		if (this.NavObject != null && this.NavObject.RemoveNavObjectClass(navObjectClass))
		{
			this.NavObject = null;
		}
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x000BEDD0 File Offset: 0x000BCFD0
	public string GetDebugName()
	{
		EntityAlive entityAlive = this as EntityAlive;
		if (entityAlive != null)
		{
			return entityAlive.EntityName;
		}
		return base.GetType().ToString();
	}

	// Token: 0x0400148D RID: 5261
	public const int EntityIdInvalid = -1;

	// Token: 0x0400148E RID: 5262
	public const int cIdCreatorIsServer = -2;

	// Token: 0x0400148F RID: 5263
	public const int cClientIdStart = -2;

	// Token: 0x04001490 RID: 5264
	public const int cClientIdCreate = -1;

	// Token: 0x04001491 RID: 5265
	public const int cClientIdNone = 0;

	// Token: 0x04001492 RID: 5266
	public const int cKillAnythingDamage = 99999;

	// Token: 0x04001493 RID: 5267
	public const int cIgnoreDamage = -1;

	// Token: 0x04001494 RID: 5268
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public FastTags<TagGroup.Global> cachedTags;

	// Token: 0x04001495 RID: 5269
	public bool RootMotion;

	// Token: 0x04001496 RID: 5270
	public bool HasDeathAnim;

	// Token: 0x04001497 RID: 5271
	public World world;

	// Token: 0x04001498 RID: 5272
	public EntityInstanceAssets assets;

	// Token: 0x04001499 RID: 5273
	public Transform PhysicsTransform;

	// Token: 0x0400149A RID: 5274
	public Transform RootTransform;

	// Token: 0x0400149B RID: 5275
	public Transform ModelTransform;

	// Token: 0x0400149C RID: 5276
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 scaledExtent;

	// Token: 0x0400149D RID: 5277
	public Bounds boundingBox;

	// Token: 0x0400149E RID: 5278
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Collider nativeCollider;

	// Token: 0x0400149F RID: 5279
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int interpolateTargetRot;

	// Token: 0x040014A0 RID: 5280
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int interpolateTargetQRot;

	// Token: 0x040014A1 RID: 5281
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isUpdatePosition;

	// Token: 0x040014A2 RID: 5282
	public int entityId;

	// Token: 0x040014A3 RID: 5283
	public int clientEntityId;

	// Token: 0x040014A4 RID: 5284
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float yOffset;

	// Token: 0x040014A5 RID: 5285
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool wasOnGround = true;

	// Token: 0x040014A6 RID: 5286
	public bool onGround;

	// Token: 0x040014A7 RID: 5287
	public bool isCollided;

	// Token: 0x040014A8 RID: 5288
	public bool isCollidedHorizontally;

	// Token: 0x040014A9 RID: 5289
	public bool isCollidedVertically;

	// Token: 0x040014AA RID: 5290
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isMotionSlowedDown;

	// Token: 0x040014AB RID: 5291
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float motionMultiplier;

	// Token: 0x040014AC RID: 5292
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool firstUpdate = true;

	// Token: 0x040014AD RID: 5293
	public Vector3 prevRotation;

	// Token: 0x040014AE RID: 5294
	public Vector3 rotation;

	// Token: 0x040014AF RID: 5295
	public Quaternion qrotation = Quaternion.identity;

	// Token: 0x040014B0 RID: 5296
	public Vector3 position;

	// Token: 0x040014B1 RID: 5297
	public Vector3 prevPos;

	// Token: 0x040014B2 RID: 5298
	public Vector3 targetPos;

	// Token: 0x040014B3 RID: 5299
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 targetRot;

	// Token: 0x040014B4 RID: 5300
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Quaternion targetQRot = Quaternion.identity;

	// Token: 0x040014B5 RID: 5301
	public Vector3i chunkPosAddedEntityTo;

	// Token: 0x040014B6 RID: 5302
	public Vector3i serverPos;

	// Token: 0x040014B7 RID: 5303
	public Vector3i serverRot;

	// Token: 0x040014B8 RID: 5304
	public Vector3[] lastTickPos = new Vector3[5];

	// Token: 0x040014B9 RID: 5305
	public Vector3 motion;

	// Token: 0x040014BA RID: 5306
	public bool IsMovementReplicated = true;

	// Token: 0x040014BB RID: 5307
	public bool IsStuck;

	// Token: 0x040014BC RID: 5308
	public bool addedToChunk;

	// Token: 0x040014BD RID: 5309
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isInWater;

	// Token: 0x040014BE RID: 5310
	public bool isSwimming;

	// Token: 0x040014BF RID: 5311
	public float inWaterLevel;

	// Token: 0x040014C0 RID: 5312
	public float inWaterPercent;

	// Token: 0x040014C1 RID: 5313
	public bool isHeadUnderwater;

	// Token: 0x040014C2 RID: 5314
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bInElevator;

	// Token: 0x040014C3 RID: 5315
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bAirBorne;

	// Token: 0x040014C4 RID: 5316
	public float stepHeight;

	// Token: 0x040014C5 RID: 5317
	public float ySize;

	// Token: 0x040014C6 RID: 5318
	public float distanceWalked;

	// Token: 0x040014C7 RID: 5319
	public float distanceSwam;

	// Token: 0x040014C8 RID: 5320
	public float distanceClimbed;

	// Token: 0x040014C9 RID: 5321
	public float fallDistance;

	// Token: 0x040014CA RID: 5322
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fallLastY;

	// Token: 0x040014CB RID: 5323
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fallVelY;

	// Token: 0x040014CC RID: 5324
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 fallLastMotion;

	// Token: 0x040014CD RID: 5325
	public float entityCollisionReduction = 0.9f;

	// Token: 0x040014CE RID: 5326
	public bool isEntityRemote;

	// Token: 0x040014CF RID: 5327
	public GameRandom rand;

	// Token: 0x040014D0 RID: 5328
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int ticksExisted;

	// Token: 0x040014D1 RID: 5329
	public static float updatePositionLerpTimeScale = 8f;

	// Token: 0x040014D2 RID: 5330
	public static float updateRotationLerpTimeScale = 8f;

	// Token: 0x040014D3 RID: 5331
	public static float tickPositionMoveTowardsMaxDistance = 3f;

	// Token: 0x040014D4 RID: 5332
	public static float tickPositionLerpMultiplier = 0.5f;

	// Token: 0x040014D5 RID: 5333
	public int entityClass;

	// Token: 0x040014D6 RID: 5334
	public float lifetime;

	// Token: 0x040014D7 RID: 5335
	public int count;

	// Token: 0x040014D8 RID: 5336
	public int belongsPlayerId;

	// Token: 0x040014D9 RID: 5337
	public bool bWillRespawn;

	// Token: 0x040014DA RID: 5338
	public ulong WorldTimeBorn;

	// Token: 0x040014DB RID: 5339
	public DataItem<bool> IsFlyMode = new DataItem<bool>();

	// Token: 0x040014DC RID: 5340
	public DataItem<bool> IsGodMode = new DataItem<bool>();

	// Token: 0x040014DD RID: 5341
	public DataItem<bool> IsNoCollisionMode = new DataItem<bool>();

	// Token: 0x040014DE RID: 5342
	public EntityFlags entityFlags;

	// Token: 0x040014DF RID: 5343
	public EntityType entityType;

	// Token: 0x040014E0 RID: 5344
	public float lootDropProb;

	// Token: 0x040014E1 RID: 5345
	public string lootList;

	// Token: 0x040014E2 RID: 5346
	public Bag bag;

	// Token: 0x040014E3 RID: 5347
	public float speedForward;

	// Token: 0x040014E4 RID: 5348
	public float speedStrafe;

	// Token: 0x040014E5 RID: 5349
	public float speedVertical;

	// Token: 0x040014E6 RID: 5350
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int speedSentTicks;

	// Token: 0x040014E7 RID: 5351
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedForwardSent = float.MaxValue;

	// Token: 0x040014E8 RID: 5352
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedStrafeSent = float.MaxValue;

	// Token: 0x040014E9 RID: 5353
	public int MovementState;

	// Token: 0x040014EA RID: 5354
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float yawSeekTime;

	// Token: 0x040014EB RID: 5355
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float yawSeekTimeMax;

	// Token: 0x040014EC RID: 5356
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float yawSeekAngle;

	// Token: 0x040014ED RID: 5357
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float yawSeekAngleEnd;

	// Token: 0x040014EE RID: 5358
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IAIDirectorMarker m_marker;

	// Token: 0x040014EF RID: 5359
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string mapIcon;

	// Token: 0x040014F0 RID: 5360
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string compassIcon;

	// Token: 0x040014F1 RID: 5361
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string compassUpIcon;

	// Token: 0x040014F2 RID: 5362
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string compassDownIcon;

	// Token: 0x040014F3 RID: 5363
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string trackerIcon;

	// Token: 0x040014F4 RID: 5364
	public bool bDead;

	// Token: 0x040014F5 RID: 5365
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bWasDead;

	// Token: 0x040014F6 RID: 5366
	[Preserve]
	public EModelBase emodel;

	// Token: 0x040014F7 RID: 5367
	public CharacterControllerAbstract m_characterController;

	// Token: 0x040014F8 RID: 5368
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isCCDelayed;

	// Token: 0x040014F9 RID: 5369
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canCCMove;

	// Token: 0x040014FA RID: 5370
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CollisionFlags collisionFlags;

	// Token: 0x040014FB RID: 5371
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Entity.MoveHitSurface groundSurface;

	// Token: 0x040014FC RID: 5372
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 hitMove;

	// Token: 0x040014FD RID: 5373
	public float projectedMove;

	// Token: 0x040014FE RID: 5374
	public bool IsRotateToGroundFlat;

	// Token: 0x040014FF RID: 5375
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isRotateToGround;

	// Token: 0x04001500 RID: 5376
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotateToGroundPitch;

	// Token: 0x04001501 RID: 5377
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotateToGroundPitchVel;

	// Token: 0x04001502 RID: 5378
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EnumSpawnerSource spawnerSource;

	// Token: 0x04001503 RID: 5379
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int spawnerSourceBiomeIdHash;

	// Token: 0x04001504 RID: 5380
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public long spawnerSourceChunkKey;

	// Token: 0x04001505 RID: 5381
	public static int InstanceCount;

	// Token: 0x04001506 RID: 5382
	public bool IsDespawned;

	// Token: 0x04001507 RID: 5383
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool markedForUnload;

	// Token: 0x04001508 RID: 5384
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public MovableSharedChunkObserver movableChunkObserver;

	// Token: 0x04001509 RID: 5385
	public bool bIsChunkObserver;

	// Token: 0x0400150A RID: 5386
	public NavObject NavObject;

	// Token: 0x0400150B RID: 5387
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isIgnoredByAI;

	// Token: 0x0400150C RID: 5388
	public Entity AttachedToEntity;

	// Token: 0x0400150D RID: 5389
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Entity[] attachedEntities;

	// Token: 0x0400150E RID: 5390
	public const int cPhysicsMasterTickRate = 2;

	// Token: 0x0400150F RID: 5391
	public bool usePhysicsMaster;

	// Token: 0x04001510 RID: 5392
	public bool isPhysicsMaster;

	// Token: 0x04001511 RID: 5393
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsMasterFromPos;

	// Token: 0x04001512 RID: 5394
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion physicsMasterFromRot;

	// Token: 0x04001513 RID: 5395
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float physicsMasterTargetElapsed;

	// Token: 0x04001514 RID: 5396
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float physicsMasterTargetTime;

	// Token: 0x04001515 RID: 5397
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsMasterTargetPos;

	// Token: 0x04001516 RID: 5398
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion physicsMasterTargetRot;

	// Token: 0x04001517 RID: 5399
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsMasterSendPos;

	// Token: 0x04001518 RID: 5400
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion physicsMasterSendRot;

	// Token: 0x04001519 RID: 5401
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float physicsHeightScale = 1f;

	// Token: 0x0400151A RID: 5402
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform physicsRBT;

	// Token: 0x0400151B RID: 5403
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public CapsuleCollider physicsCapsuleCollider;

	// Token: 0x0400151C RID: 5404
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float physicsColliderRadius;

	// Token: 0x0400151D RID: 5405
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float physicsColliderLowerY;

	// Token: 0x0400151E RID: 5406
	public float physicsBaseHeight;

	// Token: 0x0400151F RID: 5407
	public float physicsHeight;

	// Token: 0x04001520 RID: 5408
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rigidbody physicsRB;

	// Token: 0x04001521 RID: 5409
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsPos;

	// Token: 0x04001522 RID: 5410
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsBasePos;

	// Token: 0x04001523 RID: 5411
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsTargetPos;

	// Token: 0x04001524 RID: 5412
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float physicsPosMoveDistance;

	// Token: 0x04001525 RID: 5413
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion physicsRot;

	// Token: 0x04001526 RID: 5414
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasFixedUpdate;

	// Token: 0x04001527 RID: 5415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsVel;

	// Token: 0x04001528 RID: 5416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsAngVel;

	// Token: 0x04001529 RID: 5417
	public bool spawnByAllowShare;

	// Token: 0x0400152A RID: 5418
	public int spawnById = -1;

	// Token: 0x0400152B RID: 5419
	public string spawnByName;

	// Token: 0x0400152C RID: 5420
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityActivationCommand[] activationCommands;

	// Token: 0x0400152D RID: 5421
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastUpdateFrameOfActivationCommands = -1;

	// Token: 0x0400152E RID: 5422
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastUpdateActivationCommandsPlayerId = -1;

	// Token: 0x0400152F RID: 5423
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lastUpdateHadEnabledActivationCommands;

	// Token: 0x04001530 RID: 5424
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityActivationCommand[] customCmds;

	// Token: 0x04001531 RID: 5425
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cWaterHeightScale = 1.1f;

	// Token: 0x04001532 RID: 5426
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float[] waterLevelDirOffsets = new float[]
	{
		Mathf.Cos(0f),
		Mathf.Sin(0f),
		Mathf.Cos(2.0943952f),
		Mathf.Sin(2.0943952f),
		Mathf.Cos(4.1887903f),
		Mathf.Sin(4.1887903f)
	};

	// Token: 0x04001533 RID: 5427
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<Bounds> collAABB = new List<Bounds>();

	// Token: 0x04001534 RID: 5428
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float kAddFixedUpdateTimeScale = 1f;

	// Token: 0x04001535 RID: 5429
	public EnumRemoveEntityReason unloadReason;

	// Token: 0x04001536 RID: 5430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isUnloaded;

	// Token: 0x04001537 RID: 5431
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int cAttachSlotNone = -1;

	// Token: 0x04001538 RID: 5432
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i[] adjacentPositions = new Vector3i[]
	{
		Vector3i.forward,
		Vector3i.back,
		Vector3i.left,
		Vector3i.right
	};

	// Token: 0x04001539 RID: 5433
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Entity.StopAnimatorAudioType, Handle> animatorAudioMonitoringDictionary = new Dictionary<Entity.StopAnimatorAudioType, Handle>();

	// Token: 0x020003F6 RID: 1014
	public struct MoveHitSurface
	{
		// Token: 0x0400153A RID: 5434
		public Vector3 hitPoint;

		// Token: 0x0400153B RID: 5435
		public Vector3 lastHitPoint;

		// Token: 0x0400153C RID: 5436
		public Vector3 normal;

		// Token: 0x0400153D RID: 5437
		public Vector3 lastNormal;
	}

	// Token: 0x020003F7 RID: 1015
	public enum EnumPositionUpdateMovementType
	{
		// Token: 0x0400153F RID: 5439
		Lerp,
		// Token: 0x04001540 RID: 5440
		MoveTowards,
		// Token: 0x04001541 RID: 5441
		Instant
	}

	// Token: 0x020003F8 RID: 1016
	public enum StopAnimatorAudioType
	{
		// Token: 0x04001543 RID: 5443
		StopOnReloadCancel = 1,
		// Token: 0x04001544 RID: 5444
		StopOnStopHolding
	}

	// Token: 0x020003F9 RID: 1017
	[Preserve]
	public class EntityLockContext : ILockContext
	{
		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06001F8A RID: 8074 RVA: 0x000BEF96 File Offset: 0x000BD196
		// (set) Token: 0x06001F8B RID: 8075 RVA: 0x000BEF9E File Offset: 0x000BD19E
		public string Command { get; [PublicizedFrom(EAccessModifier.Private)] set; } = string.Empty;

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06001F8C RID: 8076 RVA: 0x000BEFA7 File Offset: 0x000BD1A7
		// (set) Token: 0x06001F8D RID: 8077 RVA: 0x000BEFAF File Offset: 0x000BD1AF
		public Bag Bag { get; set; }

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06001F8E RID: 8078 RVA: 0x000BEFB8 File Offset: 0x000BD1B8
		// (set) Token: 0x06001F8F RID: 8079 RVA: 0x000BEFC0 File Offset: 0x000BD1C0
		public bool FirstTimeTouched { get; set; }

		// Token: 0x06001F90 RID: 8080 RVA: 0x000BEFC9 File Offset: 0x000BD1C9
		public EntityLockContext()
		{
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x000BEFDC File Offset: 0x000BD1DC
		public EntityLockContext(string _command, Bag _bag)
		{
			this.Command = (_command ?? string.Empty);
			this.Bag = _bag;
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x000BF008 File Offset: 0x000BD208
		public void Write(PooledBinaryWriter _bw)
		{
			_bw.Write(this.Command ?? string.Empty);
			_bw.Write(this.FirstTimeTouched);
			bool flag = this.Bag != null;
			_bw.Write(flag);
			if (flag)
			{
				this.Bag.Write(_bw);
			}
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x000BF056 File Offset: 0x000BD256
		public void Read(PooledBinaryReader _br)
		{
			this.Command = _br.ReadString();
			this.FirstTimeTouched = _br.ReadBoolean();
			if (_br.ReadBoolean())
			{
				this.Bag = Bag.Read(_br);
				return;
			}
			this.Bag = null;
		}
	}
}
