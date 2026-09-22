using System;
using Newtonsoft.Json;

namespace Twitch.PubSub
{
	// Token: 0x0200188B RID: 6283
	public class PubSubSubscriptionRedemptionMessage : BasePubSubMessage
	{
		// Token: 0x170017C9 RID: 6089
		// (get) Token: 0x0600C1B9 RID: 49593 RVA: 0x0047C47D File Offset: 0x0047A67D
		// (set) Token: 0x0600C1BA RID: 49594 RVA: 0x0047C485 File Offset: 0x0047A685
		public int benefit_end_month { get; set; }

		// Token: 0x170017CA RID: 6090
		// (get) Token: 0x0600C1BB RID: 49595 RVA: 0x0047C48E File Offset: 0x0047A68E
		// (set) Token: 0x0600C1BC RID: 49596 RVA: 0x0047C496 File Offset: 0x0047A696
		public string user_name { get; set; }

		// Token: 0x170017CB RID: 6091
		// (get) Token: 0x0600C1BD RID: 49597 RVA: 0x0047C49F File Offset: 0x0047A69F
		// (set) Token: 0x0600C1BE RID: 49598 RVA: 0x0047C4A7 File Offset: 0x0047A6A7
		public string channel_name { get; set; }

		// Token: 0x170017CC RID: 6092
		// (get) Token: 0x0600C1BF RID: 49599 RVA: 0x0047C4B0 File Offset: 0x0047A6B0
		// (set) Token: 0x0600C1C0 RID: 49600 RVA: 0x0047C4B8 File Offset: 0x0047A6B8
		public string user_id { get; set; }

		// Token: 0x170017CD RID: 6093
		// (get) Token: 0x0600C1C1 RID: 49601 RVA: 0x0047C4C1 File Offset: 0x0047A6C1
		// (set) Token: 0x0600C1C2 RID: 49602 RVA: 0x0047C4C9 File Offset: 0x0047A6C9
		public string channel_id { get; set; }

		// Token: 0x170017CE RID: 6094
		// (get) Token: 0x0600C1C3 RID: 49603 RVA: 0x0047C4D2 File Offset: 0x0047A6D2
		// (set) Token: 0x0600C1C4 RID: 49604 RVA: 0x0047C4DA File Offset: 0x0047A6DA
		public string sub_plan { get; set; }

		// Token: 0x170017CF RID: 6095
		// (get) Token: 0x0600C1C5 RID: 49605 RVA: 0x0047C4E3 File Offset: 0x0047A6E3
		// (set) Token: 0x0600C1C6 RID: 49606 RVA: 0x0047C4EB File Offset: 0x0047A6EB
		public string sub_plan_name { get; set; }

		// Token: 0x170017D0 RID: 6096
		// (get) Token: 0x0600C1C7 RID: 49607 RVA: 0x0047C4F4 File Offset: 0x0047A6F4
		// (set) Token: 0x0600C1C8 RID: 49608 RVA: 0x0047C4FC File Offset: 0x0047A6FC
		public int months { get; set; }

		// Token: 0x170017D1 RID: 6097
		// (get) Token: 0x0600C1C9 RID: 49609 RVA: 0x0047C505 File Offset: 0x0047A705
		// (set) Token: 0x0600C1CA RID: 49610 RVA: 0x0047C50D File Offset: 0x0047A70D
		public int cumulative_months { get; set; }

		// Token: 0x170017D2 RID: 6098
		// (get) Token: 0x0600C1CB RID: 49611 RVA: 0x0047C516 File Offset: 0x0047A716
		// (set) Token: 0x0600C1CC RID: 49612 RVA: 0x0047C51E File Offset: 0x0047A71E
		public string context { get; set; }

		// Token: 0x170017D3 RID: 6099
		// (get) Token: 0x0600C1CD RID: 49613 RVA: 0x0047C527 File Offset: 0x0047A727
		// (set) Token: 0x0600C1CE RID: 49614 RVA: 0x0047C52F File Offset: 0x0047A72F
		public bool is_gift { get; set; }

		// Token: 0x170017D4 RID: 6100
		// (get) Token: 0x0600C1CF RID: 49615 RVA: 0x0047C538 File Offset: 0x0047A738
		// (set) Token: 0x0600C1D0 RID: 49616 RVA: 0x0047C540 File Offset: 0x0047A740
		public int multi_month_duration { get; set; }

		// Token: 0x0600C1D1 RID: 49617 RVA: 0x0047C549 File Offset: 0x0047A749
		public static PubSubSubscriptionRedemptionMessage Deserialize(string message)
		{
			return JsonConvert.DeserializeObject<PubSubSubscriptionRedemptionMessage>(message);
		}
	}
}
