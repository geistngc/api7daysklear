using System;
using Newtonsoft.Json;

namespace Twitch.PubSub
{
	// Token: 0x0200187D RID: 6269
	public class PubSubBitRedemptionMessage : BasePubSubMessage
	{
		// Token: 0x0600C176 RID: 49526 RVA: 0x0047C293 File Offset: 0x0047A493
		public static PubSubBitRedemptionMessage Deserialize(string message)
		{
			return JsonConvert.DeserializeObject<PubSubBitRedemptionMessage>(message);
		}

		// Token: 0x040092BF RID: 37567
		public PubSubBitRedemptionMessage.BitRedemptionData data;

		// Token: 0x0200187E RID: 6270
		public class BitRedemptionData : EventArgs
		{
			// Token: 0x170017B0 RID: 6064
			// (get) Token: 0x0600C178 RID: 49528 RVA: 0x0047C2A3 File Offset: 0x0047A4A3
			// (set) Token: 0x0600C179 RID: 49529 RVA: 0x0047C2AB File Offset: 0x0047A4AB
			public string user_name { get; set; }

			// Token: 0x170017B1 RID: 6065
			// (get) Token: 0x0600C17A RID: 49530 RVA: 0x0047C2B4 File Offset: 0x0047A4B4
			// (set) Token: 0x0600C17B RID: 49531 RVA: 0x0047C2BC File Offset: 0x0047A4BC
			public string channel_name { get; set; }

			// Token: 0x170017B2 RID: 6066
			// (get) Token: 0x0600C17C RID: 49532 RVA: 0x0047C2C5 File Offset: 0x0047A4C5
			// (set) Token: 0x0600C17D RID: 49533 RVA: 0x0047C2CD File Offset: 0x0047A4CD
			public string user_id { get; set; }

			// Token: 0x170017B3 RID: 6067
			// (get) Token: 0x0600C17E RID: 49534 RVA: 0x0047C2D6 File Offset: 0x0047A4D6
			// (set) Token: 0x0600C17F RID: 49535 RVA: 0x0047C2DE File Offset: 0x0047A4DE
			public string channel_id { get; set; }

			// Token: 0x170017B4 RID: 6068
			// (get) Token: 0x0600C180 RID: 49536 RVA: 0x0047C2E7 File Offset: 0x0047A4E7
			// (set) Token: 0x0600C181 RID: 49537 RVA: 0x0047C2EF File Offset: 0x0047A4EF
			public string chat_message { get; set; }

			// Token: 0x170017B5 RID: 6069
			// (get) Token: 0x0600C182 RID: 49538 RVA: 0x0047C2F8 File Offset: 0x0047A4F8
			// (set) Token: 0x0600C183 RID: 49539 RVA: 0x0047C300 File Offset: 0x0047A500
			public int bits_used { get; set; }

			// Token: 0x170017B6 RID: 6070
			// (get) Token: 0x0600C184 RID: 49540 RVA: 0x0047C309 File Offset: 0x0047A509
			// (set) Token: 0x0600C185 RID: 49541 RVA: 0x0047C311 File Offset: 0x0047A511
			public int total_bits_used { get; set; }

			// Token: 0x170017B7 RID: 6071
			// (get) Token: 0x0600C186 RID: 49542 RVA: 0x0047C31A File Offset: 0x0047A51A
			// (set) Token: 0x0600C187 RID: 49543 RVA: 0x0047C322 File Offset: 0x0047A522
			public bool is_anonymous { get; set; }
		}
	}
}
