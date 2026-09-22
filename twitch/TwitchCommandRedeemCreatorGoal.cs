using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200181F RID: 6175
	public class TwitchCommandRedeemCreatorGoal : BaseTwitchCommand
	{
		// Token: 0x17001749 RID: 5961
		// (get) Token: 0x0600BED7 RID: 48855 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700174A RID: 5962
		// (get) Token: 0x0600BED8 RID: 48856 RVA: 0x0046ADB7 File Offset: 0x00468FB7
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_goal"
				};
			}
		}

		// Token: 0x1700174B RID: 5963
		// (get) Token: 0x0600BED9 RID: 48857 RVA: 0x0046ADC7 File Offset: 0x00468FC7
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemGoal", false, null)
				};
			}
		}

		// Token: 0x0600BEDA RID: 48858 RVA: 0x0046ADE0 File Offset: 0x00468FE0
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				TwitchManager.Current.HandleCreatorGoalRedeem(array[1]);
			}
		}

		// Token: 0x0600BEDB RID: 48859 RVA: 0x0046AE0F File Offset: 0x0046900F
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				TwitchManager.Current.HandleCreatorGoalRedeem(arguments[1]);
			}
		}
	}
}
