using System;

namespace Platform
{
	// Token: 0x02001B60 RID: 7008
	public sealed class ClientAuthenticateServerContext
	{
		// Token: 0x0600D1EA RID: 53738 RVA: 0x004C9E98 File Offset: 0x004C8098
		public ClientAuthenticateServerContext(GameServerInfo _gameServerInfo, PlatformUserIdentifierAbs _platformUserId, PlatformUserIdentifierAbs _crossplatformUserId, ClientAuthenticateServerSuccessDelegate _success, ClientAuthenticateServerDisconnectDelegate _disconnect)
		{
			this.GameServerInfo = _gameServerInfo;
			this.PlatformUserId = _platformUserId;
			this.CrossplatformUserId = _crossplatformUserId;
			this.success = _success;
			this.disconnect = _disconnect;
		}

		// Token: 0x170019D7 RID: 6615
		// (get) Token: 0x0600D1EB RID: 53739 RVA: 0x004C9EC5 File Offset: 0x004C80C5
		public GameServerInfo GameServerInfo { get; }

		// Token: 0x170019D8 RID: 6616
		// (get) Token: 0x0600D1EC RID: 53740 RVA: 0x004C9ECD File Offset: 0x004C80CD
		public PlatformUserIdentifierAbs PlatformUserId { get; }

		// Token: 0x170019D9 RID: 6617
		// (get) Token: 0x0600D1ED RID: 53741 RVA: 0x004C9ED5 File Offset: 0x004C80D5
		public PlatformUserIdentifierAbs CrossplatformUserId { get; }

		// Token: 0x0600D1EE RID: 53742 RVA: 0x004C9EDD File Offset: 0x004C80DD
		public void Success()
		{
			ClientAuthenticateServerSuccessDelegate clientAuthenticateServerSuccessDelegate = this.success;
			if (clientAuthenticateServerSuccessDelegate == null)
			{
				return;
			}
			clientAuthenticateServerSuccessDelegate();
		}

		// Token: 0x0600D1EF RID: 53743 RVA: 0x004C9EF0 File Offset: 0x004C80F0
		public void DisconnectNoCrossplay()
		{
			string reason = PermissionsManager.GetPermissionDenyReason(EUserPerms.Crossplay, PermissionsManager.PermissionSources.All) ?? Localization.Get("auth_noCrossplay", false, null);
			ClientAuthenticateServerDisconnectDelegate clientAuthenticateServerDisconnectDelegate = this.disconnect;
			if (clientAuthenticateServerDisconnectDelegate == null)
			{
				return;
			}
			clientAuthenticateServerDisconnectDelegate(reason);
		}

		// Token: 0x0600D1F0 RID: 53744 RVA: 0x004C9F27 File Offset: 0x004C8127
		public void DisconnectNoCrossplay(EPlayGroup otherPlayGroup)
		{
			ClientAuthenticateServerDisconnectDelegate clientAuthenticateServerDisconnectDelegate = this.disconnect;
			if (clientAuthenticateServerDisconnectDelegate == null)
			{
				return;
			}
			clientAuthenticateServerDisconnectDelegate(string.Format(Localization.Get("auth_noCrossplayBetween", false, null), EPlayGroupExtensions.Current, otherPlayGroup));
		}

		// Token: 0x0400A0D4 RID: 41172
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ClientAuthenticateServerSuccessDelegate success;

		// Token: 0x0400A0D5 RID: 41173
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ClientAuthenticateServerDisconnectDelegate disconnect;
	}
}
