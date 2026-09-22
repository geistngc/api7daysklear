using System;
using System.Collections.Generic;

// Token: 0x02001388 RID: 5000
public class ArrayWithOffsetSparse<T>
{
	// Token: 0x06009DBE RID: 40382 RVA: 0x003BC095 File Offset: 0x003BA295
	public ArrayWithOffsetSparse(int _dimX, int _dimY, T _emptyValue)
	{
		this.DimX = _dimX;
		this.DimY = _dimY;
		this.EmptyValue = _emptyValue;
		this.myData = new Dictionary<long, T>();
		this.addX = _dimX / 2;
		this.addY = _dimY / 2;
	}

	// Token: 0x06009DBF RID: 40383 RVA: 0x003BC0CF File Offset: 0x003BA2CF
	[PublicizedFrom(EAccessModifier.Private)]
	public long makeKey(int _x, int _y)
	{
		return ((long)(_y + this.addY) & (long)((ulong)-1)) << 32 | ((long)(_x + this.addX) & (long)((ulong)-1));
	}

	// Token: 0x06009DC0 RID: 40384 RVA: 0x003BC0ED File Offset: 0x003BA2ED
	public bool Contains(int _x, int _y)
	{
		return _x >= -this.addX && _y >= -this.addY && _x < this.addX - 1 && _y < this.addY - 1;
	}

	// Token: 0x1700129F RID: 4767
	public virtual T this[int _x, int _y]
	{
		get
		{
			if (!this.myData.ContainsKey(this.makeKey(_x, _y)))
			{
				return this.EmptyValue;
			}
			return this.myData[this.makeKey(_x, _y)];
		}
		set
		{
			this.myData[this.makeKey(_x, _y)] = value;
		}
	}

	// Token: 0x04007811 RID: 30737
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<long, T> myData;

	// Token: 0x04007812 RID: 30738
	public T EmptyValue;

	// Token: 0x04007813 RID: 30739
	[PublicizedFrom(EAccessModifier.Private)]
	public int addX;

	// Token: 0x04007814 RID: 30740
	[PublicizedFrom(EAccessModifier.Private)]
	public int addY;

	// Token: 0x04007815 RID: 30741
	public int DimX;

	// Token: 0x04007816 RID: 30742
	public int DimY;
}
