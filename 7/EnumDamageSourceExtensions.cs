using System;

// Token: 0x0200053F RID: 1343
public static class EnumDamageSourceExtensions
{
	// Token: 0x06002C2C RID: 11308 RVA: 0x000CE3E3 File Offset: 0x000CC5E3
	public static bool AffectedByArmor(this EnumDamageSource s)
	{
		return s == EnumDamageSource.External;
	}
}
