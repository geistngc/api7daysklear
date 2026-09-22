using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000290 RID: 656
[Preserve]
public class ConsoleCmdShowHits : ConsoleCmdAbstract
{
	// Token: 0x1700020C RID: 524
	// (get) Token: 0x0600132E RID: 4910 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x0600132F RID: 4911 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x000761C4 File Offset: 0x000743C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"showhits"
		};
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x000761D4 File Offset: 0x000743D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Show hit entity info";
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x000761DB File Offset: 0x000743DB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Commands:\ndamage - toggle damage numbers\nhits <time> <size> - toggle hits";
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x000761E4 File Offset: 0x000743E4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		string a = _params[0].ToLower();
		if (a == "damage")
		{
			DamageText.Enabled = !DamageText.Enabled;
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Damage " + (DamageText.Enabled ? "on" : "off"));
			return;
		}
		if (!(a == "hits"))
		{
			return;
		}
		EntityAlive.ShowDebugDisplayHit = !EntityAlive.ShowDebugDisplayHit;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Hits " + (EntityAlive.ShowDebugDisplayHit ? "on" : "off"));
		if (_params.Count >= 2)
		{
			EntityAlive.DebugDisplayHitTime = StringParsers.ParseFloat(_params[1], 0, -1, NumberStyles.Any);
		}
		if (_params.Count >= 3)
		{
			EntityAlive.DebugDisplayHitSize = StringParsers.ParseFloat(_params[2], 0, -1, NumberStyles.Any);
		}
		ItemAction.ShowDebugDisplayHit = EntityAlive.ShowDebugDisplayHit;
		ItemAction.DebugDisplayHitTime = EntityAlive.DebugDisplayHitTime;
		ItemAction.DebugDisplayHitSize = EntityAlive.DebugDisplayHitSize;
	}
}
