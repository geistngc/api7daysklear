using System;
using UnityEngine.Scripting;

// Token: 0x02000138 RID: 312
[Preserve]
public class BlockPlant : Block
{
	// Token: 0x06000870 RID: 2160 RVA: 0x0003BD21 File Offset: 0x00039F21
	public BlockPlant()
	{
		this.IsRandomlyTick = true;
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0003BD37 File Offset: 0x00039F37
	public override bool CanPlaceBlockAt(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		return base.CanPlaceBlockAt(_world, _blockPos, _blockValue, _bOmitCollideCheck) && this.CanGrowOn(_world, _blockPos - Vector3i.up, _blockValue);
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0003BD5C File Offset: 0x00039F5C
	public virtual bool CanGrowOn(WorldBase _world, Vector3i _blockPos, BlockValue _blockValueOfPlant)
	{
		return GameManager.Instance.IsEditMode() || this.fertileLevel == 0 || _world.GetBlock(_blockPos).Block.blockMaterial.FertileLevel >= this.fertileLevel;
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x0003BDA5 File Offset: 0x00039FA5
	public override void OnNeighborBlockChange(WorldBase _world, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		base.OnNeighborBlockChange(_world, _myBlockPos, _myBlockValue, _blockPosThatChanged, _newNeighborBlockValue, _oldNeighborBlockValue);
		if (!_myBlockValue.ischild)
		{
			this.CheckPlantAlive(_world, _myBlockPos, _myBlockValue);
		}
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0003BDC9 File Offset: 0x00039FC9
	public override bool UpdateTick(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, GameRandom _rnd)
	{
		this.CheckPlantAlive(_world, _blockPos, _blockValue);
		return false;
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0003BDD6 File Offset: 0x00039FD6
	public virtual bool CheckPlantAlive(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (GameManager.Instance.IsEditMode())
		{
			return true;
		}
		if (!this.CanPlantStay(_world, _blockPos, _blockValue))
		{
			_world.SetBlockRPC(_blockPos, BlockValue.Air);
			return false;
		}
		return true;
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0003BE08 File Offset: 0x0003A008
	public override bool CanPlantStay(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		return GameManager.Instance.IsEditMode() || ((this.lightLevelStay == 0 || _world.GetBlockLightValue(_blockPos) >= this.lightLevelStay || _world.GetBlockLightValue(_blockPos + Vector3i.up) >= this.lightLevelStay || _world.IsOpenSkyAbove(_blockPos.x, _blockPos.y, _blockPos.z)) && this.CanGrowOn(_world, _blockPos - Vector3i.up, _blockValue));
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x0003BE82 File Offset: 0x0003A082
	public override BlockValue OnBlockPlaced(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.rotation = (byte)_rnd.RandomRange(4);
		return _blockValue;
	}

	// Token: 0x04000961 RID: 2401
	[PublicizedFrom(EAccessModifier.Protected)]
	public int lightLevelStay;

	// Token: 0x04000962 RID: 2402
	[PublicizedFrom(EAccessModifier.Protected)]
	public int fertileLevel = 1;
}
