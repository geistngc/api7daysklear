using System;
using System.IO;

namespace Platform.Shared
{
	// Token: 0x02001CB2 RID: 7346
	public class SaveGameProvider : SaveGameIOProviderFixedRoot, IPlatformSaveGameProvider, IPlatformSaveGameIOProvider
	{
		// Token: 0x0600D9E3 RID: 55779 RVA: 0x004E550C File Offset: 0x004E370C
		public SaveGameProvider() : base(null)
		{
		}

		// Token: 0x0600D9E4 RID: 55780 RVA: 0x004E5515 File Offset: 0x004E3715
		public SaveGameProvider(string rootPath, long maxStorageSizeBytes = 0L) : base(rootPath)
		{
			this.m_maxStorageSizeBytes = maxStorageSizeBytes;
			this.UpdateSizes();
		}

		// Token: 0x17001B0F RID: 6927
		// (get) Token: 0x0600D9E5 RID: 55781 RVA: 0x0002003D File Offset: 0x0001E23D
		public ESaveGameProviderStatus Status
		{
			get
			{
				return ESaveGameProviderStatus.Ok;
			}
		}

		// Token: 0x17001B10 RID: 6928
		// (get) Token: 0x0600D9E6 RID: 55782 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public IPlatformSaveGameIOProvider Cache
		{
			get
			{
				return null;
			}
		}

		// Token: 0x14000139 RID: 313
		// (add) Token: 0x0600D9E7 RID: 55783 RVA: 0x004E552B File Offset: 0x004E372B
		// (remove) Token: 0x0600D9E8 RID: 55784 RVA: 0x000027FC File Offset: 0x000009FC
		public event Action Initialized
		{
			add
			{
				value();
			}
			remove
			{
			}
		}

		// Token: 0x0600D9E9 RID: 55785 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600D9EA RID: 55786 RVA: 0x000027FC File Offset: 0x000009FC
		public void Destroy()
		{
		}

		// Token: 0x0600D9EB RID: 55787 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool ShouldBackup()
		{
			return true;
		}

		// Token: 0x0600D9EC RID: 55788 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool ShouldCommit()
		{
			return true;
		}

		// Token: 0x0600D9ED RID: 55789 RVA: 0x004CFD45 File Offset: 0x004CDF45
		public double GetCommitProgress()
		{
			return 1.0;
		}

		// Token: 0x0600D9EE RID: 55790 RVA: 0x000027FC File Offset: 0x000009FC
		public void Flush(bool waitForFlush)
		{
		}

		// Token: 0x0600D9EF RID: 55791 RVA: 0x004E5533 File Offset: 0x004E3733
		public bool ShouldLimitSize()
		{
			return !string.IsNullOrEmpty(this.m_rootPath) && this.m_maxStorageSizeBytes > 0L;
		}

		// Token: 0x0600D9F0 RID: 55792 RVA: 0x004E5550 File Offset: 0x004E3750
		public void UpdateSizes()
		{
			if (this.ShouldLimitSize())
			{
				long num = 0L;
				foreach (string fileName in Directory.EnumerateFiles(this.m_rootPath, "*", SearchOption.AllDirectories))
				{
					FileInfo fileInfo = new FileInfo(fileName);
					num += fileInfo.Length;
				}
				this.m_currentSize = new SaveDataSizes(this.m_maxStorageSizeBytes, this.m_maxStorageSizeBytes - num);
			}
		}

		// Token: 0x0600D9F1 RID: 55793 RVA: 0x004E55D4 File Offset: 0x004E37D4
		public SaveDataSizes GetSizes()
		{
			return this.m_currentSize;
		}

		// Token: 0x0400A57B RID: 42363
		[PublicizedFrom(EAccessModifier.Private)]
		public long m_maxStorageSizeBytes;

		// Token: 0x0400A57C RID: 42364
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveDataSizes m_currentSize;
	}
}
