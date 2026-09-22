using System;
using System.Collections.Generic;
using Platform.LAN;
using Twitch;
using UnityEngine.Scripting;

namespace Platform.Steam
{
	// Token: 0x02001C8B RID: 7307
	[Preserve]
	[PlatformFactory(EPlatformIdentifier.Steam)]
	public class Factory : AbsPlatform
	{
		// Token: 0x0600D89C RID: 55452 RVA: 0x004E0634 File Offset: 0x004DE834
		public override void CreateInstances()
		{
			base.Api = new Api();
			base.User = new User();
			base.AuthenticationServer = new AuthenticationServer();
			base.ServerListAnnouncer = new MasterServerAnnouncer();
			base.LobbyHost = new LobbyHost();
			base.MultiplayerInvitationDialog = new MultiplayerInvitationDialogSteam();
			base.Utils = new Utils();
			if (!base.AsServerOnly && !GameManager.IsDedicatedServer)
			{
				base.AchievementManager = new AchievementManager();
				base.RichPresence = new RichPresence();
				base.InviteListeners = new List<IJoinSessionGameInviteListener>
				{
					new JoinSessionGameInviteListener(),
					DiscordInviteListener.ListenerInstance
				};
				base.EntitlementValidators = new List<IEntitlementValidator>
				{
					new DownloadableContentValidator(),
					new TwitchEntitlementManager()
				};
				base.AuthenticationClient = new AuthenticationClient();
				base.ServerListInterfaces = new List<IServerListInterface>
				{
					new LobbyListInternet(),
					new LobbyListFriends(),
					new LANServerList()
				};
				if (PlatformManager.CrossplatformPlatform == null)
				{
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.Internet));
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.LAN));
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.Friends));
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.Favorites));
					base.ServerListInterfaces.Add(new MasterServerList(EServerRelationType.History));
				}
				base.VirtualKeyboard = new VirtualKeyboard();
			}
			if (!base.AsServerOnly)
			{
				base.Input = new PlayerInputManager();
			}
		}

		// Token: 0x17001ADF RID: 6879
		// (get) Token: 0x0600D89D RID: 55453 RVA: 0x004E07AC File Offset: 0x004DE9AC
		public override string NetworkProtocolName
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return "SteamNetworking";
			}
		}

		// Token: 0x0600D89E RID: 55454 RVA: 0x004E07B3 File Offset: 0x004DE9B3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IPlatformNetworkServer instantiateNetworkServer(ProtocolManager _protocolManager)
		{
			return new NetworkServerSteam(this, _protocolManager);
		}

		// Token: 0x0600D89F RID: 55455 RVA: 0x004E07BC File Offset: 0x004DE9BC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IPlatformNetworkClient instantiateNetworkClient(ProtocolManager _protocolManager)
		{
			return new NetworkClientSteam(this, _protocolManager);
		}
	}
}
