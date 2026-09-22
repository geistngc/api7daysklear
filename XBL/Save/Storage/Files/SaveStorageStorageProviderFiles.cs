using System;
using System.IO;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL.Save.Storage.Files
{
	// Token: 0x02001C56 RID: 7254
	public sealed class SaveStorageStorageProviderFiles : ISaveStorageProvider, IDisposable
	{
		// Token: 0x0600D6F0 RID: 55024 RVA: 0x004D8A8C File Offset: 0x004D6C8C
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogInfo(string text)
		{
			Log.Out("[XBL: SaveStorageStorageProviderFiles] " + text);
		}

		// Token: 0x0600D6F1 RID: 55025 RVA: 0x004D8A9E File Offset: 0x004D6C9E
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogError(string text)
		{
			Log.Error("[XBL: SaveStorageStorageProviderFiles] " + text);
		}

		// Token: 0x0600D6F2 RID: 55026 RVA: 0x004D8AB0 File Offset: 0x004D6CB0
		public void Dispose()
		{
			SaveStorageStorageContainerFiles rootSaveStorageContainer = this.m_rootSaveStorageContainer;
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
			this.m_gameSaveDirectory = null;
			this.m_statusChanged = null;
			this.m_taskScheduler = null;
			this.m_userClient = null;
			this.m_api = null;
			this.m_isDisposed = true;
		}

		// Token: 0x17001A9E RID: 6814
		// (get) Token: 0x0600D6F3 RID: 55027 RVA: 0x004D8B17 File Offset: 0x004D6D17
		public SizeTracker SizeTracker
		{
			get
			{
				return this.m_sizeTracker;
			}
		}

		// Token: 0x17001A9F RID: 6815
		// (get) Token: 0x0600D6F4 RID: 55028 RVA: 0x004D8B1F File Offset: 0x004D6D1F
		public ISaveStorageContainer RootSaveStorageContainer
		{
			get
			{
				return this.m_rootSaveStorageContainer;
			}
		}

		// Token: 0x0600D6F5 RID: 55029 RVA: 0x004D8B28 File Offset: 0x004D6D28
		public void InitializeAsync(IPlatform owner, long maxSizeBytes, SingleThreadTaskScheduler taskScheduler, OnGameSaveProviderStatusChanged statusChanged)
		{
			this.m_api = (XblPlatformApi)owner.Api;
			this.m_userClient = (User)owner.User;
			this.m_maxSizeBytes = maxSizeBytes;
			this.m_taskScheduler = taskScheduler;
			this.m_statusChanged = statusChanged;
			SDK.XGameSaveFilesGetFolderWithUiAsync(this.m_userClient.UserHandle, this.m_api.SCID, new XGameSaveFilesGetFolderWithUiCompleted(this.OnGameSaveFilesGetFolderWithUiCompleted));
		}

		// Token: 0x0600D6F6 RID: 55030 RVA: 0x004D8B95 File Offset: 0x004D6D95
		public void Flush(bool waitForFlush)
		{
			SaveStorageStorageContainerFiles rootSaveStorageContainer = this.m_rootSaveStorageContainer;
			if (rootSaveStorageContainer == null)
			{
				return;
			}
			rootSaveStorageContainer.Flush(waitForFlush);
		}

		// Token: 0x0600D6F7 RID: 55031 RVA: 0x004D8BA8 File Offset: 0x004D6DA8
		[PublicizedFrom(EAccessModifier.Private)]
		public void GetRemainingQuotaAsync(SizeTrackerGetRemainingQuotaCompleted completionRoutine)
		{
			this.m_taskScheduler.ExecuteNoWait(delegate()
			{
				ulong remainingQuota;
				int hresult = SDK.XGameSaveFilesGetRemainingQuota(this.m_userClient.UserHandle, this.m_api.SCID, out remainingQuota);
				completionRoutine(hresult, (long)remainingQuota);
			});
		}

		// Token: 0x0600D6F8 RID: 55032 RVA: 0x004D8BE4 File Offset: 0x004D6DE4
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnGameSaveFilesGetFolderWithUiCompleted(int hr, string folderResult)
		{
			XblHelpers.LogHR(hr, "Initialize Game Saves (Files).", false);
			if (this.m_isDisposed)
			{
				SaveStorageStorageProviderFiles.LogInfo("Disposed before XGameSaveFilesGetFolderWithUiAsync completed, cancelling setup");
				return;
			}
			if (!Unity.XGamingRuntime.Interop.HR.FAILED(hr))
			{
				this.m_gameSaveDirectory = Path.GetFullPath(folderResult);
				SaveStorageStorageProviderFiles.LogInfo("Game Save Files Directory: " + this.m_gameSaveDirectory);
				this.m_taskScheduler.ExecuteNoWait(new Action(this.OnGameSaveDirectoryReady));
				return;
			}
			if (hr == -2138898428)
			{
				this.m_statusChanged(ESaveGameProviderStatus.TemporaryError);
				SaveStorageStorageProviderFiles.LogInfo("User cancelled sync, retrying...");
				SDK.XGameSaveFilesGetFolderWithUiAsync(this.m_userClient.UserHandle, this.m_api.SCID, new XGameSaveFilesGetFolderWithUiCompleted(this.OnGameSaveFilesGetFolderWithUiCompleted));
				return;
			}
			SaveStorageStorageProviderFiles.LogError("Could not Initialize. This is a FATAL error!");
			this.m_statusChanged(ESaveGameProviderStatus.PermanentError);
		}

		// Token: 0x0600D6F9 RID: 55033 RVA: 0x004D8CB0 File Offset: 0x004D6EB0
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnGameSaveDirectoryReady()
		{
			this.m_sizeTracker = new SizeTracker(this.m_maxSizeBytes, 0L, new SizeTrackerGetRemainingQuotaAsync(this.GetRemainingQuotaAsync), false);
			this.m_sizeTracker.RefreshSync();
			this.m_rootSaveStorageContainer = new SaveStorageStorageContainerFiles(this.m_gameSaveDirectory, "root");
			this.m_statusChanged(ESaveGameProviderStatus.Ok);
		}

		// Token: 0x0400A3E3 RID: 41955
		[PublicizedFrom(EAccessModifier.Private)]
		public XblPlatformApi m_api;

		// Token: 0x0400A3E4 RID: 41956
		[PublicizedFrom(EAccessModifier.Private)]
		public User m_userClient;

		// Token: 0x0400A3E5 RID: 41957
		[PublicizedFrom(EAccessModifier.Private)]
		public SingleThreadTaskScheduler m_taskScheduler;

		// Token: 0x0400A3E6 RID: 41958
		[PublicizedFrom(EAccessModifier.Private)]
		public OnGameSaveProviderStatusChanged m_statusChanged;

		// Token: 0x0400A3E7 RID: 41959
		[PublicizedFrom(EAccessModifier.Private)]
		public string m_gameSaveDirectory;

		// Token: 0x0400A3E8 RID: 41960
		[PublicizedFrom(EAccessModifier.Private)]
		public long m_maxSizeBytes;

		// Token: 0x0400A3E9 RID: 41961
		[PublicizedFrom(EAccessModifier.Private)]
		public SizeTracker m_sizeTracker;

		// Token: 0x0400A3EA RID: 41962
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveStorageStorageContainerFiles m_rootSaveStorageContainer;

		// Token: 0x0400A3EB RID: 41963
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_isDisposed;
	}
}
