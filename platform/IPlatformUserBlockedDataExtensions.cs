using System;

namespace Platform
{
	// Token: 0x02001BEE RID: 7150
	public static class IPlatformUserBlockedDataExtensions
	{
		// Token: 0x0600D4AB RID: 54443 RVA: 0x004CE957 File Offset: 0x004CCB57
		public static bool IsBlocked(this IPlatformUserBlockedData blockedData)
		{
			return blockedData.State.IsBlocked();
		}
	}
}
