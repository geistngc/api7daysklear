using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001824 RID: 6180
	public class TwitchCommandRemoveViewer : BaseTwitchCommand
	{
		// Token: 0x17001758 RID: 5976
		// (get) Token: 0x0600BEF5 RID: 48885 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001759 RID: 5977
		// (get) Token: 0x0600BEF6 RID: 48886 RVA: 0x0046B439 File Offset: 0x00469639
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#remove_viewer"
				};
			}
		}

		// Token: 0x1700175A RID: 5978
		// (get) Token: 0x0600BEF7 RID: 48887 RVA: 0x0046B449 File Offset: 0x00469649
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RemoveViewer", false, null)
				};
			}
		}

		// Token: 0x0600BEF8 RID: 48888 RVA: 0x0046B460 File Offset: 0x00469660
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				TwitchManager.Current.ViewerData.RemoveViewerEntry(array[1]);
			}
		}

		// Token: 0x0600BEF9 RID: 48889 RVA: 0x0046B495 File Offset: 0x00469695
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				TwitchManager.Current.ViewerData.RemoveViewerEntry(arguments[1]);
			}
		}
	}
}
