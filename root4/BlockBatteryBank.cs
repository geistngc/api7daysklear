using System;
using UnityEngine.Scripting;

// Token: 0x02000144 RID: 324
[Preserve]
public class BlockBatteryBank : BlockPowerSource
{
	// Token: 0x060008E9 RID: 2281 RVA: 0x0003E907 File Offset: 0x0003CB07
	public override TileEntityPowerSource CreateTileEntity(Chunk chunk)
	{
		if (this.slotItem == null)
		{
			this.slotItem = ItemClass.GetItemClass(this.SlotItemName, false);
		}
		return new TileEntityPowerSource(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.BatteryBank,
			SlotItem = this.slotItem
		};
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x0003E93C File Offset: 0x0003CB3C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetPowerSourceIcon()
	{
		return "battery";
	}
}
