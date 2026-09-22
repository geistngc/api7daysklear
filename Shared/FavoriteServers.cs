using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platform.Shared
{
	// Token: 0x02001CAB RID: 7339
	public class FavoriteServers : IServerListInterface
	{
		// Token: 0x0600D9B1 RID: 55729 RVA: 0x004E4D90 File Offset: 0x004E2F90
		public FavoriteServers()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			Application.wantsToQuit += delegate()
			{
				this.Disconnect();
				return true;
			};
		}

		// Token: 0x17001B03 RID: 6915
		// (get) Token: 0x0600D9B2 RID: 55730 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsPrefiltered
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600D9B3 RID: 55731 RVA: 0x004E4DB1 File Offset: 0x004E2FB1
		public void Init(IPlatform _owner)
		{
			if (GameManager.IsDedicatedServer || this.initDone)
			{
				return;
			}
			this.owner = _owner;
		}

		// Token: 0x0600D9B4 RID: 55732 RVA: 0x004E4DCA File Offset: 0x004E2FCA
		public void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _errorCallback)
		{
			this.gameServerFoundCallback = _serverFound;
		}

		// Token: 0x17001B04 RID: 6916
		// (get) Token: 0x0600D9B5 RID: 55733 RVA: 0x004E4DD3 File Offset: 0x004E2FD3
		public bool IsRefreshing
		{
			get
			{
				return this.isRefreshing;
			}
		}

		// Token: 0x0600D9B6 RID: 55734 RVA: 0x004E4DDB File Offset: 0x004E2FDB
		public void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters)
		{
			this.isRefreshing = true;
			if (this.detectCoroutine == null)
			{
				this.detectCoroutine = ThreadManager.StartCoroutine(this.detectFavoriteServers());
			}
		}

		// Token: 0x0600D9B7 RID: 55735 RVA: 0x004E4DFD File Offset: 0x004E2FFD
		public void StopSearch()
		{
			this.isRefreshing = false;
			this.detectCoroutine = null;
		}

		// Token: 0x0600D9B8 RID: 55736 RVA: 0x004E4E0D File Offset: 0x004E300D
		public void Disconnect()
		{
			this.isRefreshing = false;
		}

		// Token: 0x0600D9B9 RID: 55737 RVA: 0x000880CC File Offset: 0x000862CC
		public void GetSingleServerDetails(GameServerInfo _serverInfo, EServerRelationType _relation, GameServerFoundCallback _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600D9BA RID: 55738 RVA: 0x004E4E16 File Offset: 0x004E3016
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator detectFavoriteServers()
		{
			while (this.isRefreshing)
			{
				Dictionary<ServerInfoCache.FavoritesHistoryKey, ServerInfoCache.FavoritesHistoryValue>.Enumerator dictEnumerator = ServerInfoCache.Instance.GetFavoriteServersEnumerator();
				bool flag = dictEnumerator.MoveNext();
				while (flag && this.isRefreshing)
				{
					KeyValuePair<ServerInfoCache.FavoritesHistoryKey, ServerInfoCache.FavoritesHistoryValue> keyValuePair = dictEnumerator.Current;
					GameServerInfo gameServerInfo = new GameServerInfo();
					gameServerInfo.SetValue(GameInfoString.IP, keyValuePair.Key.Address);
					gameServerInfo.SetValue(GameInfoInt.Port, keyValuePair.Key.Port);
					gameServerInfo.IsFavorite = keyValuePair.Value.IsFavorite;
					gameServerInfo.LastPlayedLinux = (int)keyValuePair.Value.LastPlayedTime;
					ServerInformationTcpClient.RequestRules(gameServerInfo, true, new ServerInformationTcpClient.RulesRequestDone(this.callback));
					yield return FavoriteServers.serverCheckInterval;
					try
					{
						flag = dictEnumerator.MoveNext();
					}
					catch (InvalidOperationException)
					{
						flag = false;
					}
				}
				dictEnumerator.Dispose();
				yield return FavoriteServers.refreshInterval;
				dictEnumerator = default(Dictionary<ServerInfoCache.FavoritesHistoryKey, ServerInfoCache.FavoritesHistoryValue>.Enumerator);
			}
			this.detectCoroutine = null;
			yield break;
		}

		// Token: 0x0600D9BB RID: 55739 RVA: 0x004E4E25 File Offset: 0x004E3025
		[PublicizedFrom(EAccessModifier.Private)]
		public void callback(bool _success, string _message, GameServerInfo _gsi)
		{
			if (!this.isRefreshing || !_success)
			{
				return;
			}
			GameServerFoundCallback gameServerFoundCallback = this.gameServerFoundCallback;
			if (gameServerFoundCallback == null)
			{
				return;
			}
			gameServerFoundCallback(this.owner, _gsi, _gsi.IsFavorite ? EServerRelationType.Favorites : EServerRelationType.History);
		}

		// Token: 0x0400A55D RID: 42333
		[PublicizedFrom(EAccessModifier.Private)]
		public bool initDone;

		// Token: 0x0400A55E RID: 42334
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A55F RID: 42335
		[PublicizedFrom(EAccessModifier.Private)]
		public GameServerFoundCallback gameServerFoundCallback;

		// Token: 0x0400A560 RID: 42336
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly WaitForSeconds refreshInterval = new WaitForSeconds(3f);

		// Token: 0x0400A561 RID: 42337
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly WaitForSeconds serverCheckInterval = new WaitForSeconds(0.1f);

		// Token: 0x0400A562 RID: 42338
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isRefreshing;

		// Token: 0x0400A563 RID: 42339
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine detectCoroutine;
	}
}
