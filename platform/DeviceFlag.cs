using System;

namespace Platform
{
	// Token: 0x02001B45 RID: 6981
	[Flags]
	public enum DeviceFlag
	{
		// Token: 0x0400A06C RID: 41068
		None = 0,
		// Token: 0x0400A06D RID: 41069
		StandaloneWindows = 1,
		// Token: 0x0400A06E RID: 41070
		StandaloneLinux = 2,
		// Token: 0x0400A06F RID: 41071
		StandaloneOSX = 4,
		// Token: 0x0400A070 RID: 41072
		XBoxSeriesS = 8,
		// Token: 0x0400A071 RID: 41073
		XBoxSeriesX = 16,
		// Token: 0x0400A072 RID: 41074
		PS5 = 32
	}
}
