using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000249 RID: 585
[Preserve]
public class ConsoleCmdMumblePositionalAudio : ConsoleCmdAbstract
{
	// Token: 0x06001184 RID: 4484 RVA: 0x0006EEA9 File Offset: 0x0006D0A9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"mumblepositionalaudio",
			"mpa"
		};
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0006EEC1 File Offset: 0x0006D0C1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Mumble Positional Audio related tools";
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x0006EEC8 File Offset: 0x0006D0C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "\r\n\t\t\t|Usage:\r\n\t\t\t|  1. mpa enable\r\n\t\t\t|  2. mpa disable\r\n\t\t\t|  3. mpa reinit\r\n\t\t\t".Unindent(true);
	}

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06001187 RID: 4487 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001188 RID: 4488 RVA: 0x0006EED8 File Offset: 0x0006D0D8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 1, found " + _params.Count.ToString() + ".");
			return;
		}
		if (_params[0].EqualsCaseInsensitive("enable"))
		{
			if (SingletonMonoBehaviour<MumblePositionalAudio>.Instance != null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Positional Audio already enabled");
				return;
			}
			MumblePositionalAudio.Init();
			return;
		}
		else if (_params[0].EqualsCaseInsensitive("disable"))
		{
			if (SingletonMonoBehaviour<MumblePositionalAudio>.Instance == null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Positional Audio already disabled");
				return;
			}
			MumblePositionalAudio.Destroy();
			return;
		}
		else if (_params[0].EqualsCaseInsensitive("reinit"))
		{
			if (SingletonMonoBehaviour<MumblePositionalAudio>.Instance == null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Positional Audio not enabled");
				return;
			}
			SingletonMonoBehaviour<MumblePositionalAudio>.Instance.ReinitShm();
			return;
		}
		else
		{
			if (!_params[0].EqualsCaseInsensitive("uiversion"))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid subcommand");
				return;
			}
			if (SingletonMonoBehaviour<MumblePositionalAudio>.Instance == null)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Positional Audio not enabled");
				return;
			}
			SingletonMonoBehaviour<MumblePositionalAudio>.Instance.printUiVersion();
			return;
		}
	}
}
