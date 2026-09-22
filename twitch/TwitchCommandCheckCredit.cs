using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001815 RID: 6165
	public class TwitchCommandCheckCredit : BaseTwitchCommand
	{
		// Token: 0x1700172E RID: 5934
		// (get) Token: 0x0600BE9E RID: 48798 RVA: 0x0046A344 File Offset: 0x00468544
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#checkcredit"
				};
			}
		}

		// Token: 0x1700172F RID: 5935
		// (get) Token: 0x0600BE9F RID: 48799 RVA: 0x0046A354 File Offset: 0x00468554
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_CheckCredit", false, null)
				};
			}
		}

		// Token: 0x0600BEA0 RID: 48800 RVA: 0x0046A36C File Offset: 0x0046856C
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				if (message.isMod || message.isBroadcaster)
				{
					string text = array[1];
					if (text.StartsWith("@"))
					{
						text = text.Substring(1).ToLower();
					}
					else
					{
						text = text.ToLower();
					}
					TwitchManager twitchManager = TwitchManager.Current;
					if (twitchManager.ViewerData.HasViewerEntry(text))
					{
						twitchManager.SendChannelCreditOutputMessage(text);
						return;
					}
					twitchManager.ircClient.SendChannelMessage(string.Format("[7DTD]: No viewer data for {0}.", array[1]), true);
					return;
				}
			}
			else if (array.Length == 1)
			{
				TwitchManager.Current.SendChannelCreditOutputMessage(message.UserName);
			}
		}

		// Token: 0x0600BEA1 RID: 48801 RVA: 0x0046A414 File Offset: 0x00468614
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count != 2)
			{
				if (arguments.Count == 1)
				{
					TwitchManager.Current.SendChannelPointOutputMessage(TwitchManager.Current.Authentication.userName);
				}
				return;
			}
			string text = arguments[1];
			if (text.StartsWith("@"))
			{
				text = text.Substring(1).ToLower();
			}
			else
			{
				text = text.ToLower();
			}
			TwitchManager twitchManager = TwitchManager.Current;
			if (twitchManager.ViewerData.HasViewerEntry(text))
			{
				twitchManager.SendChannelPointOutputMessage(text);
				return;
			}
			twitchManager.ircClient.SendChannelMessage(string.Format("[7DTD]: No viewer data for {0}.", arguments[1]), true);
		}
	}
}
