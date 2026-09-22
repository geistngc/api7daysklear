using System;

namespace Platform
{
	// Token: 0x02001B63 RID: 7011
	public interface IAuthenticationServer
	{
		// Token: 0x0600D1F9 RID: 53753
		void Init(IPlatform _owner);

		// Token: 0x0600D1FA RID: 53754
		EBeginUserAuthenticationResult AuthenticateUser(ClientInfo _cInfo);

		// Token: 0x0600D1FB RID: 53755
		void RemoveUser(ClientInfo _cInfo);

		// Token: 0x0600D1FC RID: 53756
		void StartServer(AuthenticationSuccessfulCallbackDelegate _authSuccessfulDelegate, KickPlayerDelegate _kickPlayerDelegate);

		// Token: 0x0600D1FD RID: 53757
		void StartServerSteamGroups(SteamGroupStatusResponse _groupStatusResponseDelegate);

		// Token: 0x0600D1FE RID: 53758
		void StopServer();

		// Token: 0x0600D1FF RID: 53759
		bool RequestUserInGroupStatus(ClientInfo _cInfo, string _steamIdGroup);
	}
}
