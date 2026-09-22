using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platform.Shared
{
	// Token: 0x02001CAD RID: 7341
	public class LocalServerDetect : IServerListInterface
	{
		// Token: 0x0600D9C4 RID: 55748 RVA: 0x004E4FF4 File Offset: 0x004E31F4
		public LocalServerDetect()
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

		// Token: 0x17001B07 RID: 6919
		// (get) Token: 0x0600D9C5 RID: 55749 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsPrefiltered
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600D9C6 RID: 55750 RVA: 0x004E5015 File Offset: 0x004E3215
		public void Init(IPlatform _owner)
		{
			if (GameManager.IsDedicatedServer || this.initDone)
			{
				return;
			}
			this.owner = _owner;
		}

		// Token: 0x0600D9C7 RID: 55751 RVA: 0x004E502E File Offset: 0x004E322E
		public void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _errorCallback)
		{
			this.gameServerFoundCallback = _serverFound;
		}

		// Token: 0x17001B08 RID: 6920
		// (get) Token: 0x0600D9C8 RID: 55752 RVA: 0x004E5037 File Offset: 0x004E3237
		public bool IsRefreshing
		{
			get
			{
				return this.isRefreshing;
			}
		}

		// Token: 0x0600D9C9 RID: 55753 RVA: 0x004E503F File Offset: 0x004E323F
		public void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters)
		{
			this.isRefreshing = true;
			if (this.detectCoroutine == null)
			{
				this.detectCoroutine = ThreadManager.StartCoroutine(this.detectLocalServers());
			}
		}

		// Token: 0x0600D9CA RID: 55754 RVA: 0x004E5061 File Offset: 0x004E3261
		public void StopSearch()
		{
			this.isRefreshing = false;
			this.detectCoroutine = null;
		}

		// Token: 0x0600D9CB RID: 55755 RVA: 0x004E5071 File Offset: 0x004E3271
		public void Disconnect()
		{
			this.isRefreshing = false;
		}

		// Token: 0x0600D9CC RID: 55756 RVA: 0x000880CC File Offset: 0x000862CC
		public void GetSingleServerDetails(GameServerInfo _serverInfo, EServerRelationType _relation, GameServerFoundCallback _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600D9CD RID: 55757 RVA: 0x004E507A File Offset: 0x004E327A
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator detectLocalServers()
		{
			while (this.isRefreshing)
			{
				GameServerInfo gameServerInfo = new GameServerInfo();
				gameServerInfo.SetValue(GameInfoString.IP, "127.0.0.1");
				gameServerInfo.SetValue(GameInfoInt.Port, 26900);
				ServerInformationTcpClient.RequestRules(gameServerInfo, true, new ServerInformationTcpClient.RulesRequestDone(this.callback));
				yield return LocalServerDetect.refreshInterval;
				GameServerInfo gameServerInfo2 = new GameServerInfo();
				gameServerInfo2.SetValue(GameInfoString.IP, "127.0.0.1");
				gameServerInfo2.SetValue(GameInfoInt.Port, 27020);
				ServerInformationTcpClient.RequestRules(gameServerInfo2, true, new ServerInformationTcpClient.RulesRequestDone(this.callback));
				yield return LocalServerDetect.refreshInterval;
			}
			this.detectCoroutine = null;
			yield break;
		}

		// Token: 0x0600D9CE RID: 55758 RVA: 0x004E5089 File Offset: 0x004E3289
		[PublicizedFrom(EAccessModifier.Private)]
		public void callback(bool _success, string _message, GameServerInfo _gsi)
		{
			if (!this.isRefreshing || !_success)
			{
				return;
			}
			_gsi.IsLAN = true;
			GameServerFoundCallback gameServerFoundCallback = this.gameServerFoundCallback;
			if (gameServerFoundCallback == null)
			{
				return;
			}
			gameServerFoundCallback(this.owner, _gsi, EServerRelationType.LAN);
		}

		// Token: 0x0400A568 RID: 42344
		[PublicizedFrom(EAccessModifier.Private)]
		public bool initDone;

		// Token: 0x0400A569 RID: 42345
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A56A RID: 42346
		[PublicizedFrom(EAccessModifier.Private)]
		public GameServerFoundCallback gameServerFoundCallback;

		// Token: 0x0400A56B RID: 42347
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly WaitForSeconds refreshInterval = new WaitForSeconds(3f);

		// Token: 0x0400A56C RID: 42348
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isRefreshing;

		// Token: 0x0400A56D RID: 42349
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine detectCoroutine;
	}
}
