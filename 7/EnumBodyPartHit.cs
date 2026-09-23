using System;

// Token: 0x0200046B RID: 1131
[Flags]
public enum EnumBodyPartHit
{
	// Token: 0x040017B8 RID: 6072
	None = 0,
	// Token: 0x040017B9 RID: 6073
	Torso = 1,
	// Token: 0x040017BA RID: 6074
	Head = 2,
	// Token: 0x040017BB RID: 6075
	LeftUpperArm = 4,
	// Token: 0x040017BC RID: 6076
	RightUpperArm = 8,
	// Token: 0x040017BD RID: 6077
	LeftUpperLeg = 16,
	// Token: 0x040017BE RID: 6078
	RightUpperLeg = 32,
	// Token: 0x040017BF RID: 6079
	LeftLowerArm = 64,
	// Token: 0x040017C0 RID: 6080
	RightLowerArm = 128,
	// Token: 0x040017C1 RID: 6081
	LeftLowerLeg = 256,
	// Token: 0x040017C2 RID: 6082
	RightLowerLeg = 512,
	// Token: 0x040017C3 RID: 6083
	Special = 1024,
	// Token: 0x040017C4 RID: 6084
	UpperArms = 12,
	// Token: 0x040017C5 RID: 6085
	LowerArms = 192,
	// Token: 0x040017C6 RID: 6086
	Arms = 204,
	// Token: 0x040017C7 RID: 6087
	UpperLegs = 48,
	// Token: 0x040017C8 RID: 6088
	LowerLegs = 768,
	// Token: 0x040017C9 RID: 6089
	Legs = 816,
	// Token: 0x040017CA RID: 6090
	BitsUsed = 11
}
