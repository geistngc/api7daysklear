using System;
using System.Collections.Generic;

// Token: 0x02000376 RID: 886
public class DynamicMeshServerUpdates
{
	// Token: 0x06001A32 RID: 6706 RVA: 0x0009B0DC File Offset: 0x000992DC
	public static void AddToPool(DynamicMeshServerUpdates data)
	{
		if (DynamicMeshServerUpdates.Pool.Count > 40)
		{
			data.Bytes = null;
			return;
		}
		DynamicMeshServerUpdates.Pool.Enqueue(data);
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x0009B0FF File Offset: 0x000992FF
	public static DynamicMeshServerUpdates GetFromPool()
	{
		if (DynamicMeshServerUpdates.Pool.Count > 0)
		{
			return DynamicMeshServerUpdates.Pool.Dequeue();
		}
		return new DynamicMeshServerUpdates();
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x0009B11E File Offset: 0x0009931E
	public long GetKey()
	{
		return WorldChunkCache.MakeChunkKey(World.toChunkXZ(DynamicMeshUnity.RoundChunk(this.ChunkX)), World.toChunkXZ(DynamicMeshUnity.RoundChunk(this.ChunkZ)));
	}

	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06001A35 RID: 6709 RVA: 0x0009B145 File Offset: 0x00099345
	// (set) Token: 0x06001A36 RID: 6710 RVA: 0x0009B14D File Offset: 0x0009934D
	public int ChunkX { get; set; }

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06001A37 RID: 6711 RVA: 0x0009B156 File Offset: 0x00099356
	// (set) Token: 0x06001A38 RID: 6712 RVA: 0x0009B15E File Offset: 0x0009935E
	public int ChunkZ { get; set; }

	// Token: 0x17000318 RID: 792
	// (get) Token: 0x06001A39 RID: 6713 RVA: 0x0009B167 File Offset: 0x00099367
	// (set) Token: 0x06001A3A RID: 6714 RVA: 0x0009B16F File Offset: 0x0009936F
	public int StartY { get; set; }

	// Token: 0x17000319 RID: 793
	// (get) Token: 0x06001A3B RID: 6715 RVA: 0x0009B178 File Offset: 0x00099378
	// (set) Token: 0x06001A3C RID: 6716 RVA: 0x0009B180 File Offset: 0x00099380
	public int EndY { get; set; }

	// Token: 0x1700031A RID: 794
	// (get) Token: 0x06001A3D RID: 6717 RVA: 0x0009B189 File Offset: 0x00099389
	// (set) Token: 0x06001A3E RID: 6718 RVA: 0x0009B191 File Offset: 0x00099391
	public int UpdateTime { get; set; }

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x06001A3F RID: 6719 RVA: 0x0009B19A File Offset: 0x0009939A
	// (set) Token: 0x06001A40 RID: 6720 RVA: 0x0009B1A2 File Offset: 0x000993A2
	public List<byte> Bytes { get; set; }

	// Token: 0x06001A41 RID: 6721 RVA: 0x0009B1AB File Offset: 0x000993AB
	public DynamicMeshServerUpdates()
	{
		this.Bytes = new List<byte>();
	}

	// Token: 0x06001A42 RID: 6722 RVA: 0x0009B1BE File Offset: 0x000993BE
	public void WriteAir(List<byte> tempArray)
	{
		tempArray.Add(0);
	}

	// Token: 0x06001A43 RID: 6723 RVA: 0x0009B1C7 File Offset: 0x000993C7
	public void WriteLayer(List<byte> tempArray)
	{
		this.DataLayerCount++;
		this.Bytes.Add(byte.MaxValue);
		this.Bytes.AddRange(tempArray);
	}

	// Token: 0x06001A44 RID: 6724 RVA: 0x0009B1F3 File Offset: 0x000993F3
	public void WriteEmptyLayer()
	{
		this.EmptyLayerCount++;
		this.Bytes.Add(128);
	}

	// Token: 0x06001A45 RID: 6725 RVA: 0x0009B214 File Offset: 0x00099414
	public void WriteBinaryBlock(List<byte> tempArray, BlockValue b, sbyte dens, long tex)
	{
		if (b.isair)
		{
			tempArray.Add(0);
			return;
		}
		byte[] bytes;
		if (!DynamicMeshServerUpdates.BlockBytes.TryGetValue(b.rawData, out bytes))
		{
			bytes = BitConverter.GetBytes(b.type);
			DynamicMeshServerUpdates.BlockBytes.Add(b.rawData, bytes);
		}
		tempArray.AddRange(bytes);
		if (!DynamicMeshServerUpdates.TexBytes.TryGetValue(tex, out bytes))
		{
			bytes = BitConverter.GetBytes(tex);
			DynamicMeshServerUpdates.TexBytes.Add(tex, bytes);
		}
		tempArray.AddRange(bytes);
		tempArray.Add((byte)dens);
	}

	// Token: 0x040010B5 RID: 4277
	public const int DataLayer = 255;

	// Token: 0x040010B6 RID: 4278
	public const byte EmptyLayer = 128;

	// Token: 0x040010B7 RID: 4279
	public const int EmptyBlock = 0;

	// Token: 0x040010B8 RID: 4280
	[PublicizedFrom(EAccessModifier.Private)]
	public static Queue<DynamicMeshServerUpdates> Pool = new Queue<DynamicMeshServerUpdates>(20);

	// Token: 0x040010B9 RID: 4281
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<uint, byte[]> BlockBytes = new Dictionary<uint, byte[]>();

	// Token: 0x040010BA RID: 4282
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<long, byte[]> TexBytes = new Dictionary<long, byte[]>();

	// Token: 0x040010C1 RID: 4289
	public int EmptyLayerCount;

	// Token: 0x040010C2 RID: 4290
	public int DataLayerCount;
}
