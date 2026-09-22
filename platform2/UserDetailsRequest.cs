using System;

namespace Platform
{
	// Token: 0x02001BBC RID: 7100
	public class UserDetailsRequest
	{
		// Token: 0x0600D395 RID: 54165 RVA: 0x004CB1A5 File Offset: 0x004C93A5
		public UserDetailsRequest(PlatformUserIdentifierAbs id)
		{
			this.Id = id;
			this.NativePlatform = id.PlatformIdentifier;
		}

		// Token: 0x0600D396 RID: 54166 RVA: 0x004CB1C0 File Offset: 0x004C93C0
		public UserDetailsRequest(PlatformUserIdentifierAbs id, EPlatformIdentifier platform)
		{
			this.Id = id;
			this.NativePlatform = platform;
		}

		// Token: 0x0400A174 RID: 41332
		public readonly PlatformUserIdentifierAbs Id;

		// Token: 0x0400A175 RID: 41333
		public readonly EPlatformIdentifier NativePlatform;

		// Token: 0x0400A176 RID: 41334
		public PlatformUserDetails details;

		// Token: 0x0400A177 RID: 41335
		public bool IsSuccess;
	}
}
