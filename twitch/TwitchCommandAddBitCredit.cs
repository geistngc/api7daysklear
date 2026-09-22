using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001812 RID: 6162
	public class TwitchCommandAddBitCredit : BaseTwitchCommand
	{
		// Token: 0x17001725 RID: 5925
		// (get) Token: 0x0600BE8C RID: 48780 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001726 RID: 5926
		// (get) Token: 0x0600BE8D RID: 48781 RVA: 0x0046A08A File Offset: 0x0046828A
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#addcredit"
				};
			}
		}

		// Token: 0x17001727 RID: 5927
		// (get) Token: 0x0600BE8E RID: 48782 RVA: 0x0046A09A File Offset: 0x0046829A
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_AddBitCredit", false, null)
				};
			}
		}

		// Token: 0x0600BE8F RID: 48783 RVA: 0x0046A0B4 File Offset: 0x004682B4
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 3)
			{
				int credit = 0;
				if (int.TryParse(array[2], out credit))
				{
					TwitchManager.Current.ViewerData.AddCredit(array[1], credit, true);
				}
			}
		}

		// Token: 0x0600BE90 RID: 48784 RVA: 0x0046A0FC File Offset: 0x004682FC
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 3)
			{
				int credit = 0;
				if (int.TryParse(arguments[2], out credit))
				{
					TwitchManager.Current.ViewerData.AddCredit(arguments[1], credit, true);
				}
			}
		}
	}
}
