using System;

namespace Platform
{
	// Token: 0x02001BCA RID: 7114
	public class UserDataRoamingMultiPlatform : UserDataRoamingAbs
	{
		// Token: 0x17001A2E RID: 6702
		// (get) Token: 0x0600D3C7 RID: 54215 RVA: 0x004CB202 File Offset: 0x004C9402
		public override SaveRoamingMode SaveRoamingMode
		{
			get
			{
				if (GameManager.IsDedicatedServer)
				{
					return SaveRoamingMode.None;
				}
				IUserDataRoaming userDataRoaming = PlatformManager.NativePlatform.UserDataRoaming;
				if (userDataRoaming == null)
				{
					return SaveRoamingMode.None;
				}
				return userDataRoaming.SaveRoamingMode;
			}
		}

		// Token: 0x17001A2F RID: 6703
		// (get) Token: 0x0600D3C8 RID: 54216 RVA: 0x004CB222 File Offset: 0x004C9422
		public override UserDataStorageType DefaultSaveStorage
		{
			get
			{
				if (GameManager.IsDedicatedServer)
				{
					return UserDataStorageType.DeviceLocal;
				}
				IUserDataRoaming userDataRoaming = PlatformManager.NativePlatform.UserDataRoaming;
				if (userDataRoaming == null)
				{
					return UserDataStorageType.DeviceLocal;
				}
				return userDataRoaming.DefaultSaveStorage;
			}
		}
	}
}
