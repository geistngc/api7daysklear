using System;
using System.Collections.Generic;
using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.Sessions;
using UnityEngine;

namespace Platform.EOS
{
	// Token: 0x02001D17 RID: 7447
	public class SessionsHost : IMasterServerAnnouncer
	{
		// Token: 0x0600DCB7 RID: 56503 RVA: 0x004F29E8 File Offset: 0x004F0BE8
		public static string GetMatchmakingGroupTag(EMatchmakingGroup _matchmakingGroup)
		{
			if (_matchmakingGroup == EMatchmakingGroup.CertQA)
			{
				return "CertQA";
			}
			return "<WeDontCare>";
		}

		// Token: 0x0600DCB8 RID: 56504 RVA: 0x004F29F9 File Offset: 0x004F0BF9
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += this.apiInitialized;
		}

		// Token: 0x0600DCB9 RID: 56505 RVA: 0x004F2A20 File Offset: 0x004F0C20
		public void Update()
		{
			if (!this.GameServerInitialized)
			{
				if (this.updatesSessionModification != null)
				{
					object lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.updatesSessionModification.Release();
					}
					this.updatesSessionModification = null;
				}
				return;
			}
			if (this.commitBackendCountdown.HasPassed())
			{
				this.commitBackendCountdown.Reset();
				this.commitBackendCountdown.SetTimeout(30f);
				if (this.updatesSessionModification != null)
				{
					this.commitSessionToBackend(false, this.updatesSessionModification);
					this.updatesSessionModification = null;
				}
			}
		}

		// Token: 0x0600DCBA RID: 56506 RVA: 0x004F2ACC File Offset: 0x004F0CCC
		[PublicizedFrom(EAccessModifier.Private)]
		public void apiInitialized()
		{
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.sessionsInterface = ((Api)this.owner.Api).PlatformInterface.GetSessionsInterface();
			}
		}

		// Token: 0x17001B85 RID: 7045
		// (get) Token: 0x0600DCBB RID: 56507 RVA: 0x004F2B28 File Offset: 0x004F0D28
		public bool GameServerInitialized
		{
			get
			{
				return this.sessionId != null;
			}
		}

		// Token: 0x0600DCBC RID: 56508 RVA: 0x0004E558 File Offset: 0x0004C758
		public string GetServerPorts()
		{
			return string.Empty;
		}

		// Token: 0x0600DCBD RID: 56509 RVA: 0x004F2B34 File Offset: 0x004F0D34
		[PublicizedFrom(EAccessModifier.Private)]
		public string GetBucketId()
		{
			if (!GameManager.IsDedicatedServer)
			{
				return SessionsHost.GetMatchmakingGroupTag(PlatformManager.MultiPlatform.User.GetMatchmakingGroup());
			}
			string @string = GamePrefs.GetString(EnumGamePrefs.ServerMatchmakingGroup);
			if (!string.IsNullOrEmpty(@string))
			{
				Log.Out("[EOS] using GamePref matchmaking group: " + @string);
				return @string;
			}
			return "<WeDontCare>";
		}

		// Token: 0x0600DCBE RID: 56510 RVA: 0x004F2B88 File Offset: 0x004F0D88
		public void AdvertiseServer(Action _onServerRegistered)
		{
			Log.Out("[EOS] Registering server");
			EosHelpers.AssertMainThread("SeHo.Adv");
			IUserClient user = this.owner.User;
			PlatformUserIdentifierAbs platformUserIdentifierAbs;
			if ((platformUserIdentifierAbs = ((user != null) ? user.PlatformUserId : null)) == null)
			{
				IUserClient userServer = this.owner.UserServer;
				platformUserIdentifierAbs = ((userServer != null) ? userServer.PlatformUserId : null);
			}
			UserIdentifierEos userIdentifierEos = (UserIdentifierEos)platformUserIdentifierAbs;
			if (this.sessionsInterface == null)
			{
				if (_onServerRegistered != null)
				{
					_onServerRegistered();
				}
				return;
			}
			GameServerInfo localServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo;
			localServerInfo.SetValue(GameInfoString.CombinedPrimaryId, (userIdentifierEos != null) ? userIdentifierEos.CombinedString : null);
			GameServerInfo gameServerInfo = localServerInfo;
			GameInfoString key = GameInfoString.CombinedNativeId;
			IUserClient user2 = PlatformManager.NativePlatform.User;
			string value;
			if (user2 == null)
			{
				value = null;
			}
			else
			{
				PlatformUserIdentifierAbs platformUserId = user2.PlatformUserId;
				value = ((platformUserId != null) ? platformUserId.CombinedString : null);
			}
			gameServerInfo.SetValue(key, value);
			string bucketId = this.GetBucketId();
			CreateSessionModificationOptions createSessionModificationOptions = new CreateSessionModificationOptions
			{
				SessionName = "GameHost",
				BucketId = bucketId,
				MaxPlayers = (uint)localServerInfo.GetValue(GameInfoInt.MaxPlayers),
				LocalUserId = ((userIdentifierEos != null) ? userIdentifierEos.ProductUserId : null),
				PresenceEnabled = false,
				SanctionsEnabled = this.owner.AntiCheatServer.ServerEacEnabled(),
				AllowedPlatformIds = EPlayGroupExtensions.GetCurrentlyAllowedPlatformIds()
			};
			object lockObject = AntiCheatCommon.LockObject;
			SessionModification sessionModification;
			Result result;
			lock (lockObject)
			{
				result = this.sessionsInterface.CreateSessionModification(ref createSessionModificationOptions, out sessionModification);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] Failed creating session modification: " + result.ToStringCached<Result>());
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					if (sessionModification != null)
					{
						sessionModification.Release();
					}
				}
				if (_onServerRegistered != null)
				{
					_onServerRegistered();
				}
				return;
			}
			SessionModificationSetPermissionLevelOptions sessionModificationSetPermissionLevelOptions = default(SessionModificationSetPermissionLevelOptions);
			int value2 = localServerInfo.GetValue(GameInfoInt.ServerVisibility);
			OnlineSessionPermissionLevel permissionLevel;
			if (value2 != 1)
			{
				if (value2 == 2)
				{
					permissionLevel = OnlineSessionPermissionLevel.PublicAdvertised;
				}
				else
				{
					permissionLevel = OnlineSessionPermissionLevel.JoinViaPresence;
				}
			}
			else
			{
				permissionLevel = OnlineSessionPermissionLevel.JoinViaPresence;
			}
			sessionModificationSetPermissionLevelOptions.PermissionLevel = permissionLevel;
			SessionModificationSetPermissionLevelOptions sessionModificationSetPermissionLevelOptions2 = sessionModificationSetPermissionLevelOptions;
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				result = sessionModification.SetPermissionLevel(ref sessionModificationSetPermissionLevelOptions2);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] Failed setting permission level: " + result.ToStringCached<Result>());
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					sessionModification.Release();
				}
				if (_onServerRegistered != null)
				{
					_onServerRegistered();
				}
				return;
			}
			SessionModificationSetJoinInProgressAllowedOptions sessionModificationSetJoinInProgressAllowedOptions = new SessionModificationSetJoinInProgressAllowedOptions
			{
				AllowJoinInProgress = true
			};
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				result = sessionModification.SetJoinInProgressAllowed(ref sessionModificationSetJoinInProgressAllowedOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] Failed setting join in progress: " + result.ToStringCached<Result>());
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					sessionModification.Release();
				}
				if (_onServerRegistered != null)
				{
					_onServerRegistered();
				}
				return;
			}
			SessionModificationSetInvitesAllowedOptions sessionModificationSetInvitesAllowedOptions = new SessionModificationSetInvitesAllowedOptions
			{
				InvitesAllowed = false
			};
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				result = sessionModification.SetInvitesAllowed(ref sessionModificationSetInvitesAllowedOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] Failed setting invites allowed: " + result.ToStringCached<Result>());
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					sessionModification.Release();
				}
				if (_onServerRegistered != null)
				{
					_onServerRegistered();
				}
				return;
			}
			if (!this.setBaseAttributes(sessionModification, localServerInfo))
			{
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					sessionModification.Release();
				}
				if (_onServerRegistered != null)
				{
					_onServerRegistered();
				}
				return;
			}
			this.onServerRegistered = _onServerRegistered;
			this.commitSessionToBackend(true, sessionModification);
		}

		// Token: 0x0600DCBF RID: 56511 RVA: 0x004F2FC8 File Offset: 0x004F11C8
		[PublicizedFrom(EAccessModifier.Private)]
		public void sessionRegisteredCallback(ref UpdateSessionCallbackInfo _callbackData)
		{
			if (this.onServerRegistered == null)
			{
				return;
			}
			if (_callbackData.ResultCode != Result.Success)
			{
				Log.Error("[EOS] Failed registering session on backend: " + _callbackData.ResultCode.ToStringCached<Result>());
				Log.Warning(string.Format("[EOS] Attribute count: {0}", this.registeredAttributes.Count));
				Action action = this.onServerRegistered;
				if (action != null)
				{
					action();
				}
				this.onServerRegistered = null;
				return;
			}
			this.sessionId = _callbackData.SessionId;
			Log.Out(string.Format("[EOS] Server registered, session: {0}, {1} attributes", this.sessionId, this.registeredAttributes.Count));
			GameServerInfo localServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo;
			localServerInfo.OnChangedString += this.updateSessionString;
			localServerInfo.OnChangedInt += this.updateSessionInt;
			localServerInfo.OnChangedBool += this.updateSessionBool;
			localServerInfo.SetValue(GameInfoString.IP, this.getPublicIpFromHostedSession());
			localServerInfo.SetValue(GameInfoString.UniqueId, this.sessionId);
			Action action2 = this.onServerRegistered;
			if (action2 != null)
			{
				action2();
			}
			this.onServerRegistered = null;
		}

		// Token: 0x0600DCC0 RID: 56512 RVA: 0x004F30E0 File Offset: 0x004F12E0
		[PublicizedFrom(EAccessModifier.Private)]
		public string getPublicIpFromHostedSession()
		{
			CopyActiveSessionHandleOptions copyActiveSessionHandleOptions = new CopyActiveSessionHandleOptions
			{
				SessionName = "GameHost"
			};
			object lockObject = AntiCheatCommon.LockObject;
			ActiveSession activeSession;
			Result result;
			lock (lockObject)
			{
				result = this.sessionsInterface.CopyActiveSessionHandle(ref copyActiveSessionHandleOptions, out activeSession);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] Failed getting active session: " + result.ToStringCached<Result>());
				return null;
			}
			ActiveSessionCopyInfoOptions activeSessionCopyInfoOptions = default(ActiveSessionCopyInfoOptions);
			lockObject = AntiCheatCommon.LockObject;
			ActiveSessionInfo? activeSessionInfo;
			lock (lockObject)
			{
				result = activeSession.CopyInfo(ref activeSessionCopyInfoOptions, out activeSessionInfo);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] Failed getting active session info: " + result.ToStringCached<Result>());
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					activeSession.Release();
				}
				return null;
			}
			string text = activeSessionInfo.Value.SessionDetails.Value.HostAddress;
			Log.Out("[EOS] Session address: " + Utils.MaskIp(text));
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				activeSession.Release();
			}
			return text;
		}

		// Token: 0x0600DCC1 RID: 56513 RVA: 0x004F3268 File Offset: 0x004F1468
		public void StopServer()
		{
			EosHelpers.AssertMainThread("SeHo.Stop");
			this.onServerRegistered = null;
			if (!this.GameServerInitialized)
			{
				return;
			}
			Log.Out("[EOS] Unregistering server");
			if (SingletonMonoBehaviour<ConnectionManager>.Instance != null && SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo != null)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedString -= this.updateSessionString;
				SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedInt -= this.updateSessionInt;
				SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedBool -= this.updateSessionBool;
			}
			this.registeredAttributes.Clear();
			DestroySessionOptions destroySessionOptions = new DestroySessionOptions
			{
				SessionName = "GameHost"
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.sessionsInterface.DestroySession(ref destroySessionOptions, null, delegate(ref DestroySessionCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode == Result.Success)
					{
						Log.Out("[EOS] Server unregistered");
						this.sessionId = null;
						return;
					}
					Log.Error("[EOS] Failed unregistering session on backend: " + _callbackData.ResultCode.ToStringCached<Result>());
				});
			}
		}

		// Token: 0x0600DCC2 RID: 56514 RVA: 0x004F3370 File Offset: 0x004F1570
		public void RegisterUser(ClientInfo _cInfo)
		{
			EosHelpers.AssertMainThread("SeHo.Reg");
			RegisterPlayersOptions registerPlayersOptions = new RegisterPlayersOptions
			{
				SessionName = "GameHost",
				PlayersToRegister = new ProductUserId[]
				{
					((UserIdentifierEos)_cInfo.CrossplatformId).ProductUserId
				}
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.sessionsInterface.RegisterPlayers(ref registerPlayersOptions, null, delegate(ref RegisterPlayersCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS] Failed registering player in session: " + _callbackData.ResultCode.ToStringCached<Result>());
						return;
					}
					if (_callbackData.SanctionedPlayers != null)
					{
						ProductUserId[] sanctionedPlayers = _callbackData.SanctionedPlayers;
						for (int i = 0; i < sanctionedPlayers.Length; i++)
						{
							if (sanctionedPlayers[i] == ((UserIdentifierEos)_cInfo.CrossplatformId).ProductUserId)
							{
								Log.Out("Player " + _cInfo.playerName + " has a sanction and cannot join the session, kicking player");
								GameUtils.KickPlayerForClientInfo(_cInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.CrossPlatformAuthenticationFailed, 9, default(DateTime), "Sanction"));
							}
						}
					}
				});
			}
		}

		// Token: 0x0600DCC3 RID: 56515 RVA: 0x004F341C File Offset: 0x004F161C
		public void UnregisterUser(ClientInfo _cInfo)
		{
			if (((_cInfo != null) ? _cInfo.CrossplatformId : null) == null)
			{
				return;
			}
			EosHelpers.AssertMainThread("SeHo.Free");
			UnregisterPlayersOptions unregisterPlayersOptions = new UnregisterPlayersOptions
			{
				SessionName = "GameHost",
				PlayersToUnregister = new ProductUserId[]
				{
					((UserIdentifierEos)_cInfo.CrossplatformId).ProductUserId
				}
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.sessionsInterface.UnregisterPlayers(ref unregisterPlayersOptions, null, delegate(ref UnregisterPlayersCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS] Failed unregistering player in session: " + _callbackData.ResultCode.ToStringCached<Result>());
						return;
					}
				});
			}
		}

		// Token: 0x0600DCC4 RID: 56516 RVA: 0x004F34D8 File Offset: 0x004F16D8
		[PublicizedFrom(EAccessModifier.Private)]
		public SessionModification getSessionModificationHandle()
		{
			UpdateSessionModificationOptions updateSessionModificationOptions = new UpdateSessionModificationOptions
			{
				SessionName = "GameHost"
			};
			object lockObject = AntiCheatCommon.LockObject;
			SessionModification sessionModification;
			Result result;
			lock (lockObject)
			{
				result = this.sessionsInterface.UpdateSessionModification(ref updateSessionModificationOptions, out sessionModification);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] Failed getting session modification: " + result.ToStringCached<Result>());
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					sessionModification.Release();
				}
				sessionModification = null;
			}
			return sessionModification;
		}

		// Token: 0x0600DCC5 RID: 56517 RVA: 0x004F3590 File Offset: 0x004F1790
		[PublicizedFrom(EAccessModifier.Private)]
		public bool addAttribute(SessionModification _sessionModificationHandle, string _key, string _value)
		{
			if (_value == null)
			{
				_value = "";
			}
			_value = _value + "~$#$~" + _value.ToLowerInvariant();
			return this.addAttributeInternal(_sessionModificationHandle, _key, new AttributeDataValue
			{
				AsUtf8 = _value
			}, _value);
		}

		// Token: 0x0600DCC6 RID: 56518 RVA: 0x004F35DC File Offset: 0x004F17DC
		[PublicizedFrom(EAccessModifier.Private)]
		public bool addAttribute(SessionModification _sessionModificationHandle, string _key, int _value)
		{
			return this.addAttributeInternal(_sessionModificationHandle, _key, new AttributeDataValue
			{
				AsInt64 = new long?((long)_value)
			}, _value.ToString());
		}

		// Token: 0x0600DCC7 RID: 56519 RVA: 0x004F3610 File Offset: 0x004F1810
		[PublicizedFrom(EAccessModifier.Private)]
		public bool addAttribute(SessionModification _sessionModificationHandle, string _key, bool _value)
		{
			return this.addAttributeInternal(_sessionModificationHandle, _key, new AttributeDataValue
			{
				AsBool = new bool?(_value)
			}, _value.ToString());
		}

		// Token: 0x0600DCC8 RID: 56520 RVA: 0x004F3644 File Offset: 0x004F1844
		[PublicizedFrom(EAccessModifier.Private)]
		public bool addBoolsAttribute(SessionModification _sessionModificationHandle, string _values)
		{
			return this.addAttributeInternal(_sessionModificationHandle, "-BoolValues-", new AttributeDataValue
			{
				AsUtf8 = _values
			}, _values);
		}

		// Token: 0x0600DCC9 RID: 56521 RVA: 0x004F3674 File Offset: 0x004F1874
		[PublicizedFrom(EAccessModifier.Private)]
		public bool addAttributeInternal(SessionModification _sessionModificationHandle, string _key, AttributeDataValue _value, string _valueString)
		{
			SessionModificationAddAttributeOptions sessionModificationAddAttributeOptions = new SessionModificationAddAttributeOptions
			{
				AdvertisementType = SessionAttributeAdvertisementType.Advertise,
				SessionAttribute = new AttributeData?(new AttributeData
				{
					Key = _key,
					Value = _value
				})
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = _sessionModificationHandle.AddAttribute(ref sessionModificationAddAttributeOptions);
			}
			if (result == Result.Success)
			{
				this.registeredAttributes.Add(_key);
				return true;
			}
			Log.Error(string.Format("[EOS] Failed setting {0}th attribute '{1}' to '{2}': {3}", new object[]
			{
				this.registeredAttributes.Count + 1,
				_key,
				_valueString,
				result.ToStringCached<Result>()
			}));
			return false;
		}

		// Token: 0x0600DCCA RID: 56522 RVA: 0x004F3748 File Offset: 0x004F1948
		[PublicizedFrom(EAccessModifier.Private)]
		public bool setBaseAttributes(SessionModification _sessionModificationHandle, GameServerInfo _gameServerInfo)
		{
			foreach (GameInfoInt gameInfoInt in GameServerInfo.IntInfosInGameTags)
			{
				if (!this.addAttribute(_sessionModificationHandle, gameInfoInt.ToStringCached<GameInfoInt>(), _gameServerInfo.GetValue(gameInfoInt)))
				{
					return false;
				}
			}
			if (!this.addBoolsAttribute(_sessionModificationHandle, this.getBoolsString(_gameServerInfo)))
			{
				return false;
			}
			foreach (GameInfoString gameInfoString in GameServerInfo.SearchableStringInfos)
			{
				if (!this.addAttribute(_sessionModificationHandle, gameInfoString.ToStringCached<GameInfoString>(), _gameServerInfo.GetValue(gameInfoString)))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600DCCB RID: 56523 RVA: 0x004F37CB File Offset: 0x004F19CB
		[PublicizedFrom(EAccessModifier.Private)]
		public SessionModification getUpdateSessionModification()
		{
			if (this.updatesSessionModification == null)
			{
				this.updatesSessionModification = this.getSessionModificationHandle();
			}
			return this.updatesSessionModification;
		}

		// Token: 0x0600DCCC RID: 56524 RVA: 0x004F37F0 File Offset: 0x004F19F0
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateSessionString(GameServerInfo _gameServerInfo, GameInfoString _gameInfoKey)
		{
			if (!this.GameServerInitialized)
			{
				return;
			}
			if (!GameServerInfo.IsSearchable(_gameInfoKey))
			{
				return;
			}
			if (!this.commitBackendCountdown.IsRunning)
			{
				this.commitBackendCountdown.ResetAndRestart();
			}
			if (_gameInfoKey.ToStringCached<GameInfoString>().EndsWith("ID", StringComparison.OrdinalIgnoreCase))
			{
				this.commitBackendCountdown.SetTimeout(5f);
			}
			this.addAttribute(this.getUpdateSessionModification(), _gameInfoKey.ToStringCached<GameInfoString>(), _gameServerInfo.GetValue(_gameInfoKey));
		}

		// Token: 0x0600DCCD RID: 56525 RVA: 0x004F3864 File Offset: 0x004F1A64
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateSessionInt(GameServerInfo _gameServerInfo, GameInfoInt _gameInfoKey)
		{
			if (!this.GameServerInitialized)
			{
				return;
			}
			if (!GameServerInfo.IsSearchable(_gameInfoKey))
			{
				return;
			}
			if (!this.commitBackendCountdown.IsRunning)
			{
				this.commitBackendCountdown.ResetAndRestart();
			}
			this.addAttribute(this.getUpdateSessionModification(), _gameInfoKey.ToStringCached<GameInfoInt>(), _gameServerInfo.GetValue(_gameInfoKey));
		}

		// Token: 0x0600DCCE RID: 56526 RVA: 0x004F38B5 File Offset: 0x004F1AB5
		[PublicizedFrom(EAccessModifier.Private)]
		public void updateSessionBool(GameServerInfo _gameServerInfo, GameInfoBool _gameInfoKey)
		{
			if (!this.GameServerInitialized)
			{
				return;
			}
			if (!GameServerInfo.IsSearchable(_gameInfoKey))
			{
				return;
			}
			if (!this.commitBackendCountdown.IsRunning)
			{
				this.commitBackendCountdown.ResetAndRestart();
			}
			this.addBoolsAttribute(this.getUpdateSessionModification(), this.getBoolsString(_gameServerInfo));
		}

		// Token: 0x0600DCCF RID: 56527 RVA: 0x004F38F8 File Offset: 0x004F1AF8
		[PublicizedFrom(EAccessModifier.Private)]
		public string getBoolsString(GameServerInfo _gameServerInfo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(',');
			foreach (GameInfoBool gameInfoBool in GameServerInfo.BoolInfosInGameTags)
			{
				stringBuilder.Append(gameInfoBool.ToStringCached<GameInfoBool>());
				stringBuilder.Append('=');
				stringBuilder.Append(_gameServerInfo.GetValue(gameInfoBool) ? '1' : '0');
				stringBuilder.Append(',');
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600DCD0 RID: 56528 RVA: 0x004F3968 File Offset: 0x004F1B68
		[PublicizedFrom(EAccessModifier.Private)]
		public void commitSessionToBackend(bool _initialRegistration, SessionModification _sessionModification)
		{
			UpdateSessionOptions updateSessionOptions = new UpdateSessionOptions
			{
				SessionModificationHandle = _sessionModification
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.sessionsInterface.UpdateSession(ref updateSessionOptions, new SessionsHost.SessionModificationCallbackArgs(_sessionModification, _initialRegistration, _initialRegistration ? new OnUpdateSessionCallback(this.sessionRegisteredCallback) : new OnUpdateSessionCallback(this.sessionUpdatedCallback)), new OnUpdateSessionCallback(this.commitSessionCallbackWrapper));
			}
		}

		// Token: 0x0600DCD1 RID: 56529 RVA: 0x004F39F0 File Offset: 0x004F1BF0
		[PublicizedFrom(EAccessModifier.Private)]
		public void commitSessionCallbackWrapper(ref UpdateSessionCallbackInfo _callbackData)
		{
			SessionsHost.SessionModificationCallbackArgs sessionModificationCallbackArgs = (SessionsHost.SessionModificationCallbackArgs)_callbackData.ClientData;
			if (_callbackData.ResultCode == Result.OperationWillRetry)
			{
				Log.Warning("[EOS] Failed updating session on backend, will retry");
				return;
			}
			sessionModificationCallbackArgs.SessionModification.Release();
			sessionModificationCallbackArgs.SessionModification = null;
			if (sessionModificationCallbackArgs.IsInitialRegistration || this.GameServerInitialized)
			{
				sessionModificationCallbackArgs.Callback(ref _callbackData);
			}
		}

		// Token: 0x0600DCD2 RID: 56530 RVA: 0x004F3A4C File Offset: 0x004F1C4C
		[PublicizedFrom(EAccessModifier.Private)]
		public void sessionUpdatedCallback(ref UpdateSessionCallbackInfo _callbackData)
		{
			if (_callbackData.ResultCode != Result.Success)
			{
				Log.Error("[EOS] Failed updating session on backend: " + _callbackData.ResultCode.ToStringCached<Result>() + ". From: " + StackTraceUtility.ExtractStackTrace());
				Log.Warning(string.Format("[EOS] Attribute count: {0}", this.registeredAttributes.Count));
				return;
			}
		}

		// Token: 0x0400A725 RID: 42789
		[PublicizedFrom(EAccessModifier.Private)]
		public const float sessionUpdateIntervalSecsDefault = 30f;

		// Token: 0x0400A726 RID: 42790
		[PublicizedFrom(EAccessModifier.Private)]
		public const float sessionUpdateIntervalSecsImportant = 5f;

		// Token: 0x0400A727 RID: 42791
		[PublicizedFrom(EAccessModifier.Private)]
		public const string sessionName = "GameHost";

		// Token: 0x0400A728 RID: 42792
		public const string DefaultMatchmakingGroupTag = "<WeDontCare>";

		// Token: 0x0400A729 RID: 42793
		public const string EmptyStringAttributeValue = "##EMPTY##";

		// Token: 0x0400A72A RID: 42794
		public const string LowerCaseAttributeSeparator = "~$#$~";

		// Token: 0x0400A72B RID: 42795
		public const string BoolsAttributeName = "-BoolValues-";

		// Token: 0x0400A72C RID: 42796
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A72D RID: 42797
		[PublicizedFrom(EAccessModifier.Private)]
		public SessionsInterface sessionsInterface;

		// Token: 0x0400A72E RID: 42798
		[PublicizedFrom(EAccessModifier.Private)]
		public string sessionId;

		// Token: 0x0400A72F RID: 42799
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CountdownTimer commitBackendCountdown = new CountdownTimer(30f, false);

		// Token: 0x0400A730 RID: 42800
		[PublicizedFrom(EAccessModifier.Private)]
		public SessionModification updatesSessionModification;

		// Token: 0x0400A731 RID: 42801
		[PublicizedFrom(EAccessModifier.Private)]
		public Action onServerRegistered;

		// Token: 0x0400A732 RID: 42802
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<string> registeredAttributes = new HashSet<string>();

		// Token: 0x02001D18 RID: 7448
		[PublicizedFrom(EAccessModifier.Private)]
		public class SessionModificationCallbackArgs
		{
			// Token: 0x0600DCD5 RID: 56533 RVA: 0x004F3AFF File Offset: 0x004F1CFF
			public SessionModificationCallbackArgs(SessionModification _sessionModification, bool _isInitialRegistration, OnUpdateSessionCallback _callback)
			{
				this.SessionModification = _sessionModification;
				this.IsInitialRegistration = _isInitialRegistration;
				this.Callback = _callback;
			}

			// Token: 0x0400A733 RID: 42803
			public SessionModification SessionModification;

			// Token: 0x0400A734 RID: 42804
			public readonly bool IsInitialRegistration;

			// Token: 0x0400A735 RID: 42805
			public readonly OnUpdateSessionCallback Callback;
		}
	}
}
