using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200023A RID: 570
[Preserve]
public class ConsoleCmdLogFellThroughWorldDebugInfo : ConsoleCmdAbstract
{
	// Token: 0x06001120 RID: 4384 RVA: 0x0006CBE4 File Offset: 0x0006ADE4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"ftw",
			"fellthroughworld"
		};
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x0006CBFC File Offset: 0x0006ADFC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Log the fell through world debug information for testing purposes.";
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06001122 RID: 4386 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001123 RID: 4387 RVA: 0x0006CC03 File Offset: 0x0006AE03
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GameManager.Instance.World.GetPrimaryPlayer().LogFellThroughWorldDebugInfo();
	}
}
