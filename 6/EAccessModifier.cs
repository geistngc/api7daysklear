using System;
using System.Runtime.InteropServices;

// Token: 0x02001D9A RID: 7578
[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
public enum EAccessModifier
{
	// Token: 0x0400ADBF RID: 44479
	Unknown,
	// Token: 0x0400ADC0 RID: 44480
	Public,
	// Token: 0x0400ADC1 RID: 44481
	Private,
	// Token: 0x0400ADC2 RID: 44482
	Protected,
	// Token: 0x0400ADC3 RID: 44483
	Internal,
	// Token: 0x0400ADC4 RID: 44484
	ProtectedInternal,
	// Token: 0x0400ADC5 RID: 44485
	PrivateProtected
}
