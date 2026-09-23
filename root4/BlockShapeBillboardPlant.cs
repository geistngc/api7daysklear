using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001A3 RID: 419
[Preserve]
public class BlockShapeBillboardPlant : BlockShapeBillboardRotatedAbstract
{
	// Token: 0x06000CAF RID: 3247 RVA: 0x0004F418 File Offset: 0x0004D618
	public BlockShapeBillboardPlant()
	{
		this.boundsArr = new Bounds[]
		{
			BoundsUtils.BoundsForMinMax(0.3f, 0f, 0.3f, 0.7f, 1f, 0.7f)
		};
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0004F464 File Offset: 0x0004D664
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createVertices()
	{
		this.vertices = new Vector3[]
		{
			new Vector3(-0.35f, -0.15f, -0.35f),
			new Vector3(-0.34f, 1.15f, -0.35f),
			new Vector3(1.36f, 1.15f, -0.35f),
			new Vector3(1.35f, -0.15f, -0.35f),
			new Vector3(-0.35f, -0.15f, 1.35f),
			new Vector3(-0.34f, 1.15f, 1.35f),
			new Vector3(1.36f, 1.15f, 1.35f),
			new Vector3(1.35f, -0.15f, 1.35f)
		};
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0004F555 File Offset: 0x0004D755
	public override Quaternion GetRotation(BlockValue _blockValue)
	{
		return Quaternion.AngleAxis((float)(20 * _blockValue.rotation), Vector3.up);
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0004F56C File Offset: 0x0004D76C
	public override void renderFull(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		byte sun = _lightingAround[LightingAround.Pos.Middle].sun;
		byte block = _lightingAround[LightingAround.Pos.Middle].block;
		Block block2 = _blockValue.Block;
		int meshIndex = (int)block2.MeshIndex;
		VoxelMesh voxelMesh = _meshes[meshIndex];
		if (meshIndex == 3)
		{
			Rect uvrectFromSideAndMetadata = block2.getUVRectFromSideAndMetadata(meshIndex, BlockFace.Top, Vector3.zero, _blockValue);
			BlockShapeBillboardPlant.RenderData renderData;
			renderData.count = 2 + MeshDescription.GrassQualityPlanes;
			renderData.count2 = 0;
			renderData.offsetY = -0.05f;
			renderData.scale = 1.25f;
			renderData.height = renderData.scale;
			renderData.sideShift = 0.04f;
			renderData.rotation = (float)(20 * _blockValue.rotation);
			BlockShapeBillboardPlant.RenderSpinMesh(voxelMesh, _drawPos, _vertices, uvrectFromSideAndMetadata, sun, block, renderData);
			BlockShapeBillboardPlant.AddCollider(voxelMesh, _drawPos, 0.85f);
			return;
		}
		Vector3[] array = base.rotateVertices(this.vertices, _drawPos, _blockValue);
		voxelMesh.AddBlockSide(array[7], array[0], array[1], array[6], _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block, meshIndex);
		voxelMesh.AddBlockSide(array[0], array[7], array[6], array[1], _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block, meshIndex);
		voxelMesh.AddBlockSide(array[3], array[4], array[5], array[2], _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block, meshIndex);
		voxelMesh.AddBlockSide(array[4], array[3], array[2], array[5], _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block, meshIndex);
		MemoryPools.poolVector3.Free(array);
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0004E50E File Offset: 0x0004C70E
	public override Bounds[] GetBounds(BlockValue _blockValue)
	{
		return this.boundsArr;
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0004F719 File Offset: 0x0004D919
	public override byte Rotate(bool _isLeft, int _rotation)
	{
		_rotation = (_rotation + (_isLeft ? -1 : 1) & 3);
		return (byte)_rotation;
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0004F72A File Offset: 0x0004D92A
	public override BlockValue RotateY(bool _bLeft, BlockValue _blockValue, int _rotCount)
	{
		_blockValue.rotation = (byte)((int)_blockValue.rotation + _rotCount & 3);
		return _blockValue;
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0004F740 File Offset: 0x0004D940
	public static void RenderSpinMesh(VoxelMesh _mesh, Vector3 _drawPos, Vector3[] _vertices, Rect uvTex, byte _sunlight, byte _blocklight, BlockShapeBillboardPlant.RenderData _data)
	{
		float num = _drawPos.y;
		if (_vertices != null)
		{
			num += _data.offsetY;
		}
		float num2 = num + _data.height;
		float num3 = _drawPos.x + 0.5f;
		float num4 = _drawPos.z + 0.5f;
		float num5 = 180f / (float)_data.count;
		num5 += 180f;
		float num6 = 0.5f * _data.scale;
		for (int i = 0; i < _data.count; i++)
		{
			float f = ((float)i * num5 + _data.rotation) * 0.017453292f;
			float num7 = Mathf.Sin(f);
			float num8 = Mathf.Cos(f);
			float num9 = -num6;
			float sideShift = _data.sideShift;
			Vector3 vector;
			vector.x = num3 + num9 * num8 - sideShift * num7;
			vector.y = num;
			vector.z = num4 + num9 * num7 + sideShift * num8;
			num9 = num6;
			Vector3 vector2;
			vector2.x = num3 + num9 * num8 - sideShift * num7;
			vector2.y = num;
			vector2.z = num4 + num9 * num7 + sideShift * num8;
			float num10 = num2;
			num10 += (float)(i % 3) * -0.15f;
			Vector3 v;
			v.x = vector2.x;
			v.y = num10;
			v.z = vector2.z;
			Vector3 vector3;
			vector3.x = vector.x;
			vector3.y = num10;
			vector3.z = vector.z;
			Vector3 vector4;
			vector4.x = num7;
			vector4.y = 0f;
			vector4.z = -num8;
			Vector4 tangent = (vector2 - vector).normalized;
			tangent.w = -1f;
			Vector3 vector5 = -vector4;
			_mesh.AddRectangle(vector, BlockShape.uvZero, vector2, BlockShape.uvRightBot, v, BlockShape.uvOne, vector3, BlockShape.uvLeftTop, vector4, vector4, tangent, uvTex, _sunlight, _blocklight);
			tangent.w = 1f;
			_mesh.AddRectangle(vector, BlockShape.uvZero, vector3, BlockShape.uvLeftTop, v, BlockShape.uvOne, vector2, BlockShape.uvRightBot, vector5, vector5, tangent, uvTex, _sunlight, _blocklight);
		}
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0004F970 File Offset: 0x0004DB70
	public static void RenderGridMesh(VoxelMesh _mesh, Vector3 _drawPos, Vector3[] _vertices, Rect uvTex, byte _sunlight, byte _blocklight, BlockShapeBillboardPlant.RenderData _data)
	{
		float num = _drawPos.y;
		if (_vertices != null)
		{
			num += _data.offsetY;
		}
		float num2 = num + _data.height - 0.12f;
		float num3 = _drawPos.x + 0.5f;
		float num4 = _drawPos.z + 0.5f;
		float num5 = _data.rotation;
		float num6 = 165f / (float)_data.count2;
		float num7 = _data.sideShift * 2f / ((float)_data.count - 0.99f);
		float num8 = _data.sideShift * 1.85f;
		Vector3 vector;
		vector.y = 0f;
		Vector3 normal;
		normal.y = 0f;
		for (int i = 0; i < _data.count2; i++)
		{
			float num9 = -_data.sideShift;
			float f = num5 * 0.017453292f;
			float num10 = Mathf.Sin(f);
			float num11 = Mathf.Cos(f);
			vector.x = -num11;
			vector.z = -num10;
			for (int j = 0; j < _data.count; j++)
			{
				float num12 = num3 - num8 * num10;
				float num13 = num4 + num8 * num11;
				Vector3 vector2;
				vector2.x = num12 + num9 * num11;
				vector2.y = num;
				vector2.z = num13 + num9 * num10;
				float num14 = num2;
				num14 += (float)(j % 3) * 0.06f;
				float num15 = num9 * 0.7f;
				Vector3 vector3;
				vector3.x = num12 + num15 * num11;
				vector3.y = num14;
				vector3.z = num13 + num15 * num10;
				num12 = num3 + num8 * num10;
				num13 = num4 - num8 * num11;
				Vector3 vector4;
				vector4.x = num12 + num9 * num11;
				vector4.y = num;
				vector4.z = num13 + num9 * num10;
				Vector3 vector5;
				vector5.x = num12 + num15 * num11;
				vector5.y = num14;
				vector5.z = num13 + num15 * num10;
				Vector3 vector6;
				vector6.x = vector.x;
				vector6.y = (num15 - num9) * 2.2f;
				vector6.z = vector.z;
				vector6 *= 1f / vector6.magnitude;
				normal.x = -vector.x;
				normal.z = -vector.z;
				Vector4 tangent = (vector4 - vector2).normalized;
				tangent.w = -1f;
				if ((j & 1) == 0)
				{
					_mesh.AddRectangle(vector2, BlockShape.uvZero, vector4, BlockShape.uvRightBot, vector5, BlockShape.uvOne, vector3, BlockShape.uvLeftTop, vector, vector6, tangent, uvTex, _sunlight, _blocklight);
					tangent.w = 1f;
					_mesh.AddRectangle(vector4, BlockShape.uvRightBot, vector2, BlockShape.uvZero, vector3, BlockShape.uvLeftTop, vector5, BlockShape.uvOne, normal, -vector6, tangent, uvTex, _sunlight, _blocklight);
				}
				else
				{
					_mesh.AddRectangle(vector2, BlockShape.uvRightBot, vector4, BlockShape.uvZero, vector5, BlockShape.uvLeftTop, vector3, BlockShape.uvOne, vector, vector6, tangent, uvTex, _sunlight, _blocklight);
					tangent.w = 1f;
					_mesh.AddRectangle(vector4, BlockShape.uvZero, vector2, BlockShape.uvRightBot, vector3, BlockShape.uvOne, vector5, BlockShape.uvLeftTop, normal, -vector6, tangent, uvTex, _sunlight, _blocklight);
				}
				num9 += num7;
			}
			num5 += num6;
		}
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x0004FCD4 File Offset: 0x0004DED4
	public static void AddCollider(VoxelMesh _mesh, Vector3 _drawPos, float _height)
	{
		float num = _drawPos.x + 0.5f;
		float num2 = _drawPos.z + 0.5f;
		float y = _drawPos.y;
		float y2 = y + _height;
		Vector3 vector;
		vector.x = num - 0.45f;
		vector.y = y;
		vector.z = num2;
		Vector3 vector2;
		vector2.x = num + 0.45f;
		vector2.y = y;
		vector2.z = num2;
		Vector3 v;
		v.x = vector2.x;
		v.y = y2;
		v.z = num2;
		Vector3 v2;
		v2.x = vector.x;
		v2.y = y2;
		v2.z = num2;
		_mesh.AddRectangleColliderPair(vector, vector2, v, v2);
		vector.x = num;
		vector.z = num2 - 0.45f;
		vector2.x = num;
		vector2.z = num2 + 0.45f;
		v.x = num;
		v.z = vector2.z;
		v2.x = num;
		v2.z = vector.z;
		_mesh.AddRectangleColliderPair(vector, vector2, v, v2);
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x0004FDF4 File Offset: 0x0004DFF4
	public override void CalculateCollisionHash(IncrementalHash calcHash)
	{
		int value = 1;
		calcHash.AppendDataNoAlloc(value);
		if (this.vertices != null)
		{
			foreach (Vector3 vector in this.vertices)
			{
				calcHash.AppendDataNoAlloc(vector.x);
				calcHash.AppendDataNoAlloc(vector.y);
				calcHash.AppendDataNoAlloc(vector.z);
			}
		}
	}

	// Token: 0x04000B62 RID: 2914
	[PublicizedFrom(EAccessModifier.Private)]
	public const float xzAdd = 0.35f;

	// Token: 0x04000B63 RID: 2915
	[PublicizedFrom(EAccessModifier.Private)]
	public const float yAdd = 0.15f;

	// Token: 0x020001A4 RID: 420
	public struct RenderData
	{
		// Token: 0x04000B64 RID: 2916
		public int count;

		// Token: 0x04000B65 RID: 2917
		public int count2;

		// Token: 0x04000B66 RID: 2918
		public float height;

		// Token: 0x04000B67 RID: 2919
		public float rotation;

		// Token: 0x04000B68 RID: 2920
		public float scale;

		// Token: 0x04000B69 RID: 2921
		public float sideShift;

		// Token: 0x04000B6A RID: 2922
		public float offsetY;
	}
}
