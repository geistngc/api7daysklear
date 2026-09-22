using System;
using System.Diagnostics;
using System.Threading;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL.Save
{
	// Token: 0x02001C4E RID: 7246
	public sealed class SizeTracker : IDisposable
	{
		// Token: 0x0600D6C3 RID: 54979 RVA: 0x004D8296 File Offset: 0x004D6496
		[Conditional("DEBUG_SAVE_DATA_MANAGER")]
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogTrace(string text)
		{
			Log.Out("[XBL: SizeTracker] " + text);
		}

		// Token: 0x0600D6C4 RID: 54980 RVA: 0x004D82A8 File Offset: 0x004D64A8
		public SizeTracker(long max, long used, SizeTrackerGetRemainingQuotaAsync getRemainingQuotaAsync, bool shouldUpdateSizesOnEstimate)
		{
			this.m_maxBytes = max;
			this.m_getRemainingQuotaAsync = getRemainingQuotaAsync;
			this.m_shouldUpdateSizesOnEstimate = shouldUpdateSizesOnEstimate;
			this.m_sizes = new SaveDataSizes(max, max - used);
		}

		// Token: 0x0600D6C5 RID: 54981 RVA: 0x004D82E0 File Offset: 0x004D64E0
		public void Dispose()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_disposed = true;
			}
		}

		// Token: 0x17001A95 RID: 6805
		// (get) Token: 0x0600D6C6 RID: 54982 RVA: 0x004D8324 File Offset: 0x004D6524
		public SaveDataSizes Sizes
		{
			get
			{
				object @lock = this.m_lock;
				SaveDataSizes sizes;
				lock (@lock)
				{
					sizes = this.m_sizes;
				}
				return sizes;
			}
		}

		// Token: 0x0600D6C7 RID: 54983 RVA: 0x004D8368 File Offset: 0x004D6568
		public void UpdateUsedEstimate(long deltaUsed)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_disposed)
				{
					return;
				}
				this.m_sizes = new SaveDataSizes(this.m_sizes.Total, Math.Max(this.m_sizes.Remaining - deltaUsed, 0L));
			}
			if (this.m_shouldUpdateSizesOnEstimate)
			{
				this.RefreshAsync();
			}
		}

		// Token: 0x0600D6C8 RID: 54984 RVA: 0x004D83E4 File Offset: 0x004D65E4
		public void RefreshSync()
		{
			if (this.m_disposed)
			{
				return;
			}
			long num = Interlocked.Increment(ref this.m_refreshCountExpected);
			this.m_getRemainingQuotaAsync(new SizeTrackerGetRemainingQuotaCompleted(this.XGameSaveGetRemainingQuotaAsyncCompleted));
			while (Interlocked.Read(ref this.m_refreshCount) < num)
			{
				Thread.Sleep(16);
			}
		}

		// Token: 0x0600D6C9 RID: 54985 RVA: 0x004D8434 File Offset: 0x004D6634
		public void RefreshAsync()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_disposed)
				{
					return;
				}
			}
			long num = Interlocked.Read(ref this.m_refreshCountExpected);
			if (Interlocked.Read(ref this.m_refreshCount) >= num)
			{
				return;
			}
			if (Interlocked.CompareExchange(ref this.m_refreshCountExpected, num + 1L, num) == num)
			{
				this.m_getRemainingQuotaAsync(new SizeTrackerGetRemainingQuotaCompleted(this.XGameSaveGetRemainingQuotaAsyncCompleted));
			}
		}

		// Token: 0x0600D6CA RID: 54986 RVA: 0x004D84C0 File Offset: 0x004D66C0
		[PublicizedFrom(EAccessModifier.Private)]
		public void XGameSaveGetRemainingQuotaAsyncCompleted(int hrXGameSaveGetRemainingQuota, long remaining)
		{
			if (HR.FAILED(hrXGameSaveGetRemainingQuota))
			{
				GameCoreSaveHelpers.NonTraceLogHR(hrXGameSaveGetRemainingQuota, "SizeTracker#XGameSaveGetRemainingQuotaAsync");
				Interlocked.Increment(ref this.m_refreshCount);
				return;
			}
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_disposed)
				{
					Interlocked.Increment(ref this.m_refreshCount);
					return;
				}
			}
			long total = this.m_sizes.Total;
			long total2 = this.m_sizes.Total;
			@lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_disposed)
				{
					Interlocked.Increment(ref this.m_refreshCount);
					return;
				}
				this.m_sizes = new SaveDataSizes(total2, remaining);
			}
			Interlocked.Increment(ref this.m_refreshCount);
		}

		// Token: 0x0400A3D0 RID: 41936
		[PublicizedFrom(EAccessModifier.Private)]
		public const int DefaultMaxSaveDataBytes = 1073741824;

		// Token: 0x0400A3D1 RID: 41937
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly long m_maxBytes;

		// Token: 0x0400A3D2 RID: 41938
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly SizeTrackerGetRemainingQuotaAsync m_getRemainingQuotaAsync;

		// Token: 0x0400A3D3 RID: 41939
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool m_shouldUpdateSizesOnEstimate;

		// Token: 0x0400A3D4 RID: 41940
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_disposed;

		// Token: 0x0400A3D5 RID: 41941
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object m_lock = new object();

		// Token: 0x0400A3D6 RID: 41942
		[PublicizedFrom(EAccessModifier.Private)]
		public long m_refreshCount;

		// Token: 0x0400A3D7 RID: 41943
		[PublicizedFrom(EAccessModifier.Private)]
		public long m_refreshCountExpected;

		// Token: 0x0400A3D8 RID: 41944
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveDataSizes m_sizes;
	}
}
