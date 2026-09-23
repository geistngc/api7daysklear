using System;
using UnityEngine.Scripting;

// Token: 0x02000124 RID: 292
[Preserve]
public class BlockJumpPad : Block
{
	// Token: 0x060007B9 RID: 1977 RVA: 0x00036CFD File Offset: 0x00034EFD
	public override void OnEntityWalking(WorldBase _world, int _x, int _y, int _z, BlockValue _blockValue, Entity entity)
	{
		entity.motion.y = 3f;
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x00010E62 File Offset: 0x0000F062
	public override BlockFace getInventoryFace()
	{
		return BlockFace.Top;
	}
}
