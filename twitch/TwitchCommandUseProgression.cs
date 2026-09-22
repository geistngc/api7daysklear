using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200182B RID: 6187
	public class TwitchCommandUseProgression : BaseTwitchCommand
	{
		// Token: 0x1700176D RID: 5997
		// (get) Token: 0x0600BF1F RID: 48927 RVA: 0x00080864 File Offset: 0x0007EA64
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Broadcaster;
			}
		}

		// Token: 0x1700176E RID: 5998
		// (get) Token: 0x0600BF20 RID: 48928 RVA: 0x0046B9A0 File Offset: 0x00469BA0
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#useprogression"
				};
			}
		}

		// Token: 0x1700176F RID: 5999
		// (get) Token: 0x0600BF21 RID: 48929 RVA: 0x0046B9B0 File Offset: 0x00469BB0
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_UseProgression", false, null)
				};
			}
		}

		// Token: 0x0600BF22 RID: 48930 RVA: 0x0046B9C8 File Offset: 0x00469BC8
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				bool useProgression = false;
				if (bool.TryParse(array[1], out useProgression))
				{
					TwitchManager.Current.SetUseProgression(useProgression);
				}
				TwitchManager.Current.SendChannelMessage("[7DTD]: Use Progression Enabled: " + (TwitchManager.Current.UseProgression ? "Yes" : "No"), true);
			}
		}

		// Token: 0x0600BF23 RID: 48931 RVA: 0x0046BA30 File Offset: 0x00469C30
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				bool useProgression = false;
				if (bool.TryParse(arguments[1], out useProgression))
				{
					TwitchManager.Current.SetUseProgression(useProgression);
				}
				TwitchManager.Current.SendChannelMessage("[7DTD]: Use Progression Enabled: " + (TwitchManager.Current.UseProgression ? "Yes" : "No"), true);
			}
		}
	}
}
