using System;
using UnityEngine.Scripting;

// Token: 0x02000790 RID: 1936
[Preserve]
public class AuthFinalizer : AuthorizerAbs
{
	// Token: 0x060039AD RID: 14765 RVA: 0x00179062 File Offset: 0x00177262
	public AuthFinalizer()
	{
		AuthFinalizer.Instance = this;
	}

	// Token: 0x170005F1 RID: 1521
	// (get) Token: 0x060039AE RID: 14766 RVA: 0x00179070 File Offset: 0x00177270
	public override int Order
	{
		get
		{
			return 999;
		}
	}

	// Token: 0x170005F2 RID: 1522
	// (get) Token: 0x060039AF RID: 14767 RVA: 0x00179077 File Offset: 0x00177277
	public override string AuthorizerName
	{
		get
		{
			return "Finalizer";
		}
	}

	// Token: 0x170005F3 RID: 1523
	// (get) Token: 0x060039B0 RID: 14768 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060039B1 RID: 14769 RVA: 0x00179080 File Offset: 0x00177280
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		_clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageAuthConfirmation>().Setup());
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.WaitAsync, null);
	}

	// Token: 0x060039B2 RID: 14770 RVA: 0x00178950 File Offset: 0x00176B50
	public void ReplyReceived(ClientInfo _cInfo)
	{
		this.authResponsesHandler.AuthorizationAccepted(this, _cInfo);
	}

	// Token: 0x04002F5E RID: 12126
	public static AuthFinalizer Instance;
}
