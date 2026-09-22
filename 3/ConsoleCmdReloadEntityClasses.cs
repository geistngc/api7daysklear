using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000277 RID: 631
[Preserve]
public class ConsoleCmdReloadEntityClasses : ConsoleCmdAbstract
{
	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x060012AA RID: 4778 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x000749EF File Offset: 0x00072BEF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"reloadentityclasses",
			"rec"
		};
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x00074A07 File Offset: 0x00072C07
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "reloads entityclasses xml data.";
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x00074A0E File Offset: 0x00072C0E
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		WorldStaticData.Reset("entityclasses");
		WorldStaticData.Reset("entitybandits");
	}
}
