using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001828 RID: 6184
	public class TwitchCommandSetPot : BaseTwitchCommand
	{
		// Token: 0x17001764 RID: 5988
		// (get) Token: 0x0600BF0D RID: 48909 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001765 RID: 5989
		// (get) Token: 0x0600BF0E RID: 48910 RVA: 0x0046B73B File Offset: 0x0046993B
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#setpot"
				};
			}
		}

		// Token: 0x17001766 RID: 5990
		// (get) Token: 0x0600BF0F RID: 48911 RVA: 0x0046B74B File Offset: 0x0046994B
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_SetPot", false, null)
				};
			}
		}

		// Token: 0x0600BF10 RID: 48912 RVA: 0x0046B764 File Offset: 0x00469964
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int pot = 0;
				if (int.TryParse(array[1], out pot))
				{
					TwitchManager.Current.SetPot(pot);
				}
			}
		}

		// Token: 0x0600BF11 RID: 48913 RVA: 0x0046B7A0 File Offset: 0x004699A0
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int pot = 0;
				if (int.TryParse(arguments[1], out pot))
				{
					TwitchManager.Current.SetPot(pot);
				}
			}
		}
	}
}
