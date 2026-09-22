using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001E4 RID: 484
[Preserve]
public class ConsoleCmdAIDirectorSpawnHorde : ConsoleCmdAbstract
{
	// Token: 0x06000EEE RID: 3822 RVA: 0x0006125A File Offset: 0x0005F45A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"spawnwandering",
			"spawnw"
		};
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x00061272 File Offset: 0x0005F472
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Spawn wandering entities";
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x00061279 File Offset: 0x0005F479
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Commands:\nb - bandits\nh - horde";
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x00061280 File Offset: 0x0005F480
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		string text = _params[0].ToLower();
		if (text == "b")
		{
			GameManager.Instance.World.aiDirector.GetComponent<AIDirectorWanderingHordeComponent>().StartSpawning(AIWanderingHordeSpawner.SpawnType.Bandits);
			return;
		}
		if (!(text == "h"))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown command " + text);
			return;
		}
		GameManager.Instance.World.aiDirector.GetComponent<AIDirectorWanderingHordeComponent>().StartSpawning(AIWanderingHordeSpawner.SpawnType.Horde);
	}
}
