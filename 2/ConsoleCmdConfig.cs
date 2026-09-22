using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x020001FD RID: 509
[Preserve]
public class ConsoleCmdConfig : ConsoleCmdAbstract
{
	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000F9C RID: 3996 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x00065748 File Offset: 0x00063948
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"config"
		};
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x00065758 File Offset: 0x00063958
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Import/export config data from/to external file";
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x0006575F File Offset: 0x0006395F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Imports/exports config data from/to external file\nUsage:\n   config import [filename]\n   config export [filename]\n";
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x00065768 File Offset: 0x00063968
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		try
		{
			string text = _params[1].ToLower();
			if (!string.IsNullOrEmpty(text))
			{
				string a = _params[0];
				if (!(a == "import"))
				{
					if (a == "export")
					{
						if (File.Exists(text))
						{
							File.Delete(text);
						}
						File.WriteAllText(text, GameOptionsControls.Export());
					}
				}
				else if (File.Exists(text))
				{
					GameOptionsControls.Import(File.ReadAllText(text));
				}
			}
		}
		catch
		{
		}
	}
}
