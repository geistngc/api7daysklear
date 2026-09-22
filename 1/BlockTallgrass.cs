using System;
using UnityEngine.Scripting;

// Token: 0x02000157 RID: 343
[Preserve]
public class BlockTallgrass : BlockPlant
{
	// Token: 0x0600097B RID: 2427 RVA: 0x00041CA8 File Offset: 0x0003FEA8
	public BlockTallgrass()
	{
		this.IsRandomlyTick = false;
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00041CB7 File Offset: 0x0003FEB7
	public override bool CheckPlantAlive(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!this.CanPlantStay(_world, _blockPos, _blockValue))
		{
			_world.SetBlockRPC(_blockPos, BlockValue.Air);
			return false;
		}
		return true;
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00041CD8 File Offset: 0x0003FED8
	public override void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, GameRandom _rnd)
	{
		_bpResult.blockValue.meta2and1 = this.CalcMeta(_rnd);
		_bpResult.blockValue.rotation = (byte)_rnd.RandomRange(32);
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00041D02 File Offset: 0x0003FF02
	public override BlockValue OnBlockPlaced(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.meta2and1 = this.CalcMeta(_rnd);
		_blockValue.rotation = (byte)_rnd.RandomRange(32);
		return _blockValue;
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00041D25 File Offset: 0x0003FF25
	[PublicizedFrom(EAccessModifier.Private)]
	public byte CalcMeta(GameRandom _rnd)
	{
		return (byte)(_rnd.RandomRange(6) | (_rnd.RandomRange(256) & -8));
	}
}
