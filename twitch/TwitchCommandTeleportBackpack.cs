using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001829 RID: 6185
	public class TwitchCommandTeleportBackpack : BaseTwitchCommand
	{
		// Token: 0x17001767 RID: 5991
		// (get) Token: 0x0600BF13 RID: 48915 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Mod;
			}
		}

		// Token: 0x17001768 RID: 5992
		// (get) Token: 0x0600BF14 RID: 48916 RVA: 0x0046B7D3 File Offset: 0x004699D3
		public override string[] CommandText
		{
			get
			{
				return new string[]
				{
					"#tp_backpack",
					"#teleport_backpack"
				};
			}
		}

		// Token: 0x17001769 RID: 5993
		// (get) Token: 0x0600BF15 RID: 48917 RVA: 0x0046B7EB File Offset: 0x004699EB
		public override string[] LocalizedCommandNames
		{
			get
			{
				return new string[]
				{
					Localization.Get("TwitchCommand_TeleportBackpack1", false, null),
					Localization.Get("TwitchCommand_TeleportBackpack2", false, null)
				};
			}
		}

		// Token: 0x0600BF16 RID: 48918 RVA: 0x0046B814 File Offset: 0x00469A14
		public override void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
			string[] array = message.Message.Split(' ', StringSplitOptions.None);
			EntityPlayer entityPlayer = TwitchManager.Current.LocalPlayer;
			if (array.Length != 2)
			{
				GameEventManager.Current.HandleAction("action_teleport_backpack", entityPlayer, entityPlayer, false, "", "", false, true, "", null);
				return;
			}
			int index = -1;
			if (StringParsers.TryParseSInt32(array[1], out index) && TwitchManager.Current.LocalPlayer.Party != null)
			{
				entityPlayer = TwitchManager.Current.LocalPlayer.Party.GetMemberAtIndex(index, TwitchManager.Current.LocalPlayer);
				entityPlayer == null;
				return;
			}
		}

		// Token: 0x0600BF17 RID: 48919 RVA: 0x0046B8B0 File Offset: 0x00469AB0
		public override void ExecuteConsole(List<string> arguments)
		{
			EntityPlayer entityPlayer = TwitchManager.Current.LocalPlayer;
			if (arguments.Count != 2)
			{
				GameEventManager.Current.HandleAction("action_teleport_backpack", entityPlayer, entityPlayer, false, "", "", false, true, "", null);
				return;
			}
			int index = -1;
			if (StringParsers.TryParseSInt32(arguments[1], out index) && TwitchManager.Current.LocalPlayer.Party != null)
			{
				entityPlayer = TwitchManager.Current.LocalPlayer.Party.GetMemberAtIndex(index, TwitchManager.Current.LocalPlayer);
				entityPlayer == null;
				return;
			}
		}
	}
}
