using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200078D RID: 1933
[Preserve]
public class CrossplayAuthorizer : AuthorizerAbs
{
	// Token: 0x170005E7 RID: 1511
	// (get) Token: 0x06003994 RID: 14740 RVA: 0x00178DC0 File Offset: 0x00176FC0
	public override int Order
	{
		get
		{
			return 550;
		}
	}

	// Token: 0x170005E8 RID: 1512
	// (get) Token: 0x06003995 RID: 14741 RVA: 0x00178DC7 File Offset: 0x00176FC7
	public override string AuthorizerName
	{
		get
		{
			return "Crossplay";
		}
	}

	// Token: 0x170005E9 RID: 1513
	// (get) Token: 0x06003996 RID: 14742 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x06003997 RID: 14743 RVA: 0x00178DD0 File Offset: 0x00176FD0
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		bool @bool = GamePrefs.GetBool(EnumGamePrefs.ServerAllowCrossplay);
		IUserClient user = PlatformManager.MultiPlatform.User;
		bool flag = user == null || user.Permissions.HasCrossplay();
		if (@bool && flag)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
		if (_clientInfo.device.ToPlayGroup() != DeviceFlag.StandaloneWindows.ToPlayGroup())
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.CrossplayDisabled, 0, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
