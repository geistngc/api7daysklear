using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001AF RID: 431
[Preserve]
public class BlockShapeNew : BlockShape
{
	// Token: 0x06000CFE RID: 3326 RVA: 0x00051F81 File Offset: 0x00050181
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ThreadDataInit()
	{
		if (BlockShapeNew.threadData.trisXPos == null)
		{
			BlockShapeNew.threadData.Init();
		}
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x00051F99 File Offset: 0x00050199
	public BlockShapeNew()
	{
		this.IsSolidCube = false;
		this.IsSolidSpace = false;
		this.IsRotatable = true;
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x00051FB8 File Offset: 0x000501B8
	public override void Init(Block _block)
	{
		this.ShapeName = _block.Properties.GetValue("Model");
		if (this.ShapeName == null)
		{
			throw new Exception("No model specified on block with name " + _block.GetBlockName());
		}
		Vector3 vector = new Vector3(1f, 0f, 1f);
		_block.Properties.ParseVec("ModelOffset", ref vector);
		BlockShapeNew.MeshData meshData;
		if (!BlockShapeNew.meshData.TryGetValue(this.ShapeName, out meshData))
		{
			meshData = new BlockShapeNew.MeshData();
			BlockShapeNew.meshData.Add(this.ShapeName, meshData);
		}
		BlockShapeNew.MeshData.Arrays arrays;
		if (!meshData.posArrays.TryGetValue(vector, out arrays))
		{
			if (!meshData.obj)
			{
				GameObject gameObject = DataLoader.LoadAsset<GameObject>(this.ShapeName ?? "", false);
				if (!gameObject)
				{
					throw new Exception("Model with name " + this.ShapeName + " not found");
				}
				meshData.obj = gameObject;
			}
			arrays = new BlockShapeNew.MeshData.Arrays();
			meshData.posArrays.Add(vector, arrays);
			this.ParseModel(meshData, arrays, vector);
		}
		this.visualMeshes = arrays.meshes;
		this.colliderMeshes = arrays.colliderMeshes;
		this.faceInfo = arrays.faceInfo;
		this.boundsRotations = arrays.boundsRotations;
		if (meshData.symTypeOverride != -1)
		{
			this.SymmetryType = meshData.symTypeOverride;
		}
		this.IsSolidCube = meshData.IsSolidCube;
		if (_block.PathType < 0)
		{
			this.boundsPathOffsetRotations = new Vector2[32];
			for (int i = 0; i < 28; i++)
			{
				Bounds bounds = this.boundsRotations[i];
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				Vector2 vector2;
				vector2.x = 0f;
				float num = min.x + BlockShapeNew.centerOffsetV.x;
				if (num >= -0.01f)
				{
					vector2.x = (-0.5f + num) * 0.5f;
				}
				num = max.x + BlockShapeNew.centerOffsetV.x;
				if (num <= 0.01f)
				{
					vector2.x = (0.5f + num) * 0.5f;
				}
				vector2.y = 0f;
				float num2 = min.z + BlockShapeNew.centerOffsetV.z;
				if (num2 >= -0.01f)
				{
					vector2.y = (-0.5f + num2) * 0.5f;
				}
				num2 = max.z + BlockShapeNew.centerOffsetV.z;
				if (num2 <= 0.01f)
				{
					vector2.y = (0.5f + num2) * 0.5f;
				}
				if (vector2.x != 0f || vector2.y != 0f)
				{
					_block.PathType = -1;
					this.boundsPathOffsetRotations[i] = vector2;
				}
			}
		}
		base.Init(_block);
		if (meshData.obj)
		{
			foreach (MeshRenderer meshRenderer in meshData.obj.GetComponentsInChildren<MeshRenderer>())
			{
				Material sharedMaterial = meshRenderer.sharedMaterial;
				if (sharedMaterial != null)
				{
					meshRenderer.sharedMaterial = null;
					Resources.UnloadAsset(sharedMaterial);
				}
			}
			MeshFilter[] componentsInChildren2 = meshData.obj.GetComponentsInChildren<MeshFilter>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Mesh sharedMesh = componentsInChildren2[j].sharedMesh;
				if (sharedMesh != null)
				{
					meshData.obj = null;
					Resources.UnloadAsset(sharedMesh);
				}
			}
		}
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x00052318 File Offset: 0x00050518
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParseModel(BlockShapeNew.MeshData _data, BlockShapeNew.MeshData.Arrays _arrays, Vector3 _modelOffset)
	{
		if (BlockShapeNew.convertRotationCached == null)
		{
			BlockShapeNew.convertRotationCached = new int[32, 7];
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					BlockShapeNew.convertRotationCached[i, j] = BlockShapeNew.convertRotation((BlockFace)j, i);
				}
			}
		}
		Transform transform = _data.obj.transform;
		for (int k = 0; k < transform.childCount; k++)
		{
			Transform child = transform.GetChild(k);
			string name = child.name;
			if (name == "Solid")
			{
				_data.IsSolidCube = true;
			}
			else if (name == "LOD0")
			{
				for (int l = 0; l < child.childCount; l++)
				{
					Transform child2 = child.GetChild(l);
					string name2 = child2.name;
					int num = this.CharToFaceIndex(name2[0]);
					if (num != -1)
					{
						_arrays.meshes[num] = this.CreateMeshFromMeshFilter(child2, _modelOffset, false);
						if (child2.childCount > 0)
						{
							Log.Error(string.Format("{0} face \"{1}\" has 1 base mesh and {2} submeshes, exceeding the total limit of {3} meshes per face.", new object[]
							{
								_data.obj.name,
								name2,
								child2.childCount,
								1
							}));
						}
						int num2 = 0;
						while (num2 < child2.childCount && num2 < 0)
						{
							Transform child3 = child2.GetChild(num2);
							_arrays.meshes[num + (num2 + 1) * 7] = this.CreateMeshFromMeshFilter(child3, _modelOffset, false);
							num2++;
						}
						if (name2.Length > 2)
						{
							for (int m = 2; m < name2.Length; m++)
							{
								char c = name2[m];
								if (c == 'F')
								{
									_arrays.faceInfo[num] = BlockShapeNew.EnumFaceOcclusionInfo.Full;
								}
								else if (c == 'P')
								{
									_arrays.faceInfo[num] = BlockShapeNew.EnumFaceOcclusionInfo.Part;
								}
								else if (c == 'A')
								{
									_arrays.faceInfo[num] = BlockShapeNew.EnumFaceOcclusionInfo.Remove;
								}
								else if (c == 'C')
								{
									_arrays.faceInfo[num] = BlockShapeNew.EnumFaceOcclusionInfo.Continuous;
								}
								else if (c == 'Y')
								{
									_arrays.faceInfo[num] = BlockShapeNew.EnumFaceOcclusionInfo.RemoveIfAny;
								}
								else if (c == 'O')
								{
									_arrays.faceInfo[num] = BlockShapeNew.EnumFaceOcclusionInfo.OwnFaces;
								}
								else if (c == 'H')
								{
									_arrays.faceInfo[num] = BlockShapeNew.EnumFaceOcclusionInfo.HideIfSame;
								}
								else if (c == 'T')
								{
									_arrays.faceInfo[num] = BlockShapeNew.EnumFaceOcclusionInfo.Transparent;
								}
							}
						}
					}
				}
			}
			else if (name == "Collider")
			{
				for (int n = 0; n < child.childCount; n++)
				{
					Transform child4 = child.GetChild(n);
					string name3 = child4.name;
					int num3 = this.CharToFaceIndex(name3[0]);
					if (num3 != -1)
					{
						_arrays.colliderMeshes[num3] = this.CreateMeshFromMeshFilter(child4, _modelOffset, true);
					}
				}
			}
			else if (name == "SymType_0")
			{
				_data.symTypeOverride = 0;
			}
			else if (name == "SymType_2")
			{
				_data.symTypeOverride = 2;
			}
			else if (name == "SymType_3")
			{
				_data.symTypeOverride = 3;
			}
			else if (name == "SymType_4")
			{
				_data.symTypeOverride = 4;
			}
		}
		this.CalcBounds(_arrays);
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00052638 File Offset: 0x00050838
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcBounds(BlockShapeNew.MeshData.Arrays _arrays)
	{
		Bounds bounds = default(Bounds);
		for (int i = 0; i < 28; i++)
		{
			Quaternion rotation = BlockShapeNew.rotationsToQuats[i];
			Vector3 vector = Vector3.positiveInfinity;
			Vector3 vector2 = Vector3.negativeInfinity;
			for (int j = 0; j < 6; j++)
			{
				BlockShapeNew.MySimpleMesh mySimpleMesh = _arrays.meshes[j];
				if (mySimpleMesh != null)
				{
					List<Vector3> vertices = mySimpleMesh.Vertices;
					int count = vertices.Count;
					for (int k = 0; k < count; k++)
					{
						Vector3 vector3 = rotation * (vertices[k] + BlockShapeNew.centerOffsetV);
						if (vector3.x < vector.x)
						{
							vector.x = vector3.x;
						}
						if (vector3.x > vector2.x)
						{
							vector2.x = vector3.x;
						}
						if (vector3.y < vector.y)
						{
							vector.y = vector3.y;
						}
						if (vector3.y > vector2.y)
						{
							vector2.y = vector3.y;
						}
						if (vector3.z < vector.z)
						{
							vector.z = vector3.z;
						}
						if (vector3.z > vector2.z)
						{
							vector2.z = vector3.z;
						}
					}
				}
			}
			vector -= BlockShapeNew.centerOffsetV;
			vector2 -= BlockShapeNew.centerOffsetV;
			bounds.SetMinMax(vector, vector2);
			bounds.extents = new Vector3(Utils.FastMax(bounds.extents.x, 0.1f), Utils.FastMax(bounds.extents.y, 0.1f), Utils.FastMax(bounds.extents.z, 0.1f));
			_arrays.boundsRotations[i] = bounds;
		}
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00052815 File Offset: 0x00050A15
	public static void Cleanup()
	{
		BlockShapeNew.meshData.Clear();
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00052824 File Offset: 0x00050A24
	[PublicizedFrom(EAccessModifier.Private)]
	public int CharToFaceIndex(char _c)
	{
		if (_c <= 'E')
		{
			if (_c == 'B')
			{
				return 1;
			}
			if (_c == 'E')
			{
				return 5;
			}
		}
		else
		{
			if (_c == 'M')
			{
				return 6;
			}
			if (_c == 'N')
			{
				return 2;
			}
			switch (_c)
			{
			case 'S':
				return 4;
			case 'T':
				return 0;
			case 'W':
				return 3;
			}
		}
		return -1;
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x0005287C File Offset: 0x00050A7C
	[PublicizedFrom(EAccessModifier.Private)]
	public static int convertRotation(BlockFace _face, int _rotation)
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2;
		switch (_face)
		{
		case BlockFace.Top:
			vector2 = Vector3.up;
			break;
		case BlockFace.Bottom:
			vector2 = Vector3.down;
			break;
		case BlockFace.North:
			vector2 = Vector3.forward;
			break;
		case BlockFace.West:
			vector2 = Vector3.left;
			break;
		case BlockFace.South:
			vector2 = Vector3.back;
			break;
		case BlockFace.East:
			vector2 = Vector3.right;
			break;
		default:
			vector2 = Vector3.zero;
			break;
		}
		Quaternion rotation = Quaternion.Inverse(BlockShapeNew.rotationsToQuats[_rotation]);
		vector = rotation * vector;
		vector2 = rotation * vector2;
		Vector3 vector3 = vector2 - vector;
		int result;
		if (vector3.x > 0.9f)
		{
			result = 5;
		}
		else if (vector3.x < -0.9f)
		{
			result = 3;
		}
		else if (vector3.y > 0.9f)
		{
			result = 0;
		}
		else if (vector3.y < -0.9f)
		{
			result = 1;
		}
		else if (vector3.z > 0.9f)
		{
			result = 2;
		}
		else if (vector3.z < -0.9f)
		{
			result = 4;
		}
		else
		{
			result = (int)_face;
		}
		return result;
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x00052978 File Offset: 0x00050B78
	[PublicizedFrom(EAccessModifier.Private)]
	public void convertRotationUVDirs(BlockFace _face, int _rotation, out Vector3 _dX, out Vector3 _dY)
	{
		Vector3 point;
		Vector3 point2;
		switch (_face)
		{
		case BlockFace.Top:
			point = Vector3.right;
			point2 = Vector3.forward;
			break;
		case BlockFace.Bottom:
			point = Vector3.left;
			point2 = Vector3.forward;
			break;
		case BlockFace.North:
			point = Vector3.left;
			point2 = Vector3.up;
			break;
		case BlockFace.West:
			point = Vector3.back;
			point2 = Vector3.up;
			break;
		case BlockFace.South:
			point = Vector3.right;
			point2 = Vector3.up;
			break;
		case BlockFace.East:
			point = Vector3.forward;
			point2 = Vector3.up;
			break;
		case BlockFace.Middle:
			point = Vector3.left;
			point2 = Vector3.up;
			break;
		default:
			point = Vector3.zero;
			point2 = Vector3.zero;
			break;
		}
		Quaternion rotation = BlockShapeNew.rotationsToQuats[_rotation];
		_dX = rotation * point;
		_dY = rotation * point2;
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x00052A40 File Offset: 0x00050C40
	public static int ConvertRotationFree(int _rotation, Quaternion _q, bool _bApplyRotFirst = false)
	{
		Vector3 vector = new Vector3(-0.5f, -0.5f, -0.5f);
		Vector3 vector2 = Vector3.up;
		Quaternion rotationStatic = BlockShapeNew.GetRotationStatic(_rotation);
		if (_bApplyRotFirst)
		{
			vector = _q * vector;
			vector2 = _q * vector2;
			vector = rotationStatic * vector;
			vector2 = rotationStatic * vector2;
		}
		else
		{
			vector = rotationStatic * vector;
			vector2 = rotationStatic * vector2;
			vector = _q * vector;
			vector2 = _q * vector2;
		}
		vector.x += 0.5f;
		vector.y += 0.5f;
		vector.z += 0.5f;
		Vector3i vector3i = new Vector3i(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
		Vector3i vector3i2 = new Vector3i(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y), Mathf.RoundToInt(vector2.z));
		if (vector3i2.x == 0)
		{
			if (vector3i2.z == 0)
			{
				if (vector3i2.y == 1)
				{
					if (vector3i.y == 0)
					{
						int num = BlockShapeNew.BlockFaceToRot(BlockFace.Top);
						if (vector3i.x == 0)
						{
							if (vector3i.z == 0)
							{
								return num;
							}
							return num + 1;
						}
						else
						{
							if (vector3i.z == 1)
							{
								return num + 2;
							}
							return num + 3;
						}
					}
				}
				else if (vector3i.y == 1)
				{
					int num2 = BlockShapeNew.BlockFaceToRot(BlockFace.Bottom);
					if (vector3i.x == 1)
					{
						if (vector3i.z == 0)
						{
							return num2;
						}
						return num2 + 1;
					}
					else
					{
						if (vector3i.z == 1)
						{
							return num2 + 2;
						}
						return num2 + 3;
					}
				}
			}
			if (vector3i2.y == 0)
			{
				if (vector3i2.z == 1)
				{
					if (vector3i.z == 0)
					{
						int num3 = BlockShapeNew.BlockFaceToRot(BlockFace.North);
						if (vector3i.x == 1)
						{
							if (vector3i.y == 0)
							{
								return num3;
							}
							return num3 + 1;
						}
						else
						{
							if (vector3i.y == 1)
							{
								return num3 + 2;
							}
							return num3 + 3;
						}
					}
				}
				else if (vector3i.z == 1)
				{
					int num4 = BlockShapeNew.BlockFaceToRot(BlockFace.South);
					if (vector3i.x == 0)
					{
						if (vector3i.y == 0)
						{
							return num4;
						}
						return num4 + 1;
					}
					else
					{
						if (vector3i.y == 1)
						{
							return num4 + 2;
						}
						return num4 + 3;
					}
				}
			}
		}
		else if (vector3i2.y == 0 && vector3i2.z == 0)
		{
			if (vector3i2.x == -1)
			{
				if (vector3i.x == 1)
				{
					int num5 = BlockShapeNew.BlockFaceToRot(BlockFace.West);
					if (vector3i.y == 0)
					{
						if (vector3i.z == 0)
						{
							return num5;
						}
						return num5 + 1;
					}
					else
					{
						if (vector3i.z == 1)
						{
							return num5 + 2;
						}
						return num5 + 3;
					}
				}
			}
			else if (vector3i.x == 0)
			{
				int num6 = BlockShapeNew.BlockFaceToRot(BlockFace.East);
				if (vector3i.y == 1)
				{
					if (vector3i.z == 0)
					{
						return num6;
					}
					return num6 + 1;
				}
				else
				{
					if (vector3i.z == 1)
					{
						return num6 + 2;
					}
					return num6 + 3;
				}
			}
		}
		return 0;
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x00052D00 File Offset: 0x00050F00
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockShapeNew.MySimpleMesh CreateMeshFromMeshFilter(Transform _transform, Vector3 _modelOffset, bool _snap = false)
	{
		MeshFilter component = _transform.GetComponent<MeshFilter>();
		if (component == null)
		{
			return null;
		}
		Mesh sharedMesh = component.sharedMesh;
		if (!sharedMesh.isReadable)
		{
			Log.Error("Mesh '" + sharedMesh.name + "' not readable in shape with Model=" + this.ShapeName);
			return null;
		}
		BlockShapeNew.MySimpleMesh mySimpleMesh = new BlockShapeNew.MySimpleMesh();
		Matrix4x4 localToWorldMatrix = _transform.localToWorldMatrix;
		localToWorldMatrix.m03 += _modelOffset.x;
		localToWorldMatrix.m13 += _modelOffset.y;
		localToWorldMatrix.m23 += _modelOffset.z;
		sharedMesh.GetVertices(mySimpleMesh.Vertices);
		int count = mySimpleMesh.Vertices.Count;
		for (int i = 0; i < count; i++)
		{
			Vector3 vector = mySimpleMesh.Vertices[i];
			vector = localToWorldMatrix.MultiplyPoint3x4(vector);
			if (_snap)
			{
				vector.x = Mathf.Round(vector.x * 1000f) / 1000f;
				vector.y = Mathf.Round(vector.y * 1000f) / 1000f;
				vector.z = Mathf.Round(vector.z * 1000f) / 1000f;
			}
			mySimpleMesh.Vertices[i] = vector;
		}
		if (sharedMesh.subMeshCount == 1)
		{
			sharedMesh.GetTriangles(mySimpleMesh.Indices, 0, true);
		}
		else
		{
			int[] triangles = sharedMesh.triangles;
			for (int j = 0; j < triangles.Length; j++)
			{
				mySimpleMesh.Indices.Add((ushort)triangles[j]);
			}
		}
		sharedMesh.GetNormals(mySimpleMesh.Normals);
		int count2 = mySimpleMesh.Normals.Count;
		for (int k = 0; k < count2; k++)
		{
			Vector3 vector2 = mySimpleMesh.Normals[k];
			vector2 = localToWorldMatrix.MultiplyVector(vector2);
			mySimpleMesh.Normals[k] = vector2.normalized;
		}
		sharedMesh.GetUVs(0, mySimpleMesh.Uvs);
		return mySimpleMesh;
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x00052EF4 File Offset: 0x000510F4
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		int num = 0;
		for (int i = 0; i < this.faceInfo.Length; i++)
		{
			int num2 = BlockShapeNew.convertRotationCached[(int)_blockValue.rotation, i];
			if (this.faceInfo[num2] == BlockShapeNew.EnumFaceOcclusionInfo.Full)
			{
				num |= 1 << i;
			}
		}
		return num;
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x00052F40 File Offset: 0x00051140
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		if (_blockValue.ischild)
		{
			return false;
		}
		int num = BlockShapeNew.convertRotationCached[(int)_blockValue.rotation, (int)_face];
		if (this.visualMeshes[num] == null)
		{
			return false;
		}
		Block block = _blockValue.Block;
		switch (this.faceInfo[num])
		{
		case BlockShapeNew.EnumFaceOcclusionInfo.None:
			return false;
		case BlockShapeNew.EnumFaceOcclusionInfo.Part:
		{
			BlockShapeNew blockShapeNew;
			if (!_adjBlockValue.ischild && block.MeshIndex == _adjBlockValue.Block.MeshIndex && (blockShapeNew = (_adjBlockValue.Block.shape as BlockShapeNew)) != null)
			{
				int num2 = BlockShapeNew.convertRotationCached[(int)_adjBlockValue.rotation, (int)BlockFaceFlags.OppositeFace(_face)];
				if (_adjBlockValue.rotation == _blockValue.rotation && blockShapeNew.ShapeName == this.ShapeName && blockShapeNew.faceInfo[num2] == BlockShapeNew.EnumFaceOcclusionInfo.Part)
				{
					return false;
				}
				if (blockShapeNew.faceInfo[num2] == BlockShapeNew.EnumFaceOcclusionInfo.Full)
				{
					return false;
				}
			}
			break;
		}
		case BlockShapeNew.EnumFaceOcclusionInfo.Remove:
		{
			BlockShapeNew blockShapeNew2 = _adjBlockValue.Block.shape as BlockShapeNew;
			if (blockShapeNew2 != null)
			{
				int num3 = BlockShapeNew.convertRotationCached[(int)_adjBlockValue.rotation, (int)BlockFaceFlags.OppositeFace(_face)];
				if (blockShapeNew2.faceInfo[num3] == BlockShapeNew.EnumFaceOcclusionInfo.Remove)
				{
					return false;
				}
			}
			return true;
		}
		case BlockShapeNew.EnumFaceOcclusionInfo.Continuous:
		{
			BlockShapeNew blockShapeNew3;
			if (!_adjBlockValue.ischild && block.MeshIndex == _adjBlockValue.Block.MeshIndex && (blockShapeNew3 = (_adjBlockValue.Block.shape as BlockShapeNew)) != null)
			{
				int num4 = BlockShapeNew.convertRotationCached[(int)_adjBlockValue.rotation, (int)BlockFaceFlags.OppositeFace(_face)];
				if (blockShapeNew3.faceInfo[num4] == BlockShapeNew.EnumFaceOcclusionInfo.Full)
				{
					return false;
				}
			}
			break;
		}
		case BlockShapeNew.EnumFaceOcclusionInfo.RemoveIfAny:
			if (!_adjBlockValue.isair)
			{
				return false;
			}
			break;
		case BlockShapeNew.EnumFaceOcclusionInfo.OwnFaces:
			if (_adjBlockValue.type == _blockValue.type)
			{
				return false;
			}
			break;
		case BlockShapeNew.EnumFaceOcclusionInfo.HideIfSame:
			if (_adjBlockValue.type == _blockValue.type && _adjBlockValue.rotation == _blockValue.rotation)
			{
				return false;
			}
			break;
		}
		return true;
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00053131 File Offset: 0x00051331
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockShapeNew.MySimpleMesh getVisualMesh(int _idx)
	{
		return this.visualMeshes[_idx];
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x0005313B File Offset: 0x0005133B
	[PublicizedFrom(EAccessModifier.Private)]
	public bool AbsEpsilonEqual(float A, float B, float epsilon = 0.0001f)
	{
		return Mathf.Abs(A) <= Mathf.Abs(B) + epsilon && Mathf.Abs(A) >= Mathf.Abs(B) - epsilon;
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00053164 File Offset: 0x00051364
	[PublicizedFrom(EAccessModifier.Private)]
	public static int FloorModInt(int a, int m)
	{
		if (m <= 0)
		{
			return 0;
		}
		int num = a % m;
		if (num < 0)
		{
			num += m;
		}
		return num;
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00053184 File Offset: 0x00051384
	[PublicizedFrom(EAccessModifier.Private)]
	public static int FloorModFloat(float x, int m)
	{
		return BlockShapeNew.FloorModInt(Mathf.FloorToInt(x), m);
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00053194 File Offset: 0x00051394
	[PublicizedFrom(EAccessModifier.Private)]
	public static BlockShapeNew.PlanePick ClassifyTriPlane(BlockShapeNew.MySimpleMesh mesh, int triStart, Quaternion q)
	{
		int index = (int)mesh.Indices[triStart];
		int index2 = (int)mesh.Indices[triStart + 1];
		int index3 = (int)mesh.Indices[triStart + 2];
		Vector3 b = q * mesh.Vertices[index];
		Vector3 a = q * mesh.Vertices[index2];
		Vector3 a2 = q * mesh.Vertices[index3];
		Vector3 a3 = Vector3.Cross(a - b, a2 - b);
		float magnitude = a3.magnitude;
		if (magnitude < 1E-09f)
		{
			return new BlockShapeNew.PlanePick
			{
				axis = BlockShapeNew.PlaneAxis.Y,
				neg = false
			};
		}
		Vector3 vector = a3 / magnitude;
		float num = Mathf.Abs(vector.x);
		Mathf.Abs(vector.y);
		float num2 = Mathf.Abs(vector.z);
		if (num > 0.25f && num - num2 - 0.001f > 0f)
		{
			return new BlockShapeNew.PlanePick
			{
				axis = BlockShapeNew.PlaneAxis.X,
				neg = (vector.x < 0f)
			};
		}
		if (num2 > 0.25f)
		{
			return new BlockShapeNew.PlanePick
			{
				axis = BlockShapeNew.PlaneAxis.Z,
				neg = (vector.z < 0f)
			};
		}
		return new BlockShapeNew.PlanePick
		{
			axis = BlockShapeNew.PlaneAxis.Y,
			neg = (vector.y < 0f)
		};
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00053318 File Offset: 0x00051518
	[PublicizedFrom(EAccessModifier.Private)]
	public void AppendSubsetTriPlanar(VoxelMesh targetMesh, BlockShapeNew.MySimpleMesh src, List<int> triIdx, Quaternion q, Vector3 sum1, Vector3 sum2, int texArrayIndex, int blockW, int blockH, int xTiling, int yTiling, Vector3 drawPos, LightingAround lighting, Rect cracksRect, bool imposterActive)
	{
		if (triIdx == null || triIdx.Count == 0)
		{
			return;
		}
		BlockShapeNew.threadData.subsetUsed.Clear();
		BlockShapeNew.threadData.subsetUsedList.Clear();
		for (int i = 0; i < triIdx.Count; i++)
		{
			int index = triIdx[i];
			int num = (int)src.Indices[index];
			if (!BlockShapeNew.threadData.subsetUsed.ContainsKey(num))
			{
				BlockShapeNew.threadData.subsetUsed[num] = BlockShapeNew.threadData.subsetUsedList.Count;
				BlockShapeNew.threadData.subsetUsedList.Add(num);
			}
		}
		int count = BlockShapeNew.threadData.subsetUsedList.Count;
		targetMesh.CheckVertexLimit(count);
		if (count + targetMesh.m_Vertices.Count > 786432)
		{
			return;
		}
		int num2 = targetMesh.m_Vertices.Alloc(count);
		targetMesh.m_Normals.Alloc(count);
		targetMesh.m_ColorVertices.Alloc(count);
		Color value;
		value.g = (float)texArrayIndex;
		value.a = 1f;
		value.b = (float)lighting[LightingAround.Pos.Middle].stability / 15f;
		for (int j = 0; j < count; j++)
		{
			int index2 = BlockShapeNew.threadData.subsetUsedList[j];
			int idx = num2 + j;
			Vector3 vector = q * (src.Vertices[index2] + sum1) + sum2;
			Vector3 value2 = q * src.Normals[index2];
			targetMesh.m_Vertices[idx] = vector;
			targetMesh.m_Normals[idx] = value2;
			Vector3 vector2 = vector - drawPos;
			float num3 = (float)lighting[LightingAround.Pos.X0Y0Z0].sun * (1f - vector2.x) * (1f - vector2.y) * (1f - vector2.z) + (float)lighting[LightingAround.Pos.X1Y0Z0].sun * vector2.x * (1f - vector2.y) * (1f - vector2.z) + (float)lighting[LightingAround.Pos.X0Y0Z1].sun * (1f - vector2.x) * (1f - vector2.y) * vector2.z + (float)lighting[LightingAround.Pos.X1Y0Z1].sun * vector2.x * (1f - vector2.y) * vector2.z + (float)lighting[LightingAround.Pos.X0Y1Z0].sun * (1f - vector2.x) * vector2.y * (1f - vector2.z) + (float)lighting[LightingAround.Pos.X0Y1Z1].sun * (1f - vector2.x) * vector2.y * vector2.z + (float)lighting[LightingAround.Pos.X1Y1Z0].sun * vector2.x * vector2.y * (1f - vector2.z) + (float)lighting[LightingAround.Pos.X1Y1Z1].sun * vector2.x * vector2.y * vector2.z;
			value.r = num3 / 15f;
			targetMesh.m_ColorVertices[idx] = value;
		}
		int curTriangleIndex = targetMesh.CurTriangleIndex;
		int count2 = triIdx.Count;
		int num4 = targetMesh.m_Indices.Alloc(count2);
		for (int k = 0; k < count2; k++)
		{
			int index3 = triIdx[k];
			int key = (int)src.Indices[index3];
			int num5 = BlockShapeNew.threadData.subsetUsed[key];
			targetMesh.m_Indices[num4 + k] = curTriangleIndex + num5;
		}
		targetMesh.CurTriangleIndex += count;
		int num6 = targetMesh.m_Uvs.Alloc(count);
		Vector2 value3 = new Vector2((float)blockW / (float)Mathf.Max(1, xTiling), (float)blockH / (float)Mathf.Max(1, yTiling));
		for (int l = 0; l < count; l++)
		{
			targetMesh.m_Uvs[num6 + l] = value3;
		}
		if (targetMesh.UvsCrack != null)
		{
			int num7 = targetMesh.UvsCrack.Alloc(count);
			for (int m = 0; m < count; m++)
			{
				int index4 = BlockShapeNew.threadData.subsetUsedList[m];
				Vector2 value4;
				if (!BlockShapeNew.bImposterGenerationActive)
				{
					value4.x = cracksRect.x + src.Uvs[index4].x * cracksRect.width;
					value4.y = cracksRect.y + src.Uvs[index4].y * cracksRect.height;
				}
				else
				{
					value4.x = (float)texArrayIndex;
					value4.y = 1f;
				}
				targetMesh.UvsCrack[num7 + m] = value4;
			}
		}
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00053808 File Offset: 0x00051A08
	[PublicizedFrom(EAccessModifier.Private)]
	public int ComputeTaIndexOffsFor(BlockShapeNew.PlaneAxis plane, UVRectTiling uvTexRect, Vector3 uvPos, bool flipU)
	{
		int num = Mathf.Max(1, (int)uvTexRect.uv.width);
		int num2 = Mathf.Max(1, (int)uvTexRect.uv.height);
		float x;
		float x2;
		if (plane != BlockShapeNew.PlaneAxis.X)
		{
			if (plane == BlockShapeNew.PlaneAxis.Y)
			{
				x = uvPos.x;
				x2 = uvPos.z;
			}
			else
			{
				x = uvPos.x;
				x2 = uvPos.y;
			}
		}
		else
		{
			x = uvPos.z;
			x2 = uvPos.y;
		}
		int num3 = BlockShapeNew.FloorModFloat(x, num);
		int num4 = BlockShapeNew.FloorModFloat(x2, num2);
		if (flipU)
		{
			num3 = num - 1 - num3;
		}
		num4 = num2 - 1 - num4;
		return num3 + num4 * num;
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x000538A0 File Offset: 0x00051AA0
	public override void renderFace(Vector3i _chunkPos, BlockValue _blockValue, Vector3 _drawPos, BlockFace _face, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		this.ThreadDataInit();
		int num = BlockShapeNew.convertRotationCached[(int)_blockValue.rotation, (int)_face];
		Block block = _blockValue.Block;
		int num2;
		if (this.faceInfo[num] == BlockShapeNew.EnumFaceOcclusionInfo.Transparent)
		{
			num2 = 2;
			_face = BlockFace.North;
		}
		else
		{
			num2 = (int)block.MeshIndex;
		}
		VoxelMesh voxelMesh = _meshes[num2];
		if (voxelMesh == null)
		{
			return;
		}
		MeshDescription meshDescription = MeshDescription.meshes[num2];
		bool bTextureArray = meshDescription.bTextureArray;
		Vector3 vector = _drawPos;
		if (_purpose == BlockShape.MeshPurpose.Preview)
		{
			vector.x += (float)(_chunkPos.x + 1);
			vector.y += (float)(_chunkPos.y + 1);
			vector.z += (float)(_chunkPos.z + 1);
		}
		Quaternion quaternion = BlockShapeNew.rotationsToQuats[(int)_blockValue.rotation];
		Vector3 vector2 = BlockShapeNew.centerOffsetV;
		Vector3 vector3 = _drawPos - BlockShapeNew.centerOffsetV + this.GetRotationOffset(_blockValue);
		if (voxelMesh.m_CollVertices != null)
		{
			if (this.colliderMeshes[num] != null)
			{
				BlockShapeNew.<renderFace>g__CopyColliderMesh|38_0(voxelMesh, quaternion, vector2, vector3, this.colliderMeshes[num]);
			}
			else
			{
				for (int i = 0; i < 1; i++)
				{
					int num3 = i * 7;
					BlockShapeNew.MySimpleMesh mySimpleMesh = this.visualMeshes[num3 + num];
					if (mySimpleMesh != null)
					{
						BlockShapeNew.<renderFace>g__CopyColliderMesh|38_0(voxelMesh, quaternion, vector2, vector3, mySimpleMesh);
					}
				}
			}
		}
		if (_purpose == BlockShape.MeshPurpose.SimplifiedCollisionOnly)
		{
			return;
		}
		for (int j = 0; j < 1; j++)
		{
			int num4 = j * 7;
			BlockShapeNew.MySimpleMesh visualMesh = this.getVisualMesh(num + num4);
			if (visualMesh != null)
			{
				int count = visualMesh.Vertices.Count;
				voxelMesh.CheckVertexLimit(count);
				if (count + voxelMesh.m_Vertices.Count <= 786432)
				{
					int num5 = 0;
					if (num2 == 0)
					{
						int num6 = Chunk.Value64FullToIndex(_textureFullArray[j], (BlockFace)num);
						BlockTextureData blockTextureData = BlockTextureData.list[num6];
						if (blockTextureData == null)
						{
							if (!DynamicMeshBlockSwap.InvalidPaintIds.Contains(num6))
							{
								DynamicMeshBlockSwap.InvalidPaintIds.Add(num6);
								Log.Out(string.Concat(new string[]
								{
									"Missing paint ID XML entry: ",
									num6.ToString(),
									" for block '",
									block.GetBlockName(),
									"'"
								}));
							}
						}
						else
						{
							num5 = (int)blockTextureData.TextureID;
						}
					}
					int num7 = (num5 == 0) ? block.GetSideTextureId(_blockValue, (BlockFace)num, j) : num5;
					if ((ulong)num7 < (ulong)((long)meshDescription.textureAtlas.uvMapping.Length))
					{
						UVRectTiling uvrectTiling = meshDescription.textureAtlas.uvMapping[num7];
						if (uvrectTiling.blockW == 0 || uvrectTiling.blockH == 0)
						{
							Log.Error("Block with name '{0}' uses a texture id {1} that is not in the atlas!", new object[]
							{
								block.GetBlockName(),
								num7
							});
						}
						else
						{
							int num8 = uvrectTiling.blockW;
							int num9 = uvrectTiling.blockH;
							bool flag = uvrectTiling.bGlobalUV;
							Block.UVMode uvmode = block.GetUVMode(num, j);
							if (uvmode != Block.UVMode.Global)
							{
								if (uvmode == Block.UVMode.Local)
								{
									flag = false;
								}
							}
							else
							{
								flag = true;
							}
							flag &= (_purpose != BlockShape.MeshPurpose.Local);
							Vector3 vector4;
							Vector3 vector5;
							this.convertRotationUVDirs((BlockFace)num, (int)_blockValue.rotation, out vector4, out vector5);
							float num10 = 0f;
							float num11 = 0f;
							float x = 0f;
							float x2 = 0f;
							if (!flag || !bTextureArray)
							{
								switch (_face)
								{
								case BlockFace.Top:
								case BlockFace.Bottom:
									if (vector4.x < -0.9f || vector4.x > 0.9f)
									{
										num10 = vector4.x;
										x = vector.x;
									}
									else
									{
										num10 = vector4.z;
										x = vector.z;
									}
									if (vector5.z < -0.9f || vector5.z > 0.9f)
									{
										num11 = vector5.z;
										x2 = vector.z;
									}
									else
									{
										num11 = vector5.x;
										x2 = vector.x;
									}
									break;
								case BlockFace.North:
								case BlockFace.South:
									if (vector4.x < -0.9f || vector4.x > 0.9f)
									{
										num10 = vector4.x;
										x = vector.x;
									}
									else
									{
										num10 = vector4.y;
										x = vector.y;
									}
									if (vector5.y < -0.9f || vector5.y > 0.9f)
									{
										num11 = vector5.y;
										x2 = vector.y;
									}
									else
									{
										num11 = vector5.x;
										x2 = vector.x;
									}
									break;
								case BlockFace.West:
								case BlockFace.East:
									if (vector4.z < -0.9f || vector4.z > 0.9f)
									{
										num10 = vector4.z;
										x = vector.z;
									}
									else
									{
										num10 = vector4.y;
										x = vector.y;
									}
									if (vector5.y < -0.9f || vector5.y > 0.9f)
									{
										num11 = vector5.y;
										x2 = vector.y;
									}
									else
									{
										num11 = vector5.z;
										x2 = vector.z;
									}
									break;
								}
							}
							Vector2 vector6 = Vector3.zero;
							int num12 = (int)uvrectTiling.uv.width;
							int num13 = (int)uvrectTiling.uv.height;
							int num14 = 0;
							if (!bTextureArray)
							{
								vector6.x = ((num10 > 0f) ? (uvrectTiling.uv.x + (float)BlockShapeNew.FloorModFloat(x, num8) * uvrectTiling.uv.width) : (uvrectTiling.uv.x + uvrectTiling.uv.width * (float)(num8 - 1) - (float)BlockShapeNew.FloorModFloat(x, num8) * uvrectTiling.uv.width));
								vector6.y = ((num11 > 0f) ? (uvrectTiling.uv.y + (float)BlockShapeNew.FloorModFloat(x2, num9) * uvrectTiling.uv.height) : (uvrectTiling.uv.y + uvrectTiling.uv.height * (float)(num9 - 1) - (float)BlockShapeNew.FloorModFloat(x2, num9) * uvrectTiling.uv.height));
							}
							else if (!flag)
							{
								num8 = Utils.FastMax(num8, num12);
								num9 = Utils.FastMax(num9, num13);
								vector6.x = (float)((num10 > 0f) ? BlockShapeNew.FloorModFloat(x, num8) : (num8 - 1 - BlockShapeNew.FloorModFloat(x, num8)));
								vector6.y = (float)((num11 > 0f) ? BlockShapeNew.FloorModFloat(x2, num9) : (num9 - 1 - BlockShapeNew.FloorModFloat(x2, num9)));
								if (num12 > 1)
								{
									num14 += BlockShapeNew.FloorModInt((int)vector6.x, num12);
								}
								if (num13 > 1)
								{
									num14 += BlockShapeNew.FloorModInt((int)vector6.y, num13) * num12;
								}
								vector6.x -= (float)((int)vector6.x);
								vector6.y -= (float)((int)vector6.y);
							}
							int num15 = uvrectTiling.index + num14;
							Color color;
							if (bTextureArray)
							{
								color.g = (float)num15;
							}
							else
							{
								color.g = (float)(block.Properties.Contains("Frame") ? 1 : 0);
							}
							color.b = (float)_lightingAround[LightingAround.Pos.Middle].stability / 15f;
							color.a = (float)(flag ? 1 : 0);
							if (bTextureArray && flag)
							{
								for (int k = 0; k < visualMesh.Indices.Count; k += 3)
								{
									BlockShapeNew.PlanePick planePick = BlockShapeNew.ClassifyTriPlane(visualMesh, k, quaternion);
									BlockShapeNew.PlaneAxis axis = planePick.axis;
									List<int> list;
									if (axis != BlockShapeNew.PlaneAxis.X)
									{
										if (axis != BlockShapeNew.PlaneAxis.Y)
										{
											list = (planePick.neg ? BlockShapeNew.threadData.trisZNeg : BlockShapeNew.threadData.trisZPos);
										}
										else
										{
											list = (planePick.neg ? BlockShapeNew.threadData.trisYNeg : BlockShapeNew.threadData.trisYPos);
										}
									}
									else
									{
										list = (planePick.neg ? BlockShapeNew.threadData.trisXNeg : BlockShapeNew.threadData.trisXPos);
									}
									List<int> list2 = list;
									list2.Add(k);
									list2.Add(k + 1);
									list2.Add(k + 2);
								}
								int blockW = Mathf.Max(uvrectTiling.blockW, num12);
								int blockH = Mathf.Max(uvrectTiling.blockH, num13);
								Rect cracksRect = WorldConstants.MapDamageToUVRect(_blockValue);
								Vector3 uvPos = _drawPos + _chunkPos + new Vector3(0.5f, 0.5f, 0.5f);
								if (BlockShapeNew.threadData.trisXPos.Count > 0)
								{
									int texArrayIndex = uvrectTiling.index + this.ComputeTaIndexOffsFor(BlockShapeNew.PlaneAxis.X, uvrectTiling, uvPos, false);
									this.AppendSubsetTriPlanar(voxelMesh, visualMesh, BlockShapeNew.threadData.trisXPos, quaternion, vector2, vector3, texArrayIndex, blockW, blockH, num12, num13, _drawPos, _lightingAround, cracksRect, BlockShapeNew.bImposterGenerationActive);
									BlockShapeNew.threadData.trisXPos.Clear();
								}
								if (BlockShapeNew.threadData.trisXNeg.Count > 0)
								{
									int texArrayIndex2 = uvrectTiling.index + this.ComputeTaIndexOffsFor(BlockShapeNew.PlaneAxis.X, uvrectTiling, uvPos, true);
									this.AppendSubsetTriPlanar(voxelMesh, visualMesh, BlockShapeNew.threadData.trisXNeg, quaternion, vector2, vector3, texArrayIndex2, blockW, blockH, num12, num13, _drawPos, _lightingAround, cracksRect, BlockShapeNew.bImposterGenerationActive);
									BlockShapeNew.threadData.trisXNeg.Clear();
								}
								if (BlockShapeNew.threadData.trisYPos.Count > 0)
								{
									int texArrayIndex3 = uvrectTiling.index + this.ComputeTaIndexOffsFor(BlockShapeNew.PlaneAxis.Y, uvrectTiling, uvPos, false);
									this.AppendSubsetTriPlanar(voxelMesh, visualMesh, BlockShapeNew.threadData.trisYPos, quaternion, vector2, vector3, texArrayIndex3, blockW, blockH, num12, num13, _drawPos, _lightingAround, cracksRect, BlockShapeNew.bImposterGenerationActive);
									BlockShapeNew.threadData.trisYPos.Clear();
								}
								if (BlockShapeNew.threadData.trisYNeg.Count > 0)
								{
									int texArrayIndex4 = uvrectTiling.index + this.ComputeTaIndexOffsFor(BlockShapeNew.PlaneAxis.Y, uvrectTiling, uvPos, false);
									this.AppendSubsetTriPlanar(voxelMesh, visualMesh, BlockShapeNew.threadData.trisYNeg, quaternion, vector2, vector3, texArrayIndex4, blockW, blockH, num12, num13, _drawPos, _lightingAround, cracksRect, BlockShapeNew.bImposterGenerationActive);
									BlockShapeNew.threadData.trisYNeg.Clear();
								}
								if (BlockShapeNew.threadData.trisZPos.Count > 0)
								{
									int texArrayIndex5 = uvrectTiling.index + this.ComputeTaIndexOffsFor(BlockShapeNew.PlaneAxis.Z, uvrectTiling, uvPos, true);
									this.AppendSubsetTriPlanar(voxelMesh, visualMesh, BlockShapeNew.threadData.trisZPos, quaternion, vector2, vector3, texArrayIndex5, blockW, blockH, num12, num13, _drawPos, _lightingAround, cracksRect, BlockShapeNew.bImposterGenerationActive);
									BlockShapeNew.threadData.trisZPos.Clear();
								}
								if (BlockShapeNew.threadData.trisZNeg.Count > 0)
								{
									int texArrayIndex6 = uvrectTiling.index + this.ComputeTaIndexOffsFor(BlockShapeNew.PlaneAxis.Z, uvrectTiling, uvPos, false);
									this.AppendSubsetTriPlanar(voxelMesh, visualMesh, BlockShapeNew.threadData.trisZNeg, quaternion, vector2, vector3, texArrayIndex6, blockW, blockH, num12, num13, _drawPos, _lightingAround, cracksRect, BlockShapeNew.bImposterGenerationActive);
									BlockShapeNew.threadData.trisZNeg.Clear();
								}
							}
							else
							{
								int count2 = voxelMesh.m_Vertices.Count;
								ArrayListMP<Vector3> collVertices = voxelMesh.m_CollVertices;
								if (collVertices != null)
								{
									int count3 = collVertices.Count;
								}
								int num16 = voxelMesh.m_Vertices.Alloc(count);
								voxelMesh.m_Normals.Alloc(count);
								voxelMesh.m_ColorVertices.Alloc(count);
								for (int l = 0; l < count; l++)
								{
									int idx = num16 + l;
									Vector3 vector7 = quaternion * (visualMesh.Vertices[l] + vector2) + vector3;
									voxelMesh.m_Vertices[idx] = vector7;
									Vector3 value = quaternion * visualMesh.Normals[l];
									voxelMesh.m_Normals[idx] = value;
									vector7 -= _drawPos;
									float num17 = (float)_lightingAround[LightingAround.Pos.X0Y0Z0].sun * (1f - vector7.x) * (1f - vector7.y) * (1f - vector7.z) + (float)_lightingAround[LightingAround.Pos.X1Y0Z0].sun * vector7.x * (1f - vector7.y) * (1f - vector7.z) + (float)_lightingAround[LightingAround.Pos.X0Y0Z1].sun * (1f - vector7.x) * (1f - vector7.y) * vector7.z + (float)_lightingAround[LightingAround.Pos.X1Y0Z1].sun * vector7.x * (1f - vector7.y) * vector7.z + (float)_lightingAround[LightingAround.Pos.X0Y1Z0].sun * (1f - vector7.x) * vector7.y * (1f - vector7.z) + (float)_lightingAround[LightingAround.Pos.X0Y1Z1].sun * (1f - vector7.x) * vector7.y * vector7.z + (float)_lightingAround[LightingAround.Pos.X1Y1Z0].sun * vector7.x * vector7.y * (1f - vector7.z) + (float)_lightingAround[LightingAround.Pos.X1Y1Z1].sun * vector7.x * vector7.y * vector7.z;
									color.r = num17 / 15f;
									voxelMesh.m_ColorVertices[idx] = color;
								}
								int num18 = voxelMesh.m_Indices.Alloc(visualMesh.Indices.Count);
								for (int m = 0; m < visualMesh.Indices.Count; m++)
								{
									voxelMesh.m_Indices[num18 + m] = voxelMesh.CurTriangleIndex + (int)visualMesh.Indices[m];
								}
								voxelMesh.CurTriangleIndex += visualMesh.Vertices.Count;
								int count4 = visualMesh.Uvs.Count;
								Rect cracksRect = WorldConstants.MapDamageToUVRect(_blockValue);
								if (bTextureArray)
								{
									num18 = voxelMesh.m_Uvs.Alloc(count4);
									Vector2 vector8;
									for (int n = 0; n < count4; n++)
									{
										int idx2 = num18 + n;
										if (flag)
										{
											vector8.x = (float)(num8 / num12);
											vector8.y = (float)(num9 / num13);
										}
										else
										{
											vector8.x = vector6.x + visualMesh.Uvs[n].x;
											vector8.y = vector6.y + visualMesh.Uvs[n].y;
										}
										voxelMesh.m_Uvs[idx2] = vector8;
									}
									if (voxelMesh.UvsCrack != null)
									{
										voxelMesh.UvsCrack.Alloc(count4);
										for (int num19 = 0; num19 < count4; num19++)
										{
											int idx3 = num18 + num19;
											if (!BlockShapeNew.bImposterGenerationActive)
											{
												vector8.x = cracksRect.x + visualMesh.Uvs[num19].x * cracksRect.width;
												vector8.y = cracksRect.y + visualMesh.Uvs[num19].y * cracksRect.height;
											}
											else
											{
												vector8.x = color.g;
												vector8.y = color.a;
											}
											voxelMesh.UvsCrack[idx3] = vector8;
										}
									}
								}
								else if (!flag)
								{
									for (int num20 = 0; num20 < count4; num20++)
									{
										Vector2 vector8;
										vector8.x = vector6.x + visualMesh.Uvs[num20].x * uvrectTiling.uv.width;
										vector8.y = vector6.y + visualMesh.Uvs[num20].y * uvrectTiling.uv.height;
										voxelMesh.m_Uvs.Add(vector8);
										if (voxelMesh.UvsCrack != null)
										{
											vector8.x = cracksRect.x + visualMesh.Uvs[num20].x * cracksRect.width;
											vector8.y = cracksRect.y + visualMesh.Uvs[num20].y * cracksRect.height;
											voxelMesh.UvsCrack.Add(vector8);
										}
									}
								}
								else
								{
									Vector3i vector3i;
									vector3i.x = num8;
									vector3i.y = num9;
									vector3i.z = num8;
									Vector3i vector3i2;
									vector3i2.x = Mathf.Abs(_chunkPos.x + (int)_drawPos.x);
									vector3i2.y = Mathf.Abs(_chunkPos.y + (int)_drawPos.y);
									vector3i2.z = Mathf.Abs(_chunkPos.z + (int)_drawPos.z);
									if (_chunkPos.x < 0)
									{
										vector3i2.x = Mathf.Abs(_chunkPos.x + 16 - (int)_drawPos.x);
									}
									if (_chunkPos.y < 0)
									{
										vector3i2.y = Mathf.Abs(_chunkPos.y + 16 - (int)_drawPos.y);
									}
									if (_chunkPos.z < 0)
									{
										vector3i2.z = Mathf.Abs(_chunkPos.z + 16 - (int)_drawPos.z);
									}
									Vector3 vector9;
									vector9.x = (float)(vector3i2.x % vector3i.x);
									vector9.y = (float)(vector3i2.y % vector3i.y);
									vector9.z = (float)(vector3i2.z % vector3i.z);
									bool flag2 = false;
									for (int num21 = 0; num21 < 6; num21++)
									{
										BlockShapeNew.MySimpleMesh visualMesh2 = this.getVisualMesh(num21);
										if (visualMesh2 != null && visualMesh2.Uvs.Count > 6)
										{
											flag2 = true;
											break;
										}
									}
									for (int num22 = 0; num22 < count4; num22++)
									{
										Vector3 vector10 = voxelMesh.m_Normals[count2];
										object obj = Mathf.Abs(vector10.y) > Mathf.Abs(vector10.x) && Mathf.Abs(vector10.y) > Mathf.Abs(vector10.z);
										bool flag3 = Mathf.Abs(vector10.x) > Mathf.Abs(vector10.y) && Mathf.Abs(vector10.x) > Mathf.Abs(vector10.z);
										object obj2 = obj;
										if (obj2 == null)
										{
											bool flag4 = !flag3;
										}
										if (obj2 != null)
										{
											vector3i.z = uvrectTiling.blockH;
											vector9.z = (float)(vector3i2.z % vector3i.z);
										}
										Vector2 zero = Vector2.zero;
										vector10 = voxelMesh.m_Normals[count2 + num22];
										if (!flag2)
										{
											BlockFace blockFace = BlockFace.Top;
											if (vector10.z > 0.95f)
											{
												blockFace = BlockFace.North;
											}
											else if (vector10.z < -0.95f)
											{
												blockFace = BlockFace.South;
											}
											else if (vector10.x > 0.95f)
											{
												blockFace = BlockFace.East;
											}
											else if (vector10.x < -0.95f)
											{
												blockFace = BlockFace.West;
											}
											else if (vector10.y < 0f)
											{
												blockFace = BlockFace.Bottom;
											}
											bool flag5 = Mathf.Abs(vector10.z) > 0.0001f;
											bool flag6 = vector10.y < 0f;
											bool flag7 = Mathf.Abs(vector10.y) > 0.99f;
											bool flag8 = Mathf.Abs(vector10.x) > 0.0001f;
											bool flag9 = vector10.x > 0.0001f;
											bool flag10 = vector10.z > 0.0001f;
											bool flag11 = Mathf.Abs(vector10.x) > 0.1f && Mathf.Abs(vector10.y) > 0.1f && Mathf.Abs(vector10.z) > 0.1f;
											if ((blockFace == BlockFace.Top || blockFace == BlockFace.Bottom) && flag8 && !flag5)
											{
												vector3i.x = uvrectTiling.blockH;
												vector3i.z = uvrectTiling.blockW;
											}
											Vector3 vector11 = voxelMesh.m_Vertices[count2 + num22] - _drawPos;
											vector11.x = Mathf.Abs(vector11.x);
											vector11.y = Mathf.Abs(vector11.y);
											vector11.z = Mathf.Abs(vector11.z);
											vector9.x = Mathf.Abs(vector9.x);
											vector9.y = Mathf.Abs(vector9.y);
											vector9.z = Mathf.Abs(vector9.z);
											Vector3 vector12 = vector9 + vector11;
											vector12.x = (((int)Mathf.Abs(vector12.x) != vector3i.x) ? (Mathf.Abs(vector12.x) % (float)vector3i.x) : Mathf.Abs(vector12.x));
											vector12.y = (((int)Mathf.Abs(vector12.y) != vector3i.y) ? (Mathf.Abs(vector12.y) % (float)vector3i.y) : Mathf.Abs(vector12.y));
											vector12.z = (((int)Mathf.Abs(vector12.z) != vector3i.z) ? (Mathf.Abs(vector12.z) % (float)vector3i.z) : Mathf.Abs(vector12.z));
											if ((int)vector12.x > vector3i.x)
											{
												vector12.x %= (float)vector3i.x;
											}
											if ((int)vector12.y > vector3i.y)
											{
												vector12.y %= (float)vector3i.y;
											}
											if ((int)vector12.z > vector3i.z)
											{
												vector12.z %= (float)vector3i.z;
											}
											Vector3 vector13 = vector3i.ToVector3() - vector12;
											float num23 = ((int)Mathf.Abs(vector12.x) != vector3i.x) ? (Mathf.Abs(vector12.x) % (float)vector3i.x) : Mathf.Abs(vector12.x);
											float y = ((int)Mathf.Abs(vector12.z) != vector3i.z) ? (Mathf.Abs(vector12.z) % (float)vector3i.z) : Mathf.Abs(vector12.z);
											switch (blockFace)
											{
											case BlockFace.Top:
											case BlockFace.Bottom:
												if (!flag7)
												{
													if (flag8)
													{
														if (flag9)
														{
															if (flag5)
															{
																if (flag10)
																{
																	if (flag11)
																	{
																		zero.x = (vector12.x + vector13.z) * 0.5f;
																	}
																	else
																	{
																		zero.x = (float)vector3i.x - num23;
																	}
																}
																else if (flag11)
																{
																	zero.x = (vector13.x + vector13.z) * 0.5f;
																}
																else
																{
																	zero.x = num23;
																}
																zero.y = vector12.y;
															}
															else
															{
																zero.x = vector12.z;
																if (flag6)
																{
																	zero.y = vector12.x;
																}
																else
																{
																	zero.y = vector13.x;
																}
															}
														}
														else if (flag5)
														{
															if (flag10)
															{
																if (flag11)
																{
																	zero.x = (vector13.x + vector13.z) * 0.5f;
																}
																else
																{
																	zero.x = (float)vector3i.x - num23;
																}
															}
															else if (flag11)
															{
																zero.x = (vector12.x + vector13.z) * 0.5f;
															}
															else
															{
																zero.x = num23;
															}
															zero.y = vector12.y;
														}
														else
														{
															zero.x = vector13.z;
															if (flag6)
															{
																zero.y = vector13.x;
															}
															else
															{
																zero.y = vector12.x;
															}
														}
													}
													else if (flag10)
													{
														zero.x = vector13.x;
														if (flag6)
														{
															zero.y = vector12.z;
														}
														else
														{
															zero.y = vector13.z;
														}
													}
													else
													{
														zero.x = vector12.x;
														if (flag6)
														{
															zero.y = vector13.z;
														}
														else
														{
															zero.y = vector12.z;
														}
													}
												}
												else
												{
													zero.x = num23;
													zero.y = y;
												}
												break;
											case BlockFace.North:
												zero.x = vector13.x;
												zero.y = vector12.y;
												break;
											case BlockFace.West:
												zero.x = vector13.z;
												zero.y = vector12.y;
												break;
											case BlockFace.South:
												goto IL_1828;
											case BlockFace.East:
												zero.x = vector12.z;
												zero.y = vector12.y;
												break;
											default:
												goto IL_1828;
											}
											IL_1844:
											voxelMesh.m_Uvs.Add(new Vector2(uvrectTiling.uv.x + zero.x * uvrectTiling.uv.width, uvrectTiling.uv.y + zero.y * uvrectTiling.uv.height));
											goto IL_191D;
											IL_1828:
											zero.x = vector12.x;
											zero.y = vector12.y;
											goto IL_1844;
										}
										zero.x = Mathf.Floor(1f / uvrectTiling.uv.width * 1000f);
										zero.y = (float)uvrectTiling.blockW + Mathf.Floor((float)(uvrectTiling.blockH * 1000));
										zero.x += uvrectTiling.uv.x;
										zero.y += uvrectTiling.uv.y;
										voxelMesh.m_Uvs.Add(zero);
										IL_191D:
										if (voxelMesh.UvsCrack != null)
										{
											voxelMesh.UvsCrack.Add(new Vector2(cracksRect.x + visualMesh.Uvs[num22].x * cracksRect.width, cracksRect.y + visualMesh.Uvs[num22].y * cracksRect.height));
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0002003D File Offset: 0x0001E23D
	public override bool IsRenderDecoration()
	{
		return true;
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00055248 File Offset: 0x00053448
	public override void renderDecorations(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, INeighborBlockCache _nBlocks)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		for (int i = 0; i < this.faceInfo.Length; i++)
		{
			int num = BlockShapeNew.convertRotationCached[(int)_blockValue.rotation, i];
			if (this.visualMeshes[num] != null)
			{
				BlockShapeNew.EnumFaceOcclusionInfo enumFaceOcclusionInfo = this.faceInfo[num];
				if (enumFaceOcclusionInfo != BlockShapeNew.EnumFaceOcclusionInfo.None)
				{
					if (enumFaceOcclusionInfo != BlockShapeNew.EnumFaceOcclusionInfo.OwnFaces)
					{
						if (enumFaceOcclusionInfo != BlockShapeNew.EnumFaceOcclusionInfo.Transparent)
						{
							goto IL_14F;
						}
					}
					else
					{
						BlockFace face = (BlockFace)i;
						Vector3i vector3i = BlockFaceFlags.OffsetIForFace(face);
						BlockValue blockValue = _nBlocks.Get(vector3i.x, vector3i.y + (int)_drawPos.y, vector3i.z);
						Block block = blockValue.Block;
						bool flag = !blockValue.ischild && block.shape.IsSolidCube && !block.shape.IsTerrain();
						if (!flag)
						{
							int num2 = 0;
							switch (face)
							{
							case BlockFace.Top:
								num2 = 2;
								break;
							case BlockFace.Bottom:
								num2 = 1;
								break;
							case BlockFace.North:
								num2 = 16;
								break;
							case BlockFace.West:
								num2 = 32;
								break;
							case BlockFace.South:
								num2 = 4;
								break;
							case BlockFace.East:
								num2 = 8;
								break;
							}
							int facesDrawnFullBitfield = block.shape.getFacesDrawnFullBitfield(blockValue);
							flag = (flag || (facesDrawnFullBitfield & num2) != 0);
						}
						if (flag && this.isRenderFace(_blockValue, face, blockValue))
						{
							this.renderFace(_worldPos, _blockValue, _drawPos, face, _vertices, _lightingAround, _textureFullArray, _meshes, BlockShape.MeshPurpose.World);
							goto IL_14F;
						}
						goto IL_14F;
					}
				}
				this.renderFace(_worldPos, _blockValue, _drawPos, (BlockFace)i, _vertices, _lightingAround, _textureFullArray, _meshes, BlockShape.MeshPurpose.World);
			}
			IL_14F:;
		}
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x000553B8 File Offset: 0x000535B8
	public override void renderFull(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		int num = 7;
		for (int i = 0; i < num; i++)
		{
			this.renderFace(_worldPos, _blockValue, _drawPos, (BlockFace)i, _vertices, _lightingAround, _textureFullArray, _meshes, _purpose);
		}
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x000553F2 File Offset: 0x000535F2
	[PublicizedFrom(EAccessModifier.Private)]
	public static int BlockFaceToRot(BlockFace _blockFace)
	{
		return (int)((int)_blockFace << 2);
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x000553F8 File Offset: 0x000535F8
	public static BlockFace RotToBlockFace(int _rotation)
	{
		int num = _rotation >> 2 & 7;
		if (num <= 5)
		{
			return (BlockFace)num;
		}
		return BlockFace.Top;
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x00055413 File Offset: 0x00053613
	[PublicizedFrom(EAccessModifier.Private)]
	public static int RotToLocalRot(int _rotation)
	{
		return _rotation & 3;
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x00055418 File Offset: 0x00053618
	public static Quaternion GetRotationStatic(int _rotation)
	{
		return BlockShapeNew.rotationsToQuats[_rotation];
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x00055425 File Offset: 0x00053625
	public override Quaternion GetRotation(BlockValue _blockValue)
	{
		return BlockShapeNew.rotationsToQuats[(int)_blockValue.rotation];
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x00055438 File Offset: 0x00053638
	public override byte Rotate(bool _bLeft, int _rotation)
	{
		_rotation += (_bLeft ? -1 : 1);
		if (_rotation > 23)
		{
			_rotation = 0;
		}
		if (_rotation < 0)
		{
			_rotation = 23;
		}
		return (byte)_rotation;
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x00055457 File Offset: 0x00053657
	public override Quaternion GetPreviewRotation()
	{
		return Quaternion.AngleAxis(180f, Vector3.up);
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00055468 File Offset: 0x00053668
	[PublicizedFrom(EAccessModifier.Private)]
	static BlockShapeNew()
	{
		for (int i = 1; i < 4; i++)
		{
			for (byte b = 0; b < 28; b += 1)
			{
				BlockShapeNew.rotations[i - 1, (int)b] = BlockShapeNew.CalcRotation(b, i);
			}
		}
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x0005591F File Offset: 0x00053B1F
	public override BlockValue RotateY(bool _bLeft, BlockValue _blockValue, int _rotCount)
	{
		if (_rotCount == 0)
		{
			return _blockValue;
		}
		if (_bLeft)
		{
			_rotCount = 4 - _rotCount;
		}
		_blockValue.rotation = BlockShapeNew.rotations[_rotCount - 1, (int)_blockValue.rotation];
		return _blockValue;
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x0005594C File Offset: 0x00053B4C
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte CalcRotation(byte _rotation, int _rotCount)
	{
		if (_rotation >= 24)
		{
			for (int i = 0; i < _rotCount; i++)
			{
				_rotation += 1;
				if (_rotation > 27)
				{
					_rotation = 24;
				}
				else if (_rotation < 24)
				{
					_rotation = 27;
				}
			}
		}
		else
		{
			int num = 90 * _rotCount;
			_rotation = (byte)BlockShapeNew.ConvertRotationFree((int)_rotation, Quaternion.AngleAxis((float)num, Vector3.up), false);
		}
		return _rotation;
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x000559A4 File Offset: 0x00053BA4
	public static int MirrorStatic(EnumMirrorAlong _axis, int _rotation, int _symType = 1)
	{
		switch (_symType)
		{
		case 0:
			return _rotation;
		default:
		{
			Quaternion quaternion = BlockShapeNew.GetRotationStatic(_rotation);
			switch (_axis)
			{
			case EnumMirrorAlong.XAxis:
				quaternion.x = -quaternion.x;
				quaternion.w = -quaternion.w;
				break;
			case EnumMirrorAlong.YAxis:
				quaternion.y = -quaternion.y;
				quaternion.w = -quaternion.w;
				quaternion *= Quaternion.AngleAxis(180f, Vector3.forward);
				break;
			case EnumMirrorAlong.ZAxis:
				quaternion.z = -quaternion.z;
				quaternion.w = -quaternion.w;
				quaternion *= Quaternion.AngleAxis(180f, Vector3.up);
				break;
			}
			return BlockShapeNew.ConvertRotationFree(0, quaternion, false);
		}
		case 2:
		{
			BlockFace blockFace = BlockShapeNew.RotToBlockFace(_rotation);
			int num = 0;
			switch (_axis)
			{
			case EnumMirrorAlong.XAxis:
				if (blockFace != BlockFace.West)
				{
					if (blockFace == BlockFace.East)
					{
						blockFace = BlockFace.West;
					}
				}
				else
				{
					blockFace = BlockFace.East;
				}
				break;
			case EnumMirrorAlong.YAxis:
				switch (blockFace)
				{
				case BlockFace.Top:
					blockFace = BlockFace.Bottom;
					break;
				case BlockFace.Bottom:
					blockFace = BlockFace.Top;
					break;
				case BlockFace.North:
				case BlockFace.South:
					num = 1;
					break;
				}
				break;
			case EnumMirrorAlong.ZAxis:
				if (blockFace != BlockFace.North)
				{
					if (blockFace != BlockFace.South)
					{
						num = 1;
					}
					else
					{
						blockFace = BlockFace.North;
					}
				}
				else
				{
					blockFace = BlockFace.South;
				}
				break;
			}
			int num2 = BlockShapeNew.RotToLocalRot(_rotation);
			if (num != 0)
			{
				if (num == 1)
				{
					switch (num2)
					{
					case 0:
						num2 = 1;
						break;
					case 1:
						num2 = 0;
						break;
					case 2:
						num2 = 3;
						break;
					case 3:
						num2 = 2;
						break;
					}
				}
			}
			else
			{
				switch (num2)
				{
				case 0:
					num2 = 3;
					break;
				case 1:
					num2 = 2;
					break;
				case 2:
					num2 = 1;
					break;
				case 3:
					num2 = 0;
					break;
				}
			}
			return BlockShapeNew.BlockFaceToRot(blockFace) | num2;
		}
		case 3:
		{
			BlockFace blockFace2 = BlockShapeNew.RotToBlockFace(_rotation);
			int num3 = 1;
			switch (_axis)
			{
			case EnumMirrorAlong.XAxis:
				if (blockFace2 != BlockFace.West)
				{
					if (blockFace2 == BlockFace.East)
					{
						blockFace2 = BlockFace.West;
					}
				}
				else
				{
					blockFace2 = BlockFace.East;
				}
				break;
			case EnumMirrorAlong.YAxis:
				switch (blockFace2)
				{
				case BlockFace.Top:
					blockFace2 = BlockFace.Bottom;
					break;
				case BlockFace.Bottom:
					blockFace2 = BlockFace.Top;
					break;
				case BlockFace.North:
				case BlockFace.South:
					num3 = 0;
					break;
				}
				break;
			case EnumMirrorAlong.ZAxis:
				if (blockFace2 != BlockFace.North)
				{
					if (blockFace2 != BlockFace.South)
					{
						num3 = 0;
					}
					else
					{
						blockFace2 = BlockFace.North;
					}
				}
				else
				{
					blockFace2 = BlockFace.South;
				}
				break;
			}
			int num4 = BlockShapeNew.RotToLocalRot(_rotation);
			if (num3 != 0)
			{
				if (num3 == 1)
				{
					switch (num4)
					{
					case 0:
						num4 = 1;
						break;
					case 1:
						num4 = 0;
						break;
					case 2:
						num4 = 3;
						break;
					case 3:
						num4 = 2;
						break;
					}
				}
			}
			else
			{
				switch (num4)
				{
				case 0:
					num4 = 3;
					break;
				case 1:
					num4 = 2;
					break;
				case 2:
					num4 = 1;
					break;
				case 3:
					num4 = 0;
					break;
				}
			}
			return BlockShapeNew.BlockFaceToRot(blockFace2) | num4;
		}
		case 4:
			BlockShapeNew.GetRotationStatic(_rotation);
			switch (_axis)
			{
			case EnumMirrorAlong.XAxis:
				_rotation = BlockShapeNew.ConvertRotationFree(_rotation, Quaternion.AngleAxis(180f, Vector3.up), true);
				return BlockShapeNew.ConvertRotationFree(_rotation, Quaternion.AngleAxis(180f, Vector3.right), false);
			case EnumMirrorAlong.YAxis:
				_rotation = BlockShapeNew.ConvertRotationFree(_rotation, Quaternion.AngleAxis(180f, Vector3.up), true);
				return BlockShapeNew.ConvertRotationFree(_rotation, Quaternion.AngleAxis(180f, Vector3.up), false);
			case EnumMirrorAlong.ZAxis:
				_rotation = BlockShapeNew.ConvertRotationFree(_rotation, Quaternion.AngleAxis(180f, Vector3.up), true);
				return BlockShapeNew.ConvertRotationFree(_rotation, Quaternion.AngleAxis(180f, Vector3.forward), false);
			default:
				return _rotation;
			}
			break;
		}
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x00055CE4 File Offset: 0x00053EE4
	public BlockFace GetBlockFaceFromColliderTriangle(BlockValue _blockValue, Vector3 _v1, Vector3 _v2, Vector3 _v3)
	{
		for (int i = 0; i < this.colliderMeshes.Length; i++)
		{
			if (BlockShapeNew.<GetBlockFaceFromColliderTriangle>g__CheckMeshForTri|55_0(this.colliderMeshes[i], _v1, _v2, _v3))
			{
				return (BlockFace)i;
			}
		}
		for (int j = 0; j < this.visualMeshes.Length; j++)
		{
			if (BlockShapeNew.<GetBlockFaceFromColliderTriangle>g__CheckMeshForTri|55_0(this.visualMeshes[j], _v1, _v2, _v3))
			{
				return (BlockFace)(j % 7);
			}
		}
		return BlockFace.None;
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x00055D4C File Offset: 0x00053F4C
	public int GetVisualMeshChannelFromHitInfo(Vector3i blockPos, BlockValue bv, BlockFace blockFace, WorldRayHitInfo hitInfo)
	{
		if (this.visualMeshes.Length <= 7)
		{
			return 0;
		}
		int num = 0;
		int result = -1;
		for (int i = 0; i < 1; i++)
		{
			int num2 = i * 7;
			if (this.getVisualMesh((int)(blockFace + (byte)num2)) != null)
			{
				num++;
				result = i;
			}
		}
		if (num == 1)
		{
			return result;
		}
		Vector3 b = Vector3.one * 0.5f;
		Quaternion rotation = Quaternion.Inverse(this.GetRotation(bv));
		Ray ray = hitInfo.ray;
		ray.origin = rotation * (ray.origin - blockPos - b) + b;
		ray.direction = rotation * ray.direction;
		float num3 = float.MaxValue;
		int result2 = -1;
		for (int j = 0; j < 1; j++)
		{
			int num4 = j * 7;
			BlockShapeNew.MySimpleMesh visualMesh = this.getVisualMesh((int)(blockFace + (byte)num4));
			if (visualMesh != null)
			{
				for (int k = 0; k < visualMesh.Indices.Count; k += 3)
				{
					GeometryUtils.Triangle tri = new GeometryUtils.Triangle(visualMesh.Vertices[(int)visualMesh.Indices[k]], visualMesh.Vertices[(int)visualMesh.Indices[k + 1]], visualMesh.Vertices[(int)visualMesh.Indices[k + 2]]);
					Vector3 vector;
					float num5;
					if (GeometryUtils.IntersectRayTriangle(ray, tri, out vector, out num5) && num5 < num3)
					{
						num3 = num5;
						result2 = j;
					}
				}
			}
		}
		return result2;
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00055ED4 File Offset: 0x000540D4
	public override Vector2 GetPathOffset(int _rotation)
	{
		return this.boundsPathOffsetRotations[_rotation];
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x0004E51D File Offset: 0x0004C71D
	public override float GetStepHeight(BlockValue blockDef, BlockFace crossingFace)
	{
		return (float)(blockDef.Block.IsCollideMovement ? 1 : 0);
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x00055EE2 File Offset: 0x000540E2
	public override Bounds[] GetBounds(BlockValue _blockValue)
	{
		this.boundsArr[0] = this.boundsRotations[(int)_blockValue.rotation];
		return this.boundsArr;
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x00055F08 File Offset: 0x00054108
	public override BlockFace GetRotatedBlockFace(BlockValue _blockValue, BlockFace _face)
	{
		return (BlockFace)BlockShapeNew.convertRotationCached[(int)_blockValue.rotation, (int)_face];
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x00055F20 File Offset: 0x00054120
	public override void MirrorFace(EnumMirrorAlong _axis, int _sourceRot, int _targetRot, BlockFace _face, out BlockFace _sourceFace, out BlockFace _targetFace)
	{
		_sourceFace = (BlockFace)BlockShapeNew.convertRotationCached[_sourceRot, (int)_face];
		switch (_axis)
		{
		case EnumMirrorAlong.XAxis:
			switch (_face)
			{
			case BlockFace.Top:
				_face = BlockFace.Top;
				break;
			case BlockFace.Bottom:
				_face = BlockFace.Bottom;
				break;
			case BlockFace.North:
				_face = BlockFace.North;
				break;
			case BlockFace.West:
				_face = BlockFace.East;
				break;
			case BlockFace.South:
				_face = BlockFace.South;
				break;
			case BlockFace.East:
				_face = BlockFace.West;
				break;
			}
			break;
		case EnumMirrorAlong.YAxis:
			switch (_face)
			{
			case BlockFace.Top:
				_face = BlockFace.Bottom;
				break;
			case BlockFace.Bottom:
				_face = BlockFace.Top;
				break;
			case BlockFace.North:
				_face = BlockFace.North;
				break;
			case BlockFace.West:
				_face = BlockFace.West;
				break;
			case BlockFace.South:
				_face = BlockFace.South;
				break;
			case BlockFace.East:
				_face = BlockFace.East;
				break;
			}
			break;
		case EnumMirrorAlong.ZAxis:
			switch (_face)
			{
			case BlockFace.Top:
				_face = BlockFace.Top;
				break;
			case BlockFace.Bottom:
				_face = BlockFace.Bottom;
				break;
			case BlockFace.North:
				_face = BlockFace.South;
				break;
			case BlockFace.West:
				_face = BlockFace.West;
				break;
			case BlockFace.South:
				_face = BlockFace.North;
				break;
			case BlockFace.East:
				_face = BlockFace.East;
				break;
			}
			break;
		}
		_targetFace = (BlockFace)BlockShapeNew.convertRotationCached[_targetRot, (int)_face];
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x00056034 File Offset: 0x00054234
	public override void CalculateCollisionHash(IncrementalHash calcHash)
	{
		Action<IncrementalHash, BlockShapeNew.MySimpleMesh> action = delegate(IncrementalHash hash, BlockShapeNew.MySimpleMesh mesh)
		{
			foreach (ushort value2 in mesh.Indices)
			{
				hash.AppendDataNoAlloc(value2);
			}
			foreach (Vector2 vector in mesh.Uvs)
			{
				hash.AppendDataNoAlloc(vector.x);
				hash.AppendDataNoAlloc(vector.y);
			}
			foreach (Vector3 vector2 in mesh.Vertices)
			{
				hash.AppendDataNoAlloc(vector2.x);
				hash.AppendDataNoAlloc(vector2.y);
				hash.AppendDataNoAlloc(vector2.z);
			}
			foreach (Vector3 vector3 in mesh.Normals)
			{
				hash.AppendDataNoAlloc(vector3.x);
				hash.AppendDataNoAlloc(vector3.y);
				hash.AppendDataNoAlloc(vector3.z);
			}
		};
		int value = 1;
		calcHash.AppendDataNoAlloc(value);
		if (this.colliderMeshes != null)
		{
			for (int i = 0; i < this.colliderMeshes.Length; i++)
			{
				if (this.colliderMeshes[i] != null)
				{
					action(calcHash, this.colliderMeshes[i]);
				}
			}
		}
		if (this.visualMeshes != null)
		{
			for (int j = 0; j < this.visualMeshes.Length; j++)
			{
				if (this.visualMeshes[j] != null)
				{
					action(calcHash, this.visualMeshes[j]);
				}
			}
		}
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x000560D4 File Offset: 0x000542D4
	public override int GetVertexCount()
	{
		int num = 0;
		for (int i = 0; i < this.visualMeshes.Length; i++)
		{
			num += ((this.visualMeshes[i] != null) ? this.visualMeshes[i].Vertices.Count : 0);
		}
		return num;
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x0005611C File Offset: 0x0005431C
	public override int GetTriangleCount()
	{
		int num = 0;
		for (int i = 0; i < this.visualMeshes.Length; i++)
		{
			num += ((this.visualMeshes[i] != null) ? (this.visualMeshes[i].Indices.Count / 3) : 0);
		}
		return num;
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x00056163 File Offset: 0x00054363
	public override string GetName()
	{
		return this.ShapeName;
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x0005616B File Offset: 0x0005436B
	public BlockShapeNew.EnumFaceOcclusionInfo GetFaceInfo(BlockValue _blockValue, BlockFace _face)
	{
		_face = this.GetRotatedBlockFace(_blockValue, _face);
		return this.faceInfo[(int)_face];
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x00056180 File Offset: 0x00054380
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <renderFace>g__CopyColliderMesh|38_0(VoxelMesh targetMesh, Quaternion q, Vector3 sum1, Vector3 sum2, BlockShapeNew.MySimpleMesh colliderMesh)
	{
		int count = targetMesh.m_CollVertices.Count;
		int num = targetMesh.m_CollVertices.Alloc(colliderMesh.Vertices.Count);
		for (int i = 0; i < colliderMesh.Vertices.Count; i++)
		{
			Vector3 value = q * (colliderMesh.Vertices[i] + sum1) + sum2;
			targetMesh.m_CollVertices[num + i] = value;
		}
		num = targetMesh.m_CollIndices.Alloc(colliderMesh.Indices.Count);
		for (int j = 0; j < colliderMesh.Indices.Count; j++)
		{
			targetMesh.m_CollIndices[num + j] = count + (int)colliderMesh.Indices[j];
		}
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x0005624C File Offset: 0x0005444C
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static bool <GetBlockFaceFromColliderTriangle>g__CheckMeshForTri|55_0(BlockShapeNew.MySimpleMesh mesh, Vector3 _v1, Vector3 _v2, Vector3 _v3)
	{
		if (mesh == null)
		{
			return false;
		}
		for (int i = 0; i < mesh.Indices.Count; i += 3)
		{
			if ((_v1 - mesh.Vertices[(int)mesh.Indices[i]]).sqrMagnitude < 0.001f && (_v2 - mesh.Vertices[(int)mesh.Indices[i + 1]]).sqrMagnitude < 0.001f && (_v3 - mesh.Vertices[(int)mesh.Indices[i + 2]]).sqrMagnitude < 0.001f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000B7C RID: 2940
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRotationsUsed = 28;

	// Token: 0x04000B7D RID: 2941
	public static bool bImposterGenerationActive;

	// Token: 0x04000B7E RID: 2942
	public string ShapeName;

	// Token: 0x04000B7F RID: 2943
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockShapeNew.MySimpleMesh[] visualMeshes;

	// Token: 0x04000B80 RID: 2944
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockShapeNew.MySimpleMesh[] colliderMeshes;

	// Token: 0x04000B81 RID: 2945
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockShapeNew.EnumFaceOcclusionInfo[] faceInfo;

	// Token: 0x04000B82 RID: 2946
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Vector3 centerOffsetV = new Vector3(-0.5f, -0.5f, -0.5f);

	// Token: 0x04000B83 RID: 2947
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds[] boundsRotations;

	// Token: 0x04000B84 RID: 2948
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2[] boundsPathOffsetRotations;

	// Token: 0x04000B85 RID: 2949
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[,] convertRotationCached;

	// Token: 0x04000B86 RID: 2950
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, BlockShapeNew.MeshData> meshData = new Dictionary<string, BlockShapeNew.MeshData>();

	// Token: 0x04000B87 RID: 2951
	[ThreadStatic]
	[PublicizedFrom(EAccessModifier.Private)]
	public static BlockShapeNew.ThreadData threadData;

	// Token: 0x04000B88 RID: 2952
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Quaternion[] rotationsToQuats = new Quaternion[]
	{
		Quaternion.AngleAxis(0f, Vector3.up),
		Quaternion.AngleAxis(90f, Vector3.up),
		Quaternion.AngleAxis(180f, Vector3.up),
		Quaternion.AngleAxis(270f, Vector3.up),
		Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(180f, Vector3.up),
		Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(270f, Vector3.up),
		Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(360f, Vector3.up),
		Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(450f, Vector3.up),
		Quaternion.AngleAxis(90f, Vector3.right) * Quaternion.AngleAxis(180f, Vector3.up),
		Quaternion.AngleAxis(90f, Vector3.right) * Quaternion.AngleAxis(270f, Vector3.up),
		Quaternion.AngleAxis(90f, Vector3.right) * Quaternion.AngleAxis(360f, Vector3.up),
		Quaternion.AngleAxis(90f, Vector3.right) * Quaternion.AngleAxis(450f, Vector3.up),
		Quaternion.AngleAxis(90f, Vector3.forward) * Quaternion.AngleAxis(0f, Vector3.up),
		Quaternion.AngleAxis(90f, Vector3.forward) * Quaternion.AngleAxis(90f, Vector3.up),
		Quaternion.AngleAxis(90f, Vector3.forward) * Quaternion.AngleAxis(180f, Vector3.up),
		Quaternion.AngleAxis(90f, Vector3.forward) * Quaternion.AngleAxis(270f, Vector3.up),
		Quaternion.AngleAxis(270f, Vector3.right) * Quaternion.AngleAxis(0f, Vector3.up),
		Quaternion.AngleAxis(270f, Vector3.right) * Quaternion.AngleAxis(90f, Vector3.up),
		Quaternion.AngleAxis(270f, Vector3.right) * Quaternion.AngleAxis(180f, Vector3.up),
		Quaternion.AngleAxis(270f, Vector3.right) * Quaternion.AngleAxis(270f, Vector3.up),
		Quaternion.AngleAxis(270f, Vector3.forward) * Quaternion.AngleAxis(0f, Vector3.up),
		Quaternion.AngleAxis(270f, Vector3.forward) * Quaternion.AngleAxis(90f, Vector3.up),
		Quaternion.AngleAxis(270f, Vector3.forward) * Quaternion.AngleAxis(180f, Vector3.up),
		Quaternion.AngleAxis(270f, Vector3.forward) * Quaternion.AngleAxis(270f, Vector3.up),
		Quaternion.AngleAxis(45f, Vector3.up),
		Quaternion.AngleAxis(135f, Vector3.up),
		Quaternion.AngleAxis(225f, Vector3.up),
		Quaternion.AngleAxis(315f, Vector3.up),
		Quaternion.identity,
		Quaternion.identity,
		Quaternion.identity,
		Quaternion.identity
	};

	// Token: 0x04000B89 RID: 2953
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte[,] rotations = new byte[3, 28];

	// Token: 0x04000B8A RID: 2954
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cDataVersion = 1;

	// Token: 0x020001B0 RID: 432
	public enum EnumFaceOcclusionInfo
	{
		// Token: 0x04000B8C RID: 2956
		None,
		// Token: 0x04000B8D RID: 2957
		Part,
		// Token: 0x04000B8E RID: 2958
		Full,
		// Token: 0x04000B8F RID: 2959
		Remove,
		// Token: 0x04000B90 RID: 2960
		Continuous,
		// Token: 0x04000B91 RID: 2961
		RemoveIfAny,
		// Token: 0x04000B92 RID: 2962
		OwnFaces,
		// Token: 0x04000B93 RID: 2963
		HideIfSame,
		// Token: 0x04000B94 RID: 2964
		Transparent
	}

	// Token: 0x020001B1 RID: 433
	[PublicizedFrom(EAccessModifier.Private)]
	public class MySimpleMesh
	{
		// Token: 0x06000D2F RID: 3375 RVA: 0x00056308 File Offset: 0x00054508
		public override int GetHashCode()
		{
			int num = 0;
			foreach (ushort value in this.Indices)
			{
				num = HashCode.Combine<int, ushort>(num, value);
			}
			foreach (Vector2 value2 in this.Uvs)
			{
				num = HashCode.Combine<int, Vector2>(num, value2);
			}
			foreach (Vector3 value3 in this.Vertices)
			{
				num = HashCode.Combine<int, Vector3>(num, value3);
			}
			foreach (Vector3 value4 in this.Normals)
			{
				num = HashCode.Combine<int, Vector3>(num, value4);
			}
			return num;
		}

		// Token: 0x04000B95 RID: 2965
		public readonly List<ushort> Indices = new List<ushort>();

		// Token: 0x04000B96 RID: 2966
		public readonly List<Vector2> Uvs = new List<Vector2>();

		// Token: 0x04000B97 RID: 2967
		public readonly List<Vector3> Vertices = new List<Vector3>();

		// Token: 0x04000B98 RID: 2968
		public readonly List<Vector3> Normals = new List<Vector3>();
	}

	// Token: 0x020001B2 RID: 434
	[PublicizedFrom(EAccessModifier.Private)]
	public class MeshData
	{
		// Token: 0x04000B99 RID: 2969
		public GameObject obj;

		// Token: 0x04000B9A RID: 2970
		public int symTypeOverride = -1;

		// Token: 0x04000B9B RID: 2971
		public bool IsSolidCube;

		// Token: 0x04000B9C RID: 2972
		public Dictionary<Vector3, BlockShapeNew.MeshData.Arrays> posArrays = new Dictionary<Vector3, BlockShapeNew.MeshData.Arrays>();

		// Token: 0x020001B3 RID: 435
		public class Arrays
		{
			// Token: 0x04000B9D RID: 2973
			public BlockShapeNew.MySimpleMesh[] meshes = new BlockShapeNew.MySimpleMesh[7];

			// Token: 0x04000B9E RID: 2974
			public BlockShapeNew.MySimpleMesh[] colliderMeshes = new BlockShapeNew.MySimpleMesh[7];

			// Token: 0x04000B9F RID: 2975
			public BlockShapeNew.EnumFaceOcclusionInfo[] faceInfo = new BlockShapeNew.EnumFaceOcclusionInfo[7];

			// Token: 0x04000BA0 RID: 2976
			public Bounds[] boundsRotations = new Bounds[32];
		}
	}

	// Token: 0x020001B4 RID: 436
	[PublicizedFrom(EAccessModifier.Private)]
	public struct ThreadData
	{
		// Token: 0x06000D33 RID: 3379 RVA: 0x000564B8 File Offset: 0x000546B8
		public void Init()
		{
			this.trisXPos = new List<int>(128);
			this.trisXNeg = new List<int>(128);
			this.trisYPos = new List<int>(128);
			this.trisYNeg = new List<int>(128);
			this.trisZPos = new List<int>(128);
			this.trisZNeg = new List<int>(128);
			this.subsetUsed = new Dictionary<int, int>(256);
			this.subsetUsedList = new List<int>(256);
		}

		// Token: 0x04000BA1 RID: 2977
		public List<int> trisXPos;

		// Token: 0x04000BA2 RID: 2978
		public List<int> trisXNeg;

		// Token: 0x04000BA3 RID: 2979
		public List<int> trisYPos;

		// Token: 0x04000BA4 RID: 2980
		public List<int> trisYNeg;

		// Token: 0x04000BA5 RID: 2981
		public List<int> trisZPos;

		// Token: 0x04000BA6 RID: 2982
		public List<int> trisZNeg;

		// Token: 0x04000BA7 RID: 2983
		public Dictionary<int, int> subsetUsed;

		// Token: 0x04000BA8 RID: 2984
		public List<int> subsetUsedList;
	}

	// Token: 0x020001B5 RID: 437
	[PublicizedFrom(EAccessModifier.Private)]
	public enum PlaneAxis
	{
		// Token: 0x04000BAA RID: 2986
		X,
		// Token: 0x04000BAB RID: 2987
		Y,
		// Token: 0x04000BAC RID: 2988
		Z
	}

	// Token: 0x020001B6 RID: 438
	[PublicizedFrom(EAccessModifier.Private)]
	public struct PlanePick
	{
		// Token: 0x04000BAD RID: 2989
		public BlockShapeNew.PlaneAxis axis;

		// Token: 0x04000BAE RID: 2990
		public bool neg;
	}
}
