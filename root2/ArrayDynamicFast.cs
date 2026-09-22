using System;
using System.Collections.Generic;

// Token: 0x02001385 RID: 4997
public class ArrayDynamicFast<T>
{
	// Token: 0x06009DA8 RID: 40360 RVA: 0x003BBB33 File Offset: 0x003B9D33
	public ArrayDynamicFast(int _size)
	{
		this.Size = _size;
		this.Data = new T[_size];
		this.DataAvail = new bool[_size];
		this.Count = 0;
	}

	// Token: 0x06009DA9 RID: 40361 RVA: 0x003BBB64 File Offset: 0x003B9D64
	public int Contains(T _v)
	{
		if (this.Count == 0)
		{
			return -1;
		}
		if (_v == null)
		{
			for (int i = 0; i < this.Data.Length; i++)
			{
				if (this.DataAvail[i] && this.Data[i] == null)
				{
					return i;
				}
			}
		}
		else
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int j = 0; j < this.Data.Length; j++)
			{
				if (this.DataAvail[j] && @default.Equals(this.Data[j], _v))
				{
					return j;
				}
			}
		}
		return -1;
	}

	// Token: 0x06009DAA RID: 40362 RVA: 0x003BBBF4 File Offset: 0x003B9DF4
	public void Clear()
	{
		for (int i = 0; i < this.Data.Length; i++)
		{
			this.DataAvail[i] = false;
		}
	}

	// Token: 0x06009DAB RID: 40363 RVA: 0x003BBC20 File Offset: 0x003B9E20
	public void Add(int _idx, T _texId)
	{
		if (_idx == -1)
		{
			for (int i = 0; i < this.Size; i++)
			{
				if (!this.DataAvail[i])
				{
					_idx = i;
					break;
				}
			}
		}
		if (_idx == -1)
		{
			return;
		}
		this.Data[_idx] = _texId;
		this.DataAvail[_idx] = true;
		this.Count++;
	}

	// Token: 0x04007801 RID: 30721
	public T[] Data;

	// Token: 0x04007802 RID: 30722
	public bool[] DataAvail;

	// Token: 0x04007803 RID: 30723
	public int Count;

	// Token: 0x04007804 RID: 30724
	public int Size;
}
