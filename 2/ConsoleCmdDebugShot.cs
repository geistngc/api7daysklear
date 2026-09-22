using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200020B RID: 523
[Preserve]
public class ConsoleCmdDebugShot : ConsoleCmdAbstract
{
	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000FEF RID: 4079 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x000668DC File Offset: 0x00064ADC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debugshot",
			"dbs"
		};
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x000668F4 File Offset: 0x00064AF4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GUIWindowConsole.Close();
		bool savePerks = _params.Count > 0 && StringParsers.ParseBool(_params[0], 0, -1, true);
		ThreadManager.StartCoroutine(this.openWindowLater(savePerks));
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x0006692F File Offset: 0x00064B2F
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator openWindowLater(bool _savePerks)
	{
		yield return null;
		GUIWindowScreenshotText.Open(LocalPlayerUI.primaryUI, _savePerks);
		yield break;
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x0006693E File Offset: 0x00064B3E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Creates a screenshot with some debug information";
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x00066945 File Offset: 0x00064B45
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  debugshot [save perks]\nLets you make a screenshot that will have some generic info\non it and a custom text you can enter. Also stores a list\nof your current perk levels, buffs and cvars in a CSV file\nnext to it if the optional parameter 'save perks' is set to true";
	}
}
