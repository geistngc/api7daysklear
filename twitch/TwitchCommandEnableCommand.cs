using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200181A RID: 6170
	public class TwitchCommandEnableCommand : BaseTwitchCommand
	{
		// Token: 0x1700173B RID: 5947
		// (get) Token: 0x0600BEBA RID: 48826 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700173C RID: 5948
		// (get) Token: 0x0600BEBB RID: 48827 RVA: 0x0046A8C8 File Offset: 0x00468AC8
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#enable"
				};
			}
		}

		// Token: 0x1700173D RID: 5949
		// (get) Token: 0x0600BEBC RID: 48828 RVA: 0x0046A8D8 File Offset: 0x00468AD8
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Enable", false, null)
				};
			}
		}

		// Token: 0x0600BEBD RID: 48829 RVA: 0x0046A8F0 File Offset: 0x00468AF0
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
						twitchAction.Enabled = true;
						flag = true;
					}
				}
				if (flag)
				{
					twitchManager.SendChannelMessage("[7DTD]: Command Enabled: " + array[1], true);
					twitchManager.SetupAvailableCommands();
				}
			}
		}

		// Token: 0x0600BEBE RID: 48830 RVA: 0x0046A9B8 File Offset: 0x00468BB8
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
						twitchAction.Enabled = true;
						flag = true;
					}
				}
				if (flag)
				{
					twitchManager.SendChannelMessage("[7DTD]: Command Enabled: " + arguments[1], true);
					twitchManager.SetupAvailableCommands();
				}
			}
		}
	}
}
