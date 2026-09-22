using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SandboxOptions;
using UnityEngine.Scripting;

// Token: 0x0200021F RID: 543
[Preserve]
public class ConsoleCmdGetSandboxOptions : ConsoleCmdAbstract
{
	// Token: 0x0600107B RID: 4219 RVA: 0x00067D40 File Offset: 0x00065F40
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"getsandboxoptions",
			"gso"
		};
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x0600107C RID: 4220 RVA: 0x000617F2 File Offset: 0x0005F9F2
	public override int DefaultPermissionLevel
	{
		get
		{
			return 1000;
		}
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x00067D58 File Offset: 0x00065F58
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Gets the current game's Sandbox Options";
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x00067D5F File Offset: 0x00065F5F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return ("\n\t\t\t|Usage:\n\t\t\t|  " + this.PrimaryCommand + " [show all]\n\t\t\t|Shows the currently loaded game's Sandbox Options.\n\t\t\t|By default only shows those that have non-default values, but can show all options by specifying the optional argument with \"true\".\n\t\t\t").Unindent(true);
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x00067D7C File Offset: 0x00065F7C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		bool showAll = _params.Count > 0 && StringParsers.ParseBool(_params[0], 0, -1, true);
		ConsoleCmdGetSandboxOptions.LogOptions(GameStats.GetString(EnumGameStats.SandboxCode), showAll, ConsoleCmdGetSandboxOptions.ELogType.RemoteConsole);
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00067DB4 File Offset: 0x00065FB4
	public static void LogOptions(string _code, bool _showAll, ConsoleCmdGetSandboxOptions.ELogType _logTarget)
	{
		Action<string> action;
		switch (_logTarget)
		{
		case ConsoleCmdGetSandboxOptions.ELogType.LogOnly:
			action = new Action<string>(ConsoleCmdGetSandboxOptions.<LogOptions>g__PrintLog|7_0);
			break;
		case ConsoleCmdGetSandboxOptions.ELogType.Console:
			action = new Action<string>(ConsoleCmdGetSandboxOptions.<LogOptions>g__PrintConsole|7_1);
			break;
		case ConsoleCmdGetSandboxOptions.ELogType.RemoteConsole:
			action = new Action<string>(ConsoleCmdGetSandboxOptions.<LogOptions>g__PrintRemoteConsole|7_2);
			break;
		default:
			throw new ArgumentOutOfRangeException("_logTarget", _logTarget, null);
		}
		Action<string> action2 = action;
		if (_code == null)
		{
			_code = "";
		}
		action2("Sandbox Code: " + _code);
		action2("Sandbox Options:");
		SandboxOptionManager sandboxOptionManager = SandboxOptionManager.Current;
		SandboxOptionPreset sandboxOptionPreset = new SandboxOptionPreset();
		if (!sandboxOptionManager.LoadOptionsFromCode(_code, sandboxOptionPreset))
		{
			action2("  - Invalid code -");
			return;
		}
		foreach (KeyValuePair<string, List<BaseSandboxOption>> keyValuePair in sandboxOptionManager.OptionsByCategory.dict)
		{
			string text;
			List<BaseSandboxOption> list;
			keyValuePair.Deconstruct(out text, out list);
			string text2 = text;
			List<BaseSandboxOption> list2 = list;
			bool flag = false;
			foreach (BaseSandboxOption baseSandboxOption in list2)
			{
				int defaultIndex = baseSandboxOption.GetDefaultIndex();
				int num = defaultIndex;
				if (_showAll || sandboxOptionPreset.PresetValues.TryGetValue(baseSandboxOption.Option, out num))
				{
					if (!flag)
					{
						flag = true;
						action2("*** " + text2.ToUpper() + " ***");
					}
					string valueTextFromIndex = baseSandboxOption.GetValueTextFromIndex(num, null);
					string valueTextFromIndex2 = baseSandboxOption.GetValueTextFromIndex(defaultIndex, null);
					action2(string.Format("Option {0}: {1}/{2} (default: {3}/{4})", new object[]
					{
						baseSandboxOption.Option.ToStringCached<SandboxOptions>(),
						num,
						valueTextFromIndex,
						defaultIndex,
						valueTextFromIndex2
					}));
				}
			}
		}
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x00067FB8 File Offset: 0x000661B8
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <LogOptions>g__PrintLog|7_0(string _line)
	{
		Log.WriteLine(_line);
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x00067FC0 File Offset: 0x000661C0
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <LogOptions>g__PrintConsole|7_1(string _line)
	{
		Log.Out(_line);
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x00067FC8 File Offset: 0x000661C8
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <LogOptions>g__PrintRemoteConsole|7_2(string _line)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(_line);
	}

	// Token: 0x02000220 RID: 544
	public enum ELogType
	{
		// Token: 0x04000CB8 RID: 3256
		LogOnly,
		// Token: 0x04000CB9 RID: 3257
		Console,
		// Token: 0x04000CBA RID: 3258
		RemoteConsole
	}
}
