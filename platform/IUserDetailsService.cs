using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001BBF RID: 7103
	public interface IUserDetailsService
	{
		// Token: 0x0600D39F RID: 54175
		void Init(IPlatform owner);

		// Token: 0x0600D3A0 RID: 54176
		void RequestUserDetailsUpdate(IReadOnlyList<UserDetailsRequest> requestedUsers, UserDetailsRequestCompleteHandler onComplete);
	}
}
