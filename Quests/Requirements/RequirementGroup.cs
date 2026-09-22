using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020018AA RID: 6314
	[Preserve]
	public class RequirementGroup : BaseRequirement
	{
		// Token: 0x170017F0 RID: 6128
		// (get) Token: 0x0600C2E2 RID: 49890 RVA: 0x00483923 File Offset: 0x00481B23
		// (set) Token: 0x0600C2E3 RID: 49891 RVA: 0x0048392B File Offset: 0x00481B2B
		public RequirementGroup.GroupOperator Operator { get; set; }

		// Token: 0x0600C2E4 RID: 49892 RVA: 0x00483934 File Offset: 0x00481B34
		public override void SetupRequirement()
		{
			this.Operator = EnumUtils.Parse<RequirementGroup.GroupOperator>(base.Value, false);
			for (int i = 0; i < this.ChildRequirements.Count; i++)
			{
				this.ChildRequirements[i].OwnerQuest = base.OwnerQuest;
				this.ChildRequirements[i].SetupRequirement();
			}
			if (string.IsNullOrEmpty(base.ID))
			{
				if (this.ChildRequirements.Count > 0)
				{
					base.Description = this.ChildRequirements[0].Description;
					return;
				}
			}
			else
			{
				base.Description = Localization.Get(base.ID, false, null);
			}
		}

		// Token: 0x0600C2E5 RID: 49893 RVA: 0x004839D8 File Offset: 0x00481BD8
		public override bool CheckRequirement()
		{
			if (!base.OwnerQuest.Active)
			{
				return true;
			}
			bool result = this.Operator == RequirementGroup.GroupOperator.AND;
			for (int i = 0; i < this.ChildRequirements.Count; i++)
			{
				bool flag = this.ChildRequirements[i].CheckRequirement();
				if (this.Operator == RequirementGroup.GroupOperator.AND)
				{
					if (!flag)
					{
						return false;
					}
				}
				else if (this.Operator == RequirementGroup.GroupOperator.OR && flag)
				{
					return true;
				}
			}
			return result;
		}

		// Token: 0x0600C2E6 RID: 49894 RVA: 0x00483A44 File Offset: 0x00481C44
		public override BaseRequirement Clone()
		{
			RequirementGroup requirementGroup = new RequirementGroup();
			requirementGroup.ID = base.ID;
			requirementGroup.Value = base.Value;
			requirementGroup.Phase = base.Phase;
			for (int i = 0; i < this.ChildRequirements.Count; i++)
			{
				requirementGroup.ChildRequirements.Add(this.ChildRequirements[i].Clone());
			}
			return requirementGroup;
		}

		// Token: 0x04009421 RID: 37921
		public List<BaseRequirement> ChildRequirements = new List<BaseRequirement>();

		// Token: 0x020018AB RID: 6315
		public enum GroupOperator
		{
			// Token: 0x04009424 RID: 37924
			AND,
			// Token: 0x04009425 RID: 37925
			OR
		}
	}
}
