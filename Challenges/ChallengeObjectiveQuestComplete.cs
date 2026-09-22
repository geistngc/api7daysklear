using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200190F RID: 6415
	[Preserve]
	public class ChallengeObjectiveQuestComplete : BaseChallengeObjective
	{
		// Token: 0x1700187E RID: 6270
		// (get) Token: 0x0600C614 RID: 50708 RVA: 0x00180BA4 File Offset: 0x0017EDA4
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.QuestComplete;
			}
		}

		// Token: 0x1700187F RID: 6271
		// (get) Token: 0x0600C615 RID: 50709 RVA: 0x004908F0 File Offset: 0x0048EAF0
		public override string DescriptionText
		{
			get
			{
				if (this.questText == "")
				{
					this.questText = Localization.Get("challengeTargetAnyQuest", false, null);
				}
				return this.questText + " " + Localization.Get("challengeObjectiveQuestCompleted", false, null) + ":";
			}
		}

		// Token: 0x0600C616 RID: 50710 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init()
		{
		}

		// Token: 0x0600C617 RID: 50711 RVA: 0x00490942 File Offset: 0x0048EB42
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.QuestComplete += this.Current_QuestComplete;
		}

		// Token: 0x0600C618 RID: 50712 RVA: 0x0049095A File Offset: 0x0048EB5A
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.QuestComplete -= this.Current_QuestComplete;
		}

		// Token: 0x0600C619 RID: 50713 RVA: 0x00490974 File Offset: 0x0048EB74
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_QuestComplete(FastTags<TagGroup.Global> questTags, QuestClass questClass)
		{
			if (this.questTag.IsEmpty || questTags.Test_AnySet(this.questTag))
			{
				if (this.CheckBaseRequirements())
				{
					return;
				}
				int num = base.Current;
				base.Current = num + 1;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600C61A RID: 50714 RVA: 0x004909D8 File Offset: 0x0048EBD8
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("quest_tag"))
			{
				this.questTagText = e.GetAttribute("quest_tag");
				this.questTag = FastTags<TagGroup.Global>.Parse(this.questTagText);
			}
			else
			{
				this.questTag = FastTags<TagGroup.Global>.none;
			}
			if (e.HasAttribute("quest_text_key"))
			{
				this.questText = Localization.Get(e.GetAttribute("quest_text_key"), false, null);
			}
			if (e.HasAttribute("tier"))
			{
				this.tier = StringParsers.ParseSInt32(e.GetAttribute("tier"), 0, -1, NumberStyles.Integer);
			}
		}

		// Token: 0x0600C61B RID: 50715 RVA: 0x00490A91 File Offset: 0x0048EC91
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveQuestComplete
			{
				questTag = this.questTag,
				questText = this.questText,
				tier = this.tier
			};
		}

		// Token: 0x040095C1 RID: 38337
		[PublicizedFrom(EAccessModifier.Private)]
		public string questTagText;

		// Token: 0x040095C2 RID: 38338
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> questTag = FastTags<TagGroup.Global>.none;

		// Token: 0x040095C3 RID: 38339
		[PublicizedFrom(EAccessModifier.Private)]
		public int tier;

		// Token: 0x040095C4 RID: 38340
		[PublicizedFrom(EAccessModifier.Private)]
		public string questText = "";
	}
}
