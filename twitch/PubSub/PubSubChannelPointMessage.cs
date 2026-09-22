using System;
using Newtonsoft.Json;

namespace Twitch.PubSub
{
	// Token: 0x0200187F RID: 6271
	public class PubSubChannelPointMessage : BasePubSubMessage
	{
		// Token: 0x0600C189 RID: 49545 RVA: 0x0047C32B File Offset: 0x0047A52B
		public static PubSubChannelPointMessage Deserialize(string message)
		{
			return JsonConvert.DeserializeObject<PubSubChannelPointMessage>(message);
		}

		// Token: 0x040092C8 RID: 37576
		public PubSubChannelPointMessage.ChannelRedemptionData data;

		// Token: 0x02001880 RID: 6272
		public class ChannelRedemptionData : EventArgs
		{
			// Token: 0x170017B8 RID: 6072
			// (get) Token: 0x0600C18B RID: 49547 RVA: 0x0047C333 File Offset: 0x0047A533
			// (set) Token: 0x0600C18C RID: 49548 RVA: 0x0047C33B File Offset: 0x0047A53B
			public PubSubChannelPointMessage.Redemption redemption { get; set; }
		}

		// Token: 0x02001881 RID: 6273
		public class Redemption
		{
			// Token: 0x170017B9 RID: 6073
			// (get) Token: 0x0600C18E RID: 49550 RVA: 0x0047C344 File Offset: 0x0047A544
			// (set) Token: 0x0600C18F RID: 49551 RVA: 0x0047C34C File Offset: 0x0047A54C
			public PubSubChannelPointMessage.User user { get; set; }

			// Token: 0x170017BA RID: 6074
			// (get) Token: 0x0600C190 RID: 49552 RVA: 0x0047C355 File Offset: 0x0047A555
			// (set) Token: 0x0600C191 RID: 49553 RVA: 0x0047C35D File Offset: 0x0047A55D
			public PubSubChannelPointMessage.Reward reward { get; set; }
		}

		// Token: 0x02001882 RID: 6274
		public class User
		{
			// Token: 0x170017BB RID: 6075
			// (get) Token: 0x0600C193 RID: 49555 RVA: 0x0047C366 File Offset: 0x0047A566
			// (set) Token: 0x0600C194 RID: 49556 RVA: 0x0047C36E File Offset: 0x0047A56E
			public string login { get; set; }

			// Token: 0x170017BC RID: 6076
			// (get) Token: 0x0600C195 RID: 49557 RVA: 0x0047C377 File Offset: 0x0047A577
			// (set) Token: 0x0600C196 RID: 49558 RVA: 0x0047C37F File Offset: 0x0047A57F
			public string display_name { get; set; }
		}

		// Token: 0x02001883 RID: 6275
		public class Reward
		{
			// Token: 0x170017BD RID: 6077
			// (get) Token: 0x0600C198 RID: 49560 RVA: 0x0047C388 File Offset: 0x0047A588
			// (set) Token: 0x0600C199 RID: 49561 RVA: 0x0047C390 File Offset: 0x0047A590
			public string title { get; set; }
		}
	}
}
