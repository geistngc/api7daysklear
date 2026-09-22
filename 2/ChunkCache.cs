using System;

// Token: 0x02000B10 RID: 2832
public class ChunkCache : IBlockAccess
{
	// Token: 0x0600556E RID: 21870 RVA: 0x0020B294 File Offset: 0x00209494
	public ChunkCache(int _dim)
	{
		this.chunkArray = new Chunk[_dim, _dim];
	}

	// Token: 0x0600556F RID: 21871 RVA: 0x0020B2AC File Offset: 0x002094AC
	public void Init(World world, int x, int y, int z, int sx, int sy, int sz)
	{
		this.worldObj = world;
		this.chunkX = x >> 4;
		this.chunkZ = z >> 4;
		int num = sx >> 4;
		int num2 = sz >> 4;
		for (int i = this.chunkX; i <= num; i++)
		{
			for (int j = this.chunkZ; j <= num2; j++)
			{
				Chunk chunkSync = world.ChunkCache.GetChunkSync(i, j);
				if (chunkSync != null)
				{
					this.chunkArray[i - this.chunkX, j - this.chunkZ] = chunkSync;
				}
			}
		}
	}

	// Token: 0x06005570 RID: 21872 RVA: 0x0020B32F File Offset: 0x0020952F
	public void Clear()
	{
		Array.Clear(this.chunkArray, 0, this.chunkArray.GetLength(0) * this.chunkArray.GetLength(1));
	}

	// Token: 0x06005571 RID: 21873 RVA: 0x0020B356 File Offset: 0x00209556
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsChunkValid(Chunk _chunk)
	{
		return _chunk != null && _chunk.IsInitialized;
	}

	// Token: 0x06005572 RID: 21874 RVA: 0x0020B364 File Offset: 0x00209564
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryGetChunk(int _cX, int _cY, out Chunk _chunk)
	{
		int num = _cX - this.chunkX;
		int num2 = _cY - this.chunkZ;
		Chunk chunk;
		if (num < 0 || num >= this.chunkArray.GetLength(0) || num2 < 0 || num2 >= this.chunkArray.GetLength(1))
		{
			chunk = this.worldObj.ChunkCache.GetChunkSync(_cX, _cY);
		}
		else
		{
			chunk = this.chunkArray[num, num2];
		}
		if (!this.IsChunkValid(chunk))
		{
			_chunk = null;
			return false;
		}
		_chunk = chunk;
		return true;
	}

	// Token: 0x06005573 RID: 21875 RVA: 0x0020B3E0 File Offset: 0x002095E0
	public BlockValue GetBlock(int _x, int _y, int _z)
	{
		if (_y < 0)
		{
			return BlockValue.Air;
		}
		if (_y >= 256)
		{
			return BlockValue.Air;
		}
		Chunk chunk;
		if (!this.TryGetChunk(World.toChunkXZ(_x), World.toChunkXZ(_z), out chunk))
		{
			return BlockValue.Air;
		}
		return chunk.GetBlock(_x & 15, _y, _z & 15);
	}

	// Token: 0x06005574 RID: 21876 RVA: 0x001B2C80 File Offset: 0x001B0E80
	public BlockValue GetBlock(Vector3i pos)
	{
		return IBlockAccess.DefaultGetBlock(this, pos);
	}

	// Token: 0x06005575 RID: 21877 RVA: 0x001B2C89 File Offset: 0x001B0E89
	public BlockValue GetBlock(BlockValueRef bvRef)
	{
		return IBlockAccess.DefaultGetBlock(this, bvRef);
	}

	// Token: 0x06005576 RID: 21878 RVA: 0x0020B430 File Offset: 0x00209630
	public PropValue GetProp(int chunkX, int chunkZ, int propId)
	{
		Chunk chunk;
		if (!this.TryGetChunk(chunkX, chunkZ, out chunk))
		{
			return PropValue.AIR;
		}
		return chunk.GetProp(chunkX, chunkZ, propId);
	}

	// Token: 0x06005577 RID: 21879 RVA: 0x0020B458 File Offset: 0x00209658
	public PropValue GetProp(long chunkKey, int propId)
	{
		return this.GetProp(WorldChunkCache.extractX(chunkKey), WorldChunkCache.extractZ(chunkKey), propId);
	}

	// Token: 0x06005578 RID: 21880 RVA: 0x001B2E15 File Offset: 0x001B1015
	public PropValue GetProp(Vector2i chunkPos, int propId)
	{
		return IBlockAccess.DefaultGetProp(this, chunkPos, propId);
	}

	// Token: 0x06005579 RID: 21881 RVA: 0x001B2E1F File Offset: 0x001B101F
	public PropValue GetProp(PropRef propRef)
	{
		return IBlockAccess.DefaultGetProp(this, propRef);
	}

	// Token: 0x04004247 RID: 16967
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkX;

	// Token: 0x04004248 RID: 16968
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkZ;

	// Token: 0x04004249 RID: 16969
	[PublicizedFrom(EAccessModifier.Private)]
	public Chunk[,] chunkArray;

	// Token: 0x0400424A RID: 16970
	[PublicizedFrom(EAccessModifier.Private)]
	public World worldObj;
}
