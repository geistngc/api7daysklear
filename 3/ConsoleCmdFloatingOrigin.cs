using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000217 RID: 535
[Preserve]
public class ConsoleCmdFloatingOrigin : ConsoleCmdAbstract
{
	// Token: 0x06001040 RID: 4160 RVA: 0x0006748F File Offset: 0x0006568F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"floatingorigin",
			"fo"
		};
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x00032163 File Offset: 0x00030363
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "";
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x00032163 File Offset: 0x00030363
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "";
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06001043 RID: 4163 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06001044 RID: 4164 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x000674A8 File Offset: 0x000656A8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 1 && _params[0] == "on")
		{
			if (Origin.Instance != null)
			{
				Origin.Instance.isAuto = true;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Set floating origin to on");
			return;
		}
		if (_params.Count == 1 && _params[0] == "off")
		{
			if (Origin.Instance != null)
			{
				Origin.Instance.isAuto = false;
				if (GameManager.Instance.World != null)
				{
					Origin.Instance.Reposition(Vector3.zero);
				}
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Set floating origin to off");
			return;
		}
		if (Origin.Instance != null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Floating origin is " + (Origin.Instance.isAuto ? "on" : "off") + " and is at position " + Origin.position.ToCultureInvariantString());
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No FO instance!");
	}
}
