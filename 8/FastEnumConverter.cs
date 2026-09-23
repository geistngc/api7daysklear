using System;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02001404 RID: 5124
public static class FastEnumConverter<TEnum> where TEnum : struct, IConvertible
{
	// Token: 0x0600A0E8 RID: 41192 RVA: 0x003C900D File Offset: 0x003C720D
	public static int ToInt(TEnum _enum)
	{
		return (int)((long)UnsafeUtility.EnumToInt<TEnum>(_enum) & FastEnumConverter<TEnum>.underlyingMask);
	}

	// Token: 0x04007988 RID: 31112
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly int underlyingSize = UnsafeUtility.SizeOf(Enum.GetUnderlyingType(typeof(TEnum)));

	// Token: 0x04007989 RID: 31113
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly long underlyingMask = (FastEnumConverter<TEnum>.underlyingSize >= 8) ? -1L : ((1L << FastEnumConverter<TEnum>.underlyingSize * 8) - 1L);
}
