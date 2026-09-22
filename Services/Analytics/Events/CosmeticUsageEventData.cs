using System;
using System.ComponentModel;
using BhvrAnalyticsServices.Attributes;
using Newtonsoft.Json;

namespace Services.Analytics.Events
{
	// Token: 0x0200168A RID: 5770
	[EventTitle("CosmeticUsage")]
	public class CosmeticUsageEventData : BaseEventData
	{
		// Token: 0x170015CD RID: 5581
		// (get) Token: 0x0600B4DC RID: 46300 RVA: 0x0043A790 File Offset: 0x00438990
		public override string EventType
		{
			get
			{
				return "cosmetic_usage";
			}
		}

		// Token: 0x170015CE RID: 5582
		// (get) Token: 0x0600B4DD RID: 46301 RVA: 0x0043A797 File Offset: 0x00438997
		// (set) Token: 0x0600B4DE RID: 46302 RVA: 0x0043A79F File Offset: 0x0043899F
		[JsonProperty(PropertyName = "server_id")]
		[Description("Id of the connected server, null if not in one")]
		public string ServerId { get; set; }

		// Token: 0x170015CF RID: 5583
		// (get) Token: 0x0600B4DF RID: 46303 RVA: 0x0043A7A8 File Offset: 0x004389A8
		// (set) Token: 0x0600B4E0 RID: 46304 RVA: 0x0043A7B0 File Offset: 0x004389B0
		[JsonProperty(PropertyName = "save_id")]
		[Description("Id of the save")]
		public string SaveId { get; set; }

		// Token: 0x170015D0 RID: 5584
		// (get) Token: 0x0600B4E1 RID: 46305 RVA: 0x0043A7B9 File Offset: 0x004389B9
		// (set) Token: 0x0600B4E2 RID: 46306 RVA: 0x0043A7C1 File Offset: 0x004389C1
		[JsonProperty(PropertyName = "cosmetic_change_ts")]
		[Description("The moment when the player equipped the item.")]
		[DateTimeFormat]
		public string CosmeticChangeTimestamp { get; set; }

		// Token: 0x170015D1 RID: 5585
		// (get) Token: 0x0600B4E3 RID: 46307 RVA: 0x0043A7CA File Offset: 0x004389CA
		// (set) Token: 0x0600B4E4 RID: 46308 RVA: 0x0043A7D2 File Offset: 0x004389D2
		[JsonProperty(PropertyName = "slot")]
		[Description("The slot where the equipment goes.")]
		public string Slot { get; set; }

		// Token: 0x170015D2 RID: 5586
		// (get) Token: 0x0600B4E5 RID: 46309 RVA: 0x0043A7DB File Offset: 0x004389DB
		// (set) Token: 0x0600B4E6 RID: 46310 RVA: 0x0043A7E3 File Offset: 0x004389E3
		[JsonProperty(PropertyName = "cosmetic_name")]
		[Description("The name of the item (truncated to the first 50 characters)")]
		[JsonConverter(typeof(TruncateStringSerializerConverter))]
		public string CosmeticName { get; set; }

		// Token: 0x170015D3 RID: 5587
		// (get) Token: 0x0600B4E7 RID: 46311 RVA: 0x0043A7EC File Offset: 0x004389EC
		// (set) Token: 0x0600B4E8 RID: 46312 RVA: 0x0043A7F4 File Offset: 0x004389F4
		[JsonProperty(PropertyName = "set_name")]
		[Description("If the item is part of a set, we would like to know the set name.")]
		public string SetName { get; set; }
	}
}
