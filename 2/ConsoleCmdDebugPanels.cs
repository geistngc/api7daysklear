using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200020A RID: 522
[Preserve]
public class ConsoleCmdDebugPanels : ConsoleCmdAbstract
{
	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000FE9 RID: 4073 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x00066867 File Offset: 0x00064A67
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debugpanels"
		};
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x00066877 File Offset: 0x00064A77
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "debugpanels - toggle display, enabling also enables the debug menu\r\ndebugpanels [names] - set these panels active and disable all others, uses the short name of the panel (e.g. debugpanels Ply Ge Sp)";
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x00066880 File Offset: 0x00064A80
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, true);
		NGuiWdwDebugPanels nguiWdwDebugPanels = UnityEngine.Object.FindAnyObjectByType<NGuiWdwDebugPanels>();
		if (nguiWdwDebugPanels == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Cannot find debug panel controller");
			return;
		}
		if (_params.Count == 0)
		{
			nguiWdwDebugPanels.ToggleDisplay();
			return;
		}
		nguiWdwDebugPanels.ShowGeneralData();
		nguiWdwDebugPanels.SetActivePanels(_params.ToArray());
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x000668D5 File Offset: 0x00064AD5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "allows usage of debug display panels (F3 menu) via command console";
	}
}
