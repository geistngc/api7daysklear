using System;
using UnityEngine.Scripting;

// Token: 0x0200024A RID: 586
[Preserve]
public class ConsoleCmdNetworkClient : ConsoleCmdNetworkServer
{
	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x0600118A RID: 4490 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x0006F007 File Offset: 0x0006D207
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"networkclient",
			"netc"
		};
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x0006F01F File Offset: 0x0006D21F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Client side network commands";
	}
}
