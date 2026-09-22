using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200028A RID: 650
[Preserve]
public class ConsoleCmdSetWaterValue : ConsoleCmdAbstract
{
	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001309 RID: 4873 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00075A90 File Offset: 0x00073C90
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"setwatervalue",
			"swv"
		};
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x00075AA8 File Offset: 0x00073CA8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Sets the water value for all flow-permitting blocks within the current selection area, specified in the range of 0 (empty) to 1 (full).";
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x00075AAF File Offset: 0x00073CAF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "'swv [0.0 - 1.0]' Sets water value between empty (0.0) and full (1.0) for all flow-permitting blocks within the current selection bounds. \nE.g. 'swv 0.5' will set all affected blocks to be half-full. \nBlocks which do not permit flow are unchanged.";
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00075AB8 File Offset: 0x00073CB8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		float num;
		if (_params.Count != 1 || !float.TryParse(_params[0], out num) || num < 0f || num > 1f)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		BlockToolSelection instance = BlockToolSelection.Instance;
		if (!instance.SelectionActive)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No selection active. Running this command requires an active selection box.");
			return;
		}
		BlockTools.CubeWaterRPC(GameManager.Instance, instance.SelectionStart, instance.SelectionEnd, new WaterValue((int)(num * 19500f)));
	}
}
