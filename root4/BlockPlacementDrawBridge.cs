using System;
using UnityEngine.Scripting;

// Token: 0x020000F7 RID: 247
[Preserve]
public class BlockPlacementDrawBridge : BlockPlacementTowardsPlacer
{
	// Token: 0x0600063A RID: 1594 RVA: 0x0002C5D8 File Offset: 0x0002A7D8
	public override byte LimitRotation(BlockPlacement.EnumRotationMode _mode, ref int _localRot, HitInfoDetails _hitInfo, bool _bAdd, BlockValue _bv, byte _rotation)
	{
		if (_mode != BlockPlacement.EnumRotationMode.Auto)
		{
			return base.LimitRotation(_mode, ref _localRot, _hitInfo, _bAdd, _bv, _rotation);
		}
		int num = (int)(_bAdd ? (_rotation + 1) : (_rotation - 1));
		if (num > 3)
		{
			return 0;
		}
		if (num < 0)
		{
			return 3;
		}
		return (byte)num;
	}
}
