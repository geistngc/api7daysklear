using System;
using ConcurrentCollections;

// Token: 0x0200035D RID: 861
public class ChunkQueue
{
	// Token: 0x060018F5 RID: 6389 RVA: 0x0008CD94 File Offset: 0x0008AF94
	public void Add(long item)
	{
		object @lock = this._lock;
		lock (@lock)
		{
			this.KeyQueue.Add(item);
		}
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x0008CDDC File Offset: 0x0008AFDC
	public void Clear()
	{
		this.KeyQueue.Clear();
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x0008CDE9 File Offset: 0x0008AFE9
	public bool Contains(long item)
	{
		return this.KeyQueue.Contains(item);
	}

	// Token: 0x060018F8 RID: 6392 RVA: 0x0008CDF7 File Offset: 0x0008AFF7
	public void Remove(long item)
	{
		this.KeyQueue.TryRemove(item);
	}

	// Token: 0x04000FE8 RID: 4072
	[PublicizedFrom(EAccessModifier.Private)]
	public object _lock = new object();

	// Token: 0x04000FE9 RID: 4073
	[PublicizedFrom(EAccessModifier.Private)]
	public ConcurrentHashSet<long> KeyQueue = new ConcurrentHashSet<long>();
}
