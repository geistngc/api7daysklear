using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001E6 RID: 486
[Preserve]
public class ConsoleCmdAIDirectorSpawnSupplyCrate : ConsoleCmdAbstract
{
	// Token: 0x06000EF8 RID: 3832 RVA: 0x000615BE File Offset: 0x0005F7BE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"spawnsupplycrate"
		};
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x000615D0 File Offset: 0x0005F7D0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		World world = GameManager.Instance.World;
		EntityPlayer entityPlayer;
		if (_senderInfo.IsLocalGame)
		{
			entityPlayer = world.GetPrimaryPlayer();
		}
		else
		{
			if (_senderInfo.RemoteClientInfo == null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command RemoteClientInfo null");
				return;
			}
			entityPlayer = world.Players.dict[_senderInfo.RemoteClientInfo.entityId];
		}
		Vector3 position = entityPlayer.position;
		position.y += 8f;
		Entity entity = EntityFactory.CreateEntity(EntityClass.FromString("sc_General"), position, new Vector3(0f, world.GetGameRandom().RandomFloat * 360f, 0f));
		world.SpawnEntityInWorld(entity);
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x0006167C File Offset: 0x0005F87C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Spawns a supply crate where the player is";
	}
}
