using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200191A RID: 6426
	[Preserve]
	public class BaseRequirementObjectiveGroup
	{
		// Token: 0x17001898 RID: 6296
		// (get) Token: 0x0600C679 RID: 50809 RVA: 0x00491C2C File Offset: 0x0048FE2C
		public int Count
		{
			get
			{
				if (this.PhaseList == null)
				{
					return 0;
				}
				return this.PhaseList[this.currentIndex].RequirementObjectiveList.Count;
			}
		}

		// Token: 0x0600C67A RID: 50810 RVA: 0x000027FC File Offset: 0x000009FC
		public virtual void CreateRequirements()
		{
		}

		// Token: 0x17001899 RID: 6297
		// (get) Token: 0x0600C67B RID: 50811 RVA: 0x00491C53 File Offset: 0x0048FE53
		public List<BaseChallengeObjective> CurrentObjectiveList
		{
			get
			{
				if (this.PhaseList == null)
				{
					return null;
				}
				return this.PhaseList[this.currentIndex].RequirementObjectiveList;
			}
		}

		// Token: 0x0600C67C RID: 50812 RVA: 0x00491C78 File Offset: 0x0048FE78
		public void HandleAddHooks()
		{
			if (this.PhaseList.Count == 0)
			{
				this.CreateRequirements();
			}
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				this.PhaseList[i].AddHooks();
			}
			this.CheckPrerequisites();
		}

		// Token: 0x0600C67D RID: 50813 RVA: 0x00491CC8 File Offset: 0x0048FEC8
		public void CheckPrerequisites()
		{
			if (this.PhaseList.Count == 0)
			{
				this.CreateRequirements();
			}
			this.NeedsPreRequisites = false;
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				if (this.PhaseList[i].HandleCheckStatus())
				{
					this.currentIndex = i;
					this.NeedsPreRequisites = true;
					return;
				}
			}
		}

		// Token: 0x0600C67E RID: 50814 RVA: 0x00491D28 File Offset: 0x0048FF28
		public void HandleRemoveHooks()
		{
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				this.PhaseList[i].HandleRemoveHooks();
			}
		}

		// Token: 0x0600C67F RID: 50815 RVA: 0x00010E62 File Offset: 0x0000F062
		public virtual bool HasPrerequisiteCondition()
		{
			return false;
		}

		// Token: 0x0600C680 RID: 50816 RVA: 0x00491D5C File Offset: 0x0048FF5C
		public void ResetObjectives()
		{
			if (this.PhaseList != null)
			{
				for (int i = 0; i < this.PhaseList.Count; i++)
				{
					this.PhaseList[i].ResetComplete();
				}
			}
		}

		// Token: 0x0600C681 RID: 50817 RVA: 0x00491D98 File Offset: 0x0048FF98
		public void ClonePhases(BaseRequirementObjectiveGroup group)
		{
			if (group.PhaseList != null)
			{
				for (int i = 0; i < group.PhaseList.Count; i++)
				{
					RequirementGroupPhase item = group.PhaseList[i].Clone();
					this.PhaseList.Add(item);
				}
			}
		}

		// Token: 0x0600C682 RID: 50818 RVA: 0x00491DE4 File Offset: 0x0048FFE4
		public virtual bool HandleCheckStatus()
		{
			if (this.PhaseList.Count == 0)
			{
				this.CreateRequirements();
			}
			this.ResetObjectives();
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				if (this.CheckPhaseStatus(i) && this.PhaseList[i].HandleCheckStatus())
				{
					this.currentIndex = i;
					this.NeedsPreRequisites = true;
					return true;
				}
				this.PhaseList[i].IsComplete = true;
			}
			return false;
		}

		// Token: 0x0600C683 RID: 50819 RVA: 0x0002003D File Offset: 0x0001E23D
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool CheckPhaseStatus(int index)
		{
			return true;
		}

		// Token: 0x0600C684 RID: 50820 RVA: 0x00491E60 File Offset: 0x00490060
		public virtual void UpdateStatus()
		{
			for (int i = 0; i < this.PhaseList.Count; i++)
			{
				if (!this.PhaseList[i].IsComplete)
				{
					this.PhaseList[i].UpdateStatus();
				}
			}
		}

		// Token: 0x0600C685 RID: 50821 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public virtual BaseRequirementObjectiveGroup Clone()
		{
			return null;
		}

		// Token: 0x0600C686 RID: 50822 RVA: 0x00491EA7 File Offset: 0x004900A7
		public Recipe GetItemRecipe()
		{
			return this.PhaseList[this.currentIndex].GetItemRecipe();
		}

		// Token: 0x040095E9 RID: 38377
		public Challenge Owner;

		// Token: 0x040095EA RID: 38378
		public List<RequirementGroupPhase> PhaseList = new List<RequirementGroupPhase>();

		// Token: 0x040095EB RID: 38379
		[PublicizedFrom(EAccessModifier.Protected)]
		public int currentIndex;

		// Token: 0x040095EC RID: 38380
		public bool NeedsPreRequisites;
	}
}
