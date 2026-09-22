using System;
using UnityEngine.Scripting;

namespace Quests.Requirements
{
	// Token: 0x020018AC RID: 6316
	[Preserve]
	public class RequirementHolding : BaseRequirement
	{
		// Token: 0x0600C2E8 RID: 49896 RVA: 0x00483AC4 File Offset: 0x00481CC4
		public override void SetupRequirement()
		{
			XUi xui = LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui;
			string arg = Localization.Get("RequirementHolding_keyword", false, null);
			this.expectedItem = ((base.ID != "" && base.ID != null) ? ItemClass.GetItem(base.ID, false) : xui.PlayerInventory.Toolbelt.GetBareHandItemValue());
			this.expectedItemClass = ((base.ID != "" && base.ID != null) ? ItemClass.GetItemClass(base.ID, false) : xui.PlayerInventory.Toolbelt.GetBareHandItem());
			if (base.ID == "" || base.ID == null)
			{
				base.Description = "Bare Hands";
				return;
			}
			base.Description = string.Format("{0} {1}", arg, this.expectedItemClass.GetLocalizedItemName());
		}

		// Token: 0x0600C2E9 RID: 49897 RVA: 0x00483BB8 File Offset: 0x00481DB8
		public override bool CheckRequirement()
		{
			return !base.OwnerQuest.Active || LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui.PlayerInventory.Toolbelt.holdingItemStack.itemValue.type == this.expectedItem.type;
		}

		// Token: 0x0600C2EA RID: 49898 RVA: 0x00483C14 File Offset: 0x00481E14
		public override BaseRequirement Clone()
		{
			return new RequirementHolding
			{
				ID = base.ID,
				Value = base.Value,
				Phase = base.Phase
			};
		}

		// Token: 0x04009426 RID: 37926
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None;

		// Token: 0x04009427 RID: 37927
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;
	}
}
