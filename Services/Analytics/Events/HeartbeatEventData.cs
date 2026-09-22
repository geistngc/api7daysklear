using System;
using System.ComponentModel;
using BhvrAnalyticsServices.Attributes;
using Newtonsoft.Json;

namespace Services.Analytics.Events
{
	// Token: 0x0200168E RID: 5774
	[EventTitle("Heartbeat")]
	public class HeartbeatEventData : BaseEventData
	{
		// Token: 0x170015DC RID: 5596
		// (get) Token: 0x0600B4FA RID: 46330 RVA: 0x0043A941 File Offset: 0x00438B41
		public override string EventType
		{
			get
			{
				return "heartbeat";
			}
		}

		// Token: 0x170015DD RID: 5597
		// (get) Token: 0x0600B4FB RID: 46331 RVA: 0x0043A948 File Offset: 0x00438B48
		// (set) Token: 0x0600B4FC RID: 46332 RVA: 0x0043A950 File Offset: 0x00438B50
		[JsonProperty(PropertyName = "heartbeat_ts")]
		[Description("Timestamp of the heartbeat. Sent every 5 minutes after login")]
		[DateTimeFormat]
		public string HeartbeatTimestamp { get; set; }

		// Token: 0x170015DE RID: 5598
		// (get) Token: 0x0600B4FD RID: 46333 RVA: 0x0043A959 File Offset: 0x00438B59
		// (set) Token: 0x0600B4FE RID: 46334 RVA: 0x0043A961 File Offset: 0x00438B61
		[JsonProperty(PropertyName = "server_id")]
		[Description("Id of the connected server, null if not in one")]
		public string ServerId { get; set; }

		// Token: 0x170015DF RID: 5599
		// (get) Token: 0x0600B4FF RID: 46335 RVA: 0x0043A96A File Offset: 0x00438B6A
		// (set) Token: 0x0600B500 RID: 46336 RVA: 0x0043A972 File Offset: 0x00438B72
		[JsonProperty(PropertyName = "save_id")]
		public string SaveId { get; set; }

		// Token: 0x170015E0 RID: 5600
		// (get) Token: 0x0600B501 RID: 46337 RVA: 0x0043A97B File Offset: 0x00438B7B
		// (set) Token: 0x0600B502 RID: 46338 RVA: 0x0043A983 File Offset: 0x00438B83
		[JsonProperty(PropertyName = "player_count")]
		public int? PlayerCount { get; set; }
	}
}
