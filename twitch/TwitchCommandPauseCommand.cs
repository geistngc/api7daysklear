using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200181C RID: 6172
	public class TwitchCommandPauseCommand : BaseTwitchCommand
	{
		// Token: 0x17001740 RID: 5952
		// (get) Token: 0x0600BEC5 RID: 48837 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001741 RID: 5953
		// (get) Token: 0x0600BEC6 RID: 48838 RVA: 0x0046AAC2 File Offset: 0x00468CC2
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#pause"
				};
			}
		}

		// Token: 0x17001742 RID: 5954
		// (get) Token: 0x0600BEC7 RID: 48839 RVA: 0x0046AAD2 File Offset: 0x00468CD2
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Pause", false, null)
				};
			}
		}

		// Token: 0x0600BEC8 RID: 48840 RVA: 0x0046AAE9 File Offset: 0x00468CE9
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			if (message.Message.Split(' ', StringSplitOptions.None).Length == 1)
			{
				TwitchManager.Current.SetTwitchActive(false);
			}
		}

		// Token: 0x0600BEC9 RID: 48841 RVA: 0x0046AB09 File Offset: 0x00468D09
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 1)
			{
				TwitchManager.Current.SetTwitchActive(false);
			}
		}
	}
}
