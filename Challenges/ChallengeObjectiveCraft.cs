using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001902 RID: 6402
	[Preserve]
	public class ChallengeObjectiveCraft : BaseChallengeObjective
	{
		// Token: 0x17001862 RID: 6242
		// (get) Token: 0x0600C57B RID: 50555 RVA: 0x00080864 File Offset: 0x0007EA64
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Craft;
			}
		}

		// Token: 0x17001863 RID: 6243
		// (get) Token: 0x0600C57C RID: 50556 RVA: 0x0048DFCF File Offset: 0x0048C1CF
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("lblContextActionCraft", false, null) + " " + Localization.Get(this.itemClassID, false, null) + ":";
			}
		}

		// Token: 0x17001864 RID: 6244
		// (get) Token: 0x0600C57D RID: 50557 RVA: 0x0048DFF9 File Offset: 0x0048C1F9
		public override ChallengeClass.UINavTypes NavType
		{
			get
			{
				if (this.itemRecipe == null)
				{
					return ChallengeClass.UINavTypes.None;
				}
				if (!(this.itemRecipe.craftingArea == ""))
				{
					return ChallengeClass.UINavTypes.None;
				}
				return ChallengeClass.UINavTypes.Crafting;
			}
		}

		// Token: 0x0600C57E RID: 50558 RVA: 0x0048E01F File Offset: 0x0048C21F
		public override void Init()
		{
			this.expectedItem = ItemClass.GetItem(this.itemClassID, false);
			this.expectedItemClass = ItemClass.GetItemClass(this.itemClassID, false);
			this.itemRecipe = CraftingManager.GetRecipe(this.itemClassID);
		}

		// Token: 0x0600C57F RID: 50559 RVA: 0x0048E056 File Offset: 0x0048C256
		public override void HandleOnCreated()
		{
			base.HandleOnCreated();
			this.CreateRequirements();
		}

		// Token: 0x0600C580 RID: 50560 RVA: 0x0048E064 File Offset: 0x0048C264
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreateRequirements()
		{
			if (this.itemClassIDs.Length > 1)
			{
				return;
			}
			if (!this.ShowRequirements)
			{
				return;
			}
			this.Owner.SetRequirementGroup(new RequirementObjectiveGroupGatherIngredients(this.itemClassID)
			{
				CraftObj = this
			});
		}

		// Token: 0x0600C581 RID: 50561 RVA: 0x0048E098 File Offset: 0x0048C298
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.CraftItem -= this.Current_CraftItem;
			QuestEventManager.Current.CraftItem += this.Current_CraftItem;
		}

		// Token: 0x0600C582 RID: 50562 RVA: 0x0048E0C6 File Offset: 0x0048C2C6
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.CraftItem -= this.Current_CraftItem;
		}

		// Token: 0x0600C583 RID: 50563 RVA: 0x0048E0DE File Offset: 0x0048C2DE
		public override void HandleTrackingStarted()
		{
			base.HandleTrackingStarted();
		}

		// Token: 0x0600C584 RID: 50564 RVA: 0x0048E0E6 File Offset: 0x0048C2E6
		public override void HandleTrackingEnded()
		{
			base.HandleTrackingEnded();
		}

		// Token: 0x0600C585 RID: 50565 RVA: 0x0048E0F0 File Offset: 0x0048C2F0
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_CraftItem(ItemStack stack)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			ItemClass itemClass = stack.itemValue.ItemClass;
			if (itemClass != null && this.itemClassIDs.ContainsCaseInsensitive(itemClass.Name))
			{
				base.Current += stack.count;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600C586 RID: 50566 RVA: 0x0048E143 File Offset: 0x0048C343
		public override bool CheckObjectiveComplete(bool handleComplete = true)
		{
			if (this.IsRequirement && this.CheckForNeededItem())
			{
				base.Complete = true;
				this.HandleRecipeListUpdate();
				return true;
			}
			base.Complete = false;
			this.HandleRecipeListUpdate();
			return base.CheckObjectiveComplete(handleComplete);
		}

		// Token: 0x0600C587 RID: 50567 RVA: 0x0048E178 File Offset: 0x0048C378
		public override Recipe GetRecipeItem()
		{
			return this.itemRecipe;
		}

		// Token: 0x0600C588 RID: 50568 RVA: 0x0048E180 File Offset: 0x0048C380
		public override Recipe[] GetRecipeItems()
		{
			Recipe recipeFromRequirements = this.Owner.GetRecipeFromRequirements();
			if (recipeFromRequirements != null)
			{
				return new Recipe[]
				{
					recipeFromRequirements,
					this.itemRecipe
				};
			}
			return new Recipe[]
			{
				this.itemRecipe
			};
		}

		// Token: 0x0600C589 RID: 50569 RVA: 0x0048E1BF File Offset: 0x0048C3BF
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.SetupItem(e.GetAttribute("item"));
			}
		}

		// Token: 0x0600C58A RID: 50570 RVA: 0x0048E1F0 File Offset: 0x0048C3F0
		public void SetupItem(string itemID)
		{
			this.itemClassID = itemID;
			if (this.itemClassID.Contains(','))
			{
				this.itemClassIDs = this.itemClassID.Split(',', StringSplitOptions.None);
				this.itemClassID = this.itemClassIDs[0];
				return;
			}
			this.itemClassIDs = new string[1];
			this.itemClassIDs[0] = this.itemClassID;
		}

		// Token: 0x0600C58B RID: 50571 RVA: 0x0048E250 File Offset: 0x0048C450
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckForNeededItem()
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = uiforPlayer.xui.PlayerInventory;
			ItemValue itemValue = new ItemValue(this.itemRecipe.itemValueType, false);
			int num = playerInventory.Backpack.GetItemCount(itemValue, -1, -1, true);
			num += playerInventory.Toolbelt.GetItemCount(itemValue, false, -1, -1, true);
			ItemStack currentStack = uiforPlayer.xui.DragAndDropWindow.CurrentStack;
			if (!currentStack.IsEmpty() && currentStack.itemValue.type == this.itemRecipe.itemValueType)
			{
				num += currentStack.count;
			}
			base.Current = num;
			return num >= this.MaxCount;
		}

		// Token: 0x0600C58C RID: 50572 RVA: 0x0048E300 File Offset: 0x0048C500
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleRecipeListUpdate()
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			if (uiforPlayer.xui.QuestTracker.TrackedChallenge == this.Owner)
			{
				uiforPlayer.xui.QuestTracker.HandleTrackedChallengeChanged();
			}
		}

		// Token: 0x0600C58D RID: 50573 RVA: 0x0048E34C File Offset: 0x0048C54C
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveCraft
			{
				itemClassIDs = this.itemClassIDs,
				itemClassID = this.itemClassID,
				itemRecipe = this.itemRecipe,
				expectedItem = this.expectedItem,
				expectedItemClass = this.expectedItemClass
			};
		}

		// Token: 0x0600C58E RID: 50574 RVA: 0x0048D363 File Offset: 0x0048B563
		public override void CompleteObjective(bool handleComplete = true)
		{
			base.Current = this.MaxCount;
			base.Complete = true;
			if (handleComplete)
			{
				this.Owner.HandleComplete(true, false);
			}
		}

		// Token: 0x0400958E RID: 38286
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None;

		// Token: 0x0400958F RID: 38287
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;

		// Token: 0x04009590 RID: 38288
		public string[] itemClassIDs;

		// Token: 0x04009591 RID: 38289
		public string itemClassID = "";

		// Token: 0x04009592 RID: 38290
		public Recipe itemRecipe;
	}
}
