using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001822 RID: 6178
	public class TwitchCommandRedeemRaid : BaseTwitchCommand
	{
		// Token: 0x17001752 RID: 5970
		// (get) Token: 0x0600BEE9 RID: 48873 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001753 RID: 5971
		// (get) Token: 0x0600BEEA RID: 48874 RVA: 0x0046B09F File Offset: 0x0046929F
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_raid"
				};
			}
		}

		// Token: 0x17001754 RID: 5972
		// (get) Token: 0x0600BEEB RID: 48875 RVA: 0x0046B0AF File Offset: 0x004692AF
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemRaid", false, null)
				};
			}
		}

		// Token: 0x0600BEEC RID: 48876 RVA: 0x0046B0C8 File Offset: 0x004692C8
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
				int viewerAmount = 0;
				if (StringParsers.TryParseSInt32(array[2], out viewerAmount))
				{
					TwitchManager.Current.HandleRaidRedeem(text, viewerAmount, null);
				}
			}
		}

		// Token: 0x0600BEED RID: 48877 RVA: 0x0046B12C File Offset: 0x0046932C
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
				int viewerAmount = 0;
				if (StringParsers.TryParseSInt32(arguments[2], out viewerAmount))
				{
					TwitchManager.Current.HandleRaidRedeem(text, viewerAmount, null);
				}
			}
		}
	}
}
