using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Platform.Shared;
using Platform.XBL.Save;
using Platform.XBL.Save.Storage;
using Unity.XGamingRuntime;

namespace Platform.XBL
{
	// Token: 0x02001C00 RID: 7168
	public class SaveGameProviderGameCore : IPlatformSaveGameProvider, IPlatformSaveGameIOProvider
	{
		// Token: 0x0600D4E4 RID: 54500 RVA: 0x004CF834 File Offset: 0x004CDA34
		public SaveGameProviderGameCore(long maxStorageSizeBytes = 1073741824L)
		{
			this.m_maxStorageSizeBytes = maxStorageSizeBytes;
		}

		// Token: 0x0600D4E5 RID: 54501 RVA: 0x004CF84E File Offset: 0x004CDA4E
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnUserHandleReady(XUserHandle userHandle)
		{
			if (userHandle == null)
			{
				Log.Error("[XBL] SaveGameProviderGameCore.OnUserHandleReady: Attempting to retrieve XSTS token before acquiring XUserHandle");
				return;
			}
			if (userHandle.IsInvalid)
			{
				Log.Error("[XBL] SaveGameProviderGameCore.OnUserHandleReady: m_userHandle.IsInvalid is true");
			}
			if (userHandle.IsClosed)
			{
				Log.Error("[XBL] SaveGameProviderGameCore.OnUserHandleReady: m_userHandle.IsClosed is true");
			}
			this.InitializeSaveStorageProvider();
		}

		// Token: 0x0600D4E6 RID: 54502 RVA: 0x004CF890 File Offset: 0x004CDA90
		[PublicizedFrom(EAccessModifier.Private)]
		public void InitializeSaveStorageProvider()
		{
			SaveStorageProvider value = LaunchPrefs.GameCoreSaveStorageProvider.Value;
			SaveGameProviderGameCore.LogInfo(string.Format("[GDK] Initializing Save Storage Provider '{0}'.", value));
			this.m_saveStorageProvider = value.Create();
			this.m_saveStorageProvider.InitializeAsync(this.m_owner, this.m_maxStorageSizeBytes, this.m_taskScheduler, new OnGameSaveProviderStatusChanged(this.OnGameSaveProviderStatusChanged));
		}

		// Token: 0x0600D4E7 RID: 54503 RVA: 0x004CF8F4 File Offset: 0x004CDAF4
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnGameSaveProviderStatusChanged(ESaveGameProviderStatus status)
		{
			if (status == ESaveGameProviderStatus.Ok && this.m_rootSaveContainer == null)
			{
				this.OnGameSaveProviderReady();
			}
			object initializedDelegateLock = this.m_initializedDelegateLock;
			lock (initializedDelegateLock)
			{
				bool flag2 = status == ESaveGameProviderStatus.Ok && this.Status != ESaveGameProviderStatus.Ok;
				this.Status = status;
				if (flag2)
				{
					Action initializedDelegate = this.m_initializedDelegate;
					if (initializedDelegate != null)
					{
						initializedDelegate();
					}
				}
			}
		}

		// Token: 0x0600D4E8 RID: 54504 RVA: 0x004CF970 File Offset: 0x004CDB70
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnGameSaveProviderReady()
		{
			this.m_rootSaveContainer = new SaveContainer(this.m_saveStorageProvider.RootSaveStorageContainer, this.m_saveStorageProvider.SizeTracker);
			this.ResumeIO();
		}

		// Token: 0x0600D4E9 RID: 54505 RVA: 0x004CF99C File Offset: 0x004CDB9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnApplicationStateChanged(ApplicationState newstate)
		{
			bool flag = newstate == ApplicationState.Suspended;
			if (flag != this.m_suspended)
			{
				this.m_suspended = flag;
				if (flag)
				{
					this.OnSuspend();
					return;
				}
				this.OnResume();
			}
		}

		// Token: 0x0600D4EA RID: 54506 RVA: 0x004CF9CE File Offset: 0x004CDBCE
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnSuspend()
		{
			this.PauseIO();
		}

		// Token: 0x0600D4EB RID: 54507 RVA: 0x004CF9D8 File Offset: 0x004CDBD8
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnResume()
		{
			if (!this.Status.IsTerminal())
			{
				SaveGameProviderGameCore.LogWarning("Status was not terminal on resume? Did sleep happen during loading?");
			}
			SaveContainer rootSaveContainer = this.m_rootSaveContainer;
			if (rootSaveContainer != null)
			{
				rootSaveContainer.Dispose();
			}
			this.m_rootSaveContainer = null;
			ISaveStorageProvider saveStorageProvider = this.m_saveStorageProvider;
			if (saveStorageProvider != null)
			{
				saveStorageProvider.Dispose();
			}
			this.m_saveStorageProvider = null;
			this.Status = ESaveGameProviderStatus.Uninitialized;
			this.InitializeSaveStorageProvider();
		}

		// Token: 0x0600D4EC RID: 54508 RVA: 0x004CFA3C File Offset: 0x004CDC3C
		[PublicizedFrom(EAccessModifier.Private)]
		public void PauseIO()
		{
			if (this.m_paused)
			{
				SaveGameProviderGameCore.LogError("PauseIO should only be called when IO is not paused.");
				return;
			}
			this.m_paused = true;
			while (this.m_paused && this.m_operations > 0)
			{
				Thread.Sleep(5);
			}
			this.Flush(true);
			if (!this.m_paused)
			{
				SaveGameProviderGameCore.LogWarning("Was un-paused before we finished waiting on IO to pause.");
			}
		}

		// Token: 0x0600D4ED RID: 54509 RVA: 0x004CFA95 File Offset: 0x004CDC95
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResumeIO()
		{
			if (!this.m_paused)
			{
				SaveGameProviderGameCore.LogError("ResumeIO should only be called when IO is paused.");
				return;
			}
			this.m_paused = false;
		}

		// Token: 0x0600D4EE RID: 54510 RVA: 0x004CFAB1 File Offset: 0x004CDCB1
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveGameProviderGameCore.OperationScope CreateOperationScope(string debugIdentifier)
		{
			if (this.m_paused)
			{
				while (this.m_paused)
				{
					Thread.Sleep(50);
				}
			}
			return new SaveGameProviderGameCore.OperationScope(this);
		}

		// Token: 0x0600D4EF RID: 54511 RVA: 0x004CFAD2 File Offset: 0x004CDCD2
		[PublicizedFrom(EAccessModifier.Private)]
		public void GetSaveContainerAndRelativePath(SaveDataManagedPath path, out SaveContainer saveContainer, out string relativePath)
		{
			saveContainer = this.m_rootSaveContainer;
			relativePath = path.PathRelativeToRoot;
		}

		// Token: 0x0600D4F0 RID: 54512 RVA: 0x004CFAE4 File Offset: 0x004CDCE4
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogInfo(string text)
		{
			Log.Out("[XBL: SaveGameProvider] " + text);
		}

		// Token: 0x0600D4F1 RID: 54513 RVA: 0x004CFAF6 File Offset: 0x004CDCF6
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogWarning(string text)
		{
			Log.Warning("[XBL: SaveGameProvider] " + text);
		}

		// Token: 0x0600D4F2 RID: 54514 RVA: 0x004CFB08 File Offset: 0x004CDD08
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogError(string text)
		{
			Log.Error("[XBL: SaveGameProvider] " + text);
		}

		// Token: 0x0600D4F3 RID: 54515 RVA: 0x004CFAE4 File Offset: 0x004CDCE4
		[Conditional("DEBUG_SAVE_DATA_MANAGER")]
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogTrace(string text)
		{
			Log.Out("[XBL: SaveGameProvider] " + text);
		}

		// Token: 0x17001A68 RID: 6760
		// (get) Token: 0x0600D4F4 RID: 54516 RVA: 0x004CFB1A File Offset: 0x004CDD1A
		// (set) Token: 0x0600D4F5 RID: 54517 RVA: 0x004CFB22 File Offset: 0x004CDD22
		public ESaveGameProviderStatus Status { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A69 RID: 6761
		// (get) Token: 0x0600D4F6 RID: 54518 RVA: 0x004CFB2B File Offset: 0x004CDD2B
		// (set) Token: 0x0600D4F7 RID: 54519 RVA: 0x004CFB33 File Offset: 0x004CDD33
		public IPlatformSaveGameIOProvider Cache { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1400012B RID: 299
		// (add) Token: 0x0600D4F8 RID: 54520 RVA: 0x004CFB3C File Offset: 0x004CDD3C
		// (remove) Token: 0x0600D4F9 RID: 54521 RVA: 0x004CFB9C File Offset: 0x004CDD9C
		public event Action Initialized
		{
			add
			{
				object initializedDelegateLock = this.m_initializedDelegateLock;
				lock (initializedDelegateLock)
				{
					this.m_initializedDelegate = (Action)Delegate.Combine(this.m_initializedDelegate, value);
					if (this.Status == ESaveGameProviderStatus.Ok)
					{
						value();
					}
				}
			}
			remove
			{
				object initializedDelegateLock = this.m_initializedDelegateLock;
				lock (initializedDelegateLock)
				{
					this.m_initializedDelegate = (Action)Delegate.Remove(this.m_initializedDelegate, value);
				}
			}
		}

		// Token: 0x0600D4FA RID: 54522 RVA: 0x004CFBF0 File Offset: 0x004CDDF0
		public void Init(IPlatform _owner)
		{
			this.PauseIO();
			this.m_taskScheduler = new SingleThreadTaskScheduler("GameCore", "Game Core Save Tasks");
			this.m_owner = _owner;
			this.m_appState = _owner.ApplicationState;
			this.m_appState.OnApplicationStateChanged += this.OnApplicationStateChanged;
			this.m_userClient = (User)this.m_owner.User;
			this.m_userClient.UserHandleReady += this.OnUserHandleReady;
			this.Cache = new SaveGameIOProviderFixedRoot(GameIO.GetNormalizedPath(GameIO.GetDeviceLocalUserGameDataDir()) + "Cache");
		}

		// Token: 0x0600D4FB RID: 54523 RVA: 0x004CFC90 File Offset: 0x004CDE90
		public void Destroy()
		{
			if (this.m_rootSaveContainer != null)
			{
				this.m_rootSaveContainer.Dispose();
				this.m_rootSaveContainer = null;
			}
			ISaveStorageProvider saveStorageProvider = this.m_saveStorageProvider;
			if (saveStorageProvider != null)
			{
				saveStorageProvider.Dispose();
			}
			this.m_saveStorageProvider = null;
			if (this.m_userClient != null)
			{
				this.m_userClient.UserHandleReady -= this.OnUserHandleReady;
				this.m_userClient = null;
			}
			if (this.m_appState != null)
			{
				this.m_appState.OnApplicationStateChanged -= this.OnApplicationStateChanged;
				this.m_appState = null;
			}
			this.m_owner = null;
			SingleThreadTaskScheduler taskScheduler = this.m_taskScheduler;
			if (taskScheduler != null)
			{
				taskScheduler.Dispose();
			}
			this.m_taskScheduler = null;
		}

		// Token: 0x0600D4FC RID: 54524 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool ShouldBackup()
		{
			return false;
		}

		// Token: 0x0600D4FD RID: 54525 RVA: 0x004CFD3A File Offset: 0x004CDF3A
		public bool ShouldCommit()
		{
			return !this.m_paused;
		}

		// Token: 0x0600D4FE RID: 54526 RVA: 0x004CFD45 File Offset: 0x004CDF45
		public double GetCommitProgress()
		{
			return 1.0;
		}

		// Token: 0x0600D4FF RID: 54527 RVA: 0x004CFD50 File Offset: 0x004CDF50
		public void Flush(bool waitForFlush)
		{
			ISaveStorageProvider saveStorageProvider = this.m_saveStorageProvider;
			if (saveStorageProvider == null)
			{
				return;
			}
			saveStorageProvider.Flush(waitForFlush);
		}

		// Token: 0x0600D500 RID: 54528 RVA: 0x0002003D File Offset: 0x0001E23D
		public bool ShouldLimitSize()
		{
			return true;
		}

		// Token: 0x0600D501 RID: 54529 RVA: 0x004CFD64 File Offset: 0x004CDF64
		public void UpdateSizes()
		{
			using (this.CreateOperationScope("UpdateSizes"))
			{
				this.m_saveStorageProvider.SizeTracker.RefreshSync();
			}
		}

		// Token: 0x0600D502 RID: 54530 RVA: 0x004CFDB0 File Offset: 0x004CDFB0
		public SaveDataSizes GetSizes()
		{
			SaveDataSizes sizes;
			using (this.CreateOperationScope("GetSizes"))
			{
				sizes = this.m_saveStorageProvider.SizeTracker.Sizes;
			}
			return sizes;
		}

		// Token: 0x0600D503 RID: 54531 RVA: 0x004CFDFC File Offset: 0x004CDFFC
		public void ManagedFileRead(SaveDataManagedPath path, Stream dest)
		{
			using (this.CreateOperationScope("ManagedFileRead"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				saveContainer.FileRead(str, dest);
			}
		}

		// Token: 0x0600D504 RID: 54532 RVA: 0x004CFE50 File Offset: 0x004CE050
		public void ManagedFileWrite(SaveDataManagedPath path, Stream src)
		{
			using (this.CreateOperationScope("ManagedFileWrite"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				saveContainer.FileWrite(str, src);
			}
		}

		// Token: 0x0600D505 RID: 54533 RVA: 0x004CFEA4 File Offset: 0x004CE0A4
		public void ManagedFileCopy(SaveDataManagedPath sourceFileName, SaveDataManagedPath destFileName, bool overwrite = false)
		{
			using (this.CreateOperationScope("ManagedFileCopy"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(destFileName, out saveContainer, out str);
				if (!overwrite && saveContainer.FileExists(str))
				{
					throw new IOException(string.Format("Destination '{0}' already exists.", destFileName));
				}
				SaveContainer saveContainer2;
				string str2;
				this.GetSaveContainerAndRelativePath(sourceFileName, out saveContainer2, out str2);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					saveContainer2.FileRead(str2, memoryStream);
					memoryStream.Position = 0L;
					saveContainer.FileWrite(str, memoryStream);
				}
			}
		}

		// Token: 0x0600D506 RID: 54534 RVA: 0x004CFF5C File Offset: 0x004CE15C
		public void ManagedFileDelete(SaveDataManagedPath path)
		{
			using (this.CreateOperationScope("ManagedFileDelete"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				saveContainer.FileDelete(str);
			}
		}

		// Token: 0x0600D507 RID: 54535 RVA: 0x004CFFAC File Offset: 0x004CE1AC
		public bool ManagedFileExists(SaveDataManagedPath path)
		{
			bool result;
			using (this.CreateOperationScope("ManagedFileExists"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				result = saveContainer.FileExists(str);
			}
			return result;
		}

		// Token: 0x0600D508 RID: 54536 RVA: 0x004D0000 File Offset: 0x004CE200
		public DateTime ManagedFileGetLastWriteTimeUtc(SaveDataManagedPath path)
		{
			DateTime lastWriteTimeUtc;
			using (this.CreateOperationScope("ManagedFileGetLastWriteTimeUtc"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				lastWriteTimeUtc = saveContainer.GetLastWriteTimeUtc(str);
			}
			return lastWriteTimeUtc;
		}

		// Token: 0x0600D509 RID: 54537 RVA: 0x004D0054 File Offset: 0x004CE254
		public void ManagedFileMove(SaveDataManagedPath sourceFileName, SaveDataManagedPath destFileName)
		{
			using (this.CreateOperationScope("ManagedFileMove"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(destFileName, out saveContainer, out str);
				if (saveContainer.FileExists(str))
				{
					throw new IOException(string.Format("Destination '{0}' already exists.", destFileName));
				}
				SaveContainer saveContainer2;
				string str2;
				this.GetSaveContainerAndRelativePath(sourceFileName, out saveContainer2, out str2);
				if (saveContainer2 != saveContainer)
				{
					this.ManagedFileCopy(sourceFileName, destFileName, false);
					this.ManagedFileDelete(sourceFileName);
				}
				else
				{
					saveContainer2.FileMove(str2, str);
				}
			}
		}

		// Token: 0x0600D50A RID: 54538 RVA: 0x004D00EC File Offset: 0x004CE2EC
		public SdDirectoryInfo ManagedDirectoryCreateDirectory(SaveDataManagedPath path)
		{
			SdDirectoryInfo result;
			using (this.CreateOperationScope("ManagedDirectoryCreateDirectory"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				saveContainer.DirectoryCreate(str);
				result = new SdDirectoryInfo(path.GetOriginalPath());
			}
			return result;
		}

		// Token: 0x0600D50B RID: 54539 RVA: 0x004D014C File Offset: 0x004CE34C
		public DateTime ManagedDirectoryGetLastWriteTimeUtc(SaveDataManagedPath path)
		{
			DateTime lastWriteTimeUtc;
			using (this.CreateOperationScope("ManagedDirectoryGetLastWriteTimeUtc"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				lastWriteTimeUtc = saveContainer.GetLastWriteTimeUtc(str);
			}
			return lastWriteTimeUtc;
		}

		// Token: 0x0600D50C RID: 54540 RVA: 0x004D01A0 File Offset: 0x004CE3A0
		public bool ManagedDirectoryExists(SaveDataManagedPath path)
		{
			bool result;
			using (this.CreateOperationScope("ManagedDirectoryExists"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				result = saveContainer.DirectoryExists(str);
			}
			return result;
		}

		// Token: 0x0600D50D RID: 54541 RVA: 0x004D01F4 File Offset: 0x004CE3F4
		public IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			IEnumerable<SaveDataManagedPath> result;
			using (this.CreateOperationScope("ManagedDirectoryEnumerateDirectories"))
			{
				result = this.ManagedDirectoryEnumerateInternal(path, searchPattern, searchOption, true, false).ToArray<SaveDataManagedPath>();
			}
			return result;
		}

		// Token: 0x0600D50E RID: 54542 RVA: 0x004D0240 File Offset: 0x004CE440
		public IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			IEnumerable<SaveDataManagedPath> result;
			using (this.CreateOperationScope("ManagedDirectoryEnumerateFiles"))
			{
				result = this.ManagedDirectoryEnumerateInternal(path, searchPattern, searchOption, false, true).ToArray<SaveDataManagedPath>();
			}
			return result;
		}

		// Token: 0x0600D50F RID: 54543 RVA: 0x004D028C File Offset: 0x004CE48C
		public IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateFileSystemEntries(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			IEnumerable<SaveDataManagedPath> result;
			using (this.CreateOperationScope("ManagedDirectoryEnumerateFileSystemEntries"))
			{
				result = this.ManagedDirectoryEnumerateInternal(path, searchPattern, searchOption, true, true).ToArray<SaveDataManagedPath>();
			}
			return result;
		}

		// Token: 0x0600D510 RID: 54544 RVA: 0x004D02D8 File Offset: 0x004CE4D8
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerable<SaveDataManagedPath> ManagedDirectoryEnumerateInternal(SaveDataManagedPath path, string searchPattern, SearchOption searchOption, bool includeDirectories, bool includeFiles)
		{
			SaveContainer saveContainer;
			string text;
			this.GetSaveContainerAndRelativePath(path, out saveContainer, out text);
			IEnumerable<string> subPaths = saveContainer.DirectoryEnumerate(text, searchPattern, searchOption == SearchOption.AllDirectories, includeDirectories, includeFiles);
			return SaveGameProviderHelper.GetManagedPathsFromBaseAndSubPaths(path, text, subPaths);
		}

		// Token: 0x0600D511 RID: 54545 RVA: 0x004D030C File Offset: 0x004CE50C
		public void ManagedDirectoryDelete(SaveDataManagedPath path, bool recursive)
		{
			using (this.CreateOperationScope("ManagedDirectoryDelete"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				saveContainer.DirectoryDelete(str, recursive);
			}
		}

		// Token: 0x0600D512 RID: 54546 RVA: 0x004D0360 File Offset: 0x004CE560
		public long ManagedFileInfoLength(SaveDataManagedPath path)
		{
			long result;
			using (this.CreateOperationScope("ManagedFileInfoLength"))
			{
				SaveContainer saveContainer;
				string str;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out str);
				result = saveContainer.FileLength(str);
			}
			return result;
		}

		// Token: 0x0600D513 RID: 54547 RVA: 0x004D03B4 File Offset: 0x004CE5B4
		public IEnumerable<SdDirectoryInfo> ManagedDirectoryInfoEnumerateDirectories(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			IEnumerable<SdDirectoryInfo> result;
			using (this.CreateOperationScope("ManagedDirectoryInfoEnumerateDirectories"))
			{
				result = (from managedPath in this.ManagedDirectoryEnumerateInternal(path, searchPattern, searchOption, true, false)
				select new SdDirectoryInfo(managedPath)).ToArray<SdDirectoryInfo>();
			}
			return result;
		}

		// Token: 0x0600D514 RID: 54548 RVA: 0x004D0424 File Offset: 0x004CE624
		public IEnumerable<SdFileInfo> ManagedDirectoryInfoEnumerateFiles(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			IEnumerable<SdFileInfo> result;
			using (this.CreateOperationScope("ManagedDirectoryInfoEnumerateFiles"))
			{
				result = (from managedPath in this.ManagedDirectoryEnumerateInternal(path, searchPattern, searchOption, false, true)
				select new SdFileInfo(managedPath)).ToArray<SdFileInfo>();
			}
			return result;
		}

		// Token: 0x0600D515 RID: 54549 RVA: 0x004D0494 File Offset: 0x004CE694
		public IEnumerable<SdFileSystemInfo> ManagedDirectoryInfoEnumerateFileSystemInfos(SaveDataManagedPath path, string searchPattern, SearchOption searchOption)
		{
			IEnumerable<SdFileSystemInfo> result;
			using (this.CreateOperationScope("ManagedDirectoryInfoEnumerateFileSystemInfos"))
			{
				string relativePath;
				SaveContainer saveContainer;
				this.GetSaveContainerAndRelativePath(path, out saveContainer, out relativePath);
				result = saveContainer.DirectoryEnumerate(relativePath, searchPattern, searchOption == SearchOption.AllDirectories).Select(delegate(PathEnumerationInfo x)
				{
					SaveDataManagedPath managedPathFromBaseAndSubPath = SaveGameProviderHelper.GetManagedPathFromBaseAndSubPath(path, relativePath, x.RelativePath);
					if (!x.IsDirectory)
					{
						return new SdFileInfo(managedPathFromBaseAndSubPath);
					}
					return new SdDirectoryInfo(managedPathFromBaseAndSubPath);
				}).ToArray<SdFileSystemInfo>();
			}
			return result;
		}

		// Token: 0x0400A246 RID: 41542
		[PublicizedFrom(EAccessModifier.Private)]
		public SingleThreadTaskScheduler m_taskScheduler;

		// Token: 0x0400A247 RID: 41543
		[PublicizedFrom(EAccessModifier.Private)]
		public Action m_initializedDelegate;

		// Token: 0x0400A248 RID: 41544
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object m_initializedDelegateLock = new object();

		// Token: 0x0400A249 RID: 41545
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform m_owner;

		// Token: 0x0400A24A RID: 41546
		[PublicizedFrom(EAccessModifier.Private)]
		public IApplicationStateController m_appState;

		// Token: 0x0400A24B RID: 41547
		[PublicizedFrom(EAccessModifier.Private)]
		public User m_userClient;

		// Token: 0x0400A24C RID: 41548
		[PublicizedFrom(EAccessModifier.Private)]
		public long m_maxStorageSizeBytes;

		// Token: 0x0400A24D RID: 41549
		[PublicizedFrom(EAccessModifier.Private)]
		public ISaveStorageProvider m_saveStorageProvider;

		// Token: 0x0400A24E RID: 41550
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_suspended;

		// Token: 0x0400A24F RID: 41551
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_paused;

		// Token: 0x0400A250 RID: 41552
		[PublicizedFrom(EAccessModifier.Private)]
		public int m_operations;

		// Token: 0x0400A251 RID: 41553
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveContainer m_rootSaveContainer;

		// Token: 0x02001C01 RID: 7169
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly struct OperationScope : IDisposable
		{
			// Token: 0x0600D516 RID: 54550 RVA: 0x004D0518 File Offset: 0x004CE718
			public OperationScope(SaveGameProviderGameCore provider)
			{
				this.m_provider = provider;
				Interlocked.Increment(ref this.m_provider.m_operations);
			}

			// Token: 0x0600D517 RID: 54551 RVA: 0x004D0532 File Offset: 0x004CE732
			public void Dispose()
			{
				Interlocked.Decrement(ref this.m_provider.m_operations);
			}

			// Token: 0x0400A254 RID: 41556
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly SaveGameProviderGameCore m_provider;
		}
	}
}
