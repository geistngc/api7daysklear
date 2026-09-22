using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200021C RID: 540
[Preserve]
public class ConsoleCmdGetGameStats : ConsoleCmdAbstract
{
	// Token: 0x06001062 RID: 4194 RVA: 0x00067A8C File Offset: 0x00065C8C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool prefAccessAllowed(EnumGameStats gp)
	{
		string text = gp.ToStringCached<EnumGameStats>();
		foreach (string value in this.forbiddenPrefs)
		{
			if (text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x00067AC7 File Offset: 0x00065CC7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"getgamestat",
			"ggs"
		};
	}

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06001064 RID: 4196 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06001065 RID: 4197 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x00067ADF File Offset: 0x00065CDF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Gets game stats";
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x00067AE6 File Offset: 0x00065CE6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Get all game stats or only those matching a given substring";
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x00067AF0 File Offset: 0x00065CF0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		string text = null;
		if (_params.Count > 0)
		{
			text = _params[0];
		}
		SortedList<string, string> sortedList = new SortedList<string, string>();
		foreach (EnumGameStats enumGameStats in EnumUtils.Values<EnumGameStats>())
		{
			if ((string.IsNullOrEmpty(text) || enumGameStats.ToStringCached<EnumGameStats>().ContainsCaseInsensitive(text)) && this.prefAccessAllowed(enumGameStats))
			{
				sortedList.Add(enumGameStats.ToStringCached<EnumGameStats>(), string.Format("GameStat.{0} = {1}", enumGameStats.ToStringCached<EnumGameStats>(), GameStats.GetObject(enumGameStats)));
			}
		}
		foreach (string key in sortedList.Keys)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(sortedList[key]);
		}
	}

	// Token: 0x04000CB6 RID: 3254
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] forbiddenPrefs = new string[]
	{
		"last"
	};
}
