using System;
using System.Diagnostics;
using System.Globalization;

namespace SharpEXR
{
	// Token: 0x020016D4 RID: 5844
	[Serializable]
	public struct Half : IComparable, IFormattable, IConvertible, IComparable<Half>, IEquatable<Half>
	{
		// Token: 0x0600B6A1 RID: 46753 RVA: 0x00440888 File Offset: 0x0043EA88
		public Half(float value)
		{
			this = HalfHelper.SingleToHalf(value);
		}

		// Token: 0x0600B6A2 RID: 46754 RVA: 0x00440896 File Offset: 0x0043EA96
		public Half(int value)
		{
			this = new Half((float)value);
		}

		// Token: 0x0600B6A3 RID: 46755 RVA: 0x00440896 File Offset: 0x0043EA96
		public Half(long value)
		{
			this = new Half((float)value);
		}

		// Token: 0x0600B6A4 RID: 46756 RVA: 0x00440896 File Offset: 0x0043EA96
		public Half(double value)
		{
			this = new Half((float)value);
		}

		// Token: 0x0600B6A5 RID: 46757 RVA: 0x004408A0 File Offset: 0x0043EAA0
		public Half(decimal value)
		{
			this = new Half((float)value);
		}

		// Token: 0x0600B6A6 RID: 46758 RVA: 0x004408AF File Offset: 0x0043EAAF
		public Half(uint value)
		{
			this = new Half(value);
		}

		// Token: 0x0600B6A7 RID: 46759 RVA: 0x004408AF File Offset: 0x0043EAAF
		public Half(ulong value)
		{
			this = new Half(value);
		}

		// Token: 0x0600B6A8 RID: 46760 RVA: 0x004408BA File Offset: 0x0043EABA
		public static Half Negate(Half half)
		{
			return -half;
		}

		// Token: 0x0600B6A9 RID: 46761 RVA: 0x004408C2 File Offset: 0x0043EAC2
		public static Half Add(Half half1, Half half2)
		{
			return half1 + half2;
		}

		// Token: 0x0600B6AA RID: 46762 RVA: 0x004408CB File Offset: 0x0043EACB
		public static Half Subtract(Half half1, Half half2)
		{
			return half1 - half2;
		}

		// Token: 0x0600B6AB RID: 46763 RVA: 0x004408D4 File Offset: 0x0043EAD4
		public static Half Multiply(Half half1, Half half2)
		{
			return half1 * half2;
		}

		// Token: 0x0600B6AC RID: 46764 RVA: 0x004408DD File Offset: 0x0043EADD
		public static Half Divide(Half half1, Half half2)
		{
			return half1 / half2;
		}

		// Token: 0x0600B6AD RID: 46765 RVA: 0x0012AA91 File Offset: 0x00128C91
		public static Half operator +(Half half)
		{
			return half;
		}

		// Token: 0x0600B6AE RID: 46766 RVA: 0x004408E6 File Offset: 0x0043EAE6
		public static Half operator -(Half half)
		{
			return HalfHelper.Negate(half);
		}

		// Token: 0x0600B6AF RID: 46767 RVA: 0x004408EE File Offset: 0x0043EAEE
		public static Half operator ++(Half half)
		{
			return (Half)(half + 1f);
		}

		// Token: 0x0600B6B0 RID: 46768 RVA: 0x00440901 File Offset: 0x0043EB01
		public static Half operator --(Half half)
		{
			return (Half)(half - 1f);
		}

		// Token: 0x0600B6B1 RID: 46769 RVA: 0x00440914 File Offset: 0x0043EB14
		public static Half operator +(Half half1, Half half2)
		{
			return (Half)(half1 + half2);
		}

		// Token: 0x0600B6B2 RID: 46770 RVA: 0x0044092A File Offset: 0x0043EB2A
		public static Half operator -(Half half1, Half half2)
		{
			return (Half)(half1 - half2);
		}

		// Token: 0x0600B6B3 RID: 46771 RVA: 0x00440940 File Offset: 0x0043EB40
		public static Half operator *(Half half1, Half half2)
		{
			return (Half)(half1 * half2);
		}

		// Token: 0x0600B6B4 RID: 46772 RVA: 0x00440956 File Offset: 0x0043EB56
		public static Half operator /(Half half1, Half half2)
		{
			return (Half)(half1 / half2);
		}

		// Token: 0x0600B6B5 RID: 46773 RVA: 0x0044096C File Offset: 0x0043EB6C
		public static bool operator ==(Half half1, Half half2)
		{
			return !Half.IsNaN(half1) && half1.value == half2.value;
		}

		// Token: 0x0600B6B6 RID: 46774 RVA: 0x00440986 File Offset: 0x0043EB86
		public static bool operator !=(Half half1, Half half2)
		{
			return half1.value != half2.value;
		}

		// Token: 0x0600B6B7 RID: 46775 RVA: 0x00440999 File Offset: 0x0043EB99
		public static bool operator <(Half half1, Half half2)
		{
			return half1 < half2;
		}

		// Token: 0x0600B6B8 RID: 46776 RVA: 0x004409AB File Offset: 0x0043EBAB
		public static bool operator >(Half half1, Half half2)
		{
			return half1 > half2;
		}

		// Token: 0x0600B6B9 RID: 46777 RVA: 0x004409BD File Offset: 0x0043EBBD
		public static bool operator <=(Half half1, Half half2)
		{
			return half1 == half2 || half1 < half2;
		}

		// Token: 0x0600B6BA RID: 46778 RVA: 0x004409D1 File Offset: 0x0043EBD1
		public static bool operator >=(Half half1, Half half2)
		{
			return half1 == half2 || half1 > half2;
		}

		// Token: 0x0600B6BB RID: 46779 RVA: 0x004409E5 File Offset: 0x0043EBE5
		public static implicit operator Half(byte value)
		{
			return new Half((float)value);
		}

		// Token: 0x0600B6BC RID: 46780 RVA: 0x004409E5 File Offset: 0x0043EBE5
		public static implicit operator Half(short value)
		{
			return new Half((float)value);
		}

		// Token: 0x0600B6BD RID: 46781 RVA: 0x004409E5 File Offset: 0x0043EBE5
		public static implicit operator Half(char value)
		{
			return new Half((float)value);
		}

		// Token: 0x0600B6BE RID: 46782 RVA: 0x004409E5 File Offset: 0x0043EBE5
		public static implicit operator Half(int value)
		{
			return new Half((float)value);
		}

		// Token: 0x0600B6BF RID: 46783 RVA: 0x004409E5 File Offset: 0x0043EBE5
		public static implicit operator Half(long value)
		{
			return new Half((float)value);
		}

		// Token: 0x0600B6C0 RID: 46784 RVA: 0x004409E5 File Offset: 0x0043EBE5
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x0600B6C1 RID: 46785 RVA: 0x004409E5 File Offset: 0x0043EBE5
		public static explicit operator Half(double value)
		{
			return new Half((float)value);
		}

		// Token: 0x0600B6C2 RID: 46786 RVA: 0x004409EE File Offset: 0x0043EBEE
		public static explicit operator Half(decimal value)
		{
			return new Half((float)value);
		}

		// Token: 0x0600B6C3 RID: 46787 RVA: 0x004409FC File Offset: 0x0043EBFC
		public static explicit operator byte(Half value)
		{
			return (byte)value;
		}

		// Token: 0x0600B6C4 RID: 46788 RVA: 0x00440A06 File Offset: 0x0043EC06
		public static explicit operator char(Half value)
		{
			return (char)value;
		}

		// Token: 0x0600B6C5 RID: 46789 RVA: 0x00440A10 File Offset: 0x0043EC10
		public static explicit operator short(Half value)
		{
			return (short)value;
		}

		// Token: 0x0600B6C6 RID: 46790 RVA: 0x00440A1A File Offset: 0x0043EC1A
		public static explicit operator int(Half value)
		{
			return (int)value;
		}

		// Token: 0x0600B6C7 RID: 46791 RVA: 0x00440A24 File Offset: 0x0043EC24
		public static explicit operator long(Half value)
		{
			return (long)value;
		}

		// Token: 0x0600B6C8 RID: 46792 RVA: 0x00440A2E File Offset: 0x0043EC2E
		public static implicit operator float(Half value)
		{
			return HalfHelper.HalfToSingle(value);
		}

		// Token: 0x0600B6C9 RID: 46793 RVA: 0x00440A37 File Offset: 0x0043EC37
		public static implicit operator double(Half value)
		{
			return (double)value;
		}

		// Token: 0x0600B6CA RID: 46794 RVA: 0x00440A41 File Offset: 0x0043EC41
		public static explicit operator decimal(Half value)
		{
			return (decimal)value;
		}

		// Token: 0x0600B6CB RID: 46795 RVA: 0x004409E5 File Offset: 0x0043EBE5
		public static implicit operator Half(sbyte value)
		{
			return new Half((float)value);
		}

		// Token: 0x0600B6CC RID: 46796 RVA: 0x004409E5 File Offset: 0x0043EBE5
		public static implicit operator Half(ushort value)
		{
			return new Half((float)value);
		}

		// Token: 0x0600B6CD RID: 46797 RVA: 0x00440A4F File Offset: 0x0043EC4F
		public static implicit operator Half(uint value)
		{
			return new Half(value);
		}

		// Token: 0x0600B6CE RID: 46798 RVA: 0x00440A4F File Offset: 0x0043EC4F
		public static implicit operator Half(ulong value)
		{
			return new Half(value);
		}

		// Token: 0x0600B6CF RID: 46799 RVA: 0x00440A59 File Offset: 0x0043EC59
		public static explicit operator sbyte(Half value)
		{
			return (sbyte)value;
		}

		// Token: 0x0600B6D0 RID: 46800 RVA: 0x00440A06 File Offset: 0x0043EC06
		public static explicit operator ushort(Half value)
		{
			return (ushort)value;
		}

		// Token: 0x0600B6D1 RID: 46801 RVA: 0x00440A63 File Offset: 0x0043EC63
		public static explicit operator uint(Half value)
		{
			return (uint)value;
		}

		// Token: 0x0600B6D2 RID: 46802 RVA: 0x00440A6D File Offset: 0x0043EC6D
		public static explicit operator ulong(Half value)
		{
			return (ulong)value;
		}

		// Token: 0x0600B6D3 RID: 46803 RVA: 0x00440A78 File Offset: 0x0043EC78
		public int CompareTo(Half other)
		{
			int result = 0;
			if (this < other)
			{
				result = -1;
			}
			else if (this > other)
			{
				result = 1;
			}
			else if (this != other)
			{
				if (!Half.IsNaN(this))
				{
					result = 1;
				}
				else if (!Half.IsNaN(other))
				{
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x0600B6D4 RID: 46804 RVA: 0x00440AD8 File Offset: 0x0043ECD8
		public int CompareTo(object obj)
		{
			int result;
			if (obj == null)
			{
				result = 1;
			}
			else
			{
				if (!(obj is Half))
				{
					throw new ArgumentException("Object must be of type Half.");
				}
				result = this.CompareTo((Half)obj);
			}
			return result;
		}

		// Token: 0x0600B6D5 RID: 46805 RVA: 0x00440B11 File Offset: 0x0043ED11
		public bool Equals(Half other)
		{
			return other == this || (Half.IsNaN(other) && Half.IsNaN(this));
		}

		// Token: 0x0600B6D6 RID: 46806 RVA: 0x00440B38 File Offset: 0x0043ED38
		public override bool Equals(object obj)
		{
			bool result = false;
			if (obj is Half)
			{
				Half half = (Half)obj;
				if (half == this || (Half.IsNaN(half) && Half.IsNaN(this)))
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600B6D7 RID: 46807 RVA: 0x00440B7C File Offset: 0x0043ED7C
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		// Token: 0x0600B6D8 RID: 46808 RVA: 0x0004E4E2 File Offset: 0x0004C6E2
		public TypeCode GetTypeCode()
		{
			return (TypeCode)255;
		}

		// Token: 0x0600B6D9 RID: 46809 RVA: 0x00440B89 File Offset: 0x0043ED89
		public static byte[] GetBytes(Half value)
		{
			return BitConverter.GetBytes(value.value);
		}

		// Token: 0x0600B6DA RID: 46810 RVA: 0x00440B96 File Offset: 0x0043ED96
		public static ushort GetBits(Half value)
		{
			return value.value;
		}

		// Token: 0x0600B6DB RID: 46811 RVA: 0x00440B9E File Offset: 0x0043ED9E
		public static Half ToHalf(byte[] value, int startIndex)
		{
			return Half.ToHalf((ushort)BitConverter.ToInt16(value, startIndex));
		}

		// Token: 0x0600B6DC RID: 46812 RVA: 0x00440BB0 File Offset: 0x0043EDB0
		public static Half ToHalf(ushort bits)
		{
			return new Half
			{
				value = bits
			};
		}

		// Token: 0x0600B6DD RID: 46813 RVA: 0x00440BCE File Offset: 0x0043EDCE
		public static int Sign(Half value)
		{
			if (value < 0)
			{
				return -1;
			}
			if (value > 0)
			{
				return 1;
			}
			if (value != 0)
			{
				throw new ArithmeticException("Function does not accept floating point Not-a-Number values.");
			}
			return 0;
		}

		// Token: 0x0600B6DE RID: 46814 RVA: 0x00440C0A File Offset: 0x0043EE0A
		public static Half Abs(Half value)
		{
			return HalfHelper.Abs(value);
		}

		// Token: 0x0600B6DF RID: 46815 RVA: 0x00440C12 File Offset: 0x0043EE12
		public static Half Max(Half value1, Half value2)
		{
			if (!(value1 < value2))
			{
				return value1;
			}
			return value2;
		}

		// Token: 0x0600B6E0 RID: 46816 RVA: 0x00440C20 File Offset: 0x0043EE20
		public static Half Min(Half value1, Half value2)
		{
			if (!(value1 < value2))
			{
				return value2;
			}
			return value1;
		}

		// Token: 0x0600B6E1 RID: 46817 RVA: 0x00440C2E File Offset: 0x0043EE2E
		public static bool IsNaN(Half half)
		{
			return HalfHelper.IsNaN(half);
		}

		// Token: 0x0600B6E2 RID: 46818 RVA: 0x00440C36 File Offset: 0x0043EE36
		public static bool IsInfinity(Half half)
		{
			return HalfHelper.IsInfinity(half);
		}

		// Token: 0x0600B6E3 RID: 46819 RVA: 0x00440C3E File Offset: 0x0043EE3E
		public static bool IsNegativeInfinity(Half half)
		{
			return HalfHelper.IsNegativeInfinity(half);
		}

		// Token: 0x0600B6E4 RID: 46820 RVA: 0x00440C46 File Offset: 0x0043EE46
		public static bool IsPositiveInfinity(Half half)
		{
			return HalfHelper.IsPositiveInfinity(half);
		}

		// Token: 0x0600B6E5 RID: 46821 RVA: 0x00440C4E File Offset: 0x0043EE4E
		public static Half Parse(string value)
		{
			return (Half)float.Parse(value, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600B6E6 RID: 46822 RVA: 0x00440C60 File Offset: 0x0043EE60
		public static Half Parse(string value, IFormatProvider provider)
		{
			return (Half)float.Parse(value, provider);
		}

		// Token: 0x0600B6E7 RID: 46823 RVA: 0x00440C6E File Offset: 0x0043EE6E
		public static Half Parse(string value, NumberStyles style)
		{
			return (Half)float.Parse(value, style, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600B6E8 RID: 46824 RVA: 0x00440C81 File Offset: 0x0043EE81
		public static Half Parse(string value, NumberStyles style, IFormatProvider provider)
		{
			return (Half)float.Parse(value, style, provider);
		}

		// Token: 0x0600B6E9 RID: 46825 RVA: 0x00440C90 File Offset: 0x0043EE90
		public static bool TryParse(string value, out Half result)
		{
			float num;
			if (float.TryParse(value, out num))
			{
				result = (Half)num;
				return true;
			}
			result = default(Half);
			return false;
		}

		// Token: 0x0600B6EA RID: 46826 RVA: 0x00440CC0 File Offset: 0x0043EEC0
		public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out Half result)
		{
			bool result2 = false;
			float num;
			if (float.TryParse(value, style, provider, out num))
			{
				result = (Half)num;
				result2 = true;
			}
			else
			{
				result = default(Half);
			}
			return result2;
		}

		// Token: 0x0600B6EB RID: 46827 RVA: 0x00440CF4 File Offset: 0x0043EEF4
		public override string ToString()
		{
			return this.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x0600B6EC RID: 46828 RVA: 0x00440D1C File Offset: 0x0043EF1C
		public string ToString(IFormatProvider formatProvider)
		{
			return this.ToString(formatProvider);
		}

		// Token: 0x0600B6ED RID: 46829 RVA: 0x00440D40 File Offset: 0x0043EF40
		public string ToString(string format)
		{
			return this.ToString(format, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600B6EE RID: 46830 RVA: 0x00440D68 File Offset: 0x0043EF68
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return this.ToString(format, formatProvider);
		}

		// Token: 0x0600B6EF RID: 46831 RVA: 0x00440D8B File Offset: 0x0043EF8B
		[PublicizedFrom(EAccessModifier.Private)]
		public float ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x0600B6F0 RID: 46832 RVA: 0x00440D99 File Offset: 0x0043EF99
		[PublicizedFrom(EAccessModifier.Private)]
		public TypeCode GetTypeCode()
		{
			return this.GetTypeCode();
		}

		// Token: 0x0600B6F1 RID: 46833 RVA: 0x00440DA1 File Offset: 0x0043EFA1
		[PublicizedFrom(EAccessModifier.Private)]
		public bool ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x0600B6F2 RID: 46834 RVA: 0x00440DB4 File Offset: 0x0043EFB4
		[PublicizedFrom(EAccessModifier.Private)]
		public byte ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x0600B6F3 RID: 46835 RVA: 0x00440DC7 File Offset: 0x0043EFC7
		[PublicizedFrom(EAccessModifier.Private)]
		public char ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Invalid cast from '{0}' to '{1}'.", "Half", "Char"));
		}

		// Token: 0x0600B6F4 RID: 46836 RVA: 0x00440DE7 File Offset: 0x0043EFE7
		[PublicizedFrom(EAccessModifier.Private)]
		public DateTime ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Invalid cast from '{0}' to '{1}'.", "Half", "DateTime"));
		}

		// Token: 0x0600B6F5 RID: 46837 RVA: 0x00440E07 File Offset: 0x0043F007
		[PublicizedFrom(EAccessModifier.Private)]
		public decimal ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x0600B6F6 RID: 46838 RVA: 0x00440E1A File Offset: 0x0043F01A
		[PublicizedFrom(EAccessModifier.Private)]
		public double ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x0600B6F7 RID: 46839 RVA: 0x00440E2D File Offset: 0x0043F02D
		[PublicizedFrom(EAccessModifier.Private)]
		public short ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x0600B6F8 RID: 46840 RVA: 0x00440E40 File Offset: 0x0043F040
		[PublicizedFrom(EAccessModifier.Private)]
		public int ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x0600B6F9 RID: 46841 RVA: 0x00440E53 File Offset: 0x0043F053
		[PublicizedFrom(EAccessModifier.Private)]
		public long ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x0600B6FA RID: 46842 RVA: 0x00440E66 File Offset: 0x0043F066
		[PublicizedFrom(EAccessModifier.Private)]
		public sbyte ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x0600B6FB RID: 46843 RVA: 0x00440E79 File Offset: 0x0043F079
		[PublicizedFrom(EAccessModifier.Private)]
		public string ToString(IFormatProvider provider)
		{
			return Convert.ToString(this, CultureInfo.InvariantCulture);
		}

		// Token: 0x0600B6FC RID: 46844 RVA: 0x00440E91 File Offset: 0x0043F091
		[PublicizedFrom(EAccessModifier.Private)]
		public object ToType(Type conversionType, IFormatProvider provider)
		{
			return ((IConvertible)this).ToType(conversionType, provider);
		}

		// Token: 0x0600B6FD RID: 46845 RVA: 0x00440EAB File Offset: 0x0043F0AB
		[PublicizedFrom(EAccessModifier.Private)]
		public ushort ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x0600B6FE RID: 46846 RVA: 0x00440EBE File Offset: 0x0043F0BE
		[PublicizedFrom(EAccessModifier.Private)]
		public uint ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x0600B6FF RID: 46847 RVA: 0x00440ED1 File Offset: 0x0043F0D1
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x040088D8 RID: 35032
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[NonSerialized]
		public ushort value;

		// Token: 0x040088D9 RID: 35033
		public static readonly Half Epsilon = Half.ToHalf(1);

		// Token: 0x040088DA RID: 35034
		public static readonly Half MaxValue = Half.ToHalf(31743);

		// Token: 0x040088DB RID: 35035
		public static readonly Half MinValue = Half.ToHalf(64511);

		// Token: 0x040088DC RID: 35036
		public static readonly Half NaN = Half.ToHalf(65024);

		// Token: 0x040088DD RID: 35037
		public static readonly Half NegativeInfinity = Half.ToHalf(64512);

		// Token: 0x040088DE RID: 35038
		public static readonly Half PositiveInfinity = Half.ToHalf(31744);
	}
}
