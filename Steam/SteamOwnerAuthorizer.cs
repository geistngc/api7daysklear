using System;
using UnityEngine.Scripting;

namespace Platform.Steam
{
	// Token: 0x02001C87 RID: 7303
	[Preserve]
	public class SteamOwnerAuthorizer : AuthorizerAbs
	{
		// Token: 0x17001AD9 RID: 6873
		// (get) Token: 0x0600D885 RID: 55429 RVA: 0x004E0195 File Offset: 0x004DE395
		public override int Order
		{
			get
			{
				return 430;
			}
		}

		// Token: 0x17001ADA RID: 6874
		// (get) Token: 0x0600D886 RID: 55430 RVA: 0x004E019C File Offset: 0x004DE39C
		public override string AuthorizerName
		{
			get
			{
				return "SteamFamily";
			}
		}

		// Token: 0x17001ADB RID: 6875
		// (get) Token: 0x0600D887 RID: 55431 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public override string StateLocalizationKey
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001ADC RID: 6876
		// (get) Token: 0x0600D888 RID: 55432 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override EPlatformIdentifier PlatformRestriction
		{
			get
			{
				return EPlatformIdentifier.Steam;
			}
		}

		// Token: 0x0600D889 RID: 55433 RVA: 0x004E01A4 File Offset: 0x004DE3A4
		public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
		{
			UserIdentifierSteam userIdentifierSteam = (UserIdentifierSteam)_clientInfo.PlatformId;
			UserIdentifierSteam ownerId = userIdentifierSteam.OwnerId;
			DateTime banUntil;
			string customReason;
			if (GameManager.Instance.adminTools != null && ownerId != null && !userIdentifierSteam.Equals(ownerId) && GameManager.Instance.adminTools.Blacklist.IsBanned(ownerId, out banUntil, out customReason))
			{
				return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.Banned, 0, banUntil, customReason)));
			}
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
	}
}
