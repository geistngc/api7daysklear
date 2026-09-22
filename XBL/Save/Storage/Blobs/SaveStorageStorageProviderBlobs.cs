using System;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL.Save.Storage.Blobs
{
	// Token: 0x02001C64 RID: 7268
	public sealed class SaveStorageStorageProviderBlobs : ISaveStorageProvider, IDisposable
	{
		// Token: 0x0600D734 RID: 55092 RVA: 0x004DA3E2 File Offset: 0x004D85E2
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogInfo(string text)
		{
			Log.Out("[XBL: SaveStorageStorageProviderBlobs] " + text);
		}

		// Token: 0x0600D735 RID: 55093 RVA: 0x004DA3F4 File Offset: 0x004D85F4
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogError(string text)
		{
			Log.Error("[XBL: SaveStorageStorageProviderBlobs] " + text);
		}

		// Token: 0x0600D736 RID: 55094 RVA: 0x004DA408 File Offset: 0x004D8608
		public void Dispose()
		{
			SaveStorageStorageContainerBlobs rootSaveStorageContainer = this.m_rootSaveStorageContainer;
			if (rootSaveStorageContainer != null)
			{
				rootSaveStorageContainer.Dispose();
			}
			this.m_rootSaveStorageContainer = null;
			SizeTracker sizeTracker = this.m_sizeTracker;
			if (sizeTracker != null)
			{
				sizeTracker.Dispose();
			}
			this.m_sizeTracker = null;
			if (this.m_gameSaveProviderHandle != null)
			{
				SDK.XGameSaveCloseProvider(this.m_gameSaveProviderHandle);
				XblHelpers.LogHR(0, "Uninitialize Game Saves.", false);
				this.m_gameSaveProviderHandle = null;
			}
			this.m_statusChanged = null;
			this.m_taskScheduler = null;
			this.m_userClient = null;
			this.m_api = null;
			this.m_isDisposed = true;
		}

		// Token: 0x17001AA7 RID: 6823
		// (get) Token: 0x0600D737 RID: 55095 RVA: 0x004DA494 File Offset: 0x004D8694
		public SizeTracker SizeTracker
		{
			get
			{
				return this.m_sizeTracker;
			}
		}

		// Token: 0x17001AA8 RID: 6824
		// (get) Token: 0x0600D738 RID: 55096 RVA: 0x004DA49C File Offset: 0x004D869C
		public ISaveStorageContainer RootSaveStorageContainer
		{
			get
			{
				return this.m_rootSaveStorageContainer;
			}
		}

		// Token: 0x0600D739 RID: 55097 RVA: 0x004DA4A4 File Offset: 0x004D86A4
		public void InitializeAsync(IPlatform owner, long maxSizeBytes, SingleThreadTaskScheduler taskScheduler, OnGameSaveProviderStatusChanged statusChanged)
		{
			this.m_api = (XblPlatformApi)owner.Api;
			this.m_userClient = (User)owner.User;
			this.m_maxSizeBytes = maxSizeBytes;
			this.m_taskScheduler = taskScheduler;
			this.m_statusChanged = statusChanged;
			SDK.XGameSaveInitializeProviderAsync(this.m_userClient.UserHandle, this.m_api.SCID, false, new XGameSaveInitializeProviderCompleted(this.OnXGameSaveInitializeProviderCompleted));
		}

		// Token: 0x0600D73A RID: 55098 RVA: 0x004DA512 File Offset: 0x004D8712
		public void Flush(bool waitForFlush)
		{
			SaveStorageStorageContainerBlobs rootSaveStorageContainer = this.m_rootSaveStorageContainer;
			if (rootSaveStorageContainer == null)
			{
				return;
			}
			rootSaveStorageContainer.Flush(waitForFlush);
		}

		// Token: 0x0600D73B RID: 55099 RVA: 0x004DA528 File Offset: 0x004D8728
		[PublicizedFrom(EAccessModifier.Private)]
		public void GetRemainingQuotaAsync(SizeTrackerGetRemainingQuotaCompleted completionRoutine)
		{
			SingleThreadTaskScheduler taskScheduler = this.m_taskScheduler;
			if (taskScheduler == null)
			{
				return;
			}
			XGameSaveGetRemainingQuotaCompleted <>9__1;
			taskScheduler.ExecuteNoWait(delegate()
			{
				XGameSaveProviderHandle gameSaveProviderHandle = this.m_gameSaveProviderHandle;
				XGameSaveGetRemainingQuotaCompleted onCompleted;
				if ((onCompleted = <>9__1) == null)
				{
					onCompleted = (<>9__1 = delegate(int hr, long remaining)
					{
						SingleThreadTaskScheduler taskScheduler2 = this.m_taskScheduler;
						if (taskScheduler2 == null)
						{
							return;
						}
						taskScheduler2.ExecuteNoWait(delegate()
						{
							if (this.m_rootSaveStorageContainer != null)
							{
								remaining = Math.Max(0L, remaining - (long)this.m_rootSaveStorageContainer.GetQueuedUsed());
							}
							completionRoutine(hr, remaining);
						});
					});
				}
				SDK.XGameSaveGetRemainingQuotaAsync(gameSaveProviderHandle, onCompleted);
			});
		}

		// Token: 0x0600D73C RID: 55100 RVA: 0x004DA568 File Offset: 0x004D8768
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnXGameSaveInitializeProviderCompleted(int hr, XGameSaveProviderHandle gameSaveProviderHandle)
		{
			XblHelpers.LogHR(hr, "Initialize Game Saves (Blobs).", false);
			if (this.m_isDisposed)
			{
				SaveStorageStorageProviderBlobs.LogInfo("Disposed before XGameSaveInitializeProviderAsync completed, cancelling setup");
				if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
				{
					SDK.XGameSaveCloseProvider(gameSaveProviderHandle);
				}
				return;
			}
			if (!Unity.XGamingRuntime.Interop.HR.FAILED(hr))
			{
				this.m_gameSaveProviderHandle = gameSaveProviderHandle;
				this.m_taskScheduler.ExecuteNoWait(new Action(this.OnXGameSaveProviderHandleReady));
				return;
			}
			if (hr == -2138898428)
			{
				this.m_statusChanged(ESaveGameProviderStatus.TemporaryError);
				SaveStorageStorageProviderBlobs.LogInfo("User cancelled sync, retrying...");
				SDK.XGameSaveInitializeProviderAsync(this.m_userClient.UserHandle, this.m_api.SCID, false, new XGameSaveInitializeProviderCompleted(this.OnXGameSaveInitializeProviderCompleted));
				return;
			}
			SaveStorageStorageProviderBlobs.LogError("Could not Initialize. This is a FATAL error!");
			this.m_statusChanged(ESaveGameProviderStatus.PermanentError);
		}

		// Token: 0x0600D73D RID: 55101 RVA: 0x004DA628 File Offset: 0x004D8828
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnXGameSaveProviderHandleReady()
		{
			ulong num = 0UL;
			XGameSaveContainerInfo[] array;
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(SDK.XGameSaveEnumerateContainerInfo(this.m_gameSaveProviderHandle, out array)))
			{
				foreach (XGameSaveContainerInfo xgameSaveContainerInfo in array)
				{
					SaveStorageStorageProviderBlobs.LogInfo(string.Format("Container '{0}' (display name '{1}') has {2} blobs. NeedsSync:{3} TotalSize:{4} LastModifiedTime:{5}", new object[]
					{
						xgameSaveContainerInfo.Name,
						xgameSaveContainerInfo.DisplayName,
						xgameSaveContainerInfo.BlobCount,
						xgameSaveContainerInfo.NeedsSync,
						xgameSaveContainerInfo.TotalSize,
						xgameSaveContainerInfo.LastModifiedTime
					}));
					num += xgameSaveContainerInfo.TotalSize;
				}
			}
			this.m_sizeTracker = new SizeTracker(this.m_maxSizeBytes, (long)num, new SizeTrackerGetRemainingQuotaAsync(this.GetRemainingQuotaAsync), true);
			this.m_sizeTracker.RefreshAsync();
			this.m_rootSaveStorageContainer = new SaveStorageStorageContainerBlobs("root", this.m_gameSaveProviderHandle, this.m_taskScheduler);
			this.m_statusChanged(ESaveGameProviderStatus.Ok);
		}

		// Token: 0x0400A423 RID: 42019
		[PublicizedFrom(EAccessModifier.Private)]
		public XblPlatformApi m_api;

		// Token: 0x0400A424 RID: 42020
		[PublicizedFrom(EAccessModifier.Private)]
		public User m_userClient;

		// Token: 0x0400A425 RID: 42021
		[PublicizedFrom(EAccessModifier.Private)]
		public SingleThreadTaskScheduler m_taskScheduler;

		// Token: 0x0400A426 RID: 42022
		[PublicizedFrom(EAccessModifier.Private)]
		public OnGameSaveProviderStatusChanged m_statusChanged;

		// Token: 0x0400A427 RID: 42023
		[PublicizedFrom(EAccessModifier.Private)]
		public XGameSaveProviderHandle m_gameSaveProviderHandle;

		// Token: 0x0400A428 RID: 42024
		[PublicizedFrom(EAccessModifier.Private)]
		public long m_maxSizeBytes;

		// Token: 0x0400A429 RID: 42025
		[PublicizedFrom(EAccessModifier.Private)]
		public SizeTracker m_sizeTracker;

		// Token: 0x0400A42A RID: 42026
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveStorageStorageContainerBlobs m_rootSaveStorageContainer;

		// Token: 0x0400A42B RID: 42027
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_isDisposed;
	}
}
