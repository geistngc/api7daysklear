using System;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020018FF RID: 6399
	[Preserve]
	public class ChallengeObjectiveBloodmoon : BaseChallengeObjective
	{
		// Token: 0x1700185B RID: 6235
		// (get) Token: 0x0600C55C RID: 50524 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Bloodmoon;
			}
		}

		// Token: 0x1700185C RID: 6236
		// (get) Token: 0x0600C55D RID: 50525 RVA: 0x0048D82F File Offset: 0x0048BA2F
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveBloodMoonCompleted", false, null) + ":";
			}
		}

		// Token: 0x1700185D RID: 6237
		// (get) Token: 0x0600C55E RID: 50526 RVA: 0x0048D847 File Offset: 0x0048BA47
		public override string StatusText
		{
			get
			{
				if (!EntityFactory.EnemySpawnMode)
				{
					return "--";
				}
				return base.StatusText;
			}
		}

		// Token: 0x0600C55F RID: 50527 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init()
		{
		}

		// Token: 0x0600C560 RID: 50528 RVA: 0x0048D85C File Offset: 0x0048BA5C
		public override void HandleAddHooks()
		{
			if (!EntityFactory.EnemySpawnMode)
			{
				base.Current = this.MaxCount;
				base.Complete = true;
				this.Owner.HandleComplete(false, true);
				if (this.Owner.ChallengeState == Challenge.ChallengeStates.Completed)
				{
					this.Owner.AutoCompleted = true;
					this.Owner.ChallengeState = Challenge.ChallengeStates.Redeemed;
					return;
				}
			}
			else
			{
				QuestEventManager.Current.BloodMoonSurvive += this.Current_BloodMoonSurvive;
			}
		}

		// Token: 0x0600C561 RID: 50529 RVA: 0x0048D8CD File Offset: 0x0048BACD
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.BloodMoonSurvive -= this.Current_BloodMoonSurvive;
		}

		// Token: 0x0600C562 RID: 50530 RVA: 0x0048D8E8 File Offset: 0x0048BAE8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BloodMoonSurvive()
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			int num = base.Current;
			base.Current = num + 1;
			this.CheckObjectiveComplete(true);
		}

		// Token: 0x0600C563 RID: 50531 RVA: 0x0048D916 File Offset: 0x0048BB16
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveBloodmoon();
		}
	}
}
