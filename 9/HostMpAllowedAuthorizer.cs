using System;
using UnityEngine.Scripting;

// Token: 0x02000782 RID: 1922
[Preserve]
public class HostMpAllowedAuthorizer : AuthorizerAbs
{
	// Token: 0x170005C3 RID: 1475
	// (get) Token: 0x06003950 RID: 14672 RVA: 0x0017831A File Offset: 0x0017651A
	public override int Order
	{
		get
		{
			return 41;
		}
	}

	// Token: 0x170005C4 RID: 1476
	// (get) Token: 0x06003951 RID: 14673 RVA: 0x0017831E File Offset: 0x0017651E
	public override string AuthorizerName
	{
		get
		{
			return "MpHostAllowed";
		}
	}

	// Token: 0x170005C5 RID: 1477
	// (get) Token: 0x06003952 RID: 14674 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x06003953 RID: 14675 RVA: 0x00178328 File Offset: 0x00176528
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		if (!PermissionsManager.IsMultiplayerAllowed() || !PermissionsManager.CanHostMultiplayer())
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.MultiplayerBlockedForHostAccount, 0, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
