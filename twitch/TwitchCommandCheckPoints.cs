using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001816 RID: 6166
	public class TwitchCommandCheckPoints : BaseTwitchCommand
	{
		// Token: 0x17001730 RID: 5936
		// (get) Token: 0x0600BEA3 RID: 48803 RVA: 0x0046A4B1 File Offset: 0x004686B1
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#checkpoints",
					"#cp"
				};
			}
		}

		// Token: 0x17001731 RID: 5937
		// (get) Token: 0x0600BEA4 RID: 48804 RVA: 0x0046A4C9 File Offset: 0x004686C9
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_CheckPoints1", false, null),
					Localization.Get("TwitchCommand_CheckPoints2", false, null)
				};
			}
		}

		// Token: 0x0600BEA5 RID: 48805 RVA: 0x0046A4F0 File Offset: 0x004686F0
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
						twitchManager.SendChannelPointOutputMessage(text);
						return;
					}
					twitchManager.ircClient.SendChannelMessage(string.Format("[7DTD]: No viewer data for {0}.", array[1]), true);
					return;
				}
			}
			else if (array.Length == 1)
			{
				TwitchManager.Current.SendChannelPointOutputMessage(message.UserName);
			}
		}

		// Token: 0x0600BEA6 RID: 48806 RVA: 0x0046A598 File Offset: 0x00468798
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
