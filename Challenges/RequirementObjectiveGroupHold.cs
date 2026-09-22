using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200191F RID: 6431
	[Preserve]
	public class RequirementObjectiveGroupHold : BaseRequirementObjectiveGroup
	{
		// Token: 0x0600C6A2 RID: 50850 RVA: 0x00492B05 File Offset: 0x00490D05
		public RequirementObjectiveGroupHold(string itemID)
		{
			this.ItemID = itemID;
		}

		// Token: 0x0600C6A3 RID: 50851 RVA: 0x00492B20 File Offset: 0x00490D20
		public override void CreateRequirements()
		{
			if (this.PhaseList == null)
			{
				this.PhaseList = new List<RequirementGroupPhase>();
			}
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			ChallengeObjectiveHold challengeObjectiveHold = new ChallengeObjectiveHold();
			challengeObjectiveHold.Owner = this.Owner;
			challengeObjectiveHold.itemClassID = this.ItemID;
			challengeObjectiveHold.IsRequirement = true;
			challengeObjectiveHold.MaxCount = 1;
			challengeObjectiveHold.Init();
			requirementGroupPhase.AddChallengeObjective(challengeObjectiveHold);
			this.PhaseList.Add(requirementGroupPhase);
		}

		// Token: 0x0600C6A4 RID: 50852 RVA: 0x00492B8C File Offset: 0x00490D8C
		public override bool HasPrerequisiteCondition()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(player).xui.PlayerInventory;
			ItemClass holdingItem = this.Owner.Owner.Player.inventory.holdingItem;
			return playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && holdingItem.Name != this.ItemID;
		}

		// Token: 0x0600C6A5 RID: 50853 RVA: 0x00492C0F File Offset: 0x00490E0F
		public override BaseRequirementObjectiveGroup Clone()
		{
			return new RequirementObjectiveGroupHold(this.ItemID);
		}

		// Token: 0x040095F8 RID: 38392
		public string ItemID = "";
	}
}
