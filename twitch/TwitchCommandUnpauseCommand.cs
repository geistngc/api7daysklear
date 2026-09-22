using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200182A RID: 6186
	public class TwitchCommandUnpauseCommand : BaseTwitchCommand
	{
		// Token: 0x1700176A RID: 5994
		// (get) Token: 0x0600BF19 RID: 48921 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x1700176B RID: 5995
		// (get) Token: 0x0600BF1A RID: 48922 RVA: 0x0046B943 File Offset: 0x00469B43
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#unpause"
				};
			}
		}

		// Token: 0x1700176C RID: 5996
		// (get) Token: 0x0600BF1B RID: 48923 RVA: 0x0046B953 File Offset: 0x00469B53
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Unpause", false, null)
				};
			}
		}

		// Token: 0x0600BF1C RID: 48924 RVA: 0x0046B96A File Offset: 0x00469B6A
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			if (message.Message.Split(' ', StringSplitOptions.None).Length == 1)
			{
				TwitchManager.Current.SetTwitchActive(true);
			}
		}

		// Token: 0x0600BF1D RID: 48925 RVA: 0x0046B98A File Offset: 0x00469B8A
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 1)
			{
				TwitchManager.Current.SetTwitchActive(true);
			}
		}
	}
}
