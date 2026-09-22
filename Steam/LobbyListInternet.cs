using System;
using System.Collections.Generic;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001C91 RID: 7313
	public class LobbyListInternet : LobbyListAbs
	{
		// Token: 0x0600D8C8 RID: 55496 RVA: 0x004E0C81 File Offset: 0x004DEE81
		public override void Init(IPlatform _owner)
		{
			this.owner = _owner;
			_owner.Api.ClientApiInitialized += delegate()
			{
				if (this.m_RequestLobbies == null && !GameManager.IsDedicatedServer)
				{
					this.m_RequestLobbies = CallResult<LobbyMatchList_t>.Create(new CallResult<LobbyMatchList_t>.APIDispatchDelegate(this.RequestLobbies_CallResult));
				}
			};
		}

		// Token: 0x0600D8C9 RID: 55497 RVA: 0x004E0CA1 File Offset: 0x004DEEA1
		public override void StopSearch()
		{
			if (this.m_RequestLobbies != null && this.m_RequestLobbies.IsActive())
			{
				this.m_RequestLobbies.Cancel();
			}
			this.isRefreshing = false;
		}

		// Token: 0x0600D8CA RID: 55498 RVA: 0x004E0CCC File Offset: 0x004DEECC
		public override void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters)
		{
			if (this.gameServerFoundCallback == null)
			{
				return;
			}
			SteamMatchmaking.AddRequestLobbyListStringFilter("CompatibilityVersion", global::Constants.cVersionInformation.LongStringNoBuild, ELobbyComparison.k_ELobbyComparisonEqual);
			SteamAPICall_t hAPICall = SteamMatchmaking.RequestLobbyList();
			this.m_RequestLobbies.Set(hAPICall, null);
			this.isRefreshing = true;
		}

		// Token: 0x0600D8CB RID: 55499 RVA: 0x004E0D14 File Offset: 0x004DEF14
		[PublicizedFrom(EAccessModifier.Private)]
		public void RequestLobbies_CallResult(LobbyMatchList_t _val, bool _ioFailure)
		{
			if (_ioFailure)
			{
				Log.Out("[Steamworks.NET] RequestLobbies failed");
			}
			else
			{
				int num = 0;
				while ((long)num < (long)((ulong)_val.m_nLobbiesMatching))
				{
					base.ParseLobbyData(SteamMatchmaking.GetLobbyByIndex(num), EServerRelationType.Internet);
					num++;
				}
			}
			ThreadManager.StartCoroutine(base.restartRefreshCo(3f));
		}

		// Token: 0x0400A4D6 RID: 42198
		[PublicizedFrom(EAccessModifier.Private)]
		public CallResult<LobbyMatchList_t> m_RequestLobbies;
	}
}
