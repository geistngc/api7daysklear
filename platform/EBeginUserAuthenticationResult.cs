using System;

namespace Platform
{
	// Token: 0x02001B67 RID: 7015
	public enum EBeginUserAuthenticationResult
	{
		// Token: 0x0400A0DA RID: 41178
		Ok,
		// Token: 0x0400A0DB RID: 41179
		InvalidTicket,
		// Token: 0x0400A0DC RID: 41180
		DuplicateRequest,
		// Token: 0x0400A0DD RID: 41181
		InvalidVersion,
		// Token: 0x0400A0DE RID: 41182
		GameMismatch,
		// Token: 0x0400A0DF RID: 41183
		ExpiredTicket
	}
}
