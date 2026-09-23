using System;
using System.Security.Cryptography;
using UnityEngine.Scripting;

// Token: 0x0200019F RID: 415
[Preserve]
public class BlockShapeBillboardAbstract : BlockShape
{
	// Token: 0x06000CA2 RID: 3234 RVA: 0x0004E62B File Offset: 0x0004C82B
	public BlockShapeBillboardAbstract()
	{
		this.IsSolidCube = false;
		this.IsSolidSpace = false;
		this.LightOpacity = 0;
		this.IsOmitTerrainSnappingUp = true;
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsRenderDecoration()
	{
		return true;
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return false;
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x00010E62 File Offset: 0x0000F062
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x000027FC File Offset: 0x000009FC
	public override void CalculateCollisionHash(IncrementalHash calcHash)
	{
	}

	// Token: 0x04000B5C RID: 2908
	public float yPosSubtract;
}
