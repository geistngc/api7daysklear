using System;

namespace Platform.Shared
{
	// Token: 0x02001CBA RID: 7354
	public class UserDataRoaming : UserDataRoamingAbs
	{
		// Token: 0x0600DA2E RID: 55854 RVA: 0x004E6003 File Offset: 0x004E4203
		[PublicizedFrom(EAccessModifier.Private)]
		public UserDataRoaming(SaveRoamingMode mode, UserDataStorageType storageType)
		{
			this.mode = mode;
			this.storageType = storageType;
		}

		// Token: 0x17001B19 RID: 6937
		// (get) Token: 0x0600DA2F RID: 55855 RVA: 0x004E6019 File Offset: 0x004E4219
		public override SaveRoamingMode SaveRoamingMode
		{
			get
			{
				return this.mode;
			}
		}

		// Token: 0x17001B1A RID: 6938
		// (get) Token: 0x0600DA30 RID: 55856 RVA: 0x004E6021 File Offset: 0x004E4221
		public override UserDataStorageType DefaultSaveStorage
		{
			get
			{
				return this.storageType;
			}
		}

		// Token: 0x17001B1B RID: 6939
		// (get) Token: 0x0600DA31 RID: 55857 RVA: 0x004E6029 File Offset: 0x004E4229
		public static UserDataRoaming OptionalSaveRoaming
		{
			get
			{
				return new UserDataRoaming(SaveRoamingMode.Optional, UserDataStorageType.Roaming);
			}
		}

		// Token: 0x17001B1C RID: 6940
		// (get) Token: 0x0600DA32 RID: 55858 RVA: 0x004E6032 File Offset: 0x004E4232
		public static UserDataRoaming ForcedSaveRoaming
		{
			get
			{
				return new UserDataRoaming(SaveRoamingMode.Forced, UserDataStorageType.Roaming);
			}
		}

		// Token: 0x17001B1D RID: 6941
		// (get) Token: 0x0600DA33 RID: 55859 RVA: 0x004E603B File Offset: 0x004E423B
		public static UserDataRoaming NoSaveRoaming
		{
			get
			{
				return new UserDataRoaming(SaveRoamingMode.None, UserDataStorageType.DeviceLocal);
			}
		}

		// Token: 0x0400A5AC RID: 42412
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveRoamingMode mode;

		// Token: 0x0400A5AD RID: 42413
		[PublicizedFrom(EAccessModifier.Private)]
		public UserDataStorageType storageType;
	}
}
