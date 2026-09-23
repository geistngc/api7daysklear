using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using DynamicMusic;
using DynamicMusic.Factories;
using InControl;
using Platform;
using SandboxOptions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

// Token: 0x020004C9 RID: 1225
[UnityEngine.Scripting.Preserve]
public class EntityPlayerLocal : EntityPlayer, IInventoryChangedListener, IGamePrefsChangedListener
{
	// Token: 0x14000026 RID: 38
	// (add) Token: 0x0600273A RID: 10042 RVA: 0x000EF38C File Offset: 0x000ED58C
	// (remove) Token: 0x0600273B RID: 10043 RVA: 0x000EF3C4 File Offset: 0x000ED5C4
	public event Action InventoryChangedEvent;

	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x0600273C RID: 10044 RVA: 0x000EF3F9 File Offset: 0x000ED5F9
	public PlayerActionsLocal playerInput
	{
		get
		{
			return PlatformManager.NativePlatform.Input.PrimaryPlayer;
		}
	}

	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x0600273D RID: 10045 RVA: 0x000EF40A File Offset: 0x000ED60A
	public static bool PermaDegrationOn
	{
		get
		{
			return EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.MaxDurability || EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Both || ItemAction.ItemMaxDegrationAmount > 0f;
		}
	}

	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x0600273E RID: 10046 RVA: 0x000EF42A File Offset: 0x000ED62A
	public Vector3 LookPoint
	{
		get
		{
			return this.vp_FPCamera.LookPoint;
		}
	}

	// Token: 0x0600273F RID: 10047 RVA: 0x000EF438 File Offset: 0x000ED638
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkedGetFPController()
	{
		if (!this.PhysicsTransform)
		{
			return;
		}
		if (!this.m_checked_vp_FPController || this.m_vp_FPController == null)
		{
			this.m_checked_vp_FPController = true;
			this.m_vp_FPController = this.PhysicsTransform.GetComponent<vp_FPController>();
			Transform transform = this.PhysicsTransform.Find("Camera");
			if (transform != null)
			{
				this.m_vp_FPCamera = transform.GetComponent<vp_FPCamera>();
			}
			this.m_vp_FPPlayerEventHandler = this.PhysicsTransform.GetComponent<vp_FPPlayerEventHandler>();
		}
		if (this.m_vp_FPWeapon == null)
		{
			this.m_vp_FPWeapon = this.PhysicsTransform.GetComponentInChildren<vp_FPWeapon>();
			if (this.m_vp_FPWeapon != null && this.emodel is EModelSDCS)
			{
				this.m_vp_FPWeapon.DefaultState.Preset.SetFieldValue("PositionOffset", new Vector3(0f, -1.7f, 0.02f));
				this.m_vp_FPWeapon.DefaultState.Preset.SetFieldValue("RenderingFieldOfView", 45);
			}
		}
	}

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x06002740 RID: 10048 RVA: 0x000EF549 File Offset: 0x000ED749
	public vp_FPController vp_FPController
	{
		get
		{
			this.checkedGetFPController();
			return this.m_vp_FPController;
		}
	}

	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x06002741 RID: 10049 RVA: 0x000EF557 File Offset: 0x000ED757
	public vp_FPCamera vp_FPCamera
	{
		get
		{
			this.checkedGetFPController();
			return this.m_vp_FPCamera;
		}
	}

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x06002742 RID: 10050 RVA: 0x000EF565 File Offset: 0x000ED765
	public vp_FPWeapon vp_FPWeapon
	{
		get
		{
			this.checkedGetFPController();
			return this.m_vp_FPWeapon;
		}
	}

	// Token: 0x1700046B RID: 1131
	// (get) Token: 0x06002743 RID: 10051 RVA: 0x000EF573 File Offset: 0x000ED773
	public vp_PlayerEventHandler vp_PlayerEventHandler
	{
		get
		{
			this.checkedGetFPController();
			return this.m_vp_FPPlayerEventHandler;
		}
	}

	// Token: 0x06002744 RID: 10052 RVA: 0x000EF584 File Offset: 0x000ED784
	public void ShowHoldingItemLayer(bool show)
	{
		if (this.playerCamera)
		{
			if (show)
			{
				this.playerCamera.cullingMask |= 1024;
				return;
			}
			this.playerCamera.cullingMask &= -1025;
		}
	}

	// Token: 0x14000027 RID: 39
	// (add) Token: 0x06002745 RID: 10053 RVA: 0x000EF5D0 File Offset: 0x000ED7D0
	// (remove) Token: 0x06002746 RID: 10054 RVA: 0x000EF608 File Offset: 0x000ED808
	public event Action DragAndDropItemChanged;

	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x06002747 RID: 10055 RVA: 0x000EF63D File Offset: 0x000ED83D
	// (set) Token: 0x06002748 RID: 10056 RVA: 0x000EF645 File Offset: 0x000ED845
	public ItemStack DragAndDropItem
	{
		get
		{
			return this.dragAndDropItem;
		}
		set
		{
			this.dragAndDropItem = value;
			this.DragAndDropItemChanged();
		}
	}

	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x06002749 RID: 10057 RVA: 0x000EF659 File Offset: 0x000ED859
	public PlayerMoveController MoveController
	{
		get
		{
			return this.moveController;
		}
	}

	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x0600274A RID: 10058 RVA: 0x000EF661 File Offset: 0x000ED861
	public LocalPlayerUI PlayerUI
	{
		get
		{
			return this.playerUI;
		}
	}

	// Token: 0x0600274B RID: 10059 RVA: 0x000EF669 File Offset: 0x000ED869
	public void MakeAttached(bool bAttached)
	{
		this.isLadderAttached = bAttached;
	}

	// Token: 0x0600274C RID: 10060 RVA: 0x000EF672 File Offset: 0x000ED872
	public bool IsOnLadder()
	{
		return this.isLadderAttached;
	}

	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x0600274D RID: 10061 RVA: 0x000EF67A File Offset: 0x000ED87A
	// (set) Token: 0x0600274E RID: 10062 RVA: 0x000EF682 File Offset: 0x000ED882
	public bool InAir
	{
		get
		{
			return this.inAir;
		}
		set
		{
			if (this.inAir != value)
			{
				this.inAir = value;
				this.emodel.avatarController.SetInAir(this.inAir);
			}
		}
	}

	// Token: 0x17000470 RID: 1136
	// (get) Token: 0x0600274F RID: 10063 RVA: 0x000EF6AA File Offset: 0x000ED8AA
	// (set) Token: 0x06002750 RID: 10064 RVA: 0x000EF6B2 File Offset: 0x000ED8B2
	public bool CameraRelativeMovement
	{
		get
		{
			return this._cameraRelativeMovement;
		}
		set
		{
			if (this._cameraRelativeMovement != value)
			{
				this._cameraRelativeMovement = value;
				this.vp_PlayerEventHandler.CameraRelativeMovement3P.Set(value);
			}
		}
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000EF6DA File Offset: 0x000ED8DA
	public void StartTPCameraLockTimer()
	{
		if (this.bFirstPersonView)
		{
			this.tpCameraLockTimerActive = false;
			return;
		}
		if (this.vp_FPCamera.Locked3rdPerson)
		{
			return;
		}
		this.CameraRelativeMovement = false;
		this.tpCameraLockStartTime = Time.time;
		this.tpCameraLockTimerActive = true;
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x000EF714 File Offset: 0x000ED914
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		this.playerUI = LocalPlayerUI.GetUIForPlayer(this);
		this.windowManager = this.playerUI.windowManager;
		this.nguiWindowManager = this.playerUI.nguiWindowManager;
		this.QuestJournal.OwnerPlayer = this;
		this.challengeJournal = new ChallengeJournal();
		this.challengeJournal.Player = this;
		base.Awake();
		this.dragAndDropItem = ItemStack.Empty;
		this.isEntityRemote = false;
		this.world.AddLocalPlayer(this);
		this.cameraContainerTransform = base.transform;
		this.cameraTransform = this.cameraContainerTransform.Find("Camera");
		this.playerCamera = this.cameraTransform.GetComponent<Camera>();
		this.finalCamera = this.playerCamera;
		this.ScreenEffectManager = this.cameraTransform.gameObject.AddMissingComponent<ScreenEffects>();
		Transform transform = this.cameraTransform.Find("ScreenEffectsWithDepth");
		if (transform != null)
		{
			this.uwEffectHaze = transform.Find("UnderwaterHaze");
		}
		this.uwEffectRefract = this.cameraTransform.Find("effect_refract_plane");
		this.uwEffectDebris = this.cameraTransform.Find("effect_underwater_debris");
		this.uwEffectDroplets = this.cameraTransform.Find("effect_dropletsParticle");
		this.uwEffectWaterFade = this.cameraTransform.Find("effect_water_fade");
		this.audioSourceBiomeActive = this.cameraTransform.gameObject.AddComponent<AudioSource>();
		this.audioSourceBiomeFadeOut = this.cameraTransform.gameObject.AddComponent<AudioSource>();
		this.overlayMaterial = new Material(Shader.Find("Game/UI/Screen Overlay"));
		this.CameraDOFInit();
		Shader.SetGlobalFloat("_UnderWater", 0f);
		this.renderManager = GameRenderManager.Create(this);
		this.bPlayingSpawnIn = true;
		this.spawnInTime = Time.time;
		SkyManager.SetFogDebug(-1f, float.MinValue, float.MinValue);
		WeatherManager.Instance.PushTransitions();
		this.ThreatLevel = Factory.CreateThreatLevel();
		this.ScreenEffectManager.SetScreenEffect("VibrantDeSat", 1f, 4f);
		GameManager.Instance.triggerEffectManager.EnableVibration();
		this.moveController = base.GetComponent<PlayerMoveController>();
		this.MoveController.Init();
		MumblePositionalAudio instance = SingletonMonoBehaviour<MumblePositionalAudio>.Instance;
		if (instance != null)
		{
			instance.SetPlayer(this);
		}
		this.lastWaypointUpdateTime = Time.time - 30f;
		this.characterMatrixOverride = base.transform.Find("Graphics/Model").gameObject.AddComponent<CharacterMatrixOverride>();
		this.characterMatrixOverride.Init(this);
	}

	// Token: 0x17000471 RID: 1137
	// (get) Token: 0x06002753 RID: 10067 RVA: 0x000EF999 File Offset: 0x000EDB99
	// (set) Token: 0x06002754 RID: 10068 RVA: 0x000EF9A1 File Offset: 0x000EDBA1
	public virtual Vector2 OnValue_InputMoveVector
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_MoveVector;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.m_MoveVector = (this.MovementRunning ? value.normalized : value);
		}
	}

	// Token: 0x17000472 RID: 1138
	// (get) Token: 0x06002755 RID: 10069 RVA: 0x000EF9BB File Offset: 0x000EDBBB
	// (set) Token: 0x06002756 RID: 10070 RVA: 0x000EF9C3 File Offset: 0x000EDBC3
	public virtual Vector2 OnValue_InputSmoothLook
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_SmoothLook;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.m_SmoothLook = value;
		}
	}

	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x06002757 RID: 10071 RVA: 0x000EF9BB File Offset: 0x000EDBBB
	public virtual Vector2 OnValue_InputRawLook
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_SmoothLook;
		}
	}

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x06002758 RID: 10072 RVA: 0x000D6005 File Offset: 0x000D4205
	public virtual Vector3 OnValue_CameraLookDirection
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.GetLookVector();
		}
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000EF9CC File Offset: 0x000EDBCC
	public override void Init(int _entityClass, EntityInstanceAssets _assets, EModelInstanceAssets _eModelAssets)
	{
		base.Init(_entityClass, _assets, _eModelAssets);
		Transform modelTransform = this.emodel.GetModelTransform();
		for (int i = 0; i < modelTransform.childCount; i++)
		{
			Transform child = modelTransform.GetChild(i);
			for (int j = 0; j < child.childCount; j++)
			{
				if (child.GetChild(j).GetComponent<Renderer>() is SkinnedMeshRenderer)
				{
					((SkinnedMeshRenderer)child.GetChild(j).GetComponent<Renderer>()).updateWhenOffscreen = true;
				}
			}
		}
		this.SetFirstPersonView(true, false);
		this.bPreferFirstPerson = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxDefaultFirstPersonCamera);
		this.cameraDistanceMulti = GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxCameraDistance3P);
		this.IsGodMode.Value = !GameStats.GetBool(EnumGameStats.IsPlayerDamageEnabled);
		this.IsGodMode.OnChangeDelegates += this.OnWeatherGodModeChanged;
		this.IsNoCollisionMode.Value = this.IsGodMode.Value;
		if (this.m_characterController != null)
		{
			this.PhysicsTransform.gameObject.layer = 20;
			this.m_characterController.enableOverlapRecovery = true;
		}
		if (this.vp_FPController != null)
		{
			this.vp_FPController.localPlayer = this;
			this.vp_FPController.Player.Register(this);
			this.vp_FPController.Player.FallImpact2.Register(this, "FallImpact", 0);
			this.vp_FPController.SyncCharacterController();
			this.vp_FPController.enabled = false;
		}
		GamePrefs.AddChangeListener(this);
		this.CameraRelativeMovement = true;
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000EFB3D File Offset: 0x000EDD3D
	public void OnGamePrefChanged(EnumGamePrefs _enum)
	{
		if (_enum == EnumGamePrefs.OptionsGfxCameraDistance3P)
		{
			this.cameraDistanceMulti = GamePrefs.GetFloat(_enum);
		}
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x000EFB54 File Offset: 0x000EDD54
	public bool NACommand(List<string> args)
	{
		if (args == null)
		{
			return false;
		}
		int count = args.Count;
		if (count > 0)
		{
			if (args[0] == "create")
			{
				if (count == 1)
				{
					return this.NAInit();
				}
			}
			else if (args[0] == "equip")
			{
				if (count != 1)
				{
					return this.NAEquip(args[1]);
				}
				return this.NAListEquipment();
			}
			else if (args[0] == "unequip")
			{
				if (count != 1)
				{
					return this.NAUnEquip(args[1]);
				}
			}
			else if (args[0] == "rot_x")
			{
				if (count != 1)
				{
					return this.NARotateX(args[1]);
				}
			}
			else
			{
				if (args[0] == "help")
				{
					this.NAHelp();
					return true;
				}
				if (args[0] == "parts")
				{
					return this.NAListParts();
				}
			}
		}
		this.NAHelp();
		return false;
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x000EFC47 File Offset: 0x000EDE47
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerEquippedSlots _GetNASlots()
	{
		return base.transform.GetComponent<PlayerEquippedSlots>();
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000EFC54 File Offset: 0x000EDE54
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform _GetNAOutfit()
	{
		return base.transform.Find("Graphics/Model/base");
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x000EFC68 File Offset: 0x000EDE68
	[PublicizedFrom(EAccessModifier.Private)]
	public void NAHelp()
	{
		Log.Warning("New avatar command help.");
		Log.Warning("------------------------");
		Log.Warning("na create                Create new avatar.");
		Log.Warning("na equip                 List current equipment.");
		Log.Warning("na equip {partname}      Equip a part.");
		Log.Warning("na help                  This help.");
		Log.Warning("na parts                 List available parts.");
		Log.Warning("na rot_x {degrees}       Turn player (0 faces away, 180 towards).");
		Log.Warning("na unequip {partname}    Unequip a part.");
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000EFCD0 File Offset: 0x000EDED0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAInit()
	{
		Log.Warning("New avatar init.");
		Transform transform = base.transform.Find("Graphics/Model");
		if (transform == null)
		{
			Log.Error("Entity does not have 'Graphics/Model' node!");
			return false;
		}
		if (transform.Find("base") != null)
		{
			return false;
		}
		Transform transform2 = DataLoader.LoadAsset<Transform>("@:Entities/Player/Male/maleTestPrefab.prefab", false);
		if (transform2 == null)
		{
			return false;
		}
		transform2 = UnityEngine.Object.Instantiate<Transform>(transform2, transform);
		transform2.name = "base";
		transform2.localPosition = new Vector3(0f, 0f, 1f);
		transform2.localRotation = Quaternion.Euler(0f, 135f, 0f);
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		if (playerEquippedSlots == null)
		{
			return false;
		}
		playerEquippedSlots.Init(this._GetNAOutfit());
		this.NAEquip("baseHead");
		this.NAEquip("baseBody");
		this.NAEquip("baseHands");
		this.NAEquip("baseFeet");
		return true;
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000EFDD0 File Offset: 0x000EDFD0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAListParts()
	{
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		if (playerEquippedSlots == null)
		{
			return false;
		}
		playerEquippedSlots.ListParts();
		return true;
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000EFDF8 File Offset: 0x000EDFF8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAListEquipment()
	{
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		if (playerEquippedSlots == null)
		{
			return false;
		}
		playerEquippedSlots.ListEquipment();
		return true;
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x000EFE20 File Offset: 0x000EE020
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAEquip(string partName)
	{
		Log.Warning("New avatar equip {0}", new object[]
		{
			partName
		});
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		return !(playerEquippedSlots == null) && playerEquippedSlots.Equip(partName);
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x000EFE5C File Offset: 0x000EE05C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NAUnEquip(string partName)
	{
		Log.Warning("New avatar unequip {0}", new object[]
		{
			partName
		});
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		return !(playerEquippedSlots == null) && playerEquippedSlots.UnEquip(partName);
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x000EFE98 File Offset: 0x000EE098
	[PublicizedFrom(EAccessModifier.Private)]
	public bool NARotateX(string value)
	{
		Transform transform = this._GetNAOutfit();
		if (transform == null)
		{
			return false;
		}
		transform.localRotation = Quaternion.Euler(0f, Convert.ToSingle(value), 0f);
		return true;
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000EFED4 File Offset: 0x000EE0D4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool _IsEquipped(string partName)
	{
		PlayerEquippedSlots playerEquippedSlots = this._GetNASlots();
		return !(playerEquippedSlots == null) && playerEquippedSlots.IsEquipped(partName);
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000EFEFC File Offset: 0x000EE0FC
	public override void PostInit()
	{
		base.PostInit();
		this.inventory.AddChangeListener(this);
		this.inventory.OnToolbeltItemsChangedInternal += this.callInventoryChanged;
		this.bag.OnBackpackItemsChangedInternal += this.callInventoryChanged;
		this.equipment.OnChanged += this.callInventoryChanged;
		this.DragAndDropItemChanged += this.callInventoryChanged;
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000EFF72 File Offset: 0x000EE172
	[PublicizedFrom(EAccessModifier.Private)]
	public void callInventoryChanged()
	{
		if (this.InventoryChangedEvent != null)
		{
			this.InventoryChangedEvent();
		}
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000EFF87 File Offset: 0x000EE187
	public override void OnAddedToWorld()
	{
		base.OnAddedToWorld();
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000EFF90 File Offset: 0x000EE190
	public override void OnEntityUnload()
	{
		base.OnEntityUnload();
		ItemClassHeldEntity.CleanupOnPlayerRemove(this);
		this.InventoryChangedEvent = null;
		GamePrefs.RemoveChangeListener(this);
		if (this.QuestJournal != null)
		{
			this.QuestJournal.UnHookQuests();
		}
		if (this.challengeJournal != null)
		{
			this.challengeJournal.EndChallenges();
		}
		this.inventory.Cleanup();
		this.bag.Clear();
		this.renderManager.Destroy();
		this.renderManager = null;
		if (this.cameraTransform.parent == null)
		{
			UnityEngine.Object.Destroy(this.cameraTransform.gameObject);
		}
		GameManager.Instance.World.RemoveLocalPlayer(this);
		if (this.ScreenEffectManager)
		{
			this.ClearScreenEffects();
		}
		this.playerUI = null;
		this.windowManager = null;
		this.nguiWindowManager = null;
		this.moveController = null;
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000027FC File Offset: 0x000009FC
	public void OnInventoryChanged(Inventory _inventory)
	{
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000F0066 File Offset: 0x000EE266
	public bool IsMoveStateStill()
	{
		return this.moveState == EntityPlayerLocal.MoveState.Idle || this.moveState == EntityPlayerLocal.MoveState.Crouch || (this.moveState == EntityPlayerLocal.MoveState.Swim && !this.IsSwimmingMoving());
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x000F0090 File Offset: 0x000EE290
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMoveStateToDefault()
	{
		this.SwimModeStop();
		if (!this.m_vp_FPController.enabled)
		{
			return;
		}
		vp_FPPlayerEventHandler player = this.m_vp_FPController.Player;
		if (base.IsCrouching)
		{
			if (this.MovementRunning && !player.Zoom.Active)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.CrouchRun, false);
				return;
			}
			if (player.InputMoveVector.Get() != Vector2.zero && this.m_vp_FPController.Velocity.sqrMagnitude > 0.01f)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.CrouchWalk, false);
				return;
			}
			this.SetMoveState(EntityPlayerLocal.MoveState.Crouch, false);
			return;
		}
		else
		{
			if (this.MovementRunning && !player.Zoom.Active)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.Run, false);
				return;
			}
			if (player.InputMoveVector.Get() != Vector2.zero && this.m_vp_FPController.Velocity.sqrMagnitude > 0.01f)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.Walk, false);
				return;
			}
			this.SetMoveState(EntityPlayerLocal.MoveState.Idle, false);
			return;
		}
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x000F0194 File Offset: 0x000EE394
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMoveState(EntityPlayerLocal.MoveState _state, bool _isOverride = false)
	{
		vp_FPPlayerEventHandler player = this.m_vp_FPController.Player;
		bool aimingGun = this.AimingGun;
		int value = this.inventory.holdingItem.HoldType.Value;
		bool flag = (value == 27 || value == 53 || value == 68) && this.SpecialAttack;
		if (_state != this.moveState)
		{
			EntityPlayerLocal.MoveState moveState = this.moveState;
			if (moveState - EntityPlayerLocal.MoveState.Crouch <= 2 && _state != EntityPlayerLocal.MoveState.Crouch && _state != EntityPlayerLocal.MoveState.CrouchWalk && _state != EntityPlayerLocal.MoveState.CrouchRun && !_isOverride)
			{
				if (!this.vp_FPCamera.HasOverheadSpace && !this.IsGodMode.Value)
				{
					_state = this.moveState;
				}
				else if (player.Crouch.Active && !player.Crouch.TryStop(true))
				{
					_state = this.moveState;
				}
				if (_state != this.moveState)
				{
					this.FireEvent(MinEventTypes.onSelfStand, true);
				}
			}
		}
		bool flag2 = _state != this.moveState;
		if (!flag2 && aimingGun == this.moveStateAiming && flag == this.moveStateHoldBow)
		{
			return;
		}
		this.m_vp_FPController.MotorDamping = 0.346f;
		this.m_vp_FPController.PhysicsSlopeSlideLimit = 60f;
		this.m_vp_FPController.PhysicsCrouchHeightModifier = 0.7f;
		this.SetMoveStateWeaponDamping(0.08f, 0.75f);
		if (this.m_vp_FPWeapon != null)
		{
			this.m_vp_FPWeapon.RotationLookSway = new Vector3(0.25f, 0.17f, 0f);
			this.m_vp_FPWeapon.RetractionDistance = 0.1f;
			this.m_vp_FPWeapon.BobRate = new Vector4(0.9f, 0.45f, 0f, 0f);
			this.m_vp_FPWeapon.BobAmplitude = new Vector4(0.35f, 0.5f, 0f, 0f);
			this.m_vp_FPWeapon.BobInputVelocityScale = 1f;
			this.m_vp_FPWeapon.ShakeSpeed = 0f;
			this.m_vp_FPWeapon.ShakeAmplitude = new Vector3(0.25f, 0f, 2f);
		}
		if (this.m_vp_FPCamera != null)
		{
			this.m_vp_FPCamera.RotationKneeling = 0.05f;
		}
		switch (_state)
		{
		case EntityPlayerLocal.MoveState.Off:
			if (this.m_vp_FPController.enabled)
			{
				player.Crouch.Stop(0f);
				player.Jump.Stop(0f);
				this.m_vp_FPController.Stop();
				this.m_vp_FPController.enabled = false;
			}
			this.SwimModeStop();
			break;
		case EntityPlayerLocal.MoveState.Attached:
			player.Crouch.Stop(0f);
			player.Jump.Stop(0f);
			this.SwimModeStop();
			break;
		case EntityPlayerLocal.MoveState.Idle:
			this.m_vp_FPController.MotorAcceleration = 0.12f;
			this.m_vp_FPController.MotorBackwardsSpeed = 0.8f;
			this.m_vp_FPController.MotorSidewaysSpeed = 0.8f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1.2f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 0.8f;
			if (flag2 && this.moveState != EntityPlayerLocal.MoveState.Walk)
			{
				this.FireEvent(MinEventTypes.onSelfWalk, true);
			}
			break;
		case EntityPlayerLocal.MoveState.Walk:
			this.m_vp_FPController.MotorAcceleration = 0.12f;
			this.m_vp_FPController.MotorBackwardsSpeed = 0.8f;
			this.m_vp_FPController.MotorSidewaysSpeed = 0.8f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1.2f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 0.8f;
			this.SetMoveStateWeapon();
			if (this.m_vp_FPCamera != null)
			{
				this.m_vp_FPCamera.RotationKneeling = 0.065f;
			}
			if (flag2 && this.moveState != EntityPlayerLocal.MoveState.Idle)
			{
				this.FireEvent(MinEventTypes.onSelfWalk, true);
			}
			break;
		case EntityPlayerLocal.MoveState.Run:
			this.m_vp_FPController.MotorAcceleration = 0.35f;
			this.m_vp_FPController.MotorBackwardsSpeed = 0.8f;
			this.m_vp_FPController.MotorSidewaysSpeed = 0.5f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1.2f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 0.8f;
			this.SetMoveStateWeapon();
			if (this.m_vp_FPWeapon != null)
			{
				this.m_vp_FPWeapon.BobRate = new Vector4(2f, 1f, 0f, 0f);
				this.m_vp_FPWeapon.BobAmplitude = new Vector4(1.5f, 1.2f, 0f, 0f);
			}
			if (this.m_vp_FPCamera != null)
			{
				this.m_vp_FPCamera.RotationKneeling = 0.075f;
			}
			if (flag2)
			{
				this.FireEvent(MinEventTypes.onSelfRun, true);
			}
			break;
		case EntityPlayerLocal.MoveState.Crouch:
			player.Crouch.Start(0f);
			this.m_vp_FPController.MotorAcceleration = 0.08f;
			this.m_vp_FPController.MotorBackwardsSpeed = 1f;
			this.m_vp_FPController.MotorSidewaysSpeed = 1f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 1f;
			if (flag2 && this.moveState != EntityPlayerLocal.MoveState.CrouchWalk && this.moveState != EntityPlayerLocal.MoveState.CrouchRun)
			{
				this.FireEvent(MinEventTypes.onSelfCrouch, true);
			}
			break;
		case EntityPlayerLocal.MoveState.CrouchWalk:
			player.Crouch.Start(0f);
			this.m_vp_FPController.MotorAcceleration = 0.08f;
			this.m_vp_FPController.MotorBackwardsSpeed = 1f;
			this.m_vp_FPController.MotorSidewaysSpeed = 1f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 1f;
			if (flag2)
			{
				this.FireEvent(MinEventTypes.onSelfCrouchWalk, true);
			}
			break;
		case EntityPlayerLocal.MoveState.CrouchRun:
			player.Crouch.Start(0f);
			this.m_vp_FPController.MotorAcceleration = 0.11f;
			this.m_vp_FPController.MotorBackwardsSpeed = 1f;
			this.m_vp_FPController.MotorSidewaysSpeed = 1f;
			this.m_vp_FPController.MotorSlopeSpeedDown = 1f;
			this.m_vp_FPController.MotorSlopeSpeedUp = 1f;
			if (flag2)
			{
				this.FireEvent(MinEventTypes.onSelfCrouchRun, true);
			}
			break;
		}
		if (flag)
		{
			this.SetMoveStateWeaponDamping(0.04f, 0.5f);
			if (this.m_vp_FPWeapon != null)
			{
				this.m_vp_FPWeapon.RotationLookSway = new Vector3(0.02f, 0.02f, 0f);
				this.m_vp_FPWeapon.RetractionDistance = 0f;
				this.m_vp_FPWeapon.BobAmplitude = new Vector4(0.1f, 0.05f, 0f, 0f);
				this.m_vp_FPWeapon.BobInputVelocityScale = 1f;
			}
		}
		if (aimingGun != this.moveStateAiming)
		{
			if (aimingGun)
			{
				this.FireEvent(MinEventTypes.onSelfAimingGunStart, true);
			}
			else
			{
				this.FireEvent(MinEventTypes.onSelfAimingGunStop, true);
			}
		}
		if (aimingGun)
		{
			this.SetMoveStateWeaponDamping(0.5f, 0.9f);
			if (this.m_vp_FPWeapon != null)
			{
				this.m_vp_FPWeapon.RotationLookSway = new Vector3(0.3f, 0.21f, 0f);
				this.m_vp_FPWeapon.RetractionDistance = 0f;
				this.m_vp_FPWeapon.ShakeSpeed = 0f;
				this.m_vp_FPWeapon.ShakeAmplitude = Vector3.zero;
				this.m_vp_FPWeapon.BobAmplitude = new Vector4(0.035f, 0.05f, 0f, 0f);
			}
		}
		this.m_vp_FPCamera.Refresh();
		if (this.m_vp_FPWeapon != null)
		{
			this.m_vp_FPWeapon.Refresh();
		}
		this.moveState = _state;
		this.moveStateAiming = aimingGun;
		this.moveStateHoldBow = flag;
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000F0928 File Offset: 0x000EEB28
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMoveStateWeapon()
	{
		this.SetMoveStateWeaponDamping(0.01f, 0.25f);
		if (this.m_vp_FPWeapon != null)
		{
			this.m_vp_FPWeapon.RotationLookSway = new Vector3(0.3f, 0.21f, 0f);
			this.m_vp_FPWeapon.RetractionDistance = 0f;
			this.m_vp_FPWeapon.BobAmplitude = new Vector4(0.25f, 0.15f, 0f, 0f);
			this.m_vp_FPWeapon.BobInputVelocityScale = 100f;
		}
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x000F09B8 File Offset: 0x000EEBB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMoveStateWeaponDamping(float _stiffness, float _damping)
	{
		if (this.m_vp_FPWeapon != null)
		{
			this.m_vp_FPWeapon.PositionSpringStiffness = _stiffness;
			this.m_vp_FPWeapon.PositionSpringDamping = _damping;
			this.m_vp_FPWeapon.PositionPivotSpringStiffness = _stiffness;
			this.m_vp_FPWeapon.PositionPivotSpringDamping = _damping;
			this.m_vp_FPWeapon.RotationSpringStiffness = _stiffness;
			this.m_vp_FPWeapon.RotationSpringDamping = _damping;
			this.m_vp_FPWeapon.RotationPivotSpringStiffness = _stiffness;
			this.m_vp_FPWeapon.RotationPivotSpringDamping = _damping;
		}
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x000F0A33 File Offset: 0x000EEC33
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsMoveStateCrouch()
	{
		return this.moveState == EntityPlayerLocal.MoveState.Crouch || this.moveState == EntityPlayerLocal.MoveState.CrouchWalk || this.moveState == EntityPlayerLocal.MoveState.CrouchRun;
	}

	// Token: 0x06002771 RID: 10097 RVA: 0x000F0A54 File Offset: 0x000EEC54
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CalcIfSwimming()
	{
		float inWaterPercent = this.inWaterPercent;
		if (this.swimClimbing)
		{
			return inWaterPercent >= 0.04f && !this.onGround;
		}
		if (this.IsMoveStateCrouch())
		{
			return inWaterPercent >= 0.7f;
		}
		return inWaterPercent >= 0.6f;
	}

	// Token: 0x06002772 RID: 10098 RVA: 0x000F0AA4 File Offset: 0x000EECA4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SwimModeTick()
	{
		vp_FPPlayerEventHandler player = this.m_vp_FPController.Player;
		this.SetMoveState(EntityPlayerLocal.MoveState.Swim, false);
		if (this.swimMode < 0)
		{
			this.swimMode = 1;
			this.swimExhaustedTicks = 0;
			this.swimClimbing = false;
			this.FireEvent(MinEventTypes.onSelfSwimStart, true);
			this.emodel.avatarController.SetSwim(true);
			this.m_vp_FPController.ScaleFallSpeed(0.2f);
		}
		this.m_vp_FPController.MotorFreeFly = true;
		this.m_vp_FPController.MotorJumpForce = 0f;
		this.m_vp_FPController.MotorJumpForceDamping = 0f;
		this.m_vp_FPController.MotorJumpForceHold = 0f;
		this.m_vp_FPController.MotorJumpForceHoldDamping = 1f;
		this.m_vp_FPController.MotorAcceleration = 0.00032f;
		if (!this.Jumping && !this.inputWasDown && player.InputMoveVector.Get().SqrMagnitude() < 0.001f)
		{
			this.m_vp_FPController.PhysicsGravityModifier = 0.003f;
			if (this.swimMode != 0)
			{
				this.swimMode = 0;
				this.FireEvent(MinEventTypes.onSelfSwimIdle, true);
			}
		}
		else
		{
			this.m_vp_FPController.PhysicsGravityModifier = 0f;
			if (this.MovementRunning)
			{
				this.m_vp_FPController.MotorAcceleration = 0.0024f;
				if (this.swimMode != 2)
				{
					this.swimMode = 2;
					this.FireEvent(MinEventTypes.onSelfSwimRun, true);
				}
			}
			else if (this.swimMode != 1)
			{
				this.swimMode = 1;
			}
		}
		if (this.Stamina <= 0f)
		{
			this.swimExhaustedTicks = 60;
		}
		if (this.swimExhaustedTicks > 0)
		{
			this.swimExhaustedTicks--;
			this.m_vp_FPController.PhysicsGravityModifier = 0.004f;
			if (!this.isHeadUnderwater)
			{
				this.m_vp_FPController.PhysicsGravityModifier = 0.08f;
			}
			this.m_vp_FPController.MotorAcceleration = 0.00025f;
		}
	}

	// Token: 0x06002773 RID: 10099 RVA: 0x000F0C79 File Offset: 0x000EEE79
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsSwimmingMoving()
	{
		return this.swimMode > 0;
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x000F0C84 File Offset: 0x000EEE84
	public void SwimModeUpdateThrottle()
	{
		float timeScale = Time.timeScale;
		float num = timeScale;
		if (this.swimExhaustedTicks > 0)
		{
			num *= 0.45f;
		}
		float num2 = 0.79f;
		float y = 0f;
		this.swimClimbing = false;
		if (this.inputWasJump && (this.vp_FPCamera.HasOverheadSpace || this.IsGodMode.Value))
		{
			if (this.onGround)
			{
				vp_FPController vp_FPController = this.m_vp_FPController;
				vp_FPController.m_MotorThrottle.y = vp_FPController.m_MotorThrottle.y + 0.05f * num;
			}
			else
			{
				bool flag = false;
				if (this.swimExhaustedTicks == 0 && this.moveDirection.z > 0f)
				{
					Vector3 hipPosition = this.getHipPosition();
					Vector3 forwardVector = base.GetForwardVector();
					forwardVector.y = -0.25f;
					Ray ray = new Ray(hipPosition, forwardVector);
					float num3 = this.position.y + 0.12f;
					for (float num4 = hipPosition.y + 0.3f; num4 > num3; num4 -= 0.16f)
					{
						hipPosition.y = num4;
						ray.origin = hipPosition;
						if (Voxel.Raycast(this.world, ray, 0.45f, 1073807360, 65, 0.165f) && Voxel.phyxRaycastHit.normal.y > 0.3f)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					this.swimClimbing = true;
					num2 = 0.1f;
					y = 0.02f;
				}
				else
				{
					vp_FPController vp_FPController2 = this.m_vp_FPController;
					vp_FPController2.m_MotorThrottle.y = vp_FPController2.m_MotorThrottle.y + 0.00052f * num;
				}
			}
		}
		else if (this.inputWasDown)
		{
			vp_FPController vp_FPController3 = this.m_vp_FPController;
			vp_FPController3.m_MotorThrottle.y = vp_FPController3.m_MotorThrottle.y + -0.00038f * num;
		}
		Vector3 lookVector = this.GetLookVector();
		float num5 = this.m_vp_FPController.MotorAcceleration * num;
		this.m_vp_FPController.m_MotorThrottle += lookVector * (this.moveDirection.z * num5);
		this.m_vp_FPController.m_MotorThrottle += base.transform.TransformDirection(Vector3.right) * (this.moveDirection.x * num5 * 0.7f);
		float num6 = 0.01f + Mathf.Pow(this.m_vp_FPController.m_MotorThrottle.magnitude * 5.4f, 2f);
		this.m_vp_FPController.m_MotorThrottle /= 1f + num6 * timeScale;
		if (this.swimClimbing)
		{
			this.m_vp_FPController.m_MotorThrottle.y = y;
		}
		if (this.inWaterPercent < num2 && !base.IsInElevator() && this.m_vp_FPController.m_MotorThrottle.y > 0f)
		{
			vp_FPController vp_FPController4 = this.m_vp_FPController;
			vp_FPController4.m_MotorThrottle.y = vp_FPController4.m_MotorThrottle.y * 0.5f;
			if (this.inWaterPercent < num2 - 0.04f)
			{
				this.m_vp_FPController.m_MotorThrottle.y = 0f;
			}
		}
		this.m_vp_FPController.m_MotorThrottle = vp_MathUtility.SnapToZero(this.m_vp_FPController.m_MotorThrottle, 2E-05f);
	}

	// Token: 0x06002775 RID: 10101 RVA: 0x000F0F98 File Offset: 0x000EF198
	[PublicizedFrom(EAccessModifier.Private)]
	public void SwimModeStop()
	{
		if (this.swimMode >= 0)
		{
			this.swimMode = -1;
			this.FireEvent(MinEventTypes.onSelfSwimStop, true);
			this.emodel.avatarController.SetSwim(false);
			this.m_vp_FPController.MotorFreeFly = false;
			this.m_vp_FPController.PhysicsGravityModifier = 0.2f;
			this.m_vp_FPController.MotorJumpForce = 0.13f;
			this.m_vp_FPController.MotorJumpForceDamping = 0.08f;
			this.m_vp_FPController.MotorJumpForceHold = 0.003f;
			this.m_vp_FPController.MotorJumpForceHoldDamping = 0.5f;
		}
	}

	// Token: 0x06002776 RID: 10102 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void StartJumpSwimMotion()
	{
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x000F102B File Offset: 0x000EF22B
	public override void PhysicsPush(Vector3 forceVec, Vector3 forceWorldPos, bool affectLocalPlayerController = false)
	{
		if (this.IsGodMode.Value)
		{
			return;
		}
		if (affectLocalPlayerController && this.vp_FPController != null)
		{
			this.vp_FPController.AddForce(forceVec);
			return;
		}
		base.PhysicsPush(forceVec, forceWorldPos, affectLocalPlayerController);
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000F1064 File Offset: 0x000EF264
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void onSpawnStateChanged()
	{
		base.onSpawnStateChanged();
		if (this.vp_FPController && (!this.Spawned || this.moveController.respawnReason != RespawnType.Teleport || !this.IsFlyMode.Value))
		{
			this.vp_FPController.enabled = this.Spawned;
		}
		if (this.Spawned && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && PrefabEditModeManager.Instance.IsActive())
		{
			PrefabEditModeManager.Instance.LoadRecentlyUsedOrCreateNew();
		}
	}

	// Token: 0x06002779 RID: 10105 RVA: 0x000F10E4 File Offset: 0x000EF2E4
	public override void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
		vp_FPController vp_FPController = this.vp_FPController;
		bool flag = vp_FPController != null && !this.IsStuck && !this.IsFlyMode.Value;
		bool flag2 = true;
		bool flag3 = false;
		if (this.onGround)
		{
			this.isLadderAttached = false;
			this.canLadderAirAttach = true;
		}
		if (this.isLadderAttached)
		{
			this.speedStrafe = 0f;
		}
		bool flag4 = this.jumpTrigger;
		bool flag5 = flag4 != this.wasJumpTrigger;
		this.wasJumpTrigger = this.jumpTrigger;
		if (base.IsInElevator() && !this.IsFlyMode.Value)
		{
			flag3 = true;
			bool flag6 = false;
			if (flag4)
			{
				if (flag5)
				{
					if (this.isLadderAttached)
					{
						this.isLadderAttached = false;
						this.canLadderAirAttach = false;
						this.wasLadderAttachedJump = true;
					}
					else
					{
						this.canLadderAirAttach = true;
						this.wasLadderAttachedJump = false;
					}
				}
			}
			else
			{
				this.wasLadderAttachedJump = false;
			}
			if (!this.isLadderAttached)
			{
				if (vp_FPController != null)
				{
					bool flag7 = vp_FPController.enabled ? vp_FPController.Grounded : this.onGround;
					if ((flag7 || this.isSwimming) && vp_FPController.IsCollidingWall && vp_FPController.ProjectedWallMove < 0.3f && _direction.z > 0f)
					{
						flag6 = true;
					}
					else if (!flag7 && this.canLadderAirAttach)
					{
						float y = vp_FPController.Velocity.y;
						if (y <= 0f)
						{
							if (y >= -3f)
							{
								this.isLadderAttached = true;
								this.wasLadderAttachedJump = false;
							}
							else
							{
								vp_FPController.ScaleFallSpeed(0.75f);
							}
						}
					}
				}
				else if (this.onGround && this.isCollidedHorizontally && this.projectedMove < 0.3f && _direction.z > 0f)
				{
					flag6 = true;
				}
				else if (!this.onGround)
				{
					this.isLadderAttached = true;
				}
			}
			Vector3 cameraLook = this.GetCameraLook(1f);
			if (flag6 && cameraLook.y > 0.1f)
			{
				this.isLadderAttached = true;
			}
			if (this.isLadderAttached)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.Off, true);
				float num = this.MovementRunning ? 0.17f : 0.06f;
				num *= this.GetSpeedModifier();
				if (_direction.x != 0f || _direction.z > 0f)
				{
					Vector3 vector = _direction;
					if (vector.z < 0f)
					{
						vector.z = 0f;
					}
					float num2 = num * 0.65f;
					this.Move(vector, _isDirAbsolute, num2, num2);
				}
				if (this.motion.x < -0.11f)
				{
					this.motion.x = -0.11f;
				}
				if (this.motion.x > 0.11f)
				{
					this.motion.x = 0.11f;
				}
				if (this.motion.z < -0.11f)
				{
					this.motion.z = -0.11f;
				}
				if (this.motion.z > 0.11f)
				{
					this.motion.z = 0.11f;
				}
				cameraLook.y += 0.15f;
				cameraLook.y *= 2f;
				cameraLook.y = Mathf.Clamp(cameraLook.y, -1f, 1f);
				this.motion.y = _direction.z * cameraLook.y * num;
				this.fallDistance = 0f;
				this.entityCollision(this.motion);
				this.motion *= base.ScalePhysicsMulConstant(0.545f);
				this.distanceClimbed += this.motion.magnitude;
				if (this.distanceClimbed > 0.5f)
				{
					base.internalPlayStepSound(1f);
					this.distanceClimbed = 0f;
				}
			}
			flag2 = !this.isLadderAttached;
		}
		else
		{
			this.isLadderAttached = false;
		}
		if (flag && (!flag3 || flag2))
		{
			vp_FPController.enabled = true;
			this.motion = Vector3.zero;
			this.world.CheckEntityCollisionWithBlocks(this);
			if (vp_FPController.Grounded)
			{
				Transform groundTransform = vp_FPController.GroundTransform;
				if (groundTransform && groundTransform.CompareTag("LargeEntityBlocker"))
				{
					Vector2 randomOnUnitCircle = this.rand.RandomOnUnitCircle;
					vp_FPController.AddForce(randomOnUnitCircle.x * 0.008f, 0f, randomOnUnitCircle.y * 0.008f);
				}
			}
			flag2 = false;
		}
		if (flag2)
		{
			bool inElevator = base.IsInElevator();
			base.SetInElevator(false);
			base.MoveEntityHeaded(_direction, false);
			base.SetInElevator(inElevator);
		}
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000F1568 File Offset: 0x000EF768
	public void SetCameraAttachedToPlayer(bool _b, bool _lockCamera)
	{
		if (_b)
		{
			this.cameraTransform.SetParent(this.cameraContainerTransform, false);
			this.cameraTransform.SetAsFirstSibling();
			this.cameraTransform.SetLocalPositionAndRotation(Constants.cDefaultCameraPlayerOffset, Quaternion.identity);
			this.vp_FPCamera.Locked3rdPerson = false;
			this.movementInput.bDetachedCameraMove = false;
			return;
		}
		this.cameraTransform.parent = null;
		this.vp_FPCamera.Locked3rdPerson = _lockCamera;
	}

	// Token: 0x0600277B RID: 10107 RVA: 0x000F15DB File Offset: 0x000EF7DB
	public bool IsCameraAttachedToPlayerOrScope()
	{
		return this.cameraTransform.parent != null;
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x000F15F0 File Offset: 0x000EF7F0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CheckNonSolidVertical(Vector3i blockPos, int maxY, int verticalSpace)
	{
		for (int i = 0; i < maxY; i++)
		{
			if (!this.world.GetBlock(blockPos.x, blockPos.y + i + 1, blockPos.z).Block.shape.IsSolidSpace)
			{
				bool flag = true;
				for (int j = 1; j < verticalSpace; j++)
				{
					if (this.world.GetBlock(blockPos.x, blockPos.y + i + 1 + j, blockPos.z).Block.shape.IsSolidSpace)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x000F1694 File Offset: 0x000EF894
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		if (this.AttachedToEntity != null)
		{
			return;
		}
		if (this.m_vp_FPController != null)
		{
			if (this.m_vp_FPController.enabled)
			{
				this.position = this.vp_FPController.SmoothPosition + Origin.position;
			}
			else
			{
				float elapsedPartialTicks = GameTimer.Instance.elapsedPartialTicks;
				if (elapsedPartialTicks < 1f)
				{
					base.transform.position = this.lastTickPos[0] + (this.position - this.lastTickPos[0]) * elapsedPartialTicks - Origin.position;
				}
				else
				{
					base.transform.position = this.position - Origin.position;
				}
				this.vp_FPController.SetPosition(base.transform.position);
			}
			this.rotation = this.PhysicsTransform.eulerAngles;
			if (this.vp_FPCamera != null)
			{
				this.rotation.x = -this.vp_FPCamera.Angle.x;
			}
			float num = base.width / 2f;
			float num2 = base.depth / 2f;
			this.boundingBox = BoundsUtils.BoundsForMinMax(this.position.x - num, this.position.y - this.yOffset + this.ySize, this.position.z - num2, this.position.x + num, this.position.y - this.yOffset + this.ySize + base.height, this.position.z + num2);
			return;
		}
		base.updateTransform();
	}

	// Token: 0x0600277E RID: 10110 RVA: 0x000F1854 File Offset: 0x000EFA54
	public override void SetRotation(Vector3 _rot)
	{
		base.SetRotation(_rot);
		if (this.PhysicsTransform && !this.emodel.IsRagdollActive)
		{
			this.PhysicsTransform.eulerAngles = _rot;
		}
		if (this.m_vp_FPCamera)
		{
			this.m_vp_FPCamera.Angle = new Vector2(-_rot.x, _rot.y);
		}
	}

	// Token: 0x0600277F RID: 10111 RVA: 0x000F18B8 File Offset: 0x000EFAB8
	public override void OnUpdatePosition(float _partialTicks)
	{
		if (GameManager.bPlayRecordedSession && this.Spawned)
		{
			PlayerInputRecordingSystem.Instance.Play(this, true);
		}
		if (this.m_vp_FPController != null)
		{
			this.ticksExisted++;
			this.prevPos = this.position;
			this.prevRotation = this.rotation;
			if (this.Spawned)
			{
				Vector3 vector = Vector3.zero;
				for (int i = 0; i < this.lastTickPos.Length - 1; i++)
				{
					vector += this.lastTickPos[i] - this.lastTickPos[i + 1];
				}
				vector /= (float)(this.lastTickPos.Length - 1);
				float num = Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
				this.UpdateDistanceTravelledAchievement(num);
				if (this.AttachedToEntity == null)
				{
					this.updateStepSound(vector.x, vector.z, 0f);
					base.updatePlayerLandSound(num, vector.y);
				}
				else
				{
					this.distanceWalked += num;
				}
			}
			this.updateSpeedForwardAndStrafe(this.m_vp_FPController.Velocity, _partialTicks);
			base.ReplicateSpeeds();
		}
		else
		{
			base.OnUpdatePosition(_partialTicks);
		}
		if (this.Spawned)
		{
			if (this.position.y >= 2f && this.position.y < 4f)
			{
				IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager != null)
				{
					achievementManager.SetAchievementStat(EnumAchievementDataStat.DepthAchieved, 1);
				}
			}
			else if (this.position.y >= 255f)
			{
				IAchievementManager achievementManager2 = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager2 != null)
				{
					achievementManager2.SetAchievementStat(EnumAchievementDataStat.HeightAchieved, 1);
				}
			}
		}
		GameSenseManager instance = GameSenseManager.Instance;
		if (instance != null)
		{
			instance.UpdateEventCompass(this.rotation.y);
		}
		if (GameManager.bRecordNextSession && this.Spawned)
		{
			PlayerInputRecordingSystem.Instance.Record(this, GameTimer.Instance.ticks);
		}
	}

	// Token: 0x06002780 RID: 10112 RVA: 0x000F1AB0 File Offset: 0x000EFCB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void UpdateDistanceTravelledAchievement(float distanceTravelled)
	{
		float num = distanceTravelled / 1000f;
		this.achievementDistanceAccu += num;
		if ((double)this.achievementDistanceAccu > 0.05)
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.SetAchievementStat(EnumAchievementDataStat.KMTravelled, this.achievementDistanceAccu);
			}
			this.achievementDistanceAccu -= 0.05f;
		}
	}

	// Token: 0x06002781 RID: 10113 RVA: 0x000F1B14 File Offset: 0x000EFD14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateSpeedForwardAndStrafe(Vector3 _dist, float _partialTicks)
	{
		this.speedForward = 0f;
		this.speedStrafe = 0f;
		this.speedVertical = 0f;
		if (this.isLadderAttached)
		{
			this.speedForward += _dist.y;
			this.speedStrafe = 1234f;
			this.speedVertical = 0f;
		}
		else
		{
			if (Mathf.Abs(_dist.x) > 0.001f || Mathf.Abs(_dist.z) > 0.001f)
			{
				Vector3 vector = base.transform.InverseTransformDirection(_dist).normalized * _dist.magnitude;
				this.speedForward = (float)((int)(vector.z * 100f)) / 100f;
				this.speedStrafe = (float)((int)(vector.x * 100f)) / 100f;
			}
			if (Mathf.Abs(_dist.y) > 0.001f)
			{
				this.speedVertical += _dist.y;
			}
		}
		this.SetMovementState();
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x000F1C1C File Offset: 0x000EFE1C
	public override void OnUpdateEntity()
	{
		if (this.DropTimeDelay > 0f)
		{
			this.DropTimeDelay -= 0.05f;
		}
		float value = base.Stats.Health.Value;
		float num = (this.oldHealth - value) / base.Stats.Health.Max;
		float time = Time.time;
		if (value != this.oldHealth || time > this.vibrationTimeout)
		{
			if (GamePrefs.GetInt(EnumGamePrefs.OptionsControllerVibrationStrength) > 0 && this.playerInput != null && this.moveController.GetControllerVibration())
			{
				InputDevice inputDevice = this.playerInput.Device;
				if (inputDevice == null && this.playerInput.LastInputType == BindingSourceType.DeviceBindingSource)
				{
					inputDevice = InputManager.ActiveDevice;
				}
				if (inputDevice != null)
				{
					if (this.oldHealth > value)
					{
						if (value <= 0f)
						{
							inputDevice.Vibrate(0.5f);
							GameManager.Instance.triggerEffectManager.SetGamepadVibration(0.5f);
						}
						else if (num > 0.25f)
						{
							inputDevice.Vibrate(0.5f);
							GameManager.Instance.triggerEffectManager.SetGamepadVibration(0.5f);
						}
						else if (num > 0.1f)
						{
							inputDevice.Vibrate(0.35f);
							GameManager.Instance.triggerEffectManager.SetGamepadVibration(0.35f);
						}
						else
						{
							inputDevice.Vibrate(0.25f);
							GameManager.Instance.triggerEffectManager.SetGamepadVibration(0.25f);
						}
						this.vibrationTimeout = Time.time + 0.25f;
					}
					else
					{
						inputDevice.StopVibration();
						GameManager.Instance.triggerEffectManager.StopGamepadVibration(false);
						this.vibrationTimeout = float.MaxValue;
					}
				}
			}
			if (num > 0.02f)
			{
				GameSenseManager instance = GameSenseManager.Instance;
				if (instance != null)
				{
					instance.UpdateEventHit();
				}
			}
			GameSenseManager instance2 = GameSenseManager.Instance;
			if (instance2 != null)
			{
				instance2.UpdateEventHealth((int)(base.Stats.Health.ValuePercentUI * 100f));
			}
			this.oldHealth = value;
		}
		this.equipment.Update();
		base.OnUpdateEntity();
		this.WeatherStatusTick();
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x000F1E1C File Offset: 0x000F001C
	public override void OnUpdateLive()
	{
		if (this.IsSpawned())
		{
			if (Time.time - this.updateBedrollPositionChecks > 5f)
			{
				this.updateBedrollPositionChecks = Time.time;
				if (!this.CheckSpawnPointStillThere())
				{
					this.RemoveSpawnPoints(true);
				}
			}
			if (Time.time - this.updateRadiationChecks > 5f)
			{
				this.updateRadiationChecks = Time.time;
				IChunkProvider chunkProvider = this.world.ChunkCache.ChunkProvider;
				IBiomeProvider biomeProvider;
				if (chunkProvider != null && (biomeProvider = chunkProvider.GetBiomeProvider()) != null)
				{
					float radiationAt = biomeProvider.GetRadiationAt((int)this.position.x, (int)this.position.z);
					this.Buffs.SetCustomVar("_biomeradiation", radiationAt, true, CVarOperation.set, false);
				}
			}
			GameEventManager.Current.UpdateCurrentBossGroup(this);
		}
		if (this.AttachedToEntity != null)
		{
			this.SetMoveState(EntityPlayerLocal.MoveState.Attached, true);
			base.OnUpdateLive();
			if (!this.isEntityRemote)
			{
				this.BlockRadiusEffectsTick();
			}
			this.IsStuck = false;
			return;
		}
		bool isStuck = this.IsStuck;
		this.IsStuck = false;
		if (!this.IsFlyMode.Value)
		{
			float num = this.boundingBox.min.y + 0.5f;
			this.IsStuck = this.pushOutOfBlocks(this.position.x - base.width * 0.3f, num, this.position.z + base.depth * 0.3f);
			this.IsStuck = (this.pushOutOfBlocks(this.position.x - base.width * 0.3f, num, this.position.z - base.depth * 0.3f) || this.IsStuck);
			this.IsStuck = (this.pushOutOfBlocks(this.position.x + base.width * 0.3f, num, this.position.z - base.depth * 0.3f) || this.IsStuck);
			this.IsStuck = (this.pushOutOfBlocks(this.position.x + base.width * 0.3f, num, this.position.z + base.depth * 0.3f) || this.IsStuck);
			if (!this.IsStuck)
			{
				int num2 = Utils.Fastfloor(this.position.x);
				int num3 = Utils.Fastfloor(num);
				int num4 = Utils.Fastfloor(this.position.z);
				if (this.shouldPushOutOfBlock(num2, num3, num4, true))
				{
					if (!this.shouldPushOutOfBlock(num2 - 1, num3, num4, true))
					{
						this.IsStuck = true;
						this.motion = new Vector3(-0.25f, 0f, 0f);
					}
					else if (!this.shouldPushOutOfBlock(num2 + 1, num3, num4, true))
					{
						this.IsStuck = true;
						this.motion = new Vector3(0.25f, 0f, 0f);
					}
					if (!this.shouldPushOutOfBlock(num2, num3, num4 - 1, true))
					{
						this.IsStuck = true;
						this.motion = new Vector3(0f, 0f, -0.25f);
					}
					else if (!this.shouldPushOutOfBlock(num2, num3, num4 + 1, true))
					{
						this.IsStuck = true;
						this.motion = new Vector3(0f, 0f, 0.25f);
					}
					else if (this.CheckNonSolidVertical(new Vector3i(num2, num3 + 1, num4), 4, 2))
					{
						this.IsStuck = true;
						this.motion = new Vector3(0f, 1.6f, 0f);
						Log.Warning("{0} Player is stuck, trying to unstick", new object[]
						{
							Time.frameCount
						});
					}
				}
			}
		}
		bool flag = true;
		bool flag2 = false;
		bool flag3 = this.InAir;
		this.InAir = (!this.isLadderAttached && !this.onGround);
		if (!this.wasJumping && !this.jumpTrigger && flag3 && !this.InAir && !this.isLadderAttached)
		{
			this.EndJump();
		}
		if (this.m_vp_FPController != null)
		{
			if (this.IsStuck || this.IsFlyMode.Value)
			{
				this.SetMoveState(EntityPlayerLocal.MoveState.Off, true);
			}
			else
			{
				flag = false;
				base.Stats.Health.RegenerationAmount = 0f;
				base.Stats.Stamina.RegenerationAmount = 0f;
				if (isStuck != this.IsStuck)
				{
					this.m_vp_FPController.Stop();
				}
				if (this.m_vp_FPController.enabled)
				{
					this.onGround = this.m_vp_FPController.Grounded;
				}
				bool flag4 = this.jumpTrigger;
				if (this.isSwimming)
				{
					this.SwimModeTick();
					flag2 = true;
				}
				else
				{
					if (this.vp_FPCamera != null && this.m_vp_FPController != null && this.m_vp_FPWeapon != null)
					{
						this.SetMoveStateToDefault();
					}
					if (flag4 && (this.vp_FPCamera.HasOverheadSpace || this.IsGodMode.Value))
					{
						vp_Activity jump = this.m_vp_FPController.Player.Jump;
						bool active = jump.Active;
						this.m_vp_FPController.MotorJumpForce = Mathf.Max(EffectManager.GetValue(PassiveEffects.JumpStrength, null, this.m_vp_FPController.originalMotorJumpForce, this, null, this.CurrentStanceTag | this.CurrentMovementTag, true, true, true, true, true, 1, true, false), 0f) * SandboxOptionManager.GetFloat(SandboxOptions.JumpStrength);
						this.m_vp_FPController.MotorJumpForceHold = this.m_vp_FPController.MotorJumpForce / Mathf.Lerp(90f, 180f, Mathf.Clamp01(1f - this.m_vp_FPController.originalMotorJumpForce / this.m_vp_FPController.MotorJumpForce)) * Time.timeScale;
						if (base.IsInElevator())
						{
							if (!active && !this.wasJumping && (this.onGround || this.isLadderAttached))
							{
								jump.Start(0f);
							}
						}
						else if (!this.wasJumping)
						{
							jump.TryStart(true);
						}
						if (!active && jump.Active)
						{
							this.Jumping = true;
							if (SandboxOptionManager.GetFloat(SandboxOptions.JumpStrength) > 0f)
							{
								this.Stamina -= Mathf.Max(EffectManager.GetValue(PassiveEffects.StaminaLoss, null, 4f, this, null, FastTags<TagGroup.Global>.Parse("jumping") | this.CurrentStanceTag | this.CurrentMovementTag, true, true, true, true, true, 1, true, false) * ItemActionAttack.StaminaUsageMultiplier, 0f);
								this.FireEvent(MinEventTypes.onSelfJump, true);
								this.PlayOneShot(this.GetSoundJump(), false, false, false, null, 1f);
							}
						}
						if (this.onGround && this.wasJumping)
						{
							this.Jumping = false;
							this.jumpTrigger = false;
						}
						if (this.isLadderAttached && this.wasJumping)
						{
							this.bJumping = false;
							this.jumpTrigger = false;
						}
					}
					else
					{
						this.m_vp_FPController.Player.Jump.Stop(0f);
					}
				}
				this.wasJumping = flag4;
			}
		}
		if (flag)
		{
			base.OnUpdateLive();
		}
		else
		{
			base.CheckSleeperTriggers();
			base.Stats.Tick(this.world.worldTime);
			this.m_vp_FPController.SpeedModifier = this.GetSpeedModifier() * Mathf.Clamp01(EffectManager.GetValue(PassiveEffects.Mobility, null, 1f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)) * (this.isMotionSlowedDown ? this.motionMultiplier : 1f);
			this.isMotionSlowedDown = false;
			base.updateCurrentBlockPosAndValue();
			if (this.canEntityMove())
			{
				this.MoveEntityHeaded(this.moveDirection, false);
			}
			base.checkForTeleportOutOfTraderArea();
		}
		if (this.challengeJournal != null)
		{
			this.challengeJournal.Update(this.world);
		}
		if (this.QuestJournal != null)
		{
			this.QuestJournal.Update(this.world.WorldDay);
		}
		if (!this.isEntityRemote)
		{
			this.BlockRadiusEffectsTick();
		}
		if (this.Stamina <= 0f)
		{
			this.bExhausted = true;
			this.Stamina = 0f;
		}
		float stamina = this.Stamina;
		if (this.bExhausted && stamina > base.Stats.Stamina.Max * 0.2f)
		{
			this.isExhaustedSoundAllowed = true;
			this.bExhausted = false;
		}
		if (this.bExhausted && this.isExhaustedSoundAllowed)
		{
			this.PlayOneShot(this.GetSoundStamina(), false, false, false, null, 1f);
			GameManager.ShowTooltip(this, "ttOutOfStamina", false, false, 0f);
			this.isExhaustedSoundAllowed = false;
		}
		if (this.prevStaminaValue >= stamina - 0.1f && this.moveState == EntityPlayerLocal.MoveState.Run && !flag2)
		{
			this.runTicks++;
			if (this.runTicks > 100 && stamina / base.Stats.Stamina.Max < 0.5f && this.sprintLoopSoundPlayId == -1 && !this.IsDead())
			{
				Manager.BroadcastPlay(this, "Player" + (this.IsMale ? "Male" : "Female") + "RunLoop", false, 1f);
				this.sprintLoopSoundPlayId = 0;
			}
			this.lerpCameraFastFOV += Time.deltaTime * Constants.cRunningFOVSpeedUp;
		}
		else
		{
			if (this.sprintLoopSoundPlayId != -1)
			{
				Manager.BroadcastStop(this.entityId, "Player" + (this.IsMale ? "Male" : "Female") + "RunLoop");
				this.sprintLoopSoundPlayId = -1;
				if (this.isExhaustedSoundAllowed)
				{
					this.PlayOneShot(this.GetSoundStamina(), false, false, false, null, 1f);
				}
			}
			this.lerpCameraFastFOV -= Time.deltaTime * Constants.cRunningFOVSpeedDown;
			this.runTicks = 0;
		}
		this.lerpCameraFastFOV = Mathf.Clamp01(this.lerpCameraFastFOV);
		this.prevStaminaValue = stamina;
		if (this.playerUI != null && this.playerUI.windowManager.IsModalWindowOpen())
		{
			this.TryCancelChargedAction();
		}
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x000F2808 File Offset: 0x000F0A08
	[PublicizedFrom(EAccessModifier.Private)]
	public void BlockRadiusEffectsTick()
	{
		Vector3i blockPosition = base.GetBlockPosition();
		int num = World.toChunkXZ(blockPosition.x);
		int num2 = World.toChunkXZ(blockPosition.z);
		this.blockRadiusEffectsIndex = (this.blockRadiusEffectsIndex + 1) % 3;
		int chunkZ = num2 + this.blockRadiusEffectsIndex - 1;
		for (int i = -1; i <= 1; i++)
		{
			Chunk chunk = (Chunk)this.world.GetChunkSync(num + i, chunkZ);
			if (chunk != null)
			{
				DictionaryList<Vector3i, TileEntity> tileEntities = chunk.GetTileEntities();
				for (int j = 0; j < tileEntities.list.Count; j++)
				{
					TileEntity tileEntity = tileEntities.list[j];
					if (tileEntity.IsActive(this.world))
					{
						Block block = tileEntity.block;
						if (block.RadiusEffects != null)
						{
							this.BlockRadiusEffectsApply(block, tileEntity.ToWorldPos().ToVector3());
						}
					}
				}
			}
		}
	}

	// Token: 0x06002785 RID: 10117 RVA: 0x000F28E4 File Offset: 0x000F0AE4
	public void BlockRadiusEffectsApply(Block _block, Vector3 _pos)
	{
		if (_pos.y > this.position.y)
		{
			_pos.y = Utils.FastMoveTowards(_pos.y, this.position.y, 1f);
		}
		float distanceSq = base.GetDistanceSq(_pos);
		for (int i = 0; i < _block.RadiusEffects.Length; i++)
		{
			BlockRadiusEffect blockRadiusEffect = _block.RadiusEffects[i];
			if (distanceSq <= blockRadiusEffect.radiusSq && !this.Buffs.HasBuff(blockRadiusEffect.variable))
			{
				this.Buffs.AddBuff(blockRadiusEffect.variable, -1, true, false, -1f);
			}
		}
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x000F2984 File Offset: 0x000F0B84
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleLootStageMaxCheck()
	{
		base.HandleLootStageMaxCheck();
		if (this.biomeStandingOn == null)
		{
			return;
		}
		if (GameStats.GetBool(EnumGameStats.BiomeProgression))
		{
			int lootStage = base.GetLootStage(0f, 0f);
			int num = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.LootStageMax, null, (float)this.biomeStandingOn.LootStageMax, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
			if (lootStage >= num)
			{
				if (!this.LootAtMax || this.lastLootStage == -1)
				{
					this.FireEvent(MinEventTypes.onSelfBiomeLootStageMaxEntered, true);
					this.LootAtMax = true;
				}
			}
			else if (lootStage < num && (this.LootAtMax || this.lastLootStage == -1))
			{
				this.FireEvent(MinEventTypes.onSelfBiomeLootStageMaxExited, true);
				this.LootAtMax = false;
			}
			this.lastLootStage = lootStage;
		}
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x000F2A40 File Offset: 0x000F0C40
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canEntityMove()
	{
		bool result = true;
		if (!this.IsFlyMode.Value)
		{
			Chunk chunk = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos((int)(this.position.x + this.moveDirection.x * 2f), (int)this.position.y, (int)(this.position.z + this.moveDirection.z * 2f));
			if (chunk == null || !chunk.IsCollisionMeshGenerated)
			{
				result = false;
			}
		}
		return result;
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x000F2ACC File Offset: 0x000F0CCC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool shouldPushOutOfBlock(int _x, int _y, int _z, bool pushOutOfTerrain)
	{
		BlockShape shape = this.world.GetBlock(_x, _y, _z).Block.shape;
		if (shape.IsSolidSpace && !shape.IsTerrain())
		{
			return true;
		}
		if (pushOutOfTerrain && shape.IsSolidSpace && shape.IsTerrain())
		{
			BlockShape shape2 = this.world.GetBlock(_x, _y + 1, _z).Block.shape;
			if (shape2.IsSolidSpace && shape2.IsTerrain())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x000F2B4C File Offset: 0x000F0D4C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool pushOutOfBlocks(float _x, float _y, float _z)
	{
		int num = Utils.Fastfloor(_x);
		int num2 = Utils.Fastfloor(_y);
		int num3 = Utils.Fastfloor(_z);
		float num4 = _x - (float)num;
		float num5 = _z - (float)num3;
		bool result = false;
		bool flag = base.IsCrouching || this.IsMoveStateCrouch();
		if (this.shouldPushOutOfBlock(num, num2, num3, false) || (!flag && this.shouldPushOutOfBlock(num, num2 + 1, num3, false)))
		{
			bool flag2 = !this.shouldPushOutOfBlock(num - 1, num2, num3, true) && !this.shouldPushOutOfBlock(num - 1, num2 + 1, num3, true);
			bool flag3 = !this.shouldPushOutOfBlock(num + 1, num2, num3, true) && !this.shouldPushOutOfBlock(num + 1, num2 + 1, num3, true);
			bool flag4 = !this.shouldPushOutOfBlock(num, num2, num3 - 1, true) && !this.shouldPushOutOfBlock(num, num2 + 1, num3 - 1, true);
			bool flag5 = !this.shouldPushOutOfBlock(num, num2, num3 + 1, true) && !this.shouldPushOutOfBlock(num, num2 + 1, num3 + 1, true);
			byte b = byte.MaxValue;
			float num6 = 9999f;
			if (flag2 && num4 < num6)
			{
				num6 = num4;
				b = 0;
			}
			if (flag3 && 1.0 - (double)num4 < (double)num6)
			{
				num6 = 1f - num4;
				b = 1;
			}
			if (flag4 && num5 < num6)
			{
				num6 = num5;
				b = 4;
			}
			if (flag5 && 1f - num5 < num6)
			{
				b = 5;
			}
			float num7 = 0.1f;
			if (b == 0)
			{
				this.motion.x = -num7;
			}
			if (b == 1)
			{
				this.motion.x = num7;
			}
			if (b == 4)
			{
				this.motion.z = -num7;
			}
			if (b == 5)
			{
				this.motion.z = num7;
			}
			if (b != 255)
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x000F2CFF File Offset: 0x000F0EFF
	public override void Move(Vector3 _direction, bool _isDirAbsolute, float _velocity, float _maxVelocity)
	{
		base.Move(_direction, _isDirAbsolute, _velocity, _maxVelocity);
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x000F2D0C File Offset: 0x000F0F0C
	public override void SetAlive()
	{
		base.SetAlive();
		if (this.PhysicsTransform != null)
		{
			this.PhysicsTransform.gameObject.layer = 20;
		}
		if (this.m_vp_FPController != null)
		{
			this.m_vp_FPController.Player.Dead.Stop(0f);
		}
		this.SetModelLayer(24, false, null);
		this.ShowHoldingItemLayer(true);
		this.bPlayerStatsChanged = true;
		this.bPlayerEquipmentChanged = true;
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x000F2D88 File Offset: 0x000F0F88
	[PublicizedFrom(EAccessModifier.Protected)]
	public new void LateUpdate()
	{
		if (this.bLerpCameraFlag)
		{
			this.lerpCameraLerpValue += Time.deltaTime * 4f;
			if (this.lerpCameraLerpValue >= 1f)
			{
				this.bLerpCameraFlag = false;
				this.playerCamera.fieldOfView = this.lerpCameraEndFOV;
			}
			else
			{
				this.playerCamera.fieldOfView = Mathf.Lerp(this.lerpCameraStartFOV, this.lerpCameraEndFOV, this.lerpCameraLerpValue);
			}
		}
		if (!this.AimingGun)
		{
			float num = (float)this.GetCameraFOV();
			float a = this.playerCamera.fieldOfView;
			if (this.lerpCameraLerpValue <= 0f || !this.bLerpCameraFlag)
			{
				a = num;
			}
			this.playerCamera.fieldOfView = Mathf.Lerp(a, num * Constants.cRunningFOVMultiplier, this.lerpCameraFastFOV);
			if (this.OverrideFOV != -1f)
			{
				this.playerCamera.fieldOfView = this.OverrideFOV;
				this.playerCamera.transform.LookAt(this.OverrideLookAt - Origin.position);
			}
			else if (this.lastOverrideFOV != -1f)
			{
				this.playerCamera.fieldOfView = num;
			}
			this.lastOverrideFOV = this.OverrideFOV;
		}
		this.TPCameraCheckResult = this.CharacterCameraAngleValid();
		this.WorldBoundsUpdate();
	}

	// Token: 0x17000475 RID: 1141
	// (get) Token: 0x0600278D RID: 10125 RVA: 0x000F2EC9 File Offset: 0x000F10C9
	public bool TPCameraCheckPassed
	{
		get
		{
			return this.TPCameraCheckResult == eTPCameraCheckResult.Pass;
		}
	}

	// Token: 0x0600278E RID: 10126 RVA: 0x000F2ED4 File Offset: 0x000F10D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void WorldBoundsUpdate()
	{
		this.InWorldPercent = this.world.InBoundsForPlayersPercent(this.position);
		this.InWorldLookPercent = this.InWorldPercent;
		if (this.InWorldPercent < 1f && !this.AttachedToEntity && !this.IsFlyMode.Value)
		{
			Vector3 forward = this.playerCamera.transform.forward;
			this.InWorldLookPercent = this.world.InBoundsForPlayersPercent(this.position + forward * 80f * 0.3f);
			if (this.InWorldPercent <= 0.05f)
			{
				Vector3i vector3i;
				Vector3i vector3i2;
				this.world.GetWorldExtent(out vector3i, out vector3i2);
				Vector2 vector;
				vector.x = (float)(vector3i.x + vector3i2.x) * 0.5f;
				vector.y = (float)(vector3i.z + vector3i2.z) * 0.5f;
				Vector3 normalized = new Vector3(vector.x - this.position.x, 0f, vector.y - this.position.z).normalized;
				float num = (1f - this.InWorldPercent / 0.05f) * 0.8f * Time.deltaTime;
				this.m_vp_FPController.AddForce(normalized.x * num, 0f, normalized.z * num);
				GameManager.ShowTooltip(this, Localization.Get("ttWorldEnd", false, null), false, false, 0f);
			}
		}
	}

	// Token: 0x0600278F RID: 10127 RVA: 0x000F3064 File Offset: 0x000F1264
	public void UnderwaterCameraFrameUpdate()
	{
		bool flag = this.UnderwaterCameraCheck();
		if (this.IsUnderwaterCamera != flag)
		{
			this.IsUnderwaterCamera = flag;
			Shader.SetGlobalFloat("_UnderWater", (float)(flag ? 1 : 0));
			if (this.uwEffectHaze)
			{
				this.uwEffectHaze.gameObject.SetActive(flag);
			}
			if (this.uwEffectRefract)
			{
				this.uwEffectRefract.gameObject.SetActive(flag);
			}
			if (this.uwEffectDebris)
			{
				this.uwEffectDebris.gameObject.SetActive(flag);
			}
			if (!flag)
			{
				if (this.uwEffectDroplets)
				{
					this.uwEffectDroplets.gameObject.SetActive(true);
					this.uwEffectDroplets.GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = true;
					this.uwEffectDroplets.GetComponent<ParticleSystem>().Play();
					this.uwEffectDroplets.GetComponent<ParticleSystem>().Emit(this.rand.RandomRange(60, 120));
				}
				if (this.uwEffectWaterFade)
				{
					this.uwEffectWaterFade.gameObject.SetActive(true);
					this.uwEffectWaterFade.GetComponent<ParticleSystem>().GetComponent<Renderer>().enabled = true;
					this.uwEffectWaterFade.GetComponent<ParticleSystem>().Play();
					this.uwEffectWaterFade.GetComponent<ParticleSystem>().Emit(1);
				}
			}
		}
	}

	// Token: 0x06002790 RID: 10128 RVA: 0x000F31B8 File Offset: 0x000F13B8
	public bool UnderwaterCameraCheck()
	{
		Vector3 pos = this.cameraTransform.position + Origin.position;
		pos.y += 0.28f;
		if (this.UnderwaterCameraCheckPos(pos))
		{
			return true;
		}
		Vector2 forwardVector = base.GetForwardVector2();
		pos.x -= forwardVector.x * 0.3f;
		pos.z -= forwardVector.y * 0.3f;
		return this.UnderwaterCameraCheckPos(pos);
	}

	// Token: 0x06002791 RID: 10129 RVA: 0x000F3238 File Offset: 0x000F1438
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UnderwaterCameraCheckPos(Vector3 pos)
	{
		Vector3i vector3i = World.worldToBlockPos(pos);
		float waterPercent = this.world.GetWaterPercent(vector3i);
		return waterPercent > 0f && (float)vector3i.y + waterPercent - pos.y > 0f;
	}

	// Token: 0x06002792 RID: 10130 RVA: 0x000F327A File Offset: 0x000F147A
	public override bool IsHeadUnderwater()
	{
		return this.inWaterPercent >= 0.791f;
	}

	// Token: 0x06002793 RID: 10131 RVA: 0x000F328C File Offset: 0x000F148C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHeadUnderwaterStateChanged(bool _bUnderwater)
	{
		base.OnHeadUnderwaterStateChanged(_bUnderwater);
		if (_bUnderwater)
		{
			this.Buffs.SetCustomVar("_underwater", 1f, true, CVarOperation.set, false);
		}
		else if (!this.IsDead())
		{
			this.Buffs.SetCustomVar("_underwater", 0f, true, CVarOperation.set, false);
			if (this.soundWaterSurface != null && Time.time - this.lastTimeUnderwater > 3f)
			{
				Manager.BroadcastPlay(this, this.soundWaterSurface, false, 1f);
			}
		}
		else
		{
			this.Buffs.SetCustomVar("_underwater", 0f, true, CVarOperation.set, false);
		}
		this.lastTimeUnderwater = Time.time;
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x000F332F File Offset: 0x000F152F
	public override void SetDead()
	{
		base.SetDead();
		this.lastHitDirection = Utils.EnumHitDirection.None;
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CameraDOFInit()
	{
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CameraDOFFrameUpdate()
	{
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x000F3340 File Offset: 0x000F1540
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal.HolsterState getCurrentHolsterState()
	{
		EntityPlayerLocal.HolsterState result = EntityPlayerLocal.HolsterState.Unholstered;
		AvatarMultiBodyController avatarMultiBodyController = this.emodel.avatarController as AvatarMultiBodyController;
		Animator animator = null;
		if (avatarMultiBodyController != null && avatarMultiBodyController.BodyAnimators.Count >= 2 && avatarMultiBodyController.BodyAnimators[1] != null)
		{
			animator = avatarMultiBodyController.BodyAnimators[1].Animator;
		}
		if (avatarMultiBodyController != null && animator != null)
		{
			AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(1);
			if (currentAnimatorClipInfo.Length != 0)
			{
				result = EntityPlayerLocal.HolsterState.Undefined;
				foreach (AnimatorClipInfo animatorClipInfo in currentAnimatorClipInfo)
				{
					if (animatorClipInfo.clip.name == "FP_Unarmed_Additive_HolsteHold" && animatorClipInfo.weight == 1f)
					{
						result = EntityPlayerLocal.HolsterState.Holstered;
						break;
					}
				}
			}
		}
		else
		{
			result = this.lastHolsterState;
		}
		return result;
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000F3410 File Offset: 0x000F1610
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (this.desiredHolsterState != EntityPlayerLocal.HolsterState.Undefined)
		{
			EntityPlayerLocal.HolsterState currentHolsterState = this.getCurrentHolsterState();
			if (currentHolsterState != EntityPlayerLocal.HolsterState.Undefined && currentHolsterState != this.desiredHolsterState)
			{
				this.lastHolsterState = currentHolsterState;
				this.emodel.avatarController.UpdateBool(AvatarController.holsteredHash, this.desiredHolsterState == EntityPlayerLocal.HolsterState.Holstered, true);
				this.desiredHolsterState = EntityPlayerLocal.HolsterState.Undefined;
			}
		}
		this.renderManager.FrameUpdate();
		if (this.bPlayingSpawnIn)
		{
			WeatherManager.Instance.PushTransitions();
		}
		this.CameraDOFFrameUpdate();
		float time = Time.time;
		bool flag = this.IsDead();
		if (this.Spawned != this.wasSpawned)
		{
			if (!flag && this.lastRespawnReason != RespawnType.Teleport)
			{
				this.bPlayingSpawnIn = true;
				this.spawnInTime = time;
				Manager.PlayInsidePlayerHead("spawnInStinger", -1, 0f, false, false);
				this.HolsterWeapon(false);
			}
			this.wasSpawned = this.Spawned;
		}
		if (flag)
		{
			if (this.deathTime == 0f)
			{
				this.deathTime = time;
				Manager.PlayInsidePlayerHead("player_death_stinger", this.entityId);
			}
		}
		else if (this.deathTime > 0f)
		{
			this.ClearScreenEffects();
			this.deathTime = 0f;
		}
		float num = (float)this.Health;
		if (num < this.dyingEffectHealthLast - 3f)
		{
			this.dyingEffectCur = Mathf.Clamp01(1f - Mathf.Clamp(num, 0f, 70f) / 70f);
			this.dyingEffectHitTime = time;
		}
		this.dyingEffectHealthLast = num;
		this.dyingEffectCur *= Mathf.Clamp01(1f - (time - this.dyingEffectHitTime) / 600f);
		if (this.dyingEffectCur < 0.01f)
		{
			this.dyingEffectCur = 0f;
		}
		if (this.dyingEffectLast != this.dyingEffectCur)
		{
			this.dyingEffectLast = this.dyingEffectCur;
			if (!flag)
			{
				this.ScreenEffectManager.SetScreenEffect("Dying", this.dyingEffectCur, 0f);
			}
		}
		if (this.bPlayingSpawnIn && time < this.spawnInTime + EntityPlayerLocal.spawnInEffectSpeed)
		{
			this.spawnInIntensity = Mathf.Clamp01(1f - (time - this.spawnInTime) / EntityPlayerLocal.spawnInEffectSpeed);
			this.ScreenEffectManager.SetScreenEffect("VibrantDeSat", this.spawnInIntensity, 4f);
			this.bPlayingSpawnIn = (this.spawnInIntensity > 0f);
		}
		else if (this.spawnInIntensity > 0f)
		{
			this.spawnInIntensity = 0f;
			this.ScreenEffectManager.SetScreenEffect("VibrantDeSat", 0f, 4f);
			this.bPlayingSpawnIn = false;
		}
		if ((double)base.GetCVar("_underwater") > 0.1 && !this.equipment.HasAnyItems() && this.biomeStandingOn != null && this.biomeStandingOn.m_sBiomeName == "snow")
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.SetAchievementStat(EnumAchievementDataStat.SubZeroNakedSwim, 1);
			}
		}
		ProgressionValue progressionValue = this.Progression.GetProgressionValue("attFortitude");
		if (progressionValue != null)
		{
			IAchievementManager achievementManager2 = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager2 != null)
			{
				achievementManager2.SetAchievementStat(EnumAchievementDataStat.HighestFortitude, progressionValue.Level);
			}
		}
		IAchievementManager achievementManager3 = PlatformManager.NativePlatform.AchievementManager;
		if (achievementManager3 != null)
		{
			achievementManager3.SetAchievementStat(EnumAchievementDataStat.HighestGamestage, base.gameStage);
		}
		IAchievementManager achievementManager4 = PlatformManager.NativePlatform.AchievementManager;
		if (achievementManager4 != null)
		{
			achievementManager4.SetAchievementStat(EnumAchievementDataStat.HighestPlayerLevel, this.Progression.Level);
		}
		if (base.RentedVMPosition != Vector3i.zero && this.RentalEndDay <= GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime))
		{
			base.RentedVMPosition = Vector3i.zero;
			this.RentalEndTime = 0UL;
			this.RentalEndDay = 0;
		}
		this.sneakDamageBlendTimer.Tick(Time.deltaTime);
		this.ThreatLevel.Numeric = ThreatLevelUtility.GetThreatLevelOn(this);
		if (this.Spawned)
		{
			AudioListener.volume = Mathf.Lerp(AudioListener.volume, GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel), Time.deltaTime);
		}
		float num2 = GamePrefs.GetFloat(EnumGamePrefs.OptionsAmbientVolumeLevel) * this.biomeVolume;
		if (this.audioSourceBiomeActive.isPlaying && Utils.FastAbs(this.audioSourceBiomeActive.volume - num2 * 0.95f) > 0.01f)
		{
			this.audioSourceBiomeActive.volume = Mathf.Lerp(this.audioSourceBiomeActive.volume, num2, Time.deltaTime);
			if (this.audioSourceBiomeActive.volume > num2 * 0.95f)
			{
				this.audioSourceBiomeActive.volume = num2;
			}
		}
		if (this.audioSourceBiomeFadeOut.isPlaying && this.audioSourceBiomeFadeOut.volume > 0.001f)
		{
			this.audioSourceBiomeFadeOut.volume = Mathf.Lerp(this.audioSourceBiomeFadeOut.volume, 0f, Time.deltaTime);
			if ((double)this.audioSourceBiomeFadeOut.volume < 0.05)
			{
				this.audioSourceBiomeFadeOut.clip = null;
				this.audioSourceBiomeFadeOut.Stop();
			}
		}
		this.FrameUpdateCamera();
		if (!GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		this.ShelterFrameUpdate();
		if (this.emodel.IsRagdollActive || (this.IsDead() && this.bSwitchCameraBackAfterRespawn))
		{
			this.SelfCameraFrameUpdate();
		}
		if (this.inventory.IsHoldingGun() && this.inventory.GetHoldingGun() is ItemActionRanged)
		{
			float num3 = (float)Screen.width / this.cameraTransform.GetComponent<Camera>().fieldOfView;
			float num4 = EffectManager.GetValue(PassiveEffects.SpreadDegreesHorizontal, this.inventory.holdingItemData.itemValue, 90f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * num3;
			float num5 = EffectManager.GetValue(PassiveEffects.SpreadDegreesVertical, this.inventory.holdingItemData.itemValue, 90f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * num3;
			if (num4 > num5)
			{
				this.crossHairOpenArea = (float)((int)num4) * (this.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
			}
			else
			{
				this.crossHairOpenArea = (float)((int)num5) * (this.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
			}
		}
		else
		{
			Vector3 vector = this.prevPos - this.position;
			float b = Mathf.Max(20f, Mathf.Clamp01(Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z) * 3f) * 100f);
			this.crossHairOpenArea = Mathf.Lerp(this.crossHairOpenArea, b, Time.deltaTime * 4f);
		}
		if (this.autoMove != null)
		{
			this.autoMove.Update();
		}
		List<Vector3i> obj = this.backpackPositionsFromThread;
		lock (obj)
		{
			if (this.backpackPositionsFromThread.Count > 0)
			{
				this.SetDroppedBackpackPositions(this.backpackPositionsFromThread);
				this.backpackPositionsFromThread.Clear();
			}
		}
		if (base.IsAlive())
		{
			this.recoveryPointTimer += Time.deltaTime;
			if (this.recoveryPointTimer > 30f && this.onGround)
			{
				this.TryAddRecoveryPosition(Vector3i.FromVector3Rounded(this.position));
				this.recoveryPointTimer = 0f;
			}
		}
		if (this.sharedQuestsToProcess.Count > 0 && this.IsSpawned())
		{
			foreach (NetPackageSharedQuest.SharedQuestData sharedQuestData in this.sharedQuestsToProcess)
			{
				if (this.QuestJournal.AddSharedQuestEntry(sharedQuestData) && !PartyQuests.AutoAccept)
				{
					string arg = "?";
					PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(sharedQuestData.sharedByEntityID);
					if (playerDataFromEntityID != null)
					{
						arg = playerDataFromEntityID.PlayerName.DisplayName;
					}
					GameManager.ShowTooltip(this, string.Format(Localization.Get("ttQuestShared", false, null), arg, QuestClass.GetQuest(sharedQuestData.questID).Name), string.Empty, "ui_quest_invite", null, false, false, 0f);
				}
			}
			this.sharedQuestsToProcess.Clear();
		}
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000F3C54 File Offset: 0x000F1E54
	public void AddSharedQuestEntry(NetPackageSharedQuest.SharedQuestData sqd)
	{
		this.sharedQuestsToProcess.Add(sqd);
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000F3C64 File Offset: 0x000F1E64
	public void HandleHordeEvent(AIDirector.HordeEvent msg)
	{
		string text = null;
		if (msg != AIDirector.HordeEvent.Warn2)
		{
			if (msg == AIDirector.HordeEvent.Spawn)
			{
				GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 1f, 50f, 5f));
				text = "Enemies/Horde/horde_spawn";
			}
		}
		else
		{
			text = "Enemies/Horde/horde_spawn_warning";
		}
		if (text != null)
		{
			Manager.PlayInsidePlayerHead(text, -1, 0f, false, false);
		}
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000F3CC4 File Offset: 0x000F1EC4
	public override void OnFired()
	{
		base.OnFired();
		Vector2 vector = new Vector2(EffectManager.GetValue(PassiveEffects.KickDegreesHorizontalMin, this.inventory.holdingItemItemValue, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false), EffectManager.GetValue(PassiveEffects.KickDegreesHorizontalMax, this.inventory.holdingItemItemValue, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		Vector2 vector2 = new Vector2(EffectManager.GetValue(PassiveEffects.KickDegreesVerticalMin, this.inventory.holdingItemItemValue, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false), EffectManager.GetValue(PassiveEffects.KickDegreesVerticalMax, this.inventory.holdingItemItemValue, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		if (vector.x != 0f || vector.y != 0f || vector2.x != 0f || vector2.y != 0f)
		{
			switch (this.inventory.holdingItem.GetCameraShakeType(this.inventory.holdingItemData))
			{
			case EnumCameraShake.Tiny:
				GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 0.5f, 5f, 1f));
				break;
			case EnumCameraShake.Small:
				GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 0.5f, 10f, 1f));
				break;
			case EnumCameraShake.Big:
				GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 0.5f, 20f, 1f));
				break;
			}
		}
		TriggerEffectManager.ControllerTriggerEffect controllerTriggerEffectShoot = this.inventory.holdingItem.GetControllerTriggerEffectShoot();
		if (controllerTriggerEffectShoot.XboxTriggerEffect.Effect != TriggerEffectManager.EffectXbox.Off || controllerTriggerEffectShoot.DualsenseEffect.Effect != TriggerEffectManager.EffectDualsense.Off)
		{
			GameManager.Instance.triggerEffectManager.SetTriggerEffect(TriggerEffectManager.GamepadTrigger.RightTrigger, controllerTriggerEffectShoot, false);
		}
		if (!this.AimingGun)
		{
			MovementInput movementInput = this.movementInput;
			movementInput.rotation.x = movementInput.rotation.x + this.rand.RandomRange(vector2.x, vector2.y) * 2f;
			MovementInput movementInput2 = this.movementInput;
			movementInput2.rotation.y = movementInput2.rotation.y + this.rand.RandomRange(vector.x, vector.y) * 2f;
			return;
		}
		MovementInput movementInput3 = this.movementInput;
		movementInput3.rotation.x = movementInput3.rotation.x + this.rand.RandomRange(vector2.x, vector2.y);
		MovementInput movementInput4 = this.movementInput;
		movementInput4.rotation.y = movementInput4.rotation.y + this.rand.RandomRange(vector.x, vector.y);
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000F3F69 File Offset: 0x000F2169
	public int GetCrosshairOpenArea()
	{
		return (int)this.crossHairOpenArea;
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x000F3F74 File Offset: 0x000F2174
	public Vector2 GetCrosshairPosition2D()
	{
		Vector3 vector = this.finalCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f));
		return new Vector2(vector.x, vector.y);
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x000F3FB4 File Offset: 0x000F21B4
	public Vector3 GetCrosshairPosition3D(float _z, float _attributeOffset2D, Vector3 _altStartPosition)
	{
		if (!this.playerCamera.enabled || this.movementInput.bCameraChange)
		{
			return _altStartPosition;
		}
		Vector2 crosshairPosition2D = this.GetCrosshairPosition2D();
		crosshairPosition2D.x += this.rand.RandomRange(-_attributeOffset2D, _attributeOffset2D) * 700f;
		crosshairPosition2D.y += this.rand.RandomRange(-_attributeOffset2D, _attributeOffset2D) * 700f;
		Vector3 vector = this.playerCamera.ScreenToWorldPoint(new Vector3(crosshairPosition2D.x, crosshairPosition2D.y, _z)) + Origin.position;
		if (!this.bFirstPersonView)
		{
			float num = this.vp_FPCamera.CurrentCameraDistance;
			if (Voxel.Raycast(GameManager.Instance.World, new Ray(vector, this.GetLookVector()), num + 1f, true, false))
			{
				num = Vector3.Distance(Voxel.phyxRaycastHit.point, vector - Origin.position) - 0.5f;
			}
			vector += this.GetLookVector() * num;
		}
		return vector;
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x000F40B9 File Offset: 0x000F22B9
	public override void OnHoldingItemChanged()
	{
		if (!this.IsDead())
		{
			this.SetModelLayer(24, false, null);
		}
	}

	// Token: 0x060027A0 RID: 10144 RVA: 0x000F40D0 File Offset: 0x000F22D0
	public override void SetModelLayer(int _layerId, bool force = false, string[] excludeTags = null)
	{
		if (this.emodel == null)
		{
			return;
		}
		Transform modelTransform = this.emodel.GetModelTransform();
		if (modelTransform == null)
		{
			return;
		}
		if (this.oldLayer != _layerId || force)
		{
			this.oldLayer = _layerId;
			Utils.SetLayerWithExclusionList(modelTransform.gameObject, _layerId, excludeTags);
			if (_layerId == 24 && modelTransform.childCount > 0)
			{
				Utils.SetLayerWithExclusionList(modelTransform.GetChild(0).gameObject, _layerId, excludeTags);
			}
			modelTransform.gameObject.GetComponentsInChildren<Collider>(true, EntityPlayerLocal.setLayerRecursivelyList);
			for (int i = EntityPlayerLocal.setLayerRecursivelyList.Count - 1; i >= 0; i--)
			{
				Utils.SetLayerWithExclusionList(EntityPlayerLocal.setLayerRecursivelyList[i].gameObject, _layerId, excludeTags);
			}
			EntityPlayerLocal.setLayerRecursivelyList.Clear();
		}
	}

	// Token: 0x060027A1 RID: 10145 RVA: 0x000F4190 File Offset: 0x000F2390
	public virtual void MoveByInput()
	{
		bool isCrouching = base.IsCrouching;
		if (this.IsStuck || EffectManager.GetValue(PassiveEffects.DisableMovement, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f)
		{
			this.movementInput.Clear();
		}
		if (EffectManager.GetValue(PassiveEffects.FlipControls, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f)
		{
			this.movementInput.moveForward *= -1f;
			this.movementInput.moveStrafe *= -1f;
		}
		if (this.AttachedToEntity != null)
		{
			this.Crouching = false;
			this.CrouchingLocked = false;
			base.Climbing = false;
			this.MovementRunning = false;
			this.AimingGun = false;
			this.AttachedToEntity.MoveByAttachedEntity(this);
		}
		else
		{
			bool flag = false;
			this.moveDirection.x = this.movementInput.moveStrafe;
			this.moveDirection.z = this.movementInput.moveForward;
			if (this.moveDirection.x != 0f || this.moveDirection.z != 0f)
			{
				flag = true;
			}
			bool flag2 = (!base.IsSwimming()) ? (!this.bExhausted && this.moveDirection.z > 0f) : (this.swimExhaustedTicks == 0);
			bool flag3 = this.movementInput.running && flag;
			if (!this.IsFlyMode.Value)
			{
				flag3 = (flag3 && flag2);
			}
			this.MovementRunning = flag3;
			if (base.IsSwimming())
			{
				if (!this.IsSwimmingMoving() || this.swimExhaustedTicks > 0)
				{
					this.CurrentMovementTag = EntityAlive.MovementTagFloating;
				}
				else if (!this.MovementRunning)
				{
					this.CurrentMovementTag = EntityAlive.MovementTagSwimming;
				}
				else
				{
					this.CurrentMovementTag = EntityAlive.MovementTagSwimmingRun;
				}
			}
			else if (flag)
			{
				if (!this.MovementRunning)
				{
					this.CurrentMovementTag = EntityAlive.MovementTagWalking;
				}
				else
				{
					this.CurrentMovementTag = EntityAlive.MovementTagRunning;
				}
			}
			else
			{
				this.CurrentMovementTag = EntityAlive.MovementTagIdle;
			}
			if (this.movementInput.downToggle)
			{
				this.CrouchingLocked = !this.CrouchingLocked;
			}
			this.CrouchingLocked = (this.CrouchingLocked && !this.isLadderAttached && !this.movementInput.down);
			this.Crouching = (!this.IsFlyMode.Value && !this.isLadderAttached && (this.movementInput.down || this.CrouchingLocked));
			if (!this.AimingGun)
			{
				if (!this.IsFlyMode.Value)
				{
					if (!this.JetpackWearing)
					{
						if (this.movementInput.jump && this.vp_FPController && !this.inputWasJump)
						{
							this.vp_FPController.enabled = true;
						}
						if (!this.Jumping && !this.wasJumping && this.movementInput.jump && (this.onGround || this.isLadderAttached) && this.AttachedToEntity == null)
						{
							this.jumpTrigger = true;
						}
						else if (this.wasLadderAttachedJump && !this.isLadderAttached && this.movementInput.jump && !this.inputWasJump)
						{
							this.canLadderAirAttach = true;
						}
					}
					else
					{
						if (this.movementInput.jump)
						{
							this.motion.y = this.motion.y + 0.15f;
							flag = true;
						}
						if (this.movementInput.down)
						{
							this.motion.y = this.motion.y - 0.15f;
						}
					}
				}
				else
				{
					if (this.movementInput.jump)
					{
						if (this.movementInput.running)
						{
							this.motion.y = 0.9f;
						}
						else
						{
							this.motion.y = 0.3f * this.GodModeSpeedModifier;
						}
					}
					if (this.movementInput.down)
					{
						if (this.movementInput.running)
						{
							this.motion.y = -0.9f;
						}
						else
						{
							this.motion.y = -0.3f * this.GodModeSpeedModifier;
						}
					}
				}
			}
			this.JetpackActive = (this.JetpackWearing && flag);
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			bool isCrouching2 = base.IsCrouching;
			if (isCrouching2 != isCrouching)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityStealth>().Setup(this, isCrouching2), false);
			}
		}
		if (!this.bFirstPersonView && this.vp_FPCamera.Locked3rdPerson)
		{
			Vector3 right = base.transform.right;
			Vector3 forward = base.transform.forward;
			right.y = 0f;
			forward.y = 0f;
			right.Normalize();
			forward.Normalize();
			this.moveDirection = forward * this.moveDirection.z + right * this.moveDirection.x;
		}
		else if (!this.bFirstPersonView && this.CameraRelativeMovement)
		{
			Vector3 right2 = this.playerCamera.transform.right;
			Vector3 forward2 = this.playerCamera.transform.forward;
			right2.y = 0f;
			forward2.y = 0f;
			right2.Normalize();
			forward2.Normalize();
			this.moveDirection = forward2 * this.moveDirection.z + right2 * this.moveDirection.x;
		}
		if (this.vp_FPController != null)
		{
			if (this.AttachedToEntity == null)
			{
				this.vp_FPController.Player.InputMoveVector.Set(new Vector2(this.moveDirection.x, this.moveDirection.z));
			}
			this.vp_FPController.Player.InputSmoothLook.Set(new Vector2(this.movementInput.rotation.y, -this.movementInput.rotation.x));
		}
		this.inputWasJump = this.movementInput.jump;
		this.inputWasDown = this.movementInput.down;
		this.movementInput.Clear();
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x000F47DC File Offset: 0x000F29DC
	public void SwitchFirstPersonViewFromInput()
	{
		if (this.vp_FPCamera != null && this.vp_FPCamera.Locked3rdPerson)
		{
			return;
		}
		if (this.AttachedToEntity != null)
		{
			return;
		}
		if (this.inventory.IsHoldingItemActionRunning())
		{
			return;
		}
		if (!GameManager.Instance.IsEditMode() && GameStats.GetInt(EnumGameStats.CameraRestrictionMode) > 0)
		{
			GameManager.ShowTooltip(this, Localization.Get("ttCameraRestricted", false, null), false, false, 0f);
			return;
		}
		bool flag = !this.bFirstPersonView;
		this.SetFirstPersonView(flag, true);
		if (!this.bFirstPersonView)
		{
			vp_FPCamera vp_FPCamera = this.vp_FPCamera;
			if (vp_FPCamera)
			{
				vp_FPCamera.m_Current3rdPersonBlend = 1f;
			}
		}
		this.bPreferFirstPerson = this.bFirstPersonView;
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x000F4894 File Offset: 0x000F2A94
	public void SwitchToPreferredCameraMode(bool _lerpPosition)
	{
		int @int = GameStats.GetInt(EnumGameStats.CameraRestrictionMode);
		if (GameManager.Instance.IsEditMode() || @int == 0)
		{
			this.SetFirstPersonView(this.bPreferFirstPerson, _lerpPosition);
			return;
		}
		this.SetFirstPersonView(@int == 1, _lerpPosition);
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x000F48D4 File Offset: 0x000F2AD4
	public void SetFirstPersonView(bool _bFirstPersonView, bool _bLerpPosition)
	{
		this.bFirstPersonView = _bFirstPersonView;
		base.SetCVar(".IsFPV", (float)(_bFirstPersonView ? 1 : 0));
		this.FireEvent(MinEventTypes.onSelfChangedView, true);
		if (this.bFirstPersonView)
		{
			this.SetCameraAttachedToPlayer(true, false);
			this.switchModelView(EnumEntityModelView.FirstPerson);
			this.lineOfSightObstructionPoint = Vector3.zero;
		}
		else
		{
			this.SetCameraAttachedToPlayer(false, false);
			this.switchModelView(EnumEntityModelView.ThirdPerson);
			this.vp_PlayerEventHandler.CameraRelativeMovement3P.Set(this.CameraRelativeMovement);
		}
		this.UpdateCameraFOV(_bLerpPosition);
		this.refreshHolsterState();
		this.characterMatrixOverride.enabled = !this.bFirstPersonView;
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x000F4974 File Offset: 0x000F2B74
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void switchModelView(EnumEntityModelView modelView)
	{
		base.switchModelView(modelView);
		this.SetModelLayer(24, true, null);
		if (this.vp_FPController != null)
		{
			this.vp_FPController.Player.IsFirstPerson.Set(modelView == EnumEntityModelView.FirstPerson);
		}
		this.emodel.avatarController.SetCrouching(base.IsCrouching);
		this.emodel.avatarController.SetSwim(base.IsSwimming());
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x000F49EC File Offset: 0x000F2BEC
	public override void BeforePlayerRespawn(RespawnType _type)
	{
		base.BeforePlayerRespawn(_type);
		ItemClassHeldEntity.CheckSpawnValues(this);
		switch (_type)
		{
		case RespawnType.NewGame:
		case RespawnType.LoadedGame:
		case RespawnType.Teleport:
			break;
		case RespawnType.Died:
			for (int i = 0; i < this.overlayDirectionTime.Length; i++)
			{
				this.overlayDirectionTime[i] = 0f;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x000F4A40 File Offset: 0x000F2C40
	public override void AfterPlayerRespawn(RespawnType _type)
	{
		if (_type == RespawnType.Teleport)
		{
			this.RespawnTeleportTimeoutLogged = false;
			this.ftwOutcomeReported = false;
			this.ftwOutcomeWatchActive = false;
		}
		this.desiredHolsterState = EntityPlayerLocal.HolsterState.Undefined;
		base.AfterPlayerRespawn(_type);
		this.m_vp_FPCamera.enabled = true;
		if (this.AttachedToEntity != null)
		{
			this.SetFirstPersonView(false, false);
		}
		else
		{
			this.SwitchToPreferredCameraMode(true);
		}
		if (!GameManager.Instance.IsEditMode() && (_type == RespawnType.NewGame || _type == RespawnType.EnterMultiplayer))
		{
			this.emodel.avatarController.PlayPlayerFPRevive();
		}
		if (this.world.IsEditor() || GameModeCreative.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)))
		{
			if (GameManager.Instance.IsEditMode() && !PrefabEditModeManager.Instance.IsActive())
			{
				SkyManager.SetFogDebug(0f, float.MinValue, float.MinValue);
				World world = GameManager.Instance.World;
				if (((world != null) ? world.BiomeAtmosphereEffects : null) != null)
				{
					GameManager.Instance.World.BiomeAtmosphereEffects.ForceDefault = true;
				}
			}
			if (this.Buffs != null && !this.Buffs.HasBuff("god"))
			{
				this.Buffs.AddBuff("god", -1, true, false, -1f);
			}
		}
		switch (_type)
		{
		case RespawnType.NewGame:
		case RespawnType.EnterMultiplayer:
		{
			this.SetAlive();
			this.Score = 0;
			if (this.world.IsEditor())
			{
				this.inventory.SetItem(1, new ItemValue(1, false), 64, true);
				this.inventory.SetHoldingItemIdx(0);
			}
			else
			{
				this.SetupStartingItems();
			}
			this.Buffs.UnPauseAll();
			SandboxOptionManager sandboxOptionManager = SandboxOptionManager.Current;
			sandboxOptionManager.LoadOptionsFromCode(GameStats.GetString(EnumGameStats.SandboxCode));
			sandboxOptionManager.UpdateInGameValuesWithSandboxOptions(false);
			this.AdjustItemsForSandboxOptions();
			this.FireEvent(MinEventTypes.onSelfFirstSpawn, true);
			this.FireEvent(MinEventTypes.onSelfEnteredGame, true);
			return;
		}
		case RespawnType.LoadedGame:
		case RespawnType.JoinMultiplayer:
		{
			SandboxOptionManager sandboxOptionManager2 = SandboxOptionManager.Current;
			sandboxOptionManager2.LoadOptionsFromCode(GameStats.GetString(EnumGameStats.SandboxCode));
			sandboxOptionManager2.UpdateInGameValuesWithSandboxOptions(false);
			this.AdjustItemsForSandboxOptions();
			this.FireEvent(MinEventTypes.onSelfEnteredGame, true);
			this.HandleMapObjects(true);
			return;
		}
		case RespawnType.Died:
			this.SetAlive();
			this.Health = this.GetMaxHealth();
			this.Stamina = (float)this.GetMaxStamina();
			this.Water = (float)this.GetMaxWater();
			base.Stats.Stamina.MaxModifier = 0f;
			this.CrouchingLocked = (this.Crouching = false);
			this.FireEvent(MinEventTypes.onSelfRespawn, true);
			Manager.StopLoopInsidePlayerHead("player_death_stinger_lp", this.entityId, false);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				Manager.Instance.StopDistantLoopingPositionalSounds(base.transform.position);
				return;
			}
			break;
		case RespawnType.Teleport:
			this.HolsterWeapon(false);
			this.FireEvent(MinEventTypes.onSelfTeleported, true);
			break;
		default:
			return;
		}
	}

	// Token: 0x060027A8 RID: 10152 RVA: 0x000F4CE0 File Offset: 0x000F2EE0
	public void SetupStartingItems()
	{
		for (int i = 0; i < this.itemsOnEnterGame.Count; i++)
		{
			ItemStack itemStack = this.itemsOnEnterGame[i];
			itemStack.itemValue.Meta = ItemClass.GetForId(itemStack.itemValue.type).GetInitialMetadata(itemStack.itemValue);
			this.inventory.SetItem(i + 1, itemStack);
		}
		this.inventory.SetHoldingItemIdx(0);
	}

	// Token: 0x060027A9 RID: 10153 RVA: 0x000F4D54 File Offset: 0x000F2F54
	public void AdjustItemsForSandboxOptions()
	{
		this.dragAndDropItem.AdjustForSandboxOptions();
		this.equipment.PerformActionOnSlots(delegate(ItemValue _iv)
		{
			_iv.AdjustForSandboxOptions();
		});
		this.inventory.PerformActionOnSlots(delegate(ItemStack _is)
		{
			_is.AdjustForSandboxOptions();
		});
		this.bag.PerformActionOnSlots(delegate(ItemStack _is)
		{
			_is.AdjustForSandboxOptions();
		});
	}

	// Token: 0x060027AA RID: 10154 RVA: 0x000F4DEC File Offset: 0x000F2FEC
	public override Vector3 GetCameraLook(float _t)
	{
		if (!this.bFirstPersonView)
		{
			return this.cameraTransform.forward.normalized;
		}
		return base.GetCameraLook(_t);
	}

	// Token: 0x060027AB RID: 10155 RVA: 0x000F4E1C File Offset: 0x000F301C
	public override Ray GetLookRay()
	{
		Ray result = this.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		result.origin += Origin.position;
		if (this.bFirstPersonView)
		{
			result.direction += this.playerCamera.transform.up * 0.0001f;
			return result;
		}
		result.origin += result.direction * this.m_vp_FPCamera.CurrentCameraDistance;
		result.direction += this.playerCamera.transform.up * 0.0001f;
		float num = Vector3.Distance(this.playerCamera.transform.position, this.LookPoint);
		float num2 = Vector3.Distance(this.playerCamera.transform.position, result.origin - Origin.position);
		if (num < num2)
		{
			result.origin = this.LookPoint;
			result.origin += Origin.position;
		}
		result.origin -= this.playerCamera.transform.forward * 0.15f;
		return result;
	}

	// Token: 0x060027AC RID: 10156 RVA: 0x000F4F84 File Offset: 0x000F3184
	public override Ray GetMeleeRay()
	{
		if (this.bFirstPersonView)
		{
			return this.GetLookRay();
		}
		Ray result;
		if (this.LineOfSightObstructed)
		{
			result = this.playerCamera.ViewportPointToRay(this.playerCamera.WorldToViewportPoint(this.lineOfSightObstructionPoint));
		}
		else
		{
			result = new Ray(this.cameraTransform.position + this.cameraTransform.forward * this.vp_FPCamera.CurrentCameraDistance, this.cameraTransform.forward);
		}
		result.origin += Origin.position;
		return result;
	}

	// Token: 0x060027AD RID: 10157 RVA: 0x000F501C File Offset: 0x000F321C
	public override Vector3 GetLookVector()
	{
		if (this.playerCamera.enabled)
		{
			return this.cameraTransform.forward;
		}
		return base.GetLookVector();
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x000F503D File Offset: 0x000F323D
	public override Vector3 GetLookVector(Vector3 _altLookVector)
	{
		if (!this.playerCamera.enabled)
		{
			return _altLookVector;
		}
		return this.GetLookVector();
	}

	// Token: 0x060027AF RID: 10159 RVA: 0x000027FC File Offset: 0x000009FC
	public static void CheckPos()
	{
	}

	// Token: 0x060027B0 RID: 10160 RVA: 0x000F5054 File Offset: 0x000F3254
	[PublicizedFrom(EAccessModifier.Private)]
	public void startDeathCamera()
	{
		if (this.bFirstPersonView)
		{
			this.SetFirstPersonView(false, true);
		}
		this.bSwitchCameraBackAfterRespawn = true;
		this.StartSelfCamera();
		this.selfCameraSeekPos = this.selfCameraPos - this.cameraTransform.forward * 2.8f;
		this.selfCameraSeekPos.y = this.selfCameraSeekPos.y + 2.2f;
		this.ScreenEffectManager.SetScreenEffect("Dying", 0.5f, 0.5f);
		this.ScreenEffectManager.SetScreenEffect("Dead", 1f, 0.5f);
		this.ScreenEffectManager.SetScreenEffect("FadeToBlack", 1f, (float)base.GetTimeStayAfterDeath() * 0.05f);
	}

	// Token: 0x060027B1 RID: 10161 RVA: 0x000F510E File Offset: 0x000F330E
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartSelfCamera()
	{
		this.selfCameraPos = this.cameraTransform.position + Origin.position;
		this.m_vp_FPCamera.enabled = false;
	}

	// Token: 0x060027B2 RID: 10162 RVA: 0x000F5138 File Offset: 0x000F3338
	[PublicizedFrom(EAccessModifier.Private)]
	public void SelfCameraFrameUpdate()
	{
		Vector3 vector = this.emodel.GetChestPosition();
		vector.y += 0.2f;
		if (this.selfCameraSeekPos.y < vector.y + 0.1f)
		{
			this.selfCameraSeekPos.y = this.selfCameraSeekPos.y + 0.05f;
		}
		this.selfCameraPos = Vector3.MoveTowards(this.selfCameraPos, this.selfCameraSeekPos, Time.deltaTime * 2.5f);
		Vector3 direction = this.selfCameraPos - vector;
		float magnitude = direction.magnitude;
		vector -= Origin.position;
		bool flag = false;
		float v = magnitude;
		float num = magnitude - 0.28f;
		RaycastHit raycastHit;
		if (num > 0f && Physics.SphereCast(vector, 0.28f, direction, out raycastHit, num, 65536))
		{
			v = Utils.FastMin(raycastHit.distance, v);
			flag = true;
		}
		if (!flag && Physics.Raycast(vector, direction, out raycastHit, magnitude + 0.28f, 65536))
		{
			v = raycastHit.distance - 0.28f;
			flag = true;
		}
		if (flag)
		{
			this.selfCameraPos = vector + direction.normalized * Utils.FastMax(0.01f, v) + Origin.position;
			this.selfCameraSeekPos = this.selfCameraPos;
		}
		Vector3 vector2 = this.selfCameraPos - Origin.position;
		this.cameraTransform.position = vector2;
		Quaternion b = Quaternion.LookRotation(vector - vector2);
		this.cameraTransform.rotation = Quaternion.Slerp(this.cameraTransform.rotation, b, 0.1f);
	}

	// Token: 0x060027B3 RID: 10163 RVA: 0x000F52C9 File Offset: 0x000F34C9
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearScreenEffects()
	{
		this.ScreenEffectManager.DisableScreenEffect("Dying");
		this.ScreenEffectManager.DisableScreenEffect("Dead");
		this.ScreenEffectManager.DisableScreenEffect("FadeToBlack");
	}

	// Token: 0x17000476 RID: 1142
	// (get) Token: 0x060027B4 RID: 10164 RVA: 0x000F52FB File Offset: 0x000F34FB
	public bool CancellingInventoryActions
	{
		get
		{
			return this.cancellingInventoryActions;
		}
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x000F5303 File Offset: 0x000F3503
	public IEnumerator CancelInventoryActions(Action cancelCallback, bool holsterWeapon)
	{
		this.cancellingInventoryActions = true;
		if (this.inventory.holdingItem.Actions != null && this.inventory.holdingItem.Actions.Length != 0 && this.inventory.holdingItemData.actionData != null && this.inventory.holdingItemData.actionData.Count > 0)
		{
			for (int i = 0; i < this.inventory.holdingItem.Actions.Length; i++)
			{
				if (this.inventory.holdingItem.Actions[i] != null && this.inventory.holdingItemData.actionData[i] != null)
				{
					if (this.inventory.holdingItem.Actions[i].IsActionRunning(this.inventory.holdingItemData.actionData[i]))
					{
						this.inventory.holdingItem.Actions[i].CancelAction(this.inventory.holdingItemData.actionData[i]);
					}
					this.inventory.holdingItem.Actions[i].CancelReload(this.inventory.holdingItemData.actionData[i], holsterWeapon);
				}
			}
		}
		while (this.IsReloading())
		{
			yield return null;
		}
		for (;;)
		{
			EntityPlayerLocal.HolsterState currentHolsterState = this.getCurrentHolsterState();
			if (this.desiredHolsterState == EntityPlayerLocal.HolsterState.Undefined || this.desiredHolsterState == currentHolsterState)
			{
				break;
			}
			yield return null;
		}
		cancelCallback();
		this.cancellingInventoryActions = false;
		yield break;
	}

	// Token: 0x060027B6 RID: 10166 RVA: 0x000F5320 File Offset: 0x000F3520
	public bool IsReloading()
	{
		if (this.inventory.holdingItemData.actionData != null)
		{
			foreach (ItemActionData itemActionData in this.inventory.holdingItemData.actionData)
			{
				ItemActionRanged.ItemActionDataRanged itemActionDataRanged = itemActionData as ItemActionRanged.ItemActionDataRanged;
				if (itemActionDataRanged != null)
				{
					return ItemActionRanged.Reloading(itemActionDataRanged);
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060027B7 RID: 10167 RVA: 0x000F539C File Offset: 0x000F359C
	public override void OnEntityDeath()
	{
		GameManager.Instance.TriggerSendOfLocalPlayerDataFile(0f);
		base.StartCoroutine(this.CancelInventoryActions(delegate
		{
		}, true));
		this.inventory.ReleaseAll(this.playerInput);
		this.inventory.SetActiveItemIndexOff();
		Manager.BroadcastStop(this.entityId, "Player" + (this.IsMale ? "Male" : "Female") + "RunLoop");
		this.sprintLoopSoundPlayId = -1;
		this.windowManager.CloseAllOpenModalWindows(null, false);
		this.windowManager.Close("windowpaging");
		GameManager.Instance.ClearTooltips(this.nguiWindowManager);
		this.windowManager.Open("death", false);
		this.AimingGun = false;
		this.BloodMoonParticipation = false;
		base.OnEntityDeath();
		this.startDeathCamera();
	}

	// Token: 0x060027B8 RID: 10168 RVA: 0x000F5490 File Offset: 0x000F3690
	public override void OnDeathUpdate()
	{
		base.OnDeathUpdate();
		if (this.Spawned && base.GetDeathTime() >= base.GetTimeStayAfterDeath())
		{
			this.windowManager.Close("death");
			if (this.ShouldRemoveEquipmentOnDeath(EntityPlayerLocal.DropOnDeathOption))
			{
				for (int i = 0; i < 12; i++)
				{
					ItemValue slotItem = this.equipment.GetSlotItem(i);
					if (this.dropValueCondition(slotItem))
					{
						this.equipment.SetSlotItem(i, null, true);
					}
				}
			}
			this.Respawn(RespawnType.Died);
		}
	}

	// Token: 0x060027B9 RID: 10169 RVA: 0x000F5514 File Offset: 0x000F3714
	[PublicizedFrom(EAccessModifier.Private)]
	public void FrameUpdateCamera()
	{
		this.UnderwaterCameraFrameUpdate();
		if (!this.bFirstPersonView)
		{
			if (!(this.AttachedToEntity is EntityVehicle))
			{
				bool flag = this.inventory.holdingItem.Actions[0] is ItemActionRanged || this.inventory.holdingItem.Actions[1] is ItemActionRanged;
				int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfx3PCameraMode);
				if (@int == 1)
				{
					this.StartTPCameraLockTimer();
				}
				else if (@int == 2 && flag)
				{
					this.StartTPCameraLockTimer();
				}
				else if (this.IsGodMode.Value || this.IsFlyMode.Value || this.AttachedToEntity != null || this.isLadderAttached || this.isSwimming)
				{
					this.StartTPCameraLockTimer();
				}
				else if (this.tpCameraLockTimerActive && Time.time - this.tpCameraLockStartTime > 2f && !this.AimingGun)
				{
					this.tpCameraLockTimerActive = false;
					this.CameraRelativeMovement = true;
				}
				bool flag2 = false;
				float num = 0f;
				RaycastHit raycastHit;
				if (Physics.Raycast(new Ray(base.transform.position, Vector3.up), out raycastHit, 8f, 1073807360))
				{
					flag2 = true;
					num = raycastHit.distance;
					if (this.overheadObstructionTime < 0.5f)
					{
						this.overheadObstructionTime += Time.deltaTime;
					}
				}
				else
				{
					this.overheadObstructionTime = 0f;
				}
				float num2 = 1f;
				if (flag2 && this.overheadObstructionTime >= 0.5f)
				{
					if (num < 3f)
					{
						num2 = 0.5f;
					}
					else if (num < 6f)
					{
						num2 = 0.65f;
					}
					else
					{
						num2 = 0.8f;
					}
				}
				if (this.MovementRunning)
				{
					num2 *= 1.5f;
				}
				this.camOverheadDistance = Utils.FastLerp(this.camOverheadDistance, num2, Time.deltaTime * 1.5f);
				vp_FPCamera vp_FPCamera = this.m_vp_FPCamera;
				if (this.Crouching && !this.isSwimming)
				{
					if (this.AimingGun)
					{
						vp_FPCamera.Position3rdPersonOffset = new Vector3(0.3f, 1.25f, 0.6f);
					}
					else
					{
						vp_FPCamera.Position3rdPersonOffset = new Vector3(0.3f, 1.25f, 1f);
					}
				}
				else if (this.AimingGun)
				{
					vp_FPCamera.Position3rdPersonOffset = new Vector3(0.4f, 1.65f, 0.8f);
				}
				else
				{
					vp_FPCamera.Position3rdPersonOffset = new Vector3(0.4f, 1.65f, 1.2f);
				}
				float num3 = 1f + 3f * this.cameraDistanceMulti;
				vp_FPCamera vp_FPCamera2 = vp_FPCamera;
				vp_FPCamera2.Position3rdPersonOffset.z = vp_FPCamera2.Position3rdPersonOffset.z * (this.camOverheadDistance * ((num2 >= 1f && !this.AimingGun) ? num3 : 1f));
				if (this.isAimingScoped)
				{
					vp_FPCamera.Position3rdPersonOffset.x = 0f;
				}
				else if (this.isLadderAttached && !this.AimingGun)
				{
					vp_FPCamera.Position3rdPersonOffset.x = 0.15f;
				}
				if (this.flipCameraSide)
				{
					vp_FPCamera vp_FPCamera3 = vp_FPCamera;
					vp_FPCamera3.Position3rdPersonOffset.x = vp_FPCamera3.Position3rdPersonOffset.x * -1f;
				}
			}
			this.characterMatrixOverride.Active = (this.vp_FPCamera.CameraCollisionDistance < 0.55f);
			this.ThirdPersonLineOfSightCheck();
		}
	}

	// Token: 0x060027BA RID: 10170 RVA: 0x000F584C File Offset: 0x000F3A4C
	public void RefreshDrivingCameraPositions()
	{
		if (this.AttachedToEntity != null)
		{
			EntityVehicle entityVehicle = this.AttachedToEntity as EntityVehicle;
			if (entityVehicle != null)
			{
				this.vp_FPCamera.Position3rdPersonOffset = entityVehicle.GetCameraOffset(Time.deltaTime);
			}
		}
	}

	// Token: 0x060027BB RID: 10171 RVA: 0x000F588C File Offset: 0x000F3A8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateCameraFOV(bool _bLerpPosition)
	{
		if (!this.playerCamera.enabled)
		{
			return;
		}
		if (!this.IsCameraAttachedToPlayerOrScope() && this.bFirstPersonView)
		{
			return;
		}
		if (this.AimingGun)
		{
			this.inventory.holdingItem.GetIronSights(this.inventory.holdingItemData, out this.lerpCameraEndFOV);
			if (this.lerpCameraEndFOV != 0f)
			{
				this.bLerpCameraFlag = _bLerpPosition;
				this.lerpCameraLerpValue = 0f;
				this.lerpCameraStartFOV = this.playerCamera.fieldOfView;
				return;
			}
		}
		else
		{
			float fieldOfView = (float)this.GetCameraFOV();
			this.bLerpCameraFlag = _bLerpPosition;
			if (this.bLerpCameraFlag)
			{
				this.lerpCameraLerpValue = 0f;
				this.lerpCameraStartFOV = this.playerCamera.fieldOfView;
				this.lerpCameraEndFOV = fieldOfView;
				return;
			}
			this.playerCamera.fieldOfView = fieldOfView;
		}
	}

	// Token: 0x060027BC RID: 10172 RVA: 0x000F595C File Offset: 0x000F3B5C
	public bool TryUpdateCameraDistanceMultiplier(float amount)
	{
		if (amount == 0f)
		{
			this.lastCameraDistanceAmount = amount;
			return false;
		}
		if (amount > 0f && this.bFirstPersonView && this.lastCameraDistanceAmount == 0f)
		{
			if (GamePrefs.GetInt(EnumGamePrefs.CameraRestrictionMode) != 0)
			{
				return false;
			}
			this.cameraDistanceMulti = 0f;
			this.SwitchFirstPersonViewFromInput();
			this.lastCameraDistanceAmount = amount;
			return true;
		}
		else
		{
			if (amount > 0f && this.cameraDistanceMulti == 1f)
			{
				this.lastCameraDistanceAmount = amount;
				return false;
			}
			if (amount >= 0f || this.cameraDistanceMulti != 0f || this.bFirstPersonView)
			{
				this.lastCameraDistanceAmount = amount;
				this.cameraDistanceMulti = Mathf.Clamp(this.cameraDistanceMulti + amount, 0f, 1f);
				GamePrefs.Set(EnumGamePrefs.OptionsGfxCameraDistance3P, this.cameraDistanceMulti);
				return true;
			}
			if (GamePrefs.GetInt(EnumGamePrefs.CameraRestrictionMode) != 0)
			{
				return false;
			}
			if (this.lastCameraDistanceAmount == 0f)
			{
				this.SwitchFirstPersonViewFromInput();
				this.lastCameraDistanceAmount = amount;
				return true;
			}
			this.lastCameraDistanceAmount = amount;
			return false;
		}
	}

	// Token: 0x060027BD RID: 10173 RVA: 0x000F5A64 File Offset: 0x000F3C64
	[PublicizedFrom(EAccessModifier.Private)]
	public void ThirdPersonLineOfSightCheck()
	{
		this.lineOfSightObstructionPoint = Vector3.zero;
		this.LineOfSightObstructed = false;
		Vector3 vector = base.transform.position + new Vector3(0f, this.Crouching ? (base.height * 0.7f) : (base.height * 0.95f), 0f) + (this.flipCameraSide ? (-base.transform.right) : base.transform.right) * 0.05f;
		Vector3 normalized = (this.LookPoint - vector).normalized;
		vector += Origin.position;
		this.lineOfSightRay.origin = vector;
		this.lineOfSightRay.direction = normalized;
		if (Voxel.Raycast(GameManager.Instance.World, this.lineOfSightRay, 1f, 1073807360, 8, 0f))
		{
			if (Vector3.Distance(Voxel.phyxRaycastHit.point, this.LookPoint) > 0.1f)
			{
				this.LineOfSightObstructed = true;
			}
			this.lineOfSightObstructionPoint = Voxel.phyxRaycastHit.point;
		}
	}

	// Token: 0x060027BE RID: 10174 RVA: 0x000F5B8C File Offset: 0x000F3D8C
	public bool IsCameraFacingCharacter()
	{
		return !this.bFirstPersonView && Vector3.Dot(base.transform.forward, this.cameraTransform.forward) < 0f;
	}

	// Token: 0x17000477 RID: 1143
	// (get) Token: 0x060027BF RID: 10175 RVA: 0x000F5BBC File Offset: 0x000F3DBC
	public virtual bool isAimingScoped
	{
		get
		{
			if (!this.AimingGun)
			{
				return false;
			}
			ItemActionZoom.ItemActionDataZoom itemActionDataZoom = this.inventory.holdingItemData.actionData[1] as ItemActionZoom.ItemActionDataZoom;
			return itemActionDataZoom != null && itemActionDataZoom.HasScope && itemActionDataZoom.ZoomOverlay != null;
		}
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x000F5C0A File Offset: 0x000F3E0A
	public override int GetCameraFOV()
	{
		if (!this.bFirstPersonView)
		{
			return GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV3P);
		}
		return GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
	}

	// Token: 0x060027C1 RID: 10177 RVA: 0x000F5C28 File Offset: 0x000F3E28
	public override void PhysicsResume(Vector3 pos, float rotY)
	{
		Transform transform = this.cameraTransform;
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		base.PhysicsResume(pos, rotY);
		transform.SetPositionAndRotation(position, rotation);
	}

	// Token: 0x060027C2 RID: 10178 RVA: 0x000F5C58 File Offset: 0x000F3E58
	public override void OnRagdoll(bool isActive)
	{
		base.OnRagdoll(isActive);
		if (isActive)
		{
			this.SetFirstPersonView(false, true);
			this.emodel.InitRigidBodies();
			this.StartSelfCamera();
			Vector3 forwardVector = base.GetForwardVector();
			this.selfCameraSeekPos = this.emodel.GetChestPosition();
			this.selfCameraSeekPos.x = this.selfCameraSeekPos.x - forwardVector.x * 2.2f;
			this.selfCameraSeekPos.y = this.selfCameraSeekPos.y + 1.2f;
			this.selfCameraSeekPos.z = this.selfCameraSeekPos.z - forwardVector.z * 2.2f;
			Transform transform = this.cameraTransform;
			transform.position = Vector3.MoveTowards(transform.position, this.selfCameraSeekPos, 1.2f);
			return;
		}
		this.SetRotation(this.rotation);
		this.m_vp_FPCamera.enabled = true;
		this.SwitchToPreferredCameraMode(true);
	}

	// Token: 0x060027C3 RID: 10179 RVA: 0x000F5D2E File Offset: 0x000F3F2E
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale = 1f)
	{
		_strength = base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
		if (_strength > 0)
		{
			GameManager.Instance.StartCoroutine(this.shakeCamera(_damageSource.getDirection(), 0.5f, (float)(_strength * 4), 1f));
		}
		return _strength;
	}

	// Token: 0x060027C4 RID: 10180 RVA: 0x000F5D67 File Offset: 0x000F3F67
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator shakeCamera(Vector3 _direction, float time, float strength, float speed = 1f)
	{
		this.m_vp_FPCamera.ShakeSpeed2 = speed;
		this.m_vp_FPCamera.ShakeAmplitude2 = new Vector3(-_direction.x, _direction.y, 0f) * strength;
		yield return new WaitForSeconds(time);
		this.m_vp_FPCamera.ShakeSpeed2 = 0f;
		this.m_vp_FPCamera.ShakeAmplitude2 = Vector3.zero;
		yield break;
	}

	// Token: 0x060027C5 RID: 10181 RVA: 0x000F5D94 File Offset: 0x000F3F94
	public override void Kill(DamageResponse _dmResponse)
	{
		if (!this.IsDead())
		{
			base.Kill(_dmResponse);
			GameManager.Instance.StartCoroutine(this.shakeCamera(Vector3.one, 0.5f, 20f, 1f));
			if (this.m_vp_FPController != null)
			{
				this.m_vp_FPController.Player.Dead.Start(0f);
			}
		}
	}

	// Token: 0x060027C6 RID: 10182 RVA: 0x000F5E00 File Offset: 0x000F4000
	public override void SetPosition(Vector3 _pos, bool _bUpdatePhysics = true)
	{
		base.SetPosition(_pos, _bUpdatePhysics);
		Origin.Instance.UpdateLocalPlayer(this);
		vp_FPController vp_FPController = this.vp_FPController;
		if (vp_FPController != null)
		{
			if (!this.emodel.IsRagdollActive)
			{
				vp_FPController.SetPosition(_pos - Origin.position);
			}
			vp_FPController.Stop();
			if (this.AttachedToEntity)
			{
				vp_FPController.Transform.localPosition = Vector3.zero;
			}
		}
		Manager.CameraChanged();
	}

	// Token: 0x060027C7 RID: 10183 RVA: 0x000F5E78 File Offset: 0x000F4078
	public override void Respawn(RespawnType _reason)
	{
		Log.Out("Respawning: " + _reason.ToStringCached<RespawnType>());
		base.Respawn(_reason);
		if (_reason == RespawnType.Teleport)
		{
			this.RespawnTeleportDuration = Time.realtimeSinceStartup;
			this.RespawnTeleportTimeoutLogged = false;
			this.ftwOutcomeReported = false;
			this.ftwOutcomeWatchActive = false;
		}
		this.moveController.Respawn(_reason);
		Shader.SetGlobalFloat("_UnderWater", 0f);
		this.SetControllable(false);
		if (this.vp_FPController != null && !this.AttachedToEntity)
		{
			this.vp_FPController.ResetState();
			this.vp_FPController.SetPosition(base.GetPosition() - Origin.position);
			this.vp_FPController.Stop();
			this.vp_FPCamera.Locked3rdPerson = false;
		}
		if (_reason == RespawnType.Teleport)
		{
			this.windowManager.CloseAllOpenModalWindows("map");
		}
		this.isFallDeath = false;
	}

	// Token: 0x060027C8 RID: 10184 RVA: 0x000F5F5C File Offset: 0x000F415C
	public void TeleportToPosition(Vector3 _pos, bool _onlyIfNotFlying = false, Vector3? _viewDirection = null)
	{
		if (!_onlyIfNotFlying || !this.IsFlyMode.Value)
		{
			this.Teleport(_pos, (_viewDirection != null) ? _viewDirection.Value.y : float.MinValue);
			if (_pos.y >= 0f)
			{
				ThreadManager.StartCoroutine(this.setVerticalPosition(_pos, _viewDirection));
				return;
			}
			Log.Out("Teleported to {0}", new object[]
			{
				_pos.ToCultureInvariantString()
			});
		}
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000F5FD1 File Offset: 0x000F41D1
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator setVerticalPosition(Vector3 _pos, Vector3? _viewDirection)
	{
		while (!this.Spawned)
		{
			yield return null;
		}
		if (this.AttachedToEntity != null)
		{
			this.AttachedToEntity.SetPosition(_pos, true);
		}
		else
		{
			this.SetPosition(_pos, true);
		}
		if (_viewDirection != null)
		{
			this.SetRotation(_viewDirection.Value);
		}
		Log.Out("Teleported to {0}", new object[]
		{
			_pos.ToCultureInvariantString()
		});
		yield break;
	}

	// Token: 0x060027CA RID: 10186 RVA: 0x000F5FEE File Offset: 0x000F41EE
	public void SetControllable(bool _b)
	{
		if (_b && this.bIntroAnimActive)
		{
			return;
		}
		base.transform.GetComponent<PlayerMoveController>().SetControllableOverride(_b);
	}

	// Token: 0x060027CB RID: 10187 RVA: 0x000F600D File Offset: 0x000F420D
	public void NotifySneakDamage(float multiplier)
	{
		this.sneakDamageText = string.Format(Localization.Get("sneakDamageBonus", false, null), multiplier.ToCultureInvariantString("f1")).ToUpper();
		this.sneakDamageBlendTimer.FadeIn();
	}

	// Token: 0x060027CC RID: 10188 RVA: 0x000F6041 File Offset: 0x000F4241
	public void NotifyDamageMultiplier(float multiplier)
	{
		this.sneakDamageText = string.Format(Localization.Get("stunnedDamageBonus", false, null), multiplier.ToCultureInvariantString("f1")).ToUpper();
		this.sneakDamageBlendTimer.FadeIn();
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x000F6075 File Offset: 0x000F4275
	public override void EnableCamera(bool _b)
	{
		this.playerCamera.enabled = _b;
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsAttackValid()
	{
		return true;
	}

	// Token: 0x060027CF RID: 10191 RVA: 0x000F6084 File Offset: 0x000F4284
	public override bool IsAimingGunPossible()
	{
		return (this.inventory.holdingItem.Actions[0] == null || this.inventory.holdingItem.Actions[0].IsAimingGunPossible(this.inventory.holdingItemData.actionData[0])) && (this.inventory.holdingItem.Actions[1] == null || this.inventory.holdingItem.Actions[1].IsAimingGunPossible(this.inventory.holdingItemData.actionData[1]));
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x000BD923 File Offset: 0x000BBB23
	public override bool IsDrawMapIcon()
	{
		return this.IsSpawned();
	}

	// Token: 0x060027D1 RID: 10193 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsMapIconBlinking()
	{
		return true;
	}

	// Token: 0x060027D2 RID: 10194 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool CanMapIconBeSelected()
	{
		return false;
	}

	// Token: 0x060027D3 RID: 10195 RVA: 0x000BD92B File Offset: 0x000BBB2B
	public override Color GetMapIconColor()
	{
		return Color.white;
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x000B9709 File Offset: 0x000B7909
	public override int GetLayerForMapIcon()
	{
		return 20;
	}

	// Token: 0x060027D5 RID: 10197 RVA: 0x000F611C File Offset: 0x000F431C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool BreakLeg(float chance)
	{
		if (this.rand.RandomFloat <= chance && this.Buffs.AddBuff("injuryBrokenLeg", -1, true, false, -1f) == EntityBuffs.BuffStatus.Added)
		{
			this.PlayOneShot("breakleg", false, false, false, null, 1f);
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.SetAchievementStat(EnumAchievementDataStat.LegBroken, 1);
			}
		}
		return false;
	}

	// Token: 0x060027D6 RID: 10198 RVA: 0x00010E62 File Offset: 0x0000F062
	[PublicizedFrom(EAccessModifier.Private)]
	public bool FractureLeg(float chance)
	{
		return false;
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x000F617E File Offset: 0x000F437E
	[PublicizedFrom(EAccessModifier.Private)]
	public bool SprainLeg(float chance)
	{
		return this.rand.RandomFloat <= chance && this.Buffs.AddBuff("injurySprainedLeg", -1, true, false, -1f) == EntityBuffs.BuffStatus.Added;
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x000F61AB File Offset: 0x000F43AB
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasBrokenLeg()
	{
		return this.Buffs.HasBuff("injuryBrokenLeg");
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x000F61AB File Offset: 0x000F43AB
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasFracturedLeg()
	{
		return this.Buffs.HasBuff("injuryBrokenLeg");
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x000F61BD File Offset: 0x000F43BD
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasSprainedLeg()
	{
		return this.Buffs.HasBuff("injurySprainedLeg");
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x000F61CF File Offset: 0x000F43CF
	public override float GetSpeedModifier()
	{
		return base.GetSpeedModifier() * (this.IsGodMode.Value ? this.GodModeSpeedModifier : 1f);
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x000F61F4 File Offset: 0x000F43F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void FallImpact(float speed)
	{
		if (this.IsGodMode.Value || this.AttachedToEntity != null)
		{
			return;
		}
		if (speed <= 0f)
		{
			return;
		}
		Vector3i pos = World.worldToBlockPos(this.vp_FPController.Transform.position + Origin.position);
		BlockValue block = this.world.GetBlock(pos);
		if (block.isair || block.Block.IsElevator((int)block.rotation))
		{
			pos.y--;
			block = this.world.GetBlock(pos);
		}
		float num = 1f;
		if (!block.isair)
		{
			num = block.Block.FallDamage;
			if (num <= 0f)
			{
				return;
			}
		}
		if (speed > 1f)
		{
			speed = 1f;
		}
		speed *= 1f;
		speed *= EntityPlayer.FallDamageModifier;
		speed *= num;
		speed = EffectManager.GetValue(PassiveEffects.FallDamageReduction, this.inventory.holdingItemItemValue, speed, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.fallHealth = this.Health;
		base.SetCVar("_fallSpeed", speed);
		this.FireEvent(MinEventTypes.onSelfFallImpact, true);
		if (speed > 0.05f)
		{
			this.PlayHitGroundSound(speed);
		}
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x000F632D File Offset: 0x000F452D
	public override void BuffAdded(BuffValue _buff)
	{
		if (_buff.BuffClass.NameTag.Test_Bit(EntityAlive.FallingBuffTagBit) && this.fallHealth > 0 && this.Health <= 0)
		{
			this.isFallDeath = true;
		}
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x000F6360 File Offset: 0x000F4560
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
		EntityClass entityClass = EntityClass.list[this.entityClass];
		if (entityClass.Properties.Values.ContainsKey(EntityClass.PropDropInventoryBlock))
		{
			this.dropInventoryBlock = entityClass.Properties.Values[EntityClass.PropDropInventoryBlock];
		}
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x000F63B6 File Offset: 0x000F45B6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void dropItemOnDeath()
	{
		this.removeItemsOnDeath();
		this.degradeItemsOnDeath();
		this.dropBackpack(true);
		this.inventory.SetFlashlight(false);
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x000F63D8 File Offset: 0x000F45D8
	public void dropItemOnQuit()
	{
		this.dropBackpack(false);
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x000F63E4 File Offset: 0x000F45E4
	public override void SetDroppedBackpackPositions(List<Vector3i> _positions)
	{
		if (!ThreadManager.IsMainThread())
		{
			List<Vector3i> obj = this.backpackPositionsFromThread;
			lock (obj)
			{
				this.backpackPositionsFromThread.Clear();
				if (_positions != null)
				{
					this.backpackPositionsFromThread.AddRange(_positions);
				}
			}
			return;
		}
		base.SetDroppedBackpackPositions(_positions);
		if (this.backpackNavObjects != null)
		{
			foreach (NavObject navObject in this.backpackNavObjects)
			{
				NavObjectManager.Instance.UnRegisterNavObject(navObject);
			}
			this.backpackNavObjects.Clear();
		}
		else
		{
			this.backpackNavObjects = new List<NavObject>();
		}
		if (_positions != null)
		{
			for (int i = 0; i < _positions.Count; i++)
			{
				Vector3i vector3i = _positions[i];
				if (!vector3i.Equals(Vector3i.zero))
				{
					this.backpackNavObjects.Add(NavObjectManager.Instance.RegisterNavObject("backpack_distant", vector3i.ToVector3() + new Vector3(0.5f, 0f, 0.5f), "", false, -1, null));
				}
			}
		}
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x000F6524 File Offset: 0x000F4724
	[PublicizedFrom(EAccessModifier.Protected)]
	public void removeItemsOnDeath()
	{
		EntityPlayerLocal.DropOption @int = (EntityPlayerLocal.DropOption)SandboxOptionManager.GetInt(SandboxOptions.LoseItemsOnDeathType);
		if (@int == EntityPlayerLocal.DropOption.None)
		{
			return;
		}
		bool flag = @int == EntityPlayerLocal.DropOption.All || @int == EntityPlayerLocal.DropOption.Backpack || @int == EntityPlayerLocal.DropOption.ToolbeltBackpack;
		bool flag2 = @int == EntityPlayerLocal.DropOption.All || @int == EntityPlayerLocal.DropOption.Toolbelt || @int == EntityPlayerLocal.DropOption.ToolbeltBackpack;
		bool flag3 = @int == EntityPlayerLocal.DropOption.All || @int == EntityPlayerLocal.DropOption.Equipment;
		if (this.DestroyLocationSlotList == null)
		{
			this.DestroyLocationSlotList = new List<EntityPlayerLocal.LocationSlotInfo>();
		}
		else
		{
			this.DestroyLocationSlotList.Clear();
		}
		if (flag)
		{
			ItemStack[] slots = this.bag.GetSlots();
			for (int i = 0; i < slots.Length; i++)
			{
				if (this.dropStackCondition(slots[i]))
				{
					this.DestroyLocationSlotList.Add(new EntityPlayerLocal.LocationSlotInfo(EntityPlayerLocal.LocationSlotInfo.LocationTypes.Backpack, i));
				}
			}
		}
		if (flag2)
		{
			for (int j = 0; j < this.inventory.GetItemCount(); j++)
			{
				if (j != this.inventory.DUMMY_SLOT_IDX)
				{
					ItemStack item = this.inventory.GetItem(j);
					if (this.dropStackCondition(item))
					{
						this.DestroyLocationSlotList.Add(new EntityPlayerLocal.LocationSlotInfo(EntityPlayerLocal.LocationSlotInfo.LocationTypes.Toolbelt, j));
					}
				}
			}
		}
		int num = 4;
		int num2 = 7;
		if (flag3)
		{
			for (int k = 0; k < this.equipment.GetSlotCount(); k++)
			{
				if (k < num || k > num2)
				{
					ItemValue slotItem = this.equipment.GetSlotItem(k);
					if (this.dropValueCondition(slotItem))
					{
						this.DestroyLocationSlotList.Add(new EntityPlayerLocal.LocationSlotInfo(EntityPlayerLocal.LocationSlotInfo.LocationTypes.Equipment, k));
					}
				}
			}
		}
		if (this.DestroyLocationSlotList.Count == 0)
		{
			return;
		}
		int num3 = Mathf.Min(this.rand.RandomRange(EntityPlayerLocal.LostItemOnDeathMin, EntityPlayerLocal.LostItemOnDeathMax + 1), this.DestroyLocationSlotList.Count);
		this.DestroyLocationSlotList.Shuffle(this.rand);
		if (this.DestroyLocationSlotList.Count > num3)
		{
			this.DestroyLocationSlotList.RemoveRange(num3, this.DestroyLocationSlotList.Count - num3);
		}
		if (flag)
		{
			ItemStack[] slots2 = this.bag.GetSlots();
			this.HandleRemoveRandomItems(slots2);
			this.bag.SetSlots(slots2);
		}
		if (flag3)
		{
			this.HandleRemoveRandomItems(this.equipment);
		}
		if (flag2)
		{
			this.HandleRemoveRandomItems(this.inventory);
		}
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x000F6744 File Offset: 0x000F4944
	[PublicizedFrom(EAccessModifier.Protected)]
	public void degradeItemsOnDeath()
	{
		if (EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.None)
		{
			return;
		}
		bool flag = false;
		foreach (ItemStack itemStack in this.bag.GetSlots())
		{
			if (itemStack.itemValue.HasQuality)
			{
				flag = true;
				if (EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.MaxDurability || EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Both)
				{
					float num = itemStack.itemValue.MaxDurabilityModifier;
					num -= EntityPlayerLocal.DegradeAmountOnDeath;
					if (num < EntityPlayerLocal.DegradeAmountOnDeath)
					{
						num = EntityPlayerLocal.DegradeAmountOnDeath;
					}
					itemStack.itemValue.MaxDurabilityModifier = num;
				}
				if (EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Durability || EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Both)
				{
					int maxUseTimes = itemStack.itemValue.MaxUseTimes;
					int num2 = (int)((float)maxUseTimes * EntityPlayerLocal.DegradeAmountOnDeath);
					itemStack.itemValue.UseTimes += (float)num2;
					if (itemStack.itemValue.UseTimes > (float)maxUseTimes)
					{
						itemStack.itemValue.UseTimes = (float)maxUseTimes;
					}
				}
			}
		}
		if (flag)
		{
			ItemStack[] slots;
			this.bag.SetSlots(slots);
		}
		for (int j = 0; j < this.equipment.GetSlotCount(); j++)
		{
			ItemValue slotItem = this.equipment.GetSlotItem(j);
			if (slotItem != null && !slotItem.IsEmpty() && slotItem.HasQuality)
			{
				flag = false;
				if (EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.MaxDurability || EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Both)
				{
					float num3 = slotItem.MaxDurabilityModifier;
					num3 -= EntityPlayerLocal.DegradeAmountOnDeath;
					if (num3 < EntityPlayerLocal.DegradeAmountOnDeath)
					{
						num3 = EntityPlayerLocal.DegradeAmountOnDeath;
					}
					slotItem.MaxDurabilityModifier = num3;
					flag = true;
				}
				if (EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Durability || EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Both)
				{
					int maxUseTimes2 = slotItem.MaxUseTimes;
					int num4 = (int)((float)maxUseTimes2 * EntityPlayerLocal.DegradeAmountOnDeath);
					slotItem.UseTimes += (float)num4;
					if (slotItem.UseTimes > (float)maxUseTimes2)
					{
						slotItem.UseTimes = (float)maxUseTimes2;
					}
					flag = true;
				}
				if (flag)
				{
					this.equipment.SetSlotItem(j, slotItem, true);
				}
			}
		}
		flag = false;
		foreach (ItemStack itemStack2 in this.inventory.GetSlots())
		{
			if (itemStack2.itemValue.HasQuality)
			{
				flag = true;
				if (EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.MaxDurability || EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Both)
				{
					float num5 = itemStack2.itemValue.MaxDurabilityModifier;
					num5 -= EntityPlayerLocal.DegradeAmountOnDeath;
					if (num5 < EntityPlayerLocal.DegradeAmountOnDeath)
					{
						num5 = EntityPlayerLocal.DegradeAmountOnDeath;
					}
					itemStack2.itemValue.MaxDurabilityModifier = num5;
				}
				if (EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Durability || EntityPlayerLocal.DegradeOnDeathType == EntityPlayerLocal.DegradeOnDeathTypes.Both)
				{
					int maxUseTimes3 = itemStack2.itemValue.MaxUseTimes;
					int num6 = (int)((float)maxUseTimes3 * EntityPlayerLocal.DegradeAmountOnDeath);
					itemStack2.itemValue.UseTimes += (float)num6;
					if (itemStack2.itemValue.UseTimes > (float)maxUseTimes3)
					{
						itemStack2.itemValue.UseTimes = (float)maxUseTimes3;
					}
				}
			}
		}
		if (flag)
		{
			ItemStack[] slots2;
			this.inventory.SetSlots(slots2, true);
		}
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x000F6A2C File Offset: 0x000F4C2C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void dropBackpack(bool _isDying)
	{
		EntityPlayerLocal.DropOption dropOption = _isDying ? EntityPlayerLocal.DropOnDeathOption : EntityPlayerLocal.DropOnQuitOption;
		if (dropOption == EntityPlayerLocal.DropOption.None)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.dropInventoryBlock))
		{
			return;
		}
		if (this.playerUI.xui != null)
		{
			this.playerUI.xui.CancelAllCrafting();
		}
		if (_isDying && dropOption == EntityPlayerLocal.DropOption.DeleteAll)
		{
			ItemStack[] slots = this.bag.GetSlots();
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i] = ItemStack.Empty;
			}
			this.bag.SetSlots(slots);
			for (int j = 0; j < this.inventory.GetItemCount(); j++)
			{
				this.inventory.SetItem(j, ItemStack.Empty);
			}
			for (int k = 0; k < 12; k++)
			{
				ItemValue slotItem = this.equipment.GetSlotItem(k);
				if (slotItem != null && !slotItem.ItemClassOrMissing.KeepOnDeath())
				{
					this.equipment.SetSlotItem(k, null, true);
				}
			}
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = dropOption == EntityPlayerLocal.DropOption.All || dropOption == EntityPlayerLocal.DropOption.Backpack || dropOption == EntityPlayerLocal.DropOption.ToolbeltBackpack;
		bool flag2 = dropOption == EntityPlayerLocal.DropOption.All || dropOption == EntityPlayerLocal.DropOption.Toolbelt || dropOption == EntityPlayerLocal.DropOption.ToolbeltBackpack;
		bool flag3 = dropOption == EntityPlayerLocal.DropOption.All || dropOption == EntityPlayerLocal.DropOption.Equipment;
		ItemStack[] slots2 = this.bag.GetSlots();
		for (int l = 0; l < slots2.Length; l++)
		{
			if (this.dropStackCondition(slots2[l]))
			{
				num++;
			}
		}
		for (int m = 0; m < this.inventory.GetItemCount(); m++)
		{
			if (m != this.inventory.DUMMY_SLOT_IDX)
			{
				ItemStack item = this.inventory.GetItem(m);
				if (this.dropStackCondition(item))
				{
					num2++;
				}
			}
		}
		int num4 = 4;
		int num5 = 7;
		for (int n = 0; n < this.equipment.GetSlotCount(); n++)
		{
			if (n < num4 || n > num5)
			{
				ItemValue slotItem2 = this.equipment.GetSlotItem(n);
				if (this.dropValueCondition(slotItem2))
				{
					num3++;
				}
			}
		}
		if (num == 0 && num2 == 0 && num3 == 0)
		{
			return;
		}
		EntityBackpack entityBackpack = EntityFactory.CreateEntity("Backpack".GetHashCode(), this.position + base.transform.up * 2f) as EntityBackpack;
		PreferenceTracker preferenceTracker = new PreferenceTracker(this.entityId);
		if (flag)
		{
			ItemStack[] slots3 = this.bag.GetSlots();
			preferenceTracker.SetBag(slots3, this.dropStackCondition);
			for (int num6 = 0; num6 < slots3.Length; num6++)
			{
				if (this.dropStackCondition(slots3[num6]))
				{
					entityBackpack.bag.AddItem(slots3[num6]);
					slots3[num6] = ItemStack.Empty;
				}
			}
			this.bag.SetSlots(slots3);
		}
		if (flag3)
		{
			ItemValue[] array = new ItemValue[12];
			for (int num7 = 0; num7 < array.Length; num7++)
			{
				ItemValue slotItem3 = this.equipment.GetSlotItem(num7);
				ItemValue itemValue = (slotItem3 != null) ? slotItem3.Clone() : null;
				if (this.dropValueCondition(itemValue))
				{
					array[num7] = itemValue;
					ItemStack itemStack = new ItemStack(itemValue, 1);
					entityBackpack.bag.AddItem(itemStack);
					if (!_isDying)
					{
						this.equipment.SetSlotItem(num7, null, true);
					}
				}
			}
			preferenceTracker.SetEquipment(array, this.dropValueCondition);
		}
		if (flag2)
		{
			ItemStack[] array2 = new ItemStack[this.inventory.GetItemCount()];
			for (int num8 = 0; num8 < array2.Length; num8++)
			{
				if (num8 != this.inventory.DUMMY_SLOT_IDX)
				{
					ItemStack item2 = this.inventory.GetItem(num8);
					array2[num8] = item2;
					if (this.dropStackCondition(item2))
					{
						entityBackpack.bag.AddItem(item2);
						this.inventory.SetItem(num8, ItemStack.Empty);
					}
				}
			}
			preferenceTracker.SetToolbelt(array2, this.dropStackCondition);
		}
		if (preferenceTracker.AnyPreferences)
		{
			entityBackpack.bag.preferences = preferenceTracker;
		}
		entityBackpack.RefPlayerId = this.entityId;
		EntityCreationData entityCreationData = new EntityCreationData(entityBackpack, true);
		entityCreationData.entityName = string.Format(Localization.Get("playersBackpack", false, null), this.EntityName);
		entityCreationData.id = -1;
		entityCreationData.bag = entityBackpack.bag.Clone();
		GameManager.Instance.RequestToSpawnEntityServer(entityCreationData);
		entityBackpack.OnEntityUnload();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SetDroppedBackpackPositions(GameManager.Instance.persistentLocalPlayer.GetDroppedBackpackPositions());
		}
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x000F6EA8 File Offset: 0x000F50A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleRemoveRandomItems(ItemStack[] items)
	{
		for (int i = this.DestroyLocationSlotList.Count - 1; i >= 0; i--)
		{
			if (this.DestroyLocationSlotList[i].Location == EntityPlayerLocal.LocationSlotInfo.LocationTypes.Backpack)
			{
				items[this.DestroyLocationSlotList[i].Slot].Clear();
				this.DestroyLocationSlotList.RemoveAt(i);
			}
		}
	}

	// Token: 0x060027E6 RID: 10214 RVA: 0x000F6F04 File Offset: 0x000F5104
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleRemoveRandomItems(Inventory inventory)
	{
		for (int i = this.DestroyLocationSlotList.Count - 1; i >= 0; i--)
		{
			if (this.DestroyLocationSlotList[i].Location == EntityPlayerLocal.LocationSlotInfo.LocationTypes.Toolbelt)
			{
				inventory.SetItem(this.DestroyLocationSlotList[i].Slot, ItemStack.Empty);
				this.DestroyLocationSlotList.RemoveAt(i);
			}
		}
	}

	// Token: 0x060027E7 RID: 10215 RVA: 0x000F6F68 File Offset: 0x000F5168
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleRemoveRandomItems(Equipment equipment)
	{
		for (int i = this.DestroyLocationSlotList.Count - 1; i >= 0; i--)
		{
			if (this.DestroyLocationSlotList[i].Location == EntityPlayerLocal.LocationSlotInfo.LocationTypes.Equipment)
			{
				equipment.SetSlotItem(this.DestroyLocationSlotList[i].Slot, null, true);
				this.DestroyLocationSlotList.RemoveAt(i);
			}
		}
	}

	// Token: 0x060027E8 RID: 10216 RVA: 0x000F6FC6 File Offset: 0x000F51C6
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool ShouldRemoveEquipmentOnDeath(EntityPlayerLocal.DropOption dropOption)
	{
		return dropOption == EntityPlayerLocal.DropOption.All || dropOption == EntityPlayerLocal.DropOption.DeleteAll;
	}

	// Token: 0x060027E9 RID: 10217 RVA: 0x000F6FD4 File Offset: 0x000F51D4
	public override int AttachToEntity(Entity _other, int slot = -1)
	{
		if (_other.IsAttached(this))
		{
			return -1;
		}
		vp_FPController vp_FPController = this.vp_FPController;
		vp_FPController.enabled = true;
		Transform transform = this.m_vp_FPCamera.Transform;
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		this.SetFirstPersonView(false, true);
		slot = base.AttachToEntity(_other, slot);
		if (slot >= 0)
		{
			transform.position = position;
			transform.rotation = rotation;
			vp_FPController.Stop();
			vp_FPController.Player.Driving.Start(0f);
		}
		else
		{
			this.SwitchToPreferredCameraMode(false);
		}
		EntityVehicle entityVehicle = _other as EntityVehicle;
		if (entityVehicle && entityVehicle.LocalPlayerIsOwner())
		{
			this.Waypoints.UpdateEntityVehicleWayPoint(entityVehicle, false);
			this.Waypoints.SetWaypointHiddenOnMap(this.entityId, true);
		}
		Manager.BroadcastStop(this.entityId, "Player" + (this.IsMale ? "Male" : "Female") + "RunLoop");
		this.sprintLoopSoundPlayId = -1;
		return slot;
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x000F70CC File Offset: 0x000F52CC
	public override void Detach()
	{
		this.SwitchToPreferredCameraMode(true);
		this.moveController.isAutorun = false;
		Vector3 vector = Vector3.zero;
		EntityVehicle entityVehicle = this.AttachedToEntity as EntityVehicle;
		if (entityVehicle)
		{
			vector = entityVehicle.GetExitVelocity() * Time.fixedDeltaTime;
			if (entityVehicle.LocalPlayerIsOwner())
			{
				this.Waypoints.UpdateEntityVehicleWayPoint(entityVehicle, false);
			}
		}
		base.Detach();
		vp_FPController vp_FPController = this.vp_FPController;
		vp_FPController.Player.Driving.Stop(0f);
		vp_FPController.Stop();
		vp_FPController.m_MaxHeightInitialFallSpeed = Utils.FastMin(vector.y, 0f);
		vp_FPController.AddForce(vector);
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x000F716F File Offset: 0x000F536F
	public override void ProcessDamageResponseLocal(DamageResponse _dmResponse)
	{
		base.ProcessDamageResponseLocal(_dmResponse);
		if (_dmResponse.Source.damageType != EnumDamageTypes.Weather)
		{
			this.healthLostThisRound = (_dmResponse.Strength > 0);
		}
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x000F7196 File Offset: 0x000F5396
	public override bool CanUpdateEntity()
	{
		return this.IsFlyMode.Value || base.CanUpdateEntity();
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x000F71B0 File Offset: 0x000F53B0
	public override void OnHUD()
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		NGuiWdwInGameHUD inGameHUD = this.nguiWindowManager.InGameHUD;
		if (!GameManager.Instance.gameStateManager.IsGameStarted() || GameStats.GetInt(EnumGameStats.GameState) != 1 || !this.Spawned)
		{
			return;
		}
		bool flag = this.windowManager.IsModalWindowOpen() || LocalPlayerUI.primaryUI.windowManager.IsModalWindowOpen();
		this.guiDrawOverlayTextures(inGameHUD, flag);
		if (this.windowManager.IsFullHUDDisabled())
		{
			return;
		}
		if (this.inventory != null && !flag)
		{
			this.inventory.holdingItem.OnHUD(this.inventory.holdingItemData, Screen.width - 10, Screen.height - 10);
		}
		if (!LocalPlayerUI.primaryUI.windowManager.IsModalWindowOpen() && !this.windowManager.IsWindowOpen("toolbelt") && !this.windowManager.IsWindowOpen(XUiC_InGameMenuWindow.ID) && !this.windowManager.IsWindowOpen("dialog") && !this.windowManager.IsWindowOpen("tipWindow") && !this.windowManager.IsWindowOpen("questOffer"))
		{
			this.windowManager.Open("toolbelt", false);
		}
		this.windowManager.Open(this.playerUI.xui.CalloutWindow.WindowGroup, false);
		if (!this.windowManager.IsModalWindowOpen() && !this.windowManager.IsWindowOpen(XUiC_CompassWindow.ID) && !LocalPlayerUI.primaryUI.windowManager.IsModalWindowOpen())
		{
			this.windowManager.Open(XUiC_CompassWindow.ID, false);
		}
		this.windowManager.Open(this.playerUI.xui.ToolTipWindow.WindowGroup, false);
		this.windowManager.Open(XUiC_ChatOutput.ID, false);
		if (Event.current.type == EventType.Repaint)
		{
			if (this.sneakDamageBlendTimer.Value > 0f)
			{
				this.nguiWindowManager.SetLabel(EnumNGUIWindow.CriticalHitText, this.sneakDamageText, new Color?(new Color(1f, 1f, 1f, this.sneakDamageBlendTimer.Value)), true);
			}
			else if (this.nguiWindowManager.IsShowing(EnumNGUIWindow.CriticalHitText))
			{
				this.nguiWindowManager.Show(EnumNGUIWindow.CriticalHitText, false);
			}
		}
		this.guiDrawCrosshair(inGameHUD, flag);
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x000F73F1 File Offset: 0x000F55F1
	public void ForceBloodSplatter()
	{
		this.healthLostThisRound = true;
		this.lastHitDirection = Utils.EnumHitDirection.Front;
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x000F7404 File Offset: 0x000F5604
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void guiDrawOverlayTextures(NGuiWdwInGameHUD _guiInGame, bool bModalWindowOpen)
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		this.inventory.holdingItem.OnScreenOverlay(this.inventory.holdingItemData);
		Vector3 vector = this.finalCamera.ViewportToScreenPoint(new Vector3(0f, 0f, 0f));
		Vector3 vector2 = this.finalCamera.ViewportToScreenPoint(new Vector3(1f, 1f, 0f));
		if (this.healthLostThisRound && !this.IsDead() && this.lastHitDirection != Utils.EnumHitDirection.None)
		{
			this.healthLostThisRound = false;
			int lastHitDirection = (int)this.lastHitDirection;
			this.overlayDirectionTime[lastHitDirection] = 6f;
			byte[] array = this.overlayAlternating;
			int num = lastHitDirection;
			array[num] += 1;
			int num2 = lastHitDirection * 2 + (int)(this.overlayAlternating[lastHitDirection] & 1);
			this.overlayDirectionTime[num2] = 6f;
			this.overlayBloodDropsPositions[num2 * 3] = new Vector2(this.rand.RandomRange(vector.x, vector2.x), this.rand.RandomRange(vector.y, vector2.y));
			this.overlayBloodDropsPositions[num2 * 3 + 1] = new Vector2(this.rand.RandomRange(vector.x, vector2.x), this.rand.RandomRange(vector.y, vector2.y));
			this.overlayBloodDropsPositions[num2 * 3 + 2] = new Vector2(this.rand.RandomRange(vector.x, vector2.x), this.rand.RandomRange(vector.y, vector2.y));
			this.lastHitDirection = Utils.EnumHitDirection.None;
		}
		for (int i = 0; i < 8; i++)
		{
			if (this.overlayDirectionTime[i] > 0f)
			{
				float num3 = Mathf.Pow(1f - this.overlayDirectionTime[i] / 6f, 0.28f);
				this.overlayMaterial.SetColor("_Color", new Color(num3, num3, num3));
				if (this.windowManager.IsHUDEnabled())
				{
					Vector3 vector3 = (vector2 + vector) * 0.5f;
					int pixelWidth = this.finalCamera.pixelWidth;
					int pixelHeight = this.finalCamera.pixelHeight;
					Texture2D texture2D = _guiInGame.overlayDamageTextures[i];
					float num4 = (float)pixelHeight / 512f;
					float num5 = (float)texture2D.width * num4;
					float num6 = (float)texture2D.height * num4;
					Rect screenRect = new Rect(vector3.x - num5 * 0.5f, 0f, num5, num6);
					int num7 = i >> 1;
					if (num7 == 1)
					{
						screenRect.y = (float)pixelHeight - num6;
					}
					else if (num7 >= 2)
					{
						screenRect.x = (float)pixelWidth - num5;
						screenRect.y = vector3.y - num6 * 0.5f;
						if (num7 == 3)
						{
							screenRect.x = 0f;
						}
					}
					Graphics.DrawTexture(screenRect, texture2D, this.overlayMaterial);
					int num8 = i * 3;
					Graphics.DrawTexture(new Rect(this.overlayBloodDropsPositions[num8].x, (float)pixelHeight - this.overlayBloodDropsPositions[num8].y, (float)_guiInGame.overlayDamageBloodDrops[0].width, (float)_guiInGame.overlayDamageBloodDrops[0].height), _guiInGame.overlayDamageBloodDrops[0], this.overlayMaterial);
					Graphics.DrawTexture(new Rect(this.overlayBloodDropsPositions[num8 + 1].x, (float)pixelHeight - this.overlayBloodDropsPositions[num8 + 1].y, (float)_guiInGame.overlayDamageBloodDrops[1].width, (float)_guiInGame.overlayDamageBloodDrops[1].height), _guiInGame.overlayDamageBloodDrops[1], this.overlayMaterial);
					Graphics.DrawTexture(new Rect(this.overlayBloodDropsPositions[num8 + 2].x, (float)pixelHeight - this.overlayBloodDropsPositions[num8 + 2].y, (float)_guiInGame.overlayDamageBloodDrops[2].width, (float)_guiInGame.overlayDamageBloodDrops[2].height), _guiInGame.overlayDamageBloodDrops[2], this.overlayMaterial);
				}
				if (this.Health > 0)
				{
					this.overlayDirectionTime[i] -= Time.deltaTime;
				}
				else
				{
					this.overlayDirectionTime[i] = 0f;
				}
			}
		}
	}

	// Token: 0x060027F0 RID: 10224 RVA: 0x00040FA0 File Offset: 0x0003F1A0
	[PublicizedFrom(EAccessModifier.Private)]
	public float CrosshairAlpha(NGuiWdwInGameHUD _guiInGame)
	{
		return 1f;
	}

	// Token: 0x060027F1 RID: 10225 RVA: 0x000F785C File Offset: 0x000F5A5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void guiDrawCrosshair(NGuiWdwInGameHUD _guiInGame, bool bModalWindowOpen)
	{
		if (!_guiInGame.showCrosshair)
		{
			return;
		}
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		if (this.IsDead() || this.emodel.IsRagdollActive)
		{
			return;
		}
		if (this.AttachedToEntity != null)
		{
			return;
		}
		if (!GamePrefs.GetBool(EnumGamePrefs.OptionsCrosshairEnabled))
		{
			return;
		}
		ItemClass.EnumCrosshairType enumCrosshairType = this.inventory.holdingItem.GetCrosshairType(this.inventory.holdingItemData);
		if (enumCrosshairType == ItemClass.EnumCrosshairType.None)
		{
			if (this.bFirstPersonView || !GamePrefs.GetBool(EnumGamePrefs.OptionsCrosshairDot3P))
			{
				return;
			}
			enumCrosshairType = ItemClass.EnumCrosshairType.Dot;
		}
		if (enumCrosshairType == ItemClass.EnumCrosshairType.Crosshair || (enumCrosshairType == ItemClass.EnumCrosshairType.CrosshairOnAiming && !GamePrefs.GetBool(EnumGamePrefs.OptionsCrosshairRangedEnabled)))
		{
			enumCrosshairType = ItemClass.EnumCrosshairType.Plus;
		}
		if (!GamePrefs.GetBool(EnumGamePrefs.OptionsCrosshairADS) && ((this.bFirstPersonView && this.AimingGun) || this.isAimingScoped))
		{
			return;
		}
		if (!bModalWindowOpen && this.inventory != null)
		{
			float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsCrosshairScale);
			float float2 = GamePrefs.GetFloat(EnumGamePrefs.OptionsCrosshairOpacity);
			int num = GamePrefs.GetInt(EnumGamePrefs.OptionsCrosshairColor);
			if (num < 0 || num >= EntityPlayerLocal.crosshairColors.Count)
			{
				num = 0;
			}
			Color color = EntityPlayerLocal.crosshairColors[num];
			Vector2 crosshairPosition2D = this.GetCrosshairPosition2D();
			crosshairPosition2D.y = (float)Screen.height - crosshairPosition2D.y;
			float num2 = (float)Screen.height * 0.059f;
			float num3 = num2 * @float;
			Color color2 = GUI.color;
			switch (enumCrosshairType)
			{
			case ItemClass.EnumCrosshairType.Plus:
				GUI.color = new Color(color.r, color.g, color.b, float2);
				GUI.DrawTexture(new Rect(crosshairPosition2D.x - num3 / 2f, crosshairPosition2D.y - num3 / 2f, num3, num3), _guiInGame.CrosshairTexture, ScaleMode.StretchToFill);
				GUI.color = color2;
				return;
			case ItemClass.EnumCrosshairType.Crosshair:
			case ItemClass.EnumCrosshairType.CrosshairOnAiming:
				if (enumCrosshairType != ItemClass.EnumCrosshairType.Crosshair || !this.AimingGun || ItemAction.ShowDistanceDebugInfo)
				{
					float num4 = EffectManager.GetValue(PassiveEffects.SpreadDegreesHorizontal, this.inventory.holdingItemData.itemValue, 90f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					num4 *= 0.5f;
					num4 *= (this.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
					num4 *= (float)Screen.width / this.cameraTransform.GetComponent<Camera>().fieldOfView;
					float num5 = EffectManager.GetValue(PassiveEffects.SpreadDegreesVertical, this.inventory.holdingItemData.itemValue, 90f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					num5 *= 0.5f;
					num5 *= (this.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged).lastAccuracy;
					num5 *= (float)Screen.width / this.cameraTransform.GetComponent<Camera>().fieldOfView;
					float x = crosshairPosition2D.x;
					float y = crosshairPosition2D.y;
					float length = num3 / 3f;
					EntityPlayerLocal.DrawDynamicCrosshair(x, y, num4, num5, length, GamePrefs.GetFloat(EnumGamePrefs.OptionsCrosshairThickness), float2, color);
					return;
				}
				break;
			case ItemClass.EnumCrosshairType.Damage:
				if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
				{
					float a = color2.a * this.playerUI.xui.BackgroundGlobalOpacity;
					GUI.color = new Color(color2.r, color2.g, color2.b, a);
				}
				else
				{
					GUI.color = new Color(color.r, color.g, color.b, this.CrosshairAlpha(_guiInGame));
				}
				GUI.DrawTexture(new Rect(crosshairPosition2D.x - num2 / 2f, crosshairPosition2D.y - num2 / 2f, num2, num2), _guiInGame.CrosshairDamage, ScaleMode.StretchToFill);
				GUI.color = color2;
				return;
			case ItemClass.EnumCrosshairType.Upgrade:
				if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
				{
					float a2 = color2.a * this.playerUI.xui.BackgroundGlobalOpacity;
					GUI.color = new Color(color2.r, color2.g, color2.b, a2);
				}
				else
				{
					GUI.color = new Color(color2.r, color2.g, color2.b, this.CrosshairAlpha(_guiInGame));
				}
				GUI.DrawTexture(new Rect(crosshairPosition2D.x - num2 / 2f, crosshairPosition2D.y - num2 / 2f, num2, num2), _guiInGame.CrosshairUpgrade, ScaleMode.StretchToFill);
				GUI.color = color2;
				return;
			case ItemClass.EnumCrosshairType.Repair:
				if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
				{
					float a3 = color2.a * this.playerUI.xui.BackgroundGlobalOpacity;
					GUI.color = new Color(color2.r, color2.g, color2.b, a3);
				}
				else
				{
					GUI.color = new Color(color2.r, color2.g, color2.b, this.CrosshairAlpha(_guiInGame));
				}
				GUI.DrawTexture(new Rect(crosshairPosition2D.x - num2 / 2f, crosshairPosition2D.y - num2 / 2f, num2, num2), _guiInGame.CrosshairRepair, ScaleMode.StretchToFill);
				GUI.color = color2;
				return;
			case ItemClass.EnumCrosshairType.PowerSource:
				if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
				{
					float a4 = color2.a * this.playerUI.xui.BackgroundGlobalOpacity;
					GUI.color = new Color(color2.r, color2.g, color2.b, a4);
				}
				else
				{
					GUI.color = new Color(color2.r, color2.g, color2.b, this.CrosshairAlpha(_guiInGame));
				}
				GUI.DrawTexture(new Rect(crosshairPosition2D.x - num2 / 2f, crosshairPosition2D.y - num2 / 2f, num2, num2), _guiInGame.CrosshairPowerSource, ScaleMode.StretchToFill);
				GUI.color = color2;
				return;
			case ItemClass.EnumCrosshairType.Heal:
				if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
				{
					float a5 = color2.a * this.playerUI.xui.BackgroundGlobalOpacity;
					GUI.color = new Color(color2.r, color2.g, color2.b, a5);
				}
				else
				{
					GUI.color = new Color(color2.r, color2.g, color2.b, this.CrosshairAlpha(_guiInGame));
				}
				GUI.DrawTexture(new Rect(crosshairPosition2D.x - num2 / 2f, crosshairPosition2D.y - num2 / 2f, num2, num2), _guiInGame.CrosshairRepair, ScaleMode.StretchToFill);
				GUI.color = color2;
				return;
			case ItemClass.EnumCrosshairType.PowerItem:
				if (this.playerUI.xui.BackgroundGlobalOpacity < 1f)
				{
					float a6 = color2.a * this.playerUI.xui.BackgroundGlobalOpacity;
					GUI.color = new Color(color2.r, color2.g, color2.b, a6);
				}
				else
				{
					GUI.color = new Color(color2.r, color2.g, color2.b, this.CrosshairAlpha(_guiInGame));
				}
				GUI.DrawTexture(new Rect(crosshairPosition2D.x - num2 / 2f, crosshairPosition2D.y - num2 / 2f, num2, num2), _guiInGame.CrosshairPowerItem, ScaleMode.StretchToFill);
				GUI.color = color2;
				return;
			case ItemClass.EnumCrosshairType.Blocked:
				GUI.color = new Color(1f, 0.5f, 0f, float2 * 0.5f);
				GUI.DrawTexture(new Rect(crosshairPosition2D.x - num2 / 4f, crosshairPosition2D.y - num2 / 4f, num2 / 2f, num2 / 2f), _guiInGame.CrosshairBlocked, ScaleMode.StretchToFill);
				GUI.color = color2;
				return;
			case ItemClass.EnumCrosshairType.Dot:
				GUI.color = new Color(color.r, color.g, color.b, float2);
				GUI.DrawTexture(new Rect(crosshairPosition2D.x - num2 / 4f, crosshairPosition2D.y - num2 / 4f, num2 / 2f, num2 / 2f), _guiInGame.CrosshairDot, ScaleMode.StretchToFill);
				GUI.color = color2;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060027F2 RID: 10226 RVA: 0x000F80A4 File Offset: 0x000F62A4
	public static void DrawDynamicCrosshair(float centerX, float centerY, float openAreaX, float openAreaY, float length, float thicknessMultiplier, float crosshairOpacity, Color crosshairColor)
	{
		NGuiWdwInGameHUD inGameHUD = LocalPlayerUI.primaryUI.nguiWindowManager.InGameHUD;
		float num = (float)Screen.height * 0.005f * thicknessMultiplier;
		float num2 = num / 3f;
		Color color = GUI.color;
		Color color2 = new Color(0f, 0f, 0f, crosshairOpacity * 0.5f);
		Color color3 = new Color(crosshairColor.r, crosshairColor.g, crosshairColor.b, crosshairOpacity);
		GUI.color = color2;
		GUI.DrawTexture(new Rect(centerX - openAreaX - length - num2, centerY - num / 2f + num2, length, num), inGameHUD.CrosshairSquare, ScaleMode.StretchToFill);
		GUI.color = color3;
		GUI.DrawTexture(new Rect(centerX - openAreaX - length, centerY - num / 2f, length, num), inGameHUD.CrosshairSquare, ScaleMode.StretchToFill);
		GUI.color = color2;
		GUI.DrawTexture(new Rect(centerX + openAreaX + num2, centerY - num / 2f + num2, length, num), inGameHUD.CrosshairSquare, ScaleMode.StretchToFill);
		GUI.color = color3;
		GUI.DrawTexture(new Rect(centerX + openAreaX, centerY - num / 2f, length, num), inGameHUD.CrosshairSquare, ScaleMode.StretchToFill);
		GUI.color = color2;
		GUI.DrawTexture(new Rect(centerX - num + num / 2f - num2, centerY + openAreaY + num2, num, length), inGameHUD.CrosshairSquare, ScaleMode.StretchToFill);
		GUI.color = color3;
		GUI.DrawTexture(new Rect(centerX - num + num / 2f, centerY + openAreaY, num, length), inGameHUD.CrosshairSquare, ScaleMode.StretchToFill);
		GUI.color = color2;
		GUI.DrawTexture(new Rect(centerX - num + num / 2f - num2, centerY - openAreaY - length + num2, num, length), inGameHUD.CrosshairSquare, ScaleMode.StretchToFill);
		GUI.color = color3;
		GUI.DrawTexture(new Rect(centerX - num + num / 2f, centerY - openAreaY - length, num, length), inGameHUD.CrosshairSquare, ScaleMode.StretchToFill);
		GUI.color = color;
	}

	// Token: 0x060027F3 RID: 10227 RVA: 0x000F8270 File Offset: 0x000F6470
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShelterFrameUpdate()
	{
		if (!this.shelterIsUpdating)
		{
			this.shelterIsUpdating = true;
			this.shelterStartPos = this.position - Origin.position;
			this.shelterPos = this.shelterStartPos;
			this.shelterPos.y = this.shelterPos.y + 0.62f;
			this.shelterSideCount = -10;
		}
		if (this.shelterSideCount == -10)
		{
			this.shelterSideCount = -2;
			this.ShelterCheckSkyUp();
			this.shelterAbovePercent = 1f;
			if (!this.shelterIsUpdating)
			{
				this.shelterAbovePercent = 0f;
				this.shelterPercent = 0f;
				return;
			}
		}
		else if (this.shelterSideCount < 0)
		{
			this.shelterSideCount++;
			this.ShelterCheckSkyDiagonal();
			this.shelterPos.y = this.shelterPos.y + 0.1f;
			if (!this.shelterIsUpdating)
			{
				this.shelterPercent = 0f;
				return;
			}
			if (this.shelterSideCount == 0)
			{
				this.shelterStartPos.y = this.shelterStartPos.y + 0.48f;
				this.shelterStartPos.x = this.shelterStartPos.x - 0.13f;
				this.shelterStartPos.z = this.shelterStartPos.z - 0.13f;
				this.shelterPosOffset = Vector3.zero;
				return;
			}
		}
		else
		{
			if (this.shelterSideCount == 0)
			{
				this.shelterSideCount = 1;
				this.shelterPos = this.shelterStartPos + this.shelterPosOffset;
				this.shelterDir = 0f;
				this.shelterRadius = 1f;
			}
			this.ShelterCheckSides();
			if (!this.shelterIsUpdating)
			{
				this.shelterPercent = 0f;
				return;
			}
			if (this.shelterSideCount >= 4)
			{
				if (this.shelterPosOffset.y < 0.5f)
				{
					this.shelterPosOffset.y = this.shelterPosOffset.y + (base.IsCrouching ? 0.54f : 0.65f);
					this.shelterPosOffset.x = this.shelterPosOffset.x + 0.011f;
					this.shelterPosOffset.z = this.shelterPosOffset.z + 0.013f;
					this.shelterSideCount = 0;
					return;
				}
				this.shelterIsUpdating = false;
				this.shelterPercent = 1f;
			}
		}
	}

	// Token: 0x060027F4 RID: 10228 RVA: 0x000F8480 File Offset: 0x000F6680
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShelterCheckSkyUp()
	{
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		for (int i = 0; i < 4; i++)
		{
			Vector3 origin = this.shelterPos;
			float num = this.shelterDir * 0.017453292f;
			origin.x += (float)Math.Cos((double)num) * 0.18f;
			origin.z += (float)Math.Sin((double)num) * 0.18f;
			RaycastHit raycastHit;
			if (!defaultPhysicsScene.Raycast(origin, Vector3.up, out raycastHit, 253f, 1073807360, QueryTriggerInteraction.UseGlobal))
			{
				this.shelterIsUpdating = false;
				return;
			}
			this.shelterDir += 90f;
		}
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x000F8524 File Offset: 0x000F6724
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShelterCheckSkyDiagonal()
	{
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		Vector3 direction;
		direction.y = 0.75f + (float)(this.shelterSideCount + 1) * 0.13f;
		for (int i = 0; i < 8; i++)
		{
			float num = this.shelterDir * 0.017453292f;
			direction.x = (float)Math.Cos((double)num);
			direction.z = (float)Math.Sin((double)num);
			RaycastHit raycastHit;
			if (!defaultPhysicsScene.Raycast(this.shelterPos, direction, out raycastHit, 253f, 1073807360, QueryTriggerInteraction.UseGlobal))
			{
				this.shelterIsUpdating = false;
				return;
			}
			this.shelterDir += 45f;
		}
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x000F85C4 File Offset: 0x000F67C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShelterCheckSides()
	{
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		Vector3 vector;
		vector.y = 0f;
		for (int i = 0; i < 8; i++)
		{
			float num = this.shelterDir * 0.017453292f;
			float num2 = (float)Math.Cos((double)num);
			float num3 = (float)Math.Sin((double)num);
			vector.x = num2 * this.shelterRadius;
			vector.z = num3 * this.shelterRadius;
			Vector3 origin = this.shelterPos + vector;
			RaycastHit raycastHit;
			if (defaultPhysicsScene.Raycast(this.shelterPos, vector, out raycastHit, this.shelterRadius, 1073807360, QueryTriggerInteraction.UseGlobal))
			{
				origin = raycastHit.point;
				origin.x -= num2 * 0.02f;
				origin.z -= num3 * 0.02f;
			}
			if (!defaultPhysicsScene.Raycast(origin, Vector3.up, out raycastHit, 253f, 1073807360, QueryTriggerInteraction.UseGlobal))
			{
				this.shelterIsUpdating = false;
				break;
			}
			this.shelterDir += 45f;
		}
		this.shelterDir += -359f;
		this.shelterRadius += 1f;
		if (this.shelterRadius > 3.1f)
		{
			this.shelterRadius = 0f;
			this.shelterSideCount++;
			this.shelterPos.x = this.shelterPos.x + 0.13f;
			this.shelterPos.z = this.shelterPos.z + 0.13f;
		}
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x000F873C File Offset: 0x000F693C
	public void WeatherStatusFrameUpdate()
	{
		bool flag = this.AreaMessage != null && this.stormAlertBiomeStandingOn == this.biomeStandingOn;
		this.AreaMessage = null;
		this.isPlayerInStorm = false;
		if (!EntityPlayerLocal.StormWarning)
		{
			return;
		}
		if (this.biomeStandingOn != null)
		{
			int stormLevel = this.biomeStandingOn.currentWeatherGroup.stormLevel;
			if (stormLevel > 0 && this.biomeStandingOn.Difficulty >= 2)
			{
				if (stormLevel >= 2)
				{
					this.isPlayerInStorm = !this.isIndoorsCurrent;
				}
				this.AreaMessageAlpha = 1f;
				string key = "weatherStormBuild";
				if (stormLevel == 1)
				{
					if (this.isIndoorsCurrent)
					{
						key = "weatherStormBuildSafe";
						this.AreaMessageAlpha = 0.85f;
					}
				}
				else if (stormLevel >= 2)
				{
					key = "weatherStorm";
					if (this.isIndoorsCurrent)
					{
						key = "weatherStormSafe";
						this.AreaMessageAlpha = 0.75f;
					}
				}
				if (stormLevel == 1)
				{
					int remainingSeconds = (int)WeatherManager.currentWeather.remainingSeconds;
					string arg = "";
					if (remainingSeconds <= 60)
					{
						if (!flag || (float)remainingSeconds > this.stormRemainingSeconds)
						{
							this.stormRemainingSeconds = (float)remainingSeconds;
						}
						this.stormRemainingSeconds = Mathf.MoveTowards(this.stormRemainingSeconds, (float)remainingSeconds, Time.deltaTime);
						arg = string.Format("{0:d2}", (int)(this.stormRemainingSeconds + 0.99f));
					}
					this.AreaMessage = string.Format(Localization.Get(key, false, null), arg);
				}
				else
				{
					this.AreaMessage = Localization.Get(key, false, null);
				}
				if (!flag)
				{
					Manager.PlayInsidePlayerHead("ui_weather_alert", -1, 0f, false, true);
					this.stormAlertBiomeStandingOn = this.biomeStandingOn;
				}
			}
		}
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x000F88C0 File Offset: 0x000F6AC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void WeatherStatusTick()
	{
		if (this.SpawnedTicks > 100)
		{
			bool flag = false;
			BiomeDefinition biomeStandingOn = this.biomeStandingOn;
			BiomeDefinition.WeatherGroup weatherGroup = (biomeStandingOn != null) ? biomeStandingOn.currentWeatherGroup : null;
			if (weatherGroup != null && weatherGroup != this.weatherGroup)
			{
				string format = "{0} WeatherStatusTick {1} to {2}";
				object[] array = new object[3];
				array[0] = base.GetDebugName();
				int num = 1;
				BiomeDefinition.WeatherGroup weatherGroup2 = this.weatherGroup;
				array[num] = ((weatherGroup2 != null) ? weatherGroup2.name : null);
				array[2] = weatherGroup.name;
				Log.Out(format, array);
				if (this.weatherBuff != null)
				{
					this.Buffs.RemoveBuff(this.weatherBuff, -1, true);
				}
				this.weatherGroup = weatherGroup;
				this.weatherBuff = weatherGroup.buffName;
				flag = true;
			}
			bool flag2 = this.shelterPercent > 0f;
			if (this.isIndoorsCurrent != flag2)
			{
				this.isIndoorsCurrent = flag2;
				flag = true;
			}
			if (flag)
			{
				this.WeatherBuffUpdate();
			}
		}
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x000F898C File Offset: 0x000F6B8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void WeatherBuffUpdate()
	{
		Log.Out("{0} WeatherBuffUpdate {1}, indoors {2}", new object[]
		{
			base.GetDebugName(),
			this.weatherBuff,
			this.isIndoorsCurrent
		});
		if (this.weatherBuff != null)
		{
			if (this.isIndoorsCurrent)
			{
				this.Buffs.RemoveBuff(this.weatherBuff, -1, true);
				return;
			}
			this.Buffs.AddBuff(this.weatherBuff, -1, true, false, -1f);
		}
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x000F8A07 File Offset: 0x000F6C07
	public void OnWeatherGodModeChanged(bool _oldValue, bool _newValue)
	{
		if (!_newValue)
		{
			this.WeatherBuffUpdate();
		}
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000F8A14 File Offset: 0x000F6C14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void onNewBiomeEntered(BiomeDefinition _biome)
	{
		if (_biome != null)
		{
			EnvironmentAudioManager.Instance.EnterBiome(_biome);
			this.MinEventContext.Biome = _biome;
			this.FireEvent(MinEventTypes.onSelfEnteredBiome, true);
			QuestEventManager.Current.BiomeEntered(_biome);
			this.lastLootStage = -1;
		}
		if (this.biomeStandingOn != null && this.biomeStandingOn.Buff != null)
		{
			Log.Out("{0} onNewBiomeEntered -{1}", new object[]
			{
				base.GetDebugName(),
				this.biomeStandingOn.Buff
			});
			this.Buffs.RemoveBuff(this.biomeStandingOn.Buff, -1, true);
		}
		if (_biome != null && EntityStats.NewWeatherSurvivalEnabled && _biome.Buff != null)
		{
			Log.Out("{0} onNewBiomeEntered +{1}", new object[]
			{
				base.GetDebugName(),
				_biome.Buff
			});
			this.Buffs.AddBuff(_biome.Buff, -1, true, false, -1f);
		}
		this.biomeStandingOn = _biome;
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x000F8AFF File Offset: 0x000F6CFF
	public void ResetBiomeWeatherOnDeath()
	{
		this.onNewBiomeEntered(null);
		this.isIndoorsCurrent = true;
		this.WeatherBuffUpdate();
		this.weatherBuff = null;
		this.weatherGroup = null;
	}

	// Token: 0x060027FD RID: 10237 RVA: 0x000F8B24 File Offset: 0x000F6D24
	public override float GetWetnessRate()
	{
		float num = Mathf.Pow(this.inWaterPercent, 2f);
		if (this.shelterAbovePercent == 0f)
		{
			float v = WeatherManager.Instance.GetCurrentWetPercent(this) * 0.05f;
			num = Utils.FastMax(num, v);
		}
		return num;
	}

	// Token: 0x060027FE RID: 10238 RVA: 0x000F8B6C File Offset: 0x000F6D6C
	public SpawnPosition GetSpawnPoint()
	{
		if (this.SpawnPoints == null || this.SpawnPoints.Count == 0)
		{
			return SpawnPosition.Undef;
		}
		return new SpawnPosition(this.SpawnPoints[0].ToVector3() + new Vector3(0.5f, 0f, 0.5f), 0f);
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000F8BCB File Offset: 0x000F6DCB
	public override void AddUIHarvestingItem(ItemStack itemStack, bool _bAddOnlyIfNotExisting = false)
	{
		this.playerUI.xui.CollectedItemList.AddItemStack(itemStack, _bAddOnlyIfNotExisting);
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x000F8BE4 File Offset: 0x000F6DE4
	public override Vector3 GetDropPosition()
	{
		Vector3 vector = base.GetDropPosition();
		Vector3 direction = vector - this.getHeadPosition();
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(this.getHeadPosition() - Origin.position, direction), out raycastHit, direction.magnitude, 1073807360))
		{
			vector = raycastHit.point - direction.normalized * 0.5f + Origin.position;
		}
		return vector;
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x000F8C5C File Offset: 0x000F6E5C
	public bool CheckSpawnPointStillThere()
	{
		SpawnPosition spawnPoint = this.GetSpawnPoint();
		return spawnPoint.IsUndef() || this.world.GetChunkFromWorldPos(spawnPoint.ToBlockPos()) == null || this.world.GetBlock(spawnPoint.ToBlockPos()).Block is BlockSleepingBag;
	}

	// Token: 0x06002802 RID: 10242 RVA: 0x000F8CB5 File Offset: 0x000F6EB5
	public void RemoveSpawnPoints(bool showTooltip = true)
	{
		this.SpawnPoints.Clear();
		if (showTooltip)
		{
			GameManager.ShowTooltip(this, Localization.Get("ttBedrollGone", false, null), false, false, 0f);
		}
		this.selectedSpawnPointKey = -1L;
	}

	// Token: 0x06002803 RID: 10243 RVA: 0x000F8CE8 File Offset: 0x000F6EE8
	public void EmptyBackpackAndToolbelt()
	{
		ItemStack[] slots = this.bag.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i] = ItemStack.Empty;
		}
		this.bag.SetSlots(slots);
		for (int j = 0; j < this.inventory.GetItemCount(); j++)
		{
			if (j != this.inventory.DUMMY_SLOT_IDX)
			{
				this.inventory.SetItem(j, ItemStack.Empty);
			}
		}
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000F8D58 File Offset: 0x000F6F58
	public void EmptyBackpack()
	{
		ItemStack[] slots = this.bag.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i] = ItemStack.Empty;
		}
		this.bag.SetSlots(slots);
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000F8D94 File Offset: 0x000F6F94
	public void EmptyToolbelt(int start, int end)
	{
		for (int i = start; i < end; i++)
		{
			if (i != this.inventory.DUMMY_SLOT_IDX)
			{
				this.inventory.SetItem(i, ItemStack.Empty);
			}
		}
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x000F8DCC File Offset: 0x000F6FCC
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleMapObjects(bool usePersistantBackpackPositions)
	{
		if (this.persistentPlayerData != null)
		{
			List<Vector3i> landProtectionBlocks = this.persistentPlayerData.GetLandProtectionBlocks();
			for (int i = 0; i < landProtectionBlocks.Count; i++)
			{
				NavObject navObject = NavObjectManager.Instance.RegisterNavObject("land_claim", landProtectionBlocks[i].ToVector3(), "", false, -1, null);
				if (navObject != null)
				{
					navObject.OwnerEntity = this;
				}
			}
			this.persistentPlayerData.ShowBedrollOnMap();
			this.SetDroppedBackpackPositions(usePersistantBackpackPositions ? this.persistentPlayerData.GetDroppedBackpackPositions() : this.droppedBackpackPositions);
		}
	}

	// Token: 0x06002807 RID: 10247 RVA: 0x000F8E58 File Offset: 0x000F7058
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AnalyticsSendDeath(DamageResponse _dmResponse)
	{
		DamageSource source = _dmResponse.Source;
		string text;
		if (this.isFallDeath)
		{
			text = "fall";
		}
		else if (source.BuffClass != null)
		{
			text = source.BuffClass.Name;
		}
		else if (source.ItemClass != null)
		{
			text = source.ItemClass.Name;
		}
		else
		{
			text = source.damageType.ToStringCached<EnumDamageTypes>();
		}
		if (this.entityThatKilledMe)
		{
			text += "_";
			if (this.entityThatKilledMe == this)
			{
				text += "self";
			}
			else if (this.entityThatKilledMe is EntityPlayer)
			{
				text += "player";
			}
			else
			{
				text += this.entityThatKilledMe.EntityName;
			}
		}
		GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.PlayerDeathCauses, text, 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
	}

	// Token: 0x06002808 RID: 10248 RVA: 0x000F8F20 File Offset: 0x000F7120
	public EntityPlayerLocal.AutoMove EnableAutoMove(bool _enable)
	{
		if (!_enable)
		{
			this.autoMove = null;
		}
		else
		{
			this.autoMove = new EntityPlayerLocal.AutoMove(this);
		}
		return this.autoMove;
	}

	// Token: 0x06002809 RID: 10249 RVA: 0x000F8F40 File Offset: 0x000F7140
	public void TryCancelChargedAction()
	{
		ItemAction itemAction = this.inventory.holdingItem.Actions[0];
		if (itemAction is ItemActionCatapult || itemAction is ItemActionThrowAway)
		{
			itemAction.CancelAction(this.inventory.holdingItemData.actionData[0]);
			this.inventory.holdingItemData.actionData[0].HasExecuted = false;
		}
	}

	// Token: 0x0600280A RID: 10250 RVA: 0x000F8FA8 File Offset: 0x000F71A8
	public bool TryAddRecoveryPosition(Vector3i _position)
	{
		if (this.recoveryPositions.Contains(_position))
		{
			return false;
		}
		if (!GameManager.Instance.World.CanPlayersSpawnAtPos(_position, false) || GameManager.Instance.World.GetPOIAtPosition(_position.ToVector3(), null, null) != null)
		{
			return false;
		}
		if (this.recoveryPositions.Count == 0)
		{
			this.recoveryPositions.Add(_position);
			return true;
		}
		if ((this.recoveryPositions[this.recoveryPositions.Count - 1] - _position).ToVector3().sqrMagnitude < 10000f)
		{
			return false;
		}
		if (this.recoveryPositions.Count >= 5)
		{
			this.recoveryPositions.RemoveAt(0);
		}
		this.recoveryPositions.Add(_position);
		return true;
	}

	// Token: 0x0600280B RID: 10251 RVA: 0x000F9084 File Offset: 0x000F7284
	public void GiveExp(CraftCompleteData data)
	{
		int num = (int)this.Buffs.GetCustomVar("_craftCount_" + data.RecipeName);
		int recipeUsedCount = (int)data.RecipeUsedCount;
		this.Buffs.SetCustomVar("_craftCount_" + data.RecipeName, (float)(num + recipeUsedCount), true, CVarOperation.set, false);
		this.Progression.AddLevelExp(data.CraftExpGain / (num + recipeUsedCount), "_xpFromCrafting", Progression.XPTypes.Crafting, true, true, -1, null);
		this.totalItemsCrafted += (uint)recipeUsedCount;
		QuestEventManager.Current.CraftedItem(data.CraftedItemStack);
		XUiC_RecipeStack.HandleCraftXPGained();
	}

	// Token: 0x0600280C RID: 10252 RVA: 0x000F911A File Offset: 0x000F731A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGUI()
	{
		this.renderManager.OnGUI();
	}

	// Token: 0x0600280D RID: 10253 RVA: 0x000F9127 File Offset: 0x000F7327
	public override void PlayStepSound(float _volume)
	{
		if (!this.bFirstPersonView)
		{
			base.PlayStepSound(_volume);
		}
	}

	// Token: 0x0600280E RID: 10254 RVA: 0x000F9138 File Offset: 0x000F7338
	public void HolsterWeapon(bool holster)
	{
		EntityPlayerLocal.HolsterState holsterState = holster ? EntityPlayerLocal.HolsterState.Holstered : EntityPlayerLocal.HolsterState.Unholstered;
		if (this.desiredHolsterState != holsterState)
		{
			this.desiredHolsterState = holsterState;
		}
	}

	// Token: 0x0600280F RID: 10255 RVA: 0x000F915D File Offset: 0x000F735D
	[PublicizedFrom(EAccessModifier.Private)]
	public void refreshHolsterState()
	{
		this.desiredHolsterState = EntityPlayerLocal.HolsterState.Undefined;
	}

	// Token: 0x06002810 RID: 10256 RVA: 0x000F9168 File Offset: 0x000F7368
	public eTPCameraCheckResult CharacterCameraAngleValid()
	{
		if (this.bFirstPersonView || this.vp_FPCamera.Locked3rdPerson)
		{
			return eTPCameraCheckResult.Pass;
		}
		if (this.LineOfSightObstructed)
		{
			return eTPCameraCheckResult.LineOfSightCheckFailed;
		}
		Vector3 to = new Vector3(this.cameraTransform.forward.x, 0f, this.cameraTransform.forward.z);
		if (Vector3.Angle(base.transform.forward, to) > 20f)
		{
			return eTPCameraCheckResult.AngleCheckFailed;
		}
		return eTPCameraCheckResult.Pass;
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x000F91E0 File Offset: 0x000F73E0
	public void UpdateRespawn()
	{
		float num = Time.realtimeSinceStartup - this.RespawnTeleportDuration;
		if (!this.RespawnTeleportTimeoutLogged && num > 5f)
		{
			this.RespawnTeleportTimeoutLogged = true;
			try
			{
				this.LogFellThroughWorldDebugInfo();
			}
			catch (Exception ex)
			{
				Log.Warning("[FELLTHROUGHWORLD] debug dump threw: " + ex.Message);
			}
		}
		if (this.RespawnTeleportTimeoutLogged && !this.ftwOutcomeReported && num > 20f)
		{
			this.ReportFellThroughOutcome("stuck", num);
		}
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x000F9268 File Offset: 0x000F7468
	public void OnTeleportRespawnCompleted()
	{
		if (!this.RespawnTeleportTimeoutLogged || this.ftwOutcomeReported)
		{
			return;
		}
		this.ftwRespawnSeconds = Time.realtimeSinceStartup - this.RespawnTeleportDuration;
		this.ftwSurfaceY = (int)((this.world != null) ? this.world.GetTerrainHeight((int)this.position.x, (int)this.position.z) : 0);
		this.ftwOutcomeWatchElapsed = 0f;
		this.ftwOutcomeWatchActive = true;
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x000F92E0 File Offset: 0x000F74E0
	public void UpdateRespawnOutcomeWatch()
	{
		if (!this.ftwOutcomeWatchActive || this.ftwOutcomeReported)
		{
			return;
		}
		this.ftwOutcomeWatchElapsed += Time.deltaTime;
		if (this.ftwSurfaceY > 1 && this.position.y < (float)this.ftwSurfaceY - 3f)
		{
			this.ftwOutcomeWatchActive = false;
			this.ReportFellThroughOutcome("spawned-then-fell", this.ftwRespawnSeconds);
			return;
		}
		if (this.ftwOutcomeWatchElapsed >= 3f)
		{
			this.ftwOutcomeWatchActive = false;
			this.ReportFellThroughOutcome("recovered", this.ftwRespawnSeconds);
		}
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x000F9374 File Offset: 0x000F7574
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReportFellThroughOutcome(string outcome, float respawnSeconds)
	{
		if (this.ftwOutcomeReported)
		{
			return;
		}
		this.ftwOutcomeReported = true;
		try
		{
			Vector3i vector3i = World.worldToBlockPos(this.position);
			Chunk chunk = (this.world != null) ? ((Chunk)this.world.GetChunkFromWorldPos(vector3i.x, vector3i.y, vector3i.z)) : null;
			bool flag = chunk == null;
			string text = flag ? "unknown" : chunk.IsCollisionMeshGenerated.ToString();
			int num = VoxelMeshLayer.InstanceCount - MemoryPools.poolVML.GetPoolSize();
			Log.Out(string.Format("[FELLTHROUGHWORLD] outcome={0} respawn={1:F1}s collMesh={2} chunkNull={3} vmlQueued={4} playerY={5:F1} surfaceY={6}", new object[]
			{
				outcome,
				respawnSeconds,
				text,
				flag,
				num,
				this.position.y,
				this.ftwSurfaceY
			}));
			Dictionary<string, string> attributes = new Dictionary<string, string>
			{
				{
					"ftw_outcome",
					outcome
				},
				{
					"ftw_respawn_seconds",
					respawnSeconds.ToString("F1")
				},
				{
					"ftw_collision_mesh",
					text
				},
				{
					"ftw_chunk_null",
					flag.ToString()
				},
				{
					"ftw_vml_queued",
					num.ToString()
				},
				{
					"ftw_player_y",
					this.position.y.ToString("F1")
				},
				{
					"ftw_surface_y",
					this.ftwSurfaceY.ToString()
				}
			};
			BacktraceUtils.SendErrorReport((outcome == "recovered") ? "fellthroughworld_recovered" : "fellthroughworld", "Player fell through world", null, attributes);
		}
		catch (Exception ex)
		{
			Log.Warning("[FELLTHROUGHWORLD] outcome report (" + outcome + ") threw: " + ex.Message);
		}
	}

	// Token: 0x06002815 RID: 10261 RVA: 0x000F9548 File Offset: 0x000F7748
	public void LogFellThroughWorldDebugInfo()
	{
		Vector3i vector3i = World.worldToBlockPos(this.position);
		Chunk chunk = (Chunk)this.world.GetChunkFromWorldPos(vector3i.x, vector3i.y, vector3i.z);
		if (chunk == null)
		{
			Log.Error(string.Format("[FELLTHROUGHWORLD] Teleport respawn failed. Local player blockPos: {0}, Chunk is null.", vector3i));
		}
		else
		{
			long key = chunk.Key;
			Log.Out(string.Format("[FELLTHROUGHWORLD] Teleport respawn failed. Local player blockPos: {0}, Chunk Key: {1}, Chunk Pos X:{2}, Z:{3}", new object[]
			{
				vector3i,
				(chunk != null) ? chunk.Key : "null",
				WorldChunkCache.extractX(chunk.Key) << 4,
				WorldChunkCache.extractZ(chunk.Key) << 4
			}));
			Log.Out(string.Format("[FELLTHROUGHWORLD] Time since chunk {0} selected for data generation: {1}", key, ChunkManager.SecondsSinceChunkSelectedForGeneration(chunk.Key)));
			ValueTuple<int, double> valueTuple = ChunkCluster.SecondsSinceChunkStartedRegeneration(chunk.Key);
			if (valueTuple.Item1 == -1)
			{
				Log.Out(string.Format("[FELLTHROUGHWORLD] Chunk {0} has never started mesh regeneration.", key));
			}
			else
			{
				Log.Out(string.Format("[FELLTHROUGHWORLD] Time since a layer on chunk {0} started mesh regeneration: Layer {1}, {2}", key, valueTuple.Item1, valueTuple.Item2));
			}
			valueTuple = ChunkCluster.SecondsSinceChunkEndedRegeneration(chunk.Key);
			if (valueTuple.Item1 == -1)
			{
				Log.Out(string.Format("[FELLTHROUGHWORLD] Chunk {0} has never finished mesh regeneration.", key));
			}
			else
			{
				Log.Out(string.Format("[FELLTHROUGHWORLD] Time since a layer on chunk {0} finished mesh regeneration: Layer {1}, {2}", key, valueTuple.Item1, valueTuple.Item2));
			}
		}
		ChunkCluster chunkCache = this.world.ChunkCache;
		if (chunk != null)
		{
			chunk.LogChunkState();
		}
		ChunkProviderGenerateWorld chunkProviderGenerateWorld = ((chunkCache != null) ? chunkCache.ChunkProvider : null) as ChunkProviderGenerateWorld;
		if (chunkProviderGenerateWorld != null)
		{
			chunkProviderGenerateWorld.LogCurrentChunkGeneration();
		}
		World world = this.world;
		if (world != null)
		{
			ChunkManager chunkManager = world.m_ChunkManager;
			if (chunkManager != null)
			{
				chunkManager.LogCurrentGenerationState();
			}
		}
		if (chunk != null)
		{
			ChunkCluster.LogCurrentChunkRegenerationState(chunk.Key);
		}
		this.moveController.LogCurrentRespawnState();
	}

	// Token: 0x06002816 RID: 10262 RVA: 0x000F9748 File Offset: 0x000F7948
	public void ClearMovementInputs()
	{
		this.movementInput.moveForward = 0f;
		this.movementInput.moveStrafe = 0f;
		this.movementInput.rotation = Vector3.zero;
		this.vp_FPController.Player.InputMoveVector.Set(Vector2.zero);
		this.vp_FPController.Player.InputSmoothLook.Set(Vector2.zero);
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x000F97C4 File Offset: 0x000F79C4
	public void RequestToSpawnEntityServer(EntityCreationData ecd, Action<Entity> callback, bool blocksCollect)
	{
		Guid guid = Guid.NewGuid();
		while (this.spawnRequests.ContainsKey(guid))
		{
			guid = Guid.NewGuid();
		}
		this.spawnRequests.Add(guid, new EntityPlayerLocal.SpawnRequest(callback, blocksCollect));
		ecd.requestedBy = this.entityId;
		ecd.requestKey = guid;
		GameManager.Instance.RequestToSpawnEntityServer(ecd);
	}

	// Token: 0x06002818 RID: 10264 RVA: 0x000F9820 File Offset: 0x000F7A20
	public void HandleRequestedEntitySpawn(Guid guid, Entity entity)
	{
		EntityPlayerLocal.SpawnRequest spawnRequest;
		if (this.spawnRequests.TryGetValue(guid, out spawnRequest))
		{
			this.spawnRequests.Remove(guid);
			spawnRequest.callback(entity);
		}
	}

	// Token: 0x06002819 RID: 10265 RVA: 0x000F9858 File Offset: 0x000F7A58
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool grabDisabled()
	{
		using (Dictionary<Guid, EntityPlayerLocal.SpawnRequest>.ValueCollection.Enumerator enumerator = this.spawnRequests.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.blocksCollect)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04001D1A RID: 7450
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cHolsterLayerIndex = 1;

	// Token: 0x04001D1B RID: 7451
	public UnityEvent<bool> MountEvent = new UnityEvent<bool>();

	// Token: 0x04001D1D RID: 7453
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float prevStaminaValue;

	// Token: 0x04001D1E RID: 7454
	public float weaponCrossHairAlpha = 0.8f;

	// Token: 0x04001D1F RID: 7455
	public const float cCameraTPVBaseDistance = 0.94f;

	// Token: 0x04001D20 RID: 7456
	public const float cCameraTPVOffsetMin = -0.2f;

	// Token: 0x04001D21 RID: 7457
	public const float cCameraTPVOffsetMax = 3f;

	// Token: 0x04001D22 RID: 7458
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform cameraContainerTransform;

	// Token: 0x04001D23 RID: 7459
	public Transform cameraTransform;

	// Token: 0x04001D24 RID: 7460
	public Camera playerCamera;

	// Token: 0x04001D25 RID: 7461
	public Camera finalCamera;

	// Token: 0x04001D26 RID: 7462
	public bool IsUnderwaterCamera;

	// Token: 0x04001D27 RID: 7463
	public GameRenderManager renderManager;

	// Token: 0x04001D28 RID: 7464
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPController m_vp_FPController;

	// Token: 0x04001D29 RID: 7465
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPCamera m_vp_FPCamera;

	// Token: 0x04001D2A RID: 7466
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPWeapon m_vp_FPWeapon;

	// Token: 0x04001D2B RID: 7467
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_PlayerEventHandler m_vp_FPPlayerEventHandler;

	// Token: 0x04001D2C RID: 7468
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_checked_vp_FPController;

	// Token: 0x04001D2D RID: 7469
	public WorldRayHitInfo HitInfo = new WorldRayHitInfo();

	// Token: 0x04001D2E RID: 7470
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float SPRINT_GRACE_PERIOD = 0.2f;

	// Token: 0x04001D2F RID: 7471
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeInSprintGrace;

	// Token: 0x04001D30 RID: 7472
	public float RespawnTeleportDuration;

	// Token: 0x04001D31 RID: 7473
	public const float RespawnTeleportTimeout = 5f;

	// Token: 0x04001D32 RID: 7474
	public bool RespawnTeleportTimeoutLogged;

	// Token: 0x04001D33 RID: 7475
	public const float RespawnStuckTimeout = 20f;

	// Token: 0x04001D34 RID: 7476
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float ftwPostSpawnWatchSeconds = 3f;

	// Token: 0x04001D35 RID: 7477
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float ftwFallBelowSurface = 3f;

	// Token: 0x04001D36 RID: 7478
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ftwOutcomeReported;

	// Token: 0x04001D37 RID: 7479
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ftwOutcomeWatchActive;

	// Token: 0x04001D38 RID: 7480
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ftwOutcomeWatchElapsed;

	// Token: 0x04001D39 RID: 7481
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float ftwRespawnSeconds;

	// Token: 0x04001D3A RID: 7482
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int ftwSurfaceY;

	// Token: 0x04001D3B RID: 7483
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float VIBRATION_LOW = 0.25f;

	// Token: 0x04001D3C RID: 7484
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float VIBRATION_MEDIUM = 0.35f;

	// Token: 0x04001D3D RID: 7485
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float VIBRATION_HIGH = 0.5f;

	// Token: 0x04001D3E RID: 7486
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float VIBRATION_DURATION = 0.25f;

	// Token: 0x04001D3F RID: 7487
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float oldHealth;

	// Token: 0x04001D40 RID: 7488
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float vibrationTimeout = float.MaxValue;

	// Token: 0x04001D41 RID: 7489
	public static EntityPlayerLocal.DropOption DropOnDeathOption;

	// Token: 0x04001D42 RID: 7490
	public static EntityPlayerLocal.DropOption DropOnQuitOption;

	// Token: 0x04001D43 RID: 7491
	public static int LostItemOnDeathMin = 0;

	// Token: 0x04001D44 RID: 7492
	public static int LostItemOnDeathMax = 0;

	// Token: 0x04001D45 RID: 7493
	public static EntityPlayerLocal.DegradeOnDeathTypes DegradeOnDeathType = EntityPlayerLocal.DegradeOnDeathTypes.None;

	// Token: 0x04001D46 RID: 7494
	public static float DegradeAmountOnDeath = 0.1f;

	// Token: 0x04001D47 RID: 7495
	public static float InfectionChance = 1f;

	// Token: 0x04001D48 RID: 7496
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal.HolsterState desiredHolsterState = EntityPlayerLocal.HolsterState.Undefined;

	// Token: 0x04001D49 RID: 7497
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal.HolsterState lastHolsterState = EntityPlayerLocal.HolsterState.Unholstered;

	// Token: 0x04001D4A RID: 7498
	public bool flipCameraSide;

	// Token: 0x04001D4B RID: 7499
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float cMaxLineOfSightCheckRange = 1f;

	// Token: 0x04001D4C RID: 7500
	public bool LineOfSightObstructed;

	// Token: 0x04001D4D RID: 7501
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lineOfSightObstructionPoint;

	// Token: 0x04001D4E RID: 7502
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Ray lineOfSightRay;

	// Token: 0x04001D4F RID: 7503
	public static bool StormWarning = true;

	// Token: 0x04001D50 RID: 7504
	public PersistentPlayerData persistentPlayerData;

	// Token: 0x04001D51 RID: 7505
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float lastTimeJetpackDecreased;

	// Token: 0x04001D52 RID: 7506
	public bool bFirstPersonView = true;

	// Token: 0x04001D53 RID: 7507
	public bool bPreferFirstPerson = true;

	// Token: 0x04001D54 RID: 7508
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float crossHairOpenArea;

	// Token: 0x04001D55 RID: 7509
	public MovementInput movementInput = new MovementInput();

	// Token: 0x04001D56 RID: 7510
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool inputWasJump;

	// Token: 0x04001D57 RID: 7511
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool inputWasDown;

	// Token: 0x04001D58 RID: 7512
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool jumpTrigger;

	// Token: 0x04001D59 RID: 7513
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bSwitchCameraBackAfterRespawn;

	// Token: 0x04001D5A RID: 7514
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bSwitchTo3rdPersonAfterAiming;

	// Token: 0x04001D5B RID: 7515
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 selfCameraPos;

	// Token: 0x04001D5C RID: 7516
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 selfCameraSeekPos;

	// Token: 0x04001D5D RID: 7517
	public bool bExhausted;

	// Token: 0x04001D5E RID: 7518
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isExhaustedSoundAllowed = true;

	// Token: 0x04001D5F RID: 7519
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemStack dragAndDropItem;

	// Token: 0x04001D60 RID: 7520
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isReloadCancelling;

	// Token: 0x04001D62 RID: 7522
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isLadderAttached;

	// Token: 0x04001D63 RID: 7523
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canLadderAirAttach = true;

	// Token: 0x04001D64 RID: 7524
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasJumping;

	// Token: 0x04001D65 RID: 7525
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasJumpTrigger;

	// Token: 0x04001D66 RID: 7526
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasLadderAttachedJump;

	// Token: 0x04001D67 RID: 7527
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int swimMode = -1;

	// Token: 0x04001D68 RID: 7528
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int swimExhaustedTicks;

	// Token: 0x04001D69 RID: 7529
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool swimClimbing;

	// Token: 0x04001D6A RID: 7530
	public bool bLerpCameraFlag;

	// Token: 0x04001D6B RID: 7531
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lerpCameraLerpValue;

	// Token: 0x04001D6C RID: 7532
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lerpCameraStartFOV;

	// Token: 0x04001D6D RID: 7533
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lerpCameraEndFOV;

	// Token: 0x04001D6E RID: 7534
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lerpCameraFastFOV;

	// Token: 0x04001D6F RID: 7535
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastOverrideFOV = -1f;

	// Token: 0x04001D70 RID: 7536
	public float OverrideFOV = -1f;

	// Token: 0x04001D71 RID: 7537
	public Vector3 OverrideLookAt = Vector3.zero;

	// Token: 0x04001D72 RID: 7538
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float biomeVolume;

	// Token: 0x04001D73 RID: 7539
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource audioSourceBiomeActive;

	// Token: 0x04001D74 RID: 7540
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource audioSourceBiomeFadeOut;

	// Token: 0x04001D75 RID: 7541
	public BlendCycleTimer sneakDamageBlendTimer = new BlendCycleTimer(0.5f, 2f, 0.5f);

	// Token: 0x04001D76 RID: 7542
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string sneakDamageText = "";

	// Token: 0x04001D77 RID: 7543
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string dropInventoryBlock;

	// Token: 0x04001D78 RID: 7544
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int runTicks;

	// Token: 0x04001D79 RID: 7545
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int sprintLoopSoundPlayId = -1;

	// Token: 0x04001D7A RID: 7546
	public DynamicMusicManager DynamicMusicManager;

	// Token: 0x04001D7B RID: 7547
	public IThreatLevel ThreatLevel;

	// Token: 0x04001D7C RID: 7548
	public float LastTargetEventTime;

	// Token: 0x04001D7D RID: 7549
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material overlayMaterial;

	// Token: 0x04001D7E RID: 7550
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public byte[] overlayAlternating = new byte[4];

	// Token: 0x04001D7F RID: 7551
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float[] overlayDirectionTime = new float[8];

	// Token: 0x04001D80 RID: 7552
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2[] overlayBloodDropsPositions = new Vector2[24];

	// Token: 0x04001D81 RID: 7553
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectRefract;

	// Token: 0x04001D82 RID: 7554
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectDebris;

	// Token: 0x04001D83 RID: 7555
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectDroplets;

	// Token: 0x04001D84 RID: 7556
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectWaterFade;

	// Token: 0x04001D85 RID: 7557
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform uwEffectHaze;

	// Token: 0x04001D86 RID: 7558
	public bool bIntroAnimActive;

	// Token: 0x04001D87 RID: 7559
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LocalPlayerUI playerUI;

	// Token: 0x04001D88 RID: 7560
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindowManager windowManager;

	// Token: 0x04001D89 RID: 7561
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NGUIWindowManager nguiWindowManager;

	// Token: 0x04001D8A RID: 7562
	public string AreaMessage;

	// Token: 0x04001D8B RID: 7563
	public float AreaMessageAlpha;

	// Token: 0x04001D8C RID: 7564
	public ScreenEffects ScreenEffectManager;

	// Token: 0x04001D8D RID: 7565
	public float GodModeSpeedModifier = 1f;

	// Token: 0x04001D8E RID: 7566
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public PlayerMoveController moveController;

	// Token: 0x04001D8F RID: 7567
	public bool isStunned;

	// Token: 0x04001D90 RID: 7568
	public bool isDeafened;

	// Token: 0x04001D91 RID: 7569
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int oldLayer = -1;

	// Token: 0x04001D92 RID: 7570
	public Rect ZombieCompassBounds;

	// Token: 0x04001D93 RID: 7571
	public float DropTimeDelay;

	// Token: 0x04001D94 RID: 7572
	public float InteractTimeDelay;

	// Token: 0x04001D95 RID: 7573
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float recoveryPointTimer;

	// Token: 0x04001D96 RID: 7574
	public List<Vector3i> recoveryPositions = new List<Vector3i>();

	// Token: 0x04001D97 RID: 7575
	public bool DebugDismembermentChance;

	// Token: 0x04001D98 RID: 7576
	public bool BloodMoonParticipation;

	// Token: 0x04001D99 RID: 7577
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool inAir;

	// Token: 0x04001D9A RID: 7578
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool _cameraRelativeMovement;

	// Token: 0x04001D9B RID: 7579
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cAimingCameraResetTime = 2f;

	// Token: 0x04001D9C RID: 7580
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool tpCameraLockTimerActive;

	// Token: 0x04001D9D RID: 7581
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float tpCameraLockStartTime;

	// Token: 0x04001D9E RID: 7582
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMinOvereadObstructionTime = 0.5f;

	// Token: 0x04001D9F RID: 7583
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float camOverheadDistance = 1f;

	// Token: 0x04001DA0 RID: 7584
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float overheadObstructionTime;

	// Token: 0x04001DA1 RID: 7585
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMinCameraDistanceMultiplier = 1f;

	// Token: 0x04001DA2 RID: 7586
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMaxCameraDistanceMultiplier = 3f;

	// Token: 0x04001DA3 RID: 7587
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraDistanceMulti = 1f;

	// Token: 0x04001DA4 RID: 7588
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CharacterMatrixOverride characterMatrixOverride;

	// Token: 0x04001DA5 RID: 7589
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_MoveVector;

	// Token: 0x04001DA6 RID: 7590
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_SmoothLook;

	// Token: 0x04001DA7 RID: 7591
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal.MoveState moveState;

	// Token: 0x04001DA8 RID: 7592
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool moveStateAiming;

	// Token: 0x04001DA9 RID: 7593
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool moveStateHoldBow;

	// Token: 0x04001DAA RID: 7594
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDragBase = 0.01f;

	// Token: 0x04001DAB RID: 7595
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDragScale = 5.4f;

	// Token: 0x04001DAC RID: 7596
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDragPow = 2f;

	// Token: 0x04001DAD RID: 7597
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelExhausted = 0.00025f;

	// Token: 0x04001DAE RID: 7598
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccel = 0.00032f;

	// Token: 0x04001DAF RID: 7599
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelRun = 0.0024f;

	// Token: 0x04001DB0 RID: 7600
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelUp = 0.00052f;

	// Token: 0x04001DB1 RID: 7601
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelUpGrounded = 0.05f;

	// Token: 0x04001DB2 RID: 7602
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAccelDown = -0.00038f;

	// Token: 0x04001DB3 RID: 7603
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float achievementDistanceAccu;

	// Token: 0x04001DB4 RID: 7604
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastWaypointUpdateTime;

	// Token: 0x04001DB5 RID: 7605
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cWaypointUpdateTime = 30f;

	// Token: 0x04001DB6 RID: 7606
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float updateBedrollPositionChecks;

	// Token: 0x04001DB7 RID: 7607
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float updateRadiationChecks;

	// Token: 0x04001DB8 RID: 7608
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int blockRadiusEffectsIndex;

	// Token: 0x04001DB9 RID: 7609
	public bool LootAtMax;

	// Token: 0x04001DBA RID: 7610
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastLootStage = -1;

	// Token: 0x04001DBB RID: 7611
	public eTPCameraCheckResult TPCameraCheckResult;

	// Token: 0x04001DBC RID: 7612
	public float InWorldPercent;

	// Token: 0x04001DBD RID: 7613
	public float InWorldLookPercent;

	// Token: 0x04001DBE RID: 7614
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeUnderwater;

	// Token: 0x04001DBF RID: 7615
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasSpawned;

	// Token: 0x04001DC0 RID: 7616
	public float spawnEffectPow = 4.19f;

	// Token: 0x04001DC1 RID: 7617
	public static float spawnInEffectSpeed = 3f;

	// Token: 0x04001DC2 RID: 7618
	public bool bPlayingSpawnIn;

	// Token: 0x04001DC3 RID: 7619
	public float spawnInTime;

	// Token: 0x04001DC4 RID: 7620
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float spawnInIntensity;

	// Token: 0x04001DC5 RID: 7621
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float deathTime;

	// Token: 0x04001DC6 RID: 7622
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Predicate<ItemStack> dropStackCondition = (ItemStack s) => s != null && !s.IsEmpty() && !s.itemValue.ItemClassOrMissing.KeepOnDeath();

	// Token: 0x04001DC7 RID: 7623
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Predicate<ItemValue> dropValueCondition = (ItemValue v) => v != null && !v.IsEmpty() && !v.ItemClassOrMissing.KeepOnDeath();

	// Token: 0x04001DC8 RID: 7624
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDyingEffectSpeed = 600f;

	// Token: 0x04001DC9 RID: 7625
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDyingEffectStartHealth = 70f;

	// Token: 0x04001DCA RID: 7626
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDyingEffectHealthThreshold = 3f;

	// Token: 0x04001DCB RID: 7627
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dyingEffectHitTime;

	// Token: 0x04001DCC RID: 7628
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dyingEffectCur;

	// Token: 0x04001DCD RID: 7629
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dyingEffectLast;

	// Token: 0x04001DCE RID: 7630
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dyingEffectHealthLast;

	// Token: 0x04001DCF RID: 7631
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int fallHealth;

	// Token: 0x04001DD0 RID: 7632
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFallDeath;

	// Token: 0x04001DD1 RID: 7633
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<NetPackageSharedQuest.SharedQuestData> sharedQuestsToProcess = new List<NetPackageSharedQuest.SharedQuestData>();

	// Token: 0x04001DD2 RID: 7634
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<Collider> setLayerRecursivelyList = new List<Collider>();

	// Token: 0x04001DD3 RID: 7635
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool cancellingInventoryActions;

	// Token: 0x04001DD4 RID: 7636
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastCameraDistanceAmount;

	// Token: 0x04001DD5 RID: 7637
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NavObject backpackNavObject;

	// Token: 0x04001DD6 RID: 7638
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<NavObject> backpackNavObjects;

	// Token: 0x04001DD7 RID: 7639
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3i> backpackPositionsFromThread = new List<Vector3i>();

	// Token: 0x04001DD8 RID: 7640
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<EntityPlayerLocal.LocationSlotInfo> DestroyLocationSlotList;

	// Token: 0x04001DD9 RID: 7641
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject[] screenBloodEffect;

	// Token: 0x04001DDA RID: 7642
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material[] screenBloodMtrl;

	// Token: 0x04001DDB RID: 7643
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool healthLostThisRound;

	// Token: 0x04001DDC RID: 7644
	public static List<Color> crosshairColors = new List<Color>
	{
		new Color(1f, 1f, 1f, 1f),
		new Color(1f, 0.1f, 0f, 1f),
		new Color(1f, 0.4f, 0f, 1f),
		new Color(1f, 0.8f, 0f, 1f),
		new Color(0f, 1f, 0.3f, 1f),
		new Color(0f, 1f, 1f, 1f),
		new Color(0f, 0.5f, 1f, 1f),
		new Color(1f, 0f, 0.9f, 1f),
		new Color(1f, 0.6f, 0.9f, 1f)
	};

	// Token: 0x04001DDD RID: 7645
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cCrosshairScreenHeightFactor = 0.059f;

	// Token: 0x04001DDE RID: 7646
	public float shelterPercent;

	// Token: 0x04001DDF RID: 7647
	public float shelterAbovePercent;

	// Token: 0x04001DE0 RID: 7648
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool shelterIsUpdating;

	// Token: 0x04001DE1 RID: 7649
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int shelterSideCount;

	// Token: 0x04001DE2 RID: 7650
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 shelterStartPos;

	// Token: 0x04001DE3 RID: 7651
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 shelterPos;

	// Token: 0x04001DE4 RID: 7652
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 shelterPosOffset;

	// Token: 0x04001DE5 RID: 7653
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float shelterDir;

	// Token: 0x04001DE6 RID: 7654
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float shelterRadius;

	// Token: 0x04001DE7 RID: 7655
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterSideStep = 0.13f;

	// Token: 0x04001DE8 RID: 7656
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterUpDistance = 253f;

	// Token: 0x04001DE9 RID: 7657
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cShelterAngles = 8;

	// Token: 0x04001DEA RID: 7658
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterAngleStep = 45f;

	// Token: 0x04001DEB RID: 7659
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterRadiusStep = 1f;

	// Token: 0x04001DEC RID: 7660
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cShelterRadiusMax = 3f;

	// Token: 0x04001DED RID: 7661
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cShelterMask = 1073807360;

	// Token: 0x04001DEE RID: 7662
	public bool isPlayerInStorm;

	// Token: 0x04001DEF RID: 7663
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeDefinition stormAlertBiomeStandingOn;

	// Token: 0x04001DF0 RID: 7664
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stormRemainingSeconds;

	// Token: 0x04001DF1 RID: 7665
	public bool isIndoorsCurrent = true;

	// Token: 0x04001DF2 RID: 7666
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeDefinition.WeatherGroup weatherGroup;

	// Token: 0x04001DF3 RID: 7667
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string weatherBuff;

	// Token: 0x04001DF4 RID: 7668
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal.AutoMove autoMove;

	// Token: 0x04001DF5 RID: 7669
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Guid, EntityPlayerLocal.SpawnRequest> spawnRequests = new Dictionary<Guid, EntityPlayerLocal.SpawnRequest>();

	// Token: 0x020004CA RID: 1226
	public enum DegradeOnDeathTypes
	{
		// Token: 0x04001DF7 RID: 7671
		None,
		// Token: 0x04001DF8 RID: 7672
		Durability,
		// Token: 0x04001DF9 RID: 7673
		MaxDurability,
		// Token: 0x04001DFA RID: 7674
		Both
	}

	// Token: 0x020004CB RID: 1227
	[PublicizedFrom(EAccessModifier.Private)]
	public enum HolsterState
	{
		// Token: 0x04001DFC RID: 7676
		Holstered,
		// Token: 0x04001DFD RID: 7677
		Unholstered,
		// Token: 0x04001DFE RID: 7678
		Undefined
	}

	// Token: 0x020004CC RID: 1228
	[PublicizedFrom(EAccessModifier.Private)]
	public enum MoveState
	{
		// Token: 0x04001E00 RID: 7680
		None,
		// Token: 0x04001E01 RID: 7681
		Off,
		// Token: 0x04001E02 RID: 7682
		Attached,
		// Token: 0x04001E03 RID: 7683
		Idle,
		// Token: 0x04001E04 RID: 7684
		Walk,
		// Token: 0x04001E05 RID: 7685
		Run,
		// Token: 0x04001E06 RID: 7686
		Swim,
		// Token: 0x04001E07 RID: 7687
		Crouch,
		// Token: 0x04001E08 RID: 7688
		CrouchWalk,
		// Token: 0x04001E09 RID: 7689
		CrouchRun,
		// Token: 0x04001E0A RID: 7690
		Jump
	}

	// Token: 0x020004CD RID: 1229
	[PublicizedFrom(EAccessModifier.Private)]
	public struct LocationSlotInfo
	{
		// Token: 0x0600281C RID: 10268 RVA: 0x000F9BCA File Offset: 0x000F7DCA
		public LocationSlotInfo(EntityPlayerLocal.LocationSlotInfo.LocationTypes location, int slot)
		{
			this.Location = location;
			this.Slot = slot;
		}

		// Token: 0x04001E0B RID: 7691
		public EntityPlayerLocal.LocationSlotInfo.LocationTypes Location;

		// Token: 0x04001E0C RID: 7692
		public int Slot;

		// Token: 0x020004CE RID: 1230
		public enum LocationTypes
		{
			// Token: 0x04001E0E RID: 7694
			Backpack,
			// Token: 0x04001E0F RID: 7695
			Toolbelt,
			// Token: 0x04001E10 RID: 7696
			Equipment
		}
	}

	// Token: 0x020004CF RID: 1231
	public enum DropOption
	{
		// Token: 0x04001E12 RID: 7698
		None,
		// Token: 0x04001E13 RID: 7699
		All,
		// Token: 0x04001E14 RID: 7700
		Toolbelt,
		// Token: 0x04001E15 RID: 7701
		Backpack,
		// Token: 0x04001E16 RID: 7702
		Equipment,
		// Token: 0x04001E17 RID: 7703
		ToolbeltBackpack,
		// Token: 0x04001E18 RID: 7704
		DeleteAll
	}

	// Token: 0x020004D0 RID: 1232
	public class AutoMove
	{
		// Token: 0x0600281D RID: 10269 RVA: 0x000F9BDA File Offset: 0x000F7DDA
		public AutoMove(Entity _entity)
		{
			this.entity = _entity;
		}

		// Token: 0x0600281E RID: 10270 RVA: 0x000F9BEC File Offset: 0x000F7DEC
		public void StartLine(float _duration, int _loopCount, Vector3 _endPos, Action _onMoveComplete = null)
		{
			this.mode = EntityPlayerLocal.AutoMove.Mode.Line;
			this.endTime = _duration;
			this.loopCount = _loopCount;
			this.startPos = this.entity.position;
			this.targetPos = _endPos;
			this.isPingPong = false;
			if (this.loopCount < 0)
			{
				this.loopCount *= -2;
				this.isPingPong = true;
			}
			this.onMoveComplete = _onMoveComplete;
		}

		// Token: 0x0600281F RID: 10271 RVA: 0x000F9C54 File Offset: 0x000F7E54
		public void StartOrbit(float _duration, int _loopCount, Vector3 _orbitPos, bool _lookForward = false, bool _isFlipped = false, Action _onMoveComplete = null)
		{
			this.mode = EntityPlayerLocal.AutoMove.Mode.Orbit;
			this.endTime = _duration;
			this.loopCount = _loopCount;
			this.startPos = this.entity.position;
			this.targetPos = _orbitPos;
			this.orbitLookForward = _lookForward;
			this.isFlipped = _isFlipped;
			this.isPingPong = false;
			if (this.loopCount < 0)
			{
				this.loopCount *= -2;
				this.isPingPong = true;
			}
			this.onMoveComplete = _onMoveComplete;
		}

		// Token: 0x06002820 RID: 10272 RVA: 0x000F9CCC File Offset: 0x000F7ECC
		public void StartRelative(float velX, float velZ, float rotVel)
		{
			this.mode = EntityPlayerLocal.AutoMove.Mode.Relative;
			this.vel.x = velX;
			this.vel.z = velZ;
			this.rotY = this.entity.rotation.y;
			this.rotYVel = rotVel;
		}

		// Token: 0x06002821 RID: 10273 RVA: 0x000F9D0A File Offset: 0x000F7F0A
		public void SetLookAt(Vector3 _pos)
		{
			this.lookAtPos = _pos;
		}

		// Token: 0x06002822 RID: 10274 RVA: 0x000F9D14 File Offset: 0x000F7F14
		public void Update()
		{
			if (this.mode == EntityPlayerLocal.AutoMove.Mode.Off)
			{
				return;
			}
			if (this.mode == EntityPlayerLocal.AutoMove.Mode.Line)
			{
				this.curTime += Time.deltaTime;
				float num = this.curTime / this.endTime;
				if (num > 1f)
				{
					this.curTime = 0f;
					num = 0f;
					if (this.isPingPong)
					{
						Vector3 vector = this.startPos;
						Vector3 vector2 = this.targetPos;
						this.targetPos = vector;
						this.startPos = vector2;
					}
					int num2 = this.loopCount - 1;
					this.loopCount = num2;
					if (num2 <= 0)
					{
						num = (float)(this.isPingPong ? 0 : 1);
						Action action = this.onMoveComplete;
						if (action != null)
						{
							action();
						}
						this.mode = EntityPlayerLocal.AutoMove.Mode.Off;
					}
				}
				Vector3 pos = Vector3.Lerp(this.startPos, this.targetPos, num);
				this.entity.SetPosition(pos, true);
			}
			if (this.mode == EntityPlayerLocal.AutoMove.Mode.Orbit)
			{
				this.curTime += Time.deltaTime;
				float num3 = this.curTime / this.endTime;
				if (num3 > 1f)
				{
					this.curTime -= this.endTime;
					num3 = this.curTime / this.endTime;
					if (this.isPingPong)
					{
						this.isFlipped = !this.isFlipped;
					}
					int num2 = this.loopCount - 1;
					this.loopCount = num2;
					if (num2 <= 0)
					{
						num3 = 1f;
						Action action2 = this.onMoveComplete;
						if (action2 != null)
						{
							action2();
						}
						this.mode = EntityPlayerLocal.AutoMove.Mode.Off;
					}
				}
				Vector3 point = this.startPos - this.targetPos;
				float num4 = 360f * num3;
				num4 *= (float)(this.isFlipped ? -1 : 1);
				this.entity.SetPosition(this.targetPos + Quaternion.Euler(0f, num4, 0f) * point, true);
				Quaternion quaternion;
				if (this.orbitLookForward)
				{
					quaternion = Quaternion.Euler(0f, num4 - 90f, 0f);
				}
				else
				{
					quaternion = Quaternion.LookRotation(this.targetPos - this.entity.position, Vector3.up);
				}
				Vector3 eulerAngles = quaternion.eulerAngles;
				eulerAngles.x *= -1f;
				this.entity.SetRotation(eulerAngles);
			}
			if (this.mode == EntityPlayerLocal.AutoMove.Mode.Relative)
			{
				this.rotY += this.rotYVel * Time.deltaTime;
				this.entity.SetRotation(new Vector3(0f, this.rotY, 0f));
				this.entity.SetPosition(this.entity.position + Quaternion.Euler(0f, this.rotY, 0f) * this.vel * Time.deltaTime, true);
			}
			if (this.lookAtPos.sqrMagnitude > 0f)
			{
				Vector3 eulerAngles2 = Quaternion.LookRotation(this.lookAtPos - this.entity.position, Vector3.up).eulerAngles;
				eulerAngles2.x *= -1f;
				this.entity.SetRotation(eulerAngles2);
			}
		}

		// Token: 0x04001E19 RID: 7705
		[PublicizedFrom(EAccessModifier.Private)]
		public Entity entity;

		// Token: 0x04001E1A RID: 7706
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal.AutoMove.Mode mode;

		// Token: 0x04001E1B RID: 7707
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 startPos;

		// Token: 0x04001E1C RID: 7708
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 targetPos;

		// Token: 0x04001E1D RID: 7709
		[PublicizedFrom(EAccessModifier.Private)]
		public float curTime;

		// Token: 0x04001E1E RID: 7710
		[PublicizedFrom(EAccessModifier.Private)]
		public float endTime;

		// Token: 0x04001E1F RID: 7711
		[PublicizedFrom(EAccessModifier.Private)]
		public int loopCount;

		// Token: 0x04001E20 RID: 7712
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isPingPong;

		// Token: 0x04001E21 RID: 7713
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isFlipped;

		// Token: 0x04001E22 RID: 7714
		[PublicizedFrom(EAccessModifier.Private)]
		public bool orbitLookForward;

		// Token: 0x04001E23 RID: 7715
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 vel;

		// Token: 0x04001E24 RID: 7716
		[PublicizedFrom(EAccessModifier.Private)]
		public float rotY;

		// Token: 0x04001E25 RID: 7717
		[PublicizedFrom(EAccessModifier.Private)]
		public float rotYVel;

		// Token: 0x04001E26 RID: 7718
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 lookAtPos;

		// Token: 0x04001E27 RID: 7719
		[PublicizedFrom(EAccessModifier.Private)]
		public Action onMoveComplete;

		// Token: 0x020004D1 RID: 1233
		[PublicizedFrom(EAccessModifier.Private)]
		public enum Mode
		{
			// Token: 0x04001E29 RID: 7721
			Off,
			// Token: 0x04001E2A RID: 7722
			Line,
			// Token: 0x04001E2B RID: 7723
			Orbit,
			// Token: 0x04001E2C RID: 7724
			Relative
		}
	}

	// Token: 0x020004D2 RID: 1234
	[PublicizedFrom(EAccessModifier.Protected)]
	public class SpawnRequest
	{
		// Token: 0x06002823 RID: 10275 RVA: 0x000FA040 File Offset: 0x000F8240
		public SpawnRequest(Action<Entity> _callback, bool _blocksCollect)
		{
			this.callback = _callback;
			this.blocksCollect = _blocksCollect;
		}

		// Token: 0x04001E2D RID: 7725
		public Action<Entity> callback;

		// Token: 0x04001E2E RID: 7726
		public bool blocksCollect;
	}
}
