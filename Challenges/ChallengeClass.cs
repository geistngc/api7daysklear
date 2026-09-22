using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Platform;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020018F3 RID: 6387
	[Preserve]
	public class ChallengeClass
	{
		// Token: 0x17001849 RID: 6217
		// (get) Token: 0x0600C4E6 RID: 50406 RVA: 0x0048BA55 File Offset: 0x00489C55
		// (set) Token: 0x0600C4E7 RID: 50407 RVA: 0x0048BA5D File Offset: 0x00489C5D
		public int OrderIndex { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700184A RID: 6218
		// (get) Token: 0x0600C4E8 RID: 50408 RVA: 0x0048BA66 File Offset: 0x00489C66
		public bool HasNavType
		{
			get
			{
				return this.GetNavType() > ChallengeClass.UINavTypes.None;
			}
		}

		// Token: 0x0600C4E9 RID: 50409 RVA: 0x0048BA74 File Offset: 0x00489C74
		public ChallengeClass(string name)
		{
			this.Name = name;
			this.OrderIndex = ChallengeClass.nextIndex++;
		}

		// Token: 0x0600C4EA RID: 50410 RVA: 0x0048BAD0 File Offset: 0x00489CD0
		public static ChallengeClass NewClass(string id)
		{
			if (ChallengeClass.s_Challenges.ContainsKey(id))
			{
				return null;
			}
			ChallengeClass challengeClass = new ChallengeClass(id.ToLower());
			ChallengeClass.s_Challenges[id] = challengeClass;
			return challengeClass;
		}

		// Token: 0x0600C4EB RID: 50411 RVA: 0x0048BB05 File Offset: 0x00489D05
		public static void Cleanup()
		{
			ChallengeClass.s_Challenges.Clear();
		}

		// Token: 0x0600C4EC RID: 50412 RVA: 0x0048BB14 File Offset: 0x00489D14
		public static void InitChallenges()
		{
			foreach (string key in ChallengeClass.s_Challenges.Keys)
			{
				ChallengeClass.s_Challenges[key].Init();
			}
		}

		// Token: 0x0600C4ED RID: 50413 RVA: 0x0048BB74 File Offset: 0x00489D74
		[PublicizedFrom(EAccessModifier.Private)]
		public void Init()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				this.ObjectiveList[i].BaseInit();
				if (this.ObjectiveList[i].NeedsConstantUIUpdate)
				{
					this.NeedsConstantUIUpdate = true;
				}
			}
		}

		// Token: 0x0600C4EE RID: 50414 RVA: 0x0048BBC2 File Offset: 0x00489DC2
		public bool HasEventsOrPassives()
		{
			return this.Effects != null;
		}

		// Token: 0x0600C4EF RID: 50415 RVA: 0x0048BBCD File Offset: 0x00489DCD
		public void ModifyValue(EntityAlive _ea, PassiveEffects _effect, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>))
		{
			if (this.Effects == null)
			{
				return;
			}
			this.Effects.ModifyValue(_ea, _effect, ref _base_value, ref _perc_value, 0f, _tags, 1);
		}

		// Token: 0x0600C4F0 RID: 50416 RVA: 0x0048BBF0 File Offset: 0x00489DF0
		public static ChallengeClass GetChallenge(string name)
		{
			if (ChallengeClass.s_Challenges.ContainsKey(name))
			{
				return ChallengeClass.s_Challenges[name];
			}
			return null;
		}

		// Token: 0x0600C4F1 RID: 50417 RVA: 0x0048BC0C File Offset: 0x00489E0C
		public Challenge CreateChallenge(ChallengeJournal ownerJournal)
		{
			Challenge challenge = new Challenge();
			challenge.ChallengeClass = this;
			challenge.Owner = ownerJournal;
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i];
				BaseChallengeObjective baseChallengeObjective2 = baseChallengeObjective.Clone();
				baseChallengeObjective2.Owner = challenge;
				baseChallengeObjective2.IsRequirement = baseChallengeObjective.IsRequirement;
				baseChallengeObjective2.MaxCount = baseChallengeObjective.MaxCount;
				baseChallengeObjective2.ShowRequirements = baseChallengeObjective.ShowRequirements;
				baseChallengeObjective2.Biome = baseChallengeObjective.Biome;
				baseChallengeObjective2.HandleOnCreated();
				challenge.ObjectiveList.Add(baseChallengeObjective2);
			}
			return challenge;
		}

		// Token: 0x0600C4F2 RID: 50418 RVA: 0x0048BCA1 File Offset: 0x00489EA1
		public string GetNextChallengeName()
		{
			if (this.NextChallenge != null)
			{
				return this.NextChallenge.Name;
			}
			return "";
		}

		// Token: 0x0600C4F3 RID: 50419 RVA: 0x0048BCBC File Offset: 0x00489EBC
		public void AddObjective(BaseChallengeObjective objective)
		{
			this.ObjectiveList.Add(objective);
			objective.OwnerClass = this;
		}

		// Token: 0x0600C4F4 RID: 50420 RVA: 0x0048BCD4 File Offset: 0x00489ED4
		public bool ResetObjectives(Challenge challenge)
		{
			int count = challenge.ObjectiveList.Count;
			if (count > this.ObjectiveList.Count)
			{
				challenge.ObjectiveList.RemoveRange(this.ObjectiveList.Count, count - this.ObjectiveList.Count);
			}
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i];
				if (i < count && baseChallengeObjective.GetType() != challenge.ObjectiveList[i].GetType())
				{
					return false;
				}
				BaseChallengeObjective baseChallengeObjective2 = baseChallengeObjective.Clone();
				baseChallengeObjective2.Owner = challenge;
				baseChallengeObjective2.IsRequirement = baseChallengeObjective.IsRequirement;
				baseChallengeObjective2.MaxCount = baseChallengeObjective.MaxCount;
				baseChallengeObjective2.ShowRequirements = baseChallengeObjective.ShowRequirements;
				baseChallengeObjective2.Biome = baseChallengeObjective.Biome;
				baseChallengeObjective2.HandleOnCreated();
				if (i < count)
				{
					BaseChallengeObjective obj = challenge.ObjectiveList[i];
					baseChallengeObjective2.CopyValues(obj, baseChallengeObjective);
					challenge.ObjectiveList[i] = baseChallengeObjective2;
					baseChallengeObjective2.Current = Utils.FastMin(baseChallengeObjective2.Current, baseChallengeObjective2.MaxCount);
				}
				else
				{
					challenge.ObjectiveList.Add(baseChallengeObjective2);
				}
				if (!challenge.IsActive)
				{
					baseChallengeObjective2.Current = baseChallengeObjective2.MaxCount;
					baseChallengeObjective2.Complete = true;
				}
			}
			return true;
		}

		// Token: 0x0600C4F5 RID: 50421 RVA: 0x0048BE18 File Offset: 0x0048A018
		public string GetHint(bool isPreReq)
		{
			if (this.ChallengeHint == null)
			{
				return "";
			}
			if (isPreReq)
			{
				if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
				{
					string key = this.PreReqChallengeHint + "_alt";
					if (Localization.Exists(key, false))
					{
						return Localization.Get(key, false, null);
					}
				}
				return Localization.Get(this.PreReqChallengeHint, false, null);
			}
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
			{
				string key2 = this.ChallengeHint + "_alt";
				if (Localization.Exists(key2, false))
				{
					return Localization.Get(key2, false, null);
				}
			}
			return Localization.Get(this.ChallengeHint, false, null);
		}

		// Token: 0x0600C4F6 RID: 50422 RVA: 0x0048BEBC File Offset: 0x0048A0BC
		public string GetDescription()
		{
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
			{
				string key = this.Description + "_alt";
				if (Localization.Exists(key, false))
				{
					return Localization.Get(key, false, null);
				}
			}
			return Localization.Get(this.Description, false, null);
		}

		// Token: 0x0600C4F7 RID: 50423 RVA: 0x0048BF0B File Offset: 0x0048A10B
		public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
		{
			if (this.Effects != null)
			{
				this.Effects.FireEvent(_eventType, _params);
			}
		}

		// Token: 0x0600C4F8 RID: 50424 RVA: 0x0048BF24 File Offset: 0x0048A124
		public void ParseElement(XElement e)
		{
			if (e.HasAttribute("icon"))
			{
				this.Icon = e.GetAttribute("icon");
			}
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
			if (e.HasAttribute("group"))
			{
				ChallengeGroup challengeGroup = ChallengeGroup.s_ChallengeGroups[e.GetAttribute("group")];
				this.ChallengeGroup = challengeGroup;
				challengeGroup.AddChallenge(this);
			}
			if (e.HasAttribute("prerequisite_hint"))
			{
				this.PreReqChallengeHint = e.GetAttribute("prerequisite_hint");
			}
			if (e.HasAttribute("hint"))
			{
				this.ChallengeHint = e.GetAttribute("hint");
			}
			if (e.HasAttribute("short_description_key"))
			{
				this.ShortDescription = Localization.Get(e.GetAttribute("short_description_key"), false, null);
			}
			else if (e.HasAttribute("short_description"))
			{
				this.ShortDescription = e.GetAttribute("short_description");
			}
			if (e.HasAttribute("description_key"))
			{
				this.Description = e.GetAttribute("description_key");
			}
			else if (e.HasAttribute("description"))
			{
				this.Description = e.GetAttribute("description");
			}
			if (e.HasAttribute("reward_event"))
			{
				this.RewardEvent = e.GetAttribute("reward_event");
			}
			else
			{
				this.RewardEvent = ChallengesFromXml.DefaultRewardEvent;
			}
			if (e.HasAttribute("reward_text_key"))
			{
				this.RewardText = Localization.Get(e.GetAttribute("reward_text_key"), false, null);
			}
			else if (e.HasAttribute("reward_text"))
			{
				this.RewardText = e.GetAttribute("reward_text");
			}
			else
			{
				this.RewardText = ChallengesFromXml.DefaultRewardText;
			}
			if (e.HasAttribute("tags"))
			{
				this.TagName = e.GetAttribute("tags");
				this.Tags = FastTags<TagGroup.Global>.Parse(this.TagName);
			}
			if (e.HasAttribute("redeem_always"))
			{
				this.RedeemAlways = StringParsers.ParseBool(e.GetAttribute("redeem_always"), 0, -1, true);
			}
			if (e.HasAttribute("harvest_required"))
			{
				string[] array = e.GetAttribute("harvest_required").Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					ChallengeClass.ResourceRequiredTypes item = ChallengeClass.ResourceRequiredTypes.Resource;
					if (Enum.TryParse<ChallengeClass.ResourceRequiredTypes>(array[i], true, out item))
					{
						if (this.RequiredResourceTypeList == null)
						{
							this.RequiredResourceTypeList = new List<ChallengeClass.ResourceRequiredTypes>();
						}
						this.RequiredResourceTypeList.Add(item);
					}
				}
			}
		}

		// Token: 0x0600C4F9 RID: 50425 RVA: 0x0048C264 File Offset: 0x0048A464
		public ChallengeClass.UINavTypes GetNavType()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i];
				if (baseChallengeObjective.NavType != ChallengeClass.UINavTypes.None)
				{
					return baseChallengeObjective.NavType;
				}
			}
			return ChallengeClass.UINavTypes.None;
		}

		// Token: 0x0600C4FA RID: 50426 RVA: 0x0048C2A4 File Offset: 0x0048A4A4
		public bool HandleResourceRequirement()
		{
			if (ItemActionAttack.BlockDamagePercent == 0f)
			{
				return false;
			}
			if (this.RequiredResourceTypeList == null)
			{
				return true;
			}
			for (int i = 0; i < this.RequiredResourceTypeList.Count; i++)
			{
				switch (this.RequiredResourceTypeList[i])
				{
				case ChallengeClass.ResourceRequiredTypes.Resource:
					if (XUiM_Recipes.HarvestingOutputModifier == 0f)
					{
						return false;
					}
					break;
				case ChallengeClass.ResourceRequiredTypes.Ore:
					if (XUiM_Recipes.MiningOutputModifier == 0f)
					{
						return false;
					}
					break;
				case ChallengeClass.ResourceRequiredTypes.Seed:
					if (XUiM_Recipes.SeedDropOutputModifier == 0f)
					{
						return false;
					}
					break;
				case ChallengeClass.ResourceRequiredTypes.Crop:
					if (XUiM_Recipes.CropOutputModifier == 0f)
					{
						return false;
					}
					break;
				}
			}
			return true;
		}

		// Token: 0x04009519 RID: 38169
		public static Dictionary<string, ChallengeClass> s_Challenges = new CaseInsensitiveStringDictionary<ChallengeClass>();

		// Token: 0x0400951A RID: 38170
		public string Name;

		// Token: 0x0400951B RID: 38171
		public string Title;

		// Token: 0x0400951C RID: 38172
		public string Icon;

		// Token: 0x0400951D RID: 38173
		public ChallengeGroup ChallengeGroup;

		// Token: 0x0400951E RID: 38174
		public string ShortDescription;

		// Token: 0x0400951F RID: 38175
		public string Description;

		// Token: 0x04009520 RID: 38176
		public string PreReqChallengeHint;

		// Token: 0x04009521 RID: 38177
		public string ChallengeHint;

		// Token: 0x04009522 RID: 38178
		public string RewardEvent;

		// Token: 0x04009523 RID: 38179
		public string RewardText = "";

		// Token: 0x04009524 RID: 38180
		public string TagName = string.Empty;

		// Token: 0x04009525 RID: 38181
		public FastTags<TagGroup.Global> Tags = FastTags<TagGroup.Global>.none;

		// Token: 0x04009526 RID: 38182
		public List<BaseChallengeObjective> ObjectiveList = new List<BaseChallengeObjective>();

		// Token: 0x04009527 RID: 38183
		public ChallengeClass NextChallenge;

		// Token: 0x04009528 RID: 38184
		[PublicizedFrom(EAccessModifier.Private)]
		public static int nextIndex = 0;

		// Token: 0x0400952A RID: 38186
		public bool RedeemAlways;

		// Token: 0x0400952B RID: 38187
		public bool NeedsConstantUIUpdate;

		// Token: 0x0400952C RID: 38188
		public MinEffectController Effects;

		// Token: 0x0400952D RID: 38189
		public List<ChallengeClass.ResourceRequiredTypes> RequiredResourceTypeList;

		// Token: 0x020018F4 RID: 6388
		public enum UINavTypes
		{
			// Token: 0x0400952F RID: 38191
			None,
			// Token: 0x04009530 RID: 38192
			Crafting,
			// Token: 0x04009531 RID: 38193
			TwitchActions
		}

		// Token: 0x020018F5 RID: 6389
		public enum ResourceRequiredTypes
		{
			// Token: 0x04009533 RID: 38195
			Resource,
			// Token: 0x04009534 RID: 38196
			Ore,
			// Token: 0x04009535 RID: 38197
			Seed,
			// Token: 0x04009536 RID: 38198
			Crop
		}
	}
}
