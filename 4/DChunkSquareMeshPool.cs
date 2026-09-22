using System;
using System.Collections.Generic;

// Token: 0x02000C26 RID: 3110
public class DChunkSquareMeshPool
{
	// Token: 0x170009AD RID: 2477
	// (get) Token: 0x06005ED0 RID: 24272 RVA: 0x002501F0 File Offset: 0x0024E3F0
	public int Count
	{
		get
		{
			object poolLock = this._poolLock;
			int result;
			lock (poolLock)
			{
				int num = 0;
				for (int i = 0; i < this.pool.Length; i++)
				{
					num += this.pool[i].Count;
				}
				result = num;
			}
			return result;
		}
	}

	// Token: 0x06005ED1 RID: 24273 RVA: 0x00250254 File Offset: 0x0024E454
	public DChunkSquareMeshPool(int initialCapacity, int NbLODLevel)
	{
		this._poolLock = new object();
		this.pool = new List<DChunkSquareMesh>[NbLODLevel];
		for (int i = 0; i < NbLODLevel; i++)
		{
			this.pool[i] = new List<DChunkSquareMesh>(initialCapacity);
		}
	}

	// Token: 0x06005ED2 RID: 24274 RVA: 0x00250298 File Offset: 0x0024E498
	public DChunkSquareMesh GetObject(DistantChunkMap DCMap, int LODLevel)
	{
		object poolLock = this._poolLock;
		DChunkSquareMesh result;
		lock (poolLock)
		{
			List<DChunkSquareMesh> list = this.pool[LODLevel];
			if (list.Count == 0)
			{
				result = new DChunkSquareMesh(DCMap, LODLevel);
			}
			else
			{
				DChunkSquareMesh dchunkSquareMesh = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				result = dchunkSquareMesh;
			}
		}
		return result;
	}

	// Token: 0x06005ED3 RID: 24275 RVA: 0x00250310 File Offset: 0x0024E510
	public void ReturnObject(DChunkSquareMesh item, int LODLevel)
	{
		if (item == null)
		{
			throw new ArgumentNullException("DChunkSquareMesh is null");
		}
		object poolLock = this._poolLock;
		lock (poolLock)
		{
			if (this.pool[LODLevel].Contains(item))
			{
				throw new InvalidOperationException("ThreadProcessing already in pool");
			}
			this.pool[LODLevel].Add(item);
		}
	}

	// Token: 0x040049C0 RID: 18880
	[PublicizedFrom(EAccessModifier.Private)]
	public List<DChunkSquareMesh>[] pool;

	// Token: 0x040049C1 RID: 18881
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object _poolLock;
}
