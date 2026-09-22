using System;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020018AE RID: 6318
	[Preserve]
	public class RequirementWearing : BaseRequirement
	{
		// Token: 0x0600C2F0 RID: 49904 RVA: 0x00483D00 File Offset: 0x00481F00
		public override void SetupRequirement()
		{
			string arg = Localization.Get("RequirementWearing_keyword", false, null);
			this.expectedItem = ItemClass.GetItem(base.ID, false);
			this.expectedItemClass = ItemClass.GetItemClass(base.ID, false);
			base.Description = string.Format("{0} {1}", arg, this.expectedItemClass.GetLocalizedItemName());
		}

		// Token: 0x0600C2F1 RID: 49905 RVA: 0x00483D5A File Offset: 0x00481F5A
		public override bool CheckRequirement()
		{
			return !base.OwnerQuest.Active || LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui.PlayerEquipment.IsWearing(this.expectedItem);
		}

		// Token: 0x0600C2F2 RID: 49906 RVA: 0x00483D95 File Offset: 0x00481F95
		public override BaseRequirement Clone()
		{
			return new RequirementWearing
			{
				ID = base.ID,
				Value = base.Value,
				Phase = base.Phase
			};
		}

		// Token: 0x04009429 RID: 37929
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None;

		// Token: 0x0400942A RID: 37930
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;
	}
}
