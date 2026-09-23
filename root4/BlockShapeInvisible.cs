using System;
using System.Security.Cryptography;
using UnityEngine.Scripting;

// Token: 0x020001AB RID: 427
[Preserve]
public class BlockShapeInvisible : BlockShape
{
	// Token: 0x06000CD9 RID: 3289 RVA: 0x00050D28 File Offset: 0x0004EF28
	public BlockShapeInvisible()
	{
		this.IsSolidCube = false;
		this.IsSolidSpace = false;
		this.LightOpacity = 0;
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00050D45 File Offset: 0x0004EF45
	public override void Init(Block _block)
	{
		base.Init(_block);
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return false;
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00010E62 File Offset: 0x0000F062
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x000027FC File Offset: 0x000009FC
	public override void CalculateCollisionHash(IncrementalHash calcHash)
	{
	}
}
