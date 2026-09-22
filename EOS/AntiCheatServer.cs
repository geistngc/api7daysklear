using System;
using System.IO;
using System.Runtime.CompilerServices;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.AntiCheatServer;

namespace Platform.EOS
{
	// Token: 0x02001CE6 RID: 7398
	public class AntiCheatServer : IAntiCheatServer, IAntiCheatEncryption, IEncryptionModule
	{
		// Token: 0x0600DB7B RID: 56187 RVA: 0x004E975B File Offset: 0x004E795B
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += this.apiInitialized;
		}

		// Token: 0x0600DB7C RID: 56188 RVA: 0x004E9780 File Offset: 0x004E7980
		[PublicizedFrom(EAccessModifier.Private)]
		public void apiInitialized()
		{
			EosHelpers.AssertMainThread("ACS.Init");
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.antiCheatInterface = ((Api)this.owner.Api).PlatformInterface.GetAntiCheatServerInterface();
			}
			if (this.antiCheatInterface == null)
			{
				Log.Out("[EAC] AntiCheatServer initialized with null interface");
				return;
			}
		}

		// Token: 0x0600DB7D RID: 56189 RVA: 0x004E97FC File Offset: 0x004E79FC
		[PublicizedFrom(EAccessModifier.Private)]
		public void addCallbacks()
		{
			AddNotifyMessageToClientOptions addNotifyMessageToClientOptions = default(AddNotifyMessageToClientOptions);
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.handleMessageToClientID = this.antiCheatInterface.AddNotifyMessageToClient(ref addNotifyMessageToClientOptions, null, new OnMessageToClientCallback(this.handleMessageToClient));
			}
			AddNotifyClientActionRequiredOptions addNotifyClientActionRequiredOptions = default(AddNotifyClientActionRequiredOptions);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.handleClientActionRequiredID = this.antiCheatInterface.AddNotifyClientActionRequired(ref addNotifyClientActionRequiredOptions, null, new OnClientActionRequiredCallback(this.handleClientAction));
			}
			AddNotifyClientAuthStatusChangedOptions addNotifyClientAuthStatusChangedOptions = default(AddNotifyClientAuthStatusChangedOptions);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.handleClientAuthStateChangeID = this.antiCheatInterface.AddNotifyClientAuthStatusChanged(ref addNotifyClientAuthStatusChangedOptions, null, new OnClientAuthStatusChangedCallback(this.handleClientAuthStateChange));
			}
		}

		// Token: 0x0600DB7E RID: 56190 RVA: 0x004E9904 File Offset: 0x004E7B04
		[PublicizedFrom(EAccessModifier.Private)]
		public void removeCallbacks()
		{
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				if (this.handleMessageToClientID > 0UL)
				{
					this.antiCheatInterface.RemoveNotifyMessageToClient(this.handleMessageToClientID);
					this.handleMessageToClientID = 0UL;
				}
				if (this.handleClientActionRequiredID > 0UL)
				{
					this.antiCheatInterface.RemoveNotifyClientActionRequired(this.handleClientActionRequiredID);
					this.handleClientActionRequiredID = 0UL;
				}
				if (this.handleClientAuthStateChangeID > 0UL)
				{
					this.antiCheatInterface.RemoveNotifyClientAuthStatusChanged(this.handleClientAuthStateChangeID);
					this.handleClientAuthStateChangeID = 0UL;
				}
			}
		}

		// Token: 0x0600DB7F RID: 56191 RVA: 0x000027FC File Offset: 0x000009FC
		public void Update()
		{
		}

		// Token: 0x0600DB80 RID: 56192 RVA: 0x004E99A8 File Offset: 0x004E7BA8
		public bool GetHostUserIdAndToken([TupleElementNames(new string[]
		{
			"userId",
			"token"
		})] out ValueTuple<PlatformUserIdentifierAbs, string> _hostUserIdAndToken)
		{
			_hostUserIdAndToken = default(ValueTuple<PlatformUserIdentifierAbs, string>);
			return false;
		}

		// Token: 0x0600DB81 RID: 56193 RVA: 0x004E99B4 File Offset: 0x004E7BB4
		public bool StartServer(AuthenticationSuccessfulCallbackDelegate _authSuccessfulDelegate, KickPlayerDelegate _kickPlayerDelegate)
		{
			if (this.ServerEacEnabled())
			{
				this.addCallbacks();
				Log.Out("[EAC] Starting EAC server");
				this.authSuccessfulDelegate = _authSuccessfulDelegate;
				this.kickPlayerDelegate = _kickPlayerDelegate;
				IUserClient user = this.owner.User;
				PlatformUserIdentifierAbs platformUserIdentifierAbs;
				if ((platformUserIdentifierAbs = ((user != null) ? user.PlatformUserId : null)) == null)
				{
					IUserClient userServer = this.owner.UserServer;
					platformUserIdentifierAbs = ((userServer != null) ? userServer.PlatformUserId : null);
				}
				UserIdentifierEos userIdentifierEos = (UserIdentifierEos)platformUserIdentifierAbs;
				ProductUserId localUserId = (userIdentifierEos != null) ? userIdentifierEos.ProductUserId : null;
				string value = SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoString.GameHost);
				BeginSessionOptions beginSessionOptions = new BeginSessionOptions
				{
					EnableGameplayData = false,
					LocalUserId = localUserId,
					RegisterTimeoutSeconds = 60U,
					ServerName = value
				};
				object lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.antiCheatInterface.BeginSession(ref beginSessionOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-ACS] Starting module failed: " + result.ToStringCached<Result>());
				}
				else
				{
					this.serverRunning = true;
				}
				return result == Result.Success;
			}
			return true;
		}

		// Token: 0x0600DB82 RID: 56194 RVA: 0x004E9AD8 File Offset: 0x004E7CD8
		public bool RegisterUser(ClientInfo _client)
		{
			if (!this.serverRunning)
			{
				return false;
			}
			Log.Out(string.Format("[EOS-ACS] Registering user: {0}", _client));
			EosHelpers.AssertMainThread("ACS.Reg");
			RegisterClientOptions registerClientOptions = new RegisterClientOptions
			{
				UserId = ((UserIdentifierEos)_client.CrossplatformId).ProductUserId,
				ClientHandle = AntiCheatCommon.ClientInfoToIntPtr(_client),
				ClientPlatform = EosHelpers.DeviceTypeToAntiCheatPlatformMappings[_client.device],
				ClientType = (_client.requiresAntiCheat ? AntiCheatCommonClientType.ProtectedClient : AntiCheatCommonClientType.UnprotectedClient),
				IpAddress = _client.ip
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.RegisterClient(ref registerClientOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACS] Failed registerung user: " + result.ToStringCached<Result>());
				return false;
			}
			return true;
		}

		// Token: 0x0600DB83 RID: 56195 RVA: 0x004E9BCC File Offset: 0x004E7DCC
		public void FreeUser(ClientInfo _client)
		{
			if (!this.serverRunning)
			{
				return;
			}
			EosHelpers.AssertMainThread("ACS.Free");
			Log.Out(string.Format("[EOS-ACS] FreeUser: {0}", _client));
			UnregisterClientOptions unregisterClientOptions = new UnregisterClientOptions
			{
				ClientHandle = AntiCheatCommon.ClientInfoToIntPtr(_client)
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.UnregisterClient(ref unregisterClientOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACS] Failed unregistering user: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600DB84 RID: 56196 RVA: 0x004E9C68 File Offset: 0x004E7E68
		public void HandleMessageFromClient(ClientInfo _cInfo, byte[] _data)
		{
			if (!this.serverRunning)
			{
				Log.Warning("[EOS-ACS] Server: Received EAC package but EAC was not initialized");
				return;
			}
			if (AntiCheatCommon.DebugEacVerbose)
			{
				Log.Out(string.Format("[EOS-ACS] PushNetworkMessage (len={0}, from={1})", _data.Length, _cInfo.InternalId));
			}
			ReceiveMessageFromClientOptions receiveMessageFromClientOptions = new ReceiveMessageFromClientOptions
			{
				Data = new ArraySegment<byte>(_data),
				ClientHandle = AntiCheatCommon.ClientInfoToIntPtr(_cInfo)
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.ReceiveMessageFromClient(ref receiveMessageFromClientOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACS] Failed handling message: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600DB85 RID: 56197 RVA: 0x004E9D24 File Offset: 0x004E7F24
		public void StopServer()
		{
			if (!this.serverRunning)
			{
				return;
			}
			this.removeCallbacks();
			EndSessionOptions endSessionOptions = default(EndSessionOptions);
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.EndSession(ref endSessionOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACS] Stopping module failed: " + result.ToStringCached<Result>());
			}
			this.serverRunning = false;
			this.authSuccessfulDelegate = null;
			this.kickPlayerDelegate = null;
		}

		// Token: 0x0600DB86 RID: 56198 RVA: 0x004E9DB0 File Offset: 0x004E7FB0
		[PublicizedFrom(EAccessModifier.Private)]
		public void handleMessageToClient(ref OnMessageToClientCallbackInfo _data)
		{
			ClientInfo clientInfo = AntiCheatCommon.IntPtrToClientInfo(_data.ClientHandle, "[EOS-ACS] Got message for unknown client number: {0}");
			if (clientInfo == null)
			{
				Log.Out(string.Format("[EOS-ACS] FreeUser: {0}", _data.ClientHandle));
				UnregisterClientOptions unregisterClientOptions = new UnregisterClientOptions
				{
					ClientHandle = _data.ClientHandle
				};
				object lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.antiCheatInterface.UnregisterClient(ref unregisterClientOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-ACS] Failed unregistering user: " + result.ToStringCached<Result>());
				}
			}
			if (AntiCheatCommon.DebugEacVerbose)
			{
				Log.Out(string.Format("[EOS-ACS] Forward message to client (len={0}, to={1})", _data.MessageData.Count, clientInfo.InternalId));
			}
			if (clientInfo != null)
			{
				clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEAC>().Setup(_data.MessageData.Count, _data.MessageData.Array));
			}
		}

		// Token: 0x0600DB87 RID: 56199 RVA: 0x004E9EBC File Offset: 0x004E80BC
		[PublicizedFrom(EAccessModifier.Private)]
		public void handleClientAction(ref OnClientActionRequiredCallbackInfo _data)
		{
			ClientInfo clientInfo = AntiCheatCommon.IntPtrToClientInfo(_data.ClientHandle, "[EOS-ACS] Got action for unknown client number: {0}");
			if (clientInfo == null)
			{
				return;
			}
			AntiCheatCommonClientAction clientAction = _data.ClientAction;
			AntiCheatCommonClientActionReason actionReasonCode = _data.ActionReasonCode;
			string text = _data.ActionReasonDetailsString;
			if (clientAction != AntiCheatCommonClientAction.RemovePlayer)
			{
				Log.Warning(string.Format("[EOS-ACS] Got invalid action ({0}), reason='{1}', details={2}, client={3}", new object[]
				{
					clientAction.ToStringCached<AntiCheatCommonClientAction>(),
					actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(),
					text,
					clientInfo
				}));
				return;
			}
			Log.Out(string.Format("[EOS-ACS] Kicking player. Reason={0}, details='{1}', client={2}", actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(), text, clientInfo));
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

		// Token: 0x0600DB88 RID: 56200 RVA: 0x004E9F6C File Offset: 0x004E816C
		[PublicizedFrom(EAccessModifier.Private)]
		public void handleClientAuthStateChange(ref OnClientAuthStatusChangedCallbackInfo _data)
		{
			ClientInfo cInfo = AntiCheatCommon.IntPtrToClientInfo(_data.ClientHandle, "[EOS-ACS] Got auth state change for unknown client number: {0}");
			if (_data.ClientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
			{
				AuthenticationSuccessfulCallbackDelegate authenticationSuccessfulCallbackDelegate = this.authSuccessfulDelegate;
				if (authenticationSuccessfulCallbackDelegate == null)
				{
					return;
				}
				authenticationSuccessfulCallbackDelegate(cInfo);
			}
		}

		// Token: 0x0600DB89 RID: 56201 RVA: 0x000027FC File Offset: 0x000009FC
		public void Destroy()
		{
		}

		// Token: 0x0600DB8A RID: 56202 RVA: 0x004E9FA4 File Offset: 0x004E81A4
		public bool ServerEacEnabled()
		{
			return this.antiCheatInterface != null && GamePrefs.GetBool(EnumGamePrefs.EACEnabled);
		}

		// Token: 0x0600DB8B RID: 56203 RVA: 0x004E9FBD File Offset: 0x004E81BD
		public bool ServerEacAvailable()
		{
			return this.antiCheatInterface != null;
		}

		// Token: 0x0600DB8C RID: 56204 RVA: 0x004E9FCB File Offset: 0x004E81CB
		public bool EncryptionAvailable()
		{
			return this.ServerEacEnabled();
		}

		// Token: 0x0600DB8D RID: 56205 RVA: 0x004E9FD4 File Offset: 0x004E81D4
		public bool EncryptStream(ClientInfo _cInfo, MemoryStream _stream)
		{
			int num = (int)_stream.Length;
			_stream.SetLength((long)(num + 40));
			ArraySegment<byte> data = new ArraySegment<byte>(_stream.GetBuffer(), 0, num);
			ProtectMessageOptions protectMessageOptions = new ProtectMessageOptions
			{
				ClientHandle = AntiCheatCommon.ClientInfoToIntPtr(_cInfo),
				Data = data,
				OutBufferSizeBytes = (uint)(num + 40)
			};
			byte[] array = MemoryPools.poolByte.Alloc(num + 40);
			ArraySegment<byte> outBuffer = new ArraySegment<byte>(array);
			object lockObject = AntiCheatCommon.LockObject;
			uint num2;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.ProtectMessage(ref protectMessageOptions, outBuffer, out num2);
			}
			_stream.SetLength(0L);
			_stream.Write(array, 0, (int)num2);
			_stream.Position = 0L;
			MemoryPools.poolByte.Free(array);
			if (result != Result.Success)
			{
				Log.Error(string.Format("[EOS-ACS] Failed encrypting stream for {0}: {1}", _cInfo.InternalId, result.ToStringCached<Result>()));
				return false;
			}
			if (AntiCheatCommon.DebugEacVerbose)
			{
				Log.Out(string.Format("[EOS-ACS] Encrypted. Orig stream len={0}, result len={1}", num, num2));
			}
			_stream.SetLength((long)((ulong)num2));
			return true;
		}

		// Token: 0x0600DB8E RID: 56206 RVA: 0x004EA100 File Offset: 0x004E8300
		public bool DecryptStream(ClientInfo _cInfo, MemoryStream _stream)
		{
			int num = (int)_stream.Length;
			ArraySegment<byte> data = new ArraySegment<byte>(_stream.GetBuffer(), 0, num);
			UnprotectMessageOptions unprotectMessageOptions = new UnprotectMessageOptions
			{
				ClientHandle = AntiCheatCommon.ClientInfoToIntPtr(_cInfo),
				Data = data,
				OutBufferSizeBytes = (uint)num
			};
			byte[] array = MemoryPools.poolByte.Alloc(num);
			ArraySegment<byte> outBuffer = new ArraySegment<byte>(array);
			object lockObject = AntiCheatCommon.LockObject;
			uint num2;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.UnprotectMessage(ref unprotectMessageOptions, outBuffer, out num2);
			}
			_stream.SetLength(0L);
			try
			{
				_stream.Write(array, 0, (int)num2);
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			_stream.Position = 0L;
			MemoryPools.poolByte.Free(array);
			if (result != Result.Success)
			{
				Log.Error(string.Format("[EOS-ACS] Failed decrypting stream from {0}: {1}", _cInfo.InternalId, result.ToStringCached<Result>()));
				return false;
			}
			if (AntiCheatCommon.DebugEacVerbose)
			{
				Log.Out(string.Format("[EOS-ACS] Decrypted. Orig stream len={0}, result len={1}", num, num2));
			}
			_stream.SetLength((long)((ulong)num2));
			return true;
		}

		// Token: 0x0400A634 RID: 42548
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A635 RID: 42549
		[PublicizedFrom(EAccessModifier.Private)]
		public AntiCheatServerInterface antiCheatInterface;

		// Token: 0x0400A636 RID: 42550
		[PublicizedFrom(EAccessModifier.Private)]
		public bool serverRunning;

		// Token: 0x0400A637 RID: 42551
		[PublicizedFrom(EAccessModifier.Private)]
		public AuthenticationSuccessfulCallbackDelegate authSuccessfulDelegate;

		// Token: 0x0400A638 RID: 42552
		[PublicizedFrom(EAccessModifier.Private)]
		public KickPlayerDelegate kickPlayerDelegate;

		// Token: 0x0400A639 RID: 42553
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handleMessageToClientID;

		// Token: 0x0400A63A RID: 42554
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handleClientAuthStateChangeID;

		// Token: 0x0400A63B RID: 42555
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handleClientActionRequiredID;
	}
}
