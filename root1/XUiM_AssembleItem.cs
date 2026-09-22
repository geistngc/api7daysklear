using System;

// Token: 0x02001124 RID: 4388
public class XUiM_AssembleItem : XUiModel
{
	// Token: 0x17001012 RID: 4114
	// (get) Token: 0x06008AB2 RID: 35506 RVA: 0x0034D119 File Offset: 0x0034B319
	// (set) Token: 0x06008AB3 RID: 35507 RVA: 0x0034D121 File Offset: 0x0034B321
	public ItemStack CurrentItem
	{
		get
		{
			return this.currentItem;
		}
		set
		{
			this.currentItem = value;
			if (value != null)
			{
				this.SetPartCount();
			}
		}
	}

	// Token: 0x17001013 RID: 4115
	// (get) Token: 0x06008AB4 RID: 35508 RVA: 0x0034D133 File Offset: 0x0034B333
	// (set) Token: 0x06008AB5 RID: 35509 RVA: 0x0034D13B File Offset: 0x0034B33B
	public XUiC_ItemStack CurrentItemStackController
	{
		get
		{
			return this.currentItemStackController;
		}
		set
		{
			if (this.currentItemStackController != null)
			{
				this.currentItemStackController.AssembleLock = false;
			}
			this.currentItemStackController = value;
			if (this.currentItemStackController != null)
			{
				this.currentItemStackController.AssembleLock = true;
			}
		}
	}

	// Token: 0x17001014 RID: 4116
	// (get) Token: 0x06008AB6 RID: 35510 RVA: 0x0034D16C File Offset: 0x0034B36C
	// (set) Token: 0x06008AB7 RID: 35511 RVA: 0x0034D174 File Offset: 0x0034B374
	public XUiC_EquipmentStack CurrentEquipmentStackController
	{
		get
		{
			return this.currentEquipmentStackController;
		}
		set
		{
			this.currentEquipmentStackController = value;
		}
	}

	// Token: 0x17001015 RID: 4117
	// (get) Token: 0x06008AB8 RID: 35512 RVA: 0x0034D17D File Offset: 0x0034B37D
	// (set) Token: 0x06008AB9 RID: 35513 RVA: 0x0034D185 File Offset: 0x0034B385
	public int PartCount { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06008ABA RID: 35514 RVA: 0x0034D190 File Offset: 0x0034B390
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetPartCount()
	{
		this.PartCount = 0;
		for (int i = 0; i < this.CurrentItem.itemValue.Modifications.Length; i++)
		{
			if (this.CurrentItem.itemValue.Modifications[i] == null)
			{
				this.CurrentItem.itemValue.Modifications[i] = ItemValue.None;
			}
			if (!this.CurrentItem.itemValue.Modifications[i].IsEmpty())
			{
				int partCount = this.PartCount;
				this.PartCount = partCount + 1;
			}
		}
	}

	// Token: 0x06008ABB RID: 35515 RVA: 0x0034D218 File Offset: 0x0034B418
	public void RefreshAssembleItem()
	{
		this.PartCount = 0;
		bool flag = false;
		for (int i = 0; i < this.CurrentItem.itemValue.Modifications.Length; i++)
		{
			if (!this.CurrentItem.itemValue.Modifications[i].IsEmpty())
			{
				int partCount = this.PartCount;
				this.PartCount = partCount + 1;
			}
			else
			{
				flag = true;
			}
		}
		if (this.CurrentItemStackController != null)
		{
			this.CurrentItemStackController.ForceSetItemStack(this.CurrentItem);
			this.CurrentItemStackController.AssembleLock = true;
		}
		if (this.currentEquipmentStackController != null)
		{
			this.currentEquipmentStackController.ItemStack = this.CurrentItem;
		}
		if (flag)
		{
			QuestEventManager.Current.AssembledItem(this.CurrentItem);
		}
	}

	// Token: 0x06008ABC RID: 35516 RVA: 0x0034D2CC File Offset: 0x0034B4CC
	public bool AddPartToItem(ItemStack _partStack, out ItemStack _resultStack)
	{
		if (this.CurrentItem == null || this.CurrentItem.IsEmpty())
		{
			_resultStack = _partStack;
			return false;
		}
		ItemClassModifier itemClassModifier = _partStack.itemValue.ItemClass as ItemClassModifier;
		if (itemClassModifier != null)
		{
			if (this.CurrentItem.itemValue.ItemClass.HasAnyTags(itemClassModifier.DisallowedTags))
			{
				_resultStack = _partStack;
				return false;
			}
			if (itemClassModifier.ModifierTags.Test_AnySet(ItemClassModifier.CosmeticModTypes))
			{
				for (int i = 0; i < this.CurrentItem.itemValue.CosmeticMods.Length; i++)
				{
					if (this.CurrentItem.itemValue.CosmeticMods[i] != null && this.CurrentItem.itemValue.CosmeticMods[i].ItemClass != null)
					{
						ItemClassModifier itemClassModifier2 = this.CurrentItem.itemValue.CosmeticMods[i].ItemClass as ItemClassModifier;
						if (itemClassModifier2 != null && itemClassModifier.ModifierTags.Test_AnySet(itemClassModifier2.ModifierTags))
						{
							_resultStack = _partStack;
							return false;
						}
					}
				}
			}
			else
			{
				int num = 0;
				for (int j = 0; j < this.CurrentItem.itemValue.Modifications.Length; j++)
				{
					if (this.CurrentItem.itemValue.Modifications[j] != null && this.CurrentItem.itemValue.Modifications[j].ItemClass != null && !itemClassModifier.HasAnyTags(EntityDrone.StorageModifierTags))
					{
						ItemClassModifier itemClassModifier3 = this.CurrentItem.itemValue.Modifications[j].ItemClass as ItemClassModifier;
						if (itemClassModifier3 != null && itemClassModifier.ModifierTags.Test_AnySet(itemClassModifier3.ModifierTags))
						{
							num++;
						}
					}
				}
				if (num >= itemClassModifier.MaxModsAllowed)
				{
					_resultStack = _partStack;
					return false;
				}
			}
		}
		if (this.CurrentItem.itemValue.ItemClass.HasAnyTags(itemClassModifier.InstallableTags))
		{
			if (itemClassModifier.ModifierTags.Test_AnySet(ItemClassModifier.CosmeticModTypes))
			{
				if (this.CurrentItem.itemValue.CosmeticMods != null)
				{
					for (int k = 0; k < this.CurrentItem.itemValue.CosmeticMods.Length; k++)
					{
						if (this.CurrentItem.itemValue.CosmeticMods[k] == null || this.CurrentItem.itemValue.CosmeticMods[k].IsEmpty())
						{
							float num2 = 1f - this.CurrentItem.itemValue.PercentUsesLeft;
							this.CurrentItem.itemValue.CosmeticMods[k] = _partStack.itemValue.Clone();
							if (this.CurrentItemStackController != null)
							{
								XUiC_AssembleWindowGroup.GetWindowGroup(this.CurrentItemStackController.xui).ItemStack = this.CurrentItem;
							}
							if (this.currentEquipmentStackController != null)
							{
								XUiC_AssembleWindowGroup.GetWindowGroup(this.CurrentEquipmentStackController.xui).ItemStack = this.CurrentItem;
							}
							this.RefreshAssembleItem();
							if (this.CurrentItem.itemValue.MaxUseTimes > 0)
							{
								this.CurrentItem.itemValue.UseTimes = (float)((int)(num2 * (float)this.CurrentItem.itemValue.MaxUseTimes));
							}
							this.UpdateAssembleWindow();
							_resultStack = ItemStack.Empty;
							return true;
						}
					}
				}
			}
			else if (this.CurrentItem.itemValue.Modifications != null)
			{
				for (int l = 0; l < this.CurrentItem.itemValue.Modifications.Length; l++)
				{
					if (this.CurrentItem.itemValue.Modifications[l] == null || this.CurrentItem.itemValue.Modifications[l].IsEmpty())
					{
						float num3 = 1f - this.CurrentItem.itemValue.PercentUsesLeft;
						this.CurrentItem.itemValue.Modifications[l] = _partStack.itemValue.Clone();
						if (this.CurrentItemStackController != null)
						{
							XUiC_AssembleWindowGroup.GetWindowGroup(this.CurrentItemStackController.xui).ItemStack = this.CurrentItem;
						}
						if (this.currentEquipmentStackController != null)
						{
							XUiC_AssembleWindowGroup.GetWindowGroup(this.CurrentEquipmentStackController.xui).ItemStack = this.CurrentItem;
						}
						this.RefreshAssembleItem();
						if (this.CurrentItem.itemValue.MaxUseTimes > 0)
						{
							this.CurrentItem.itemValue.UseTimes = (float)((int)(num3 * (float)this.CurrentItem.itemValue.MaxUseTimes));
						}
						this.UpdateAssembleWindow();
						_resultStack = ItemStack.Empty;
						return true;
					}
				}
			}
		}
		_resultStack = _partStack;
		return false;
	}

	// Token: 0x06008ABD RID: 35517 RVA: 0x0034D721 File Offset: 0x0034B921
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateAssembleWindow()
	{
		if (this.AssembleWindow != null)
		{
			this.AssembleWindow.ItemStack = this.CurrentItem;
			this.AssembleWindow.OnChanged();
		}
	}

	// Token: 0x040066E8 RID: 26344
	public XUiC_AssembleWindow AssembleWindow;

	// Token: 0x040066E9 RID: 26345
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack currentItem;

	// Token: 0x040066EA RID: 26346
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemStack currentItemStackController;

	// Token: 0x040066EB RID: 26347
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_EquipmentStack currentEquipmentStackController;
}
