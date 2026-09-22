using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001826 RID: 6182
	public class TwitchCommandSetBitPot : BaseTwitchCommand
	{
		// Token: 0x1700175E RID: 5982
		// (get) Token: 0x0600BF01 RID: 48897 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700175F RID: 5983
		// (get) Token: 0x0600BF02 RID: 48898 RVA: 0x0046B5EC File Offset: 0x004697EC
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#setbitpot"
				};
			}
		}

		// Token: 0x17001760 RID: 5984
		// (get) Token: 0x0600BF03 RID: 48899 RVA: 0x0046B5FC File Offset: 0x004697FC
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_SetBitPot", false, null)
				};
			}
		}

		// Token: 0x0600BF04 RID: 48900 RVA: 0x0046B614 File Offset: 0x00469814
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int bitPot = 0;
				if (int.TryParse(array[1], out bitPot))
				{
					TwitchManager.Current.SetBitPot(bitPot);
				}
			}
		}

		// Token: 0x0600BF05 RID: 48901 RVA: 0x0046B650 File Offset: 0x00469850
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int bitPot = 0;
				if (int.TryParse(arguments[1], out bitPot))
				{
					TwitchManager.Current.SetBitPot(bitPot);
				}
			}
		}
	}
}
