using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000231 RID: 561
[Preserve]
public class ConsoleCmdKill : ConsoleCmdAbstract
{
	// Token: 0x060010EE RID: 4334 RVA: 0x0006BBD5 File Offset: 0x00069DD5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"kill"
		};
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x0006BBE5 File Offset: 0x00069DE5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Kill a given entity";
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x0006BBEC File Offset: 0x00069DEC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Kill a given entity.\nUsage:\n   1. kill <entity id>\n   2. kill <player name / steam id>\n1. can be used to kill any entity that can be killed (zombies, players).\n2. can only be used to kill players.";
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x0006BBF4 File Offset: 0x00069DF4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 1, found " + _params.Count.ToString() + ".");
			return;
		}
		Entity entity = null;
		ClientInfo forNameOrId = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.GetForNameOrId(_params[0], true, false);
		int key;
		if (forNameOrId != null)
		{
			entity = GameManager.Instance.World.Players.dict[forNameOrId.entityId];
		}
		else if (int.TryParse(_params[0], out key) && GameManager.Instance.World.Entities.dict.ContainsKey(key))
		{
			entity = GameManager.Instance.World.Entities.dict[key];
		}
		if (entity == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Playername or entity id not found.");
			return;
		}
		entity.DamageEntity(new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Suicide), 99999, false, 1f);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Gave 99999 damage to entity " + _params[0]);
	}
}
