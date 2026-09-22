using System;

// Token: 0x02001386 RID: 4998
public class ArrayListMP<T> where T : new()
{
	// Token: 0x06009DAC RID: 40364 RVA: 0x003BBC7A File Offset: 0x003B9E7A
	public ArrayListMP(MemoryPooledArray<T> _pool, int _minSize = 0)
	{
		this.pool = _pool;
		this.Count = 0;
		if (_minSize > 0)
		{
			this.Items = this.pool.Alloc(_minSize);
		}
	}

	// Token: 0x06009DAD RID: 40365 RVA: 0x003BBCA8 File Offset: 0x003B9EA8
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~ArrayListMP()
	{
		if (this.Items != null)
		{
			this.pool.Free(this.Items);
			this.Items = null;
		}
	}

	// Token: 0x06009DAE RID: 40366 RVA: 0x003BBCF0 File Offset: 0x003B9EF0
	public void Add(T _item)
	{
		if (this.Items == null)
		{
			this.Items = this.pool.Alloc(0);
		}
		if (this.Count >= this.Items.Length)
		{
			this.Items = this.pool.Grow(this.Items);
		}
		T[] items = this.Items;
		int count = this.Count;
		this.Count = count + 1;
		items[count] = _item;
	}

	// Token: 0x1700129D RID: 4765
	public T this[int idx]
	{
		get
		{
			return this.Items[idx];
		}
		set
		{
			this.Items[idx] = value;
		}
	}

	// Token: 0x06009DB1 RID: 40369 RVA: 0x003BBD78 File Offset: 0x003B9F78
	public void Clear()
	{
		this.Count = 0;
		if (this.Items != null)
		{
			this.pool.Free(this.Items);
			this.Items = null;
		}
	}

	// Token: 0x06009DB2 RID: 40370 RVA: 0x003BBDA4 File Offset: 0x003B9FA4
	public T[] ToArray()
	{
		if (this.Items == null)
		{
			return new T[0];
		}
		T[] array = new T[this.Count];
		Array.Copy(this.Items, array, this.Count);
		return array;
	}

	// Token: 0x06009DB3 RID: 40371 RVA: 0x003BBDDF File Offset: 0x003B9FDF
	public void AddRange(T[] _range)
	{
		this.AddRange(_range, 0, _range.Length);
	}

	// Token: 0x06009DB4 RID: 40372 RVA: 0x003BBDEC File Offset: 0x003B9FEC
	public void AddRange(T[] _range, int _offs, int _count)
	{
		if (_range == null || _range.Length == 0)
		{
			return;
		}
		if (this.Items == null)
		{
			this.Items = this.pool.Alloc(_count);
		}
		if (this.Count + _count >= this.Items.Length)
		{
			this.Items = this.pool.Grow(this.Items, this.Count + _count);
		}
		Array.Copy(_range, _offs, this.Items, this.Count, _count);
		this.Count += _count;
	}

	// Token: 0x06009DB5 RID: 40373 RVA: 0x003BBE70 File Offset: 0x003BA070
	public int Alloc(int _count)
	{
		if (this.Items == null)
		{
			this.Items = this.pool.Alloc(_count);
		}
		else if (this.Count + _count > this.Items.Length)
		{
			this.Items = this.pool.Grow(this.Items, this.Count + _count);
		}
		int count = this.Count;
		this.Count += _count;
		return count;
	}

	// Token: 0x06009DB6 RID: 40374 RVA: 0x003BBEDE File Offset: 0x003BA0DE
	public void Grow(int newSize)
	{
		if (this.Items == null)
		{
			this.Items = this.pool.Alloc(newSize);
			return;
		}
		if (newSize > this.Items.Length)
		{
			this.Items = this.pool.Grow(this.Items, newSize);
		}
	}

	// Token: 0x04007805 RID: 30725
	public readonly MemoryPooledArray<T> pool;

	// Token: 0x04007806 RID: 30726
	public T[] Items;

	// Token: 0x04007807 RID: 30727
	public int Count;
}
