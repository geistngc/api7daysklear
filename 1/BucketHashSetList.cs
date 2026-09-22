using System;
using System.Collections.Generic;

// Token: 0x020013B1 RID: 5041
public class BucketHashSetList
{
	// Token: 0x06009ED9 RID: 40665 RVA: 0x003C140F File Offset: 0x003BF60F
	public BucketHashSetList()
	{
		this.buckets = new OptimizedList<HashSetLong>(4);
	}

	// Token: 0x06009EDA RID: 40666 RVA: 0x003C143C File Offset: 0x003BF63C
	public BucketHashSetList(int _noBuckets)
	{
		this.buckets = new OptimizedList<HashSetLong>(_noBuckets);
		for (int i = 0; i < _noBuckets; i++)
		{
			this.buckets.Add(new HashSetLong());
		}
	}

	// Token: 0x06009EDB RID: 40667 RVA: 0x003C148D File Offset: 0x003BF68D
	public void Add(int _bucketIdx, long _value)
	{
		this.buckets.array[_bucketIdx].Add(_value);
	}

	// Token: 0x06009EDC RID: 40668 RVA: 0x003C14A3 File Offset: 0x003BF6A3
	public void Add(int _bucketIdx, HashSetLong _otherBucket)
	{
		this.buckets.array[_bucketIdx].UnionWithHashSetLong(_otherBucket);
	}

	// Token: 0x06009EDD RID: 40669 RVA: 0x003C14B8 File Offset: 0x003BF6B8
	public bool Contains(long _value)
	{
		if (!this.IsRecalc)
		{
			return false;
		}
		for (int i = 0; i < this.buckets.Count; i++)
		{
			if (this.buckets.array[i].Contains(_value))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06009EDE RID: 40670 RVA: 0x003C1500 File Offset: 0x003BF700
	public void Remove(long _value)
	{
		this.list.Remove(_value);
		for (int i = 0; i < this.buckets.Count; i++)
		{
			if (this.buckets.array[i].Contains(_value))
			{
				this.buckets.array[i].Remove(_value);
				return;
			}
		}
	}

	// Token: 0x06009EDF RID: 40671 RVA: 0x003C155C File Offset: 0x003BF75C
	public void Clear()
	{
		for (int i = 0; i < this.buckets.Count; i++)
		{
			this.buckets.array[i].Clear();
		}
		this.list.Clear();
		this.IsRecalc = false;
	}

	// Token: 0x06009EE0 RID: 40672 RVA: 0x003C15A4 File Offset: 0x003BF7A4
	public void ExceptTarget(HashSetLong hash)
	{
		if (!this.IsRecalc)
		{
			return;
		}
		for (int i = 0; i < this.buckets.Count; i++)
		{
			hash.ExceptWithHashSetLong(this.buckets.array[i]);
		}
	}

	// Token: 0x06009EE1 RID: 40673 RVA: 0x003C15E4 File Offset: 0x003BF7E4
	public void RecalcHashSetList()
	{
		this.list.Clear();
		this.elementsInList.Clear();
		this.IsRecalc = true;
		for (int i = 0; i < this.buckets.Count; i++)
		{
			foreach (long item in this.buckets.array[i])
			{
				if (this.elementsInList.Add(item))
				{
					this.list.Add(item);
				}
			}
		}
	}

	// Token: 0x06009EE2 RID: 40674 RVA: 0x003C1684 File Offset: 0x003BF884
	public BucketHashSetList Clone()
	{
		BucketHashSetList bucketHashSetList = new BucketHashSetList(this.buckets.Count);
		for (int i = 0; i < this.buckets.Count; i++)
		{
			bucketHashSetList.buckets.array[i].UnionWithHashSetLong(this.buckets.array[i]);
		}
		return bucketHashSetList;
	}

	// Token: 0x040078A3 RID: 30883
	public List<long> list = new List<long>();

	// Token: 0x040078A4 RID: 30884
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<long> elementsInList = new HashSet<long>();

	// Token: 0x040078A5 RID: 30885
	public OptimizedList<HashSetLong> buckets;

	// Token: 0x040078A6 RID: 30886
	public bool IsRecalc;
}
