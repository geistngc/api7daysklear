using System;

namespace Platform
{
	// Token: 0x02001BED RID: 7149
	public interface IPlatformUserBlockedData
	{
		// Token: 0x17001A60 RID: 6752
		// (get) Token: 0x0600D4A7 RID: 54439
		EBlockType Type { get; }

		// Token: 0x17001A61 RID: 6753
		// (get) Token: 0x0600D4A8 RID: 54440
		EUserBlockState State { get; }

		// Token: 0x17001A62 RID: 6754
		// (get) Token: 0x0600D4A9 RID: 54441
		// (set) Token: 0x0600D4AA RID: 54442
		bool Locally { get; set; }
	}
}
