using System;
using System.Collections.Generic;

namespace Challenges
{
	// Token: 0x0200191B RID: 6427
	public class RequirementGroupPhase
	{
		// Token: 0x0600C688 RID: 50824 RVA: 0x00491ED2 File Offset: 0x004900D2
		public void AddChallengeObjective(BaseChallengeObjective obj)
		{
			this.RequirementObjectiveList.Add(obj);
		}

		// Token: 0x0600C689 RID: 50825 RVA: 0x00491EE0 File Offset: 0x004900E0
		public void AddHooks()
		{
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				for (int j = 0; j < this.RequirementObjectiveList.Count; j++)
				{
					this.RequirementObjectiveList[j].HandleAddHooks();
				}
			}
		}

		// Token: 0x0600C68A RID: 50826 RVA: 0x00491F2C File Offset: 0x0049012C
		public bool HandleCheckStatus()
		{
			bool result = false;
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				if (!this.RequirementObjectiveList[i].HandleCheckStatus())
				{
					result = true;
				}
				this.RequirementObjectiveList[i].UpdateStatus();
			}
			return result;
		}

		// Token: 0x0600C68B RID: 50827 RVA: 0x00491F78 File Offset: 0x00490178
		public void HandleRemoveHooks()
		{
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				this.RequirementObjectiveList[i].HandleRemoveHooks();
			}
		}

		// Token: 0x0600C68C RID: 50828 RVA: 0x00491FAC File Offset: 0x004901AC
		public void ResetComplete()
		{
			this.IsComplete = false;
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				this.RequirementObjectiveList[i].ResetComplete();
			}
		}

		// Token: 0x0600C68D RID: 50829 RVA: 0x00491FE8 File Offset: 0x004901E8
		public virtual void UpdateStatus()
		{
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				this.RequirementObjectiveList[i].UpdateStatus();
			}
		}

		// Token: 0x0600C68E RID: 50830 RVA: 0x0049201C File Offset: 0x0049021C
		public void Clone(RequirementGroupPhase phase)
		{
			for (int i = 0; i < phase.RequirementObjectiveList.Count; i++)
			{
				BaseChallengeObjective item = phase.RequirementObjectiveList[i].Clone();
				if (this.RequirementObjectiveList == null)
				{
					this.RequirementObjectiveList = new List<BaseChallengeObjective>();
				}
				this.RequirementObjectiveList.Add(item);
			}
		}

		// Token: 0x0600C68F RID: 50831 RVA: 0x00492070 File Offset: 0x00490270
		public RequirementGroupPhase Clone()
		{
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				BaseChallengeObjective item = this.RequirementObjectiveList[i].Clone();
				if (this.RequirementObjectiveList == null)
				{
					this.RequirementObjectiveList = new List<BaseChallengeObjective>();
				}
				requirementGroupPhase.RequirementObjectiveList.Add(item);
			}
			return requirementGroupPhase;
		}

		// Token: 0x0600C690 RID: 50832 RVA: 0x004920CC File Offset: 0x004902CC
		public Recipe GetItemRecipe()
		{
			for (int i = 0; i < this.RequirementObjectiveList.Count; i++)
			{
				Recipe recipeItem = this.RequirementObjectiveList[i].GetRecipeItem();
				if (recipeItem != null)
				{
					return recipeItem;
				}
			}
			return null;
		}

		// Token: 0x040095ED RID: 38381
		public List<BaseChallengeObjective> RequirementObjectiveList = new List<BaseChallengeObjective>();

		// Token: 0x040095EE RID: 38382
		public bool IsComplete;
	}
}
