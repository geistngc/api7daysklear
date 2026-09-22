using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200181B RID: 6171
	public class TwitchCommandGamestage : BaseTwitchCommand
	{
		// Token: 0x1700173E RID: 5950
		// (get) Token: 0x0600BEC0 RID: 48832 RVA: 0x0046AA78 File Offset: 0x00468C78
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#gamestage",
					"#gs"
				};
			}
		}

		// Token: 0x1700173F RID: 5951
		// (get) Token: 0x0600BEC1 RID: 48833 RVA: 0x0046AA90 File Offset: 0x00468C90
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_Gamestage1", false, null),
					Localization.Get("TwitchCommand_Gamestage1", false, null)
				};
			}
		}

		// Token: 0x0600BEC2 RID: 48834 RVA: 0x0046AAB6 File Offset: 0x00468CB6
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			TwitchManager.Current.DisplayGameStage();
		}

		// Token: 0x0600BEC3 RID: 48835 RVA: 0x0046AAB6 File Offset: 0x00468CB6
		public override void ExecuteConsole(List<string> arguments)
		{
			TwitchManager.Current.DisplayGameStage();
		}
	}
}
