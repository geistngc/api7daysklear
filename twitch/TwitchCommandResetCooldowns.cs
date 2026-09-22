using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001825 RID: 6181
	public class TwitchCommandResetCooldowns : BaseTwitchCommand
	{
		// Token: 0x1700175B RID: 5979
		// (get) Token: 0x0600BEFB RID: 48891 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700175C RID: 5980
		// (get) Token: 0x0600BEFC RID: 48892 RVA: 0x0046B4B7 File Offset: 0x004696B7
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#reset_cooldowns"
				};
			}
		}

		// Token: 0x1700175D RID: 5981
		// (get) Token: 0x0600BEFD RID: 48893 RVA: 0x0046B4C7 File Offset: 0x004696C7
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_ResetCooldowns", false, null)
				};
			}
		}

		// Token: 0x0600BEFE RID: 48894 RVA: 0x0046B4E0 File Offset: 0x004696E0
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			TwitchManager twitchManager = TwitchManager.Current;
			float currentUnityTime = twitchManager.CurrentUnityTime;
			foreach (string key in TwitchActionManager.TwitchActions.Keys)
			{
				TwitchActionManager.TwitchActions[key].ResetCooldown(currentUnityTime);
			}
			twitchManager.SetupAvailableCommands();
			twitchManager.SendChannelMessage("Action Cooldowns have been reset!", true);
		}

		// Token: 0x0600BEFF RID: 48895 RVA: 0x0046B560 File Offset: 0x00469760
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 1)
			{
				TwitchManager twitchManager = TwitchManager.Current;
				float currentUnityTime = twitchManager.CurrentUnityTime;
				foreach (string key in TwitchActionManager.TwitchActions.Keys)
				{
					TwitchActionManager.TwitchActions[key].ResetCooldown(currentUnityTime);
				}
				twitchManager.SetupAvailableCommands();
				twitchManager.SendChannelMessage("Action Cooldowns have been reset!", true);
			}
		}
	}
}
