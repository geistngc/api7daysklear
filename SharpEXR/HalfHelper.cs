using System;
using System.Runtime.InteropServices;

namespace SharpEXR
{
	// Token: 0x020016D5 RID: 5845
	[ComVisible(false)]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static class HalfHelper
	{
		// Token: 0x0600B701 RID: 46849 RVA: 0x00440F48 File Offset: 0x0043F148
		[PublicizedFrom(EAccessModifier.Private)]
		public static uint ConvertMantissa(int i)
		{
			uint num = (uint)((uint)i << 13);
			uint num2 = 0U;
			while ((num & 8388608U) == 0U)
			{
				num2 -= 8388608U;
				num <<= 1;
			}
			num &= 4286578687U;
			num2 += 947912704U;
			return num | num2;
		}

		// Token: 0x0600B702 RID: 46850 RVA: 0x00440F88 File Offset: 0x0043F188
		[PublicizedFrom(EAccessModifier.Private)]
		public static uint[] GenerateMantissaTable()
		{
			uint[] array = new uint[2048];
			array[0] = 0U;
			for (int i = 1; i < 1024; i++)
			{
				array[i] = HalfHelper.ConvertMantissa(i);
			}
			for (int j = 1024; j < 2048; j++)
			{
				array[j] = (uint)(939524096 + (j - 1024 << 13));
			}
			return array;
		}

		// Token: 0x0600B703 RID: 46851 RVA: 0x00440FE8 File Offset: 0x0043F1E8
		[PublicizedFrom(EAccessModifier.Private)]
		public static uint[] GenerateExponentTable()
		{
			uint[] array = new uint[64];
			array[0] = 0U;
			for (int i = 1; i < 31; i++)
			{
				array[i] = (uint)((uint)i << 23);
			}
			array[31] = 1199570944U;
			array[32] = 2147483648U;
			for (int j = 33; j < 63; j++)
			{
				array[j] = (uint)((ulong)int.MinValue + (ulong)((long)((long)(j - 32) << 23)));
			}
			array[63] = 3347054592U;
			return array;
		}

		// Token: 0x0600B704 RID: 46852 RVA: 0x00441054 File Offset: 0x0043F254
		[PublicizedFrom(EAccessModifier.Private)]
		public static ushort[] GenerateOffsetTable()
		{
			ushort[] array = new ushort[64];
			array[0] = 0;
			for (int i = 1; i < 32; i++)
			{
				array[i] = 1024;
			}
			array[32] = 0;
			for (int j = 33; j < 64; j++)
			{
				array[j] = 1024;
			}
			return array;
		}

		// Token: 0x0600B705 RID: 46853 RVA: 0x004410A0 File Offset: 0x0043F2A0
		[PublicizedFrom(EAccessModifier.Private)]
		public static ushort[] GenerateBaseTable()
		{
			ushort[] array = new ushort[512];
			for (int i = 0; i < 256; i++)
			{
				sbyte b = (sbyte)(127 - i);
				if (b > 24)
				{
					array[i | 0] = 0;
					array[i | 256] = 32768;
				}
				else if (b > 14)
				{
					array[i | 0] = (ushort)(1024 >> (int)(18 + b));
					array[i | 256] = (ushort)(1024 >> (int)(18 + b) | 32768);
				}
				else if (b >= -15)
				{
					array[i | 0] = (ushort)(15 - b << 10);
					array[i | 256] = (ushort)((int)(15 - b) << 10 | 32768);
				}
				else if (b > -128)
				{
					array[i | 0] = 31744;
					array[i | 256] = 64512;
				}
				else
				{
					array[i | 0] = 31744;
					array[i | 256] = 64512;
				}
			}
			return array;
		}

		// Token: 0x0600B706 RID: 46854 RVA: 0x0044118C File Offset: 0x0043F38C
		[PublicizedFrom(EAccessModifier.Private)]
		public static sbyte[] GenerateShiftTable()
		{
			sbyte[] array = new sbyte[512];
			for (int i = 0; i < 256; i++)
			{
				sbyte b = (sbyte)(127 - i);
				if (b > 24)
				{
					array[i | 0] = 24;
					array[i | 256] = 24;
				}
				else if (b > 14)
				{
					array[i | 0] = b - 1;
					array[i | 256] = b - 1;
				}
				else if (b >= -15)
				{
					array[i | 0] = 13;
					array[i | 256] = 13;
				}
				else if (b > -128)
				{
					array[i | 0] = 24;
					array[i | 256] = 24;
				}
				else
				{
					array[i | 0] = 13;
					array[i | 256] = 13;
				}
			}
			return array;
		}

		// Token: 0x0600B707 RID: 46855 RVA: 0x0044123C File Offset: 0x0043F43C
		public unsafe static float HalfToSingle(Half half)
		{
			uint num = HalfHelper.mantissaTable[(int)(HalfHelper.offsetTable[half.value >> 10] + (half.value & 1023))] + HalfHelper.exponentTable[half.value >> 10];
			return *(float*)(&num);
		}

		// Token: 0x0600B708 RID: 46856 RVA: 0x00441280 File Offset: 0x0043F480
		public unsafe static Half SingleToHalf(float single)
		{
			uint num = *(uint*)(&single);
			return Half.ToHalf((ushort)((uint)HalfHelper.baseTable[(int)(num >> 23 & 511U)] + ((num & 8388607U) >> (int)HalfHelper.shiftTable[(int)(num >> 23)])));
		}

		// Token: 0x0600B709 RID: 46857 RVA: 0x004412BE File Offset: 0x0043F4BE
		public static Half Negate(Half half)
		{
			return Half.ToHalf(half.value ^ 32768);
		}

		// Token: 0x0600B70A RID: 46858 RVA: 0x004412D2 File Offset: 0x0043F4D2
		public static Half Abs(Half half)
		{
			return Half.ToHalf(half.value & 32767);
		}

		// Token: 0x0600B70B RID: 46859 RVA: 0x004412E6 File Offset: 0x0043F4E6
		public static bool IsNaN(Half half)
		{
			return (half.value & 32767) > 31744;
		}

		// Token: 0x0600B70C RID: 46860 RVA: 0x004412FB File Offset: 0x0043F4FB
		public static bool IsInfinity(Half half)
		{
			return (half.value & 32767) == 31744;
		}

		// Token: 0x0600B70D RID: 46861 RVA: 0x00441310 File Offset: 0x0043F510
		public static bool IsPositiveInfinity(Half half)
		{
			return half.value == 31744;
		}

		// Token: 0x0600B70E RID: 46862 RVA: 0x0044131F File Offset: 0x0043F51F
		public static bool IsNegativeInfinity(Half half)
		{
			return half.value == 64512;
		}

		// Token: 0x040088DF RID: 35039
		[PublicizedFrom(EAccessModifier.Private)]
		public static uint[] mantissaTable = HalfHelper.GenerateMantissaTable();

		// Token: 0x040088E0 RID: 35040
		[PublicizedFrom(EAccessModifier.Private)]
		public static uint[] exponentTable = HalfHelper.GenerateExponentTable();

		// Token: 0x040088E1 RID: 35041
		[PublicizedFrom(EAccessModifier.Private)]
		public static ushort[] offsetTable = HalfHelper.GenerateOffsetTable();

		// Token: 0x040088E2 RID: 35042
		[PublicizedFrom(EAccessModifier.Private)]
		public static ushort[] baseTable = HalfHelper.GenerateBaseTable();

		// Token: 0x040088E3 RID: 35043
		[PublicizedFrom(EAccessModifier.Private)]
		public static sbyte[] shiftTable = HalfHelper.GenerateShiftTable();
	}
}
