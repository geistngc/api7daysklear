using System;
using System.IO;
using System.Runtime.CompilerServices;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatClient;
using Epic.OnlineServices.AntiCheatCommon;

namespace Platform.EOS
{
	// Token: 0x02001CE7 RID: 7399
	public class AntiCheatServerP2P : IAntiCheatServer, IAntiCheatEncryption, IEncryptionModule
	{
		// Token: 0x0600DB90 RID: 56208 RVA: 0x004EA230 File Offset: 0x004E8430
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += this.apiInitialized;
		}

		// Token: 0x0600DB91 RID: 56209 RVA: 0x004EA258 File Offset: 0x004E8458
		[PublicizedFrom(EAccessModifier.Private)]
		public void apiInitialized()
		{
			EosHelpers.AssertMainThread("ACSP2P.Init");
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.antiCheatInterface = ((Api)this.owner.Api).PlatformInterface.GetAntiCheatClientInterface();
			}
			if (this.antiCheatInterface == null)
			{
				Log.Out("[EAC] AntiCheatServerP2P initialized with null interface");
				return;
			}
		}

		// Token: 0x0600DB92 RID: 56210 RVA: 0x004EA2D4 File Offset: 0x004E84D4
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddCallbacks()
		{
			AddNotifyMessageToPeerOptions addNotifyMessageToPeerOptions = default(AddNotifyMessageToPeerOptions);
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.handleMessageToPeerID = this.antiCheatInterface.AddNotifyMessageToPeer(ref addNotifyMessageToPeerOptions, null, new OnMessageToPeerCallback(this.handleMessageToPeer));
			}
			AddNotifyPeerAuthStatusChangedOptions addNotifyPeerAuthStatusChangedOptions = default(AddNotifyPeerAuthStatusChangedOptions);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.handlePeerAuthStateChangeID = this.antiCheatInterface.AddNotifyPeerAuthStatusChanged(ref addNotifyPeerAuthStatusChangedOptions, null, new OnPeerAuthStatusChangedCallback(this.handlePeerAuthStateChange));
			}
			AddNotifyPeerActionRequiredOptions addNotifyPeerActionRequiredOptions = default(AddNotifyPeerActionRequiredOptions);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.handlePeerActionRequiredID = this.antiCheatInterface.AddNotifyPeerActionRequired(ref addNotifyPeerActionRequiredOptions, null, new OnPeerActionRequiredCallback(this.handlePeerActionRequired));
			}
		}

		// Token: 0x0600DB93 RID: 56211 RVA: 0x004EA3DC File Offset: 0x004E85DC
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveCallbacks()
		{
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.antiCheatInterface.RemoveNotifyMessageToPeer(this.handleMessageToPeerID);
				this.antiCheatInterface.RemoveNotifyPeerAuthStatusChanged(this.handlePeerAuthStateChangeID);
				this.antiCheatInterface.RemoveNotifyPeerActionRequired(this.handlePeerActionRequiredID);
			}
		}

		// Token: 0x0600DB94 RID: 56212 RVA: 0x004EA448 File Offset: 0x004E8648
		public bool GetHostUserIdAndToken([TupleElementNames(new string[]
		{
			"userId",
			"token"
		})] out ValueTuple<PlatformUserIdentifierAbs, string> _hostUserIdAndToken)
		{
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			PlatformUserIdentifierAbs item;
			if (crossplatformPlatform == null)
			{
				item = null;
			}
			else
			{
				IUserClient user = crossplatformPlatform.User;
				item = ((user != null) ? user.PlatformUserId : null);
			}
			IPlatform crossplatformPlatform2 = PlatformManager.CrossplatformPlatform;
			string item2;
			if (crossplatformPlatform2 == null)
			{
				item2 = null;
			}
			else
			{
				IAuthenticationClient authenticationClient = crossplatformPlatform2.AuthenticationClient;
				item2 = ((authenticationClient != null) ? authenticationClient.GetAuthTicket() : null);
			}
			_hostUserIdAndToken = new ValueTuple<PlatformUserIdentifierAbs, string>(item, item2);
			return true;
		}

		// Token: 0x0600DB95 RID: 56213 RVA: 0x000027FC File Offset: 0x000009FC
		public void Update()
		{
		}

		// Token: 0x0600DB96 RID: 56214 RVA: 0x004EA49C File Offset: 0x004E869C
		public bool StartServer(AuthenticationSuccessfulCallbackDelegate _authSuccessfulDelegate, KickPlayerDelegate _kickPlayerDelegate)
		{
			if (this.ServerEacEnabled())
			{
				this.AddCallbacks();
				Log.Out("[EAC] Starting EAC peer to peer server");
				this.authSuccessfulDelegate = _authSuccessfulDelegate;
				this.kickPlayerDelegate = _kickPlayerDelegate;
				ProductUserId productUserId = ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId;
				BeginSessionOptions beginSessionOptions = new BeginSessionOptions
				{
					LocalUserId = productUserId,
					Mode = AntiCheatClientMode.PeerToPeer
				};
				object lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.antiCheatInterface.BeginSession(ref beginSessionOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-ACSP2P] Starting module failed: " + result.ToStringCached<Result>());
				}
				else
				{
					this.serverRunning = true;
				}
				return result == Result.Success;
			}
			return true;
		}

		// Token: 0x0600DB97 RID: 56215 RVA: 0x004EA570 File Offset: 0x004E8770
		public bool RegisterUser(ClientInfo _client)
		{
			if (!this.serverRunning)
			{
				return false;
			}
			Log.Out(string.Format("[EOS-ACSP2P] Registering user: {0}", _client));
			EosHelpers.AssertMainThread("ACSP2P.Reg");
			RegisterPeerOptions registerPeerOptions = new RegisterPeerOptions
			{
				PeerHandle = AntiCheatCommon.ClientInfoToIntPtr(_client),
				ClientPlatform = EosHelpers.DeviceTypeToAntiCheatPlatformMappings[_client.device],
				PeerProductUserId = ((UserIdentifierEos)_client.CrossplatformId).ProductUserId,
				ClientType = (_client.requiresAntiCheat ? AntiCheatCommonClientType.ProtectedClient : AntiCheatCommonClientType.UnprotectedClient),
				IpAddress = _client.ip,
				AuthenticationTimeout = 60U
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.RegisterPeer(ref registerPeerOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACSP2P] Failed registering user: " + result.ToStringCached<Result>());
				return false;
			}
			if (!_client.requiresAntiCheat)
			{
				this.authSuccessfulDelegate(_client);
			}
			return true;
		}

		// Token: 0x0600DB98 RID: 56216 RVA: 0x004EA680 File Offset: 0x004E8880
		public void FreeUser(ClientInfo _client)
		{
			if (!this.serverRunning)
			{
				return;
			}
			EosHelpers.AssertMainThread("ACS.Free");
			Log.Out(string.Format("[EOS-ACSP2P] FreeUser: {0}", _client));
			UnregisterPeerOptions unregisterPeerOptions = new UnregisterPeerOptions
			{
				PeerHandle = AntiCheatCommon.ClientInfoToIntPtr(_client)
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.UnregisterPeer(ref unregisterPeerOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACSP2P] Failed unregistering user: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600DB99 RID: 56217 RVA: 0x004EA71C File Offset: 0x004E891C
		public void HandleMessageFromClient(ClientInfo _cInfo, byte[] _data)
		{
			if (!this.serverRunning)
			{
				Log.Warning("[EOS-ACSP2P] Server: Received EAC package but EAC was not initialized");
				return;
			}
			if (AntiCheatCommon.DebugEacVerbose)
			{
				Log.Out(string.Format("[EOS-ACSP2P] PushNetworkMessage (len={0}, from={1})", _data.Length, _cInfo.InternalId));
			}
			ReceiveMessageFromPeerOptions receiveMessageFromPeerOptions = new ReceiveMessageFromPeerOptions
			{
				Data = new ArraySegment<byte>(_data),
				PeerHandle = AntiCheatCommon.ClientInfoToIntPtr(_cInfo)
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.ReceiveMessageFromPeer(ref receiveMessageFromPeerOptions);
			}
			if (result != Result.AntiCheatPeerNotFound && result != Result.Success)
			{
				Log.Error("[EOS-ACSP2P] Failed handling message: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600DB9A RID: 56218 RVA: 0x004EA7E0 File Offset: 0x004E89E0
		public void StopServer()
		{
			if (!this.serverRunning)
			{
				return;
			}
			this.RemoveCallbacks();
			EndSessionOptions endSessionOptions = default(EndSessionOptions);
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.EndSession(ref endSessionOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACSP2P] Stopping module failed: " + result.ToStringCached<Result>());
			}
			this.serverRunning = false;
			this.authSuccessfulDelegate = null;
			this.kickPlayerDelegate = null;
		}

		// Token: 0x0600DB9B RID: 56219 RVA: 0x004EA86C File Offset: 0x004E8A6C
		[PublicizedFrom(EAccessModifier.Private)]
		public void handleMessageToPeer(ref OnMessageToClientCallbackInfo _data)
		{
			if (!this.serverRunning)
			{
				return;
			}
			ClientInfo clientInfo = AntiCheatCommon.IntPtrToClientInfo(_data.ClientHandle, "[EOS-ACSP2P] Got message for unknown client number: {0}");
			if (clientInfo == null)
			{
				Log.Out(string.Format("[EOS-ACSP2P] FreeUser: {0}", _data.ClientHandle));
				UnregisterPeerOptions unregisterPeerOptions = new UnregisterPeerOptions
				{
					PeerHandle = _data.ClientHandle
				};
				object lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.antiCheatInterface.UnregisterPeer(ref unregisterPeerOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-ACSP2P] Failed unregistering user: " + result.ToStringCached<Result>());
				}
			}
			if (clientInfo != null)
			{
				clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEAC>().Setup(_data.MessageData.Count, _data.MessageData.Array));
			}
		}

		// Token: 0x0600DB9C RID: 56220 RVA: 0x004EA950 File Offset: 0x004E8B50
		[PublicizedFrom(EAccessModifier.Private)]
		public void handlePeerActionRequired(ref OnClientActionRequiredCallbackInfo _data)
		{
			if (!this.serverRunning)
			{
				return;
			}
			ClientInfo clientInfo = AntiCheatCommon.IntPtrToClientInfo(_data.ClientHandle, "[EOS-ACSP2P] Got action for unknown client number: {0}");
			if (clientInfo == null)
			{
				return;
			}
			AntiCheatCommonClientAction clientAction = _data.ClientAction;
			AntiCheatCommonClientActionReason actionReasonCode = _data.ActionReasonCode;
			string text = _data.ActionReasonDetailsString;
			if (clientAction != AntiCheatCommonClientAction.RemovePlayer)
			{
				Log.Warning(string.Format("[EOS-ACSP2P] Got invalid action ({0}), reason='{1}', details={2}, client={3}", new object[]
				{
					clientAction.ToStringCached<AntiCheatCommonClientAction>(),
					actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(),
					text,
					clientInfo
				}));
				return;
			}
			Log.Out(string.Format("[EOS-ACSP2P] Kicking player. Reason={0}, details='{1}', client={2}", actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(), text, clientInfo));
			KickPlayerDelegate kickPlayerDelegate = this.kickPlayerDelegate;
			if (kickPlayerDelegate == null)
			{
				return;
			}
			ClientInfo cInfo = clientInfo;
			GameUtils.EKickReason kickReason = GameUtils.EKickReason.EosEacViolation;
			int apiResponseEnum = (int)actionReasonCode;
			string customReason = text;
			kickPlayerDelegate(cInfo, new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customReason));
		}

		// Token: 0x0600DB9D RID: 56221 RVA: 0x004EAA08 File Offset: 0x004E8C08
		[PublicizedFrom(EAccessModifier.Private)]
		public void handlePeerAuthStateChange(ref OnClientAuthStatusChangedCallbackInfo _data)
		{
			if (!this.serverRunning)
			{
				return;
			}
			ClientInfo cInfo = AntiCheatCommon.IntPtrToClientInfo(_data.ClientHandle, "[EOS-ACSP2P] Got auth state change for unknown client number: {0}");
			if (_data.ClientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
			{
				Log.Out(string.Format("[EOS-ACSP2P] Remote Auth complete for client number {0}", _data.ClientHandle));
				AuthenticationSuccessfulCallbackDelegate authenticationSuccessfulCallbackDelegate = this.authSuccessfulDelegate;
				if (authenticationSuccessfulCallbackDelegate == null)
				{
					return;
				}
				authenticationSuccessfulCallbackDelegate(cInfo);
			}
		}

		// Token: 0x0600DB9E RID: 56222 RVA: 0x000027FC File Offset: 0x000009FC
		public void Destroy()
		{
		}

		// Token: 0x0600DB9F RID: 56223 RVA: 0x004EAA63 File Offset: 0x004E8C63
		public bool ServerEacEnabled()
		{
			return this.antiCheatInterface != null && GamePrefs.GetBool(EnumGamePrefs.ServerEACPeerToPeer);
		}

		// Token: 0x0600DBA0 RID: 56224 RVA: 0x004EAA7C File Offset: 0x004E8C7C
		public bool ServerEacAvailable()
		{
			return this.antiCheatInterface != null;
		}

		// Token: 0x0600DBA1 RID: 56225 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool EncryptionAvailable()
		{
			return false;
		}

		// Token: 0x0600DBA2 RID: 56226 RVA: 0x004EAA8A File Offset: 0x004E8C8A
		public bool EncryptStream(ClientInfo _cInfo, MemoryStream _stream)
		{
			throw new NotImplementedException("Encryption is not supported for a Peer to Peer AntiCheatServer.");
		}

		// Token: 0x0600DBA3 RID: 56227 RVA: 0x004EAA8A File Offset: 0x004E8C8A
		public bool DecryptStream(ClientInfo _cInfo, MemoryStream _stream)
		{
			throw new NotImplementedException("Encryption is not supported for a Peer to Peer AntiCheatServer.");
		}

		// Token: 0x0400A63C RID: 42556
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A63D RID: 42557
		[PublicizedFrom(EAccessModifier.Private)]
		public AntiCheatClientInterface antiCheatInterface;

		// Token: 0x0400A63E RID: 42558
		[PublicizedFrom(EAccessModifier.Private)]
		public bool serverRunning;

		// Token: 0x0400A63F RID: 42559
		[PublicizedFrom(EAccessModifier.Private)]
		public AuthenticationSuccessfulCallbackDelegate authSuccessfulDelegate;

		// Token: 0x0400A640 RID: 42560
		[PublicizedFrom(EAccessModifier.Private)]
		public KickPlayerDelegate kickPlayerDelegate;

		// Token: 0x0400A641 RID: 42561
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handleMessageToPeerID;

		// Token: 0x0400A642 RID: 42562
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handlePeerAuthStateChangeID;

		// Token: 0x0400A643 RID: 42563
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handlePeerActionRequiredID;
	}
}
