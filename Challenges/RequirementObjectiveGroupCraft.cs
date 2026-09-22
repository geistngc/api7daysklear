using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200191D RID: 6429
	[Preserve]
	public class RequirementObjectiveGroupCraft : BaseRequirementObjectiveGroup
	{
		// Token: 0x0600C69A RID: 50842 RVA: 0x00492759 File Offset: 0x00490959
		public RequirementObjectiveGroupCraft(string itemID)
		{
			this.ItemID = itemID;
			this.ItemRecipe = CraftingManager.GetRecipe(itemID);
		}

		// Token: 0x0600C69B RID: 50843 RVA: 0x00492780 File Offset: 0x00490980
		public override void CreateRequirements()
		{
			if (this.PhaseList == null)
			{
				this.PhaseList = new List<RequirementGroupPhase>();
			}
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			ChallengeObjectiveCraft challengeObjectiveCraft = new ChallengeObjectiveCraft();
			challengeObjectiveCraft.Owner = this.Owner;
			challengeObjectiveCraft.SetupItem(this.ItemID);
			challengeObjectiveCraft.IsRequirement = true;
			challengeObjectiveCraft.MaxCount = 1;
			challengeObjectiveCraft.Init();
			requirementGroupPhase.AddChallengeObjective(challengeObjectiveCraft);
			this.PhaseList.Add(requirementGroupPhase);
		}

		// Token: 0x0600C69C RID: 50844 RVA: 0x004927EC File Offset: 0x004909EC
		public override bool HasPrerequisiteCondition()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(player).xui.PlayerInventory;
			int craftingTier = this.ItemRecipe.GetOutputItemClass().HasQuality ? 1 : 0;
			for (int i = 0; i < this.ItemRecipe.ingredients.Count; i++)
			{
				ItemStack itemStack = this.ItemRecipe.ingredients[i].Clone();
				if (this.ItemRecipe.UseIngredientModifier)
				{
					itemStack.count = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)itemStack.count, this.Owner.Owner.Player, this.ItemRecipe, FastTags<TagGroup.Global>.Parse(itemStack.itemValue.ItemClass.GetItemName()), true, true, true, true, true, craftingTier, true, false);
					if (itemStack.count > 0)
					{
						itemStack.count = (int)((float)itemStack.count * XUiM_Recipes.GetCraftingInputModifier(this.ItemRecipe));
						if (XUiM_Recipes.CraftingInputModifier > 0f)
						{
							itemStack.count = Utils.FastMax(1, itemStack.count);
						}
					}
				}
				if (itemStack.count != 0 && !playerInventory.HasItem(itemStack))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600C69D RID: 50845 RVA: 0x0049292F File Offset: 0x00490B2F
		public override BaseRequirementObjectiveGroup Clone()
		{
			return new RequirementObjectiveGroupCraft(this.ItemID)
			{
				ItemRecipe = this.ItemRecipe
			};
		}

		// Token: 0x040095F3 RID: 38387
		public string ItemID = "";

		// Token: 0x040095F4 RID: 38388
		public Recipe ItemRecipe;
	}
}
