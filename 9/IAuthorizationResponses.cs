using System;

// Token: 0x02000768 RID: 1896
public interface IAuthorizationResponses
{
	// Token: 0x0600383E RID: 14398
	void AuthorizationDenied(IAuthorizer _authorizer, ClientInfo _clientInfo, GameUtils.KickPlayerData _kickPlayerData);

	// Token: 0x0600383F RID: 14399
	void AuthorizationAccepted(IAuthorizer _authorizer, ClientInfo _clientInfo);
}
