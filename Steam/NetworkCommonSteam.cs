using System;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001C9D RID: 7325
	public static class NetworkCommonSteam
	{
		// Token: 0x02001C9E RID: 7326
		public enum ESteamNetChannels : byte
		{
			// Token: 0x0400A515 RID: 42261
			NetpackageChannel0,
			// Token: 0x0400A516 RID: 42262
			NetpackageChannel1,
			// Token: 0x0400A517 RID: 42263
			Authentication = 50,
			// Token: 0x0400A518 RID: 42264
			Ping = 60
		}

		// Token: 0x02001C9F RID: 7327
		public readonly struct SendInfo
		{
			// Token: 0x0600D941 RID: 55617 RVA: 0x004E2D7A File Offset: 0x004E0F7A
			public SendInfo(CSteamID _recipient, ArrayListMP<byte> _data)
			{
				this.Recipient = _recipient;
				this.Data = _data;
			}

			// Token: 0x0400A519 RID: 42265
			public readonly CSteamID Recipient;

			// Token: 0x0400A51A RID: 42266
			public readonly ArrayListMP<byte> Data;
		}
	}
}
