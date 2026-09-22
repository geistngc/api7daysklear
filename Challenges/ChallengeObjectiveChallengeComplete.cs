using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001900 RID: 6400
	[Preserve]
	public class ChallengeObjectiveChallengeComplete : BaseChallengeObjective
	{
		// Token: 0x1700185E RID: 6238
		// (get) Token: 0x0600C565 RID: 50533 RVA: 0x00266E4D File Offset: 0x0026504D
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.ChallengeComplete;
			}
		}

		// Token: 0x1700185F RID: 6239
		// (get) Token: 0x0600C566 RID: 50534 RVA: 0x0048D928 File Offset: 0x0048BB28
		public override string DescriptionText
		{
			get
			{
				string str = Localization.Get("challengeTargetAnyChallenge", false, null);
				if (this.ChallengeName != "")
				{
					if (this.IsGroup)
					{
						ChallengeGroup challengeGroup = ChallengeGroup.s_ChallengeGroups[this.ChallengeName];
						if (challengeGroup != null)
						{
							str = challengeGroup.Title;
						}
					}
					else
					{
						ChallengeClass challenge = ChallengeClass.GetChallenge(this.ChallengeName);
						if (challenge != null)
						{
							str = challenge.Title;
						}
					}
				}
				if (this.IsRedeemed)
				{
					return Localization.Get("challengeObjectiveRedeem", false, null) + " [DECEA3]" + str + "[-]:";
				}
				return Localization.Get("challengeObjectiveComplete", false, null) + " [DECEA3]" + str + "[-]:";
			}
		}

		// Token: 0x0600C567 RID: 50535 RVA: 0x0048D9D0 File Offset: 0x0048BBD0
		public override void BaseInit()
		{
			base.BaseInit();
			this.UpdateMax();
		}

		// Token: 0x0600C568 RID: 50536 RVA: 0x0048D9DE File Offset: 0x0048BBDE
		public override void HandleOnCreated()
		{
			base.HandleOnCreated();
			this.CreateRequirements();
		}

		// Token: 0x0600C569 RID: 50537 RVA: 0x0048D9EC File Offset: 0x0048BBEC
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateMax()
		{
			if (this.IsGroup)
			{
				ChallengeGroup challengeGroup = ChallengeGroup.s_ChallengeGroups[this.ChallengeName];
				this.MaxCount = challengeGroup.ChallengeClasses.Count;
				if (this.OwnerClass.ChallengeGroup == challengeGroup)
				{
					this.MaxCount--;
				}
			}
		}

		// Token: 0x0600C56A RID: 50538 RVA: 0x0048DA3F File Offset: 0x0048BC3F
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreateRequirements()
		{
			if (!this.ShowRequirements)
			{
				return;
			}
			this.Owner.SetRequirementGroup(new RequirementObjectiveGroupWindowOpen("Challenges"));
		}

		// Token: 0x0600C56B RID: 50539 RVA: 0x0048DA5F File Offset: 0x0048BC5F
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.ChallengeComplete += this.Current_ChallengeComplete;
		}

		// Token: 0x0600C56C RID: 50540 RVA: 0x0048DA77 File Offset: 0x0048BC77
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.ChallengeComplete -= this.Current_ChallengeComplete;
		}

		// Token: 0x0600C56D RID: 50541 RVA: 0x0048DA90 File Offset: 0x0048BC90
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_ChallengeComplete(ChallengeClass _challenge, bool _isRedeemed)
		{
			if (this.IsGroup)
			{
				base.Current = 0;
				List<ChallengeClass> challengeClasses = ChallengeGroup.s_ChallengeGroups[this.ChallengeName].ChallengeClasses;
				int i = 0;
				while (i < challengeClasses.Count)
				{
					Challenge challenge = this.Owner.Owner.ChallengeDictionary[challengeClasses[i].Name];
					bool flag = false;
					if (challenge.ChallengeState != Challenge.ChallengeStates.Active)
					{
						goto IL_65;
					}
					if (challenge == this.Owner)
					{
						flag = true;
						goto IL_65;
					}
					IL_9C:
					i++;
					continue;
					IL_65:
					if (!flag && (!this.IsRedeemed || challenge.ChallengeState == Challenge.ChallengeStates.Redeemed) && (this.IsRedeemed || challenge.ChallengeState == Challenge.ChallengeStates.Completed))
					{
						int num = base.Current;
						base.Current = num + 1;
						goto IL_9C;
					}
					goto IL_9C;
				}
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
					return;
				}
			}
			else if ((string.IsNullOrEmpty(this.ChallengeName) || string.Compare(_challenge.Name, this.ChallengeName, true) == 0) && _isRedeemed == this.IsRedeemed)
			{
				int num = base.Current;
				base.Current = num + 1;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600C56E RID: 50542 RVA: 0x0048DBC8 File Offset: 0x0048BDC8
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("challenge"))
			{
				this.ChallengeName = e.GetAttribute("challenge");
			}
			if (e.HasAttribute("is_group"))
			{
				this.IsGroup = StringParsers.ParseBool(e.GetAttribute("is_group"), 0, -1, true);
				if (this.IsGroup)
				{
					this.MaxCount = -1;
				}
			}
			if (e.HasAttribute("is_redeemed"))
			{
				this.IsRedeemed = StringParsers.ParseBool(e.GetAttribute("is_redeemed"), 0, -1, true);
			}
		}

		// Token: 0x0600C56F RID: 50543 RVA: 0x0048DC73 File Offset: 0x0048BE73
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveChallengeComplete
			{
				ChallengeName = this.ChallengeName,
				IsRedeemed = this.IsRedeemed,
				IsGroup = this.IsGroup
			};
		}

		// Token: 0x04009585 RID: 38277
		public string ChallengeName = "";

		// Token: 0x04009586 RID: 38278
		public bool IsGroup;

		// Token: 0x04009587 RID: 38279
		public bool IsRedeemed;
	}
}
