using System;

namespace Audio
{
	// Token: 0x02001B13 RID: 6931
	public class StrAudioClip
	{
		// Token: 0x04009E4E RID: 40526
		public const string ItemCollected = "item_pickup";

		// Token: 0x04009E4F RID: 40527
		public const string ItemPlantCollected = "item_plant_pickup";

		// Token: 0x04009E50 RID: 40528
		public const string EntityHitsGround = "entityhitsground";

		// Token: 0x04009E51 RID: 40529
		public const string ItemDropped = "itemdropped";

		// Token: 0x04009E52 RID: 40530
		public const string OpenDoor = "open_door_wood";

		// Token: 0x04009E53 RID: 40531
		public const string CloseDoor = "close_door_wood";

		// Token: 0x04009E54 RID: 40532
		public const string FallingIntoWater = "waterfallinginto";

		// Token: 0x04009E55 RID: 40533
		public const string BuildingCompleted = "placeblock";

		// Token: 0x04009E56 RID: 40534
		public const string RotateBlock = "rotateblock";

		// Token: 0x04009E57 RID: 40535
		public const string TrunkBreaks = "trunkbreak";

		// Token: 0x04009E58 RID: 40536
		public const string TrunkFallImpact = "treefallimpact";

		// Token: 0x04009E59 RID: 40537
		public const string FlashlightToggle = "flashlight_toggle";

		// Token: 0x04009E5A RID: 40538
		public const string GenericHolster = "generic_holster";

		// Token: 0x04009E5B RID: 40539
		public const string GenericUnholster = "generic_unholster";

		// Token: 0x04009E5C RID: 40540
		public const string MissingItemToRepair = "missingitemtorepair";

		// Token: 0x04009E5D RID: 40541
		public const string CraftingClick = "craft_click_craft";

		// Token: 0x04009E5E RID: 40542
		public const string RecipeUnlocked = "recipe_unlocked";

		// Token: 0x04009E5F RID: 40543
		public const string OpenInventory = "open_inventory";

		// Token: 0x04009E60 RID: 40544
		public const string CloseInventory = "close_inventory";

		// Token: 0x04009E61 RID: 40545
		public const string CampfireOpen = "campfire_open";

		// Token: 0x04009E62 RID: 40546
		public const string CampfireClose = "campfire_close";

		// Token: 0x04009E63 RID: 40547
		public const string CampfireCookClick = "campfire_cook_click";

		// Token: 0x04009E64 RID: 40548
		public const string ForgeOpen = "forge_open";

		// Token: 0x04009E65 RID: 40549
		public const string ForgeClose = "forge_close";

		// Token: 0x04009E66 RID: 40550
		public const string ForgeSmeltClick = "forge_smelt_click";

		// Token: 0x04009E67 RID: 40551
		public const string ForgeBurn = "forge_burn_fuel";

		// Token: 0x04009E68 RID: 40552
		public const string ForgeFireDie = "forge_fire_die";

		// Token: 0x04009E69 RID: 40553
		public const string CementMixerOpen = "cement_mixer_open";

		// Token: 0x04009E6A RID: 40554
		public const string CementMixerClose = "cement_mixer_close";

		// Token: 0x04009E6B RID: 40555
		public const string CementMixerClick = "cement_mixer_start_click";

		// Token: 0x04009E6C RID: 40556
		public const string CraftComplete = "craft_complete_item";

		// Token: 0x04009E6D RID: 40557
		public const string CampfireComplete = "campfire_complete_item";

		// Token: 0x04009E6E RID: 40558
		public const string ForgeComplete = "forge_item_complete";

		// Token: 0x04009E6F RID: 40559
		public const string CementMixerComplete = "cement_mixer_complete";

		// Token: 0x04009E70 RID: 40560
		public const string ChemStationOpen = "chem_station_open";

		// Token: 0x04009E71 RID: 40561
		public const string ChemStationClose = "chem_station_close";

		// Token: 0x04009E72 RID: 40562
		public const string ChemStationClick = "chem_station_mix_click";

		// Token: 0x04009E73 RID: 40563
		public const string ChemStationComplete = "chem_station_complete_item";

		// Token: 0x04009E74 RID: 40564
		public const string QuestNoteOffered = "quest_note_offer";

		// Token: 0x04009E75 RID: 40565
		public const string QuestNoteDeclined = "quest_note_decline";

		// Token: 0x04009E76 RID: 40566
		public const string QuestStarted = "quest_started";

		// Token: 0x04009E77 RID: 40567
		public const string QuestFailed = "quest_failed";

		// Token: 0x04009E78 RID: 40568
		public const string QuestCompleted = "quest_subtask_complete";

		// Token: 0x04009E79 RID: 40569
		public const string QuestObjective = "quest_objective_complete";

		// Token: 0x04009E7A RID: 40570
		public const string QuestChainCompleted = "quest_master_complete";

		// Token: 0x04009E7B RID: 40571
		public const string SkillPurchase = "ui_skill_purchase";

		// Token: 0x04009E7C RID: 40572
		public const string TraderPurchase = "ui_trader_purchase";

		// Token: 0x04009E7D RID: 40573
		public const string VendingPurchase = "ui_vending_purchase";

		// Token: 0x04009E7E RID: 40574
		public const string VendingOpen = "open_vending";

		// Token: 0x04009E7F RID: 40575
		public const string VendingClose = "close_vending";

		// Token: 0x04009E80 RID: 40576
		public const string UITab = "ui_tab";

		// Token: 0x04009E81 RID: 40577
		public const string UIHover = "ui_hover";

		// Token: 0x04009E82 RID: 40578
		public const string UIDenied = "ui_denied";

		// Token: 0x04009E83 RID: 40579
		public const string MapZoomIn = "map_zoom_in";

		// Token: 0x04009E84 RID: 40580
		public const string MapZoomOut = "map_zoom_out";

		// Token: 0x04009E85 RID: 40581
		public const string WaypointAdd = "ui_waypoint_add";

		// Token: 0x04009E86 RID: 40582
		public const string WaypointDelete = "ui_waypoint_delete";

		// Token: 0x04009E87 RID: 40583
		public const string PickupItem = "craft_take_item";

		// Token: 0x04009E88 RID: 40584
		public const string PlaceItem = "craft_place_item";

		// Token: 0x04009E89 RID: 40585
		public const string SignOpen = "open_sign";

		// Token: 0x04009E8A RID: 40586
		public const string SignClose = "close_sign";

		// Token: 0x04009E8B RID: 40587
		public const string BatteryBankStart = "batterybank_start";

		// Token: 0x04009E8C RID: 40588
		public const string BatteryBankStop = "batterybank_stop";

		// Token: 0x04009E8D RID: 40589
		public const string GeneratorStart = "generator_start";

		// Token: 0x04009E8E RID: 40590
		public const string GeneratorStop = "generator_stop";

		// Token: 0x04009E8F RID: 40591
		public const string SolarPanelStart = "solarpanel_on";

		// Token: 0x04009E90 RID: 40592
		public const string SolarPanelStop = "solarpanel_off";

		// Token: 0x04009E91 RID: 40593
		public const string SwitchOn = "switch_up";

		// Token: 0x04009E92 RID: 40594
		public const string SwitchOff = "switch_down";

		// Token: 0x04009E93 RID: 40595
		public const string WireConnectLive = "wire_live_connect";

		// Token: 0x04009E94 RID: 40596
		public const string WireConnectDead = "wire_dead_connect";

		// Token: 0x04009E95 RID: 40597
		public const string WireBreakLive = "wire_live_break";

		// Token: 0x04009E96 RID: 40598
		public const string WireBreakDead = "wire_dead_break";

		// Token: 0x04009E97 RID: 40599
		public const string PressurePlateDown = "pressureplate_down";

		// Token: 0x04009E98 RID: 40600
		public const string PressurePlateUp = "pressureplate_up";

		// Token: 0x04009E99 RID: 40601
		public const string MotionSensorTrigger = "motion_sensor_trigger";

		// Token: 0x04009E9A RID: 40602
		public const string TripWireTrigger = "trip_wire_trigger";

		// Token: 0x04009E9B RID: 40603
		public const string LightOn = "light_on";

		// Token: 0x04009E9C RID: 40604
		public const string LightOff = "light_off";

		// Token: 0x04009E9D RID: 40605
		public const string TimerRelayStart = "timer_start";

		// Token: 0x04009E9E RID: 40606
		public const string TimerRelayStop = "timer_stop";

		// Token: 0x04009E9F RID: 40607
		public const string ItemBreak = "itembreak";

		// Token: 0x04009EA0 RID: 40608
		public const string TwitchNoAttack = "twitch_no_attack";

		// Token: 0x04009EA1 RID: 40609
		public const string TwitchVoteAdded = "twitch_vote_received";

		// Token: 0x04009EA2 RID: 40610
		public const string TwitchVoteStarted = "twitch_vote_started";

		// Token: 0x04009EA3 RID: 40611
		public const string TwitchVoteEnded = "twitch_vote_ended";

		// Token: 0x04009EA4 RID: 40612
		public const string TwitchInflate = "twitch_bighead_inflate";

		// Token: 0x04009EA5 RID: 40613
		public const string TwitchDeflate = "twitch_bighead_deflate";

		// Token: 0x04009EA6 RID: 40614
		public const string TwitchCelebrate = "twitch_celebrate";

		// Token: 0x04009EA7 RID: 40615
		public const string TwitchBalloonPop = "twitch_baseball_balloon_pop";

		// Token: 0x04009EA8 RID: 40616
		public const string TwitchBalloonSpawn = "twitch_balloon_spawn";

		// Token: 0x04009EA9 RID: 40617
		public const string TwitchBalloonDespawn = "twitch_balloon_despawn";

		// Token: 0x04009EAA RID: 40618
		public const string TwitchRefund = "twitch_refund";

		// Token: 0x04009EAB RID: 40619
		public const string TwitchPausedActions = "twitch_pause";

		// Token: 0x04009EAC RID: 40620
		public const string TwitchUnPausedActions = "twitch_unpause";

		// Token: 0x04009EAD RID: 40621
		public const string PartyInviteRecieve = "party_invite_receive";

		// Token: 0x04009EAE RID: 40622
		public const string PartyJoin = "party_join";

		// Token: 0x04009EAF RID: 40623
		public const string PartyLeave = "party_leave";

		// Token: 0x04009EB0 RID: 40624
		public const string PartyMemberJoin = "party_member_join";

		// Token: 0x04009EB1 RID: 40625
		public const string PartyMemberLeave = "party_member_leave";

		// Token: 0x04009EB2 RID: 40626
		public const string ChallengeTrack = "ui_challenge_track";

		// Token: 0x04009EB3 RID: 40627
		public const string ChallengeRedeem = "ui_challenge_redeem";

		// Token: 0x04009EB4 RID: 40628
		public const string ChallengeComplete = "ui_challenge_complete";

		// Token: 0x04009EB5 RID: 40629
		public const string ChallengeCompleteRow = "ui_challenge_complete_row";

		// Token: 0x04009EB6 RID: 40630
		public const string ChallengeUnhideRow = "ui_challenge_unhide_row";

		// Token: 0x04009EB7 RID: 40631
		public const string ChallengeObjectiveComplete = "ui_challenge_objective_complete";
	}
}
