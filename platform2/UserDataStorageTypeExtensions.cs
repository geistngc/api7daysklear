using System;

namespace Platform
{
	// Token: 0x02001BB8 RID: 7096
	public static class UserDataStorageTypeExtensions
	{
		// Token: 0x0600D383 RID: 54147 RVA: 0x004CB0B4 File Offset: 0x004C92B4
		public static bool UsesDataLimit(this UserDataStorageType storage)
		{
			return storage == UserDataStorageType.Roaming;
		}

		// Token: 0x0600D384 RID: 54148 RVA: 0x004CB0BA File Offset: 0x004C92BA
		public static string LocalizedName(this UserDataStorageType storage)
		{
			return Localization.Get("xuiStorageTypeOptions" + storage.ToStringCached<UserDataStorageType>(), false, null);
		}
	}
}
