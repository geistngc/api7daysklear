using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Platform.MultiPlatform
{
	// Token: 0x02001CC0 RID: 7360
	[Preserve]
	public class Factory : IPlatform
	{
		// Token: 0x17001B25 RID: 6949
		// (get) Token: 0x0600DA58 RID: 55896 RVA: 0x004E6463 File Offset: 0x004E4663
		// (set) Token: 0x0600DA59 RID: 55897 RVA: 0x004E646B File Offset: 0x004E466B
		public bool AsServerOnly { get; set; }

		// Token: 0x17001B26 RID: 6950
		// (get) Token: 0x0600DA5A RID: 55898 RVA: 0x004E6474 File Offset: 0x004E4674
		// (set) Token: 0x0600DA5B RID: 55899 RVA: 0x004E647C File Offset: 0x004E467C
		public bool IsCrossplatform { get; set; }

		// Token: 0x17001B27 RID: 6951
		// (get) Token: 0x0600DA5C RID: 55900 RVA: 0x00010E62 File Offset: 0x0000F062
		public EPlatformIdentifier PlatformIdentifier
		{
			get
			{
				return EPlatformIdentifier.None;
			}
		}

		// Token: 0x17001B28 RID: 6952
		// (get) Token: 0x0600DA5D RID: 55901 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public string PlatformDisplayName
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600DA5E RID: 55902 RVA: 0x004E6485 File Offset: 0x004E4685
		public void Init()
		{
			this.User.Init(this);
			this.ServerListAnnouncer.Init(this);
			this.RichPresence.Init(this);
			this.UserDataRoaming.Init(this);
		}

		// Token: 0x0600DA5F RID: 55903 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool HasNetworkingEnabled(IList<string> _disabledProtocolNames)
		{
			return false;
		}

		// Token: 0x0600DA60 RID: 55904 RVA: 0x000880CC File Offset: 0x000862CC
		public INetworkServer GetNetworkingServer(ProtocolManager _protocolManager)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600DA61 RID: 55905 RVA: 0x000880CC File Offset: 0x000862CC
		public INetworkClient GetNetworkingClient(ProtocolManager _protocolManager)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600DA62 RID: 55906 RVA: 0x004E64B7 File Offset: 0x004E46B7
		public void UserAdded(PlatformUserIdentifierAbs _id, bool _isPrimary)
		{
			PlatformManager.NativePlatform.UserAdded(_id, _isPrimary);
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform == null)
			{
				return;
			}
			crossplatformPlatform.UserAdded(_id, _isPrimary);
		}

		// Token: 0x0600DA63 RID: 55907 RVA: 0x004C9A1B File Offset: 0x004C7C1B
		public string[] GetArgumentsForRelaunch()
		{
			return new string[0];
		}

		// Token: 0x0600DA64 RID: 55908 RVA: 0x004E64D6 File Offset: 0x004E46D6
		public void CreateInstances()
		{
			this.User = new User();
			this.ServerListAnnouncer = new ServerListAnnouncer();
			this.RichPresence = new RichPresence();
			this.PlayerInteractionsRecorder = new PlayerInteractionsRecorderMulti();
			this.UserDataRoaming = new UserDataRoamingMultiPlatform();
		}

		// Token: 0x0600DA65 RID: 55909 RVA: 0x000027FC File Offset: 0x000009FC
		public void Update()
		{
		}

		// Token: 0x0600DA66 RID: 55910 RVA: 0x000027FC File Offset: 0x000009FC
		public void LateUpdate()
		{
		}

		// Token: 0x0600DA67 RID: 55911 RVA: 0x004E650F File Offset: 0x004E470F
		public void Destroy()
		{
			this.ServerListAnnouncer = null;
			IUserClient user = this.User;
			if (user != null)
			{
				user.Destroy();
			}
			this.User = null;
		}

		// Token: 0x17001B29 RID: 6953
		// (get) Token: 0x0600DA68 RID: 55912 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IPlatformApi Api
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B2A RID: 6954
		// (get) Token: 0x0600DA69 RID: 55913 RVA: 0x004E6530 File Offset: 0x004E4730
		// (set) Token: 0x0600DA6A RID: 55914 RVA: 0x004E6538 File Offset: 0x004E4738
		public IUserClient User { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001B2B RID: 6955
		// (get) Token: 0x0600DA6B RID: 55915 RVA: 0x004E6541 File Offset: 0x004E4741
		// (set) Token: 0x0600DA6C RID: 55916 RVA: 0x004E6549 File Offset: 0x004E4749
		public IUserClient UserServer { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001B2C RID: 6956
		// (get) Token: 0x0600DA6D RID: 55917 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IAuthenticationClient AuthenticationClient
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B2D RID: 6957
		// (get) Token: 0x0600DA6E RID: 55918 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IAuthenticationServer AuthenticationServer
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B2E RID: 6958
		// (get) Token: 0x0600DA6F RID: 55919 RVA: 0x004E6554 File Offset: 0x004E4754
		public IList<IServerListInterface> ServerListInterfaces
		{
			get
			{
				if (this.serverListInterfaces != null)
				{
					return this.serverListInterfaces;
				}
				this.serverListInterfaces = new List<IServerListInterface>();
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				IList<IServerListInterface> list = (crossplatformPlatform != null) ? crossplatformPlatform.ServerListInterfaces : null;
				if (list != null)
				{
					this.serverListInterfaces.AddRange(list);
				}
				list = PlatformManager.NativePlatform.ServerListInterfaces;
				if (list != null)
				{
					this.serverListInterfaces.AddRange(list);
				}
				return this.serverListInterfaces;
			}
		}

		// Token: 0x17001B2F RID: 6959
		// (get) Token: 0x0600DA70 RID: 55920 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IServerListInterface ServerLookupInterface
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B30 RID: 6960
		// (get) Token: 0x0600DA71 RID: 55921 RVA: 0x004E65BC File Offset: 0x004E47BC
		// (set) Token: 0x0600DA72 RID: 55922 RVA: 0x004E65C4 File Offset: 0x004E47C4
		public IMasterServerAnnouncer ServerListAnnouncer { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001B31 RID: 6961
		// (get) Token: 0x0600DA73 RID: 55923 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public ILobbyHost LobbyHost
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B32 RID: 6962
		// (get) Token: 0x0600DA74 RID: 55924 RVA: 0x004E65CD File Offset: 0x004E47CD
		// (set) Token: 0x0600DA75 RID: 55925 RVA: 0x004E65D5 File Offset: 0x004E47D5
		public IPlayerInteractionsRecorder PlayerInteractionsRecorder { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001B33 RID: 6963
		// (get) Token: 0x0600DA76 RID: 55926 RVA: 0x004E65DE File Offset: 0x004E47DE
		// (set) Token: 0x0600DA77 RID: 55927 RVA: 0x004E65E6 File Offset: 0x004E47E6
		public IGameplayNotifier GameplayNotifier { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001B34 RID: 6964
		// (get) Token: 0x0600DA78 RID: 55928 RVA: 0x004E65EF File Offset: 0x004E47EF
		public IList<IJoinSessionGameInviteListener> InviteListeners
		{
			get
			{
				return PlatformManager.NativePlatform.InviteListeners;
			}
		}

		// Token: 0x17001B35 RID: 6965
		// (get) Token: 0x0600DA79 RID: 55929 RVA: 0x004E65FB File Offset: 0x004E47FB
		public IMultiplayerInvitationDialog MultiplayerInvitationDialog
		{
			get
			{
				return PlatformManager.NativePlatform.MultiplayerInvitationDialog;
			}
		}

		// Token: 0x17001B36 RID: 6966
		// (get) Token: 0x0600DA7A RID: 55930 RVA: 0x004E6607 File Offset: 0x004E4807
		public IPartyVoice PartyVoice
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.PartyVoice : null) ?? PlatformManager.NativePlatform.PartyVoice;
			}
		}

		// Token: 0x17001B37 RID: 6967
		// (get) Token: 0x0600DA7B RID: 55931 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IUtils Utils
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B38 RID: 6968
		// (get) Token: 0x0600DA7C RID: 55932 RVA: 0x004E6628 File Offset: 0x004E4828
		public IPlatformMemory Memory
		{
			get
			{
				return PlatformManager.NativePlatform.Memory;
			}
		}

		// Token: 0x17001B39 RID: 6969
		// (get) Token: 0x0600DA7D RID: 55933 RVA: 0x004E6634 File Offset: 0x004E4834
		public IAntiCheatClient AntiCheatClient
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.AntiCheatClient : null) ?? PlatformManager.NativePlatform.AntiCheatClient;
			}
		}

		// Token: 0x17001B3A RID: 6970
		// (get) Token: 0x0600DA7E RID: 55934 RVA: 0x004E6655 File Offset: 0x004E4855
		public IAntiCheatServer AntiCheatServer
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.AntiCheatServer : null) ?? PlatformManager.NativePlatform.AntiCheatServer;
			}
		}

		// Token: 0x17001B3B RID: 6971
		// (get) Token: 0x0600DA7F RID: 55935 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IUserIdentifierMappingService IdMappingService
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B3C RID: 6972
		// (get) Token: 0x0600DA80 RID: 55936 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IUserDetailsService UserDetailsService
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B3D RID: 6973
		// (get) Token: 0x0600DA81 RID: 55937 RVA: 0x004E6676 File Offset: 0x004E4876
		public IPlayerReporting PlayerReporting
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.PlayerReporting : null) ?? PlatformManager.NativePlatform.PlayerReporting;
			}
		}

		// Token: 0x17001B3E RID: 6974
		// (get) Token: 0x0600DA82 RID: 55938 RVA: 0x004E6697 File Offset: 0x004E4897
		public ITextCensor TextCensor
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				ITextCensor result;
				if ((result = ((crossplatformPlatform != null) ? crossplatformPlatform.TextCensor : null)) == null)
				{
					IPlatform nativePlatform = PlatformManager.NativePlatform;
					if (nativePlatform == null)
					{
						return null;
					}
					result = nativePlatform.TextCensor;
				}
				return result;
			}
		}

		// Token: 0x17001B3F RID: 6975
		// (get) Token: 0x0600DA83 RID: 55939 RVA: 0x004E66BE File Offset: 0x004E48BE
		public IRemoteFileStorage RemoteFileStorage
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.RemoteFileStorage : null) ?? PlatformManager.NativePlatform.RemoteFileStorage;
			}
		}

		// Token: 0x17001B40 RID: 6976
		// (get) Token: 0x0600DA84 RID: 55940 RVA: 0x004E66DF File Offset: 0x004E48DF
		public IRemotePlayerFileStorage RemotePlayerFileStorage
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				return ((crossplatformPlatform != null) ? crossplatformPlatform.RemotePlayerFileStorage : null) ?? PlatformManager.NativePlatform.RemotePlayerFileStorage;
			}
		}

		// Token: 0x17001B41 RID: 6977
		// (get) Token: 0x0600DA85 RID: 55941 RVA: 0x004E6700 File Offset: 0x004E4900
		public IList<IEntitlementValidator> EntitlementValidators
		{
			get
			{
				return PlatformManager.NativePlatform.EntitlementValidators;
			}
		}

		// Token: 0x17001B42 RID: 6978
		// (get) Token: 0x0600DA86 RID: 55942 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IPlatformNetworkServer NetworkServer
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B43 RID: 6979
		// (get) Token: 0x0600DA87 RID: 55943 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public PlayerInputManager Input
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B44 RID: 6980
		// (get) Token: 0x0600DA88 RID: 55944 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IVirtualKeyboard VirtualKeyboard
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B45 RID: 6981
		// (get) Token: 0x0600DA89 RID: 55945 RVA: 0x004E670C File Offset: 0x004E490C
		public IPlatformSaveGameProvider SaveGameProvider
		{
			get
			{
				return PlatformManager.NativePlatform.SaveGameProvider;
			}
		}

		// Token: 0x17001B46 RID: 6982
		// (get) Token: 0x0600DA8A RID: 55946 RVA: 0x004E6718 File Offset: 0x004E4918
		// (set) Token: 0x0600DA8B RID: 55947 RVA: 0x004E6720 File Offset: 0x004E4920
		public IUserDataRoaming UserDataRoaming { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001B47 RID: 6983
		// (get) Token: 0x0600DA8C RID: 55948 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IAchievementManager AchievementManager
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001B48 RID: 6984
		// (get) Token: 0x0600DA8D RID: 55949 RVA: 0x004E6729 File Offset: 0x004E4929
		// (set) Token: 0x0600DA8E RID: 55950 RVA: 0x004E6731 File Offset: 0x004E4931
		public IRichPresence RichPresence { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001B49 RID: 6985
		// (get) Token: 0x0600DA8F RID: 55951 RVA: 0x004E673A File Offset: 0x004E493A
		public IApplicationStateController ApplicationState
		{
			get
			{
				return PlatformManager.NativePlatform.ApplicationState;
			}
		}

		// Token: 0x0400A5BA RID: 42426
		[PublicizedFrom(EAccessModifier.Private)]
		public List<IServerListInterface> serverListInterfaces;
	}
}
