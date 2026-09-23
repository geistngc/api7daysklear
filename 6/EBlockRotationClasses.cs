using System;

// Token: 0x02000104 RID: 260
[Flags]
public enum EBlockRotationClasses
{
	// Token: 0x04000705 RID: 1797
	None = 0,
	// Token: 0x04000706 RID: 1798
	Basic90 = 1,
	// Token: 0x04000707 RID: 1799
	Headfirst = 2,
	// Token: 0x04000708 RID: 1800
	Sideways = 4,
	// Token: 0x04000709 RID: 1801
	Basic45 = 8,
	// Token: 0x0400070A RID: 1802
	Basic90And45 = 9,
	// Token: 0x0400070B RID: 1803
	No45 = 7,
	// Token: 0x0400070C RID: 1804
	Advanced = 6,
	// Token: 0x0400070D RID: 1805
	All = 15
}
