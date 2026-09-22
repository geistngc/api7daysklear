using System;

namespace Platform
{
	// Token: 0x02001B78 RID: 7032
	public interface IPlatformApi
	{
		// Token: 0x17001A0C RID: 6668
		// (get) Token: 0x0600D281 RID: 53889
		EApiStatus ClientApiStatus { get; }

		// Token: 0x14000122 RID: 290
		// (add) Token: 0x0600D282 RID: 53890
		// (remove) Token: 0x0600D283 RID: 53891
		event Action ClientApiInitialized;

		// Token: 0x0600D284 RID: 53892
		void Init(IPlatform _owner);

		// Token: 0x0600D285 RID: 53893
		bool InitClientApis();

		// Token: 0x0600D286 RID: 53894
		bool InitServerApis();

		// Token: 0x0600D287 RID: 53895
		void ServerApiLoaded();

		// Token: 0x0600D288 RID: 53896
		void Update();

		// Token: 0x0600D289 RID: 53897
		void Destroy();

		// Token: 0x0600D28A RID: 53898
		float GetScreenBoundsValueFromSystem();
	}
}
