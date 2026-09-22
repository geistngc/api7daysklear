using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001820 RID: 6176
	public class TwitchCommandRedeemGiftSub : BaseTwitchCommand
	{
		// Token: 0x1700174C RID: 5964
		// (get) Token: 0x0600BEDD RID: 48861 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700174D RID: 5965
		// (get) Token: 0x0600BEDE RID: 48862 RVA: 0x0046AE2B File Offset: 0x0046902B
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_giftsub"
				};
			}
		}

		// Token: 0x1700174E RID: 5966
		// (get) Token: 0x0600BEDF RID: 48863 RVA: 0x0046AE3B File Offset: 0x0046903B
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemGiftSubs", false, null)
				};
			}
		}

		// Token: 0x0600BEE0 RID: 48864 RVA: 0x0046AE54 File Offset: 0x00469054
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 3)
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
				int giftCounts = 0;
				if (StringParsers.TryParseSInt32(array[2], out giftCounts))
				{
					TwitchManager.Current.HandleGiftSubEvent(text, giftCounts, TwitchSubEventEntry.SubTierTypes.Tier1);
					return;
				}
			}
			else if (array.Length == 4)
			{
				string text2 = array[1];
				if (text2.StartsWith("@"))
				{
					text2 = text2.Substring(1).ToLower();
				}
				else
				{
					text2 = text2.ToLower();
				}
				int giftCounts2 = 0;
				int num = 0;
				TwitchSubEventEntry.SubTierTypes tier = TwitchSubEventEntry.SubTierTypes.Tier1;
				StringParsers.TryParseSInt32(array[2], out giftCounts2);
				StringParsers.TryParseSInt32(array[3], out num);
				if (num != 2)
				{
					if (num == 3)
					{
						tier = TwitchSubEventEntry.SubTierTypes.Tier3;
					}
				}
				else
				{
					tier = TwitchSubEventEntry.SubTierTypes.Tier2;
				}
				TwitchManager.Current.HandleGiftSubEvent(text2, giftCounts2, tier);
			}
		}

		// Token: 0x0600BEE1 RID: 48865 RVA: 0x0046AF28 File Offset: 0x00469128
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 3)
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
				int giftCounts = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out giftCounts))
				{
					TwitchManager.Current.HandleGiftSubEvent(text, giftCounts, TwitchSubEventEntry.SubTierTypes.Tier1);
					return;
				}
			}
			else if (arguments.Count == 4)
			{
				string text2 = arguments[1];
				if (text2.StartsWith("@"))
				{
					text2 = text2.Substring(1).ToLower();
				}
				else
				{
					text2 = text2.ToLower();
				}
				int giftCounts2 = 0;
				int num = 0;
				TwitchSubEventEntry.SubTierTypes tier = TwitchSubEventEntry.SubTierTypes.Tier1;
				StringParsers.TryParseSInt32(arguments[2], out giftCounts2);
				StringParsers.TryParseSInt32(arguments[3], out num);
				if (num != 2)
				{
					if (num == 3)
					{
						tier = TwitchSubEventEntry.SubTierTypes.Tier3;
					}
				}
				else
				{
					tier = TwitchSubEventEntry.SubTierTypes.Tier2;
				}
				TwitchManager.Current.HandleGiftSubEvent(text2, giftCounts2, tier);
			}
		}
	}
}
