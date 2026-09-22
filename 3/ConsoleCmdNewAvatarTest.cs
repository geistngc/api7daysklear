using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200024C RID: 588
[Preserve]
public class ConsoleCmdNewAvatarTest : ConsoleCmdAbstract
{
	// Token: 0x06001194 RID: 4500 RVA: 0x0006F14F File Offset: 0x0006D34F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Test new HD stuff.";
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x0006F156 File Offset: 0x0006D356
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  1. na\n";
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x0006F15D File Offset: 0x0006D35D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"na"
		};
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x0006F16D File Offset: 0x0006D36D
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		Log.Warning("No New Avatar!");
	}
}
