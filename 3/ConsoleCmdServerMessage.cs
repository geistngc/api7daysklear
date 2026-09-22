using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000283 RID: 643
[Preserve]
public class ConsoleCmdServerMessage : ConsoleCmdAbstract
{
	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060012E0 RID: 4832 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x00075328 File Offset: 0x00073528
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"say"
		};
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x00075338 File Offset: 0x00073538
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Sends a message to all connected clients";
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x00075340 File Offset: 0x00073540
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 1, found " + _params.Count.ToString() + ".");
			return;
		}
		string msg = _params[0];
		GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, msg, null, EMessageSender.Server, GeneratedTextManager.BbCodeSupportMode.Supported);
	}
}
