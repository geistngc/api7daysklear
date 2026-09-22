using System;

namespace Platform
{
	// Token: 0x02001B6E RID: 7022
	public interface IMasterServerAnnouncer
	{
		// Token: 0x0600D224 RID: 53796
		void Init(IPlatform _owner);

		// Token: 0x0600D225 RID: 53797
		void Update();

		// Token: 0x170019DD RID: 6621
		// (get) Token: 0x0600D226 RID: 53798
		bool GameServerInitialized { get; }

		// Token: 0x0600D227 RID: 53799
		string GetServerPorts();

		// Token: 0x0600D228 RID: 53800
		void AdvertiseServer(Action _onServerRegistered);

		// Token: 0x0600D229 RID: 53801
		void StopServer();
	}
}
