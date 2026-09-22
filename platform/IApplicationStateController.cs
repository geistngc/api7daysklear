using System;

namespace Platform
{
	// Token: 0x02001B5C RID: 7004
	public interface IApplicationStateController
	{
		// Token: 0x1400011C RID: 284
		// (add) Token: 0x0600D1D5 RID: 53717
		// (remove) Token: 0x0600D1D6 RID: 53718
		event IApplicationStateController.ApplicationStateChanged OnApplicationStateChanged;

		// Token: 0x1400011D RID: 285
		// (add) Token: 0x0600D1D7 RID: 53719
		// (remove) Token: 0x0600D1D8 RID: 53720
		event IApplicationStateController.NetworkStateChanged OnNetworkStateChanged;

		// Token: 0x170019D5 RID: 6613
		// (get) Token: 0x0600D1D9 RID: 53721
		bool NetworkConnectionState { get; }

		// Token: 0x170019D6 RID: 6614
		// (get) Token: 0x0600D1DA RID: 53722
		ApplicationState CurrentApplicationState { get; }

		// Token: 0x0600D1DB RID: 53723
		void Init(IPlatform owner);

		// Token: 0x0600D1DC RID: 53724
		void Destroy();

		// Token: 0x0600D1DD RID: 53725
		void Update();

		// Token: 0x02001B5D RID: 7005
		// (Invoke) Token: 0x0600D1DF RID: 53727
		public delegate void ApplicationStateChanged(ApplicationState newState);

		// Token: 0x02001B5E RID: 7006
		// (Invoke) Token: 0x0600D1E3 RID: 53731
		public delegate void NetworkStateChanged(bool connectionState);
	}
}
