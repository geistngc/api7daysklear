using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200181E RID: 6174
	public class TwitchCommandRedeemCharity : BaseTwitchCommand
	{
		// Token: 0x17001746 RID: 5958
		// (get) Token: 0x0600BED1 RID: 48849 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001747 RID: 5959
		// (get) Token: 0x0600BED2 RID: 48850 RVA: 0x0046AC6B File Offset: 0x00468E6B
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_charity"
				};
			}
		}

		// Token: 0x17001748 RID: 5960
		// (get) Token: 0x0600BED3 RID: 48851 RVA: 0x0046AC7B File Offset: 0x00468E7B
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemCharity", false, null)
				};
			}
		}

		// Token: 0x0600BED4 RID: 48852 RVA: 0x0046AC94 File Offset: 0x00468E94
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int charityAmount = 0;
				if (StringParsers.TryParseSInt32(array[1], out charityAmount))
				{
					TwitchManager.Current.HandleCharityRedeem(message.UserName, charityAmount, null);
					return;
				}
			}
			else if (array.Length == 3)
			{
				string text = array[1];
				if (text.StartsWith("@"))
				{
					text = text.Substring(1).ToLower();
				}
				else
				{
					text = text.ToLower();
				}
				int charityAmount2 = 0;
				if (StringParsers.TryParseSInt32(array[2], out charityAmount2))
				{
					TwitchManager.Current.HandleCharityRedeem(text, charityAmount2, null);
				}
			}
		}

		// Token: 0x0600BED5 RID: 48853 RVA: 0x0046AD20 File Offset: 0x00468F20
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int charityAmount = 0;
				if (StringParsers.TryParseSInt32(arguments[1], out charityAmount))
				{
					TwitchManager.Current.HandleCharityRedeem(TwitchManager.Current.Authentication.userName, charityAmount, null);
					return;
				}
			}
			else if (arguments.Count == 3)
			{
				string text = arguments[1];
				if (text.StartsWith("@"))
				{
					text = text.Substring(1).ToLower();
				}
				else
				{
					text = text.ToLower();
				}
				int charityAmount2 = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out charityAmount2))
				{
					TwitchManager.Current.HandleCharityRedeem(text, charityAmount2, null);
				}
			}
		}
	}
}
