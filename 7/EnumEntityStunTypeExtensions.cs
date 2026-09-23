using System;
using UnityEngine.Scripting;

// Token: 0x02000469 RID: 1129
[Preserve]
public static class EnumEntityStunTypeExtensions
{
	// Token: 0x0600221C RID: 8732 RVA: 0x000CE3E3 File Offset: 0x000CC5E3
	public static bool CanMove(this EnumEntityStunType type)
	{
		return type == EnumEntityStunType.None;
	}
}
