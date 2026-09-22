using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001821 RID: 6177
	public class TwitchCommandRedeemHypeTrain : BaseTwitchCommand
	{
		// Token: 0x1700174F RID: 5967
		// (get) Token: 0x0600BEE3 RID: 48867 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001750 RID: 5968
		// (get) Token: 0x0600BEE4 RID: 48868 RVA: 0x0046B008 File Offset: 0x00469208
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#redeem_hypetrain"
				};
			}
		}

		// Token: 0x17001751 RID: 5969
		// (get) Token: 0x0600BEE5 RID: 48869 RVA: 0x0046B018 File Offset: 0x00469218
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_RedeemHypeTrain", false, null)
				};
			}
		}

		// Token: 0x0600BEE6 RID: 48870 RVA: 0x0046B030 File Offset: 0x00469230
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int hypeTrainLevel = 0;
				if (StringParsers.TryParseSInt32(array[1], out hypeTrainLevel))
				{
					TwitchManager.Current.HandleHypeTrainRedeem(hypeTrainLevel);
				}
			}
		}

		// Token: 0x0600BEE7 RID: 48871 RVA: 0x0046B06C File Offset: 0x0046926C
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int hypeTrainLevel = 0;
				if (StringParsers.TryParseSInt32(arguments[1], out hypeTrainLevel))
				{
					TwitchManager.Current.HandleHypeTrainRedeem(hypeTrainLevel);
				}
			}
		}
	}
}
