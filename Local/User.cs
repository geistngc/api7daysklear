using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Platform.Local
{
	// Token: 0x02001CCE RID: 7374
	public class User : IUserClient
	{
		// Token: 0x17001B55 RID: 6997
		// (get) Token: 0x0600DAE6 RID: 56038 RVA: 0x004E75A4 File Offset: 0x004E57A4
		// (set) Token: 0x0600DAE7 RID: 56039 RVA: 0x004E75AC File Offset: 0x004E57AC
		public EUserStatus UserStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1400013D RID: 317
		// (add) Token: 0x0600DAE8 RID: 56040 RVA: 0x004E75B8 File Offset: 0x004E57B8
		// (remove) Token: 0x0600DAE9 RID: 56041 RVA: 0x000027FC File Offset: 0x000009FC
		public event Action<IPlatform> UserLoggedIn
		{
			add
			{
				lock (this)
				{
					value(this.owner);
				}
			}
			remove
			{
			}
		}

		// Token: 0x1400013E RID: 318
		// (add) Token: 0x0600DAEA RID: 56042 RVA: 0x000027FC File Offset: 0x000009FC
		// (remove) Token: 0x0600DAEB RID: 56043 RVA: 0x000027FC File Offset: 0x000009FC
		public event UserBlocksChangedCallback UserBlocksChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		// Token: 0x0600DAEC RID: 56044 RVA: 0x004E75FC File Offset: 0x004E57FC
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			GamePrefs.OnGamePrefChanged += delegate(EnumGamePrefs _pref)
			{
				if (_pref == EnumGamePrefs.PlayerName)
				{
					this.platformUserId = new UserIdentifierLocal(GamePrefs.GetString(EnumGamePrefs.PlayerName));
				}
			};
		}

		// Token: 0x17001B56 RID: 6998
		// (get) Token: 0x0600DAED RID: 56045 RVA: 0x004E7616 File Offset: 0x004E5816
		public PlatformUserIdentifierAbs PlatformUserId
		{
			get
			{
				return this.platformUserId;
			}
		}

		// Token: 0x0600DAEE RID: 56046 RVA: 0x004E761E File Offset: 0x004E581E
		public void Login(LoginUserCallback _delegate)
		{
			this.platformUserId = new UserIdentifierLocal(GamePrefs.GetString(EnumGamePrefs.PlayerName));
			_delegate(this.owner, EApiStatusReason.Ok, null);
		}

		// Token: 0x0600DAEF RID: 56047 RVA: 0x004E7641 File Offset: 0x004E5841
		public void PlayOffline(LoginUserCallback _delegate)
		{
			this.UserStatus = EUserStatus.OfflineMode;
			_delegate(this.owner, EApiStatusReason.Ok, null);
		}

		// Token: 0x0600DAF0 RID: 56048 RVA: 0x000027FC File Offset: 0x000009FC
		public void StartAdvertisePlaying(GameServerInfo _serverInfo)
		{
		}

		// Token: 0x0600DAF1 RID: 56049 RVA: 0x000027FC File Offset: 0x000009FC
		public void StopAdvertisePlaying()
		{
		}

		// Token: 0x0600DAF2 RID: 56050 RVA: 0x000880CC File Offset: 0x000862CC
		public void GetLoginTicket(Action<bool, byte[], string> _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600DAF3 RID: 56051 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public string GetFriendName(PlatformUserIdentifierAbs _playerId)
		{
			return null;
		}

		// Token: 0x0600DAF4 RID: 56052 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool IsFriend(PlatformUserIdentifierAbs _playerId)
		{
			return true;
		}

		// Token: 0x17001B57 RID: 6999
		// (get) Token: 0x0600DAF5 RID: 56053 RVA: 0x001B2E28 File Offset: 0x001B1028
		public EUserPerms Permissions
		{
			get
			{
				return EUserPerms.All;
			}
		}

		// Token: 0x0600DAF6 RID: 56054 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public string GetPermissionDenyReason(EUserPerms _perms)
		{
			return null;
		}

		// Token: 0x0600DAF7 RID: 56055 RVA: 0x004E45F0 File Offset: 0x004E27F0
		public IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
		{
			return Enumerable.Empty<object>().GetEnumerator();
		}

		// Token: 0x0600DAF8 RID: 56056 RVA: 0x000027FC File Offset: 0x000009FC
		public void UserAdded(PlatformUserIdentifierAbs _userId, bool _isPrimary)
		{
		}

		// Token: 0x0600DAF9 RID: 56057 RVA: 0x004E45F0 File Offset: 0x004E27F0
		public IEnumerator ResolveUserBlocks(IReadOnlyList<IPlatformUserBlockedResults> _results)
		{
			return Enumerable.Empty<object>().GetEnumerator();
		}

		// Token: 0x0600DAFA RID: 56058 RVA: 0x000027FC File Offset: 0x000009FC
		public void Destroy()
		{
		}

		// Token: 0x0400A5D3 RID: 42451
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A5D4 RID: 42452
		[PublicizedFrom(EAccessModifier.Private)]
		public UserIdentifierLocal platformUserId;
	}
}
