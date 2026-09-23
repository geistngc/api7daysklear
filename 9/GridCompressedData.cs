using System;
using System.Runtime.CompilerServices;

// Token: 0x02001419 RID: 5145
public class GridCompressedData<[IsUnmanaged] T> where T : struct, ValueType, IEquatable<T>
{
	// Token: 0x0600A18E RID: 41358 RVA: 0x003CBBAC File Offset: 0x003C9DAC
	public GridCompressedData(int _width, int _height, int _cellSizeX, int _cellSizeY)
	{
		if (_width % _cellSizeX != 0)
		{
			throw new Exception(string.Format("Cell width must be a multiple of data width. Cell width: {0}, width: {1}", _cellSizeX, _width));
		}
		if (_height % _cellSizeY != 0)
		{
			throw new Exception(string.Format("Cell height must be a multiple of data height. Cell height: {0}, height: {1}", _cellSizeX, _width));
		}
		this.width = _width;
		this.height = _height;
		this.cellSizeX = _cellSizeX;
		this.cellSizeY = _cellSizeY;
		this.widthCells = _width / this.cellSizeX;
		this.heightCells = _height / this.cellSizeY;
		int num = this.widthCells * this.heightCells;
		this.cells = new T[num][];
		this.sameValues = new T[num];
	}

	// Token: 0x0600A18F RID: 41359 RVA: 0x003CBC64 File Offset: 0x003C9E64
	public void SetValue(int _x, int _y, T value)
	{
		int num = _x / this.cellSizeX + _y / this.cellSizeY * this.widthCells;
		T[] array = this.cells[num];
		if (array == null)
		{
			T t = this.sameValues[num];
			if (value.Equals(t))
			{
				return;
			}
			array = (this.cells[num] = new T[this.cellSizeX * this.cellSizeY]);
			Array.Fill<T>(array, t);
		}
		int num2 = _x % this.cellSizeX + _y % this.cellSizeY * this.cellSizeX;
		array[num2] = value;
	}

	// Token: 0x0600A190 RID: 41360 RVA: 0x003CBCFC File Offset: 0x003C9EFC
	public void SetValue(int _cellIndex, int _cellX, int _cellY, T _value)
	{
		T[] array = this.cells[_cellIndex];
		if (array == null)
		{
			T value = this.sameValues[_cellIndex];
			if (_value.Equals(this.sameValues[_cellIndex]))
			{
				return;
			}
			array = (this.cells[_cellIndex] = new T[this.cellSizeX * this.cellSizeY]);
			Array.Fill<T>(array, value);
		}
		int num = _cellX + _cellY * this.cellSizeX;
		array[num] = _value;
	}

	// Token: 0x0600A191 RID: 41361 RVA: 0x003CBD77 File Offset: 0x003C9F77
	public void SetSameValue(int _cellIndex, T _value)
	{
		this.sameValues[_cellIndex] = _value;
		this.cells[_cellIndex] = null;
	}

	// Token: 0x0600A192 RID: 41362 RVA: 0x003CBD90 File Offset: 0x003C9F90
	public void Fill(T _value)
	{
		for (int i = 0; i < this.cells.Length; i++)
		{
			this.sameValues[i] = _value;
			this.cells[i] = null;
		}
	}

	// Token: 0x0600A193 RID: 41363 RVA: 0x003CBDC8 File Offset: 0x003C9FC8
	public T GetValue(int _x, int _y)
	{
		int num = _x / this.cellSizeX + _y / this.cellSizeY * this.widthCells;
		T[] array = this.cells[num];
		if (array == null)
		{
			return this.sameValues[num];
		}
		int num2 = _x % this.cellSizeX + _y % this.cellSizeY * this.cellSizeX;
		return array[num2];
	}

	// Token: 0x0600A194 RID: 41364 RVA: 0x003CBE27 File Offset: 0x003CA027
	public T GetValue(int _offs)
	{
		return this.GetValue(_offs % this.width, _offs / this.width);
	}

	// Token: 0x0600A195 RID: 41365 RVA: 0x003CBE40 File Offset: 0x003CA040
	public void CheckSameValue(int _cellIndex)
	{
		T[] array = this.cells[_cellIndex];
		if (array != null && GridCompressedData<T>.CheckSameValue(this.cells[_cellIndex]))
		{
			this.sameValues[_cellIndex] = array[0];
			this.cells[_cellIndex] = null;
		}
	}

	// Token: 0x0600A196 RID: 41366 RVA: 0x003CBE84 File Offset: 0x003CA084
	public void CheckSameValues()
	{
		for (int i = 0; i < this.sameValues.Length; i++)
		{
			this.CheckSameValue(i);
		}
	}

	// Token: 0x0600A197 RID: 41367 RVA: 0x003CBEAC File Offset: 0x003CA0AC
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool CheckSameValue(T[] _cell)
	{
		T t = _cell[0];
		for (int i = 1; i < _cell.Length; i++)
		{
			if (!t.Equals(_cell[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600A198 RID: 41368 RVA: 0x003CBEE8 File Offset: 0x003CA0E8
	public T[] ToArray()
	{
		T[] array = new T[this.widthCells * this.cellSizeX * this.heightCells * this.cellSizeY];
		for (int i = 0; i < this.cells.Length; i++)
		{
			int num = i % this.widthCells;
			int num2 = i / this.widthCells;
			int num3 = num * this.cellSizeX + num2 * this.cellSizeY * this.width;
			T[] array2 = this.cells[i];
			if (array2 == null)
			{
				T value = this.sameValues[i];
				for (int j = 0; j < this.cellSizeY; j++)
				{
					Array.Fill<T>(array, value, num3, this.cellSizeX);
					num3 += this.width;
				}
			}
			else
			{
				int num4 = 0;
				for (int k = 0; k < this.cellSizeY; k++)
				{
					Array.Copy(array2, num4, array, num3, this.cellSizeX);
					num3 += this.width;
					num4 += this.cellSizeX;
				}
			}
		}
		return array;
	}

	// Token: 0x0600A199 RID: 41369 RVA: 0x003CBFE4 File Offset: 0x003CA1E4
	public void FromArray(T[] _pixs)
	{
		if (_pixs.Length != this.width * this.height)
		{
			throw new Exception(string.Format("Source array does not contain enough data. Expected length: {0}, Actual length: {1}", this.width * this.height, _pixs.Length));
		}
		int num = 0;
		for (int i = 0; i < this.heightCells; i++)
		{
			for (int j = 0; j < this.widthCells; j++)
			{
				int num2 = i * this.cellSizeY;
				int num3 = j * this.cellSizeX;
				T value = _pixs[num3 + num2 * this.width];
				this.SetSameValue(num, value);
				for (int k = 0; k < this.cellSizeY; k++)
				{
					for (int l = 0; l < this.cellSizeX; l++)
					{
						int num4 = num3 + l + (num2 + k) * this.width;
						this.SetValue(num, l, k, _pixs[num4]);
					}
				}
				num++;
			}
		}
	}

	// Token: 0x0600A19A RID: 41370 RVA: 0x003CC0E0 File Offset: 0x003CA2E0
	public int EstimateOwnedBytes()
	{
		return 0 + MemoryTracker.GetSize<T>(this.sameValues) + MemoryTracker.GetSize<T>(this.cells);
	}

	// Token: 0x040079DF RID: 31199
	[PublicizedFrom(EAccessModifier.Private)]
	public T[][] cells;

	// Token: 0x040079E0 RID: 31200
	[PublicizedFrom(EAccessModifier.Private)]
	public T[] sameValues;

	// Token: 0x040079E1 RID: 31201
	public int cellSizeX;

	// Token: 0x040079E2 RID: 31202
	public int cellSizeY;

	// Token: 0x040079E3 RID: 31203
	public int widthCells;

	// Token: 0x040079E4 RID: 31204
	public int heightCells;

	// Token: 0x040079E5 RID: 31205
	public int width;

	// Token: 0x040079E6 RID: 31206
	public int height;
}
