using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200029F RID: 671
[Preserve]
public class ConsoleCmdSpawnScreen : ConsoleCmdAbstract
{
	// Token: 0x06001386 RID: 4998 RVA: 0x00077928 File Offset: 0x00075B28
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"SpawnScreen"
		};
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x00077938 File Offset: 0x00075B38
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Display SpawnScreen";
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x0007793F File Offset: 0x00075B3F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "SpawnScreen on/off";
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x00077948 File Offset: 0x00075B48
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		foreach (EntityPlayerLocal entityPlayerLocal in GameManager.Instance.World.GetLocalPlayers())
		{
			if (entityPlayerLocal)
			{
				entityPlayerLocal.spawnInTime = Time.time;
				entityPlayerLocal.bPlayingSpawnIn = true;
				if (_params.Count > 0)
				{
					int num = 0;
					if (int.TryParse(_params[0], out num))
					{
						EntityPlayerLocal.spawnInEffectSpeed = (float)num;
					}
				}
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("SpawnEffect initiated...");
			}
		}
	}
}
