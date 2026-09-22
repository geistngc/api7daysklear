using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatClient;
using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.Connect;

namespace Platform.EOS
{
	// Token: 0x02001CE2 RID: 7394
	public class AntiCheatClientP2P
	{
		// Token: 0x1400013F RID: 319
		// (add) Token: 0x0600DB5F RID: 56159 RVA: 0x004E8BE8 File Offset: 0x004E6DE8
		// (remove) Token: 0x0600DB60 RID: 56160 RVA: 0x004E8C48 File Offset: 0x004E6E48
		public event Action OnRemoteAuthComplete
		{
			add
			{
				object obj = this.lockObject;
				lock (obj)
				{
					this.onRemoteAuthComplete = (Action)Delegate.Combine(this.onRemoteAuthComplete, value);
					if (this.clientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
					{
						value();
					}
				}
			}
			remove
			{
				object obj = this.lockObject;
				lock (obj)
				{
					this.onRemoteAuthComplete = (Action)Delegate.Remove(this.onRemoteAuthComplete, value);
				}
			}
		}

		// Token: 0x0600DB61 RID: 56161 RVA: 0x004E8C9C File Offset: 0x004E6E9C
		public AntiCheatClientP2P(IPlatform _owner, AntiCheatClientInterface _antiCheatInterface)
		{
			this.owner = _owner;
			this.antiCheatInterface = _antiCheatInterface;
			this.connectInterface = ((Api)this.owner.Api).ConnectInterface;
		}

		// Token: 0x0600DB62 RID: 56162 RVA: 0x004E8CFC File Offset: 0x004E6EFC
		public void Activate()
		{
			if (this.handleMessageToPeerID == 0UL)
			{
				AddNotifyMessageToPeerOptions addNotifyMessageToPeerOptions = default(AddNotifyMessageToPeerOptions);
				object obj = AntiCheatCommon.LockObject;
				lock (obj)
				{
					this.handleMessageToPeerID = this.antiCheatInterface.AddNotifyMessageToPeer(ref addNotifyMessageToPeerOptions, null, new OnMessageToPeerCallback(this.handleMessageToPeer));
				}
			}
			if (this.handlePeerAuthStateChangeID == 0UL)
			{
				AddNotifyPeerAuthStatusChangedOptions addNotifyPeerAuthStatusChangedOptions = default(AddNotifyPeerAuthStatusChangedOptions);
				object obj = AntiCheatCommon.LockObject;
				lock (obj)
				{
					this.handlePeerAuthStateChangeID = this.antiCheatInterface.AddNotifyPeerAuthStatusChanged(ref addNotifyPeerAuthStatusChangedOptions, null, new OnPeerAuthStatusChangedCallback(this.handlePeerAuthStateChange));
				}
			}
			if (this.handlePeerActionRequiredID == 0UL)
			{
				AddNotifyPeerActionRequiredOptions addNotifyPeerActionRequiredOptions = default(AddNotifyPeerActionRequiredOptions);
				object obj = AntiCheatCommon.LockObject;
				lock (obj)
				{
					this.handlePeerActionRequiredID = this.antiCheatInterface.AddNotifyPeerActionRequired(ref addNotifyPeerActionRequiredOptions, null, new OnPeerActionRequiredCallback(this.handlePeerActionRequired));
				}
			}
		}

		// Token: 0x0600DB63 RID: 56163 RVA: 0x004E8E18 File Offset: 0x004E7018
		public void Deactivate()
		{
			object obj = AntiCheatCommon.LockObject;
			lock (obj)
			{
				if (this.handleMessageToPeerID != 0UL)
				{
					this.antiCheatInterface.RemoveNotifyMessageToPeer(this.handleMessageToPeerID);
					this.handleMessageToPeerID = 0UL;
				}
				if (this.handlePeerAuthStateChangeID != 0UL)
				{
					this.antiCheatInterface.RemoveNotifyPeerAuthStatusChanged(this.handlePeerAuthStateChangeID);
					this.handlePeerAuthStateChangeID = 0UL;
				}
				if (this.handlePeerActionRequiredID != 0UL)
				{
					this.antiCheatInterface.RemoveNotifyPeerActionRequired(this.handlePeerActionRequiredID);
					this.handlePeerActionRequiredID = 0UL;
				}
			}
		}

		// Token: 0x0600DB64 RID: 56164 RVA: 0x004E8EB4 File Offset: 0x004E70B4
		[PublicizedFrom(EAccessModifier.Private)]
		public bool BeginSession()
		{
			ProductUserId productUserId = ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId;
			BeginSessionOptions beginSessionOptions = new BeginSessionOptions
			{
				LocalUserId = productUserId,
				Mode = AntiCheatClientMode.PeerToPeer
			};
			object obj = AntiCheatCommon.LockObject;
			Result result;
			lock (obj)
			{
				result = this.antiCheatInterface.BeginSession(ref beginSessionOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACCP2P] Starting module failed: " + result.ToStringCached<Result>());
				return false;
			}
			return true;
		}

		// Token: 0x0600DB65 RID: 56165 RVA: 0x004E8F50 File Offset: 0x004E7150
		public void ConnectToServer([TupleElementNames(new string[]
		{
			"userId",
			"token"
		})] ValueTuple<PlatformUserIdentifierAbs, string> _hostIdentifierAndToken, Action _onConnectionComplete, Action<string> _onConnectionFailed)
		{
			AntiCheatClientP2P.<>c__DisplayClass18_0 CS$<>8__locals1 = new AntiCheatClientP2P.<>c__DisplayClass18_0();
			CS$<>8__locals1._onConnectionFailed = _onConnectionFailed;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1._onConnectionComplete = _onConnectionComplete;
			PlatformUserIdentifierAbs item = _hostIdentifierAndToken.Item1;
			CS$<>8__locals1.identifierEos = (item as UserIdentifierEos);
			if (CS$<>8__locals1.identifierEos != null)
			{
				CS$<>8__locals1.identifierEos.DecodeTicket(_hostIdentifierAndToken.Item2);
				IdToken value = new IdToken
				{
					JsonWebToken = CS$<>8__locals1.identifierEos.Ticket,
					ProductUserId = CS$<>8__locals1.identifierEos.ProductUserId
				};
				VerifyIdTokenOptions verifyIdTokenOptions = new VerifyIdTokenOptions
				{
					IdToken = new IdToken?(value)
				};
				object obj = AntiCheatCommon.LockObject;
				lock (obj)
				{
					this.connectInterface.VerifyIdToken(ref verifyIdTokenOptions, null, new OnVerifyIdTokenCallback(CS$<>8__locals1.<ConnectToServer>g__VerifyIdTokenCallback|0));
				}
				return;
			}
			Log.Warning(string.Format("[EOS] [ACl.Auth] Expected EOS Crossplatform ID? But got: {0}", _hostIdentifierAndToken.Item1));
			Action<string> onConnectionFailed = CS$<>8__locals1._onConnectionFailed;
			if (onConnectionFailed == null)
			{
				return;
			}
			onConnectionFailed("Invalid EOS Crossplatform ID");
		}

		// Token: 0x0600DB66 RID: 56166 RVA: 0x004E9068 File Offset: 0x004E7268
		public bool IsServerAntiCheatProtected()
		{
			return this.serverDeviceType.RequiresAntiCheat();
		}

		// Token: 0x0600DB67 RID: 56167 RVA: 0x004E9078 File Offset: 0x004E7278
		public void HandleMessageFromPeer(byte[] _data)
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				return;
			}
			if (AntiCheatCommon.DebugEacVerbose)
			{
				Log.Out(string.Format("[EOS-ACC] PushNetworkMessage (len={0})", _data.Length));
			}
			EosHelpers.AssertMainThread("ACC.HMFP");
			ReceiveMessageFromPeerOptions receiveMessageFromPeerOptions = new ReceiveMessageFromPeerOptions
			{
				Data = new ArraySegment<byte>(_data),
				PeerHandle = this.serverHandle
			};
			object obj = AntiCheatCommon.LockObject;
			Result result;
			lock (obj)
			{
				result = this.antiCheatInterface.ReceiveMessageFromPeer(ref receiveMessageFromPeerOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACC] Failed handling message: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600DB68 RID: 56168 RVA: 0x004E9134 File Offset: 0x004E7334
		public void DisconnectFromServer()
		{
			this.clientAuthStatus = AntiCheatCommonClientAuthStatus.Invalid;
			this.serverDeviceType = ClientInfo.EDeviceType.Unknown;
			this.onRemoteAuthComplete = null;
			this.EndSession();
			this.Deactivate();
			Log.Out("[EOS-ACC] Disconnected from game server");
		}

		// Token: 0x0600DB69 RID: 56169 RVA: 0x004E9164 File Offset: 0x004E7364
		[PublicizedFrom(EAccessModifier.Private)]
		public void EndSession()
		{
			EosHelpers.AssertMainThread("ACC.Disc");
			EndSessionOptions endSessionOptions = default(EndSessionOptions);
			object obj = AntiCheatCommon.LockObject;
			Result result;
			lock (obj)
			{
				result = this.antiCheatInterface.EndSession(ref endSessionOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACC] Stopping module failed: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600DB6A RID: 56170 RVA: 0x004E91DC File Offset: 0x004E73DC
		[PublicizedFrom(EAccessModifier.Private)]
		public void handleMessageToPeer(ref OnMessageToClientCallbackInfo _data)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEAC>().Setup(_data.MessageData.Count, _data.MessageData.Array), false);
		}

		// Token: 0x0600DB6B RID: 56171 RVA: 0x004E921C File Offset: 0x004E741C
		[PublicizedFrom(EAccessModifier.Private)]
		public void handlePeerActionRequired(ref OnClientActionRequiredCallbackInfo _data)
		{
			if (_data.ClientHandle != this.serverHandle)
			{
				Log.Error("[EOS-ACCP2P] Received Peer action for non-server peer as a client.");
				return;
			}
			AntiCheatCommonClientAction clientAction = _data.ClientAction;
			AntiCheatCommonClientActionReason actionReasonCode = _data.ActionReasonCode;
			string text = _data.ActionReasonDetailsString;
			if (clientAction == AntiCheatCommonClientAction.RemovePlayer)
			{
				Log.Out(string.Concat(new string[]
				{
					"[EOS-ACCP2P] Disconnecting from server. Reason=",
					actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(),
					", details='",
					text,
					"'"
				}));
				GameUtils.EKickReason kickReason = GameUtils.EKickReason.EosEacViolation;
				int apiResponseEnum = (int)actionReasonCode;
				string customReason = text;
				this.QueueDisconnectFromServer(new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customReason));
				return;
			}
			Log.Warning(string.Concat(new string[]
			{
				"[EOS-ACCP2P] Got invalid action (",
				clientAction.ToStringCached<AntiCheatCommonClientAction>(),
				"), reason='",
				actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(),
				"', details='",
				text,
				"'"
			}));
		}

		// Token: 0x0600DB6C RID: 56172 RVA: 0x004E92FC File Offset: 0x004E74FC
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator DisconnectOnNextFrame(GameUtils.KickPlayerData _kickPlayerData)
		{
			yield return null;
			this.EndSession();
			this.Deactivate();
			SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
			GameManager.Instance.ShowMessagePlayerDenied(_kickPlayerData);
			yield break;
		}

		// Token: 0x0600DB6D RID: 56173 RVA: 0x004E9312 File Offset: 0x004E7512
		[PublicizedFrom(EAccessModifier.Private)]
		public void QueueDisconnectFromServer(GameUtils.KickPlayerData _kickPlayerData)
		{
			ThreadManager.StartCoroutine(this.DisconnectOnNextFrame(_kickPlayerData));
		}

		// Token: 0x0600DB6E RID: 56174 RVA: 0x004E9324 File Offset: 0x004E7524
		[PublicizedFrom(EAccessModifier.Private)]
		public void handlePeerAuthStateChange(ref OnClientAuthStatusChangedCallbackInfo _data)
		{
			if (_data.ClientHandle != this.serverHandle)
			{
				Log.Error("[EOS-ACCP2P] Received Peer auth state change for non-server peer as a client.");
				return;
			}
			this.clientAuthStatus = _data.ClientAuthStatus;
			if (this.clientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
			{
				Log.Out("[EOS-ACCP2P] Auth State Change for Server : " + this.clientAuthStatus.ToStringCached<AntiCheatCommonClientAuthStatus>());
				Action action = this.onRemoteAuthComplete;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x0400A61D RID: 42525
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A61E RID: 42526
		[PublicizedFrom(EAccessModifier.Private)]
		public ConnectInterface connectInterface;

		// Token: 0x0400A61F RID: 42527
		[PublicizedFrom(EAccessModifier.Private)]
		public AntiCheatClientInterface antiCheatInterface;

		// Token: 0x0400A620 RID: 42528
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handleMessageToPeerID;

		// Token: 0x0400A621 RID: 42529
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handlePeerAuthStateChangeID;

		// Token: 0x0400A622 RID: 42530
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handlePeerActionRequiredID;

		// Token: 0x0400A623 RID: 42531
		[PublicizedFrom(EAccessModifier.Private)]
		public IntPtr serverHandle = new IntPtr(int.MaxValue);

		// Token: 0x0400A624 RID: 42532
		[PublicizedFrom(EAccessModifier.Private)]
		public ClientInfo.EDeviceType serverDeviceType = ClientInfo.EDeviceType.Unknown;

		// Token: 0x0400A625 RID: 42533
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object lockObject = new object();

		// Token: 0x0400A626 RID: 42534
		[PublicizedFrom(EAccessModifier.Private)]
		public AntiCheatCommonClientAuthStatus clientAuthStatus;

		// Token: 0x0400A627 RID: 42535
		[PublicizedFrom(EAccessModifier.Private)]
		public Action onRemoteAuthComplete;
	}
}
