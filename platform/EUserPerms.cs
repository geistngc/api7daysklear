using System;

namespace Platform
{
	// Token: 0x02001BB1 RID: 7089
	[Flags]
	public enum EUserPerms
	{
		// Token: 0x0400A15E RID: 41310
		Multiplayer = 1,
		// Token: 0x0400A15F RID: 41311
		Communication = 2,
		// Token: 0x0400A160 RID: 41312
		Crossplay = 4,
		// Token: 0x0400A161 RID: 41313
		HostMultiplayer = 8,
		// Token: 0x0400A162 RID: 41314
		All = 15
	}
}
