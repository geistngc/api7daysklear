using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Platform.LAN
{
	// Token: 0x02001CD7 RID: 7383
	public class LANServerList : IServerListInterface
	{
		// Token: 0x17001B5F RID: 7007
		// (get) Token: 0x0600DB20 RID: 56096 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsPrefiltered
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17001B60 RID: 7008
		// (get) Token: 0x0600DB21 RID: 56097 RVA: 0x004E7BCF File Offset: 0x004E5DCF
		public bool IsRefreshing
		{
			get
			{
				return this.shouldRefresh;
			}
		}

		// Token: 0x0600DB22 RID: 56098 RVA: 0x004E7BD7 File Offset: 0x004E5DD7
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x0600DB23 RID: 56099 RVA: 0x004E7BE0 File Offset: 0x004E5DE0
		public void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _sessionSearchErrorCallback)
		{
			this.serverFoundCallback = _serverFound;
		}

		// Token: 0x0600DB24 RID: 56100 RVA: 0x000880CC File Offset: 0x000862CC
		public void GetSingleServerDetails(GameServerInfo _serverInfo, EServerRelationType _relation, GameServerFoundCallback _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600DB25 RID: 56101 RVA: 0x004E7BEC File Offset: 0x004E5DEC
		public void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters)
		{
			try
			{
				this.shouldRefresh = true;
				this.isPaused = false;
				this.udpClient = new UdpClient(AddressFamily.InterNetwork);
				this.sendHandler = new UdpClientSendHandler(this.udpClient);
				this.receiveHandler = new UdpClientReceiveHandler(this.udpClient);
				this.requestCoroutine = ThreadManager.StartCoroutine(this.LANServerInfoRequestCoroutine());
				this.receiveCoroutine = ThreadManager.StartCoroutine(this.LANServerInfoReceiveCoroutine());
			}
			catch (Exception e)
			{
				Log.Error("[LANServerList] Could not start LAN server search");
				Log.Exception(e);
			}
		}

		// Token: 0x0600DB26 RID: 56102 RVA: 0x004E7C7C File Offset: 0x004E5E7C
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator LANServerInfoRequestCoroutine()
		{
			while (this.shouldRefresh)
			{
				IPEndPoint endPoint = new IPEndPoint(LANServerSearchConfig.MulticastGroupIp, 11000);
				if (!this.sendHandler.BeginSend(LANServerList.emptyMessage, 0, endPoint))
				{
					yield break;
				}
				while (!this.sendHandler.isComplete)
				{
					yield return null;
				}
				yield return new WaitForSeconds(5f);
				while (this.isPaused)
				{
					yield return null;
				}
			}
			yield break;
		}

		// Token: 0x0600DB27 RID: 56103 RVA: 0x004E7C8B File Offset: 0x004E5E8B
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator LANServerInfoReceiveCoroutine()
		{
			while (this.shouldRefresh)
			{
				if (!this.receiveHandler.BeginReceive())
				{
					yield break;
				}
				while (!this.receiveHandler.isComplete)
				{
					yield return null;
				}
				while (this.isPaused)
				{
					yield return null;
				}
				IPEndPoint remoteEP = this.receiveHandler.remoteEP;
				byte[] message = this.receiveHandler.message;
				int length = this.receiveHandler.length;
				if (remoteEP != null && message != null && length == 4)
				{
					int num = 0;
					int port = StreamUtils.ReadInt32(message, ref num);
					IPEndPoint ipendPoint = remoteEP;
					ipendPoint.Port = port;
					this.OnServerFound(ipendPoint);
				}
			}
			yield break;
		}

		// Token: 0x0600DB28 RID: 56104 RVA: 0x004E7C9C File Offset: 0x004E5E9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnServerFound(IPEndPoint endpoint)
		{
			string addressString = endpoint.Address.ToString();
			if (!this.cacheControl.IsUpdateRequired(addressString, endpoint.Port))
			{
				return;
			}
			GameServerInfo gameServerInfo = new GameServerInfo();
			gameServerInfo.SetValue(GameInfoString.IP, endpoint.Address.ToString());
			gameServerInfo.SetValue(GameInfoInt.Port, endpoint.Port);
			ServerInformationTcpClient.RequestRules(gameServerInfo, false, new ServerInformationTcpClient.RulesRequestDone(this.OnRulesRequestDone));
		}

		// Token: 0x0600DB29 RID: 56105 RVA: 0x004E7D00 File Offset: 0x004E5F00
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnRulesRequestDone(bool _success, string _message, GameServerInfo _gsi)
		{
			this.cacheControl.SetUpdated(_gsi.GetValue(GameInfoString.IP), _gsi.GetValue(GameInfoInt.Port));
			_gsi.IsLAN = true;
			this.serverFoundCallback(this.owner, _gsi, EServerRelationType.LAN);
		}

		// Token: 0x0600DB2A RID: 56106 RVA: 0x004E7D35 File Offset: 0x004E5F35
		public void StopSearch()
		{
			this.isPaused = true;
			this.cacheControl.Clear();
		}

		// Token: 0x0600DB2B RID: 56107 RVA: 0x004E7D4C File Offset: 0x004E5F4C
		public void Disconnect()
		{
			this.StopSearch();
			this.isPaused = false;
			this.shouldRefresh = false;
			if (this.requestCoroutine != null)
			{
				ThreadManager.StopCoroutine(this.requestCoroutine);
			}
			if (this.receiveCoroutine != null)
			{
				ThreadManager.StopCoroutine(this.receiveCoroutine);
			}
			UdpClient udpClient = this.udpClient;
			if (udpClient != null)
			{
				udpClient.Dispose();
			}
			this.udpClient = null;
		}

		// Token: 0x0400A5EB RID: 42475
		[PublicizedFrom(EAccessModifier.Private)]
		public const int serverBroadcastIntervalSeconds = 5;

		// Token: 0x0400A5EC RID: 42476
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan rulesRefreshInterval = new TimeSpan(0, 2, 0);

		// Token: 0x0400A5ED RID: 42477
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan knownServerTimeout = new TimeSpan(0, 1, 0);

		// Token: 0x0400A5EE RID: 42478
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A5EF RID: 42479
		[PublicizedFrom(EAccessModifier.Private)]
		public GameServerFoundCallback serverFoundCallback;

		// Token: 0x0400A5F0 RID: 42480
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IPAddress multicastGroupIp;

		// Token: 0x0400A5F1 RID: 42481
		[PublicizedFrom(EAccessModifier.Private)]
		public UdpClient udpClient;

		// Token: 0x0400A5F2 RID: 42482
		[PublicizedFrom(EAccessModifier.Private)]
		public bool shouldRefresh;

		// Token: 0x0400A5F3 RID: 42483
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isPaused;

		// Token: 0x0400A5F4 RID: 42484
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine requestCoroutine;

		// Token: 0x0400A5F5 RID: 42485
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine receiveCoroutine;

		// Token: 0x0400A5F6 RID: 42486
		[PublicizedFrom(EAccessModifier.Private)]
		public UdpClientSendHandler sendHandler;

		// Token: 0x0400A5F7 RID: 42487
		[PublicizedFrom(EAccessModifier.Private)]
		public UdpClientReceiveHandler receiveHandler;

		// Token: 0x0400A5F8 RID: 42488
		[PublicizedFrom(EAccessModifier.Private)]
		public LANServerCacheControl cacheControl = new LANServerCacheControl(LANServerList.rulesRefreshInterval, LANServerList.knownServerTimeout);

		// Token: 0x0400A5F9 RID: 42489
		[PublicizedFrom(EAccessModifier.Private)]
		public static byte[] emptyMessage = new byte[0];
	}
}
