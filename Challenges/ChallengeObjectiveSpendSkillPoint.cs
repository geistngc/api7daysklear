using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001911 RID: 6417
	[Preserve]
	public class ChallengeObjectiveSpendSkillPoint : BaseChallengeObjective
	{
		// Token: 0x17001882 RID: 6274
		// (get) Token: 0x0600C626 RID: 50726 RVA: 0x00490C4B File Offset: 0x0048EE4B
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.SpendSkillPoint;
			}
		}

		// Token: 0x17001883 RID: 6275
		// (get) Token: 0x0600C627 RID: 50727 RVA: 0x00490C4F File Offset: 0x0048EE4F
		public override string DescriptionText
		{
			get
			{
				return string.Format(Localization.Get("ObjectiveSpendSkillPoints_keyword", false, null), Localization.Get("goAnyValue", false, null)) + ":";
			}
		}

		// Token: 0x0600C628 RID: 50728 RVA: 0x00490C78 File Offset: 0x0048EE78
		public override void HandleOnCreated()
		{
			base.HandleOnCreated();
			this.CreateRequirements();
		}

		// Token: 0x0600C629 RID: 50729 RVA: 0x00490C86 File Offset: 0x0048EE86
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreateRequirements()
		{
			if (!this.ShowRequirements)
			{
				return;
			}
			this.Owner.SetRequirementGroup(new RequirementObjectiveGroupWindowOpen("Skills"));
		}

		// Token: 0x0600C62A RID: 50730 RVA: 0x00490CA6 File Offset: 0x0048EEA6
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.SkillPointSpent += this.Current_SkillPointSpent;
		}

		// Token: 0x0600C62B RID: 50731 RVA: 0x00490CBE File Offset: 0x0048EEBE
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.SkillPointSpent -= this.Current_SkillPointSpent;
		}

		// Token: 0x0600C62C RID: 50732 RVA: 0x00490CD8 File Offset: 0x0048EED8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_SkillPointSpent(string skillName)
		{
			if (this.progressionName == "" || this.progressionName.EqualsCaseInsensitive(skillName))
			{
				int num = base.Current;
				base.Current = num + 1;
			}
			if (base.Current >= this.MaxCount)
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600C62D RID: 50733 RVA: 0x00490D37 File Offset: 0x0048EF37
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("skill_name"))
			{
				this.progressionName = e.GetAttribute("skill_name");
			}
		}

		// Token: 0x0600C62E RID: 50734 RVA: 0x00490D68 File Offset: 0x0048EF68
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveSpendSkillPoint
			{
				progressionName = this.progressionName
			};
		}

		// Token: 0x040095C8 RID: 38344
		[PublicizedFrom(EAccessModifier.Private)]
		public string progressionName = "";
	}
}
