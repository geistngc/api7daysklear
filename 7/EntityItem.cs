using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004B9 RID: 1209
[Preserve]
public class EntityItem : Entity
{
	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x0600264C RID: 9804 RVA: 0x0002F184 File Offset: 0x0002D384
	public override Entity.EnumPositionUpdateMovementType positionUpdateMovementType
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return Entity.EnumPositionUpdateMovementType.Instant;
		}
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x000EAA9F File Offset: 0x000E8C9F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitLocalActivationCommands(Action<EntityActivationCommand> _addCallback)
	{
		_addCallback(new EntityActivationCommand("take", "hand", null, null));
		_addCallback(new EntityActivationCommand("search", "search", null, null));
	}

	// Token: 0x0600264E RID: 9806 RVA: 0x000EAAD0 File Offset: 0x000E8CD0
	public override bool AllowActivationCommand(ReadOnlySpan<char> _commandName, EntityPlayerLocal _playerFocusing)
	{
		if (base.CommandIs(_commandName, "take"))
		{
			return this.CanCollect() && this.onGround;
		}
		if (base.CommandIs(_commandName, "search"))
		{
			return this.bag != null;
		}
		return base.AllowActivationCommand(_commandName, _playerFocusing);
	}

	// Token: 0x0600264F RID: 9807 RVA: 0x000EAB1C File Offset: 0x000E8D1C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityActivated(EntityActivationCommand _command, EntityPlayerLocal _playerFocusing)
	{
		if (!base.CommandIs(_command.commandId, "take"))
		{
			if (base.CommandIs(_command.commandId, "search"))
			{
				LockManager.Instance.LockRequestLocal(this, new Entity.EntityLockContext(_command.commandId.ToString(), this.bag), 0);
			}
			return;
		}
		if (_playerFocusing.inventory.CanTakeItem(this.itemStack) || _playerFocusing.bag.CanTakeItem(this.itemStack))
		{
			base.Collect(_playerFocusing.entityId);
			return;
		}
		GameManager.ShowTooltip(_playerFocusing, Localization.Get("xuiInventoryFullForPickup", false, null), string.Empty, "ui_denied", null, false, false, 0f);
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x000EABD4 File Offset: 0x000E8DD4
	public override string GetActivationText()
	{
		GameManager instance = GameManager.Instance;
		EntityPlayerLocal entityPlayerLocal;
		if (instance == null)
		{
			entityPlayerLocal = null;
		}
		else
		{
			World world = instance.World;
			entityPlayerLocal = ((world != null) ? world.GetPrimaryPlayer() : null);
		}
		EntityPlayerLocal entityPlayerLocal2 = entityPlayerLocal;
		if (entityPlayerLocal2 == null)
		{
			return string.Empty;
		}
		PlayerActionsLocal playerInput = entityPlayerLocal2.playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		if (this.CanCollect() && this.onGround)
		{
			string localizedItemName = ItemClass.GetForId(this.itemStack.itemValue.type).GetLocalizedItemName();
			if (this.itemStack.count > 1)
			{
				return string.Format(Localization.Get("itemTooltipFocusedSeveral", false, null), arg, localizedItemName, this.itemStack.count);
			}
			return string.Format(Localization.Get("itemTooltipFocusedOne", false, null), arg, localizedItemName);
		}
		else
		{
			if (this.bag == null)
			{
				return string.Empty;
			}
			string arg2 = Localization.Get(this.LocalizedEntityName, false, null);
			if (!this.bag.Touched)
			{
				return string.Format(Localization.Get("lootTooltipNew", false, null), arg, arg2);
			}
			if (this.bag.IsEmpty())
			{
				return string.Format(Localization.Get("lootTooltipEmpty", false, null), arg, arg2);
			}
			return string.Format(Localization.Get("lootTooltipTouched", false, null), arg, arg2);
		}
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x000EAD24 File Offset: 0x000E8F24
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		this.usePhysicsMaster = true;
		this.isPhysicsMaster = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		base.Awake();
		this.yOffset = 0.15f;
		EntityItem.ItemInstanceCount++;
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
		}
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000EAD7C File Offset: 0x000E8F7C
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~EntityItem()
	{
		EntityItem.ItemInstanceCount--;
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000EADB0 File Offset: 0x000E8FB0
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
		this.itemRB = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x000EADC8 File Offset: 0x000E8FC8
	public override void PostInit()
	{
		base.PostInit();
		base.PhysicsSetRB(this.itemRB);
		base.transform.eulerAngles = this.rotation;
		if (this.itemClass != null)
		{
			this.stickPercent = this.itemClass.Properties.GetFloat("StickPercent");
			if (this.itemStack != null)
			{
				this.itemWorldData = this.itemClass.CreateWorldData(GameManager.Instance, this, this.itemStack.itemValue, this.belongsPlayerId);
			}
		}
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x000EAE4C File Offset: 0x000E904C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleNavObject()
	{
		if (!this.bWasThrown)
		{
			return;
		}
		if (this.itemClass != null && this.OwnerId != -1 && this.world.GetEntity(this.OwnerId) as EntityPlayerLocal != null && this.itemClass.NavObject != "")
		{
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(this.itemClass.NavObject, base.transform, "", false);
		}
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x000EAED0 File Offset: 0x000E90D0
	public void SetItemStack(ItemStack _itemStack)
	{
		if (this.itemStack == null)
		{
			this.itemStack = ItemStack.Empty;
		}
		this.lastCachedItemStack = this.itemStack.Clone();
		this.itemStack = _itemStack;
		this.itemClass = ItemClass.GetForId(this.itemStack.itemValue.type);
		this.distractionRadiusSq = EffectManager.GetValue(PassiveEffects.DistractionRadius, this.itemStack.itemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.distractionRadiusSq *= this.distractionRadiusSq;
		this.distractionLifetime = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.DistractionLifetime, this.itemStack.itemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		this.distractionEatTicks = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.DistractionEatTicks, this.itemStack.itemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		this.distractionStrength = EffectManager.GetValue(PassiveEffects.DistractionStrength, this.itemStack.itemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public new void FixedUpdate()
	{
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x000EB000 File Offset: 0x000E9200
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (!this.bMeshCreated)
		{
			this.createMesh();
		}
		if (this.itemWorldData != null)
		{
			this.itemClass.OnDroppedUpdate(this.itemWorldData);
		}
		if (Utils.FastAbs(this.position.y - this.prevPos.y) < 0.1f)
		{
			this.onGroundCounter++;
			if (this.onGroundCounter > 10)
			{
				this.onGround = true;
			}
		}
		if (this.isPhysicsMaster && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && (this.ticksExisted & 1) != 0)
		{
			base.PhysicsMasterSendToServer(base.transform);
		}
		this.checkGravitySetting(this.isPhysicsMaster);
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.itemTransform)
		{
			this.lifetime = 0f;
		}
		this.lifetime -= 0.05f;
		if (this.lifetime <= 0f)
		{
			this.SetDead();
		}
		if (this.itemClass != null && this.itemClass.IsEatDistraction && this.distractionLifetime > 0 && this.distractionEatTicks <= 0)
		{
			this.SetDead();
		}
		if (base.transform.position.y + Origin.position.y < 0f)
		{
			this.SetDead();
		}
		if (!this.IsDead())
		{
			this.tickDistraction();
		}
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000EB15B File Offset: 0x000E935B
	[PublicizedFrom(EAccessModifier.Private)]
	public void playThrowSound(string _name)
	{
		Manager.Play(this, "throw" + _name, 1f, false);
		Manager.Play(this, "throwdefault", 1f, false);
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x000EB187 File Offset: 0x000E9387
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale = 1f)
	{
		if (_strength >= 99999)
		{
			this.lifetime = 0f;
		}
		return base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000EB1A8 File Offset: 0x000E93A8
	public override void OnDamagedByExplosion()
	{
		if (this.itemWorldData != null)
		{
			ItemClass forId = ItemClass.GetForId(this.itemStack.itemValue.type);
			if (forId != null)
			{
				forId.OnDamagedByExplosion(this.itemWorldData);
			}
		}
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000EB1E4 File Offset: 0x000E93E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void createMesh()
	{
		if (this.itemStack.itemValue.type == 0 || this.itemClass == null)
		{
			Log.Error(string.Format("Could not create item with id {0}", this.itemStack.itemValue.type));
			this.SetDead();
			return;
		}
		if (this.meshGameObject != null && this.lastCachedItemStack.itemValue.type != 0)
		{
			UnityEngine.Object.Destroy(this.meshGameObject);
			this.meshGameObject = null;
		}
		this.itemTransform = null;
		float num = 0f;
		Vector3 zero = Vector3.zero;
		if (this.itemClass.IsBlock())
		{
			BlockValue blockValue = this.itemStack.itemValue.ToBlockValue(false);
			if (this.itemTransform == null)
			{
				this.itemTransform = this.itemClass.CloneModel(this.meshGameObject, this.world, blockValue, null, Vector3.zero, base.transform, BlockShape.MeshPurpose.Drop, default(TextureFullArray));
			}
			Block block = blockValue.Block;
			if (block.Properties.Values.ContainsKey("DropScale"))
			{
				num = StringParsers.ParseFloat(block.Properties.Values["DropScale"], 0, -1, NumberStyles.Any);
			}
		}
		else
		{
			if (this.itemTransform == null)
			{
				this.itemTransform = this.itemClass.CloneModel(this.world, this.itemStack.itemValue, Vector3.zero, base.transform, BlockShape.MeshPurpose.Drop, default(TextureFullArray));
			}
			if (this.itemClass.Properties.Values.ContainsKey("DropScale"))
			{
				num = StringParsers.ParseFloat(this.itemClass.Properties.Values["DropScale"], 0, -1, NumberStyles.Any);
			}
		}
		if (num != 0f)
		{
			this.itemTransform.localScale = new Vector3(num, num, num);
		}
		this.itemTransform.localEulerAngles = this.itemClass.GetDroppedCorrectionRotation();
		this.itemTransform.localPosition = zero;
		bool enabled = true;
		Collider[] componentsInChildren = this.itemTransform.GetComponentsInChildren<Collider>();
		int num2 = 0;
		while (componentsInChildren != null && num2 < componentsInChildren.Length)
		{
			Collider collider = componentsInChildren[num2];
			Rigidbody component = collider.gameObject.GetComponent<Rigidbody>();
			if ((component && component.isKinematic) || (collider is MeshCollider && !((MeshCollider)collider).convex))
			{
				collider.enabled = false;
			}
			else
			{
				collider.gameObject.layer = 13;
				collider.enabled = true;
				enabled = false;
				collider.gameObject.AddMissingComponent<RootTransformRefEntity>();
			}
			num2++;
		}
		base.transform.GetComponent<Collider>().enabled = enabled;
		this.meshGameObject = this.itemTransform.gameObject;
		this.meshGameObject.SetActive(true);
		if (this.itemWorldData != null)
		{
			this.itemClass.OnMeshCreated(this.itemWorldData);
		}
		this.bMeshCreated = true;
		this.meshRenderers = this.itemTransform.GetComponentsInChildren<Renderer>(true);
		this.VisiblityCheck(0f, false);
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x000EB4F0 File Offset: 0x000E96F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkGravitySetting(bool hasGravity)
	{
		bool flag = hasGravity && !this.stickT;
		if (this.useGravity != flag)
		{
			this.useGravity = flag;
			Rigidbody rigidbody = this.itemRB;
			if (flag)
			{
				rigidbody.useGravity = true;
				rigidbody.isKinematic = false;
				rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				return;
			}
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
		}
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x000EB558 File Offset: 0x000E9758
	public override void AddVelocity(Vector3 _vel)
	{
		this.bWasThrown = true;
		base.SetAirBorne(true);
		if (!this.bMeshCreated)
		{
			this.createMesh();
		}
		this.checkGravitySetting(this.isPhysicsMaster);
		if (this.isPhysicsMaster)
		{
			this.itemRB.angularVelocity = this.rand.RandomOnUnitSphere * (1f + _vel.magnitude * this.rand.RandomFloat * 8f);
			this.itemRB.AddForce(_vel * 6f, ForceMode.Impulse);
		}
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x000EB5E8 File Offset: 0x000E97E8
	public override void VisiblityCheck(float _distanceSqr, bool _masterIsZooming)
	{
		if (this.itemTransform && this.meshRenderers != null)
		{
			bool enabled = _distanceSqr < (float)(_masterIsZooming ? 8100 : 3600);
			if (this.ticksExisted < 3 && _distanceSqr < 0.64000005f)
			{
				enabled = false;
			}
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].enabled = enabled;
			}
		}
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool CanCollideWith(Entity _other)
	{
		return true;
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x000EB652 File Offset: 0x000E9852
	public virtual bool CanCollect()
	{
		return this.itemClass != null && this.itemClass.CanCollect(this.itemStack.itemValue);
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x000EB674 File Offset: 0x000E9874
	public override void OnCollectServer(int _playerId)
	{
		this.world.RemoveEntity(this.entityId, EnumRemoveEntityReason.Killed);
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x000EB68C File Offset: 0x000E988C
	public override void OnCollectLocal(int _playerId)
	{
		EntityPlayerLocal entityPlayerLocal = this.world.GetEntity(_playerId) as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		bool flag = entityPlayerLocal.inventory.IsHoldingItemActionRunning();
		int num = flag ? uiforPlayer.xui.PlayerInventory.CountAvailableSpaceForItem(this.itemStack.itemValue, false) : -1;
		if ((!flag || num - this.itemStack.itemValue.ItemClass.MaxCount > this.itemStack.count) && uiforPlayer.xui.PlayerInventory.AddItem(this.itemStack))
		{
			return;
		}
		GameManager.Instance.ItemDropServer(this.itemStack, base.GetPosition(), Vector3.zero, _playerId, 60f, false);
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x000EB740 File Offset: 0x000E9940
	public override void OnLoadedFromEntityCache(EntityCreationData _ecd)
	{
		this.markedForUnload = false;
		base.transform.name = "Item_" + _ecd.id.ToString();
		this.SetItemStack(_ecd.itemStack);
		this.bMeshCreated = false;
		if (this.meshRenderers != null)
		{
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].enabled = false;
			}
		}
		UpdateLightOnChunkMesh component;
		if (this.meshGameObject != null && (component = this.meshGameObject.GetComponent<UpdateLightOnChunkMesh>()) != null)
		{
			component.Reset();
		}
		this.itemWorldData = null;
		this.bDead = false;
		this.motion = Vector3.zero;
		this.addedToChunk = false;
		this.fallDistance = 0f;
	}

	// Token: 0x06002665 RID: 9829 RVA: 0x000EB806 File Offset: 0x000E9A06
	public override Transform GetModelTransform()
	{
		return this.itemTransform;
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x000EB80E File Offset: 0x000E9A0E
	public override void PhysicsMasterBecome()
	{
		this.checkGravitySetting(true);
		base.PhysicsMasterBecome();
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000EB820 File Offset: 0x000E9A20
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		if (!this.isPhysicsMaster)
		{
			float deltaTime = Time.deltaTime;
			base.transform.position = Vector3.Lerp(base.transform.position, this.position - Origin.position, deltaTime * 7f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.qrotation, deltaTime * 3f);
			return;
		}
		Vector3 position = base.transform.position;
		if (!float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z) && !float.IsInfinity(position.x) && !float.IsInfinity(position.y) && !float.IsInfinity(position.z))
		{
			this.SetPosition(position + Origin.position, true);
		}
		Quaternion rotation = base.transform.rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		if (!float.IsNaN(eulerAngles.x) && !float.IsNaN(eulerAngles.y) && !float.IsNaN(eulerAngles.z) && !float.IsInfinity(eulerAngles.x) && !float.IsInfinity(eulerAngles.y) && !float.IsInfinity(eulerAngles.z))
		{
			this.SetRotation(eulerAngles);
			this.qrotation = rotation;
		}
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000EB974 File Offset: 0x000E9B74
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.stickT)
		{
			Vector3 position = this.stickT.TransformPoint(this.stickRelativePos);
			base.transform.position = position;
			base.transform.rotation = this.stickT.rotation * this.stickRot;
		}
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x000EB9D0 File Offset: 0x000E9BD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void tickDistraction()
	{
		if (this.itemClass == null || this.distractionLifetime <= 0)
		{
			return;
		}
		if ((this.isCollided || !this.itemClass.IsRequireContactDistraction) && this.distractionRadiusSq > 0f)
		{
			int num = this.nextDistractionTick + 1;
			this.nextDistractionTick = num;
			if (num > 20)
			{
				this.nextDistractionTick = 0;
				Vector3 position = this.position;
				Bounds bb = new Bounds(position, new Vector3(this.distractionRadiusSq, this.distractionRadiusSq, this.distractionRadiusSq));
				this.world.GetEntitiesInBounds(typeof(EntityAlive), bb, EntityItem.distractionTargets);
				for (int i = 0; i < EntityItem.distractionTargets.Count; i++)
				{
					EntityAlive entityAlive = (EntityAlive)EntityItem.distractionTargets[i];
					if (!entityAlive.IsSleeping && entityAlive.distraction == null)
					{
						EntityClass entityClass = EntityClass.list[entityAlive.entityClass];
						if (this.itemClass.DistractionTags.IsEmpty || this.itemClass.DistractionTags.Test_AnySet(entityClass.Tags))
						{
							float distanceSq = base.GetDistanceSq(entityAlive);
							if (distanceSq <= this.distractionRadiusSq && (entityAlive.pendingDistraction == null || distanceSq < entityAlive.pendingDistractionDistanceSq))
							{
								float num2 = entityAlive.distractionResistance - this.distractionStrength;
								if (num2 <= 0f || num2 < this.rand.RandomFloat * 100f)
								{
									entityAlive.pendingDistraction = this;
									entityAlive.pendingDistractionDistanceSq = distanceSq;
								}
							}
						}
					}
				}
				EntityItem.distractionTargets.Clear();
				if (this.distractionLifetime > 0)
				{
					this.distractionLifetime--;
				}
			}
		}
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x000EBB90 File Offset: 0x000E9D90
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnCollisionEnter(Collision collision)
	{
		if (!this.CanCollide(collision))
		{
			return;
		}
		this.CheckStick(collision);
		if (!this.isCollided)
		{
			this.isCollided = true;
			if (this.isPhysicsMaster && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				base.PhysicsMasterSendToServer(base.transform);
			}
		}
		if (this.contactPoints == null)
		{
			this.contactPoints = new List<ContactPoint>(2);
		}
		for (int i = collision.GetContacts(this.contactPoints) - 1; i >= 0; i--)
		{
			ContactPoint contactPoint = this.contactPoints[i];
			if (Utils.FastAbs(Vector3.Dot(collision.relativeVelocity, contactPoint.normal)) >= 1f)
			{
				Entity hitRootEntity = GameUtils.GetHitRootEntity(contactPoint.otherCollider.transform.tag, contactPoint.otherCollider.transform);
				if (hitRootEntity != null)
				{
					string value = EntityClass.list[hitRootEntity.entityClass].Properties.GetValue("SurfaceCategory");
					if (!string.IsNullOrEmpty(value) && this.itemClass != null && this.itemClass.MadeOfMaterial != null)
					{
						this.playThrowSound(this.itemClass.MadeOfMaterial.id + "hit" + value);
						return;
					}
					break;
				}
				else
				{
					Vector3i pos = World.worldToBlockPos(contactPoint.point - 0.25f * contactPoint.normal + Origin.position);
					BlockValue block = this.world.GetBlock(pos);
					if (block.isair)
					{
						WorldRayHitInfo worldRayHitInfo = new WorldRayHitInfo();
						GameUtils.FindMasterBlockForEntityModelBlock(this.world, -contactPoint.normal, contactPoint.otherCollider.transform.tag, contactPoint.point + Origin.position, contactPoint.otherCollider.transform, worldRayHitInfo);
						pos = worldRayHitInfo.hit.blockPos;
						block = this.world.GetBlock(pos);
						if (block.isair)
						{
							goto IL_2FB;
						}
					}
					float num = Utils.FastAbs(contactPoint.normal.x);
					float num2 = Utils.FastAbs(contactPoint.normal.y);
					float num3 = Utils.FastAbs(contactPoint.normal.z);
					BlockFace side = BlockFace.Top;
					if (num >= num2 && num >= num3)
					{
						if (contactPoint.normal.x < 0f)
						{
							side = BlockFace.East;
						}
						else if (contactPoint.normal.x > 0f)
						{
							side = BlockFace.West;
						}
					}
					else if (num3 >= num && num3 >= num2)
					{
						if (contactPoint.normal.z < 0f)
						{
							side = BlockFace.North;
						}
						else if (contactPoint.normal.z > 0f)
						{
							side = BlockFace.South;
						}
					}
					else if (contactPoint.normal.y < 0f)
					{
						side = BlockFace.Bottom;
					}
					string surfaceCategory = block.Block.GetMaterialForSide(block, side).SurfaceCategory;
					if (this.itemClass != null && this.itemClass.MadeOfMaterial != null)
					{
						this.playThrowSound(this.itemClass.MadeOfMaterial.id + "hit" + surfaceCategory);
						return;
					}
					break;
				}
			}
			IL_2FB:;
		}
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x000EBEA4 File Offset: 0x000EA0A4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CanCollide(Collision collision)
	{
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		if (!this.bWasThrown && this.itemClass is ItemClassTimeBomb)
		{
			return false;
		}
		Transform transform = collision.transform;
		if (!transform)
		{
			return false;
		}
		string tag = transform.tag;
		if (tag != null && tag.StartsWith("E_"))
		{
			Transform hitRootTransform = GameUtils.GetHitRootTransform(tag, transform);
			if (hitRootTransform != null)
			{
				Entity component = hitRootTransform.GetComponent<Entity>();
				if (component != null && component.entityId == this.belongsPlayerId)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000EBF30 File Offset: 0x000EA130
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckStick(Collision collision)
	{
		if (this.stickPercent <= 0f)
		{
			return;
		}
		float d = 1f - this.stickPercent;
		this.itemRB.velocity *= d;
		this.itemRB.angularVelocity *= d;
		if (this.stickPercent >= 1f && !this.stickT)
		{
			this.stickT = collision.transform;
			this.stickRelativePos = this.stickT.InverseTransformPoint(base.transform.position);
			this.stickRot = Quaternion.Inverse(this.stickT.rotation) * base.transform.rotation;
			Collider[] componentsInChildren = this.itemRB.GetComponentsInChildren<Collider>();
			for (int i = componentsInChildren.Length - 1; i >= 0; i--)
			{
				componentsInChildren[i].gameObject.layer = 0;
			}
			this.checkGravitySetting(this.isPhysicsMaster);
			this.PlayOneShot(this.itemClass.SoundStick, false, false, false, null, 1f);
		}
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000EC044 File Offset: 0x000EA244
	public override string ToString()
	{
		if (this.itemStack.itemValue.HasQuality)
		{
			return string.Format("[type={0}, name={1}, cnt={2}, quality={3}]", new object[]
			{
				base.GetType().Name,
				this.itemClass.Name,
				this.itemStack.count,
				this.itemStack.itemValue.Quality
			});
		}
		return string.Format("[type={0}, name={1}, cnt={2}]", base.GetType().Name, (this.itemClass != null) ? this.itemClass.Name : string.Empty, this.itemStack.count);
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsQRotationUsed()
	{
		return true;
	}

	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x0600266F RID: 9839 RVA: 0x000EC0FA File Offset: 0x000EA2FA
	public bool IsDistractionActive
	{
		get
		{
			return this.distractionLifetime > 0;
		}
	}

	// Token: 0x04001C78 RID: 7288
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Rigidbody itemRB;

	// Token: 0x04001C79 RID: 7289
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool useGravity;

	// Token: 0x04001C7A RID: 7290
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Renderer[] meshRenderers;

	// Token: 0x04001C7B RID: 7291
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject meshGameObject;

	// Token: 0x04001C7C RID: 7292
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform itemTransform;

	// Token: 0x04001C7D RID: 7293
	public ItemStack itemStack = ItemStack.Empty;

	// Token: 0x04001C7E RID: 7294
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemStack lastCachedItemStack = ItemStack.Empty;

	// Token: 0x04001C7F RID: 7295
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bMeshCreated;

	// Token: 0x04001C80 RID: 7296
	public ItemClass itemClass;

	// Token: 0x04001C81 RID: 7297
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stickPercent;

	// Token: 0x04001C82 RID: 7298
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform stickT;

	// Token: 0x04001C83 RID: 7299
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 stickRelativePos;

	// Token: 0x04001C84 RID: 7300
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion stickRot;

	// Token: 0x04001C85 RID: 7301
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemWorldData itemWorldData;

	// Token: 0x04001C86 RID: 7302
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bWasThrown;

	// Token: 0x04001C87 RID: 7303
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int onGroundCounter;

	// Token: 0x04001C88 RID: 7304
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int distractionLifetime;

	// Token: 0x04001C89 RID: 7305
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float distractionStrength;

	// Token: 0x04001C8A RID: 7306
	public int distractionEatTicks;

	// Token: 0x04001C8B RID: 7307
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int nextDistractionTick;

	// Token: 0x04001C8C RID: 7308
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float distractionRadiusSq;

	// Token: 0x04001C8D RID: 7309
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Entity> distractionTargets = new List<Entity>();

	// Token: 0x04001C8E RID: 7310
	public int OwnerId = -1;

	// Token: 0x04001C8F RID: 7311
	public static int ItemInstanceCount;

	// Token: 0x04001C90 RID: 7312
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<ContactPoint> contactPoints;
}
