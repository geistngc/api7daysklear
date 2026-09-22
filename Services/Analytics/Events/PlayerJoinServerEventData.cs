using System;
using BhvrAnalyticsServices.Attributes;
using Newtonsoft.Json;

namespace Services.Analytics.Events
{
	// Token: 0x02001690 RID: 5776
	[EventTitle("Player Join Server")]
	public class PlayerJoinServerEventData : BaseEventData
	{
		// Token: 0x170015E8 RID: 5608
		// (get) Token: 0x0600B511 RID: 46353 RVA: 0x0043A9F0 File Offset: 0x00438BF0
		public override string EventType
		{
			get
			{
				return "player_join_server";
			}
		}

		// Token: 0x170015E9 RID: 5609
		// (get) Token: 0x0600B512 RID: 46354 RVA: 0x0043A9F7 File Offset: 0x00438BF7
		// (set) Token: 0x0600B513 RID: 46355 RVA: 0x0043A9FF File Offset: 0x00438BFF
		[JsonProperty(PropertyName = "server_id")]
		public string ServerId { get; set; }

		// Token: 0x170015EA RID: 5610
		// (get) Token: 0x0600B514 RID: 46356 RVA: 0x0043AA08 File Offset: 0x00438C08
		// (set) Token: 0x0600B515 RID: 46357 RVA: 0x0043AA10 File Offset: 0x00438C10
		[JsonProperty(PropertyName = "save_id")]
		public string SaveId { get; set; }

		// Token: 0x170015EB RID: 5611
		// (get) Token: 0x0600B516 RID: 46358 RVA: 0x0043AA19 File Offset: 0x00438C19
		// (set) Token: 0x0600B517 RID: 46359 RVA: 0x0043AA21 File Offset: 0x00438C21
		[JsonProperty(PropertyName = "server_join_ts")]
		[DateTimeFormat]
		public string ServerJoinTimestamp { get; set; }

		// Token: 0x170015EC RID: 5612
		// (get) Token: 0x0600B518 RID: 46360 RVA: 0x0043AA2A File Offset: 0x00438C2A
		// (set) Token: 0x0600B519 RID: 46361 RVA: 0x0043AA32 File Offset: 0x00438C32
		[JsonProperty(PropertyName = "online_players")]
		public int? OnlinePlayers { get; set; }

		// Token: 0x170015ED RID: 5613
		// (get) Token: 0x0600B51A RID: 46362 RVA: 0x0043AA3B File Offset: 0x00438C3B
		// (set) Token: 0x0600B51B RID: 46363 RVA: 0x0043AA43 File Offset: 0x00438C43
		[JsonProperty(PropertyName = "server_join_source")]
		public string ServerJoinSource { get; set; }

		// Token: 0x170015EE RID: 5614
		// (get) Token: 0x0600B51C RID: 46364 RVA: 0x0043AA4C File Offset: 0x00438C4C
		// (set) Token: 0x0600B51D RID: 46365 RVA: 0x0043AA54 File Offset: 0x00438C54
		[JsonProperty(PropertyName = "local_mods")]
		public object LocalMods { get; set; }

		// Token: 0x170015EF RID: 5615
		// (get) Token: 0x0600B51E RID: 46366 RVA: 0x0043AA5D File Offset: 0x00438C5D
		// (set) Token: 0x0600B51F RID: 46367 RVA: 0x0043AA65 File Offset: 0x00438C65
		[JsonProperty(PropertyName = "has_modified_xml_files")]
		public bool? HasModifiedXML { get; set; }

		// Token: 0x170015F0 RID: 5616
		// (get) Token: 0x0600B520 RID: 46368 RVA: 0x0043AA6E File Offset: 0x00438C6E
		// (set) Token: 0x0600B521 RID: 46369 RVA: 0x0043AA76 File Offset: 0x00438C76
		[JsonProperty(PropertyName = "elapsed_player_time")]
		public uint? ElapsedPlayerTime { get; set; }

		// Token: 0x170015F1 RID: 5617
		// (get) Token: 0x0600B522 RID: 46370 RVA: 0x0043AA7F File Offset: 0x00438C7F
		// (set) Token: 0x0600B523 RID: 46371 RVA: 0x0043AA87 File Offset: 0x00438C87
		[JsonProperty(PropertyName = "elapsed_world_save_time")]
		public ulong? ElapsedWorldSaveTime { get; set; }

		// Token: 0x170015F2 RID: 5618
		// (get) Token: 0x0600B524 RID: 46372 RVA: 0x0043AA90 File Offset: 0x00438C90
		// (set) Token: 0x0600B525 RID: 46373 RVA: 0x0043AA98 File Offset: 0x00438C98
		[JsonProperty(PropertyName = "in_game_days")]
		public int? InGameDays { get; set; }

		// Token: 0x170015F3 RID: 5619
		// (get) Token: 0x0600B526 RID: 46374 RVA: 0x0043AAA1 File Offset: 0x00438CA1
		// (set) Token: 0x0600B527 RID: 46375 RVA: 0x0043AAA9 File Offset: 0x00438CA9
		[JsonProperty(PropertyName = "character_level")]
		public int? CharacterLevel { get; set; }

		// Token: 0x170015F4 RID: 5620
		// (get) Token: 0x0600B528 RID: 46376 RVA: 0x0043AAB2 File Offset: 0x00438CB2
		// (set) Token: 0x0600B529 RID: 46377 RVA: 0x0043AABA File Offset: 0x00438CBA
		[JsonProperty(PropertyName = "total_deaths")]
		public int? TotalDeaths { get; set; }

		// Token: 0x170015F5 RID: 5621
		// (get) Token: 0x0600B52A RID: 46378 RVA: 0x0043AAC3 File Offset: 0x00438CC3
		// (set) Token: 0x0600B52B RID: 46379 RVA: 0x0043AACB File Offset: 0x00438CCB
		[JsonProperty(PropertyName = "personal_game_stage_modified")]
		public float? PersonalGameStageModified { get; set; }

		// Token: 0x170015F6 RID: 5622
		// (get) Token: 0x0600B52C RID: 46380 RVA: 0x0043AAD4 File Offset: 0x00438CD4
		// (set) Token: 0x0600B52D RID: 46381 RVA: 0x0043AADC File Offset: 0x00438CDC
		[JsonProperty(PropertyName = "personal_game_stage_unmodified")]
		public float? PersonalGameStageUnmodified { get; set; }

		// Token: 0x170015F7 RID: 5623
		// (get) Token: 0x0600B52E RID: 46382 RVA: 0x0043AAE5 File Offset: 0x00438CE5
		// (set) Token: 0x0600B52F RID: 46383 RVA: 0x0043AAED File Offset: 0x00438CED
		[JsonProperty(PropertyName = "game_stages_json")]
		public object GameStagesJson { get; set; }

		// Token: 0x170015F8 RID: 5624
		// (get) Token: 0x0600B530 RID: 46384 RVA: 0x0043AAF6 File Offset: 0x00438CF6
		// (set) Token: 0x0600B531 RID: 46385 RVA: 0x0043AAFE File Offset: 0x00438CFE
		[JsonProperty(PropertyName = "total_mods")]
		public int? TotalMods { get; set; }
	}
}
