using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Scripting;

namespace Platform.Steam
{
	// Token: 0x02001C86 RID: 7302
	[Preserve]
	public class SteamGroupsAuthorizer : AuthorizerAbs
	{
		// Token: 0x17001AD4 RID: 6868
		// (get) Token: 0x0600D87C RID: 55420 RVA: 0x004DFFC1 File Offset: 0x004DE1C1
		public override int Order
		{
			get
			{
				return 470;
			}
		}

		// Token: 0x17001AD5 RID: 6869
		// (get) Token: 0x0600D87D RID: 55421 RVA: 0x004DFFC8 File Offset: 0x004DE1C8
		public override string AuthorizerName
		{
			get
			{
				return "SteamGroups";
			}
		}

		// Token: 0x17001AD6 RID: 6870
		// (get) Token: 0x0600D87E RID: 55422 RVA: 0x004DFFCF File Offset: 0x004DE1CF
		public override string StateLocalizationKey
		{
			get
			{
				return "authstate_steamgroups";
			}
		}

		// Token: 0x17001AD7 RID: 6871
		// (get) Token: 0x0600D87F RID: 55423 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override EPlatformIdentifier PlatformRestriction
		{
			get
			{
				return EPlatformIdentifier.Steam;
			}
		}

		// Token: 0x17001AD8 RID: 6872
		// (get) Token: 0x0600D880 RID: 55424 RVA: 0x004DFFD6 File Offset: 0x004DE1D6
		public override bool AuthorizerActive
		{
			get
			{
				return GameManager.Instance.adminTools != null && PlatformManager.InstanceForPlatformIdentifier(EPlatformIdentifier.Steam) != null;
			}
		}

		// Token: 0x0600D881 RID: 55425 RVA: 0x004DFFEF File Offset: 0x004DE1EF
		public override void ServerStart()
		{
			base.ServerStart();
			IAuthenticationServer authenticationServer = PlatformManager.NativePlatform.AuthenticationServer;
			if (authenticationServer == null)
			{
				return;
			}
			authenticationServer.StartServerSteamGroups(new SteamGroupStatusResponse(this.groupStatusCallback));
		}

		// Token: 0x0600D882 RID: 55426 RVA: 0x004E0018 File Offset: 0x004DE218
		public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
		{
			Dictionary<string, AdminWhitelist.WhitelistGroup> groups = GameManager.Instance.adminTools.Whitelist.GetGroups();
			Dictionary<string, AdminUsers.GroupPermission> groups2 = GameManager.Instance.adminTools.Users.GetGroups();
			if (groups.Count == 0 && groups2.Count == 0)
			{
				return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
			}
			EPlatformIdentifier platformIdentifier = _clientInfo.PlatformId.PlatformIdentifier;
			IPlatform platform = PlatformManager.InstanceForPlatformIdentifier(platformIdentifier);
			if (platform == null)
			{
				EAuthorizerSyncResult item = EAuthorizerSyncResult.SyncDeny;
				GameUtils.EKickReason kickReason = GameUtils.EKickReason.UnsupportedPlatform;
				int apiResponseEnum = 0;
				string customReason = platformIdentifier.ToStringCached<EPlatformIdentifier>();
				return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customReason)));
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			groups.CopyKeysTo(hashSet);
			groups2.CopyKeysTo(hashSet);
			_clientInfo.groupMembershipsWaiting = hashSet.Count;
			foreach (string steamIdGroup in hashSet)
			{
				if (!platform.AuthenticationServer.RequestUserInGroupStatus(_clientInfo, steamIdGroup))
				{
					Interlocked.Decrement(ref _clientInfo.groupMembershipsWaiting);
				}
			}
			if (_clientInfo.groupMembershipsWaiting == 0)
			{
				return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
			}
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.WaitAsync, null);
		}

		// Token: 0x0600D883 RID: 55427 RVA: 0x004E015C File Offset: 0x004DE35C
		[PublicizedFrom(EAccessModifier.Private)]
		public void groupStatusCallback(ClientInfo _clientInfo, ulong _groupId, bool _member, bool _officer)
		{
			bool flag = Interlocked.Decrement(ref _clientInfo.groupMembershipsWaiting) != 0;
			if (_member)
			{
				_clientInfo.groupMemberships[_groupId.ToString()] = (_officer ? 2 : 1);
			}
			if (!flag)
			{
				this.authResponsesHandler.AuthorizationAccepted(this, _clientInfo);
			}
		}
	}
}
