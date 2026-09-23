using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using Audio;
using BhvrAnalyticsServices.Interfaces;
using GUI_2;
using InControl;
using MapRendering;
using Platform;
using SandboxOptions;
using Services;
using Services.Analytics;
using Services.Analytics.Events;
using Twitch;
using Unity.Collections;
using UnityEngine;
using Webserver;

// Token: 0x020011A7 RID: 4519
public class GameManager : MonoBehaviour, IGameManager
{
	// Token: 0x06009053 RID: 36947 RVA: 0x003639F8 File Offset: 0x00361BF8
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator waitForGameStart()
	{
		if (!GameManager.IsDedicatedServer && !this.IsEditMode())
		{
			EntityPlayerLocal epl = null;
			while (this.World != null)
			{
				epl = this.World.GetPrimaryPlayer();
				if (epl != null)
				{
					while (!epl.IsSpawned())
					{
						yield return null;
					}
					epl.HasUpdated = false;
					while (!epl.HasUpdated)
					{
						yield return false;
					}
					yield return null;
					this.GameHasStarted = true;
					epl = null;
					goto IL_10F;
				}
				yield return null;
			}
			yield break;
		}
		IL_10F:
		yield break;
	}

	// Token: 0x06009054 RID: 36948 RVA: 0x00363A08 File Offset: 0x00361C08
	public void ShowBackground(bool show)
	{
		if (this.bShowBackground == show)
		{
			return;
		}
		this.bShowBackground = show;
		Camera main = Camera.main;
		if (main != null)
		{
			if (!this.bShowBackground)
			{
				this.cameraCullMask = main.cullingMask;
				main.cullingMask = LayerMask.GetMask(new string[]
				{
					"LocalPlayer"
				});
				main.backgroundColor = this.backgroundColor;
				return;
			}
			main.cullingMask = this.cameraCullMask;
		}
	}

	// Token: 0x06009055 RID: 36949 RVA: 0x00363A7B File Offset: 0x00361C7B
	public bool ShowBackground()
	{
		return this.bShowBackground;
	}

	// Token: 0x06009056 RID: 36950 RVA: 0x00363A84 File Offset: 0x00361C84
	public void IncreaseBackgroundColor()
	{
		switch (this.currentBackgroundColorChannel)
		{
		case 0:
			this.backgroundColor.r = this.backgroundColor.r + 0.003921569f;
			break;
		case 1:
			this.backgroundColor.g = this.backgroundColor.g + 0.003921569f;
			break;
		case 2:
			this.backgroundColor.b = this.backgroundColor.b + 0.003921569f;
			break;
		}
		this.backgroundColor.r = Mathf.Clamp01(this.backgroundColor.r);
		this.backgroundColor.g = Mathf.Clamp01(this.backgroundColor.g);
		this.backgroundColor.b = Mathf.Clamp01(this.backgroundColor.b);
		Camera main = Camera.main;
		if (main != null)
		{
			main.backgroundColor = this.backgroundColor;
		}
	}

	// Token: 0x06009057 RID: 36951 RVA: 0x00363B58 File Offset: 0x00361D58
	public void DecreaseBackgroundColor()
	{
		switch (this.currentBackgroundColorChannel)
		{
		case 0:
			this.backgroundColor.r = this.backgroundColor.r - 0.003921569f;
			break;
		case 1:
			this.backgroundColor.g = this.backgroundColor.g - 0.003921569f;
			break;
		case 2:
			this.backgroundColor.b = this.backgroundColor.b - 0.003921569f;
			break;
		}
		this.backgroundColor.r = Mathf.Clamp01(this.backgroundColor.r);
		this.backgroundColor.g = Mathf.Clamp01(this.backgroundColor.g);
		this.backgroundColor.b = Mathf.Clamp01(this.backgroundColor.b);
		Camera main = Camera.main;
		if (main != null)
		{
			main.backgroundColor = this.backgroundColor;
		}
	}

	// Token: 0x06009058 RID: 36952 RVA: 0x00363C2C File Offset: 0x00361E2C
	public void BackgroundColorNext()
	{
		this.currentBackgroundColorChannel++;
		if (this.currentBackgroundColorChannel > 2)
		{
			this.currentBackgroundColorChannel = 0;
		}
	}

	// Token: 0x06009059 RID: 36953 RVA: 0x00363C4C File Offset: 0x00361E4C
	public void BackgroundColorPrev()
	{
		this.currentBackgroundColorChannel--;
		if (this.currentBackgroundColorChannel < 0)
		{
			this.currentBackgroundColorChannel = 2;
		}
	}

	// Token: 0x17001146 RID: 4422
	// (get) Token: 0x0600905A RID: 36954 RVA: 0x00363C6C File Offset: 0x00361E6C
	public static bool IsDedicatedServer
	{
		get
		{
			if (!GameManager.isDedicatedChecked)
			{
				string[] commandLineArgs = GameStartupHelper.GetCommandLineArgs();
				for (int i = 0; i < commandLineArgs.Length; i++)
				{
					if (commandLineArgs[i].Equals(global::Constants.cArgDedicatedServer))
					{
						GameManager.isDedicated = true;
					}
				}
				GameManager.isDedicatedChecked = true;
			}
			return GameManager.isDedicated;
		}
	}

	// Token: 0x0600905B RID: 36955 RVA: 0x00363CB4 File Offset: 0x00361EB4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnUserDetailsUpdated(IPlatformUserData userData, string name)
	{
		if (this.persistentPlayers != null)
		{
			this.persistentPlayers.HandlePlayerDetailsUpdate(userData, name);
		}
	}

	// Token: 0x17001147 RID: 4423
	// (get) Token: 0x0600905C RID: 36956 RVA: 0x00363CCB File Offset: 0x00361ECB
	// (set) Token: 0x0600905D RID: 36957 RVA: 0x00363CD3 File Offset: 0x00361ED3
	public bool GameIsFocused { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17001148 RID: 4424
	// (get) Token: 0x0600905E RID: 36958 RVA: 0x00363CDC File Offset: 0x00361EDC
	public bool IsMouseCursorVisible
	{
		get
		{
			return PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard && Cursor.visible;
		}
	}

	// Token: 0x140000F1 RID: 241
	// (add) Token: 0x0600905F RID: 36959 RVA: 0x00363CF8 File Offset: 0x00361EF8
	// (remove) Token: 0x06009060 RID: 36960 RVA: 0x00363D30 File Offset: 0x00361F30
	public event GameManager.OnWorldChangedEvent OnWorldChanged;

	// Token: 0x140000F2 RID: 242
	// (add) Token: 0x06009061 RID: 36961 RVA: 0x00363D68 File Offset: 0x00361F68
	// (remove) Token: 0x06009062 RID: 36962 RVA: 0x00363DA0 File Offset: 0x00361FA0
	public event GameManager.OnLocalPlayerChangedEvent OnLocalPlayerChanged;

	// Token: 0x140000F3 RID: 243
	// (add) Token: 0x06009063 RID: 36963 RVA: 0x00363DD8 File Offset: 0x00361FD8
	// (remove) Token: 0x06009064 RID: 36964 RVA: 0x00363E10 File Offset: 0x00362010
	public event Action<ClientInfo> OnClientSpawned;

	// Token: 0x06009065 RID: 36965 RVA: 0x00363E45 File Offset: 0x00362045
	public void ApplyAllOptions()
	{
		if (this.windowManager != null)
		{
			GameOptionsManager.ApplyAllOptions(this.windowManager.playerUI);
		}
	}

	// Token: 0x06009066 RID: 36966 RVA: 0x00363E68 File Offset: 0x00362068
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		if (!GameEntrypoint.EntrypointSuccess)
		{
			return;
		}
		MicroStopwatch microStopwatch = new MicroStopwatch(true);
		GameManager.Instance = this;
		this.GameIsFocused = (!GameManager.IsDedicatedServer && Application.isFocused);
		Log.Out("Awake IsFocused: " + Application.isFocused.ToString());
		Log.Out("Awake");
		ThreadManager.SetMonoBehaviour(this);
		Utils.InitStatic();
		LoadManager.Init();
		if (Application.isEditor)
		{
			Application.runInBackground = true;
			this.bCursorVisibleOverride = true;
			this.bCursorVisibleOverrideState = true;
		}
		this._analyticsService = ServiceProvider.Instance.Get<IAnalyticsService>();
		if (!GameManager.IsDedicatedServer)
		{
			GameOptionsManager.ResolutionChanged += this.OnResolutionChanged;
			this.RefreshRefreshRate();
			this.UpdateFPSCap();
			this._analyticsService.SessionRefreshed += this.OnAnalyticsSessionRefreshed;
		}
		Application.wantsToQuit += this.OnApplicationQuit;
		if (GameManager.IsDedicatedServer && GamePrefs.GetBool(EnumGamePrefs.TerminalWindowEnabled))
		{
			try
			{
				WinFormInstance server = new WinFormInstance();
				SingletonMonoBehaviour<SdtdConsole>.Instance.RegisterServer(server);
			}
			catch (Exception e)
			{
				Log.Error("Could not start Terminal Window:");
				Log.Exception(e);
			}
		}
		this.windowManager = UnityEngine.Object.FindAnyObjectByType<GUIWindowManager>();
		this.nguiWindowManager = UnityEngine.Object.FindAnyObjectByType<NGUIWindowManager>();
		TaskManager.Init();
		LocalPlayerManager.Init();
		GameStatsBridge.Init();
		if (!GameManager.IsDedicatedServer)
		{
			GameOptionsControls.Load();
		}
		MeshDataManager.Init();
		OcclusionManager.Load();
		if (GameManager.IsDedicatedServer)
		{
			GameOptionsManager.ApplyTextureQuality(3);
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = -1;
		}
		else
		{
			QualitySettings.vSyncCount = GamePrefs.GetInt(PlatformApplicationManager.Application.VSyncCountPref);
		}
		ServerDateTimeRequest.GetNtpTimeAsync(delegate(ServerDateTimeResult result)
		{
			GameManager.ServerClockSync = result;
		}, "pool.ntp.org", 5000);
		GameObjectPool.Instance.Init();
		MemoryPools.InitStatic(!GameManager.IsDedicatedServer);
		this.gameRandomManager = GameRandomManager.Instance;
		this.gameStateManager = new GameStateManager(this);
		this.prefabLODManager = new PrefabLODManager();
		new PrefabEditModeManager();
		UnityEngine.Object.Instantiate<GameObject>(DataLoader.LoadAsset<GameObject>("@:Sound_Mixers/AudioMixerManager.prefab", false));
		this.m_SoundsGameObject = GameObject.Find("Sounds");
		this.PhysicsInit();
		ParticleEffect.Init();
		SelectionBoxManager.Instance.SetupCategories();
		if (!GameManager.IsDedicatedServer)
		{
			if (GameOptionsReset.NeedsResetGame())
			{
				GameOptionsReset.ResetGame(RoamingPrefs.Store);
				GamePrefs.Instance.Save();
				Log.Out("Game Reset");
			}
			else
			{
				if (GameOptionsReset.Graphics.NeedsReset())
				{
					GameOptionsReset.Graphics.Reset(RoamingPrefs.Store);
					GamePrefs.Instance.Save();
					Log.Out("Graphics Reset");
				}
				if (GameOptionsReset.Controls.NeedsReset())
				{
					GameOptionsReset.Controls.Reset(RoamingPrefs.Store);
					GamePrefs.Instance.Save();
					Log.Out("Controls Reset");
				}
				if (GameOptionsReset.Bindings.NeedsReset())
				{
					GameOptionsReset.Bindings.Reset(RoamingPrefs.Store);
					GamePrefs.Instance.Save();
					Log.Out("Bindings Reset");
				}
			}
		}
		DeviceGamePrefs.Apply();
		GameOptionsManager.ValidateGamePrefs();
		if (!GameManager.IsDedicatedServer)
		{
			GameOptionsManager.ApplyAllOptions(this.windowManager.playerUI);
			UIUtils.LoadAtlas();
		}
		Manager.Init();
		UIOptions.Init();
		UIRoot uiroot = UnityEngine.Object.FindAnyObjectByType<UIRoot>();
		if (!GameManager.IsDedicatedServer)
		{
			this.InitMultiSourceUiAtlases(uiroot.gameObject);
		}
		this.windowManager.gameObject.AddComponent<LocalPlayerUI>();
		this.blockSelectionTool = new BlockToolSelection();
		this.nguiWindowManager.ParseWindows();
		float activeUiScale = GameOptionsManager.GetActiveUiScale();
		this.nguiWindowManager.SetBackgroundScale(activeUiScale);
		this.AddWindows(this.windowManager);
		PrefabVolumeManager.Instance.Init();
		ModManager.LoadMods();
		ThreadManager.RunCoroutineSync(ModManager.LoadPatchStuff(false));
		this.adminTools = new AdminTools();
		SingletonMonoBehaviour<SdtdConsole>.Instance.RegisterCommands();
		IEnumerator enumerator = this.loadStaticData();
		if (GameManager.IsDedicatedServer)
		{
			this.bStaticDataLoadSync = true;
			ThreadManager.RunCoroutineSync(enumerator);
		}
		else
		{
			this.bStaticDataLoadSync = false;
			ThreadManager.StartCoroutine(enumerator);
		}
		if (!GameManager.IsDedicatedServer)
		{
			CursorControllerAbs.LoadStaticData(LoadManager.CreateGroup());
		}
		else
		{
			InputManager.Enabled = false;
		}
		if (GameManager.IsDedicatedServer && GamePrefs.GetBool(EnumGamePrefs.TelnetEnabled))
		{
			try
			{
				TelnetConsole server2 = new TelnetConsole();
				SingletonMonoBehaviour<SdtdConsole>.Instance.RegisterServer(server2);
			}
			catch (Exception e2)
			{
				Log.Error("Could not start network console:");
				Log.Exception(e2);
			}
		}
		WebServer.Init();
		MapRendering.Init();
		AuthorizationManager.Instance.Init();
		ModEvents.SGameAwakeData sgameAwakeData;
		ModEvents.GameAwake.Invoke(ref sgameAwakeData);
		this.nguiWindowManager.Show(EnumNGUIWindow.InGameHUD, false);
		ConsoleCmdShow.Init();
		if (!GameManager.IsDedicatedServer)
		{
			GameSenseManager instance = GameSenseManager.Instance;
			if (instance != null)
			{
				instance.Init();
			}
			if (GamePrefs.GetBool(EnumGamePrefs.OptionsMumblePositionalAudioSupport))
			{
				MumblePositionalAudio.Init();
			}
		}
		DiscordManager instance2 = DiscordManager.Instance;
		if (this.BackgroundMusicClip || this.CreditsSongClip)
		{
			if (!GameManager.IsDedicatedServer)
			{
				base.gameObject.AddComponent<BackgroundMusicMono>();
			}
			else
			{
				Resources.UnloadAsset(this.BackgroundMusicClip);
				Resources.UnloadAsset(this.CreditsSongClip);
			}
		}
		PartyQuests.EnforeInstance();
		Input.simulateMouseWithTouches = false;
		IPlatform nativePlatform = PlatformManager.NativePlatform;
		IApplicationStateController applicationStateController = (nativePlatform != null) ? nativePlatform.ApplicationState : null;
		if (applicationStateController != null)
		{
			ApplicationState lastState = ApplicationState.Foreground;
			applicationStateController.OnApplicationStateChanged += delegate(ApplicationState state)
			{
				if (state != ApplicationState.Suspended && lastState == ApplicationState.Suspended)
				{
					this.OnApplicationResume();
				}
				lastState = state;
			};
			applicationStateController.OnNetworkStateChanged += this.OnNetworkStateChanged;
		}
		PlatformUserManager.DetailsUpdated += this.OnUserDetailsUpdated;
		if (!GameManager.IsDedicatedServer)
		{
			this.triggerEffectManager = new TriggerEffectManager();
			TriggerEffectManager.SetMainMenuLightbarColor();
		}
		Log.Out("Awake done in " + microStopwatch.ElapsedMilliseconds.ToString() + " ms");
	}

	// Token: 0x06009067 RID: 36967 RVA: 0x003643F8 File Offset: 0x003625F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitMultiSourceUiAtlases(GameObject _parent)
	{
		GameManager.<>c__DisplayClass126_0 CS$<>8__locals1 = new GameManager.<>c__DisplayClass126_0();
		CS$<>8__locals1.atlasesGo = new GameObject("UIAtlases");
		CS$<>8__locals1.atlasesGo.transform.parent = _parent.transform;
		Shader shader = Shader.Find("Unlit/Transparent Colored");
		Shader shader2 = Shader.Find("Unlit/Transparent Greyscale");
		MultiSourceAtlasManager multiSourceAtlasManager = MultiSourceAtlasManager.Create(CS$<>8__locals1.atlasesGo, "ItemIconAtlas");
		MultiSourceAtlasManager atlasManager = MultiSourceAtlasManager.Create(CS$<>8__locals1.atlasesGo, "ItemIconAtlasGreyscale");
		ModManager.ModAtlasesDefaults(CS$<>8__locals1.atlasesGo, shader);
		ModManager.RegisterAtlasManager(multiSourceAtlasManager, false, shader, new Action<INGUIAtlas, bool>(this.AddGreyscaleItemIconAtlas));
		ModManager.RegisterAtlasManager(atlasManager, false, shader2, null);
		Resources.Load<UIAtlas>("GUI/Prefabs/SymbolAtlas");
		Resources.Load<UIAtlas>("GUI/Prefabs/ControllerArtAtlas");
		GameManager.<>c__DisplayClass126_0 CS$<>8__locals2 = CS$<>8__locals1;
		INGUIAtlas[] atlases = Resources.FindObjectsOfTypeAll<UIAtlas>();
		CS$<>8__locals2.<InitMultiSourceUiAtlases>g__addLoadedAtlases|1(atlases);
		GameManager.<>c__DisplayClass126_0 CS$<>8__locals3 = CS$<>8__locals1;
		atlases = Resources.FindObjectsOfTypeAll<NGUIAtlas>();
		CS$<>8__locals3.<InitMultiSourceUiAtlases>g__addLoadedAtlases|1(atlases);
		CS$<>8__locals1.mipFilter = GameOptionsPlatforms.GetItemIconFilterString();
		LoadManager.AssetsRequestTask<GameObject> assetsRequestTask = LoadManager.LoadAssetsFromAddressables<GameObject>("iconatlas", (string address) => address.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase) && address.Contains(CS$<>8__locals1.mipFilter), null, false, true);
		List<GameObject> list = new List<GameObject>();
		assetsRequestTask.CollectResults(list);
		foreach (GameObject original in list)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original);
			gameObject.transform.parent = multiSourceAtlasManager.transform;
			UIAtlas component = gameObject.GetComponent<UIAtlas>();
			multiSourceAtlasManager.AddAtlas(component, gameObject, false);
			this.AddGreyscaleItemIconAtlas(component, false);
		}
	}

	// Token: 0x06009068 RID: 36968 RVA: 0x00364568 File Offset: 0x00362768
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddGreyscaleItemIconAtlas(INGUIAtlas _atlas, bool _isLoadingInGame)
	{
		MultiSourceAtlasManager atlasManager = ModManager.GetAtlasManager("ItemIconAtlasGreyscale");
		Shader shader = Shader.Find("Unlit/Transparent Greyscale");
		INGUIAtlas inguiatlas = _atlas.Clone();
		GameObject gameObject = null;
		UIAtlas uiatlas = inguiatlas as UIAtlas;
		if (uiatlas != null)
		{
			uiatlas.gameObject.transform.parent = atlasManager.transform;
			gameObject = uiatlas.gameObject;
		}
		inguiatlas.spriteMaterial = new Material(shader)
		{
			mainTexture = inguiatlas.texture
		};
		atlasManager.AddAtlas(inguiatlas, gameObject, _isLoadingInGame);
	}

	// Token: 0x06009069 RID: 36969 RVA: 0x003645DF File Offset: 0x003627DF
	public void AddWindows(GUIWindowManager _guiWindowManager)
	{
		if (_guiWindowManager == this.windowManager)
		{
			_guiWindowManager.Add(GUIWindowConsole.GetNewInstance());
			_guiWindowManager.Add(new GUIWindowScreenshotText());
		}
		_guiWindowManager.Add(new GUIWindowNGUI(EnumNGUIWindow.InGameHUD));
		_guiWindowManager.CloseAllOpenModalWindows(null, false);
	}

	// Token: 0x0600906A RID: 36970 RVA: 0x0036461A File Offset: 0x0036281A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator loadStaticData()
	{
		this.CurrentLoadAction = Localization.Get("loadActionCharacterModels", false, null);
		yield return null;
		this.CurrentLoadAction = Localization.Get("loadActionTerrainTextures", false, null);
		yield return null;
		yield return null;
		yield return WorldStaticData.Init(false, GameManager.IsDedicatedServer, delegate(string _progressText, float _percentage)
		{
			this.CurrentLoadAction = _progressText;
		}, null);
		this.CurrentLoadAction = Localization.Get("loadActionDone", false, null);
		this.bStaticDataLoaded = true;
		yield break;
	}

	// Token: 0x17001149 RID: 4425
	// (get) Token: 0x0600906B RID: 36971 RVA: 0x00364629 File Offset: 0x00362829
	// (set) Token: 0x0600906C RID: 36972 RVA: 0x00364631 File Offset: 0x00362831
	public bool IsStartingGame { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x0600906D RID: 36973 RVA: 0x0036463C File Offset: 0x0036283C
	public void StartGame(bool _offline)
	{
		Time.timeScale = 1f;
		GamePrefs.Set(EnumGamePrefs.GameGuidClient, "");
		PlatformManager.MultiPlatform.UserDataRoaming.ValidateRoamingMode();
		if (GameSparksManager.Instance() != null)
		{
			GameSparksManager.Instance().PrepareNewSession();
		}
		base.StartCoroutine(this.startGameCo(_offline));
	}

	// Token: 0x0600906E RID: 36974 RVA: 0x00364696 File Offset: 0x00362896
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator startGameCo(bool _offline)
	{
		this.IsStartingGame = true;
		Log.Out("StartGame");
		ModEvents.SGameStartingData sgameStartingData = new ModEvents.SGameStartingData(SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer);
		ModEvents.GameStarting.Invoke(ref sgameStartingData);
		this.allowQuit = false;
		this.backgroundColor = Color.white;
		EntityStats.WeatherSurvivalEnabled = true;
		yield return null;
		yield return ModManager.LoadPatchStuff(true);
		yield return null;
		SaveInfoProvider.Instance.ClearResources();
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield break;
		}
		if (!GameManager.IsDedicatedServer)
		{
			XUiC_MainMenu.CloseGlobalMenuWindows(this.windowManager.playerUI.xui);
			this.windowManager.CloseAllOpenModalWindows(null, false);
			XUiFromXml.ClearData();
			LocalPlayerUI.QueueUIForNewPlayerEntity(LocalPlayerUI.CreateUIForNewLocalPlayer());
			this.windowManager.Open(XUiC_LoadingScreen.ID, false);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.EACEnabled)
				{
					this.windowManager.Open("eacWarning", false);
				}
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.AllowsCrossplay)
				{
					this.windowManager.Open("crossplayWarning", false);
				}
			}
			XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, Localization.Get("uiLoadStartingGame", false, null), null, true, true, false);
		}
		yield return null;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			GamePrefs.Set(EnumGamePrefs.GameWorld, string.Empty);
		}
		this.isEditMode = GameModeEditWorld.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode));
		GamePrefs.Set(EnumGamePrefs.DebugStopEnemiesMoving, this.IsEditMode());
		GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, this.isEditMode || GameUtils.IsPlaytesting());
		GamePrefs.Set(EnumGamePrefs.CreativeMenuEnabled, this.isEditMode || GameUtils.IsPlaytesting());
		GamePrefs.Instance.Save();
		if (!Application.isEditor)
		{
			GameUtils.DebugOutputGamePrefs(delegate(string _text)
			{
				Log.WriteLine("GamePref." + _text);
			});
			GameUtils.DebugOutputGameStats(delegate(string _text)
			{
				Log.WriteLine("GameStat." + _text);
			});
			ConsoleCmdGetSandboxOptions.LogOptions(GamePrefs.GetString(EnumGamePrefs.SandboxCode), false, ConsoleCmdGetSandboxOptions.ELogType.LogOnly);
		}
		yield return null;
		CraftingManager.InitForNewGame();
		yield return null;
		GameManager.bSavingActive = true;
		GameManager.bPhysicsActive = !this.IsEditMode();
		GameManager.bTickingActive = !this.IsEditMode();
		GameManager.bShowDecorBlocks = true;
		GameManager.bShowLootBlocks = true;
		GameManager.bShowPaintables = true;
		GameManager.bShowUnpaintables = true;
		GameManager.bShowTerrain = true;
		GameManager.bVolumeBlocksEditing = true;
		Block.nameIdMapping = null;
		ItemClass.nameIdMapping = null;
		PlatformApplicationManager.SetRestartRequired();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			yield return this.StartAsServer(_offline);
		}
		else
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				yield break;
			}
			XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, Localization.Get("uiLoadWaitingForServer", false, null), null, true, true, false);
			this.StartAsClient();
		}
		DismembermentManager.Init();
		yield return null;
		if (GameSparksManager.Instance() != null)
		{
			GameSparksManager.Instance().SessionStarted(GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameMode), SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer);
		}
		if (!GameManager.IsDedicatedServer && SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode != ProtocolManager.NetworkType.OfflineServer)
		{
			PlatformManager.MultiPlatform.User.StartAdvertisePlaying(SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo);
		}
		Log.Out("Loading dymesh settings");
		DynamicMeshManager.CONTENT_ENABLED = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshEnabled);
		DynamicMeshSettings.OnlyPlayerAreas = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshLandClaimOnly);
		DynamicMeshSettings.UseImposterValues = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshUseImposters);
		DynamicMeshSettings.MaxViewDistance = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshDistance);
		DynamicMeshSettings.PlayerAreaChunkBuffer = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshLandClaimBuffer);
		DynamicMeshSettings.MaxRegionMeshData = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshMaxRegionCache);
		DynamicMeshSettings.MaxDyMeshData = GamePrefs.GetInt(EnumGamePrefs.DynamicMeshMaxItemCache);
		DynamicMeshSettings.LogSettings();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			DynamicMeshManager.Init();
		}
		ModEvents.SGameStartDoneData eventData;
		ModEvents.GameStartDone.Invoke(ref eventData);
		EnumResetUnprotectedChunksGroupingMode @int = (EnumResetUnprotectedChunksGroupingMode)GamePrefs.GetInt(EnumGamePrefs.ResetUnprotectedChunks);
		this.ResetUnprotectedChunksOnLoad(@int);
		if (GameManager.IsDedicatedServer)
		{
			Application.targetFrameRate = 20;
		}
		Log.Out("StartGame done");
		this.IsStartingGame = false;
		yield break;
	}

	// Token: 0x0600906F RID: 36975 RVA: 0x003646AC File Offset: 0x003628AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetUnprotectedChunksOnLoad(EnumResetUnprotectedChunksGroupingMode _resetChunksMode)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (_resetChunksMode == EnumResetUnprotectedChunksGroupingMode.Disabled)
		{
			return;
		}
		World world = this.m_World;
		object obj;
		if (world == null)
		{
			obj = null;
		}
		else
		{
			ChunkCluster chunkCache = world.ChunkCache;
			obj = ((chunkCache != null) ? chunkCache.ChunkProvider : null);
		}
		ChunkProviderGenerateWorld chunkProviderGenerateWorld = obj as ChunkProviderGenerateWorld;
		if (chunkProviderGenerateWorld != null)
		{
			chunkProviderGenerateWorld.MainThreadCacheProtectedPositions();
			chunkProviderGenerateWorld.ResetAllChunks(ChunkProtectionLevel.All, _resetChunksMode);
			Log.Out("[GameManager] Unprotected chunks have been reset.");
		}
	}

	// Token: 0x06009070 RID: 36976 RVA: 0x0036470C File Offset: 0x0036290C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateTimeOfDay()
	{
		if (!GameManager.IsDedicatedServer || this.m_World.Players.list.Count > 0)
		{
			int @int = GameStats.GetInt(EnumGameStats.TimeOfDayIncPerSec);
			if (@int == 0)
			{
				this.msPassedSinceLastUpdate += (int)(Time.deltaTime * 1000f);
				if (this.msPassedSinceLastUpdate >= 100)
				{
					this.m_World.SetTime(this.m_World.worldTime);
					this.msPassedSinceLastUpdate = 0;
				}
			}
			else
			{
				float num = 1000f / (float)@int;
				this.msPassedSinceLastUpdate += (int)(Time.deltaTime * 1000f);
				if ((float)this.msPassedSinceLastUpdate <= Utils.FastMax(num, 50f))
				{
					return;
				}
				int num2 = (int)((float)this.msPassedSinceLastUpdate / num);
				this.msPassedSinceLastUpdate -= (int)num * num2;
				ulong time = this.m_World.worldTime + (ulong)((long)num2);
				this.m_World.SetTime(time);
			}
		}
		ILobbyHost lobbyHost = PlatformManager.NativePlatform.LobbyHost;
		if (lobbyHost != null)
		{
			lobbyHost.UpdateGameTimePlayers(this.m_World.worldTime, this.m_World.Players.list.Count);
		}
		GameSenseManager instance = GameSenseManager.Instance;
		if (instance != null)
		{
			instance.UpdateEventTime(this.m_World.worldTime);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.UpdateGameTimePlayers(this.m_World.worldTime, this.m_World.Players.list.Count);
			if (Time.time - this.lastTimeWorldTickTimeSentToClients > global::Constants.cSendWorldTickTimeToClients)
			{
				this.lastTimeWorldTickTimeSentToClients = Time.time;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWorldTime>().Setup(this.m_World.worldTime), true, -1, -1, -1, null, 192, false);
				if (WeatherManager.Instance != null)
				{
					WeatherManager.Instance.SendPackages();
				}
			}
		}
	}

	// Token: 0x06009071 RID: 36977 RVA: 0x003648F0 File Offset: 0x00362AF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateSendClientPlayerPositionToServer()
	{
		EntityPlayerLocal primaryPlayer = this.m_World.GetPrimaryPlayer();
		EntityAlive entityAlive = primaryPlayer;
		if (entityAlive != null)
		{
			if (entityAlive.AttachedToEntity != null)
			{
				entityAlive = (entityAlive.AttachedToEntity as EntityAlive);
				this.bLastWasAttached = true;
				if (entityAlive.isEntityRemote)
				{
					return;
				}
			}
			else
			{
				if (this.bLastWasAttached)
				{
					this.lastTimeAbsPosSentToServer = int.MaxValue;
				}
				this.bLastWasAttached = false;
			}
		}
		if (entityAlive == null)
		{
			return;
		}
		if (primaryPlayer.bPlayerStatsChanged)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerStats>().Setup(primaryPlayer), false);
			primaryPlayer.bPlayerStatsChanged = false;
		}
		if (primaryPlayer.bPlayerTwitchChanged)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerTwitchStats>().Setup(primaryPlayer), false);
			primaryPlayer.bPlayerTwitchChanged = false;
		}
		if (primaryPlayer.bEntityAliveFlagsChanged)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAliveFlags>().Setup(primaryPlayer), false);
			primaryPlayer.bEntityAliveFlagsChanged = false;
		}
		if (primaryPlayer.bPlayerEquipmentChanged)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerEquipment>().Setup(primaryPlayer), false);
			primaryPlayer.bPlayerEquipmentChanged = false;
		}
		Vector3i vector3i = NetEntityDistributionEntry.EncodePos(entityAlive.position);
		Vector3i vector3i2 = vector3i - entityAlive.serverPos;
		bool flag = Utils.FastAbs((float)vector3i2.x) >= 2f || Utils.FastAbs((float)vector3i2.y) >= 2f || Utils.FastAbs((float)vector3i2.z) >= 2f || entityAlive.emodel.IsRagdollActive;
		Vector3i vector3i3 = NetEntityDistributionEntry.EncodeRot(entityAlive.rotation);
		Vector3i vector3i4 = vector3i3 - entityAlive.serverRot;
		bool flag2 = Utils.FastAbs((float)vector3i4.x) >= 1f || Utils.FastAbs((float)vector3i4.y) >= 1f || Utils.FastAbs((float)vector3i4.z) >= 1f || entityAlive.emodel.IsRagdollActive;
		if (flag || flag2)
		{
			if (vector3i2.x < -256 || vector3i2.x >= 256 || vector3i2.y < -256 || vector3i2.y >= 256 || vector3i2.z < -256 || vector3i2.z >= 256)
			{
				this.lastTimeAbsPosSentToServer = 0;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityTeleport>().Setup(entityAlive), false);
			}
			else if (vector3i2.x < -128 || vector3i2.x >= 128 || vector3i2.y < -128 || vector3i2.y >= 128 || vector3i2.z < -128 || vector3i2.z >= 128 || this.lastTimeAbsPosSentToServer > 100)
			{
				this.lastTimeAbsPosSentToServer = 0;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityPosAndRot>().Setup(entityAlive), false);
			}
			else
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityRelPosAndRot>().Setup(entityAlive.entityId, vector3i2, vector3i3, entityAlive.qrotation, entityAlive.onGround, entityAlive.IsQRotationUsed(), 3), false);
			}
			entityAlive.serverPos = vector3i;
			entityAlive.serverRot = vector3i3;
			this.lastTimeAbsPosSentToServer++;
		}
		if (entityAlive != primaryPlayer)
		{
			if (entityAlive.bPlayerStatsChanged)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerStats>().Setup(entityAlive), false);
				entityAlive.bPlayerStatsChanged = false;
			}
			if (entityAlive.bPlayerTwitchChanged)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerTwitchStats>().Setup(entityAlive), false);
				entityAlive.bPlayerTwitchChanged = false;
			}
			if (entityAlive.bEntityAliveFlagsChanged)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAliveFlags>().Setup(entityAlive), false);
				entityAlive.bEntityAliveFlagsChanged = false;
			}
			if (primaryPlayer.bPlayerEquipmentChanged)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerEquipment>().Setup(primaryPlayer), false);
				primaryPlayer.bPlayerEquipmentChanged = false;
			}
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(primaryPlayer);
		if (this.countdownSendPlayerDataFileToServer.HasPassed() && uiforPlayer.xui != null && uiforPlayer.xui.IsReady)
		{
			this.countdownSendPlayerDataFileToServer.ResetAndRestart();
			this.doSendLocalPlayerData(primaryPlayer);
		}
		if (this.countdownSendPlayerInventoryToServer.HasPassed())
		{
			this.countdownSendPlayerInventoryToServer.Reset();
			this.doSendLocalInventory(primaryPlayer);
		}
		if (primaryPlayer.persistentPlayerData != null && primaryPlayer.persistentPlayerData.questPositionsChanged)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerQuestPositions>().Setup(primaryPlayer.entityId, primaryPlayer.persistentPlayerData), false);
			primaryPlayer.persistentPlayerData.questPositionsChanged = false;
		}
	}

	// Token: 0x06009072 RID: 36978 RVA: 0x00364D37 File Offset: 0x00362F37
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdate()
	{
		GameManager.fixedUpdateCount++;
	}

	// Token: 0x06009073 RID: 36979 RVA: 0x00364D45 File Offset: 0x00362F45
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		this.gmUpdate();
	}

	// Token: 0x06009074 RID: 36980 RVA: 0x00364D50 File Offset: 0x00362F50
	[PublicizedFrom(EAccessModifier.Private)]
	public void gmUpdate()
	{
		GameManager.frameCount = Time.frameCount;
		GameManager.frameTime = Time.time;
		GameManager.fixedUpdateCount = 0;
		this.updatePauseState();
		GameOptionsManager.CheckResolution();
		ModEvents.SUnityUpdateData sunityUpdateData;
		ModEvents.UnityUpdate.Invoke(ref sunityUpdateData);
		this.handleGlobalActions();
		if (!GameManager.ReportUnusedAssets(false))
		{
			return;
		}
		if ((double)Time.timeScale <= 0.001)
		{
			Physics.SyncTransforms();
		}
		LoadManager.Update();
		PlatformManager.Update();
		InviteManager.Instance.Update();
		LockManager.Instance.Update();
		this.swUpdateTime.ResetAndRestart();
		this.fps.Update();
		BlockLiquidv2.UpdateTime();
		if (QuestEventManager.Current != null)
		{
			QuestEventManager.Current.Update();
		}
		if (this.m_World != null)
		{
			this.m_World.triggerManager.Update();
		}
		if (TwitchVoteScheduler.Current != null)
		{
			TwitchVoteScheduler.Current.Update(Time.deltaTime);
		}
		if (TwitchManager.Current != null)
		{
			TwitchManager.Current.Update(Time.unscaledDeltaTime);
		}
		if (GameEventManager.Current != null)
		{
			GameEventManager.Current.Update(Time.deltaTime);
		}
		if (PowerManager.HasInstance)
		{
			PowerManager.Instance.Update();
		}
		if (PartyManager.HasInstance)
		{
			PartyManager.Current.Update();
		}
		if (VehicleManager.Instance != null)
		{
			VehicleManager.Instance.Update();
		}
		if (DroneManager.Instance != null)
		{
			DroneManager.Instance.Update();
		}
		if (DismembermentManager.Instance != null)
		{
			DismembermentManager.Instance.Update();
		}
		if (TurretTracker.Instance != null)
		{
			TurretTracker.Instance.Update();
		}
		if (RaycastPathManager.Instance)
		{
			RaycastPathManager.Instance.Update();
		}
		if (TokenManager.Instance != null)
		{
			TokenManager.Instance.Update();
		}
		TrajectorySimulation.UpdateSimulationQueue();
		if (FactionManager.Instance != null)
		{
			FactionManager.Instance.Update();
		}
		if (NavObjectManager.HasInstance)
		{
			NavObjectManager.Instance.Update();
		}
		if (BlockedPlayerList.Instance != null)
		{
			BlockedPlayerList.Instance.Update();
		}
		PrefabEditModeManager instance = PrefabEditModeManager.Instance;
		if (instance != null)
		{
			instance.Update();
		}
		TriggerEffectManager triggerEffectManager = this.triggerEffectManager;
		if (triggerEffectManager != null)
		{
			triggerEffectManager.Update();
		}
		SpeedTreeWindHistoryBufferManager.Instance.Update();
		ThreadManager.UpdateMainThreadTasks();
		if (!GameManager.IsDedicatedServer)
		{
			if (XUiC_MainMenu.openedOnce && !this.isQuitting)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				bool? flag;
				string text;
				if (crossplatformPlatform == null)
				{
					flag = null;
				}
				else
				{
					IAntiCheatClient antiCheatClient = crossplatformPlatform.AntiCheatClient;
					flag = ((antiCheatClient != null) ? new bool?(antiCheatClient.GetUnhandledViolationMessage(out text)) : null);
				}
				bool? flag2 = flag;
				if (flag2.GetValueOrDefault())
				{
					GUIWindowManager guiwindowManager = LocalPlayerUI.primaryUI.windowManager;
					if (guiwindowManager != null)
					{
						string title = "EAC: " + Localization.Get("eacIntegrityViolation", false, null);
						if (!string.IsNullOrEmpty(text))
						{
							text += "\n";
						}
						else
						{
							text = "";
						}
						text += Localization.Get("eacUnableToPlayOnProtected", false, null);
						XUiC_MessageBoxWindowGroup.ShowOk(guiwindowManager.playerUI.xui, title, text, "", null, !this.gameStateManager.IsGameStarted(), true, false);
					}
				}
			}
			if (!this.bCursorVisibleOverride && !this.isQuitting)
			{
				bool flag3 = this.isAnyCursorWindowOpen(null);
				if (this.GameIsFocused && this.bCursorVisible != flag3)
				{
					this.setCursorEnabled(flag3);
				}
				if (!flag3 && Cursor.visible && PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
				{
					this.setCursorEnabled(false);
				}
			}
			this.UpdateFPSCap();
		}
		object syncRoot = ((ICollection)this.tileEntitiesMusicToRemove).SyncRoot;
		lock (syncRoot)
		{
			for (int i = 0; i < this.tileEntitiesMusicToRemove.Count; i++)
			{
				UnityEngine.Object.Destroy(this.tileEntitiesMusicToRemove[i]);
			}
		}
		if (!this.gameStateManager.IsGameStarted())
		{
			GameTimer.Instance.Reset(GameTimer.Instance.ticks);
			return;
		}
		this.m_World.entityAsyncManager.Update();
		GameTimer.Instance.updateTimer(GameManager.IsDedicatedServer && this.m_World.Players.Count == 0);
		this.updateBlockParticles();
		this.updateTimeOfDay();
		Manager.FrameUpdate();
		WaterSimulationNative.Instance.Update();
		SignTextureManager.Instance.MainThreadUpdate();
		WaterEvaporationManager.UpdateEvaporation();
		if (GameTimer.Instance.elapsedTicks > 0 || this.m_World.m_ChunkManager.IsForceUpdate() || this.m_World.Players.list.Count == 0)
		{
			this.m_World.m_ChunkManager.DetermineChunksToLoad();
		}
		if (GameManager.IsDedicatedServer && this.m_World.Players.list.Count == 0 && this.lastPlayerCount > 0)
		{
			this.timeToClearAllPools = 8f;
		}
		this.lastPlayerCount = this.m_World.Players.list.Count;
		if (this.m_World.Players.list.Count == 0 && this.timeToClearAllPools > 0f && (this.timeToClearAllPools -= Time.deltaTime) <= 0f)
		{
			Log.Out("Clearing all pools");
			MemoryPools.Cleanup();
			this.m_World.ClearCaches();
		}
		if (!this.UpdateTick())
		{
			return;
		}
		this.m_World.m_ChunkManager.GroundAlignFrameUpdate();
		int num = GameManager.IsDedicatedServer ? 25000 : 2500;
		this.swCopyChunks.ResetAndRestart();
		while (this.m_World.m_ChunkManager.CopyChunksToUnity() && this.swCopyChunks.ElapsedMicroseconds < (long)num)
		{
		}
		if (this.prefabLODManager != null)
		{
			this.prefabLODManager.FrameUpdate();
		}
		this.ExplodeGroupFrameUpdate();
		this.fpsCountdownTimer -= Time.deltaTime;
		if (this.fpsCountdownTimer <= 0f)
		{
			this.fpsCountdownTimer = 30f;
			GameManager.MaxMemoryConsumption = Math.Max(GC.GetTotalMemory(false), GameManager.MaxMemoryConsumption);
			if (!GameManager.IsDedicatedServer || SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() > 0 || this.lastStatsPlayerCount > 0)
			{
				this.lastStatsPlayerCount = SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount();
				Log.Out(ConsoleCmdMem.GetStats(false, this));
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.m_World.ChunkCache.ChunkProvider.Update();
			this.wsCountdownTimer -= Time.deltaTime;
			if (this.wsCountdownTimer <= 0f)
			{
				this.wsCountdownTimer = 30f;
				if (!this.isEditMode)
				{
					this.m_World.SaveWorldState();
					if (Block.nameIdMapping != null)
					{
						Block.nameIdMapping.SaveIfDirty(true);
					}
					if (ItemClass.nameIdMapping != null)
					{
						ItemClass.nameIdMapping.SaveIfDirty(true);
					}
					EventPrefabs eventPrefabs = this.m_World.ChunkCache.ChunkProvider.GetEventPrefabs();
					if (eventPrefabs != null)
					{
						eventPrefabs.Save(false);
					}
				}
			}
			this.playerPositionsCountdownTimer -= Time.deltaTime;
			if (this.playerPositionsCountdownTimer <= 0f)
			{
				this.playerPositionsCountdownTimer = 6f;
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() > 0)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerPositions>().Setup(this.persistentPlayers), false, -1, -1, -1, null, 192, true);
				}
			}
		}
		if (GameManager.IsDedicatedServer)
		{
			this.gcCountdownTimer -= Time.deltaTime;
			if (this.gcCountdownTimer <= 0f)
			{
				this.gcCountdownTimer = 120f;
				GC.Collect();
			}
		}
		else
		{
			GameSenseManager instance2 = GameSenseManager.Instance;
			if (instance2 != null)
			{
				instance2.Update();
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.countdownSaveLocalPlayerDataFile.HasPassed())
			{
				this.countdownSaveLocalPlayerDataFile.ResetAndRestart();
				this.SaveLocalPlayerData();
			}
			this.unloadAssetsDuration += Time.deltaTime;
			if (this.unloadAssetsDuration > 1200f)
			{
				bool flag5 = this.unloadAssetsDuration > 3600f;
				if (!this.isAnyModalWindowOpen())
				{
					this.isUnloadAssetsReady = true;
				}
				else if (this.isUnloadAssetsReady)
				{
					flag5 = true;
				}
				if (flag5)
				{
					this.stopwatchUnloadAssets.ResetAndRestart();
					Resources.UnloadUnusedAssets();
					this.stopwatchUnloadAssets.Stop();
					Log.Out("UnloadUnusedAssets after {0} m, took {1} ms", new object[]
					{
						this.unloadAssetsDuration / 60f,
						this.stopwatchUnloadAssets.ElapsedMilliseconds
					});
					this.unloadAssetsDuration = 0f;
					this.isUnloadAssetsReady = false;
				}
			}
		}
		if (this.stabilityViewer != null)
		{
			this.stabilityViewer.Update();
		}
		ModEvents.SGameUpdateData sgameUpdateData;
		ModEvents.GameUpdate.Invoke(ref sgameUpdateData);
		GameObjectPool.Instance.FrameUpdate();
	}

	// Token: 0x06009075 RID: 36981 RVA: 0x003655B8 File Offset: 0x003637B8
	public void LateUpdate()
	{
		ThreadManager.LateUpdate();
		PlatformManager.LateUpdate();
		if (this.m_World != null && this.m_World.aiDirector != null)
		{
			this.m_World.aiDirector.DebugFrameLateUpdate();
		}
		this.UpdateMultiplayerServices();
		MeshDataManager.Instance.LateUpdate();
	}

	// Token: 0x06009076 RID: 36982 RVA: 0x00365604 File Offset: 0x00363804
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UpdateTick()
	{
		GameTimer instance = GameTimer.Instance;
		if (instance.elapsedTicks <= 0 && this.m_World.Players.list.Count != 0)
		{
			this.m_World.TickEntitiesSlice();
			return true;
		}
		this.m_World.TickEntitiesFlush();
		float partialTicks = (Time.time - this.lastTime) * 20f;
		this.lastTime = Time.time;
		this.m_World.OnUpdateTick(partialTicks, this.m_World.m_ChunkManager.GetActiveChunkSet());
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !this.gameStateManager.OnUpdateTick())
		{
			return false;
		}
		this.m_World.TickEntities(partialTicks);
		this.m_World.LetBlocksFall();
		if (!GameManager.IsDedicatedServer)
		{
			this.m_World.SetEntitiesVisibleNearToLocalPlayer();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.m_World.entityDistributer.OnUpdateEntities();
			this.m_World.m_ChunkManager.SendChunksToClients();
			if (GameManager.bSavingActive)
			{
				ChunkCluster chunkCache = this.m_World.ChunkCache;
				ChunkProviderGenerateWorld chunkProviderGenerateWorld = ((chunkCache != null) ? chunkCache.ChunkProvider : null) as ChunkProviderGenerateWorld;
				if (chunkProviderGenerateWorld != null)
				{
					chunkProviderGenerateWorld.MainThreadCacheProtectedPositions();
				}
				if (instance.ticks % 40UL == 0UL)
				{
					if (chunkCache != null)
					{
						chunkCache.ChunkProvider.SaveRandomChunks(2, instance.ticks, this.m_World.m_ChunkManager.GetActiveChunkSet());
					}
				}
				else if (Time.time - this.lastTimeDecoSaved > 60f)
				{
					this.lastTimeDecoSaved = Time.time;
					this.m_World.SaveDecorations();
					if (chunkCache != null)
					{
						EventPrefabs eventPrefabs = chunkCache.ChunkProvider.GetEventPrefabs();
						if (eventPrefabs != null)
						{
							eventPrefabs.Save(false);
						}
					}
				}
			}
		}
		else
		{
			this.updateSendClientPlayerPositionToServer();
		}
		if (this.lastTime - this.activityCheck >= 1f)
		{
			PlatformManager.MultiPlatform.RichPresence.UpdateRichPresence(IRichPresence.PresenceStates.InGame);
			this.activityCheck = this.lastTime;
		}
		return true;
	}

	// Token: 0x06009077 RID: 36983 RVA: 0x003657DC File Offset: 0x003639DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnNetworkStateChanged(bool connectionState)
	{
		if (!connectionState)
		{
			this.ShutdownMultiplayerServices(GameManager.EMultiShutReason.AppNoNetwork);
		}
	}

	// Token: 0x06009078 RID: 36984 RVA: 0x003657E8 File Offset: 0x003639E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnApplicationResume()
	{
		this._analyticsService.CheckForAndRefreshStaleSession();
		PlatformApplicationManager.SetRestartRequired();
		if (!this.IsSafeToConnect())
		{
			this.ShutdownMultiplayerServices(GameManager.EMultiShutReason.AppSuspended);
			return;
		}
		ThreadManager.StartCoroutine(PlatformApplicationManager.CheckRestartCoroutine(false));
	}

	// Token: 0x06009079 RID: 36985 RVA: 0x00365818 File Offset: 0x00363A18
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateMultiplayerServices()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (this.shuttingDownMultiplayerServices)
		{
			return;
		}
		if (this.IsSafeToConnect() || !this.IsSafeToDisconnect())
		{
			return;
		}
		ConnectionManager instance = SingletonMonoBehaviour<ConnectionManager>.Instance;
		if (!(instance == null))
		{
			ProtocolManager.NetworkType currentMode = instance.CurrentMode;
			if (currentMode != ProtocolManager.NetworkType.None && currentMode != ProtocolManager.NetworkType.OfflineServer)
			{
				EUserPerms permissions = PermissionsManager.GetPermissions(PermissionsManager.PermissionSources.All);
				if (!permissions.HasMultiplayer())
				{
					this.ShutdownMultiplayerServices(GameManager.EMultiShutReason.PermMissingMultiplayer);
					return;
				}
				if ((SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient ? SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo).AllowsCrossplay && !permissions.HasCrossplay())
				{
					this.ShutdownMultiplayerServices(GameManager.EMultiShutReason.PermMissingCrossplay);
					return;
				}
				return;
			}
		}
	}

	// Token: 0x0600907A RID: 36986 RVA: 0x003658B8 File Offset: 0x00363AB8
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetLocalizationKey(GameManager.EMultiShutReason _reason)
	{
		string result;
		switch (_reason)
		{
		case GameManager.EMultiShutReason.AppNoNetwork:
			result = "app_noNetwork";
			break;
		case GameManager.EMultiShutReason.AppSuspended:
			result = "app_suspended";
			break;
		case GameManager.EMultiShutReason.PermMissingMultiplayer:
			result = "permMissing_multiplayer";
			break;
		case GameManager.EMultiShutReason.PermMissingCrossplay:
			result = "permMissing_crossplay";
			break;
		default:
			throw new ArgumentOutOfRangeException("_reason", _reason, string.Format("Unknown Localization for {0}.{1}", "EMultiShutReason", _reason));
		}
		return result;
	}

	// Token: 0x0600907B RID: 36987 RVA: 0x00365924 File Offset: 0x00363B24
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShutdownMultiplayerServices(GameManager.EMultiShutReason _reason)
	{
		ThreadManager.StartCoroutine(this.ShutdownMultiplayerServicesCoroutine(_reason));
	}

	// Token: 0x0600907C RID: 36988 RVA: 0x00365933 File Offset: 0x00363B33
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ShutdownMultiplayerServicesCoroutine(GameManager.EMultiShutReason _reason)
	{
		yield return null;
		if (GameManager.IsDedicatedServer)
		{
			yield break;
		}
		if (this.shuttingDownMultiplayerServices)
		{
			yield break;
		}
		this.shuttingDownMultiplayerServices = true;
		bool isClient = false;
		bool success = false;
		bool failReasonProvided = false;
		try
		{
			Log.Out(string.Format("Waiting to Shut Down Multiplayer Services ({0})...", _reason));
			while (this.shuttingDownMultiplayerServices && !this.IsSafeToConnect() && !this.IsSafeToDisconnect())
			{
				yield return null;
			}
			ConnectionManager connectionManager = SingletonMonoBehaviour<ConnectionManager>.Instance;
			for (;;)
			{
				yield return null;
				if (!this.shuttingDownMultiplayerServices)
				{
					break;
				}
				if (this.IsSafeToConnect())
				{
					goto Block_9;
				}
				if (connectionManager == null)
				{
					goto IL_18D;
				}
				ProtocolManager.NetworkType currentMode = connectionManager.CurrentMode;
				if (currentMode == ProtocolManager.NetworkType.None || currentMode == ProtocolManager.NetworkType.OfflineServer)
				{
					goto IL_18D;
				}
				if (this.IsSafeToDisconnect())
				{
					goto Block_12;
				}
			}
			Log.Warning(string.Format("Cancelled Shutting Down Multiplayer Services ({0}) because already shutting down.", _reason));
			failReasonProvided = true;
			yield break;
			Block_9:
			Log.Warning(string.Format("Cancelled Shutting Down Multiplayer Services ({0}) because safe to connect.", _reason));
			failReasonProvided = true;
			yield break;
			IL_18D:
			Log.Warning(string.Format("Cancelled Shutting Down Multiplayer Services ({0}) because no online connection.", _reason));
			failReasonProvided = true;
			yield break;
			Block_12:
			Log.Out(string.Format("Shutting Down Multiplayer Services ({0})...", _reason));
			if (connectionManager.IsClient)
			{
				this.Disconnect();
				isClient = true;
				success = true;
				yield break;
			}
			ClientInfo[] clientInfos = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List.ToArray<ClientInfo>();
			if (clientInfos.Length != 0)
			{
				NetPackagePlayerDenied package = NetPackageManager.GetPackage<NetPackagePlayerDenied>().Setup(new GameUtils.KickPlayerData(GameUtils.EKickReason.SessionClosed, 0, default(DateTime), ""));
				ClientInfo[] array = clientInfos;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SendPackage(package);
				}
				yield return new WaitForSecondsRealtime(1f);
				foreach (ClientInfo clientInfo in clientInfos)
				{
					try
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.DisconnectClient(clientInfo, false, false);
					}
					catch (Exception arg)
					{
						Log.Warning(string.Format("Failed to disconnect client '{0}' : {1}", clientInfo.playerName, arg));
					}
				}
			}
			this.ShutdownMultiplayerServicesNow();
			connectionManager.MakeServerOffline();
			GamePrefs.Set(EnumGamePrefs.ServerMaxPlayerCount, 1);
			success = true;
			connectionManager = null;
			clientInfos = null;
		}
		finally
		{
			this.shuttingDownMultiplayerServices = false;
			if (success)
			{
				Log.Out(string.Format("Multiplayer Services ({0}) have been shut down.", _reason));
				string title = Localization.Get(isClient ? "multiShut_titleClient" : "multiShut_titleHost", false, null);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat(Localization.Get("auth_reason", false, null), Localization.Get(GameManager.GetLocalizationKey(_reason), false, null));
				if (!isClient)
				{
					stringBuilder.Append('\n');
					stringBuilder.Append(Localization.Get("multiShut_commonHost", false, null));
				}
				XUiC_MessageBoxWindowGroup.ShowOk(this.windowManager.playerUI.xui, title, stringBuilder.ToString(), "", new Action(this.OnShutdownMultiplayerServicesMessageBoxClosed), true, true, false);
			}
			else if (!failReasonProvided)
			{
				Log.Warning(string.Format("Failed Shutting Down Multiplayer Services ({0}).", _reason));
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x0600907D RID: 36989 RVA: 0x00365949 File Offset: 0x00363B49
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnShutdownMultiplayerServicesMessageBoxClosed()
	{
		if (this.World != null)
		{
			this.Pause(false);
		}
	}

	// Token: 0x0600907E RID: 36990 RVA: 0x0036595A File Offset: 0x00363B5A
	public void CreateStabilityViewer()
	{
		if (this.stabilityViewer == null)
		{
			this.stabilityViewer = new StabilityViewer();
		}
	}

	// Token: 0x0600907F RID: 36991 RVA: 0x0036596F File Offset: 0x00363B6F
	public void ClearStabilityViewer()
	{
		if (this.stabilityViewer != null)
		{
			this.stabilityViewer.worldIsReady = false;
			this.stabilityViewer.Clear();
			this.stabilityViewer = null;
		}
	}

	// Token: 0x06009080 RID: 36992 RVA: 0x00365998 File Offset: 0x00363B98
	[PublicizedFrom(EAccessModifier.Private)]
	public void setLocalPlayerEntity(EntityPlayerLocal _playerEntity)
	{
		_playerEntity.IsFlyMode.Value = this.IsEditMode();
		_playerEntity.SetEntityName(GamePrefs.GetString(EnumGamePrefs.PlayerName));
		this.myPlayerId = _playerEntity.entityId;
		this.myEntityPlayerLocal = _playerEntity;
		this.persistentLocalPlayer = this.getPersistentPlayerData(null);
		_playerEntity.persistentPlayerData = this.persistentLocalPlayer;
		_playerEntity.InventoryChangedEvent += this.LocalPlayerInventoryChanged;
		_playerEntity.inventory.OnToolbeltItemsChangedInternal += delegate()
		{
			this.sendPlayerToolbelt = true;
		};
		_playerEntity.bag.OnBackpackItemsChangedInternal += delegate()
		{
			this.sendPlayerBag = true;
		};
		_playerEntity.equipment.OnChanged += delegate()
		{
			this.sendPlayerEquipment = true;
		};
		_playerEntity.DragAndDropItemChanged += delegate()
		{
			this.sendDragAndDropItem = true;
		};
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.persistentPlayers != null)
		{
			if (this.persistentLocalPlayer == null)
			{
				this.persistentLocalPlayer = this.persistentPlayers.CreatePlayerData(this.getPersistentPlayerID(null), PlatformManager.NativePlatform.User.PlatformUserId, _playerEntity.EntityName, DeviceFlag.StandaloneWindows.ToPlayGroup());
				this.persistentLocalPlayer.EntityId = this.myPlayerId;
				this.persistentPlayers.MapPlayer(this.persistentLocalPlayer);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(this.persistentLocalPlayer, EnumPersistentPlayerDataReason.New), true, -1, -1, -1, null, 192, false);
				this.persistentPlayers.SavePersistentPlayerData();
			}
			else
			{
				this.persistentLocalPlayer.Update(PlatformManager.NativePlatform.User.PlatformUserId, new AuthoredText(_playerEntity.EntityName, this.persistentLocalPlayer.PrimaryId), DeviceFlag.StandaloneWindows.ToPlayGroup());
				this.persistentLocalPlayer.EntityId = this.myPlayerId;
				this.persistentPlayers.MapPlayer(this.persistentLocalPlayer);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(this.persistentLocalPlayer, EnumPersistentPlayerDataReason.Login), true, -1, -1, -1, null, 192, false);
			}
		}
		this.m_World.SetLocalPlayer(_playerEntity);
		LocalPlayerUI.DispatchNewPlayerForUI(_playerEntity);
		if (this.OnLocalPlayerChanged != null)
		{
			this.OnLocalPlayerChanged(_playerEntity);
		}
		GameSenseManager instance = GameSenseManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.SessionStarted(_playerEntity);
	}

	// Token: 0x06009081 RID: 36993 RVA: 0x00365BCD File Offset: 0x00363DCD
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator StartAsServer(bool _offline)
	{
		while (XUiC_WorldGenerationWindow.WorldBuilderIsGenerating)
		{
			yield return null;
		}
		Log.Out("StartAsServer");
		if (!SandboxOptionManager.HasInstance)
		{
			Log.Warning("Sandbox Option Manager not initialized before starting server, this may cause issues with some settings such as crossplay");
		}
		SandboxOptionManager.Current.LoadOptionsFromCode(GamePrefs.GetString(EnumGamePrefs.SandboxCode));
		this.gameStateManager.InitGame(true);
		SandboxOptionManager.Current.UpdateInGameValuesWithSandboxOptions(true);
		GameServerInfo.PrepareLocalServerInfo();
		this.CalculatePersistentPlayerCount(GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameName), (UserDataStorageType)GamePrefs.GetInt(EnumGamePrefs.GameSaveStorageType));
		PlatformManager.MultiPlatform.RichPresence.UpdateRichPresence(IRichPresence.PresenceStates.Loading);
		XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, Localization.Get("uiLoadLoadingXml", false, null), null, true, true, false);
		yield return null;
		WorldStaticData.Cleanup(null);
		Block.nameIdMapping = null;
		ItemClass.nameIdMapping = null;
		string @string = GamePrefs.GetString(EnumGamePrefs.GameWorld);
		if (!@string.Equals("Empty") && !@string.Equals("Playtesting"))
		{
			string path = GameIO.GetSaveGameDir() + "/main.ttw";
			string text = GameIO.GetSaveGameDir() + "/" + global::Constants.cFileBlockMappings;
			string text2 = GameIO.GetSaveGameDir() + "/" + global::Constants.cFileItemMappings;
			if (!SdFile.Exists(path))
			{
				if (!SdDirectory.Exists(GameIO.GetSaveGameDir()))
				{
					SdDirectory.CreateDirectory(GameIO.GetSaveGameDir());
				}
				Block.nameIdMapping = new NameIdMapping(text, Block.MAX_BLOCKS);
				Block.nameIdMapping.WriteToFile();
				ItemClass.nameIdMapping = new NameIdMapping(text2, ItemClass.MAX_ITEMS);
				ItemClass.nameIdMapping.WriteToFile();
			}
			else
			{
				Block.nameIdMapping = new NameIdMapping(text, Block.MAX_BLOCKS);
				if (!Block.nameIdMapping.LoadFromFile())
				{
					Log.Warning("Could not load block-name-mappings file '" + text + "'!");
					Block.nameIdMapping = null;
				}
				ItemClass.nameIdMapping = new NameIdMapping(text2, ItemClass.MAX_ITEMS);
				if (!ItemClass.nameIdMapping.LoadFromFile())
				{
					Log.Warning("Could not load item-name-mappings file '" + text2 + "'!");
					ItemClass.nameIdMapping = null;
				}
			}
		}
		yield return WorldStaticData.LoadAllXmlsCo(false, null, null);
		yield return null;
		SingletonMonoBehaviour<ConnectionManager>.Instance.ServerReady();
		Manager.CreateServer();
		LightManager.CreateServer();
		yield return null;
		PowerManager.Instance.LoadPowerManager();
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadCreatingWorld", false, null), true);
		yield return null;
		if (this.isEditMode)
		{
			this.persistentPlayers = new PersistentPlayerList();
		}
		else
		{
			this.persistentPlayers = PersistentPlayerList.ReadXML(GameIO.GetSaveGameDir() + "/players.xml");
			if (this.persistentPlayers != null && this.persistentPlayers.CleanupPlayers())
			{
				this.persistentPlayers.SavePersistentPlayerData();
			}
		}
		string levelName;
		PathAbstractions.AbstractedLocation worldLocation;
		PathAbstractions.Contextual.FindActiveWorld(out levelName, out worldLocation);
		yield return this.createWorld(levelName, worldLocation, GamePrefs.GetString(EnumGamePrefs.GameName), false);
		GameServerInfo.SetLocalServerWorldInfo();
		NetPackageWorldInfo.PrepareWorldHashes();
		yield return null;
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadCreatingPlayer", false, null), true);
		yield return null;
		if (!GameManager.IsDedicatedServer)
		{
			GameManager.<>c__DisplayClass166_1 CS$<>8__locals2 = new GameManager.<>c__DisplayClass166_1();
			CS$<>8__locals2.persistentPlayerId = this.getPersistentPlayerID(null).CombinedString;
			if (!GamePrefs.GetBool(EnumGamePrefs.SkipSpawnButton) && !this.IsEditMode())
			{
				this.canSpawnPlayer = false;
				bool firstTimeSpawn = !PlayerDataFile.Exists(GameIO.GetPlayerDataDir(), CS$<>8__locals2.persistentPlayerId);
				XUiC_SpawnSelectionWindow.Open(LocalPlayerUI.primaryUI, false, true, firstTimeSpawn);
				while (!this.canSpawnPlayer)
				{
					yield return null;
				}
				yield return new WaitForSeconds(0.1f);
			}
			PlayerDataFile playerDataFile = new PlayerDataFile();
			playerDataFile.Load(GameIO.GetPlayerDataDir(), CS$<>8__locals2.persistentPlayerId);
			EntityCreationData entityCreationData = new EntityCreationData();
			Vector3 pos;
			Vector3 rot;
			int num2;
			if (playerDataFile.bLoaded)
			{
				pos = playerDataFile.ecd.pos;
				rot = new Vector3(playerDataFile.ecd.rot.x, playerDataFile.ecd.rot.y, 0f);
				if (this.isEditMode)
				{
					playerDataFile.id = -1;
				}
				int num;
				if (playerDataFile.id == -1)
				{
					EntityFactory.nextEntityID = (num = EntityFactory.nextEntityID) + 1;
				}
				else
				{
					num = playerDataFile.id;
				}
				num2 = num;
				entityCreationData.entityData = playerDataFile.ecd.entityData;
				entityCreationData.readFileVersion = playerDataFile.ecd.readFileVersion;
			}
			else
			{
				SpawnPosition randomSpawnPosition = this.GetSpawnPointList().GetRandomSpawnPosition(this.m_World, null, 0, 0);
				if (this.m_World.IsRandomWorld())
				{
					DynamicPrefabDecorator dynamicPrefabDecorator = this.GetDynamicPrefabDecorator();
					if (dynamicPrefabDecorator != null)
					{
						PrefabInstance closestPOIToWorldPos = dynamicPrefabDecorator.GetClosestPOIToWorldPos(QuestEventManager.traderTag, Vector3.zero, null, -1, false, BiomeFilterTypes.OnlyBiome, BiomeDefinition.BiomeNames[3], "traderquest");
						if (closestPOIToWorldPos != null)
						{
							randomSpawnPosition = this.GetSpawnPointList().GetRandomSpawnPosition(this.m_World, new Vector3?(closestPOIToWorldPos.GetAABB().center), 250, 750);
							Vector3.Distance(randomSpawnPosition.position, closestPOIToWorldPos.GetAABB().center);
						}
					}
				}
				pos = randomSpawnPosition.position;
				rot = new Vector3(0f, randomSpawnPosition.heading, 0f);
				num2 = EntityFactory.nextEntityID++;
			}
			if (playerDataFile.bLoaded && playerDataFile.ecd.playerProfile != null && GamePrefs.GetBool(EnumGamePrefs.PersistentPlayerProfiles))
			{
				entityCreationData.entityClass = EntityClass.FromString(playerDataFile.ecd.playerProfile.EntityClassName);
				entityCreationData.playerProfile = playerDataFile.ecd.playerProfile;
			}
			else
			{
				entityCreationData.playerProfile = PlayerProfile.LoadLocalProfile();
				entityCreationData.entityClass = EntityClass.FromString(entityCreationData.playerProfile.EntityClassName);
			}
			entityCreationData.skinTexture = GamePrefs.GetString(EnumGamePrefs.OptionsPlayerModelTexture);
			entityCreationData.id = num2;
			entityCreationData.pos = pos;
			entityCreationData.rot = rot;
			entityCreationData.belongsPlayerId = num2;
			EntityPlayerLocal entityPlayerLocal = (EntityPlayerLocal)EntityFactory.CreateEntity(entityCreationData);
			this.setLocalPlayerEntity(entityPlayerLocal);
			if (playerDataFile.bLoaded)
			{
				playerDataFile.ToPlayer(entityPlayerLocal);
				entityPlayerLocal.bPreferFirstPerson = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxDefaultFirstPersonCamera);
				entityPlayerLocal.SetFirstPersonView(true, false);
			}
			this.m_World.SpawnEntityInWorld(entityPlayerLocal);
			this.myEntityPlayerLocal.Respawn(playerDataFile.bLoaded ? RespawnType.LoadedGame : RespawnType.NewGame);
			this.myEntityPlayerLocal.ChunkObserver = this.m_World.m_ChunkManager.AddChunkObserver(this.myEntityPlayerLocal.GetPosition(), true, Utils.FastMin(12, GameUtils.GetViewDistance()), -1);
			IMapChunkDatabase.TryCreateOrLoad(this.myEntityPlayerLocal.entityId, out this.myEntityPlayerLocal.ChunkObserver.mapDatabase, () => new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataDir(), CS$<>8__locals2.persistentPlayerId));
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
			uiforPlayer.xui.SetDataConnections();
			uiforPlayer.xui.SetCraftingData(playerDataFile.craftingData);
			CS$<>8__locals2 = null;
		}
		Log.Out("Loaded player");
		yield return null;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			VehicleManager.Init();
			DroneManager.Init();
			TurretTracker.Init();
			RaycastPathManager.Init();
			TokenManager.Init();
			BlockLimitTracker.Init();
			ChunkProviderGenerateWorld chunkProviderGenerateWorld = this.m_World.ChunkCache.ChunkProvider as ChunkProviderGenerateWorld;
			if (chunkProviderGenerateWorld != null)
			{
				chunkProviderGenerateWorld.CheckPersistentData();
			}
		}
		yield return null;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.m_World.ChunkCache.IsFixedSize && !this.IsEditMode() && this.m_World.m_WorldEnvironment != null)
		{
			this.m_World.m_WorldEnvironment.SetColliders((float)((this.m_World.ChunkCache.ChunkMinPos.x + 1) * 16), (float)((this.m_World.ChunkCache.ChunkMinPos.y + 1) * 16), (float)((this.m_World.ChunkCache.ChunkMaxPos.x - this.m_World.ChunkCache.ChunkMinPos.x - 1) * 16), (float)((this.m_World.ChunkCache.ChunkMaxPos.y - this.m_World.ChunkCache.ChunkMinPos.y - 1) * 16), global::Constants.cSizePlanesAround, 0f);
			this.m_World.m_WorldEnvironment.CreateLevelBorderBox(this.m_World);
		}
		if (this.isEditMode)
		{
			PrefabEditModeManager instance = PrefabEditModeManager.Instance;
			if (instance != null)
			{
				instance.Init();
			}
			yield return null;
		}
		yield return null;
		if (GameManager.IsDedicatedServer || !_offline)
		{
			ServerInformationTcpProvider.Instance.StartServer();
			IMasterServerAnnouncer serverListAnnouncer = PlatformManager.MultiPlatform.ServerListAnnouncer;
			if (serverListAnnouncer != null)
			{
				serverListAnnouncer.AdvertiseServer(delegate
				{
					ILobbyHost lobbyHost = PlatformManager.NativePlatform.LobbyHost;
					if (lobbyHost != null)
					{
						lobbyHost.UpdateLobby(SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo);
					}
					ModEvents.SServerRegisteredData sserverRegisteredData;
					ModEvents.ServerRegistered.Invoke(ref sserverRegisteredData);
					this.LogServerStartEventAnalytics(_offline);
				});
			}
			else
			{
				this.LogServerStartEventAnalytics(_offline);
			}
			PlayerInteractions.Instance.JoinedMultiplayerServer(this.persistentPlayers);
			AuthorizationManager.Instance.ServerStart();
		}
		else
		{
			GamePrefs.Set(EnumGamePrefs.ServerMaxPlayerCount, 1);
			this.LogServerStartEventAnalytics(_offline);
		}
		yield return GCUtils.UnloadAndCollectCo();
		this.gameStateManager.StartGame();
		yield break;
	}

	// Token: 0x06009082 RID: 36994 RVA: 0x00365BE3 File Offset: 0x00363DE3
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator LogPlayerJoinServerEventAnalyticsCoroutine()
	{
		if (this.IsEditMode() || GameUtils.IsPlaytesting())
		{
			yield break;
		}
		string joinTs = DateTime.UtcNow.ToString("O");
		yield return new WaitForSeconds(2.5f);
		PlayerJoinServerEventData playerJoinServerEventData = new PlayerJoinServerEventData();
		playerJoinServerEventData.ServerId = Helper.GetServerId();
		string saveId;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			World world = this.World;
			saveId = ((world != null) ? world.Guid : null);
		}
		else
		{
			saveId = GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
		}
		playerJoinServerEventData.SaveId = saveId;
		playerJoinServerEventData.ServerJoinTimestamp = joinTs;
		playerJoinServerEventData.OnlinePlayers = Helper.GetServerPlayerCount();
		playerJoinServerEventData.ServerJoinSource = SingletonMonoBehaviour<ConnectionManager>.Instance.LastJoinSource;
		playerJoinServerEventData.LocalMods = ((ModManager.GetLoadedMods().Count > 0) ? Helper.GetTruncatedLoadedMods(100) : null);
		playerJoinServerEventData.HasModifiedXML = new bool?(!StockFileHashes.HasStockXMLs());
		World world2 = this.World;
		playerJoinServerEventData.ElapsedWorldSaveTime = ((world2 != null) ? new ulong?(world2.worldTime) : null);
		World world3 = this.World;
		playerJoinServerEventData.InGameDays = ((world3 != null) ? new int?(world3.WorldDay) : null);
		playerJoinServerEventData.TotalMods = new int?(ModManager.GetLoadedMods().Count);
		PlayerJoinServerEventData playerJoinServerEventData2 = playerJoinServerEventData;
		if (this.myEntityPlayerLocal)
		{
			PlayerDataFile playerDataFile = new PlayerDataFile();
			playerDataFile.FromPlayer(this.myEntityPlayerLocal);
			Dictionary<string, float?> dictionary = new Dictionary<string, float?>();
			dictionary.Add("Days Alive", new float?(Mathf.Clamp((float)((this.myEntityPlayerLocal.world.worldTime - this.myEntityPlayerLocal.gameStageBornAtWorldTime) / 24000UL), 0f, (float)this.myEntityPlayerLocal.Progression.Level)));
			dictionary.Add("Difficulty Bonus", new float?(GameStageDefinition.DifficultyBonus));
			string key = "Quest Modifier";
			Quest activeQuest = this.myEntityPlayerLocal.QuestJournal.ActiveQuest;
			dictionary.Add(key, (activeQuest != null) ? new float?(activeQuest.QuestClass.GameStageMod) : null);
			string key2 = "Quest Bonus";
			Quest activeQuest2 = this.myEntityPlayerLocal.QuestJournal.ActiveQuest;
			dictionary.Add(key2, (activeQuest2 != null) ? new float?(activeQuest2.QuestClass.GameStageBonus) : null);
			dictionary.Add("Biome GameStage Modifier", new float?(EntityPlayer.BiomeGameStageModifier));
			string key3 = "Biome Modifier";
			BiomeDefinition biomeStandingOn = this.myEntityPlayerLocal.biomeStandingOn;
			dictionary.Add(key3, ((biomeStandingOn != null) ? new float?(biomeStandingOn.GameStageMod) : null) * EntityPlayer.BiomeGameStageModifier);
			string key4 = "Biome Bonus";
			BiomeDefinition biomeStandingOn2 = this.myEntityPlayerLocal.biomeStandingOn;
			dictionary.Add(key4, ((biomeStandingOn2 != null) ? new float?(biomeStandingOn2.GameStageBonus) : null) * EntityPlayer.BiomeGameStageModifier);
			Dictionary<string, float?> gameStagesJson = dictionary;
			playerJoinServerEventData2.ElapsedPlayerTime = new uint?((uint)(playerDataFile.totalTimePlayed * 60f));
			playerJoinServerEventData2.PersonalGameStageUnmodified = new float?((float)this.myEntityPlayerLocal.unModifiedGameStage);
			playerJoinServerEventData2.PersonalGameStageModified = new float?((float)this.myEntityPlayerLocal.gameStage);
			playerJoinServerEventData2.CharacterLevel = new int?(this.myEntityPlayerLocal.Progression.Level);
			playerJoinServerEventData2.TotalDeaths = new int?(playerDataFile.deaths);
			playerJoinServerEventData2.GameStagesJson = gameStagesJson;
		}
		this._analyticsService.LogEvent(playerJoinServerEventData2);
		SingletonMonoBehaviour<ConnectionManager>.Instance.LastJoinSource = "Unknown";
		yield break;
	}

	// Token: 0x06009083 RID: 36995 RVA: 0x00365BF4 File Offset: 0x00363DF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogServerStartEventAnalytics(bool isOffline)
	{
		if (this.IsEditMode() || GameUtils.IsPlaytesting())
		{
			return;
		}
		SandboxOptionManager sandboxOptionManager = SandboxOptionManager.Current;
		SandboxOptionPreset preset = sandboxOptionManager.GetPreset(GamePrefs.GetString(EnumGamePrefs.SandboxPreset));
		string value = string.IsNullOrEmpty((preset != null) ? preset.LocalizedName : null) ? ((preset != null) ? preset.Name : null) : Localization.Get((preset != null) ? preset.LocalizedName : null, false, "english");
		SandboxOptionPreset preset2 = new SandboxOptionPreset();
		sandboxOptionManager.LoadOptionsFromCode(GamePrefs.GetString(EnumGamePrefs.SandboxCode), preset2);
		Dictionary<string, object> generalSettings = new Dictionary<string, object>
		{
			{
				"Group",
				(preset != null) ? preset.Group : null
			},
			{
				"Preset",
				value
			},
			{
				"Server_Enabled",
				GamePrefs.GetBool(EnumGamePrefs.ServerEnabled)
			},
			{
				"Region",
				GamePrefs.GetString(EnumGamePrefs.Region)
			},
			{
				"Server_Visibility",
				((EnumServerVisibility)GamePrefs.GetInt(EnumGamePrefs.ServerVisibility)).ToString()
			},
			{
				"EAC_Protected",
				GamePrefs.GetBool(EnumGamePrefs.ServerEACPeerToPeer)
			},
			{
				"Crossplay_Enabled",
				GamePrefs.GetBool(EnumGamePrefs.ServerAllowCrossplay)
			},
			{
				"Max_Players",
				GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount)
			},
			{
				"Chunk_Reset_Timer",
				GamePrefs.GetInt(EnumGamePrefs.MaxChunkAge)
			},
			{
				"Reset_Unprotected_Chunks",
				((EnumResetUnprotectedChunksGroupingMode)GamePrefs.GetInt(EnumGamePrefs.ResetUnprotectedChunks)).ToString()
			},
			{
				"Creative_Mode",
				GamePrefs.GetBool(EnumGamePrefs.BuildCreate)
			},
			{
				"Persistent_Profiles",
				GamePrefs.GetBool(EnumGamePrefs.PersistentPlayerProfiles)
			},
			{
				"Restrict_Camera_Mode",
				((EnumCameraRestriction)GamePrefs.GetInt(EnumGamePrefs.CameraRestrictionMode)).ToString()
			},
			{
				"Player_Killing",
				((EnumPlayerKillingMode)GamePrefs.GetInt(EnumGamePrefs.PlayerKillingMode)).ToString()
			},
			{
				"Claim_Size",
				GamePrefs.GetInt(EnumGamePrefs.LandClaimSize)
			},
			{
				"Claim_Deadzone",
				GamePrefs.GetInt(EnumGamePrefs.LandClaimDeadZone)
			},
			{
				"Claim_Duration",
				GamePrefs.GetInt(EnumGamePrefs.LandClaimExpiryTime)
			},
			{
				"Claim_Decay_Mode",
				GamePrefs.GetInt(EnumGamePrefs.LandClaimDecayMode)
			},
			{
				"Claim_Health_Online",
				GamePrefs.GetInt(EnumGamePrefs.LandClaimOnlineDurabilityModifier)
			},
			{
				"Claim_Health_Offline",
				GamePrefs.GetInt(EnumGamePrefs.LandClaimOfflineDurabilityModifier)
			},
			{
				"Bedroll_Deadzone",
				GamePrefs.GetInt(EnumGamePrefs.BedrollDeadZoneSize)
			},
			{
				"Bedroll_Duration",
				GamePrefs.GetInt(EnumGamePrefs.BedrollExpiryTime)
			},
			{
				"Party_Shared_Kill_Range",
				GamePrefs.GetInt(EnumGamePrefs.PartySharedKillRange)
			}
		};
		ServerStartEventData analyticsEventData = new ServerStartEventData
		{
			ServerId = Helper.GetServerId(),
			SaveId = this.World.Guid,
			ServerStartTimestamp = DateTime.UtcNow.ToString("o"),
			ServerType = (GameManager.IsDedicatedServer ? "Dedicated" : (isOffline ? "Offline" : "Listen")),
			SandboxSeed = GameStats.GetString(EnumGameStats.SandboxCode),
			SandboxSettingsDelta = Helper.GetSandboxSettingsDelta(preset2),
			WorldName = GamePrefs.GetString(EnumGamePrefs.GameWorld),
			GeneralSettings = generalSettings,
			ServerMods = ((ModManager.GetLoadedMods().Count > 0) ? Helper.GetTruncatedLoadedMods(100) : null),
			TotalMods = ModManager.GetLoadedMods().Count
		};
		this._analyticsService.LogEvent(analyticsEventData);
		if (!GameManager.IsDedicatedServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.LastJoinSource = "Server Start";
			base.StartCoroutine(this.LogPlayerJoinServerEventAnalyticsCoroutine());
		}
	}

	// Token: 0x06009084 RID: 36996 RVA: 0x00365FA8 File Offset: 0x003641A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartAsClient()
	{
		Log.Out("StartAsClient");
		this.worldCreated = false;
		this.chunkClusterLoaded = false;
		this.worldInitInfoReceived = false;
		GamePrefs.Set(EnumGamePrefs.GameMode, string.Empty);
		GamePrefs.Set(EnumGamePrefs.GameWorld, string.Empty);
		WorldStaticData.WaitForConfigsFromServer();
		PlatformManager.MultiPlatform.RichPresence.UpdateRichPresence(IRichPresence.PresenceStates.Connecting);
		IAntiCheatClient antiCheatClient = PlatformManager.MultiPlatform.AntiCheatClient;
		if (antiCheatClient == null || !antiCheatClient.ClientAntiCheatEnabled())
		{
			Log.Out("Sending RequestToEnterGame...");
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageRequestToEnterGame>(), false);
		}
		else
		{
			PlatformManager.MultiPlatform.AntiCheatClient.WaitForRemoteAuth(delegate
			{
				Log.Out("Sending RequestToEnterGame...");
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageRequestToEnterGame>(), false);
			});
		}
		BlockLimitTracker.Init();
	}

	// Token: 0x06009085 RID: 36997 RVA: 0x0036606B File Offset: 0x0036426B
	public bool IsSafeToConnect()
	{
		return SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode == ProtocolManager.NetworkType.None;
	}

	// Token: 0x06009086 RID: 36998 RVA: 0x0036607C File Offset: 0x0036427C
	public bool IsSafeToDisconnect()
	{
		return SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode == ProtocolManager.NetworkType.None || ((!PrefabEditModeManager.Instance.IsActive() || !PrefabEditModeManager.Instance.NeedsSaving) && (this.gameStateManager.IsGameStarted() && !this.IsStartingGame) && !this.isDisconnectingLater);
	}

	// Token: 0x06009087 RID: 36999 RVA: 0x003660D4 File Offset: 0x003642D4
	public void Disconnect()
	{
		Log.Out("Disconnect");
		if (!GameManager.IsDedicatedServer)
		{
			this.windowManager.CloseAllOpenModalWindows(null, false);
			if (this.m_World != null)
			{
				List<EntityPlayerLocal> localPlayers = this.m_World.GetLocalPlayers();
				for (int i = 0; i < localPlayers.Count; i++)
				{
					LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(localPlayers[i]);
					if (null != uiforPlayer && null != uiforPlayer.windowManager)
					{
						uiforPlayer.windowManager.CloseAllOpenModalWindows(null, false);
						uiforPlayer.xui.gameObject.SetActive(false);
					}
				}
			}
			XUiC_SubtitlesDisplay.Close(LocalPlayerUI.primaryUI.xui);
			Manager.StopAllLocal();
		}
		this.Pause(false);
		if (!GameManager.IsDedicatedServer)
		{
			if (!this.isEditMode && this.myEntityPlayerLocal)
			{
				GameSenseManager instance = GameSenseManager.Instance;
				if (instance != null)
				{
					instance.SessionEnded();
				}
				if (this.myEntityPlayerLocal.AttachedToEntity)
				{
					this.myEntityPlayerLocal.Detach();
				}
				this.myEntityPlayerLocal.FireEvent(MinEventTypes.onSelfLeaveGame, true);
				this.myEntityPlayerLocal.dropItemOnQuit();
			}
			this.triggerEffectManager.StopGamepadVibration(true);
			PlatformManager.MultiPlatform.User.StopAdvertisePlaying();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerDisconnect>().Setup(this.myEntityPlayerLocal), true);
			base.StartCoroutine(this.disconnectLater());
		}
		else if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.StopServers();
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.DisconnectFromServer();
		}
		if (GameSparksManager.Instance() != null)
		{
			GameSparksManager.Instance().SessionEnded();
		}
	}

	// Token: 0x06009088 RID: 37000 RVA: 0x00366272 File Offset: 0x00364472
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator disconnectLater()
	{
		this.isDisconnectingLater = true;
		yield return new WaitForSeconds(0.2f);
		SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
		GamePrefs.Set(EnumGamePrefs.GameGuidClient, "");
		this.isDisconnectingLater = false;
		yield break;
	}

	// Token: 0x06009089 RID: 37001 RVA: 0x00366284 File Offset: 0x00364484
	public void SaveAndCleanupWorld()
	{
		Log.Out("SaveAndCleanupWorld");
		World world = this.m_World;
		if (world != null)
		{
			EntityAsyncManager entityAsyncManager = world.entityAsyncManager;
			if (entityAsyncManager != null)
			{
				entityAsyncManager.CompletePendingCreateTasks();
			}
		}
		ModEvents.SWorldShuttingDownData sworldShuttingDownData;
		ModEvents.WorldShuttingDown.Invoke(ref sworldShuttingDownData);
		this.shuttingDownMultiplayerServices = false;
		PathAbstractions.CacheEnabled = false;
		this.OnClientSpawned = null;
		PlayerInputRecordingSystem.Instance.AutoSave();
		this.gameStateManager.EndGame();
		PlatformManager.MultiPlatform.RichPresence.UpdateRichPresence(IRichPresence.PresenceStates.Menu);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GameManager.bSavingActive && !this.IsEditMode())
		{
			if (VehicleManager.Instance != null)
			{
				VehicleManager.Instance.RemoveAllVehiclesFromMap();
			}
			if (DroneManager.Instance != null)
			{
				DroneManager.Instance.RemoveAllDronesFromMap();
			}
			if (QuestEventManager.HasInstance)
			{
				QuestEventManager.Current.HandleAllPlayersDisconnect();
			}
			this.SaveLocalPlayerData();
			this.SaveWorld();
			World world2 = this.m_World;
			EntityPlayerLocal entityPlayerLocal = (world2 != null) ? world2.GetPrimaryPlayer() : null;
			if (this.persistentPlayers != null)
			{
				foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in this.persistentPlayers.Players)
				{
					if (keyValuePair.Value.EntityId != -1)
					{
						if (entityPlayerLocal && keyValuePair.Value.EntityId == entityPlayerLocal.entityId)
						{
							keyValuePair.Value.Position = new Vector3i(entityPlayerLocal.position);
						}
						keyValuePair.Value.LastLogin = DateTime.Now;
					}
				}
				this.persistentPlayers.SavePersistentPlayerData();
			}
		}
		if (Block.nameIdMapping != null)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				Block.nameIdMapping.SaveIfDirty(true);
			}
			Block.nameIdMapping = null;
		}
		if (ItemClass.nameIdMapping != null)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				ItemClass.nameIdMapping.SaveIfDirty(true);
			}
			ItemClass.nameIdMapping = null;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GameManager.bSavingActive && !this.IsEditMode())
		{
			if (this.m_World != null && this.m_World.GetPrimaryPlayer() != null && this.m_World.GetPrimaryPlayer().ChunkObserver.mapDatabase != null)
			{
				ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(this.m_World.GetPrimaryPlayer().ChunkObserver.mapDatabase.SaveAsync), new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataLocalDir(), this.persistentLocalPlayer.PrimaryId.CombinedString), null, true);
			}
			if (!GameManager.IsDedicatedServer && this.m_World != null)
			{
				foreach (EntityPlayerLocal entityPlayerLocal2 in this.m_World.GetLocalPlayers())
				{
					entityPlayerLocal2.EnableCamera(false);
					entityPlayerLocal2.SetControllable(false);
				}
			}
		}
		this.ShutdownMultiplayerServicesNow();
		PlayerInteractions.Instance.OnNewPlayerInteraction -= this.HandleFirstSpawnInteractions;
		PlayerInteractions.Instance.Shutdown();
		IGameplayNotifier gameplayNotifier = PlatformManager.NativePlatform.GameplayNotifier;
		if (gameplayNotifier != null)
		{
			gameplayNotifier.GameplayEnd();
		}
		if (!GameManager.IsDedicatedServer)
		{
			if (this.myEntityPlayerLocal != null)
			{
				this.myEntityPlayerLocal.EnableCamera(false);
				this.myEntityPlayerLocal.SetControllable(false);
				if (this.OnLocalPlayerChanged != null)
				{
					this.OnLocalPlayerChanged(null);
				}
				this.m_World.RemoveEntity(this.myPlayerId, EnumRemoveEntityReason.Unloaded);
				this.myPlayerId = -1;
				this.myEntityPlayerLocal = null;
			}
			foreach (LocalPlayerUI localPlayerUI in LocalPlayerUI.PlayerUIs)
			{
				if (!localPlayerUI.isPrimaryUI && !localPlayerUI.IsCleanCopy)
				{
					if (localPlayerUI.entityPlayer)
					{
						localPlayerUI.entityPlayer.EnableCamera(false);
						localPlayerUI.entityPlayer.SetControllable(false);
						World world3 = this.m_World;
						if (world3 != null)
						{
							world3.RemoveEntity(localPlayerUI.entityPlayer.entityId, EnumRemoveEntityReason.Unloaded);
						}
					}
					if (localPlayerUI.gameObject)
					{
						localPlayerUI.xui.Shutdown(false);
						localPlayerUI.windowManager.CloseAllOpenModalWindows(null, false);
						UnityEngine.Object.Destroy(localPlayerUI.gameObject);
					}
				}
			}
		}
		ModManager.GameEnded();
		if (!GameManager.IsDedicatedServer)
		{
			if (!PlatformApplicationManager.IsRestartRequired)
			{
				GameManager.LoadRemoteResources(null);
			}
			GUIWindowConsole.Close();
			this.windowManager.Close(XUiC_LoadingScreen.ID);
			if (!GameManager.bHideMainMenuNextTime)
			{
				this.windowManager.Open(XUiC_MainMenu.ID, true);
			}
			GameManager.bHideMainMenuNextTime = false;
		}
		PrefabInstanceClientManager.Instance.Cleanup();
		TrajectorySimulation.Cleanup();
		TokenManager.Cleanup();
		AstarManager.Cleanup();
		DynamicMeshManager.OnWorldUnload();
		if (GameEventManager.HasInstance)
		{
			GameEventManager.Current.Cleanup();
		}
		if (this.m_World != null)
		{
			if (this.OnWorldChanged != null)
			{
				this.OnWorldChanged(null);
			}
			this.prefabLODManager.Cleanup();
			PrefabEditModeManager instance = PrefabEditModeManager.Instance;
			if (instance != null && instance.IsActive())
			{
				PrefabEditModeManager instance2 = PrefabEditModeManager.Instance;
				if (instance2 != null)
				{
					instance2.Cleanup();
				}
			}
			EnvironmentAudioManager.DestroyInstance();
			LightManager.Clear();
			SkyManager.Cleanup();
			WeatherManager.Cleanup();
			CharacterGazeController.Cleanup();
			WaterSplashCubes.Clear();
			WaterEvaporationManager.ClearAll();
			SleeperVolumeToolManager.CleanUp();
			POIMarkerToolManager.CleanUp();
			this.ClearStabilityViewer();
			if (this.m_World.GetPrimaryPlayer() && this.m_World.GetPrimaryPlayer().DynamicMusicManager != null)
			{
				this.m_World.GetPrimaryPlayer().DynamicMusicManager.CleanUpDynamicMembers();
			}
			this.m_World.UnloadWorld(true);
			this.m_World.Cleanup();
			this.m_World = null;
			this.GameHasStarted = false;
		}
		WaterSimulationNative.Instance.Cleanup();
		ProjectileManager.Cleanup();
		VehicleManager.Cleanup();
		DroneManager.Cleanup();
		DismembermentManager.Cleanup();
		TurretTracker.Cleanup();
		BlockLimitTracker.Cleanup();
		MapObjectManager.Reset();
		vp_TargetEventHandler.UnregisterAll();
		this.lootManager = null;
		this.traderManager = null;
		if (QuestEventManager.HasInstance)
		{
			QuestEventManager.Current.Cleanup();
		}
		if (TwitchVoteScheduler.HasInstance)
		{
			TwitchVoteScheduler.Current.Cleanup();
		}
		if (TwitchManager.HasInstance)
		{
			TwitchManager.Current.Cleanup();
		}
		if (PowerManager.HasInstance)
		{
			PowerManager.Instance.Cleanup();
		}
		if (WireManager.HasInstance)
		{
			WireManager.Instance.Cleanup();
		}
		if (PartyManager.HasInstance)
		{
			PartyManager.Current.Cleanup();
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GameManager.bSavingActive)
		{
			this.IsEditMode();
		}
		if (UIDisplayInfoManager.HasInstance)
		{
			UIDisplayInfoManager.Current.Cleanup();
		}
		if (TextureLoadingManager.Instance != null)
		{
			TextureLoadingManager.Instance.Cleanup();
		}
		if (NavObjectManager.HasInstance)
		{
			NavObjectManager.Instance.Cleanup();
		}
		SignTextureExporter.Instance.Cleanup();
		SignTextureManager.Instance.Cleanup(false);
		if (SignDataManager.HasInstance)
		{
			SignDataManager.Instance.Cleanup();
		}
		SelectionBoxManager.Instance.Clear();
		Origin.Cleanup();
		GameObjectPool.Instance.Cleanup();
		MemoryPools.Cleanup();
		VoxelMeshLayer.StaticCleanup();
		GamePrefs.Instance.Save();
		GameManager.bRecordNextSession = false;
		GameManager.bPlayRecordedSession = false;
	}

	// Token: 0x0600908A RID: 37002 RVA: 0x0036697C File Offset: 0x00364B7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShutdownMultiplayerServicesNow()
	{
		if (!GameManager.IsDedicatedServer)
		{
			PlatformManager.MultiPlatform.User.StopAdvertisePlaying();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			AuthorizationManager.Instance.ServerStop();
		}
		ILobbyHost lobbyHost = PlatformManager.NativePlatform.LobbyHost;
		if (lobbyHost != null)
		{
			lobbyHost.ExitLobby();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			PlatformManager.MultiPlatform.ServerListAnnouncer.StopServer();
			ServerInformationTcpProvider.Instance.StopServer();
		}
		IGameplayNotifier gameplayNotifier = PlatformManager.NativePlatform.GameplayNotifier;
		if (gameplayNotifier == null)
		{
			return;
		}
		gameplayNotifier.EndOnlineMultiplayer();
	}

	// Token: 0x0600908B RID: 37003 RVA: 0x00366A03 File Offset: 0x00364C03
	public void SaveWorld()
	{
		if (this.m_World != null)
		{
			this.m_World.Save();
		}
	}

	// Token: 0x0600908C RID: 37004 RVA: 0x00366A18 File Offset: 0x00364C18
	public void SaveLocalPlayerData()
	{
		if (this.m_World == null)
		{
			return;
		}
		EntityPlayerLocal primaryPlayer = this.m_World.GetPrimaryPlayer();
		if (primaryPlayer == null || !GameManager.bSavingActive)
		{
			return;
		}
		string combinedString = this.getPersistentPlayerID(null).CombinedString;
		PlayerDataFile playerDataFile = new PlayerDataFile();
		playerDataFile.FromPlayer(primaryPlayer);
		playerDataFile.Save(GameIO.GetPlayerDataDir(), combinedString);
		if (primaryPlayer.ChunkObserver.mapDatabase != null)
		{
			ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(primaryPlayer.ChunkObserver.mapDatabase.SaveAsync), new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataDir(), combinedString), null, true);
		}
	}

	// Token: 0x0600908D RID: 37005 RVA: 0x00366AA8 File Offset: 0x00364CA8
	public void Cleanup()
	{
		Log.Out("Cleanup");
		WaterSimulationNative.Instance.Cleanup();
		ModEvents.SGameShutdownData sgameShutdownData;
		ModEvents.GameShutdown.Invoke(ref sgameShutdownData);
		AuthorizationManager.Instance.Cleanup();
		VehicleManager.Cleanup();
		Cursor.visible = true;
		Cursor.lockState = SoftCursor.DefaultCursorLockState;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Cleanup();
		WorldStaticData.Cleanup();
		this.adminTools = null;
		GUIWindowConsole.Shutdown();
		GameObjectPool.Instance.Cleanup();
		SaveDataUtils.SaveDataManager.Cleanup();
		LocalPlayerManager.Destroy();
		PlatformManager.Destroy();
		LoadManager.Destroy();
		TaskManager.Destroy();
		MemoryPools.Cleanup();
		GC.Collect();
	}

	// Token: 0x1700114A RID: 4426
	// (get) Token: 0x0600908E RID: 37006 RVA: 0x00366B41 File Offset: 0x00364D41
	public bool IsQuitting
	{
		get
		{
			return this.isQuitting;
		}
	}

	// Token: 0x0600908F RID: 37007 RVA: 0x00366B4C File Offset: 0x00364D4C
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool OnApplicationQuit()
	{
		AdminTools adminTools = this.adminTools;
		if (adminTools != null)
		{
			adminTools.DestroyFileWatcher();
		}
		if (!this.allowQuit)
		{
			if (!this.isQuitting)
			{
				this.isQuitting = true;
				base.StartCoroutine(this.ApplicationQuitCo(0.3f));
			}
			return false;
		}
		GameSenseManager instance = GameSenseManager.Instance;
		if (instance != null)
		{
			instance.Cleanup();
		}
		ThreadManager.Shutdown();
		WorldStaticData.QuitCleanup();
		if (SingletonMonoBehaviour<SdtdConsole>.Instance != null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Cleanup();
		}
		if (!GameManager.IsDedicatedServer)
		{
			this._analyticsService.SessionRefreshed -= this.OnAnalyticsSessionRefreshed;
		}
		Log.Out("OnApplicationQuit");
		return true;
	}

	// Token: 0x06009090 RID: 37008 RVA: 0x00366BF0 File Offset: 0x00364DF0
	public void OnApplicationFocus(bool _focus)
	{
		if (!GameManager.IsDedicatedServer)
		{
			this.GameIsFocused = _focus;
			if (Application.isEditor)
			{
				return;
			}
			if (!_focus)
			{
				this.setCursorEnabled(true);
			}
			else if (this.bCursorVisibleOverride)
			{
				this.setCursorEnabled(this.bCursorVisibleOverrideState);
			}
			else if (!this.isAnyCursorWindowOpen(null))
			{
				this.setCursorEnabled(false);
			}
			if (ActionSetManager.DebugLevel != ActionSetManager.EDebugLevel.Off)
			{
				Log.Out("Focus: " + _focus.ToString());
				Log.Out("Input state:");
				foreach (PlayerActionsBase playerActionsBase in PlatformManager.NativePlatform.Input.ActionSets)
				{
					Log.Out(string.Format("   {0}: {1}", playerActionsBase.GetType().Name, playerActionsBase.Enabled));
				}
				Log.Out("Modal window open: " + LocalPlayerUI.PlayerUIs.Any((LocalPlayerUI ui) => ui.windowManager.IsModalWindowOpen()).ToString());
				Log.Out("Cursor window: " + this.isAnyCursorWindowOpen(null).ToString());
			}
			ModEvents.SGameFocusData sgameFocusData = new ModEvents.SGameFocusData(_focus);
			ModEvents.GameFocus.Invoke(ref sgameFocusData);
		}
	}

	// Token: 0x06009091 RID: 37009 RVA: 0x00366D4C File Offset: 0x00364F4C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isAnyModalWindowOpen()
	{
		IList<LocalPlayerUI> playerUIs = LocalPlayerUI.PlayerUIs;
		for (int i = playerUIs.Count - 1; i >= 0; i--)
		{
			if (playerUIs[i].windowManager.IsModalWindowOpen())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06009092 RID: 37010 RVA: 0x00366D88 File Offset: 0x00364F88
	public bool isAnyCursorWindowOpen(LocalPlayerUI _ui = null)
	{
		if (_ui == null)
		{
			IList<LocalPlayerUI> playerUIs = LocalPlayerUI.PlayerUIs;
			for (int i = 0; i < playerUIs.Count; i++)
			{
				if (playerUIs[i].windowManager.IsModalWindowOpen() || playerUIs[i].windowManager.IsCursorWindowOpen())
				{
					return true;
				}
			}
		}
		else if (_ui.windowManager.IsModalWindowOpen() || _ui.windowManager.IsCursorWindowOpen())
		{
			return true;
		}
		return false;
	}

	// Token: 0x06009093 RID: 37011 RVA: 0x00366DFC File Offset: 0x00364FFC
	public void SetCursorEnabledOverride(bool _bOverrideOn, bool _bOverrideState)
	{
		if (this.bCursorVisibleOverride != _bOverrideOn)
		{
			this.bCursorVisibleOverride = _bOverrideOn;
			this.setCursorEnabled(_bOverrideState);
		}
	}

	// Token: 0x06009094 RID: 37012 RVA: 0x00366E15 File Offset: 0x00365015
	public bool GetCursorEnabledOverride()
	{
		return this.bCursorVisibleOverride;
	}

	// Token: 0x06009095 RID: 37013 RVA: 0x00366E1D File Offset: 0x0036501D
	[PublicizedFrom(EAccessModifier.Private)]
	public void setCursorEnabled(bool _e)
	{
		if (this.IsQuitting)
		{
			return;
		}
		this.bCursorVisible = _e;
		if (ActionSetManager.DebugLevel == ActionSetManager.EDebugLevel.Verbose)
		{
			Log.Out("CursorEnabled: " + _e.ToString());
		}
		SoftCursor.SetCursorVisible(this.bCursorVisible);
	}

	// Token: 0x06009096 RID: 37014 RVA: 0x00366E58 File Offset: 0x00365058
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ApplicationQuitCo(float _delay)
	{
		Log.Out("Preparing quit");
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentMode != ProtocolManager.NetworkType.None)
		{
			try
			{
				this.Disconnect();
			}
			catch (Exception e)
			{
				Log.Error("Disconnecting failed:");
				Log.Exception(e);
			}
			yield return new WaitForSeconds(_delay);
		}
		if (!GameManager.IsDedicatedServer)
		{
			this.windowManager.CloseAllOpenModalWindows(null, false);
			this.windowManager.playerUI.xui.StopAllVideo();
		}
		GamePrefs.Instance.Save();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.StopServers();
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
		}
		this.Cleanup();
		yield return new WaitForSeconds(0.05f);
		this.allowQuit = true;
		Application.Quit();
		yield break;
	}

	// Token: 0x06009097 RID: 37015 RVA: 0x00366E70 File Offset: 0x00365070
	public void ShowMessagePlayerDenied(GameUtils.KickPlayerData _kickData)
	{
		Log.Out("[NET] Kicked from server: " + _kickData.ToString());
		XUiC_MessageBoxWindowGroup.ShowOk(this.windowManager.playerUI.xui, Localization.Get("auth_messageTitle", false, null), _kickData.LocalizedMessage(), "", null, true, true, false);
	}

	// Token: 0x06009098 RID: 37016 RVA: 0x00366ECA File Offset: 0x003650CA
	public void ShowMessageServerAuthFailed(string _message)
	{
		Log.Out("Client failed to authorize server: " + _message);
		XUiC_MessageBoxWindowGroup.ShowOk(this.windowManager.playerUI.xui, Localization.Get("auth_serverAuthFailedTitle", false, null), _message, "", null, true, true, false);
	}

	// Token: 0x06009099 RID: 37017 RVA: 0x00366F08 File Offset: 0x00365108
	public void PlayerLoginRPC(ClientInfo _cInfo, string _playerName, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _platformUserAndToken, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _crossplatformUserAndToken, string _compatibilityVersion, ulong _discordUserId)
	{
		Log.Out("PlayerLogin: " + _playerName + "/" + _compatibilityVersion);
		Log.Out("Client IP: " + _cInfo.ip);
		AuthorizationManager.Instance.Authorize(_cInfo, _playerName, _platformUserAndToken, _crossplatformUserAndToken, _compatibilityVersion, _discordUserId);
	}

	// Token: 0x0600909A RID: 37018 RVA: 0x00366F54 File Offset: 0x00365154
	public IEnumerator RequestToEnterGame(ClientInfo _cInfo)
	{
		ModEvents.SPlayerJoinedGameData splayerJoinedGameData = new ModEvents.SPlayerJoinedGameData(_cInfo);
		ModEvents.PlayerJoinedGame.Invoke(ref splayerJoinedGameData);
		string playerName = _cInfo.playerName;
		Log.Out("RequestToEnterGame: " + _cInfo.InternalId.CombinedString + "/" + playerName);
		IPlatformUserData userData = PlatformUserManager.GetOrCreate(_cInfo.CrossplatformId);
		if (userData != null)
		{
			userData.MarkBlockedStateChanged();
			yield return PlatformUserManager.ResolveUserBlockedCoroutine(userData);
			if (userData.Blocked[EBlockType.Play].IsBlocked())
			{
				Log.Out(string.Format("Player {0} is blocked", _cInfo.InternalId));
				_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerDenied>().Setup(new GameUtils.KickPlayerData(GameUtils.EKickReason.ManualKick, 0, default(DateTime), "")));
				yield break;
			}
		}
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() && !this.persistentPlayerIds.Contains(_cInfo.InternalId.ToString()))
		{
			if (this.persistentPlayerCount + 1 > 100)
			{
				Log.Out("Persistent player data entries limit reached, rejecting new player {0}", new object[]
				{
					_cInfo.InternalId.ToString()
				});
				_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerDenied>().Setup(new GameUtils.KickPlayerData(GameUtils.EKickReason.PersistentPlayerDataExceeded, 0, default(DateTime), "")));
				yield break;
			}
			this.persistentPlayerIds.Add(_cInfo.InternalId.ToString());
		}
		PersistentPlayerList ppList = (this.persistentPlayers != null) ? this.persistentPlayers.NetworkCloneRelevantForPlayer() : null;
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageIdMapping>().Setup("blocks", Block.fullMappingDataForClients));
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageIdMapping>().Setup("items", ItemClass.fullMappingDataForClients));
		yield return NetPackageLocalization.StartSendingPacketsToClient(_cInfo);
		WorldStaticData.SendXmlsToClient(_cInfo);
		PlatformUserIdentifierAbs persistentPlayerID = this.getPersistentPlayerID(_cInfo);
		bool flag = !PlayerDataFile.Exists(GameIO.GetPlayerDataDir(), persistentPlayerID.CombinedString);
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageWorldInfo>().Setup(GamePrefs.GetString(EnumGamePrefs.GameMode), GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameName), this.m_World.Guid, ppList, GameTimer.Instance.ticks, this.m_World.ChunkCache.IsFixedSize, flag));
		DecoManager.Instance.SendDecosToClient(_cInfo);
		ChunkCluster chunkCache = this.m_World.ChunkCache;
		if (chunkCache != null)
		{
			_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChunkClusterInfo>().Setup(chunkCache));
		}
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageWorldSpawnPoints>().Setup(this.GetSpawnPointList()));
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageWorldAreas>().Setup(this.m_World.TraderAreas));
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageGameStats>().Setup(GameStats.Instance));
		yield break;
	}

	// Token: 0x0600909B RID: 37019 RVA: 0x00366F6C File Offset: 0x0036516C
	public void WorldInfo(string _gameMode, string _levelName, string _gameName, string _guid, PersistentPlayerList _playerList, ulong _ticks, bool _fixedSizeCC, bool _firstTimeJoin, Dictionary<string, uint> _worldFileHashes, long _worldDataSize)
	{
		Log.Out("Received game GUID: " + _guid);
		GamePrefs.Set(EnumGamePrefs.GameMode, _gameMode);
		GameIO.SetSaveGameLocalGuid(_guid);
		GamePrefs.Set(EnumGamePrefs.GameWorld, _levelName);
		this.persistentPlayers = _playerList;
		base.StartCoroutine(this.worldInfoCo(_levelName, _gameName, _fixedSizeCC, _firstTimeJoin, _worldFileHashes, _worldDataSize));
	}

	// Token: 0x0600909C RID: 37020 RVA: 0x00366FC0 File Offset: 0x003651C0
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator worldInfoCo(string _levelName, string _gameName, bool _fixedSizeCC, bool _firstTimeJoin, Dictionary<string, uint> _worldFileHashes, long _worldDataSize)
	{
		while (!WorldStaticData.AllConfigsReceivedAndLoaded())
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				yield break;
			}
			yield return null;
		}
		GeneratedTextManager.PrefilterText(SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.ServerLoginConfirmationText, GeneratedTextManager.TextFilteringMode.Filter);
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadCreatingWorld", false, null), true);
		yield return null;
		string dataDir = GameIO.GetSaveGameLocalDir();
		string rwiFilename = Path.Combine(dataDir, "RemoteWorldInfo.xml");
		bool downloadWorld = false;
		PathAbstractions.AbstractedLocation worldLocation = PathAbstractions.AbstractedLocation.None;
		List<PathAbstractions.AbstractedLocation> availablePathsList = PathAbstractions.WorldsSearchPaths.GetAvailablePathsList(null, false, null);
		foreach (PathAbstractions.AbstractedLocation possibleWorld in availablePathsList)
		{
			if (possibleWorld.Type == PathAbstractions.EAbstractedLocationType.LocalSave && SdFile.Exists(possibleWorld.FullPath + "/completed"))
			{
				worldLocation = possibleWorld;
				break;
			}
			if (possibleWorld.Type != PathAbstractions.EAbstractedLocationType.None)
			{
				if (possibleWorld.Name.Equals(_levelName))
				{
					bool worldValid = true;
					yield return NetPackageWorldFolder.TestWorldValid(possibleWorld.FullPath, _worldFileHashes, delegate(bool _valid)
					{
						worldValid = _valid;
					});
					if (worldValid)
					{
						worldLocation = possibleWorld;
						break;
					}
				}
			}
			else
			{
				possibleWorld = default(PathAbstractions.AbstractedLocation);
			}
		}
		List<PathAbstractions.AbstractedLocation>.Enumerator enumerator = default(List<PathAbstractions.AbstractedLocation>.Enumerator);
		if (worldLocation.Type == PathAbstractions.EAbstractedLocationType.None)
		{
			Log.Out("Matching world for " + _levelName + " not found. Will download from server");
			downloadWorld = true;
		}
		else
		{
			GamePrefs.Set(EnumGamePrefs.GameWorldLocationType, (int)worldLocation.Type);
			GamePrefs.Set(EnumGamePrefs.UserWorldStorageType, (int)worldLocation.StorageType);
		}
		int value = SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.GetValue(GameInfoInt.WorldSize);
		long num = SaveDataLimitUtils.CalculatePlayerMapSize(new Vector2i(value, value));
		long requiredSpace = 2048L + num;
		if (downloadWorld || worldLocation.Type == PathAbstractions.EAbstractedLocationType.LocalSave)
		{
			requiredSpace += _worldDataSize;
		}
		UserDataStorageType @int = (UserDataStorageType)GamePrefs.GetInt(EnumGamePrefs.GameSaveStorageType);
		if (SaveInfoProvider.DataLimitEnabled && @int.UsesDataLimit())
		{
			long num2 = 0L;
			string @string = GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
			SaveInfoProvider.SaveEntryInfo saveEntryInfo;
			if (SaveInfoProvider.Instance.TryGetRemoteSaveEntry(@string, out saveEntryInfo))
			{
				num2 = saveEntryInfo.SizeInfo.ReportedSize;
			}
			if (num2 < requiredSpace)
			{
				long pendingBytes = requiredSpace - num2;
				string protectedPath = (saveEntryInfo != null) ? saveEntryInfo.SaveDir : null;
				XUiC_SaveSpaceNeeded confirmationWindow = XUiC_SaveSpaceNeeded.Open(pendingBytes, protectedPath, @int, false, true, false, "xuiDmRemoteSaveTitle", "xuiDmRemoteSaveBody", null, null, "xuiStart", null);
				if (confirmationWindow != null)
				{
					while (confirmationWindow.IsOpen || confirmationWindow.Result == XUiC_SaveSpaceNeeded.ConfirmationResult.Pending)
					{
						yield return null;
					}
					if (confirmationWindow.Result != XUiC_SaveSpaceNeeded.ConfirmationResult.Confirmed)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
					}
					if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
					{
						yield break;
					}
				}
				XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, null, null, true, true, false);
				confirmationWindow = null;
			}
			else if (num2 > requiredSpace)
			{
				requiredSpace = num2;
			}
			SaveInfoProvider.Instance.ClearResources();
		}
		try
		{
			if (!SdDirectory.Exists(dataDir))
			{
				SdDirectory.CreateDirectory(dataDir);
			}
			else
			{
				SdFile.Delete(Path.Combine(dataDir, "archived.flag"));
			}
		}
		catch (Exception e)
		{
			Log.Error("Exception creating local save dir: " + dataDir + " - GUID len: " + GamePrefs.GetString(EnumGamePrefs.GameGuidClient).Length.ToString());
			Log.Exception(e);
			throw;
		}
		string path = Path.Combine(dataDir, "hosts.txt");
		string item = SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.GetValue(GameInfoString.IP) + ":" + SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.GetValue(GameInfoInt.Port).ToString();
		List<string> list;
		if (SdFile.Exists(path))
		{
			list = new List<string>(SdFile.ReadAllLines(path));
		}
		else
		{
			list = new List<string>();
		}
		list.Remove(item);
		list.Insert(0, item);
		SdFile.WriteAllLines(path, list.ToArray());
		VersionInformation gameVersion;
		if (VersionInformation.TryParseSerializedString(SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.GetValue(GameInfoString.ServerVersion), out gameVersion))
		{
			RemoteWorldInfo remoteWorldInfo = new RemoteWorldInfo(_gameName, _levelName, gameVersion, requiredSpace);
			remoteWorldInfo.Write(rwiFilename);
		}
		else
		{
			Log.Error("Failed writing RemoteWorldInfo. Could not parse LastGameServerInfo information.");
		}
		if (downloadWorld)
		{
			XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, string.Format(Localization.Get("uiLoadDownloadingWorldWait", false, null), 0f, 0, 0), true);
			yield return NetPackageWorldFolder.RequestWorld();
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				yield break;
			}
			Log.Out("World received");
			GamePrefs.Set(EnumGamePrefs.UserWorldStorageType, GamePrefs.GetInt(EnumGamePrefs.GameSaveStorageType));
			GamePrefs.Set(EnumGamePrefs.GameWorldLocationType, 1);
			worldLocation = PathAbstractions.Contextual.FindActiveWorldLocation();
		}
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadDownloadingSigns", false, null), true);
		yield return SignDataManager.Instance.RequestWorldSignDataFromServer();
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield break;
		}
		yield return this.createWorld(_levelName, worldLocation, _gameName, _fixedSizeCC);
		this.GetDynamicPrefabDecorator();
		yield return this.GetDynamicPrefabDecorator().RequestWorldPOIMetadataFromServer();
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield break;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWorldInitInfoRequest>().Setup(), false);
		while (!this.worldInitInfoReceived)
		{
			yield return null;
		}
		XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, Localization.Get("uiLoadCreatingPlayer", false, null), true);
		yield return null;
		this.worldCreated = true;
		this.firstTimeJoin = _firstTimeJoin;
		string confirmationText = GeneratedTextManager.GetDisplayTextImmediately(SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.ServerLoginConfirmationText, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.Supported);
		if (string.IsNullOrEmpty(confirmationText))
		{
			confirmationText = SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo.ServerLoginConfirmationText.Text;
		}
		if (!string.IsNullOrEmpty(confirmationText))
		{
			LocalPlayerUI playerUI = LocalPlayerUI.GetUIForPrimaryPlayer();
			while (!playerUI.xui.IsReady)
			{
				yield return null;
			}
			yield return null;
			if (!string.IsNullOrEmpty(XUiC_ServerJoinRulesDialog.ID) && playerUI.xui.FindWindowGroupByName(XUiC_ServerJoinRulesDialog.ID) != null)
			{
				XUiC_ProgressWindow.Close(LocalPlayerUI.primaryUI);
				this.windowManager.Close("crossplayWarning");
				XUiC_ServerJoinRulesDialog.Show(playerUI, confirmationText);
			}
			else
			{
				this.DoSpawn();
			}
			playerUI = null;
		}
		else
		{
			this.DoSpawn();
		}
		confirmationText = null;
		DynamicMeshManager.Init();
		yield break;
		yield break;
	}

	// Token: 0x0600909D RID: 37021 RVA: 0x00366FFC File Offset: 0x003651FC
	public void DoSpawn()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.SkipSpawnButton))
		{
			this.RequestToSpawn(-1);
			return;
		}
		XUiC_SpawnSelectionWindow.Open(LocalPlayerUI.primaryUI, false, true, this.firstTimeJoin);
	}

	// Token: 0x0600909E RID: 37022 RVA: 0x00367024 File Offset: 0x00365224
	public void RequestToSpawn(int _nearEntityId = -1)
	{
		XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, null, null, true, true, false);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageRequestToSpawnPlayer>().Setup(Utils.FastMin(12, GamePrefs.GetInt(EnumGamePrefs.OptionsGfxViewDistance)), PlayerProfile.LoadLocalProfile(), _nearEntityId), false);
	}

	// Token: 0x0600909F RID: 37023 RVA: 0x0036705D File Offset: 0x0036525D
	public void ChunkClusterInfo(string _name, bool _bInifiniteTerrain, Vector2i _cMin, Vector2i _cMax, Vector3 _pos)
	{
		base.StartCoroutine(this.chunkClusterInfoCo(_name, _bInifiniteTerrain, _cMin, _cMax, _pos));
	}

	// Token: 0x060090A0 RID: 37024 RVA: 0x00367073 File Offset: 0x00365273
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator chunkClusterInfoCo(string _name, bool _bInifiniteTerrain, Vector2i _cMin, Vector2i _cMax, Vector3 _pos)
	{
		while (!this.worldCreated && SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield return null;
		}
		if (!this.worldCreated)
		{
			yield break;
		}
		if (this.m_World == null)
		{
			yield break;
		}
		ChunkCluster chunkCache = this.m_World.ChunkCache;
		chunkCache.Position = _pos;
		chunkCache.ChunkMinPos = _cMin;
		chunkCache.ChunkMaxPos = _cMax;
		if (!_bInifiniteTerrain && this.m_World.m_WorldEnvironment != null)
		{
			this.m_World.m_WorldEnvironment.SetColliders((float)((_cMin.x + 1) * 16), (float)((_cMin.y + 1) * 16), (float)((_cMax.x - _cMin.x - 1) * 16), (float)((_cMax.y - _cMin.y - 1) * 16), global::Constants.cSizePlanesAround, 0f);
			this.m_World.m_WorldEnvironment.CreateLevelBorderBox(this.m_World);
			this.m_World.ChunkCache.IsFixedSize = true;
		}
		this.chunkClusterLoaded = true;
		yield break;
	}

	// Token: 0x060090A1 RID: 37025 RVA: 0x003670A0 File Offset: 0x003652A0
	public void RequestToSpawnPlayer(ClientInfo _cInfo, int _chunkViewDim, PlayerProfile _playerProfile, int _nearEntityId)
	{
		int num = GamePrefs.GetInt(EnumGamePrefs.ServerMaxAllowedViewDistance);
		if (num < 4)
		{
			num = 4;
		}
		else if (num > 12)
		{
			num = 12;
		}
		_chunkViewDim = Mathf.Clamp(_chunkViewDim, 4, num);
		PlatformUserIdentifierAbs persistentPlayerId = this.getPersistentPlayerID(_cInfo);
		PlayerDataFile playerDataFile = new PlayerDataFile();
		playerDataFile.Load(GameIO.GetPlayerDataDir(), persistentPlayerId.CombinedString);
		playerDataFile.lastSpawnPosition = SpawnPosition.Undef;
		int num2 = 0;
		int num3;
		if (!playerDataFile.bLoaded || playerDataFile.id == -1)
		{
			EntityFactory.nextEntityID = (num3 = EntityFactory.nextEntityID) + 1;
		}
		else
		{
			num3 = playerDataFile.id;
		}
		int num4 = num3;
		if (this.m_World.GetEntity(num4) != null)
		{
			num4 = EntityFactory.nextEntityID++;
			playerDataFile.id = num4;
		}
		Log.Out(string.Format("RequestToSpawnPlayer: {0}, {1}, {2}", num4, _cInfo.playerName, _chunkViewDim));
		if (GameStats.GetBool(EnumGameStats.IsSpawnNearOtherPlayer))
		{
			for (int i = 0; i < this.m_World.Players.list.Count; i++)
			{
				int x;
				int y;
				int z;
				if (this.m_World.Players.list[i].TeamNumber == num2 && this.m_World.FindRandomSpawnPointNearPlayer(this.m_World.Players.list[i], 15, out x, out y, out z, 15))
				{
					playerDataFile.lastSpawnPosition = new SpawnPosition(new Vector3i(x, y, z), 0f);
					break;
				}
			}
		}
		if (_nearEntityId != -1)
		{
			AllowSpawnNearFriend spawnNearFriendMode = XUiC_SpawnNearFriendsList.SpawnNearFriendMode;
			Entity entity = this.m_World.GetEntity(_nearEntityId);
			if (entity && spawnNearFriendMode != AllowSpawnNearFriend.Disabled)
			{
				int num5 = 15;
				Vector3 vector;
				bool flag;
				do
				{
					num5--;
					flag = this.m_World.GetRandomSpawnPositionMinMaxToPosition(entity.position, 40, 150, 1, true, out vector, num4, true, 20, true, EnumLandClaimOwner.None, false);
					if (!flag)
					{
						break;
					}
					if (spawnNearFriendMode == AllowSpawnNearFriend.InForest)
					{
						BiomeDefinition biomeInWorld = this.m_World.GetBiomeInWorld((int)vector.x, (int)vector.z);
						BiomeDefinition.BiomeType? biomeType = (biomeInWorld != null) ? new BiomeDefinition.BiomeType?(biomeInWorld.m_BiomeType) : null;
						if (biomeType == null)
						{
							goto IL_234;
						}
						BiomeDefinition.BiomeType valueOrDefault = biomeType.GetValueOrDefault();
						if (valueOrDefault - BiomeDefinition.BiomeType.Forest > 1)
						{
							goto IL_234;
						}
						bool flag2 = true;
						IL_237:
						flag = flag2;
						goto IL_23B;
						IL_234:
						flag2 = false;
						goto IL_237;
					}
					IL_23B:;
				}
				while (num5 > 0 && !flag);
				if (flag)
				{
					playerDataFile.lastSpawnPosition = new SpawnPosition(vector, this.m_World.RandomRange(0f, 360f));
				}
				else
				{
					Log.Warning(string.Format("RequestToSpawnPlayer: Failed getting a valid spawn position near player with entity ID {0}", _nearEntityId));
				}
			}
		}
		if (playerDataFile.lastSpawnPosition.IsUndef())
		{
			playerDataFile.lastSpawnPosition = this.GetSpawnPointList().GetRandomSpawnPosition(this.m_World, null, 0, 0);
		}
		if (!playerDataFile.bLoaded)
		{
			playerDataFile.ecd.pos = playerDataFile.lastSpawnPosition.position;
		}
		EntityCreationData entityCreationData = new EntityCreationData();
		if (!playerDataFile.bLoaded || playerDataFile.ecd.playerProfile == null || !GamePrefs.GetBool(EnumGamePrefs.PersistentPlayerProfiles))
		{
			playerDataFile.ecd.playerProfile = _playerProfile;
		}
		if (playerDataFile.bLoaded)
		{
			entityCreationData.entityData = playerDataFile.ecd.entityData;
			entityCreationData.readFileVersion = playerDataFile.ecd.readFileVersion;
		}
		entityCreationData.entityClass = EntityClass.FromString(playerDataFile.ecd.playerProfile.EntityClassName);
		entityCreationData.playerProfile = playerDataFile.ecd.playerProfile;
		entityCreationData.id = num4;
		entityCreationData.teamNumber = num2;
		entityCreationData.pos = playerDataFile.ecd.pos;
		entityCreationData.rot = playerDataFile.ecd.rot;
		EntityPlayer entityPlayer = (EntityPlayer)EntityFactory.CreateEntity(entityCreationData);
		entityPlayer.isEntityRemote = true;
		entityPlayer.Respawn(playerDataFile.bLoaded ? RespawnType.JoinMultiplayer : RespawnType.EnterMultiplayer);
		playerDataFile.ToPlayer(entityPlayer);
		bool flag3 = false;
		PersistentPlayerList persistentPlayerList = this.persistentPlayers;
		PersistentPlayerData persistentPlayerData = (persistentPlayerList != null) ? persistentPlayerList.GetPlayerData(persistentPlayerId) : null;
		if (persistentPlayerData == null)
		{
			PersistentPlayerList persistentPlayerList2 = this.persistentPlayers;
			persistentPlayerData = ((persistentPlayerList2 != null) ? persistentPlayerList2.CreatePlayerData(persistentPlayerId, _cInfo.PlatformId, _cInfo.playerName, _cInfo.device.ToPlayGroup()) : null);
		}
		else
		{
			persistentPlayerData.Update(_cInfo.PlatformId, new AuthoredText(_cInfo.playerName, persistentPlayerId), _cInfo.device.ToPlayGroup());
			flag3 = true;
		}
		persistentPlayerData.LastLogin = DateTime.Now;
		persistentPlayerData.EntityId = num4;
		if (this.persistentPlayers != null)
		{
			this.persistentPlayers.MapPlayer(persistentPlayerData);
		}
		PersistentPlayerList persistentPlayerList3 = this.persistentPlayers;
		if (persistentPlayerList3 != null)
		{
			persistentPlayerList3.SavePersistentPlayerData();
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SetClientEntityId(_cInfo, num4, playerDataFile);
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerId>().Setup(num4, num2, playerDataFile, _chunkViewDim));
		GameManager.Instance.World.aiDirector.GetComponent<AIDirectorAirDropComponent>().RefreshCrates(num4);
		this.m_World.SpawnEntityInWorld(entityPlayer);
		entityPlayer.ChunkObserver = this.m_World.m_ChunkManager.AddChunkObserver(entityPlayer.GetPosition(), false, _chunkViewDim, entityPlayer.entityId);
		IMapChunkDatabase.TryCreateOrLoad(entityPlayer.entityId, out entityPlayer.ChunkObserver.mapDatabase, () => new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataDir(), persistentPlayerId.CombinedString));
		if (this.persistentPlayers != null)
		{
			this.persistentPlayers.DispatchPlayerEvent(persistentPlayerData, null, EnumPersistentPlayerDataReason.Login);
		}
		if (flag3)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(persistentPlayerData, EnumPersistentPlayerDataReason.Login), false, -1, -1, -1, null, 192, false);
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(persistentPlayerData, EnumPersistentPlayerDataReason.New), false, -1, -1, -1, null, 192, false);
		}
		ModEvents.SPlayerSpawningData splayerSpawningData = new ModEvents.SPlayerSpawningData(_cInfo, _chunkViewDim, _playerProfile);
		ModEvents.PlayerSpawning.Invoke(ref splayerSpawningData);
	}

	// Token: 0x060090A2 RID: 37026 RVA: 0x0036764C File Offset: 0x0036584C
	public void PersistentPlayerLogin(PersistentPlayerData ppData)
	{
		if (this.persistentPlayers == null)
		{
			return;
		}
		this.persistentPlayers.SetPlayerData(ppData);
		if (this.myPlayerId != -1 && ppData.EntityId == this.myPlayerId)
		{
			this.persistentLocalPlayer = ppData;
			if (this.myEntityPlayerLocal != null)
			{
				this.myEntityPlayerLocal.persistentPlayerData = this.persistentLocalPlayer;
			}
		}
		this.persistentPlayers.DispatchPlayerEvent(ppData, null, EnumPersistentPlayerDataReason.Login);
	}

	// Token: 0x060090A3 RID: 37027 RVA: 0x003676BC File Offset: 0x003658BC
	public void HandlePersistentPlayerDisconnected(int _entityId)
	{
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_entityId);
		if (playerDataFromEntityID != null)
		{
			this.persistentPlayers.DispatchPlayerEvent(playerDataFromEntityID, null, EnumPersistentPlayerDataReason.Disconnected);
			this.persistentPlayers.UnmapPlayer(playerDataFromEntityID.PrimaryId);
		}
	}

	// Token: 0x060090A4 RID: 37028 RVA: 0x003676F8 File Offset: 0x003658F8
	public void PlayerId(int _playerId, int _teamNumber, PlayerDataFile _playerDataFile, int _chunkViewDim)
	{
		Log.Out(string.Format("PlayerId({0}, {1})", _playerId, _teamNumber));
		Log.Out("Allowed ChunkViewDistance: " + _chunkViewDim.ToString());
		GameStats.Set(EnumGameStats.AllowedViewDistance, _chunkViewDim);
		this.myPlayerId = _playerId;
		EntityCreationData entityCreationData = new EntityCreationData();
		entityCreationData.id = _playerId;
		entityCreationData.teamNumber = _teamNumber;
		if (_playerDataFile.bLoaded)
		{
			entityCreationData.entityClass = EntityClass.FromString(_playerDataFile.ecd.playerProfile.EntityClassName);
			entityCreationData.playerProfile = _playerDataFile.ecd.playerProfile;
		}
		else
		{
			entityCreationData.playerProfile = PlayerProfile.LoadLocalProfile();
			entityCreationData.entityClass = EntityClass.FromString(entityCreationData.playerProfile.EntityClassName);
		}
		entityCreationData.skinTexture = GamePrefs.GetString(EnumGamePrefs.OptionsPlayerModelTexture);
		entityCreationData.id = _playerId;
		entityCreationData.pos = _playerDataFile.ecd.pos;
		entityCreationData.rot = _playerDataFile.ecd.rot;
		entityCreationData.belongsPlayerId = _playerId;
		EntityPlayerLocal entityPlayerLocal = EntityFactory.CreateEntity(entityCreationData) as EntityPlayerLocal;
		this.setLocalPlayerEntity(entityPlayerLocal);
		Log.Out(string.Format("Found own player entity with id {0}", entityPlayerLocal.entityId));
		entityPlayerLocal.lastSpawnPosition = _playerDataFile.lastSpawnPosition;
		if (_playerDataFile.bLoaded)
		{
			_playerDataFile.ToPlayer(entityPlayerLocal);
			this.clientRespawnType = RespawnType.JoinMultiplayer;
		}
		else
		{
			this.clientRespawnType = RespawnType.EnterMultiplayer;
		}
		this.m_World.SpawnEntityInWorld(entityPlayerLocal);
		entityPlayerLocal.ChunkObserver = this.m_World.m_ChunkManager.AddChunkObserver(entityPlayerLocal.GetPosition(), true, GameUtils.GetViewDistance(), -1);
		IMapChunkDatabase.TryCreateOrLoad(entityPlayerLocal.entityId, out entityPlayerLocal.ChunkObserver.mapDatabase, delegate
		{
			string combinedString = this.getPersistentPlayerID(null).CombinedString;
			return new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataLocalDir(), combinedString);
		});
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		uiforPlayer.xui.SetDataConnections();
		uiforPlayer.xui.SetCraftingData(_playerDataFile.craftingData);
		this.SetWorldTime(this.m_World.worldTime);
		PlayerInteractions.Instance.JoinedMultiplayerServer(this.persistentPlayers);
		entityPlayerLocal.Respawn(this.clientRespawnType);
		this.gameStateManager.InitGame(false);
		this.gameStateManager.StartGame();
		base.StartCoroutine(this.LogPlayerJoinServerEventAnalyticsCoroutine());
	}

	// Token: 0x060090A5 RID: 37029 RVA: 0x0036790C File Offset: 0x00365B0C
	public void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos, int _entityId)
	{
		if (_entityId == -1)
		{
			return;
		}
		Entity entity;
		if (!this.m_World.Entities.dict.TryGetValue(_entityId, out entity))
		{
			return;
		}
		EntityPlayer entityPlayer = entity as EntityPlayer;
		if (entityPlayer == null)
		{
			return;
		}
		if (_respawnReason == RespawnType.Died && entityPlayer.isEntityRemote)
		{
			entityPlayer.SetAlive();
		}
		if (_respawnReason == RespawnType.EnterMultiplayer || _respawnReason == RespawnType.JoinMultiplayer)
		{
			this.DisplayGameMessage(EnumGameMessages.JoinedGame, _entityId, -1, true);
		}
		PlayerInteractions.Instance.PlayerSpawnedInMultiplayerServer(this.persistentPlayers, _entityId, _respawnReason);
		bool flag = _respawnReason == RespawnType.NewGame || _respawnReason == RespawnType.EnterMultiplayer || _respawnReason == RespawnType.JoinMultiplayer || _respawnReason == RespawnType.LoadedGame;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && flag)
		{
			VehicleManager.Instance.UpdateVehicleWaypointsForPlayer(_entityId);
			DroneManager.Instance.UpdateWaypointsForPlayer(_entityId);
			DroneManager.Instance.SpawnFollowingDronesForPLayer(_entityId, this.World);
		}
		ModEvents.SPlayerSpawnedInWorldData splayerSpawnedInWorldData = new ModEvents.SPlayerSpawnedInWorldData(_cInfo, entityPlayer is EntityPlayerLocal, _entityId, _respawnReason, _pos);
		ModEvents.PlayerSpawnedInWorld.Invoke(ref splayerSpawnedInWorldData);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Action<ClientInfo> onClientSpawned = this.OnClientSpawned;
			if (onClientSpawned != null)
			{
				onClientSpawned(_cInfo);
			}
			Log.Out("PlayerSpawnedInWorld (reason: {0}, position: {2}): {1}", new object[]
			{
				_respawnReason.ToStringCached<RespawnType>(),
				(_cInfo != null) ? _cInfo.ToString() : "localplayer",
				_pos.ToString()
			});
		}
	}

	// Token: 0x060090A6 RID: 37030 RVA: 0x00367A4C File Offset: 0x00365C4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleFirstSpawnInteractions(PlayerInteraction _interaction)
	{
		if (_interaction.Type != PlayerInteractionType.FirstSpawn)
		{
			return;
		}
		int num = this.persistentPlayers.PlayerToEntityMap[_interaction.PlayerData.PrimaryId];
		if (!(this.myEntityPlayerLocal == null))
		{
			int num2 = num;
			EntityPlayerLocal entityPlayerLocal = this.myEntityPlayerLocal;
			int? num3 = (entityPlayerLocal != null) ? new int?(entityPlayerLocal.entityId) : null;
			if (!(num2 == num3.GetValueOrDefault() & num3 != null))
			{
				IPlatformUserData orCreate = PlatformUserManager.GetOrCreate(_interaction.PlayerData.PrimaryId);
				if (num != -1 && orCreate != null && orCreate.Blocked[EBlockType.Play].IsBlocked())
				{
					this.DisplayGameMessage(EnumGameMessages.BlockedPlayerAlert, num, -1, true);
					return;
				}
				if (GamePrefs.GetBool(EnumGamePrefs.OptionsAutoPartyWithFriends))
				{
					PersistentPlayerData persistentPlayerData = this.myEntityPlayerLocal.persistentPlayerData;
					if (persistentPlayerData != null && persistentPlayerData.IsAlly(_interaction.PlayerData.PrimaryId))
					{
						if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.SendInvite, this.myEntityPlayerLocal.entityId, num, null, null), false);
							return;
						}
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.SendInvite, this.myEntityPlayerLocal.entityId, num, null, null), false, -1, -1, -1, null, 192, false);
					}
				}
				return;
			}
		}
	}

	// Token: 0x060090A7 RID: 37031 RVA: 0x00367B94 File Offset: 0x00365D94
	public void PlayerDisconnected(ClientInfo _cInfo)
	{
		if (_cInfo.entityId != -1)
		{
			EntityPlayer entityPlayer = (EntityPlayer)this.m_World.GetEntity(_cInfo.entityId);
			Log.Out("Player {0} disconnected after {1} minutes", new object[]
			{
				GameUtils.SafeStringFormat(entityPlayer.EntityName),
				((Time.timeSinceLevelLoad - entityPlayer.CreationTimeSinceLevelLoad) / 60f).ToCultureInvariantString("0.0")
			});
		}
		if (GameManager.IsDedicatedServer)
		{
			GC.Collect();
			MemoryPools.Cleanup();
		}
		PersistentPlayerData persistentPlayerData = this.getPersistentPlayerData(_cInfo);
		if (persistentPlayerData != null)
		{
			persistentPlayerData.LastLogin = DateTime.Now;
			persistentPlayerData.EntityId = -1;
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePersistentPlayerState>().Setup(persistentPlayerData, EnumPersistentPlayerDataReason.Disconnected), false, -1, -1, -1, null, 192, true);
		}
		PersistentPlayerList persistentPlayerList = this.persistentPlayers;
		if (persistentPlayerList != null)
		{
			persistentPlayerList.SavePersistentPlayerData();
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.DisconnectClient(_cInfo, false, true);
	}

	// Token: 0x060090A8 RID: 37032 RVA: 0x00367C74 File Offset: 0x00365E74
	public void SavePlayerData(ClientInfo _cInfo, PlayerDataFile _playerDataFile)
	{
		_cInfo.latestPlayerData = _playerDataFile;
		int entityId = _cInfo.entityId;
		if (entityId != -1)
		{
			EntityPlayer entityPlayer = (EntityPlayer)this.m_World.GetEntity(entityId);
			if (entityPlayer != null)
			{
				_playerDataFile.Save(GameIO.GetPlayerDataDir(), _cInfo.InternalId.CombinedString);
				if (entityPlayer.ChunkObserver.mapDatabase != null)
				{
					ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(entityPlayer.ChunkObserver.mapDatabase.SaveAsync), new IMapChunkDatabase.DirectoryPlayerId(GameIO.GetPlayerDataDir(), _cInfo.InternalId.CombinedString), null, true);
				}
				entityPlayer.QuestJournal = _playerDataFile.questJournal;
				if (this.persistentPlayers != null)
				{
					foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in this.persistentPlayers.Players)
					{
						if (keyValuePair.Value.EntityId == _playerDataFile.id)
						{
							keyValuePair.Value.Position = new Vector3i(_playerDataFile.ecd.pos);
							break;
						}
					}
				}
			}
		}
		ModEvents.SSavePlayerDataData ssavePlayerDataData = new ModEvents.SSavePlayerDataData(_cInfo, _playerDataFile);
		ModEvents.SavePlayerData.Invoke(ref ssavePlayerDataData);
	}

	// Token: 0x060090A9 RID: 37033 RVA: 0x00367DA8 File Offset: 0x00365FA8
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs getPersistentPlayerID(ClientInfo _cInfo)
	{
		return ((_cInfo != null) ? _cInfo.InternalId : null) ?? PlatformManager.InternalLocalUserIdentifier;
	}

	// Token: 0x060090AA RID: 37034 RVA: 0x00367DBF File Offset: 0x00365FBF
	[PublicizedFrom(EAccessModifier.Private)]
	public PersistentPlayerData getPersistentPlayerData(ClientInfo _cInfo)
	{
		PersistentPlayerList persistentPlayerList = this.persistentPlayers;
		if (persistentPlayerList == null)
		{
			return null;
		}
		return persistentPlayerList.GetPlayerData(this.getPersistentPlayerID(_cInfo));
	}

	// Token: 0x060090AB RID: 37035 RVA: 0x00367DD9 File Offset: 0x00365FD9
	public PersistentPlayerList GetPersistentPlayerList()
	{
		return this.persistentPlayers;
	}

	// Token: 0x060090AC RID: 37036 RVA: 0x00367DE1 File Offset: 0x00365FE1
	public PersistentPlayerData GetPersistentLocalPlayer()
	{
		return this.persistentLocalPlayer;
	}

	// Token: 0x060090AD RID: 37037 RVA: 0x00367DE9 File Offset: 0x00365FE9
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator createWorld(string _levelName, PathAbstractions.AbstractedLocation _worldLocation, string _sGameName, bool _fixedSizeCC = false)
	{
		Log.Out(string.Format("createWorld: {0}, {1}, {2}, {3}", new object[]
		{
			_levelName,
			_worldLocation,
			_sGameName,
			GamePrefs.GetString(EnumGamePrefs.GameMode)
		}));
		GamePrefs.Set(EnumGamePrefs.GameNameClient, _sGameName);
		bool flag = GameModeEditWorld.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode));
		PathAbstractions.CacheEnabled = !flag;
		if (flag)
		{
			global::Constants.cDigAndBuildDistance = 50f;
			global::Constants.cBuildIntervall = 0.2f;
			global::Constants.cCollectItemDistance = 50f;
		}
		else if (GameModeCreative.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)))
		{
			global::Constants.cDigAndBuildDistance = 25f;
			global::Constants.cBuildIntervall = 0.2f;
			global::Constants.cCollectItemDistance = 25f;
		}
		else
		{
			global::Constants.cDigAndBuildDistance = 5f;
			global::Constants.cBuildIntervall = 0.5f;
			global::Constants.cCollectItemDistance = 3.5f;
		}
		OcclusionManager.Instance.WorldChanging(flag);
		yield return null;
		this.m_World = new World();
		if (GameManager.IsDedicatedServer || this.IsEditMode())
		{
			this.GameHasStarted = true;
		}
		else
		{
			base.StartCoroutine(this.waitForGameStart());
		}
		this.m_World.Init(this, WorldBiomes.Instance);
		yield return null;
		if (this.biomeParticleManager == null)
		{
			this.biomeParticleManager = new BiomeParticleManager();
		}
		if (this.OnWorldChanged != null)
		{
			this.OnWorldChanged(this.m_World);
		}
		PlayerInteractions.Instance.OnNewPlayerInteraction += this.HandleFirstSpawnInteractions;
		yield return null;
		SignTextureManager.Instance.SetQuality((SignTextureManager.SignTextureQuality)GamePrefs.GetInt(EnumGamePrefs.OptionsGfxSignQuality));
		SignTextureManager.Instance.SetTileSizeForCurrentQuality();
		SignTextureManager.Instance.Initialize();
		yield return null;
		yield return this.m_World.LoadWorld(_levelName, _worldLocation, _fixedSizeCC);
		yield return null;
		AstarManager.Init(base.gameObject);
		yield return null;
		this.lootManager = new LootManager(this.m_World);
		yield return null;
		LockManager.Instance.Init();
		yield return null;
		this.traderManager = new TraderManager(this.m_World);
		yield return null;
		ResourceRequest weatherLoading = Resources.LoadAsync("Prefabs/WeatherManager");
		while (!weatherLoading.isDone)
		{
			yield return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(weatherLoading.asset as GameObject) as GameObject;
		gameObject.transform.SetParent(base.transform, false);
		WeatherManager.Init(this.m_World, gameObject);
		yield return null;
		yield return EnvironmentAudioManager.CreateNewInstance();
		yield return null;
		new WaterSplashCubes();
		yield return null;
		WireManager.Instance.Init();
		yield return null;
		LoadManager.AssetRequestTask<GameObject> requestTask = LoadManager.LoadAsset<GameObject>("@:Prefabs/SkySystem/SkySystem.prefab", null, null, false, false, false);
		yield return new WaitUntil(() => requestTask.IsDone);
		SkyManager.Loaded(UnityEngine.Object.Instantiate<GameObject>(requestTask.Asset));
		yield return null;
		if (WeatherManager.Instance)
		{
			WeatherManager.Instance.CloudsFrameUpdateNow();
			WeatherManager.Instance.InitParticles();
		}
		yield return null;
		if (this.IsEditMode())
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = this.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator != null && this.IsEditMode())
			{
				dynamicPrefabDecorator.CreateBoundingBoxes();
			}
			SpawnPointList spawnPointList = this.GetSpawnPointList();
			for (int i = 0; i < spawnPointList.Count; i++)
			{
				SpawnPoint spawnPoint = spawnPointList[i];
				SelectionBoxManager.Instance.CategoryStartPoint.AddBox(spawnPoint.spawnPosition.ToBlockPos().ToString(), spawnPoint.spawnPosition.ToBlockPos(), Vector3i.one, true, false).FacingDirection = spawnPoint.spawnPosition.heading;
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				PrefabInstanceClientManager.Instance.StartAsServer();
			}
			else
			{
				PrefabInstanceClientManager.Instance.StartAsClient();
			}
		}
		ModEvents.SCreateWorldDoneData screateWorldDoneData = default(ModEvents.SCreateWorldDoneData);
		ModEvents.CreateWorldDone.Invoke(ref screateWorldDoneData);
		Log.Out("createWorld() done");
		yield break;
	}

	// Token: 0x1700114B RID: 4427
	// (get) Token: 0x060090AE RID: 37038 RVA: 0x00367E15 File Offset: 0x00366015
	public World World
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_World;
		}
	}

	// Token: 0x060090AF RID: 37039 RVA: 0x00367E1D File Offset: 0x0036601D
	public SpawnPointList GetSpawnPointList()
	{
		return this.m_World.ChunkCache.ChunkProvider.GetSpawnPointList();
	}

	// Token: 0x060090B0 RID: 37040 RVA: 0x00367E34 File Offset: 0x00366034
	public ChunkManager.ChunkObserver AddChunkObserver(Vector3 _initialPosition, bool _bBuildVisualMeshAround, int _viewDim, int _entityIdToSendChunksTo)
	{
		return this.m_World.m_ChunkManager.AddChunkObserver(_initialPosition, _bBuildVisualMeshAround, _viewDim, _entityIdToSendChunksTo);
	}

	// Token: 0x060090B1 RID: 37041 RVA: 0x00367E4B File Offset: 0x0036604B
	public void RemoveChunkObserver(ChunkManager.ChunkObserver _observer)
	{
		this.m_World.m_ChunkManager.RemoveChunkObserver(_observer);
	}

	// Token: 0x060090B2 RID: 37042 RVA: 0x00367E60 File Offset: 0x00366060
	public void ExplosionServer(Vector3 _worldPos, Vector3i _blockPos, Quaternion _rotation, ExplosionData _explosionData, int _entityId, float _delay, bool _bRemoveBlockAtExplPosition, ItemValue _itemValueExplosionSource = null)
	{
		if (_bRemoveBlockAtExplPosition)
		{
			this.m_World.SetBlockRPC(_blockPos, BlockValue.Air);
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageExplosionInitiate>().Setup(_worldPos, _blockPos, _rotation, _explosionData, _entityId, _delay, _bRemoveBlockAtExplPosition, _itemValueExplosionSource), false);
			return;
		}
		if (_delay <= 0f)
		{
			this.explode(_worldPos, _blockPos, _rotation, _explosionData, _entityId, _itemValueExplosionSource);
			return;
		}
		base.StartCoroutine(this.explodeLater(_worldPos, _blockPos, _rotation, _explosionData, _entityId, _itemValueExplosionSource, _delay));
	}

	// Token: 0x060090B3 RID: 37043 RVA: 0x00367EE8 File Offset: 0x003660E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator explodeLater(Vector3 _position, Vector3i _blockPos, Quaternion _rotation, ExplosionData _explosionData, int _entityId, ItemValue _itemValueExplosionSource, float _delayInSec)
	{
		yield return new WaitForSeconds(_delayInSec);
		this.explode(_position, _blockPos, _rotation, _explosionData, _entityId, _itemValueExplosionSource);
		yield break;
	}

	// Token: 0x060090B4 RID: 37044 RVA: 0x00367F38 File Offset: 0x00366138
	[PublicizedFrom(EAccessModifier.Private)]
	public void explode(Vector3 _worldPos, Vector3i _blockPos, Quaternion _rotation, ExplosionData _explosionData, int _entityId, ItemValue _itemValueExplosionSource)
	{
		Explosion explosion = new Explosion(this.m_World, _worldPos, _blockPos, _explosionData, _entityId);
		explosion.AttackBlocks(_entityId, _itemValueExplosionSource);
		explosion.AttackEntites(_entityId, _itemValueExplosionSource, _explosionData.DamageType);
		this.tempExplPositions.Clear();
		explosion.ChangedBlockPositions.CopyValuesTo(this.tempExplPositions);
		GameManager.ExplodeGroup explodeGroup = new GameManager.ExplodeGroup();
		explodeGroup.pos = _worldPos;
		explodeGroup.radius = _explosionData.BlockRadius;
		explodeGroup.delay = 3;
		foreach (BlockChangeInfo blockChangeInfo in this.tempExplPositions)
		{
			if (blockChangeInfo.blockValue.isair)
			{
				BlockValue block = this.m_World.GetBlock(blockChangeInfo.blockValueRef);
				if (!block.isair && block.Block.IsExplosionAffected)
				{
					GameManager.ExplodeGroup.Falling item;
					item.pos = blockChangeInfo.blockValueRef;
					item.bv = block;
					explodeGroup.fallings.Add(item);
				}
			}
		}
		if (explodeGroup.fallings.Count > 0)
		{
			this.explodeFallingGroups.Add(explodeGroup);
		}
		GameObject gameObject = this.ExplosionClient(_worldPos, _rotation, _explosionData.ParticleIndex, _explosionData.BlastPower, (float)_explosionData.EntityRadius, _explosionData.BlockDamage, _entityId, this.tempExplPositions);
		if (gameObject != null)
		{
			if (_explosionData.Duration > 0f)
			{
				TemporaryObject component = gameObject.GetComponent<TemporaryObject>();
				if (component != null)
				{
					component.SetLife(_explosionData.Duration);
				}
			}
			ExplosionDamageArea explosionDamageArea;
			if (gameObject.TryGetComponent<ExplosionDamageArea>(out explosionDamageArea))
			{
				explosionDamageArea.BuffActions = _explosionData.BuffActions;
				explosionDamageArea.InitiatorEntityId = _entityId;
			}
			if (this.m_World.aiDirector != null && !_explosionData.IgnoreHeatMap)
			{
				AudioPlayer component2 = gameObject.GetComponent<AudioPlayer>();
				if (component2)
				{
					this.m_World.aiDirector.OnSoundPlayedAtPosition(_entityId, _worldPos, component2.soundName, 1f);
				}
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() > 0)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageExplosionClient>().Setup(_worldPos, _rotation, _explosionData.ParticleIndex, _explosionData.BlastPower, _explosionData.BlockDamage, (float)_explosionData.EntityRadius, _entityId, this.tempExplPositions), true, -1, -1, -1, null, 192, false);
		}
		this.tempExplPositions.Clear();
	}

	// Token: 0x060090B5 RID: 37045 RVA: 0x0036819C File Offset: 0x0036639C
	public GameObject ExplosionClient(Vector3 _center, Quaternion _rotation, int _index, int _blastPower, float _blastRadius, float _blockDamage, int _entityId, List<BlockChangeInfo> _explosionChanges)
	{
		if (this.m_World == null)
		{
			return null;
		}
		GameObject result = null;
		if (_index > 0 && _index < WorldStaticData.prefabExplosions.Length && WorldStaticData.prefabExplosions[_index] != null)
		{
			result = UnityEngine.Object.Instantiate<GameObject>(WorldStaticData.prefabExplosions[_index].gameObject, _center - Origin.position, _rotation);
			ApplyExplosionForce.Explode(_center, (float)_blastPower, _blastRadius);
		}
		if (_explosionChanges.Count > 0)
		{
			this.ChangeBlocks(null, _explosionChanges);
		}
		QuestEventManager.Current.DetectedExplosion(_center, _entityId, _blockDamage);
		return result;
	}

	// Token: 0x060090B6 RID: 37046 RVA: 0x00368220 File Offset: 0x00366420
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExplodeGroupFrameUpdate()
	{
		int et = EntityClass.FromString("fallingBlock");
		GameRandom gameRandom = this.m_World.GetGameRandom();
		for (int i = this.explodeFallingGroups.Count - 1; i >= 0; i--)
		{
			GameManager.ExplodeGroup explodeGroup = this.explodeFallingGroups[i];
			GameManager.ExplodeGroup explodeGroup2 = explodeGroup;
			int num = explodeGroup2.delay - 1;
			explodeGroup2.delay = num;
			if (num <= 0)
			{
				float num2 = 20f + Mathf.Pow((float)explodeGroup.fallings.Count, 0.73f);
				float num3 = Utils.FastMax(1f, (float)explodeGroup.fallings.Count / num2);
				float num4 = 1f;
				for (int j = 0; j < explodeGroup.fallings.Count; j++)
				{
					if ((num4 -= 1f) <= 0f)
					{
						num4 += num3;
						GameManager.ExplodeGroup.Falling falling = explodeGroup.fallings[j];
						Vector3 vector = falling.pos.ToVector3Center();
						vector.y += 1.4f;
						if (Physics.Raycast(vector - Origin.position, Vector3.down, 3.4028235E+38f, 65536))
						{
							vector.y -= 1.4f;
							Block block = falling.bv.Block;
							block.DropItemsOnEvent(this.m_World, falling.bv, EnumDropEvent.Destroy, 0.5f, vector, Vector3.zero, global::Constants.cItemExplosionLifetime, -1, true);
							if (block.ShowModelOnFall())
							{
								EntityFallingBlock entityFallingBlock = (EntityFallingBlock)EntityFactory.CreateEntity(et, -1, new BlockValue[]
								{
									falling.bv
								}, new TextureFullArray[]
								{
									this.m_World.GetTextureFullArray(falling.pos.x, falling.pos.y, falling.pos.z)
								}, 1, vector, Vector3.zero, -1f, -1, -1, "");
								Vector3 vector2 = vector - explodeGroup.pos;
								float num5 = 1f - Mathf.Clamp01(vector2.magnitude / explodeGroup.radius) * 0.6f;
								float d = 18f * num5;
								vector2.y += -0.2f + gameRandom.RandomFloat * 6f;
								entityFallingBlock.SetStartVelocity(vector2.normalized * d, (gameRandom.RandomFloat * 15f + 2f) * num5);
								this.m_World.SpawnEntityInWorld(entityFallingBlock);
							}
						}
					}
				}
				this.explodeFallingGroups.RemoveAt(i);
			}
		}
	}

	// Token: 0x060090B7 RID: 37047 RVA: 0x003684B0 File Offset: 0x003666B0
	public void ChangeBlocks(PlatformUserIdentifierAbs persistentPlayerId, List<BlockChangeInfo> _blocksToChange)
	{
		if (this.m_World == null)
		{
			return;
		}
		List<ChunkCluster> obj = this.ccChanged;
		lock (obj)
		{
			Entity entity = null;
			if (persistentPlayerId == null)
			{
				PersistentPlayerData playerData = this.persistentLocalPlayer;
				entity = this.myEntityPlayerLocal;
			}
			else if (this.persistentPlayers != null)
			{
				PersistentPlayerData playerData = this.persistentPlayers.GetPlayerData(persistentPlayerId);
				if (playerData != null && playerData.EntityId != -1)
				{
					entity = this.m_World.GetEntity(playerData.EntityId);
				}
			}
			bool flag2 = false;
			ChunkCluster chunkCluster = null;
			int num = 0;
			int i = 0;
			while (i < _blocksToChange.Count)
			{
				BlockChangeInfo blockChangeInfo = _blocksToChange[i];
				if (chunkCluster != null)
				{
					goto IL_C0;
				}
				chunkCluster = this.m_World.ChunkCache;
				if (chunkCluster != null)
				{
					if (!this.ccChanged.Contains(chunkCluster))
					{
						this.ccChanged.Add(chunkCluster);
						num++;
						chunkCluster.ChunkPosNeedsRegeneration_DelayedStart();
						goto IL_C0;
					}
					goto IL_C0;
				}
				IL_558:
				i++;
				continue;
				IL_C0:
				bool flag3 = blockChangeInfo.bChangeDensity;
				bool bForceDensity = blockChangeInfo.bForceDensity;
				sbyte density = chunkCluster.GetDensity(blockChangeInfo.blockValueRef);
				sbyte b = blockChangeInfo.density;
				if (!flag3)
				{
					if (density < 0 && blockChangeInfo.blockValue.isair)
					{
						b = MarchingCubes.DensityAir;
						flag3 = true;
					}
					else if (density >= 0 && blockChangeInfo.blockValue.Block.shape.IsTerrain())
					{
						b = MarchingCubes.DensityTerrain;
						flag3 = true;
					}
				}
				if (density == b)
				{
					flag3 = false;
				}
				if (blockChangeInfo.bChangeDamage && chunkCluster.GetBlock(blockChangeInfo.blockValueRef).type != blockChangeInfo.blockValue.type)
				{
					goto IL_558;
				}
				if (blockChangeInfo.blockValueRef.Type != BlockValueRefType.Block)
				{
					chunkCluster.SetBlockValue(blockChangeInfo.blockValueRef, blockChangeInfo.blockValue);
					goto IL_558;
				}
				Chunk chunk = chunkCluster.GetChunkFromWorldPos(blockChangeInfo.blockValueRef) as Chunk;
				Vector3i vector3i;
				if (chunk != null && blockChangeInfo.blockValueRef.TryGetBlockPos(out vector3i))
				{
					int num2 = World.toBlockXZ(vector3i.x);
					int num3 = World.toBlockXZ(vector3i.z);
					if (vector3i.y >= (int)chunk.GetHeight(num2, num3) && blockChangeInfo.blockValue.Block.shape.IsTerrain())
					{
						chunk.SetTopSoilBroken(num2, num3);
						Chunk chunk2 = chunk;
						if (num3 == 15)
						{
							chunk2 = chunkCluster.GetChunkSync(chunk.X, chunk.Z + 1);
						}
						if (chunk2 != null)
						{
							chunk2.SetTopSoilBroken(num2, World.toBlockXZ(num3 + 1));
						}
						chunk2 = chunk;
						if (num2 == 15)
						{
							chunk2 = chunkCluster.GetChunkSync(chunk.X + 1, chunk.Z);
						}
						if (chunk2 != null)
						{
							chunk2.SetTopSoilBroken(World.toBlockXZ(num2 + 1), num3);
						}
						chunk2 = chunk;
						if (num3 == 0)
						{
							chunk2 = chunkCluster.GetChunkSync(chunk.X, chunk.Z - 1);
						}
						if (chunk2 != null)
						{
							chunk2.SetTopSoilBroken(num2, World.toBlockXZ(num3 - 1));
						}
						chunk2 = chunk;
						if (num2 == 0)
						{
							chunk2 = chunkCluster.GetChunkSync(chunk.X - 1, chunk.Z);
						}
						if (chunk2 != null)
						{
							chunk2.SetTopSoilBroken(World.toBlockXZ(num2 - 1), num3);
						}
					}
					this.m_World.UncullChunk(chunk);
				}
				TileEntity tileEntity = null;
				if (!blockChangeInfo.blockValue.ischild)
				{
					tileEntity = this.m_World.GetTileEntity(blockChangeInfo.blockValueRef);
				}
				BlockValue bvOld = chunkCluster.SetBlock(blockChangeInfo.blockValueRef, blockChangeInfo.bChangeBlockValue, blockChangeInfo.blockValue, flag3, b, true, blockChangeInfo.bUpdateLight, bForceDensity, false, blockChangeInfo.changedByEntityId);
				if (tileEntity != null)
				{
					TileEntity tileEntity2 = this.m_World.GetTileEntity(blockChangeInfo.blockValueRef);
					bool flag4 = false;
					if (tileEntity != tileEntity2 && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
					{
						flag4 = true;
						tileEntity.ReplacedBy(bvOld, blockChangeInfo.blockValue, tileEntity2);
					}
					if (blockChangeInfo.blockValue.isair)
					{
						flag4 = true;
						if (chunk != null)
						{
							chunk.RemoveTileEntityAt<TileEntity>(this.m_World, World.toBlock(blockChangeInfo.blockValueRef));
						}
					}
					else if (tileEntity != tileEntity2)
					{
						flag4 = true;
						if (tileEntity2 != null)
						{
							tileEntity2.UpgradeDowngradeFrom(tileEntity);
						}
					}
					if (flag4 && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && LockManager.Instance.IsLockedServer(tileEntity, 0))
					{
						LockManager.Instance.ForceUnlockLockTarget(tileEntity);
					}
				}
				if (chunk != null && blockChangeInfo.blockValue.isair)
				{
					chunk.RemoveBlockTrigger(World.toBlock(blockChangeInfo.blockValueRef));
				}
				if (bvOld.type != blockChangeInfo.blockValue.type)
				{
					Block block = blockChangeInfo.blockValue.Block;
					Block block2 = bvOld.Block;
					QuestEventManager.Current.BlockChanged(block2, block, blockChangeInfo.blockValueRef);
					if (block is BlockSleepingBag || block2 is BlockSleepingBag)
					{
						EntityAlive entityAlive = entity as EntityAlive;
						if (entityAlive)
						{
							if (block is BlockSleepingBag)
							{
								NavObjectManager.Instance.UnRegisterNavObjectByOwnerEntity(entityAlive, "sleeping_bag");
								entityAlive.SpawnPoints.Set(blockChangeInfo.blockValueRef);
							}
							else
							{
								this.persistentPlayers.SpawnPointRemoved(blockChangeInfo.blockValueRef);
							}
							flag2 = true;
						}
					}
				}
				if (blockChangeInfo.bChangeTexture)
				{
					chunkCluster.SetTextureFullArray(blockChangeInfo.blockValueRef, blockChangeInfo.textureFull);
					goto IL_558;
				}
				if (bvOld.Block.CanBlocksReplace)
				{
					chunkCluster.SetTextureFullArray(blockChangeInfo.blockValueRef, new TextureFullArray(0L));
					goto IL_558;
				}
				goto IL_558;
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && flag2)
			{
				PersistentPlayerList persistentPlayerList = this.persistentPlayers;
				if (persistentPlayerList != null)
				{
					persistentPlayerList.SavePersistentPlayerData();
				}
			}
			if (num > 0)
			{
				int num4 = this.ccChanged.Count;
				for (int j = 0; j < num; j++)
				{
					this.ccChanged[--num4].ChunkPosNeedsRegeneration_DelayedStop();
				}
				this.ccChanged.RemoveRange(num4, num);
			}
		}
	}

	// Token: 0x060090B8 RID: 37048 RVA: 0x00368ABC File Offset: 0x00366CBC
	public void ChangeProps(PlatformUserIdentifierAbs persistentPlayerId, List<PropChangeInfo> propsToChange)
	{
		if (this.m_World == null)
		{
			return;
		}
		List<ChunkCluster> obj = this.ccChanged;
		lock (obj)
		{
			Log.Warning(string.Format("ChangeProps player {0}, count {1}", (persistentPlayerId != null) ? persistentPlayerId.ToString() : "", propsToChange.Count));
			ChunkCluster chunkCache = this.m_World.ChunkCache;
			int num = 0;
			if (chunkCache != null)
			{
				if (!this.ccChanged.Contains(chunkCache))
				{
					this.ccChanged.Add(chunkCache);
					num++;
					chunkCache.ChunkPosNeedsRegeneration_DelayedStart();
				}
				foreach (PropChangeInfo propChangeInfo in propsToChange)
				{
					if (chunkCache.GetChunkSync(propChangeInfo.ChunkPos) == null)
					{
						return;
					}
					chunkCache.SetProp(propChangeInfo.ChunkPos, propChangeInfo.PropId, propChangeInfo.Position, propChangeInfo.Rotation, propChangeInfo.Scale, propChangeInfo.BlockValue);
				}
			}
			if (num > 0)
			{
				int num2 = this.ccChanged.Count;
				for (int i = 0; i < num; i++)
				{
					this.ccChanged[--num2].ChunkPosNeedsRegeneration_DelayedStop();
				}
				this.ccChanged.RemoveRange(num2, num);
			}
		}
	}

	// Token: 0x060090B9 RID: 37049 RVA: 0x00368C3C File Offset: 0x00366E3C
	public void SetBlocksRPC(List<BlockChangeInfo> _changes, PlatformUserIdentifierAbs _persistentPlayerId = null)
	{
		this.ChangeBlocks(_persistentPlayerId, _changes);
		NetPackageSetBlock package = NetPackageManager.GetPackage<NetPackageSetBlock>().Setup(this.persistentLocalPlayer, _changes, GameManager.IsDedicatedServer ? -1 : this.myPlayerId);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SetBlocksOnClients(-1, package);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x060090BA RID: 37050 RVA: 0x00368C94 File Offset: 0x00366E94
	public void SetPropsRPC(List<PropChangeInfo> _changes, PlatformUserIdentifierAbs _persistentPlayerId = null)
	{
		this.ChangeProps(_persistentPlayerId, _changes);
		NetPackageSetProp package = NetPackageManager.GetPackage<NetPackageSetProp>().Setup(this.persistentLocalPlayer, _changes, GameManager.IsDedicatedServer ? -1 : this.myPlayerId);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SetPropsOnClients(-1, package);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x060090BB RID: 37051 RVA: 0x00368CEC File Offset: 0x00366EEC
	public void SetBlocksOnClients(int _exceptThisEntityId, NetPackageSetBlock package)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, _exceptThisEntityId, -1, null, 192, false);
	}

	// Token: 0x060090BC RID: 37052 RVA: 0x00368D18 File Offset: 0x00366F18
	public void SetPropsOnClients(int _exceptThisEntityId, NetPackageSetProp package)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, _exceptThisEntityId, -1, null, 192, false);
	}

	// Token: 0x060090BD RID: 37053 RVA: 0x00368D44 File Offset: 0x00366F44
	public void SetWaterRPC(NetPackageWaterSet package)
	{
		if (this.m_World != null)
		{
			ChunkCluster chunkCache = this.m_World.ChunkCache;
			if (chunkCache != null)
			{
				package.ApplyChanges(chunkCache);
			}
		}
		package.SetSenderId(GameManager.IsDedicatedServer ? -1 : this.myPlayerId);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x060090BE RID: 37054 RVA: 0x00368DBC File Offset: 0x00366FBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBlockParticles()
	{
		List<GameManager.BlockParticleCreationData> obj = this.blockParticlesToSpawn;
		lock (obj)
		{
			for (int i = 0; i < this.blockParticlesToSpawn.Count; i++)
			{
				if (this.m_BlockParticles.ContainsKey(this.blockParticlesToSpawn[i].blockPos))
				{
					this.RemoveBlockParticleEffect(this.blockParticlesToSpawn[i].blockPos);
				}
				Transform value = GameManager.Instance.SpawnParticleEffectClientForceCreation(this.blockParticlesToSpawn[i].particleEffect, -1, true);
				this.m_BlockParticles[this.blockParticlesToSpawn[i].blockPos] = value;
			}
			this.blockParticlesToSpawn.Clear();
		}
	}

	// Token: 0x060090BF RID: 37055 RVA: 0x00368E8C File Offset: 0x0036708C
	public void SpawnBlockParticleEffect(Vector3i _blockPos, ParticleEffect _pe)
	{
		List<GameManager.BlockParticleCreationData> obj = this.blockParticlesToSpawn;
		lock (obj)
		{
			this.blockParticlesToSpawn.Add(new GameManager.BlockParticleCreationData(_blockPos, _pe));
		}
	}

	// Token: 0x060090C0 RID: 37056 RVA: 0x00368ED8 File Offset: 0x003670D8
	public bool HasBlockParticleEffect(Vector3i _blockPos)
	{
		return this.m_BlockParticles.ContainsKey(_blockPos);
	}

	// Token: 0x060090C1 RID: 37057 RVA: 0x00368EE6 File Offset: 0x003670E6
	public Transform GetBlockParticleEffect(Vector3i _blockPos)
	{
		return this.m_BlockParticles[_blockPos];
	}

	// Token: 0x060090C2 RID: 37058 RVA: 0x00368EF4 File Offset: 0x003670F4
	public void RemoveBlockParticleEffect(Vector3i _blockPos)
	{
		List<GameManager.BlockParticleCreationData> obj = this.blockParticlesToSpawn;
		lock (obj)
		{
			if (this.m_BlockParticles.ContainsKey(_blockPos))
			{
				Transform transform = this.m_BlockParticles[_blockPos];
				this.m_BlockParticles.Remove(_blockPos);
				if (transform != null)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			else
			{
				for (int i = this.blockParticlesToSpawn.Count - 1; i >= 0; i--)
				{
					if (this.blockParticlesToSpawn[i].blockPos == _blockPos)
					{
						this.blockParticlesToSpawn.RemoveAt(i);
					}
				}
			}
		}
	}

	// Token: 0x060090C3 RID: 37059 RVA: 0x00368FA8 File Offset: 0x003671A8
	public void SpawnParticleEffectServer(ParticleEffect _pe, int _entityId, bool _forceCreation = false, bool _worldSpawn = false)
	{
		if (this.m_World == null)
		{
			return;
		}
		ParticleEffect.SpawnParticleEffect(_pe, _entityId, _forceCreation, _worldSpawn);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, _forceCreation, _worldSpawn), false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, _forceCreation, _worldSpawn), false, -1, _entityId, -1, null, 192, false);
	}

	// Token: 0x060090C4 RID: 37060 RVA: 0x0036901C File Offset: 0x0036721C
	public Transform SpawnParticleEffectClientForceCreation(ParticleEffect _pe, int _entityThatCausedIt, bool _worldSpawn)
	{
		return ParticleEffect.SpawnParticleEffect(_pe, _entityThatCausedIt, true, _worldSpawn);
	}

	// Token: 0x060090C5 RID: 37061 RVA: 0x00369027 File Offset: 0x00367227
	public void SpawnParticleEffectClient(ParticleEffect _pe, int _entityThatCausedIt, bool _forceCreation = false, bool _worldSpawn = false)
	{
		ParticleEffect.SpawnParticleEffect(_pe, _entityThatCausedIt, _forceCreation, _worldSpawn);
	}

	// Token: 0x060090C6 RID: 37062 RVA: 0x00369034 File Offset: 0x00367234
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsInit()
	{
		Physics.ContactEvent += this.PhysicsContactEvent;
	}

	// Token: 0x060090C7 RID: 37063 RVA: 0x00369048 File Offset: 0x00367248
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsContactEvent(PhysicsScene scene, NativeArray<ContactPairHeader>.ReadOnly pairHeaders)
	{
		int length = pairHeaders.Length;
		for (int i = 0; i < length; i++)
		{
			Rigidbody rigidbody = pairHeaders[i].Body as Rigidbody;
			if (rigidbody)
			{
				EntityFallingBlock entityFallingBlock;
				EntityFallingBlocks entityFallingBlocks;
				if (rigidbody.TryGetComponent<EntityFallingBlock>(out entityFallingBlock))
				{
					entityFallingBlock.OnContactEvent();
				}
				else if (rigidbody.TryGetComponent<EntityFallingBlocks>(out entityFallingBlocks))
				{
					entityFallingBlocks.OnContactEvent();
				}
			}
		}
	}

	// Token: 0x060090C8 RID: 37064 RVA: 0x003690AB File Offset: 0x003672AB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsEditMode()
	{
		return this.isEditMode;
	}

	// Token: 0x060090C9 RID: 37065 RVA: 0x003690B4 File Offset: 0x003672B4
	public void GameMessage(EnumGameMessages _type, EntityAlive _mainEntity, EntityAlive _otherEntity)
	{
		if (_mainEntity == null)
		{
			return;
		}
		int secondaryEntityId = -1;
		if (_mainEntity is EntityPlayer)
		{
			int mainEntityId;
			switch (_type)
			{
			case EnumGameMessages.PlainTextLocal:
			case EnumGameMessages.ChangedTeam:
				return;
			case EnumGameMessages.EntityWasKilled:
				mainEntityId = _mainEntity.entityId;
				if (_otherEntity is EntityPlayer)
				{
					secondaryEntityId = _otherEntity.entityId;
				}
				break;
			case EnumGameMessages.JoinedGame:
			case EnumGameMessages.LeftGame:
			case EnumGameMessages.Chat:
				mainEntityId = _mainEntity.entityId;
				break;
			default:
				return;
			}
			this.GameMessageServer(null, _type, mainEntityId, secondaryEntityId);
			return;
		}
		if (_type == EnumGameMessages.EntityWasKilled || _type == EnumGameMessages.Chat)
		{
			int mainEntityId = (_mainEntity != null) ? _mainEntity.entityId : -1;
			this.GameMessageServer(null, _type, mainEntityId, secondaryEntityId);
			return;
		}
	}

	// Token: 0x060090CA RID: 37066 RVA: 0x0036914C File Offset: 0x0036734C
	public void GameMessageServer(ClientInfo _cInfo, EnumGameMessages _type, int _mainEntityId, int _secondaryEntityId)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageGameMessage>().Setup(_type, _mainEntityId, _secondaryEntityId), false);
			return;
		}
		Entity entity = this.World.GetEntity(_mainEntityId);
		EntityPlayer entityPlayer = entity as EntityPlayer;
		string mainName;
		if (entityPlayer != null)
		{
			mainName = entityPlayer.PlayerDisplayName;
		}
		else
		{
			EntityAlive entityAlive = entity as EntityAlive;
			if (entityAlive != null)
			{
				mainName = Localization.Get(entityAlive.EntityName, false, null);
			}
			else
			{
				mainName = Localization.Get("xuiChatServer", false, null);
			}
		}
		this.FinishGameMessageServer(_cInfo, _type, _mainEntityId, _secondaryEntityId, mainName);
	}

	// Token: 0x060090CB RID: 37067 RVA: 0x003691D4 File Offset: 0x003673D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void FinishGameMessageServer(ClientInfo _cInfo, EnumGameMessages _type, int _mainEntityId, int _secondaryEntityId, string _mainName)
	{
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_secondaryEntityId);
		string secondaryName = (playerDataFromEntityID != null) ? playerDataFromEntityID.PlayerName.DisplayName : null;
		ModEvents.SGameMessageData sgameMessageData = new ModEvents.SGameMessageData(_cInfo, _type, _mainName, secondaryName);
		ValueTuple<ModEvents.EModEventResult, Mod> valueTuple = ModEvents.GameMessage.Invoke(ref sgameMessageData);
		ModEvents.EModEventResult item = valueTuple.Item1;
		Mod item2 = valueTuple.Item2;
		string text = this.DisplayGameMessage(_type, _mainEntityId, _secondaryEntityId, item2 == null);
		if (item != ModEvents.EModEventResult.StopHandlersAndVanilla)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameMessage>().Setup(_type, _mainEntityId, _secondaryEntityId), true, -1, -1, -1, null, 192, false);
			return;
		}
		Log.Out("GameMessage handled by mod '{0}': {1}", new object[]
		{
			item2.Name,
			text
		});
	}

	// Token: 0x060090CC RID: 37068 RVA: 0x00369284 File Offset: 0x00367484
	public string DisplayGameMessage(EnumGameMessages _type, int _mainEntity, int _secondaryEntity = -1, bool _log = true)
	{
		string text = null;
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_mainEntity);
		string text2;
		if (playerDataFromEntityID == null)
		{
			text2 = null;
		}
		else
		{
			PersistentPlayerName playerName = playerDataFromEntityID.PlayerName;
			text2 = ((playerName != null) ? playerName.DisplayName : null);
		}
		string text3 = text2;
		string text4;
		if (_secondaryEntity != -1)
		{
			PersistentPlayerData playerDataFromEntityID2 = this.persistentPlayers.GetPlayerDataFromEntityID(_secondaryEntity);
			if (playerDataFromEntityID2 == null)
			{
				text4 = null;
			}
			else
			{
				PersistentPlayerName playerName2 = playerDataFromEntityID2.PlayerName;
				text4 = ((playerName2 != null) ? playerName2.DisplayName : null);
			}
		}
		else
		{
			text4 = null;
		}
		string text5 = text4;
		switch (_type)
		{
		case EnumGameMessages.EntityWasKilled:
		{
			string message;
			if (!string.IsNullOrEmpty(text5))
			{
				text = string.Format("GMSG: Player '{0}' killed by '{1}'", text3, text5);
				message = string.Format(Localization.Get("killedGameMessage", false, null), text5, text3);
				goto IL_138;
			}
			text = string.Format("GMSG: Player '{0}' died", text3);
			message = string.Format(Localization.Get("diedGameMessage", false, null), text3);
			goto IL_138;
		}
		case EnumGameMessages.JoinedGame:
		{
			text = string.Format("GMSG: Player '{0}' joined the game", text3);
			string message = string.Format(Localization.Get("joinGameMessage", false, null), text3);
			goto IL_138;
		}
		case EnumGameMessages.LeftGame:
		{
			text = string.Format("GMSG: Player '{0}' left the game", text3);
			string message = string.Format(Localization.Get("leaveGameMessage", false, null), text3);
			goto IL_138;
		}
		case EnumGameMessages.BlockedPlayerAlert:
		{
			text = string.Format("GMSG: Blocked player '{0}' is present on this server!", text3);
			string message = string.Format("[FF0000A0]" + Localization.Get("blockedPlayerMessage", false, null), text3);
			goto IL_138;
		}
		}
		return text;
		IL_138:
		if (_log)
		{
			Log.Out(text);
		}
		if (!GameManager.IsDedicatedServer)
		{
			if (_type == EnumGameMessages.BlockedPlayerAlert)
			{
				string message;
				XUiC_ChatOutput.AddMessage(this.myEntityPlayerLocal.PlayerUI.xui, _type, message, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.None, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
			}
			else
			{
				foreach (EntityPlayerLocal entityPlayer in this.m_World.GetLocalPlayers())
				{
					string message;
					XUiC_ChatOutput.AddMessage(LocalPlayerUI.GetUIForPlayer(entityPlayer).xui, _type, message, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.None, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
				}
			}
		}
		return text;
	}

	// Token: 0x060090CD RID: 37069 RVA: 0x00369460 File Offset: 0x00367660
	public void ChatMessageServer(ClientInfo _cInfo, EChatType _chatType, int _senderEntityId, string _msg, List<int> _recipientEntityIds, EMessageSender _msgSender, GeneratedTextManager.BbCodeSupportMode _bbMode = GeneratedTextManager.BbCodeSupportMode.Supported)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			string text = null;
			if (_senderEntityId != -1)
			{
				PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_senderEntityId);
				string input;
				if (playerDataFromEntityID == null)
				{
					input = null;
				}
				else
				{
					PersistentPlayerName playerName = playerDataFromEntityID.PlayerName;
					if (playerName == null)
					{
						input = null;
					}
					else
					{
						AuthoredText authoredName = playerName.AuthoredName;
						input = ((authoredName != null) ? authoredName.Text : null);
					}
				}
				text = Utils.EscapeBbCodes(input, false, false);
			}
			ModEvents.SChatMessageData schatMessageData = new ModEvents.SChatMessageData(_cInfo, _chatType, _senderEntityId, _msg, text, _recipientEntityIds);
			ValueTuple<ModEvents.EModEventResult, Mod> valueTuple = ModEvents.ChatMessage.Invoke(ref schatMessageData);
			ModEvents.EModEventResult item = valueTuple.Item1;
			Mod item2 = valueTuple.Item2;
			this.ChatMessageClient(_chatType, _senderEntityId, _msg, _recipientEntityIds, _msgSender, GeneratedTextManager.BbCodeSupportMode.Supported);
			string text2 = (((_cInfo != null) ? _cInfo.PlatformId : null) != null) ? _cInfo.PlatformId.CombinedString : "-non-player-";
			string text3 = string.Format("Chat (from '{0}', entity id '{1}', to '{2}'): {3}{4}", new object[]
			{
				text2,
				_senderEntityId,
				_chatType.ToStringCached<EChatType>(),
				(text != null) ? ("'" + text + "': ") : "",
				_msg
			});
			if (item == ModEvents.EModEventResult.StopHandlersAndVanilla)
			{
				Log.Out("Chat handled by mod '{0}': {1}", new object[]
				{
					item2.Name,
					text3
				});
			}
			else
			{
				Log.Out(text3);
			}
			if (item != ModEvents.EModEventResult.StopHandlersAndVanilla)
			{
				if (_recipientEntityIds != null)
				{
					using (List<int>.Enumerator enumerator = _recipientEntityIds.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int entityId = enumerator.Current;
							ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityId);
							if (clientInfo != null)
							{
								clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg, null, _msgSender, _bbMode));
							}
						}
						return;
					}
				}
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg, null, _msgSender, _bbMode), true, -1, -1, -1, null, 192, false);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageChat>().Setup(_chatType, _senderEntityId, _msg, _recipientEntityIds, _msgSender, _bbMode), false);
		}
	}

	// Token: 0x060090CE RID: 37070 RVA: 0x00369658 File Offset: 0x00367858
	public void ChatMessageClient(EChatType _chatType, int _senderEntityId, string _msg, List<int> _recipientEntityIds, EMessageSender _msgSender, GeneratedTextManager.BbCodeSupportMode _bbMode)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		foreach (EntityPlayerLocal entityPlayerLocal in this.m_World.GetLocalPlayers())
		{
			if (_recipientEntityIds == null || _recipientEntityIds.Contains(entityPlayerLocal.entityId))
			{
				XUiC_ChatOutput.AddMessage(LocalPlayerUI.GetUIForPlayer(entityPlayerLocal).xui, EnumGameMessages.Chat, _msg, _chatType, EChatDirection.Inbound, _senderEntityId, null, _senderEntityId.ToString(), _msgSender, GeneratedTextManager.TextFilteringMode.Filter, _bbMode);
			}
		}
	}

	// Token: 0x060090CF RID: 37071 RVA: 0x003696E8 File Offset: 0x003678E8
	public void RemoveChunk(long _chunkKey)
	{
		this.m_World.m_ChunkManager.RemoveChunk(_chunkKey);
	}

	// Token: 0x060090D0 RID: 37072 RVA: 0x003696FB File Offset: 0x003678FB
	public IBlockTool GetActiveBlockTool()
	{
		if (this.activeBlockTool == null)
		{
			return this.blockSelectionTool;
		}
		return this.activeBlockTool;
	}

	// Token: 0x060090D1 RID: 37073 RVA: 0x00369712 File Offset: 0x00367912
	public void SetActiveBlockTool(IBlockTool _tool)
	{
		this.activeBlockTool = _tool;
	}

	// Token: 0x060090D2 RID: 37074 RVA: 0x0036971C File Offset: 0x0036791C
	public DynamicPrefabDecorator GetDynamicPrefabDecorator()
	{
		if (this.m_World == null)
		{
			return null;
		}
		ChunkCluster chunkCache = this.m_World.ChunkCache;
		if (chunkCache == null)
		{
			return null;
		}
		return chunkCache.ChunkProvider.GetDynamicPrefabDecorator();
	}

	// Token: 0x060090D3 RID: 37075 RVA: 0x00369750 File Offset: 0x00367950
	public void SimpleRPC(int _entityId, SimpleRPCType _rpcType, bool _bExeLocal, bool _bOnlyLocal)
	{
		if (_bExeLocal)
		{
			EntityAlive entityAlive = (EntityAlive)this.m_World.GetEntity(_entityId);
			if (entityAlive != null)
			{
				if (_rpcType != SimpleRPCType.OnActivateItem)
				{
					if (_rpcType == SimpleRPCType.OnResetItem)
					{
						entityAlive.inventory.holdingItem.OnHoldingReset(entityAlive.inventory.holdingItemData);
					}
				}
				else
				{
					entityAlive.inventory.holdingItem.OnHoldingItemActivated(entityAlive.inventory.holdingItemData);
				}
			}
		}
		if (_bOnlyLocal)
		{
			return;
		}
		NetPackage package = NetPackageManager.GetPackage<NetPackageSimpleRPC>().Setup(_entityId, _rpcType);
		if (this.m_World.IsRemote())
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
			return;
		}
		this.m_World.entityDistributer.SendPacketToTrackedPlayers(_entityId, _entityId, package, false);
	}

	// Token: 0x060090D4 RID: 37076 RVA: 0x003697FC File Offset: 0x003679FC
	public void ItemDropServer(ItemStack _itemStack, Vector3 _dropPos, Vector3 _randomPosAdd, int _entityId = -1, float _lifetime = 60f, bool _bDropPosIsRelativeToHead = false)
	{
		this.ItemDropServer(_itemStack, _dropPos, _randomPosAdd, Vector3.zero, _entityId, _lifetime, _bDropPosIsRelativeToHead, 0);
	}

	// Token: 0x060090D5 RID: 37077 RVA: 0x00369820 File Offset: 0x00367A20
	public void ItemDropServer(ItemStack _itemStack, Vector3 _dropPos, Vector3 _randomPosAdd, Vector3 _initialMotion, int _entityId = -1, float _lifetime = 60f, bool _bDropPosIsRelativeToHead = false, int _clientEntityId = 0)
	{
		if (this.m_World == null)
		{
			return;
		}
		bool flag = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		Entity entity = this.m_World.GetEntity(_entityId);
		if (_clientEntityId != 0)
		{
			if (!entity)
			{
				return;
			}
			flag = !entity.isEntityRemote;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (_clientEntityId == -1)
			{
				World world = this.m_World;
				int num = world.clientLastEntityId - 1;
				world.clientLastEntityId = num;
				_clientEntityId = num;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageItemDrop>().Setup(_itemStack, _dropPos, _initialMotion, _randomPosAdd, _lifetime, _entityId, _bDropPosIsRelativeToHead, _clientEntityId), false);
			if (!flag)
			{
				return;
			}
		}
		if (_bDropPosIsRelativeToHead)
		{
			if (entity == null)
			{
				return;
			}
			_dropPos += entity.getHeadPosition();
		}
		if (!_randomPosAdd.Equals(Vector3.zero))
		{
			_dropPos += new Vector3(this.m_World.RandomRange(-_randomPosAdd.x, _randomPosAdd.x), this.m_World.RandomRange(-_randomPosAdd.y, _randomPosAdd.y), this.m_World.RandomRange(-_randomPosAdd.z, _randomPosAdd.z));
		}
		EntityCreationData entityCreationData = new EntityCreationData();
		entityCreationData.entityClass = EntityClass.FromString("item");
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && _clientEntityId < -1)
		{
			entityCreationData.id = _clientEntityId;
		}
		else
		{
			entityCreationData.id = EntityFactory.nextEntityID++;
		}
		entityCreationData.itemStack = _itemStack.Clone();
		entityCreationData.pos = _dropPos;
		entityCreationData.rot = new Vector3(20f, 0f, 20f);
		entityCreationData.lifetime = _lifetime;
		entityCreationData.belongsPlayerId = _entityId;
		if (_clientEntityId != -1)
		{
			entityCreationData.clientEntityId = _clientEntityId;
		}
		EntityItem entityItem = (EntityItem)EntityFactory.CreateEntity(entityCreationData);
		entityItem.isPhysicsMaster = flag;
		if (_initialMotion.sqrMagnitude > 0.01f)
		{
			entityItem.AddVelocity(_initialMotion);
		}
		this.m_World.SpawnEntityInWorld(entityItem);
		Chunk chunk = (Chunk)this.m_World.GetChunkSync(World.toChunkXZ((int)_dropPos.x), World.toChunkXZ((int)_dropPos.z));
		if (chunk != null)
		{
			List<EntityItem> list = new List<EntityItem>();
			for (int i = 0; i < chunk.entityLists.Length; i++)
			{
				if (chunk.entityLists[i] != null)
				{
					for (int j = 0; j < chunk.entityLists[i].Count; j++)
					{
						if (chunk.entityLists[i][j] is EntityItem)
						{
							list.Add(chunk.entityLists[i][j] as EntityItem);
						}
					}
				}
			}
			int num2 = list.Count - 50;
			if (num2 > 0)
			{
				list.Sort(new GameManager.EntityItemLifetimeComparer());
				int num3 = list.Count - 1;
				while (num3 >= 0 && num2 > 0)
				{
					list[num3].MarkToUnload();
					num2--;
					num3--;
				}
			}
		}
	}

	// Token: 0x060090D6 RID: 37078 RVA: 0x00369AF7 File Offset: 0x00367CF7
	public void AddExpServer(int _entityId, string UNUSED_skill, int _experience)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAddExpServer>().Setup(_entityId, _experience), false);
		}
	}

	// Token: 0x060090D7 RID: 37079 RVA: 0x00369B1C File Offset: 0x00367D1C
	public void AddScoreServer(int _entityId, int _zombieKills, int _playerKills, int _otherTeamnumber, int _conditions)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAddScoreServer>().Setup(_entityId, _zombieKills, _playerKills, _otherTeamnumber, _conditions), false);
			return;
		}
		EntityAlive entityAlive = (EntityAlive)this.m_World.GetEntity(_entityId);
		if (entityAlive == null)
		{
			return;
		}
		if (entityAlive.isEntityRemote)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAddScoreClient>().Setup(_entityId, _zombieKills, _playerKills, _otherTeamnumber, _conditions), false, entityAlive.entityId, -1, -1, null, 192, false);
			return;
		}
		entityAlive.AddScore(0, _zombieKills, _playerKills, _otherTeamnumber, _conditions);
	}

	// Token: 0x060090D8 RID: 37080 RVA: 0x00369BB8 File Offset: 0x00367DB8
	public void AwardKill(EntityAlive killer, EntityAlive killedEntity)
	{
		if (killer.isEntityRemote)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAwardKillServer>().Setup(killer.entityId, killedEntity.entityId), false, killer.entityId, -1, -1, null, 192, false);
			return;
		}
		QuestEventManager.Current.EntityKilled(killer, killedEntity);
	}

	// Token: 0x060090D9 RID: 37081 RVA: 0x00369C14 File Offset: 0x00367E14
	public void ItemReloadServer(int _entityId)
	{
		if (this.m_World == null)
		{
			return;
		}
		this.ItemReloadClient(_entityId);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageItemReload>().Setup(_entityId), false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageItemReload>().Setup(_entityId), false, -1, _entityId, -1, null, 192, false);
	}

	// Token: 0x060090DA RID: 37082 RVA: 0x00369C7C File Offset: 0x00367E7C
	public void ItemReloadClient(int _entityId)
	{
		if (this.m_World == null)
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)this.m_World.GetEntity(_entityId);
		if (entityAlive != null && entityAlive.inventory.IsHoldingGun())
		{
			entityAlive.inventory.GetHoldingGun().ReloadGun(entityAlive.inventory.holdingItemData.actionData[0]);
		}
	}

	// Token: 0x060090DB RID: 37083 RVA: 0x00369CE0 File Offset: 0x00367EE0
	public void ItemActionEffectsServer(int _entityId, int _slotIdx, int _itemActionIdx, int _firingState, Vector3 _startPos, Vector3 _direction, int _userData = 0)
	{
		if (this.m_World == null)
		{
			return;
		}
		this.ItemActionEffectsClient(_entityId, _slotIdx, _itemActionIdx, _firingState, _startPos, _direction, _userData);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageItemActionEffects>().Setup(_entityId, _slotIdx, _itemActionIdx, (ItemActionFiringState)_firingState, _startPos, _direction, _userData), false);
			return;
		}
		int allButAttachedToEntityId = _entityId;
		Entity entity = this.m_World.GetEntity(_entityId);
		if (entity != null && entity.AttachedMainEntity != null)
		{
			allButAttachedToEntityId = entity.AttachedMainEntity.entityId;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageItemActionEffects>().Setup(_entityId, _slotIdx, _itemActionIdx, (ItemActionFiringState)_firingState, _startPos, _direction, _userData), false, -1, allButAttachedToEntityId, _entityId, null, 192, false);
	}

	// Token: 0x060090DC RID: 37084 RVA: 0x00369D98 File Offset: 0x00367F98
	public void ItemActionEffectsClient(int _entityId, int _slotIdx, int _itemActionIdx, int _firingState, Vector3 _startPos, Vector3 _direction, int _userData = 0)
	{
		if (this.m_World == null)
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)this.m_World.GetEntity(_entityId);
		if (entityAlive == null)
		{
			return;
		}
		ItemAction itemActionInSlot = entityAlive.inventory.GetItemActionInSlot(_slotIdx, _itemActionIdx);
		if (itemActionInSlot == null)
		{
			return;
		}
		itemActionInSlot.ItemActionEffects(this, entityAlive.inventory.GetItemActionDataInSlot(_slotIdx, _itemActionIdx), _firingState, _startPos, _direction, _userData);
	}

	// Token: 0x060090DD RID: 37085 RVA: 0x00369DF8 File Offset: 0x00367FF8
	public void SetWorldTime(ulong _worldTime)
	{
		if (this.m_World == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			_worldTime = this.m_World.worldTime;
		}
		this.m_World.SetTime(_worldTime);
	}

	// Token: 0x060090DE RID: 37086 RVA: 0x00369E28 File Offset: 0x00368028
	public void AddVelocityToEntityServer(int _entityId, Vector3 _velToAdd)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAddVelocity>().Setup(_entityId, _velToAdd), false);
			return;
		}
		Entity entity = this.m_World.GetEntity(_entityId);
		if (entity != null)
		{
			entity.AddVelocity(_velToAdd);
		}
	}

	// Token: 0x060090DF RID: 37087 RVA: 0x00369E78 File Offset: 0x00368078
	public void PickupBlockServer(Vector3i _blockPos, BlockValue _blockValue, int _playerId, PlatformUserIdentifierAbs persistentPlayerId = null)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePickupBlock>().Setup(_blockPos, _blockValue, _playerId, this.persistentLocalPlayer), false);
			return;
		}
		if (this.m_World.GetBlock(_blockPos).type != _blockValue.type)
		{
			return;
		}
		if (this.m_World.IsLocalPlayer(_playerId))
		{
			this.PickupBlockClient(_blockPos, _blockValue, _playerId);
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePickupBlock>().Setup(_blockPos, _blockValue, _playerId, null), false, _playerId, -1, -1, null, 192, false);
		}
		BlockValue blockValue = (_blockValue.Block.PickupSource != null) ? Block.GetBlockValue(_blockValue.Block.PickupSource, false) : BlockValue.Air;
		this.SetBlocksRPC(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_blockPos, blockValue, true)
		}, persistentPlayerId);
	}

	// Token: 0x060090E0 RID: 37088 RVA: 0x00369F5C File Offset: 0x0036815C
	public void PickupBlockClient(Vector3i _blockPos, BlockValue _blockValue, int _playerId)
	{
		if (this.m_World.GetBlock(_blockPos).type != _blockValue.type)
		{
			return;
		}
		ItemStack itemStack = _blockValue.Block.OnBlockPickedUp(this.m_World, _blockPos, _blockValue, _playerId);
		QuestEventManager.Current.BlockPickedUp(_blockValue.Block.GetBlockName(), _blockPos);
		QuestEventManager.Current.ItemAdded(itemStack);
		foreach (EntityPlayerLocal entityPlayerLocal in this.m_World.GetLocalPlayers())
		{
			if (entityPlayerLocal.entityId == _playerId && entityPlayerLocal.PlayerUI.xui.PlayerInventory.AddItem(itemStack, true))
			{
				return;
			}
		}
		this.ItemDropServer(itemStack, _blockPos.ToVector3() + Vector3.one * 0.5f, Vector3.zero, _playerId, 60f, false);
	}

	// Token: 0x060090E1 RID: 37089 RVA: 0x0036A058 File Offset: 0x00368258
	public void PlaySoundAtPositionServer(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance, float _volumeScale = 1f)
	{
		this.PlaySoundAtPositionServer(_pos, _audioClipName, _mode, _distance, this.m_World.GetPrimaryPlayerId(), _volumeScale);
	}

	// Token: 0x060090E2 RID: 37090 RVA: 0x0036A074 File Offset: 0x00368274
	public void PlaySoundAtPositionServer(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance, int _entityId, float _volumeScale = 1f)
	{
		if (this.m_World == null)
		{
			return;
		}
		if (!GameManager.IsDedicatedServer)
		{
			Manager.BroadcastPlay(_pos, _audioClipName, 0f);
			if (this.m_World.aiDirector != null)
			{
				this.m_World.aiDirector.NotifyNoise(this.m_World.GetEntity(_entityId), _pos, _audioClipName, _volumeScale);
			}
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSoundAtPosition>().Setup(_pos, _audioClipName, _mode, _distance, _entityId, _volumeScale), false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSoundAtPosition>().Setup(_pos, _audioClipName, _mode, _distance, _entityId, _volumeScale), false, -1, _entityId, -1, null, 192, false);
	}

	// Token: 0x060090E3 RID: 37091 RVA: 0x0036A127 File Offset: 0x00368327
	public void PlaySoundAtPositionClient(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance, float _volumeScale)
	{
		if (this.m_World == null)
		{
			return;
		}
		Manager.Play(_pos, _audioClipName, -1, false, _volumeScale);
		if (this.m_World.aiDirector != null)
		{
			this.m_World.aiDirector.NotifyNoise(null, _pos, _audioClipName, _volumeScale);
		}
	}

	// Token: 0x060090E4 RID: 37092 RVA: 0x0036A160 File Offset: 0x00368360
	public void WaypointInviteServer(Waypoint _waypoint, EnumWaypointInviteMode _inviteMode, int _inviterEntityId)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWaypoint>().Setup(_waypoint, _inviteMode, _inviterEntityId), false);
			return;
		}
		_waypoint = _waypoint.Clone();
		_waypoint.bTracked = false;
		if (_inviteMode != EnumWaypointInviteMode.Friends)
		{
			for (int i = 0; i < this.m_World.Players.list.Count; i++)
			{
				EntityPlayer entityPlayer = this.m_World.Players.list[i];
				if (entityPlayer.entityId != _inviterEntityId)
				{
					if (this.m_World.IsLocalPlayer(entityPlayer.entityId))
					{
						this.WaypointInviteClient(_waypoint, _inviteMode, _inviterEntityId, null);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWaypoint>().Setup(_waypoint, _inviteMode, _inviterEntityId), false, entityPlayer.entityId, -1, -1, null, 192, false);
					}
				}
			}
			return;
		}
		if (this.m_World.GetEntity(_inviterEntityId) as EntityPlayer == null)
		{
			return;
		}
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_inviterEntityId);
		if (playerDataFromEntityID == null)
		{
			return;
		}
		for (int j = 0; j < this.m_World.Players.list.Count; j++)
		{
			EntityPlayer entityPlayer2 = this.m_World.Players.list[j];
			if (entityPlayer2.entityId != _inviterEntityId)
			{
				PersistentPlayerData other = (this.persistentPlayers != null) ? this.persistentPlayers.GetPlayerDataFromEntityID(entityPlayer2.entityId) : null;
				if (playerDataFromEntityID.IsAlly(other))
				{
					if (this.m_World.IsLocalPlayer(entityPlayer2.entityId))
					{
						this.WaypointInviteClient(_waypoint, _inviteMode, _inviterEntityId, null);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWaypoint>().Setup(_waypoint, _inviteMode, _inviterEntityId), false, entityPlayer2.entityId, -1, -1, null, 192, false);
					}
				}
			}
		}
	}

	// Token: 0x060090E5 RID: 37093 RVA: 0x0036A32C File Offset: 0x0036852C
	public void RemovePartyInvitesFromAllPlayers(EntityPlayer _player)
	{
		for (int i = 0; i < this.m_World.Players.list.Count; i++)
		{
			EntityPlayer entityPlayer = this.m_World.Players.list[i];
			if (entityPlayer != _player)
			{
				entityPlayer.RemovePartyInvite(_player.entityId);
			}
		}
	}

	// Token: 0x060090E6 RID: 37094 RVA: 0x0036A388 File Offset: 0x00368588
	public void WaypointInviteClient(Waypoint _waypoint, EnumWaypointInviteMode _inviteMode, int _inviterEntityId, EntityPlayerLocal _player = null)
	{
		if (_player == null)
		{
			_player = this.myEntityPlayerLocal;
		}
		if (_player == null)
		{
			return;
		}
		PersistentPlayerData playerDataFromEntityID = this.persistentPlayers.GetPlayerDataFromEntityID(_inviterEntityId);
		if (playerDataFromEntityID != null && playerDataFromEntityID.PlatformData.Blocked[EBlockType.TextChat].IsBlocked())
		{
			return;
		}
		if (_player.Waypoints.ContainsWaypoint(_waypoint))
		{
			return;
		}
		for (int i = 0; i < _player.WaypointInvites.Count; i++)
		{
			if (_player.WaypointInvites[i].Equals(_waypoint))
			{
				return;
			}
		}
		_player.WaypointInvites.Insert(0, _waypoint);
		XUiV_Window window = LocalPlayerUI.GetUIForPlayer(_player).xui.GetWindow("mapInvites");
		if (window != null && window.IsVisible)
		{
			((XUiC_MapInvitesList)window.Controller.GetChildById("invitesList")).UpdateInvitesList();
		}
		string strPlayerName = "?";
		EntityPlayer entityPlayer = this.m_World.GetEntity(_inviterEntityId) as EntityPlayer;
		if (entityPlayer != null)
		{
			strPlayerName = entityPlayer.PlayerDisplayName;
		}
		GeneratedTextManager.GetDisplayText(_waypoint.name, delegate(string _filtered)
		{
			GameManager.ShowTooltip(_player, string.Format(Localization.Get("tooltipInviteMarker", false, null), strPlayerName, _waypoint.bUsingLocalizationId ? Localization.Get(_filtered, false, null) : _filtered), false, false, 0f);
		}, true, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
	}

	// Token: 0x060090E7 RID: 37095 RVA: 0x0036A500 File Offset: 0x00368700
	public void QuestShareServer(NetPackageSharedQuest.SharedQuestData sqd)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(sqd), false);
			return;
		}
		if (this.m_World.IsLocalPlayer(sqd.sharedWithEntityID))
		{
			this.QuestShareClient(sqd, null);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(sqd), false, sqd.sharedWithEntityID, -1, -1, null, 192, false);
	}

	// Token: 0x060090E8 RID: 37096 RVA: 0x0036A57C File Offset: 0x0036877C
	public void QuestShareClient(NetPackageSharedQuest.SharedQuestData sqd, EntityPlayerLocal _player = null)
	{
		if (_player == null)
		{
			_player = this.myEntityPlayerLocal;
		}
		if (_player == null)
		{
			return;
		}
		if (_player.QuestJournal.HasActiveQuestByQuestCode(sqd.questCode))
		{
			if (PartyQuests.AutoAccept)
			{
				Log.Out(string.Format("Ignoring received quest, already have one active with the quest code {0}:", sqd.questCode));
				for (int i = 0; i < _player.QuestJournal.quests.Count; i++)
				{
					Quest quest = _player.QuestJournal.quests[i];
					Log.Out(string.Format("  {0}.: id={1}, code={2}, name={3}, POI={4}, state={5}, owner={6}", new object[]
					{
						i,
						quest.ID,
						quest.QuestCode,
						quest.QuestClass.Name,
						quest.GetParsedText("{poi.name}"),
						quest.CurrentState,
						quest.SharedOwnerID
					}));
				}
			}
			return;
		}
		_player.AddSharedQuestEntry(new NetPackageSharedQuest.SharedQuestData(sqd));
	}

	// Token: 0x060090E9 RID: 37097 RVA: 0x0036A68C File Offset: 0x0036888C
	public void SharedKillServer(int _entityID, int _killerID, float _xpModifier = 1f)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSharedPartyKill>().Setup(_entityID, _killerID), false);
			return;
		}
		EntityPlayer entityPlayer = (EntityPlayer)this.m_World.GetEntity(_killerID);
		EntityAlive entityAlive = this.m_World.GetEntity(_entityID) as EntityAlive;
		if (entityPlayer == null || entityAlive == null)
		{
			return;
		}
		int num = EntityClass.list[entityAlive.entityClass].ExperienceValue;
		num = (int)EffectManager.GetValue(PassiveEffects.ExperienceGain, entityAlive.inventory.holdingItemItemValue, (float)num, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (_xpModifier != 1f)
		{
			num = (int)((float)num * _xpModifier + 0.5f);
		}
		if (entityPlayer.IsInParty())
		{
			int num2 = entityPlayer.Party.MemberCountInRange(entityPlayer);
			num = (int)((float)num * (1f - 0.1f * (float)num2));
		}
		if (entityPlayer.Party != null)
		{
			for (int i = 0; i < entityPlayer.Party.MemberList.Count; i++)
			{
				EntityPlayer entityPlayer2 = entityPlayer.Party.MemberList[i];
				if (!(entityPlayer2 == entityPlayer) && Vector3.Distance(entityPlayer.position, entityPlayer2.position) < (float)GameStats.GetInt(EnumGameStats.PartySharedKillRange))
				{
					if (this.m_World.IsLocalPlayer(entityPlayer2.entityId))
					{
						this.SharedKillClient(entityAlive.entityClass, num, null, entityAlive.entityId, -1);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedPartyKill>().Setup(entityAlive.entityClass, num, _killerID, entityAlive.entityId), false, entityPlayer2.entityId, -1, -1, null, 192, false);
					}
				}
			}
		}
	}

	// Token: 0x060090EA RID: 37098 RVA: 0x0036A848 File Offset: 0x00368A48
	public void SharedKillClient(int _entityTypeID, int _xp, EntityPlayerLocal _player = null, int _entityID = -1, int _killerID = -1)
	{
		if (_player == null)
		{
			_player = this.myEntityPlayerLocal;
		}
		if (_player == null)
		{
			return;
		}
		string entityClassName = EntityClass.list[_entityTypeID].entityClassName;
		_xp = _player.Progression.AddLevelExp(_xp, "_xpFromParty", Progression.XPTypes.Kill, true, true, _killerID, null);
		_player.bPlayerStatsChanged = true;
		if (_xp > 0 && (Progression.ShowXPType == Progression.ShowXPTypes.All || Progression.ShowXPType == Progression.ShowXPTypes.NotificationsOnly))
		{
			GameManager.ShowTooltip(_player, string.Format(Localization.Get("ttPartySharedXPReceived", false, null), _xp), false, false, 0f);
		}
		QuestEventManager.Current.EntityKilled(_player, (_entityID == -1) ? null : (this.m_World.GetEntity(_entityID) as EntityAlive));
	}

	// Token: 0x060090EB RID: 37099 RVA: 0x0036A8FD File Offset: 0x00368AFD
	public IEnumerator ShowExitingGameUICoroutine()
	{
		bool flag = this.windowManager.IsWindowOpen(XUiC_ExitingGame.ID);
		this.windowManager.Open(XUiC_ExitingGame.ID, false);
		if (flag)
		{
			yield break;
		}
		yield return null;
		yield return null;
		yield break;
	}

	// Token: 0x060090EC RID: 37100 RVA: 0x0036A90C File Offset: 0x00368B0C
	public static void ShowTooltipMP(EntityPlayer _player, string _text, string _alertSound = "")
	{
		if (_player is EntityPlayerLocal)
		{
			GameManager.ShowTooltip(_player as EntityPlayerLocal, _text, string.Empty, _alertSound, null, false, false, 0f);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageShowToolbeltMessage>().Setup(_text, _alertSound), false, _player.entityId, -1, -1, null, 192, false);
	}

	// Token: 0x060090ED RID: 37101 RVA: 0x0036A96A File Offset: 0x00368B6A
	public static void ShowTooltip(EntityPlayerLocal _player, string _text, bool _showImmediately = false, bool _pinTooltip = false, float _timeout = 0f)
	{
		GameManager.ShowTooltip(_player, _text, null, null, null, _showImmediately, _pinTooltip, _timeout);
	}

	// Token: 0x060090EE RID: 37102 RVA: 0x0036A97A File Offset: 0x00368B7A
	public static void ShowTooltip(EntityPlayerLocal _player, string _text, string _arg, string _alertSound = null, ToolTipEvent _handler = null, bool _showImmediately = false, bool _pinTooltip = false, float _timeout = 0f)
	{
		GameManager.ShowTooltip(_player, _text, new string[]
		{
			_arg
		}, _alertSound, _handler, _showImmediately, false, _timeout);
	}

	// Token: 0x060090EF RID: 37103 RVA: 0x0036A995 File Offset: 0x00368B95
	public static void ShowTooltip(EntityPlayerLocal _player, string _text, string[] _args, string _alertSound = null, ToolTipEvent _handler = null, bool _showImmediately = false, bool _pinTooltip = false, float _timeout = 0f)
	{
		if (GameManager.IsDedicatedServer || _player == null)
		{
			return;
		}
		XUiC_PopupToolTip.QueueTooltip(LocalPlayerUI.GetUIForPlayer(_player).nguiWindowManager.WindowManager.playerUI.xui, _text, _args, _alertSound, _handler, _showImmediately, _pinTooltip, _timeout);
	}

	// Token: 0x060090F0 RID: 37104 RVA: 0x0036A9D2 File Offset: 0x00368BD2
	public static void RemovePinnedTooltip(EntityPlayerLocal _player, string _key)
	{
		if (GameManager.IsDedicatedServer || _player == null)
		{
			return;
		}
		XUiC_PopupToolTip.RemovePinnedTooltip(LocalPlayerUI.GetUIForPlayer(_player).nguiWindowManager.WindowManager.playerUI.xui, _key);
	}

	// Token: 0x060090F1 RID: 37105 RVA: 0x0036AA05 File Offset: 0x00368C05
	public void ClearTooltips(NGUIWindowManager _nguiWindowManager)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		XUiC_PopupToolTip.ClearTooltips(_nguiWindowManager.WindowManager.playerUI.xui);
	}

	// Token: 0x060090F2 RID: 37106 RVA: 0x0036AA24 File Offset: 0x00368C24
	public void ClearCurrentTooltip(NGUIWindowManager _nguiWindowManager)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		XUiC_PopupToolTip.ClearCurrentTooltip(_nguiWindowManager.WindowManager.playerUI.xui);
	}

	// Token: 0x060090F3 RID: 37107 RVA: 0x0036AA43 File Offset: 0x00368C43
	public void SetToolTipPause(NGUIWindowManager _nguiWindowManager, bool _isPaused)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		XUiC_PopupToolTip.SetToolTipPause(_nguiWindowManager.WindowManager.playerUI.xui, _isPaused);
	}

	// Token: 0x060090F4 RID: 37108 RVA: 0x0036AA63 File Offset: 0x00368C63
	public static void ShowSubtitle(XUi _xui, string speaker, string content, float duration, bool centerAlign = false)
	{
		XUiC_SubtitlesDisplay.DisplaySubtitle(_xui, speaker, content, duration, centerAlign);
	}

	// Token: 0x060090F5 RID: 37109 RVA: 0x0036AA70 File Offset: 0x00368C70
	public static void PlayVideo(string id, bool skippable, XUiC_VideoPlayer.DelegateOnVideoFinished callback = null)
	{
		XUiC_VideoPlayer.PlayVideo(LocalPlayerUI.primaryUI.xui, VideoManager.GetVideoData(id), skippable, callback);
	}

	// Token: 0x060090F6 RID: 37110 RVA: 0x0036AA89 File Offset: 0x00368C89
	public static bool IsVideoPlaying()
	{
		return XUiC_VideoPlayer.IsVideoPlaying;
	}

	// Token: 0x060090F7 RID: 37111 RVA: 0x0036AA90 File Offset: 0x00368C90
	public void CheckDestroyTileEntity(ITileEntity _te, Vector3i _blockPos)
	{
		ITileEntityLootable tileEntityLootable = _te as ITileEntityLootable;
		if (tileEntityLootable == null)
		{
			return;
		}
		if (!tileEntityLootable.ShouldDestroyOnClose())
		{
			return;
		}
		BlockValue block = this.m_World.GetBlock(_blockPos);
		this.DropContentOfLootContainerServer(block, _blockPos, null);
		block.Block.DamageBlock(this.m_World, _blockPos, block, block.Block.MaxDamage, -1, null, false, false);
	}

	// Token: 0x060090F8 RID: 37112 RVA: 0x0036AAF4 File Offset: 0x00368CF4
	public void DropContentOfLootContainerServer(BlockValue _bvOld, Vector3i _worldPos, ITileEntityLootable _teOld = null)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Log.Warning("DropContentOfLootContainerServer can not be called on clients! From:\n" + StackTraceUtility.ExtractStackTrace());
			return;
		}
		string text = "DroppedLootContainer";
		ITileEntityLootable tileEntityLootable = _teOld ?? this.m_World.GetTileEntity(_worldPos);
		ITileEntityLootable tileEntityLootable2 = (tileEntityLootable != null) ? tileEntityLootable.GetSelfOrFeature<ITileEntityLootable>() : null;
		if (tileEntityLootable2 == null || LockManager.Instance.IsLockedServer(tileEntityLootable2, 0))
		{
			return;
		}
		ITileEntityLootable tileEntityLootable3 = tileEntityLootable2;
		Vector3 transformPos = tileEntityLootable3.ToWorldPos().ToVector3() + new Vector3(0.5f, 0.75f, 0.5f);
		if (_bvOld.Block.Properties.Values.ContainsKey("DroppedEntityClass"))
		{
			text = _bvOld.Block.Properties.Values["DroppedEntityClass"];
		}
		if (!tileEntityLootable3.bTouched)
		{
			this.lootManager.LootContainerOpened(tileEntityLootable3, -1, _bvOld.Block.Tags);
		}
		if (!tileEntityLootable3.IsEmpty())
		{
			EntityLootContainer entityLootContainer = EntityFactory.CreateEntity(text.GetHashCode(), transformPos, Vector3.zero) as EntityLootContainer;
			if (entityLootContainer != null)
			{
				entityLootContainer.SetContent(ItemStack.Clone(tileEntityLootable3.items));
			}
			this.m_World.SpawnEntityInWorld(entityLootContainer);
		}
		tileEntityLootable3.SetEmpty();
	}

	// Token: 0x060090F9 RID: 37113 RVA: 0x0036AC30 File Offset: 0x00368E30
	public void DropContentInLootContainerServer(int _droppedByID, string _containerEntity, Vector3 _pos, ItemStack[] _items, bool _skipIfEmpty = false, Vector3? increment = null)
	{
		List<EntityLootContainer> list = new List<EntityLootContainer>();
		if (_skipIfEmpty && ItemStack.IsEmpty(_items))
		{
			return;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageDropItemsContainer>().Setup(_droppedByID, _containerEntity, _pos, _items), false);
			return;
		}
		_pos.y += 0.25f;
		int hashCode = _containerEntity.GetHashCode();
		Vector2i size = LootContainer.GetLootContainer(EntityClass.list[hashCode].Properties.GetString(EntityClass.PropLootList), true).size;
		int val = size.x * size.y;
		int i = 0;
		int num = _items.Length;
		while (i < num)
		{
			EntityLootContainer entityLootContainer = EntityFactory.CreateEntity(_containerEntity.GetHashCode(), _pos, Vector3.zero) as EntityLootContainer;
			if (increment != null)
			{
				_pos += increment.Value;
			}
			if (entityLootContainer)
			{
				int val2 = num - i;
				int num2 = Math.Min(val, val2);
				entityLootContainer.SetContent(ItemStack.Clone(_items, i, num2));
				entityLootContainer.spawnById = _droppedByID;
				this.m_World.SpawnEntityInWorld(entityLootContainer);
				i += num2;
				list.Add(entityLootContainer);
			}
		}
	}

	// Token: 0x060090FA RID: 37114 RVA: 0x0036AD5A File Offset: 0x00368F5A
	public GameStateManager GetGameStateManager()
	{
		return this.gameStateManager;
	}

	// Token: 0x060090FB RID: 37115 RVA: 0x0036AD64 File Offset: 0x00368F64
	public void IdMappingReceived(string _name, byte[] _data)
	{
		Log.Out("Received mapping data for: " + _name);
		if (_name == "blocks")
		{
			Block.nameIdMapping = new NameIdMapping(null, Block.MAX_BLOCKS);
			Block.nameIdMapping.LoadFromArray(_data);
			return;
		}
		if (!(_name == "items"))
		{
			Log.Warning("Unknown mapping received for: " + _name);
			return;
		}
		ItemClass.nameIdMapping = new NameIdMapping(null, ItemClass.MAX_ITEMS);
		ItemClass.nameIdMapping.LoadFromArray(_data);
	}

	// Token: 0x060090FC RID: 37116 RVA: 0x0036ADE7 File Offset: 0x00368FE7
	public void SetSpawnPointList(SpawnPointList _startPoints)
	{
		base.StartCoroutine(this.setSpawnPointListCo(_startPoints));
	}

	// Token: 0x060090FD RID: 37117 RVA: 0x0036ADF7 File Offset: 0x00368FF7
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator setSpawnPointListCo(SpawnPointList _startPoints)
	{
		while (!this.chunkClusterLoaded && SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield return null;
		}
		if (!this.chunkClusterLoaded)
		{
			yield break;
		}
		this.m_World.ChunkCache.ChunkProvider.SetSpawnPointList(_startPoints);
		yield break;
	}

	// Token: 0x060090FE RID: 37118 RVA: 0x0036AE10 File Offset: 0x00369010
	public void RequestToSpawnEntityServer(EntityCreationData _ecd)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageRequestToSpawnEntity>().Setup(_ecd), false);
			return;
		}
		Entity entity = this.SpawnEntityServer(_ecd);
		if (_ecd.requestedBy != -1)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageConfirmSpawnEntity>().Setup(entity.entityId, _ecd.requestKey), true, _ecd.requestedBy, -1, -1, null, 192, false);
		}
	}

	// Token: 0x060090FF RID: 37119 RVA: 0x0036AE8C File Offset: 0x0036908C
	public Entity SpawnEntityServer(EntityCreationData _ecd)
	{
		if (_ecd.entityClass == "fallingTree".GetHashCode())
		{
			for (int i = 0; i < this.m_World.Entities.list.Count; i++)
			{
				if (this.m_World.Entities.list[i] is EntityFallingTree && ((EntityFallingTree)this.m_World.Entities.list[i]).GetBlockPos() == _ecd.blockPos)
				{
					return null;
				}
			}
		}
		Entity entity = EntityFactory.CreateEntity(_ecd);
		EntityBackpack entityBackpack = entity as EntityBackpack;
		if (entityBackpack != null)
		{
			foreach (PersistentPlayerData persistentPlayerData in this.persistentPlayers.Players.Values)
			{
				if (persistentPlayerData.EntityId == entityBackpack.RefPlayerId)
				{
					uint timestamp = GameUtils.WorldTimeToTotalMinutes(this.m_World.worldTime);
					persistentPlayerData.AddDroppedBackpack(entity.entityId, new Vector3i(_ecd.pos), timestamp);
					break;
				}
			}
		}
		this.m_World.SpawnEntityInWorld(entity);
		return entity;
	}

	// Token: 0x06009100 RID: 37120 RVA: 0x0036AFB8 File Offset: 0x003691B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void LocalPlayerInventoryChanged()
	{
		this.countdownSendPlayerInventoryToServer.ResetAndRestart();
	}

	// Token: 0x06009101 RID: 37121 RVA: 0x0036AFC5 File Offset: 0x003691C5
	public void TriggerSendOfLocalPlayerDataFile(float _sendItInSeconds)
	{
		this.countdownSendPlayerDataFileToServer.SetPassedIn(_sendItInSeconds);
	}

	// Token: 0x06009102 RID: 37122 RVA: 0x0036AFD4 File Offset: 0x003691D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void doSendLocalInventory(EntityPlayerLocal _player)
	{
		if (!(this.sendPlayerToolbelt | this.sendPlayerBag | this.sendPlayerEquipment | this.sendDragAndDropItem))
		{
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerInventory>().Setup(_player, this.sendPlayerToolbelt, this.sendPlayerBag, this.sendPlayerEquipment, this.sendDragAndDropItem), false);
		this.sendPlayerToolbelt = false;
		this.sendPlayerBag = false;
		this.sendPlayerEquipment = false;
		this.sendDragAndDropItem = false;
	}

	// Token: 0x06009103 RID: 37123 RVA: 0x0036B04C File Offset: 0x0036924C
	[PublicizedFrom(EAccessModifier.Private)]
	public void doSendLocalPlayerData(EntityPlayerLocal _player)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SaveLocalPlayerData();
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerData>().Setup(_player), false);
		this.sendPlayerToolbelt = false;
		this.sendPlayerBag = false;
		this.sendPlayerEquipment = false;
		this.sendDragAndDropItem = false;
	}

	// Token: 0x06009104 RID: 37124 RVA: 0x0036B0A0 File Offset: 0x003692A0
	public void SetPauseWindowEffects(bool _bOn)
	{
		if (_bOn && !GameModeSurvivalSP.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)))
		{
			foreach (EntityPlayerLocal entityPlayerLocal in this.m_World.GetLocalPlayers())
			{
				if (entityPlayerLocal.AimingGun)
				{
					entityPlayerLocal.AimingGun = false;
				}
			}
		}
	}

	// Token: 0x06009105 RID: 37125 RVA: 0x0036B118 File Offset: 0x00369318
	public static bool ReportUnusedAssets(bool bStart = false)
	{
		if (bStart)
		{
			if (GameManager.materialsBefore == null)
			{
				GameManager.materialsBefore = new List<string>();
			}
			else
			{
				GameManager.materialsBefore.Clear();
			}
			Material[] array = Resources.FindObjectsOfTypeAll<Material>();
			for (int i = 0; i < array.Length; i++)
			{
				GameManager.materialsBefore.Add(array[i].name);
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
			GameManager.runningAssetsUnused = true;
			GameManager.unusedAssetsTimer = Time.realtimeSinceStartup;
			GameManager.Instance.Pause(true);
		}
		else
		{
			if (GameManager.materialsBefore == null)
			{
				return true;
			}
			if (!GameManager.runningAssetsUnused)
			{
				return true;
			}
			if (Time.realtimeSinceStartup < GameManager.unusedAssetsTimer + 5f)
			{
				return false;
			}
			Material[] array2 = Resources.FindObjectsOfTypeAll<Material>();
			if (GameManager.materialsBefore.Count == array2.Length)
			{
				Log.Out("No unused assets found. ( " + GameManager.materialsBefore.Count.ToString() + " materials found. )");
			}
			else
			{
				Log.Out("Material before: " + GameManager.materialsBefore.Count.ToString());
				Log.Out("Material after: " + array2.Length.ToString());
				string text = "Material Diff: ";
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				for (int j = 0; j < array2.Length; j++)
				{
					int num;
					if (dictionary.TryGetValue(array2[j].name, out num))
					{
						num++;
					}
					else
					{
						dictionary.Add(array2[j].name, 1);
					}
				}
				for (int k = 0; k < GameManager.materialsBefore.Count; k++)
				{
					if (!dictionary.ContainsKey(GameManager.materialsBefore[k]))
					{
						text = text + GameManager.materialsBefore[k] + ", ";
					}
				}
				Log.Out(text);
			}
			GameManager.Instance.Pause(false);
			GameManager.runningAssetsUnused = false;
		}
		return true;
	}

	// Token: 0x06009106 RID: 37126 RVA: 0x0036B2E5 File Offset: 0x003694E5
	public bool IsPaused()
	{
		return this.gamePaused;
	}

	// Token: 0x06009107 RID: 37127 RVA: 0x0036B2ED File Offset: 0x003694ED
	public void Pause(bool _bOn)
	{
		this.requestedPauseState = new bool?(_bOn);
	}

	// Token: 0x06009108 RID: 37128 RVA: 0x0036B2FC File Offset: 0x003694FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePauseState()
	{
		if (this.requestedPauseState == null)
		{
			return;
		}
		bool flag = this.requestedPauseState.Value;
		this.requestedPauseState = null;
		if (flag == this.gamePaused)
		{
			return;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsSinglePlayer || GameModeEditWorld.TypeName.Equals(GamePrefs.GetString(EnumGamePrefs.GameMode)) || GameStats.GetInt(EnumGameStats.GameState) == 0)
		{
			flag = false;
		}
		this.SetPauseWindowEffects(flag);
		if (flag)
		{
			GameStats.Set(EnumGameStats.GameState, 2);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.SaveLocalPlayerData();
				this.SaveWorld();
			}
			Time.timeScale = 0f;
			if (this.World.GetPrimaryPlayer() != null)
			{
				this.triggerEffectManager.StopGamepadVibration(false);
			}
		}
		else
		{
			if (GameStats.GetInt(EnumGameStats.GameState) != 0)
			{
				GameStats.Set(EnumGameStats.GameState, 1);
			}
			Time.timeScale = 1f;
		}
		if (this.gamePaused != flag && this.World != null)
		{
			if (flag)
			{
				Manager.PauseGameplayAudio();
				EnvironmentAudioManager.Instance.Pause();
				this.m_World.dmsConductor.OnPauseGame();
			}
			else
			{
				Manager.UnPauseGameplayAudio();
				EnvironmentAudioManager.Instance.UnPause();
				this.m_World.dmsConductor.OnUnPauseGame();
			}
		}
		this.gamePaused = flag;
	}

	// Token: 0x06009109 RID: 37129 RVA: 0x000027FC File Offset: 0x000009FC
	public void AddLMPPersistentPlayerData(EntityPlayerLocal _playerEntity)
	{
	}

	// Token: 0x0600910A RID: 37130 RVA: 0x0036B42C File Offset: 0x0036962C
	public void SetBlockTextureServer(BlockValueRef _bvRef, BlockFace _blockFace, int _idx, int _playerIdThatChanged, byte _channel = 255)
	{
		this.SetBlockTextureClient(_bvRef, _blockFace, _idx, _channel);
		NetPackageSetBlockTexture package = NetPackageManager.GetPackage<NetPackageSetBlockTexture>().Setup(_bvRef, _blockFace, _idx, GameManager.IsDedicatedServer ? -1 : this.myPlayerId, _channel);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x0600910B RID: 37131 RVA: 0x0036B4A0 File Offset: 0x003696A0
	public void SetBlockTextureClient(BlockValueRef _bvRef, BlockFace _blockFace, int _idx, byte _channel)
	{
		DynamicMeshManager.ChunkChanged(_bvRef, -1, 1);
		int num;
		int num2;
		if (_channel == 255)
		{
			num = 0;
			num2 = 0;
		}
		else
		{
			if (_channel >= 1)
			{
				Log.Error(string.Format("Specified texture channel \"{0}\" is out of range of the project channel count of \"{1}\".", _channel, 1));
				return;
			}
			num2 = (int)_channel;
			num = (int)_channel;
		}
		for (int i = num; i <= num2; i++)
		{
			if (_blockFace != BlockFace.None)
			{
				this.m_World.ChunkCache.SetBlockFaceTexture(_bvRef, _blockFace, _idx, i);
			}
			else
			{
				long num3 = (long)_idx;
				long textureFull = num3 | num3 << 8 | num3 << 16 | num3 << 24 | num3 << 32 | num3 << 40;
				this.m_World.ChunkCache.SetTextureFull(_bvRef, textureFull, i);
			}
		}
	}

	// Token: 0x0600910C RID: 37132 RVA: 0x0036B550 File Offset: 0x00369750
	[PublicizedFrom(EAccessModifier.Private)]
	public void handleGlobalActions()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (PlayerActionsGlobal.Instance.Console.WasPressed)
		{
			GUIWindowConsole.Open();
		}
		if (PlayerActionsGlobal.Instance.Fullscreen.WasPressed)
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
		if (PlayerActionsGlobal.Instance.Screenshot.WasPressed)
		{
			Manager.PlayButtonClick();
			GameUtils.TakeScreenShot(GameUtils.EScreenshotMode.Both, null, 0f, false, 0, 0, InputUtils.ControlKeyPressed);
		}
		if (PlayerActionsGlobal.Instance.DebugScreenshot.WasPressed)
		{
			Manager.PlayButtonClick();
			LocalPlayerUI.primaryUI.windowManager.Open(GUIWindowScreenshotText.ID, false);
		}
		if (LocalPlayerUI.primaryUI != null)
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			bool? flag;
			if (nativePlatform == null)
			{
				flag = null;
			}
			else
			{
				PlayerActionsLocal primaryPlayer = nativePlatform.Input.PrimaryPlayer;
				if (primaryPlayer == null)
				{
					flag = null;
				}
				else
				{
					PlayerActionsGUI guiactions = primaryPlayer.GUIActions;
					if (guiactions == null)
					{
						flag = null;
					}
					else
					{
						PlayerAction focusSearch = guiactions.FocusSearch;
						flag = ((focusSearch != null) ? new bool?(focusSearch.WasPressed) : null);
					}
				}
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				XUiC_TextInput.SelectCurrentSearchField(LocalPlayerUI.primaryUI);
			}
		}
		LocalPlayerUI uiforPrimaryPlayer = LocalPlayerUI.GetUIForPrimaryPlayer();
		if (uiforPrimaryPlayer != null)
		{
			PlayerActionsLocal playerInput = uiforPrimaryPlayer.playerInput;
			bool? flag3;
			if (playerInput == null)
			{
				flag3 = null;
			}
			else
			{
				PlayerActionsGUI guiactions2 = playerInput.GUIActions;
				if (guiactions2 == null)
				{
					flag3 = null;
				}
				else
				{
					PlayerAction focusSearch2 = guiactions2.FocusSearch;
					flag3 = ((focusSearch2 != null) ? new bool?(focusSearch2.WasPressed) : null);
				}
			}
			bool? flag2 = flag3;
			if (flag2.GetValueOrDefault())
			{
				XUiC_TextInput.SelectCurrentSearchField(uiforPrimaryPlayer);
			}
		}
	}

	// Token: 0x0600910D RID: 37133 RVA: 0x0036B6D4 File Offset: 0x003698D4
	public static bool IsSplatMapAvailable()
	{
		string @string = GamePrefs.GetString(EnumGamePrefs.GameWorld);
		return !(@string == "Empty") && !(@string == "Playtesting");
	}

	// Token: 0x1700114C RID: 4428
	// (get) Token: 0x0600910E RID: 37134 RVA: 0x0036B706 File Offset: 0x00369906
	// (set) Token: 0x0600910F RID: 37135 RVA: 0x0036B70D File Offset: 0x0036990D
	public static bool UpdatingRemoteResources { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x1700114D RID: 4429
	// (get) Token: 0x06009110 RID: 37136 RVA: 0x0036B715 File Offset: 0x00369915
	// (set) Token: 0x06009111 RID: 37137 RVA: 0x0036B71C File Offset: 0x0036991C
	public static bool RemoteResourcesLoaded { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06009112 RID: 37138 RVA: 0x0036B724 File Offset: 0x00369924
	public static void LoadRemoteResources(GameManager.RemoteResourcesCompleteHandler _callback = null)
	{
		if (GameManager.UpdatingRemoteResources)
		{
			return;
		}
		NewsManager.Instance.UpdateNews(false);
		if (BlockedPlayerList.Instance != null)
		{
			GameManager.Instance.StartCoroutine(BlockedPlayerList.Instance.ReadStorageAndResolve());
		}
		DLCTitleStorageManager.Instance.FetchFromSource();
		if (PlatformManager.NativePlatform.User.UserStatus == EUserStatus.LoggedIn)
		{
			GameManager.Instance.StartCoroutine(GameManager.Instance.UpdateRemoteResourcesRoutine(_callback));
			return;
		}
		GameManager.UpdatingRemoteResources = false;
		GameManager.RemoteResourcesLoaded = true;
	}

	// Token: 0x06009113 RID: 37139 RVA: 0x0036B79E File Offset: 0x0036999E
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator UpdateRemoteResourcesRoutine(GameManager.RemoteResourcesCompleteHandler _callback)
	{
		IRemoteFileStorage storage = PlatformManager.MultiPlatform.RemoteFileStorage;
		if (storage == null)
		{
			GameManager.RemoteResourcesLoaded = true;
			yield break;
		}
		GameManager.UpdatingRemoteResources = true;
		float readyTime = Time.time;
		while (!storage.IsReady)
		{
			yield return null;
			if (Time.time - readyTime > 3f)
			{
				Log.Warning("Waiting for remote resources timed out");
				GameManager.UpdatingRemoteResources = false;
				GameManager.RemoteResourcesLoaded = true;
				yield break;
			}
		}
		this.retrievingEula = true;
		float getFileStart = Time.time;
		string eulaFileName = string.Format("eula_{0}", Localization.ActiveLanguage.ToLower());
		storage.GetFile(eulaFileName, new IRemoteFileStorage.FileDownloadCompleteCallback(this.EulaProviderCallback));
		while (this.retrievingEula)
		{
			if (Time.time - getFileStart > 5f)
			{
				Log.Warning("Waiting for EULA retrieve timed out.");
				if (!storage.CancelGetFile(eulaFileName, "Timed Out"))
				{
					Log.Warning("EULA CancelGetFile call failed.");
				}
			}
			yield return null;
		}
		if (BacktraceUtils.Initialized)
		{
			this.retrievingBacktraceConfig = true;
			getFileStart = Time.time;
			storage.GetFile("backtraceconfig.xml", new IRemoteFileStorage.FileDownloadCompleteCallback(this.BacktraceConfigProviderCallback));
			while (this.retrievingBacktraceConfig)
			{
				if (Time.time - getFileStart > 5f)
				{
					Log.Warning("Waiting for Backtrace Config timed out.");
					if (!storage.CancelGetFile("backtraceconfig.xml", "Timed Out"))
					{
						Log.Warning("Backtrace Config CancelGetFile call failed.");
					}
				}
				yield return null;
			}
		}
		GameManager.UpdatingRemoteResources = false;
		GameManager.RemoteResourcesLoaded = true;
		if (_callback != null)
		{
			_callback();
		}
		yield break;
	}

	// Token: 0x06009114 RID: 37140 RVA: 0x0036B7B4 File Offset: 0x003699B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void EulaProviderCallback(IRemoteFileStorage.EFileDownloadResult _result, string _errorDetails, byte[] _data)
	{
		this.retrievingEula = false;
		if (_result != IRemoteFileStorage.EFileDownloadResult.Ok)
		{
			Log.Warning(string.Concat(new string[]
			{
				"Retrieving EULA file failed: ",
				_result.ToStringCached<IRemoteFileStorage.EFileDownloadResult>(),
				" (",
				_errorDetails,
				")"
			}));
			return;
		}
		string retrievedEula;
		if (this.LoadEulaXML(_data, out retrievedEula))
		{
			XUiC_EulaWindow.RetrievedEula = retrievedEula;
		}
	}

	// Token: 0x06009115 RID: 37141 RVA: 0x0036B814 File Offset: 0x00369A14
	[PublicizedFrom(EAccessModifier.Private)]
	public bool LoadEulaXML(byte[] _data, out string contents)
	{
		contents = "";
		XmlFile xmlFile;
		try
		{
			xmlFile = new XmlFile(_data, true);
		}
		catch (Exception ex)
		{
			Log.Error("Failed loading EULA XML: {0}", new object[]
			{
				ex.Message
			});
			return false;
		}
		XElement root = xmlFile.XmlDoc.Root;
		if (root == null)
		{
			return false;
		}
		int num = int.Parse(root.GetAttribute("version").Trim());
		contents = root.Value;
		if (num > GamePrefs.GetInt(EnumGamePrefs.EulaLatestVersion))
		{
			GamePrefs.Set(EnumGamePrefs.EulaLatestVersion, num);
		}
		return true;
	}

	// Token: 0x06009116 RID: 37142 RVA: 0x0036B8B4 File Offset: 0x00369AB4
	public static bool HasAcceptedLatestEula()
	{
		return GamePrefs.GetInt(EnumGamePrefs.EulaVersionAccepted) >= GamePrefs.GetInt(EnumGamePrefs.EulaLatestVersion);
	}

	// Token: 0x06009117 RID: 37143 RVA: 0x0036B8D0 File Offset: 0x00369AD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BacktraceConfigProviderCallback(IRemoteFileStorage.EFileDownloadResult _result, string _errorDetails, byte[] _data)
	{
		this.retrievingBacktraceConfig = false;
		if (_result != IRemoteFileStorage.EFileDownloadResult.Ok)
		{
			Log.Warning(string.Concat(new string[]
			{
				"Retrieving Backtrace config file failed: ",
				_result.ToStringCached<IRemoteFileStorage.EFileDownloadResult>(),
				" (",
				_errorDetails,
				")"
			}));
			return;
		}
		try
		{
			BacktraceUtils.UpdateConfig(new XmlFile(_data, true));
		}
		catch (Exception ex)
		{
			Log.Error("Failed loading Backtrace config XML: {0}", new object[]
			{
				ex.Message
			});
		}
	}

	// Token: 0x06009118 RID: 37144 RVA: 0x0036B958 File Offset: 0x00369B58
	public bool IsGoreCensored()
	{
		return GameManager.DebugCensorship;
	}

	// Token: 0x1700114E RID: 4430
	// (get) Token: 0x06009119 RID: 37145 RVA: 0x0036B964 File Offset: 0x00369B64
	public int persistentPlayerCount
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.persistentPlayerIds.Count;
		}
	}

	// Token: 0x0600911A RID: 37146 RVA: 0x0036B974 File Offset: 0x00369B74
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalculatePersistentPlayerCount(string worldName, string saveName, UserDataStorageType storage)
	{
		this.persistentPlayerIds = new List<string>();
		string path = GameIO.GetSaveGameDir(worldName, saveName, storage) + "/Player";
		if (!SdDirectory.Exists(path))
		{
			Log.Warning("save folder does not exist");
			return;
		}
		foreach (SdFileSystemInfo sdFileSystemInfo in new SdDirectoryInfo(path).GetFileSystemInfos())
		{
			int length;
			string item;
			if ((length = sdFileSystemInfo.Name.IndexOf('.')) != -1)
			{
				item = sdFileSystemInfo.Name.Substring(0, length);
			}
			else
			{
				item = sdFileSystemInfo.Name;
			}
			if (!this.persistentPlayerIds.Contains(item))
			{
				this.persistentPlayerIds.Add(item);
			}
		}
	}

	// Token: 0x0600911B RID: 37147 RVA: 0x0036BA1C File Offset: 0x00369C1C
	public void OnResolutionChanged(int width, int height)
	{
		this.RefreshRefreshRate();
	}

	// Token: 0x0600911C RID: 37148 RVA: 0x0036BA24 File Offset: 0x00369C24
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshRefreshRate()
	{
		this.currentRefreshRate = (int)PlatformApplicationManager.Application.GetCurrentRefreshRate().value;
	}

	// Token: 0x0600911D RID: 37149 RVA: 0x0036BA4C File Offset: 0x00369C4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateFPSCap()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		int num;
		if (GamePrefs.GetInt(PlatformApplicationManager.Application.VSyncCountPref) != 0)
		{
			num = -1;
		}
		else if (this.GameHasStarted)
		{
			int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxLimitFpsInGame);
			num = ((@int <= 0) ? -1 : @int);
		}
		else
		{
			num = this.currentRefreshRate;
		}
		if (Application.targetFrameRate != num)
		{
			Application.targetFrameRate = num;
		}
	}

	// Token: 0x0600911E RID: 37150 RVA: 0x0036BAA9 File Offset: 0x00369CA9
	public void OnAnalyticsSessionRefreshed()
	{
		base.StartCoroutine(Helper.LoginEventAnalyticCoroutine(false));
	}

	// Token: 0x04006AD3 RID: 27347
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cMinSpawnDistanceFromTrader = 250;

	// Token: 0x04006AD4 RID: 27348
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cMaxSpawnDistanceFromTrader = 750;

	// Token: 0x04006AD5 RID: 27349
	public static int frameCount;

	// Token: 0x04006AD6 RID: 27350
	public static float frameTime;

	// Token: 0x04006AD7 RID: 27351
	public static int fixedUpdateCount;

	// Token: 0x04006AD8 RID: 27352
	public AudioSource UIAudioSource;

	// Token: 0x04006AD9 RID: 27353
	public AudioClip BackgroundMusicClip;

	// Token: 0x04006ADA RID: 27354
	public AudioClip CreditsSongClip;

	// Token: 0x04006ADB RID: 27355
	public bool DebugAILines;

	// Token: 0x04006ADC RID: 27356
	public bool DebugAIAim;

	// Token: 0x04006ADD RID: 27357
	public StabilityViewer stabilityViewer;

	// Token: 0x04006ADE RID: 27358
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BiomeParticleManager biomeParticleManager;

	// Token: 0x04006ADF RID: 27359
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int cameraCullMask;

	// Token: 0x04006AE0 RID: 27360
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bShowBackground = true;

	// Token: 0x04006AE1 RID: 27361
	public static bool enableNetworkdPrioritization = true;

	// Token: 0x04006AE2 RID: 27362
	public static bool unreliableNetPackets = true;

	// Token: 0x04006AE3 RID: 27363
	public static ServerDateTimeResult ServerClockSync;

	// Token: 0x04006AE4 RID: 27364
	public NetPackageMetrics netpackageMetrics;

	// Token: 0x04006AE5 RID: 27365
	public bool showOpenerMovieOnLoad;

	// Token: 0x04006AE6 RID: 27366
	public bool GameHasStarted;

	// Token: 0x04006AE7 RID: 27367
	public RuntimeAnimatorController FirstPersonWeaponAnimatorController;

	// Token: 0x04006AE8 RID: 27368
	public RuntimeAnimatorController ThirdPersonWeaponAnimatorController;

	// Token: 0x04006AE9 RID: 27369
	public Color backgroundColor = Color.white;

	// Token: 0x04006AEA RID: 27370
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int currentBackgroundColorChannel;

	// Token: 0x04006AEB RID: 27371
	public static bool bPhysicsActive;

	// Token: 0x04006AEC RID: 27372
	public static bool bTickingActive;

	// Token: 0x04006AED RID: 27373
	public static bool bSavingActive = true;

	// Token: 0x04006AEE RID: 27374
	public static bool bShowDecorBlocks = true;

	// Token: 0x04006AEF RID: 27375
	public static bool bShowLootBlocks = true;

	// Token: 0x04006AF0 RID: 27376
	public static bool bShowPaintables = true;

	// Token: 0x04006AF1 RID: 27377
	public static bool bShowUnpaintables = true;

	// Token: 0x04006AF2 RID: 27378
	public static bool bShowTerrain = true;

	// Token: 0x04006AF3 RID: 27379
	public static bool bVolumeBlocksEditing;

	// Token: 0x04006AF4 RID: 27380
	public static bool bHideMainMenuNextTime;

	// Token: 0x04006AF5 RID: 27381
	public static bool bRecordNextSession;

	// Token: 0x04006AF6 RID: 27382
	public static bool bPlayRecordedSession;

	// Token: 0x04006AF7 RID: 27383
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool isDedicatedChecked = false;

	// Token: 0x04006AF8 RID: 27384
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool isDedicated = false;

	// Token: 0x04006AF9 RID: 27385
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public World m_World;

	// Token: 0x04006AFA RID: 27386
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool worldCreated;

	// Token: 0x04006AFB RID: 27387
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool chunkClusterLoaded;

	// Token: 0x04006AFC RID: 27388
	public bool worldInitInfoReceived;

	// Token: 0x04006AFD RID: 27389
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int myPlayerId = -1;

	// Token: 0x04006AFE RID: 27390
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal myEntityPlayerLocal;

	// Token: 0x04006AFF RID: 27391
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IMapChunkDatabase fowDatabaseForLocalPlayer;

	// Token: 0x04006B00 RID: 27392
	public FPS fps = new FPS(5f);

	// Token: 0x04006B01 RID: 27393
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject m_SoundsGameObject;

	// Token: 0x04006B02 RID: 27394
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeWorldTickTimeSentToClients;

	// Token: 0x04006B03 RID: 27395
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeGameStateCheckedAndSynced;

	// Token: 0x04006B04 RID: 27396
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeDecoSaved;

	// Token: 0x04006B05 RID: 27397
	public AdminTools adminTools;

	// Token: 0x04006B06 RID: 27398
	public PersistentPlayerList persistentPlayers;

	// Token: 0x04006B07 RID: 27399
	public PersistentPlayerData persistentLocalPlayer;

	// Token: 0x04006B08 RID: 27400
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GUIWindowManager windowManager;

	// Token: 0x04006B09 RID: 27401
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public NGUIWindowManager nguiWindowManager;

	// Token: 0x04006B0A RID: 27402
	public LootManager lootManager;

	// Token: 0x04006B0B RID: 27403
	public TraderManager traderManager;

	// Token: 0x04006B0C RID: 27404
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastDisplayedValueOfTeamTickets;

	// Token: 0x04006B0D RID: 27405
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Vector3i, GameObject> m_PositionSoundMap = new Dictionary<Vector3i, GameObject>();

	// Token: 0x04006B0E RID: 27406
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameObject> tileEntitiesMusicToRemove = new List<GameObject>();

	// Token: 0x04006B0F RID: 27407
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int msPassedSinceLastUpdate;

	// Token: 0x04006B10 RID: 27408
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IBlockTool activeBlockTool;

	// Token: 0x04006B11 RID: 27409
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IBlockTool blockSelectionTool;

	// Token: 0x04006B12 RID: 27410
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isEditMode;

	// Token: 0x04006B14 RID: 27412
	public bool bCursorVisible = true;

	// Token: 0x04006B15 RID: 27413
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bCursorVisibleOverride;

	// Token: 0x04006B16 RID: 27414
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bCursorVisibleOverrideState;

	// Token: 0x04006B17 RID: 27415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DictionarySave<Vector3i, Transform> m_BlockParticles = new DictionarySave<Vector3i, Transform>();

	// Token: 0x04006B18 RID: 27416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CountdownTimer countdownSendPlayerDataFileToServer = new CountdownTimer(30f, true);

	// Token: 0x04006B19 RID: 27417
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CountdownTimer countdownSaveLocalPlayerDataFile = new CountdownTimer(30f, true);

	// Token: 0x04006B1A RID: 27418
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float unloadAssetsDuration;

	// Token: 0x04006B1B RID: 27419
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isUnloadAssetsReady;

	// Token: 0x04006B1C RID: 27420
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly MicroStopwatch stopwatchUnloadAssets = new MicroStopwatch(false);

	// Token: 0x04006B1D RID: 27421
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CountdownTimer countdownSendPlayerInventoryToServer = new CountdownTimer(0.1f, false);

	// Token: 0x04006B1E RID: 27422
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool sendPlayerToolbelt;

	// Token: 0x04006B1F RID: 27423
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool sendPlayerBag;

	// Token: 0x04006B20 RID: 27424
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool sendPlayerEquipment;

	// Token: 0x04006B21 RID: 27425
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool sendDragAndDropItem;

	// Token: 0x04006B22 RID: 27426
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameRandomManager gameRandomManager;

	// Token: 0x04006B23 RID: 27427
	public GameStateManager gameStateManager;

	// Token: 0x04006B24 RID: 27428
	public PrefabLODManager prefabLODManager;

	// Token: 0x04006B25 RID: 27429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public RespawnType clientRespawnType;

	// Token: 0x04006B26 RID: 27430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fpsCountdownTimer = 30f;

	// Token: 0x04006B27 RID: 27431
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float gcCountdownTimer = 120f;

	// Token: 0x04006B28 RID: 27432
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float wsCountdownTimer = 30f;

	// Token: 0x04006B29 RID: 27433
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float playerPositionsCountdownTimer = 10f;

	// Token: 0x04006B2A RID: 27434
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MicroStopwatch swCopyChunks = new MicroStopwatch();

	// Token: 0x04006B2B RID: 27435
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MicroStopwatch swUpdateTime = new MicroStopwatch();

	// Token: 0x04006B2C RID: 27436
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastStatsPlayerCount;

	// Token: 0x04006B2D RID: 27437
	public static long MaxMemoryConsumption;

	// Token: 0x04006B2E RID: 27438
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameManager.BlockParticleCreationData> blockParticlesToSpawn = new List<GameManager.BlockParticleCreationData>();

	// Token: 0x04006B2F RID: 27439
	public static GameManager Instance;

	// Token: 0x04006B33 RID: 27443
	public TriggerEffectManager triggerEffectManager;

	// Token: 0x04006B34 RID: 27444
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastPlayerCount;

	// Token: 0x04006B35 RID: 27445
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int currentRefreshRate = 60;

	// Token: 0x04006B36 RID: 27446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public IAnalyticsService _analyticsService;

	// Token: 0x04006B37 RID: 27447
	public bool bStaticDataLoadSync;

	// Token: 0x04006B38 RID: 27448
	public bool bStaticDataLoaded;

	// Token: 0x04006B39 RID: 27449
	public string CurrentLoadAction;

	// Token: 0x04006B3B RID: 27451
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lastTimeAbsPosSentToServer;

	// Token: 0x04006B3C RID: 27452
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bLastWasAttached;

	// Token: 0x04006B3D RID: 27453
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTime;

	// Token: 0x04006B3E RID: 27454
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeToClearAllPools = -1f;

	// Token: 0x04006B3F RID: 27455
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float activityCheck;

	// Token: 0x04006B40 RID: 27456
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool shuttingDownMultiplayerServices;

	// Token: 0x04006B41 RID: 27457
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int testing;

	// Token: 0x04006B42 RID: 27458
	public bool canSpawnPlayer;

	// Token: 0x04006B43 RID: 27459
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDisconnectingLater;

	// Token: 0x04006B44 RID: 27460
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool allowQuit;

	// Token: 0x04006B45 RID: 27461
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isQuitting;

	// Token: 0x04006B46 RID: 27462
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool firstTimeJoin;

	// Token: 0x04006B47 RID: 27463
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<BlockChangeInfo> tempExplPositions = new List<BlockChangeInfo>();

	// Token: 0x04006B48 RID: 27464
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameManager.ExplodeGroup> explodeFallingGroups = new List<GameManager.ExplodeGroup>();

	// Token: 0x04006B49 RID: 27465
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<ChunkCluster> ccChanged = new List<ChunkCluster>();

	// Token: 0x04006B4A RID: 27466
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static List<string> materialsBefore = null;

	// Token: 0x04006B4B RID: 27467
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float unusedAssetsTimer = 0f;

	// Token: 0x04006B4C RID: 27468
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool runningAssetsUnused = false;

	// Token: 0x04006B4D RID: 27469
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool? requestedPauseState;

	// Token: 0x04006B4E RID: 27470
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool gamePaused;

	// Token: 0x04006B51 RID: 27473
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool retrievingEula;

	// Token: 0x04006B52 RID: 27474
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool retrievingBacktraceConfig;

	// Token: 0x04006B53 RID: 27475
	public static bool DebugCensorship;

	// Token: 0x04006B54 RID: 27476
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<string> persistentPlayerIds;

	// Token: 0x020011A8 RID: 4520
	[PublicizedFrom(EAccessModifier.Private)]
	public struct BlockParticleCreationData
	{
		// Token: 0x06009127 RID: 37159 RVA: 0x0036BC8D File Offset: 0x00369E8D
		public BlockParticleCreationData(Vector3i _blockPos, ParticleEffect _particleEffect)
		{
			this.blockPos = _blockPos;
			this.particleEffect = _particleEffect;
		}

		// Token: 0x04006B55 RID: 27477
		public Vector3i blockPos;

		// Token: 0x04006B56 RID: 27478
		public ParticleEffect particleEffect;
	}

	// Token: 0x020011A9 RID: 4521
	// (Invoke) Token: 0x06009129 RID: 37161
	public delegate void OnWorldChangedEvent(World _world);

	// Token: 0x020011AA RID: 4522
	// (Invoke) Token: 0x0600912D RID: 37165
	public delegate void OnLocalPlayerChangedEvent(EntityPlayerLocal _localPlayer);

	// Token: 0x020011AB RID: 4523
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EMultiShutReason
	{
		// Token: 0x04006B58 RID: 27480
		AppNoNetwork,
		// Token: 0x04006B59 RID: 27481
		AppSuspended,
		// Token: 0x04006B5A RID: 27482
		PermMissingMultiplayer,
		// Token: 0x04006B5B RID: 27483
		PermMissingCrossplay
	}

	// Token: 0x020011AC RID: 4524
	[PublicizedFrom(EAccessModifier.Private)]
	public class ExplodeGroup
	{
		// Token: 0x04006B5C RID: 27484
		public Vector3 pos;

		// Token: 0x04006B5D RID: 27485
		public float radius;

		// Token: 0x04006B5E RID: 27486
		public int delay;

		// Token: 0x04006B5F RID: 27487
		public List<GameManager.ExplodeGroup.Falling> fallings = new List<GameManager.ExplodeGroup.Falling>();

		// Token: 0x020011AD RID: 4525
		public struct Falling
		{
			// Token: 0x04006B60 RID: 27488
			public Vector3i pos;

			// Token: 0x04006B61 RID: 27489
			public BlockValue bv;
		}
	}

	// Token: 0x020011AE RID: 4526
	public class EntityItemLifetimeComparer : IComparer<EntityItem>
	{
		// Token: 0x06009131 RID: 37169 RVA: 0x0036BCB0 File Offset: 0x00369EB0
		public int Compare(EntityItem _obj1, EntityItem _obj2)
		{
			return (int)(_obj2.lifetime - _obj1.lifetime);
		}
	}

	// Token: 0x020011AF RID: 4527
	// (Invoke) Token: 0x06009134 RID: 37172
	public delegate void RemoteResourcesCompleteHandler();
}
