using System;

// Token: 0x02000B12 RID: 2834
public class ChunkCacheNeighborBlocks : INeighborBlockCache
{
	// Token: 0x06005583 RID: 21891 RVA: 0x0020B470 File Offset: 0x00209670
	public ChunkCacheNeighborBlocks(ChunkCacheNeighborChunks _nChunks)
	{
		IChunk[] array = new Chunk[9];
		this.chunks = array;
		this.centerBX = int.MaxValue;
		this.centerBZ = int.MaxValue;
		this.curChunkX = int.MaxValue;
		this.curChunkZ = int.MaxValue;
		this.firstException = true;
		base..ctor();
		this.nChunks = _nChunks;
	}

	// Token: 0x06005584 RID: 21892 RVA: 0x0020B4CC File Offset: 0x002096CC
	public void Init(int _bX, int _bZ)
	{
		IChunk chunk = this.nChunks[0, 0];
		if (_bX == this.centerBX && _bZ == this.centerBZ && chunk.X == this.curChunkX && chunk.Z == this.curChunkZ)
		{
			return;
		}
		this.centerBX = _bX;
		this.centerBZ = _bZ;
		this.curChunkX = chunk.X;
		this.curChunkZ = chunk.Z;
		this.chunks[0] = ((_bX > 0) ? ((_bZ > 0) ? chunk : this.nChunks[0, -1]) : ((_bZ > 0) ? this.nChunks[-1, 0] : this.nChunks[-1, -1]));
		this.chunks[1] = ((_bZ > 0) ? chunk : this.nChunks[0, -1]);
		this.chunks[2] = ((_bX < 15) ? ((_bZ > 0) ? chunk : this.nChunks[0, -1]) : ((_bZ > 0) ? this.nChunks[1, 0] : this.nChunks[1, -1]));
		this.chunks[3] = ((_bX > 0) ? chunk : this.nChunks[-1, 0]);
		this.chunks[4] = chunk;
		this.chunks[5] = ((_bX < 15) ? chunk : this.nChunks[1, 0]);
		this.chunks[6] = ((_bX > 0) ? ((_bZ < 15) ? chunk : this.nChunks[0, 1]) : ((_bZ < 15) ? this.nChunks[-1, 0] : this.nChunks[-1, 1]));
		this.chunks[7] = ((_bZ < 15) ? chunk : this.nChunks[0, 1]);
		this.chunks[8] = ((_bX < 15) ? ((_bZ < 15) ? chunk : this.nChunks[0, 1]) : ((_bZ < 15) ? this.nChunks[1, 0] : this.nChunks[1, 1]));
	}

	// Token: 0x06005585 RID: 21893 RVA: 0x0020B6C8 File Offset: 0x002098C8
	public void Clear()
	{
		this.centerBX = int.MaxValue;
		this.centerBZ = int.MaxValue;
		this.curChunkX = int.MaxValue;
		this.curChunkZ = int.MaxValue;
		Array.Clear(this.chunks, 0, this.chunks.Length);
	}

	// Token: 0x06005586 RID: 21894 RVA: 0x0020B715 File Offset: 0x00209915
	public IChunk GetChunk(int x, int z)
	{
		return this.chunks[x + 1 + (z + 1) * 3];
	}

	// Token: 0x06005587 RID: 21895 RVA: 0x0020B727 File Offset: 0x00209927
	public IChunk GetNeighborChunk(int x, int z)
	{
		return this.nChunks[x, z];
	}

	// Token: 0x06005588 RID: 21896 RVA: 0x0020B738 File Offset: 0x00209938
	public bool IsBlockInCache(int relx, int absy, int relz)
	{
		if (absy < 0 || absy > 255)
		{
			return false;
		}
		int num = relx + 1 + (relz + 1) * 3;
		return num <= 8 && this.chunks[num] != null;
	}

	// Token: 0x06005589 RID: 21897 RVA: 0x0020B76E File Offset: 0x0020996E
	public Vector3i GetChunkPos(int relx, int absy, int relz)
	{
		return new Vector3i(this.centerBX + relx & 15, absy, this.centerBZ + relz & 15);
	}

	// Token: 0x0600558A RID: 21898 RVA: 0x0020B78C File Offset: 0x0020998C
	public BlockValue Get(int absy)
	{
		if (absy < 0)
		{
			return BlockValue.Air;
		}
		BlockValue result;
		try
		{
			BlockValue blockValue = this.chunks[4].GetBlock(this.centerBX & 15, absy, this.centerBZ & 15);
			if (!GameManager.bShowDecorBlocks && blockValue.Block.IsDecoration)
			{
				blockValue = BlockValue.Air;
			}
			if (!GameManager.bShowLootBlocks)
			{
				BlockCompositeTileEntity blockCompositeTileEntity = blockValue.Block as BlockCompositeTileEntity;
				if (blockCompositeTileEntity != null && blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>())
				{
					blockValue = BlockValue.Air;
				}
			}
			result = blockValue;
		}
		catch (Exception)
		{
			if (this.firstException)
			{
				Log.Error("ChunkCacheNeighborBlocks.Get: len=" + this.chunks.Length.ToString());
				this.firstException = false;
			}
			result = BlockValue.Air;
		}
		return result;
	}

	// Token: 0x0600558B RID: 21899 RVA: 0x0020B858 File Offset: 0x00209A58
	public BlockValue Get(int relx, int absy, int relz)
	{
		int num = relx + 1 + (relz + 1) * 3;
		if (absy < 0 || num >= 9)
		{
			return BlockValue.Air;
		}
		BlockValue result;
		try
		{
			BlockValue blockValue = this.chunks[num].GetBlock(this.centerBX + relx & 15, absy, this.centerBZ + relz & 15);
			if (!GameManager.bShowDecorBlocks && blockValue.Block.IsDecoration)
			{
				blockValue = BlockValue.Air;
			}
			if (!GameManager.bShowLootBlocks)
			{
				BlockCompositeTileEntity blockCompositeTileEntity = blockValue.Block as BlockCompositeTileEntity;
				if (blockCompositeTileEntity != null && blockCompositeTileEntity.CompositeData.HasFeature<ITileEntityLootable>())
				{
					blockValue = BlockValue.Air;
				}
			}
			result = blockValue;
		}
		catch (Exception)
		{
			if (this.firstException)
			{
				Log.Error(string.Concat(new string[]
				{
					"ChunkCacheNeighborBlocks.Get: relX=",
					relx.ToString(),
					" relz=",
					relz.ToString(),
					" len=",
					this.chunks.Length.ToString()
				}));
				this.firstException = false;
			}
			result = BlockValue.Air;
		}
		return result;
	}

	// Token: 0x0600558C RID: 21900 RVA: 0x0020B968 File Offset: 0x00209B68
	public byte GetStab(int relx, int absy, int relz)
	{
		byte result = 0;
		try
		{
			result = this.chunks[relx + 1 + (relz + 1) * 3].GetStability(this.centerBX + relx & 15, absy, this.centerBZ + relz & 15);
		}
		catch (Exception ex)
		{
			Log.Out(string.Concat(new string[]
			{
				"Bad ChunkCacheNeighborBlocks index (",
				relx.ToString(),
				", ",
				absy.ToString(),
				", ",
				relz.ToString(),
				"), \nException: ",
				ex.ToString()
			}));
		}
		return result;
	}

	// Token: 0x0600558D RID: 21901 RVA: 0x0020BA14 File Offset: 0x00209C14
	public bool IsWater(int relx, int absy, int relz)
	{
		int num = relx + 1 + (relz + 1) * 3;
		if (absy < 0 || num >= 9)
		{
			return false;
		}
		bool result = false;
		try
		{
			result = this.chunks[num].IsWater(this.centerBX + relx & 15, absy, this.centerBZ + relz & 15);
		}
		catch (Exception ex)
		{
			Log.Out(string.Concat(new string[]
			{
				"Bad ChunkCacheNeighborBlocks index (",
				relx.ToString(),
				", ",
				absy.ToString(),
				", ",
				relz.ToString(),
				"), \nException: ",
				ex.ToString()
			}));
		}
		return result;
	}

	// Token: 0x0600558E RID: 21902 RVA: 0x0020BACC File Offset: 0x00209CCC
	public bool IsAir(int relx, int absy, int relz)
	{
		int num = relx + 1 + (relz + 1) * 3;
		if (absy < 0 || num >= 9)
		{
			return false;
		}
		int x = this.centerBX + relx & 15;
		int z = this.centerBZ + relz & 15;
		bool result = false;
		try
		{
			IChunk chunk = this.chunks[num];
			return chunk.GetBlock(x, absy, z).isair && !chunk.IsWater(x, absy, z);
		}
		catch (Exception ex)
		{
			Log.Out(string.Concat(new string[]
			{
				"Bad ChunkCacheNeighborBlocks index (",
				relx.ToString(),
				", ",
				absy.ToString(),
				", ",
				relz.ToString(),
				"), \nException: ",
				ex.ToString()
			}));
		}
		return result;
	}

	// Token: 0x0600558F RID: 21903 RVA: 0x0020BBAC File Offset: 0x00209DAC
	public override string ToString()
	{
		return string.Format("BlockCache -- Chunk Pos ({0}, {1}) -- Block Pos ({2}, {3})", new object[]
		{
			this.curChunkX,
			this.curChunkZ,
			this.centerBX,
			this.centerBZ
		});
	}

	// Token: 0x0400424B RID: 16971
	[PublicizedFrom(EAccessModifier.Private)]
	public IChunk[] chunks;

	// Token: 0x0400424C RID: 16972
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkCacheNeighborChunks nChunks;

	// Token: 0x0400424D RID: 16973
	[PublicizedFrom(EAccessModifier.Private)]
	public int centerBX;

	// Token: 0x0400424E RID: 16974
	[PublicizedFrom(EAccessModifier.Private)]
	public int centerBZ;

	// Token: 0x0400424F RID: 16975
	[PublicizedFrom(EAccessModifier.Private)]
	public int curChunkX;

	// Token: 0x04004250 RID: 16976
	[PublicizedFrom(EAccessModifier.Private)]
	public int curChunkZ;

	// Token: 0x04004251 RID: 16977
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstException;
}
