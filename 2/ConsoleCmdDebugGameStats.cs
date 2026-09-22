using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000207 RID: 519
[Preserve]
public class ConsoleCmdDebugGameStats : ConsoleCmdAbstract
{
	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000FD7 RID: 4055 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x00066501 File Offset: 0x00064701
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debuggamestats"
		};
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x00066511 File Offset: 0x00064711
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "GameStats commands";
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x00066518 File Offset: 0x00064718
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count >= 1)
		{
			if (_params[0].ToUpper() == "LOG")
			{
				bool flag;
				if (_params.Count >= 2 && bool.TryParse(_params[1], out flag))
				{
					if (flag)
					{
						this.logFile = Path.Join(PlatformApplicationManager.Application.temporaryCachePath, "GameStats.tsv");
						this.stringBuilder = new StringBuilder();
						this.stringBuilder.AppendLine(DebugGameStats.GetHeader('\t'));
						File.AppendAllText(this.logFile, this.stringBuilder.ToString());
						DebugGameStats.StartStatisticsUpdate(new DebugGameStats.StatisticsUpdatedCallback(this.logDebugGameStats));
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Started logging debug stats to " + this.logFile + ".");
						return;
					}
					DebugGameStats.StopStatisticsUpdate();
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Stopped logging debug stats to " + this.logFile + ".");
					return;
				}
			}
			else if (_params[0].ToUpper() == "PRINT")
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(DebugGameStats.GetHeader(','));
				stringBuilder.AppendLine(DebugGameStats.GetCurrentStatsString(','));
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(stringBuilder.ToString());
				return;
			}
		}
		Log.Out("Incorrect params, expected 'log [true|false]' or 'print'");
	}

	// Token: 0x06000FDB RID: 4059 RVA: 0x00066674 File Offset: 0x00064874
	[PublicizedFrom(EAccessModifier.Private)]
	public void logDebugGameStats(Dictionary<string, string> statisticsDictionary)
	{
		this.stringBuilder.Clear();
		foreach (KeyValuePair<string, string> keyValuePair in statisticsDictionary)
		{
			this.stringBuilder.Append(keyValuePair.Value);
			this.stringBuilder.Append('\t');
		}
		this.stringBuilder.AppendLine();
		File.AppendAllText(this.logFile, this.stringBuilder.ToString());
	}

	// Token: 0x04000CAA RID: 3242
	[PublicizedFrom(EAccessModifier.Private)]
	public string logFile;

	// Token: 0x04000CAB RID: 3243
	[PublicizedFrom(EAccessModifier.Private)]
	public StringBuilder stringBuilder;
}
