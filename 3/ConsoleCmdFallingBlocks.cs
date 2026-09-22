using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000216 RID: 534
[Preserve]
public class ConsoleCmdFallingBlocks : ConsoleCmdAbstract
{
	// Token: 0x06001036 RID: 4150 RVA: 0x00067403 File Offset: 0x00065603
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"fallingblocks",
			"fb"
		};
	}

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06001037 RID: 4151 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x06001038 RID: 4152 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x0006741B File Offset: 0x0006561B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "FallingBlocks WIP Settings";
	}

	// Token: 0x1700017B RID: 379
	// (get) Token: 0x0600103A RID: 4154 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x0600103B RID: 4155 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x0600103C RID: 4156 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x00067424 File Offset: 0x00065624
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params[0].ToLowerInvariant() == "enable")
		{
			EntityFallingBlocks.Enabled = !EntityFallingBlocks.Enabled;
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("EntityFallingBlocks Feature Enabled: {0}", EntityFallingBlocks.Enabled));
			return;
		}
	}

	// Token: 0x04000CB1 RID: 3249
	public static bool ClientVerification = true;

	// Token: 0x04000CB2 RID: 3250
	public static bool AllowSendToPeer = true;

	// Token: 0x04000CB3 RID: 3251
	public static bool AllowReceiveFromPeer = true;

	// Token: 0x04000CB4 RID: 3252
	public static bool DisablePeerAction = false;
}
