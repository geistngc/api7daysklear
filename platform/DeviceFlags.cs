using System;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x02001B46 RID: 6982
	public static class DeviceFlags
	{
		// Token: 0x0600D142 RID: 53570 RVA: 0x0003CD33 File Offset: 0x0003AF33
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsCurrent(this DeviceFlag flags)
		{
			return (flags & DeviceFlag.StandaloneWindows) > DeviceFlag.None;
		}

		// Token: 0x0400A073 RID: 41075
		public const DeviceFlag Standalone = DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX;

		// Token: 0x0400A074 RID: 41076
		public const DeviceFlag XBoxSeries = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX;

		// Token: 0x0400A075 RID: 41077
		public const DeviceFlag PS5 = DeviceFlag.PS5;

		// Token: 0x0400A076 RID: 41078
		public const DeviceFlag Console = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

		// Token: 0x0400A077 RID: 41079
		public const DeviceFlag All = DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

		// Token: 0x0400A078 RID: 41080
		public const DeviceFlag None = DeviceFlag.None;

		// Token: 0x0400A079 RID: 41081
		public const DeviceFlag Current = DeviceFlag.StandaloneWindows;
	}
}
