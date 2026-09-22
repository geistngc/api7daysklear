using System;
using UnityEngine.Scripting;

// Token: 0x02000246 RID: 582
[Preserve]
public class ConsoleCmdMemCl : ConsoleCmdMem
{
	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06001170 RID: 4464 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x06001171 RID: 4465 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x0006ECF9 File Offset: 0x0006CEF9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"memcl"
		};
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x0006ED09 File Offset: 0x0006CF09
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Prints memory information on client and calls garbage collector";
	}
}
