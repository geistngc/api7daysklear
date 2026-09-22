using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200078B RID: 1931
[Preserve]
public class CrossplatformAuthorizer : AuthorizerAbs
{
	// Token: 0x170005DF RID: 1503
	// (get) Token: 0x06003983 RID: 14723 RVA: 0x00178A5F File Offset: 0x00176C5F
	public override int Order
	{
		get
		{
			return 490;
		}
	}

	// Token: 0x170005E0 RID: 1504
	// (get) Token: 0x06003984 RID: 14724 RVA: 0x00178A66 File Offset: 0x00176C66
	public override string AuthorizerName
	{
		get
		{
			return "CrossplatformAuth";
		}
	}

	// Token: 0x170005E1 RID: 1505
	// (get) Token: 0x06003985 RID: 14725 RVA: 0x00178A6D File Offset: 0x00176C6D
	public override string StateLocalizationKey
	{
		get
		{
			return "authstate_crossplatform";
		}
	}

	// Token: 0x170005E2 RID: 1506
	// (get) Token: 0x06003986 RID: 14726 RVA: 0x00178A74 File Offset: 0x00176C74
	public override bool AuthorizerActive
	{
		get
		{
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			return ((crossplatformPlatform != null) ? crossplatformPlatform.AuthenticationServer : null) != null;
		}
	}

	// Token: 0x06003987 RID: 14727 RVA: 0x00178A8C File Offset: 0x00176C8C
	public override void ServerStart()
	{
		base.ServerStart();
		foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.ServerPlatforms)
		{
			if (keyValuePair.Value.IsCrossplatform)
			{
				IAuthenticationServer authenticationServer = keyValuePair.Value.AuthenticationServer;
				if (authenticationServer != null)
				{
					authenticationServer.StartServer(new AuthenticationSuccessfulCallbackDelegate(this.authPlayerSteamSuccessfulCallback), new KickPlayerDelegate(this.kickPlayerCallback));
				}
			}
		}
	}

	// Token: 0x06003988 RID: 14728 RVA: 0x00178B14 File Offset: 0x00176D14
	public override void ServerStop()
	{
		base.ServerStop();
		foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.ServerPlatforms)
		{
			if (keyValuePair.Value.IsCrossplatform)
			{
				IAuthenticationServer authenticationServer = keyValuePair.Value.AuthenticationServer;
				if (authenticationServer != null)
				{
					authenticationServer.StopServer();
				}
			}
		}
	}

	// Token: 0x06003989 RID: 14729 RVA: 0x00178B84 File Offset: 0x00176D84
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		if (_clientInfo.CrossplatformId == null)
		{
			EAuthorizerSyncResult item = EAuthorizerSyncResult.SyncDeny;
			GameUtils.EKickReason kickReason = GameUtils.EKickReason.WrongCrossPlatform;
			int apiResponseEnum = 0;
			string customReason = EPlatformIdentifier.None.ToStringCached<EPlatformIdentifier>();
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customReason)));
		}
		EPlatformIdentifier platformIdentifier = _clientInfo.CrossplatformId.PlatformIdentifier;
		IPlatform platform = PlatformManager.InstanceForPlatformIdentifier(platformIdentifier);
		if (platform == null)
		{
			EAuthorizerSyncResult item2 = EAuthorizerSyncResult.SyncDeny;
			GameUtils.EKickReason kickReason2 = GameUtils.EKickReason.UnsupportedPlatform;
			int apiResponseEnum2 = 0;
			string customReason = platformIdentifier.ToStringCached<EPlatformIdentifier>();
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item2, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason2, apiResponseEnum2, default(DateTime), customReason)));
		}
		if (platform.PlatformIdentifier != PlatformManager.CrossplatformPlatform.PlatformIdentifier)
		{
			EAuthorizerSyncResult item3 = EAuthorizerSyncResult.SyncDeny;
			GameUtils.EKickReason kickReason3 = GameUtils.EKickReason.WrongCrossPlatform;
			int apiResponseEnum3 = 0;
			string customReason = platformIdentifier.ToStringCached<EPlatformIdentifier>();
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item3, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason3, apiResponseEnum3, default(DateTime), customReason)));
		}
		if (platform.AuthenticationServer == null)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
		EBeginUserAuthenticationResult ebeginUserAuthenticationResult = platform.AuthenticationServer.AuthenticateUser(_clientInfo);
		if (ebeginUserAuthenticationResult != EBeginUserAuthenticationResult.Ok)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.CrossPlatformAuthenticationBeginFailed, (int)ebeginUserAuthenticationResult, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.WaitAsync, null);
	}

	// Token: 0x0600398A RID: 14730 RVA: 0x00178950 File Offset: 0x00176B50
	[PublicizedFrom(EAccessModifier.Private)]
	public void authPlayerSteamSuccessfulCallback(ClientInfo _clientInfo)
	{
		this.authResponsesHandler.AuthorizationAccepted(this, _clientInfo);
	}

	// Token: 0x0600398B RID: 14731 RVA: 0x0017895F File Offset: 0x00176B5F
	[PublicizedFrom(EAccessModifier.Private)]
	public void kickPlayerCallback(ClientInfo _cInfo, GameUtils.KickPlayerData _kickData)
	{
		this.authResponsesHandler.AuthorizationDenied(this, _cInfo, _kickData);
	}

	// Token: 0x0600398C RID: 14732 RVA: 0x00178C8F File Offset: 0x00176E8F
	public override void Disconnect(ClientInfo _clientInfo)
	{
		if (_clientInfo != null)
		{
			PlatformUserIdentifierAbs crossplatformId = _clientInfo.CrossplatformId;
			if (((crossplatformId != null) ? crossplatformId.ReadablePlatformUserIdentifier : null) != null)
			{
				IPlatform platform = PlatformManager.InstanceForPlatformIdentifier(_clientInfo.CrossplatformId.PlatformIdentifier);
				if (platform == null)
				{
					return;
				}
				IAuthenticationServer authenticationServer = platform.AuthenticationServer;
				if (authenticationServer == null)
				{
					return;
				}
				authenticationServer.RemoveUser(_clientInfo);
			}
		}
	}
}
