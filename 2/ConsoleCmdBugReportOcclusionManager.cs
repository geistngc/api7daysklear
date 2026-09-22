using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001EF RID: 495
[Preserve]
public class ConsoleCmdBugReportOcclusionManager : ConsoleCmdAbstract
{
	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000F44 RID: 3908 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x00063929 File Offset: 0x00061B29
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"testoccreport",
			"toccr"
		};
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x00063941 File Offset: 0x00061B41
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Test the occlusion manager self reporting to backtrace, requires Backtrace to be enabled at build creation";
	}

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000F47 RID: 3911 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool AllowedInMainMenu
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x00063948 File Offset: 0x00061B48
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
		{
			return;
		}
		List<string> list;
		if (OcclusionManager.Instance.WriteListToDisk(out list))
		{
			BacktraceUtils.SendErrorReport("OcclusionManagerUsedUpAllEntries", "Occlusion Manager used all entries", list, null);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("testoccreport: Wrote Files to disk: " + string.Join(",", list));
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("testoccreport: Posted report to backtrace");
		}
	}
}
