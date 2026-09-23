using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x02000143 RID: 323
[Preserve]
public class BlockSolarPanel : BlockPowerSource
{
	// Token: 0x060008E3 RID: 2275 RVA: 0x0003E839 File Offset: 0x0003CA39
	public override TileEntityPowerSource CreateTileEntity(Chunk chunk)
	{
		if (this.slotItem == null)
		{
			this.slotItem = ItemClass.GetItemClass(this.SlotItemName, false);
		}
		return new TileEntityPowerSource(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.SolarPanel,
			SlotItem = this.slotItem
		};
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0003E870 File Offset: 0x0003CA70
	public override bool CanPlaceBlockAt(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (!base.CanPlaceBlockAt(_world, _blockPos, _blockValue, _bOmitCollideCheck))
		{
			return false;
		}
		Vector3i blockPos = _blockPos + Vector3i.up;
		ChunkCluster chunkCache = _world.ChunkCache;
		return chunkCache == null || chunkCache.GetLight(blockPos, Chunk.LIGHT_TYPE.SUN) >= 15;
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0003E8B1 File Offset: 0x0003CAB1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetPowerSourceIcon()
	{
		return "electric_solar";
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0003E8B8 File Offset: 0x0003CAB8
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		Manager.BroadcastStop(_blockPos.ToVector3(), this.runningSound);
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x0003E8D7 File Offset: 0x0003CAD7
	public override void OnBlockUnloaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockUnloaded(_world, _blockPos, _blockValue);
		Manager.Stop(_blockPos.ToVector3(), this.runningSound);
	}

	// Token: 0x04000994 RID: 2452
	[PublicizedFrom(EAccessModifier.Private)]
	public string runningSound = "solarpanel_idle";
}
