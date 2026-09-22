using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200021D RID: 541
[Preserve]
public class ConsoleCmdGetLogfilePath : ConsoleCmdAbstract
{
	// Token: 0x0600106A RID: 4202 RVA: 0x00067BF8 File Offset: 0x00065DF8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"getlogpath",
			"glp"
		};
	}

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x0600106B RID: 4203 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x0600106C RID: 4204 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x0600106D RID: 4205 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x00067C10 File Offset: 0x00065E10
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Get the path of the logfile the game currently writes to";
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x00067C17 File Offset: 0x00065E17
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(Application.consoleLogPath);
	}
}
