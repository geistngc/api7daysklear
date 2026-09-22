using System;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020018A9 RID: 6313
	[Preserve]
	public class RequirementBuff : BaseRequirement
	{
		// Token: 0x0600C2DE RID: 49886 RVA: 0x00483874 File Offset: 0x00481A74
		public override void SetupRequirement()
		{
			string arg = Localization.Get("RequirementBuff_keyword", false, null);
			base.Description = string.Format("{0} {1}", arg, BuffManager.GetBuff(base.ID).Name);
		}

		// Token: 0x0600C2DF RID: 49887 RVA: 0x004838AF File Offset: 0x00481AAF
		public override bool CheckRequirement()
		{
			return !base.OwnerQuest.Active || base.OwnerQuest.OwnerJournal.OwnerPlayer.Buffs.HasBuff(base.ID);
		}

		// Token: 0x0600C2E0 RID: 49888 RVA: 0x004838E5 File Offset: 0x00481AE5
		public override BaseRequirement Clone()
		{
			return new RequirementBuff
			{
				ID = base.ID,
				Value = base.Value,
				Phase = base.Phase
			};
		}

		// Token: 0x04009420 RID: 37920
		[PublicizedFrom(EAccessModifier.Private)]
		public string name = "";
	}
}
