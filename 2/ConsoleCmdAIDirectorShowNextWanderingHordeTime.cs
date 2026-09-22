using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001E2 RID: 482
[Preserve]
public class ConsoleCmdAIDirectorShowNextWanderingHordeTime : ConsoleCmdAbstract
{
	// Token: 0x06000EE6 RID: 3814 RVA: 0x000611E4 File Offset: 0x0005F3E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"shownexthordetime"
		};
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x000611F4 File Offset: 0x0005F3F4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.Instance.World.aiDirector != null)
		{
			GameManager.Instance.World.aiDirector.GetComponent<AIDirectorWanderingHordeComponent>().LogTimes();
		}
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x00061220 File Offset: 0x0005F420
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Displays the wandering horde time";
	}
}
