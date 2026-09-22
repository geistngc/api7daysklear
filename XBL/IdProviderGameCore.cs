using System;
using Platform.EOS;
using Platform.Shared;

namespace Platform.XBL
{
	// Token: 0x02001BFA RID: 7162
	public class IdProviderGameCore
	{
		// Token: 0x17001A67 RID: 6759
		// (get) Token: 0x0600D4D0 RID: 54480 RVA: 0x004CF2D7 File Offset: 0x004CD4D7
		// (set) Token: 0x0600D4D1 RID: 54481 RVA: 0x004CF2DF File Offset: 0x004CD4DF
		public UserIdentifierXbl Id { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600D4D2 RID: 54482 RVA: 0x004CF2E8 File Offset: 0x004CD4E8
		public IdProviderGameCore(User _nativeUser, IUserClient _crossplatformUser)
		{
			this.m_nativeUser = _nativeUser;
			if (_crossplatformUser != null)
			{
				_crossplatformUser.UserLoggedIn += this.CrossplatformLogin;
				return;
			}
			this.m_nativeUser.UserLoggedIn += this.NativeLogin;
		}

		// Token: 0x0600D4D3 RID: 54483 RVA: 0x004CF324 File Offset: 0x004CD524
		[PublicizedFrom(EAccessModifier.Private)]
		public void NativeLogin(IPlatform _platform)
		{
			this.Id = new UserIdentifierXbl(this.m_nativeUser.LocalID.Value.ToString());
			Log.Out("[XBL] Initializing user id with Local ID");
			XblXuidMapper.SetXuid(this.Id, this.m_nativeUser.Xuid);
		}

		// Token: 0x0600D4D4 RID: 54484 RVA: 0x004CF374 File Offset: 0x004CD574
		[PublicizedFrom(EAccessModifier.Private)]
		public void CrossplatformLogin(IPlatform _platform)
		{
			PlatformUserIdentifierAbs nativePlatformUserId = ((User)_platform.User).NativePlatformUserId;
			if (nativePlatformUserId != null)
			{
				UserIdentifierXbl userIdentifierXbl = nativePlatformUserId as UserIdentifierXbl;
				if (userIdentifierXbl == null)
				{
					Log.Error(string.Format("[XBL] Got different native platform id from EOS: {0}", nativePlatformUserId.PlatformIdentifier));
					return;
				}
				this.Id = userIdentifierXbl;
				Log.Out("[XBL] Initializing user id with PXUID " + userIdentifierXbl.CombinedString);
				PlatformIdCache.SetCachedId(this.Id);
				XblXuidMapper.SetXuid(this.Id, this.m_nativeUser.Xuid);
			}
		}

		// Token: 0x0600D4D5 RID: 54485 RVA: 0x004CF3F8 File Offset: 0x004CD5F8
		public bool LoadOfflineId()
		{
			UserIdentifierXbl userIdentifierXbl;
			if (PlatformIdCache.TryGetCachedId<UserIdentifierXbl>(out userIdentifierXbl))
			{
				this.Id = userIdentifierXbl;
				Log.Out("[XBL] Retrieved offline user id: " + userIdentifierXbl.CombinedString);
				return true;
			}
			return false;
		}

		// Token: 0x0400A230 RID: 41520
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly User m_nativeUser;
	}
}
