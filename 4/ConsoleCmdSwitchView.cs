using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020002A9 RID: 681
[Preserve]
public class ConsoleCmdSwitchView : ConsoleCmdAbstract
{
	// Token: 0x060013B8 RID: 5048 RVA: 0x0007868A File Offset: 0x0007688A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"switchview",
			"sv"
		};
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x000786A2 File Offset: 0x000768A2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Switch between fpv and tpv";
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x000786AC File Offset: 0x000768AC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.Instance.World.GetPrimaryPlayer() == null)
		{
			return;
		}
		GameManager.Instance.World.GetPrimaryPlayer().SwitchFirstPersonViewFromInput();
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Switched to " + (GameManager.Instance.World.GetPrimaryPlayer().bFirstPersonView ? "FPV" : "TPV"));
	}
}
