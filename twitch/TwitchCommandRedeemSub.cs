using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001823 RID: 6179
	public class TwitchCommandRedeemSub : BaseTwitchCommand
	{
		// Token: 0x17001755 RID: 5973
		// (get) Token: 0x0600BEEF RID: 48879 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001756 RID: 5974
		// (get) Token: 0x0600BEF0 RID: 48880 RVA: 0x0046B18C File Offset: 0x0046938C
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_sub"
				};
			}
		}

		// Token: 0x17001757 RID: 5975
		// (get) Token: 0x0600BEF1 RID: 48881 RVA: 0x0046B19C File Offset: 0x0046939C
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemSub", false, null)
				};
			}
		}

		// Token: 0x0600BEF2 RID: 48882 RVA: 0x0046B1B4 File Offset: 0x004693B4
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
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
				TwitchManager.Current.HandleSubEvent(text, 1, TwitchSubEventEntry.SubTierTypes.Tier1);
				return;
			}
			if (array.Length == 3)
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
				int months = 0;
				if (StringParsers.TryParseSInt32(array[2], out months))
				{
					TwitchManager.Current.HandleSubEvent(text2, months, TwitchSubEventEntry.SubTierTypes.Tier1);
					return;
				}
			}
			else if (array.Length == 4)
			{
				string text3 = array[1];
				if (text3.StartsWith("@"))
				{
					text3 = text3.Substring(1).ToLower();
				}
				else
				{
					text3 = text3.ToLower();
				}
				int months2 = 0;
				int num = 0;
				TwitchSubEventEntry.SubTierTypes tier = TwitchSubEventEntry.SubTierTypes.Tier1;
				StringParsers.TryParseSInt32(array[2], out months2);
				if (array[3].Trim().ToLower() == "prime")
				{
					tier = TwitchSubEventEntry.SubTierTypes.Prime;
				}
				else
				{
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
				}
				TwitchManager.Current.HandleSubEvent(text3, months2, tier);
			}
		}

		// Token: 0x0600BEF3 RID: 48883 RVA: 0x0046B2F0 File Offset: 0x004694F0
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
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
				TwitchManager.Current.HandleSubEvent(text, 1, TwitchSubEventEntry.SubTierTypes.Tier1);
				return;
			}
			if (arguments.Count == 3)
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
				int months = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out months))
				{
					TwitchManager.Current.HandleSubEvent(text2, months, TwitchSubEventEntry.SubTierTypes.Tier1);
					return;
				}
			}
			else if (arguments.Count == 4)
			{
				string text3 = arguments[1];
				if (text3.StartsWith("@"))
				{
					text3 = text3.Substring(1).ToLower();
				}
				else
				{
					text3 = text3.ToLower();
				}
				int months2 = 0;
				int num = 0;
				TwitchSubEventEntry.SubTierTypes tier = TwitchSubEventEntry.SubTierTypes.Tier1;
				StringParsers.TryParseSInt32(arguments[2], out months2);
				if (arguments[3].Trim().ToLower() == "prime")
				{
					tier = TwitchSubEventEntry.SubTierTypes.Prime;
				}
				else
				{
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
				}
				TwitchManager.Current.HandleSubEvent(text3, months2, tier);
			}
		}
	}
}
