using System;
using System.Collections;
using System.Text;
using System.Threading;
using Steamworks;
using UnityEngine.Networking;

namespace Platform.Steam
{
	// Token: 0x02001C9B RID: 7323
	public class NetworkClientSteam : IPlatformNetworkClient, INetworkClient
	{
		// Token: 0x17001AF2 RID: 6898
		// (get) Token: 0x0600D921 RID: 55585 RVA: 0x004E2554 File Offset: 0x004E0754
		public bool IsConnected
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.serverId != CSteamID.Nil && this.handlerThread != null && (this.protoManager.IsClient || this.connecting) && this.owner.User.UserStatus == EUserStatus.LoggedIn;
			}
		}

		// Token: 0x0600D922 RID: 55586 RVA: 0x004E25A8 File Offset: 0x004E07A8
		public NetworkClientSteam(IPlatform _owner, IProtocolManagerProtocolInterface _protoManager)
		{
			this.owner = _owner;
			this.protoManager = _protoManager;
			this.owner.Api.ClientApiInitialized += delegate()
			{
				if (!GameManager.IsDedicatedServer)
				{
					this.m_P2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(new Callback<P2PSessionConnectFail_t>.DispatchDelegate(this.P2PSessionConnectFail));
				}
			};
		}

		// Token: 0x0600D923 RID: 55587 RVA: 0x004E2618 File Offset: 0x004E0818
		public void Connect(GameServerInfo _gsi)
		{
			this.disconnectEventReceived = false;
			this.anyPacketsSent = false;
			Log.Out("NET: Steam NW trying to connect to: " + _gsi.GetValue(GameInfoString.IP) + ":" + _gsi.GetValue(GameInfoInt.Port).ToString());
			if (string.IsNullOrEmpty(_gsi.GetValue(GameInfoString.SteamID)))
			{
				Log.Out("[Steamworks.NET] NET: Resolving SteamID for IP " + _gsi.GetValue(GameInfoString.IP) + ":" + _gsi.GetValue(GameInfoInt.Port).ToString());
				ServerInformationTcpClient.RequestRules(_gsi, false, new ServerInformationTcpClient.RulesRequestDone(this.RulesRequestTcpDone));
				this.connecting = true;
				return;
			}
			this.ConnectInternal(_gsi);
		}

		// Token: 0x0600D924 RID: 55588 RVA: 0x004E26B7 File Offset: 0x004E08B7
		[PublicizedFrom(EAccessModifier.Private)]
		public void RulesRequestTcpDone(bool _success, string _message, GameServerInfo _gsi)
		{
			if (_success && this.connecting)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo = _gsi;
				this.ConnectInternal(_gsi);
				return;
			}
			this.Disconnect();
			ThreadManager.StartCoroutine(this.connectionFailedLater(_message));
		}

		// Token: 0x0600D925 RID: 55589 RVA: 0x004E26EC File Offset: 0x004E08EC
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConnectInternal(GameServerInfo _gsi)
		{
			if (string.IsNullOrEmpty(_gsi.GetValue(GameInfoString.SteamID)))
			{
				this.Disconnect();
				ThreadManager.StartCoroutine(this.connectionFailedLater(Localization.Get("netSteamNetworking_NoServerID", false, null)));
				return;
			}
			string password = ServerInfoCache.Instance.GetPassword(_gsi);
			ArrayListMP<byte> arrayListMP;
			if (!string.IsNullOrEmpty(password))
			{
				int byteCount = Encoding.UTF8.GetByteCount(password);
				arrayListMP = new ArrayListMP<byte>(MemoryPools.poolByte, byteCount + 1)
				{
					Count = byteCount + 1
				};
				Encoding.UTF8.GetBytes(password, 0, password.Length, arrayListMP.Items, 0);
			}
			else
			{
				arrayListMP = new ArrayListMP<byte>(MemoryPools.poolByte, 1)
				{
					Count = 1
				};
			}
			this.serverId = new CSteamID(ulong.Parse(_gsi.GetValue(GameInfoString.SteamID)));
			string str = "[Steamworks.NET] NET: Connecting to SteamID ";
			CSteamID csteamID = this.serverId;
			Log.Out(str + csteamID.ToString());
			if (this.handlerThread == null)
			{
				this.handlerThread = ThreadManager.StartThread("SteamNetworkingClient", new ThreadManager.ThreadFunctionDelegate(this.threadHandlerMethod), null, null, true, false);
			}
			this.connecting = true;
			this.SendData(50, arrayListMP);
		}

		// Token: 0x0600D926 RID: 55590 RVA: 0x004E2800 File Offset: 0x004E0A00
		public void Disconnect()
		{
			this.connecting = false;
			if (this.serverId != CSteamID.Nil)
			{
				this.sendBufs.Clear();
			}
			if (this.handlerThread == null)
			{
				return;
			}
			this.signalThread.Set();
			this.handlerThread.WaitForEnd(30);
			this.handlerThread = null;
			this.serverId = CSteamID.Nil;
		}

		// Token: 0x0600D927 RID: 55591 RVA: 0x004E2868 File Offset: 0x004E0A68
		public NetworkError SendData(int _channel, ArrayListMP<byte> _data)
		{
			if (this.IsConnected)
			{
				CSteamID recipient = this.serverId;
				_data[_data.Count - 1] = (byte)_channel;
				this.sendBufs.Enqueue(new NetworkCommonSteam.SendInfo(recipient, _data));
				this.signalThread.Set();
			}
			else
			{
				Log.Warning("[Steamworks.NET] NET: Tried to send a package while not connected to a server");
			}
			return NetworkError.Ok;
		}

		// Token: 0x0600D928 RID: 55592 RVA: 0x004E28BF File Offset: 0x004E0ABF
		public void Update()
		{
			if (!this.IsConnected)
			{
				return;
			}
			if (this.disconnectEventReceived)
			{
				this.OnDisconnectedFromServer();
			}
		}

		// Token: 0x0600D929 RID: 55593 RVA: 0x004E28D8 File Offset: 0x004E0AD8
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator connectionFailedLater(string _message)
		{
			yield return null;
			yield return null;
			this.protoManager.ConnectionFailedEv(_message);
			yield break;
		}

		// Token: 0x0600D92A RID: 55594 RVA: 0x004E28EE File Offset: 0x004E0AEE
		public void LateUpdate()
		{
			this.flushBuffers = true;
			this.signalThread.Set();
		}

		// Token: 0x0600D92B RID: 55595 RVA: 0x004E2905 File Offset: 0x004E0B05
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisconnectedFromServer()
		{
			this.Disconnect();
			this.protoManager.DisconnectedFromServerEv(Localization.Get("netSteamNetworking_ConnectionClosedByServer", false, null));
		}

		// Token: 0x0600D92C RID: 55596 RVA: 0x004E2924 File Offset: 0x004E0B24
		[PublicizedFrom(EAccessModifier.Private)]
		public void P2PSessionConnectFail(P2PSessionConnectFail_t _par)
		{
			if (!this.connecting)
			{
				return;
			}
			this.Disconnect();
			Log.Out("[Steamworks.NET] NET: P2PSessionConnectFail to: " + _par.m_steamIDRemote.m_SteamID.ToString() + " - Error: " + ((EP2PSessionError)_par.m_eP2PSessionError).ToStringCached<EP2PSessionError>());
			string msg = Localization.Get("netSteamNetworkingSessionError_" + ((EP2PSessionError)_par.m_eP2PSessionError).ToStringCached<EP2PSessionError>(), false, null);
			this.protoManager.ConnectionFailedEv(msg);
		}

		// Token: 0x0600D92D RID: 55597 RVA: 0x004E299C File Offset: 0x004E0B9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void threadHandlerMethod(ThreadManager.ThreadInfo _threadinfo)
		{
			while (!_threadinfo.TerminationRequested())
			{
				if (!this.IsConnected)
				{
					this.signalThread.WaitOne(100);
				}
				else
				{
					this.signalThread.WaitOne(6);
					if (this.anyPacketsSent)
					{
						this.CheckConnection();
					}
					this.ReceivePackets();
					while (this.sendBufs.HasData())
					{
						NetworkCommonSteam.SendInfo sendInfo = this.sendBufs.Dequeue();
						if (!SteamNetworking.SendP2PPacket(sendInfo.Recipient, sendInfo.Data.Items, (uint)sendInfo.Data.Count, EP2PSend.k_EP2PSendReliableWithBuffering, 0))
						{
							Log.Error("[Steamworks.NET] NET: Could not send package to server");
						}
						else
						{
							this.packetsPendingSend = true;
							this.anyPacketsSent = true;
						}
					}
					if (this.flushBuffers && this.packetsPendingSend)
					{
						this.packetsPendingSend = false;
						this.flushBuffers = false;
						this.FlushBuffer();
					}
				}
			}
			if (this.serverId != CSteamID.Nil)
			{
				SteamNetworking.CloseP2PSessionWithUser(this.serverId);
			}
		}

		// Token: 0x0600D92E RID: 55598 RVA: 0x004E2A98 File Offset: 0x004E0C98
		[PublicizedFrom(EAccessModifier.Private)]
		public void CheckConnection()
		{
			P2PSessionState_t p2PSessionState_t;
			if (SteamNetworking.GetP2PSessionState(this.serverId, out p2PSessionState_t))
			{
				if (p2PSessionState_t.m_bConnectionActive != 0 && this.connecting)
				{
					this.connecting = false;
					Log.Out("[Steamworks.NET] NET: Connection established");
					return;
				}
				if (p2PSessionState_t.m_bConnecting == 0 && p2PSessionState_t.m_bConnectionActive == 0 && this.protoManager.IsClient)
				{
					Log.Out("[Steamworks.NET] NET: Connection closed");
					this.disconnectEventReceived = true;
					return;
				}
			}
			else if (this.protoManager.IsClient)
			{
				Log.Out("[Steamworks.NET] NET: Connection closed");
				this.disconnectEventReceived = true;
			}
		}

		// Token: 0x0600D92F RID: 55599 RVA: 0x004E2B24 File Offset: 0x004E0D24
		[PublicizedFrom(EAccessModifier.Private)]
		public void ReceivePackets()
		{
			uint num;
			CSteamID csteamID;
			bool flag = SteamNetworking.ReadP2PPacket(this.recvBuf, (uint)this.recvBuf.Length, out num, out csteamID, 0);
			while (flag)
			{
				if (num > 0U)
				{
					num -= 1U;
					NetworkCommonSteam.ESteamNetChannels esteamNetChannels = (NetworkCommonSteam.ESteamNetChannels)this.recvBuf[(int)num];
					if (esteamNetChannels > NetworkCommonSteam.ESteamNetChannels.NetpackageChannel1)
					{
						if (esteamNetChannels != NetworkCommonSteam.ESteamNetChannels.Authentication)
						{
							if (esteamNetChannels == NetworkCommonSteam.ESteamNetChannels.Ping)
							{
								ArrayListMP<byte> arrayListMP = new ArrayListMP<byte>(MemoryPools.poolByte, (int)(num + 1U));
								Array.Copy(this.recvBuf, arrayListMP.Items, (long)((ulong)num));
								arrayListMP.Count = (int)(num + 1U);
								this.SendData(60, arrayListMP);
							}
						}
						else
						{
							if (this.connecting)
							{
								this.connecting = false;
								Log.Out("[Steamworks.NET] NET: Connection established");
							}
							if (this.recvBuf[0] == 0)
							{
								Log.Out("[Steamworks.NET] NET: Received invalid password package");
								ThreadManager.AddSingleTaskMainThread("SteamNetInvalidPassword", delegate(object _info)
								{
									this.protoManager.InvalidPasswordEv();
								}, null);
							}
							else
							{
								Log.Out("[Steamworks.NET] NET: Password accepted");
								this.OnConnectedToServer();
							}
						}
					}
					else if (num > 0U)
					{
						byte[] array = MemoryPools.poolByte.Alloc((int)num);
						Array.Copy(this.recvBuf, array, (long)((ulong)num));
						SingletonMonoBehaviour<ConnectionManager>.Instance.Net_DataReceivedClient((int)esteamNetChannels, array, (int)num);
					}
				}
				flag = SteamNetworking.ReadP2PPacket(this.recvBuf, (uint)this.recvBuf.Length, out num, out csteamID, 0);
			}
		}

		// Token: 0x0600D930 RID: 55600 RVA: 0x004E2C58 File Offset: 0x004E0E58
		[PublicizedFrom(EAccessModifier.Private)]
		public void FlushBuffer()
		{
			if (!SteamNetworking.SendP2PPacket(this.serverId, NetworkClientSteam.emptyData, 0U, EP2PSend.k_EP2PSendReliable, 0))
			{
				Log.Error("[Steamworks.NET] NET: Could not flush the data buffer");
			}
		}

		// Token: 0x0600D931 RID: 55601 RVA: 0x004E2C7C File Offset: 0x004E0E7C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnConnectedToServer()
		{
			INetConnection[] array = new INetConnection[2];
			for (int i = 0; i < 2; i++)
			{
				array[i] = new NetConnectionSteam(i, null, this, null);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SetConnectionToServer(array);
		}

		// Token: 0x0600D932 RID: 55602 RVA: 0x000027FC File Offset: 0x000009FC
		public void SetLatencySimulation(bool _enable, int _minLatency, int _maxLatency)
		{
		}

		// Token: 0x0600D933 RID: 55603 RVA: 0x000027FC File Offset: 0x000009FC
		public void SetPacketLossSimulation(bool _enable, int _chance)
		{
		}

		// Token: 0x0600D934 RID: 55604 RVA: 0x000027FC File Offset: 0x000009FC
		public void EnableStatistics()
		{
		}

		// Token: 0x0600D935 RID: 55605 RVA: 0x000027FC File Offset: 0x000009FC
		public void DisableStatistics()
		{
		}

		// Token: 0x0600D936 RID: 55606 RVA: 0x00032163 File Offset: 0x00030363
		public string PrintNetworkStatistics()
		{
			return "";
		}

		// Token: 0x0600D937 RID: 55607 RVA: 0x000027FC File Offset: 0x000009FC
		public void ResetNetworkStatistics()
		{
		}

		// Token: 0x0400A502 RID: 42242
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A503 RID: 42243
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IProtocolManagerProtocolInterface protoManager;

		// Token: 0x0400A504 RID: 42244
		[PublicizedFrom(EAccessModifier.Private)]
		public Callback<P2PSessionConnectFail_t> m_P2PSessionConnectFail;

		// Token: 0x0400A505 RID: 42245
		[PublicizedFrom(EAccessModifier.Private)]
		public ThreadManager.ThreadInfo handlerThread;

		// Token: 0x0400A506 RID: 42246
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly AutoResetEvent signalThread = new AutoResetEvent(false);

		// Token: 0x0400A507 RID: 42247
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly BlockingQueue<NetworkCommonSteam.SendInfo> sendBufs = new BlockingQueue<NetworkCommonSteam.SendInfo>();

		// Token: 0x0400A508 RID: 42248
		[PublicizedFrom(EAccessModifier.Private)]
		public volatile bool flushBuffers;

		// Token: 0x0400A509 RID: 42249
		[PublicizedFrom(EAccessModifier.Private)]
		public bool packetsPendingSend;

		// Token: 0x0400A50A RID: 42250
		[PublicizedFrom(EAccessModifier.Private)]
		public bool anyPacketsSent;

		// Token: 0x0400A50B RID: 42251
		[PublicizedFrom(EAccessModifier.Private)]
		public CSteamID serverId = CSteamID.Nil;

		// Token: 0x0400A50C RID: 42252
		[PublicizedFrom(EAccessModifier.Private)]
		public bool connecting;

		// Token: 0x0400A50D RID: 42253
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disconnectEventReceived;

		// Token: 0x0400A50E RID: 42254
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly byte[] recvBuf = new byte[1048576];

		// Token: 0x0400A50F RID: 42255
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly byte[] emptyData = new byte[0];
	}
}
