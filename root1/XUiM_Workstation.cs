using System;

// Token: 0x02001143 RID: 4419
public class XUiM_Workstation : XUiModel
{
	// Token: 0x17001020 RID: 4128
	// (get) Token: 0x06008BBA RID: 35770 RVA: 0x00353657 File Offset: 0x00351857
	public TileEntityWorkstation TileEntity
	{
		get
		{
			return this.tileEntity;
		}
	}

	// Token: 0x06008BBB RID: 35771 RVA: 0x0035365F File Offset: 0x0035185F
	public XUiM_Workstation(TileEntityWorkstation _te)
	{
		this.tileEntity = _te;
	}

	// Token: 0x06008BBC RID: 35772 RVA: 0x0035366E File Offset: 0x0035186E
	public bool GetIsBurning()
	{
		return this.tileEntity.IsBurning;
	}

	// Token: 0x06008BBD RID: 35773 RVA: 0x0035367B File Offset: 0x0035187B
	public bool GetIsBesideWater()
	{
		return this.tileEntity.IsBesideWater;
	}

	// Token: 0x06008BBE RID: 35774 RVA: 0x00353688 File Offset: 0x00351888
	public void SetIsBurning(bool _isBurning)
	{
		this.tileEntity.IsBurning = _isBurning;
		this.tileEntity.ResetTickTime();
	}

	// Token: 0x06008BBF RID: 35775 RVA: 0x003536A1 File Offset: 0x003518A1
	public ItemStack[] GetInputStacks()
	{
		return this.tileEntity.Input;
	}

	// Token: 0x06008BC0 RID: 35776 RVA: 0x003536AE File Offset: 0x003518AE
	public void SetInputStacks(ItemStack[] _itemStacks)
	{
		this.tileEntity.Input = _itemStacks;
	}

	// Token: 0x06008BC1 RID: 35777 RVA: 0x003536BC File Offset: 0x003518BC
	public void SetInputInSlot(int _idx, ItemStack _itemStack)
	{
		this.tileEntity.Input[_idx] = _itemStack.Clone();
	}

	// Token: 0x06008BC2 RID: 35778 RVA: 0x003536D1 File Offset: 0x003518D1
	public ItemStack[] GetOutputStacks()
	{
		return this.tileEntity.Output;
	}

	// Token: 0x06008BC3 RID: 35779 RVA: 0x003536DE File Offset: 0x003518DE
	public void SetOutputStacks(ItemStack[] _itemStacks)
	{
		this.tileEntity.Output = _itemStacks;
	}

	// Token: 0x06008BC4 RID: 35780 RVA: 0x003536EC File Offset: 0x003518EC
	public void SetOutputInSlot(int _idx, ItemStack _itemStack)
	{
		this.tileEntity.Output[_idx] = _itemStack.Clone();
	}

	// Token: 0x06008BC5 RID: 35781 RVA: 0x00353701 File Offset: 0x00351901
	public ItemStack[] GetToolStacks()
	{
		return this.tileEntity.Tools;
	}

	// Token: 0x06008BC6 RID: 35782 RVA: 0x0035370E File Offset: 0x0035190E
	public void SetToolStacks(ItemStack[] _itemStacks)
	{
		this.tileEntity.Tools = _itemStacks;
	}

	// Token: 0x06008BC7 RID: 35783 RVA: 0x0035371C File Offset: 0x0035191C
	public void SetToolInSlot(int _idx, ItemStack _itemStack)
	{
		this.tileEntity.Tools[_idx] = _itemStack.Clone();
	}

	// Token: 0x06008BC8 RID: 35784 RVA: 0x00353731 File Offset: 0x00351931
	public ItemStack[] GetFuelStacks()
	{
		return this.tileEntity.Fuel;
	}

	// Token: 0x06008BC9 RID: 35785 RVA: 0x0035373E File Offset: 0x0035193E
	public void SetFuelStacks(ItemStack[] _itemStacks)
	{
		this.tileEntity.Fuel = _itemStacks;
	}

	// Token: 0x06008BCA RID: 35786 RVA: 0x0035374C File Offset: 0x0035194C
	public void SetFuelInSlot(int _idx, ItemStack _itemStack)
	{
		this.tileEntity.Fuel[_idx] = _itemStack.Clone();
	}

	// Token: 0x06008BCB RID: 35787 RVA: 0x00353761 File Offset: 0x00351961
	public float GetBurnTimeLeft()
	{
		if (this.tileEntity.BurnTimeLeft == 0f)
		{
			return 0f;
		}
		return this.tileEntity.BurnTimeLeft + 0.5f;
	}

	// Token: 0x06008BCC RID: 35788 RVA: 0x0035378C File Offset: 0x0035198C
	public float GetTotalBurnTimeLeft()
	{
		if (this.tileEntity.BurnTotalTimeLeft == 0f)
		{
			return 0f;
		}
		return this.tileEntity.BurnTotalTimeLeft + 0.5f;
	}

	// Token: 0x06008BCD RID: 35789 RVA: 0x003537B7 File Offset: 0x003519B7
	public RecipeQueueItem[] GetRecipeQueueItems()
	{
		return this.tileEntity.Queue;
	}

	// Token: 0x06008BCE RID: 35790 RVA: 0x003537C4 File Offset: 0x003519C4
	public void SetRecipeQueueItems(RecipeQueueItem[] _queueStacks)
	{
		this.tileEntity.Queue = _queueStacks;
	}

	// Token: 0x06008BCF RID: 35791 RVA: 0x003537D2 File Offset: 0x003519D2
	public void SetQueueInSlot(int _idx, RecipeQueueItem _queueStack)
	{
		this.tileEntity.Queue[_idx] = _queueStack;
	}

	// Token: 0x06008BD0 RID: 35792 RVA: 0x003537E2 File Offset: 0x003519E2
	public void SetUserAccessing(bool _isUserAccessing)
	{
		this.tileEntity.SetUserAccessing(_isUserAccessing);
	}

	// Token: 0x06008BD1 RID: 35793 RVA: 0x003537F0 File Offset: 0x003519F0
	public string[] GetMaterialNames()
	{
		return this.tileEntity.MaterialNames;
	}

	// Token: 0x04006750 RID: 26448
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly TileEntityWorkstation tileEntity;
}
