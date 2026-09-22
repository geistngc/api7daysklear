using System;

namespace Platform
{
	// Token: 0x02001BB9 RID: 7097
	public interface IUserDataRoaming
	{
		// Token: 0x0600D385 RID: 54149
		void Init(IPlatform platform);

		// Token: 0x17001A24 RID: 6692
		// (get) Token: 0x0600D386 RID: 54150
		bool IsSupported { get; }

		// Token: 0x17001A25 RID: 6693
		// (get) Token: 0x0600D387 RID: 54151
		SaveRoamingMode SaveRoamingMode { get; }

		// Token: 0x17001A26 RID: 6694
		// (get) Token: 0x0600D388 RID: 54152
		bool SaveRoamingEnabled { get; }

		// Token: 0x17001A27 RID: 6695
		// (get) Token: 0x0600D389 RID: 54153
		bool IsRoamingOptional { get; }

		// Token: 0x17001A28 RID: 6696
		// (get) Token: 0x0600D38A RID: 54154
		UserDataStorageType DefaultSaveStorage { get; }

		// Token: 0x0600D38B RID: 54155
		void ValidateRoamingMode();
	}
}
