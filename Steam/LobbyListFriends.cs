using System;
using System.Collections.Generic;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001C90 RID: 7312
	public class LobbyListFriends : LobbyListAbs
	{
		// Token: 0x0600D8C1 RID: 55489 RVA: 0x004E0B40 File Offset: 0x004DED40
		public override void Init(IPlatform _owner)
		{
			this.owner = _owner;
			_owner.Api.ClientApiInitialized += delegate()
			{
				if (this.m_lobbyDataUpdate == null && !GameManager.IsDedicatedServer)
				{
					this.m_lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(new Callback<LobbyDataUpdate_t>.DispatchDelegate(this.Lobby_DataUpdate));
				}
			};
		}

		// Token: 0x0600D8C2 RID: 55490 RVA: 0x004E0B60 File Offset: 0x004DED60
		public override void StopSearch()
		{
			this.currentFriend = -1;
			this.isRefreshing = false;
		}

		// Token: 0x0600D8C3 RID: 55491 RVA: 0x004E0B70 File Offset: 0x004DED70
		public override void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters)
		{
			if (this.gameServerFoundCallback == null)
			{
				return;
			}
			this.isRefreshing = true;
			this.currentFriend = 0;
			this.queryNextFriend();
		}

		// Token: 0x0600D8C4 RID: 55492 RVA: 0x004E0B90 File Offset: 0x004DED90
		[PublicizedFrom(EAccessModifier.Private)]
		public void queryNextFriend()
		{
			while (this.currentFriend < SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll))
			{
				FriendGameInfo_t friendGameInfo_t;
				if (SteamFriends.GetFriendGamePlayed(SteamFriends.GetFriendByIndex(this.currentFriend, EFriendFlags.k_EFriendFlagAll), out friendGameInfo_t) && friendGameInfo_t.m_steamIDLobby != CSteamID.Nil)
				{
					SteamMatchmaking.RequestLobbyData(friendGameInfo_t.m_steamIDLobby);
					return;
				}
				this.currentFriend++;
			}
			ThreadManager.StartCoroutine(base.restartRefreshCo(2f));
		}

		// Token: 0x0600D8C5 RID: 55493 RVA: 0x004E0C08 File Offset: 0x004DEE08
		[PublicizedFrom(EAccessModifier.Private)]
		public void Lobby_DataUpdate(LobbyDataUpdate_t _val)
		{
			CSteamID lobbyId = new CSteamID(_val.m_ulSteamIDLobby);
			if (_val.m_bSuccess == 0)
			{
				return;
			}
			base.ParseLobbyData(lobbyId, EServerRelationType.Friends);
			if (this.currentFriend < 0)
			{
				return;
			}
			this.currentFriend++;
			this.queryNextFriend();
		}

		// Token: 0x0400A4D4 RID: 42196
		[PublicizedFrom(EAccessModifier.Private)]
		public Callback<LobbyDataUpdate_t> m_lobbyDataUpdate;

		// Token: 0x0400A4D5 RID: 42197
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentFriend;
	}
}
