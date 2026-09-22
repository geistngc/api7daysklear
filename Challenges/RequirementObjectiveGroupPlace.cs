using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001920 RID: 6432
	[Preserve]
	public class RequirementObjectiveGroupPlace : BaseRequirementObjectiveGroup
	{
		// Token: 0x0600C6A6 RID: 50854 RVA: 0x00492C1C File Offset: 0x00490E1C
		public RequirementObjectiveGroupPlace(string itemID)
		{
			this.ItemID = itemID;
		}

		// Token: 0x0600C6A7 RID: 50855 RVA: 0x00492C38 File Offset: 0x00490E38
		public override void CreateRequirements()
		{
			if (this.PhaseList == null)
			{
				this.PhaseList = new List<RequirementGroupPhase>();
			}
			this.PhaseList.Add(this.AddIngredientGatheringReqs());
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			ChallengeObjectiveCraft challengeObjectiveCraft = new ChallengeObjectiveCraft();
			challengeObjectiveCraft.Owner = this.Owner;
			challengeObjectiveCraft.SetupItem(this.ItemID);
			challengeObjectiveCraft.IsRequirement = true;
			challengeObjectiveCraft.MaxCount = 1;
			challengeObjectiveCraft.Init();
			requirementGroupPhase.AddChallengeObjective(challengeObjectiveCraft);
			this.PhaseList.Add(requirementGroupPhase);
			requirementGroupPhase = new RequirementGroupPhase();
			ChallengeObjectiveHold challengeObjectiveHold = new ChallengeObjectiveHold();
			challengeObjectiveHold.Owner = this.Owner;
			challengeObjectiveHold.itemClassID = this.ItemID;
			challengeObjectiveHold.IsRequirement = true;
			challengeObjectiveHold.MaxCount = 1;
			challengeObjectiveHold.Init();
			requirementGroupPhase.AddChallengeObjective(challengeObjectiveHold);
			this.PhaseList.Add(requirementGroupPhase);
		}

		// Token: 0x0600C6A8 RID: 50856 RVA: 0x00492D00 File Offset: 0x00490F00
		[PublicizedFrom(EAccessModifier.Private)]
		public RequirementGroupPhase AddIngredientGatheringReqs()
		{
			Recipe recipe = CraftingManager.GetRecipe(this.ItemID);
			RequirementGroupPhase requirementGroupPhase = new RequirementGroupPhase();
			int craftingTier = recipe.GetOutputItemClass().HasQuality ? 1 : 0;
			for (int i = 0; i < recipe.ingredients.Count; i++)
			{
				int num = recipe.ingredients[i].count;
				if (recipe.UseIngredientModifier)
				{
					num = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)num, this.Owner.Owner.Player, recipe, FastTags<TagGroup.Global>.Parse(recipe.ingredients[i].itemValue.ItemClass.GetItemName()), true, true, true, true, true, craftingTier, true, false);
					if (num > 0)
					{
						num = (int)((float)num * XUiM_Recipes.GetCraftingInputModifier(recipe));
						if (XUiM_Recipes.CraftingInputModifier > 0f)
						{
							num = Utils.FastMax(1, num);
						}
					}
				}
				if (num != 0)
				{
					ChallengeObjectiveGatherIngredient challengeObjectiveGatherIngredient = new ChallengeObjectiveGatherIngredient();
					challengeObjectiveGatherIngredient.Owner = this.Owner;
					challengeObjectiveGatherIngredient.Parent = this;
					challengeObjectiveGatherIngredient.IsRequirement = true;
					challengeObjectiveGatherIngredient.itemRecipe = recipe;
					challengeObjectiveGatherIngredient.IngredientIndex = i;
					challengeObjectiveGatherIngredient.IngredientCount = num;
					challengeObjectiveGatherIngredient.NeededCount = 1;
					challengeObjectiveGatherIngredient.MaxCount = num;
					challengeObjectiveGatherIngredient.Init();
					requirementGroupPhase.AddChallengeObjective(challengeObjectiveGatherIngredient);
				}
			}
			return requirementGroupPhase;
		}

		// Token: 0x0600C6A9 RID: 50857 RVA: 0x00492E3C File Offset: 0x0049103C
		public override bool HasPrerequisiteCondition()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = uiforPlayer.xui.PlayerInventory;
			ItemClass holdingItem = this.Owner.Owner.Player.inventory.holdingItem;
			return (!playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && !this.CheckDragDropItem(uiforPlayer.xui.DragAndDropWindow.CurrentStack, this.ItemID)) || holdingItem.Name != this.ItemID;
		}

		// Token: 0x0600C6AA RID: 50858 RVA: 0x00492EDA File Offset: 0x004910DA
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckDragDropItem(ItemStack stack, string itemID)
		{
			return !stack.IsEmpty() && stack.itemValue.ItemClass.GetItemName() == itemID;
		}

		// Token: 0x0600C6AB RID: 50859 RVA: 0x00492EFC File Offset: 0x004910FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool CheckPhaseStatus(int index)
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(player).xui.PlayerInventory;
			ItemClass holdingItem = this.Owner.Owner.Player.inventory.holdingItem;
			if (playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && holdingItem.Name == this.ItemID)
			{
				return false;
			}
			if (index > 1)
			{
				return index != 2 || ((playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) || this.CheckDragDropItem(uiforPlayer.xui.DragAndDropWindow.CurrentStack, this.ItemID)) && holdingItem.Name != this.ItemID);
			}
			return !playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && !this.CheckDragDropItem(uiforPlayer.xui.DragAndDropWindow.CurrentStack, this.ItemID);
		}

		// Token: 0x0600C6AC RID: 50860 RVA: 0x0049300A File Offset: 0x0049120A
		public override BaseRequirementObjectiveGroup Clone()
		{
			return new RequirementObjectiveGroupPlace(this.ItemID);
		}

		// Token: 0x040095F9 RID: 38393
		public string ItemID = "";
	}
}
