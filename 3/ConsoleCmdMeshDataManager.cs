using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000248 RID: 584
[Preserve]
public class ConsoleCmdMeshDataManager : ConsoleCmdAbstract
{
	// Token: 0x170001BE RID: 446
	// (get) Token: 0x0600117A RID: 4474 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x0600117B RID: 4475 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x0600117C RID: 4476 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x0600117D RID: 4477 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x0600117E RID: 4478 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0006EE48 File Offset: 0x0006D048
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"meshdatamanager",
			"mdm"
		};
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0006EE60 File Offset: 0x0006D060
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Toggle the MeshDataManager";
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x0006EE67 File Offset: 0x0006D067
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "mdm";
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x0006EE6E File Offset: 0x0006D06E
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		MeshDataManager.Enabled = !MeshDataManager.Enabled;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("MeshDataManager " + (MeshDataManager.Enabled ? "enabled" : "disabled") + ".");
	}
}
