using System;

namespace Platform
{
	// Token: 0x02001B8E RID: 7054
	public interface IPlatformSaveGameProvider : IPlatformSaveGameIOProvider
	{
		// Token: 0x17001A14 RID: 6676
		// (get) Token: 0x0600D2E7 RID: 53991
		ESaveGameProviderStatus Status { get; }

		// Token: 0x17001A15 RID: 6677
		// (get) Token: 0x0600D2E8 RID: 53992
		IPlatformSaveGameIOProvider Cache { get; }

		// Token: 0x14000125 RID: 293
		// (add) Token: 0x0600D2E9 RID: 53993
		// (remove) Token: 0x0600D2EA RID: 53994
		event Action Initialized;

		// Token: 0x0600D2EB RID: 53995
		void Init(IPlatform _owner);

		// Token: 0x0600D2EC RID: 53996
		void Destroy();

		// Token: 0x0600D2ED RID: 53997
		bool ShouldBackup();

		// Token: 0x0600D2EE RID: 53998
		bool ShouldCommit();

		// Token: 0x0600D2EF RID: 53999
		double GetCommitProgress();

		// Token: 0x0600D2F0 RID: 54000
		void Flush(bool waitForFlush);

		// Token: 0x0600D2F1 RID: 54001
		bool ShouldLimitSize();

		// Token: 0x0600D2F2 RID: 54002
		void UpdateSizes();

		// Token: 0x0600D2F3 RID: 54003
		SaveDataSizes GetSizes();
	}
}
