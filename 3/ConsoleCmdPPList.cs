using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000265 RID: 613
[Preserve]
public class ConsoleCmdPPList : ConsoleCmdAbstract
{
	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x06001233 RID: 4659 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001234 RID: 4660 RVA: 0x00071E30 File Offset: 0x00070030
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"pplist"
		};
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x00071E40 File Offset: 0x00070040
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(persistentPlayers.Players.Count.ToString() + " Persistent Player(s)");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in persistentPlayers.Players)
		{
			SdtdConsole instance = SingletonMonoBehaviour<SdtdConsole>.Instance;
			string str = "   ";
			PlatformUserIdentifierAbs key = keyValuePair.Key;
			instance.Output(str + ((key != null) ? key.ToString() : null) + " -> " + keyValuePair.Value.EntityId.ToString());
		}
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x00071EF8 File Offset: 0x000700F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Lists all PersistentPlayer data";
	}
}
