using System;

namespace mumblelib
{
	// Token: 0x020016A2 RID: 5794
	public static class LinkFileManager
	{
		// Token: 0x0600B5A7 RID: 46503 RVA: 0x0043BC11 File Offset: 0x00439E11
		public static ILinkFile Open()
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				Log.Out("[MumbleLF] Loading Windows Mumble Link");
				return new WindowsLinkFile();
			}
			Log.Out("[MumbleLF] Loading Unix Mumble Link");
			return new UnixLinkFile();
		}
	}
}
