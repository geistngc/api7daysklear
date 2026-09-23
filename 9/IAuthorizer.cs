using System;
using Platform;

// Token: 0x0200077E RID: 1918
public interface IAuthorizer
{
	// Token: 0x170005B3 RID: 1459
	// (get) Token: 0x0600392F RID: 14639
	int Order { get; }

	// Token: 0x170005B4 RID: 1460
	// (get) Token: 0x06003930 RID: 14640
	string AuthorizerName { get; }

	// Token: 0x170005B5 RID: 1461
	// (get) Token: 0x06003931 RID: 14641
	string StateLocalizationKey { get; }

	// Token: 0x170005B6 RID: 1462
	// (get) Token: 0x06003932 RID: 14642
	EPlatformIdentifier PlatformRestriction { get; }

	// Token: 0x170005B7 RID: 1463
	// (get) Token: 0x06003933 RID: 14643
	bool AuthorizerActive { get; }

	// Token: 0x06003934 RID: 14644
	void Init(IAuthorizationResponses _authResponsesHandler);

	// Token: 0x06003935 RID: 14645
	void Cleanup();

	// Token: 0x06003936 RID: 14646
	void ServerStart();

	// Token: 0x06003937 RID: 14647
	void ServerStop();

	// Token: 0x06003938 RID: 14648
	ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo);

	// Token: 0x06003939 RID: 14649
	void Disconnect(ClientInfo _clientInfo);
}
