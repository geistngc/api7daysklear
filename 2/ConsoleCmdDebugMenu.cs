using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000209 RID: 521
[Preserve]
public class ConsoleCmdDebugMenu : ConsoleCmdAbstract
{
	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000FE4 RID: 4068 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x0006680C File Offset: 0x00064A0C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debugmenu",
			"dm"
		};
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x00066824 File Offset: 0x00064A24
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GamePrefs.Set(EnumGamePrefs.DebugMenuEnabled, !GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled));
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("debugmenu " + (GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) ? "on" : "off"));
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x00066860 File Offset: 0x00064A60
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "enables/disables the debugmenu ";
	}
}
