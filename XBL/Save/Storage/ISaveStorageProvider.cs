using System;

namespace Platform.XBL.Save.Storage
{
	// Token: 0x02001C51 RID: 7249
	public interface ISaveStorageProvider : IDisposable
	{
		// Token: 0x17001A99 RID: 6809
		// (get) Token: 0x0600D6D9 RID: 55001
		SizeTracker SizeTracker { get; }

		// Token: 0x17001A9A RID: 6810
		// (get) Token: 0x0600D6DA RID: 55002
		ISaveStorageContainer RootSaveStorageContainer { get; }

		// Token: 0x0600D6DB RID: 55003
		void InitializeAsync(IPlatform owner, long maxSizeBytes, SingleThreadTaskScheduler taskScheduler, OnGameSaveProviderStatusChanged statusChanged);

		// Token: 0x0600D6DC RID: 55004
		void Flush(bool waitForFlush);
	}
}
