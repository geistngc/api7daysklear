using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Platform;
using UnityEngine;

// Token: 0x02000079 RID: 121
public class AutomationRunner
{
	// Token: 0x06000247 RID: 583 RVA: 0x00012D78 File Offset: 0x00010F78
	[PublicizedFrom(EAccessModifier.Private)]
	public AutomationRunner()
	{
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x06000248 RID: 584 RVA: 0x00012D8B File Offset: 0x00010F8B
	// (set) Token: 0x06000249 RID: 585 RVA: 0x00012D93 File Offset: 0x00010F93
	public bool IsRunning { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x0600024A RID: 586 RVA: 0x00012D9C File Offset: 0x00010F9C
	// (set) Token: 0x0600024B RID: 587 RVA: 0x00012DA4 File Offset: 0x00010FA4
	public AutomationScript CurrentScript { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new AutomationScript();

	// Token: 0x0600024C RID: 588 RVA: 0x00012DAD File Offset: 0x00010FAD
	public bool LoadScript(AutomationScript script)
	{
		Log.Error("[AutomationRunner] Disabled for this build type.");
		return false;
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00012DBA File Offset: 0x00010FBA
	[PublicizedFrom(EAccessModifier.Internal)]
	public void LoadScriptUnchecked(AutomationScript script)
	{
		Log.Error("[AutomationRunner] Disabled for this build type.");
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00012DBA File Offset: 0x00010FBA
	public void StartRuns()
	{
		Log.Error("[AutomationRunner] Disabled for this build type.");
	}

	// Token: 0x0600024F RID: 591 RVA: 0x00012DC6 File Offset: 0x00010FC6
	public void Abort()
	{
		if (!this.IsRunning)
		{
			Log.Out("[AutomationRunner] Nothing to abort.");
			return;
		}
		this._abortRequested = true;
		Log.Out("[AutomationRunner] Abort requested — finishing current step.");
	}

	// Token: 0x06000250 RID: 592 RVA: 0x00012DEC File Offset: 0x00010FEC
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator RunScript(AutomationScript script)
	{
		int stepIndex = 0;
		while (stepIndex < script.steps.Count && !this._abortRequested)
		{
			AutomationStep automationStep = script.steps[stepIndex];
			if (automationStep.type == AutomationStep.StepType.StartPerfSession)
			{
				int endIndex = AutomationRunner.FindMatchingEnd(script, stepIndex);
				yield return this.ExecutePerfSession(script, stepIndex, endIndex);
				stepIndex = endIndex + 1;
			}
			else
			{
				yield return this.ExecuteStep(automationStep, 0, script.ResolveSessionDir());
				int num = stepIndex;
				stepIndex = num + 1;
			}
		}
		this.IsRunning = false;
		Log.Out("[AutomationRunner] Script finished.");
		yield break;
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00012E02 File Offset: 0x00011002
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ExecutePerfSession(AutomationScript script, int startIndex, int endIndex)
	{
		string sessionDir = script.ResolveSessionDir();
		AutomationStep automationStep = script.steps[startIndex];
		int runs = automationStep.runCount;
		int num = 0;
		string text = null;
		for (int j = startIndex + 1; j < endIndex; j++)
		{
			if (script.steps[j].type == AutomationStep.StepType.StopPerfCapture)
			{
				num++;
			}
			if (script.steps[j].type == AutomationStep.StepType.StartPerfCapture && text == null)
			{
				text = script.steps[j].capturePrefix;
			}
		}
		int num2 = runs * num;
		if (num2 > 0)
		{
			PerformanceProfiler.BeginSession(sessionDir, num2, text);
		}
		Log.Out(string.Format("[AutomationRunner] ── PerfSession: {0} run(s) × {1} capture(s)/run ──", runs, num));
		int num3;
		for (int run = 0; run < runs; run = num3 + 1)
		{
			if (this._abortRequested)
			{
				Log.Out(string.Format("[AutomationRunner] PerfSession aborted before run {0}/{1}.", run + 1, runs));
				PerformanceProfiler.AbortSession();
				yield break;
			}
			Log.Out(string.Format("[AutomationRunner] PerfSession run {0}/{1}", run + 1, runs));
			for (int i = startIndex + 1; i < endIndex; i = num3 + 1)
			{
				if (this._abortRequested)
				{
					PerformanceProfiler.AbortSession();
					yield break;
				}
				yield return this.ExecuteStep(script.steps[i], run, sessionDir);
				num3 = i;
			}
			num3 = run;
		}
		Log.Out("[AutomationRunner] ── PerfSession complete ──");
		yield break;
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00012E28 File Offset: 0x00011028
	[PublicizedFrom(EAccessModifier.Private)]
	public static int FindMatchingEnd(AutomationScript script, int startIndex)
	{
		for (int i = startIndex + 1; i < script.steps.Count; i++)
		{
			if (script.steps[i].type == AutomationStep.StepType.StopPerfSession)
			{
				return i;
			}
		}
		Log.Error("Performance Capture session start has no matching stop.");
		return script.steps.Count;
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00012E78 File Offset: 0x00011078
	[PublicizedFrom(EAccessModifier.Private)]
	public static EntityPlayerLocal GetPlayer()
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return null;
		}
		return world.GetPrimaryPlayer();
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00012E8F File Offset: 0x0001108F
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ExecuteStep(AutomationStep step, int runIndex, string sessionDir)
	{
		AutomationRunner.<>c__DisplayClass21_0 CS$<>8__locals1 = new AutomationRunner.<>c__DisplayClass21_0();
		CS$<>8__locals1.<>4__this = this;
		switch (step.type)
		{
		case AutomationStep.StepType.LoadGame:
		{
			if (GameManager.Instance.World != null)
			{
				Log.Out("[AutomationRunner] LoadGame: world already loaded — skipping.");
				goto IL_A0C;
			}
			GamePrefs.Set(EnumGamePrefs.GameWorld, step.world);
			GamePrefs.Set(EnumGamePrefs.GameName, step.gameName);
			GamePrefs.Set(EnumGamePrefs.GameSaveStorageType, (int)PlatformManager.MultiPlatform.UserDataRoaming.DefaultSaveStorage);
			GamePrefs.Instance.Load(GameIO.GetSaveGameDir() + "/gameOptions.sdf");
			Log.Out(string.Concat(new string[]
			{
				"[AutomationRunner] LoadGame: starting '",
				step.world,
				"' / '",
				step.gameName,
				"'..."
			}));
			this._gameStartDone = false;
			ModEvents.GameStartDone.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameStartDoneData>(this.OnGameStartDone));
			NetworkConnectionError networkConnectionError = SingletonMonoBehaviour<ConnectionManager>.Instance.StartServers(GamePrefs.GetString(EnumGamePrefs.ServerPassword), false);
			if (networkConnectionError != NetworkConnectionError.NoError)
			{
				Log.Error(string.Format("[AutomationRunner] LoadGame failed: {0}", networkConnectionError));
				ModEvents.GameStartDone.UnregisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameStartDoneData>(this.OnGameStartDone));
				this._abortRequested = true;
				yield break;
			}
			while (!this._gameStartDone)
			{
				yield return null;
			}
			Log.Out("[AutomationRunner] LoadGame: world loaded and ready.");
			goto IL_A0C;
		}
		case AutomationStep.StepType.PreparePlayer:
		{
			EntityPlayerLocal player2 = AutomationRunner.GetPlayer();
			if (player2 == null)
			{
				Log.Error("[AutomationRunner] PreparePlayer: no player.");
				yield break;
			}
			player2.IsGodMode.Value = true;
			player2.IsNoCollisionMode.Value = true;
			player2.IsFlyMode.Value = true;
			player2.IsSpectator = true;
			player2.Buffs.AddBuff("god", -1, true, false, -1f);
			GameManager.Instance.World.SetTimeJump(12000UL, true);
			List<Entity> list = new List<Entity>(GameManager.Instance.World.Entities.list);
			for (int j = 0; j < list.Count; j++)
			{
				Entity entity = list[j];
				if (entity != null && !(entity is EntityPlayer))
				{
					entity.DamageEntity(new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Suicide), 99999, false, 1f);
				}
			}
			GameStats.Set(EnumGameStats.TimeOfDayIncPerSec, 0);
			goto IL_A0C;
		}
		case AutomationStep.StepType.Teleport:
		{
			EntityPlayerLocal player = AutomationRunner.GetPlayer();
			if (player == null)
			{
				Log.Error("[AutomationRunner] Teleport: no player.");
				yield break;
			}
			player.TeleportToPosition(step.position, false, new Vector3?(step.lookDir));
			while (!player.Spawned)
			{
				yield return null;
			}
			yield return null;
			yield return null;
			Log.Out(string.Format("[AutomationRunner] Teleport: settled at {0}.", step.position));
			goto IL_A0C;
		}
		case AutomationStep.StepType.Wait:
			yield return new WaitForSeconds(step.duration);
			goto IL_A0C;
		case AutomationStep.StepType.MoveLine:
		{
			EntityPlayerLocal player3 = AutomationRunner.GetPlayer();
			if (player3 == null)
			{
				Log.Error("[AutomationRunner] MoveLine: no player.");
				yield break;
			}
			if (step.hasStart)
			{
				player3.SetPosition(step.positionB, true);
			}
			bool done = false;
			player3.EnableAutoMove(true).StartLine(step.duration, 0, step.position, delegate
			{
				done = true;
			});
			while (!done)
			{
				yield return null;
			}
			goto IL_A0C;
		}
		case AutomationStep.StepType.Orbit:
		{
			EntityPlayerLocal player4 = AutomationRunner.GetPlayer();
			if (player4 == null)
			{
				Log.Error("[AutomationRunner] Orbit: no player.");
				yield break;
			}
			bool done = false;
			player4.EnableAutoMove(true).StartOrbit(step.duration, 0, step.position, step.lookForward, step.isFlipped, delegate
			{
				done = true;
			});
			while (!done)
			{
				yield return null;
			}
			goto IL_A0C;
		}
		case AutomationStep.StepType.WaitForChunksLoaded:
		{
			EntityPlayerLocal player5 = AutomationRunner.GetPlayer();
			if (player5 == null)
			{
				Log.Error("[AutomationRunner] WaitForChunksLoaded: no player.");
				yield break;
			}
			ChunkManager.ChunkObserver chunkObserver = player5.ChunkObserver;
			if (chunkObserver == null)
			{
				Log.Error("[AutomationRunner] WaitForChunksLoaded: no chunk observer.");
				yield break;
			}
			Log.Out("[AutomationRunner] WaitForChunksLoaded: waiting for chunks to load...");
			yield return ProfilerGameUtils.WaitForChunksAroundObserverToLoad(chunkObserver, ChunkConditions.Displayed);
			Log.Out("[AutomationRunner] WaitForChunksLoaded: chunks loaded.");
			goto IL_A0C;
		}
		case AutomationStep.StepType.StartPerfCapture:
		{
			string prefix = string.IsNullOrEmpty(step.capturePrefix) ? string.Format("run_{0:D2}", runIndex + 1) : string.Format("{0}_{1:D2}", step.capturePrefix, runIndex + 1);
			PerformanceProfiler.StartCapture(sessionDir, prefix, step.targetFps);
			goto IL_A0C;
		}
		case AutomationStep.StepType.StopPerfCapture:
			if (PerformanceProfiler.IsCapturing())
			{
				PerformanceProfiler.StopCapture(true);
				goto IL_A0C;
			}
			Log.Warning("[AutomationRunner] StopPerfCapture step: no capture in progress.");
			goto IL_A0C;
		case AutomationStep.StepType.ExitToMenu:
			Log.Out("[AutomationRunner] ExitToMenu: disconnecting and returning to main menu.");
			GameManager.Instance.Disconnect();
			goto IL_A0C;
		case AutomationStep.StepType.Cleanup:
			ModEvents.GameStartDone.UnregisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameStartDoneData>(this.OnGameStartDone));
			Log.Out("[AutomationRunner] Cleanup: event handlers unregistered.");
			goto IL_A0C;
		case AutomationStep.StepType.AutomationComplete:
		{
			if (string.IsNullOrEmpty(step.url))
			{
				Log.Warning("[AutomationRunner] AutomationComplete: no url configured — skipping callback.");
				goto IL_A0C;
			}
			this.IsAwaitingShutdown = true;
			CS$<>8__locals1.callbackUrl = step.url;
			Log.Out("[AutomationRunner] AutomationComplete: POSTing to " + CS$<>8__locals1.callbackUrl);
			CS$<>8__locals1.postDone = false;
			CS$<>8__locals1.postOk = false;
			Task.Run(delegate()
			{
				AutomationRunner.<>c__DisplayClass21_0.<<ExecuteStep>b__3>d <<ExecuteStep>b__3>d;
				<<ExecuteStep>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ExecuteStep>b__3>d.<>4__this = CS$<>8__locals1;
				<<ExecuteStep>b__3>d.<>1__state = -1;
				<<ExecuteStep>b__3>d.<>t__builder.Start<AutomationRunner.<>c__DisplayClass21_0.<<ExecuteStep>b__3>d>(ref <<ExecuteStep>b__3>d);
				return <<ExecuteStep>b__3>d.<>t__builder.Task;
			});
			float waited = 0f;
			while (!CS$<>8__locals1.postDone && waited < 15f)
			{
				yield return new WaitForSeconds(0.25f);
				waited += 0.25f;
			}
			if (CS$<>8__locals1.postOk)
			{
				Log.Out("[AutomationRunner] AutomationComplete: callback sent successfully.");
				goto IL_A0C;
			}
			if (!CS$<>8__locals1.postDone)
			{
				Log.Error("[AutomationRunner] AutomationComplete: callback timed out.");
				goto IL_A0C;
			}
			goto IL_A0C;
		}
		case AutomationStep.StepType.LogError:
			Log.Error(step.text);
			goto IL_A0C;
		case AutomationStep.StepType.DeleteSave:
		{
			if (GameManager.Instance.World != null)
			{
				Log.Out("[AutomationRunner] DeleteSave: Can only be run while not in a game.");
				goto IL_A0C;
			}
			string saveGameDir = GameIO.GetSaveGameDir(step.world, step.gameName, PlatformManager.MultiPlatform.UserDataRoaming.DefaultSaveStorage);
			if (SdDirectory.Exists(saveGameDir))
			{
				SdDirectory.Delete(saveGameDir, true);
				SaveDataUtils.SaveDataManager.CommitSync();
				Log.Out(string.Concat(new string[]
				{
					"[AutomationRunner] DeleteSave: Save ",
					step.world,
					"/",
					step.gameName,
					" deleted."
				}));
				goto IL_A0C;
			}
			Log.Out("[AutomationRunner] DeleteSave: Save not found.");
			goto IL_A0C;
		}
		case AutomationStep.StepType.ConsoleCmd:
			GUIWindowConsole.AddLines(SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(step.text, null));
			goto IL_A0C;
		case AutomationStep.StepType.MovePingPong:
		{
			EntityPlayerLocal player = AutomationRunner.GetPlayer();
			if (player == null)
			{
				Log.Error("[AutomationRunner] MovePingPong: no player.");
				yield break;
			}
			EntityPlayerLocal.AutoMove am = player.EnableAutoMove(true);
			int segments = 2 * Mathf.Max(1, step.pingPongCount);
			int num;
			for (int i = 0; i < segments; i = num + 1)
			{
				AutomationRunner.<>c__DisplayClass21_2 CS$<>8__locals4 = new AutomationRunner.<>c__DisplayClass21_2();
				if (this._abortRequested)
				{
					yield break;
				}
				Vector3 pos = (i % 2 == 0) ? step.position : step.positionB;
				Vector3 vector = (i % 2 == 0) ? step.positionB : step.position;
				player.SetPosition(pos, true);
				am.SetLookAt(vector);
				CS$<>8__locals4.done = false;
				am.StartLine(step.duration, 0, vector, delegate
				{
					CS$<>8__locals4.done = true;
				});
				while (!CS$<>8__locals4.done)
				{
					yield return null;
				}
				CS$<>8__locals4 = null;
				num = i;
			}
			goto IL_A0C;
		}
		}
		Log.Warning(string.Format("[AutomationRunner] Unknown step type '{0}' — skipping.", step.type));
		IL_A0C:
		yield break;
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00012EB3 File Offset: 0x000110B3
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGameStartDone(ref ModEvents.SGameStartDoneData _data)
	{
		this._gameStartDone = true;
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00012EBC File Offset: 0x000110BC
	public bool RestartAllowed()
	{
		return !this.IsRunning && !this.IsAwaitingShutdown;
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00012ED1 File Offset: 0x000110D1
	public static string GetAutomationDataPath()
	{
		return GameIO.GetPostTerminationAccessiblePath() + "Automation/";
	}

	// Token: 0x06000258 RID: 600 RVA: 0x00012EE4 File Offset: 0x000110E4
	public static void InitialiseLogging()
	{
		string text = AutomationRunner.GetAutomationDataPath() + "Logs/";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string str = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss.fff");
		Log.AddOutputPath(Path.Join(text, "Player-" + str + ".log"));
		string[] files = Directory.GetFiles(text, "Player*.log");
		if (files.Length > 3)
		{
			Array.Sort<string>(files, StringComparer.InvariantCulture);
			for (int i = 0; i < files.Length - 3; i++)
			{
				File.Delete(files[i]);
				Log.Out("[AUTOMATION] Deleted old log: " + files[i]);
			}
		}
	}

	// Token: 0x040002FA RID: 762
	public static readonly AutomationRunner Instance = new AutomationRunner();

	// Token: 0x040002FC RID: 764
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsAwaitingShutdown;

	// Token: 0x040002FE RID: 766
	[PublicizedFrom(EAccessModifier.Private)]
	public bool _abortRequested;

	// Token: 0x040002FF RID: 767
	[PublicizedFrom(EAccessModifier.Private)]
	public bool _gameStartDone;
}
