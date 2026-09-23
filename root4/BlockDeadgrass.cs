using System;
using UnityEngine.Scripting;

// Token: 0x0200011C RID: 284
[Preserve]
public class BlockDeadgrass : Block
{
	// Token: 0x0600077F RID: 1919 RVA: 0x0003569B File Offset: 0x0003389B
	public override void Init()
	{
		base.Init();
		this.IsDecoration = true;
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x000356AC File Offset: 0x000338AC
	public override void OnNeighborBlockChange(WorldBase world, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		if (_blockPosThatChanged.x == _myBlockPos.x && _blockPosThatChanged.z == _myBlockPos.z && _blockPosThatChanged.y == _myBlockPos.y - 1 && !_newNeighborBlockValue.Block.shape.IsSolidCube)
		{
			world.SetBlockRPC(_myBlockPos, BlockValue.Air);
		}
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x0003570C File Offset: 0x0003390C
	public override BlockValue OnBlockPlaced(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.meta = (byte)_rnd.RandomRange(16);
		return _blockValue;
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x00035720 File Offset: 0x00033920
	public override void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, GameRandom _rnd)
	{
		_bpResult.blockValue.meta = (byte)_rnd.RandomRange(16);
	}
}
