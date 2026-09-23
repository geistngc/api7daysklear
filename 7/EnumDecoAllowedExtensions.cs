using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// Token: 0x02000C23 RID: 3107
public static class EnumDecoAllowedExtensions
{
	// Token: 0x06005EB9 RID: 24249 RVA: 0x0024FBB1 File Offset: 0x0024DDB1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static EnumDecoAllowedSlope GetSlope(this EnumDecoAllowed decoAllowed)
	{
		return (EnumDecoAllowedSlope)((decoAllowed & (EnumDecoAllowed.SlopeLo | EnumDecoAllowed.SlopeHi)) / EnumDecoAllowed.SlopeLo);
	}

	// Token: 0x06005EBA RID: 24250 RVA: 0x0024FBB9 File Offset: 0x0024DDB9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static EnumDecoAllowed WithSlope(this EnumDecoAllowed decoAllowed, EnumDecoAllowedSlope slope)
	{
		return (EnumDecoAllowed)(((int)decoAllowed & -4) | (int)slope);
	}

	// Token: 0x06005EBB RID: 24251 RVA: 0x0024FBC2 File Offset: 0x0024DDC2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static EnumDecoAllowedSize GetSize(this EnumDecoAllowed decoAllowed)
	{
		return (EnumDecoAllowedSize)((decoAllowed & (EnumDecoAllowed.SizeLo | EnumDecoAllowed.SizeHi)) / EnumDecoAllowed.SizeLo);
	}

	// Token: 0x06005EBC RID: 24252 RVA: 0x0024FBCB File Offset: 0x0024DDCB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static EnumDecoAllowed WithSize(this EnumDecoAllowed decoAllowed, EnumDecoAllowedSize size)
	{
		return (EnumDecoAllowed)(((int)decoAllowed & -13) | (int)(size * (EnumDecoAllowedSize)4));
	}

	// Token: 0x06005EBD RID: 24253 RVA: 0x0024FBD6 File Offset: 0x0024DDD6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool AllowBigDeco(this EnumDecoAllowed decoAllowed)
	{
		return decoAllowed.GetSize() == EnumDecoAllowedSize.Any;
	}

	// Token: 0x06005EBE RID: 24254 RVA: 0x0024FBE1 File Offset: 0x0024DDE1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool AllowSmallDeco(this EnumDecoAllowed decoAllowed)
	{
		return decoAllowed.GetSize() < EnumDecoAllowedSize.None;
	}

	// Token: 0x06005EBF RID: 24255 RVA: 0x0024FBEC File Offset: 0x0024DDEC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool GetStreetOnly(this EnumDecoAllowed decoAllowed)
	{
		return (decoAllowed & EnumDecoAllowed.StreetOnly) == EnumDecoAllowed.StreetOnly;
	}

	// Token: 0x06005EC0 RID: 24256 RVA: 0x0024FBF6 File Offset: 0x0024DDF6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static EnumDecoAllowed WithStreetOnly(this EnumDecoAllowed decoAllowed, bool streetOnly)
	{
		if (streetOnly)
		{
			return decoAllowed | EnumDecoAllowed.StreetOnly;
		}
		return (EnumDecoAllowed)((int)decoAllowed & -17);
	}

	// Token: 0x06005EC1 RID: 24257 RVA: 0x0024FC06 File Offset: 0x0024DE06
	public static bool IsNothing(this EnumDecoAllowed decoAllowed)
	{
		return decoAllowed.GetSlope().IsNothing() || decoAllowed.GetSize().IsNothing();
	}

	// Token: 0x06005EC2 RID: 24258 RVA: 0x0024FC22 File Offset: 0x0024DE22
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNothing(this EnumDecoAllowedSlope decoSlope)
	{
		return decoSlope >= EnumDecoAllowedSlope.Steep;
	}

	// Token: 0x06005EC3 RID: 24259 RVA: 0x0024FC22 File Offset: 0x0024DE22
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNothing(this EnumDecoAllowedSize decoSize)
	{
		return decoSize >= EnumDecoAllowedSize.None;
	}

	// Token: 0x06005EC4 RID: 24260 RVA: 0x0024FC2C File Offset: 0x0024DE2C
	public static string ToStringFriendlyCached(this EnumDecoAllowed decoAllowed)
	{
		string result;
		if (EnumDecoAllowedExtensions.s_toStringCache.TryGetValue(decoAllowed, out result))
		{
			return result;
		}
		string text = EnumDecoAllowedExtensions.ToStringInternal(decoAllowed);
		EnumDecoAllowedExtensions.s_toStringCache[decoAllowed] = text;
		return text;
	}

	// Token: 0x06005EC5 RID: 24261 RVA: 0x0024FC60 File Offset: 0x0024DE60
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ToStringInternal(EnumDecoAllowed decoAllowed)
	{
		if (decoAllowed == EnumDecoAllowed.Everything)
		{
			return "Everything";
		}
		if (decoAllowed == EnumDecoAllowed.Nothing)
		{
			return "Nothing";
		}
		List<string> list = new List<string>();
		EnumDecoAllowedSlope slope = decoAllowed.GetSlope();
		if (slope > EnumDecoAllowedSlope.Flat)
		{
			list.Add(slope.ToStringCached<EnumDecoAllowedSlope>());
		}
		EnumDecoAllowedSize size = decoAllowed.GetSize();
		if (size > EnumDecoAllowedSize.Any)
		{
			list.Add(size.ToStringCached<EnumDecoAllowedSize>());
		}
		if (decoAllowed.GetStreetOnly())
		{
			list.Add("StreetOnly");
		}
		if (list.Count > 0)
		{
			return string.Join(",", list);
		}
		return "Unknown";
	}

	// Token: 0x040049AB RID: 18859
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly EnumDictionary<EnumDecoAllowed, string> s_toStringCache = new EnumDictionary<EnumDecoAllowed, string>();
}
