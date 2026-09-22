using System;
using System.Collections.Generic;
using System.Text;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000228 RID: 552
[Preserve]
public class ConsoleCmdHelp : ConsoleCmdAbstract
{
	// Token: 0x060010B7 RID: 4279 RVA: 0x0006A3D3 File Offset: 0x000685D3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"help"
		};
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x060010B8 RID: 4280 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x060010B9 RID: 4281 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x060010BA RID: 4282 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x0006A3E3 File Offset: 0x000685E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Help on console and specific commands";
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x0006A3EA File Offset: 0x000685EA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "\r\n\t\t\t|Usage:\r\n\t\t\t|  1. help\r\n\t\t\t|  2. help * <searchstring>\r\n\t\t\t|  3. help <command name>\r\n\t\t\t|  4. help output\r\n\t\t\t|  5. help outputdetailed\r\n\t\t\t|1. Show general help and list all available commands\r\n\t\t\t|2. List commands where either the name or the description contains the given text\r\n\t\t\t|3. Show help for the given command\r\n\t\t\t|4. Write command list to log file\r\n\t\t\t|5. Write command list with help texts to log file\r\n\t\t\t".Unindent(true);
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x0006A3F8 File Offset: 0x000685F8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		ConsoleCmdHelp.sb.Clear();
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("*** Generic Console Help ***");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("To get further help on a specific topic or command type (without the brackets)");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("    help <topic / command>");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Generic notation of command parameters:");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("   <param name>              Required parameter");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("   <entityId / player name>  Possible types of parameter values");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("   [param name]              Optional parameter");
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("");
			ConsoleCmdHelp.sb.Clear();
			ConsoleCmdHelp.BuildStringCommandDescriptions();
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(ConsoleCmdHelp.sb.ToString());
			ConsoleCmdHelp.sb.Clear();
			return;
		}
		Action<List<string>> action = null;
		if (ConsoleCmdHelp.helpTopics.ContainsKey(_params[0]))
		{
			action = ConsoleCmdHelp.helpTopics[_params[0]].Action;
		}
		ConsoleCmdHelp.sb.Clear();
		ConsoleCmdHelp.BuildStringHelpText(_params[0]);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(ConsoleCmdHelp.sb.ToString());
		ConsoleCmdHelp.sb.Clear();
		if (action != null)
		{
			action(_params);
		}
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x0006A540 File Offset: 0x00068740
	public static void ValidateNoCommandOverlap()
	{
		IList<IConsoleCommand> commands = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommands();
		for (int i = 0; i < commands.Count; i++)
		{
			foreach (string text in commands[i].GetCommands())
			{
				if (ConsoleCmdHelp.helpTopics.ContainsKey(text))
				{
					Log.Warning("Command with alias \"" + text + "\" conflicts with help topic command");
				}
			}
		}
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x0006A5B0 File Offset: 0x000687B0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void BuildStringCommandDescriptions()
	{
		ConsoleCmdHelp.sb.AppendLine("*** List of Help Topics ***");
		foreach (KeyValuePair<string, ConsoleCmdHelp.HelpTopic> keyValuePair in ConsoleCmdHelp.helpTopics)
		{
			ConsoleCmdHelp.sb.Append(keyValuePair.Key);
			ConsoleCmdHelp.sb.Append(" => ");
			ConsoleCmdHelp.sb.AppendLine(keyValuePair.Value.Description);
		}
		ConsoleCmdHelp.sb.AppendLine("");
		ConsoleCmdHelp.sb.AppendLine("*** List of Commands ***");
		IList<IConsoleCommand> commands = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommands();
		for (int i = 0; i < commands.Count; i++)
		{
			IConsoleCommand consoleCommand = commands[i];
			if (consoleCommand.CanExecuteForDevice && consoleCommand != null)
			{
				foreach (string value in consoleCommand.GetCommands())
				{
					ConsoleCmdHelp.sb.Append(" ");
					ConsoleCmdHelp.sb.Append(value);
				}
				ConsoleCmdHelp.sb.Append(" => ");
				ConsoleCmdHelp.sb.AppendLine(consoleCommand.GetDescription());
			}
		}
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x0006A6FC File Offset: 0x000688FC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void BuildStringHelpText(string key)
	{
		string text = null;
		string text2 = null;
		if (ConsoleCmdHelp.helpTopics.ContainsKey(key))
		{
			text = "Topic: " + key;
			text2 = ConsoleCmdHelp.helpTopics[key].ActionCompleteText;
		}
		else
		{
			IConsoleCommand command = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand(key, false);
			if (command != null && command.CanExecuteForDevice)
			{
				text = "Command(s): " + string.Join(", ", command.GetCommands());
				text2 = command.GetHelp();
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "No detailed help available.\nDescription: " + command.GetDescription();
				}
			}
		}
		if (text != null)
		{
			ConsoleCmdHelp.sb.AppendLine("*** " + text + " ***");
			foreach (string value in text2.Split('\n', StringSplitOptions.None))
			{
				ConsoleCmdHelp.sb.AppendLine(value);
			}
			return;
		}
		ConsoleCmdHelp.sb.AppendLine("No command or topic found by \"" + key + "\"");
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x0006A7F5 File Offset: 0x000689F5
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OutputHelp(List<string> _params)
	{
		ConsoleCmdHelp.sb.Clear();
		ConsoleCmdHelp.BuildStringCommandDescriptions();
		Log.Out(ConsoleCmdHelp.sb.ToString());
		ConsoleCmdHelp.sb.Clear();
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x0006A824 File Offset: 0x00068A24
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OutputDetailedHelp(List<string> _params)
	{
		ConsoleCmdHelp.sb.Clear();
		ConsoleCmdHelp.sb.AppendLine("*** List of Help Topics ***");
		foreach (KeyValuePair<string, ConsoleCmdHelp.HelpTopic> keyValuePair in ConsoleCmdHelp.helpTopics)
		{
			ConsoleCmdHelp.BuildStringHelpText(keyValuePair.Key);
			ConsoleCmdHelp.sb.AppendLine();
		}
		ConsoleCmdHelp.sb.AppendLine("*** List of Commands ***");
		foreach (IConsoleCommand consoleCommand in SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommands())
		{
			ConsoleCmdHelp.BuildStringHelpText(consoleCommand.GetCommands()[0]);
			ConsoleCmdHelp.sb.AppendLine();
		}
		Log.Out(ConsoleCmdHelp.sb.ToString());
		ConsoleCmdHelp.sb.Clear();
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x0006A91C File Offset: 0x00068B1C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SearchHelp(List<string> _params)
	{
		ConsoleCmdHelp.sb.Clear();
		if (_params.Count < 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument for search mask missing");
			return;
		}
		string text = _params[1];
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("*** List of Commands for \"" + text + "\" ***");
		IList<IConsoleCommand> commands = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommands();
		for (int i = 0; i < commands.Count; i++)
		{
			IConsoleCommand consoleCommand = commands[i];
			string description = consoleCommand.GetDescription();
			bool flag = text == null;
			if (!flag)
			{
				flag = description.ContainsCaseInsensitive(text);
				foreach (string a in consoleCommand.GetCommands())
				{
					flag |= a.ContainsCaseInsensitive(text);
				}
			}
			if (flag)
			{
				foreach (string value in consoleCommand.GetCommands())
				{
					ConsoleCmdHelp.sb.Append(" ");
					ConsoleCmdHelp.sb.Append(value);
				}
				ConsoleCmdHelp.sb.Append(" => ");
				ConsoleCmdHelp.sb.Append(description);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(ConsoleCmdHelp.sb.ToString());
				ConsoleCmdHelp.sb.Length = 0;
			}
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(ConsoleCmdHelp.sb.ToString());
		ConsoleCmdHelp.sb.Clear();
	}

	// Token: 0x04000CC5 RID: 3269
	[PublicizedFrom(EAccessModifier.Private)]
	public static StringBuilder sb = new StringBuilder();

	// Token: 0x04000CC6 RID: 3270
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, ConsoleCmdHelp.HelpTopic> helpTopics = new Dictionary<string, ConsoleCmdHelp.HelpTopic>
	{
		{
			"output",
			new ConsoleCmdHelp.HelpTopic("Prints commands to log file", new Action<List<string>>(ConsoleCmdHelp.OutputHelp), "Printed commands to log file")
		},
		{
			"outputdetailed",
			new ConsoleCmdHelp.HelpTopic("Prints commands with details to log file", new Action<List<string>>(ConsoleCmdHelp.OutputDetailedHelp), "Printed commands with details to log file")
		},
		{
			"search",
			new ConsoleCmdHelp.HelpTopic("Search for all commands matching a string", new Action<List<string>>(ConsoleCmdHelp.SearchHelp), "<first argument will be the string to match>")
		},
		{
			"*",
			new ConsoleCmdHelp.HelpTopic("Search for all commands matching a string", new Action<List<string>>(ConsoleCmdHelp.SearchHelp), "<first argument will be the string to match>")
		}
	};

	// Token: 0x02000229 RID: 553
	[PublicizedFrom(EAccessModifier.Private)]
	public struct HelpTopic
	{
		// Token: 0x060010C6 RID: 4294 RVA: 0x0006AB3D File Offset: 0x00068D3D
		public HelpTopic(string _desc, Action<List<string>> _action, string _actionCompleteText)
		{
			this.Description = _desc;
			this.Action = _action;
			this.ActionCompleteText = _actionCompleteText;
		}

		// Token: 0x04000CC7 RID: 3271
		public string Description;

		// Token: 0x04000CC8 RID: 3272
		public Action<List<string>> Action;

		// Token: 0x04000CC9 RID: 3273
		public string ActionCompleteText;
	}
}
