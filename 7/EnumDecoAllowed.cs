using System;

// Token: 0x02000C20 RID: 3104
[Flags]
public enum EnumDecoAllowed : byte
{
	// Token: 0x0400499B RID: 18843
	Everything = 0,
	// Token: 0x0400499C RID: 18844
	SlopeLo = 1,
	// Token: 0x0400499D RID: 18845
	SlopeHi = 2,
	// Token: 0x0400499E RID: 18846
	SizeLo = 4,
	// Token: 0x0400499F RID: 18847
	SizeHi = 8,
	// Token: 0x040049A0 RID: 18848
	StreetOnly = 16,
	// Token: 0x040049A1 RID: 18849
	Nothing = 255
}
