using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using UnityEngine.Scripting;

// Token: 0x02000272 RID: 626
[Preserve]
public class ConsoleCmdProfiling : ConsoleCmdAbstract
{
	// Token: 0x170001EC RID: 492
	// (get) Token: 0x0600128D RID: 4749 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00074212 File Offset: 0x00072412
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"profiling"
		};
	}

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x0600128F RID: 4751 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x00074224 File Offset: 0x00072424
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (this.cmdNetwork == null)
		{
			this.cmdNetwork = (SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand("profilenetwork", true) as ConsoleCmdProfileNetwork);
		}
		if (_params.Count != 1 || !_params[0].EqualsCaseInsensitive("stop"))
		{
			if (!Profiler.enabled)
			{
				this.profileNetwork = true;
				int num = 300;
				if (_params.Count > 0 && !int.TryParse(_params[0], out num))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Not a valid integer for number of frames (\"{0}\")", num));
					return;
				}
				if (num < 10 || num > 3000)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Number of frames needs to be within {0} and {1}", 10, 3000));
					return;
				}
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Enabled profiling for {0} frames (typically 5 - 10 seconds)", num));
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Profiler mem: {0}", Profiler.maxUsedMemory));
				Profiler.logFile = string.Format("{0}/profiling_{1:yyyy-MM-dd_HH-mm-ss}_unity.log", GameIO.GetApplicationPath(), DateTime.Now);
				Profiler.enableBinaryLog = true;
				Profiler.enabled = true;
				ThreadManager.StartCoroutine(this.stopProfilingLater(num));
				if (this.profileNetwork)
				{
					ConsoleCmdProfileNetwork consoleCmdProfileNetwork = this.cmdNetwork;
					if (consoleCmdProfileNetwork == null)
					{
						return;
					}
					consoleCmdProfileNetwork.resetData();
				}
			}
			return;
		}
		if (!Profiler.enabled)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Profiling not running.");
			return;
		}
		this.stopProfiling();
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Profiling stopped.");
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x000743A2 File Offset: 0x000725A2
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator stopProfilingLater(int _frames)
	{
		int i = 0;
		while (i < _frames && Profiler.enabled)
		{
			yield return null;
			int num = i;
			i = num + 1;
		}
		if (Profiler.enabled)
		{
			this.stopProfiling();
			Log.Out("Profiling done");
		}
		yield break;
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x000743B8 File Offset: 0x000725B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void stopProfiling()
	{
		Profiler.enabled = false;
		Profiler.logFile = null;
		if (this.profileNetwork)
		{
			ConsoleCmdProfileNetwork consoleCmdProfileNetwork = this.cmdNetwork;
			if (consoleCmdProfileNetwork == null)
			{
				return;
			}
			consoleCmdProfileNetwork.doProfileNetwork();
		}
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x000743DE File Offset: 0x000725DE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Enable Unity profiling for 300 frames";
	}

	// Token: 0x04000D48 RID: 3400
	[PublicizedFrom(EAccessModifier.Private)]
	public ConsoleCmdProfileNetwork cmdNetwork;

	// Token: 0x04000D49 RID: 3401
	[PublicizedFrom(EAccessModifier.Private)]
	public bool profileNetwork;

	// Token: 0x04000D4A RID: 3402
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FramesDefault = 300;

	// Token: 0x04000D4B RID: 3403
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FramesMin = 10;

	// Token: 0x04000D4C RID: 3404
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FramesMax = 3000;
}
