using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UniLinq;

namespace Challenges
{
	// Token: 0x020018F6 RID: 6390
	public class ChallengeGroup
	{
		// Token: 0x0600C4FC RID: 50428 RVA: 0x0048C34F File Offset: 0x0048A54F
		public ChallengeGroup(string name)
		{
			this.Name = name;
		}

		// Token: 0x0600C4FD RID: 50429 RVA: 0x0048C384 File Offset: 0x0048A584
		public static ChallengeGroup NewClass(string id)
		{
			if (ChallengeGroup.s_ChallengeGroups.ContainsKey(id))
			{
				return null;
			}
			ChallengeGroup challengeGroup = new ChallengeGroup(id.ToLower());
			ChallengeGroup.s_ChallengeGroups[id] = challengeGroup;
			return challengeGroup;
		}

		// Token: 0x0600C4FE RID: 50430 RVA: 0x0048C3B9 File Offset: 0x0048A5B9
		public static ChallengeGroup GetGroup(string id)
		{
			if (!ChallengeGroup.s_ChallengeGroups.ContainsKey(id))
			{
				return null;
			}
			return ChallengeGroup.s_ChallengeGroups[id];
		}

		// Token: 0x0600C4FF RID: 50431 RVA: 0x0048C3D5 File Offset: 0x0048A5D5
		public void AddChallengeCount(string tag, int count)
		{
			if (this.ChallengeCounts == null)
			{
				this.ChallengeCounts = new List<ChallengeGroup.ChallengeCount>();
			}
			this.ChallengeCounts.Add(new ChallengeGroup.ChallengeCount
			{
				Tags = FastTags<TagGroup.Global>.Parse(tag),
				Count = count
			});
		}

		// Token: 0x0600C500 RID: 50432 RVA: 0x0048C40D File Offset: 0x0048A60D
		public void AddChallenge(ChallengeClass challenge)
		{
			this.ChallengeClasses.Add(challenge);
		}

		// Token: 0x0600C501 RID: 50433 RVA: 0x0048C41C File Offset: 0x0048A61C
		public void ParseElement(XElement e)
		{
			if (e.HasAttribute("title_key"))
			{
				this.Title = Localization.Get(e.GetAttribute("title_key"), false, null);
			}
			else if (e.HasAttribute("title"))
			{
				this.Title = e.GetAttribute("title");
			}
			else
			{
				this.Title = this.Name;
			}
			if (e.HasAttribute("category"))
			{
				this.Category = e.GetAttribute("category");
			}
			if (e.HasAttribute("reward_event"))
			{
				this.RewardEvent = e.GetAttribute("reward_event");
			}
			if (e.HasAttribute("reward_text_key"))
			{
				this.RewardText = Localization.Get(e.GetAttribute("reward_text_key"), false, null);
			}
			else if (e.HasAttribute("reward_text"))
			{
				this.RewardText = e.GetAttribute("reward_text");
			}
			if (e.HasAttribute("objective_text_key"))
			{
				this.ObjectiveText = Localization.Get(e.GetAttribute("objective_text_key"), false, null);
			}
			else if (e.HasAttribute("objective_text"))
			{
				this.ObjectiveText = e.GetAttribute("objective_text");
			}
			if (e.HasAttribute("active_challenge_count"))
			{
				this.ActiveChallengeCount = StringParsers.ParseSInt32(e.GetAttribute("active_challenge_count"), 0, -1, NumberStyles.Integer);
			}
			if (e.HasAttribute("day_reset"))
			{
				this.DayReset = StringParsers.ParseSInt32(e.GetAttribute("day_reset"), 0, -1, NumberStyles.Integer);
			}
			if (e.HasAttribute("is_random"))
			{
				this.IsRandom = StringParsers.ParseBool(e.GetAttribute("is_random"), 0, -1, true);
			}
			if (e.HasAttribute("link_challenges"))
			{
				this.LinkChallenges = StringParsers.ParseBool(e.GetAttribute("link_challenges"), 0, -1, true);
			}
			if (e.HasAttribute("hidden_by"))
			{
				this.HiddenBy = e.GetAttribute("hidden_by");
			}
			if (e.HasAttribute("is_intro"))
			{
				this.IsIntro = StringParsers.ParseBool(e.GetAttribute("is_intro"), 0, -1, true);
			}
		}

		// Token: 0x1700184B RID: 6219
		// (get) Token: 0x0600C502 RID: 50434 RVA: 0x0048C6AA File Offset: 0x0048A8AA
		public bool IsActive
		{
			get
			{
				return (!this.IsIntro || ChallengeJournal.IntroChallengesEnabled) && ChallengeCategory.s_ChallengeCategories[this.Category].IsActive;
			}
		}

		// Token: 0x0600C503 RID: 50435 RVA: 0x0048C6D4 File Offset: 0x0048A8D4
		public bool IsVisible(EntityPlayer player)
		{
			if (!ChallengeJournal.IntroChallengesEnabled && this.IsIntro)
			{
				return false;
			}
			if (this.HiddenBy == "")
			{
				return true;
			}
			if (!ChallengeCategory.s_ChallengeCategories[this.Category].CanShow(player))
			{
				return false;
			}
			if (ChallengeGroup.s_ChallengeGroups.ContainsKey(this.HiddenBy))
			{
				ChallengeGroup challengeGroup = ChallengeGroup.s_ChallengeGroups[this.HiddenBy];
				if (challengeGroup.IsActive)
				{
					return challengeGroup.IsComplete;
				}
			}
			return true;
		}

		// Token: 0x0600C504 RID: 50436 RVA: 0x0048C753 File Offset: 0x0048A953
		public bool HasEventsOrPassives()
		{
			return this.Effects != null;
		}

		// Token: 0x0600C505 RID: 50437 RVA: 0x0048C75E File Offset: 0x0048A95E
		public void ModifyValue(EntityAlive _ea, PassiveEffects _effect, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>))
		{
			if (this.Effects == null || !this.IsComplete)
			{
				return;
			}
			this.Effects.ModifyValue(_ea, _effect, ref _base_value, ref _perc_value, 0f, _tags, 1);
		}

		// Token: 0x0600C506 RID: 50438 RVA: 0x0048C78C File Offset: 0x0048A98C
		public List<ChallengeClass> GetChallengeClassesForCreate()
		{
			List<ChallengeClass> list = new List<ChallengeClass>();
			for (int i = 0; i < this.ChallengeClasses.Count; i++)
			{
				list.Add(this.ChallengeClasses[i]);
			}
			GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
			for (int j = 0; j < list.Count * 2; j++)
			{
				int index = gameRandom.RandomRange(list.Count);
				int index2 = gameRandom.RandomRange(list.Count);
				ChallengeClass value = list[index];
				list[index] = list[index2];
				list[index2] = value;
			}
			if (this.ChallengeCounts != null)
			{
				for (int k = 0; k < this.ChallengeCounts.Count; k++)
				{
					ChallengeGroup.ChallengeCount challengeCount = this.ChallengeCounts[k];
					int num = challengeCount.Count;
					for (int l = list.Count - 1; l >= 0; l--)
					{
						if (list[l].Tags.Test_AnySet(challengeCount.Tags))
						{
							if (num == 0)
							{
								list.RemoveAt(l);
							}
							else
							{
								num--;
							}
						}
					}
				}
				list = (from c in list
				orderby c.TagName
				select c).ToList<ChallengeClass>();
			}
			return list;
		}

		// Token: 0x04009537 RID: 38199
		public static Dictionary<string, ChallengeGroup> s_ChallengeGroups = new CaseInsensitiveStringDictionary<ChallengeGroup>();

		// Token: 0x04009538 RID: 38200
		public string Name;

		// Token: 0x04009539 RID: 38201
		public string Title;

		// Token: 0x0400953A RID: 38202
		public bool IsComplete;

		// Token: 0x0400953B RID: 38203
		public string RewardEvent;

		// Token: 0x0400953C RID: 38204
		public string RewardText;

		// Token: 0x0400953D RID: 38205
		public string ObjectiveText;

		// Token: 0x0400953E RID: 38206
		public bool IsRandom;

		// Token: 0x0400953F RID: 38207
		public bool IsIntro;

		// Token: 0x04009540 RID: 38208
		public int ActiveChallengeCount = 10;

		// Token: 0x04009541 RID: 38209
		public int DayReset = -1;

		// Token: 0x04009542 RID: 38210
		public bool LinkChallenges;

		// Token: 0x04009543 RID: 38211
		public string Category;

		// Token: 0x04009544 RID: 38212
		public string HiddenBy = "";

		// Token: 0x04009545 RID: 38213
		public bool UIDirty;

		// Token: 0x04009546 RID: 38214
		public List<ChallengeGroup.ChallengeCount> ChallengeCounts;

		// Token: 0x04009547 RID: 38215
		public List<ChallengeClass> ChallengeClasses = new List<ChallengeClass>();

		// Token: 0x04009548 RID: 38216
		public MinEffectController Effects;

		// Token: 0x020018F7 RID: 6391
		public class ChallengeCount
		{
			// Token: 0x04009549 RID: 38217
			public FastTags<TagGroup.Global> Tags;

			// Token: 0x0400954A RID: 38218
			public int Count;
		}
	}
}
