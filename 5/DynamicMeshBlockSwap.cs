using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConcurrentCollections;

// Token: 0x0200035E RID: 862
public class DynamicMeshBlockSwap
{
	// Token: 0x060018FA RID: 6394 RVA: 0x0008CE24 File Offset: 0x0008B024
	public static bool IsValidBlock(int type)
	{
		return DynamicMeshBlockSwap.OpaqueBlocks.Contains(type) || DynamicMeshBlockSwap.TerrainBlocks.Contains(type);
	}

	// Token: 0x060018FB RID: 6395 RVA: 0x0008CE40 File Offset: 0x0008B040
	public static void Init()
	{
		DynamicMeshBlockSwap.BlockSwaps.Clear();
		DynamicMeshBlockSwap.TextureSwaps.Clear();
		DynamicMeshBlockSwap.OpaqueBlocks.Clear();
		DynamicMeshBlockSwap.DoorBlocks.Clear();
		DynamicMeshBlockSwap.TerrainBlocks.Clear();
		DynamicMeshBlockSwap.DoorReplacement = Block.GetBlockValue("imposterBlock", true);
		if (DynamicMeshBlockSwap.DoorReplacement.isair)
		{
			Log.Warning("Dynamic mesh door replacement block not found");
		}
		else
		{
			Log.Out("Dymesh door replacement: " + DynamicMeshBlockSwap.DoorReplacement.Block.GetBlockName());
		}
		foreach (Block block in Block.list)
		{
			if (block != null && block.blockID != 0)
			{
				BlockCompositeTileEntity blockCompositeTileEntity = block as BlockCompositeTileEntity;
				bool flag = blockCompositeTileEntity != null && blockCompositeTileEntity.CompositeData.HasFeature<TEFeatureDoor>();
				if (block.MeshIndex == 0 || flag)
				{
					int type = Block.GetBlockValue(block.GetBlockName(), false).type;
					bool flag2 = block is BlockModelTree;
					bool flag3 = block.shape is BlockShapeModelEntity;
					if (!flag2 && !block.IsPlant() && !flag3)
					{
						DynamicMeshBlockSwap.OpaqueBlocks.Add(type);
					}
					if (flag)
					{
						DynamicMeshBlockSwap.DoorBlocks.Add(type);
					}
					if (block.bImposterExcludeAndStop || block.bImposterExclude || (block.IsTerrainDecoration && block.ImposterExchange == 0))
					{
						DynamicMeshBlockSwap.BlockSwaps.TryAdd(block.blockID, 0);
					}
					else if (block.ImposterExchange != 0)
					{
						DynamicMeshBlockSwap.BlockSwaps.TryAdd(block.blockID, block.ImposterExchange);
						DynamicMeshBlockSwap.TextureSwaps.TryAdd(block.blockID, (long)((ulong)block.ImposterExchangeTexIdx));
					}
				}
				else if (block.MeshIndex == 5)
				{
					int type2 = Block.GetBlockValue(block.GetBlockName(), false).type;
					DynamicMeshBlockSwap.TerrainBlocks.Add(type2);
				}
			}
		}
	}

	// Token: 0x04000FEA RID: 4074
	public static BlockValue DoorReplacement;

	// Token: 0x04000FEB RID: 4075
	public static ConcurrentHashSet<int> DoorBlocks = new ConcurrentHashSet<int>();

	// Token: 0x04000FEC RID: 4076
	public static ConcurrentHashSet<int> OpaqueBlocks = new ConcurrentHashSet<int>();

	// Token: 0x04000FED RID: 4077
	public static ConcurrentHashSet<int> TerrainBlocks = new ConcurrentHashSet<int>();

	// Token: 0x04000FEE RID: 4078
	public static ConcurrentDictionary<int, int> BlockSwaps = new ConcurrentDictionary<int, int>();

	// Token: 0x04000FEF RID: 4079
	public static ConcurrentDictionary<int, long> TextureSwaps = new ConcurrentDictionary<int, long>();

	// Token: 0x04000FF0 RID: 4080
	public static HashSet<int> InvalidPaintIds = new HashSet<int>();
}
