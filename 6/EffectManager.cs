using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000657 RID: 1623
public static class EffectManager
{
	// Token: 0x06003487 RID: 13447 RVA: 0x0015CFF8 File Offset: 0x0015B1F8
	public static float GetValue(PassiveEffects _passiveEffect, ItemValue _originalItemValue = null, float _originalValue = 0f, EntityAlive _entity = null, Recipe _recipe = null, FastTags<TagGroup.Global> tags = default(FastTags<TagGroup.Global>), bool calcEquipment = true, bool calcHoldingItem = true, bool calcProgression = true, bool calcBuffs = true, bool calcChallenges = true, int craftingTier = 1, bool useMods = true, bool _useDurability = false)
	{
		float num = 1f;
		if (_entity != null)
		{
			MinEventParams.CopyTo(_entity.MinEventContext, MinEventParams.CachedEventParam);
		}
		if (_originalItemValue != null)
		{
			if (_entity != null && _entity.MinEventContext.ItemValue == null)
			{
				_entity.MinEventContext.ItemValue = _originalItemValue;
			}
			MinEventParams.CachedEventParam.ItemValue = _originalItemValue;
			if (_originalItemValue.type != 0 && tags.IsEmpty)
			{
				ItemClass itemClass = _originalItemValue.ItemClass;
				if (itemClass != null)
				{
					tags = itemClass.ItemTags;
				}
			}
		}
		if (_entity == null)
		{
			if (_recipe != null)
			{
				_recipe.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags, craftingTier);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref num, tags, true, false);
			}
		}
		else
		{
			if (GameManager.Instance == null || GameManager.Instance.gameStateManager == null || !GameManager.Instance.gameStateManager.IsGameStarted())
			{
				return _originalValue;
			}
			EntityClass entityClass;
			if (EntityClass.list.TryGetValue(_entity.entityClass, out entityClass) && entityClass.Effects != null)
			{
				entityClass.Effects.ModifyValue(_entity, _passiveEffect, ref _originalValue, ref num, 0f, tags, 1);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref num, tags, useMods, false);
			}
			else
			{
				EntityVehicle entityVehicle = _entity as EntityVehicle;
				if (entityVehicle != null)
				{
					Vehicle vehicle = entityVehicle.GetVehicle();
					if (vehicle != null)
					{
						vehicle.GetUpdatedItemValue().ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref num, tags, true, false);
					}
				}
				else if (calcHoldingItem && _entity.inventory != null && _entity.inventory.holdingItemItemValue != _originalItemValue && !_entity.inventory.holdingItemItemValue.IsMod)
				{
					_entity.inventory.ModifyValue(_originalItemValue, _passiveEffect, ref _originalValue, ref num, tags);
				}
			}
			if (calcEquipment && _entity.equipment != null)
			{
				_entity.equipment.ModifyValue(_originalItemValue, _passiveEffect, ref _originalValue, ref num, tags, _useDurability);
			}
			if (_originalItemValue != null)
			{
				if (_entity != null)
				{
					_entity.MinEventContext.ItemValue = _originalItemValue;
				}
				MinEventParams.CachedEventParam.ItemValue = _originalItemValue;
			}
			if (calcProgression && _entity.Progression != null)
			{
				_entity.Progression.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags);
			}
			if (calcChallenges && _entity.challengeJournal != null)
			{
				_entity.challengeJournal.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags);
			}
			if (_recipe != null)
			{
				_recipe.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags, craftingTier);
			}
			EntityPlayerLocal entityPlayerLocal = _entity as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				if (EffectManager.slotsCached == null || _entity.entityId != EffectManager.slotsQueriedForEntity || EffectManager.slotsQueriedFrame != Time.frameCount)
				{
					LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
					EffectManager.slotsCached = ((uiforPlayer.xui.CurrentWorkstationToolGrid != null) ? uiforPlayer.xui.CurrentWorkstationToolGrid.GetSlots() : null);
					EffectManager.slotsQueriedFrame = Time.frameCount;
					EffectManager.slotsQueriedForEntity = _entity.entityId;
				}
				if (EffectManager.slotsCached != null)
				{
					for (int i = 0; i < EffectManager.slotsCached.Length; i++)
					{
						if (!EffectManager.slotsCached[i].IsEmpty())
						{
							EffectManager.slotsCached[i].itemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref num, tags, true, false);
						}
					}
				}
			}
			if (calcBuffs && _entity.Buffs != null)
			{
				_entity.Buffs.ModifyValue(_passiveEffect, ref _originalValue, ref num, tags);
			}
		}
		if (_originalItemValue != null && _originalItemValue.ItemClass != null && _originalItemValue.Quality > 0 && useMods && _originalItemValue.ItemClass.Effects != null)
		{
			for (int j = 0; j < _originalItemValue.Modifications.Length; j++)
			{
				if (_originalItemValue.Modifications[j] != null && _originalItemValue.Modifications[j].ItemClass is ItemClassModifier)
				{
					_originalItemValue.ItemClass.Effects.ModifyValue(_entity, PassiveEffects.ModPowerBonus, ref _originalValue, ref num, (float)_originalItemValue.Quality, FastTags<TagGroup.Global>.Parse(_passiveEffect.ToStringCached<PassiveEffects>()), 1);
				}
			}
		}
		return _originalValue * num;
	}

	// Token: 0x06003488 RID: 13448 RVA: 0x0015D3C4 File Offset: 0x0015B5C4
	public static float GetItemValue(PassiveEffects _passiveEffect, ItemValue _originalItemValue, float _originalValue = 0f)
	{
		float num = 1f;
		if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
		{
			MinEventParams.CachedEventParam.ItemValue = _originalItemValue;
			_originalItemValue.ModifyValue(null, null, _passiveEffect, ref _originalValue, ref num, _originalItemValue.ItemClass.ItemTags, true, false);
		}
		return _originalValue * num;
	}

	// Token: 0x06003489 RID: 13449 RVA: 0x0015D414 File Offset: 0x0015B614
	public static float GetDisplayValues(PassiveEffects _passiveEffect, out float baseValueChange, out float percValueMultiplier, ItemValue _originalItemValue = null, float _originalValue = 0f, EntityAlive _entity = null, Recipe _recipe = null, FastTags<TagGroup.Global> tags = default(FastTags<TagGroup.Global>), int craftingTier = 1)
	{
		float num = _originalValue;
		baseValueChange = 0f;
		percValueMultiplier = 1f;
		if (GameManager.Instance == null || GameManager.Instance.gameStateManager == null || !GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return _originalValue;
		}
		if (_entity == null)
		{
			if (_recipe != null)
			{
				_recipe.ModifyValue(_passiveEffect, ref baseValueChange, ref percValueMultiplier, tags, craftingTier);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref percValueMultiplier, tags, true, false);
			}
		}
		else
		{
			if (EntityClass.list.ContainsKey(_entity.entityClass) && EntityClass.list[_entity.entityClass].Effects != null)
			{
				EntityClass.list[_entity.entityClass].Effects.ModifyValue(_entity, _passiveEffect, ref _originalValue, ref percValueMultiplier, 0f, tags, 1);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.ModifyValue(_entity, null, _passiveEffect, ref _originalValue, ref percValueMultiplier, tags, true, false);
			}
			else
			{
				if (_entity.inventory != null && _entity.inventory.holdingItemItemValue != _originalItemValue)
				{
					_entity.inventory.ModifyValue(_originalItemValue, _passiveEffect, ref _originalValue, ref percValueMultiplier, tags);
				}
				if (_entity.equipment != null)
				{
					_entity.equipment.ModifyValue(_originalItemValue, _passiveEffect, ref _originalValue, ref percValueMultiplier, tags, false);
				}
			}
			if (_entity.Progression != null)
			{
				_entity.Progression.ModifyValue(_passiveEffect, ref _originalValue, ref percValueMultiplier, tags);
			}
			if (_recipe != null)
			{
				_recipe.ModifyValue(_passiveEffect, ref baseValueChange, ref percValueMultiplier, tags, craftingTier);
			}
			if (_entity.Buffs != null)
			{
				_entity.Buffs.ModifyValue(_passiveEffect, ref _originalValue, ref percValueMultiplier, tags);
			}
		}
		if (_originalItemValue != null && _originalItemValue.ItemClass != null && _originalItemValue.Quality > 0 && _originalItemValue.ItemClass.Effects != null)
		{
			for (int i = 0; i < _originalItemValue.Modifications.Length; i++)
			{
				if (_originalItemValue.Modifications[i] != null && _originalItemValue.Modifications[i].ItemClass is ItemClassModifier)
				{
					_originalItemValue.ItemClass.Effects.ModifyValue(_entity, PassiveEffects.ModPowerBonus, ref _originalValue, ref percValueMultiplier, (float)_originalItemValue.Quality, FastTags<TagGroup.Global>.Parse(_passiveEffect.ToStringCached<PassiveEffects>()), 1);
				}
			}
		}
		baseValueChange = _originalValue - num;
		return _originalValue * percValueMultiplier;
	}

	// Token: 0x0600348A RID: 13450 RVA: 0x0015D640 File Offset: 0x0015B840
	public static string GetInfoString(PassiveEffects _gAttribute, ItemValue _itemValue, EntityAlive _ea = null, float modAmount = 0f)
	{
		return string.Format("{0}: {1}\n", _gAttribute.ToStringCached<PassiveEffects>(), (modAmount + EffectManager.GetValue(_gAttribute, _itemValue, 0f, _ea, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)).ToCultureInvariantString("0.0"));
	}

	// Token: 0x0600348B RID: 13451 RVA: 0x0015D688 File Offset: 0x0015B888
	public static string GetColoredInfoString(PassiveEffects _passiveEffect, ItemValue _itemValue, EntityAlive _ea = null)
	{
		EffectManager.GetDisplayValues(_passiveEffect, out EffectManager.cInfoStringBaseValue, out EffectManager.cInfoStringPercValue, _itemValue, 0f, _ea, null, default(FastTags<TagGroup.Global>), 1);
		return string.Format("{0}: [REPLACE_COLOR]{1}*{2}[-]\n", _passiveEffect.ToStringCached<PassiveEffects>(), EffectManager.cInfoStringBaseValue.ToCultureInvariantString("0.0"), EffectManager.cInfoStringPercValue.ToCultureInvariantString("0.0"));
	}

	// Token: 0x0600348C RID: 13452 RVA: 0x0015D6E8 File Offset: 0x0015B8E8
	public static List<EffectManager.ModifierValuesAndSources> GetValuesAndSources(PassiveEffects _passiveEffect, ItemValue _originalItemValue = null, float _originalValue = 0f, EntityAlive _entity = null, Recipe _recipe = null, FastTags<TagGroup.Global> tags = default(FastTags<TagGroup.Global>), bool calcEquipment = true, bool calcHoldingItem = true)
	{
		float num = 1f;
		List<EffectManager.ModifierValuesAndSources> list = new List<EffectManager.ModifierValuesAndSources>();
		if (_entity == null)
		{
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Self, _entity, null, _passiveEffect, ref _originalValue, ref num, tags);
			}
		}
		else
		{
			if (GameManager.Instance == null || GameManager.Instance.gameStateManager == null || !GameManager.Instance.gameStateManager.IsGameStarted())
			{
				return list;
			}
			if (EntityClass.list.ContainsKey(_entity.entityClass) && EntityClass.list[_entity.entityClass].Effects != null)
			{
				EntityClass.list[_entity.entityClass].Effects.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Base, _entity, _passiveEffect, ref _originalValue, ref num, 0f, tags, 1);
			}
			if (_originalItemValue != null && _originalItemValue.type != 0 && _originalItemValue.ItemClass != null)
			{
				_originalItemValue.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Self, _entity, null, _passiveEffect, ref _originalValue, ref num, tags);
			}
			else
			{
				if (calcHoldingItem && _entity.inventory != null && _entity.inventory.holdingItemItemValue != _originalItemValue && !_entity.inventory.holdingItemItemValue.IsMod)
				{
					_entity.inventory.holdingItemItemValue.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Held, _entity, _originalItemValue, _passiveEffect, ref _originalValue, ref num, tags);
				}
				if (calcEquipment && _entity.equipment != null)
				{
					_entity.equipment.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Worn, _originalItemValue, _passiveEffect, ref _originalValue, ref num, tags);
				}
			}
			if (_entity.Progression != null)
			{
				_entity.Progression.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Progression, _passiveEffect, ref _originalValue, ref num, tags);
			}
			if (_entity.Buffs != null)
			{
				_entity.Buffs.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.Buff, _passiveEffect, ref _originalValue, ref num, tags);
			}
		}
		if (_originalItemValue != null && _originalItemValue.ItemClass != null && _originalItemValue.Quality > 0 && _originalItemValue.ItemClass.Effects != null)
		{
			for (int i = 0; i < _originalItemValue.Modifications.Length; i++)
			{
				if (_originalItemValue.Modifications[i] != null && _originalItemValue.Modifications[i].ItemClass is ItemClassModifier)
				{
					_originalItemValue.ItemClass.Effects.GetModifiedValueData(list, EffectManager.ModifierValuesAndSources.ValueSourceType.ModBonus, _entity, PassiveEffects.ModPowerBonus, ref _originalValue, ref num, (float)_originalItemValue.Quality, FastTags<TagGroup.Global>.Parse(_passiveEffect.ToStringCached<PassiveEffects>()), 1);
				}
			}
		}
		return list;
	}

	// Token: 0x04002AA7 RID: 10919
	public static FastEnumIntEqualityComparer<PassiveEffects> PassiveEffectsComparer = new FastEnumIntEqualityComparer<PassiveEffects>();

	// Token: 0x04002AA8 RID: 10920
	[PublicizedFrom(EAccessModifier.Private)]
	public static int slotsQueriedFrame;

	// Token: 0x04002AA9 RID: 10921
	[PublicizedFrom(EAccessModifier.Private)]
	public static int slotsQueriedForEntity;

	// Token: 0x04002AAA RID: 10922
	[PublicizedFrom(EAccessModifier.Private)]
	public static ItemStack[] slotsCached;

	// Token: 0x04002AAB RID: 10923
	[PublicizedFrom(EAccessModifier.Private)]
	public static float cInfoStringBaseValue;

	// Token: 0x04002AAC RID: 10924
	[PublicizedFrom(EAccessModifier.Private)]
	public static float cInfoStringPercValue;

	// Token: 0x02000658 RID: 1624
	public class ModifierValuesAndSources
	{
		// Token: 0x04002AAD RID: 10925
		public PassiveEffects PassiveEffectName;

		// Token: 0x04002AAE RID: 10926
		public MinEffectController.SourceParentType ParentType;

		// Token: 0x04002AAF RID: 10927
		public EffectManager.ModifierValuesAndSources.ValueSourceType ValueSource;

		// Token: 0x04002AB0 RID: 10928
		public EffectManager.ModifierValuesAndSources.ValueTypes ValueType;

		// Token: 0x04002AB1 RID: 10929
		public FastTags<TagGroup.Global> Tags;

		// Token: 0x04002AB2 RID: 10930
		public object Source;

		// Token: 0x04002AB3 RID: 10931
		public float Value;

		// Token: 0x04002AB4 RID: 10932
		public PassiveEffect.ValueModifierTypes ModifierType;

		// Token: 0x04002AB5 RID: 10933
		public int ModItemSource;

		// Token: 0x02000659 RID: 1625
		public enum ValueSourceType
		{
			// Token: 0x04002AB7 RID: 10935
			None,
			// Token: 0x04002AB8 RID: 10936
			Self,
			// Token: 0x04002AB9 RID: 10937
			Held,
			// Token: 0x04002ABA RID: 10938
			Worn,
			// Token: 0x04002ABB RID: 10939
			Attribute,
			// Token: 0x04002ABC RID: 10940
			Skill,
			// Token: 0x04002ABD RID: 10941
			Perk,
			// Token: 0x04002ABE RID: 10942
			Mod,
			// Token: 0x04002ABF RID: 10943
			CosmeticMod,
			// Token: 0x04002AC0 RID: 10944
			Fault,
			// Token: 0x04002AC1 RID: 10945
			Buff,
			// Token: 0x04002AC2 RID: 10946
			Progression,
			// Token: 0x04002AC3 RID: 10947
			Base,
			// Token: 0x04002AC4 RID: 10948
			Ammo,
			// Token: 0x04002AC5 RID: 10949
			ModBonus
		}

		// Token: 0x0200065A RID: 1626
		public enum ValueTypes
		{
			// Token: 0x04002AC7 RID: 10951
			None,
			// Token: 0x04002AC8 RID: 10952
			BaseValue,
			// Token: 0x04002AC9 RID: 10953
			PercentValue
		}

		// Token: 0x0200065B RID: 1627
		public enum ModTypes
		{
			// Token: 0x04002ACB RID: 10955
			None,
			// Token: 0x04002ACC RID: 10956
			Base,
			// Token: 0x04002ACD RID: 10957
			Percentage
		}
	}
}
