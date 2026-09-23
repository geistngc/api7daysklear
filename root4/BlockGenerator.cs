using System;
using UnityEngine.Scripting;

// Token: 0x02000142 RID: 322
[Preserve]
public class BlockGenerator : BlockPowerSource
{
	// Token: 0x060008DF RID: 2271 RVA: 0x0003E7E4 File Offset: 0x0003C9E4
	public override TileEntityPowerSource CreateTileEntity(Chunk chunk)
	{
		if (this.slotItem == null)
		{
			this.slotItem = ItemClass.GetItemClass(this.SlotItemName, false);
		}
		return new TileEntityPowerSource(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.Generator,
			SlotItem = this.slotItem
		};
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0003E819 File Offset: 0x0003CA19
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetPowerSourceIcon()
	{
		return "electric_generator";
	}

	// Token: 0x04000993 RID: 2451
	public static FastTags<TagGroup.Global> tag = FastTags<TagGroup.Global>.Parse("gasoline");
}
