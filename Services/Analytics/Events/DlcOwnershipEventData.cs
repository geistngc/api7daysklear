using System;
using System.Collections.Generic;
using BhvrAnalyticsServices.Attributes;
using Newtonsoft.Json;

namespace Services.Analytics.Events
{
	// Token: 0x0200168B RID: 5771
	[EventTitle("DLC Ownership")]
	public class DlcOwnershipEventData : BaseEventData
	{
		// Token: 0x170015D4 RID: 5588
		// (get) Token: 0x0600B4EA RID: 46314 RVA: 0x0043A7FD File Offset: 0x004389FD
		public override string EventType
		{
			get
			{
				return "dlc_ownership";
			}
		}

		// Token: 0x170015D5 RID: 5589
		// (get) Token: 0x0600B4EB RID: 46315 RVA: 0x0043A804 File Offset: 0x00438A04
		// (set) Token: 0x0600B4EC RID: 46316 RVA: 0x0043A80C File Offset: 0x00438A0C
		[JsonProperty(PropertyName = "dlc_owned")]
		public IEnumerable<DlcOwnershipEventData.EntitlementData> DlcOwned { get; set; }

		// Token: 0x170015D6 RID: 5590
		// (get) Token: 0x0600B4ED RID: 46317 RVA: 0x0043A815 File Offset: 0x00438A15
		// (set) Token: 0x0600B4EE RID: 46318 RVA: 0x0043A81D File Offset: 0x00438A1D
		[JsonProperty(PropertyName = "is_family_share")]
		public bool? IsFamilyShare { get; set; }

		// Token: 0x0200168C RID: 5772
		[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
		public struct EntitlementData
		{
			// Token: 0x040087E4 RID: 34788
			public string Name;

			// Token: 0x040087E5 RID: 34789
			public string ID;

			// Token: 0x040087E6 RID: 34790
			public string AcquiredDate;
		}
	}
}
