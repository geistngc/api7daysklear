using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001817 RID: 6167
	public class TwitchCommandCommands : BaseTwitchCommand
	{
		// Token: 0x17001732 RID: 5938
		// (get) Token: 0x0600BEA8 RID: 48808 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001733 RID: 5939
		// (get) Token: 0x0600BEA9 RID: 48809 RVA: 0x0046A635 File Offset: 0x00468835
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#commands"
				};
			}
		}

		// Token: 0x17001734 RID: 5940
		// (get) Token: 0x0600BEAA RID: 48810 RVA: 0x0046A645 File Offset: 0x00468845
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Commands", false, null)
				};
			}
		}

		// Token: 0x0600BEAB RID: 48811 RVA: 0x0046A65C File Offset: 0x0046885C
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			TwitchManager.Current.DisplayCommands(message.isBroadcaster, message.isMod, message.isVIP, message.isSub);
		}

		// Token: 0x0600BEAC RID: 48812 RVA: 0x0046A680 File Offset: 0x00468880
		public override void ExecuteConsole(List<string> arguments)
		{
			TwitchManager.Current.DisplayCommands(true, true, true, true);
		}
	}
}
