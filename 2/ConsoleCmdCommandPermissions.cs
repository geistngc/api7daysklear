using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001FC RID: 508
[Preserve]
public class ConsoleCmdCommandPermissions : ConsoleCmdAbstract
{
	// Token: 0x06000F92 RID: 3986 RVA: 0x00065444 File Offset: 0x00063644
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"commandpermission",
			"cp"
		};
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000F93 RID: 3987 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000F94 RID: 3988 RVA: 0x00060537 File Offset: 0x0005E737
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x0006545C File Offset: 0x0006365C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Manage command permission levels";
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x00065463 File Offset: 0x00063663
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Set/get permission levels required to execute a given command. Default\nlevel required for commands that are not explicitly specified is 0.\nUsage:\n   cp add <command> <level>\n   cp remove <command>\n   cp list";
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0006546C File Offset: 0x0006366C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.Instance.adminTools == null)
		{
			return;
		}
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No sub command given.");
			return;
		}
		if (_params[0].EqualsCaseInsensitive("add"))
		{
			this.ExecuteAdd(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("remove"))
		{
			this.ExecuteRemove(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("list"))
		{
			this.ExecuteList();
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid sub command \"" + _params[0] + "\".");
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x00065510 File Offset: 0x00063710
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteAdd(List<string> _params)
	{
		if (_params.Count != 3)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 3, found " + _params.Count.ToString() + ".");
			return;
		}
		if (SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand(_params[1], false) == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[1] + "\" is not a valid command.");
			return;
		}
		int num;
		if (!int.TryParse(_params[2], out num))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[2] + "\" is not a valid integer.");
			return;
		}
		GameManager.Instance.adminTools.Commands.AddCommand(_params[1], num, true);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("{0} added with permission level of {1}.", _params[1], num));
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x000655F4 File Offset: 0x000637F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteRemove(List<string> _params)
	{
		if (_params.Count != 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 2, found " + _params.Count.ToString() + ".");
			return;
		}
		IConsoleCommand command = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand(_params[1], false);
		if (command == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[1] + "\" is not a valid command.");
			return;
		}
		GameManager.Instance.adminTools.Commands.RemoveCommand(command.GetCommands());
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("{0} removed from permissions list.", _params[1]));
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x000656A0 File Offset: 0x000638A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteList()
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Defined Command Permissions:");
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("  Level: Command");
		foreach (KeyValuePair<string, AdminCommands.CommandPermission> keyValuePair in GameManager.Instance.adminTools.Commands.GetCommands())
		{
			AdminCommands.CommandPermission value = keyValuePair.Value;
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("  {0,5}: {1}", value.PermissionLevel, value.Command));
		}
	}
}
