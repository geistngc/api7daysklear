using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Epic.OnlineServices;
using Epic.OnlineServices.Connect;

namespace Platform.EOS
{
	// Token: 0x02001D29 RID: 7465
	public abstract class UserBase : IUserClient
	{
		// Token: 0x17001B8E RID: 7054
		// (get) Token: 0x0600DD19 RID: 56601 RVA: 0x004F4FD4 File Offset: 0x004F31D4
		public ConnectInterface ConnectInterface
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return ((Api)this.Owner.Api).ConnectInterface;
			}
		}

		// Token: 0x0600DD1A RID: 56602 RVA: 0x004F4FEB File Offset: 0x004F31EB
		public virtual void Init(IPlatform _owner)
		{
			this.Owner = _owner;
			this.Owner.Api.ClientApiInitialized += this.apiInitialized;
		}

		// Token: 0x17001B8F RID: 7055
		// (get) Token: 0x0600DD1B RID: 56603 RVA: 0x004F5010 File Offset: 0x004F3210
		// (set) Token: 0x0600DD1C RID: 56604 RVA: 0x004F5018 File Offset: 0x004F3218
		public EUserStatus UserStatus { get; [PublicizedFrom(EAccessModifier.Protected)] set; } = EUserStatus.NotAttempted;

		// Token: 0x14000142 RID: 322
		// (add) Token: 0x0600DD1D RID: 56605 RVA: 0x004F5024 File Offset: 0x004F3224
		// (remove) Token: 0x0600DD1E RID: 56606 RVA: 0x004F5084 File Offset: 0x004F3284
		public event Action<IPlatform> UserLoggedIn
		{
			add
			{
				lock (this)
				{
					this.userLoggedIn = (Action<IPlatform>)Delegate.Combine(this.userLoggedIn, value);
					if (this.UserStatus == EUserStatus.LoggedIn)
					{
						value(this.Owner);
					}
				}
			}
			remove
			{
				lock (this)
				{
					this.userLoggedIn = (Action<IPlatform>)Delegate.Remove(this.userLoggedIn, value);
				}
			}
		}

		// Token: 0x14000143 RID: 323
		// (add) Token: 0x0600DD1F RID: 56607 RVA: 0x000027FC File Offset: 0x000009FC
		// (remove) Token: 0x0600DD20 RID: 56608 RVA: 0x000027FC File Offset: 0x000009FC
		public event UserBlocksChangedCallback UserBlocksChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		// Token: 0x17001B90 RID: 7056
		// (get) Token: 0x0600DD21 RID: 56609 RVA: 0x004F50D0 File Offset: 0x004F32D0
		public PlatformUserIdentifierAbs PlatformUserId
		{
			get
			{
				return this.PlatformUserIdEos;
			}
		}

		// Token: 0x0600DD22 RID: 56610 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void Login(LoginUserCallback _delegate)
		{
		}

		// Token: 0x0600DD23 RID: 56611 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void PlayOffline(LoginUserCallback _delegate)
		{
		}

		// Token: 0x0600DD24 RID: 56612 RVA: 0x000027FC File Offset: 0x000009FC
		public void StartAdvertisePlaying(GameServerInfo _serverInfo)
		{
		}

		// Token: 0x0600DD25 RID: 56613 RVA: 0x000027FC File Offset: 0x000009FC
		public void StopAdvertisePlaying()
		{
		}

		// Token: 0x0600DD26 RID: 56614 RVA: 0x000880CC File Offset: 0x000862CC
		public void GetLoginTicket(Action<bool, byte[], string> _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600DD27 RID: 56615 RVA: 0x000880CC File Offset: 0x000862CC
		public string GetFriendName(PlatformUserIdentifierAbs _playerId)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600DD28 RID: 56616 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsFriend(PlatformUserIdentifierAbs _playerId)
		{
			return false;
		}

		// Token: 0x17001B91 RID: 7057
		// (get) Token: 0x0600DD29 RID: 56617
		public abstract EUserPerms Permissions { get; }

		// Token: 0x0600DD2A RID: 56618 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual string GetPermissionDenyReason(EUserPerms _perms)
		{
			return null;
		}

		// Token: 0x0600DD2B RID: 56619 RVA: 0x004F50D8 File Offset: 0x004F32D8
		public virtual IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
		{
			yield break;
		}

		// Token: 0x0600DD2C RID: 56620 RVA: 0x004E45F0 File Offset: 0x004E27F0
		public IEnumerator ResolveUserBlocks(IReadOnlyList<IPlatformUserBlockedResults> _results)
		{
			return Enumerable.Empty<object>().GetEnumerator();
		}

		// Token: 0x0600DD2D RID: 56621 RVA: 0x004F50E0 File Offset: 0x004F32E0
		public virtual void Destroy()
		{
			EosHelpers.AssertMainThread("Usr.Destroy");
			this.removeNotifications();
		}

		// Token: 0x0600DD2E RID: 56622 RVA: 0x004F50F2 File Offset: 0x004F32F2
		[PublicizedFrom(EAccessModifier.Private)]
		public void apiInitialized()
		{
			EosHelpers.AssertMainThread("Usr.Init");
			this.addNotifications();
		}

		// Token: 0x0600DD2F RID: 56623 RVA: 0x004F5104 File Offset: 0x004F3304
		[PublicizedFrom(EAccessModifier.Private)]
		public void addNotifications()
		{
			if (this.ConnectInterface == null)
			{
				return;
			}
			EosHelpers.AssertMainThread("Usr.AddNtfs");
			AddNotifyAuthExpirationOptions addNotifyAuthExpirationOptions = default(AddNotifyAuthExpirationOptions);
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.notifyAuthExpirationHandle = this.ConnectInterface.AddNotifyAuthExpiration(ref addNotifyAuthExpirationOptions, null, new OnAuthExpirationCallback(this.OnAuthExpiration));
			}
		}

		// Token: 0x0600DD30 RID: 56624 RVA: 0x004F5180 File Offset: 0x004F3380
		[PublicizedFrom(EAccessModifier.Private)]
		public void removeNotifications()
		{
			if (this.ConnectInterface == null)
			{
				return;
			}
			EosHelpers.AssertMainThread("Usr.RemNtfs");
			if (this.notifyAuthExpirationHandle != 0UL)
			{
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.ConnectInterface.RemoveNotifyAuthExpiration(this.notifyAuthExpirationHandle);
				}
				this.notifyAuthExpirationHandle = 0UL;
			}
		}

		// Token: 0x0600DD31 RID: 56625 RVA: 0x004F51F4 File Offset: 0x004F33F4
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnAuthExpiration(ref AuthExpirationCallbackInfo _data)
		{
			Log.Out("[EOS] Refreshing Login");
			this.startLogin(null, true);
		}

		// Token: 0x0600DD32 RID: 56626
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract void startLogin(LoginUserCallback _delegate, bool _refreshing = false);

		// Token: 0x0600DD33 RID: 56627 RVA: 0x004F5208 File Offset: 0x004F3408
		[PublicizedFrom(EAccessModifier.Protected)]
		public void connectLogin(byte[] _byteTicket, string _stringTicket, ExternalCredentialType _externalType, LoginUserCallback _callback, bool _refreshing, string _displayName = null)
		{
			EosHelpers.AssertMainThread("Usr.Log");
			Utf8String token = (_byteTicket != null) ? Common.ToString(new ArraySegment<byte>(_byteTicket)) : ((_stringTicket != null) ? new Utf8String(_stringTicket) : null);
			LoginOptions loginOptions = new LoginOptions
			{
				Credentials = new Credentials?(new Credentials
				{
					Token = token,
					Type = _externalType
				}),
				UserLoginInfo = null
			};
			if (_displayName != null)
			{
				loginOptions.UserLoginInfo = new UserLoginInfo?(new UserLoginInfo
				{
					DisplayName = _displayName
				});
			}
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.ConnectInterface.Login(ref loginOptions, null, delegate(ref LoginCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode == Result.Success)
					{
						if (!_refreshing)
						{
							string str = "[EOS] Login succeeded, PUID: ";
							ProductUserId localUserId = _callbackData.LocalUserId;
							Log.Out(str + ((localUserId != null) ? localUserId.ToString() : null));
							this.eosLoggedIn(_callbackData.LocalUserId, _callback);
							return;
						}
						Log.Out("[EOS] Login refreshed");
						return;
					}
					else if (Common.IsOperationComplete(_callbackData.ResultCode))
					{
						if (_callbackData.ResultCode == Result.InvalidUser)
						{
							if (!_refreshing)
							{
								this.connectCreateUser(_callbackData.ContinuanceToken, _callback);
								return;
							}
							Log.Error("[EOS] Login refresh failed, invalid user");
							return;
						}
						else
						{
							Log.Warning(string.Format("[EOS] Login {0}failed: {1}", _refreshing ? "refresh " : "", _callbackData.ResultCode));
							Result resultCode = _callbackData.ResultCode;
							if (resultCode != Result.UnrecognizedResponse)
							{
								if (resultCode != Result.ConnectExternalServiceUnavailable)
								{
									if (resultCode != Result.UnexpectedError)
									{
										this.UserStatus = EUserStatus.TemporaryError;
										LoginUserCallback callback = _callback;
										if (callback == null)
										{
											return;
										}
										callback(this.Owner, EApiStatusReason.Unknown, _callbackData.ResultCode.ToStringCached<Result>());
										return;
									}
									else
									{
										this.UserStatus = EUserStatus.OfflineMode;
										LoginUserCallback callback2 = _callback;
										if (callback2 == null)
										{
											return;
										}
										callback2(this.Owner, EApiStatusReason.Unknown, _callbackData.ResultCode.ToStringCached<Result>());
										return;
									}
								}
								else
								{
									this.UserStatus = EUserStatus.OfflineMode;
									LoginUserCallback callback3 = _callback;
									if (callback3 == null)
									{
										return;
									}
									callback3(this.Owner, EApiStatusReason.ExternalAuthUnavailable, PlatformManager.NativePlatform.PlatformDisplayName);
									return;
								}
							}
							else
							{
								this.UserStatus = EUserStatus.OfflineMode;
								LoginUserCallback callback4 = _callback;
								if (callback4 == null)
								{
									return;
								}
								callback4(this.Owner, EApiStatusReason.Unknown, _callbackData.ResultCode.ToStringCached<Result>());
								return;
							}
						}
					}
					else
					{
						Log.Error("[EOS] Login " + (_refreshing ? "refresh " : "") + "error: " + _callbackData.ResultCode.ToString());
						this.UserStatus = EUserStatus.PermanentError;
						LoginUserCallback callback5 = _callback;
						if (callback5 == null)
						{
							return;
						}
						callback5(this.Owner, EApiStatusReason.Unknown, _callbackData.ResultCode.ToStringCached<Result>());
						return;
					}
				});
			}
		}

		// Token: 0x0600DD34 RID: 56628
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract void connectCreateUser(ContinuanceToken _continuanceToken, LoginUserCallback _callback);

		// Token: 0x0600DD35 RID: 56629 RVA: 0x004F5310 File Offset: 0x004F3510
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void eosLoggedIn(ProductUserId _puid, LoginUserCallback _callback)
		{
			this.PlatformUserIdEos = new UserIdentifierEos(_puid);
			if (EosHelpers.UserAccountState != EUserAccountState.NewUser)
			{
				EosHelpers.UserAccountState = EUserAccountState.ReturningUser;
			}
		}

		// Token: 0x0600DD36 RID: 56630 RVA: 0x004F532C File Offset: 0x004F352C
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void eosLoginDone(LoginUserCallback _callback)
		{
			this.UserStatus = EUserStatus.LoggedIn;
			Action<IPlatform> action = this.userLoggedIn;
			if (action != null)
			{
				action(this.Owner);
			}
			if (_callback != null)
			{
				_callback(this.Owner, EApiStatusReason.Ok, null);
			}
		}

		// Token: 0x0600DD37 RID: 56631 RVA: 0x004F535E File Offset: 0x004F355E
		[PublicizedFrom(EAccessModifier.Protected)]
		public UserBase()
		{
		}

		// Token: 0x0400A769 RID: 42857
		[PublicizedFrom(EAccessModifier.Protected)]
		public IPlatform Owner;

		// Token: 0x0400A76A RID: 42858
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong notifyAuthExpirationHandle;

		// Token: 0x0400A76C RID: 42860
		[PublicizedFrom(EAccessModifier.Protected)]
		public Action<IPlatform> userLoggedIn;

		// Token: 0x0400A76D RID: 42861
		[PublicizedFrom(EAccessModifier.Protected)]
		public UserIdentifierEos PlatformUserIdEos;
	}
}
