using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000211 RID: 529
[Preserve]
public class ConsoleCmdDynamicProperties : ConsoleCmdAbstract
{
	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06001018 RID: 4120 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06001019 RID: 4121 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x00066E66 File Offset: 0x00065066
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"dynamicproperties",
			"dprop"
		};
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x00066E80 File Offset: 0x00065080
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params[0] == "block")
		{
			if (_params.Count == 1)
			{
				Debug.LogError("Needs sub-command - cachestats");
				return;
			}
			if (_params[1] == "cachestats")
			{
				Block.CacheStats();
			}
		}
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x00066ECC File Offset: 0x000650CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Dynamic Properties debugging";
	}
}
