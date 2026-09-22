using System;
using System.Collections.Generic;
using System.Text;

namespace Twitch
{
	// Token: 0x02001818 RID: 6168
	public class TwitchCommandDebug : BaseTwitchCommand
	{
		// Token: 0x17001735 RID: 5941
		// (get) Token: 0x0600BEAE RID: 48814 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001736 RID: 5942
		// (get) Token: 0x0600BEAF RID: 48815 RVA: 0x0046A690 File Offset: 0x00468890
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#debug"
				};
			}
		}

		// Token: 0x17001737 RID: 5943
		// (get) Token: 0x0600BEB0 RID: 48816 RVA: 0x0046A6A0 File Offset: 0x004688A0
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("#debug", false, null)
				};
			}
		}

		// Token: 0x0600BEB1 RID: 48817 RVA: 0x0046A6B7 File Offset: 0x004688B7
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			TwitchManager.Current.DisplayDebug(message.Message);
		}

		// Token: 0x0600BEB2 RID: 48818 RVA: 0x0046A6CC File Offset: 0x004688CC
		public override void ExecuteConsole(List<string> arguments)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < arguments.Count; i++)
			{
				stringBuilder.Append(arguments[i] + " ");
			}
			TwitchManager.Current.DisplayDebug(stringBuilder.ToString());
		}
	}
}
