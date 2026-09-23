using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001BA RID: 442
[Preserve]
public class BlockShapeWater : BlockShapeCube
{
	// Token: 0x06000D4B RID: 3403 RVA: 0x0005700A File Offset: 0x0005520A
	public BlockShapeWater()
	{
		this.IsSolidCube = false;
		this.IsSolidSpace = false;
		this.LightOpacity = 0;
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x00057027 File Offset: 0x00055227
	public override void renderFace(Vector3[] _vertices, LightingAround _lightingAround, long _textureFull, VoxelMesh[] _meshes, Vector2 UVdata, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		_meshes[1].AddBasicQuad(_vertices, Color.white, UVdata, true, false);
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00010E62 File Offset: 0x0000F062
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return false;
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x0003D2E2 File Offset: 0x0003B4E2
	public override float GetStepHeight(BlockValue _blockValue, BlockFace crossingFace)
	{
		return 0f;
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x00010E62 File Offset: 0x0000F062
	public override bool IsMovementBlocked(BlockValue _blockValue, BlockFace crossingFace)
	{
		return false;
	}
}
