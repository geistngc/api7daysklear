using System;
using UnityEngine.Scripting;

// Token: 0x0200078C RID: 1932
[Preserve]
public class BansAndWhitelistAuthorizer : AuthorizerAbs
{
	// Token: 0x170005E3 RID: 1507
	// (get) Token: 0x0600398E RID: 14734 RVA: 0x00178CCD File Offset: 0x00176ECD
	public override int Order
	{
		get
		{
			return 500;
		}
	}

	// Token: 0x170005E4 RID: 1508
	// (get) Token: 0x0600398F RID: 14735 RVA: 0x00178CD4 File Offset: 0x00176ED4
	public override string AuthorizerName
	{
		get
		{
			return "BansAndWhitelist";
		}
	}

	// Token: 0x170005E5 RID: 1509
	// (get) Token: 0x06003990 RID: 14736 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x170005E6 RID: 1510
	// (get) Token: 0x06003991 RID: 14737 RVA: 0x00178CDB File Offset: 0x00176EDB
	public override bool AuthorizerActive
	{
		get
		{
			return GameManager.Instance.adminTools != null;
		}
	}

	// Token: 0x06003992 RID: 14738 RVA: 0x00178CEC File Offset: 0x00176EEC
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		AdminTools adminTools = GameManager.Instance.adminTools;
		DateTime banUntil;
		string customReason;
		if (adminTools.Blacklist.IsBanned(_clientInfo.PlatformId, out banUntil, out customReason))
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.Banned, 0, banUntil, customReason)));
		}
		DateTime banUntil2;
		string customReason2;
		if (_clientInfo.CrossplatformId != null && adminTools.Blacklist.IsBanned(_clientInfo.CrossplatformId, out banUntil2, out customReason2))
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.Banned, 0, banUntil2, customReason2)));
		}
		if (adminTools.Whitelist.IsWhiteListEnabled() && !adminTools.Whitelist.IsWhitelisted(_clientInfo) && !adminTools.Users.HasEntry(_clientInfo))
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.NotOnWhitelist, 0, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
