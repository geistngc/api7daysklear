using System;
using System.Collections.Generic;
using InControl;

namespace Platform
{
	// Token: 0x02001B54 RID: 6996
	public abstract class AbsPlatform : IPlatform
	{
		// Token: 0x0600D162 RID: 53602 RVA: 0x004C95C0 File Offset: 0x004C77C0
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsPlatform()
		{
			Type typeFromHandle = typeof(PlatformFactoryAttribute);
			object[] customAttributes = base.GetType().GetCustomAttributes(typeFromHandle, false);
			if (customAttributes.Length != 1)
			{
				throw new Exception("Platform has no PlatformFactory attribute");
			}
			PlatformFactoryAttribute platformFactoryAttribute = (PlatformFactoryAttribute)customAttributes[0];
			this.PlatformIdentifier = platformFactoryAttribute.TargetPlatform;
		}

		// Token: 0x170019B0 RID: 6576
		// (get) Token: 0x0600D163 RID: 53603 RVA: 0x004C9610 File Offset: 0x004C7810
		// (set) Token: 0x0600D164 RID: 53604 RVA: 0x004C9618 File Offset: 0x004C7818
		public bool AsServerOnly { get; set; }

		// Token: 0x170019B1 RID: 6577
		// (get) Token: 0x0600D165 RID: 53605 RVA: 0x004C9621 File Offset: 0x004C7821
		// (set) Token: 0x0600D166 RID: 53606 RVA: 0x004C9629 File Offset: 0x004C7829
		public bool IsCrossplatform { get; set; }

		// Token: 0x170019B2 RID: 6578
		// (get) Token: 0x0600D167 RID: 53607 RVA: 0x004C9632 File Offset: 0x004C7832
		public string PlatformDisplayName
		{
			get
			{
				return PlatformManager.GetPlatformDisplayName(this.PlatformIdentifier);
			}
		}

		// Token: 0x0600D168 RID: 53608 RVA: 0x004C9640 File Offset: 0x004C7840
		public virtual void Init()
		{
			Log.Out("[Platform] Initializing " + this.PlatformIdentifier.ToString());
			IPlatformApi api = this.Api;
			if (api != null)
			{
				api.Init(this);
			}
			IUserClient user = this.User;
			if (user != null)
			{
				user.Init(this);
			}
			IUserClient userServer = this.UserServer;
			if (userServer != null)
			{
				userServer.Init(this);
			}
			IAuthenticationClient authenticationClient = this.AuthenticationClient;
			if (authenticationClient != null)
			{
				authenticationClient.Init(this);
			}
			IAuthenticationServer authenticationServer = this.AuthenticationServer;
			if (authenticationServer != null)
			{
				authenticationServer.Init(this);
			}
			if (this.ServerListInterfaces != null)
			{
				foreach (IServerListInterface serverListInterface in this.ServerListInterfaces)
				{
					serverListInterface.Init(this);
				}
			}
			IMasterServerAnnouncer serverListAnnouncer = this.ServerListAnnouncer;
			if (serverListAnnouncer != null)
			{
				serverListAnnouncer.Init(this);
			}
			if (this.InviteListeners != null)
			{
				foreach (IJoinSessionGameInviteListener joinSessionGameInviteListener in this.InviteListeners)
				{
					if (joinSessionGameInviteListener != null)
					{
						joinSessionGameInviteListener.Init(this);
					}
				}
			}
			IMultiplayerInvitationDialog multiplayerInvitationDialog = this.MultiplayerInvitationDialog;
			if (multiplayerInvitationDialog != null)
			{
				multiplayerInvitationDialog.Init(this);
			}
			ILobbyHost lobbyHost = this.LobbyHost;
			if (lobbyHost != null)
			{
				lobbyHost.Init(this);
			}
			IPlayerInteractionsRecorder playerInteractionsRecorder = this.PlayerInteractionsRecorder;
			if (playerInteractionsRecorder != null)
			{
				playerInteractionsRecorder.Init(this);
			}
			IGameplayNotifier gameplayNotifier = this.GameplayNotifier;
			if (gameplayNotifier != null)
			{
				gameplayNotifier.Init(this);
			}
			IPartyVoice partyVoice = this.PartyVoice;
			if (partyVoice != null)
			{
				partyVoice.Init(this);
			}
			IUtils utils = this.Utils;
			if (utils != null)
			{
				utils.Init(this);
			}
			IUtils utils2 = this.Utils;
			if (utils2 != null)
			{
				utils2.ClearTempFiles();
			}
			if (this.Utils != null)
			{
				InputManager.OnDeviceDetached += this.Utils.ControllerDisconnected;
			}
			IAntiCheatClient antiCheatClient = this.AntiCheatClient;
			if (antiCheatClient != null)
			{
				antiCheatClient.Init(this);
			}
			IAntiCheatServer antiCheatServer = this.AntiCheatServer;
			if (antiCheatServer != null)
			{
				antiCheatServer.Init(this);
			}
			IPlayerReporting playerReporting = this.PlayerReporting;
			if (playerReporting != null)
			{
				playerReporting.Init(this);
			}
			ITextCensor textCensor = this.TextCensor;
			if (textCensor != null)
			{
				textCensor.Init(this);
			}
			IVirtualKeyboard virtualKeyboard = this.VirtualKeyboard;
			if (virtualKeyboard != null)
			{
				virtualKeyboard.Init(this);
			}
			IPlatformSaveGameProvider saveGameProvider = this.SaveGameProvider;
			if (saveGameProvider != null)
			{
				saveGameProvider.Init(this);
			}
			IUserDataRoaming userDataRoaming = this.UserDataRoaming;
			if (userDataRoaming != null)
			{
				userDataRoaming.Init(this);
			}
			IAchievementManager achievementManager = this.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.Init(this);
			}
			IRichPresence richPresence = this.RichPresence;
			if (richPresence != null)
			{
				richPresence.Init(this);
			}
			IRemoteFileStorage remoteFileStorage = this.RemoteFileStorage;
			if (remoteFileStorage != null)
			{
				remoteFileStorage.Init(this);
			}
			IRemotePlayerFileStorage remotePlayerFileStorage = this.RemotePlayerFileStorage;
			if (remotePlayerFileStorage != null)
			{
				remotePlayerFileStorage.Init(this);
			}
			if (this.EntitlementValidators != null)
			{
				foreach (IEntitlementValidator entitlementValidator in this.EntitlementValidators)
				{
					if (entitlementValidator != null)
					{
						entitlementValidator.Init(this);
					}
				}
			}
			IApplicationStateController applicationState = this.ApplicationState;
			if (applicationState != null)
			{
				applicationState.Init(this);
			}
			IUserDetailsService userDetailsService = this.UserDetailsService;
			if (userDetailsService != null)
			{
				userDetailsService.Init(this);
			}
			if (!GameManager.IsDedicatedServer && !this.AsServerOnly)
			{
				IPlatformApi api2 = this.Api;
				if (api2 != null)
				{
					api2.InitClientApis();
				}
			}
			if (GameManager.IsDedicatedServer)
			{
				IPlatformApi api3 = this.Api;
				if (api3 == null)
				{
					return;
				}
				api3.InitServerApis();
			}
		}

		// Token: 0x0600D169 RID: 53609 RVA: 0x004C9974 File Offset: 0x004C7B74
		public virtual bool HasNetworkingEnabled(IList<string> _disabledProtocolNames)
		{
			string networkProtocolName = this.NetworkProtocolName;
			if (string.IsNullOrEmpty(networkProtocolName))
			{
				return false;
			}
			string text = networkProtocolName.ToLowerInvariant();
			bool flag = GameUtils.GetLaunchArgument("no" + text) == null && !_disabledProtocolNames.Contains(text);
			if (!flag)
			{
				Log.Out("[NET] Disabling protocol: " + networkProtocolName);
			}
			return flag;
		}

		// Token: 0x0600D16A RID: 53610 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual IPlatformNetworkServer instantiateNetworkServer(ProtocolManager _protocolManager)
		{
			return null;
		}

		// Token: 0x0600D16B RID: 53611 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual IPlatformNetworkClient instantiateNetworkClient(ProtocolManager _protocolManager)
		{
			return null;
		}

		// Token: 0x0600D16C RID: 53612 RVA: 0x004C99CC File Offset: 0x004C7BCC
		public INetworkServer GetNetworkingServer(ProtocolManager _protocolManager)
		{
			IPlatformNetworkServer result;
			if ((result = this.NetworkServer) == null)
			{
				result = (this.NetworkServer = this.instantiateNetworkServer(_protocolManager));
			}
			return result;
		}

		// Token: 0x0600D16D RID: 53613 RVA: 0x004C99F4 File Offset: 0x004C7BF4
		public INetworkClient GetNetworkingClient(ProtocolManager _protocolManager)
		{
			IPlatformNetworkClient result;
			if ((result = this.NetworkClient) == null)
			{
				result = (this.NetworkClient = this.instantiateNetworkClient(_protocolManager));
			}
			return result;
		}

		// Token: 0x0600D16E RID: 53614 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void UserAdded(PlatformUserIdentifierAbs _id, bool _isPrimary)
		{
		}

		// Token: 0x0600D16F RID: 53615 RVA: 0x004C9A1B File Offset: 0x004C7C1B
		public virtual string[] GetArgumentsForRelaunch()
		{
			return new string[0];
		}

		// Token: 0x0600D170 RID: 53616
		public abstract void CreateInstances();

		// Token: 0x0600D171 RID: 53617 RVA: 0x004C9A24 File Offset: 0x004C7C24
		public virtual void Update()
		{
			IPlatformApi api = this.Api;
			if (api != null)
			{
				api.Update();
			}
			IMasterServerAnnouncer serverListAnnouncer = this.ServerListAnnouncer;
			if (serverListAnnouncer != null)
			{
				serverListAnnouncer.Update();
			}
			IAntiCheatServer antiCheatServer = this.AntiCheatServer;
			if (antiCheatServer != null)
			{
				antiCheatServer.Update();
			}
			PlayerInputManager input = this.Input;
			if (input != null)
			{
				input.Update();
			}
			ITextCensor textCensor = this.TextCensor;
			if (textCensor != null)
			{
				textCensor.Update();
			}
			IApplicationStateController applicationState = this.ApplicationState;
			if (applicationState == null)
			{
				return;
			}
			applicationState.Update();
		}

		// Token: 0x0600D172 RID: 53618 RVA: 0x000027FC File Offset: 0x000009FC
		public void LateUpdate()
		{
		}

		// Token: 0x0600D173 RID: 53619 RVA: 0x004C9A98 File Offset: 0x004C7C98
		public virtual void Destroy()
		{
			this.RichPresence = null;
			IAchievementManager achievementManager = this.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.Destroy();
			}
			this.AchievementManager = null;
			this.Input = null;
			IPlatformSaveGameProvider saveGameProvider = this.SaveGameProvider;
			if (saveGameProvider != null)
			{
				saveGameProvider.Destroy();
			}
			this.SaveGameProvider = null;
			this.UserDataRoaming = null;
			IApplicationStateController applicationState = this.ApplicationState;
			if (applicationState != null)
			{
				applicationState.Destroy();
			}
			this.ApplicationState = null;
			IVirtualKeyboard virtualKeyboard = this.VirtualKeyboard;
			if (virtualKeyboard != null)
			{
				virtualKeyboard.Destroy();
			}
			this.VirtualKeyboard = null;
			this.NetworkClient = null;
			this.NetworkServer = null;
			this.PlayerReporting = null;
			this.TextCensor = null;
			IAntiCheatServer antiCheatServer = this.AntiCheatServer;
			if (antiCheatServer != null)
			{
				antiCheatServer.Destroy();
			}
			this.AntiCheatServer = null;
			IAntiCheatClient antiCheatClient = this.AntiCheatClient;
			if (antiCheatClient != null)
			{
				antiCheatClient.Destroy();
			}
			this.AntiCheatClient = null;
			this.Memory = null;
			IUtils utils = this.Utils;
			if (utils != null)
			{
				utils.ClearTempFiles();
			}
			this.Utils = null;
			IPartyVoice partyVoice = this.PartyVoice;
			if (partyVoice != null)
			{
				partyVoice.Destroy();
			}
			this.PartyVoice = null;
			this.LobbyHost = null;
			IPlayerInteractionsRecorder playerInteractionsRecorder = this.PlayerInteractionsRecorder;
			if (playerInteractionsRecorder != null)
			{
				playerInteractionsRecorder.Destroy();
			}
			this.PlayerInteractionsRecorder = null;
			this.InviteListeners = null;
			this.ServerListAnnouncer = null;
			this.ServerListInterfaces = null;
			this.AuthenticationServer = null;
			IAuthenticationClient authenticationClient = this.AuthenticationClient;
			if (authenticationClient != null)
			{
				authenticationClient.Destroy();
			}
			this.AuthenticationClient = null;
			IUserClient userServer = this.UserServer;
			if (userServer != null)
			{
				userServer.Destroy();
			}
			this.UserServer = null;
			IUserClient user = this.User;
			if (user != null)
			{
				user.Destroy();
			}
			this.User = null;
			IPlatformApi api = this.Api;
			if (api != null)
			{
				api.Destroy();
			}
			this.Api = null;
		}

		// Token: 0x170019B3 RID: 6579
		// (get) Token: 0x0600D174 RID: 53620 RVA: 0x004C9C38 File Offset: 0x004C7E38
		public EPlatformIdentifier PlatformIdentifier { get; }

		// Token: 0x170019B4 RID: 6580
		// (get) Token: 0x0600D175 RID: 53621 RVA: 0x004C9C40 File Offset: 0x004C7E40
		// (set) Token: 0x0600D176 RID: 53622 RVA: 0x004C9C48 File Offset: 0x004C7E48
		public IPlatformApi Api { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019B5 RID: 6581
		// (get) Token: 0x0600D177 RID: 53623 RVA: 0x004C9C51 File Offset: 0x004C7E51
		// (set) Token: 0x0600D178 RID: 53624 RVA: 0x004C9C59 File Offset: 0x004C7E59
		public IUserClient User { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019B6 RID: 6582
		// (get) Token: 0x0600D179 RID: 53625 RVA: 0x004C9C62 File Offset: 0x004C7E62
		// (set) Token: 0x0600D17A RID: 53626 RVA: 0x004C9C6A File Offset: 0x004C7E6A
		public IUserClient UserServer { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019B7 RID: 6583
		// (get) Token: 0x0600D17B RID: 53627 RVA: 0x004C9C73 File Offset: 0x004C7E73
		// (set) Token: 0x0600D17C RID: 53628 RVA: 0x004C9C7B File Offset: 0x004C7E7B
		public IAuthenticationClient AuthenticationClient { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019B8 RID: 6584
		// (get) Token: 0x0600D17D RID: 53629 RVA: 0x004C9C84 File Offset: 0x004C7E84
		// (set) Token: 0x0600D17E RID: 53630 RVA: 0x004C9C8C File Offset: 0x004C7E8C
		public IAuthenticationServer AuthenticationServer { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019B9 RID: 6585
		// (get) Token: 0x0600D17F RID: 53631 RVA: 0x004C9C95 File Offset: 0x004C7E95
		// (set) Token: 0x0600D180 RID: 53632 RVA: 0x004C9C9D File Offset: 0x004C7E9D
		public IList<IServerListInterface> ServerListInterfaces { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019BA RID: 6586
		// (get) Token: 0x0600D181 RID: 53633 RVA: 0x004C9CA6 File Offset: 0x004C7EA6
		// (set) Token: 0x0600D182 RID: 53634 RVA: 0x004C9CAE File Offset: 0x004C7EAE
		public IServerListInterface ServerLookupInterface { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019BB RID: 6587
		// (get) Token: 0x0600D183 RID: 53635 RVA: 0x004C9CB7 File Offset: 0x004C7EB7
		// (set) Token: 0x0600D184 RID: 53636 RVA: 0x004C9CBF File Offset: 0x004C7EBF
		public IMasterServerAnnouncer ServerListAnnouncer { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019BC RID: 6588
		// (get) Token: 0x0600D185 RID: 53637 RVA: 0x004C9CC8 File Offset: 0x004C7EC8
		// (set) Token: 0x0600D186 RID: 53638 RVA: 0x004C9CD0 File Offset: 0x004C7ED0
		public ILobbyHost LobbyHost { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019BD RID: 6589
		// (get) Token: 0x0600D187 RID: 53639 RVA: 0x004C9CD9 File Offset: 0x004C7ED9
		// (set) Token: 0x0600D188 RID: 53640 RVA: 0x004C9CE1 File Offset: 0x004C7EE1
		public IPlayerInteractionsRecorder PlayerInteractionsRecorder { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019BE RID: 6590
		// (get) Token: 0x0600D189 RID: 53641 RVA: 0x004C9CEA File Offset: 0x004C7EEA
		// (set) Token: 0x0600D18A RID: 53642 RVA: 0x004C9CF2 File Offset: 0x004C7EF2
		public IGameplayNotifier GameplayNotifier { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019BF RID: 6591
		// (get) Token: 0x0600D18B RID: 53643 RVA: 0x004C9CFB File Offset: 0x004C7EFB
		// (set) Token: 0x0600D18C RID: 53644 RVA: 0x004C9D03 File Offset: 0x004C7F03
		public IList<IJoinSessionGameInviteListener> InviteListeners { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C0 RID: 6592
		// (get) Token: 0x0600D18D RID: 53645 RVA: 0x004C9D0C File Offset: 0x004C7F0C
		// (set) Token: 0x0600D18E RID: 53646 RVA: 0x004C9D14 File Offset: 0x004C7F14
		public IMultiplayerInvitationDialog MultiplayerInvitationDialog { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C1 RID: 6593
		// (get) Token: 0x0600D18F RID: 53647 RVA: 0x004C9D1D File Offset: 0x004C7F1D
		// (set) Token: 0x0600D190 RID: 53648 RVA: 0x004C9D25 File Offset: 0x004C7F25
		public IPartyVoice PartyVoice { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C2 RID: 6594
		// (get) Token: 0x0600D191 RID: 53649 RVA: 0x004C9D2E File Offset: 0x004C7F2E
		// (set) Token: 0x0600D192 RID: 53650 RVA: 0x004C9D36 File Offset: 0x004C7F36
		public IUtils Utils { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C3 RID: 6595
		// (get) Token: 0x0600D193 RID: 53651 RVA: 0x004C9D3F File Offset: 0x004C7F3F
		// (set) Token: 0x0600D194 RID: 53652 RVA: 0x004C9D47 File Offset: 0x004C7F47
		public IPlatformMemory Memory { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C4 RID: 6596
		// (get) Token: 0x0600D195 RID: 53653 RVA: 0x004C9D50 File Offset: 0x004C7F50
		// (set) Token: 0x0600D196 RID: 53654 RVA: 0x004C9D58 File Offset: 0x004C7F58
		public IAntiCheatClient AntiCheatClient { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C5 RID: 6597
		// (get) Token: 0x0600D197 RID: 53655 RVA: 0x004C9D61 File Offset: 0x004C7F61
		// (set) Token: 0x0600D198 RID: 53656 RVA: 0x004C9D69 File Offset: 0x004C7F69
		public IAntiCheatServer AntiCheatServer { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C6 RID: 6598
		// (get) Token: 0x0600D199 RID: 53657 RVA: 0x004C9D72 File Offset: 0x004C7F72
		// (set) Token: 0x0600D19A RID: 53658 RVA: 0x004C9D7A File Offset: 0x004C7F7A
		public IUserIdentifierMappingService IdMappingService { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C7 RID: 6599
		// (get) Token: 0x0600D19B RID: 53659 RVA: 0x004C9D83 File Offset: 0x004C7F83
		// (set) Token: 0x0600D19C RID: 53660 RVA: 0x004C9D8B File Offset: 0x004C7F8B
		public IUserDetailsService UserDetailsService { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C8 RID: 6600
		// (get) Token: 0x0600D19D RID: 53661 RVA: 0x004C9D94 File Offset: 0x004C7F94
		// (set) Token: 0x0600D19E RID: 53662 RVA: 0x004C9D9C File Offset: 0x004C7F9C
		public IPlayerReporting PlayerReporting { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019C9 RID: 6601
		// (get) Token: 0x0600D19F RID: 53663 RVA: 0x004C9DA5 File Offset: 0x004C7FA5
		// (set) Token: 0x0600D1A0 RID: 53664 RVA: 0x004C9DAD File Offset: 0x004C7FAD
		public ITextCensor TextCensor { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019CA RID: 6602
		// (get) Token: 0x0600D1A1 RID: 53665 RVA: 0x004C9DB6 File Offset: 0x004C7FB6
		// (set) Token: 0x0600D1A2 RID: 53666 RVA: 0x004C9DBE File Offset: 0x004C7FBE
		public IRemoteFileStorage RemoteFileStorage { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019CB RID: 6603
		// (get) Token: 0x0600D1A3 RID: 53667 RVA: 0x004C9DC7 File Offset: 0x004C7FC7
		// (set) Token: 0x0600D1A4 RID: 53668 RVA: 0x004C9DCF File Offset: 0x004C7FCF
		public IRemotePlayerFileStorage RemotePlayerFileStorage { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019CC RID: 6604
		// (get) Token: 0x0600D1A5 RID: 53669 RVA: 0x004C9DD8 File Offset: 0x004C7FD8
		// (set) Token: 0x0600D1A6 RID: 53670 RVA: 0x004C9DE0 File Offset: 0x004C7FE0
		public IList<IEntitlementValidator> EntitlementValidators { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019CD RID: 6605
		// (get) Token: 0x0600D1A7 RID: 53671 RVA: 0x004C9DE9 File Offset: 0x004C7FE9
		// (set) Token: 0x0600D1A8 RID: 53672 RVA: 0x004C9DF1 File Offset: 0x004C7FF1
		public PlayerInputManager Input { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019CE RID: 6606
		// (get) Token: 0x0600D1A9 RID: 53673 RVA: 0x004C9DFA File Offset: 0x004C7FFA
		// (set) Token: 0x0600D1AA RID: 53674 RVA: 0x004C9E02 File Offset: 0x004C8002
		public IVirtualKeyboard VirtualKeyboard { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019CF RID: 6607
		// (get) Token: 0x0600D1AB RID: 53675 RVA: 0x004C9E0B File Offset: 0x004C800B
		// (set) Token: 0x0600D1AC RID: 53676 RVA: 0x004C9E13 File Offset: 0x004C8013
		public IAchievementManager AchievementManager { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019D0 RID: 6608
		// (get) Token: 0x0600D1AD RID: 53677 RVA: 0x004C9E1C File Offset: 0x004C801C
		// (set) Token: 0x0600D1AE RID: 53678 RVA: 0x004C9E24 File Offset: 0x004C8024
		public IRichPresence RichPresence { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019D1 RID: 6609
		// (get) Token: 0x0600D1AF RID: 53679 RVA: 0x004C9E2D File Offset: 0x004C802D
		// (set) Token: 0x0600D1B0 RID: 53680 RVA: 0x004C9E35 File Offset: 0x004C8035
		public IApplicationStateController ApplicationState { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019D2 RID: 6610
		// (get) Token: 0x0600D1B1 RID: 53681 RVA: 0x004C9E3E File Offset: 0x004C803E
		// (set) Token: 0x0600D1B2 RID: 53682 RVA: 0x004C9E46 File Offset: 0x004C8046
		public IPlatformSaveGameProvider SaveGameProvider { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019D3 RID: 6611
		// (get) Token: 0x0600D1B3 RID: 53683 RVA: 0x004C9E4F File Offset: 0x004C804F
		// (set) Token: 0x0600D1B4 RID: 53684 RVA: 0x004C9E57 File Offset: 0x004C8057
		public IUserDataRoaming UserDataRoaming { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170019D4 RID: 6612
		// (get) Token: 0x0600D1B5 RID: 53685 RVA: 0x004C9E60 File Offset: 0x004C8060
		public virtual string NetworkProtocolName { [PublicizedFrom(EAccessModifier.Protected)] get; }

		// Token: 0x0400A0CD RID: 41165
		[PublicizedFrom(EAccessModifier.Protected)]
		public IPlatformNetworkServer NetworkServer;

		// Token: 0x0400A0CE RID: 41166
		[PublicizedFrom(EAccessModifier.Protected)]
		public IPlatformNetworkClient NetworkClient;
	}
}
