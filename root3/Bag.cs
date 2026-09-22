using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x0200056A RID: 1386
public class Bag : InventoryBase
{
	// Token: 0x14000036 RID: 54
	// (add) Token: 0x06002CF9 RID: 11513 RVA: 0x0011FD4C File Offset: 0x0011DF4C
	// (remove) Token: 0x06002CFA RID: 11514 RVA: 0x0011FD84 File Offset: 0x0011DF84
	public event XUiEvent_BackpackItemsChangedInternal OnBackpackItemsChangedInternal;

	// Token: 0x06002CFB RID: 11515 RVA: 0x0011FDB9 File Offset: 0x0011DFB9
	[PublicizedFrom(EAccessModifier.Private)]
	public Bag()
	{
	}

	// Token: 0x06002CFC RID: 11516 RVA: 0x0011FDC1 File Offset: 0x0011DFC1
	public Bag(int _size)
	{
		this.items = ItemStack.CreateArray(_size);
	}

	// Token: 0x170004CB RID: 1227
	// (get) Token: 0x06002CFD RID: 11517 RVA: 0x0011FDD5 File Offset: 0x0011DFD5
	public int SlotCount
	{
		get
		{
			if (this.items != null)
			{
				return this.items.Length;
			}
			return 0;
		}
	}

	// Token: 0x06002CFE RID: 11518 RVA: 0x0011FDEC File Offset: 0x0011DFEC
	public void Write(BinaryWriter bw)
	{
		bw.Write(1);
		ItemStack[] slots = this.GetSlots();
		bw.Write((ushort)slots.Length);
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i].Write(bw);
		}
		bool flag = this.LockedSlots != null;
		bw.Write(flag);
		if (flag)
		{
			this.LockedSlots.Write(bw);
		}
		bw.Write(this.Touched);
		bool flag2 = this.preferences != null;
		bw.Write(flag2);
		if (!flag2)
		{
			return;
		}
		PooledBinaryWriter pooledBinaryWriter = bw as PooledBinaryWriter;
		if (pooledBinaryWriter != null)
		{
			this.preferences.Write(pooledBinaryWriter);
			return;
		}
		throw new InvalidOperationException("[Bag] Writing preferences requires PooledBinaryWriter.");
	}

	// Token: 0x06002CFF RID: 11519 RVA: 0x0011FE8D File Offset: 0x0011E08D
	public static Bag Read(BinaryReader br)
	{
		return new Bag().ReadInto(br);
	}

	// Token: 0x06002D00 RID: 11520 RVA: 0x0011FE9C File Offset: 0x0011E09C
	public Bag ReadInto(BinaryReader br)
	{
		byte b = br.ReadByte();
		int num = (int)br.ReadUInt16();
		if (this.items == null || this.SlotCount != num)
		{
			this.items = ItemStack.CreateArray(num);
		}
		for (int i = 0; i < num; i++)
		{
			this.items[i].Read(br);
		}
		if (br.ReadBoolean())
		{
			this.LockedSlots = (this.LockedSlots ?? new PackedBoolArray(0));
			this.LockedSlots.Read(br);
		}
		else
		{
			this.LockedSlots = null;
		}
		if (b >= 1)
		{
			this.Touched = br.ReadBoolean();
			if (br.ReadBoolean())
			{
				if (this.preferences == null)
				{
					this.preferences = new PreferenceTracker(-1);
				}
				PooledBinaryReader pooledBinaryReader = br as PooledBinaryReader;
				if (pooledBinaryReader == null)
				{
					throw new InvalidOperationException("[Bag] Reading preferences requires PooledBinaryReader.");
				}
				this.preferences.Read(pooledBinaryReader);
			}
			else
			{
				this.preferences = null;
			}
		}
		else
		{
			this.preferences = null;
		}
		return this;
	}

	// Token: 0x06002D01 RID: 11521 RVA: 0x0011FF84 File Offset: 0x0011E184
	public Bag Clone()
	{
		Bag bag = new Bag();
		bag.items = ItemStack.Clone(this.items);
		PackedBoolArray lockedSlots = this.LockedSlots;
		bag.LockedSlots = ((lockedSlots != null) ? lockedSlots.Clone() : null);
		bag.Touched = this.Touched;
		PreferenceTracker preferenceTracker = this.preferences;
		bag.preferences = ((preferenceTracker != null) ? preferenceTracker.Clone() : null);
		return bag;
	}

	// Token: 0x06002D02 RID: 11522 RVA: 0x0011FFE4 File Offset: 0x0011E1E4
	public void Clear()
	{
		ItemStack[] array = this.items;
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
		}
		this.onBackpackChanged();
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x00120017 File Offset: 0x0011E217
	[PublicizedFrom(EAccessModifier.Private)]
	public void onBackpackChanged()
	{
		XUiEvent_BackpackItemsChangedInternal onBackpackItemsChangedInternal = this.OnBackpackItemsChangedInternal;
		if (onBackpackItemsChangedInternal == null)
		{
			return;
		}
		onBackpackItemsChangedInternal();
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x0012002C File Offset: 0x0011E22C
	public bool CanStackNoEmpty(ItemStack _itemStack)
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			int num;
			if (slots[i].CanStackPartlyWith(_itemStack, out num))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x00120060 File Offset: 0x0011E260
	public bool IsEmpty()
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (!slots[i].IsEmpty())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x00120090 File Offset: 0x0011E290
	public bool CanStack(ItemStack _itemStack)
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].IsEmpty() || slots[i].CanStackWith(_itemStack, false))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x001200CC File Offset: 0x0011E2CC
	[return: TupleElementNames(new string[]
	{
		"anyMoved",
		"allMoved"
	})]
	public override ValueTuple<bool, bool> TryStackItem(int startIndex, ItemStack _itemStack)
	{
		if (!_itemStack.CanMoveTo(XUiC_ItemStack.StackLocationTypes.Backpack, -1))
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		ItemStack[] slots = this.GetSlots();
		int num = 0;
		bool item = false;
		for (int i = startIndex; i < slots.Length; i++)
		{
			num = _itemStack.count;
			if (_itemStack.itemValue.type == slots[i].itemValue.type && slots[i].CanStackPartly(ref num))
			{
				slots[i].count += num;
				_itemStack.count -= num;
				this.onBackpackChanged();
				item = true;
				if (_itemStack.count == 0)
				{
					return new ValueTuple<bool, bool>(true, true);
				}
			}
		}
		return new ValueTuple<bool, bool>(item, false);
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x00120170 File Offset: 0x0011E370
	public bool CanTakeItem(ItemStack _itemStack)
	{
		ItemStack[] slots = this.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			int num;
			if (slots[i].CanStackPartlyWith(_itemStack, out num))
			{
				return true;
			}
			if (slots[i].IsEmpty())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x001201AE File Offset: 0x0011E3AE
	public ItemStack[] GetSlots()
	{
		return this.items;
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x001201B6 File Offset: 0x0011E3B6
	public void SetSlots(ItemStack[] _slots)
	{
		this.items = _slots;
		this.onBackpackChanged();
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x001201C5 File Offset: 0x0011E3C5
	public void SetSlot(int index, ItemStack _stack, bool callChangedEvent = true)
	{
		if (index >= this.items.Length)
		{
			return;
		}
		this.items[index] = _stack;
		if (callChangedEvent)
		{
			this.onBackpackChanged();
		}
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x001201E8 File Offset: 0x0011E3E8
	public override void PerformActionOnSlots(Action<ItemStack> _action)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			_action(this.items[i]);
		}
	}

	// Token: 0x06002D0D RID: 11533 RVA: 0x00120216 File Offset: 0x0011E416
	public override bool AddItem(ItemStack _itemStack)
	{
		if (!_itemStack.CanMoveTo(XUiC_ItemStack.StackLocationTypes.Backpack, -1))
		{
			return false;
		}
		bool flag = ItemStack.AddToItemStackArray(this.items, _itemStack, -1) >= 0;
		if (flag)
		{
			this.onBackpackChanged();
		}
		return flag;
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x00120240 File Offset: 0x0011E440
	public int DecItem(ItemValue _itemValue, int _count, bool _ignoreModdedItems = false, IList<ItemStack> _removedItems = null)
	{
		int num = _count;
		ItemStack[] slots = this.GetSlots();
		int num2 = 0;
		while (_count > 0 && num2 < this.GetSlots().Length)
		{
			if (slots[num2].itemValue.type == _itemValue.type && (!_ignoreModdedItems || !slots[num2].itemValue.HasModSlots || !slots[num2].itemValue.HasMods()))
			{
				if (ItemClass.GetForId(slots[num2].itemValue.type).CanStack())
				{
					int count = slots[num2].count;
					int num3 = (count >= _count) ? _count : count;
					if (_removedItems != null)
					{
						_removedItems.Add(new ItemStack(slots[num2].itemValue.Clone(), num3));
					}
					slots[num2].count -= num3;
					_count -= num3;
					if (slots[num2].count <= 0)
					{
						slots[num2].Clear();
					}
				}
				else
				{
					if (_removedItems != null)
					{
						_removedItems.Add(slots[num2].Clone());
					}
					slots[num2].Clear();
					_count--;
				}
			}
			num2++;
		}
		this.SetSlots(slots);
		return num - _count;
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x00120350 File Offset: 0x0011E550
	public int GetUsedSlotCount()
	{
		ItemStack[] slots = this.GetSlots();
		int num = 0;
		for (int i = 0; i < slots.Length; i++)
		{
			if (!slots[i].IsEmpty())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x00120384 File Offset: 0x0011E584
	public int GetItemCount(ItemValue _itemValue, int _seed = -1, int _meta = -1, bool _ignoreModdedItems = true)
	{
		ItemStack[] slots = this.GetSlots();
		int num = 0;
		for (int i = 0; i < slots.Length; i++)
		{
			if ((!_ignoreModdedItems || !slots[i].itemValue.HasModSlots || !slots[i].itemValue.HasMods()) && slots[i].itemValue.type == _itemValue.type && (_seed == -1 || _seed == (int)slots[i].itemValue.Seed) && (_meta == -1 || _meta == slots[i].itemValue.Meta))
			{
				num += slots[i].count;
			}
		}
		return num;
	}

	// Token: 0x06002D11 RID: 11537 RVA: 0x00120414 File Offset: 0x0011E614
	public int GetItemCount(FastTags<TagGroup.Global> itemTags, int _seed = -1, int _meta = -1, bool _ignoreModdedItems = true)
	{
		ItemStack[] slots = this.GetSlots();
		int num = 0;
		for (int i = 0; i < slots.Length; i++)
		{
			if ((!_ignoreModdedItems || !slots[i].itemValue.HasModSlots || !slots[i].itemValue.HasMods()) && !slots[i].itemValue.IsEmpty() && slots[i].itemValue.ItemClass.ItemTags.Test_AnySet(itemTags) && (_seed == -1 || _seed == (int)slots[i].itemValue.Seed) && (_meta == -1 || _meta == slots[i].itemValue.Meta))
			{
				num += slots[i].count;
			}
		}
		return num;
	}

	// Token: 0x06002D12 RID: 11538 RVA: 0x001204BD File Offset: 0x0011E6BD
	public override bool HasItem(ItemValue _item)
	{
		return this.GetItemCount(_item, -1, -1, true) > 0;
	}

	// Token: 0x0400235D RID: 9053
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte Version = 1;

	// Token: 0x0400235F RID: 9055
	public bool Touched;

	// Token: 0x04002360 RID: 9056
	public PackedBoolArray LockedSlots;

	// Token: 0x04002361 RID: 9057
	public PreferenceTracker preferences;

	// Token: 0x04002362 RID: 9058
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] items;
}
