using System;

namespace Platform
{
	// Token: 0x02001BC6 RID: 7110
	public class MappedAccountReverseRequest
	{
		// Token: 0x0600D3B4 RID: 54196 RVA: 0x004CB1EC File Offset: 0x004C93EC
		public MappedAccountReverseRequest(EPlatformIdentifier _platform, string _id)
		{
			this.Platform = _platform;
			this.Id = _id;
		}

		// Token: 0x0400A182 RID: 41346
		public readonly EPlatformIdentifier Platform;

		// Token: 0x0400A183 RID: 41347
		public readonly string Id;

		// Token: 0x0400A184 RID: 41348
		public MappedAccountQueryResult Result;

		// Token: 0x0400A185 RID: 41349
		public PlatformUserIdentifierAbs PlatformId;
	}
}
