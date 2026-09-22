using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001814 RID: 6164
	public class TwitchCommandAddSpecialPoints : BaseTwitchCommand
	{
		// Token: 0x1700172B RID: 5931
		// (get) Token: 0x0600BE98 RID: 48792 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700172C RID: 5932
		// (get) Token: 0x0600BE99 RID: 48793 RVA: 0x0046A244 File Offset: 0x00468444
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#addsp"
				};
			}
		}

		// Token: 0x1700172D RID: 5933
		// (get) Token: 0x0600BE9A RID: 48794 RVA: 0x0046A254 File Offset: 0x00468454
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_AddSpecialPoints", false, null)
				};
			}
		}

		// Token: 0x0600BE9B RID: 48795 RVA: 0x0046A26C File Offset: 0x0046846C
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 3)
			{
				int points = 0;
				if (int.TryParse(array[2], out points))
				{
					if (array[1].EqualsCaseInsensitive(BaseTwitchCommand.allText))
					{
						TwitchManager.Current.ViewerData.AddPoints("", points, true, true);
						return;
					}
					TwitchManager.Current.ViewerData.AddPoints(array[1], points, true, true);
				}
			}
		}

		// Token: 0x0600BE9C RID: 48796 RVA: 0x0046A2D8 File Offset: 0x004684D8
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 3)
			{
				int points = 0;
				if (int.TryParse(arguments[2], out points))
				{
					if (arguments[1].EqualsCaseInsensitive(BaseTwitchCommand.allText))
					{
						TwitchManager.Current.ViewerData.AddPoints("", points, true, true);
						return;
					}
					TwitchManager.Current.ViewerData.AddPoints(arguments[1], points, true, true);
				}
			}
		}
	}
}
