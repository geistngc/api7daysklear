using System;
using System.Runtime.CompilerServices;

// Token: 0x02001384 RID: 4996
public class Array3DWithOffset<T>
{
	// Token: 0x06009DA2 RID: 40354 RVA: 0x0000640C File Offset: 0x0000460C
	public Array3DWithOffset()
	{
	}

	// Token: 0x06009DA3 RID: 40355 RVA: 0x003BBA30 File Offset: 0x003B9C30
	public Array3DWithOffset(int _dimX, int _dimY, int _dimZ)
	{
		this.DimX = _dimX;
		this.DimY = _dimY;
		this.DimZ = _dimZ;
		this.data = new T[_dimX * _dimY * _dimZ];
		this.addX = _dimX / 2;
		this.addY = _dimY / 2;
		this.addZ = _dimZ / 2;
	}

	// Token: 0x06009DA4 RID: 40356 RVA: 0x003BBA83 File Offset: 0x003B9C83
	[PublicizedFrom(EAccessModifier.Protected)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetIndex(int _x, int _y, int _z)
	{
		return _x + this.addX + (_z + this.addZ) * this.DimX + (_y + this.addY) * this.DimZ * this.DimX;
	}

	// Token: 0x1700129C RID: 4764
	public virtual T this[int _x, int _y, int _z]
	{
		get
		{
			return this.data[this.GetIndex(_x, _y, _z)];
		}
		set
		{
			this.data[this.GetIndex(_x, _y, _z)] = value;
		}
	}

	// Token: 0x06009DA7 RID: 40359 RVA: 0x003BBAE4 File Offset: 0x003B9CE4
	public bool Contains(int _x, int _y, int _z)
	{
		return _x >= -this.addX && _y >= -this.addY && _x < this.addX - 1 && _y < this.addY - 1 && _z < this.addZ - 1 && _z < this.addZ - 1;
	}

	// Token: 0x040077FA RID: 30714
	[PublicizedFrom(EAccessModifier.Protected)]
	public T[] data;

	// Token: 0x040077FB RID: 30715
	[PublicizedFrom(EAccessModifier.Protected)]
	public int addX;

	// Token: 0x040077FC RID: 30716
	[PublicizedFrom(EAccessModifier.Protected)]
	public int addY;

	// Token: 0x040077FD RID: 30717
	[PublicizedFrom(EAccessModifier.Protected)]
	public int addZ;

	// Token: 0x040077FE RID: 30718
	public int DimX;

	// Token: 0x040077FF RID: 30719
	public int DimY;

	// Token: 0x04007800 RID: 30720
	public int DimZ;
}
