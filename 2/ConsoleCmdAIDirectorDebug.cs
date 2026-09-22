using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001E1 RID: 481
[Preserve]
public class ConsoleCmdAIDirectorDebug : ConsoleCmdAbstract
{
	// Token: 0x06000EE0 RID: 3808 RVA: 0x00061198 File Offset: 0x0005F398
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"aiddebug"
		};
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000EE1 RID: 3809 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x000611A8 File Offset: 0x0005F3A8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		AIDirectorConstants.DebugOutput = !AIDirectorConstants.DebugOutput;
		if (AIDirectorConstants.DebugOutput)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("AIDirector debug output is ON.");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("AIDirector debug output is OFF.");
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x000611DD File Offset: 0x0005F3DD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Toggles AIDirector debug output.";
	}
}
