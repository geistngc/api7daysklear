using System;
using System.IO;

// Token: 0x02000B8A RID: 2954
[PublicizedFrom(EAccessModifier.Internal)]
public class ChunkMemoryStreamWriter : MemoryStream
{
	// Token: 0x06005980 RID: 22912 RVA: 0x00224516 File Offset: 0x00222716
	public ChunkMemoryStreamWriter() : this(new byte[512000])
	{
	}

	// Token: 0x06005981 RID: 22913 RVA: 0x00224528 File Offset: 0x00222728
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkMemoryStreamWriter(byte[] _buffer) : base(_buffer)
	{
		this.buffer = _buffer;
	}

	// Token: 0x06005982 RID: 22914 RVA: 0x00224538 File Offset: 0x00222738
	public void Init(RegionFileAccessMultipleChunks _regionFileAccess, string _dir, int _x, int _z, string _ext)
	{
		this.dir = _dir;
		this.chunkX = _x;
		this.chunkZ = _z;
		this.ext = _ext;
		this.regionFileAccess = _regionFileAccess;
		this.Seek(0L, SeekOrigin.Begin);
	}

	// Token: 0x06005983 RID: 22915 RVA: 0x00224569 File Offset: 0x00222769
	public override void Close()
	{
		this.regionFileAccess.Write(this.dir, this.chunkX, this.chunkZ, this.ext, this.buffer, (int)this.Position);
	}

	// Token: 0x04004570 RID: 17776
	[PublicizedFrom(EAccessModifier.Private)]
	public string dir;

	// Token: 0x04004571 RID: 17777
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkX;

	// Token: 0x04004572 RID: 17778
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkZ;

	// Token: 0x04004573 RID: 17779
	[PublicizedFrom(EAccessModifier.Private)]
	public string ext;

	// Token: 0x04004574 RID: 17780
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileAccessMultipleChunks regionFileAccess;

	// Token: 0x04004575 RID: 17781
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] buffer;
}
