using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001E3 RID: 483
[Preserve]
public class ConsoleCmdAIDirectorSpawnAirDrop : ConsoleCmdAbstract
{
	// Token: 0x06000EEA RID: 3818 RVA: 0x00061227 File Offset: 0x0005F427
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"spawnairdrop"
		};
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x00061237 File Offset: 0x0005F437
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GameManager.Instance.World.aiDirector.GetComponent<AIDirectorAirDropComponent>().SpawnAirDrop();
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x00061253 File Offset: 0x0005F453
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Spawns an air drop";
	}
}
