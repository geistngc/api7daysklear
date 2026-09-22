using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001819 RID: 6169
	public class TwitchCommandDisableCommand : BaseTwitchCommand
	{
		// Token: 0x17001738 RID: 5944
		// (get) Token: 0x0600BEB4 RID: 48820 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001739 RID: 5945
		// (get) Token: 0x0600BEB5 RID: 48821 RVA: 0x0046A718 File Offset: 0x00468918
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#disable"
				};
			}
		}

		// Token: 0x1700173A RID: 5946
		// (get) Token: 0x0600BEB6 RID: 48822 RVA: 0x0046A728 File Offset: 0x00468928
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Disable", false, null)
				};
			}
		}

		// Token: 0x0600BEB7 RID: 48823 RVA: 0x0046A740 File Offset: 0x00468940
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				bool flag = false;
				foreach (string key in TwitchActionManager.TwitchActions.Keys)
				{
					TwitchAction twitchAction = TwitchActionManager.TwitchActions[key];
					if (twitchAction.IsInPreset(twitchManager.CurrentActionPreset) && twitchAction.Command.Equals(array[1]))
					{
						twitchAction.Enabled = false;
						flag = true;
					}
				}
				if (flag)
				{
					twitchManager.SendChannelMessage("[7DTD]: Command Disabled: " + array[1], true);
					twitchManager.SetupAvailableCommands();
				}
			}
		}

		// Token: 0x0600BEB8 RID: 48824 RVA: 0x0046A808 File Offset: 0x00468A08
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				bool flag = false;
				foreach (string key in TwitchActionManager.TwitchActions.Keys)
				{
					TwitchAction twitchAction = TwitchActionManager.TwitchActions[key];
					if (twitchAction.IsInPreset(twitchManager.CurrentActionPreset) && twitchAction.Command.Equals(arguments[1]))
					{
						twitchAction.Enabled = false;
						flag = true;
					}
				}
				if (flag)
				{
					twitchManager.SendChannelMessage("[7DTD]: Command Disabled: " + arguments[1], true);
					twitchManager.SetupAvailableCommands();
				}
			}
		}
	}
}
