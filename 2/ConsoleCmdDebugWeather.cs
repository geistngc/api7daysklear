using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200020D RID: 525
[Preserve]
public class ConsoleCmdDebugWeather : ConsoleCmdAbstract
{
	// Token: 0x06000FFE RID: 4094 RVA: 0x000669B0 File Offset: 0x00064BB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debugweather"
		};
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000FFF RID: 4095 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06001000 RID: 4096 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06001001 RID: 4097 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x000669C0 File Offset: 0x00064BC0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Dumps internal weather state to the console.";
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06001003 RID: 4099 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x000669C8 File Offset: 0x00064BC8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count > 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: debugweather");
			return;
		}
		EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Outside Temp: " + primaryPlayer.PlayerStats.CoreTemp.ToCultureInvariantString());
		}
	}
}
