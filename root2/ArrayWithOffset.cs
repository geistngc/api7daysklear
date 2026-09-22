using System;

// Token: 0x02001387 RID: 4999
public class ArrayWithOffset<T>
{
	// Token: 0x06009DB7 RID: 40375 RVA: 0x0000640C File Offset: 0x0000460C
	public ArrayWithOffset()
	{
	}

	// Token: 0x06009DB8 RID: 40376 RVA: 0x003BBF1E File Offset: 0x003BA11E
	public ArrayWithOffset(int _dimX, int _dimY) : this(_dimX, _dimY, 0, 0)
	{
	}

	// Token: 0x06009DB9 RID: 40377 RVA: 0x003BBF2C File Offset: 0x003BA12C
	public ArrayWithOffset(int _dimX, int _dimY, int _addXOffs, int _addYOffs)
	{
		this.DimX = _dimX;
		this.DimY = _dimY;
		this.data = new T[_dimX, _dimY];
		this.sizeXHalf = _dimX / 2;
		this.sizeYHalf = _dimY / 2;
		this.MinPos = new Vector2i(-this.sizeXHalf - _addXOffs, -this.sizeYHalf - _addYOffs);
		this.MaxPos = new Vector2i(this.sizeXHalf - _addXOffs - 1, this.sizeYHalf - _addYOffs - 1);
		this.addXOffs = _addXOffs + this.sizeXHalf;
		this.addYOffs = _addYOffs + this.sizeXHalf;
	}

	// Token: 0x1700129E RID: 4766
	public virtual T this[int _x, int _y]
	{
		get
		{
			return this.data[_x + this.addXOffs, _y + this.addYOffs];
		}
		set
		{
			this.data[_x + this.addXOffs, _y + this.addYOffs] = value;
		}
	}

	// Token: 0x06009DBC RID: 40380 RVA: 0x003BC002 File Offset: 0x003BA202
	public bool Contains(int _x, int _y)
	{
		return _x >= this.MinPos.x && _y >= this.MinPos.y && _x < this.MaxPos.x && _y < this.MaxPos.y;
	}

	// Token: 0x06009DBD RID: 40381 RVA: 0x003BC040 File Offset: 0x003BA240
	public void CopyInto(ArrayWithOffset<T> _other)
	{
		for (int i = 0; i < this.data.GetLength(0); i++)
		{
			for (int j = 0; j < this.data.GetLength(1); j++)
			{
				_other.data[i, j] = this.data[i, j];
			}
		}
	}

	// Token: 0x04007808 RID: 30728
	[PublicizedFrom(EAccessModifier.Private)]
	public T[,] data;

	// Token: 0x04007809 RID: 30729
	[PublicizedFrom(EAccessModifier.Private)]
	public int sizeXHalf;

	// Token: 0x0400780A RID: 30730
	[PublicizedFrom(EAccessModifier.Private)]
	public int sizeYHalf;

	// Token: 0x0400780B RID: 30731
	[PublicizedFrom(EAccessModifier.Private)]
	public int addXOffs;

	// Token: 0x0400780C RID: 30732
	[PublicizedFrom(EAccessModifier.Private)]
	public int addYOffs;

	// Token: 0x0400780D RID: 30733
	public int DimX;

	// Token: 0x0400780E RID: 30734
	public int DimY;

	// Token: 0x0400780F RID: 30735
	public Vector2i MinPos;

	// Token: 0x04007810 RID: 30736
	public Vector2i MaxPos;
}
