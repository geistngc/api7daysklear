using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000335 RID: 821
[Preserve]
public class ConsoleCmdDiscord : ConsoleCmdAbstract
{
	// Token: 0x060017CE RID: 6094 RVA: 0x00089310 File Offset: 0x00087510
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"discord",
			"dc"
		};
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x060017CF RID: 6095 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x060017D0 RID: 6096 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x060017D1 RID: 6097 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x060017D2 RID: 6098 RVA: 0x00089328 File Offset: 0x00087528
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Toggle Discord debug window";
	}

	// Token: 0x060017D3 RID: 6099 RVA: 0x0008932F File Offset: 0x0008752F
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		LocalPlayerUI.primaryUI.windowManager.SwitchVisible(XUiC_DiscordWindow.ID, false);
	}
}
