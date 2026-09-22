using System;

namespace Platform.EOS
{
	// Token: 0x02001CFB RID: 7419
	public static class NetworkCommonEos
	{
		// Token: 0x0400A6A1 RID: 42657
		public const int MaxUsedPacketSize = 1120;

		// Token: 0x02001CFC RID: 7420
		public enum ESteamNetChannels : byte
		{
			// Token: 0x0400A6A3 RID: 42659
			NetpackageChannel0,
			// Token: 0x0400A6A4 RID: 42660
			NetpackageChannel1,
			// Token: 0x0400A6A5 RID: 42661
			Authentication = 50,
			// Token: 0x0400A6A6 RID: 42662
			Ping = 60
		}

		// Token: 0x02001CFD RID: 7421
		public readonly struct SendInfo
		{
			// Token: 0x0600DC18 RID: 56344 RVA: 0x004EDE9E File Offset: 0x004EC09E
			public SendInfo(ClientInfo _clientInfo, ArrayListMP<byte> _data)
			{
				this.Recipient = _clientInfo;
				this.Data = _data;
			}

			// Token: 0x0400A6A7 RID: 42663
			public readonly ClientInfo Recipient;

			// Token: 0x0400A6A8 RID: 42664
			public readonly ArrayListMP<byte> Data;
		}
	}
}
