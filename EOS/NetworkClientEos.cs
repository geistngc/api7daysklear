using System;
using System.Collections;
using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.P2P;
using UnityEngine.Networking;

namespace Platform.EOS
{
	// Token: 0x02001CF9 RID: 7417
	public class NetworkClientEos : IPlatformNetworkClient, INetworkClient
	{
		// Token: 0x17001B6C RID: 7020
		// (get) Token: 0x0600DBFA RID: 56314 RVA: 0x004ED2A3 File Offset: 0x004EB4A3
		public bool IsConnected
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.serverId != null && (this.protoManager.IsClient || this.connecting) && this.owner.User.UserStatus == EUserStatus.LoggedIn;
			}
		}

		// Token: 0x0600DBFB RID: 56315 RVA: 0x004ED2E0 File Offset: 0x004EB4E0
		public NetworkClientEos(IPlatform _owner, IProtocolManagerProtocolInterface _protoManager)
		{
			this.owner = _owner;
			this.protoManager = _protoManager;
			this.owner.Api.ClientApiInitialized += delegate()
			{
				if (!GameManager.IsDedicatedServer)
				{
					EosHelpers.AssertMainThread("P2P.Init");
					this.localUser = ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId;
					this.socketId = new SocketId
					{
						SocketName = "Game"
					};
					object lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface = ((Api)this.owner.Api).PlatformInterface.GetP2PInterface();
					}
					AddNotifyPeerConnectionRequestOptions addNotifyPeerConnectionRequestOptions = new AddNotifyPeerConnectionRequestOptions
					{
						LocalUserId = this.localUser,
						SocketId = new SocketId?(this.socketId)
					};
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface.AddNotifyPeerConnectionRequest(ref addNotifyPeerConnectionRequestOptions, null, new OnIncomingConnectionRequestCallback(this.ConnectionRequestHandler));
					}
					AddNotifyPeerConnectionEstablishedOptions addNotifyPeerConnectionEstablishedOptions = new AddNotifyPeerConnectionEstablishedOptions
					{
						LocalUserId = this.localUser,
						SocketId = new SocketId?(this.socketId)
					};
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface.AddNotifyPeerConnectionEstablished(ref addNotifyPeerConnectionEstablishedOptions, null, new OnPeerConnectionEstablishedCallback(this.ConnectionEstablishedHandler));
					}
					AddNotifyPeerConnectionClosedOptions addNotifyPeerConnectionClosedOptions = new AddNotifyPeerConnectionClosedOptions
					{
						LocalUserId = this.localUser,
						SocketId = new SocketId?(this.socketId)
					};
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface.AddNotifyPeerConnectionClosed(ref addNotifyPeerConnectionClosedOptions, null, new OnRemoteConnectionClosedCallback(this.ConnectionClosedHandler));
					}
					AddNotifyIncomingPacketQueueFullOptions addNotifyIncomingPacketQueueFullOptions = default(AddNotifyIncomingPacketQueueFullOptions);
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface.AddNotifyIncomingPacketQueueFull(ref addNotifyIncomingPacketQueueFullOptions, null, new OnIncomingPacketQueueFullCallback(this.IncomingPacketQueueFullHandler));
					}
				}
			};
		}

		// Token: 0x0600DBFC RID: 56316 RVA: 0x004ED340 File Offset: 0x004EB540
		public void Connect(GameServerInfo _gsi)
		{
			this.disconnectEventReceived = false;
			Log.Out("[EOS-P2PC] Trying to connect to: " + _gsi.GetValue(GameInfoString.IP) + ":" + _gsi.GetValue(GameInfoInt.Port).ToString());
			if (string.IsNullOrEmpty(_gsi.GetValue(GameInfoString.CombinedPrimaryId)))
			{
				Log.Out("[EOS-P2PC] Resolving EOS ID for IP " + _gsi.GetValue(GameInfoString.IP) + ":" + _gsi.GetValue(GameInfoInt.Port).ToString());
				ServerInformationTcpClient.RequestRules(_gsi, false, new ServerInformationTcpClient.RulesRequestDone(this.RulesRequestTcpDone));
				this.connecting = true;
				return;
			}
			this.ConnectInternal(_gsi);
		}

		// Token: 0x0600DBFD RID: 56317 RVA: 0x004ED3D9 File Offset: 0x004EB5D9
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

		// Token: 0x0600DBFE RID: 56318 RVA: 0x004ED40C File Offset: 0x004EB60C
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConnectInternal(GameServerInfo _gsi)
		{
			string value = _gsi.GetValue(GameInfoString.CombinedPrimaryId);
			if (string.IsNullOrEmpty(value))
			{
				Log.Error("Server info does not have a CombinedPrimaryId");
				this.Disconnect();
				ThreadManager.StartCoroutine(this.connectionFailedLater(Localization.Get("netSteamNetworking_NoServerID", false, null)));
				return;
			}
			if (_gsi.AllowsCrossplay && !PermissionsManager.IsCrossplayAllowed())
			{
				this.Disconnect();
				this.protoManager.ConnectionFailedEv(Localization.Get("auth_noCrossplay", false, null));
				return;
			}
			UserIdentifierEos userIdentifierEos = PlatformUserIdentifierAbs.FromCombinedString(value, true) as UserIdentifierEos;
			if (userIdentifierEos == null)
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
				Encoding.UTF8.GetBytes(password, 0, password.Length, arrayListMP.Items, 1);
			}
			else
			{
				arrayListMP = new ArrayListMP<byte>(MemoryPools.poolByte, 1)
				{
					Count = 1
				};
			}
			EosHelpers.AssertMainThread("P2P.ConInt.PUID");
			this.serverId = userIdentifierEos.ProductUserId;
			string str = "[EOS-P2PC] Connecting to EOS ID ";
			ProductUserId productUserId = this.serverId;
			Log.Out(str + ((productUserId != null) ? productUserId.ToString() : null));
			this.connecting = true;
			this.SendData(50, arrayListMP);
		}

		// Token: 0x0600DBFF RID: 56319 RVA: 0x004ED560 File Offset: 0x004EB760
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator connectionFailedLater(string _message)
		{
			yield return null;
			yield return null;
			this.protoManager.ConnectionFailedEv(_message);
			yield break;
		}

		// Token: 0x0600DC00 RID: 56320 RVA: 0x004ED578 File Offset: 0x004EB778
		public void Disconnect()
		{
			this.connecting = false;
			this.sendBufs.Clear();
			EosHelpers.AssertMainThread("P2P.CloseCons");
			CloseConnectionsOptions closeConnectionsOptions = new CloseConnectionsOptions
			{
				SocketId = new SocketId?(this.socketId),
				LocalUserId = this.localUser
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.ptpInterface.CloseConnections(ref closeConnectionsOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-P2PC] Failed closing connections: " + result.ToStringCached<Result>());
			}
			this.serverId = null;
		}

		// Token: 0x0600DC01 RID: 56321 RVA: 0x004ED628 File Offset: 0x004EB828
		public NetworkError SendData(int _channel, ArrayListMP<byte> _data)
		{
			if (this.IsConnected)
			{
				_data[0] = (byte)_channel;
				this.sendBufs.Enqueue(new NetworkCommonEos.SendInfo(null, _data));
			}
			else
			{
				Log.Warning("[EOS-P2PC] Tried to send a package while not connected to a server");
			}
			return NetworkError.Ok;
		}

		// Token: 0x0600DC02 RID: 56322 RVA: 0x000027FC File Offset: 0x000009FC
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConnectionRequestHandler(ref OnIncomingConnectionRequestInfo _data)
		{
		}

		// Token: 0x0600DC03 RID: 56323 RVA: 0x004ED65A File Offset: 0x004EB85A
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConnectionEstablishedHandler(ref OnPeerConnectionEstablishedInfo _data)
		{
			Log.Out(string.Format("[EOS-P2PC] Connection established: {0}", _data.RemoteUserId));
		}

		// Token: 0x0600DC04 RID: 56324 RVA: 0x004ED674 File Offset: 0x004EB874
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConnectionClosedHandler(ref OnRemoteConnectionClosedInfo _data)
		{
			ProductUserId remoteUserId = _data.RemoteUserId;
			if (this.connecting)
			{
				this.Disconnect();
				Log.Out(string.Format("[EOS-P2PC] P2PSessionConnectFail to: {0} - Error: {1}", _data.RemoteUserId, _data.Reason.ToStringCached<ConnectionClosedReason>()));
				string msg = Localization.Get("netSteamNetworkingSessionError_" + _data.Reason.ToStringCached<ConnectionClosedReason>(), false, null);
				this.protoManager.ConnectionFailedEv(msg);
				return;
			}
			if (!this.IsConnected)
			{
				return;
			}
			Log.Out(string.Format("[EOS-P2PC] Connection closed by {0}: ", remoteUserId) + _data.Reason.ToStringCached<ConnectionClosedReason>());
			if (_data.Reason == ConnectionClosedReason.ClosedByLocalUser)
			{
				return;
			}
			CloseConnectionOptions closeConnectionOptions = new CloseConnectionOptions
			{
				SocketId = new SocketId?(this.socketId),
				LocalUserId = this.localUser,
				RemoteUserId = remoteUserId
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.ptpInterface.CloseConnection(ref closeConnectionOptions);
			}
			if (result != Result.Success)
			{
				Log.Error(string.Format("[EOS-P2PC] Failed closing connection to {0}: {1}", remoteUserId, result.ToStringCached<Result>()));
			}
			this.OnDisconnectedFromServer();
		}

		// Token: 0x0600DC05 RID: 56325 RVA: 0x004ED7A4 File Offset: 0x004EB9A4
		[PublicizedFrom(EAccessModifier.Private)]
		public void IncomingPacketQueueFullHandler(ref OnIncomingPacketQueueFullInfo _data)
		{
			if (!this.IsConnected)
			{
				return;
			}
			Log.Error(string.Format("[EOS-P2PC] Packet queue full: Chn={0}, IncSize={1}, Used={2}, Max={3}", new object[]
			{
				_data.OverflowPacketChannel,
				_data.OverflowPacketSizeBytes,
				_data.PacketQueueCurrentSizeBytes,
				_data.PacketQueueMaxSizeBytes
			}));
		}

		// Token: 0x0600DC06 RID: 56326 RVA: 0x004ED808 File Offset: 0x004EBA08
		public void Update()
		{
			if (!this.IsConnected)
			{
				return;
			}
			Result result;
			for (;;)
			{
				ReceivePacketOptions receivePacketOptions = new ReceivePacketOptions
				{
					LocalUserId = this.localUser,
					MaxDataSizeBytes = 1170U
				};
				ProductUserId productUserId = new ProductUserId();
				SocketId socketId = default(SocketId);
				object lockObject = AntiCheatCommon.LockObject;
				uint num;
				lock (lockObject)
				{
					byte b;
					result = this.ptpInterface.ReceivePacket(ref receivePacketOptions, ref productUserId, ref socketId, out b, this.receiveBuffer, out num);
				}
				if (result != Result.Success)
				{
					break;
				}
				if (num > 0U)
				{
					NetworkCommonEos.ESteamNetChannels esteamNetChannels = (NetworkCommonEos.ESteamNetChannels)this.receiveBuffer.Array[0];
					if (esteamNetChannels > NetworkCommonEos.ESteamNetChannels.NetpackageChannel1)
					{
						if (esteamNetChannels != NetworkCommonEos.ESteamNetChannels.Authentication)
						{
							if (esteamNetChannels == NetworkCommonEos.ESteamNetChannels.Ping)
							{
								SendPacketOptions sendPacketOptions = new SendPacketOptions
								{
									SocketId = new SocketId?(this.socketId),
									LocalUserId = this.localUser,
									RemoteUserId = this.serverId,
									Channel = 0,
									Reliability = PacketReliability.ReliableOrdered,
									AllowDelayedDelivery = true,
									Data = this.receiveBuffer
								};
								lockObject = AntiCheatCommon.LockObject;
								Result result2;
								lock (lockObject)
								{
									result2 = this.ptpInterface.SendPacket(ref sendPacketOptions);
								}
								if (result2 != Result.Success)
								{
									Log.Error("[EOS-P2PC] Could not send ping package to server: " + result2.ToStringCached<Result>());
								}
							}
						}
						else
						{
							if (this.connecting)
							{
								this.connecting = false;
								Log.Out("[EOS-P2PC] Connection established");
							}
							if (this.receiveBuffer.Array[1] == 0)
							{
								Log.Out("[EOS-P2PC] Received invalid password package");
								ThreadManager.AddSingleTaskMainThread("SteamNetInvalidPassword", delegate(object _info)
								{
									this.protoManager.InvalidPasswordEv();
								}, null);
							}
							else
							{
								Log.Out("[EOS-P2PC] Password accepted");
								this.OnConnectedToServer();
							}
						}
					}
					else
					{
						byte[] array = MemoryPools.poolByte.Alloc((int)num);
						Array.Copy(this.receiveBuffer.Array, array, (long)((ulong)num));
						SingletonMonoBehaviour<ConnectionManager>.Instance.Net_DataReceivedClient((int)esteamNetChannels, array, (int)num);
					}
				}
			}
			if (result != Result.NotFound)
			{
				Log.Error("[EOS-P2PS] Error reading packages: " + result.ToStringCached<Result>());
				return;
			}
		}

		// Token: 0x0600DC07 RID: 56327 RVA: 0x004EDA50 File Offset: 0x004EBC50
		public void LateUpdate()
		{
			if (!this.IsConnected)
			{
				return;
			}
			while (this.sendBufs.HasData())
			{
				NetworkCommonEos.SendInfo sendInfo = this.sendBufs.Dequeue();
				ProductUserId remoteUserId = this.serverId;
				SendPacketOptions sendPacketOptions = new SendPacketOptions
				{
					SocketId = new SocketId?(this.socketId),
					LocalUserId = this.localUser,
					RemoteUserId = remoteUserId,
					Channel = 0,
					Reliability = PacketReliability.ReliableOrdered,
					AllowDelayedDelivery = true,
					Data = new ArraySegment<byte>(sendInfo.Data.Items, 0, sendInfo.Data.Count)
				};
				object lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.ptpInterface.SendPacket(ref sendPacketOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-P2PC] Could not send package to server: " + result.ToStringCached<Result>());
				}
			}
		}

		// Token: 0x0600DC08 RID: 56328 RVA: 0x004EDB50 File Offset: 0x004EBD50
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisconnectedFromServer()
		{
			this.Disconnect();
			this.protoManager.DisconnectedFromServerEv(Localization.Get("netSteamNetworking_ConnectionClosedByServer", false, null));
		}

		// Token: 0x0600DC09 RID: 56329 RVA: 0x004EDB70 File Offset: 0x004EBD70
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnConnectedToServer()
		{
			INetConnection[] array = new INetConnection[2];
			for (int i = 0; i < 2; i++)
			{
				array[i] = new NetConnectionSimple(i, null, this, null, 1, 1120);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SetConnectionToServer(array);
		}

		// Token: 0x0600DC0A RID: 56330 RVA: 0x000027FC File Offset: 0x000009FC
		public void SetLatencySimulation(bool _enable, int _minLatency, int _maxLatency)
		{
		}

		// Token: 0x0600DC0B RID: 56331 RVA: 0x000027FC File Offset: 0x000009FC
		public void SetPacketLossSimulation(bool _enable, int _chance)
		{
		}

		// Token: 0x0600DC0C RID: 56332 RVA: 0x000027FC File Offset: 0x000009FC
		public void EnableStatistics()
		{
		}

		// Token: 0x0600DC0D RID: 56333 RVA: 0x000027FC File Offset: 0x000009FC
		public void DisableStatistics()
		{
		}

		// Token: 0x0600DC0E RID: 56334 RVA: 0x00032163 File Offset: 0x00030363
		public string PrintNetworkStatistics()
		{
			return "";
		}

		// Token: 0x0600DC0F RID: 56335 RVA: 0x000027FC File Offset: 0x000009FC
		public void ResetNetworkStatistics()
		{
		}

		// Token: 0x0400A692 RID: 42642
		[PublicizedFrom(EAccessModifier.Private)]
		public const string socketName = "Game";

		// Token: 0x0400A693 RID: 42643
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A694 RID: 42644
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IProtocolManagerProtocolInterface protoManager;

		// Token: 0x0400A695 RID: 42645
		[PublicizedFrom(EAccessModifier.Private)]
		public P2PInterface ptpInterface;

		// Token: 0x0400A696 RID: 42646
		[PublicizedFrom(EAccessModifier.Private)]
		public ProductUserId localUser;

		// Token: 0x0400A697 RID: 42647
		[PublicizedFrom(EAccessModifier.Private)]
		public SocketId socketId;

		// Token: 0x0400A698 RID: 42648
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly BlockingQueue<NetworkCommonEos.SendInfo> sendBufs = new BlockingQueue<NetworkCommonEos.SendInfo>();

		// Token: 0x0400A699 RID: 42649
		[PublicizedFrom(EAccessModifier.Private)]
		public ProductUserId serverId;

		// Token: 0x0400A69A RID: 42650
		[PublicizedFrom(EAccessModifier.Private)]
		public bool connecting;

		// Token: 0x0400A69B RID: 42651
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disconnectEventReceived;

		// Token: 0x0400A69C RID: 42652
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(new byte[1170]);
	}
}
