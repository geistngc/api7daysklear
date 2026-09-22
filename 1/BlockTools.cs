using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class BlockTools
{
	// Token: 0x06000D51 RID: 3409 RVA: 0x0005703C File Offset: 0x0005523C
	public static void PlaceTerrain(WorldBase _world, Vector3i _targetPos, BlockTools.Brush _brush, List<BlockChangeInfo> _blockChanges)
	{
		BlockValue block = _world.GetBlock(_targetPos);
		Vector3i[] cubesInBrush = _brush.GetCubesInBrush();
		for (int i = 0; i < cubesInBrush.Length; i++)
		{
			Vector3i vector3i = _targetPos + cubesInBrush[i];
			if (BlockTools.HasValidNeighbor(vector3i, 1, _world))
			{
				sbyte b = _world.GetDensity(vector3i);
				BlockValue block2 = _world.GetBlock(vector3i);
				bool flag = block2.Equals(BlockValue.Air);
				if (block2.Block.shape.IsRenderDecoration())
				{
					_blockChanges.Add(new BlockChangeInfo(vector3i, -1, false));
				}
				float num = Vector3.Distance(_targetPos.ToVector3(), vector3i.ToVector3());
				float num2 = (num <= (float)_brush.Size) ? 1f : (1f - num / (float)(_brush.Size + _brush.Falloff));
				if (b > 0 && !flag)
				{
					b = -1 * b;
				}
				else if (b == 0 && !flag)
				{
					b = -1;
				}
				float num3 = (float)_brush.Strength * num2;
				sbyte b2 = (sbyte)Mathf.Clamp((float)b - num3, -128f, 127f);
				if (b2 != b)
				{
					if (b2 >= 0 && !flag)
					{
						_blockChanges.Add(new BlockChangeInfo(vector3i, BlockValue.Air, b2));
					}
					else if (b2 <= -1 && flag)
					{
						_blockChanges.Add(new BlockChangeInfo(vector3i, block, b2));
					}
					else
					{
						_blockChanges.Add(new BlockChangeInfo(vector3i, block2, b2));
					}
				}
			}
		}
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x000571B0 File Offset: 0x000553B0
	public static void PaintTerrain(WorldBase _world, Vector3i _targetPos, BlockTools.Brush _brush, List<BlockChangeInfo> _blockChanges, BlockValue _paintBlock)
	{
		_world.GetBlock(_targetPos);
		Vector3i[] cubesInBrush = _brush.GetCubesInBrush();
		for (int i = 0; i < cubesInBrush.Length; i++)
		{
			Vector3i vector3i = _targetPos + cubesInBrush[i];
			sbyte density = _world.GetDensity(vector3i);
			BlockValue block = _world.GetBlock(vector3i);
			if (!block.Equals(BlockValue.Air) && block.Block.shape.IsTerrain() && _paintBlock.Block.shape.IsTerrain())
			{
				_blockChanges.Add(new BlockChangeInfo(vector3i, _paintBlock, density));
			}
		}
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x00057244 File Offset: 0x00055444
	public static void RemoveTerrain(WorldBase _world, Vector3i _targetPos, BlockTools.Brush _brush, List<BlockChangeInfo> _blockChanges)
	{
		BlockValue block = _world.GetBlock(_targetPos);
		Vector3i[] cubesInBrush = _brush.GetCubesInBrush();
		for (int i = 0; i < cubesInBrush.Length; i++)
		{
			Vector3i vector3i = _targetPos + cubesInBrush[i];
			if (BlockTools.HasValidNeighbor(vector3i, 1, _world))
			{
				sbyte density = _world.GetDensity(vector3i);
				BlockValue block2 = _world.GetBlock(vector3i);
				bool flag = block2.Equals(BlockValue.Air);
				float num = Vector3.Distance(_targetPos.ToVector3(), vector3i.ToVector3());
				float num2 = (num <= (float)_brush.Size) ? 1f : (1f - num / (float)(_brush.Size + _brush.Falloff));
				float num3 = (float)_brush.Strength * num2;
				sbyte b = (sbyte)Mathf.Clamp((float)density + num3, -128f, 127f);
				if (b != density)
				{
					if (b >= 0 && !flag)
					{
						_blockChanges.Add(new BlockChangeInfo(vector3i, BlockValue.Air, b));
					}
					else if (b < 0 && flag)
					{
						_blockChanges.Add(new BlockChangeInfo(vector3i, block, b));
					}
					else
					{
						_blockChanges.Add(new BlockChangeInfo(vector3i, block2, b));
					}
				}
			}
		}
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00057374 File Offset: 0x00055574
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool HasValidNeighbor(Vector3i _pos, sbyte _denThreshold, WorldBase _world)
	{
		if (_denThreshold < 0)
		{
			return _world.GetDensity(_pos.x - 1, _pos.y, _pos.z) <= _denThreshold || _world.GetDensity(_pos.x + 1, _pos.y, _pos.z) <= _denThreshold || _world.GetDensity(_pos.x, _pos.y - 1, _pos.z) <= _denThreshold || _world.GetDensity(_pos.x, _pos.y + 1, _pos.z) <= _denThreshold || _world.GetDensity(_pos.x, _pos.y, _pos.z - 1) <= _denThreshold || _world.GetDensity(_pos.x, _pos.y, _pos.z + 1) <= _denThreshold;
		}
		return _denThreshold >= 0 && (_world.GetDensity(_pos.x - 1, _pos.y, _pos.z) >= _denThreshold || _world.GetDensity(_pos.x + 1, _pos.y, _pos.z) >= _denThreshold || _world.GetDensity(_pos.x, _pos.y - 1, _pos.z) >= _denThreshold || _world.GetDensity(_pos.x, _pos.y + 1, _pos.z) >= _denThreshold || _world.GetDensity(_pos.x, _pos.y, _pos.z - 1) >= _denThreshold || _world.GetDensity(_pos.x, _pos.y, _pos.z + 1) >= _denThreshold);
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00057508 File Offset: 0x00055708
	public static void CubeRPC(GameManager _gm, Vector3i pos1, Vector3i pos2, BlockValue _blockValue, sbyte _density, int _Fillmode, TextureFullArray _textureFull)
	{
		List<BlockChangeInfo> list = new List<BlockChangeInfo>();
		int num = pos1.y;
		for (;;)
		{
			int num2 = pos1.x;
			for (;;)
			{
				int num3 = pos1.z;
				for (;;)
				{
					if (_Fillmode == 0 || (_Fillmode == 1 && (num == pos1.y || num2 == pos1.x || num3 == pos1.z || num == pos2.y || num2 == pos2.x || num3 == pos2.z)) || (_Fillmode == 2 && ((num2 == pos1.x && num == pos1.y) || (num == pos1.y && num3 == pos1.z) || (num2 == pos1.x && num3 == pos1.z) || (num2 == pos2.x && num == pos2.y) || (num == pos2.y && num3 == pos2.z) || (num2 == pos2.x && num3 == pos2.z) || (num2 == pos1.x && num == pos2.y) || (num == pos1.y && num2 == pos2.x) || (num2 == pos1.x && num3 == pos2.z) || (num3 == pos1.z && num2 == pos2.x) || (num == pos1.y && num3 == pos2.z) || (num == pos2.y && num3 == pos1.z))))
					{
						list.Add(new BlockChangeInfo(new Vector3i(num2, num, num3), _blockValue, _density)
						{
							textureFull = _textureFull,
							bChangeTexture = true
						});
					}
					if (num3 == pos2.z)
					{
						break;
					}
					num3 += Math.Sign(pos2.z - pos1.z);
				}
				if (num2 == pos2.x)
				{
					break;
				}
				num2 += Math.Sign(pos2.x - pos1.x);
			}
			if (num == pos2.y)
			{
				break;
			}
			num += Math.Sign(pos2.y - pos1.y);
		}
		_gm.SetBlocksRPC(list, null);
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x00057714 File Offset: 0x00055914
	public static void CubeWaterRPC(GameManager _gm, Vector3i _pos1, Vector3i _pos2, WaterValue _waterValue)
	{
		NetPackageWaterSet package = NetPackageManager.GetPackage<NetPackageWaterSet>();
		int num = _pos1.y;
		for (;;)
		{
			int num2 = _pos1.x;
			for (;;)
			{
				int num3 = _pos1.z;
				for (;;)
				{
					if (WaterUtils.CanWaterFlowThrough(_gm.World.GetBlock(num2, num, num3)))
					{
						Vector3i worldPos = new Vector3i(num2, num, num3);
						package.AddChange(worldPos, _waterValue);
					}
					if (num3 == _pos2.z)
					{
						break;
					}
					num3 += Math.Sign(_pos2.z - _pos1.z);
				}
				if (num2 == _pos2.x)
				{
					break;
				}
				num2 += Math.Sign(_pos2.x - _pos1.x);
			}
			if (num == _pos2.y)
			{
				break;
			}
			num += Math.Sign(_pos2.y - _pos1.y);
		}
		_gm.SetWaterRPC(package);
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x000577D0 File Offset: 0x000559D0
	public static void CubeDensityRPC(GameManager _gm, Vector3i pos1, Vector3i pos2, sbyte _density)
	{
		List<BlockChangeInfo> list = new List<BlockChangeInfo>();
		int num = Math.Sign(pos2.x - pos1.x);
		int num2 = Math.Sign(pos2.y - pos1.y);
		int num3 = Math.Sign(pos2.z - pos1.z);
		int num4 = pos1.y;
		for (;;)
		{
			int num5 = pos1.x;
			for (;;)
			{
				int num6 = pos1.z;
				for (;;)
				{
					BlockChangeInfo item = new BlockChangeInfo(new Vector3i(num5, num4, num6), _density, false);
					list.Add(item);
					if (num6 == pos2.z)
					{
						break;
					}
					num6 += num3;
				}
				if (num5 == pos2.x)
				{
					break;
				}
				num5 += num;
			}
			if (num4 == pos2.y)
			{
				break;
			}
			num4 += num2;
		}
		_gm.SetBlocksRPC(list, null);
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x00057894 File Offset: 0x00055A94
	public static void CubeRandomRPC(GameManager _gm, Vector3i pos1, Vector3i pos2, BlockValue _blockValue, float _rnd, EBlockRotationClasses? _randomRotation = null)
	{
		List<BlockChangeInfo> list = new List<BlockChangeInfo>();
		GameRandom gameRandom = _gm.World.GetGameRandom();
		int num = pos1.y;
		for (;;)
		{
			int num2 = pos1.x;
			for (;;)
			{
				int num3 = pos1.z;
				for (;;)
				{
					if (gameRandom.RandomFloat < _rnd)
					{
						Vector3i vector3i = new Vector3i(num2, num, num3);
						if (_randomRotation != null)
						{
							_blockValue.rotation = BlockTools.GetRandomBlockRotation(_randomRotation.Value, gameRandom);
						}
						_blockValue = _blockValue.Block.OnBlockPlaced(_gm.World, vector3i, _blockValue, gameRandom);
						BlockChangeInfo item = new BlockChangeInfo(vector3i, _blockValue);
						list.Add(item);
					}
					if (num3 == pos2.z)
					{
						break;
					}
					num3 += Math.Sign(pos2.z - pos1.z);
				}
				if (num2 == pos2.x)
				{
					break;
				}
				num2 += Math.Sign(pos2.x - pos1.x);
			}
			if (num == pos2.y)
			{
				break;
			}
			num += Math.Sign(pos2.y - pos1.y);
		}
		_gm.SetBlocksRPC(list, null);
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x000579A0 File Offset: 0x00055BA0
	public static byte GetMaxBlockRotations(EBlockRotationClasses _rotations)
	{
		byte result = 28;
		if ((_rotations & EBlockRotationClasses.Basic45) == EBlockRotationClasses.None)
		{
			result = 24;
		}
		if ((_rotations & EBlockRotationClasses.Sideways) == EBlockRotationClasses.None)
		{
			result = 8;
		}
		if ((_rotations & EBlockRotationClasses.Headfirst) == EBlockRotationClasses.None)
		{
			result = 4;
		}
		return result;
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x000579C8 File Offset: 0x00055BC8
	public static byte GetRandomBlockRotation(EBlockRotationClasses _rotations, GameRandom _rnd = null)
	{
		if (_rnd == null)
		{
			_rnd = GameManager.Instance.World.GetGameRandom();
		}
		int num = _rnd.RandomRange(28);
		if (num >= 24 && (_rotations & EBlockRotationClasses.Basic45) == EBlockRotationClasses.None)
		{
			num %= 24;
		}
		if (num >= 8 && (_rotations & EBlockRotationClasses.Sideways) == EBlockRotationClasses.None)
		{
			num %= 8;
		}
		if (num >= 4 && (_rotations & EBlockRotationClasses.Headfirst) == EBlockRotationClasses.None)
		{
			num %= 4;
		}
		return (byte)num;
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x00057A20 File Offset: 0x00055C20
	public static Prefab CopyIntoStorage(GameManager _gm, Vector3i pos1, Vector3i pos2)
	{
		Prefab prefab = new Prefab(new Vector3i(Math.Abs(pos1.x - pos2.x) + 1, Math.Abs(pos1.y - pos2.y) + 1, Math.Abs(pos1.z - pos2.z) + 1));
		int num = Math.Min(pos1.x, pos2.x);
		int num2 = Math.Max(pos1.x, pos2.x);
		int num3 = Math.Min(pos1.y, pos2.y);
		int num4 = Math.Max(pos1.y, pos2.y);
		int num5 = Math.Min(pos1.z, pos2.z);
		int num6 = Math.Max(pos1.z, pos2.z);
		int num7 = 0;
		int i = num3;
		while (i <= num4)
		{
			int num8 = 0;
			int j = num;
			while (j <= num2)
			{
				int num9 = 0;
				int k = num5;
				while (k <= num6)
				{
					BlockValue bv = _gm.World.GetBlock(j, i, k);
					if (bv.isWater)
					{
						bv = BlockValue.Air;
					}
					prefab.SetBlock(num8, num7, num9, bv);
					WaterValue water = _gm.World.GetWater(j, i, k);
					prefab.SetWater(num8, num7, num9, water);
					TextureFullArray textureFullArray = _gm.World.GetTextureFullArray(j, i, k);
					prefab.SetTexture(num8, num7, num9, textureFullArray);
					sbyte density = _gm.World.GetDensity(j, i, k);
					prefab.SetDensity(num8, num7, num9, density);
					k++;
					num9++;
				}
				j++;
				num8++;
			}
			i++;
			num7++;
		}
		return prefab;
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x00057BD4 File Offset: 0x00055DD4
	public static void ClearRPC(World _world, Vector3i _pos, int _xd, int _yd, int _zd, bool _bClearLight)
	{
		List<BlockChangeInfo> list = new List<BlockChangeInfo>();
		for (int i = _pos.x; i < _pos.x + _xd; i++)
		{
			for (int j = _pos.z; j < _pos.z + _zd; j++)
			{
				if ((Chunk)_world.GetChunkFromWorldPos(i, 0, j) != null)
				{
					for (int k = _pos.y; k < _pos.y + _yd; k++)
					{
						World.toBlockXZ(i);
						World.toBlockXZ(j);
						BlockChangeInfo blockChangeInfo = new BlockChangeInfo(new Vector3i(i, k, j), BlockValue.Air, 0);
						blockChangeInfo.textureFull.Fill(0L);
						blockChangeInfo.bChangeTexture = true;
						list.Add(blockChangeInfo);
					}
				}
			}
		}
		_world.SetBlocksRPC(list);
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00057C94 File Offset: 0x00055E94
	public static bool IsCubeBorderEmpty(World _world, Vector3i _pos, Vector3i _size)
	{
		for (int i = 0; i <= _size.x; i++)
		{
			for (int j = 0; j < _size.z; j++)
			{
				int num = _pos.x + i;
				int num2 = _pos.z + j;
				Chunk chunk = (Chunk)_world.GetChunkFromWorldPos(num, 0, num2);
				if (chunk != null)
				{
					for (int k = 0; k <= _size.y; k++)
					{
						if (i == 0 || i == _size.x || k == 0 || k == _size.y || j == 0 || j == _size.z)
						{
							int y = _pos.y + k;
							if (!chunk.GetBlock(World.toBlockXZ(num), y, World.toBlockXZ(num2)).isair)
							{
								return false;
							}
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x020001BC RID: 444
	public class Brush
	{
		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000D5F RID: 3423 RVA: 0x00057D5E File Offset: 0x00055F5E
		// (set) Token: 0x06000D60 RID: 3424 RVA: 0x00057D66 File Offset: 0x00055F66
		public int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
				this.bRecalcNeeded = true;
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000D61 RID: 3425 RVA: 0x00057D76 File Offset: 0x00055F76
		public int SizeHalf
		{
			get
			{
				return this.size / 2;
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000D62 RID: 3426 RVA: 0x00057D80 File Offset: 0x00055F80
		// (set) Token: 0x06000D63 RID: 3427 RVA: 0x00057D88 File Offset: 0x00055F88
		public int Strength
		{
			get
			{
				return this.strength;
			}
			set
			{
				this.strength = value;
				this.bRecalcNeeded = true;
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000D64 RID: 3428 RVA: 0x00057D98 File Offset: 0x00055F98
		// (set) Token: 0x06000D65 RID: 3429 RVA: 0x00057DA0 File Offset: 0x00055FA0
		public int Falloff
		{
			get
			{
				return this.falloff;
			}
			set
			{
				this.falloff = value;
				this.bRecalcNeeded = true;
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000D66 RID: 3430 RVA: 0x00057DB0 File Offset: 0x00055FB0
		// (set) Token: 0x06000D67 RID: 3431 RVA: 0x00057DB8 File Offset: 0x00055FB8
		public BlockTools.Brush.BrushShape Shape
		{
			get
			{
				return this.shape;
			}
			set
			{
				this.shape = value;
				this.bRecalcNeeded = true;
			}
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x00057DC8 File Offset: 0x00055FC8
		public Brush(BlockTools.Brush.BrushShape _shape, int _size, int _falloff, int _strength)
		{
			this.shape = _shape;
			this.size = _size;
			this.strength = _strength;
			this.falloff = _falloff;
			this.bRecalcNeeded = true;
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x00057E04 File Offset: 0x00056004
		public Vector3i[] GetCubesInBrush()
		{
			if (this.bRecalcNeeded)
			{
				if (this.shape == BlockTools.Brush.BrushShape.Sphere)
				{
					this.CalcCubesInSphere();
				}
				else if (this.shape == BlockTools.Brush.BrushShape.Cube)
				{
					this.CalcCubesInCube();
				}
				else
				{
					this.positions = new Vector3i[0];
				}
				this.bRecalcNeeded = false;
			}
			return this.positions;
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x00057E54 File Offset: 0x00056054
		[PublicizedFrom(EAccessModifier.Private)]
		public void CalcCubesInSphere()
		{
			List<Vector3i> list = new List<Vector3i>();
			int num = this.size + this.falloff;
			for (int i = -num; i < num; i++)
			{
				for (int j = -num; j < num; j++)
				{
					for (int k = -num; k < num; k++)
					{
						Vector3i item = new Vector3i(j, i, k);
						if (Vector3.Distance(Vector3.zero, item.ToVector3()) <= (float)(this.size + this.falloff))
						{
							list.Add(item);
						}
					}
				}
			}
			this.positions = list.ToArray();
		}

		// Token: 0x06000D6B RID: 3435 RVA: 0x00057EE0 File Offset: 0x000560E0
		[PublicizedFrom(EAccessModifier.Private)]
		public void CalcCubesInCube()
		{
			List<Vector3i> list = new List<Vector3i>();
			int num = this.size + this.falloff;
			for (int i = -num; i < num; i++)
			{
				for (int j = -num; j < num; j++)
				{
					for (int k = -num; k < num; k++)
					{
						list.Add(new Vector3i(j, i, k));
					}
				}
			}
			this.positions = list.ToArray();
		}

		// Token: 0x04000BB7 RID: 2999
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3i[] positions;

		// Token: 0x04000BB8 RID: 3000
		[PublicizedFrom(EAccessModifier.Private)]
		public int size = 1;

		// Token: 0x04000BB9 RID: 3001
		[PublicizedFrom(EAccessModifier.Private)]
		public int strength = 1;

		// Token: 0x04000BBA RID: 3002
		[PublicizedFrom(EAccessModifier.Private)]
		public int falloff;

		// Token: 0x04000BBB RID: 3003
		[PublicizedFrom(EAccessModifier.Private)]
		public BlockTools.Brush.BrushShape shape;

		// Token: 0x04000BBC RID: 3004
		[PublicizedFrom(EAccessModifier.Private)]
		public bool bRecalcNeeded;

		// Token: 0x020001BD RID: 445
		public enum BrushShape
		{
			// Token: 0x04000BBE RID: 3006
			Sphere,
			// Token: 0x04000BBF RID: 3007
			Cube
		}
	}
}
