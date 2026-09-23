using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000FD RID: 253
[Preserve]
public class BlockPlacementTowardsPlacer90 : BlockPlacement
{
	// Token: 0x06000646 RID: 1606 RVA: 0x0002CA0C File Offset: 0x0002AC0C
	public override BlockPlacement.Result OnPlaceBlock(BlockPlacement.EnumPlacement _placement, BlockPlacement.EnumRotationMode _mode, int _localRot, WorldBase _world, BlockValue _blockValue, PropTransform _propTransform, HitInfoDetails _hitInfo, Vector3 _entityPos)
	{
		if (_mode != BlockPlacement.EnumRotationMode.Auto)
		{
			return base.OnPlaceBlock(_placement, _mode, _localRot, _world, _blockValue, _propTransform, _hitInfo, _entityPos);
		}
		BlockPlacement.Result result = new BlockPlacement.Result(_placement, _blockValue, _propTransform, _hitInfo);
		result.blockValue.rotation = 0;
		float num = _entityPos.x - _hitInfo.pos.x;
		float num2 = _entityPos.z - _hitInfo.pos.z;
		if (Mathf.Abs(num) > Mathf.Abs(num2) && num > 0f)
		{
			result.blockValue.rotation = 0;
		}
		else if (Mathf.Abs(num) > Mathf.Abs(num2) && num <= 0f)
		{
			result.blockValue.rotation = 2;
		}
		else if (Mathf.Abs(num2) > Mathf.Abs(num) && num2 > 0f)
		{
			result.blockValue.rotation = 3;
		}
		else if (Mathf.Abs(num2) > Mathf.Abs(num) && num2 <= 0f)
		{
			result.blockValue.rotation = 1;
		}
		return result;
	}
}
