using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using InControl;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001CA5 RID: 7333
	public class User : IUserClient
	{
		// Token: 0x17001AF9 RID: 6905
		// (get) Token: 0x0600D96D RID: 55661 RVA: 0x004E41B3 File Offset: 0x004E23B3
		// (set) Token: 0x0600D96E RID: 55662 RVA: 0x004E41BB File Offset: 0x004E23BB
		public EUserStatus UserStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EUserStatus.NotAttempted;

		// Token: 0x14000137 RID: 311
		// (add) Token: 0x0600D96F RID: 55663 RVA: 0x004E41C4 File Offset: 0x004E23C4
		// (remove) Token: 0x0600D970 RID: 55664 RVA: 0x004E4224 File Offset: 0x004E2424
		public event Action<IPlatform> UserLoggedIn
		{
			add
			{
				lock (this)
				{
					this.userLoggedIn = (Action<IPlatform>)Delegate.Combine(this.userLoggedIn, value);
					if (this.UserStatus == EUserStatus.LoggedIn)
					{
						value(this.owner);
					}
				}
			}
			remove
			{
				lock (this)
				{
					this.userLoggedIn = (Action<IPlatform>)Delegate.Remove(this.userLoggedIn, value);
				}
			}
		}

		// Token: 0x14000138 RID: 312
		// (add) Token: 0x0600D971 RID: 55665 RVA: 0x000027FC File Offset: 0x000009FC
		// (remove) Token: 0x0600D972 RID: 55666 RVA: 0x000027FC File Offset: 0x000009FC
		public event UserBlocksChangedCallback UserBlocksChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		// Token: 0x17001AFA RID: 6906
		// (get) Token: 0x0600D973 RID: 55667 RVA: 0x004E4270 File Offset: 0x004E2470
		public PlatformUserIdentifierAbs PlatformUserId
		{
			get
			{
				return this.platformUserId;
			}
		}

		// Token: 0x0600D974 RID: 55668 RVA: 0x004E4278 File Offset: 0x004E2478
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += delegate()
			{
				if (!GameManager.IsDedicatedServer && this.m_gameOverlayActivated == null)
				{
					this.m_gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.GameOverlayActivated));
				}
			};
		}

		// Token: 0x0600D975 RID: 55669 RVA: 0x004E42A0 File Offset: 0x004E24A0
		public void Login(LoginUserCallback _delegate)
		{
			if (this.UserStatus == EUserStatus.LoggedIn)
			{
				Log.Out("[Steamworks.NET] Already logged in");
				_delegate(this.owner, EApiStatusReason.Ok, null);
				return;
			}
			if (this.owner.Api.ClientApiStatus == EApiStatus.PermanentError)
			{
				Log.Out("[Steamworks.NET] API could not be loaded.");
				this.UserStatus = EUserStatus.PermanentError;
				_delegate(this.owner, EApiStatusReason.ApiNotLoadable, null);
				return;
			}
			if (this.owner.Api.ClientApiStatus == EApiStatus.TemporaryError)
			{
				this.owner.Api.InitClientApis();
				if (this.owner.Api.ClientApiStatus == EApiStatus.TemporaryError)
				{
					Log.Out("[Steamworks.NET] API could not be initialized - probably Steam not running.");
					this.UserStatus = EUserStatus.TemporaryError;
					_delegate(this.owner, EApiStatusReason.SteamNotRunning, null);
					return;
				}
			}
			if (!SteamApps.BIsSubscribedApp((AppId_t)251570U))
			{
				Log.Out("[Steamworks.NET] User not licensed for app.");
				this.UserStatus = EUserStatus.PermanentError;
				_delegate(this.owner, EApiStatusReason.NoLicense, null);
				return;
			}
			string personaName = SteamFriends.GetPersonaName();
			if (string.IsNullOrEmpty(personaName))
			{
				Log.Out("[Steamworks.NET] Username not found.");
				this.UserStatus = EUserStatus.TemporaryError;
				_delegate(this.owner, EApiStatusReason.NoFriendsName, null);
				return;
			}
			GamePrefs.Set(EnumGamePrefs.PlayerName, personaName);
			this.platformUserId = new UserIdentifierSteam(SteamUser.GetSteamID());
			if (!SteamUser.BLoggedOn())
			{
				this.UserStatus = EUserStatus.OfflineMode;
				Log.Out("[Steamworks.NET] User not logged in.");
				_delegate(this.owner, EApiStatusReason.NotLoggedOn, null);
				return;
			}
			Log.Out("[Steamworks.NET] Login ok.");
			this.UserStatus = EUserStatus.LoggedIn;
			Action<IPlatform> action = this.userLoggedIn;
			if (action != null)
			{
				action(this.owner);
			}
			_delegate(this.owner, EApiStatusReason.Ok, null);
		}

		// Token: 0x0600D976 RID: 55670 RVA: 0x004E4430 File Offset: 0x004E2630
		public void PlayOffline(LoginUserCallback _delegate)
		{
			if (this.UserStatus != EUserStatus.OfflineMode && this.UserStatus != EUserStatus.LoggedIn)
			{
				throw new Exception("Can not explicitly set Steam to offline mode");
			}
			this.UserStatus = EUserStatus.OfflineMode;
			Action<IPlatform> action = this.userLoggedIn;
			if (action != null)
			{
				action(this.owner);
			}
			_delegate(this.owner, EApiStatusReason.Ok, null);
		}

		// Token: 0x0600D977 RID: 55671 RVA: 0x000027FC File Offset: 0x000009FC
		public void StartAdvertisePlaying(GameServerInfo _serverInfo)
		{
		}

		// Token: 0x0600D978 RID: 55672 RVA: 0x000027FC File Offset: 0x000009FC
		public void StopAdvertisePlaying()
		{
		}

		// Token: 0x0600D979 RID: 55673 RVA: 0x004E4488 File Offset: 0x004E2688
		public void GetLoginTicket(Action<bool, byte[], string> _callback)
		{
			if (this.requestEncryptedAppTicketCallback == null)
			{
				this.requestEncryptedAppTicketCallback = CallResult<EncryptedAppTicketResponse_t>.Create(new CallResult<EncryptedAppTicketResponse_t>.APIDispatchDelegate(this.EncryptedAppTicketCallback));
			}
			this.encryptedAppTicketCallback = _callback;
			SteamAPICall_t hAPICall = SteamUser.RequestEncryptedAppTicket(null, 0);
			this.requestEncryptedAppTicketCallback.Set(hAPICall, null);
		}

		// Token: 0x0600D97A RID: 55674 RVA: 0x004E44D0 File Offset: 0x004E26D0
		[PublicizedFrom(EAccessModifier.Private)]
		public void EncryptedAppTicketCallback(EncryptedAppTicketResponse_t _result, bool _ioFailure)
		{
			if (_ioFailure || _result.m_eResult != EResult.k_EResultOK)
			{
				this.<EncryptedAppTicketCallback>g__Callback|24_0(null, "[Steamworks.NET] RequestEncryptedAppTicket failed (result=" + _result.m_eResult.ToStringCached<EResult>() + ")");
				return;
			}
			uint num;
			SteamUser.GetEncryptedAppTicket(null, 0, out num);
			if (num == 0U || num > 1024U)
			{
				this.<EncryptedAppTicketCallback>g__Callback|24_0(null, string.Format("[Steamworks.NET] Fetching encrypted app ticket size: {0}", num));
				return;
			}
			byte[] array = new byte[num];
			uint num2;
			if (!SteamUser.GetEncryptedAppTicket(array, (int)num, out num2))
			{
				this.<EncryptedAppTicketCallback>g__Callback|24_0(null, "[Steamworks.NET] Failed fetching encrypted app ticket");
				return;
			}
			if (num2 != num)
			{
				this.<EncryptedAppTicketCallback>g__Callback|24_0(null, string.Format("[Steamworks.NET] Ticket size expected {0} does not match ticket size received {1}", num, num2));
				return;
			}
			this.<EncryptedAppTicketCallback>g__Callback|24_0(array, null);
		}

		// Token: 0x0600D97B RID: 55675 RVA: 0x004E4584 File Offset: 0x004E2784
		public string GetFriendName(PlatformUserIdentifierAbs _playerId)
		{
			UserIdentifierSteam userIdentifierSteam = _playerId as UserIdentifierSteam;
			if (userIdentifierSteam == null)
			{
				return null;
			}
			return SteamFriends.GetFriendPersonaName(new CSteamID(userIdentifierSteam.SteamId));
		}

		// Token: 0x0600D97C RID: 55676 RVA: 0x004E45B0 File Offset: 0x004E27B0
		public bool IsFriend(PlatformUserIdentifierAbs _playerId)
		{
			UserIdentifierSteam userIdentifierSteam = _playerId as UserIdentifierSteam;
			return userIdentifierSteam != null && this.owner.Api.ClientApiStatus == EApiStatus.Ok && SteamFriends.GetFriendRelationship(new CSteamID(userIdentifierSteam.SteamId)) == EFriendRelationship.k_EFriendRelationshipFriend;
		}

		// Token: 0x17001AFB RID: 6907
		// (get) Token: 0x0600D97D RID: 55677 RVA: 0x001B2E28 File Offset: 0x001B1028
		public EUserPerms Permissions
		{
			get
			{
				return EUserPerms.All;
			}
		}

		// Token: 0x0600D97E RID: 55678 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public string GetPermissionDenyReason(EUserPerms _perms)
		{
			return null;
		}

		// Token: 0x0600D97F RID: 55679 RVA: 0x004E45F0 File Offset: 0x004E27F0
		public IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
		{
			return Enumerable.Empty<object>().GetEnumerator();
		}

		// Token: 0x0600D980 RID: 55680 RVA: 0x000027FC File Offset: 0x000009FC
		public void UserAdded(PlatformUserIdentifierAbs _userId, bool _isPrimary)
		{
		}

		// Token: 0x0600D981 RID: 55681 RVA: 0x004E45F0 File Offset: 0x004E27F0
		public IEnumerator ResolveUserBlocks(IReadOnlyList<IPlatformUserBlockedResults> _results)
		{
			return Enumerable.Empty<object>().GetEnumerator();
		}

		// Token: 0x0600D982 RID: 55682 RVA: 0x0002F184 File Offset: 0x0002D384
		public EMatchmakingGroup GetMatchmakingGroup()
		{
			return EMatchmakingGroup.Retail;
		}

		// Token: 0x0600D983 RID: 55683 RVA: 0x000027FC File Offset: 0x000009FC
		public void Destroy()
		{
		}

		// Token: 0x0600D984 RID: 55684 RVA: 0x004E45FC File Offset: 0x004E27FC
		[PublicizedFrom(EAccessModifier.Private)]
		public void GameOverlayActivated(GameOverlayActivated_t _val)
		{
			InputManager.Enabled = (_val.m_bActive == 0);
		}

		// Token: 0x0600D987 RID: 55687 RVA: 0x004E4643 File Offset: 0x004E2843
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <EncryptedAppTicketCallback>g__Callback|24_0(byte[] _ticket, string _message)
		{
			if (_message != null)
			{
				Log.Error(_message);
			}
			Action<bool, byte[], string> action = this.encryptedAppTicketCallback;
			if (action != null)
			{
				action(_message == null, _ticket, null);
			}
			this.encryptedAppTicketCallback = null;
		}

		// Token: 0x0400A547 RID: 42311
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A549 RID: 42313
		[PublicizedFrom(EAccessModifier.Private)]
		public Action<IPlatform> userLoggedIn;

		// Token: 0x0400A54A RID: 42314
		[PublicizedFrom(EAccessModifier.Private)]
		public Callback<GameOverlayActivated_t> m_gameOverlayActivated;

		// Token: 0x0400A54B RID: 42315
		[PublicizedFrom(EAccessModifier.Private)]
		public UserIdentifierSteam platformUserId;

		// Token: 0x0400A54C RID: 42316
		[PublicizedFrom(EAccessModifier.Private)]
		public CallResult<EncryptedAppTicketResponse_t> requestEncryptedAppTicketCallback;

		// Token: 0x0400A54D RID: 42317
		[PublicizedFrom(EAccessModifier.Private)]
		public Action<bool, byte[], string> encryptedAppTicketCallback;
	}
}
