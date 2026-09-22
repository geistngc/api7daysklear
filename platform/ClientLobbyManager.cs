using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001B43 RID: 6979
	public class ClientLobbyManager
	{
		// Token: 0x0600D137 RID: 53559 RVA: 0x004C8633 File Offset: 0x004C6833
		public ClientLobbyManager()
		{
			ConnectionManager.OnClientDisconnected += this.OnClientDisconnected;
		}

		// Token: 0x0600D138 RID: 53560 RVA: 0x004C8664 File Offset: 0x004C6864
		public bool TryGetLobbyId(EPlatformIdentifier platform, out PlatformLobbyId lobbyId)
		{
			object obj = this.lockObj;
			bool result;
			lock (obj)
			{
				ClientLobbyManager.Lobby lobby;
				if (this.lobbies.TryGetValue(platform, out lobby))
				{
					lobbyId = lobby.Id;
					result = true;
				}
				else
				{
					lobbyId = null;
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600D139 RID: 53561 RVA: 0x004C86C0 File Offset: 0x004C68C0
		public void RegisterLobbyClient(PlatformLobbyId platformLobbyId, ClientInfo client, bool overwrite = false)
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.Contains(client))
			{
				Log.Warning(string.Format("[ClientLobbyManager] could not register {0} for client lobby {1} : {2} as they are no longer connected", client.playerName, platformLobbyId.PlatformIdentifier, platformLobbyId.LobbyId));
				return;
			}
			object obj = this.lockObj;
			lock (obj)
			{
				ClientLobbyManager.Lobby lobby;
				if (!this.lobbies.TryGetValue(platformLobbyId.PlatformIdentifier, out lobby))
				{
					Log.Out(string.Format("[ClientLobbyManager] registering new lobby for client platform {0} : {1}", platformLobbyId.PlatformIdentifier, platformLobbyId.LobbyId));
					lobby = new ClientLobbyManager.Lobby(platformLobbyId);
					lobby.AddClient(client);
					this.lobbies.Add(platformLobbyId.PlatformIdentifier, lobby);
				}
				else if (lobby.Id.LobbyId.Equals(platformLobbyId.LobbyId))
				{
					lobby.AddClient(client);
				}
				else if (overwrite)
				{
					Log.Warning(string.Format("[ClientLobbyManager] overwriting existing lobby for {0}", platformLobbyId.PlatformIdentifier));
					ClientLobbyManager.Lobby lobby2 = new ClientLobbyManager.Lobby(platformLobbyId);
					lobby2.AddClient(client);
					foreach (ClientInfo clientInfo in lobby.Clients)
					{
						clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageLobbyJoin>().Setup(platformLobbyId));
						lobby2.AddClient(clientInfo);
					}
					this.lobbies[platformLobbyId.PlatformIdentifier] = lobby2;
				}
				else
				{
					Log.Warning(string.Format("[ClientLobbyManager] a different client lobby already registered for {0}, sending to client", platformLobbyId.PlatformIdentifier));
					client.SendPackage(NetPackageManager.GetPackage<NetPackageLobbyJoin>().Setup(lobby.Id));
				}
			}
		}

		// Token: 0x0600D13A RID: 53562 RVA: 0x004C8890 File Offset: 0x004C6A90
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnClientDisconnected(ClientInfo client)
		{
			object obj = this.lockObj;
			lock (obj)
			{
				ClientLobbyManager.Lobby lobby;
				if (this.lobbies.TryGetValue(client.PlatformId.PlatformIdentifier, out lobby))
				{
					lobby.RemoveClient(client);
					if (lobby.IsEmpty)
					{
						Log.Out(string.Format("[ClientLobbyManager] removing registered lobby {0} : {1}", lobby.Id.PlatformIdentifier, lobby.Id.LobbyId));
						this.lobbies.Remove(client.PlatformId.PlatformIdentifier);
					}
				}
			}
		}

		// Token: 0x0400A067 RID: 41063
		[PublicizedFrom(EAccessModifier.Private)]
		public object lockObj = new object();

		// Token: 0x0400A068 RID: 41064
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<EPlatformIdentifier, ClientLobbyManager.Lobby> lobbies = new Dictionary<EPlatformIdentifier, ClientLobbyManager.Lobby>();

		// Token: 0x02001B44 RID: 6980
		[PublicizedFrom(EAccessModifier.Private)]
		public class Lobby
		{
			// Token: 0x170019AA RID: 6570
			// (get) Token: 0x0600D13B RID: 53563 RVA: 0x004C8934 File Offset: 0x004C6B34
			public PlatformLobbyId Id
			{
				get
				{
					return this.id;
				}
			}

			// Token: 0x170019AB RID: 6571
			// (get) Token: 0x0600D13C RID: 53564 RVA: 0x004C893C File Offset: 0x004C6B3C
			public bool IsEmpty
			{
				get
				{
					return this.clients.Count == 0;
				}
			}

			// Token: 0x170019AC RID: 6572
			// (get) Token: 0x0600D13D RID: 53565 RVA: 0x004C894C File Offset: 0x004C6B4C
			public IReadOnlyList<ClientInfo> Clients
			{
				get
				{
					return this.clients;
				}
			}

			// Token: 0x0600D13E RID: 53566 RVA: 0x004C8954 File Offset: 0x004C6B54
			public Lobby(PlatformLobbyId id)
			{
				this.id = id;
			}

			// Token: 0x0600D13F RID: 53567 RVA: 0x004C896E File Offset: 0x004C6B6E
			public Lobby(EPlatformIdentifier platform, string lobbyId)
			{
				this.id = new PlatformLobbyId(platform, lobbyId);
			}

			// Token: 0x0600D140 RID: 53568 RVA: 0x004C8990 File Offset: 0x004C6B90
			public void AddClient(ClientInfo client)
			{
				this.clients.Add(client);
				Log.Out(string.Format("[ClientLobbyManager] registered member {0} for client lobby {1} : {2}. Total members: {3}", new object[]
				{
					client.playerName,
					this.id.PlatformIdentifier,
					this.id.LobbyId,
					this.clients.Count
				}));
			}

			// Token: 0x0600D141 RID: 53569 RVA: 0x004C89FC File Offset: 0x004C6BFC
			public void RemoveClient(ClientInfo client)
			{
				if (this.clients.Remove(client))
				{
					Log.Out(string.Format("[ClientLobbyManager] removed member {0} from client lobby {1} : {2}. Total members: {3}", new object[]
					{
						client.playerName,
						this.id.PlatformIdentifier,
						this.id.LobbyId,
						this.clients.Count
					}));
					return;
				}
				Log.Warning(string.Format("[ClientLobbyManager] remove member {0} from client lobby {1} : {2} failed. They are not a member", client.playerName, this.id.PlatformIdentifier, this.id.LobbyId));
			}

			// Token: 0x0400A069 RID: 41065
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly PlatformLobbyId id;

			// Token: 0x0400A06A RID: 41066
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly List<ClientInfo> clients = new List<ClientInfo>();
		}
	}
}
