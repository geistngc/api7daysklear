using System;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001C9A RID: 7322
	public class MultiplayerInvitationDialogSteam : IMultiplayerInvitationDialog
	{
		// Token: 0x17001AF1 RID: 6897
		// (get) Token: 0x0600D91D RID: 55581 RVA: 0x004E24AB File Offset: 0x004E06AB
		public bool CanShow
		{
			get
			{
				return this.lobbyHost != null && this.lobbyHost.IsInLobby;
			}
		}

		// Token: 0x0600D91E RID: 55582 RVA: 0x004E24C2 File Offset: 0x004E06C2
		public void Init(IPlatform owner)
		{
			this.lobbyHost = (LobbyHost)owner.LobbyHost;
		}

		// Token: 0x0600D91F RID: 55583 RVA: 0x004E24D8 File Offset: 0x004E06D8
		public void ShowInviteDialog()
		{
			if (this.lobbyHost == null)
			{
				Log.Error("[Steam] Cannot open invite dialog, lobby host is null");
				return;
			}
			string lobbyId = this.lobbyHost.LobbyId;
			if (string.IsNullOrEmpty(lobbyId))
			{
				Log.Error("[Steam] Cannot open invite dialog, no lobby id set");
				return;
			}
			ulong num;
			if (StringParsers.TryParseUInt64(lobbyId, out num))
			{
				Log.Out(string.Format("[Steam] Opening invite dialog for lobby: {0}", num));
				SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(num));
				return;
			}
			Log.Error("[Steam] Cannot open invite dialog, could not parse Steam lobby id: " + lobbyId);
		}

		// Token: 0x0400A501 RID: 42241
		[PublicizedFrom(EAccessModifier.Private)]
		public LobbyHost lobbyHost;
	}
}
