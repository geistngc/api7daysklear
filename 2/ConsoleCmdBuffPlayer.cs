using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001EE RID: 494
[Preserve]
public class ConsoleCmdBuffPlayer : ConsoleCmdAbstract
{
	// Token: 0x06000F3F RID: 3903 RVA: 0x00063880 File Offset: 0x00061A80
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Apply a buff to a player";
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x00063887 File Offset: 0x00061A87
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n   buffplayer <player name / steam id / entity id> <buff name>\nApply the given buff to the player given by the player name or entity id (as given by e.g. \"lpi\").";
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0006388E File Offset: 0x00061A8E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"buffplayer"
		};
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x000638A0 File Offset: 0x00061AA0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid arguments, requires a target player and a buff name");
			ConsoleCmdBuff.PrintAvailableBuffNames();
			return;
		}
		string str = _params[1];
		ClientInfo clientInfo = ConsoleHelper.ParseParamIdOrName(_params[0], true, false);
		if (clientInfo != null)
		{
			clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup("buff " + str, true));
			return;
		}
		if (_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Use the \"buff\" command for the local player.");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Playername or entity ID not found.");
	}
}
