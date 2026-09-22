using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200181D RID: 6173
	public class TwitchCommandRedeemBits : BaseTwitchCommand
	{
		// Token: 0x17001743 RID: 5955
		// (get) Token: 0x0600BECB RID: 48843 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001744 RID: 5956
		// (get) Token: 0x0600BECC RID: 48844 RVA: 0x0046AB1F File Offset: 0x00468D1F
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_bits"
				};
			}
		}

		// Token: 0x17001745 RID: 5957
		// (get) Token: 0x0600BECD RID: 48845 RVA: 0x0046AB2F File Offset: 0x00468D2F
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemBits", false, null)
				};
			}
		}

		// Token: 0x0600BECE RID: 48846 RVA: 0x0046AB48 File Offset: 0x00468D48
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int bitAmount = 0;
				if (StringParsers.TryParseSInt32(array[1], out bitAmount))
				{
					TwitchManager.Current.HandleBitRedeem(message.UserName, bitAmount, null);
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
				int bitAmount2 = 0;
				if (StringParsers.TryParseSInt32(array[2], out bitAmount2))
				{
					TwitchManager.Current.HandleBitRedeem(text, bitAmount2, null);
				}
			}
		}

		// Token: 0x0600BECF RID: 48847 RVA: 0x0046ABD4 File Offset: 0x00468DD4
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int bitAmount = 0;
				if (StringParsers.TryParseSInt32(arguments[1], out bitAmount))
				{
					TwitchManager.Current.HandleBitRedeem(TwitchManager.Current.Authentication.userName, bitAmount, null);
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
				int bitAmount2 = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out bitAmount2))
				{
					TwitchManager.Current.HandleBitRedeem(text, bitAmount2, null);
				}
			}
		}
	}
}
