using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020002AD RID: 685
[Preserve]
public class ConsoleCmdTeleportPoi : ConsoleCmdAbstract
{
	// Token: 0x060013CE RID: 5070 RVA: 0x00078DBC File Offset: 0x00076FBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"tppoi"
		};
	}

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x060013CF RID: 5071 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x00078DCC File Offset: 0x00076FCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Open POI Teleporter window";
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x00078DD4 File Offset: 0x00076FD4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be used on game clients");
			return;
		}
		if (_params.Count == 0)
		{
			GUIWindowConsole.Close();
			LocalPlayerUI.GetUIForPrimaryPlayer().windowManager.Open(XUiC_PoiTeleportMenu.ID, true);
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Not implemented yet");
	}
}
