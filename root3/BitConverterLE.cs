using System;

// Token: 0x020013A7 RID: 5031
[PublicizedFrom(EAccessModifier.Internal)]
public static class BitConverterLE
{
	// Token: 0x06009E9E RID: 40606 RVA: 0x003BFE6F File Offset: 0x003BE06F
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static void GetUIntBytes(byte* bytes, byte[] _buffer)
	{
		if (BitConverter.IsLittleEndian)
		{
			_buffer[0] = *bytes;
			_buffer[1] = bytes[1];
			_buffer[2] = bytes[2];
			_buffer[3] = bytes[3];
			return;
		}
		_buffer[0] = bytes[3];
		_buffer[1] = bytes[2];
		_buffer[2] = bytes[1];
		_buffer[3] = *bytes;
	}

	// Token: 0x06009E9F RID: 40607 RVA: 0x003BFEB0 File Offset: 0x003BE0B0
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static void GetULongBytes(byte* bytes, byte[] _buffer)
	{
		if (BitConverter.IsLittleEndian)
		{
			_buffer[0] = *bytes;
			_buffer[1] = bytes[1];
			_buffer[2] = bytes[2];
			_buffer[3] = bytes[3];
			_buffer[4] = bytes[4];
			_buffer[5] = bytes[5];
			_buffer[6] = bytes[6];
			_buffer[7] = bytes[7];
			return;
		}
		_buffer[0] = bytes[7];
		_buffer[1] = bytes[6];
		_buffer[2] = bytes[5];
		_buffer[3] = bytes[4];
		_buffer[4] = bytes[3];
		_buffer[5] = bytes[2];
		_buffer[6] = bytes[1];
		_buffer[7] = *bytes;
	}

	// Token: 0x06009EA0 RID: 40608 RVA: 0x003BFF31 File Offset: 0x003BE131
	public unsafe static void GetBytes(float _value, byte[] _buffer)
	{
		BitConverterLE.GetUIntBytes((byte*)(&_value), _buffer);
	}

	// Token: 0x06009EA1 RID: 40609 RVA: 0x003BFF3C File Offset: 0x003BE13C
	public unsafe static void GetBytes(double value, byte[] _buffer)
	{
		BitConverterLE.GetULongBytes((byte*)(&value), _buffer);
	}

	// Token: 0x06009EA2 RID: 40610 RVA: 0x003BFF48 File Offset: 0x003BE148
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static void UIntFromBytes(byte* _dst, byte[] _src, int _startIndex)
	{
		if (BitConverter.IsLittleEndian)
		{
			*_dst = _src[_startIndex];
			_dst[1] = _src[_startIndex + 1];
			_dst[2] = _src[_startIndex + 2];
			_dst[3] = _src[_startIndex + 3];
			return;
		}
		*_dst = _src[_startIndex + 3];
		_dst[1] = _src[_startIndex + 2];
		_dst[2] = _src[_startIndex + 1];
		_dst[3] = _src[_startIndex];
	}

	// Token: 0x06009EA3 RID: 40611 RVA: 0x003BFFA0 File Offset: 0x003BE1A0
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static void ULongFromBytes(byte* _dst, byte[] _src, int _startIndex)
	{
		if (BitConverter.IsLittleEndian)
		{
			for (int i = 0; i < 8; i++)
			{
				_dst[i] = _src[_startIndex + i];
			}
			return;
		}
		for (int j = 0; j < 8; j++)
		{
			_dst[j] = _src[_startIndex + (7 - j)];
		}
	}

	// Token: 0x06009EA4 RID: 40612 RVA: 0x003BFFE4 File Offset: 0x003BE1E4
	public unsafe static float ToSingle(byte[] _value, int _startIndex)
	{
		float result;
		BitConverterLE.UIntFromBytes((byte*)(&result), _value, _startIndex);
		return result;
	}

	// Token: 0x06009EA5 RID: 40613 RVA: 0x003BFFFC File Offset: 0x003BE1FC
	public unsafe static double ToDouble(byte[] _value, int _startIndex)
	{
		double result;
		BitConverterLE.ULongFromBytes((byte*)(&result), _value, _startIndex);
		return result;
	}
}
