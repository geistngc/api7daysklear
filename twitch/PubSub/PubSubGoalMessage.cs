using System;
using Newtonsoft.Json;

namespace Twitch.PubSub
{
	// Token: 0x02001884 RID: 6276
	public class PubSubGoalMessage : BasePubSubMessage
	{
		// Token: 0x170017BE RID: 6078
		// (get) Token: 0x0600C19B RID: 49563 RVA: 0x0047C399 File Offset: 0x0047A599
		// (set) Token: 0x0600C19C RID: 49564 RVA: 0x0047C3A1 File Offset: 0x0047A5A1
		public new string type { get; set; }

		// Token: 0x170017BF RID: 6079
		// (get) Token: 0x0600C19D RID: 49565 RVA: 0x0047C3AA File Offset: 0x0047A5AA
		public string TheType
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x170017C0 RID: 6080
		// (get) Token: 0x0600C19E RID: 49566 RVA: 0x0047C3B2 File Offset: 0x0047A5B2
		// (set) Token: 0x0600C19F RID: 49567 RVA: 0x0047C3BA File Offset: 0x0047A5BA
		public PubSubGoalMessage.GoalData data { get; set; }

		// Token: 0x0600C1A0 RID: 49568 RVA: 0x0047C3C3 File Offset: 0x0047A5C3
		public static PubSubGoalMessage Deserialize(string message)
		{
			return JsonConvert.DeserializeObject<PubSubGoalMessage>(message);
		}

		// Token: 0x02001885 RID: 6277
		public class GoalData
		{
			// Token: 0x170017C1 RID: 6081
			// (get) Token: 0x0600C1A2 RID: 49570 RVA: 0x0047C3CB File Offset: 0x0047A5CB
			// (set) Token: 0x0600C1A3 RID: 49571 RVA: 0x0047C3D3 File Offset: 0x0047A5D3
			public PubSubGoalMessage.Goal goal { get; set; }
		}

		// Token: 0x02001886 RID: 6278
		public class Goal
		{
			// Token: 0x170017C2 RID: 6082
			// (get) Token: 0x0600C1A5 RID: 49573 RVA: 0x0047C3DC File Offset: 0x0047A5DC
			// (set) Token: 0x0600C1A6 RID: 49574 RVA: 0x0047C3E4 File Offset: 0x0047A5E4
			public string contributionType { get; set; }

			// Token: 0x170017C3 RID: 6083
			// (get) Token: 0x0600C1A7 RID: 49575 RVA: 0x0047C3ED File Offset: 0x0047A5ED
			// (set) Token: 0x0600C1A8 RID: 49576 RVA: 0x0047C3F5 File Offset: 0x0047A5F5
			public string state { get; set; }

			// Token: 0x170017C4 RID: 6084
			// (get) Token: 0x0600C1A9 RID: 49577 RVA: 0x0047C3FE File Offset: 0x0047A5FE
			// (set) Token: 0x0600C1AA RID: 49578 RVA: 0x0047C406 File Offset: 0x0047A606
			public int currentContributions { get; set; }

			// Token: 0x170017C5 RID: 6085
			// (get) Token: 0x0600C1AB RID: 49579 RVA: 0x0047C40F File Offset: 0x0047A60F
			// (set) Token: 0x0600C1AC RID: 49580 RVA: 0x0047C417 File Offset: 0x0047A617
			public int targetContributions { get; set; }
		}
	}
}
