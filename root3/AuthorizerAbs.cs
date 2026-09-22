using System;
using Platform;

// Token: 0x0200077F RID: 1919
public abstract class AuthorizerAbs : IAuthorizer
{
	// Token: 0x170005B8 RID: 1464
	// (get) Token: 0x0600393A RID: 14650
	public abstract int Order { get; }

	// Token: 0x170005B9 RID: 1465
	// (get) Token: 0x0600393B RID: 14651
	public abstract string AuthorizerName { get; }

	// Token: 0x170005BA RID: 1466
	// (get) Token: 0x0600393C RID: 14652
	public abstract string StateLocalizationKey { get; }

	// Token: 0x170005BB RID: 1467
	// (get) Token: 0x0600393D RID: 14653 RVA: 0x00081531 File Offset: 0x0007F731
	public virtual EPlatformIdentifier PlatformRestriction
	{
		get
		{
			return EPlatformIdentifier.Count;
		}
	}

	// Token: 0x170005BC RID: 1468
	// (get) Token: 0x0600393E RID: 14654 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool AuthorizerActive
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600393F RID: 14655 RVA: 0x0017821B File Offset: 0x0017641B
	public virtual void Init(IAuthorizationResponses _authResponsesHandler)
	{
		this.authResponsesHandler = _authResponsesHandler;
	}

	// Token: 0x06003940 RID: 14656 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Cleanup()
	{
	}

	// Token: 0x06003941 RID: 14657 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void ServerStart()
	{
	}

	// Token: 0x06003942 RID: 14658 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void ServerStop()
	{
	}

	// Token: 0x06003943 RID: 14659
	public abstract ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo);

	// Token: 0x06003944 RID: 14660 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Disconnect(ClientInfo _clientInfo)
	{
	}

	// Token: 0x06003945 RID: 14661 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Protected)]
	public AuthorizerAbs()
	{
	}

	// Token: 0x04002F5D RID: 12125
	[PublicizedFrom(EAccessModifier.Protected)]
	public IAuthorizationResponses authResponsesHandler;
}
