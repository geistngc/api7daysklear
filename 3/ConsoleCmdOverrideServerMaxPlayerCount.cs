using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000250 RID: 592
[Preserve]
public class ConsoleCmdOverrideServerMaxPlayerCount : ConsoleCmdAbstract
{
	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x060011AA RID: 4522 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x0006F43A File Offset: 0x0006D63A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"overridemaxplayercount"
		};
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x0006F44A File Offset: 0x0006D64A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Override Max Server Player Count";
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x0006F454 File Offset: 0x0006D654
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		int num;
		if (_params.Count >= 1 && int.TryParse(_params[0], out num))
		{
			GameModeSurvival.OverrideMaxPlayerCount = num;
			Log.Out(string.Format("Survival Max Player Count Override set to {0}", num));
			return;
		}
		Log.Out("Incorrect param, expected an integer for max player count.");
	}
}
