using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200078E RID: 1934
[Preserve]
public class EacAuthorizer : AuthorizerAbs
{
	// Token: 0x170005EA RID: 1514
	// (get) Token: 0x06003999 RID: 14745 RVA: 0x00178E58 File Offset: 0x00177058
	public override int Order
	{
		get
		{
			return 600;
		}
	}

	// Token: 0x170005EB RID: 1515
	// (get) Token: 0x0600399A RID: 14746 RVA: 0x00178E5F File Offset: 0x0017705F
	public override string AuthorizerName
	{
		get
		{
			return "EAC";
		}
	}

	// Token: 0x170005EC RID: 1516
	// (get) Token: 0x0600399B RID: 14747 RVA: 0x00178E66 File Offset: 0x00177066
	public override string StateLocalizationKey
	{
		get
		{
			return "authstate_eac";
		}
	}

	// Token: 0x170005ED RID: 1517
	// (get) Token: 0x0600399C RID: 14748 RVA: 0x00178E6D File Offset: 0x0017706D
	public override bool AuthorizerActive
	{
		get
		{
			IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
			return antiCheatServer != null && antiCheatServer.ServerEacEnabled();
		}
	}

	// Token: 0x0600399D RID: 14749 RVA: 0x00178E84 File Offset: 0x00177084
	public override void ServerStart()
	{
		base.ServerStart();
		IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
		if (antiCheatServer == null)
		{
			return;
		}
		antiCheatServer.StartServer(new AuthenticationSuccessfulCallbackDelegate(this.authPlayerEacSuccessfulCallback), new KickPlayerDelegate(this.kickPlayerCallback));
	}

	// Token: 0x0600399E RID: 14750 RVA: 0x00178EB9 File Offset: 0x001770B9
	public override void ServerStop()
	{
		base.ServerStop();
		IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
		if (antiCheatServer == null)
		{
			return;
		}
		antiCheatServer.StopServer();
	}

	// Token: 0x0600399F RID: 14751 RVA: 0x00178ED8 File Offset: 0x001770D8
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
		if (antiCheatServer != null)
		{
			antiCheatServer.RegisterUser(_clientInfo);
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.WaitAsync, null);
	}

	// Token: 0x060039A0 RID: 14752 RVA: 0x00178F0B File Offset: 0x0017710B
	[PublicizedFrom(EAccessModifier.Private)]
	public void authPlayerEacSuccessfulCallback(ClientInfo _cInfo)
	{
		_cInfo.acAuthDone = true;
		this.authResponsesHandler.AuthorizationAccepted(this, _cInfo);
	}

	// Token: 0x060039A1 RID: 14753 RVA: 0x0017895F File Offset: 0x00176B5F
	[PublicizedFrom(EAccessModifier.Private)]
	public void kickPlayerCallback(ClientInfo _cInfo, GameUtils.KickPlayerData _kickData)
	{
		this.authResponsesHandler.AuthorizationDenied(this, _cInfo, _kickData);
	}

	// Token: 0x060039A2 RID: 14754 RVA: 0x00178F21 File Offset: 0x00177121
	public override void Disconnect(ClientInfo _clientInfo)
	{
		IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
		if (antiCheatServer == null)
		{
			return;
		}
		antiCheatServer.FreeUser(_clientInfo);
	}
}
