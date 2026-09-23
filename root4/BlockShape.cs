using System;
using System.Security.Cryptography;
using UnityEngine;

// Token: 0x0200019D RID: 413
public abstract class BlockShape
{
	// Token: 0x06000C79 RID: 3193 RVA: 0x0004E40C File Offset: 0x0004C60C
	public BlockShape()
	{
		this.bounds = BoundsUtils.BoundsForMinMax(0f, 0f, 0f, 1f, 1f, 1f);
		this.boundsArr = new Bounds[]
		{
			this.bounds
		};
		this.IsSolidCube = true;
		this.IsSolidSpace = true;
		this.IsRotatable = false;
		this.IsOmitTerrainSnappingUp = false;
		this.LightOpacity = byte.MaxValue;
		this.minBounds = Vector3.zero;
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0004E49A File Offset: 0x0004C69A
	public virtual void Init(Block _block)
	{
		this.block = _block;
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void LateInit()
	{
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x0004E4A3 File Offset: 0x0004C6A3
	public virtual Quaternion GetRotation(BlockValue _blockValue)
	{
		return Quaternion.identity;
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x0004E4AA File Offset: 0x0004C6AA
	public virtual Vector3 GetRotationOffset(BlockValue _blockValue)
	{
		return Vector3.zero;
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual int[][] GetRotationLookup(int _rotation)
	{
		return null;
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0004E4B1 File Offset: 0x0004C6B1
	public virtual byte Rotate(bool _bLeft, int _rotation)
	{
		return (byte)_rotation;
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x0004E4B5 File Offset: 0x0004C6B5
	public virtual BlockValue RotateY(bool _bLeft, BlockValue _blockValue, int _rotCount)
	{
		_blockValue.rotation = (byte)((int)_blockValue.rotation + _rotCount & 15);
		return _blockValue;
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x0004E4CC File Offset: 0x0004C6CC
	public virtual BlockValue MirrorY(bool _bAlongZ, BlockValue _blockValue)
	{
		_blockValue = this.RotateY(true, _blockValue, 1);
		return this.RotateY(true, _blockValue, 1);
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0004E4E2 File Offset: 0x0004C6E2
	public virtual int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 255;
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return true;
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void renderFace(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, BlockFace _face, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void renderFace(Vector3[] _vertices, LightingAround _lightingAround, long _textureFull, VoxelMesh[] _meshes, Vector2 UVdata, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void renderFull(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsRenderDecoration()
	{
		return false;
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x0004E4EC File Offset: 0x0004C6EC
	public virtual void renderDecorations(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, INeighborBlockCache _nBlocks)
	{
		this.renderFull(_worldPos, _blockValue, _drawPos, _vertices, _lightingAround, _textureFullArray, _meshes, BlockShape.MeshPurpose.World);
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x0004E50B File Offset: 0x0004C70B
	public virtual int MapSideAndRotationToTextureIdx(BlockValue _blockValue, BlockFace _side)
	{
		return (int)_side;
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x0004E50E File Offset: 0x0004C70E
	public virtual Bounds[] GetBounds(BlockValue _blockValue)
	{
		return this.boundsArr;
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0004E516 File Offset: 0x0004C716
	public virtual Vector2 GetPathOffset(int _rotation)
	{
		return Vector2.zero;
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x0004E4A3 File Offset: 0x0004C6A3
	public virtual Quaternion GetPreviewRotation()
	{
		return Quaternion.identity;
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0004E4AA File Offset: 0x0004C6AA
	public virtual Vector3 GetPreviewPosition()
	{
		return Vector3.zero;
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0004E51D File Offset: 0x0004C71D
	public virtual float GetStepHeight(BlockValue blockDef, BlockFace crossingFace)
	{
		return (float)(blockDef.Block.IsCollideMovement ? 1 : 0);
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0004E532 File Offset: 0x0004C732
	public virtual bool IsMovementBlocked(BlockValue blockDef, BlockFace crossingFace)
	{
		return this.GetStepHeight(blockDef, crossingFace) > 0.5f;
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x0004E543 File Offset: 0x0004C743
	public void SetMinAABB(Vector3 _add)
	{
		this.minBounds = _add;
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsTerrain()
	{
		return false;
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnBlockValueChanged(WorldBase _world, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, BlockEntityData _ebcd)
	{
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnBlockLoaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void OnBlockUnloaded(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public virtual VoxelMesh GetBoundsMesh(BlockValue _blockValue)
	{
		return null;
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x0004E50B File Offset: 0x0004C70B
	public virtual BlockFace GetRotatedBlockFace(BlockValue _blockValue, BlockFace _face)
	{
		return _face;
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x0004E54C File Offset: 0x0004C74C
	public virtual void MirrorFace(EnumMirrorAlong _axis, int _sourceRot, int _targetRot, BlockFace _face, out BlockFace _sourceFace, out BlockFace _targetFace)
	{
		_sourceFace = _face;
		_targetFace = _face;
	}

	// Token: 0x06000C9C RID: 3228
	public abstract void CalculateCollisionHash(IncrementalHash calcHash);

	// Token: 0x06000C9D RID: 3229 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual int GetVertexCount()
	{
		return 0;
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual int GetTriangleCount()
	{
		return 0;
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0004E558 File Offset: 0x0004C758
	public virtual string GetName()
	{
		return string.Empty;
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool UseRepairDamageState(BlockValue _blockValue)
	{
		return false;
	}

	// Token: 0x04000B3F RID: 2879
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvZero = Vector2.zero;

	// Token: 0x04000B40 RID: 2880
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvOne = Vector2.one;

	// Token: 0x04000B41 RID: 2881
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvRightBot = new Vector2(1f, 0f);

	// Token: 0x04000B42 RID: 2882
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvLeftTop = new Vector2(0f, 1f);

	// Token: 0x04000B43 RID: 2883
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvMiddle = new Vector2(0.5f, 0.5f);

	// Token: 0x04000B44 RID: 2884
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvMidBot = new Vector2(0.5f, 0f);

	// Token: 0x04000B45 RID: 2885
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvMidTop = new Vector2(0.5f, 1f);

	// Token: 0x04000B46 RID: 2886
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvLeftMid = new Vector2(0f, 0.5f);

	// Token: 0x04000B47 RID: 2887
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector2 uvRightMid = new Vector2(1f, 0.5f);

	// Token: 0x04000B48 RID: 2888
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector4 tngRight = new Vector4(1f, 0f, 0f, 1f);

	// Token: 0x04000B49 RID: 2889
	[PublicizedFrom(EAccessModifier.Protected)]
	public Block block;

	// Token: 0x04000B4A RID: 2890
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds bounds;

	// Token: 0x04000B4B RID: 2891
	[PublicizedFrom(EAccessModifier.Protected)]
	public Bounds[] boundsArr;

	// Token: 0x04000B4C RID: 2892
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 minBounds;

	// Token: 0x04000B4D RID: 2893
	public bool IsSolidCube;

	// Token: 0x04000B4E RID: 2894
	public bool IsRotatable;

	// Token: 0x04000B4F RID: 2895
	public bool IsSolidSpace;

	// Token: 0x04000B50 RID: 2896
	public bool IsOmitTerrainSnappingUp;

	// Token: 0x04000B51 RID: 2897
	public bool IsNotifyOnLoadUnload;

	// Token: 0x04000B52 RID: 2898
	public byte LightOpacity;

	// Token: 0x04000B53 RID: 2899
	public int SymmetryType = 1;

	// Token: 0x04000B54 RID: 2900
	public bool Has45DegreeRotations;

	// Token: 0x0200019E RID: 414
	public enum MeshPurpose
	{
		// Token: 0x04000B56 RID: 2902
		World,
		// Token: 0x04000B57 RID: 2903
		Drop,
		// Token: 0x04000B58 RID: 2904
		Hold,
		// Token: 0x04000B59 RID: 2905
		Local,
		// Token: 0x04000B5A RID: 2906
		Preview,
		// Token: 0x04000B5B RID: 2907
		SimplifiedCollisionOnly
	}
}
