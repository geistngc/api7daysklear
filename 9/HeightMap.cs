using System;
using UnityEngine;

// Token: 0x02000C43 RID: 3139
public sealed class HeightMap : IDisposable
{
	// Token: 0x06005F90 RID: 24464 RVA: 0x0025D160 File Offset: 0x0025B360
	public HeightMap(int _w, int _h, float _maxHeight, IBackedArray<ushort> _data, int _targetSize = 0)
	{
		this.w = _w;
		this.h = _h;
		this.scaleShift = ((_targetSize != 0) ? ((int)Mathf.Log((float)(_targetSize / _w), 2f)) : 0);
		this.scalePixs = _targetSize / _w;
		this.maxHeight = _maxHeight;
		this.data = BackedArrays.CreateSingleView<ushort>(_data, BackedArrayHandleMode.ReadOnly, 16 * _w);
	}

	// Token: 0x06005F91 RID: 24465 RVA: 0x0025D1C1 File Offset: 0x0025B3C1
	public void Dispose()
	{
		IBackedArrayView<ushort> backedArrayView = this.data;
		if (backedArrayView != null)
		{
			backedArrayView.Dispose();
		}
		this.data = null;
	}

	// Token: 0x06005F92 RID: 24466 RVA: 0x0025D1DC File Offset: 0x0025B3DC
	public float GetAt(int _x, int _z)
	{
		ushort num;
		if (this.scaleShift == 0 && _x + _z * this.w < this.data.Length)
		{
			num = this.data[_x + _z * this.w];
		}
		else
		{
			num = this.getInterpolatedHeight(_x, _z);
		}
		return (float)num * (this.maxHeight / 65535f);
	}

	// Token: 0x06005F93 RID: 24467 RVA: 0x0025D238 File Offset: 0x0025B438
	public ushort getInterpolatedHeight(int xf, int zf)
	{
		object obj = (xf >= 0) ? (xf >> this.scaleShift) : (xf - this.scalePixs + 1 >> this.scaleShift);
		int num = (zf >= 0) ? (zf >> this.scaleShift) : (zf - this.scalePixs + 1 >> this.scaleShift);
		object obj2 = obj;
		int num2 = obj2 + num * this.w;
		ushort num3 = this.data[num2 + this.w & this.data.Length - 1];
		ushort num4 = this.data[num2 & this.data.Length - 1];
		ushort num5 = this.data[num2 + 1 & this.data.Length - 1];
		ushort num6 = this.data[num2 + 1 + this.w & this.data.Length - 1];
		int num7 = obj2 << (this.scaleShift & 31);
		int num8 = num << this.scaleShift;
		float num9 = 1f - (float)(zf - num8) / (float)this.scalePixs;
		float num10 = (float)(xf - num7) / (float)this.scalePixs;
		return (ushort)((1f - num9) * ((1f - num10) * (float)num3 + num10 * (float)num6) + num9 * ((1f - num10) * (float)num4 + num10 * (float)num5));
	}

	// Token: 0x06005F94 RID: 24468 RVA: 0x0025D38C File Offset: 0x0025B58C
	public float GetAt(int _offs)
	{
		ushort num;
		if (this.scaleShift == 0)
		{
			num = this.data[_offs];
		}
		else
		{
			int zf = _offs / this.w;
			int xf = _offs % this.w;
			num = this.getInterpolatedHeight(xf, zf);
		}
		return (float)num * this.maxHeight / 65535f;
	}

	// Token: 0x06005F95 RID: 24469 RVA: 0x0025D3DA File Offset: 0x0025B5DA
	public int CalcOffset(int _x, int _z)
	{
		_x >>= this.scaleShift;
		_z >>= this.scaleShift;
		return _x + _z * this.w;
	}

	// Token: 0x06005F96 RID: 24470 RVA: 0x0025D400 File Offset: 0x0025B600
	public int GetWidth()
	{
		return this.w;
	}

	// Token: 0x06005F97 RID: 24471 RVA: 0x0025D408 File Offset: 0x0025B608
	public int GetHeight()
	{
		return this.h;
	}

	// Token: 0x06005F98 RID: 24472 RVA: 0x0025D410 File Offset: 0x0025B610
	public int GetScaleSteps()
	{
		return this.scalePixs;
	}

	// Token: 0x06005F99 RID: 24473 RVA: 0x0025D418 File Offset: 0x0025B618
	public int GetScaleShift()
	{
		return this.scaleShift;
	}

	// Token: 0x04004AF1 RID: 19185
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int w;

	// Token: 0x04004AF2 RID: 19186
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int h;

	// Token: 0x04004AF3 RID: 19187
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int scaleShift;

	// Token: 0x04004AF4 RID: 19188
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int scalePixs;

	// Token: 0x04004AF5 RID: 19189
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly float maxHeight;

	// Token: 0x04004AF6 RID: 19190
	[PublicizedFrom(EAccessModifier.Private)]
	public IBackedArrayView<ushort> data;
}
