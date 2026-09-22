using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200186E RID: 6254
	public class TwitchVoteType
	{
		// Token: 0x0600C115 RID: 49429 RVA: 0x0047A21E File Offset: 0x0047841E
		public bool IsInPreset(string preset)
		{
			return this.PresetNames == null || this.PresetNames.Contains(preset);
		}

		// Token: 0x0600C116 RID: 49430 RVA: 0x0047A236 File Offset: 0x00478436
		public bool CanUse()
		{
			return this.Enabled && (this.MaxTimesPerDay == -1 || this.MaxTimesPerDay > this.CurrentDayCount);
		}

		// Token: 0x0600C117 RID: 49431 RVA: 0x0047A25C File Offset: 0x0047845C
		public virtual void ParseProperties(DynamicProperties properties)
		{
			properties.ParseString(TwitchVoteType.PropTitle, ref this.Title);
			if (properties.Values.ContainsKey(TwitchVoteType.PropTitleKey))
			{
				this.Title = Localization.Get(properties.Values[TwitchVoteType.PropTitleKey], false, null);
			}
			properties.ParseString(TwitchVoteType.PropIcon, ref this.Icon);
			properties.ParseBool(TwitchVoteType.PropSpawnBlocked, ref this.SpawnBlocked);
			properties.ParseInt(TwitchVoteType.PropMaxTimesPerDay, ref this.MaxTimesPerDay);
			properties.ParseInt(TwitchVoteType.PropAllowedStartHour, ref this.AllowedStartHour);
			properties.ParseInt(TwitchVoteType.PropAllowedEndHour, ref this.AllowedEndHour);
			properties.ParseBool(TwitchVoteType.PropBloodMoonDay, ref this.BloodMoonDay);
			properties.ParseBool(TwitchVoteType.PropBloodMoonAllowed, ref this.BloodMoonAllowed);
			properties.ParseString(TwitchVoteType.PropGuaranteedGroups, ref this.GuaranteedGroup);
			properties.ParseBool(TwitchVoteType.PropCooldownOnEnd, ref this.CooldownOnEnd);
			properties.ParseBool(TwitchVoteType.PropUseMystery, ref this.UseMystery);
			properties.ParseBool(TwitchVoteType.PropActionLockout, ref this.ActionLockout);
			properties.ParseString(TwitchVoteType.PropGroup, ref this.Group);
			properties.ParseBool(TwitchVoteType.PropEnabled, ref this.Enabled);
			properties.ParseBool(TwitchVoteType.PropAllowedWithActions, ref this.AllowedWithActions);
			properties.ParseInt(TwitchVoteType.PropVoteChoiceCount, ref this.VoteChoiceCount);
			properties.ParseBool(TwitchVoteType.PropIsBoss, ref this.IsBoss);
			properties.ParseBool(TwitchVoteType.PropManualStart, ref this.ManualStart);
			if (properties.Values.ContainsKey(TwitchVoteType.PropPresets))
			{
				this.PresetNames = new List<string>();
				this.PresetNames.AddRange(properties.Values[TwitchVoteType.PropPresets].Split(',', StringSplitOptions.None));
			}
		}

		// Token: 0x0400922C RID: 37420
		public static string PropTitle = "title";

		// Token: 0x0400922D RID: 37421
		public static string PropTitleKey = "title_key";

		// Token: 0x0400922E RID: 37422
		public static string PropSpawnBlocked = "spawn_blocked";

		// Token: 0x0400922F RID: 37423
		public static string PropExcludeTimeIndex = "exclude_time_index";

		// Token: 0x04009230 RID: 37424
		public static string PropMaxTimesPerDay = "max_times_per_day";

		// Token: 0x04009231 RID: 37425
		public static string PropAllowedStartHour = "allowed_start_hour";

		// Token: 0x04009232 RID: 37426
		public static string PropAllowedEndHour = "allowed_end_hour";

		// Token: 0x04009233 RID: 37427
		public static string PropBloodMoonDay = "blood_moon_day";

		// Token: 0x04009234 RID: 37428
		public static string PropBloodMoonAllowed = "blood_moon_allowed";

		// Token: 0x04009235 RID: 37429
		public static string PropGuaranteedGroups = "guaranteed_group";

		// Token: 0x04009236 RID: 37430
		public static string PropCooldownOnEnd = "cooldown_on_end";

		// Token: 0x04009237 RID: 37431
		public static string PropUseMystery = "use_mystery";

		// Token: 0x04009238 RID: 37432
		public static string PropActionLockout = "action_lockout";

		// Token: 0x04009239 RID: 37433
		public static string PropGroup = "group";

		// Token: 0x0400923A RID: 37434
		public static string PropEnabled = "enabled";

		// Token: 0x0400923B RID: 37435
		public static string PropVoteChoiceCount = "vote_choice_count";

		// Token: 0x0400923C RID: 37436
		public static string PropAllowedWithActions = "allowed_with_actions";

		// Token: 0x0400923D RID: 37437
		public static string PropIsBoss = "is_boss";

		// Token: 0x0400923E RID: 37438
		public static string PropManualStart = "manual_start";

		// Token: 0x0400923F RID: 37439
		public static string PropIcon = "icon";

		// Token: 0x04009240 RID: 37440
		public static string PropPresets = "presets";

		// Token: 0x04009241 RID: 37441
		public string Name;

		// Token: 0x04009242 RID: 37442
		public string Title;

		// Token: 0x04009243 RID: 37443
		public string Icon;

		// Token: 0x04009244 RID: 37444
		public string Group;

		// Token: 0x04009245 RID: 37445
		public bool SpawnBlocked = true;

		// Token: 0x04009246 RID: 37446
		public bool BloodMoonDay = true;

		// Token: 0x04009247 RID: 37447
		public bool BloodMoonAllowed = true;

		// Token: 0x04009248 RID: 37448
		public bool CooldownOnEnd;

		// Token: 0x04009249 RID: 37449
		public bool UseMystery;

		// Token: 0x0400924A RID: 37450
		public bool ActionLockout;

		// Token: 0x0400924B RID: 37451
		public bool AllowedWithActions = true;

		// Token: 0x0400924C RID: 37452
		public int MaxTimesPerDay = -1;

		// Token: 0x0400924D RID: 37453
		public int AllowedStartHour;

		// Token: 0x0400924E RID: 37454
		public int AllowedEndHour = 24;

		// Token: 0x0400924F RID: 37455
		public int VoteChoiceCount = 3;

		// Token: 0x04009250 RID: 37456
		public int CurrentDayCount;

		// Token: 0x04009251 RID: 37457
		public string GuaranteedGroup = "";

		// Token: 0x04009252 RID: 37458
		public bool Enabled = true;

		// Token: 0x04009253 RID: 37459
		public bool ManualStart;

		// Token: 0x04009254 RID: 37460
		public bool IsBoss;

		// Token: 0x04009255 RID: 37461
		public List<string> PresetNames;
	}
}
