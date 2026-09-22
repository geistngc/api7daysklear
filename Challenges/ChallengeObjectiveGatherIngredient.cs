using System;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001907 RID: 6407
	[Preserve]
	public class ChallengeObjectiveGatherIngredient : ChallengeBaseTrackedItemObjective
	{
		// Token: 0x1700186D RID: 6253
		// (get) Token: 0x0600C5C0 RID: 50624 RVA: 0x00081531 File Offset: 0x0007F731
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.GatherIngredient;
			}
		}

		// Token: 0x1700186E RID: 6254
		// (get) Token: 0x0600C5C1 RID: 50625 RVA: 0x0048E6FD File Offset: 0x0048C8FD
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveGather", false, null) + " " + this.expectedItemClass.GetLocalizedItemName();
			}
		}

		// Token: 0x1700186F RID: 6255
		// (get) Token: 0x0600C5C2 RID: 50626 RVA: 0x0048F00C File Offset: 0x0048D20C
		public override string StatusText
		{
			get
			{
				int num = Math.Max(0, this.MaxCount - this.currentNeededCount);
				if (base.Complete)
				{
					return string.Format("{0}/{1}", num, num);
				}
				return string.Format("{0}/{1}", this.current, num);
			}
		}

		// Token: 0x0600C5C3 RID: 50627 RVA: 0x0048F067 File Offset: 0x0048D267
		public override void Init()
		{
			this.expectedItem = this.itemRecipe.ingredients[this.IngredientIndex].itemValue;
			this.expectedItemClass = this.expectedItem.ItemClass;
		}

		// Token: 0x0600C5C4 RID: 50628 RVA: 0x0048F09C File Offset: 0x0048D29C
		public override void HandleAddHooks()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(player).xui.PlayerInventory;
			playerInventory.Backpack.OnBackpackItemsChangedInternal -= this.ItemsChangedInternal;
			playerInventory.Toolbelt.OnToolbeltItemsChangedInternal -= this.ItemsChangedInternal;
			playerInventory.Backpack.OnBackpackItemsChangedInternal += this.ItemsChangedInternal;
			playerInventory.Toolbelt.OnToolbeltItemsChangedInternal += this.ItemsChangedInternal;
			player.DragAndDropItemChanged += this.ItemsChangedInternal;
			base.HandleAddHooks();
			if (this.trackingEntry != null)
			{
				this.Owner.AddTrackingEntry(this.trackingEntry);
				this.trackingEntry.TrackingHelper = this.Owner.TrackingHandler;
				this.trackingEntry.AddHooks();
			}
		}

		// Token: 0x0600C5C5 RID: 50629 RVA: 0x0048F18B File Offset: 0x0048D38B
		public override bool CheckObjectiveComplete(bool handleComplete = true)
		{
			if (this.CheckForNeededItem())
			{
				base.Current = this.MaxCount;
				base.Complete = true;
				if (handleComplete)
				{
					this.Owner.HandleComplete(true, false);
				}
				return true;
			}
			base.Complete = false;
			return base.CheckObjectiveComplete(handleComplete);
		}

		// Token: 0x0600C5C6 RID: 50630 RVA: 0x0048F1C8 File Offset: 0x0048D3C8
		[PublicizedFrom(EAccessModifier.Private)]
		public void ItemsChangedInternal()
		{
			if (this.CheckObjectiveComplete(true))
			{
				if (this.trackingEntry != null)
				{
					this.trackingEntry.RemoveHooks();
				}
				this.Parent.CheckPrerequisites();
				return;
			}
			if (this.trackingEntry != null)
			{
				this.trackingEntry.AddHooks();
			}
		}

		// Token: 0x0600C5C7 RID: 50631 RVA: 0x0048E949 File Offset: 0x0048CB49
		public override void UpdateStatus()
		{
			base.UpdateStatus();
			if (base.Complete)
			{
				if (this.trackingEntry != null)
				{
					this.trackingEntry.RemoveHooks();
					return;
				}
			}
			else if (this.trackingEntry != null)
			{
				this.trackingEntry.AddHooks();
			}
		}

		// Token: 0x0600C5C8 RID: 50632 RVA: 0x0048F208 File Offset: 0x0048D408
		public override void HandleRemoveHooks()
		{
			EntityPlayerLocal player = this.Owner.Owner.Player;
			if (player == null)
			{
				return;
			}
			LocalPlayerUI.GetUIForPlayer(player);
			XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(player).xui.PlayerInventory;
			playerInventory.Backpack.OnBackpackItemsChangedInternal -= this.ItemsChangedInternal;
			playerInventory.Toolbelt.OnToolbeltItemsChangedInternal -= this.ItemsChangedInternal;
			player.DragAndDropItemChanged -= this.ItemsChangedInternal;
			if (this.trackingEntry != null)
			{
				this.trackingEntry.RemoveHooks();
				this.Owner.RemoveTrackingEntry(this.trackingEntry);
			}
		}

		// Token: 0x0600C5C9 RID: 50633 RVA: 0x0048F2AC File Offset: 0x0048D4AC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleUpdatingCurrent()
		{
			base.HandleUpdatingCurrent();
			int num = this.itemRecipe.ingredients[this.IngredientIndex].count;
			ItemValue itemValue = new ItemValue(this.itemRecipe.itemValueType, false);
			if (this.itemRecipe.UseIngredientModifier)
			{
				num = (int)EffectManager.GetValue(PassiveEffects.CraftingIngredientCount, null, (float)num, this.Owner.Owner.Player, this.itemRecipe, FastTags<TagGroup.Global>.Parse(this.expectedItemClass.GetItemName()), true, true, true, true, true, itemValue.HasQuality ? 1 : 0, true, false);
				if (num > 0)
				{
					num = (int)((float)num * XUiM_Recipes.GetCraftingInputModifier(this.itemRecipe));
					num = Utils.FastMax(1, num);
				}
			}
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			CraftingData craftingData = uiforPlayer.xui.GetCraftingData();
			XUiM_PlayerInventory playerInventory = uiforPlayer.xui.PlayerInventory;
			RecipeQueueItem[] recipeQueueItems = craftingData.RecipeQueueItems;
			int num2 = 0;
			if (recipeQueueItems != null)
			{
				foreach (RecipeQueueItem recipeQueueItem in recipeQueueItems)
				{
					if (recipeQueueItem.Recipe != null && recipeQueueItem.Recipe.itemValueType == this.itemRecipe.itemValueType)
					{
						num2 += recipeQueueItem.Recipe.count * (int)recipeQueueItem.Multiplier;
					}
				}
			}
			num2 += playerInventory.Backpack.GetItemCount(itemValue, -1, -1, true);
			num2 += playerInventory.Toolbelt.GetItemCount(itemValue, false, -1, -1, true);
			int num3 = this.IngredientCount * Math.Max(0, this.NeededCount - num2);
			int num4 = playerInventory.Backpack.GetItemCount(this.expectedItem, -1, -1, true);
			num4 += playerInventory.Toolbelt.GetItemCount(this.expectedItem, false, -1, -1, true);
			if (num4 > num3)
			{
				num4 = num3;
			}
			if (this.current != num4)
			{
				base.Current = num4;
			}
		}

		// Token: 0x0600C5CA RID: 50634 RVA: 0x0048F47C File Offset: 0x0048D67C
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckForNeededItem()
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = uiforPlayer.xui.PlayerInventory;
			ItemValue itemValue = new ItemValue(this.itemRecipe.itemValueType, false);
			RecipeQueueItem[] recipeQueueItems = uiforPlayer.xui.GetCraftingData().RecipeQueueItems;
			int num = playerInventory.Backpack.GetItemCount(this.expectedItem, -1, -1, true);
			num += playerInventory.Toolbelt.GetItemCount(this.expectedItem, false, -1, -1, true);
			this.currentNeededCount = 0;
			this.currentNeededCount = playerInventory.Backpack.GetItemCount(itemValue, -1, -1, true);
			this.currentNeededCount += playerInventory.Toolbelt.GetItemCount(itemValue, false, -1, -1, true);
			int num2 = 0;
			if (recipeQueueItems != null)
			{
				foreach (RecipeQueueItem recipeQueueItem in recipeQueueItems)
				{
					if (recipeQueueItem.Recipe != null && recipeQueueItem.Recipe.itemValueType == this.itemRecipe.itemValueType)
					{
						num2 += recipeQueueItem.Recipe.count * (int)recipeQueueItem.Multiplier;
					}
				}
			}
			return num >= this.IngredientCount * Math.Max(0, this.NeededCount - (this.currentNeededCount + num2));
		}

		// Token: 0x0600C5CB RID: 50635 RVA: 0x0048F5B0 File Offset: 0x0048D7B0
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveGatherIngredient
			{
				itemRecipe = this.itemRecipe,
				IngredientIndex = this.IngredientIndex,
				expectedItem = this.expectedItem,
				expectedItemClass = this.expectedItemClass,
				NeededCount = this.NeededCount
			};
		}

		// Token: 0x0400959D RID: 38301
		public Recipe itemRecipe;

		// Token: 0x0400959E RID: 38302
		public int IngredientIndex = -1;

		// Token: 0x0400959F RID: 38303
		public int IngredientCount = -1;

		// Token: 0x040095A0 RID: 38304
		public int NeededCount;

		// Token: 0x040095A1 RID: 38305
		public int currentNeededCount;

		// Token: 0x040095A2 RID: 38306
		public BaseRequirementObjectiveGroup Parent;
	}
}
