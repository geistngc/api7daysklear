using System;

namespace Platform
{
	// Token: 0x02001B5F RID: 7007
	public interface IAuthenticationClient
	{
		// Token: 0x0600D1E6 RID: 53734
		void Init(IPlatform _owner);

		// Token: 0x0600D1E7 RID: 53735
		string GetAuthTicket();

		// Token: 0x0600D1E8 RID: 53736
		void AuthenticateServer(ClientAuthenticateServerContext _context);

		// Token: 0x0600D1E9 RID: 53737
		void Destroy();
	}
}
