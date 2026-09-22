using System;

namespace Platform
{
	// Token: 0x02001B48 RID: 6984
	public static class DeviceCapabilities
	{
		// Token: 0x0600D144 RID: 53572 RVA: 0x004C8B15 File Offset: 0x004C6D15
		public static bool CanUserAccessFilesystem()
		{
			return !Submission.Enabled || (DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent();
		}
	}
}
