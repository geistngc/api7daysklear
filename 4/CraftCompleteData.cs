using System;
using System.IO;

// Token: 0x020002D7 RID: 727
public class CraftCompleteData
{
	// Token: 0x060014F1 RID: 5361 RVA: 0x0007EB87 File Offset: 0x0007CD87
	public CraftCompleteData()
	{
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x0007EBA8 File Offset: 0x0007CDA8
	public CraftCompleteData(int crafterEntityID, ItemStack craftedItemStack, string recipeName, string itemScrapped, int craftExpGain, ushort recipeUsedCount)
	{
		this.CrafterEntityID = crafterEntityID;
		this.CraftedItemStack = craftedItemStack;
		this.RecipeName = recipeName;
		this.ItemScrapped = itemScrapped;
		this.RecipeUsedCount = recipeUsedCount;
		this.CraftExpGain = craftExpGain;
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x0007EC00 File Offset: 0x0007CE00
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(1);
		_bw.Write(this.CrafterEntityID);
		this.CraftedItemStack.Write(_bw);
		_bw.Write(this.RecipeName);
		_bw.Write(this.CraftExpGain);
		_bw.Write(this.RecipeUsedCount);
		_bw.Write(this.ItemScrapped);
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x0007EC5C File Offset: 0x0007CE5C
	public void Read(BinaryReader _br)
	{
		_br.ReadUInt16();
		this.CrafterEntityID = _br.ReadInt32();
		this.CraftedItemStack = new ItemStack().Read(_br);
		this.RecipeName = _br.ReadString();
		this.CraftExpGain = _br.ReadInt32();
		this.RecipeUsedCount = _br.ReadUInt16();
		this.ItemScrapped = _br.ReadString();
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x0007ECC0 File Offset: 0x0007CEC0
	public void ReadLegacy(BinaryReader _br)
	{
		this.CrafterEntityID = _br.ReadInt32();
		this.CraftedItemStack = new ItemStack().Read(_br);
		this.RecipeName = _br.ReadString();
		this.CraftExpGain = _br.ReadInt32();
		this.RecipeUsedCount = _br.ReadUInt16();
		this.ItemScrapped = _br.ReadString();
	}

	// Token: 0x04000DF8 RID: 3576
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort Version = 1;

	// Token: 0x04000DF9 RID: 3577
	public int CrafterEntityID;

	// Token: 0x04000DFA RID: 3578
	public ItemStack CraftedItemStack;

	// Token: 0x04000DFB RID: 3579
	public string RecipeName = "";

	// Token: 0x04000DFC RID: 3580
	public string ItemScrapped = "";

	// Token: 0x04000DFD RID: 3581
	public ushort RecipeUsedCount;

	// Token: 0x04000DFE RID: 3582
	public int CraftExpGain;
}
