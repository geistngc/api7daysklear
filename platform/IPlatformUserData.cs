using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001BEC RID: 7148
	public interface IPlatformUserData : IPlatformUser
	{
		// Token: 0x17001A5E RID: 6750
		// (get) Token: 0x0600D4A3 RID: 54435
		IReadOnlyDictionary<EBlockType, IPlatformUserBlockedData> Blocked { get; }

		// Token: 0x0600D4A4 RID: 54436
		void MarkBlockedStateChanged();

		// Token: 0x17001A5F RID: 6751
		// (get) Token: 0x0600D4A5 RID: 54437
		string Name { get; }

		// Token: 0x0600D4A6 RID: 54438
		void RequestUserDetailsUpdate();
	}
}
