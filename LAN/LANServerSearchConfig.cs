using System;
using System.Net;

namespace Platform.LAN
{
	// Token: 0x02001CDA RID: 7386
	public static class LANServerSearchConfig
	{
		// Token: 0x0400A600 RID: 42496
		public static readonly IPAddress MulticastGroupIp = IPAddress.Parse("239.192.0.1");

		// Token: 0x0400A601 RID: 42497
		public const int DefaultPort = 11000;
	}
}
