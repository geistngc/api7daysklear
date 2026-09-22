using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020002B9 RID: 697
[Preserve]
public class ConsoleCommandUnlockInventories : ConsoleCmdAbstract
{
	// Token: 0x17000232 RID: 562
	// (get) Token: 0x06001417 RID: 5143 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06001418 RID: 5144 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsExecuteOnClient
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x0007A310 File Offset: 0x00078510
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Force unlock inventories for everyone or a specific player.";
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x0007A317 File Offset: 0x00078517
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  unlock\n  unlock <player id>\n";
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x0007A31E File Offset: 0x0007851E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"unlock"
		};
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x0007A330 File Offset: 0x00078530
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		int num = -1;
		if (_params.Count == 2 && !int.TryParse(_params[1], out num))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Failed to parse int for player id.");
			return;
		}
		if (num == -1)
		{
			LockManager.Instance.ForceUnlockAll();
			return;
		}
		LockManager.Instance.ForceUnlockByPlayer(num);
	}
}
