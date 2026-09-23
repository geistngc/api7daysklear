using System;

// Token: 0x0200048E RID: 1166
[Flags]
public enum EntityFlags : uint
{
	// Token: 0x040019A6 RID: 6566
	None = 0U,
	// Token: 0x040019A7 RID: 6567
	Player = 1U,
	// Token: 0x040019A8 RID: 6568
	Zombie = 2U,
	// Token: 0x040019A9 RID: 6569
	Animal = 4U,
	// Token: 0x040019AA RID: 6570
	Bandit = 8U,
	// Token: 0x040019AB RID: 6571
	Edible = 16U,
	// Token: 0x040019AC RID: 6572
	Timid = 32U,
	// Token: 0x040019AD RID: 6573
	All = 4294967295U,
	// Token: 0x040019AE RID: 6574
	AIHearing = 14U,
	// Token: 0x040019AF RID: 6575
	AISmelling = 6U
}
