using System;

namespace Platform.XBL
{
	// Token: 0x02001C34 RID: 7220
	public class ApplicationStateController : IApplicationStateController
	{
		// Token: 0x14000133 RID: 307
		// (add) Token: 0x0600D630 RID: 54832 RVA: 0x004D4730 File Offset: 0x004D2930
		// (remove) Token: 0x0600D631 RID: 54833 RVA: 0x004D4768 File Offset: 0x004D2968
		public event IApplicationStateController.ApplicationStateChanged OnApplicationStateChanged;

		// Token: 0x14000134 RID: 308
		// (add) Token: 0x0600D632 RID: 54834 RVA: 0x004D47A0 File Offset: 0x004D29A0
		// (remove) Token: 0x0600D633 RID: 54835 RVA: 0x004D47D8 File Offset: 0x004D29D8
		public event IApplicationStateController.NetworkStateChanged OnNetworkStateChanged;

		// Token: 0x17001A91 RID: 6801
		// (get) Token: 0x0600D634 RID: 54836 RVA: 0x00010E62 File Offset: 0x0000F062
		public ApplicationState CurrentApplicationState
		{
			get
			{
				return ApplicationState.Foreground;
			}
		}

		// Token: 0x17001A92 RID: 6802
		// (get) Token: 0x0600D635 RID: 54837 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool NetworkConnectionState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600D636 RID: 54838 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform owner)
		{
		}

		// Token: 0x0600D637 RID: 54839 RVA: 0x000027FC File Offset: 0x000009FC
		public void Destroy()
		{
		}

		// Token: 0x0600D638 RID: 54840 RVA: 0x000027FC File Offset: 0x000009FC
		public void Update()
		{
		}
	}
}
