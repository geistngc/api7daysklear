using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Audio;
using UnityEngine;

// Token: 0x0200051E RID: 1310
public class Equipment
{
	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06002AFC RID: 11004 RVA: 0x00110344 File Offset: 0x0010E544
	// (remove) Token: 0x06002AFD RID: 11005 RVA: 0x0011037C File Offset: 0x0010E57C
	public event Action OnChanged;

	// Token: 0x170004A8 RID: 1192
	// (get) Token: 0x06002AFE RID: 11006 RVA: 0x001103B1 File Offset: 0x0010E5B1
	// (set) Token: 0x06002AFF RID: 11007 RVA: 0x001103B9 File Offset: 0x0010E5B9
	public ItemClass[] CosmeticSlots
	{
		get
		{
			return this.m_cosmeticSlots;
		}
		set
		{
			this.m_cosmeticSlots = value;
		}
	}

	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06002B00 RID: 11008 RVA: 0x001103C4 File Offset: 0x0010E5C4
	// (remove) Token: 0x06002B01 RID: 11009 RVA: 0x001103FC File Offset: 0x0010E5FC
	public event Equipment_CosmeticUnlocked CosmeticUnlocked;

	// Token: 0x06002B02 RID: 11010 RVA: 0x00110434 File Offset: 0x0010E634
	public Equipment()
	{
		int num = 12;
		this.m_slots = new ItemValue[num];
		this.CosmeticSlots = new ItemClass[num];
		this.preferredItemSlots = new int[num];
	}

	// Token: 0x06002B03 RID: 11011 RVA: 0x00110496 File Offset: 0x0010E696
	public Equipment(EntityAlive _entity) : this()
	{
		this.m_entity = _entity;
	}

	// Token: 0x06002B04 RID: 11012 RVA: 0x001104A5 File Offset: 0x0010E6A5
	public ItemClass GetCosmeticSlot(int index, bool useTemporary)
	{
		if (index >= this.CosmeticSlots.Length)
		{
			return null;
		}
		if (useTemporary && this.tempCosmeticSlotIndex == index)
		{
			return this.tempCosmeticSlot;
		}
		return this.CosmeticSlots[index];
	}

	// Token: 0x06002B05 RID: 11013 RVA: 0x001104D0 File Offset: 0x0010E6D0
	public void SetCosmeticSlot(ItemClassArmor itemClass)
	{
		if (itemClass.EquipSlot < EquipmentSlots.BiomeBadge)
		{
			if (!this.HasCosmeticUnlocked(itemClass).Item1)
			{
				return;
			}
			int equipSlot = (int)itemClass.EquipSlot;
			ItemClassArmor itemClassArmor = this.CosmeticSlots[equipSlot] as ItemClassArmor;
			if (itemClassArmor != null && itemClassArmor.ArmorGroup[0] == itemClass.ArmorGroup[0])
			{
				return;
			}
			this.CosmeticSlots[equipSlot] = itemClass;
			if (this.m_entity && !this.m_entity.isEntityRemote)
			{
				this.m_entity.bPlayerEquipmentChanged = true;
			}
		}
	}

	// Token: 0x06002B06 RID: 11014 RVA: 0x00110554 File Offset: 0x0010E754
	public void SetCosmeticSlot(int slotID, int id)
	{
		if (id == 0)
		{
			this.CosmeticSlots[slotID] = null;
			if (this.m_entity && !this.m_entity.isEntityRemote)
			{
				this.m_entity.bPlayerEquipmentChanged = true;
				return;
			}
		}
		else
		{
			ItemClass itemClass = ItemClass.GetItemClass(Equipment.CosmeticMappingIDString[id], false);
			ItemClassArmor itemClassArmor = itemClass as ItemClassArmor;
			if (itemClassArmor != null)
			{
				if (itemClassArmor.EquipSlot < EquipmentSlots.BiomeBadge)
				{
					this.CosmeticSlots[(int)itemClassArmor.EquipSlot] = itemClass;
					if (this.m_entity && !this.m_entity.isEntityRemote)
					{
						this.m_entity.bPlayerEquipmentChanged = true;
						return;
					}
				}
			}
			else
			{
				this.CosmeticSlots[slotID] = itemClass;
				if (this.m_entity && !this.m_entity.isEntityRemote)
				{
					this.m_entity.bPlayerEquipmentChanged = true;
				}
			}
		}
	}

	// Token: 0x06002B07 RID: 11015 RVA: 0x00110624 File Offset: 0x0010E824
	public int[] GetCosmeticIDs()
	{
		int num = 12;
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			if (this.m_cosmeticSlots[i] == null)
			{
				array[i] = 0;
			}
			else
			{
				array[i] = Equipment.CosmeticMappingStringID[this.m_cosmeticSlots[i].GetItemName()];
			}
		}
		return array;
	}

	// Token: 0x06002B08 RID: 11016 RVA: 0x00110674 File Offset: 0x0010E874
	public void ClearCosmeticSlots()
	{
		for (int i = 0; i < this.m_cosmeticSlots.Length; i++)
		{
			this.m_cosmeticSlots[i] = null;
		}
		if (this.m_entity && !this.m_entity.isEntityRemote)
		{
			this.m_entity.bPlayerEquipmentChanged = true;
		}
	}

	// Token: 0x06002B09 RID: 11017 RVA: 0x001106C4 File Offset: 0x0010E8C4
	public void UnlockCosmeticItem(ItemClass itemClass)
	{
		if (itemClass == null)
		{
			return;
		}
		string itemName = itemClass.GetItemName();
		if (Equipment.CosmeticMappingStringID.ContainsKey(itemName))
		{
			int item = Equipment.CosmeticMappingStringID[itemName];
			if (!this.m_unlockedCosmetics.Contains(item))
			{
				this.m_unlockedCosmetics.Add(item);
				if (this.CosmeticUnlocked != null)
				{
					this.CosmeticUnlocked(itemName);
				}
			}
		}
	}

	// Token: 0x06002B0A RID: 11018 RVA: 0x00110724 File Offset: 0x0010E924
	public void ModifyValue(ItemValue _originalItemValue, PassiveEffects _passiveEffect, ref float _base_val, ref float _perc_val, FastTags<TagGroup.Global> tags, bool _useDurability = false)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null && !itemValue.Equals(_originalItemValue) && itemValue.ItemClass != null)
			{
				itemValue.ModifyValue(this.m_entity, _originalItemValue, _passiveEffect, ref _base_val, ref _perc_val, tags, true, _useDurability);
			}
		}
	}

	// Token: 0x06002B0B RID: 11019 RVA: 0x00110778 File Offset: 0x0010E978
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, ItemValue _originalItemValue, PassiveEffects _passiveEffect, ref float _base_val, ref float _perc_val, FastTags<TagGroup.Global> tags)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null && !itemValue.Equals(_originalItemValue) && itemValue.ItemClass != null)
			{
				itemValue.GetModifiedValueData(_modValueSources, _sourceType, this.m_entity, _originalItemValue, _passiveEffect, ref _base_val, ref _perc_val, tags);
			}
		}
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x001107CC File Offset: 0x0010E9CC
	[return: TupleElementNames(new string[]
	{
		"isUnlocked",
		"set"
	})]
	public ValueTuple<bool, EntitlementSetEnum> HasCosmeticUnlocked(ItemClass itemClass)
	{
		if (itemClass == null)
		{
			return new ValueTuple<bool, EntitlementSetEnum>(false, EntitlementSetEnum.None);
		}
		string itemName = itemClass.GetItemName();
		if (!Equipment.CosmeticMappingStringID.ContainsKey(itemName))
		{
			return new ValueTuple<bool, EntitlementSetEnum>(false, EntitlementSetEnum.None);
		}
		EntitlementSetEnum entitlementSetEnum = EntitlementSetEnum.None;
		if (itemClass.SDCSData != null)
		{
			entitlementSetEnum = EntitlementManager.Instance.GetSetForAsset(itemClass.SDCSData.PrefabName);
			if (entitlementSetEnum != EntitlementSetEnum.None && EntitlementManager.Instance.HasEntitlement(entitlementSetEnum))
			{
				return new ValueTuple<bool, EntitlementSetEnum>(true, entitlementSetEnum);
			}
		}
		return new ValueTuple<bool, EntitlementSetEnum>(this.m_unlockedCosmetics.Contains(Equipment.CosmeticMappingStringID[itemClass.GetItemName()]), entitlementSetEnum);
	}

	// Token: 0x06002B0D RID: 11021 RVA: 0x0011085C File Offset: 0x0010EA5C
	public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null && itemValue.ItemClass != null)
			{
				itemValue.FireEvent(_eventType, _params);
			}
		}
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x00110898 File Offset: 0x0010EA98
	public void DropItems()
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				this.DropItemOnGround(itemValue);
				this.SetSlotItem(i, null, true);
			}
		}
		this.updateInsulation();
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x001108DC File Offset: 0x0010EADC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateInsulation()
	{
		this.waterProof = 0f;
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				this.waterProof += itemValue.ItemClass.WaterProof;
			}
		}
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x0011092C File Offset: 0x0010EB2C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DropItemOnGround(ItemValue _itemValue)
	{
		this.m_entity.world.GetGameManager().ItemDropServer(new ItemStack(_itemValue, 1), this.m_entity.GetPosition(), new Vector3(0.5f, 0f, 0.5f), this.m_entity.belongsPlayerId, 60f, false);
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x00110985 File Offset: 0x0010EB85
	public float GetTotalInsulation()
	{
		return this.insulation;
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x0011098D File Offset: 0x0010EB8D
	public float GetTotalWaterproof()
	{
		return this.waterProof;
	}

	// Token: 0x06002B13 RID: 11027 RVA: 0x00110995 File Offset: 0x0010EB95
	public int GetSlotCount()
	{
		return this.m_slots.Length;
	}

	// Token: 0x06002B14 RID: 11028 RVA: 0x0011099F File Offset: 0x0010EB9F
	public ItemValue[] GetItems()
	{
		return this.m_slots;
	}

	// Token: 0x06002B15 RID: 11029 RVA: 0x001109A7 File Offset: 0x0010EBA7
	public ItemValue GetSlotItem(int index)
	{
		if (index >= this.m_slots.Length)
		{
			return null;
		}
		return this.m_slots[index];
	}

	// Token: 0x06002B16 RID: 11030 RVA: 0x001109C0 File Offset: 0x0010EBC0
	public ItemValue GetSlotItemOrNone(int index)
	{
		if (index >= this.m_slots.Length)
		{
			return ItemValue.None;
		}
		ItemValue itemValue = this.m_slots[index];
		if (itemValue == null)
		{
			return ItemValue.None;
		}
		return itemValue;
	}

	// Token: 0x06002B17 RID: 11031 RVA: 0x001109F4 File Offset: 0x0010EBF4
	public bool HasAnyItems()
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			if (this.m_slots[i] != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x00110A24 File Offset: 0x0010EC24
	public void PerformActionOnSlots(Action<ItemValue> _action)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				_action(itemValue);
			}
		}
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x00110A57 File Offset: 0x0010EC57
	public void SetTempCosmeticSlot(int index, ItemClass itemClass)
	{
		this.tempCosmeticSlotIndex = index;
		this.tempCosmeticSlot = itemClass;
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x00110A67 File Offset: 0x0010EC67
	public void ClearTempCosmeticSlot()
	{
		this.tempCosmeticSlotIndex = -1;
		this.tempCosmeticSlot = null;
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x00110A78 File Offset: 0x0010EC78
	public void ApplyTempCosmeticSlot()
	{
		if (this.tempCosmeticSlotIndex != -1)
		{
			this.m_cosmeticSlots[this.tempCosmeticSlotIndex] = this.tempCosmeticSlot;
			if (this.m_entity && !this.m_entity.isEntityRemote)
			{
				this.m_entity.bPlayerEquipmentChanged = true;
			}
		}
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x00110AC8 File Offset: 0x0010ECC8
	public void CalcDamage(ref int entityDamageTaken, ref int armorDamageTaken, FastTags<TagGroup.Global> damageTypeTag, EntityAlive attacker, ItemValue attackingItem)
	{
		armorDamageTaken = entityDamageTaken;
		if (damageTypeTag.Test_AnySet(Equipment.physicalDamageTypes))
		{
			if (entityDamageTaken > 0)
			{
				float num = this.GetTotalPhysicalArmorRating(attacker, attackingItem) / 100f;
				armorDamageTaken = Utils.FastMax((num > 0f) ? 1 : 0, Utils.FastRoundToInt((float)entityDamageTaken * num));
				entityDamageTaken -= armorDamageTaken;
				return;
			}
		}
		else
		{
			entityDamageTaken = Utils.FastRoundToInt(Utils.FastMax(0f, (float)entityDamageTaken * (1f - EffectManager.GetValue(PassiveEffects.ElementalDamageResist, null, 0f, this.m_entity, null, damageTypeTag, true, true, true, true, true, 1, true, false) / 100f)));
			armorDamageTaken = Utils.FastRoundToInt((float)Utils.FastMax(0, armorDamageTaken - entityDamageTaken));
		}
	}

	// Token: 0x06002B1D RID: 11037 RVA: 0x00110B78 File Offset: 0x0010ED78
	public float GetTotalPhysicalArmorRating(EntityAlive attacker, ItemValue attackingItem)
	{
		FastTags<TagGroup.Global> fastTags = Equipment.coreDamageResist;
		if (attackingItem != null)
		{
			fastTags |= attackingItem.ItemClassOrMissing.ItemTags;
		}
		float value = EffectManager.GetValue(PassiveEffects.PhysicalDamageResist, null, 0f, this.m_entity, null, fastTags, true, true, true, true, true, 1, true, true);
		return EffectManager.GetValue(PassiveEffects.TargetArmor, attackingItem, value, attacker, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06002B1E RID: 11038 RVA: 0x00110BE0 File Offset: 0x0010EDE0
	public List<ItemValue> GetArmor()
	{
		List<ItemValue> list = new List<ItemValue>();
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				float num = 0f;
				float num2 = 1f;
				itemValue.ModifyValue(this.m_entity, null, PassiveEffects.PhysicalDamageResist, ref num, ref num2, FastTags<TagGroup.Global>.all, true, false);
				if (num != 0f)
				{
					list.Add(itemValue);
				}
			}
		}
		return list;
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x00110C4C File Offset: 0x0010EE4C
	public bool CheckBreakUseItems()
	{
		bool result = false;
		this.CurrentLowestDurability = 1f;
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				ItemClass forId = ItemClass.GetForId(itemValue.type);
				float percentUsesLeft = itemValue.PercentUsesLeft;
				if (percentUsesLeft < this.CurrentLowestDurability)
				{
					this.CurrentLowestDurability = percentUsesLeft;
				}
				if (forId != null && itemValue.MaxUseTimes > 0 && forId.MaxUseTimesBreaksAfter.Value && itemValue.UseTimes > (float)itemValue.MaxUseTimes)
				{
					this.SetSlotItem(i, null, true);
					if (this.m_entity != null && forId.Properties.Values.ContainsKey(ItemClass.PropSoundDestroy))
					{
						Manager.BroadcastPlay(this.m_entity, forId.Properties.Values[ItemClass.PropSoundDestroy], false, 1f);
					}
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x00110D34 File Offset: 0x0010EF34
	public void SetSlotItem(int index, ItemValue value, bool isLocal = true)
	{
		if (value != null && value.IsEmpty())
		{
			value = null;
		}
		ItemValue itemValue = this.m_slots[index];
		if (value == null && itemValue == null)
		{
			return;
		}
		this.m_entity.IsEquipping = true;
		bool flag = false;
		if (itemValue != null && itemValue.Equals(value))
		{
			this.m_slots[index] = value;
			this.m_entity.MinEventContext.ItemValue = value;
			value.FireEvent(MinEventTypes.onSelfEquipStart, this.m_entity.MinEventContext);
		}
		else
		{
			flag = true;
			if (itemValue != null)
			{
				if (itemValue.ItemClass.HasTrigger(MinEventTypes.onSelfItemActivate) && itemValue.Activated)
				{
					this.m_entity.MinEventContext.ItemValue = itemValue;
					itemValue.FireEvent(MinEventTypes.onSelfItemDeactivate, this.m_entity.MinEventContext);
					itemValue.Activated = false;
				}
				for (int i = 0; i < itemValue.Modifications.Length; i++)
				{
					ItemValue itemValue2 = itemValue.Modifications[i];
					if (itemValue2 != null)
					{
						ItemClass itemClass = itemValue2.ItemClass;
						if (itemClass != null && itemClass.HasTrigger(MinEventTypes.onSelfItemActivate) && itemValue2.Activated)
						{
							this.m_entity.MinEventContext.ItemValue = itemValue2;
							itemValue2.FireEvent(MinEventTypes.onSelfItemDeactivate, this.m_entity.MinEventContext);
							itemValue2.Activated = false;
						}
					}
				}
				this.m_entity.MinEventContext.ItemValue = itemValue;
				itemValue.FireEvent(MinEventTypes.onSelfEquipStop, this.m_entity.MinEventContext);
			}
			this.preferredItemSlots[index] = ((value == null) ? 0 : value.type);
			this.m_slots[index] = value;
			this.slotsSetFlags |= 1 << index;
			this.slotsChangedFlags |= 1 << index;
		}
		if (flag)
		{
			if (this.m_entity && !this.m_entity.isEntityRemote)
			{
				this.m_entity.bPlayerEquipmentChanged = true;
			}
			this.ResetArmorGroups();
			if (this.OnChanged != null)
			{
				this.OnChanged();
			}
		}
		this.m_entity.IsEquipping = false;
	}

	// Token: 0x06002B21 RID: 11041 RVA: 0x00110F13 File Offset: 0x0010F113
	public void SetSlotItemRaw(int index, ItemValue _iv)
	{
		if (_iv != null && _iv.IsEmpty())
		{
			_iv = null;
		}
		this.m_slots[index] = _iv;
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x00110F2C File Offset: 0x0010F12C
	public void FireEventsForSlots(MinEventTypes _event, int _flags = -1)
	{
		this.m_entity.MinEventContext.Self = this.m_entity;
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			if ((_flags & 1 << i) > 0)
			{
				ItemValue itemValue = this.m_slots[i];
				if (itemValue != null && itemValue.ItemClass != null)
				{
					this.m_entity.MinEventContext.ItemValue = itemValue;
					itemValue.FireEvent(_event, this.m_entity.MinEventContext);
				}
			}
		}
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x00110FA4 File Offset: 0x0010F1A4
	public void FireEventsForSetSlots()
	{
		this.FireEventsForSlots(MinEventTypes.onSelfEquipStart, this.slotsSetFlags);
		this.slotsSetFlags = 0;
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x00110FBC File Offset: 0x0010F1BC
	public void FireEventsForChangedSlots()
	{
		if (this.slotsChangedFlags == 0)
		{
			return;
		}
		this.m_entity.IsEquipping = true;
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			if ((this.slotsChangedFlags & 1 << i) > 0)
			{
				ItemValue itemValue = this.m_slots[i];
				if (itemValue != null)
				{
					ItemClass itemClass = itemValue.ItemClass;
					if (itemClass != null)
					{
						this.m_entity.MinEventContext.Self = this.m_entity;
						this.m_entity.MinEventContext.ItemValue = itemValue;
						itemValue.FireEvent(MinEventTypes.onSelfEquipChanged, this.m_entity.MinEventContext);
						if (itemClass.HasTrigger(MinEventTypes.onSelfItemActivate))
						{
							if (!itemValue.Activated)
							{
								itemValue.FireEvent(MinEventTypes.onSelfItemDeactivate, this.m_entity.MinEventContext);
							}
							else
							{
								itemValue.FireEvent(MinEventTypes.onSelfItemActivate, this.m_entity.MinEventContext);
							}
						}
						for (int j = 0; j < itemValue.Modifications.Length; j++)
						{
							ItemValue itemValue2 = itemValue.Modifications[j];
							if (itemValue2 != null && itemValue2.ItemClass != null && itemValue2.ItemClass.HasTrigger(MinEventTypes.onSelfItemActivate))
							{
								this.m_entity.MinEventContext.ItemValue = itemValue2;
								if (!itemValue2.Activated)
								{
									itemValue2.FireEvent(MinEventTypes.onSelfItemDeactivate, this.m_entity.MinEventContext);
								}
								else
								{
									itemValue2.FireEvent(MinEventTypes.onSelfItemActivate, this.m_entity.MinEventContext);
								}
							}
						}
					}
				}
			}
		}
		this.slotsChangedFlags = 0;
		this.m_entity.IsEquipping = false;
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x0011112C File Offset: 0x0010F32C
	public void Update()
	{
		if (Time.time - this.lastUpdateTime >= 1f)
		{
			for (int i = 0; i < this.m_slots.Length; i++)
			{
				ItemValue itemValue = this.m_slots[i];
				if (itemValue != null)
				{
					this.m_entity.MinEventContext.ItemValue = itemValue;
					itemValue.FireEvent(MinEventTypes.onSelfEquipUpdate, this.m_entity.MinEventContext);
				}
			}
			this.lastUpdateTime = Time.time;
		}
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x0011119A File Offset: 0x0010F39A
	public void SetPreferredItemSlot(int _slot, ItemValue _itemValue)
	{
		if (_slot >= 0 && _slot < this.preferredItemSlots.Length)
		{
			this.preferredItemSlots[_slot] = _itemValue.type;
		}
	}

	// Token: 0x06002B27 RID: 11047 RVA: 0x001111BC File Offset: 0x0010F3BC
	public int PreferredItemSlot(ItemValue _itemValue)
	{
		for (int i = 0; i < this.preferredItemSlots.Length; i++)
		{
			if (_itemValue.type == this.preferredItemSlots[i])
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002B28 RID: 11048 RVA: 0x001111F0 File Offset: 0x0010F3F0
	public int PreferredItemSlot(ItemStack _itemStack)
	{
		for (int i = 0; i < this.preferredItemSlots.Length; i++)
		{
			if (_itemStack.itemValue.type == this.preferredItemSlots[i])
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002B29 RID: 11049 RVA: 0x00111228 File Offset: 0x0010F428
	public void Write(BinaryWriter writer)
	{
		writer.Write(4);
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue.Write(this.m_slots[i], writer);
		}
		for (int j = 0; j < this.m_cosmeticSlots.Length; j++)
		{
			if (this.m_cosmeticSlots[j] == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(Equipment.CosmeticMappingStringID[this.m_cosmeticSlots[j].GetItemName()]);
			}
		}
		writer.Write(this.m_unlockedCosmetics.Count);
		for (int k = 0; k < this.m_unlockedCosmetics.Count; k++)
		{
			writer.Write(this.m_unlockedCosmetics[k]);
		}
	}

	// Token: 0x06002B2A RID: 11050 RVA: 0x001112DC File Offset: 0x0010F4DC
	public static Equipment Read(BinaryReader reader)
	{
		int num = (int)reader.ReadByte();
		Equipment equipment = new Equipment();
		int num2 = equipment.m_slots.Length;
		if (num <= 2)
		{
			num2 = 5;
		}
		else if (num <= 3)
		{
			num2 = 8;
		}
		for (int i = 0; i < num2; i++)
		{
			equipment.m_slots[i] = ItemValue.ReadOrNull(reader);
		}
		if (num >= 2)
		{
			for (int j = 0; j < num2; j++)
			{
				int num3 = reader.ReadInt32();
				if (num3 == 0)
				{
					equipment.m_cosmeticSlots[j] = null;
				}
				else
				{
					equipment.m_cosmeticSlots[j] = ItemClass.GetItemClass(Equipment.CosmeticMappingIDString[num3], false);
				}
			}
			equipment.m_unlockedCosmetics.Clear();
			int num4 = reader.ReadInt32();
			for (int k = 0; k < num4; k++)
			{
				equipment.m_unlockedCosmetics.Add(reader.ReadInt32());
			}
		}
		return equipment;
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x001113A4 File Offset: 0x0010F5A4
	public virtual bool ReturnItem(ItemStack _itemStack, bool isLocal = true)
	{
		int num = this.PreferredItemSlot(_itemStack);
		if (num < 0 || num >= this.m_slots.Length)
		{
			return false;
		}
		if (this.m_slots[num] == null)
		{
			this.SetSlotItem(num, _itemStack.itemValue, isLocal);
			return true;
		}
		return false;
	}

	// Token: 0x06002B2C RID: 11052 RVA: 0x001113E8 File Offset: 0x0010F5E8
	public void Apply(Equipment eq, bool isLocal = true)
	{
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			this.SetSlotItem(i, eq.m_slots[i], isLocal);
		}
		for (int j = 0; j < this.m_cosmeticSlots.Length; j++)
		{
			this.m_cosmeticSlots[j] = eq.m_cosmeticSlots[j];
		}
		this.m_unlockedCosmetics.Clear();
		for (int k = 0; k < eq.m_unlockedCosmetics.Count; k++)
		{
			this.m_unlockedCosmetics.Add(eq.m_unlockedCosmetics[k]);
		}
		this.FireEventsForSetSlots();
		EModelSDCS emodelSDCS = this.m_entity.emodel as EModelSDCS;
		if (emodelSDCS != null)
		{
			emodelSDCS.UpdateEquipment();
		}
		this.FireEventsForChangedSlots();
	}

	// Token: 0x06002B2D RID: 11053 RVA: 0x00111499 File Offset: 0x0010F699
	public void InitializeEquipmentTransforms()
	{
		this.FireEventsForSlots(MinEventTypes.onSelfEquipStop, -1);
		this.FireEventsForSlots(MinEventTypes.onSelfEquipStart, -1);
		this.slotsSetFlags = 0;
		this.slotsChangedFlags = -1;
		this.FireEventsForChangedSlots();
	}

	// Token: 0x06002B2E RID: 11054 RVA: 0x001114C4 File Offset: 0x0010F6C4
	public Equipment Clone()
	{
		Equipment equipment = new Equipment();
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			if (this.m_slots[i] != null)
			{
				equipment.m_slots[i] = this.m_slots[i].Clone();
			}
		}
		for (int j = 0; j < this.m_cosmeticSlots.Length; j++)
		{
			ItemClass itemClass = this.m_cosmeticSlots[j];
			equipment.m_cosmeticSlots[j] = this.m_cosmeticSlots[j];
		}
		for (int k = 0; k < this.m_unlockedCosmetics.Count; k++)
		{
			if (equipment.m_unlockedCosmetics == null)
			{
				equipment.m_unlockedCosmetics = new List<int>();
			}
			equipment.m_unlockedCosmetics.Add(this.m_unlockedCosmetics[k]);
		}
		return equipment;
	}

	// Token: 0x06002B2F RID: 11055 RVA: 0x00111578 File Offset: 0x0010F778
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddArmorGroup(string armorGroup, int quality)
	{
		if (this.ArmorGroupEquipped.ContainsKey(armorGroup))
		{
			Equipment.ArmorGroupInfo armorGroupInfo = this.ArmorGroupEquipped[armorGroup];
			armorGroupInfo.Count++;
			if (armorGroupInfo.LowestQuality > quality)
			{
				armorGroupInfo.LowestQuality = quality;
				return;
			}
		}
		else
		{
			this.ArmorGroupEquipped[armorGroup] = new Equipment.ArmorGroupInfo
			{
				Count = 1,
				LowestQuality = quality
			};
		}
	}

	// Token: 0x06002B30 RID: 11056 RVA: 0x001115E0 File Offset: 0x0010F7E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetArmorGroups()
	{
		this.ArmorGroupEquipped.Clear();
		for (int i = 0; i < this.m_slots.Length; i++)
		{
			ItemValue itemValue = this.m_slots[i];
			if (itemValue != null)
			{
				ItemClassArmor itemClassArmor = itemValue.ItemClass as ItemClassArmor;
				if (itemClassArmor != null)
				{
					for (int j = 0; j < itemClassArmor.ArmorGroup.Length; j++)
					{
						this.AddArmorGroup(itemClassArmor.ArmorGroup[j], (int)itemValue.Quality);
					}
				}
			}
		}
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x0011164D File Offset: 0x0010F84D
	public int GetArmorGroupCount(string armorGroup)
	{
		if (this.ArmorGroupEquipped.ContainsKey(armorGroup))
		{
			return this.ArmorGroupEquipped[armorGroup].Count;
		}
		return 0;
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x00111670 File Offset: 0x0010F870
	public int GetArmorGroupLowestQuality(string armorGroup)
	{
		if (this.ArmorGroupEquipped.ContainsKey(armorGroup))
		{
			return this.ArmorGroupEquipped[armorGroup].LowestQuality;
		}
		return 0;
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x00111693 File Offset: 0x0010F893
	public static void AddCosmeticMapping(int ID, string name)
	{
		Equipment.CosmeticMappingIDString.Add(ID, name);
		Equipment.CosmeticMappingStringID.Add(name, ID);
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x001116B0 File Offset: 0x0010F8B0
	public static void SetupCosmeticMapping()
	{
		Equipment.CosmeticMappingIDString.Clear();
		Equipment.CosmeticMappingStringID.Clear();
		Equipment.AddCosmeticMapping(-1, "missingItem");
		Equipment.AddCosmeticMapping(1, "armorPrimitiveHelmet");
		Equipment.AddCosmeticMapping(2, "armorPrimitiveOutfit");
		Equipment.AddCosmeticMapping(3, "armorPrimitiveGloves");
		Equipment.AddCosmeticMapping(4, "armorPrimitiveBoots");
		Equipment.AddCosmeticMapping(5, "armorLumberjackHelmet");
		Equipment.AddCosmeticMapping(6, "armorLumberjackOutfit");
		Equipment.AddCosmeticMapping(7, "armorLumberjackGloves");
		Equipment.AddCosmeticMapping(8, "armorLumberjackBoots");
		Equipment.AddCosmeticMapping(9, "armorPreacherHelmet");
		Equipment.AddCosmeticMapping(10, "armorPreacherOutfit");
		Equipment.AddCosmeticMapping(11, "armorPreacherGloves");
		Equipment.AddCosmeticMapping(12, "armorPreacherBoots");
		Equipment.AddCosmeticMapping(13, "armorRogueHelmet");
		Equipment.AddCosmeticMapping(14, "armorRogueOutfit");
		Equipment.AddCosmeticMapping(15, "armorRogueGloves");
		Equipment.AddCosmeticMapping(16, "armorRogueBoots");
		Equipment.AddCosmeticMapping(17, "armorAthleticHelmet");
		Equipment.AddCosmeticMapping(18, "armorAthleticOutfit");
		Equipment.AddCosmeticMapping(19, "armorAthleticGloves");
		Equipment.AddCosmeticMapping(20, "armorAthleticBoots");
		Equipment.AddCosmeticMapping(21, "armorEnforcerHelmet");
		Equipment.AddCosmeticMapping(22, "armorEnforcerOutfit");
		Equipment.AddCosmeticMapping(23, "armorEnforcerGloves");
		Equipment.AddCosmeticMapping(24, "armorEnforcerBoots");
		Equipment.AddCosmeticMapping(25, "armorFarmerHelmet");
		Equipment.AddCosmeticMapping(26, "armorFarmerOutfit");
		Equipment.AddCosmeticMapping(27, "armorFarmerGloves");
		Equipment.AddCosmeticMapping(28, "armorFarmerBoots");
		Equipment.AddCosmeticMapping(29, "armorBikerHelmet");
		Equipment.AddCosmeticMapping(30, "armorBikerOutfit");
		Equipment.AddCosmeticMapping(31, "armorBikerGloves");
		Equipment.AddCosmeticMapping(32, "armorBikerBoots");
		Equipment.AddCosmeticMapping(33, "armorScavengerHelmet");
		Equipment.AddCosmeticMapping(34, "armorScavengerOutfit");
		Equipment.AddCosmeticMapping(35, "armorScavengerGloves");
		Equipment.AddCosmeticMapping(36, "armorScavengerBoots");
		Equipment.AddCosmeticMapping(37, "armorRangerHelmet");
		Equipment.AddCosmeticMapping(38, "armorRangerOutfit");
		Equipment.AddCosmeticMapping(39, "armorRangerGloves");
		Equipment.AddCosmeticMapping(40, "armorRangerBoots");
		Equipment.AddCosmeticMapping(41, "armorCommandoHelmet");
		Equipment.AddCosmeticMapping(42, "armorCommandoOutfit");
		Equipment.AddCosmeticMapping(43, "armorCommandoGloves");
		Equipment.AddCosmeticMapping(44, "armorCommandoBoots");
		Equipment.AddCosmeticMapping(45, "armorAssassinHelmet");
		Equipment.AddCosmeticMapping(46, "armorAssassinOutfit");
		Equipment.AddCosmeticMapping(47, "armorAssassinGloves");
		Equipment.AddCosmeticMapping(48, "armorAssassinBoots");
		Equipment.AddCosmeticMapping(49, "armorMinerHelmet");
		Equipment.AddCosmeticMapping(50, "armorMinerOutfit");
		Equipment.AddCosmeticMapping(51, "armorMinerGloves");
		Equipment.AddCosmeticMapping(52, "armorMinerBoots");
		Equipment.AddCosmeticMapping(53, "armorNomadHelmet");
		Equipment.AddCosmeticMapping(54, "armorNomadOutfit");
		Equipment.AddCosmeticMapping(55, "armorNomadGloves");
		Equipment.AddCosmeticMapping(56, "armorNomadBoots");
		Equipment.AddCosmeticMapping(57, "armorNerdHelmet");
		Equipment.AddCosmeticMapping(58, "armorNerdOutfit");
		Equipment.AddCosmeticMapping(59, "armorNerdGloves");
		Equipment.AddCosmeticMapping(60, "armorNerdBoots");
		Equipment.AddCosmeticMapping(61, "armorRaiderHelmet");
		Equipment.AddCosmeticMapping(62, "armorRaiderOutfit");
		Equipment.AddCosmeticMapping(63, "armorRaiderGloves");
		Equipment.AddCosmeticMapping(64, "armorRaiderBoots");
		Equipment.AddCosmeticMapping(65, "armorDesertHelmet");
		Equipment.AddCosmeticMapping(66, "armorDesertOutfit");
		Equipment.AddCosmeticMapping(67, "armorDesertGloves");
		Equipment.AddCosmeticMapping(68, "armorDesertBoots");
		Equipment.AddCosmeticMapping(69, "armorHoarderHelmet");
		Equipment.AddCosmeticMapping(70, "armorHoarderOutfit");
		Equipment.AddCosmeticMapping(71, "armorHoarderGloves");
		Equipment.AddCosmeticMapping(72, "armorHoarderBoots");
		Equipment.AddCosmeticMapping(73, "armorMarauderHelmet");
		Equipment.AddCosmeticMapping(74, "armorMarauderOutfit");
		Equipment.AddCosmeticMapping(75, "armorMarauderGloves");
		Equipment.AddCosmeticMapping(76, "armorMarauderBoots");
		Equipment.AddCosmeticMapping(77, "armorCrimsonWarlordHelmet");
		Equipment.AddCosmeticMapping(78, "armorCrimsonWarlordOutfit");
		Equipment.AddCosmeticMapping(79, "armorCrimsonWarlordGloves");
		Equipment.AddCosmeticMapping(80, "armorCrimsonWarlordBoots");
		Equipment.AddCosmeticMapping(81, "armorSamuraiHelmet");
		Equipment.AddCosmeticMapping(82, "armorSamuraiOutfit");
		Equipment.AddCosmeticMapping(83, "armorSamuraiGloves");
		Equipment.AddCosmeticMapping(84, "armorSamuraiBoots");
		Equipment.AddCosmeticMapping(85, "armorPimpHatBlueHelmet");
		Equipment.AddCosmeticMapping(86, "armorPimpHatPurpleHelmet");
		Equipment.AddCosmeticMapping(87, "armorWatcherHelmet");
		Equipment.AddCosmeticMapping(88, "armorWatcherOutfit");
		Equipment.AddCosmeticMapping(89, "armorWatcherGloves");
		Equipment.AddCosmeticMapping(90, "armorWatcherBoots");
		Equipment.AddCosmeticMapping(91, "armorCrackABookHelmet");
		Equipment.AddCosmeticMapping(92, "armorCrackABookOutfit");
		Equipment.AddCosmeticMapping(93, "armorMoPowerHelmet");
		Equipment.AddCosmeticMapping(94, "armorMoPowerOutfit");
		Equipment.AddCosmeticMapping(95, "armorPassNGasHelmet");
		Equipment.AddCosmeticMapping(96, "armorPassNGasOutfit");
		Equipment.AddCosmeticMapping(97, "armorPopNPillsHelmet");
		Equipment.AddCosmeticMapping(98, "armorPopNPillsOutfit");
		Equipment.AddCosmeticMapping(99, "armorSavageCountryHelmet");
		Equipment.AddCosmeticMapping(100, "armorSavageCountryOutfit");
		Equipment.AddCosmeticMapping(101, "armorShamwayHelmet");
		Equipment.AddCosmeticMapping(102, "armorShamwayOutfit");
		Equipment.AddCosmeticMapping(103, "armorShotgunMessiahHelmet");
		Equipment.AddCosmeticMapping(104, "armorShotgunMessiahOutfit");
		Equipment.AddCosmeticMapping(105, "armorWorkingStiffsHelmet");
		Equipment.AddCosmeticMapping(106, "armorWorkingStiffsOutfit");
		Equipment.AddCosmeticMapping(107, "armorPirateHelmet");
		Equipment.AddCosmeticMapping(108, "armorPirateOutfit");
		Equipment.AddCosmeticMapping(109, "armorPirateGloves");
		Equipment.AddCosmeticMapping(110, "armorPirateBoots");
		Equipment.AddCosmeticMapping(111, "armorHellreaverHelmet");
		Equipment.AddCosmeticMapping(112, "armorHellreaverOutfit");
		Equipment.AddCosmeticMapping(113, "armorHellreaverGloves");
		Equipment.AddCosmeticMapping(114, "armorHellreaverBoots");
		Equipment.AddCosmeticMapping(115, "armorMonsterMaskHelmet");
		Equipment.AddCosmeticMapping(116, "armorVampireMaskHelmet");
		Equipment.AddCosmeticMapping(117, "armorWerewolfMaskHelmet");
		Equipment.AddCosmeticMapping(118, "armorZombieMaskHelmet");
		Equipment.AddCosmeticMapping(119, "armorInspectorMaskHelmet");
		Equipment.AddCosmeticMapping(120, "armorNobleThiefMaskHelmet");
		Equipment.AddCosmeticMapping(121, "armorPennyPincherMaskHelmet");
		Equipment.AddCosmeticMapping(122, "armorTheHatterMaskHelmet");
		Equipment.AddCosmeticMapping(123, "armorClassicSurvivorHelmet");
		Equipment.AddCosmeticMapping(124, "armorClassicSurvivorOutfit");
		Equipment.AddCosmeticMapping(125, "armorClassicSurvivorBoots");
		Equipment.AddCosmeticMapping(126, "armorSantaHatHelmet");
		Equipment.AddCosmeticMapping(127, "armorButcherHelmet");
		Equipment.AddCosmeticMapping(128, "armorButcherOutfit");
		Equipment.AddCosmeticMapping(129, "armorButcherGloves");
		Equipment.AddCosmeticMapping(130, "armorButcherBoots");
		Equipment.AddCosmeticMapping(131, "armorElfHatHelmet");
		Equipment.AddCosmeticMapping(132, "armorReindeerHatHelmet");
		Equipment.AddCosmeticMapping(133, "armorSnowmanHatHelmet");
		Equipment.AddCosmeticMapping(134, "armorTreeHatHelmet");
		Equipment.AddCosmeticMapping(135, "armorWinterHelmet");
		Equipment.AddCosmeticMapping(136, "armorWinterOutfit");
		Equipment.AddCosmeticMapping(137, "armorWinterGloves");
		Equipment.AddCosmeticMapping(138, "armorWinterBoots");
		Equipment.AddCosmeticMapping(139, "armorWorkingStiffHelmet");
		Equipment.AddCosmeticMapping(140, "armorWorkingStiffOutfit");
		Equipment.AddCosmeticMapping(141, "armorWorkingStiffGloves");
		Equipment.AddCosmeticMapping(142, "armorWorkingStiffBoots");
		Equipment.AddCosmeticMapping(143, "armorCasualOutfit");
		Equipment.AddCosmeticMapping(144, "armorCasualGloves");
		Equipment.AddCosmeticMapping(145, "armorCasualBoots");
		Equipment.AddCosmeticMapping(146, "armorBeachHelmet");
		Equipment.AddCosmeticMapping(147, "armorBeachOutfit");
		Equipment.AddCosmeticMapping(148, "armorBeachGloves");
		Equipment.AddCosmeticMapping(149, "armorBeachBoots");
		Equipment.AddCosmeticMapping(150, "armorBeachHelmet02");
		Equipment.AddCosmeticMapping(151, "armorBigbeakHelmet");
		Equipment.AddCosmeticMapping(152, "armorBigbeakOutfit");
		Equipment.AddCosmeticMapping(153, "armorBigbeakGloves");
		Equipment.AddCosmeticMapping(154, "armorBigbeakBoots");
	}

	// Token: 0x040020F5 RID: 8437
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CurrentSaveVersion = 4;

	// Token: 0x040020F7 RID: 8439
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue[] m_slots;

	// Token: 0x040020F8 RID: 8440
	[PublicizedFrom(EAccessModifier.Private)]
	public int slotsSetFlags;

	// Token: 0x040020F9 RID: 8441
	[PublicizedFrom(EAccessModifier.Private)]
	public int slotsChangedFlags;

	// Token: 0x040020FA RID: 8442
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] preferredItemSlots;

	// Token: 0x040020FB RID: 8443
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass[] m_cosmeticSlots;

	// Token: 0x040020FC RID: 8444
	public List<int> m_unlockedCosmetics = new List<int>();

	// Token: 0x040020FD RID: 8445
	public int tempCosmeticSlotIndex = -1;

	// Token: 0x040020FE RID: 8446
	public ItemClass tempCosmeticSlot;

	// Token: 0x040020FF RID: 8447
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive m_entity;

	// Token: 0x04002100 RID: 8448
	[PublicizedFrom(EAccessModifier.Private)]
	public float insulation;

	// Token: 0x04002101 RID: 8449
	[PublicizedFrom(EAccessModifier.Private)]
	public float waterProof;

	// Token: 0x04002102 RID: 8450
	public float CurrentLowestDurability = 1f;

	// Token: 0x04002103 RID: 8451
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, Equipment.ArmorGroupInfo> ArmorGroupEquipped = new Dictionary<string, Equipment.ArmorGroupInfo>();

	// Token: 0x04002104 RID: 8452
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastUpdateTime;

	// Token: 0x04002106 RID: 8454
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> physicalDamageTypes = FastTags<TagGroup.Global>.Parse("piercing,bashing,slashing,crushing,none,corrosive");

	// Token: 0x04002107 RID: 8455
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> coreDamageResist = FastTags<TagGroup.Global>.Parse("coredamageresist");

	// Token: 0x04002108 RID: 8456
	public static Dictionary<int, string> CosmeticMappingIDString = new Dictionary<int, string>();

	// Token: 0x04002109 RID: 8457
	public static Dictionary<string, int> CosmeticMappingStringID = new Dictionary<string, int>();

	// Token: 0x0200051F RID: 1311
	[PublicizedFrom(EAccessModifier.Private)]
	public class ArmorGroupInfo
	{
		// Token: 0x0400210A RID: 8458
		public int Count;

		// Token: 0x0400210B RID: 8459
		public int LowestQuality;
	}
}
