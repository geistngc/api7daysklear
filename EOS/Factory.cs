using System;
using System.Collections.Generic;
using Platform.Shared;
using UnityEngine.Scripting;

namespace Platform.EOS
{
	// Token: 0x02001CF8 RID: 7416
	[Preserve]
	[PlatformFactory(EPlatformIdentifier.EOS)]
	public class Factory : AbsPlatform
	{
		// Token: 0x0600DBF3 RID: 56307 RVA: 0x004ED0F8 File Offset: 0x004EB2F8
		public override void CreateInstances()
		{
			EosBindingsUnityEditor.Init();
			base.Api = new Api();
			base.AuthenticationServer = new AuthServer();
			base.ServerListAnnouncer = new SessionsHost();
			IAntiCheatServer antiCheatServer2;
			if (!GameManager.IsDedicatedServer)
			{
				IAntiCheatServer antiCheatServer = new AntiCheatServerP2P();
				antiCheatServer2 = antiCheatServer;
			}
			else
			{
				IAntiCheatServer antiCheatServer = new AntiCheatServer();
				antiCheatServer2 = antiCheatServer;
			}
			base.AntiCheatServer = antiCheatServer2;
			if (!base.AsServerOnly && !GameManager.IsDedicatedServer)
			{
				base.User = new User();
				base.AuthenticationClient = new AuthClient();
				base.AntiCheatClient = new AntiCheatClientManager();
				base.PlayerReporting = new PlayerReporting();
				SessionsClient sessionsClient = new SessionsClient();
				base.ServerListInterfaces = new List<IServerListInterface>
				{
					sessionsClient,
					new FavoriteServers()
				};
				base.ServerLookupInterface = sessionsClient;
				base.PartyVoice = new Voice();
				base.RemotePlayerFileStorage = new RemotePlayerFileStorage();
				base.IdMappingService = new EosUserIdMapper((Api)base.Api, (User)base.User);
				base.UserDetailsService = new UserDetailsServiceEos();
			}
			else
			{
				base.UserServer = new UserServer();
			}
			if (!base.AsServerOnly)
			{
				base.RemoteFileStorage = new RemoteFileStorage();
			}
			AntiCheatCommon.Init();
		}

		// Token: 0x0600DBF4 RID: 56308 RVA: 0x004ED21C File Offset: 0x004EB41C
		public override bool HasNetworkingEnabled(IList<string> _disabledProtocolNames)
		{
			if (!GameManager.IsDedicatedServer && base.HasNetworkingEnabled(_disabledProtocolNames) && PlatformManager.NativePlatform.User.UserStatus == EUserStatus.LoggedIn && PlatformManager.NativePlatform.User.Permissions.HasMultiplayer())
			{
				User user = (User)base.User;
				return user != null && user.Permissions.HasMultiplayer();
			}
			return false;
		}

		// Token: 0x17001B6B RID: 7019
		// (get) Token: 0x0600DBF5 RID: 56309 RVA: 0x004ED27D File Offset: 0x004EB47D
		public override string NetworkProtocolName
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return "EosNetworking";
			}
		}

		// Token: 0x0600DBF6 RID: 56310 RVA: 0x004ED284 File Offset: 0x004EB484
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IPlatformNetworkServer instantiateNetworkServer(ProtocolManager _protocolManager)
		{
			return new NetworkServerEos(this, _protocolManager);
		}

		// Token: 0x0600DBF7 RID: 56311 RVA: 0x004ED28D File Offset: 0x004EB48D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IPlatformNetworkClient instantiateNetworkClient(ProtocolManager _protocolManager)
		{
			return new NetworkClientEos(this, _protocolManager);
		}

		// Token: 0x0600DBF8 RID: 56312 RVA: 0x004ED296 File Offset: 0x004EB496
		public override void Destroy()
		{
			base.Destroy();
			EosBindingsUnityEditor.Shutdown();
		}
	}
}
