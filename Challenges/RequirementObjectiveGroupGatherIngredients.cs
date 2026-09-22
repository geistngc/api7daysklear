using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200191E RID: 6430
	[Preserve]
	public class RequirementObjectiveGroupGatherIngredients : BaseRequirementObjectiveGroup
	{
		// Token: 0x0600C69E RID: 50846 RVA: 0x00492948 File Offset: 0x00490B48
		public RequirementObjectiveGroupGatherIngredients(string itemID)
		{
			this.ItemID = itemID;
			this.itemRecipe = CraftingManager.GetRecipe(itemID);
		}

		// Token: 0x0600C69F RID: 50847 RVA: 0x00492970 File Offset: 0x00490B70
		public override void CreateRequirements()
		{
			if (this.PhaseList == null)
			{
				this.PhaseList = new List<RequirementGroupPhase>();
			}
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			int craftingTier = this.itemRecipe.GetOutputItemClass().HasQuality ? 1 : 0;
			for (int i = 0; i < this.itemRecipe.ingredients.Count; i++)
			{
				int num = this.itemRecipe.ingredients[i].count;
				if (this.itemRecipe.UseIngredientModifier)
				{
					num = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)num, this.Owner.Owner.Player, this.itemRecipe, FastTags<TagGroup.Global>.Parse(this.itemRecipe.ingredients[i].itemValue.ItemClass.GetItemName()), true, true, true, true, true, craftingTier, true, false);
					if (num > 0)
					{
						num = (int)((float)num * XUiM_Recipes.GetCraftingInputModifier(this.itemRecipe));
						if (XUiM_Recipes.CraftingInputModifier > 0f)
						{
							num = Utils.FastMax(1, num);
						}
					}
				}
				if (num != 0)
				{
					ChallengeObjectiveGatherIngredient challengeObjectiveGatherIngredient = new ChallengeObjectiveGatherIngredient();
					challengeObjectiveGatherIngredient.Parent = this;
					challengeObjectiveGatherIngredient.Owner = this.Owner;
					challengeObjectiveGatherIngredient.IsRequirement = true;
					challengeObjectiveGatherIngredient.itemRecipe = this.itemRecipe;
					challengeObjectiveGatherIngredient.IngredientIndex = i;
					challengeObjectiveGatherIngredient.IngredientCount = num;
					challengeObjectiveGatherIngredient.NeededCount = ((this.CraftObj == null) ? 1 : this.CraftObj.MaxCount);
					challengeObjectiveGatherIngredient.MaxCount = num * challengeObjectiveGatherIngredient.NeededCount;
					challengeObjectiveGatherIngredient.Init();
					requirementGroupPhase.AddChallengeObjective(challengeObjectiveGatherIngredient);
				}
			}
			this.PhaseList.Add(requirementGroupPhase);
		}

		// Token: 0x0600C6A0 RID: 50848 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool HasPrerequisiteCondition()
		{
			return true;
		}

		// Token: 0x0600C6A1 RID: 50849 RVA: 0x00492AF8 File Offset: 0x00490CF8
		public override BaseRequirementObjectiveGroup Clone()
		{
			return new RequirementObjectiveGroupGatherIngredients(this.ItemID);
		}

		// Token: 0x040095F5 RID: 38389
		public string ItemID = "";

		// Token: 0x040095F6 RID: 38390
		[PublicizedFrom(EAccessModifier.Private)]
		public Recipe itemRecipe;

		// Token: 0x040095F7 RID: 38391
		public ChallengeObjectiveCraft CraftObj;
	}
}
