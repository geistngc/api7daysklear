using System;

namespace Platform
{
	// Token: 0x02001BC3 RID: 7107
	public class MappedAccountRequest
	{
		// Token: 0x0600D3AB RID: 54187 RVA: 0x004CB1D6 File Offset: 0x004C93D6
		public MappedAccountRequest(PlatformUserIdentifierAbs _id, EPlatformIdentifier _platform)
		{
			this.Id = _id;
			this.Platform = _platform;
		}

		// Token: 0x0400A17D RID: 41341
		public readonly PlatformUserIdentifierAbs Id;

		// Token: 0x0400A17E RID: 41342
		public readonly EPlatformIdentifier Platform;

		// Token: 0x0400A17F RID: 41343
		public string MappedAccountId;

		// Token: 0x0400A180 RID: 41344
		public string DisplayName;

		// Token: 0x0400A181 RID: 41345
		public MappedAccountQueryResult Result;
	}
}
