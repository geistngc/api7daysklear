using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020002A7 RID: 679
[Preserve]
public class ConsoleCmdStab : ConsoleCmdAbstract
{
	// Token: 0x060013AE RID: 5038 RVA: 0x00078523 File Offset: 0x00076723
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"stab"
		};
	}

	// Token: 0x060013AF RID: 5039 RVA: 0x00078533 File Offset: 0x00076733
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "stability";
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x00032163 File Offset: 0x00030363
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "";
	}

	// Token: 0x060013B1 RID: 5041 RVA: 0x0007853C File Offset: 0x0007673C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Running stability");
		GameManager.Instance.CreateStabilityViewer();
		if (_params.Count == 0)
		{
			GameManager.Instance.stabilityViewer.StartSearch(100);
			return;
		}
		if (_params.Count != 1)
		{
			return;
		}
		if (_params[0].EqualsCaseInsensitive("Clear"))
		{
			GameManager.Instance.ClearStabilityViewer();
			return;
		}
		if (_params[0].EqualsCaseInsensitive("Redo"))
		{
			GameManager.Instance.stabilityViewer.StartSearch(100);
			return;
		}
		int asynCount = 31;
		int.TryParse(_params[0], out asynCount);
		GameManager.Instance.stabilityViewer.StartSearch(asynCount);
	}
}
