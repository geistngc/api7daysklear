using System;

namespace Platform
{
	// Token: 0x02001B47 RID: 6983
	public static class DeviceName
	{
		// Token: 0x0600D143 RID: 53571 RVA: 0x004C8A9C File Offset: 0x004C6C9C
		public static string GetDeviceName(this DeviceFlag _deviceId)
		{
			if (_deviceId <= DeviceFlag.XBoxSeriesS)
			{
				switch (_deviceId)
				{
				case DeviceFlag.StandaloneWindows:
					return "Windows";
				case DeviceFlag.StandaloneLinux:
					return "Linux";
				case DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux:
					break;
				case DeviceFlag.StandaloneOSX:
					return "OSX";
				default:
					if (_deviceId == DeviceFlag.XBoxSeriesS)
					{
						return "XBoxSeriesS";
					}
					break;
				}
			}
			else
			{
				if (_deviceId == DeviceFlag.XBoxSeriesX)
				{
					return "XBoxSeriesX";
				}
				if (_deviceId == DeviceFlag.PS5)
				{
					return "PS5";
				}
			}
			Log.Warning(string.Format("Device name for flag '{0}' is unknown", _deviceId));
			return string.Empty;
		}

		// Token: 0x0400A07A RID: 41082
		public const string StandaloneWindows = "Windows";

		// Token: 0x0400A07B RID: 41083
		public const string StandaloneLinux = "Linux";

		// Token: 0x0400A07C RID: 41084
		public const string StandaloneOSX = "OSX";

		// Token: 0x0400A07D RID: 41085
		public const string PS5 = "PS5";

		// Token: 0x0400A07E RID: 41086
		public const string XBoxSeriesS = "XBoxSeriesS";

		// Token: 0x0400A07F RID: 41087
		public const string XBoxSeriesX = "XBoxSeriesX";
	}
}
