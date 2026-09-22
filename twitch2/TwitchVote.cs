using System;
using System.Collections.Generic;
using UnityEngine;

namespace Twitch
{
	// Token: 0x02001868 RID: 6248
	public class TwitchVote
	{
		// Token: 0x1700179F RID: 6047
		// (get) Token: 0x0600C0FE RID: 49406 RVA: 0x00479754 File Offset: 0x00477954
		public string VoteDescription
		{
			get
			{
				switch (this.DisplayType)
				{
				case TwitchVote.VoteDisplayTypes.Single:
				case TwitchVote.VoteDisplayTypes.Special:
					return this.Title;
				case TwitchVote.VoteDisplayTypes.GoodBad:
					return this.Title + " / " + this.VoteLine1;
				case TwitchVote.VoteDisplayTypes.HordeBuffed:
					return this.Title + " (" + this.VoteLine1 + ")";
				default:
					return "";
				}
			}
		}

		// Token: 0x170017A0 RID: 6048
		// (get) Token: 0x0600C0FF RID: 49407 RVA: 0x004797C0 File Offset: 0x004779C0
		public string VoteHeight
		{
			get
			{
				TwitchVote.VoteDisplayTypes displayType = this.DisplayType;
				if (displayType == TwitchVote.VoteDisplayTypes.Single || displayType == TwitchVote.VoteDisplayTypes.Special)
				{
					return "-50";
				}
				return "-90";
			}
		}

		// Token: 0x0600C100 RID: 49408 RVA: 0x004797E6 File Offset: 0x004779E6
		public bool IsInPreset(TwitchVotePreset preset)
		{
			return (this.PresetNames == null && !preset.IsEmpty) || (this.PresetNames != null && this.PresetNames.Contains(preset.Name));
		}

		// Token: 0x0600C101 RID: 49409 RVA: 0x00479818 File Offset: 0x00477A18
		public bool CanUse(int hour, int gamestage, EntityPlayer player)
		{
			if ((this.StartGameStage != -1 && this.StartGameStage > gamestage) || (this.EndGameStage != -1 && this.EndGameStage < gamestage) || hour < this.AllowedStartHour || hour > this.AllowedEndHour || (this.MaxTimesPerDay != -1 && this.MaxTimesPerDay <= this.CurrentDayCount))
			{
				return false;
			}
			if (this.tempCooldown > 0f && TwitchManager.Current.CurrentUnityTime - this.tempCooldownSet < this.tempCooldown)
			{
				return false;
			}
			this.tempCooldown = 0f;
			this.tempCooldownSet = 0f;
			if (this.TwitchRequirements == null)
			{
				return true;
			}
			for (int i = 0; i < this.TwitchRequirements.Count; i++)
			{
				if (!this.TwitchRequirements[i].CanPerform(player))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600C102 RID: 49410 RVA: 0x004798F8 File Offset: 0x00477AF8
		public virtual void ParseProperties(DynamicProperties properties)
		{
			this.Properties = properties;
			string text = "";
			string text2 = "";
			properties.ParseLocalizedString(TwitchVote.PropTitleVarKey, ref text);
			properties.ParseLocalizedString(TwitchVote.PropDisplayVarKey, ref text2);
			if (text == "" && text2 != "")
			{
				text = text2;
			}
			else if (text != "" && text2 == "")
			{
				text2 = text;
			}
			properties.ParseLocalizedString(TwitchVote.PropTitleKey, ref this.Title);
			if (this.Title == "")
			{
				properties.ParseString(TwitchVote.PropTitle, ref this.Title);
			}
			string text3 = "";
			properties.ParseLocalizedString(TwitchVote.PropTitleFormatKey, ref text3);
			if (text3 != "")
			{
				this.Title = string.Format(text3, text);
			}
			properties.ParseLocalizedString(TwitchVote.PropDescriptionKey, ref this.Description);
			if (this.Description == "")
			{
				properties.ParseString(TwitchVote.PropDescription, ref this.Description);
			}
			text3 = "";
			properties.ParseLocalizedString(TwitchVote.PropDescriptionFormatKey, ref text3);
			if (text3 != "")
			{
				this.Description = string.Format(text3, text);
			}
			if (properties.Values.ContainsKey(TwitchVote.PropDisplayKey))
			{
				this.Display = Localization.Get(properties.Values[TwitchVote.PropDisplayKey], false, null);
			}
			else
			{
				properties.ParseString(TwitchVote.PropDisplay, ref this.Display);
			}
			text3 = "";
			properties.ParseLocalizedString(TwitchVote.PropDisplayFormatKey, ref text3);
			if (text3 != "")
			{
				this.Display = string.Format(text3, text2);
			}
			properties.ParseString(TwitchVote.PropEventName, ref this.GameEvent);
			properties.ParseString(TwitchVote.PropGroup, ref this.Group);
			properties.ParseInt(TwitchVote.PropStartGameStage, ref this.StartGameStage);
			properties.ParseInt(TwitchVote.PropEndGameStage, ref this.EndGameStage);
			properties.ParseInt(TwitchVote.PropAllowedStartHour, ref this.AllowedStartHour);
			properties.ParseInt(TwitchVote.PropAllowedEndHour, ref this.AllowedEndHour);
			properties.ParseString(TwitchVote.PropVoteLine1, ref this.VoteLine1);
			properties.ParseString(TwitchVote.PropVoteLine2, ref this.VoteLine2);
			properties.ParseEnum<TwitchVote.VoteDisplayTypes>(TwitchVote.PropDisplayType, ref this.DisplayType);
			properties.ParseInt(TwitchVote.PropMaxTimesPerDay, ref this.MaxTimesPerDay);
			string text4 = "";
			properties.ParseString(TwitchVote.PropVoteType, ref text4);
			if (text4 != "")
			{
				this.VoteTypes = text4.Split(',', StringSplitOptions.None);
				this.MainVoteType = TwitchManager.Current.VotingManager.GetVoteType(this.VoteTypes[0]);
			}
			properties.ParseBool(TwitchVote.PropEnabled, ref this.Enabled);
			this.OriginalEnabled = this.Enabled;
			properties.ParseString(TwitchVote.PropTitleColor, ref this.TitleColor);
			if (this.Display == "")
			{
				this.Display = this.Title;
			}
			this.Properties.ParseLocalizedString(TwitchVote.PropVoteLine1Key, ref this.VoteLine1);
			this.Properties.ParseLocalizedString(TwitchVote.PropVoteLine2Key, ref this.VoteLine2);
			this.VoteTip = this.Description;
			this.Properties.ParseString(TwitchVote.PropVoteTip, ref this.VoteTip);
			this.Properties.ParseLocalizedString(TwitchVote.PropVoteTipKey, ref this.VoteTip);
			if (properties.Values.ContainsKey(TwitchVote.PropPresets))
			{
				this.PresetNames = new List<string>();
				this.PresetNames.AddRange(properties.Values[TwitchVote.PropPresets].Split(',', StringSplitOptions.None));
			}
			if (!GameEventManager.GameEventSequences.ContainsKey(this.GameEvent))
			{
				Debug.LogError(string.Format("TwitchVote: Game Event Sequence '{0}' does not exist!", this.GameEvent));
			}
		}

		// Token: 0x0600C103 RID: 49411 RVA: 0x00479CAA File Offset: 0x00477EAA
		public void AddCooldownAddition(TwitchActionCooldownAddition newCooldown)
		{
			if (this.CooldownAdditions == null)
			{
				this.CooldownAdditions = new List<TwitchActionCooldownAddition>();
			}
			this.CooldownAdditions.Add(newCooldown);
		}

		// Token: 0x0600C104 RID: 49412 RVA: 0x00479CCB File Offset: 0x00477ECB
		public void AddRequirement(BaseTwitchRequirement requirement)
		{
			if (this.TwitchRequirements == null)
			{
				this.TwitchRequirements = new List<BaseTwitchRequirement>();
			}
			this.TwitchRequirements.Add(requirement);
		}

		// Token: 0x0600C105 RID: 49413 RVA: 0x00479CEC File Offset: 0x00477EEC
		public void HandleVoteComplete()
		{
			if (this.CooldownAdditions != null)
			{
				float actionCooldownModifier = TwitchManager.Current.ActionCooldownModifier;
				for (int i = 0; i < this.CooldownAdditions.Count; i++)
				{
					TwitchActionCooldownAddition twitchActionCooldownAddition = this.CooldownAdditions[i];
					if (twitchActionCooldownAddition.IsAction && TwitchActionManager.TwitchActions.ContainsKey(twitchActionCooldownAddition.ActionName))
					{
						TwitchAction twitchAction = TwitchActionManager.TwitchActions[twitchActionCooldownAddition.ActionName];
						twitchAction.tempCooldown = twitchActionCooldownAddition.CooldownTime * actionCooldownModifier;
						twitchAction.tempCooldownSet = Time.time;
					}
					else if (!twitchActionCooldownAddition.IsAction && TwitchActionManager.TwitchVotes.ContainsKey(twitchActionCooldownAddition.ActionName))
					{
						TwitchVote twitchVote = TwitchActionManager.TwitchVotes[twitchActionCooldownAddition.ActionName];
						twitchVote.tempCooldown = twitchActionCooldownAddition.CooldownTime * actionCooldownModifier;
						twitchVote.tempCooldownSet = Time.time;
					}
				}
			}
		}

		// Token: 0x040091D8 RID: 37336
		public static string PropTitleVarKey = "title_var_key";

		// Token: 0x040091D9 RID: 37337
		public static string PropDisplayVarKey = "display_var_key";

		// Token: 0x040091DA RID: 37338
		public static string PropTitle = "title";

		// Token: 0x040091DB RID: 37339
		public static string PropTitleKey = "title_key";

		// Token: 0x040091DC RID: 37340
		public static string PropDescription = "description";

		// Token: 0x040091DD RID: 37341
		public static string PropDescriptionKey = "description_key";

		// Token: 0x040091DE RID: 37342
		public static string PropDisplay = "display";

		// Token: 0x040091DF RID: 37343
		public static string PropDisplayKey = "display_key";

		// Token: 0x040091E0 RID: 37344
		public static string PropEventName = "event_name";

		// Token: 0x040091E1 RID: 37345
		public static string PropVoteType = "vote_type";

		// Token: 0x040091E2 RID: 37346
		public static string PropGroup = "group";

		// Token: 0x040091E3 RID: 37347
		public static string PropTitleFormatKey = "title_format_key";

		// Token: 0x040091E4 RID: 37348
		public static string PropDescriptionFormatKey = "description_format_key";

		// Token: 0x040091E5 RID: 37349
		public static string PropDisplayFormatKey = "display_format_key";

		// Token: 0x040091E6 RID: 37350
		public static string PropStartGameStage = "start_gamestage";

		// Token: 0x040091E7 RID: 37351
		public static string PropEndGameStage = "end_gamestage";

		// Token: 0x040091E8 RID: 37352
		public static string PropAllowedStartHour = "allowed_start_hour";

		// Token: 0x040091E9 RID: 37353
		public static string PropAllowedEndHour = "allowed_end_hour";

		// Token: 0x040091EA RID: 37354
		public static string PropDisplayType = "display_type";

		// Token: 0x040091EB RID: 37355
		public static string PropTitleColor = "title_color";

		// Token: 0x040091EC RID: 37356
		public static string PropVoteLine1 = "line1_desc";

		// Token: 0x040091ED RID: 37357
		public static string PropVoteLine1Key = "line1_desc_key";

		// Token: 0x040091EE RID: 37358
		public static string PropVoteLine2 = "line2_desc";

		// Token: 0x040091EF RID: 37359
		public static string PropVoteLine2Key = "line2_desc_key";

		// Token: 0x040091F0 RID: 37360
		public static string PropEnabled = "enabled";

		// Token: 0x040091F1 RID: 37361
		public static string PropMaxTimesPerDay = "max_times_per_day";

		// Token: 0x040091F2 RID: 37362
		public static string PropVoteTip = "vote_tip";

		// Token: 0x040091F3 RID: 37363
		public static string PropVoteTipKey = "vote_tip_key";

		// Token: 0x040091F4 RID: 37364
		public static string PropPresets = "presets";

		// Token: 0x040091F5 RID: 37365
		public static HashSet<string> ExtendsExcludes = new HashSet<string>
		{
			TwitchVote.PropStartGameStage,
			TwitchVote.PropEndGameStage
		};

		// Token: 0x040091F6 RID: 37366
		public string VoteName;

		// Token: 0x040091F7 RID: 37367
		public string Title;

		// Token: 0x040091F8 RID: 37368
		public string Description;

		// Token: 0x040091F9 RID: 37369
		public string Display = "";

		// Token: 0x040091FA RID: 37370
		public string GameEvent;

		// Token: 0x040091FB RID: 37371
		public string[] VoteTypes;

		// Token: 0x040091FC RID: 37372
		public TwitchVoteType MainVoteType;

		// Token: 0x040091FD RID: 37373
		public string Group = "";

		// Token: 0x040091FE RID: 37374
		public bool Enabled = true;

		// Token: 0x040091FF RID: 37375
		public bool OriginalEnabled;

		// Token: 0x04009200 RID: 37376
		public string TitleColor = "";

		// Token: 0x04009201 RID: 37377
		public int StartGameStage = -1;

		// Token: 0x04009202 RID: 37378
		public int EndGameStage = -1;

		// Token: 0x04009203 RID: 37379
		public int AllowedStartHour;

		// Token: 0x04009204 RID: 37380
		public int AllowedEndHour = 24;

		// Token: 0x04009205 RID: 37381
		public string VoteLine1 = "";

		// Token: 0x04009206 RID: 37382
		public string VoteLine2 = "";

		// Token: 0x04009207 RID: 37383
		public int MaxTimesPerDay = -1;

		// Token: 0x04009208 RID: 37384
		public int CurrentDayCount;

		// Token: 0x04009209 RID: 37385
		public float tempCooldownSet;

		// Token: 0x0400920A RID: 37386
		public float tempCooldown;

		// Token: 0x0400920B RID: 37387
		public string VoteTip = "";

		// Token: 0x0400920C RID: 37388
		public DynamicProperties Properties;

		// Token: 0x0400920D RID: 37389
		public List<TwitchActionCooldownAddition> CooldownAdditions;

		// Token: 0x0400920E RID: 37390
		public List<BaseTwitchRequirement> TwitchRequirements;

		// Token: 0x0400920F RID: 37391
		public TwitchVote.VoteDisplayTypes DisplayType = TwitchVote.VoteDisplayTypes.GoodBad;

		// Token: 0x04009210 RID: 37392
		public List<string> PresetNames;

		// Token: 0x02001869 RID: 6249
		public enum VoteDisplayTypes
		{
			// Token: 0x04009212 RID: 37394
			Single,
			// Token: 0x04009213 RID: 37395
			GoodBad,
			// Token: 0x04009214 RID: 37396
			Special,
			// Token: 0x04009215 RID: 37397
			HordeBuffed
		}
	}
}
