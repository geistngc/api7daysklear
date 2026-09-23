using System;
using System.IO;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004E0 RID: 1248
[Preserve]
public class EntityTurret : EntityAlive
{
	// Token: 0x17000484 RID: 1156
	// (get) Token: 0x0600289C RID: 10396 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsValidAimAssistSnapTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000485 RID: 1157
	// (get) Token: 0x0600289D RID: 10397 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsValidAimAssistSlowdownTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000486 RID: 1158
	// (get) Token: 0x0600289E RID: 10398 RVA: 0x000EC766 File Offset: 0x000EA966
	public override string LocalizedEntityName
	{
		get
		{
			return Localization.Get(this.EntityName, false, null);
		}
	}

	// Token: 0x17000487 RID: 1159
	// (get) Token: 0x0600289F RID: 10399 RVA: 0x000FC7EF File Offset: 0x000FA9EF
	// (set) Token: 0x060028A0 RID: 10400 RVA: 0x000FC7FC File Offset: 0x000FA9FC
	public int AmmoCount
	{
		get
		{
			return this.OriginalItemValue.Meta;
		}
		set
		{
			this.OriginalItemValue.Meta = value;
		}
	}

	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x060028A1 RID: 10401 RVA: 0x000FC80A File Offset: 0x000FAA0A
	public bool IsTurning
	{
		get
		{
			return this.IsOn && (this.YawController.IsTurning || this.PitchController.IsTurning);
		}
	}

	// Token: 0x17000489 RID: 1161
	// (get) Token: 0x060028A2 RID: 10402 RVA: 0x000FC830 File Offset: 0x000FAA30
	// (set) Token: 0x060028A3 RID: 10403 RVA: 0x000FC855 File Offset: 0x000FAA55
	public override int Health
	{
		get
		{
			return (int)Mathf.Max((float)this.OriginalItemValue.MaxUseTimes - this.OriginalItemValue.UseTimes, 1f);
		}
		set
		{
			this.OriginalItemValue.UseTimes = (float)(this.OriginalItemValue.MaxUseTimes - value);
		}
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000FC870 File Offset: 0x000FAA70
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
		EntityClass entityClass = EntityClass.list[this.entityClass];
		base.transform.tag = "E_Vehicle";
		Transform transform = base.transform;
		this.thisRigidBody = transform.GetComponent<Rigidbody>();
		if (this.thisRigidBody)
		{
			this.thisRigidBody.centerOfMass = new Vector3(0f, 0.1f, 0f);
			this.thisRigidBody.sleepThreshold = this.thisRigidBody.mass * 0.01f * 0.01f * 0.5f;
			transform.gameObject.AddComponent<CollisionCallForward>().Entity = this;
			transform.gameObject.layer = 21;
			Utils.SetTagsRecursively(transform, "E_Vehicle");
		}
		this.alertEnabled = false;
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000FC93E File Offset: 0x000FAB3E
	public override void Kill(DamageResponse _dmResponse)
	{
		_dmResponse.Fatal = false;
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x000027FC File Offset: 0x000009FC
	public override void SetDead()
	{
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000FC93E File Offset: 0x000FAB3E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ClientKill(DamageResponse _dmResponse)
	{
		_dmResponse.Fatal = false;
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x000FC948 File Offset: 0x000FAB48
	[PublicizedFrom(EAccessModifier.Protected)]
	public override DamageResponse damageEntityLocal(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
	{
		DamageResponse result = base.damageEntityLocal(_damageSource, _strength, _criticalHit, impulseScale);
		result.Fatal = false;
		return result;
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x000FC96C File Offset: 0x000FAB6C
	public override void OnEntityUnload()
	{
		base.OnEntityUnload();
		this.IsOn = false;
		if (GameManager.Instance != null && GameManager.Instance.World != null && this.belongsPlayerId != -1)
		{
			EntityAlive entityAlive = (EntityAlive)GameManager.Instance.World.GetEntity(this.belongsPlayerId);
			if (entityAlive != null)
			{
				entityAlive.RemoveOwnedEntity(this.entityId);
			}
		}
		this.FireController.Update();
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddCharacterController()
	{
	}

	// Token: 0x060028AB RID: 10411 RVA: 0x000FC9E4 File Offset: 0x000FABE4
	public override void PostInit()
	{
		Transform transform = base.transform;
		transform.rotation = this.qrotation;
		this.StaticPosition = this.position;
		this.fallPos = this.position;
		this.YawController = transform.GetComponentInChildren<AutoTurretYawLerp>();
		this.PitchController = transform.GetComponentInChildren<AutoTurretPitchLerp>();
		this.FireController = transform.GetComponentInChildren<MiniTurretFireController>();
		this.Laser = transform.FindInChilds("turret_laser", false);
		this.Cone = transform.FindInChilds("turret_cone", false);
		PersistentPlayerData playerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(this.OwnerID);
		if (playerData != null)
		{
			this.belongsPlayerId = playerData.EntityId;
		}
		this.HandleNavObject();
		this.InitTurret();
	}

	// Token: 0x060028AC RID: 10412 RVA: 0x000FCA95 File Offset: 0x000FAC95
	public override void InitInventory()
	{
		this.inventory = new EntityTurret.TurretInventory(GameManager.Instance, this);
	}

	// Token: 0x060028AD RID: 10413 RVA: 0x000FCAA8 File Offset: 0x000FACA8
	public void InitTurret()
	{
		this.FireController.Init(base.EntityClass.Properties, this);
	}

	// Token: 0x060028AE RID: 10414 RVA: 0x000FCAC4 File Offset: 0x000FACC4
	public override void OnAddedToWorld()
	{
		base.OnAddedToWorld();
		if (this.OriginalItemValue.HasMods())
		{
			for (int i = 0; i < this.OriginalItemValue.Modifications.Length; i++)
			{
				ItemValue itemValue = this.OriginalItemValue.Modifications[i];
				if (((itemValue != null) ? itemValue.ItemClass : null) != null && itemValue.ItemClass.Name == "modMeleeClubBurningShaft")
				{
					Transform transform = base.transform.FindInChilds("mod_junk_sledge_flamePrefab", false);
					if (transform)
					{
						transform.gameObject.SetActive(true);
					}
				}
			}
		}
	}

	// Token: 0x060028AF RID: 10415 RVA: 0x000FCB58 File Offset: 0x000FAD58
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (this.belongsPlayerId == -1)
		{
			PersistentPlayerList persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
			if (persistentPlayerList != null)
			{
				PersistentPlayerData playerData = persistentPlayerList.GetPlayerData(this.OwnerID);
				if (playerData != null)
				{
					this.belongsPlayerId = playerData.EntityId;
				}
			}
		}
		if (!this.Owner)
		{
			this.Owner = (EntityAlive)GameManager.Instance.World.GetEntity(this.belongsPlayerId);
			if (this.Owner != null)
			{
				this.Owner.AddOwnedEntity(this);
			}
		}
		if (this.uloam == null && this.OriginalItemValue.ItemClass != null)
		{
			this.uloam = base.gameObject.AddMissingComponent<UpdateLightOnAllMaterials>();
			this.uloam.AddRendererNameToIgnore("turret_laser");
			this.uloam.SetTintColorForItem(Vector3.one);
			if (this.OriginalItemValue.ItemClass.Properties.Values.ContainsKey(Block.PropTintColor))
			{
				this.uloam.SetTintColorForItem(Block.StringToVector3(this.OriginalItemValue.GetPropertyOverride(Block.PropTintColor, this.OriginalItemValue.ItemClass.Properties.Values[Block.PropTintColor])));
			}
			else
			{
				this.uloam.SetTintColorForItem(Block.StringToVector3(this.OriginalItemValue.GetPropertyOverride(Block.PropTintColor, "255,255,255")));
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.IsOn = (this.OriginalItemValue.PercentUsesLeft > 0f);
			if ((int)EffectManager.GetValue(PassiveEffects.MagazineSize, this.OriginalItemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0)
			{
				this.IsOn &= (this.OriginalItemValue.Meta > 0);
			}
			if (GameManager.Instance != null && GameManager.Instance.World != null && this.belongsPlayerId != -1)
			{
				this.IsOn &= (this.Owner != null);
				if (this.Owner != null)
				{
					if (EffectManager.GetValue(PassiveEffects.DisableItem, this.OriginalItemValue, 0f, this.Owner, null, this.OriginalItemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false) > 0f)
					{
						this.IsOn = false;
					}
					else
					{
						this.maxOwnerDistance = (int)EffectManager.GetValue(PassiveEffects.JunkTurretActiveRange, this.OriginalItemValue, 10f, this.Owner, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
						if (this.IsOn)
						{
							this.DistanceToOwner = base.GetDistanceSq(this.Owner);
							this.IsOn &= (this.DistanceToOwner < (float)(this.maxOwnerDistance * this.maxOwnerDistance));
						}
						if (this.IsOn)
						{
							int num = (int)EffectManager.GetValue(PassiveEffects.JunkTurretActiveCount, this.OriginalItemValue, 1f, this.Owner, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
							int num2 = 0;
							for (int i = 0; i < this.Owner.ownedEntities.Count; i++)
							{
								EntityTurret entityTurret = GameManager.Instance.World.GetEntity(this.Owner.ownedEntities[i].Id) as EntityTurret;
								if (!(entityTurret == null) && entityTurret.entityId != this.entityId)
								{
									if (entityTurret.IsOn)
									{
										num2++;
									}
									this.IsOn &= (num2 <= num || this.DistanceToOwner < entityTurret.DistanceToOwner || this.ForceOn);
									if (!this.IsOn)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
			else if (this.IsOn)
			{
				this.IsOn &= (this.belongsPlayerId == -1 && this.OwnerID == null);
			}
			this.ForceOn = false;
			if (this.TargetEntityId != this.lastTargetEntityId || this.IsOn != this.lastIsOn || this.OriginalItemValue.Equals(this.lastOriginalItemValue))
			{
				this.lastOriginalItemValue = this.OriginalItemValue.Clone();
				this.lastTargetEntityId = this.TargetEntityId;
				this.lastIsOn = this.IsOn;
				NetPackageTurretSync package = NetPackageManager.GetPackage<NetPackageTurretSync>().Setup(this.entityId, this.TargetEntityId, this.IsOn, this.OriginalItemValue);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, true, -1, -1, -1, null, 192, false);
			}
		}
		if (this.Laser != null && this.IsOn != this.Laser.gameObject.activeSelf)
		{
			this.Laser.gameObject.SetActive(this.IsOn);
		}
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x000027FC File Offset: 0x000009FC
	public override void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x000FD024 File Offset: 0x000FB224
	public void InitDynamicSpawn()
	{
		for (int i = 1; i < ItemClass.list.Length - 1; i++)
		{
			if (ItemClass.list[i] != null)
			{
				string name = ItemClass.list[i].Name;
				if (name == "gunBotT1JunkSledge" || name == "gunBotT2JunkTurret")
				{
					this.OwnerID = PlatformManager.InternalLocalUserIdentifier;
					this.OriginalItemValue = new ItemValue(ItemClass.list[i].Id, false);
					this.AmmoCount = ItemClass.GetForId(ItemClass.list[i].Id).GetInitialMetadata(this.OriginalItemValue);
					this.ForceOn = true;
					PersistentPlayerData playerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(this.OwnerID);
					if (playerData != null)
					{
						(GameManager.Instance.World.GetEntity(playerData.EntityId) as EntityAlive).AddOwnedEntity(this);
					}
				}
			}
		}
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x000FD108 File Offset: 0x000FB308
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		Vector3 vector = base.transform.position + Origin.position;
		this.position = vector;
		this.StaticPosition = vector;
		Chunk chunk = GameManager.Instance.World.GetChunkFromWorldPos((int)vector.x, (int)vector.y, (int)vector.z) as Chunk;
		if (chunk != null && chunk.IsCollisionMeshGenerated)
		{
			if (this.posCheckTimer <= 0f)
			{
				this.posCheckTimer = 0.5f;
				int modelLayer = base.GetModelLayer();
				this.SetModelLayer(2, false, null);
				float y = this.fallPos.y;
				this.fallPos = vector;
				Ray ray = new Ray(vector + Vector3.up * 0.375f, Vector3.down);
				if (Voxel.Raycast(GameManager.Instance.World, ray, 255f, 1082195969, 128, 0.25f))
				{
					this.groundUpDirection = Voxel.phyxRaycastHit.normal;
					this.fallPos.y = Voxel.voxelRayHitInfo.fmcHit.pos.y;
					if (Vector3.Dot(Vector3.up, this.groundUpDirection) < 0.7f)
					{
						this.fallPos.y = this.fallPos.y - 0.1f;
					}
					if (this.fallPos.y < y)
					{
						this.fallDelay = 5;
					}
				}
				this.SetModelLayer(modelLayer, false, null);
			}
			float deltaTime = Time.deltaTime;
			this.posCheckTimer -= deltaTime;
			this.isFalling = false;
			if (vector != this.fallPos)
			{
				this.posCheckTimer = 0f;
				int num = this.fallDelay - 1;
				this.fallDelay = num;
				if (num < 0)
				{
					this.isFalling = true;
					base.transform.position = Vector3.MoveTowards(base.transform.position, this.fallPos - Origin.position, 5f * deltaTime);
					return;
				}
			}
		}
		else
		{
			this.posCheckTimer = 0.5f;
		}
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x000FD306 File Offset: 0x000FB506
	public override void OnCollectServer(int _playerId)
	{
		this.OriginalItemValue = ItemValue.None;
		this.PickedUpWaitingToDelete = true;
		this.bPlayerStatsChanged = true;
		base.transform.gameObject.SetActive(false);
		this.world.RemoveEntity(this.entityId, EnumRemoveEntityReason.Killed);
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x000FD348 File Offset: 0x000FB548
	public override void OnCollectLocal(int _playerId)
	{
		EntityPlayerLocal entityPlayerLocal = this.world.GetEntity(_playerId) as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		ItemStack itemStack = new ItemStack(this.OriginalItemValue, 1);
		if (!uiforPlayer.xui.PlayerInventory.AddItem(itemStack))
		{
			GameManager.Instance.ItemDropServer(itemStack, entityPlayerLocal.GetPosition(), Vector3.zero, _playerId, 60f, false);
		}
	}

	// Token: 0x060028B5 RID: 10421 RVA: 0x000FD3A9 File Offset: 0x000FB5A9
	public bool CanInteract(int _interactingEntityId)
	{
		return !this.isFalling && !this.PickedUpWaitingToDelete && this.OriginalItemValue.type != 0 && (this.belongsPlayerId == _interactingEntityId || this.Health <= 1);
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x000FD3E4 File Offset: 0x000FB5E4
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
		return string.Format(Localization.Get("turretPickUp", false, null), arg, this.LocalizedEntityName);
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x000FD464 File Offset: 0x000FB664
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitLocalActivationCommands(Action<EntityActivationCommand> _addCallback)
	{
		_addCallback(new EntityActivationCommand("take", "hand", null, null));
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x000FD47D File Offset: 0x000FB67D
	public override bool AllowActivationCommand(ReadOnlySpan<char> _commandName, EntityPlayerLocal _playerFocusing)
	{
		if (base.CommandIs(_commandName, "take"))
		{
			return this.CanInteract(_playerFocusing.entityId);
		}
		return base.AllowActivationCommand(_commandName, _playerFocusing);
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x000FD4A4 File Offset: 0x000FB6A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityActivated(EntityActivationCommand _command, EntityPlayerLocal _playerFocusing)
	{
		if (base.CommandIs(_command.commandId, "take"))
		{
			ItemStack itemStack = new ItemStack(this.OriginalItemValue, 1);
			if (_playerFocusing.inventory.CanTakeItem(itemStack) || _playerFocusing.bag.CanTakeItem(itemStack))
			{
				base.Collect(_playerFocusing.entityId);
				return;
			}
			GameManager.ShowTooltip(_playerFocusing, Localization.Get("xuiInventoryFullForPickup", false, null), string.Empty, "ui_denied", null, false, false, 0f);
		}
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsDead()
	{
		return false;
	}

	// Token: 0x060028BB RID: 10427 RVA: 0x000FD523 File Offset: 0x000FB723
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		_bw.Write(1);
		this.OwnerID.ToStream(_bw, false);
		this.OriginalItemValue.Write(_bw);
		StreamUtils.Write(_bw, this.StaticPosition);
	}

	// Token: 0x060028BC RID: 10428 RVA: 0x000FD55C File Offset: 0x000FB75C
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		int num = _br.ReadInt32();
		this.OwnerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		this.OriginalItemValue = ItemValue.None;
		this.OriginalItemValue.Read(_br);
		if (num > 0)
		{
			this.StaticPosition = StreamUtils.ReadVector3(_br);
		}
	}

	// Token: 0x04001E7B RID: 7803
	public const int SaveVersion = 1;

	// Token: 0x04001E7C RID: 7804
	public const string JunkTurretSledgeItem = "gunBotT1JunkSledge";

	// Token: 0x04001E7D RID: 7805
	public const string JunkTurretRangedItem = "gunBotT2JunkTurret";

	// Token: 0x04001E7E RID: 7806
	public AutoTurretYawLerp YawController;

	// Token: 0x04001E7F RID: 7807
	public AutoTurretPitchLerp PitchController;

	// Token: 0x04001E80 RID: 7808
	public MiniTurretFireController FireController;

	// Token: 0x04001E81 RID: 7809
	public Transform Laser;

	// Token: 0x04001E82 RID: 7810
	public Transform Cone;

	// Token: 0x04001E83 RID: 7811
	public Material ConeMaterial;

	// Token: 0x04001E84 RID: 7812
	public Color ConeColor;

	// Token: 0x04001E85 RID: 7813
	public float CenteredYaw;

	// Token: 0x04001E86 RID: 7814
	public float CenteredPitch;

	// Token: 0x04001E87 RID: 7815
	public bool TargetOwner;

	// Token: 0x04001E88 RID: 7816
	public bool TargetAllies;

	// Token: 0x04001E89 RID: 7817
	public bool TargetStrangers = true;

	// Token: 0x04001E8A RID: 7818
	public bool TargetEnemies = true;

	// Token: 0x04001E8B RID: 7819
	public int maxOwnerDistance = 10;

	// Token: 0x04001E8C RID: 7820
	public ItemValue OriginalItemValue = ItemValue.None;

	// Token: 0x04001E8D RID: 7821
	public bool PickedUpWaitingToDelete;

	// Token: 0x04001E8E RID: 7822
	public PlatformUserIdentifierAbs OwnerID;

	// Token: 0x04001E8F RID: 7823
	public bool IsOn;

	// Token: 0x04001E90 RID: 7824
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rigidbody thisRigidBody;

	// Token: 0x04001E91 RID: 7825
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UpdateLightOnAllMaterials uloam;

	// Token: 0x04001E92 RID: 7826
	public EntityAlive Owner;

	// Token: 0x04001E93 RID: 7827
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastTargetEntityId = -2;

	// Token: 0x04001E94 RID: 7828
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lastIsOn;

	// Token: 0x04001E95 RID: 7829
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemValue lastOriginalItemValue = ItemValue.None;

	// Token: 0x04001E96 RID: 7830
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cPOSITION_UPDATE_CHECK_TIME = 0.5f;

	// Token: 0x04001E97 RID: 7831
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float posCheckTimer = 0.5f;

	// Token: 0x04001E98 RID: 7832
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int fallDelay;

	// Token: 0x04001E99 RID: 7833
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 fallPos;

	// Token: 0x04001E9A RID: 7834
	public int TargetEntityId = -1;

	// Token: 0x04001E9B RID: 7835
	public bool ForceOn;

	// Token: 0x04001E9C RID: 7836
	public float DistanceToOwner = float.MaxValue;

	// Token: 0x04001E9D RID: 7837
	public Vector3 groundPosition;

	// Token: 0x04001E9E RID: 7838
	public Vector3 groundUpDirection;

	// Token: 0x04001E9F RID: 7839
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFalling;

	// Token: 0x04001EA0 RID: 7840
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 StaticPosition;

	// Token: 0x04001EA1 RID: 7841
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLerpTimeScale = 8f;

	// Token: 0x04001EA2 RID: 7842
	public int tmpBelongsPlayerID;

	// Token: 0x020004E1 RID: 1249
	public class TurretInventory : Inventory
	{
		// Token: 0x060028BE RID: 10430 RVA: 0x000FD610 File Offset: 0x000FB810
		public TurretInventory(IGameManager _gameManager, EntityAlive _entity) : base(_gameManager, _entity)
		{
			this.cSlotCount = base.PUBLIC_SLOTS + 1;
			this.SetupSlots();
		}

		// Token: 0x060028BF RID: 10431 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Execute(int _actionIdx, bool _bReleased, PlayerActionsLocal _playerActions = null)
		{
		}

		// Token: 0x060028C0 RID: 10432 RVA: 0x000FD62E File Offset: 0x000FB82E
		public void SetupSlots()
		{
			this.slots = new ItemInventoryData[this.cSlotCount];
			this.models = new Transform[this.cSlotCount];
			this.m_HoldingItemIdx = 0;
			base.Clear();
		}

		// Token: 0x060028C1 RID: 10433 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void updateHoldingItem()
		{
		}

		// Token: 0x04001EA3 RID: 7843
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int cSlotCount;
	}
}
