using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x0200013F RID: 319
[Preserve]
public class BlockBladeTrap : BlockPoweredTrap
{
	// Token: 0x060008C3 RID: 2243 RVA: 0x0003DB64 File Offset: 0x0003BD64
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("RunningSound"))
		{
			this.runningSound = base.Properties.Values["RunningSound"];
		}
		if (base.Properties.Values.ContainsKey("RunningSoundBreaking"))
		{
			this.runningSoundPartlyBroken = base.Properties.Values["RunningSoundBreaking"];
		}
		if (base.Properties.Values.ContainsKey("RunningSoundBroken"))
		{
			this.runningSoundBroken = base.Properties.Values["RunningSoundBroken"];
		}
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0003DC10 File Offset: 0x0003BE10
	public override bool ActivateTrap(BlockEntityData blockEntity, bool isOn)
	{
		SpinningBladeTrapController component = blockEntity.transform.gameObject.GetComponent<SpinningBladeTrapController>();
		if (component == null)
		{
			return false;
		}
		component.Init(base.Properties, this);
		component.BlockPosition = blockEntity.pos;
		component.HealthRatio = 1f - (float)blockEntity.blockValue.damage / (float)blockEntity.blockValue.Block.MaxDamage;
		component.IsOn = isOn;
		return true;
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x0003DC84 File Offset: 0x0003BE84
	public override int OnBlockDamaged(WorldBase _world, BlockValueRef _bvRef, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		ChunkCluster chunkCache = _world.ChunkCache;
		if (chunkCache != null)
		{
			IChunk chunkSync = chunkCache.GetChunkSync(_bvRef);
			if (chunkSync != null)
			{
				BlockEntityData blockEntity = chunkSync.GetBlockEntity(_bvRef);
				if (blockEntity != null && blockEntity.bHasTransform)
				{
					SpinningBladeTrapController component = blockEntity.transform.gameObject.GetComponent<SpinningBladeTrapController>();
					if (component != null)
					{
						component.HealthRatio = 1f - (float)blockEntity.blockValue.damage / (float)blockEntity.blockValue.Block.MaxDamage;
					}
				}
			}
		}
		return base.OnBlockDamaged(_world, _bvRef, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x0003DD18 File Offset: 0x0003BF18
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		Manager.BroadcastStop(_blockPos.ToVector3(), this.runningSound);
		Manager.BroadcastStop(_blockPos.ToVector3(), this.runningSoundPartlyBroken);
		Manager.BroadcastStop(_blockPos.ToVector3(), this.runningSoundBroken);
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0003DD68 File Offset: 0x0003BF68
	public override void OnBlockUnloaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		Manager.Stop(_blockPos.ToVector3(), this.runningSound);
		Manager.Stop(_blockPos.ToVector3(), this.runningSoundPartlyBroken);
		Manager.Stop(_blockPos.ToVector3(), this.runningSoundBroken);
		base.OnBlockUnloaded(_world, _blockPos, _blockValue);
	}

	// Token: 0x04000988 RID: 2440
	[PublicizedFrom(EAccessModifier.Private)]
	public string runningSound = "Electricity/BladeTrap/bladetrap_fire_lp";

	// Token: 0x04000989 RID: 2441
	[PublicizedFrom(EAccessModifier.Private)]
	public string runningSoundPartlyBroken = "Electricity/BladeTrap/bladetrap_dm1_lp";

	// Token: 0x0400098A RID: 2442
	[PublicizedFrom(EAccessModifier.Private)]
	public string runningSoundBroken = "Electricity/BladeTrap/bladetrap_dm2_lp";
}
