using System;

namespace Platform
{
	// Token: 0x02001BEB RID: 7147
	public interface IPlatformUser
	{
		// Token: 0x17001A5C RID: 6748
		// (get) Token: 0x0600D4A0 RID: 54432
		PlatformUserIdentifierAbs PrimaryId { get; }

		// Token: 0x17001A5D RID: 6749
		// (get) Token: 0x0600D4A1 RID: 54433
		// (set) Token: 0x0600D4A2 RID: 54434
		PlatformUserIdentifierAbs NativeId { get; set; }
	}
}
