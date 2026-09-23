using System;
using System.Collections;
using System.Collections.Generic;
using Platform;
using Twitch;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020011F5 RID: 4597
public class GameSparksManager : MonoBehaviour
{
	// Token: 0x17001183 RID: 4483
	// (get) Token: 0x060092E4 RID: 37604 RVA: 0x00377D5E File Offset: 0x00375F5E
	public int sessionUpdateIntervalSec
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return (this.DebugEnabled ? 2 : 15) * 60;
		}
	}

	// Token: 0x060092E5 RID: 37605 RVA: 0x00377D70 File Offset: 0x00375F70
	public static GameSparksManager Instance()
	{
		GameSparksManager.instance != null;
		return GameSparksManager.instance;
	}

	// Token: 0x17001184 RID: 4484
	// (get) Token: 0x060092E6 RID: 37606 RVA: 0x00010E62 File Offset: 0x0000F062
	public bool DebugEnabled
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return false;
		}
	}

	// Token: 0x17001185 RID: 4485
	// (get) Token: 0x060092E7 RID: 37607 RVA: 0x00032163 File Offset: 0x00030363
	public string DeviceId
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "";
		}
	}

	// Token: 0x060092E8 RID: 37608 RVA: 0x00148373 File Offset: 0x00146573
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x060092E9 RID: 37609 RVA: 0x00377D83 File Offset: 0x00375F83
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator Start()
	{
		yield return new WaitForSeconds(1f);
		this.DeviceAuth(new GameSparksManager.OnPlayerAuthenticated(this.PlayerAuthenticated), new GameSparksManager.OnError(this.AuthError));
		yield break;
	}

	// Token: 0x060092EA RID: 37610 RVA: 0x00377D94 File Offset: 0x00375F94
	[PublicizedFrom(EAccessModifier.Private)]
	public void AuthError(string _error)
	{
		if (_error.Contains("cannot authenticate this month"))
		{
			Log.Out("[GSM] Skipping me");
			return;
		}
		if (_error.Contains("timeout"))
		{
			Log.Error("[GSM] AuthError TimeOut");
			return;
		}
		if (_error.Contains("UNRECOGNISED"))
		{
			Log.Error("[GSM] AuthError UNRECOGNISED");
			return;
		}
		Log.Error("[GSM] AuthError" + _error);
	}

	// Token: 0x060092EB RID: 37611 RVA: 0x00377DF9 File Offset: 0x00375FF9
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerAuthenticated(PlayerDetails _playerDetails)
	{
		this.ProgramStarted();
	}

	// Token: 0x060092EC RID: 37612 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void DeviceAuth(GameSparksManager.OnPlayerAuthenticated _onPlayerAuthSuccess, GameSparksManager.OnError _onAuthError)
	{
	}

	// Token: 0x060092ED RID: 37613 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrepareAndSendRequest(GSRequestData _data, string _eventKey)
	{
	}

	// Token: 0x060092EE RID: 37614 RVA: 0x00377E04 File Offset: 0x00376004
	public void ProgramStarted()
	{
		string eventKey = "PROGRAM_START";
		GSRequestData gsrequestData = new GSRequestData();
		gsrequestData.AddString("uniqueID", this.DeviceId);
		gsrequestData.AddBoolean("IsDedicated", GameManager.IsDedicatedServer);
		GSRequestData gsrequestData2 = new GSRequestData();
		gsrequestData2.AddString("Text", Constants.cVersionInformation.ShortString);
		gsrequestData2.AddString("LongText", Constants.cVersionInformation.LongString);
		gsrequestData2.AddString("Type", Constants.cVersionInformation.ReleaseType.ToStringCached<VersionInformation.EGameReleaseType>());
		gsrequestData2.AddNumber("Major", Constants.cVersionInformation.Major);
		gsrequestData2.AddNumber("Minor", Constants.cVersionInformation.Minor);
		gsrequestData2.AddNumber("Build", Constants.cVersionInformation.Build);
		gsrequestData.AddObject("GameVersion", gsrequestData2);
		if (!GameManager.IsDedicatedServer)
		{
			gsrequestData.AddString("BuildPlatform", Application.platform.ToStringCached<RuntimePlatform>());
			gsrequestData.AddString("OperatingSystemFamily", SystemInfo.operatingSystemFamily.ToStringCached<OperatingSystemFamily>());
			gsrequestData.AddString("OperatingSystemFull", SystemInfo.operatingSystem);
			gsrequestData.AddString("ProcessorType", SystemInfo.processorType);
			gsrequestData.AddNumber("ProcessorCount", SystemInfo.processorCount);
			gsrequestData.AddNumber("ProcessorClockMHz", MathUtils.RoundToSignificantDigits((double)SystemInfo.processorFrequency, 2));
			gsrequestData.AddNumber("SystemMemoryMB", MathUtils.TruncateToSignificantDigits((double)SystemInfo.systemMemorySize, 2));
			GSRequestData gsrequestData3 = gsrequestData;
			string key = "Country";
			IUtils utils = PlatformManager.NativePlatform.Utils;
			gsrequestData3.AddString(key, ((utils != null) ? utils.GetCountry() : null) ?? "-n/a-");
			gsrequestData.AddString("Language", Localization.ActiveLanguage.ToLower());
			GSRequestData gsrequestData4 = gsrequestData;
			string key2 = "EacActive";
			IAntiCheatClient antiCheatClient = PlatformManager.MultiPlatform.AntiCheatClient;
			gsrequestData4.AddBoolean(key2, antiCheatClient != null && antiCheatClient.ClientAntiCheatEnabled());
			gsrequestData.AddString("GraphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
			gsrequestData.AddString("GraphicsDeviceName", SystemInfo.graphicsDeviceName);
			gsrequestData.AddNumber("GraphicsMemoryMB", MathUtils.RoundToSignificantDigits((double)SystemInfo.graphicsMemorySize, 2));
			gsrequestData.AddString("GraphicsApi", SystemInfo.graphicsDeviceType.ToStringCached<GraphicsDeviceType>());
			gsrequestData.AddString("GraphicsVersion", SystemInfo.graphicsDeviceVersion);
			gsrequestData.AddNumber("GraphicsShaderLevel", (double)((float)SystemInfo.graphicsShaderLevel / 10f));
			gsrequestData.AddBoolean("GameSenseInstalled", GameSenseManager.GameSenseInstalled);
			Display main = Display.main;
			ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string> valueTuple = XUiC_OptionsVideo.ResolutionInfo.DimensionsToAspectRatio(main.systemWidth, main.systemHeight);
			float item = valueTuple.Item2;
			string item2 = valueTuple.Item3;
			GSRequestData gsrequestData5 = new GSRequestData();
			gsrequestData5.AddString("Text", string.Format("{0}x{1}", main.systemWidth, main.systemHeight));
			gsrequestData5.AddNumber("Width", main.systemWidth);
			gsrequestData5.AddNumber("Height", main.systemHeight);
			gsrequestData5.AddNumber("AspectRatio", (double)item);
			gsrequestData5.AddString("AspectRatioName", item2);
			gsrequestData.AddObject("ScreenResolution", gsrequestData5);
			GSRequestData gsrequestData6 = new GSRequestData();
			ValueTuple<XUiC_OptionsVideo.ResolutionInfo.EAspectRatio, float, string> valueTuple2 = XUiC_OptionsVideo.ResolutionInfo.DimensionsToAspectRatio(Screen.width, Screen.height);
			float item3 = valueTuple2.Item2;
			string item4 = valueTuple2.Item3;
			gsrequestData6.AddString("Text", string.Format("{0}x{1}", Screen.width, Screen.height));
			gsrequestData6.AddNumber("Width", Screen.width);
			gsrequestData6.AddNumber("Height", Screen.height);
			gsrequestData6.AddNumber("AspectRatio", (double)item3);
			gsrequestData6.AddString("AspectRatioName", item4);
			gsrequestData.AddObject("Resolution", gsrequestData6);
			gsrequestData.AddString("FullscreenMode", Screen.fullScreenMode.ToStringCached<FullScreenMode>());
			GSRequestData gsrequestData7 = new GSRequestData();
			foreach (EnumGamePrefs enumGamePrefs in EnumUtils.Values<EnumGamePrefs>())
			{
				string text = enumGamePrefs.ToStringCached<EnumGamePrefs>();
				if (text.StartsWith("Options", StringComparison.Ordinal))
				{
					GamePrefs.EnumType? prefType = GamePrefs.GetPrefType(enumGamePrefs);
					if (prefType != null)
					{
						switch (prefType.GetValueOrDefault())
						{
						case GamePrefs.EnumType.Int:
							gsrequestData7.AddNumber(text, GamePrefs.GetInt(enumGamePrefs));
							break;
						case GamePrefs.EnumType.Float:
							gsrequestData7.AddNumber(text, (double)GamePrefs.GetFloat(enumGamePrefs));
							break;
						case GamePrefs.EnumType.String:
							gsrequestData7.AddString(text, GamePrefs.GetString(enumGamePrefs));
							break;
						case GamePrefs.EnumType.Bool:
							gsrequestData7.AddBoolean(text, GamePrefs.GetBool(enumGamePrefs));
							break;
						case GamePrefs.EnumType.Binary:
							Log.Warning("Options GamePref with type Binary: " + text);
							break;
						default:
							Log.Warning("Options GamePref with unknown type: " + text);
							break;
						}
					}
					else
					{
						Log.Warning("Options GamePref with no declaration entry: " + text);
					}
				}
			}
			gsrequestData.AddObject("GameSettings", gsrequestData7);
		}
		else
		{
			gsrequestData.AddString("DediBuildPlatform", Application.platform.ToStringCached<RuntimePlatform>());
			gsrequestData.AddString("DediOperatingSystemFamily", SystemInfo.operatingSystemFamily.ToStringCached<OperatingSystemFamily>());
			gsrequestData.AddString("DediOperatingSystemFull", SystemInfo.operatingSystem);
			gsrequestData.AddString("DediProcessorType", SystemInfo.processorType);
			gsrequestData.AddNumber("DediProcessorCount", SystemInfo.processorCount);
			gsrequestData.AddNumber("DediProcessorClockMHz", MathUtils.RoundToSignificantDigits((double)SystemInfo.processorFrequency, 2));
			gsrequestData.AddNumber("DediSystemMemoryMB", MathUtils.TruncateToSignificantDigits((double)SystemInfo.systemMemorySize, 2));
		}
		this.PrepareAndSendRequest(gsrequestData, eventKey);
	}

	// Token: 0x060092EF RID: 37615 RVA: 0x00378364 File Offset: 0x00376564
	public void PrepareNewSession()
	{
		GameSparksCollector.GetSessionUpdateDataAndReset();
		GameSparksCollector.GetSessionTotalData(true);
		PlatformManager.NativePlatform.Input.ResetInputStyleUsage();
	}

	// Token: 0x060092F0 RID: 37616 RVA: 0x00378384 File Offset: 0x00376584
	public void SessionStarted(string _world, string _gameMode, bool _isServer)
	{
		GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
		string value = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? GameManager.Instance.World.Guid : GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
		int num = 1;
		bool flag = num == 1 || num == 2;
		bool flag2 = false;
		flag2 = gameServerInfo.GetValue(GameInfoBool.ModdedConfig);
		string eventKey = "SESSION_START";
		GSRequestData gsrequestData = new GSRequestData();
		gsrequestData.AddString("uniqueID", value);
		gsrequestData.AddBoolean("StockSettings", flag);
		gsrequestData.AddBoolean("VanillaConfig", !flag2);
		string value2;
		if (GameManager.IsDedicatedServer)
		{
			value2 = "DedicatedServer";
		}
		else if (_isServer)
		{
			value2 = "ListenServer";
		}
		else if (gameServerInfo.IsDedicated)
		{
			value2 = "ClientOnDedicated";
		}
		else
		{
			value2 = "ClientOnListen";
		}
		gsrequestData.AddString("PeerType", value2);
		GSRequestData gsrequestData2 = new GSRequestData();
		List<Mod> loadedMods = ModManager.GetLoadedMods();
		if (loadedMods.Count == 0)
		{
			gsrequestData2.AddNumber("_Vanilla_", 1);
		}
		else
		{
			foreach (Mod mod in loadedMods)
			{
				gsrequestData2.AddNumber(mod.Name, 1);
			}
		}
		gsrequestData.AddObject(GameManager.IsDedicatedServer ? "ModsLoadedDedi" : "ModsLoadedClient", gsrequestData2);
		if (_isServer)
		{
			GSRequestData gsrequestData3 = new GSRequestData();
			foreach (KeyValuePair<GameInfoString, string> keyValuePair in gameServerInfo.Strings)
			{
				if (!GameSparksManager.IgnoredGameInfoStrings.Contains(keyValuePair.Key))
				{
					gsrequestData3.AddString(keyValuePair.Key.ToStringCached<GameInfoString>(), keyValuePair.Value);
				}
			}
			foreach (KeyValuePair<GameInfoInt, int> keyValuePair2 in gameServerInfo.Ints)
			{
				if (!GameSparksManager.IgnoredGameInfoInts.Contains(keyValuePair2.Key))
				{
					gsrequestData3.AddNumber(keyValuePair2.Key.ToStringCached<GameInfoInt>(), keyValuePair2.Value);
				}
			}
			foreach (KeyValuePair<GameInfoBool, bool> keyValuePair3 in gameServerInfo.Bools)
			{
				if (!GameSparksManager.IgnoredGameInfoBools.Contains(keyValuePair3.Key))
				{
					gsrequestData3.AddBoolean(keyValuePair3.Key.ToStringCached<GameInfoBool>(), keyValuePair3.Value);
				}
			}
			gsrequestData.AddObject("WorldSettings", gsrequestData3);
		}
		this.PrepareAndSendRequest(gsrequestData, eventKey);
		this.sessionUpdateCoroutine = null;
		this.endCoroutine = false;
		GameSparksCollector.CollectGamePlayData = (flag && !flag2);
		this.sessionUpdateCoroutine = ThreadManager.StartCoroutine(this.SessionUpdate());
		GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.UsedTwitchIntegration, null, 0, false, GameSparksCollector.GSDataCollection.SessionTotal);
		GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.PeakConcurrentClients, null, 0, false, GameSparksCollector.GSDataCollection.SessionTotal);
		GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.PeakConcurrentPlayers, null, GameManager.IsDedicatedServer ? 0 : 1, false, GameSparksCollector.GSDataCollection.SessionTotal);
		this.nextDediSessionEndTransmitTime = Time.unscaledTime + 28800f;
	}

	// Token: 0x060092F1 RID: 37617 RVA: 0x003786C8 File Offset: 0x003768C8
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SessionUpdate()
	{
		while (!this.endCoroutine)
		{
			yield return new WaitForSecondsRealtime((float)this.sessionUpdateIntervalSec);
			if (this.endCoroutine)
			{
				yield break;
			}
			if (TwitchManager.HasInstance && TwitchManager.Current.IsReady)
			{
				GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.UsedTwitchIntegration, null, 1, false, GameSparksCollector.GSDataCollection.SessionTotal);
			}
			string eventKey = "SESSION_UPDATE";
			GSRequestData sessionUpdateDataAndReset = GameSparksCollector.GetSessionUpdateDataAndReset();
			if (GameManager.IsDedicatedServer && this.nextDediSessionEndTransmitTime <= Time.unscaledTime)
			{
				foreach (KeyValuePair<string, object> keyValuePair in GameSparksCollector.GetSessionTotalData(false).BaseData)
				{
					sessionUpdateDataAndReset.Add(keyValuePair.Key, keyValuePair.Value);
				}
				this.nextDediSessionEndTransmitTime = Time.unscaledTime + 28800f;
			}
			this.PrepareAndSendRequest(sessionUpdateDataAndReset, eventKey);
		}
		yield break;
	}

	// Token: 0x060092F2 RID: 37618 RVA: 0x003786D8 File Offset: 0x003768D8
	public void SessionEnded()
	{
		if (this.sessionUpdateCoroutine != null)
		{
			this.endCoroutine = true;
			ThreadManager.StopCoroutine(this.sessionUpdateCoroutine);
			this.sessionUpdateCoroutine = null;
		}
		if (TwitchManager.HasInstance && TwitchManager.Current.IsReady)
		{
			GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.UsedTwitchIntegration, null, 1, false, GameSparksCollector.GSDataCollection.SessionTotal);
		}
		string eventKey = "SESSION_END";
		GSRequestData sessionUpdateDataAndReset = GameSparksCollector.GetSessionUpdateDataAndReset();
		sessionUpdateDataAndReset.AddString("uniqueID", this.DeviceId);
		foreach (KeyValuePair<string, object> keyValuePair in GameSparksCollector.GetSessionTotalData(true).BaseData)
		{
			sessionUpdateDataAndReset.Add(keyValuePair.Key, keyValuePair.Value);
		}
		GSRequestData value = new GSRequestData();
		sessionUpdateDataAndReset.AddObject("RunningTotals", value);
		if (!GameManager.IsDedicatedServer)
		{
			sessionUpdateDataAndReset.AddString("InputDeviceStyle", PlatformManager.NativePlatform.Input.MostUsedInputStyle().ToStringCached<PlayerInputManager.InputStyle>());
			PlatformManager.NativePlatform.Input.ResetInputStyleUsage();
		}
		this.PrepareAndSendRequest(sessionUpdateDataAndReset, eventKey);
	}

	// Token: 0x04006DCB RID: 28107
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int dediSessionEndIntervalSec = 28800;

	// Token: 0x04006DCC RID: 28108
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<GameInfoString> IgnoredGameInfoStrings = new List<GameInfoString>
	{
		GameInfoString.GameHost,
		GameInfoString.GameName,
		GameInfoString.IP,
		GameInfoString.ServerDescription,
		GameInfoString.ServerLoginConfirmationText,
		GameInfoString.ServerWebsiteURL
	};

	// Token: 0x04006DCD RID: 28109
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<GameInfoInt> IgnoredGameInfoInts = new List<GameInfoInt>
	{
		GameInfoInt.CurrentPlayers,
		GameInfoInt.DayCount,
		GameInfoInt.Port,
		GameInfoInt.CurrentServerTime
	};

	// Token: 0x04006DCE RID: 28110
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<GameInfoBool> IgnoredGameInfoBools = new List<GameInfoBool>
	{
		GameInfoBool.Architecture64
	};

	// Token: 0x04006DCF RID: 28111
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool authenticated;

	// Token: 0x04006DD0 RID: 28112
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameSparksManager instance;

	// Token: 0x04006DD1 RID: 28113
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Coroutine sessionUpdateCoroutine;

	// Token: 0x04006DD2 RID: 28114
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool endCoroutine;

	// Token: 0x04006DD3 RID: 28115
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float nextDediSessionEndTransmitTime = -1f;

	// Token: 0x020011F6 RID: 4598
	// (Invoke) Token: 0x060092F6 RID: 37622
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate void OnError(string _error);

	// Token: 0x020011F7 RID: 4599
	// (Invoke) Token: 0x060092FA RID: 37626
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate void OnPlayerAuthenticated(PlayerDetails _playerDetails);
}
