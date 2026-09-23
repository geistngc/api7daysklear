using System;
using System.Collections.Generic;

// Token: 0x02001403 RID: 5123
public class FastEnumIntEqualityComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct, IConvertible
{
	// Token: 0x0600A0E4 RID: 41188 RVA: 0x003C8FEA File Offset: 0x003C71EA
	[PublicizedFrom(EAccessModifier.Private)]
	public int ToInt(TEnum _enum)
	{
		return EnumInt32ToInt.Convert<TEnum>(_enum);
	}

	// Token: 0x0600A0E5 RID: 41189 RVA: 0x003C8FF2 File Offset: 0x003C71F2
	public bool Equals(TEnum firstEnum, TEnum secondEnum)
	{
		return this.ToInt(firstEnum) == this.ToInt(secondEnum);
	}

	// Token: 0x0600A0E6 RID: 41190 RVA: 0x003C9004 File Offset: 0x003C7204
	public int GetHashCode(TEnum firstEnum)
	{
		return this.ToInt(firstEnum);
	}
}
