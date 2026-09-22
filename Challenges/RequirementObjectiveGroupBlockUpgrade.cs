using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200191C RID: 6428
	[Preserve]
	public class RequirementObjectiveGroupBlockUpgrade : BaseRequirementObjectiveGroup
	{
		// Token: 0x0600C692 RID: 50834 RVA: 0x0049211A File Offset: 0x0049031A
		public RequirementObjectiveGroupBlockUpgrade(string itemID, string neededResourceID, int neededResourceCount)
		{
			this.ItemID = itemID;
			this.NeededResourceID = neededResourceID;
			this.NeededResourceCount = neededResourceCount;
		}

		// Token: 0x0600C693 RID: 50835 RVA: 0x00492154 File Offset: 0x00490354
		public override void CreateRequirements()
		{
			if (this.PhaseList == null)
			{
				this.PhaseList = new List<RequirementGroupPhase>();
			}
			this.ResourceRecipe = CraftingManager.GetRecipe(this.NeededResourceID);
			RequirementGroupPhase requirementGroupPhase;
			if (this.ResourceRecipe == null || (this.ResourceRecipe != null && this.ResourceRecipe.ingredients.Count == 0))
			{
				requirementGroupPhase = new RequirementGroupPhase();
				ChallengeObjectiveGather challengeObjectiveGather = new ChallengeObjectiveGather();
				challengeObjectiveGather.Owner = this.Owner;
				challengeObjectiveGather.IsRequirement = true;
				challengeObjectiveGather.Parent = this;
				challengeObjectiveGather.SetupItem(this.NeededResourceID);
				challengeObjectiveGather.MaxCount = this.NeededResourceCount;
				challengeObjectiveGather.Init();
				requirementGroupPhase.AddChallengeObjective(challengeObjectiveGather);
				this.PhaseList.Add(requirementGroupPhase);
			}
			else
			{
				requirementGroupPhase = this.AddIngredientGatheringReqs();
				if (requirementGroupPhase != null)
				{
					this.PhaseList.Add(requirementGroupPhase);
					requirementGroupPhase = new RequirementGroupPhase();
					ChallengeObjectiveCraft challengeObjectiveCraft = new ChallengeObjectiveCraft();
					challengeObjectiveCraft.Owner = this.Owner;
					challengeObjectiveCraft.SetupItem(this.NeededResourceID);
					challengeObjectiveCraft.IsRequirement = true;
					challengeObjectiveCraft.MaxCount = this.NeededResourceCount;
					challengeObjectiveCraft.Init();
					requirementGroupPhase.AddChallengeObjective(challengeObjectiveCraft);
					this.PhaseList.Add(requirementGroupPhase);
				}
			}
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

		// Token: 0x0600C694 RID: 50836 RVA: 0x004922B8 File Offset: 0x004904B8
		[PublicizedFrom(EAccessModifier.Private)]
		public RequirementGroupPhase AddIngredientGatheringReqs()
		{
			Recipe recipe = CraftingManager.GetRecipe(this.NeededResourceID);
			if (recipe == null)
			{
				return null;
			}
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
					challengeObjectiveGatherIngredient.NeededCount = this.NeededResourceCount;
					challengeObjectiveGatherIngredient.Init();
					challengeObjectiveGatherIngredient.MaxCount = num * this.NeededResourceCount;
					requirementGroupPhase.AddChallengeObjective(challengeObjectiveGatherIngredient);
				}
			}
			return requirementGroupPhase;
		}

		// Token: 0x0600C695 RID: 50837 RVA: 0x00492408 File Offset: 0x00490608
		public override bool HasPrerequisiteCondition()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = uiforPlayer.xui.PlayerInventory;
			ItemClass holdingItem = this.Owner.Owner.Player.inventory.holdingItem;
			return !playerInventory.HasItem(new ItemStack(ItemClass.GetItem(this.NeededResourceID, false), this.NeededResourceCount)) || (!playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && !this.CheckDragDropItem(uiforPlayer.xui.DragAndDropWindow.CurrentStack)) || holdingItem.Name != this.ItemID;
		}

		// Token: 0x0600C696 RID: 50838 RVA: 0x004924C1 File Offset: 0x004906C1
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckResourceDragDropItem(ItemStack stack)
		{
			return !stack.IsEmpty() && stack.itemValue.ItemClass.GetItemName() == this.NeededResourceID && stack.count >= this.NeededResourceCount;
		}

		// Token: 0x0600C697 RID: 50839 RVA: 0x004924FD File Offset: 0x004906FD
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckDragDropItem(ItemStack stack)
		{
			return !stack.IsEmpty() && stack.itemValue.ItemClass.GetItemName() == this.ItemID;
		}

		// Token: 0x0600C698 RID: 50840 RVA: 0x00492524 File Offset: 0x00490724
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool CheckPhaseStatus(int index)
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(player).xui.PlayerInventory;
			ItemClass holdingItem = this.Owner.Owner.Player.inventory.holdingItem;
			if (playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && playerInventory.HasItem(new ItemStack(ItemClass.GetItem(this.NeededResourceID, false), this.NeededResourceCount)) && holdingItem.Name == this.ItemID)
			{
				return false;
			}
			if (this.ResourceRecipe == null || (this.ResourceRecipe != null && this.ResourceRecipe.ingredients.Count == 0))
			{
				if (index == 0)
				{
					return !playerInventory.HasItem(new ItemStack(ItemClass.GetItem(this.NeededResourceID, false), this.NeededResourceCount)) && !this.CheckResourceDragDropItem(uiforPlayer.xui.DragAndDropWindow.CurrentStack);
				}
				if (index == 1)
				{
					return (playerInventory.HasItem(new ItemStack(ItemClass.GetItem(this.NeededResourceID, false), this.NeededResourceCount)) && !playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && !this.CheckDragDropItem(uiforPlayer.xui.DragAndDropWindow.CurrentStack)) || holdingItem.Name != this.ItemID;
				}
			}
			else
			{
				if (index <= 1)
				{
					return !playerInventory.HasItem(new ItemStack(ItemClass.GetItem(this.NeededResourceID, false), this.NeededResourceCount)) && !this.CheckResourceDragDropItem(uiforPlayer.xui.DragAndDropWindow.CurrentStack);
				}
				if (index == 2)
				{
					return (playerInventory.HasItem(new ItemStack(ItemClass.GetItem(this.NeededResourceID, false), this.NeededResourceCount)) && !playerInventory.HasItem(ItemClass.GetItem(this.ItemID, false)) && !this.CheckDragDropItem(uiforPlayer.xui.DragAndDropWindow.CurrentStack)) || holdingItem.Name != this.ItemID;
				}
			}
			return true;
		}

		// Token: 0x0600C699 RID: 50841 RVA: 0x00492740 File Offset: 0x00490940
		public override BaseRequirementObjectiveGroup Clone()
		{
			return new RequirementObjectiveGroupBlockUpgrade(this.ItemID, this.NeededResourceID, this.NeededResourceCount);
		}

		// Token: 0x040095EF RID: 38383
		public string ItemID = "";

		// Token: 0x040095F0 RID: 38384
		public string NeededResourceID = "";

		// Token: 0x040095F1 RID: 38385
		public int NeededResourceCount = 1;

		// Token: 0x040095F2 RID: 38386
		public Recipe ResourceRecipe;
	}
}
