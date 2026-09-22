using System;
using System.ComponentModel;
using BhvrAnalyticsServices.Attributes;
using Newtonsoft.Json;

namespace Services.Analytics.Events
{
	// Token: 0x0200168F RID: 5775
	[EventTitle("Login")]
	public class LoginEventData : BaseEventData
	{
		// Token: 0x170015E1 RID: 5601
		// (get) Token: 0x0600B504 RID: 46340 RVA: 0x0043A98C File Offset: 0x00438B8C
		public override string EventType
		{
			get
			{
				return "login";
			}
		}

		// Token: 0x170015E2 RID: 5602
		// (get) Token: 0x0600B505 RID: 46341 RVA: 0x0043A993 File Offset: 0x00438B93
		// (set) Token: 0x0600B506 RID: 46342 RVA: 0x0043A99B File Offset: 0x00438B9B
		[JsonProperty(PropertyName = "session_start_ts")]
		[DateTimeFormat]
		public string SessionStartTimeStamp { get; set; }

		// Token: 0x170015E3 RID: 5603
		// (get) Token: 0x0600B507 RID: 46343 RVA: 0x0043A9A4 File Offset: 0x00438BA4
		[JsonProperty(PropertyName = "country_code", NullValueHandling = NullValueHandling.Include)]
		[Description("The country that the game session was initiated from. Converted From IP Address")]
		public string CountryCode { get; }

		// Token: 0x170015E4 RID: 5604
		// (get) Token: 0x0600B508 RID: 46344 RVA: 0x0043A9AC File Offset: 0x00438BAC
		// (set) Token: 0x0600B509 RID: 46345 RVA: 0x0043A9B4 File Offset: 0x00438BB4
		[JsonProperty(PropertyName = "ip")]
		[IpToCountryTransform("$.ip")]
		[DropTransform("$.ip")]
		[EventAdditionalProperty]
		public string IP { get; set; }

		// Token: 0x170015E5 RID: 5605
		// (get) Token: 0x0600B50A RID: 46346 RVA: 0x0043A9BD File Offset: 0x00438BBD
		// (set) Token: 0x0600B50B RID: 46347 RVA: 0x0043A9C5 File Offset: 0x00438BC5
		[JsonProperty(PropertyName = "crossplay_enabled")]
		[Description("Was crossplay enabled when the player launched the game")]
		public bool CrossplayEnabled { get; set; }

		// Token: 0x170015E6 RID: 5606
		// (get) Token: 0x0600B50C RID: 46348 RVA: 0x0043A9CE File Offset: 0x00438BCE
		// (set) Token: 0x0600B50D RID: 46349 RVA: 0x0043A9D6 File Offset: 0x00438BD6
		[JsonProperty(PropertyName = "is_first_launch_eos")]
		[Description("A flag to identify first session based on EOS")]
		public bool? IsFirstLaunchEos { get; set; }

		// Token: 0x170015E7 RID: 5607
		// (get) Token: 0x0600B50E RID: 46350 RVA: 0x0043A9DF File Offset: 0x00438BDF
		// (set) Token: 0x0600B50F RID: 46351 RVA: 0x0043A9E7 File Offset: 0x00438BE7
		[JsonProperty(PropertyName = "is_new_login")]
		[Description("Is false if the session became stale")]
		public bool IsNewLogin { get; set; }
	}
}
