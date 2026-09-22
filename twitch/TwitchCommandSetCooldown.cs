using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001827 RID: 6183
	public class TwitchCommandSetCooldown : BaseTwitchCommand
	{
		// Token: 0x17001761 RID: 5985
		// (get) Token: 0x0600BF07 RID: 48903 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001762 RID: 5986
		// (get) Token: 0x0600BF08 RID: 48904 RVA: 0x0046B683 File Offset: 0x00469883
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#setcooldown"
				};
			}
		}

		// Token: 0x17001763 RID: 5987
		// (get) Token: 0x0600BF09 RID: 48905 RVA: 0x0046B693 File Offset: 0x00469893
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_SetCooldown", false, null)
				};
			}
		}

		// Token: 0x0600BF0A RID: 48906 RVA: 0x0046B6AC File Offset: 0x004698AC
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			if (array.Length == 2)
			{
				int num = 0;
				if (int.TryParse(array[1], out num))
				{
					if (num < 0)
					{
						num = 0;
					}
					TwitchManager.Current.SetCooldown((float)num + 0.5f, TwitchManager.CooldownTypes.Time, false, true);
				}
			}
		}

		// Token: 0x0600BF0B RID: 48907 RVA: 0x0046B6F8 File Offset: 0x004698F8
		public override void ExecuteConsole(List<string> arguments)
		{
			if (arguments.Count == 2)
			{
				int num = 0;
				if (int.TryParse(arguments[1], out num))
				{
					if (num < 0)
					{
						num = 0;
					}
					TwitchManager.Current.SetCooldown((float)num + 0.5f, TwitchManager.CooldownTypes.Time, false, true);
				}
			}
		}
	}
}
