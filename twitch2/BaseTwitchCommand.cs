using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001810 RID: 6160
	public class BaseTwitchCommand
	{
		// Token: 0x17001722 RID: 5922
		// (get) Token: 0x0600BE80 RID: 48768 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual BaseTwitchCommand.PermissionLevels RequiredPermission
		{
			get
			{
				return BaseTwitchCommand.PermissionLevels.Everyone;
			}
		}

		// Token: 0x17001723 RID: 5923
		// (get) Token: 0x0600BE81 RID: 48769 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual string[] CommandText
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001724 RID: 5924
		// (get) Token: 0x0600BE82 RID: 48770 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual string[] LocalizedCommandNames
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600BE84 RID: 48772 RVA: 0x00469F5E File Offset: 0x0046815E
		public static void ClearCommandPermissionOverrides()
		{
			BaseTwitchCommand.CommandPermissionOverrides.Clear();
		}

		// Token: 0x0600BE85 RID: 48773 RVA: 0x00469F6A File Offset: 0x0046816A
		public static void AddCommandPermissionOverride(string commandName, BaseTwitchCommand.PermissionLevels permissionLevel)
		{
			if (!BaseTwitchCommand.CommandPermissionOverrides.ContainsKey(commandName))
			{
				BaseTwitchCommand.CommandPermissionOverrides.Add(commandName, permissionLevel);
			}
		}

		// Token: 0x0600BE86 RID: 48774 RVA: 0x00469F85 File Offset: 0x00468185
		public static BaseTwitchCommand.PermissionLevels GetPermission(BaseTwitchCommand cmd)
		{
			if (BaseTwitchCommand.CommandPermissionOverrides.ContainsKey(cmd.CommandText[0]))
			{
				return BaseTwitchCommand.CommandPermissionOverrides[cmd.CommandText[0]];
			}
			return cmd.RequiredPermission;
		}

		// Token: 0x0600BE87 RID: 48775 RVA: 0x00469FB4 File Offset: 0x004681B4
		public BaseTwitchCommand()
		{
			this.SetupCommandTextList();
			if (BaseTwitchCommand.allText == "")
			{
				BaseTwitchCommand.allText = Localization.Get("lblAll", false, null);
			}
		}

		// Token: 0x0600BE88 RID: 48776 RVA: 0x00469FF0 File Offset: 0x004681F0
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupCommandTextList()
		{
			this.CommandTextList.AddRange(this.CommandText);
			string[] localizedCommandNames = this.LocalizedCommandNames;
			for (int i = 0; i < localizedCommandNames.Length; i++)
			{
				if (!this.CommandTextList.Contains(localizedCommandNames[i]))
				{
					this.CommandTextList.Add(localizedCommandNames[i]);
				}
			}
		}

		// Token: 0x0600BE89 RID: 48777 RVA: 0x0046A044 File Offset: 0x00468244
		public virtual bool CheckAllowed(TwitchIRCClient.TwitchChatMessage message)
		{
			BaseTwitchCommand.PermissionLevels permission = BaseTwitchCommand.GetPermission(this);
			if (permission == BaseTwitchCommand.PermissionLevels.Everyone)
			{
				return true;
			}
			if (permission == BaseTwitchCommand.PermissionLevels.Mod)
			{
				return message.isMod;
			}
			if (permission == BaseTwitchCommand.PermissionLevels.Broadcaster)
			{
				return message.isBroadcaster;
			}
			if (permission == BaseTwitchCommand.PermissionLevels.VIP)
			{
				return message.isVIP;
			}
			return permission == BaseTwitchCommand.PermissionLevels.Sub && message.isSub;
		}

		// Token: 0x0600BE8A RID: 48778 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Execute(ViewerEntry entry, TwitchIRCClient.TwitchChatMessage message)
		{
		}

		// Token: 0x0600BE8B RID: 48779 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void ExecuteConsole(List<string> arguments)
		{
		}

		// Token: 0x04008FAD RID: 36781
		public List<string> CommandTextList = new List<string>();

		// Token: 0x04008FAE RID: 36782
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string allText = "";

		// Token: 0x04008FAF RID: 36783
		[PublicizedFrom(EAccessModifier.Protected)]
		public static Dictionary<string, BaseTwitchCommand.PermissionLevels> CommandPermissionOverrides = new Dictionary<string, BaseTwitchCommand.PermissionLevels>();

		// Token: 0x02001811 RID: 6161
		public enum PermissionLevels
		{
			// Token: 0x04008FB1 RID: 36785
			Everyone,
			// Token: 0x04008FB2 RID: 36786
			VIP,
			// Token: 0x04008FB3 RID: 36787
			Sub,
			// Token: 0x04008FB4 RID: 36788
			Mod,
			// Token: 0x04008FB5 RID: 36789
			Broadcaster
		}
	}
}
