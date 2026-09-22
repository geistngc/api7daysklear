using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Challenges;
using UnityEngine;

// Token: 0x0200113F RID: 4415
public class XUiM_Recipes : XUiModel
{
	// Token: 0x140000EB RID: 235
	// (add) Token: 0x06008B88 RID: 35720 RVA: 0x003522C8 File Offset: 0x003504C8
	// (remove) Token: 0x06008B89 RID: 35721 RVA: 0x00352300 File Offset: 0x00350500
	public event XUiEvent_TrackedQuestChanged OnTrackedRecipeChanged;

	// Token: 0x1700101D RID: 4125
	// (get) Token: 0x06008B8A RID: 35722 RVA: 0x00352335 File Offset: 0x00350535
	// (set) Token: 0x06008B8B RID: 35723 RVA: 0x00352340 File Offset: 0x00350540
	public Recipe TrackedRecipe
	{
		get
		{
			return this.trackedRecipe;
		}
		set
		{
			if (this.trackedRecipe != null)
			{
				this.trackedRecipe.IsTracked = false;
			}
			this.trackedRecipe = value;
			if (this.trackedRecipe != null)
			{
				this.trackedRecipe.IsTracked = true;
			}
			if (this.OnTrackedRecipeChanged != null)
			{
				this.OnTrackedRecipeChanged();
			}
		}
	}

	// Token: 0x06008B8C RID: 35724 RVA: 0x00352390 File Offset: 0x00350590
	public void SetPreviousTracked(EntityPlayerLocal player)
	{
		XUi xui = player.PlayerUI.xui;
		Quest trackedQuest = player.QuestJournal.TrackedQuest;
		XUiM_Recipes.sPreviouslyTrackedChallenge = xui.QuestTracker.TrackedChallenge;
		player.QuestJournal.TrackedQuest = null;
		xui.QuestTracker.TrackedChallenge = null;
		XUiM_Recipes.sPreviouslyTrackedQuest = trackedQuest;
	}

	// Token: 0x06008B8D RID: 35725 RVA: 0x003523E4 File Offset: 0x003505E4
	public void ResetToPreviousTracked(EntityPlayerLocal player)
	{
		XUi xui = player.PlayerUI.xui;
		if (XUiM_Recipes.sPreviouslyTrackedQuest != null)
		{
			if (!player.QuestJournal.QuestIsActive(XUiM_Recipes.sPreviouslyTrackedQuest))
			{
				XUiM_Recipes.sPreviouslyTrackedQuest = player.QuestJournal.FindActiveQuest();
			}
		}
		else if (XUiM_Recipes.sPreviouslyTrackedChallenge != null && !XUiM_Recipes.sPreviouslyTrackedChallenge.IsActive)
		{
			XUiM_Recipes.sPreviouslyTrackedChallenge = XUiM_Recipes.sPreviouslyTrackedChallenge.Owner.GetNextChallenge(XUiM_Recipes.sPreviouslyTrackedChallenge);
		}
		player.QuestJournal.TrackedQuest = XUiM_Recipes.sPreviouslyTrackedQuest;
		xui.QuestTracker.TrackedChallenge = XUiM_Recipes.sPreviouslyTrackedChallenge;
	}

	// Token: 0x06008B8E RID: 35726 RVA: 0x00352472 File Offset: 0x00350672
	public void RefreshTrackedRecipe()
	{
		if (this.OnTrackedRecipeChanged != null)
		{
			this.OnTrackedRecipeChanged();
		}
	}

	// Token: 0x06008B8F RID: 35727 RVA: 0x00352487 File Offset: 0x00350687
	public static ReadOnlyCollection<Recipe> GetRecipes()
	{
		return CraftingManager.NonScrapableRecipes;
	}

	// Token: 0x06008B90 RID: 35728 RVA: 0x00352490 File Offset: 0x00350690
	public static void UpdateRecipesforBackpackCrafting()
	{
		ReadOnlyCollection<Recipe> recipes = XUiM_Recipes.GetRecipes();
		for (int i = 0; i < recipes.Count; i++)
		{
			if (recipes[i].craftingArea == "")
			{
				BackpackCraftingOptions backpackCrafting = XUiM_Recipes.BackpackCrafting;
				if (backpackCrafting != BackpackCraftingOptions.BasicsOnly)
				{
					if (backpackCrafting != BackpackCraftingOptions.WorkbenchOnly)
					{
						recipes[i].craftingArea = "workbench";
					}
					else if (!recipes[i].tags.Test_AnySet(XUiM_Recipes.workbenchOnlyTag))
					{
						recipes[i].craftingArea = "workbench";
					}
				}
				else if (!recipes[i].GetOutputItemClass().IsLimitedItem)
				{
					recipes[i].craftingArea = "workbench";
				}
			}
		}
	}

	// Token: 0x06008B91 RID: 35729 RVA: 0x00352544 File Offset: 0x00350744
	public static void FilterRecipesByCategory(string _category, ref List<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			Recipe recipe = recipeList[i];
			if (recipe.isChallenge)
			{
				list.Add(recipe);
			}
			else if (recipe.isQuest)
			{
				list.Add(recipe);
			}
			else if (recipe.IsTracked)
			{
				list.Add(recipe);
			}
			else
			{
				ItemClass forId = ItemClass.GetForId(recipe.itemValueType);
				if (forId != null)
				{
					string[] array;
					if (!forId.IsBlock())
					{
						array = forId.Groups;
					}
					else
					{
						array = Block.list[forId.Id].GroupNames;
					}
					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							if (array[j] != null && array[j].EqualsCaseInsensitive(_category))
							{
								list.Add(recipe);
								break;
							}
						}
					}
				}
			}
		}
		recipeList = list;
	}

	// Token: 0x06008B92 RID: 35730 RVA: 0x0035261C File Offset: 0x0035081C
	public static List<Recipe> FilterRecipesByWorkstation(string _workstation, IList<Recipe> recipeList)
	{
		if (_workstation == null)
		{
			_workstation = "";
		}
		List<Recipe> list = new List<Recipe>();
		if (_workstation != "")
		{
			if (!XUiM_Recipes.WorkstationCrafting)
			{
				return list;
			}
		}
		else if (XUiM_Recipes.BackpackCrafting == BackpackCraftingOptions.Disabled)
		{
			return list;
		}
		for (int i = 0; i < recipeList.Count; i++)
		{
			if (recipeList[i] != null)
			{
				Recipe recipe = recipeList[i];
				if ((World.BiomeProgressionEnabled || !recipe.tags.Test_AnySet(XUiM_Recipes.biomeprogressionTag)) && (ItemClass.MaxTechType >= ItemClass.ItemTechTypes.T3 || recipe.GetOutputItemClass().ItemTechType <= ItemClass.MaxTechType))
				{
					if (_workstation != "")
					{
						Block blockByName = Block.GetBlockByName(_workstation, false);
						if (blockByName != null && blockByName.Properties.Contains("Workstation", "CraftingAreaRecipes"))
						{
							string @string = blockByName.Properties.GetString("Workstation", "CraftingAreaRecipes");
							string[] array = new string[]
							{
								@string
							};
							if (@string.Contains(","))
							{
								array = @string.Replace(", ", ",").Replace(" ,", ",").Replace(" , ", ",").Split(',', StringSplitOptions.None);
							}
							bool flag = false;
							for (int j = 0; j < array.Length; j++)
							{
								if (recipe.craftingArea != null && recipe.craftingArea.EqualsCaseInsensitive(array[j]))
								{
									if (recipe.craftingArea == "forge")
									{
										if (XUiM_Recipes.DisableSmelter && recipe.tags.Test_AnySet(XUiM_Recipes.useSmelterTag))
										{
											flag = true;
											goto IL_1DC;
										}
										if (!XUiM_Recipes.DisableSmelter && recipe.tags.Test_AnySet(XUiM_Recipes.replaceSmelterTag))
										{
											flag = true;
											goto IL_1DC;
										}
									}
									list.Add(recipe);
									flag = true;
									break;
								}
								if ((recipe.craftingArea == null || recipe.craftingArea == "") && array[j].EqualsCaseInsensitive("player"))
								{
									list.Add(recipe);
									flag = true;
									break;
								}
								IL_1DC:;
							}
							if (!flag && recipe.craftingArea != null && recipe.craftingArea.EqualsCaseInsensitive(_workstation))
							{
								list.Add(recipe);
							}
						}
						else if (recipe.craftingArea != null && recipe.craftingArea.EqualsCaseInsensitive(_workstation))
						{
							list.Add(recipe);
						}
					}
					else if ((recipe.craftingArea == null || recipe.craftingArea == "") && XUiM_Recipes.FilterByBackpackSandboxSetting(recipe))
					{
						list.Add(recipe);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06008B93 RID: 35731 RVA: 0x00352894 File Offset: 0x00350A94
	public static List<Recipe> FilterRecipesByName(string _name, IList<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			Recipe recipe = recipeList[i];
			if (!recipe.craftingArea.EqualsCaseInsensitive("assembly") && (World.BiomeProgressionEnabled || !recipe.tags.Test_AnySet(XUiM_Recipes.biomeprogressionTag)))
			{
				ItemClass forId = ItemClass.GetForId(recipe.itemValueType);
				if (forId != null && (ItemClass.MaxTechType >= ItemClass.ItemTechTypes.T3 || forId.ItemTechType <= ItemClass.MaxTechType) && (!(recipe.craftingArea == "forge") || ((!XUiM_Recipes.DisableSmelter || !recipe.tags.Test_AnySet(XUiM_Recipes.useSmelterTag)) && (XUiM_Recipes.DisableSmelter || !recipe.tags.Test_AnySet(XUiM_Recipes.replaceSmelterTag)))))
				{
					string a;
					if (Localization.TryGet(recipe.GetName(), out a) && a.ContainsCaseInsensitive(_name))
					{
						list.Add(recipe);
					}
					else if (!forId.IsBlock())
					{
						if (forId.GetItemName().ContainsCaseInsensitive(_name))
						{
							list.Add(recipe);
						}
						else if (forId.GetLocalizedItemName().ContainsCaseInsensitive(_name))
						{
							list.Add(recipe);
						}
					}
					else
					{
						Block block = Block.list[forId.Id];
						if (block != null)
						{
							if (block.GetBlockName().ContainsCaseInsensitive(_name))
							{
								list.Add(recipe);
							}
							else if (block.GetLocalizedBlockName().ContainsCaseInsensitive(_name))
							{
								list.Add(recipe);
							}
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06008B94 RID: 35732 RVA: 0x00352A04 File Offset: 0x00350C04
	public static List<Recipe> FilterRecipesByIngredient(ItemStack stack, IList<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		ItemValue[] items = new ItemValue[]
		{
			stack.itemValue
		};
		for (int i = 0; i < recipeList.Count; i++)
		{
			if (recipeList[i].ContainsIngredients(items))
			{
				list.Add(recipeList[i]);
			}
		}
		return list;
	}

	// Token: 0x06008B95 RID: 35733 RVA: 0x00352A58 File Offset: 0x00350C58
	public static List<Recipe> FilterRecipesByItem(List<int> itemIDs, IList<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			if (itemIDs.Contains(recipeList[i].itemValueType))
			{
				list.Add(recipeList[i]);
			}
		}
		return list;
	}

	// Token: 0x06008B96 RID: 35734 RVA: 0x00352AA0 File Offset: 0x00350CA0
	public static List<Recipe> FilterRecipesByID(int itemID, IList<Recipe> recipeList)
	{
		List<Recipe> list = new List<Recipe>();
		for (int i = 0; i < recipeList.Count; i++)
		{
			if (itemID == recipeList[i].itemValueType)
			{
				list.Add(recipeList[i]);
			}
		}
		return list;
	}

	// Token: 0x06008B97 RID: 35735 RVA: 0x00352AE4 File Offset: 0x00350CE4
	public static bool FilterByBackpackSandboxSetting(Recipe recipe)
	{
		if ((XUiM_Recipes.BackpackCrafting == BackpackCraftingOptions.Disabled && recipe.craftingArea == "") || (XUiM_Recipes.BackpackCrafting == BackpackCraftingOptions.WorkbenchOnly && !recipe.tags.Test_AnySet(XUiM_Recipes.workbenchOnlyTag)))
		{
			return false;
		}
		if (XUiM_Recipes.BackpackCrafting != BackpackCraftingOptions.BasicsOnly)
		{
			return true;
		}
		ItemClass forId = ItemClass.GetForId(recipe.itemValueType);
		if (forId == null)
		{
			return false;
		}
		string[] array;
		if (!forId.IsBlock())
		{
			array = forId.Groups;
		}
		else
		{
			array = Block.list[forId.Id].GroupNames;
		}
		if (array == null)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].EqualsCaseInsensitive("limited"))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06008B98 RID: 35736 RVA: 0x00352B8E File Offset: 0x00350D8E
	public static int GetCount(XUi xui)
	{
		return CraftingManager.GetRecipes().Count;
	}

	// Token: 0x06008B99 RID: 35737 RVA: 0x00352B9A File Offset: 0x00350D9A
	public static string GetRecipeName(Recipe _recipe)
	{
		return _recipe.GetName();
	}

	// Token: 0x06008B9A RID: 35738 RVA: 0x00352BA2 File Offset: 0x00350DA2
	public static string GetRecipeSpriteName(Recipe _recipe)
	{
		return _recipe.GetIcon();
	}

	// Token: 0x06008B9B RID: 35739 RVA: 0x00352BAA File Offset: 0x00350DAA
	public static bool GetRecipeIsUnlocked(XUi xui, Recipe _recipe)
	{
		return _recipe.IsUnlocked(xui.playerUI.entityPlayer);
	}

	// Token: 0x06008B9C RID: 35740 RVA: 0x00352BC0 File Offset: 0x00350DC0
	public static bool GetRecipeIsUnlocked(XUi xui, string itemName)
	{
		Recipe recipe = CraftingManager.GetRecipe(itemName);
		return recipe != null && XUiM_Recipes.GetRecipeIsUnlocked(xui, recipe);
	}

	// Token: 0x06008B9D RID: 35741 RVA: 0x00352BE0 File Offset: 0x00350DE0
	public static bool GetRecipeIsUnlockable(XUi xui, Recipe _recipe)
	{
		return _recipe.IsLearnable;
	}

	// Token: 0x06008B9E RID: 35742 RVA: 0x00352BE8 File Offset: 0x00350DE8
	public static bool GetRecipeIsFavorite(XUi xui, Recipe _recipe)
	{
		return CraftingManager.RecipeIsFavorite(_recipe);
	}

	// Token: 0x06008B9F RID: 35743 RVA: 0x00352BF0 File Offset: 0x00350DF0
	public static float GetRecipeCraftTime(XUi xui, Recipe _recipe)
	{
		float num = EffectManager.GetValue(PassiveEffects.CraftingTime, null, _recipe.craftingTime, xui.playerUI.entityPlayer, _recipe, _recipe.tags, true, true, true, true, true, 1, true, false) * XUiM_Recipes.CraftingTimeModifier;
		if (num < 0f)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x06008BA0 RID: 35744 RVA: 0x00352C3C File Offset: 0x00350E3C
	public static int GetRecipeCraftOutputCount(XUi xui, Recipe _recipe)
	{
		float num = XUiM_Recipes.CraftingOutputModifier;
		if (_recipe.tags.Test_AnySet(XUiM_Recipes.SandboxIgnoreTag))
		{
			num = 1f;
		}
		return (int)Mathf.Max(EffectManager.GetValue(PassiveEffects.CraftingOutputCount, null, (float)_recipe.count, xui.playerUI.entityPlayer, _recipe, _recipe.tags, true, true, true, true, true, 1, true, false) * num, 1f);
	}

	// Token: 0x06008BA1 RID: 35745 RVA: 0x00352C9D File Offset: 0x00350E9D
	public static ItemStack GetRecipeOutput(Recipe _recipe)
	{
		return new ItemStack(new ItemValue(_recipe.itemValueType, false), _recipe.count);
	}

	// Token: 0x06008BA2 RID: 35746 RVA: 0x00352CB6 File Offset: 0x00350EB6
	public static List<ItemStack> GetRecipeIngredients(Recipe _recipe)
	{
		return _recipe.GetIngredientsSummedUp();
	}

	// Token: 0x06008BA3 RID: 35747 RVA: 0x000027FC File Offset: 0x000009FC
	public static void GetCurrentSlots()
	{
	}

	// Token: 0x06008BA4 RID: 35748 RVA: 0x00352CBE File Offset: 0x00350EBE
	public static bool HasIngredientsForRecipe(IList<ItemStack> allItems, Recipe _recipe, EntityAlive _ea = null)
	{
		return _recipe.CanCraftAny(allItems, _ea);
	}

	// Token: 0x06008BA5 RID: 35749 RVA: 0x00352CC8 File Offset: 0x00350EC8
	public static float GetCraftingInputModifier(Recipe recipe)
	{
		if (recipe.tags.Test_AnySet(XUiM_Recipes.SandboxIgnoreTag))
		{
			return 1f;
		}
		return XUiM_Recipes.CraftingInputModifier;
	}

	// Token: 0x0400671F RID: 26399
	public static float CraftingTimeModifier = 1f;

	// Token: 0x04006720 RID: 26400
	public static float CraftingInputModifier = 1f;

	// Token: 0x04006721 RID: 26401
	public static float CraftingOutputModifier = 1f;

	// Token: 0x04006722 RID: 26402
	public static float SmeltingTimeModifier = 1f;

	// Token: 0x04006723 RID: 26403
	public static bool DisableSmelter = false;

	// Token: 0x04006724 RID: 26404
	public static float ScrappingOutputModifier = 1f;

	// Token: 0x04006725 RID: 26405
	public static float SmeltingOutputModifier = 1f;

	// Token: 0x04006726 RID: 26406
	public static float DewCollectorTimeModifier = 1f;

	// Token: 0x04006727 RID: 26407
	public static float DewCollectorOutput = 1f;

	// Token: 0x04006728 RID: 26408
	public static float DewCollectorInput = 1f;

	// Token: 0x04006729 RID: 26409
	public static float ApiaryTimeModifier = 1f;

	// Token: 0x0400672A RID: 26410
	public static float ApiaryOutput = 1f;

	// Token: 0x0400672B RID: 26411
	public static float ApiaryInput = 1f;

	// Token: 0x0400672C RID: 26412
	public static float ChickenCoopTimeModifier = 1f;

	// Token: 0x0400672D RID: 26413
	public static float ChickenCoopOutput = 1f;

	// Token: 0x0400672E RID: 26414
	public static float ChickenCoopInput = 1f;

	// Token: 0x0400672F RID: 26415
	public static float CollectorTimeModifier = 1f;

	// Token: 0x04006730 RID: 26416
	public static int CollectorOutput = 1;

	// Token: 0x04006731 RID: 26417
	public static float MiningOutputModifier = 1f;

	// Token: 0x04006732 RID: 26418
	public static float HarvestingOutputModifier = 1f;

	// Token: 0x04006733 RID: 26419
	public static float CropOutputModifier = 1f;

	// Token: 0x04006734 RID: 26420
	public static float SeedDropOutputModifier = 1f;

	// Token: 0x04006735 RID: 26421
	public static int CraftingMaxTier = 6;

	// Token: 0x04006736 RID: 26422
	public static bool CraftingProgression = true;

	// Token: 0x04006737 RID: 26423
	public static BackpackCraftingOptions BackpackCrafting = BackpackCraftingOptions.Enabled;

	// Token: 0x04006738 RID: 26424
	public static bool WorkstationCrafting = true;

	// Token: 0x0400673A RID: 26426
	[PublicizedFrom(EAccessModifier.Private)]
	public static Quest sPreviouslyTrackedQuest = null;

	// Token: 0x0400673B RID: 26427
	[PublicizedFrom(EAccessModifier.Private)]
	public static Challenge sPreviouslyTrackedChallenge = null;

	// Token: 0x0400673C RID: 26428
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> SandboxIgnoreTag = FastTags<TagGroup.Global>.Parse("sandboxIgnore");

	// Token: 0x0400673D RID: 26429
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> workbenchTag = FastTags<TagGroup.Global>.Parse("workbench");

	// Token: 0x0400673E RID: 26430
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe trackedRecipe;

	// Token: 0x0400673F RID: 26431
	public int TrackedRecipeQuality = 1;

	// Token: 0x04006740 RID: 26432
	public int TrackedRecipeCount = 1;

	// Token: 0x04006741 RID: 26433
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> workbenchOnlyTag = FastTags<TagGroup.Global>.Parse("isworkbench");

	// Token: 0x04006742 RID: 26434
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> biomeprogressionTag = FastTags<TagGroup.Global>.Parse("biomeProgression");

	// Token: 0x04006743 RID: 26435
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> useSmelterTag = FastTags<TagGroup.Global>.Parse("use_smelter");

	// Token: 0x04006744 RID: 26436
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> replaceSmelterTag = FastTags<TagGroup.Global>.Parse("replace_smelter");
}
