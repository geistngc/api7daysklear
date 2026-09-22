using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020002A0 RID: 672
[Preserve]
public class ConsoleCmdSpectatorMode : ConsoleCmdAbstract
{
	// Token: 0x0600138B RID: 5003 RVA: 0x000779E8 File Offset: 0x00075BE8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "enables/disables spectator mode";
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x000779EF File Offset: 0x00075BEF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"spectator",
			"spectatormode",
			"sm"
		};
	}

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x0600138D RID: 5005 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x00077A10 File Offset: 0x00075C10
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.IsDedicatedServer)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Cannot execute spectatormode.");
		}
		if (_params.Count != 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid arguments");
			return;
		}
		EntityPlayer player = XUiM_Player.GetPlayer();
		if (player != null)
		{
			player.IsSpectator = !player.IsSpectator;
			player.bPlayerStatsChanged = true;
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Spectator Mode: " + player.IsSpectator.ToString());
		}
	}
}
