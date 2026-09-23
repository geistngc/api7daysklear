using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000FB RID: 251
[Preserve]
public class BlockPlacementTorch : BlockPlacement
{
	// Token: 0x06000642 RID: 1602 RVA: 0x0002C86C File Offset: 0x0002AA6C
	public override BlockPlacement.Result OnPlaceBlock(BlockPlacement.EnumPlacement _placement, BlockPlacement.EnumRotationMode _mode, int _localRot, WorldBase _world, BlockValue _blockValue, PropTransform _propTransform, HitInfoDetails _hitInfo, Vector3 _entityPos)
	{
		if (_mode != BlockPlacement.EnumRotationMode.Auto)
		{
			return base.OnPlaceBlock(_placement, _mode, _localRot, _world, _blockValue, _propTransform, _hitInfo, _entityPos);
		}
		BlockPlacement.Result result = new BlockPlacement.Result(_placement, _blockValue, _propTransform, _hitInfo);
		result.blockValue.meta = 0;
		switch (_hitInfo.blockFace)
		{
		case BlockFace.North:
			result.blockValue.rotation = 0;
			break;
		case BlockFace.West:
			result.blockValue.rotation = 3;
			break;
		case BlockFace.South:
			result.blockValue.rotation = 2;
			break;
		case BlockFace.East:
			result.blockValue.rotation = 1;
			break;
		}
		return result;
	}
}
