using System;
using System.IO;

// Token: 0x020002D5 RID: 725
public class CraftingData
{
	// Token: 0x060014ED RID: 5357 RVA: 0x0007EA62 File Offset: 0x0007CC62
	public CraftingData()
	{
		this.items = new ItemStack[0];
		this.outputItems = new ItemStack[0];
		this.breakDownType = CraftingData.BreakdownType.None;
		this.RecipeQueueItems = new RecipeQueueItem[0];
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x0007EA98 File Offset: 0x0007CC98
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(2);
		int num = this.RecipeQueueItems.Length;
		_bw.Write((byte)num);
		for (int i = 0; i < num; i++)
		{
			if (this.RecipeQueueItems[i] == null)
			{
				this.RecipeQueueItems[i] = new RecipeQueueItem();
			}
			this.RecipeQueueItems[i].Write(_bw);
		}
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x0007EAF0 File Offset: 0x0007CCF0
	public void Read(BinaryReader _br)
	{
		_br.ReadUInt16();
		int num = (int)_br.ReadByte();
		this.RecipeQueueItems = new RecipeQueueItem[num];
		for (int i = 0; i < num; i++)
		{
			this.RecipeQueueItems[i] = new RecipeQueueItem();
			this.RecipeQueueItems[i].Read(_br);
		}
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x0007EB40 File Offset: 0x0007CD40
	public void ReadLegacy(BinaryReader _br)
	{
		int num = (int)_br.ReadByte();
		this.RecipeQueueItems = new RecipeQueueItem[num];
		for (int i = 0; i < num; i++)
		{
			this.RecipeQueueItems[i] = new RecipeQueueItem();
			this.RecipeQueueItems[i].ReadLegacy(_br);
		}
	}

	// Token: 0x04000DE6 RID: 3558
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort Version = 2;

	// Token: 0x04000DE7 RID: 3559
	public ItemStack[] items;

	// Token: 0x04000DE8 RID: 3560
	public ItemStack[] outputItems;

	// Token: 0x04000DE9 RID: 3561
	public bool isCrafting;

	// Token: 0x04000DEA RID: 3562
	public ulong lastWorldTick;

	// Token: 0x04000DEB RID: 3563
	public int totalLeftToCraft;

	// Token: 0x04000DEC RID: 3564
	public float currentRecipeTimer;

	// Token: 0x04000DED RID: 3565
	public Recipe currentRecipeToCraft;

	// Token: 0x04000DEE RID: 3566
	public Recipe lastRecipeToCraft;

	// Token: 0x04000DEF RID: 3567
	public ItemValue repairedItem;

	// Token: 0x04000DF0 RID: 3568
	public CraftingData.BreakdownType breakDownType;

	// Token: 0x04000DF1 RID: 3569
	public bool isItemPlacedByUser;

	// Token: 0x04000DF2 RID: 3570
	public ulong savedWorldTick;

	// Token: 0x04000DF3 RID: 3571
	public RecipeQueueItem[] RecipeQueueItems;

	// Token: 0x020002D6 RID: 726
	public enum BreakdownType
	{
		// Token: 0x04000DF5 RID: 3573
		None,
		// Token: 0x04000DF6 RID: 3574
		Part,
		// Token: 0x04000DF7 RID: 3575
		Recipe
	}
}
