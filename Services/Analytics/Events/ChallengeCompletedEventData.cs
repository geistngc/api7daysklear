using System;
using System.ComponentModel;
using BhvrAnalyticsServices.Attributes;
using Newtonsoft.Json;

namespace Services.Analytics.Events
{
	// Token: 0x02001689 RID: 5769
	[EventTitle("Challenge Completed")]
	public class ChallengeCompletedEventData : BaseEventData
	{
		// Token: 0x170015C2 RID: 5570
		// (get) Token: 0x0600B4C6 RID: 46278 RVA: 0x0043A6DF File Offset: 0x004388DF
		public override string EventType
		{
			get
			{
				return "challenge_completed";
			}
		}

		// Token: 0x170015C3 RID: 5571
		// (get) Token: 0x0600B4C7 RID: 46279 RVA: 0x0043A6E6 File Offset: 0x004388E6
		// (set) Token: 0x0600B4C8 RID: 46280 RVA: 0x0043A6EE File Offset: 0x004388EE
		[JsonProperty(PropertyName = "server_id")]
		[Description("Id of the connected server, null if not in one")]
		public string ServerId { get; set; }

		// Token: 0x170015C4 RID: 5572
		// (get) Token: 0x0600B4C9 RID: 46281 RVA: 0x0043A6F7 File Offset: 0x004388F7
		// (set) Token: 0x0600B4CA RID: 46282 RVA: 0x0043A6FF File Offset: 0x004388FF
		[JsonProperty(PropertyName = "save_id")]
		public string SaveId { get; set; }

		// Token: 0x170015C5 RID: 5573
		// (get) Token: 0x0600B4CB RID: 46283 RVA: 0x0043A708 File Offset: 0x00438908
		// (set) Token: 0x0600B4CC RID: 46284 RVA: 0x0043A710 File Offset: 0x00438910
		[JsonProperty(PropertyName = "challenge_completed_ts")]
		[Description("Timestamp of when the challenged was completed")]
		[DateTimeFormat]
		public string ChallengeCompletedTimeStamp { get; set; }

		// Token: 0x170015C6 RID: 5574
		// (get) Token: 0x0600B4CD RID: 46285 RVA: 0x0043A719 File Offset: 0x00438919
		// (set) Token: 0x0600B4CE RID: 46286 RVA: 0x0043A721 File Offset: 0x00438921
		[JsonProperty(PropertyName = "elapsed_time")]
		[Description("Cumulative number of seconds online on this server at the time the challenge was completed")]
		public uint ElapsedTime { get; set; }

		// Token: 0x170015C7 RID: 5575
		// (get) Token: 0x0600B4CF RID: 46287 RVA: 0x0043A72A File Offset: 0x0043892A
		// (set) Token: 0x0600B4D0 RID: 46288 RVA: 0x0043A732 File Offset: 0x00438932
		[JsonProperty(PropertyName = "player_level")]
		[Description("Level of the player when challenge completed")]
		public int PlayerLevel { get; set; }

		// Token: 0x170015C8 RID: 5576
		// (get) Token: 0x0600B4D1 RID: 46289 RVA: 0x0043A73B File Offset: 0x0043893B
		// (set) Token: 0x0600B4D2 RID: 46290 RVA: 0x0043A743 File Offset: 0x00438943
		[JsonProperty(PropertyName = "personal_game_stage")]
		[Description("Personal Game Stage of the player when challenge completed")]
		public int PersonalGameStage { get; set; }

		// Token: 0x170015C9 RID: 5577
		// (get) Token: 0x0600B4D3 RID: 46291 RVA: 0x0043A74C File Offset: 0x0043894C
		// (set) Token: 0x0600B4D4 RID: 46292 RVA: 0x0043A754 File Offset: 0x00438954
		[JsonProperty(PropertyName = "challenge_name")]
		[Description("Name of the challenge completed")]
		public string ChallengeName { get; set; }

		// Token: 0x170015CA RID: 5578
		// (get) Token: 0x0600B4D5 RID: 46293 RVA: 0x0043A75D File Offset: 0x0043895D
		// (set) Token: 0x0600B4D6 RID: 46294 RVA: 0x0043A765 File Offset: 0x00438965
		[JsonProperty(PropertyName = "challenge_group")]
		[Description("Group of the challenge completed")]
		public string ChallengeGroup { get; set; }

		// Token: 0x170015CB RID: 5579
		// (get) Token: 0x0600B4D7 RID: 46295 RVA: 0x0043A76E File Offset: 0x0043896E
		// (set) Token: 0x0600B4D8 RID: 46296 RVA: 0x0043A776 File Offset: 0x00438976
		[JsonProperty(PropertyName = "challenge_category")]
		[Description("Category of the challenge completed")]
		public string ChallengeCategory { get; set; }

		// Token: 0x170015CC RID: 5580
		// (get) Token: 0x0600B4D9 RID: 46297 RVA: 0x0043A77F File Offset: 0x0043897F
		// (set) Token: 0x0600B4DA RID: 46298 RVA: 0x0043A787 File Offset: 0x00438987
		[JsonProperty(PropertyName = "is_forced_completion")]
		[Description("If the challenge was force completed")]
		public bool IsForcedCompletion { get; set; }
	}
}
