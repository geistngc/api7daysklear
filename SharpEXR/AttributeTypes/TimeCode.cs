using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016EC RID: 5868
	public struct TimeCode
	{
		// Token: 0x0600B743 RID: 46915 RVA: 0x004420FF File Offset: 0x004402FF
		public TimeCode(uint timeAndFlags, uint userData)
		{
			this.TimeAndFlags = timeAndFlags;
			this.UserData = userData;
		}

		// Token: 0x0400893E RID: 35134
		public readonly uint TimeAndFlags;

		// Token: 0x0400893F RID: 35135
		public readonly uint UserData;
	}
}
