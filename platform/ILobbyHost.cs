using System;

namespace Platform
{
	// Token: 0x02001B6D RID: 7021
	public interface ILobbyHost
	{
		// Token: 0x170019DA RID: 6618
		// (get) Token: 0x0600D21B RID: 53787
		string LobbyId { get; }

		// Token: 0x170019DB RID: 6619
		// (get) Token: 0x0600D21C RID: 53788
		bool IsInLobby { get; }

		// Token: 0x170019DC RID: 6620
		// (get) Token: 0x0600D21D RID: 53789 RVA: 0x00010E62 File Offset: 0x0000F062
		bool AllowClientLobby
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600D21E RID: 53790
		void Init(IPlatform _owner);

		// Token: 0x0600D21F RID: 53791
		void UpdateLobby(GameServerInfo _gameServerInfo);

		// Token: 0x0600D220 RID: 53792
		void JoinLobby(string _lobbyId, Action<LobbyHostJoinResult> _onComplete);

		// Token: 0x0600D221 RID: 53793
		void ExitLobby();

		// Token: 0x0600D222 RID: 53794
		void UpdateGameTimePlayers(ulong _time, int _players);

		// Token: 0x0600D223 RID: 53795 RVA: 0x004C9F5C File Offset: 0x004C815C
		[PublicizedFrom(EAccessModifier.Protected)]
		public static void NotifyJoinedSession(string sessionId, bool overwriteHostLobby)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				PlatformLobbyId lobbyId = new PlatformLobbyId(PlatformManager.NativePlatform.PlatformIdentifier, sessionId);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageLobbyRegisterClient>().Setup(lobbyId, overwriteHostLobby), false);
			}
		}
	}
}
