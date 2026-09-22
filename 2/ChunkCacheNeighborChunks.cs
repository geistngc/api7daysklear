using System;

// Token: 0x02000B13 RID: 2835
public class ChunkCacheNeighborChunks
{
	// Token: 0x06005590 RID: 21904 RVA: 0x0020BC04 File Offset: 0x00209E04
	public ChunkCacheNeighborChunks(IChunkAccess _chunkAccess)
	{
		IChunk[,] array = new Chunk[3, 3];
		this.chunks = array;
		base..ctor();
		this.chunkAccess = _chunkAccess;
	}

	// Token: 0x06005591 RID: 21905 RVA: 0x0020BC30 File Offset: 0x00209E30
	public void Init(IChunk _chunk, IChunk[] _chunkArr)
	{
		this[0, 0] = _chunk;
		this[-1, 0] = _chunkArr[1];
		this[1, 0] = _chunkArr[0];
		this[0, -1] = _chunkArr[3];
		this[0, 1] = _chunkArr[2];
		this[-1, -1] = _chunkArr[5];
		this[1, -1] = _chunkArr[7];
		this[-1, 1] = _chunkArr[6];
		this[1, 1] = _chunkArr[4];
	}

	// Token: 0x06005592 RID: 21906 RVA: 0x0020BC9E File Offset: 0x00209E9E
	public void Clear()
	{
		Array.Clear(this.chunks, 0, this.chunks.GetLength(0) * this.chunks.GetLength(1));
	}

	// Token: 0x17000926 RID: 2342
	public IChunk this[int x, int y]
	{
		get
		{
			return this.chunks[x + 1, y + 1];
		}
		set
		{
			this.chunks[x + 1, y + 1] = value;
		}
	}

	// Token: 0x04004252 RID: 16978
	[PublicizedFrom(EAccessModifier.Private)]
	public IChunk[,] chunks;

	// Token: 0x04004253 RID: 16979
	[PublicizedFrom(EAccessModifier.Private)]
	public IChunkAccess chunkAccess;
}
