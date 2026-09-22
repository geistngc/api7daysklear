using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Platform.Steam
{
	// Token: 0x02001C8E RID: 7310
	public abstract class LobbyListAbs : IServerListInterface
	{
		// Token: 0x0600D8AF RID: 55471 RVA: 0x004E09E9 File Offset: 0x004DEBE9
		[PublicizedFrom(EAccessModifier.Protected)]
		public LobbyListAbs()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			Application.wantsToQuit += this.OnApplicationQuit;
		}

		// Token: 0x17001AE2 RID: 6882
		// (get) Token: 0x0600D8B0 RID: 55472 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsPrefiltered
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600D8B1 RID: 55473
		public abstract void Init(IPlatform _owner);

		// Token: 0x0600D8B2 RID: 55474 RVA: 0x004E0A0B File Offset: 0x004DEC0B
		public void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _errorCallback)
		{
			this.gameServerFoundCallback = _serverFound;
		}

		// Token: 0x17001AE3 RID: 6883
		// (get) Token: 0x0600D8B3 RID: 55475 RVA: 0x004E0A14 File Offset: 0x004DEC14
		public bool IsRefreshing
		{
			get
			{
				return this.isRefreshing;
			}
		}

		// Token: 0x0600D8B4 RID: 55476
		public abstract void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters);

		// Token: 0x0600D8B5 RID: 55477
		public abstract void StopSearch();

		// Token: 0x0600D8B6 RID: 55478 RVA: 0x004E0A1C File Offset: 0x004DEC1C
		public virtual void Disconnect()
		{
			this.StopSearch();
			this.gameServerFoundCallback = null;
		}

		// Token: 0x0600D8B7 RID: 55479 RVA: 0x000880CC File Offset: 0x000862CC
		public void GetSingleServerDetails(GameServerInfo _serverInfo, EServerRelationType _relation, GameServerFoundCallback _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600D8B8 RID: 55480 RVA: 0x004E0A2B File Offset: 0x004DEC2B
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool OnApplicationQuit()
		{
			this.Disconnect();
			return true;
		}

		// Token: 0x0600D8B9 RID: 55481 RVA: 0x004E0A34 File Offset: 0x004DEC34
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator restartRefreshCo(float _delay)
		{
			yield return new WaitForSeconds(_delay);
			this.StartSearch(null);
			yield break;
		}

		// Token: 0x0600D8BA RID: 55482 RVA: 0x004E0A4C File Offset: 0x004DEC4C
		[PublicizedFrom(EAccessModifier.Protected)]
		public void ParseLobbyData(CSteamID _lobbyId, EServerRelationType _source)
		{
			if (this.gameServerFoundCallback == null)
			{
				return;
			}
			GameServerInfo gameServerInfo = new GameServerInfo
			{
				IsLobby = true
			};
			int lobbyDataCount = SteamMatchmaking.GetLobbyDataCount(_lobbyId);
			for (int i = 0; i < lobbyDataCount; i++)
			{
				string key;
				string value;
				if (SteamMatchmaking.GetLobbyDataByIndex(_lobbyId, i, out key, 100, out value, 200))
				{
					gameServerInfo.ParseAny(key, value);
				}
			}
			if (PlatformManager.CrossplatformPlatform == null)
			{
				gameServerInfo.SetValue(GameInfoString.UniqueId, gameServerInfo.GetValue(GameInfoString.SteamID));
			}
			gameServerInfo.IsFriends = (_source == EServerRelationType.Friends);
			this.gameServerFoundCallback(this.owner, gameServerInfo, _source);
		}

		// Token: 0x0400A4CD RID: 42189
		[PublicizedFrom(EAccessModifier.Protected)]
		public IPlatform owner;

		// Token: 0x0400A4CE RID: 42190
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameServerFoundCallback gameServerFoundCallback;

		// Token: 0x0400A4CF RID: 42191
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isRefreshing;
	}
}
