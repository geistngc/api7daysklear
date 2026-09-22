using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL
{
	// Token: 0x02001C0D RID: 7181
	public class User : IUserClient
	{
		// Token: 0x0600D54B RID: 54603 RVA: 0x004D1008 File Offset: 0x004CF208
		[PublicizedFrom(EAccessModifier.Private)]
		static User()
		{
			Dictionary<EBlockType, XblPermission> dictionary = EnumUtils.Values<EBlockType>().ToDictionary((EBlockType blockType) => blockType, delegate(EBlockType blockType)
			{
				XblPermission result;
				switch (blockType)
				{
				case EBlockType.TextChat:
					result = XblPermission.CommunicateUsingText;
					break;
				case EBlockType.VoiceChat:
					result = XblPermission.CommunicateUsingVoice;
					break;
				case EBlockType.Play:
					result = XblPermission.PlayMultiplayer;
					break;
				default:
					throw new NotImplementedException(string.Format("Mapping from {0}.{1} to {2} not implemented!", "EBlockType", blockType, "XblPermission"));
				}
				return result;
			});
			User.userBlockedPermissions = dictionary.Values.ToArray<XblPermission>();
			User.xblPermissionToBlockType = new EnumDictionary<XblPermission, EBlockType>();
			foreach (KeyValuePair<EBlockType, XblPermission> keyValuePair in dictionary)
			{
				EBlockType eblockType;
				XblPermission xblPermission;
				keyValuePair.Deconstruct(out eblockType, out xblPermission);
				EBlockType value = eblockType;
				XblPermission key = xblPermission;
				User.xblPermissionToBlockType.Add(key, value);
			}
			User.userBlockedAnonymousTypes = new XblAnonymousUserType[]
			{
				XblAnonymousUserType.CrossNetworkFriend,
				XblAnonymousUserType.CrossNetworkUser
			};
		}

		// Token: 0x17001A70 RID: 6768
		// (get) Token: 0x0600D54C RID: 54604 RVA: 0x004D10EC File Offset: 0x004CF2EC
		public bool IsMultiplayerActivityActive
		{
			get
			{
				object activityLock = this.m_activityLock;
				bool isActivityActive;
				lock (activityLock)
				{
					isActivityActive = this.m_isActivityActive;
				}
				return isActivityActive;
			}
		}

		// Token: 0x17001A71 RID: 6769
		// (get) Token: 0x0600D54D RID: 54605 RVA: 0x004D1130 File Offset: 0x004CF330
		public XUserLocalId LocalID
		{
			get
			{
				return this.m_userLocalId;
			}
		}

		// Token: 0x17001A72 RID: 6770
		// (get) Token: 0x0600D54E RID: 54606 RVA: 0x004D1138 File Offset: 0x004CF338
		public ulong Xuid
		{
			get
			{
				return this.m_userXuid;
			}
		}

		// Token: 0x17001A73 RID: 6771
		// (get) Token: 0x0600D54F RID: 54607 RVA: 0x004D1140 File Offset: 0x004CF340
		public Unity.XGamingRuntime.XblContextHandle XblContextHandle
		{
			get
			{
				return this.m_contextHandle;
			}
		}

		// Token: 0x17001A74 RID: 6772
		// (get) Token: 0x0600D550 RID: 54608 RVA: 0x004D1148 File Offset: 0x004CF348
		public XUserHandle UserHandle
		{
			get
			{
				return this.m_userHandle;
			}
		}

		// Token: 0x17001A75 RID: 6773
		// (get) Token: 0x0600D551 RID: 54609 RVA: 0x004D1150 File Offset: 0x004CF350
		// (set) Token: 0x0600D552 RID: 54610 RVA: 0x004D1158 File Offset: 0x004CF358
		public EUserStatus UserStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EUserStatus.NotAttempted;

		// Token: 0x1400012C RID: 300
		// (add) Token: 0x0600D553 RID: 54611 RVA: 0x004D1164 File Offset: 0x004CF364
		// (remove) Token: 0x0600D554 RID: 54612 RVA: 0x004D11D0 File Offset: 0x004CF3D0
		public event Action<XUserHandle> UserHandleReady
		{
			add
			{
				object userHandleLock = this.m_userHandleLock;
				lock (userHandleLock)
				{
					this.m_userHandleReady = (Action<XUserHandle>)Delegate.Combine(this.m_userHandleReady, value);
					if (this.m_userHandle != null)
					{
						value(this.m_userHandle);
					}
				}
			}
			remove
			{
				object userHandleLock = this.m_userHandleLock;
				lock (userHandleLock)
				{
					this.m_userHandleReady = (Action<XUserHandle>)Delegate.Remove(this.m_userHandleReady, value);
				}
			}
		}

		// Token: 0x1400012D RID: 301
		// (add) Token: 0x0600D555 RID: 54613 RVA: 0x004D1224 File Offset: 0x004CF424
		// (remove) Token: 0x0600D556 RID: 54614 RVA: 0x004D128C File Offset: 0x004CF48C
		public event Action<IPlatform> UserLoggedIn
		{
			add
			{
				object loginLock = this.m_loginLock;
				lock (loginLock)
				{
					this.m_userLoggedIn = (Action<IPlatform>)Delegate.Combine(this.m_userLoggedIn, value);
					if (this.UserStatus == EUserStatus.LoggedIn)
					{
						value(this.m_owner);
					}
				}
			}
			remove
			{
				object loginLock = this.m_loginLock;
				lock (loginLock)
				{
					this.m_userLoggedIn = (Action<IPlatform>)Delegate.Remove(this.m_userLoggedIn, value);
				}
			}
		}

		// Token: 0x1400012E RID: 302
		// (add) Token: 0x0600D557 RID: 54615 RVA: 0x004D12E0 File Offset: 0x004CF4E0
		// (remove) Token: 0x0600D558 RID: 54616 RVA: 0x004D1318 File Offset: 0x004CF518
		public event UserBlocksChangedCallback UserBlocksChanged;

		// Token: 0x17001A76 RID: 6774
		// (get) Token: 0x0600D559 RID: 54617 RVA: 0x004D134D File Offset: 0x004CF54D
		public PlatformUserIdentifierAbs PlatformUserId
		{
			get
			{
				return this.m_idProvider.Id;
			}
		}

		// Token: 0x17001A77 RID: 6775
		// (get) Token: 0x0600D55A RID: 54618 RVA: 0x004D135A File Offset: 0x004CF55A
		// (set) Token: 0x0600D55B RID: 54619 RVA: 0x004D1362 File Offset: 0x004CF562
		public string GamerTag { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A78 RID: 6776
		// (get) Token: 0x0600D55C RID: 54620 RVA: 0x004D136B File Offset: 0x004CF56B
		// (set) Token: 0x0600D55D RID: 54621 RVA: 0x004D1373 File Offset: 0x004CF573
		public SocialManagerXbl SocialManager { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A79 RID: 6777
		// (get) Token: 0x0600D55E RID: 54622 RVA: 0x004D137C File Offset: 0x004CF57C
		// (set) Token: 0x0600D55F RID: 54623 RVA: 0x004D1384 File Offset: 0x004CF584
		public MultiplayerActivityQueryManager MultiplayerActivityQueryManager { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A7A RID: 6778
		// (get) Token: 0x0600D560 RID: 54624 RVA: 0x004D138D File Offset: 0x004CF58D
		// (set) Token: 0x0600D561 RID: 54625 RVA: 0x004D1395 File Offset: 0x004CF595
		public XblSandboxHelper SandboxHelper { get; [PublicizedFrom(EAccessModifier.Private)] set; } = new XblSandboxHelper();

		// Token: 0x0600D562 RID: 54626 RVA: 0x004D13A0 File Offset: 0x004CF5A0
		public void Init(IPlatform _owner)
		{
			this.m_owner = _owner;
			this.m_api = _owner.Api;
			this.m_appState = _owner.ApplicationState;
			this.m_api.ClientApiInitialized += this.OnClientApiInitialized;
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			this.m_idProvider = new IdProviderGameCore(this, (crossplatformPlatform != null) ? crossplatformPlatform.User : null);
			XblXuidMapper.XuidMapped += this.OnXuidMapped;
			this.UserLoggedIn += this.SetNameOnPlayerLogin;
			this.m_appState.OnNetworkStateChanged += this.OnNetworkStateChanged;
		}

		// Token: 0x0600D563 RID: 54627 RVA: 0x004D143C File Offset: 0x004CF63C
		public void Destroy()
		{
			this.m_appState.OnNetworkStateChanged -= this.OnNetworkStateChanged;
			if (this.m_userHandle != null)
			{
				SDK.XUserCloseHandle(this.m_userHandle);
				XblHelpers.LogHR(0, "Close User Handle.", false);
				this.m_userHandle = null;
				this.m_userLocalId = null;
			}
			this.m_loginUserCallback = null;
			this.m_api.ClientApiInitialized -= this.OnClientApiInitialized;
		}

		// Token: 0x0600D564 RID: 54628 RVA: 0x004D14B4 File Offset: 0x004CF6B4
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetNameOnPlayerLogin(IPlatform platform)
		{
			string text;
			if (Unity.XGamingRuntime.Interop.HR.FAILED(SDK.XUserGetGamertag(this.UserHandle, XUserGamertagComponent.Classic, out text)))
			{
				Log.Error("[XBL] Failed to get player's gamertag");
				return;
			}
			GamePrefs.Set(EnumGamePrefs.PlayerName, text);
			this.GamerTag = text;
		}

		// Token: 0x0600D565 RID: 54629 RVA: 0x004D14F0 File Offset: 0x004CF6F0
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnClientApiInitialized()
		{
			this.SandboxHelper.RefreshSandboxId();
			SDK.XUserAddAsync(XUserAddOptions.AddDefaultUserSilently, new XUserAddCompleted(this.<OnClientApiInitialized>g__XUserAddAsyncSilentCompletionRoutine|77_0));
		}

		// Token: 0x0600D566 RID: 54630 RVA: 0x004D1510 File Offset: 0x004CF710
		public void Login(LoginUserCallback _delegate)
		{
			User.<>c__DisplayClass78_0 CS$<>8__locals1 = new User.<>c__DisplayClass78_0();
			CS$<>8__locals1._delegate = _delegate;
			CS$<>8__locals1.<>4__this = this;
			object loginLock = this.m_loginLock;
			lock (loginLock)
			{
				if (this.m_loginDone)
				{
					this.UserStatus = this.m_loginUserStatus;
					XblHelpers.LogHR((this.m_loginUserStatus == EUserStatus.LoggedIn) ? 0 : -2147467259, string.Format("Login with cached initial results. User Status: {0}, Reason: {1}, Additional: '{2}'.", this.m_loginUserStatus, this.m_loginUserCallbackReason, this.m_loginUserCallbackReasonAdditional), false);
					CS$<>8__locals1.<Login>g__Callback|0(this.m_owner, this.m_loginUserCallbackReason, this.m_loginUserCallbackReasonAdditional);
				}
				else
				{
					this.m_loginUserCallback = new LoginUserCallback(CS$<>8__locals1.<Login>g__Callback|0);
				}
			}
		}

		// Token: 0x0600D567 RID: 54631 RVA: 0x004D15DC File Offset: 0x004CF7DC
		public void PlayOffline(LoginUserCallback _delegate)
		{
			if (!this.m_idProvider.LoadOfflineId())
			{
				this.UserStatus = EUserStatus.TemporaryError;
				_delegate(this.m_owner, EApiStatusReason.NoOnlineStart, null);
				return;
			}
			this.UserStatus = EUserStatus.OfflineMode;
			_delegate(this.m_owner, EApiStatusReason.Ok, null);
			Action<IPlatform> userLoggedIn = this.m_userLoggedIn;
			if (userLoggedIn == null)
			{
				return;
			}
			userLoggedIn(this.m_owner);
		}

		// Token: 0x0600D568 RID: 54632 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public Action IsConnectToServerFromCommandline()
		{
			return null;
		}

		// Token: 0x0600D569 RID: 54633 RVA: 0x004D1638 File Offset: 0x004CF838
		public void StartAdvertisePlaying(GameServerInfo _serverInfo)
		{
			if (_serverInfo == null)
			{
				return;
			}
			object activityLock = this.m_activityLock;
			lock (activityLock)
			{
				this.m_shouldActivityBeActive = true;
				if (_serverInfo != this.m_activityLastServerInfo)
				{
					if (this.m_activityLastServerInfo != null)
					{
						this.UnregisterGameServerInfoEvents(this.m_activityLastServerInfo);
					}
					this.m_activityLastServerInfo = _serverInfo;
					this.RegisterGameServerInfoEvents(_serverInfo);
				}
				this.SetActivity(_serverInfo);
			}
		}

		// Token: 0x0600D56A RID: 54634 RVA: 0x004D16B0 File Offset: 0x004CF8B0
		[PublicizedFrom(EAccessModifier.Private)]
		public void RegisterGameServerInfoEvents(GameServerInfo _serverInfo)
		{
			_serverInfo.OnChangedString += this.OnServerInfoChangedString;
			_serverInfo.OnChangedInt += this.OnServerInfoChangedInt;
		}

		// Token: 0x0600D56B RID: 54635 RVA: 0x004D16D6 File Offset: 0x004CF8D6
		[PublicizedFrom(EAccessModifier.Private)]
		public void UnregisterGameServerInfoEvents(GameServerInfo _serverInfo)
		{
			_serverInfo.OnChangedInt -= this.OnServerInfoChangedInt;
			_serverInfo.OnChangedString -= this.OnServerInfoChangedString;
		}

		// Token: 0x0600D56C RID: 54636 RVA: 0x004D16FC File Offset: 0x004CF8FC
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnServerInfoChangedString(GameServerInfo _serverInfo, GameInfoString _key)
		{
			if (_key == GameInfoString.UniqueId)
			{
				this.SetActivity(_serverInfo);
			}
		}

		// Token: 0x0600D56D RID: 54637 RVA: 0x004D170A File Offset: 0x004CF90A
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnServerInfoChangedInt(GameServerInfo _serverInfo, GameInfoInt _key)
		{
			if (_key == GameInfoInt.CurrentPlayers || _key == GameInfoInt.ServerVisibility || _key == GameInfoInt.MaxPlayers)
			{
				this.SetActivity(_serverInfo);
			}
		}

		// Token: 0x0600D56E RID: 54638 RVA: 0x004D1720 File Offset: 0x004CF920
		public void StopAdvertisePlaying()
		{
			object activityLock = this.m_activityLock;
			lock (activityLock)
			{
				this.m_shouldActivityBeActive = false;
				if (this.m_activityLastServerInfo != null)
				{
					this.UnregisterGameServerInfoEvents(this.m_activityLastServerInfo);
					this.m_activityLastServerInfo = null;
				}
				this.DeleteActivity();
			}
		}

		// Token: 0x0600D56F RID: 54639 RVA: 0x004D1784 File Offset: 0x004CF984
		[PublicizedFrom(EAccessModifier.Private)]
		public void DeleteActivity()
		{
			object activityLock = this.m_activityLock;
			lock (activityLock)
			{
				if (!this.m_isActivityActive || this.m_shouldActivityBeActive)
				{
					this.m_shouldRetryDeleteActivity = false;
					return;
				}
				if (this.m_currentlyDeletingActivity)
				{
					this.m_shouldRetryDeleteActivity = true;
					return;
				}
				this.m_shouldRetryDeleteActivity = false;
				this.m_currentlyDeletingActivity = true;
				SDK.XBL.XblMultiplayerActivityDeleteActivityAsync(this.XblContextHandle, new SDK.XBL.XblMultiplayerActivityAsyncOperationCompleted(this.<DeleteActivity>g__CompletionRoutine|87_0));
			}
		}

		// Token: 0x0600D570 RID: 54640 RVA: 0x004D1810 File Offset: 0x004CFA10
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnNetworkStateChanged(bool state)
		{
			object activityLock = this.m_activityLock;
			lock (activityLock)
			{
				if (this.m_isActivityActive && !this.m_shouldActivityBeActive)
				{
					this.DeleteActivity();
				}
			}
		}

		// Token: 0x0600D571 RID: 54641 RVA: 0x004D1860 File Offset: 0x004CFA60
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetActivity(GameServerInfo _serverInfo)
		{
			if (_serverInfo == null)
			{
				return;
			}
			string value = _serverInfo.GetValue(GameInfoString.UniqueId);
			int value2 = _serverInfo.GetValue(GameInfoInt.CurrentPlayers);
			int value3 = _serverInfo.GetValue(GameInfoInt.ServerVisibility);
			int value4 = _serverInfo.GetValue(GameInfoInt.MaxPlayers);
			if (value4 < 2)
			{
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			XblMultiplayerActivityInfo xblMultiplayerActivityInfo = new XblMultiplayerActivityInfo();
			xblMultiplayerActivityInfo.ConnectionString = value;
			xblMultiplayerActivityInfo.CurrentPlayers = (uint)value2;
			xblMultiplayerActivityInfo.GroupId = "Dummy";
			XblMultiplayerActivityInfo xblMultiplayerActivityInfo2 = xblMultiplayerActivityInfo;
			XblMultiplayerActivityJoinRestriction joinRestriction;
			if (value3 != 1)
			{
				if (value3 == 2)
				{
					joinRestriction = XblMultiplayerActivityJoinRestriction.Public;
				}
				else
				{
					joinRestriction = XblMultiplayerActivityJoinRestriction.InviteOnly;
				}
			}
			else
			{
				joinRestriction = XblMultiplayerActivityJoinRestriction.Followed;
			}
			xblMultiplayerActivityInfo2.JoinRestriction = joinRestriction;
			xblMultiplayerActivityInfo.MaxPlayers = (uint)value4;
			xblMultiplayerActivityInfo.Platform = XblMultiplayerActivityPlatform.All;
			xblMultiplayerActivityInfo.Xuid = this.m_idProvider.Id.Xuid;
			XblMultiplayerActivityInfo activityInfo = xblMultiplayerActivityInfo;
			object activityLock = this.m_activityLock;
			lock (activityLock)
			{
				if (this.m_shouldActivityBeActive)
				{
					SDK.XBL.XblMultiplayerActivitySetActivityAsync(this.XblContextHandle, activityInfo, true, new SDK.XBL.XblMultiplayerActivityAsyncOperationCompleted(this.<SetActivity>g__CompletionRoutine|89_0));
				}
			}
		}

		// Token: 0x0600D572 RID: 54642 RVA: 0x004D1968 File Offset: 0x004CFB68
		public void GetLoginTicket(Action<bool, byte[], string> _callback)
		{
			User.<>c__DisplayClass93_0 CS$<>8__locals1 = new User.<>c__DisplayClass93_0();
			CS$<>8__locals1._callback = _callback;
			if (this.m_userHandle == null)
			{
				Log.Error("[XBL] Attempting to retrieve XSTS token before acquiring XUserHandle");
				CS$<>8__locals1._callback(false, null, null);
				return;
			}
			SDK.XUserGetTokenAndSignatureUtf16Async(this.m_userHandle, XUserGetTokenAndSignatureOptions.None, "GET", "https://eos.epicgames.com", User.eosRelyingPartyRequestHeaders, null, new XUserGetTokenAndSignatureUtf16Result(CS$<>8__locals1.<GetLoginTicket>g__CompletionRoutine|0));
		}

		// Token: 0x0600D573 RID: 54643 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public string GetFriendName(PlatformUserIdentifierAbs _playerId)
		{
			return null;
		}

		// Token: 0x0600D574 RID: 54644 RVA: 0x004D19D4 File Offset: 0x004CFBD4
		public bool IsFriend(PlatformUserIdentifierAbs _playerId)
		{
			ulong xuid = XblXuidMapper.GetXuid(_playerId);
			if (xuid == 0UL)
			{
				return false;
			}
			FriendsListXbl friendsListXbl = this.friendsList;
			return friendsListXbl != null && friendsListXbl.IsFriend(xuid);
		}

		// Token: 0x0600D575 RID: 54645 RVA: 0x004D19FF File Offset: 0x004CFBFF
		public bool CanShowProfile(PlatformUserIdentifierAbs _playerId)
		{
			return XblXuidMapper.GetXuid(_playerId) > 0UL;
		}

		// Token: 0x0600D576 RID: 54646 RVA: 0x004D1A0C File Offset: 0x004CFC0C
		public void ShowProfile(PlatformUserIdentifierAbs _playerId)
		{
			ulong xuid = XblXuidMapper.GetXuid(_playerId);
			if (xuid != 0UL)
			{
				SDK.XGameUiShowPlayerProfileCardAsync(this.m_userHandle, xuid, delegate(int hr)
				{
					XblHelpers.LogHR(0, "XGameUiShowPlayerProfileCardAsync", false);
					if (Unity.XGamingRuntime.Interop.HR.FAILED(hr))
					{
						Log.Error("XBL-GXDK: Showing Player Profile Failed.");
						return;
					}
					Log.Out("XBL-GXDK: Showing Player Profile Succeeded.");
				});
			}
		}

		// Token: 0x17001A7B RID: 6779
		// (get) Token: 0x0600D577 RID: 54647 RVA: 0x004D1A50 File Offset: 0x004CFC50
		public EUserPerms Permissions
		{
			get
			{
				if (this.m_privilegeHelper == null)
				{
					return (EUserPerms)0;
				}
				EUserPerms euserPerms = (EUserPerms)0;
				if (this.m_privilegeHelper.MultiplayerAllowed.Has())
				{
					euserPerms |= (EUserPerms.Multiplayer | EUserPerms.HostMultiplayer);
				}
				if (this.m_privilegeHelper.CommunicationAllowed.Has())
				{
					euserPerms |= EUserPerms.Communication;
				}
				if (this.m_privilegeHelper.CrossPlayAllowed.Has())
				{
					euserPerms |= EUserPerms.Crossplay;
				}
				return euserPerms;
			}
		}

		// Token: 0x0600D578 RID: 54648 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public string GetPermissionDenyReason(EUserPerms _perms)
		{
			return null;
		}

		// Token: 0x0600D579 RID: 54649 RVA: 0x004D1AAD File Offset: 0x004CFCAD
		public IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
		{
			Log.Out(string.Format("[XBL] {0}({1}: [{2}], {3}: {4})", new object[]
			{
				"ResolvePermissions",
				"_perms",
				_perms,
				"_canPrompt",
				_canPrompt
			}));
			if (this.m_privilegeHelper == null)
			{
				yield break;
			}
			yield return this.m_privilegeHelper.ResolvePermissions(_perms, _canPrompt, _cancellationToken);
			yield break;
		}

		// Token: 0x0600D57A RID: 54650 RVA: 0x004D1AD1 File Offset: 0x004CFCD1
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnXuidMapped(IReadOnlyCollection<PlatformUserIdentifierAbs> userIds, ulong xuid)
		{
			UserBlocksChangedCallback userBlocksChanged = this.UserBlocksChanged;
			if (userBlocksChanged == null)
			{
				return;
			}
			userBlocksChanged(userIds);
		}

		// Token: 0x0600D57B RID: 54651 RVA: 0x004D1AE4 File Offset: 0x004CFCE4
		public IEnumerator ResolveUserBlocks(IReadOnlyList<IPlatformUserBlockedResults> _results)
		{
			User.<>c__DisplayClass103_0 CS$<>8__locals1 = new User.<>c__DisplayClass103_0();
			CS$<>8__locals1._results = _results;
			CS$<>8__locals1.<>4__this = this;
			this.userBlockedXuidToResultsTemp.Clear();
			this.userBlockedAnonymousResultsTemp.Clear();
			PlatformUserIdentifierAbs platformUserId = this.PlatformUserId;
			foreach (IPlatformUserBlockedResults platformUserBlockedResults in CS$<>8__locals1._results)
			{
				PlatformUserIdentifierAbs nativeId = platformUserBlockedResults.User.NativeId;
				if (!object.Equals(platformUserId, nativeId))
				{
					UserIdentifierXbl userIdentifierXbl = nativeId as UserIdentifierXbl;
					ulong xuid;
					if (userIdentifierXbl == null || (xuid = userIdentifierXbl.Xuid) == 0UL)
					{
						this.userBlockedAnonymousResultsTemp.Add(platformUserBlockedResults);
					}
					else
					{
						this.userBlockedXuidToResultsTemp[xuid] = platformUserBlockedResults;
					}
				}
			}
			CS$<>8__locals1.running = true;
			SDK.XBL.XblPrivacyBatchCheckPermissionAsync(this.XblContextHandle, User.userBlockedPermissions, this.userBlockedXuidToResultsTemp.Keys.ToArray<ulong>(), (this.userBlockedAnonymousResultsTemp.Count > 0) ? User.userBlockedAnonymousTypes : Array.Empty<XblAnonymousUserType>(), new XblPrivacyBatchCheckPermissionCompleted(CS$<>8__locals1.<ResolveUserBlocks>g__CompletionRoutine|0));
			while (CS$<>8__locals1.running)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600D57C RID: 54652 RVA: 0x004D1AFC File Offset: 0x004CFCFC
		public EMatchmakingGroup GetMatchmakingGroup()
		{
			string sandboxId = this.SandboxHelper.SandboxId;
			if (sandboxId == null)
			{
				EMatchmakingGroup ematchmakingGroup = EMatchmakingGroup.Retail;
				Log.Warning(string.Format("[XBL] {0} no sandbox id. Defaulting to {1}", "GetMatchmakingGroup", ematchmakingGroup));
				return ematchmakingGroup;
			}
			return XblSandboxHelper.SandboxIdToMatchmakingGroup(sandboxId);
		}

		// Token: 0x0600D57E RID: 54654 RVA: 0x004D1B98 File Offset: 0x004CFD98
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <OnClientApiInitialized>g__XUserAddAsyncSilentCompletionRoutine|77_0(int hr, XUserHandle userHandle)
		{
			XblHelpers.LogHR(hr, "XUserAddAsync: AddDefaultUserSilently", false);
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				this.<OnClientApiInitialized>g__XUserAddSucceeded|77_2(userHandle);
				return;
			}
			if (hr == -1994108666)
			{
				this.UserStatus = EUserStatus.TemporaryError;
				SDK.XUserAddAsync(XUserAddOptions.AddDefaultUserAllowingUI, new XUserAddCompleted(this.<OnClientApiInitialized>g__XUserAddAsyncAllowingUICompletionRoutine|77_1));
				return;
			}
			this.UserStatus = EUserStatus.TemporaryError;
			SDK.XUserAddAsync(XUserAddOptions.AddDefaultUserSilently, new XUserAddCompleted(this.<OnClientApiInitialized>g__XUserAddAsyncSilentCompletionRoutine|77_0));
		}

		// Token: 0x0600D57F RID: 54655 RVA: 0x004D1BFE File Offset: 0x004CFDFE
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <OnClientApiInitialized>g__XUserAddAsyncAllowingUICompletionRoutine|77_1(int hr, XUserHandle userHandle)
		{
			XblHelpers.LogHR(hr, "XUserAddAsync: AddDefaultUserAllowingUI", false);
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				this.<OnClientApiInitialized>g__XUserAddSucceeded|77_2(userHandle);
				return;
			}
			this.UserStatus = EUserStatus.TemporaryError;
			SDK.XUserAddAsync(XUserAddOptions.AddDefaultUserSilently, new XUserAddCompleted(this.<OnClientApiInitialized>g__XUserAddAsyncSilentCompletionRoutine|77_0));
		}

		// Token: 0x0600D580 RID: 54656 RVA: 0x004D1C38 File Offset: 0x004CFE38
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <OnClientApiInitialized>g__XUserAddSucceeded|77_2(XUserHandle userHandle)
		{
			object userHandleLock = this.m_userHandleLock;
			lock (userHandleLock)
			{
				this.m_userHandle = userHandle;
				Action<XUserHandle> userHandleReady = this.m_userHandleReady;
				if (userHandleReady != null)
				{
					userHandleReady(userHandle);
				}
			}
			this.m_privilegeHelper = new UserPrivilegeHelper(this.m_userHandle);
			this.m_privilegeHelper.AllAllowed.ResolveSilent();
			this.SocialManager = new SocialManagerXbl(this.m_userHandle);
			this.friendsList = new FriendsListXbl(this.SocialManager);
			this.<OnClientApiInitialized>g__GetXuid|77_3();
		}

		// Token: 0x0600D581 RID: 54657 RVA: 0x004D1CD4 File Offset: 0x004CFED4
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <OnClientApiInitialized>g__GetXuid|77_3()
		{
			int num = SDK.XUserGetId(this.m_userHandle, out this.m_userXuid);
			XblHelpers.LogHR(num, "XUserGetId", false);
			if (!Unity.XGamingRuntime.Interop.HR.FAILED(num))
			{
				this.<OnClientApiInitialized>g__PostLogin|77_6();
				return;
			}
			if (num == -1994108670)
			{
				this.<OnClientApiInitialized>g__ResolveXuidIssue|77_4();
				return;
			}
			this.<OnClientApiInitialized>g__DoLoginUserCallback|77_7(EUserStatus.PermanentError, EApiStatusReason.Other, "Could not obtain User's Xuid.");
		}

		// Token: 0x0600D582 RID: 54658 RVA: 0x004D1D2A File Offset: 0x004CFF2A
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <OnClientApiInitialized>g__ResolveXuidIssue|77_4()
		{
			SDK.XUserResolveIssueWithUiUtf16Async(this.m_userHandle, null, new XUserResolveIssueWithUiUtf16Result(this.<OnClientApiInitialized>g__ResolveXuidIssueCompletionRoutine|77_5));
		}

		// Token: 0x0600D583 RID: 54659 RVA: 0x004D1D45 File Offset: 0x004CFF45
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <OnClientApiInitialized>g__ResolveXuidIssueCompletionRoutine|77_5(int hrResolve)
		{
			XblHelpers.LogHR(hrResolve, "XUserResolveIssueWithUiUtf16Async", false);
			this.<OnClientApiInitialized>g__GetXuid|77_3();
		}

		// Token: 0x0600D584 RID: 54660 RVA: 0x004D1D5C File Offset: 0x004CFF5C
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <OnClientApiInitialized>g__PostLogin|77_6()
		{
			int hr = SDK.XUserGetLocalId(this.m_userHandle, out this.m_userLocalId);
			XblHelpers.LogHR(hr, "XUserGetLocalId", false);
			if (Unity.XGamingRuntime.Interop.HR.FAILED(hr))
			{
				this.<OnClientApiInitialized>g__DoLoginUserCallback|77_7(EUserStatus.PermanentError, EApiStatusReason.Other, "Could not obtain User's Local ID.");
				return;
			}
			int hr2 = SDK.XBL.XblContextCreateHandle(this.m_userHandle, out this.m_contextHandle);
			XblHelpers.LogHR(hr2, "Create Xbox Live Context Handle", false);
			if (Unity.XGamingRuntime.Interop.HR.FAILED(hr2))
			{
				this.<OnClientApiInitialized>g__DoLoginUserCallback|77_7(EUserStatus.PermanentError, EApiStatusReason.Other, "Could not obtain Xbox Live Context Handle.");
				return;
			}
			this.MultiplayerActivityQueryManager = new MultiplayerActivityQueryManager(this.m_contextHandle);
			this.<OnClientApiInitialized>g__DoLoginUserCallback|77_7(EUserStatus.LoggedIn, EApiStatusReason.Ok, null);
		}

		// Token: 0x0600D585 RID: 54661 RVA: 0x004D1DE8 File Offset: 0x004CFFE8
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <OnClientApiInitialized>g__DoLoginUserCallback|77_7(EUserStatus userStatus, EApiStatusReason reason, string reasonAdditional)
		{
			object loginLock = this.m_loginLock;
			lock (loginLock)
			{
				this.UserStatus = userStatus;
				this.m_loginUserStatus = userStatus;
				this.m_loginUserCallbackReason = reason;
				this.m_loginUserCallbackReasonAdditional = reasonAdditional;
				this.m_loginDone = true;
				XblHelpers.LogHR((userStatus == EUserStatus.LoggedIn) ? 0 : -2147467259, string.Format("Initial Login Callback Done. User Status: {0}, Reason: {1}, Additional: '{2}'.", userStatus, reason, reasonAdditional), false);
				LoginUserCallback loginUserCallback = this.m_loginUserCallback;
				if (loginUserCallback != null)
				{
					loginUserCallback(this.m_owner, reason, reasonAdditional);
				}
				this.m_loginUserCallback = null;
			}
		}

		// Token: 0x0600D586 RID: 54662 RVA: 0x004D1E90 File Offset: 0x004D0090
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <DeleteActivity>g__CompletionRoutine|87_0(int _hresult)
		{
			object activityLock = this.m_activityLock;
			lock (activityLock)
			{
				this.m_currentlyDeletingActivity = false;
				if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(_hresult))
				{
					if (this.m_isActivityActive)
					{
						Log.Out("[XBL] Activity Deleted");
						this.m_isActivityActive = false;
					}
					this.m_shouldRetryDeleteActivity = false;
					if (this.m_shouldActivityBeActive)
					{
						Log.Out("[XBL] Setting activity since it should be active.");
						this.SetActivity(this.m_activityLastServerInfo);
					}
				}
				else
				{
					XblHelpers.LogHR(_hresult, "Delete Activity", true);
					if (this.m_isActivityActive && !this.m_shouldActivityBeActive)
					{
						if (this.m_shouldRetryDeleteActivity)
						{
							Log.Warning("[XBL] Failed to delete activity. Will retry now because a change (e.g. Network State) happened while deleting the activity.");
							this.m_shouldRetryDeleteActivity = false;
							SDK.XBL.XblMultiplayerActivityDeleteActivityAsync(this.XblContextHandle, new SDK.XBL.XblMultiplayerActivityAsyncOperationCompleted(this.<DeleteActivity>g__CompletionRoutine|87_0));
						}
						else
						{
							Log.Warning("[XBL] Failed to delete activity. Will retry when a change occurs (e.g. Network State).");
						}
					}
				}
			}
		}

		// Token: 0x0600D587 RID: 54663 RVA: 0x004D1F74 File Offset: 0x004D0174
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <SetActivity>g__CompletionRoutine|89_0(int _hresult)
		{
			object activityLock = this.m_activityLock;
			lock (activityLock)
			{
				if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(_hresult))
				{
					if (!this.m_isActivityActive)
					{
						Log.Out("[XBL] Activity Created");
						this.m_isActivityActive = true;
					}
					if (!this.m_shouldActivityBeActive)
					{
						Log.Out("[XBL] Deleting activity since it should not be active.");
						this.DeleteActivity();
					}
				}
				else
				{
					XblHelpers.LogHR(_hresult, "Set Activity", false);
				}
			}
		}

		// Token: 0x0400A27A RID: 41594
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly EnumDictionary<XblPermission, EBlockType> xblPermissionToBlockType;

		// Token: 0x0400A27B RID: 41595
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly XblPermission[] userBlockedPermissions;

		// Token: 0x0400A27C RID: 41596
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly XblAnonymousUserType[] userBlockedAnonymousTypes;

		// Token: 0x0400A27D RID: 41597
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform m_owner;

		// Token: 0x0400A27E RID: 41598
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatformApi m_api;

		// Token: 0x0400A27F RID: 41599
		[PublicizedFrom(EAccessModifier.Private)]
		public IApplicationStateController m_appState;

		// Token: 0x0400A280 RID: 41600
		[PublicizedFrom(EAccessModifier.Private)]
		public XUserHandle m_userHandle;

		// Token: 0x0400A281 RID: 41601
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object m_userHandleLock = new object();

		// Token: 0x0400A282 RID: 41602
		[PublicizedFrom(EAccessModifier.Private)]
		public Action<XUserHandle> m_userHandleReady;

		// Token: 0x0400A283 RID: 41603
		[PublicizedFrom(EAccessModifier.Private)]
		public Unity.XGamingRuntime.XblContextHandle m_contextHandle;

		// Token: 0x0400A284 RID: 41604
		[PublicizedFrom(EAccessModifier.Private)]
		public XUserLocalId m_userLocalId;

		// Token: 0x0400A285 RID: 41605
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong m_userXuid;

		// Token: 0x0400A286 RID: 41606
		[PublicizedFrom(EAccessModifier.Private)]
		public UserIdentifierXbl m_platformUserId;

		// Token: 0x0400A287 RID: 41607
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object m_loginLock = new object();

		// Token: 0x0400A288 RID: 41608
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_loginDone;

		// Token: 0x0400A289 RID: 41609
		[PublicizedFrom(EAccessModifier.Private)]
		public EUserStatus m_loginUserStatus;

		// Token: 0x0400A28A RID: 41610
		[PublicizedFrom(EAccessModifier.Private)]
		public LoginUserCallback m_loginUserCallback;

		// Token: 0x0400A28B RID: 41611
		[PublicizedFrom(EAccessModifier.Private)]
		public EApiStatusReason m_loginUserCallbackReason;

		// Token: 0x0400A28C RID: 41612
		[PublicizedFrom(EAccessModifier.Private)]
		public string m_loginUserCallbackReasonAdditional;

		// Token: 0x0400A28D RID: 41613
		[PublicizedFrom(EAccessModifier.Private)]
		public UserPrivilegeHelper m_privilegeHelper;

		// Token: 0x0400A28E RID: 41614
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object m_activityLock = new object();

		// Token: 0x0400A28F RID: 41615
		[PublicizedFrom(EAccessModifier.Private)]
		public GameServerInfo m_activityLastServerInfo;

		// Token: 0x0400A290 RID: 41616
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_isActivityActive;

		// Token: 0x0400A291 RID: 41617
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_shouldActivityBeActive;

		// Token: 0x0400A292 RID: 41618
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_shouldRetryDeleteActivity;

		// Token: 0x0400A293 RID: 41619
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_currentlyDeletingActivity;

		// Token: 0x0400A294 RID: 41620
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<ulong, IPlatformUserBlockedResults> userBlockedXuidToResultsTemp = new Dictionary<ulong, IPlatformUserBlockedResults>();

		// Token: 0x0400A295 RID: 41621
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<IPlatformUserBlockedResults> userBlockedAnonymousResultsTemp = new List<IPlatformUserBlockedResults>();

		// Token: 0x0400A297 RID: 41623
		public string SandboxName;

		// Token: 0x0400A298 RID: 41624
		[PublicizedFrom(EAccessModifier.Private)]
		public Action<IPlatform> m_userLoggedIn;

		// Token: 0x0400A29A RID: 41626
		[PublicizedFrom(EAccessModifier.Private)]
		public IdProviderGameCore m_idProvider;

		// Token: 0x0400A29D RID: 41629
		[PublicizedFrom(EAccessModifier.Private)]
		public FriendsListXbl friendsList;

		// Token: 0x0400A2A0 RID: 41632
		[PublicizedFrom(EAccessModifier.Private)]
		public const string eosRelyingPartyUrl = "https://eos.epicgames.com";

		// Token: 0x0400A2A1 RID: 41633
		[PublicizedFrom(EAccessModifier.Private)]
		public const string eosRelyingPartyHttpMethod = "GET";

		// Token: 0x0400A2A2 RID: 41634
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly XUserGetTokenAndSignatureUtf16HttpHeader[] eosRelyingPartyRequestHeaders = new XUserGetTokenAndSignatureUtf16HttpHeader[]
		{
			new XUserGetTokenAndSignatureUtf16HttpHeader
			{
				Name = "X-XBL-Contract-Version",
				Value = "2"
			}
		};
	}
}
