using System;

namespace SharpEXR
{
	// Token: 0x020016D1 RID: 5841
	[Flags]
	public enum EXRVersionFlags
	{
		// Token: 0x040088CF RID: 35023
		IsSinglePartTiled = 512,
		// Token: 0x040088D0 RID: 35024
		LongNames = 1024,
		// Token: 0x040088D1 RID: 35025
		NonImageParts = 2048,
		// Token: 0x040088D2 RID: 35026
		MultiPart = 4096
	}
}
