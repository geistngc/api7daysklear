using System;

namespace Twitch.PubSub
{
	// Token: 0x02001889 RID: 6281
	public class PubSubListenMessage : BasePubSubMessage
	{
		// Token: 0x0600C1B3 RID: 49587 RVA: 0x0047C448 File Offset: 0x0047A648
		public PubSubListenMessage()
		{
			base.type = "LISTEN";
		}

		// Token: 0x040092D8 RID: 37592
		public PubSubListenMessage.PubSubListenData data;

		// Token: 0x0200188A RID: 6282
		public class PubSubListenData
		{
			// Token: 0x170017C7 RID: 6087
			// (get) Token: 0x0600C1B4 RID: 49588 RVA: 0x0047C45B File Offset: 0x0047A65B
			// (set) Token: 0x0600C1B5 RID: 49589 RVA: 0x0047C463 File Offset: 0x0047A663
			public string[] topics { get; set; }

			// Token: 0x170017C8 RID: 6088
			// (get) Token: 0x0600C1B6 RID: 49590 RVA: 0x0047C46C File Offset: 0x0047A66C
			// (set) Token: 0x0600C1B7 RID: 49591 RVA: 0x0047C474 File Offset: 0x0047A674
			public string auth_token { get; set; }
		}
	}
}
