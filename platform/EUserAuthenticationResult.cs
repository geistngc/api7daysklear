using System;

namespace Platform
{
	// Token: 0x02001B68 RID: 7016
	public enum EUserAuthenticationResult
	{
		// Token: 0x0400A0E1 RID: 41185
		Ok,
		// Token: 0x0400A0E2 RID: 41186
		UserNotConnectedToPlatform,
		// Token: 0x0400A0E3 RID: 41187
		NoLicenseOrExpired,
		// Token: 0x0400A0E4 RID: 41188
		PlatformBanned,
		// Token: 0x0400A0E5 RID: 41189
		LoggedInElseWhere,
		// Token: 0x0400A0E6 RID: 41190
		PlatformBanCheckTimedOut,
		// Token: 0x0400A0E7 RID: 41191
		AuthTicketCanceled,
		// Token: 0x0400A0E8 RID: 41192
		AuthTicketInvalidAlreadyUsed,
		// Token: 0x0400A0E9 RID: 41193
		AuthTicketInvalid,
		// Token: 0x0400A0EA RID: 41194
		PublisherIssuedBan,
		// Token: 0x0400A0EB RID: 41195
		EosTicketFailed = 50
	}
}
