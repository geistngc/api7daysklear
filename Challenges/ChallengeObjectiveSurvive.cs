using System;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001912 RID: 6418
	[Preserve]
	public class ChallengeObjectiveSurvive : BaseChallengeObjective
	{
		// Token: 0x17001884 RID: 6276
		// (get) Token: 0x0600C630 RID: 50736 RVA: 0x00490D8E File Offset: 0x0048EF8E
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Survive;
			}
		}

		// Token: 0x17001885 RID: 6277
		// (get) Token: 0x0600C631 RID: 50737 RVA: 0x00490D92 File Offset: 0x0048EF92
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveSurvive", false, null) + ":";
			}
		}

		// Token: 0x17001886 RID: 6278
		// (get) Token: 0x0600C632 RID: 50738 RVA: 0x00490DAA File Offset: 0x0048EFAA
		public override string StatusText
		{
			get
			{
				return string.Format("{0}/{1}", XUiM_PlayerBuffs.GetTimeString((float)this.current * 60f), XUiM_PlayerBuffs.GetTimeString((float)this.MaxCount * 60f));
			}
		}

		// Token: 0x0600C633 RID: 50739 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init()
		{
		}

		// Token: 0x0600C634 RID: 50740 RVA: 0x00490DDC File Offset: 0x0048EFDC
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.TimeSurvive += this.Current_TimeSurvive;
			base.Current = (int)base.Player.longestLife;
			if (base.Current >= this.MaxCount)
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600C635 RID: 50741 RVA: 0x00490E33 File Offset: 0x0048F033
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.TimeSurvive -= this.Current_TimeSurvive;
		}

		// Token: 0x0600C636 RID: 50742 RVA: 0x00490E4B File Offset: 0x0048F04B
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_TimeSurvive(float val)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			base.Current = (int)val;
			if (base.Current >= this.MaxCount)
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600C637 RID: 50743 RVA: 0x00490E80 File Offset: 0x0048F080
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveSurvive();
		}
	}
}
