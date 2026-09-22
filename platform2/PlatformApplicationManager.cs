using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Platform
{
	// Token: 0x02001BCD RID: 7117
	public static class PlatformApplicationManager
	{
		// Token: 0x17001A34 RID: 6708
		// (get) Token: 0x0600D3DA RID: 54234 RVA: 0x004CB685 File Offset: 0x004C9885
		// (set) Token: 0x0600D3DB RID: 54235 RVA: 0x004CB68C File Offset: 0x004C988C
		public static IPlatformApplication Application { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A35 RID: 6709
		// (get) Token: 0x0600D3DC RID: 54236 RVA: 0x004CB694 File Offset: 0x004C9894
		public static bool IsRestartRequired
		{
			get
			{
				return PlatformApplicationManager.isRestartRequired;
			}
		}

		// Token: 0x0600D3DD RID: 54237 RVA: 0x004CB69B File Offset: 0x004C989B
		public static bool Init()
		{
			PlatformApplicationManager.Application = IPlatformApplication.Create();
			return true;
		}

		// Token: 0x0600D3DE RID: 54238 RVA: 0x004CB6A8 File Offset: 0x004C98A8
		public static void SetRestartRequired()
		{
			PlatformApplicationManager.isRestartRequired = PlatformApplicationManager.RestartIsSupported;
			Log.Out(string.Format("[PlatformApplication] restart required = {0}", PlatformApplicationManager.isRestartRequired));
		}

		// Token: 0x0600D3DF RID: 54239 RVA: 0x004CB6CD File Offset: 0x004C98CD
		public static bool CheckRestartCoroutineReady()
		{
			return PlatformApplicationManager.isRestartRequired && !PlatformApplicationManager.isRestarting && !InviteManager.Instance.IsConnectingToInvite();
		}

		// Token: 0x0600D3E0 RID: 54240 RVA: 0x004CB6EC File Offset: 0x004C98EC
		public static IEnumerator CheckRestartCoroutine(bool loadSaveGame = false)
		{
			if (!PlatformApplicationManager.CheckRestartCoroutineReady())
			{
				yield break;
			}
			PlatformApplicationManager.isRestartRequired = false;
			if (!PlatformOptimizations.RestartProcessSupported)
			{
				Log.Error("[PlatformApplication] Process restart debug: application would have restarted here if supported by the platform/device");
				yield break;
			}
			PlatformApplicationManager.isRestarting = true;
			try
			{
				yield return GameManager.Instance.ShowExitingGameUICoroutine();
				PlatformApplicationManager.RestartProcess(loadSaveGame);
			}
			finally
			{
				Log.Error("[PlatformApplication] failed to restart process.");
				PlatformApplicationManager.isRestarting = false;
			}
			yield break;
			yield break;
		}

		// Token: 0x0600D3E1 RID: 54241 RVA: 0x004CB6FC File Offset: 0x004C98FC
		[PublicizedFrom(EAccessModifier.Private)]
		public static string[] RemoveFirstRunArguments(string[] argv)
		{
			return (from arg in argv
			where !arg.StartsWith("-LoadSaveGame=", StringComparison.OrdinalIgnoreCase)
			where !arg.StartsWith("-RunAutomation=", StringComparison.OrdinalIgnoreCase)
			select arg).ToArray<string>();
		}

		// Token: 0x0600D3E2 RID: 54242 RVA: 0x004CB758 File Offset: 0x004C9958
		[PublicizedFrom(EAccessModifier.Private)]
		public static void RestartProcess(bool loadSaveGame)
		{
			List<string> list = new List<string>();
			list.AddRange(PlatformApplicationManager.RemoveFirstRunArguments(GameStartupHelper.RemoveTemporaryArguments(GameStartupHelper.GetCommandLineArgs())));
			list.Add("[REMOVE_ON_RESTART]");
			list.Add("-skipintro");
			list.Add(LaunchPrefs.SkipNewsScreen.ToCommandLine(true));
			if (PlatformOptimizations.RestartAfterRwg && loadSaveGame)
			{
				Log.Out(string.Format("[LoadSaveGame] After restart should load: worldName={0} saveName={1} ({2})", GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameName), (UserDataStorageType)GamePrefs.GetInt(EnumGamePrefs.GameSaveStorageType)));
				list.Add(LaunchPrefs.LoadSaveGame.ToCommandLine(true));
			}
			list.AddRange(InviteManager.Instance.GetCommandLineArguments());
			list.AddRange(PlatformManager.NativePlatform.GetArgumentsForRelaunch());
			try
			{
				GamePrefs.Instance.Save();
				SaveDataUtils.Destroy();
				PlatformManager.Destroy();
			}
			catch (Exception e)
			{
				Log.Error("Exception thrown while preparing for process restart. This may cause errors in the next run");
				Log.Exception(e);
			}
			PlatformApplicationManager.Application.RestartProcess(list.ToArray());
		}

		// Token: 0x0400A186 RID: 41350
		public static bool RestartIsSupported = PlatformOptimizations.RestartProcessSupported;

		// Token: 0x0400A188 RID: 41352
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool isRestartRequired;

		// Token: 0x0400A189 RID: 41353
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool isRestarting;

		// Token: 0x02001BCE RID: 7118
		public static class LoadSaveGame
		{
			// Token: 0x0600D3E4 RID: 54244 RVA: 0x004CB860 File Offset: 0x004C9A60
			public static EPlatformLoadSaveGameState GetState()
			{
				if (!LaunchPrefs.LoadSaveGame.Value)
				{
					return EPlatformLoadSaveGameState.Done;
				}
				if (PlatformApplicationManager.LoadSaveGame.loadSaveGameState != EPlatformLoadSaveGameState.Init)
				{
					return PlatformApplicationManager.LoadSaveGame.loadSaveGameState;
				}
				PlatformApplicationManager.LoadSaveGame.GameWorld = GamePrefs.GetString(EnumGamePrefs.GameWorld);
				PlatformApplicationManager.LoadSaveGame.GameName = GamePrefs.GetString(EnumGamePrefs.GameName);
				PlatformApplicationManager.LoadSaveGame.SaveStorageType = (UserDataStorageType)GamePrefs.GetInt(EnumGamePrefs.GameSaveStorageType);
				bool found = false;
				bool isArchived = false;
				GameIO.GetPlayerSaves(delegate(UserDataStorageType foundStorage, string foundSaveName, string foundWorldName, DateTime _, WorldState _, bool foundIsArchived)
				{
					if (foundStorage == PlatformApplicationManager.LoadSaveGame.SaveStorageType && foundSaveName.EqualsCaseInsensitive(PlatformApplicationManager.LoadSaveGame.GameName) && foundWorldName.EqualsCaseInsensitive(PlatformApplicationManager.LoadSaveGame.GameWorld))
					{
						found = true;
						isArchived = foundIsArchived;
					}
				}, true);
				if (!found)
				{
					Log.Out(string.Concat(new string[]
					{
						"[LoadSaveGame] Creating new save game '",
						PlatformApplicationManager.LoadSaveGame.GameName,
						"' from the world '",
						PlatformApplicationManager.LoadSaveGame.GameWorld,
						"'."
					}));
					return PlatformApplicationManager.LoadSaveGame.loadSaveGameState = EPlatformLoadSaveGameState.NewGameOpen;
				}
				if (isArchived)
				{
					Log.Warning(string.Concat(new string[]
					{
						"[LoadSaveGame] Can not load archived save '",
						PlatformApplicationManager.LoadSaveGame.GameName,
						"' (world '",
						PlatformApplicationManager.LoadSaveGame.GameWorld,
						"')."
					}));
					return PlatformApplicationManager.LoadSaveGame.loadSaveGameState = EPlatformLoadSaveGameState.Done;
				}
				Log.Out(string.Concat(new string[]
				{
					"[LoadSaveGame] Loading existing save game '",
					PlatformApplicationManager.LoadSaveGame.GameName,
					"' (world '",
					PlatformApplicationManager.LoadSaveGame.GameWorld,
					"')."
				}));
				return PlatformApplicationManager.LoadSaveGame.loadSaveGameState = EPlatformLoadSaveGameState.ContinueGameOpen;
			}

			// Token: 0x0600D3E5 RID: 54245 RVA: 0x004CB9A8 File Offset: 0x004C9BA8
			public static void AdvanceStateFrom(EPlatformLoadSaveGameState previousState)
			{
				if (PlatformApplicationManager.LoadSaveGame.loadSaveGameState != previousState)
				{
					Log.Error(string.Format("[LoadSaveGame] Expected advance from {0} but was {1}", PlatformApplicationManager.LoadSaveGame.loadSaveGameState, previousState));
					PlatformApplicationManager.LoadSaveGame.loadSaveGameState = EPlatformLoadSaveGameState.Done;
				}
				EPlatformLoadSaveGameState eplatformLoadSaveGameState;
				switch (previousState)
				{
				case EPlatformLoadSaveGameState.Init:
					throw new NotSupportedException("Init state should be manually advanced from because it branches.");
				case EPlatformLoadSaveGameState.NewGameOpen:
					eplatformLoadSaveGameState = EPlatformLoadSaveGameState.NewGameSelect;
					break;
				case EPlatformLoadSaveGameState.NewGameSelect:
					eplatformLoadSaveGameState = EPlatformLoadSaveGameState.NewGamePlay;
					break;
				case EPlatformLoadSaveGameState.NewGamePlay:
					eplatformLoadSaveGameState = EPlatformLoadSaveGameState.Done;
					break;
				case EPlatformLoadSaveGameState.ContinueGameOpen:
					eplatformLoadSaveGameState = EPlatformLoadSaveGameState.ContinueGameSelect;
					break;
				case EPlatformLoadSaveGameState.ContinueGameSelect:
					eplatformLoadSaveGameState = EPlatformLoadSaveGameState.ContinueGamePlay;
					break;
				case EPlatformLoadSaveGameState.ContinueGamePlay:
					eplatformLoadSaveGameState = EPlatformLoadSaveGameState.Done;
					break;
				case EPlatformLoadSaveGameState.Done:
					throw new NotSupportedException("Can't advance from the final state.");
				default:
					throw new ArgumentOutOfRangeException();
				}
				PlatformApplicationManager.LoadSaveGame.loadSaveGameState = eplatformLoadSaveGameState;
				Log.Out(string.Format("[LoadSaveGame] Advanced to state {0} (was {1})", PlatformApplicationManager.LoadSaveGame.loadSaveGameState, previousState));
			}

			// Token: 0x0600D3E6 RID: 54246 RVA: 0x004CBA63 File Offset: 0x004C9C63
			public static void SetFailed()
			{
				if (PlatformApplicationManager.LoadSaveGame.loadSaveGameState == EPlatformLoadSaveGameState.Done)
				{
					return;
				}
				Log.Warning(string.Format("[LoadSaveGame] Failed to automate creating or loading the save game. State: {0}", PlatformApplicationManager.LoadSaveGame.loadSaveGameState));
				PlatformApplicationManager.LoadSaveGame.loadSaveGameState = EPlatformLoadSaveGameState.Done;
			}

			// Token: 0x0400A18A RID: 41354
			public static string GameWorld;

			// Token: 0x0400A18B RID: 41355
			public static string GameName;

			// Token: 0x0400A18C RID: 41356
			public static UserDataStorageType SaveStorageType;

			// Token: 0x0400A18D RID: 41357
			[PublicizedFrom(EAccessModifier.Private)]
			public static EPlatformLoadSaveGameState loadSaveGameState;
		}
	}
}
