using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000255 RID: 597
[Preserve]
public class ConsoleCmdPerformanceProfiler : ConsoleCmdAbstract
{
	// Token: 0x060011C2 RID: 4546 RVA: 0x0006FA49 File Offset: 0x0006DC49
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"performanceprofiler",
			"pp"
		};
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x0006FA61 File Offset: 0x0006DC61
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Performance Profiling Utility";
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x0006FA68 File Offset: 0x0006DC68
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Type 'pp' to toggle capture, or 'pp start [fps]' / 'pp stop'.";
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x060011C5 RID: 4549 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x060011C6 RID: 4550 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x060011C7 RID: 4551 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x0006FA70 File Offset: 0x0006DC70
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			this.Toggle();
			return;
		}
		string a = _params[0].ToLower();
		if (a == "start")
		{
			this.CmdStart(_params);
			return;
		}
		if (!(a == "stop"))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown sub-command '" + _params[0] + "'. See 'help pp'.");
			return;
		}
		this.CmdStop();
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x0006FAE4 File Offset: 0x0006DCE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Toggle()
	{
		if (PerformanceProfiler.IsCapturing())
		{
			Log.Out(string.Format("[PerformanceProfiler] Stopping after {0} frames.", PerformanceProfiler.GetCurrentFrameCount()));
			PerformanceProfiler.StopCapture(true);
			return;
		}
		Log.Out("[PerformanceProfiler] Starting capture (30 FPS target).");
		PerformanceProfiler.StartCapture(null, null, 30);
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x0006FB24 File Offset: 0x0006DD24
	[PublicizedFrom(EAccessModifier.Private)]
	public void CmdStart(List<string> p)
	{
		if (PerformanceProfiler.IsCapturing())
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Capture already running.");
			return;
		}
		int num2;
		int num = (p.Count > 1 && int.TryParse(p[1], out num2)) ? num2 : 30;
		PerformanceProfiler.StartCapture(null, null, num);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Capture started at {0} FPS target.", num));
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x0006FB89 File Offset: 0x0006DD89
	[PublicizedFrom(EAccessModifier.Private)]
	public void CmdStop()
	{
		if (!PerformanceProfiler.IsCapturing())
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No capture in progress.");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Stopping capture after {0} frames.", PerformanceProfiler.GetCurrentFrameCount()));
		PerformanceProfiler.StopCapture(true);
	}
}
