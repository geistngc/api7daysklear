using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BhvrAnalyticsServices.Interfaces;
using Platform;
using Services;
using Services.Analytics;
using Services.Analytics.Events;
using UnityEngine;

// Token: 0x0200076E RID: 1902
public class ConnectionManager : SingletonMonoBehaviour<ConnectionManager>
{
	// Token: 0x14000041 RID: 65
	// (add) Token: 0x06003864 RID: 14436 RVA: 0x00172A14 File Offset: 0x00170C14
	// (remove) Token: 0x06003865 RID: 14437 RVA: 0x00172A4C File Offset: 0x00170C4C
	public event Action OnDisconnectFromServer;

	// Token: 0x14000042 RID: 66
	// (add) Token: 0x06003866 RID: 14438 RVA: 0x00172A84 File Offset: 0x00170C84
	// (remove) Token: 0x06003867 RID: 14439 RVA: 0x00172AB8 File Offset: 0x00170CB8
	public static event ConnectionManager.ClientConnectionAction OnClientAdded;

	// Token: 0x14000043 RID: 67
	// (add) Token: 0x06003868 RID: 14440 RVA: 0x00172AEC File Offset: 0x00170CEC
	// (remove) Token: 0x06003869 RID: 14441 RVA: 0x00172B20 File Offset: 0x00170D20
	public static event ConnectionManager.ClientConnectionAction OnClientDisconnected;

	// Token: 0x17000596 RID: 1430
	// (get) Token: 0x0600386A RID: 14442 RVA: 0x00172B53 File Offset: 0x00170D53
	public bool HasRunningServers
	{
		get
		{
			return this.protocolManager.HasRunningServers;
		}
	}

	// Token: 0x17000597 RID: 1431
	// (get) Token: 0x0600386B RID: 14443 RVA: 0x00172B60 File Offset: 0x00170D60
	public ProtocolManager.NetworkType CurrentMode
	{
		get
		{
			return this.protocolManager.CurrentMode;
		}
	}

	// Token: 0x17000598 RID: 1432
	// (get) Token: 0x0600386C RID: 14444 RVA: 0x00172B6D File Offset: 0x00170D6D
	public bool IsServer
	{
		get
		{
			return this.protocolManager.IsServer;
		}
	}

	// Token: 0x17000599 RID: 1433
	// (get) Token: 0x0600386D RID: 14445 RVA: 0x00172B7A File Offset: 0x00170D7A
	public bool IsClient
	{
		get
		{
			return this.protocolManager.IsClient;
		}
	}

	// Token: 0x1700059A RID: 1434
	// (get) Token: 0x0600386E RID: 14446 RVA: 0x00172B87 File Offset: 0x00170D87
	public bool IsSinglePlayer
	{
		get
		{
			return this.IsServer && this.ClientCount() == 0;
		}
	}

	// Token: 0x1700059B RID: 1435
	// (get) Token: 0x0600386F RID: 14447 RVA: 0x00172B9C File Offset: 0x00170D9C
	// (set) Token: 0x06003870 RID: 14448 RVA: 0x00172BA4 File Offset: 0x00170DA4
	public GameServerInfo LastGameServerInfo { get; set; }

	// Token: 0x1700059C RID: 1436
	// (get) Token: 0x06003871 RID: 14449 RVA: 0x00172BAD File Offset: 0x00170DAD
	// (set) Token: 0x06003872 RID: 14450 RVA: 0x00172BB5 File Offset: 0x00170DB5
	public GameServerInfo LocalServerInfo { get; set; }

	// Token: 0x1700059D RID: 1437
	// (get) Token: 0x06003873 RID: 14451 RVA: 0x00172BBE File Offset: 0x00170DBE
	public GameServerInfo CurrentGameServerInfoServerOrClient
	{
		get
		{
			if (this.IsServer)
			{
				return this.LocalServerInfo;
			}
			if (!this.IsClient)
			{
				return null;
			}
			return this.LastGameServerInfo;
		}
	}

	// Token: 0x1700059E RID: 1438
	// (get) Token: 0x06003874 RID: 14452 RVA: 0x00172BDF File Offset: 0x00170DDF
	// (set) Token: 0x06003875 RID: 14453 RVA: 0x00172BE7 File Offset: 0x00170DE7
	public string LastJoinSource { get; set; } = "Unknown";

	// Token: 0x06003876 RID: 14454 RVA: 0x00172BF0 File Offset: 0x00170DF0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void singletonAwake()
	{
		this.windowManager = (GUIWindowManager)UnityEngine.Object.FindObjectOfType(typeof(GUIWindowManager));
		if (GameUtils.GetLaunchArgument("debugnet") != null)
		{
			ConnectionManager.VerboseNetLogging = true;
		}
		this.protocolManager = new ProtocolManager();
		GamePrefs.OnGamePrefChanged += this.OnGamePrefChanged;
		NetPackageLogger.Init();
	}

	// Token: 0x06003877 RID: 14455 RVA: 0x00172C4A File Offset: 0x00170E4A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void singletonDestroy()
	{
		base.singletonDestroy();
		GamePrefs.OnGamePrefChanged -= this.OnGamePrefChanged;
	}

	// Token: 0x06003878 RID: 14456 RVA: 0x00172C63 File Offset: 0x00170E63
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGamePrefChanged(EnumGamePrefs _pref)
	{
		if (_pref == EnumGamePrefs.ServerPassword)
		{
			this.protocolManager.SetServerPassword(GamePrefs.GetString(EnumGamePrefs.ServerPassword));
		}
	}

	// Token: 0x06003879 RID: 14457 RVA: 0x00172C7C File Offset: 0x00170E7C
	public void Disconnect()
	{
		if (this.Clients != null)
		{
			for (int i = 0; i < this.Clients.List.Count; i++)
			{
				ClientInfo cInfo = this.Clients.List[i];
				this.DisconnectClient(cInfo, true, false);
			}
			this.Clients.Clear();
		}
		if (this.connectionToServer[0] != null)
		{
			IAntiCheatClient antiCheatClient = PlatformManager.MultiPlatform.AntiCheatClient;
			if (antiCheatClient != null)
			{
				antiCheatClient.DisconnectFromServer();
			}
			this.connectionToServer[0].Disconnect(false);
			this.LastGameServerInfo = null;
		}
		INetConnection netConnection = this.connectionToServer[1];
		if (netConnection != null)
		{
			netConnection.Disconnect(false);
		}
		this.connectionToServer[0] = null;
		this.connectionToServer[1] = null;
		if (this.IsConnected && !this.IsServer)
		{
			this.protocolManager.Disconnect();
		}
		this.IsConnected = false;
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x00172D4E File Offset: 0x00170F4E
	public void BeginHeartbeat(float seconds)
	{
		this.countdownAnalyticsHeartbeat.SetTimeout(seconds);
		this.countdownAnalyticsHeartbeat.ResetAndRestart();
	}

	// Token: 0x0600387B RID: 14459 RVA: 0x00172D68 File Offset: 0x00170F68
	[PublicizedFrom(EAccessModifier.Private)]
	public void openConnectProgressWindow(GameServerInfo _gameServerInfo)
	{
		string text = GeneratedTextManager.IsFiltered(_gameServerInfo.ServerDisplayName) ? GeneratedTextManager.GetDisplayTextImmediately(_gameServerInfo.ServerDisplayName, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes) : _gameServerInfo.GetValue(GameInfoString.GameHost);
		string text2;
		if (!string.IsNullOrEmpty(text))
		{
			Log.Out("Connecting to server " + text + "...");
			text2 = string.Format(Localization.Get("msgConnectingToServer", false, null), Utils.EscapeBbCodes(text, false, false));
		}
		else
		{
			Log.Out(string.Concat(new string[]
			{
				"Connecting to server ",
				_gameServerInfo.GetValue(GameInfoString.IP),
				":",
				_gameServerInfo.GetValue(GameInfoInt.Port).ToString(),
				"..."
			}));
			text2 = string.Format(Localization.Get("msgConnectingToServer", false, null), _gameServerInfo.GetValue(GameInfoString.IP) + ":" + _gameServerInfo.GetValue(GameInfoInt.Port).ToString());
		}
		text2 = text2 + "\n\n[FFFFFF]" + Utils.GetCancellationMessage();
		XUiC_ProgressWindow.Open(LocalPlayerUI.primaryUI, text2, delegate
		{
			this.Disconnect();
			LocalPlayerUI.primaryUI.windowManager.Open(XUiC_MainMenu.ID, true);
		}, true, true, false);
	}

	// Token: 0x0600387C RID: 14460 RVA: 0x00172E74 File Offset: 0x00171074
	public void Connect(GameServerInfo _gameServerInfo)
	{
		if (PlatformApplicationManager.IsRestartRequired)
		{
			Log.Warning("A restart was pending when attempting to connect to a server.");
			this.Net_ConnectionFailed(Localization.Get("app_restartRequired", false, null));
			return;
		}
		if (!PermissionsManager.IsMultiplayerAllowed())
		{
			this.Net_ConnectionFailed(Localization.Get("xuiConnectFailed_MpNotAllowed", false, null));
			return;
		}
		if (ProfileSDF.CurrentProfileName().Length == 0)
		{
			string[] profiles = ProfileSDF.GetProfiles();
			if (profiles.Length != 0)
			{
				ProfileSDF.SetSelectedProfile(profiles[UnityEngine.Random.Range(0, profiles.Length - 1)]);
			}
		}
		this.IsConnected = true;
		this.LastGameServerInfo = _gameServerInfo;
		this.openConnectProgressWindow(_gameServerInfo);
		NetPackageManager.StartClient();
		this.protocolManager.ConnectToServer(_gameServerInfo);
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x00172F0D File Offset: 0x0017110D
	public void SetConnectionToServer(INetConnection[] _cons)
	{
		this.connectionToServer = _cons;
	}

	// Token: 0x0600387E RID: 14462 RVA: 0x00172F16 File Offset: 0x00171116
	public INetConnection[] GetConnectionToServer()
	{
		return this.connectionToServer;
	}

	// Token: 0x0600387F RID: 14463 RVA: 0x00172F1E File Offset: 0x0017111E
	public void DisconnectFromServer()
	{
		Action onDisconnectFromServer = this.OnDisconnectFromServer;
		if (onDisconnectFromServer != null)
		{
			onDisconnectFromServer();
		}
		this.Disconnect();
		if (GameManager.Instance != null)
		{
			GameManager.Instance.SaveAndCleanupWorld();
		}
		if (GamePrefs.GetInt(EnumGamePrefs.AutopilotMode) > 0)
		{
			Application.Quit();
		}
	}

	// Token: 0x06003880 RID: 14464 RVA: 0x00172F60 File Offset: 0x00171160
	public void SendToServer(NetPackage _package, bool _flush = false)
	{
		int channel = _package.Channel;
		if (this.connectionToServer[channel] == null)
		{
			if (this.IsConnected)
			{
				Log.Error("Can not queue package for server: NetConnection null");
			}
			return;
		}
		this.connectionToServer[channel].AddToSendQueue(_package);
		if (_flush)
		{
			this.connectionToServer[channel].FlushSendQueue();
		}
	}

	// Token: 0x06003881 RID: 14465 RVA: 0x00172FB0 File Offset: 0x001711B0
	public NetworkConnectionError StartServers(string _password, bool _offline)
	{
		if (PlatformApplicationManager.IsRestartRequired)
		{
			Log.Warning("A restart was pending when attempting to start servers.");
			return NetworkConnectionError.RestartRequired;
		}
		NetworkConnectionError networkConnectionError = NetworkConnectionError.NoError;
		if (!GameManager.IsDedicatedServer && !_offline)
		{
			if (PlatformManager.MultiPlatform.User.UserStatus == EUserStatus.OfflineMode)
			{
				Log.Out("Can not start servers in online mode because user is in offline mode. Starting server in offline mode.");
				_offline = true;
			}
			else if (!PermissionsManager.IsMultiplayerAllowed() || !PermissionsManager.CanHostMultiplayer())
			{
				Log.Out("Can not start servers in online mode because user does not have multiplayer hosting permissions. Starting in offline mode.");
				_offline = true;
			}
		}
		if (_offline)
		{
			this.protocolManager.StartOfflineServer();
		}
		else
		{
			networkConnectionError = this.protocolManager.StartServers(_password);
		}
		if (networkConnectionError == NetworkConnectionError.NoError)
		{
			GameManager.Instance.StartGame(_offline);
		}
		NetPackageManager.StartServer();
		return networkConnectionError;
	}

	// Token: 0x06003882 RID: 14466 RVA: 0x0017304A File Offset: 0x0017124A
	public void MakeServerOffline()
	{
		this.protocolManager.MakeServerOffline();
	}

	// Token: 0x06003883 RID: 14467 RVA: 0x00173058 File Offset: 0x00171258
	public void StopServers()
	{
		Log.Out("[NET] ServerShutdown");
		this.protocolManager.StopServers();
		this.Disconnect();
		if (GameManager.Instance != null)
		{
			GameManager.Instance.SaveAndCleanupWorld();
		}
		if (this.LocalServerInfo != null)
		{
			this.LocalServerInfo.ClearOnChanged();
			this.LocalServerInfo = null;
		}
		NetPackageManager.ResetMappings();
		if (GamePrefs.GetInt(EnumGamePrefs.AutopilotMode) > 0)
		{
			Application.Quit();
		}
	}

	// Token: 0x06003884 RID: 14468 RVA: 0x001730C5 File Offset: 0x001712C5
	public void ServerReady()
	{
		if (!this.IsConnected)
		{
			this.Clients.Clear();
		}
		this.IsConnected = true;
	}

	// Token: 0x06003885 RID: 14469 RVA: 0x001730E1 File Offset: 0x001712E1
	public int ClientCount()
	{
		return this.Clients.Count;
	}

	// Token: 0x06003886 RID: 14470 RVA: 0x001730F0 File Offset: 0x001712F0
	public void AddClient(ClientInfo _cInfo)
	{
		ConnectionManager.ClientConnectionAction onClientAdded = ConnectionManager.OnClientAdded;
		if (onClientAdded != null)
		{
			onClientAdded(_cInfo);
		}
		this.Clients.Add(_cInfo);
		GameSparksCollector.SetMax(GameSparksCollector.GSDataKey.PeakConcurrentClients, null, this.ClientCount(), false, GameSparksCollector.GSDataCollection.SessionTotal);
		GameSparksCollector.SetMax(GameSparksCollector.GSDataKey.PeakConcurrentPlayers, null, this.ClientCount() + (GameManager.IsDedicatedServer ? 0 : 1), false, GameSparksCollector.GSDataCollection.SessionTotal);
	}

	// Token: 0x06003887 RID: 14471 RVA: 0x00173148 File Offset: 0x00171348
	public void DisconnectClient(ClientInfo _cInfo, bool _bShutdown = false, bool _clientDisconnect = false)
	{
		if (!ThreadManager.IsMainThread())
		{
			ThreadManager.AddSingleTaskMainThread("CM.DisconnectClient-" + _cInfo.ClientNumber.ToString(), delegate(object _parameter)
			{
				ValueTuple<ClientInfo, bool, bool> valueTuple = (ValueTuple<ClientInfo, bool, bool>)_parameter;
				ClientInfo item = valueTuple.Item1;
				bool item2 = valueTuple.Item2;
				bool item3 = valueTuple.Item3;
				this.DisconnectClient(item, item2, item3);
			}, new ValueTuple<ClientInfo, bool, bool>(_cInfo, _bShutdown, _clientDisconnect));
			return;
		}
		if (_cInfo == null)
		{
			Log.Error("DisconnectClient: ClientInfo is null");
			return;
		}
		if (!this.Clients.Contains(_cInfo))
		{
			Log.Warning("DisconnectClient: Player " + _cInfo.InternalId.CombinedString + " not found");
			Log.Out("From: " + StackTraceUtility.ExtractStackTrace());
			return;
		}
		ConnectionManager.ClientConnectionAction onClientDisconnected = ConnectionManager.OnClientDisconnected;
		if (onClientDisconnected != null)
		{
			onClientDisconnected(_cInfo);
		}
		ModEvents.SPlayerDisconnectedData splayerDisconnectedData = new ModEvents.SPlayerDisconnectedData(_cInfo, _bShutdown);
		ModEvents.PlayerDisconnected.Invoke(ref splayerDisconnectedData);
		Log.Out(string.Format("Player disconnected: {0}", _cInfo));
		if (_cInfo.latestPlayerData != null)
		{
			PlayerDataFile latestPlayerData = _cInfo.latestPlayerData;
			if (latestPlayerData.bModifiedSinceLastSave)
			{
				latestPlayerData.Save(GameIO.GetPlayerDataDir(), _cInfo.InternalId.CombinedString);
			}
		}
		INetConnection netConnection = _cInfo.netConnection[0];
		if (netConnection != null)
		{
			netConnection.Disconnect(false);
		}
		INetConnection netConnection2 = _cInfo.netConnection[1];
		if (netConnection2 != null)
		{
			netConnection2.Disconnect(false);
		}
		AuthorizationManager.Instance.Disconnect(_cInfo);
		if (!_bShutdown)
		{
			World world = GameManager.Instance.World;
			EntityPlayer entityPlayer = ((EntityAlive)((world != null) ? world.GetEntity(_cInfo.entityId) : null)) as EntityPlayer;
			if (entityPlayer != null)
			{
				entityPlayer.bWillRespawn = false;
				entityPlayer.PartyDisconnect();
				QuestEventManager.Current.HandlePlayerDisconnect(entityPlayer);
				LockManager.Instance.ForceUnlockByPlayer(_cInfo.entityId);
				GameManager.Instance.GameMessage(EnumGameMessages.LeftGame, entityPlayer, null);
				if (GameManager.Instance.World.m_ChunkManager != null)
				{
					GameManager.Instance.World.m_ChunkManager.RemoveChunkObserver(entityPlayer.ChunkObserver);
				}
				GameManager.Instance.World.RemoveEntity(_cInfo.entityId, EnumRemoveEntityReason.Unloaded);
				GameEventManager.Current.HandleForceBossDespawn(entityPlayer);
			}
		}
		else
		{
			World world2 = GameManager.Instance.World;
			EntityAlive entityAlive = (EntityAlive)((world2 != null) ? world2.GetEntity(_cInfo.entityId) : null);
			if (entityAlive != null)
			{
				QuestEventManager.Current.HandlePlayerDisconnect(entityAlive as EntityPlayer);
			}
		}
		if (!_bShutdown)
		{
			this.Clients.Remove(_cInfo);
			_cInfo.network.DropClient(_cInfo, _clientDisconnect);
		}
	}

	// Token: 0x06003888 RID: 14472 RVA: 0x0017338A File Offset: 0x0017158A
	public void SetClientEntityId(ClientInfo _cInfo, int _entityId, PlayerDataFile _pdf)
	{
		_cInfo.entityId = _entityId;
		_cInfo.bAttachedToEntity = true;
		_cInfo.latestPlayerData = _pdf;
	}

	// Token: 0x06003889 RID: 14473 RVA: 0x001733A4 File Offset: 0x001715A4
	public void SendPackage(List<NetPackage> _packages, bool _onlyClientsAttachedToAnEntity = false, int _attachedToEntityId = -1, int _allButAttachedToEntityId = -1, int _entitiesInRangeOfEntity = -1, Vector3? _entitiesInRangeOfWorldPos = null, int _range = 192, bool _onlyClientsNotAttachedToAnEntity = false)
	{
		if (this.Clients == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < _packages.Count; i++)
		{
			_packages[i].RegisterSendQueue();
		}
		for (int j = 0; j < this.Clients.List.Count; j++)
		{
			ClientInfo clientInfo = this.Clients.List[j];
			if (clientInfo.loginDone && (!_onlyClientsAttachedToAnEntity || clientInfo.bAttachedToEntity) && (!_onlyClientsNotAttachedToAnEntity || !clientInfo.bAttachedToEntity) && (_attachedToEntityId == -1 || (clientInfo.bAttachedToEntity && clientInfo.entityId == _attachedToEntityId)) && (_allButAttachedToEntityId == -1 || (clientInfo.bAttachedToEntity && clientInfo.entityId != _allButAttachedToEntityId)) && (_entitiesInRangeOfEntity == -1 || GameManager.Instance.World.IsEntityInRange(_entitiesInRangeOfEntity, clientInfo.entityId, _range)) && (_entitiesInRangeOfWorldPos == null || GameManager.Instance.World.IsEntityInRange(clientInfo.entityId, _entitiesInRangeOfWorldPos.Value, _range)))
			{
				for (int k = 0; k < _packages.Count; k++)
				{
					NetPackage netPackage = _packages[k];
					clientInfo.netConnection[netPackage.Channel].AddToSendQueue(netPackage);
					if (netPackage.Channel == 1)
					{
						flag2 = true;
					}
					else
					{
						flag = true;
					}
					flag3 |= netPackage.FlushQueue;
				}
				if (flag3)
				{
					if (flag)
					{
						clientInfo.netConnection[0].FlushSendQueue();
					}
					if (flag2)
					{
						clientInfo.netConnection[1].FlushSendQueue();
					}
				}
			}
		}
		for (int l = 0; l < _packages.Count; l++)
		{
			_packages[l].SendQueueHandled();
		}
	}

	// Token: 0x0600388A RID: 14474 RVA: 0x00173564 File Offset: 0x00171764
	public void SendPackage(NetPackage _package, bool _onlyClientsAttachedToAnEntity = false, int _attachedToEntityId = -1, int _allButAttachedToEntityId = -1, int _entitiesInRangeOfEntity = -1, Vector3? _entitiesInRangeOfWorldPos = null, int _range = 192, bool _onlyClientsNotAttachedToAnEntity = false)
	{
		if (this.Clients == null)
		{
			return;
		}
		_package.RegisterSendQueue();
		for (int i = 0; i < this.Clients.List.Count; i++)
		{
			ClientInfo clientInfo = this.Clients.List[i];
			if (clientInfo.loginDone && (!_onlyClientsAttachedToAnEntity || clientInfo.bAttachedToEntity) && (!_onlyClientsNotAttachedToAnEntity || !clientInfo.bAttachedToEntity) && (_attachedToEntityId == -1 || (clientInfo.bAttachedToEntity && clientInfo.entityId == _attachedToEntityId)) && (_allButAttachedToEntityId == -1 || (clientInfo.bAttachedToEntity && clientInfo.entityId != _allButAttachedToEntityId)) && (_entitiesInRangeOfEntity == -1 || GameManager.Instance.World.IsEntityInRange(_entitiesInRangeOfEntity, clientInfo.entityId, _range)) && (_entitiesInRangeOfWorldPos == null || GameManager.Instance.World.IsEntityInRange(clientInfo.entityId, _entitiesInRangeOfWorldPos.Value, _range)))
			{
				clientInfo.netConnection[_package.Channel].AddToSendQueue(_package);
				if (_package.FlushQueue)
				{
					clientInfo.netConnection[_package.Channel].FlushSendQueue();
				}
			}
		}
		_package.SendQueueHandled();
	}

	// Token: 0x0600388B RID: 14475 RVA: 0x0017368C File Offset: 0x0017188C
	public void FlushClientSendQueues()
	{
		for (int i = 0; i < this.Clients.List.Count; i++)
		{
			ClientInfo clientInfo = this.Clients.List[i];
			clientInfo.netConnection[0].FlushSendQueue();
			clientInfo.netConnection[1].FlushSendQueue();
		}
	}

	// Token: 0x0600388C RID: 14476 RVA: 0x001736E0 File Offset: 0x001718E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdatePings()
	{
		for (int i = 0; i < this.Clients.List.Count; i++)
		{
			this.Clients.List[i].UpdatePing();
		}
	}

	// Token: 0x0600388D RID: 14477 RVA: 0x0017371E File Offset: 0x0017191E
	public string GetRequiredPortsString()
	{
		return this.protocolManager.GetGamePortsString();
	}

	// Token: 0x0600388E RID: 14478 RVA: 0x0017372C File Offset: 0x0017192C
	public void SendToClientsOrServer(NetPackage _package)
	{
		if (!this.IsServer)
		{
			this.SendToServer(_package, false);
			return;
		}
		this.SendPackage(_package, false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x0600388F RID: 14479 RVA: 0x00173764 File Offset: 0x00171964
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		this.protocolManager.Update();
		if (this.IsServer)
		{
			bool flag = Time.time - this.lastBadPacketCheck > 1f;
			if (flag)
			{
				this.lastBadPacketCheck = Time.time;
			}
			for (int i = 0; i < this.Clients.Count; i++)
			{
				ClientInfo clientInfo = this.Clients.List[i];
				if (!clientInfo.netConnection[0].IsDisconnected())
				{
					if (flag && clientInfo.entityId != -1 && !clientInfo.disconnecting && clientInfo.network.GetBadPacketCount(clientInfo) >= 3)
					{
						GameUtils.KickPlayerForClientInfo(clientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.BadMTUPackets, 0, default(DateTime), ""));
					}
					else
					{
						this.ProcessPackages(clientInfo.netConnection[0], NetPackageDirection.ToClient, clientInfo);
						if (i < this.Clients.Count)
						{
							this.ProcessPackages(clientInfo.netConnection[1], NetPackageDirection.ToClient, clientInfo);
						}
					}
				}
			}
			this.FlushClientSendQueues();
			if (this.updateClientInfo.HasPassed() && GameManager.Instance.World != null && this.ClientCount() > 0)
			{
				this.UpdatePings();
				this.updateClientInfo.ResetAndRestart();
				this.SendPackage(NetPackageManager.GetPackage<NetPackageClientInfo>().Setup(GameManager.Instance.World, this.Clients.List), true, -1, -1, -1, null, 192, false);
			}
		}
		else
		{
			if (this.connectionToServer[0] != null && !this.connectionToServer[0].IsDisconnected())
			{
				this.ProcessPackages(this.connectionToServer[0], NetPackageDirection.ToServer, null);
				INetConnection netConnection = this.connectionToServer[0];
				if (netConnection != null)
				{
					netConnection.FlushSendQueue();
				}
			}
			if (this.connectionToServer[1] != null && !this.connectionToServer[1].IsDisconnected())
			{
				this.ProcessPackages(this.connectionToServer[1], NetPackageDirection.ToServer, null);
				INetConnection netConnection2 = this.connectionToServer[1];
				if (netConnection2 != null)
				{
					netConnection2.FlushSendQueue();
				}
			}
		}
		if (!GameManager.IsDedicatedServer && this.countdownAnalyticsHeartbeat.HasPassed())
		{
			this.countdownAnalyticsHeartbeat.ResetAndRestart();
			HeartbeatEventData heartbeatEventData = new HeartbeatEventData();
			heartbeatEventData.HeartbeatTimestamp = DateTime.UtcNow.ToString("O");
			heartbeatEventData.ServerId = Helper.GetServerId();
			string saveId;
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				GameManager instance = GameManager.Instance;
				if (instance == null)
				{
					saveId = null;
				}
				else
				{
					World world = instance.World;
					saveId = ((world != null) ? world.Guid : null);
				}
			}
			else
			{
				saveId = GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
			}
			heartbeatEventData.SaveId = saveId;
			heartbeatEventData.PlayerCount = Helper.GetServerPlayerCount();
			HeartbeatEventData analyticsEventData = heartbeatEventData;
			ServiceProvider.Instance.Get<IAnalyticsService>().LogEvent(analyticsEventData);
		}
	}

	// Token: 0x06003890 RID: 14480 RVA: 0x001739F0 File Offset: 0x00171BF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		this.protocolManager.LateUpdate();
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x00173A00 File Offset: 0x00171C00
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessPackages(INetConnection _connection, NetPackageDirection _disallowedDirection, ClientInfo _clientInfo = null)
	{
		if (_connection == null)
		{
			Log.Error("ProcessPackages: connection == null");
			return;
		}
		_connection.GetPackages(this.packagesToProcess);
		if (this.packagesToProcess == null)
		{
			Log.Error("ProcessPackages: packages == null");
			return;
		}
		for (int i = 0; i < this.packagesToProcess.Count; i++)
		{
			NetPackage netPackage = this.packagesToProcess[i];
			if (netPackage == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"ProcessPackages: packages [",
					i.ToString(),
					"] == null (packages.Count == ",
					this.packagesToProcess.Count.ToString(),
					")"
				}));
			}
			else if (netPackage.PackageDirection == _disallowedDirection)
			{
				if (_clientInfo == null)
				{
					Log.Warning(string.Format("[NET] Received package {0} which is only allowed to be sent to the server", netPackage));
				}
				else
				{
					Log.Warning(string.Format("[NET] Received package {0} which is only allowed to be sent to clients from client {1}", netPackage, _clientInfo));
				}
			}
			else if (_clientInfo != null && !netPackage.AllowedBeforeAuth && !_clientInfo.loginDone)
			{
				Log.Warning(string.Format("[NET] Received an unexpected package ({0}) before authentication was finished from client {1}", netPackage, _clientInfo));
			}
			else if (netPackage.ShouldProcess(GameManager.Instance.World, GameManager.Instance))
			{
				netPackage.ProcessPackage(GameManager.Instance.World, GameManager.Instance);
				NetPackageManager.FreePackage(netPackage);
			}
			else
			{
				netPackage.HandleSkipped(GameManager.Instance.World, GameManager.Instance);
			}
		}
	}

	// Token: 0x06003892 RID: 14482 RVA: 0x00173B58 File Offset: 0x00171D58
	public void PlayerAllowed(string _gameInfo, PlatformLobbyId _platformLobbyId, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _platformUserAndToken, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _crossplatformUserAndToken)
	{
		ConnectionManager.<>c__DisplayClass77_0 CS$<>8__locals1 = new ConnectionManager.<>c__DisplayClass77_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1._platformUserAndToken = _platformUserAndToken;
		CS$<>8__locals1._crossplatformUserAndToken = _crossplatformUserAndToken;
		CS$<>8__locals1._platformLobbyId = _platformLobbyId;
		if (!this.IsClient)
		{
			return;
		}
		Log.Out("Player allowed");
		if (this.LastGameServerInfo.GetValue(GameInfoBool.IsDedicated))
		{
			ServerInfoCache.Instance.AddHistory(this.LastGameServerInfo);
		}
		this.LastGameServerInfo.GetValue(GameInfoString.IP);
		this.LastGameServerInfo.GetValue(GameInfoInt.Port);
		this.LastGameServerInfo = new GameServerInfo(_gameInfo);
		if ((!LaunchPrefs.AllowJoinConfigModded.Value && this.LastGameServerInfo.GetValue(GameInfoBool.ModdedConfig)) || ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() && this.LastGameServerInfo.GetValue(GameInfoBool.RequiresMod)))
		{
			CS$<>8__locals1.<PlayerAllowed>g__AuthorizerDisconnect|2(Localization.Get("auth_moddedconfigdetected", false, null));
			return;
		}
		PlatformUserIdentifierAbs item = CS$<>8__locals1._platformUserAndToken.Item1;
		if (item != null)
		{
			item.DecodeTicket(CS$<>8__locals1._platformUserAndToken.Item2);
		}
		PlatformUserIdentifierAbs item2 = CS$<>8__locals1._crossplatformUserAndToken.Item1;
		if (item2 != null)
		{
			item2.DecodeTicket(CS$<>8__locals1._crossplatformUserAndToken.Item2);
		}
		CS$<>8__locals1.authorizers = ConnectionManager.<PlayerAllowed>g__GetAuthenticationClients|77_3();
		CS$<>8__locals1.authorizerIndex = -1;
		CS$<>8__locals1.<PlayerAllowed>g__NextAuthorizer|0();
	}

	// Token: 0x06003893 RID: 14483 RVA: 0x00173C84 File Offset: 0x00171E84
	public void PlayerDenied(string _reason)
	{
		if (this.IsClient)
		{
			this.protocolManager.Disconnect();
			Log.Out("Player denied: " + _reason);
			XUiC_MessageBoxWindowGroup.ShowOk(this.windowManager.playerUI.xui, Localization.Get("mmLblErrorConnectionDeniedTitle", false, null), _reason, "", null, true, true, false);
		}
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x00173CE0 File Offset: 0x00171EE0
	public void ServerConsoleCommand(ClientInfo _cInfo, string _cmd)
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		if (_cmd.Length > 300)
		{
			Log.Warning("Client tried to execute command with {0} characters. First 20: '{1}'", new object[]
			{
				_cmd.Length,
				_cmd.Substring(0, 20)
			});
			return;
		}
		IConsoleCommand command = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand(_cmd, false);
		if (command == null)
		{
			_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup("Unknown command", false));
			return;
		}
		if (!command.CanExecuteForDevice)
		{
			_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup("Command not permitted on the server's device", false));
			return;
		}
		string[] commands = command.GetCommands();
		AdminTools adminTools = GameManager.Instance.adminTools;
		if (adminTools == null || !adminTools.CommandAllowedFor(commands, _cInfo))
		{
			Log.Out(string.Format("Denying command '{0}' from client {1}", _cmd, _cInfo));
			_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup(string.Format(Localization.Get("msgServer25", false, null), _cmd, _cInfo.playerName), false));
			return;
		}
		if (command.IsExecuteOnClient)
		{
			Log.Out("Client {0}/{1} executing client side command: {2}", new object[]
			{
				_cInfo.InternalId.CombinedString,
				_cInfo.playerName,
				_cmd
			});
			_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup(_cmd, true));
			return;
		}
		List<string> lines = SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(_cmd, _cInfo);
		_cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup(lines, false));
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x00173E3C File Offset: 0x0017203C
	public void SendLogin()
	{
		PlatformUserIdentifierAbs platformUserId = PlatformManager.NativePlatform.User.PlatformUserId;
		IAuthenticationClient authenticationClient = PlatformManager.NativePlatform.AuthenticationClient;
		ValueTuple<PlatformUserIdentifierAbs, string> platformUserAndToken = new ValueTuple<PlatformUserIdentifierAbs, string>(platformUserId, (authenticationClient != null) ? authenticationClient.GetAuthTicket() : null);
		IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
		PlatformUserIdentifierAbs item = (crossplatformPlatform != null) ? crossplatformPlatform.User.PlatformUserId : null;
		IPlatform crossplatformPlatform2 = PlatformManager.CrossplatformPlatform;
		ValueTuple<PlatformUserIdentifierAbs, string> crossplatformUserAndToken = new ValueTuple<PlatformUserIdentifierAbs, string>(item, ((crossplatformPlatform2 != null) ? crossplatformPlatform2.AuthenticationClient.GetAuthTicket() : null) ?? "");
		ulong discordUserId = DiscordManager.Instance.IsReady ? DiscordManager.Instance.LocalUser.ID : 0UL;
		this.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerLogin>().Setup(GamePrefs.GetString(EnumGamePrefs.PlayerName), platformUserAndToken, crossplatformUserAndToken, Constants.cVersionInformation.LongStringNoBuild, Constants.cVersionInformation.LongStringNoBuild, discordUserId), false);
	}

	// Token: 0x06003896 RID: 14486 RVA: 0x00173F00 File Offset: 0x00172100
	public void Net_ConnectionFailed(string _message)
	{
		Log.Error("[NET] Connection to server failed: " + _message);
		XUiC_MessageBoxWindowGroup.ShowOk(this.windowManager.playerUI.xui, Localization.Get("mmLblErrorConnectionFailed", false, null), _message, "", null, true, true, false);
		this.IsConnected = false;
		IAntiCheatClient antiCheatClient = PlatformManager.MultiPlatform.AntiCheatClient;
		if (antiCheatClient == null)
		{
			return;
		}
		antiCheatClient.DisconnectFromServer();
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x00173F63 File Offset: 0x00172163
	public void Net_InvalidPassword()
	{
		XUiC_ServerPasswordWindow.OpenPasswordWindow(LocalPlayerUI.primaryUI.xui, true, ServerInfoCache.Instance.GetPassword(this.LastGameServerInfo), true, delegate(string _pwd)
		{
			ServerInfoCache.Instance.SavePassword(this.LastGameServerInfo, _pwd);
			this.Connect(this.LastGameServerInfo);
		}, delegate
		{
			this.windowManager.Open(XUiC_ServerBrowser.ID, true);
			this.Disconnect();
		});
	}

	// Token: 0x06003898 RID: 14488 RVA: 0x00173FA0 File Offset: 0x001721A0
	public void Net_DisconnectedFromServer(string _reason)
	{
		Log.Out("[NET] DisconnectedFromServer: " + _reason);
		this.DisconnectFromServer();
		XUiC_MessageBoxWindowGroup.ShowOk(this.windowManager.playerUI.xui, Localization.Get("mmLblErrorConnectionLost", false, null), _reason, "", null, true, true, false);
	}

	// Token: 0x06003899 RID: 14489 RVA: 0x00173FEE File Offset: 0x001721EE
	public void Net_DataReceivedClient(int _channel, byte[] _data, int _size)
	{
		if (this.connectionToServer[_channel] != null)
		{
			this.connectionToServer[_channel].AppendToReaderStream(_data, _size);
		}
	}

	// Token: 0x0600389A RID: 14490 RVA: 0x00174009 File Offset: 0x00172209
	public void Net_DataReceivedServer(ClientInfo _cInfo, int _channel, byte[] _data, int _size)
	{
		if (_cInfo != null)
		{
			INetConnection netConnection = _cInfo.netConnection[_channel];
			if (netConnection == null)
			{
				return;
			}
			netConnection.AppendToReaderStream(_data, _size);
		}
	}

	// Token: 0x0600389B RID: 14491 RVA: 0x00174023 File Offset: 0x00172223
	public void Net_PlayerConnected(ClientInfo _cInfo)
	{
		Log.Out(string.Format("[NET] PlayerConnected {0}", _cInfo));
		_cInfo.netConnection[0].AddToSendQueue(NetPackageManager.GetPackage<NetPackagePackageIds>().Setup());
	}

	// Token: 0x0600389C RID: 14492 RVA: 0x0017404C File Offset: 0x0017224C
	public void Net_PlayerDisconnected(ClientInfo _cInfo)
	{
		if (_cInfo != null)
		{
			Log.Out(string.Format("[NET] PlayerDisconnected {0}", _cInfo));
			this.DisconnectClient(_cInfo, false, false);
		}
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x0017406A File Offset: 0x0017226A
	public void SetLatencySimulation(bool _enable, int _min, int _max)
	{
		this.protocolManager.SetLatencySimulation(_enable, _min, _max);
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x0017407A File Offset: 0x0017227A
	public void SetPacketLossSimulation(bool _enable, int _chance)
	{
		this.protocolManager.SetPacketLossSimulation(_enable, _chance);
	}

	// Token: 0x0600389F RID: 14495 RVA: 0x00174089 File Offset: 0x00172289
	public void EnableNetworkStatistics()
	{
		this.protocolManager.EnableNetworkStatistics();
	}

	// Token: 0x060038A0 RID: 14496 RVA: 0x00174096 File Offset: 0x00172296
	public void DisableNetworkStatistics()
	{
		this.protocolManager.DisableNetworkStatistics();
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x001740A3 File Offset: 0x001722A3
	public string PrintNetworkStatistics()
	{
		return this.protocolManager.PrintNetworkStatistics();
	}

	// Token: 0x060038A2 RID: 14498 RVA: 0x001740B0 File Offset: 0x001722B0
	public void ResetNetworkStatistics()
	{
		this.protocolManager.ResetNetworkStatistics();
		this.protocolManager.DisableNetworkStatistics();
	}

	// Token: 0x060038A6 RID: 14502 RVA: 0x00174190 File Offset: 0x00172390
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static IAuthenticationClient[] <PlayerAllowed>g__GetAuthenticationClients|77_3()
	{
		IAuthenticationClient[] array = new IAuthenticationClient[2];
		array[0] = PlatformManager.NativePlatform.AuthenticationClient;
		int num = 1;
		IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
		array[num] = ((crossplatformPlatform != null) ? crossplatformPlatform.AuthenticationClient : null);
		return (from authorizer in array
		where authorizer != null
		select authorizer).ToArray<IAuthenticationClient>();
	}

	// Token: 0x04002E69 RID: 11881
	public const int CHANNELCOUNT = 2;

	// Token: 0x04002E6A RID: 11882
	public static bool VerboseNetLogging;

	// Token: 0x04002E6B RID: 11883
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GUIWindowManager windowManager;

	// Token: 0x04002E6C RID: 11884
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public INetConnection[] connectionToServer = new INetConnection[2];

	// Token: 0x04002E6E RID: 11886
	public readonly AntiCheatEncryptionAuthClient AntiCheatEncryptionAuthClient = new AntiCheatEncryptionAuthClient();

	// Token: 0x04002E6F RID: 11887
	public readonly ClientInfoCollection Clients = new ClientInfoCollection();

	// Token: 0x04002E72 RID: 11890
	public readonly AntiCheatEncryptionAuthServer AntiCheatEncryptionAuthServer = new AntiCheatEncryptionAuthServer();

	// Token: 0x04002E73 RID: 11891
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastBadPacketCheck;

	// Token: 0x04002E74 RID: 11892
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int badPacketDisconnectThreshold = 3;

	// Token: 0x04002E75 RID: 11893
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ProtocolManager protocolManager;

	// Token: 0x04002E76 RID: 11894
	public bool IsConnected;

	// Token: 0x04002E77 RID: 11895
	public int ReceivedBytesThisFrame;

	// Token: 0x04002E78 RID: 11896
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly CountdownTimer updateClientInfo = new CountdownTimer(5f, true);

	// Token: 0x04002E79 RID: 11897
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CountdownTimer countdownAnalyticsHeartbeat = new CountdownTimer(300f, false);

	// Token: 0x04002E7D RID: 11901
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<NetPackage> packagesToProcess = new List<NetPackage>();

	// Token: 0x0200076F RID: 1903
	// (Invoke) Token: 0x060038AA RID: 14506
	public delegate void ClientConnectionAction(ClientInfo _clientInfo);
}
