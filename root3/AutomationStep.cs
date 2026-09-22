using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

// Token: 0x02000082 RID: 130
[Serializable]
public class AutomationStep
{
	// Token: 0x06000276 RID: 630 RVA: 0x00013FD8 File Offset: 0x000121D8
	public static AutomationStep LoadGame(string world, string gameName)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.LoadGame,
			world = world,
			gameName = gameName
		};
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00013FF4 File Offset: 0x000121F4
	public static AutomationStep PreparePlayer()
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.PreparePlayer
		};
	}

	// Token: 0x06000278 RID: 632 RVA: 0x00014002 File Offset: 0x00012202
	public static AutomationStep Teleport(Vector3 pos, Vector3 look)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.Teleport,
			position = pos,
			lookDir = look
		};
	}

	// Token: 0x06000279 RID: 633 RVA: 0x0001401E File Offset: 0x0001221E
	public static AutomationStep Wait(float secs)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.Wait,
			duration = secs
		};
	}

	// Token: 0x0600027A RID: 634 RVA: 0x00014033 File Offset: 0x00012233
	public static AutomationStep MoveLine(Vector3 dest, float dur)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.MoveLine,
			position = dest,
			duration = dur
		};
	}

	// Token: 0x0600027B RID: 635 RVA: 0x0001404F File Offset: 0x0001224F
	public static AutomationStep MoveLine(Vector3 start, Vector3 dest, float dur)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.MoveLine,
			position = dest,
			positionB = start,
			hasStart = true,
			duration = dur
		};
	}

	// Token: 0x0600027C RID: 636 RVA: 0x00014079 File Offset: 0x00012279
	public static AutomationStep MovePingPong(Vector3 a, Vector3 b, int count, float legDuration)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.MovePingPong,
			position = a,
			positionB = b,
			pingPongCount = Mathf.Max(1, count),
			duration = legDuration
		};
	}

	// Token: 0x0600027D RID: 637 RVA: 0x000140AA File Offset: 0x000122AA
	public static AutomationStep Orbit(Vector3 center, float dur, bool lookForward = true, bool isFlipped = false)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.Orbit,
			position = center,
			duration = dur,
			lookForward = lookForward,
			isFlipped = isFlipped
		};
	}

	// Token: 0x0600027E RID: 638 RVA: 0x000140D4 File Offset: 0x000122D4
	public static AutomationStep WaitForChunksLoaded()
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.WaitForChunksLoaded
		};
	}

	// Token: 0x0600027F RID: 639 RVA: 0x000140E2 File Offset: 0x000122E2
	public static AutomationStep StartPerfSession(int runs)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.StartPerfSession,
			runCount = runs
		};
	}

	// Token: 0x06000280 RID: 640 RVA: 0x000140F7 File Offset: 0x000122F7
	public static AutomationStep StopPerfSession()
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.StopPerfSession
		};
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00014105 File Offset: 0x00012305
	public static AutomationStep StartPerfCapture(int fps = 30, string prefix = "")
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.StartPerfCapture,
			targetFps = fps,
			capturePrefix = prefix
		};
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00014122 File Offset: 0x00012322
	public static AutomationStep StopPerfCapture()
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.StopPerfCapture
		};
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00014131 File Offset: 0x00012331
	public static AutomationStep ExitToMenu()
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.ExitToMenu
		};
	}

	// Token: 0x06000284 RID: 644 RVA: 0x00014140 File Offset: 0x00012340
	public static AutomationStep Cleanup()
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.Cleanup
		};
	}

	// Token: 0x06000285 RID: 645 RVA: 0x0001414F File Offset: 0x0001234F
	public static AutomationStep AutomationComplete(string url = "")
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.AutomationComplete,
			url = url
		};
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00014165 File Offset: 0x00012365
	public static AutomationStep LogError(string text = "")
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.LogError,
			text = text
		};
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0001417B File Offset: 0x0001237B
	public static AutomationStep DeleteSave(string world, string gameName)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.DeleteSave,
			world = world,
			gameName = gameName
		};
	}

	// Token: 0x06000288 RID: 648 RVA: 0x00014198 File Offset: 0x00012398
	public static AutomationStep ConsoleCmd(string command)
	{
		return new AutomationStep
		{
			type = AutomationStep.StepType.ConsoleCmd,
			text = command
		};
	}

	// Token: 0x06000289 RID: 649 RVA: 0x000141B0 File Offset: 0x000123B0
	public string Describe(int index)
	{
		string text;
		switch (this.type)
		{
		case AutomationStep.StepType.LoadGame:
			text = string.Concat(new string[]
			{
				"LoadGame     world='",
				this.world,
				"'  game='",
				this.gameName,
				"'"
			});
			break;
		case AutomationStep.StepType.PreparePlayer:
			text = "PreparePlayer  (god + fly + freeze time)";
			break;
		case AutomationStep.StepType.Teleport:
			text = "Teleport     pos=" + AutomationStep.Fmt(this.position) + "  look=" + AutomationStep.Fmt(this.lookDir);
			break;
		case AutomationStep.StepType.Wait:
			text = string.Format("Wait         {0:F2}s", this.duration);
			break;
		case AutomationStep.StepType.MoveLine:
			text = (this.hasStart ? string.Format("MoveLine  {0} -> {1}  over {2:F2}s", AutomationStep.Fmt(this.positionB), AutomationStep.Fmt(this.position), this.duration) : string.Format("MoveLine  →  {0}  over {1:F2}s", AutomationStep.Fmt(this.position), this.duration));
			break;
		case AutomationStep.StepType.Orbit:
			text = string.Format("Orbit     around {0}  for {1:F2}s  lookForward={2}  isFlipped={3}", new object[]
			{
				AutomationStep.Fmt(this.position),
				this.duration,
				this.lookForward,
				this.isFlipped
			});
			break;
		case AutomationStep.StepType.WaitForChunksLoaded:
			text = "WaitForChunksLoaded  (wait until chunks around player are displayed)";
			break;
		case AutomationStep.StepType.StartPerfSession:
			text = string.Format("StartPerfSession  runs={0}", this.runCount);
			break;
		case AutomationStep.StepType.StopPerfSession:
			text = "StopPerfSession";
			break;
		case AutomationStep.StepType.StartPerfCapture:
			text = string.Format("  StartPerfCapture  fps={0}  prefix='{1}'", this.targetFps, this.capturePrefix);
			break;
		case AutomationStep.StepType.StopPerfCapture:
			text = "  StopPerfCapture";
			break;
		case AutomationStep.StepType.ExitToMenu:
			text = "ExitToMenu  (disconnect and return to main menu)";
			break;
		case AutomationStep.StepType.Cleanup:
			text = "Cleanup  (unregister event handlers)";
			break;
		case AutomationStep.StepType.AutomationComplete:
			text = "AutomationComplete  url='" + this.url + "'";
			break;
		case AutomationStep.StepType.LogError:
			text = "LogError text='" + this.text + "'";
			break;
		case AutomationStep.StepType.DeleteSave:
			text = string.Concat(new string[]
			{
				"DeleteSave     world='",
				this.world,
				"'  game='",
				this.gameName,
				"'"
			});
			break;
		case AutomationStep.StepType.ConsoleCmd:
			text = "ConsoleCmd     command='" + this.text + "'";
			break;
		case AutomationStep.StepType.MovePingPong:
			text = string.Format("MovePingPong  {0} ↔ {1}  ×{2}  ({3:F2}s/leg)", new object[]
			{
				AutomationStep.Fmt(this.position),
				AutomationStep.Fmt(this.positionB),
				this.pingPongCount,
				this.duration
			});
			break;
		default:
			text = string.Format("??? Unknown type: '{0}'", this.type);
			break;
		}
		string arg = text;
		return string.Format("  [{0:D2}] {1}", index, arg);
	}

	// Token: 0x0600028A RID: 650 RVA: 0x000144B6 File Offset: 0x000126B6
	[PublicizedFrom(EAccessModifier.Private)]
	public static string Fmt(Vector3 v)
	{
		return string.Format("({0:F1}, {1:F1}, {2:F1})", v.x, v.y, v.z);
	}

	// Token: 0x0400032B RID: 811
	[JsonConverter(typeof(StringEnumConverter))]
	public AutomationStep.StepType type;

	// Token: 0x0400032C RID: 812
	public string world;

	// Token: 0x0400032D RID: 813
	public string gameName;

	// Token: 0x0400032E RID: 814
	public Vector3 position;

	// Token: 0x0400032F RID: 815
	public Vector3 positionB;

	// Token: 0x04000330 RID: 816
	public Vector3 lookDir;

	// Token: 0x04000331 RID: 817
	public float duration;

	// Token: 0x04000332 RID: 818
	public bool lookForward = true;

	// Token: 0x04000333 RID: 819
	public bool isFlipped;

	// Token: 0x04000334 RID: 820
	public bool hasStart;

	// Token: 0x04000335 RID: 821
	public int runCount = 1;

	// Token: 0x04000336 RID: 822
	public int pingPongCount = 1;

	// Token: 0x04000337 RID: 823
	public int targetFps = 30;

	// Token: 0x04000338 RID: 824
	public string capturePrefix = string.Empty;

	// Token: 0x04000339 RID: 825
	public string url = string.Empty;

	// Token: 0x0400033A RID: 826
	public string text = string.Empty;

	// Token: 0x02000083 RID: 131
	public enum StepType
	{
		// Token: 0x0400033C RID: 828
		LoadGame,
		// Token: 0x0400033D RID: 829
		PreparePlayer,
		// Token: 0x0400033E RID: 830
		Teleport,
		// Token: 0x0400033F RID: 831
		Wait,
		// Token: 0x04000340 RID: 832
		MoveLine,
		// Token: 0x04000341 RID: 833
		Orbit,
		// Token: 0x04000342 RID: 834
		WaitForChunksLoaded,
		// Token: 0x04000343 RID: 835
		StartPerfSession,
		// Token: 0x04000344 RID: 836
		StopPerfSession,
		// Token: 0x04000345 RID: 837
		StartPerfCapture,
		// Token: 0x04000346 RID: 838
		StopPerfCapture,
		// Token: 0x04000347 RID: 839
		ExitToMenu,
		// Token: 0x04000348 RID: 840
		Cleanup,
		// Token: 0x04000349 RID: 841
		AutomationComplete,
		// Token: 0x0400034A RID: 842
		LogError,
		// Token: 0x0400034B RID: 843
		DeleteSave,
		// Token: 0x0400034C RID: 844
		ConsoleCmd,
		// Token: 0x0400034D RID: 845
		MovePingPong
	}
}
