using System;
using Epic.OnlineServices;
using Epic.OnlineServices.Connect;

namespace Platform.EOS
{
	// Token: 0x02001D30 RID: 7472
	public class UserServer : UserBase
	{
		// Token: 0x17001B9B RID: 7067
		// (get) Token: 0x0600DD59 RID: 56665 RVA: 0x001B2E28 File Offset: 0x001B1028
		public override EUserPerms Permissions
		{
			get
			{
				return EUserPerms.All;
			}
		}

		// Token: 0x0600DD5A RID: 56666 RVA: 0x004F5A34 File Offset: 0x004F3C34
		public override void Login(LoginUserCallback _delegate)
		{
			if (base.UserStatus == EUserStatus.LoggedIn)
			{
				Log.Out("[EOS] Login already done.");
				this.eosLoginDone(_delegate);
				return;
			}
			Log.Out("[EOS] Login");
			EosHelpers.TestEosConnection(delegate(bool _success)
			{
				if (_success)
				{
					this.startLogin(_delegate, false);
					return;
				}
				this.UserStatus = EUserStatus.OfflineMode;
				_delegate(this.Owner, EApiStatusReason.Other, "No connection to EOS backend");
			});
		}

		// Token: 0x0600DD5B RID: 56667 RVA: 0x004F5A8F File Offset: 0x004F3C8F
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void startLogin(LoginUserCallback _delegate, bool _refreshing = false)
		{
			this.fetchDeviceId(_delegate, _refreshing);
		}

		// Token: 0x0600DD5C RID: 56668 RVA: 0x004F5A9C File Offset: 0x004F3C9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void fetchDeviceId(LoginUserCallback _delegate, bool _refreshing)
		{
			Log.Out(string.Format("[EOS] Fetching DeviceId from native platform, with delegate={0}, refreshing={1}", _delegate != null, _refreshing));
			CreateDeviceIdOptions createDeviceIdOptions = new CreateDeviceIdOptions
			{
				DeviceModel = "DedicatedServer"
			};
			base.ConnectInterface.CreateDeviceId(ref createDeviceIdOptions, null, delegate(ref CreateDeviceIdCallbackInfo _result)
			{
				if (_result.ResultCode == Result.Success || _result.ResultCode == Result.DuplicateNotAllowed)
				{
					this.connectLogin(null, null, ExternalCredentialType.DeviceidAccessToken, _delegate, _refreshing, "DedicatedServer");
					return;
				}
				Log.Error("[EOS] Failed creating DeviceId: " + _result.ResultCode.ToStringCached<Result>());
				this.UserStatus = EUserStatus.TemporaryError;
				LoginUserCallback @delegate = _delegate;
				if (@delegate == null)
				{
					return;
				}
				@delegate(this.Owner, EApiStatusReason.NoLoginTicket, null);
			});
		}

		// Token: 0x0600DD5D RID: 56669 RVA: 0x000880CC File Offset: 0x000862CC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void connectCreateUser(ContinuanceToken _continuanceToken, LoginUserCallback _callback)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600DD5E RID: 56670 RVA: 0x004F5B21 File Offset: 0x004F3D21
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void eosLoggedIn(ProductUserId _puid, LoginUserCallback _callback)
		{
			base.eosLoggedIn(_puid, _callback);
			this.eosLoginDone(_callback);
		}
	}
}
