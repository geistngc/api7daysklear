using System;
using UnityEngine.Scripting;

// Token: 0x020004F5 RID: 1269
[Preserve]
public class EntityZombie : EntityHuman
{
	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x0600298E RID: 10638 RVA: 0x00010E62 File Offset: 0x0000F062
	// (set) Token: 0x0600298F RID: 10639 RVA: 0x000027FC File Offset: 0x000009FC
	public override bool AimingGun
	{
		get
		{
			return false;
		}
		set
		{
		}
	}
}
