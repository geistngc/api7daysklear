using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000239 RID: 569
[Preserve]
public class ConsoleCmdLogEnvironment : ConsoleCmdAbstract
{
	// Token: 0x0600111A RID: 4378 RVA: 0x0006CB23 File Offset: 0x0006AD23
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"logenv"
		};
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x0600111B RID: 4379 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x0006CB33 File Offset: 0x0006AD33
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Log the process environment variables";
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x00032163 File Offset: 0x00030363
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "";
	}

	// Token: 0x0600111E RID: 4382 RVA: 0x0006CB3C File Offset: 0x0006AD3C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		foreach (object obj in Environment.GetEnvironmentVariables())
		{
			DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
			string text = dictionaryEntry.Key as string;
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = dictionaryEntry.Value as string;
				Log.Out(string.Concat(new string[]
				{
					"Environment variable '",
					text,
					"' = '",
					text2,
					"'"
				}));
			}
		}
	}
}
