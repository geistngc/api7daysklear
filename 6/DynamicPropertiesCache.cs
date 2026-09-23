using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using MemoryPack;
using Platform;

// Token: 0x020013F4 RID: 5108
public class DynamicPropertiesCache
{
	// Token: 0x0600A066 RID: 41062 RVA: 0x003C6EEC File Offset: 0x003C50EC
	public DynamicPropertiesCache()
	{
		Log.Out(string.Format("[BLOCKPROPERTIES] Creating DynamicProperties Cache, max cache size {0}", 1000));
		this.m_filePath = PlatformManager.NativePlatform.Utils.GetTempFileName("dpc", ".dpc");
		this.m_fileStream = new FileStream(this.m_filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);
		this.m_buffer = new ArrayBufferWriter<byte>(65536);
		this.m_cache = new Dictionary<int, DynamicProperties>(1000);
		this.m_queue = new LinkedList<int>();
		this.offsetsAndLengths = new ValueTuple<long, int>[Block.MAX_BLOCKS];
	}

	// Token: 0x0600A067 RID: 41063 RVA: 0x003C6F9C File Offset: 0x003C519C
	public void Cleanup()
	{
		this.m_cache.Clear();
		this.m_cache = null;
		this.m_queue.Clear();
		this.m_queue = null;
		this.m_buffer.Clear();
		this.m_buffer = null;
		this.m_fileStream.Close();
	}

	// Token: 0x0600A068 RID: 41064 RVA: 0x003C6FEC File Offset: 0x003C51EC
	public bool Store(int blockID, DynamicProperties props)
	{
		long position = this.m_fileStream.Position;
		this.m_buffer.Clear();
		IBufferWriter<byte> buffer = this.m_buffer;
		MemoryPackSerializer.Serialize<DynamicProperties>(buffer, props, null);
		int writtenCount = this.m_buffer.WrittenCount;
		this.m_fileStream.Write(this.m_buffer.WrittenSpan);
		this.offsetsAndLengths[blockID] = new ValueTuple<long, int>(position, writtenCount);
		return true;
	}

	// Token: 0x0600A069 RID: 41065 RVA: 0x003C7058 File Offset: 0x003C5258
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicProperties Retrieve(long offset, int length)
	{
		this.m_buffer.Clear();
		Span<byte> span = this.m_buffer.GetSpan(length).Slice(0, length);
		this.m_fileStream.Seek(offset, SeekOrigin.Begin);
		int num;
		for (int i = 0; i < length; i += num)
		{
			num = this.m_fileStream.Read(span.Slice(i, length - i));
			if (num <= 0)
			{
				throw new IOException(string.Format("Expected to read {0} bytes total but only read {1} bytes.", length, i));
			}
		}
		return MemoryPackSerializer.Deserialize<DynamicProperties>(span, null);
	}

	// Token: 0x0600A06A RID: 41066 RVA: 0x003C70E4 File Offset: 0x003C52E4
	public DynamicProperties Cache(int blockID)
	{
		LinkedListNode<int> linkedListNode = null;
		object cacheLock = this._cacheLock;
		DynamicProperties dynamicProperties;
		lock (cacheLock)
		{
			if (!this.m_cache.TryGetValue(blockID, out dynamicProperties))
			{
				this.m_cacheMisses++;
				dynamicProperties = this.Retrieve(this.offsetsAndLengths[blockID].Item1, this.offsetsAndLengths[blockID].Item2);
				this.m_cache.Add(blockID, dynamicProperties);
				while (this.m_queue.Count >= 1000)
				{
					linkedListNode = this.m_queue.Last;
					this.m_queue.Remove(linkedListNode);
					this.m_cache.Remove(linkedListNode.Value);
				}
				if (linkedListNode == null)
				{
					linkedListNode = new LinkedListNode<int>(blockID);
				}
				else
				{
					linkedListNode.Value = blockID;
				}
				this.m_queue.AddFirst(linkedListNode);
			}
			else
			{
				this.m_cacheHits++;
				linkedListNode = this.m_queue.Find(blockID);
				this.m_queue.Remove(linkedListNode);
				this.m_queue.AddFirst(linkedListNode);
			}
		}
		return dynamicProperties;
	}

	// Token: 0x0600A06B RID: 41067 RVA: 0x003C720C File Offset: 0x003C540C
	public void Stats()
	{
		Log.Out("[BLOCKPROPERTIES] Block DynamicProperties Cache Stats:");
		Log.Out(string.Format("[BLOCKPROPERTIES] Cache Size: {0}", this.m_cache.Count));
		Log.Out(string.Format("[BLOCKPROPERTIES] Hits: {0}, Misses: {1}, Rate: {2}%", this.m_cacheHits, this.m_cacheMisses, (float)this.m_cacheHits / (float)(this.m_cacheHits + this.m_cacheMisses) * 100f));
	}

	// Token: 0x04007960 RID: 31072
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string m_filePath;

	// Token: 0x04007961 RID: 31073
	[PublicizedFrom(EAccessModifier.Private)]
	public FileStream m_fileStream;

	// Token: 0x04007962 RID: 31074
	[PublicizedFrom(EAccessModifier.Private)]
	public ArrayBufferWriter<byte> m_buffer;

	// Token: 0x04007963 RID: 31075
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FILE_STREAM_BUFFER_SIZE = 4096;

	// Token: 0x04007964 RID: 31076
	[PublicizedFrom(EAccessModifier.Private)]
	public ValueTuple<long, int>[] offsetsAndLengths;

	// Token: 0x04007965 RID: 31077
	[PublicizedFrom(EAccessModifier.Private)]
	public LinkedList<int> m_queue;

	// Token: 0x04007966 RID: 31078
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<int, DynamicProperties> m_cache;

	// Token: 0x04007967 RID: 31079
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_cacheHits;

	// Token: 0x04007968 RID: 31080
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_cacheMisses;

	// Token: 0x04007969 RID: 31081
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cacheSize = 1000;

	// Token: 0x0400796A RID: 31082
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object _cacheLock = new object();
}
