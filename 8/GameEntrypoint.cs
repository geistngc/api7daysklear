using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using BhvrAnalyticsServices;
using BhvrAnalyticsServices.Interfaces;
using BhvrAnalyticsServices.Services;
using Platform;
using SandboxOptions;
using Services;
using Services.Analytics;
using UnityEngine;

// Token: 0x0200119C RID: 4508
public class GameEntrypoint : MonoBehaviour
{
	// Token: 0x17001141 RID: 4417
	// (get) Token: 0x06009018 RID: 36888 RVA: 0x0036351C File Offset: 0x0036171C
	// (set) Token: 0x06009019 RID: 36889 RVA: 0x00363523 File Offset: 0x00361723
	public static bool EntrypointSuccess { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x0600901A RID: 36890 RVA: 0x0036352B File Offset: 0x0036172B
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		if (!GameEntrypoint.s_entrypointEntered)
		{
			Log.Error("[GameEntrypoint] Blocking initialization in Awake!");
		}
		ThreadManager.RunCoroutineSync(GameEntrypoint.EntrypointCoroutine());
	}

	// Token: 0x0600901B RID: 36891 RVA: 0x00363548 File Offset: 0x00361748
	public static bool FirstFrameInit()
	{
		BacktraceUtils.InitializeBacktrace();
		Cursor.visible = false;
		ThreadManager.SetMainThreadRef(Thread.CurrentThread);
		PlatformOptimizations.Init();
		if (GameEntrypoint.HasPrefCollisions())
		{
			return false;
		}
		GamePrefs.InitPropertyDeclarations();
		if (!GameStartupHelper.Instance.InitCommandLine())
		{
			return false;
		}
		if (!string.IsNullOrEmpty(LaunchPrefs.RunAutomation.Value))
		{
			AutomationRunner.InitialiseLogging();
		}
		GameIO.InitializeUserDataPaths(LaunchPrefs.UserDataFolder.Value);
		if (!PlatformApplicationManager.Init())
		{
			return false;
		}
		if (!PlatformManager.Init())
		{
			return false;
		}
		ServiceProvider.Init();
		LogLevel logLevel = LogLevel.None;
		string launchArgument = GameUtils.GetLaunchArgument("analyticsLogLevel");
		if (launchArgument != null)
		{
			EnumUtils.TryParse<LogLevel>(launchArgument, out logLevel, true);
		}
		ServiceProvider.Instance.Register(typeof(IAnalyticsService), AnalyticsServiceFactory.CreateAnalyticsService(AnalyticsServiceFactory.LoadServiceConfiguration(GameManager.IsDedicatedServer), EventTypes.VersionDictionary, logLevel));
		ServiceProvider.Instance.Get<IAnalyticsService>().StartService();
		Application.targetFrameRate = (int)PlatformApplicationManager.Application.GetCurrentRefreshRate().value;
		return true;
	}

	// Token: 0x0600901C RID: 36892 RVA: 0x00363632 File Offset: 0x00361832
	public static IEnumerator EntrypointCoroutine()
	{
		if (GameEntrypoint.s_entrypointEntered)
		{
			while (!GameEntrypoint.s_entrypointFinished)
			{
				yield return null;
			}
			yield break;
		}
		GameEntrypoint.s_entrypointEntered = true;
		try
		{
			if (!GameEntrypoint.FirstFrameInit())
			{
				yield break;
			}
			yield return GameEntrypoint.EntrypointCoroutineInternal();
		}
		finally
		{
			GameEntrypoint.s_entrypointFinished = true;
			if (!GameEntrypoint.EntrypointSuccess)
			{
				Log.Error("[GameEntrypoint] Failed initializing core systems, shutting down");
				Application.Quit();
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x0600901D RID: 36893 RVA: 0x0036363A File Offset: 0x0036183A
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator EntrypointCoroutineInternal()
	{
		yield return SaveDataUtils.InitStaticCoroutine();
		yield return null;
		GameOptionsReset.Init();
		RoamingPrefs.Init();
		PrefVersionStore store = RoamingPrefs.Store;
		if (store != null)
		{
			store.Apply();
		}
		GamePrefs.InitPrefs();
		SandboxOptionManager.Current.Init();
		GamePrefs.SetupSandboxReferences();
		GameStats.SetupSandboxReferences();
		if (!GameStartupHelper.Instance.InitGamePrefs())
		{
			yield break;
		}
		PlatformManager.MultiPlatform.UserDataRoaming.ValidateRoamingMode();
		yield return null;
		try
		{
			Localization.Init();
		}
		catch (Exception ex)
		{
			Log.Error(string.Format("[GameEntrypoint] Failed initializing localization: {0}", ex.GetType()));
			Log.Exception(ex);
			yield break;
		}
		ProfileSDF.InitializeStatics();
		Archetype.InitializeStatics();
		GameEntrypoint.EntrypointSuccess = true;
		yield break;
	}

	// Token: 0x0600901E RID: 36894 RVA: 0x00363644 File Offset: 0x00361844
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool HasPrefCollisions()
	{
		foreach (string text in EnumUtils.Names<EnumGamePrefs>())
		{
			ILaunchPref launchPref;
			if (LaunchPrefs.All.TryGetValue(text, out launchPref))
			{
				Log.Error(string.Concat(new string[]
				{
					"Name collision between LaunchPref '",
					launchPref.Name,
					"' and GamePref '",
					text,
					"'."
				}));
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600901F RID: 36895 RVA: 0x003636D4 File Offset: 0x003618D4
	[Conditional("NEVER_DEFINED")]
	public static void ProfileSection(string identifier)
	{
		if (GameEntrypoint.s_profileTotal == null)
		{
			GameEntrypoint.s_profileTotal = new MicroStopwatch(true);
		}
		if (GameEntrypoint.s_profileSection == null)
		{
			GameEntrypoint.s_profileSection = new MicroStopwatch(true);
		}
		GameEntrypoint.s_profileIdentifier = identifier;
	}

	// Token: 0x06009020 RID: 36896 RVA: 0x00363700 File Offset: 0x00361900
	[Conditional("PROFILE_GAME_ENTRYPOINT")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ProfileSectionEnd()
	{
		if (GameEntrypoint.s_profileIdentifier == null)
		{
			return;
		}
		Log.Out(string.Format("[GameEntrypoint: Profile] Section {0} {1:F3} ms", GameEntrypoint.s_profileIdentifier, GameEntrypoint.s_profileSection.Elapsed.TotalMilliseconds));
		GameEntrypoint.s_profileSection.Restart();
		GameEntrypoint.s_profileIdentifier = null;
	}

	// Token: 0x06009021 RID: 36897 RVA: 0x00363750 File Offset: 0x00361950
	[Conditional("PROFILE_GAME_ENTRYPOINT")]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ProfileEnd()
	{
		if (GameEntrypoint.s_profileTotal == null)
		{
			return;
		}
		Log.Out(string.Format("[GameEntrypoint: Profile] TOTAL {0:F3} ms", GameEntrypoint.s_profileTotal.Elapsed.TotalMilliseconds));
		GameEntrypoint.s_profileTotal = null;
	}

	// Token: 0x04006AA4 RID: 27300
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool s_entrypointEntered;

	// Token: 0x04006AA5 RID: 27301
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool s_entrypointFinished;

	// Token: 0x04006AA7 RID: 27303
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static MicroStopwatch s_profileTotal;

	// Token: 0x04006AA8 RID: 27304
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static MicroStopwatch s_profileSection;

	// Token: 0x04006AA9 RID: 27305
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static string s_profileIdentifier;
}
