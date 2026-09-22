using System;
using System.Collections.ObjectModel;
using UnityEngine.Scripting;

// Token: 0x02000784 RID: 1924
[Preserve]
public class DuplicateUserIdAuthorizer : AuthorizerAbs
{
	// Token: 0x170005C9 RID: 1481
	// (get) Token: 0x0600395A RID: 14682 RVA: 0x001783C5 File Offset: 0x001765C5
	public override int Order
	{
		get
		{
			return 60;
		}
	}

	// Token: 0x170005CA RID: 1482
	// (get) Token: 0x0600395B RID: 14683 RVA: 0x001783C9 File Offset: 0x001765C9
	public override string AuthorizerName
	{
		get
		{
			return "DuplicateUserId";
		}
	}

	// Token: 0x170005CB RID: 1483
	// (get) Token: 0x0600395C RID: 14684 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x0600395D RID: 14685 RVA: 0x001783D0 File Offset: 0x001765D0
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		ReadOnlyCollection<ClientInfo> list = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List;
		for (int i = 0; i < list.Count; i++)
		{
			ClientInfo clientInfo = list[i];
			if (clientInfo != _clientInfo)
			{
				if (!_clientInfo.PlatformId.Equals(clientInfo.PlatformId))
				{
					PlatformUserIdentifierAbs crossplatformId = _clientInfo.CrossplatformId;
					if (crossplatformId == null || !crossplatformId.Equals(clientInfo.CrossplatformId))
					{
						goto IL_89;
					}
				}
				GameUtils.KickPlayerForClientInfo(clientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.DuplicatePlayerID, 0, default(DateTime), ""));
				return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.DuplicatePlayerID, 0, default(DateTime), "")));
			}
			IL_89:;
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
