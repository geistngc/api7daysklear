using System;
using UnityEngine.Scripting;

// Token: 0x02000154 RID: 340
[Preserve]
public class BlockStairs : Block
{
	// Token: 0x0600096B RID: 2411 RVA: 0x00041766 File Offset: 0x0003F966
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFace _face)
	{
		return !_blockValue.ischild;
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x00041766 File Offset: 0x0003F966
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFaceFlag _sides)
	{
		return !_blockValue.ischild;
	}
}
