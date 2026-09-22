using System;
using System.ComponentModel;
using BhvrAnalyticsServices.Attributes;
using Newtonsoft.Json;

namespace Services.Analytics.Events
{
	// Token: 0x02001688 RID: 5768
	[EventTitle("Challenge Claimed")]
	public class ChallengeClaimedEventData : BaseEventData
	{
		// Token: 0x170015B6 RID: 5558
		// (get) Token: 0x0600B4AE RID: 46254 RVA: 0x0043A615 File Offset: 0x00438815
		public override string EventType
		{
			get
			{
				return "challenge_claimed";
			}
		}

		// Token: 0x170015B7 RID: 5559
		// (get) Token: 0x0600B4AF RID: 46255 RVA: 0x0043A61C File Offset: 0x0043881C
		// (set) Token: 0x0600B4B0 RID: 46256 RVA: 0x0043A624 File Offset: 0x00438824
		[JsonProperty(PropertyName = "server_id")]
		[Description("Id of the connected server, null if not in one")]
		public string ServerId { get; set; }

		// Token: 0x170015B8 RID: 5560
		// (get) Token: 0x0600B4B1 RID: 46257 RVA: 0x0043A62D File Offset: 0x0043882D
		// (set) Token: 0x0600B4B2 RID: 46258 RVA: 0x0043A635 File Offset: 0x00438835
		[JsonProperty(PropertyName = "save_id")]
		public string SaveId { get; set; }

		// Token: 0x170015B9 RID: 5561
		// (get) Token: 0x0600B4B3 RID: 46259 RVA: 0x0043A63E File Offset: 0x0043883E
		// (set) Token: 0x0600B4B4 RID: 46260 RVA: 0x0043A646 File Offset: 0x00438846
		[JsonProperty(PropertyName = "challenge_claimed_ts")]
		[Description("Timestamp of when the challenged was claimed")]
		[DateTimeFormat]
		public string ChallengeClaimedTimeStamp { get; set; }

		// Token: 0x170015BA RID: 5562
		// (get) Token: 0x0600B4B5 RID: 46261 RVA: 0x0043A64F File Offset: 0x0043884F
		// (set) Token: 0x0600B4B6 RID: 46262 RVA: 0x0043A657 File Offset: 0x00438857
		[JsonProperty(PropertyName = "elapsed_time")]
		[Description("Cumulative number of seconds online on this server at the time the challenge was claimed")]
		public uint ElapsedTime { get; set; }

		// Token: 0x170015BB RID: 5563
		// (get) Token: 0x0600B4B7 RID: 46263 RVA: 0x0043A660 File Offset: 0x00438860
		// (set) Token: 0x0600B4B8 RID: 46264 RVA: 0x0043A668 File Offset: 0x00438868
		[JsonProperty(PropertyName = "player_level")]
		[Description("Level of the player when challenge claimed")]
		public int PlayerLevel { get; set; }

		// Token: 0x170015BC RID: 5564
		// (get) Token: 0x0600B4B9 RID: 46265 RVA: 0x0043A671 File Offset: 0x00438871
		// (set) Token: 0x0600B4BA RID: 46266 RVA: 0x0043A679 File Offset: 0x00438879
		[JsonProperty(PropertyName = "personal_game_stage")]
		[Description("Personal Game Stage of the player when challenge claimed")]
		public int PersonalGameStage { get; set; }

		// Token: 0x170015BD RID: 5565
		// (get) Token: 0x0600B4BB RID: 46267 RVA: 0x0043A682 File Offset: 0x00438882
		// (set) Token: 0x0600B4BC RID: 46268 RVA: 0x0043A68A File Offset: 0x0043888A
		[JsonProperty(PropertyName = "challenge_name")]
		[Description("Name of the challenge claimed")]
		public string ChallengeName { get; set; }

		// Token: 0x170015BE RID: 5566
		// (get) Token: 0x0600B4BD RID: 46269 RVA: 0x0043A693 File Offset: 0x00438893
		// (set) Token: 0x0600B4BE RID: 46270 RVA: 0x0043A69B File Offset: 0x0043889B
		[JsonProperty(PropertyName = "challenge_group")]
		[Description("Group of the challenge claimed")]
		public string ChallengeGroup { get; set; }

		// Token: 0x170015BF RID: 5567
		// (get) Token: 0x0600B4BF RID: 46271 RVA: 0x0043A6A4 File Offset: 0x004388A4
		// (set) Token: 0x0600B4C0 RID: 46272 RVA: 0x0043A6AC File Offset: 0x004388AC
		[JsonProperty(PropertyName = "is_challenge_group_complete")]
		[Description("Group of the challenge claimed")]
		public bool IsChallengeGroupComplete { get; set; }

		// Token: 0x170015C0 RID: 5568
		// (get) Token: 0x0600B4C1 RID: 46273 RVA: 0x0043A6B5 File Offset: 0x004388B5
		// (set) Token: 0x0600B4C2 RID: 46274 RVA: 0x0043A6BD File Offset: 0x004388BD
		[JsonProperty(PropertyName = "challenge_category")]
		[Description("Category of the challenge claimed")]
		public string ChallengeCategory { get; set; }

		// Token: 0x170015C1 RID: 5569
		// (get) Token: 0x0600B4C3 RID: 46275 RVA: 0x0043A6C6 File Offset: 0x004388C6
		// (set) Token: 0x0600B4C4 RID: 46276 RVA: 0x0043A6CE File Offset: 0x004388CE
		[JsonProperty(PropertyName = "reward_event")]
		[Description("Reward Event of the challenge claimed")]
		public string RewardEvent { get; set; }
	}
}
