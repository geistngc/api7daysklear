using System;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020018AD RID: 6317
	[Preserve]
	public class RequirementLevel : BaseRequirement
	{
		// Token: 0x0600C2EC RID: 49900 RVA: 0x00483C54 File Offset: 0x00481E54
		public override void SetupRequirement()
		{
			string arg = Localization.Get("RequirementLevel_keyword", false, null);
			this.expectedLevel = Convert.ToInt32(base.Value);
			base.Description = string.Format("{0} {1}", arg, this.expectedLevel);
		}

		// Token: 0x0600C2ED RID: 49901 RVA: 0x00483C9B File Offset: 0x00481E9B
		public override bool CheckRequirement()
		{
			return !base.OwnerQuest.Active || XUiM_Player.GetLevel(base.OwnerQuest.OwnerJournal.OwnerPlayer) >= this.expectedLevel;
		}

		// Token: 0x0600C2EE RID: 49902 RVA: 0x00483CCC File Offset: 0x00481ECC
		public override BaseRequirement Clone()
		{
			return new RequirementLevel
			{
				ID = base.ID,
				Value = base.Value,
				Phase = base.Phase
			};
		}

		// Token: 0x04009428 RID: 37928
		[PublicizedFrom(EAccessModifier.Private)]
		public int expectedLevel;
	}
}
