using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x0200020E RID: 526
[Preserve]
public class ConsoleCmdDecoMgr : ConsoleCmdAbstract
{
	// Token: 0x06001006 RID: 4102 RVA: 0x00066A2B File Offset: 0x00064C2B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"decomgr"
		};
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x00066A3B File Offset: 0x00064C3B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "\"decomgr\": Saves a debug texture visualising the DecoOccupiedMap.\n\"decomgr state\": Saves a debug texture visualising the location/state of all of the DecoObjects saved in decorations.7dtd.";
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06001008 RID: 4104 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06001009 RID: 4105 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x00066A44 File Offset: 0x00064C44
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count > 0 && _params[0] == "state")
		{
			DecoManager.Instance.SaveStateDebugTexture(Path.Join(GameIO.GetApplicationTempPath(), "decostate.png"));
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Saved decostate.png to temp directory.");
			return;
		}
		DecoManager.Instance.SaveDebugTexture(Path.Join(GameIO.GetApplicationTempPath(), "deco.png"), false);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Saved deco.png to temp directory.");
	}
}
