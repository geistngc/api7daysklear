using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200015C RID: 348
[Preserve]
public class BlockTorchHeatMap : BlockTorch
{
	// Token: 0x06000995 RID: 2453 RVA: 0x0004225D File Offset: 0x0004045D
	public BlockTorchHeatMap()
	{
		this.IsRandomlyTick = true;
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0004226C File Offset: 0x0004046C
	public override bool UpdateTick(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		base.UpdateTick(_world, _blockPos, _blockValue, _bRandomTick, _ticksIfLoaded, _rnd);
		if (this.HeatMapStrength > 0f)
		{
			AIDirector aidirector = _world.GetAIDirector();
			if (aidirector != null)
			{
				float num = 1f;
				num *= 0.4f;
				aidirector.NotifyActivity(EnumAIDirectorChunkEvent.Torch, _blockPos, this.HeatMapStrength * num, 720f);
			}
		}
		return true;
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x000422C4 File Offset: 0x000404C4
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _blockValue, _ebcd);
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache == null)
		{
			return;
		}
		IChunk chunkSync = chunkCache.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
		if (chunkSync == null)
		{
			return;
		}
		BlockEntityData blockEntity = chunkSync.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return;
		}
		Transform transform = blockEntity.transform.FindInChildren("MainLight");
		if (transform)
		{
			LightLOD component = transform.GetComponent<LightLOD>();
			if (component)
			{
				component.SetBlockEntityData(blockEntity);
			}
		}
	}
}
