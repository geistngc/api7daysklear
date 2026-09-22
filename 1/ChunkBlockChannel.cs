using System;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x02000B0B RID: 2827
public class ChunkBlockChannel
{
	// Token: 0x06005516 RID: 21782 RVA: 0x002090F0 File Offset: 0x002072F0
	public ChunkBlockChannel(long _defaultValue, int _bytesPerVal = 1)
	{
		this.defaultValue = _defaultValue;
		this.bytesPerVal = _bytesPerVal;
		this.sameValue = new byte[64 * this.bytesPerVal];
		this.fillSameValue(-1L);
		this.layers = new CBCLayer[64 * this.bytesPerVal];
	}

	// Token: 0x06005517 RID: 21783 RVA: 0x00209141 File Offset: 0x00207341
	[PublicizedFrom(EAccessModifier.Private)]
	public CBCLayer allocLayer()
	{
		return MemoryPools.poolCBC.AllocSync(true);
	}

	// Token: 0x06005518 RID: 21784 RVA: 0x0020914E File Offset: 0x0020734E
	[PublicizedFrom(EAccessModifier.Private)]
	public void freeLayer(int _idx)
	{
		if (this.layers[_idx] == null)
		{
			return;
		}
		MemoryPools.poolCBC.FreeSync(this.layers[_idx]);
		this.layers[_idx] = null;
	}

	// Token: 0x06005519 RID: 21785 RVA: 0x00209176 File Offset: 0x00207376
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int calcOffset(int _x, int _y, int _z)
	{
		return _x + _z * 16 + (_y & 3) * 256;
	}

	// Token: 0x0600551A RID: 21786 RVA: 0x00209188 File Offset: 0x00207388
	[PublicizedFrom(EAccessModifier.Private)]
	public void fillSameValue(long _value = -1L)
	{
		long num = (_value == -1L) ? this.defaultValue : _value;
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			byte b = (byte)(num >> i * 8);
			for (int j = 63; j >= 0; j--)
			{
				this.sameValue[j * this.bytesPerVal + i] = b;
			}
		}
	}

	// Token: 0x0600551B RID: 21787 RVA: 0x002091E0 File Offset: 0x002073E0
	[PublicizedFrom(EAccessModifier.Private)]
	public long getSameValue(int _idx)
	{
		long num = 0L;
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			num |= (long)((long)((ulong)this.sameValue[_idx + i]) << i * 8);
		}
		return num;
	}

	// Token: 0x0600551C RID: 21788 RVA: 0x00209218 File Offset: 0x00207418
	[PublicizedFrom(EAccessModifier.Private)]
	public void setSameValue(int _idx, long _value)
	{
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			this.sameValue[_idx + i] = (byte)_value;
			_value >>= 8;
		}
	}

	// Token: 0x0600551D RID: 21789 RVA: 0x00209248 File Offset: 0x00207448
	[PublicizedFrom(EAccessModifier.Private)]
	public long getData(int _idx, int _offs)
	{
		long num = 0L;
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			CBCLayer cbclayer = this.layers[_idx + i];
			if (cbclayer == null)
			{
				break;
			}
			num |= (long)((long)((ulong)cbclayer.data[_offs]) << i * 8);
		}
		return num;
	}

	// Token: 0x0600551E RID: 21790 RVA: 0x0020928C File Offset: 0x0020748C
	[PublicizedFrom(EAccessModifier.Private)]
	public long getSetData(int _idx, int _offs, long _value)
	{
		long num = 0L;
		for (int i = 0; i < this.bytesPerVal; i++)
		{
			CBCLayer cbclayer = this.layers[_idx + i];
			if (cbclayer == null)
			{
				break;
			}
			num |= (long)((long)((ulong)cbclayer.data[_offs]) << i * 8);
			cbclayer.data[_offs] = (byte)_value;
			_value >>= 8;
		}
		return num;
	}

	// Token: 0x0600551F RID: 21791 RVA: 0x002092E0 File Offset: 0x002074E0
	public long GetSet(int _x, int _y, int _z, long _value)
	{
		int num = (_y >> 2) * this.bytesPerVal;
		if (this.layers[num] == null)
		{
			long num2 = this.getSameValue(num);
			if (num2 == _value)
			{
				return _value;
			}
			for (int i = 0; i < this.bytesPerVal; i++)
			{
				CBCLayer cbclayer = this.allocLayer();
				this.layers[num + i] = cbclayer;
				byte b = (byte)(num2 >> i * 8);
				for (int j = 1023; j >= 0; j--)
				{
					cbclayer.data[j] = b;
				}
			}
		}
		int offs = ChunkBlockChannel.calcOffset(_x, _y, _z);
		return this.getSetData(num, offs, _value);
	}

	// Token: 0x06005520 RID: 21792 RVA: 0x0020937C File Offset: 0x0020757C
	public void Set(int _x, int _y, int _z, long _value)
	{
		int num = (_y >> 2) * this.bytesPerVal;
		if (this.layers[num] == null)
		{
			long num2 = this.getSameValue(num);
			if (num2 == _value)
			{
				return;
			}
			for (int i = 0; i < this.bytesPerVal; i++)
			{
				CBCLayer cbclayer = this.allocLayer();
				this.layers[num + i] = cbclayer;
				byte b = (byte)(num2 >> i * 8);
				for (int j = 1023; j >= 0; j--)
				{
					cbclayer.data[j] = b;
				}
			}
		}
		int num3 = ChunkBlockChannel.calcOffset(_x, _y, _z);
		for (int k = 0; k < this.bytesPerVal; k++)
		{
			CBCLayer cbclayer = this.layers[num + k];
			if (cbclayer == null)
			{
				break;
			}
			cbclayer.data[num3] = (byte)_value;
			_value >>= 8;
		}
	}

	// Token: 0x06005521 RID: 21793 RVA: 0x00209440 File Offset: 0x00207640
	public long Get(int _x, int _y, int _z)
	{
		int num = (_y >> 2) * this.bytesPerVal;
		if (num < 0)
		{
			return 0L;
		}
		CBCLayer cbclayer = this.layers[num];
		if (cbclayer == null)
		{
			return this.getSameValue(num);
		}
		int num2 = ChunkBlockChannel.calcOffset(_x, _y, _z);
		if (this.bytesPerVal == 1)
		{
			return (long)((ulong)cbclayer.data[num2]);
		}
		return this.getData(num, num2);
	}

	// Token: 0x06005522 RID: 21794 RVA: 0x00209498 File Offset: 0x00207698
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte GetByte(int _x, int _y, int _z)
	{
		int num = _y >> 2;
		if (num >= 64)
		{
			return 0;
		}
		CBCLayer cbclayer = this.layers[num];
		if (cbclayer == null)
		{
			return this.sameValue[num];
		}
		int num2 = ChunkBlockChannel.calcOffset(_x, _y, _z);
		return cbclayer.data[num2];
	}

	// Token: 0x06005523 RID: 21795 RVA: 0x002094D8 File Offset: 0x002076D8
	public void Read(BinaryReader _br, uint _version, bool _bNetworkRead)
	{
		if (_version > 34U)
		{
			for (int i = 0; i < 64; i++)
			{
				int num = i * this.bytesPerVal;
				bool flag = _br.ReadByte() == 1;
				for (int j = 0; j < this.bytesPerVal; j++)
				{
					int num2 = num + j;
					if (!flag)
					{
						if (this.layers[num2] == null)
						{
							this.layers[num2] = this.allocLayer();
						}
						_br.Read(this.layers[num2].data, 0, 1024);
					}
					else
					{
						this.sameValue[num2] = _br.ReadByte();
						this.freeLayer(num2);
					}
				}
				this.onLayerRead(num);
			}
			return;
		}
		for (int k = 0; k < 64; k++)
		{
			int num3 = k * this.bytesPerVal;
			bool flag2 = _br.ReadBoolean();
			for (int l = 0; l < this.bytesPerVal; l++)
			{
				if (!flag2)
				{
					if (this.layers[num3 + l] == null)
					{
						this.layers[num3 + l] = this.allocLayer();
					}
					_br.Read(this.layers[num3 + l].data, 0, 1024);
				}
				else
				{
					this.sameValue[num3 + l] = _br.ReadByte();
					this.freeLayer(num3 + l);
				}
			}
			this.onLayerRead(num3);
		}
	}

	// Token: 0x06005524 RID: 21796 RVA: 0x0020962C File Offset: 0x0020782C
	public void Write(BinaryWriter _bw, bool _bNetworkWrite, byte[] temp)
	{
		int num = 0;
		for (int i = 0; i < 64; i++)
		{
			int num2 = i * this.bytesPerVal;
			bool flag = this.layers[num2] == null;
			temp[num++] = (flag ? 1 : 0);
			if (num == temp.Length)
			{
				_bw.Write(temp, 0, num);
				num = 0;
			}
			for (int j = 0; j < this.bytesPerVal; j++)
			{
				if (!flag)
				{
					if (num > 0)
					{
						_bw.Write(temp, 0, num);
						num = 0;
					}
					_bw.Write(this.layers[num2 + j].data, 0, 1024);
				}
				else
				{
					temp[num++] = this.sameValue[num2 + j];
					if (num == temp.Length)
					{
						_bw.Write(temp, 0, num);
						num = 0;
					}
				}
			}
		}
		if (num > 0)
		{
			_bw.Write(temp, 0, num);
		}
	}

	// Token: 0x06005525 RID: 21797 RVA: 0x002096FA File Offset: 0x002078FA
	[PublicizedFrom(EAccessModifier.Private)]
	public void onLayerRead(int _idx)
	{
		if (this.layers[_idx] == null)
		{
			return;
		}
		this.checkSameValue(_idx);
	}

	// Token: 0x06005526 RID: 21798 RVA: 0x00209710 File Offset: 0x00207910
	public void CheckSameValue()
	{
		for (int i = 63; i >= 0; i--)
		{
			this.checkSameValue(i * this.bytesPerVal);
		}
	}

	// Token: 0x06005527 RID: 21799 RVA: 0x00209738 File Offset: 0x00207938
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkSameValue(int _idx)
	{
		if (this.layers[_idx] == null)
		{
			return;
		}
		long data = this.getData(_idx, 0);
		for (int i = 1; i < 1024; i++)
		{
			if (data != this.getData(_idx, i))
			{
				return;
			}
		}
		this.setSameValue(_idx, data);
		for (int j = 0; j < this.bytesPerVal; j++)
		{
			this.freeLayer(_idx + j);
		}
	}

	// Token: 0x06005528 RID: 21800 RVA: 0x00209798 File Offset: 0x00207998
	public bool HasSameValue(int _y)
	{
		int num = (_y >> 2) * this.bytesPerVal;
		return this.layers[num] == null;
	}

	// Token: 0x06005529 RID: 21801 RVA: 0x002097BC File Offset: 0x002079BC
	public long GetSameValue(int _y)
	{
		int idx = (_y >> 2) * this.bytesPerVal;
		return this.getSameValue(idx);
	}

	// Token: 0x0600552A RID: 21802 RVA: 0x002097DC File Offset: 0x002079DC
	public bool IsDefault()
	{
		for (int i = 63; i >= 0; i--)
		{
			if (!this.IsDefaultLayer(i))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600552B RID: 21803 RVA: 0x00209804 File Offset: 0x00207A04
	public bool IsDefault(int _y)
	{
		int blockLayer = _y >> 2;
		return this.IsDefaultLayer(blockLayer);
	}

	// Token: 0x0600552C RID: 21804 RVA: 0x0020981C File Offset: 0x00207A1C
	public bool IsDefaultLayer(int _blockLayer)
	{
		return this.isDefault(_blockLayer * this.bytesPerVal);
	}

	// Token: 0x0600552D RID: 21805 RVA: 0x0020982C File Offset: 0x00207A2C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDefault(int _idx)
	{
		this.checkSameValue(_idx);
		return this.layers[_idx] == null && this.getSameValue(_idx) == this.defaultValue;
	}

	// Token: 0x0600552E RID: 21806 RVA: 0x00209850 File Offset: 0x00207A50
	public int GetUsedMem()
	{
		int num = 0;
		for (int i = this.layers.Length - 1; i >= 0; i--)
		{
			if (this.layers[i] != null)
			{
				num += 1024;
			}
		}
		num += this.sameValue.Length;
		return num + this.layers.Length * 4;
	}

	// Token: 0x0600552F RID: 21807 RVA: 0x0020989F File Offset: 0x00207A9F
	public void FreeLayers()
	{
		MemoryPools.poolCBC.FreeSync(this.layers);
		this.fillSameValue(-1L);
	}

	// Token: 0x06005530 RID: 21808 RVA: 0x002098BC File Offset: 0x00207ABC
	public void Clear(long _defaultValue = -1L)
	{
		for (int i = 0; i < this.layers.Length; i++)
		{
			this.freeLayer(i);
		}
		this.fillSameValue(_defaultValue);
	}

	// Token: 0x06005531 RID: 21809 RVA: 0x002098EC File Offset: 0x00207AEC
	public void ClearHalf(bool _bClearUpperHalf)
	{
		byte b = _bClearUpperHalf ? 15 : 240;
		for (int i = 0; i < 64; i++)
		{
			CBCLayer cbclayer = this.layers[i];
			if (cbclayer != null)
			{
				for (int j = 0; j < 1024; j++)
				{
					byte[] data = cbclayer.data;
					int num = j;
					data[num] &= b;
				}
			}
			else
			{
				byte[] array = this.sameValue;
				int num2 = i;
				array[num2] &= b;
			}
		}
	}

	// Token: 0x06005532 RID: 21810 RVA: 0x00209958 File Offset: 0x00207B58
	public void SetHalf(bool _bSetUpperHalf, byte _v)
	{
		byte b = _bSetUpperHalf ? 15 : 240;
		for (int i = 0; i < 64; i++)
		{
			CBCLayer cbclayer = this.layers[i];
			if (cbclayer != null)
			{
				for (int j = 0; j < 1024; j++)
				{
					byte[] data = cbclayer.data;
					int num = j;
					data[num] &= b;
					byte[] data2 = cbclayer.data;
					int num2 = j;
					data2[num2] |= _v;
				}
			}
			else
			{
				byte[] array = this.sameValue;
				int num3 = i;
				array[num3] &= b;
				byte[] array2 = this.sameValue;
				int num4 = i;
				array2[num4] |= _v;
			}
		}
	}

	// Token: 0x06005533 RID: 21811 RVA: 0x002099E8 File Offset: 0x00207BE8
	public void CopyFrom(ChunkBlockChannel _other)
	{
		for (int i = 0; i < _other.layers.Length; i++)
		{
			if (_other.layers[i] != null)
			{
				if (this.layers[i] == null)
				{
					this.layers[i] = this.allocLayer();
				}
				this.layers[i].CopyFrom(_other.layers[i]);
			}
			else
			{
				this.freeLayer(i);
			}
		}
		for (int j = 0; j < _other.sameValue.Length; j++)
		{
			this.sameValue[j] = _other.sameValue[j];
		}
	}

	// Token: 0x06005534 RID: 21812 RVA: 0x00209A6C File Offset: 0x00207C6C
	public void Convert(SmartArray _sa, int _shiftBits)
	{
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					byte b = _sa.get(j, i, k);
					byte b2 = (byte)this.Get(j, i, k);
					b2 |= (byte)(b << _shiftBits);
					this.Set(j, i, k, (long)((ulong)b2));
				}
			}
		}
		this.CheckSameValue();
	}

	// Token: 0x06005535 RID: 21813 RVA: 0x00209AD8 File Offset: 0x00207CD8
	public void Convert(ChunkBlockLayerLegacy[] m_BlockLayers)
	{
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					byte b;
					if (m_BlockLayers[i] != null)
					{
						b = m_BlockLayers[i].GetStabilityAt(j, k);
					}
					else
					{
						b = 0;
					}
					this.Set(j, i, k, (long)((ulong)b));
				}
			}
		}
		this.CheckSameValue();
	}

	// Token: 0x04004227 RID: 16935
	public const int cElementsPerLayer = 1024;

	// Token: 0x04004228 RID: 16936
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly byte[] sameValue;

	// Token: 0x04004229 RID: 16937
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CBCLayer[] layers;

	// Token: 0x0400422A RID: 16938
	[PublicizedFrom(EAccessModifier.Private)]
	public long defaultValue;

	// Token: 0x0400422B RID: 16939
	[PublicizedFrom(EAccessModifier.Private)]
	public int bytesPerVal;
}
