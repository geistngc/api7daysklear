using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001813 RID: 6163
	public class TwitchCommandAddPoints : BaseTwitchCommand
	{
		// Token: 0x17001728 RID: 5928
		// (get) Token: 0x0600BE92 RID: 48786 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001729 RID: 5929
		// (get) Token: 0x0600BE93 RID: 48787 RVA: 0x0046A145 File Offset: 0x00468345
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#addpp"
				};
			}
		}

		// Token: 0x1700172A RID: 5930
		// (get) Token: 0x0600BE94 RID: 48788 RVA: 0x0046A155 File Offset: 0x00468355
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_AddPoints", false, null)
				};
			}
		}

		// Token: 0x0600BE95 RID: 48789 RVA: 0x0046A16C File Offset: 0x0046836C
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
						TwitchManager.Current.ViewerData.AddPoints("", points, false, true);
						return;
					}
					TwitchManager.Current.ViewerData.AddPoints(array[1], points, false, true);
				}
			}
		}

		// Token: 0x0600BE96 RID: 48790 RVA: 0x0046A1D8 File Offset: 0x004683D8
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 3)
			{
				int points = 0;
				if (int.TryParse(arguments[2], out points))
				{
					if (arguments[1].EqualsCaseInsensitive(BaseTwitchCommand.allText))
					{
						TwitchManager.Current.ViewerData.AddPoints("", points, false, true);
						return;
					}
					TwitchManager.Current.ViewerData.AddPoints(arguments[1], points, false, true);
				}
			}
		}
	}
}
