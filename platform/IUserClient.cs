using System;
using System.Collections;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001BAC RID: 7084
	public interface IUserClient
	{
		// Token: 0x17001A20 RID: 6688
		// (get) Token: 0x0600D35D RID: 54109
		EUserStatus UserStatus { get; }

		// Token: 0x14000126 RID: 294
		// (add) Token: 0x0600D35E RID: 54110
		// (remove) Token: 0x0600D35F RID: 54111
		event Action<IPlatform> UserLoggedIn;

		// Token: 0x14000127 RID: 295
		// (add) Token: 0x0600D360 RID: 54112
		// (remove) Token: 0x0600D361 RID: 54113
		event UserBlocksChangedCallback UserBlocksChanged;

		// Token: 0x17001A21 RID: 6689
		// (get) Token: 0x0600D362 RID: 54114
		PlatformUserIdentifierAbs PlatformUserId { get; }

		// Token: 0x0600D363 RID: 54115
		void Init(IPlatform _owner);

		// Token: 0x0600D364 RID: 54116
		void Login(LoginUserCallback _delegate);

		// Token: 0x0600D365 RID: 54117
		void PlayOffline(LoginUserCallback _delegate);

		// Token: 0x0600D366 RID: 54118 RVA: 0x00010E62 File Offset: 0x0000F062
		EMatchmakingGroup GetMatchmakingGroup()
		{
			return EMatchmakingGroup.Dev;
		}

		// Token: 0x0600D367 RID: 54119
		void StartAdvertisePlaying(GameServerInfo _serverInfo);

		// Token: 0x0600D368 RID: 54120
		void StopAdvertisePlaying();

		// Token: 0x0600D369 RID: 54121
		void GetLoginTicket(Action<bool, byte[], string> _callback);

		// Token: 0x0600D36A RID: 54122
		string GetFriendName(PlatformUserIdentifierAbs _playerId);

		// Token: 0x0600D36B RID: 54123
		bool IsFriend(PlatformUserIdentifierAbs _playerId);

		// Token: 0x0600D36C RID: 54124 RVA: 0x00010E62 File Offset: 0x0000F062
		bool CanShowProfile(PlatformUserIdentifierAbs _playerId)
		{
			return false;
		}

		// Token: 0x0600D36D RID: 54125 RVA: 0x000027FC File Offset: 0x000009FC
		void ShowProfile(PlatformUserIdentifierAbs _playerId)
		{
		}

		// Token: 0x17001A22 RID: 6690
		// (get) Token: 0x0600D36E RID: 54126
		EUserPerms Permissions { get; }

		// Token: 0x0600D36F RID: 54127
		string GetPermissionDenyReason(EUserPerms _perms);

		// Token: 0x0600D370 RID: 54128
		IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken cancellationToken = null);

		// Token: 0x0600D371 RID: 54129
		IEnumerator ResolveUserBlocks(IReadOnlyList<IPlatformUserBlockedResults> _results);

		// Token: 0x0600D372 RID: 54130
		void Destroy();
	}
}
