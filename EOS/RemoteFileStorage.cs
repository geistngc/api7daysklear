using System;
using System.Collections.Generic;
using System.IO;
using Epic.OnlineServices;
using Epic.OnlineServices.TitleStorage;

namespace Platform.EOS
{
	// Token: 0x02001D05 RID: 7429
	public class RemoteFileStorage : IRemoteFileStorage
	{
		// Token: 0x0600DC4C RID: 56396 RVA: 0x004EF184 File Offset: 0x004ED384
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += this.OnClientApiInitialized;
			if (this.owner.User != null)
			{
				this.owner.User.UserLoggedIn += this.OnUserLoggedIn;
			}
		}

		// Token: 0x0600DC4D RID: 56397 RVA: 0x004EF1DD File Offset: 0x004ED3DD
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnUserLoggedIn(IPlatform _obj)
		{
			if (this.IsReady)
			{
				this.clearCache();
			}
		}

		// Token: 0x0600DC4E RID: 56398 RVA: 0x004EF1ED File Offset: 0x004ED3ED
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnClientApiInitialized()
		{
			this.titleStorageInterface = ((Api)this.owner.Api).PlatformInterface.GetTitleStorageInterface();
		}

		// Token: 0x17001B74 RID: 7028
		// (get) Token: 0x0600DC4F RID: 56399 RVA: 0x004EF210 File Offset: 0x004ED410
		public bool IsReady
		{
			get
			{
				if (!(this.titleStorageInterface != null))
				{
					return false;
				}
				if (GameManager.IsDedicatedServer)
				{
					return true;
				}
				IPlatform platform = this.owner;
				if (platform == null)
				{
					return false;
				}
				IUserClient user = platform.User;
				EUserStatus? euserStatus = (user != null) ? new EUserStatus?(user.UserStatus) : null;
				EUserStatus euserStatus2 = EUserStatus.LoggedIn;
				return euserStatus.GetValueOrDefault() == euserStatus2 & euserStatus != null;
			}
		}

		// Token: 0x17001B75 RID: 7029
		// (get) Token: 0x0600DC50 RID: 56400 RVA: 0x004EF278 File Offset: 0x004ED478
		public bool Unavailable
		{
			get
			{
				bool flag = !this.IsReady;
				if (flag)
				{
					IPlatform platform = this.owner;
					EUserStatus? euserStatus;
					if (platform == null)
					{
						euserStatus = null;
					}
					else
					{
						IUserClient user = platform.User;
						euserStatus = ((user != null) ? new EUserStatus?(user.UserStatus) : null);
					}
					EUserStatus? euserStatus2 = euserStatus;
					bool flag2;
					if (euserStatus2 != null)
					{
						EUserStatus valueOrDefault = euserStatus2.GetValueOrDefault();
						if (valueOrDefault == EUserStatus.OfflineMode || valueOrDefault - EUserStatus.PermanentError <= 1)
						{
							flag2 = true;
							goto IL_64;
						}
					}
					flag2 = false;
					IL_64:
					flag = flag2;
				}
				return flag;
			}
		}

		// Token: 0x0600DC51 RID: 56401 RVA: 0x004EF2EC File Offset: 0x004ED4EC
		public void GetFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback)
		{
			if (_callback == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(_filename))
			{
				_callback(IRemoteFileStorage.EFileDownloadResult.EmptyFilename, null, null);
				return;
			}
			IUserClient user = this.owner.User;
			PlatformUserIdentifierAbs platformUserIdentifierAbs;
			if ((platformUserIdentifierAbs = ((user != null) ? user.PlatformUserId : null)) == null)
			{
				IUserClient userServer = this.owner.UserServer;
				platformUserIdentifierAbs = ((userServer != null) ? userServer.PlatformUserId : null);
			}
			UserIdentifierEos userIdentifierEos = (UserIdentifierEos)platformUserIdentifierAbs;
			ProductUserId localUserId = (userIdentifierEos != null) ? userIdentifierEos.ProductUserId : null;
			object obj = this.requestsLock;
			bool flag2;
			lock (obj)
			{
				RemoteFileStorage.RequestDetails requestDetails;
				flag2 = !this.requests.TryGetValue(_filename, out requestDetails);
				if (flag2)
				{
					Log.Out("[EOS] Created RFS Request: " + _filename);
					requestDetails = new RemoteFileStorage.RequestDetails(_filename, localUserId, _callback);
					this.requests.Add(_filename, requestDetails);
					this.requestQueue.Enqueue(requestDetails);
				}
				else
				{
					Log.Out("[EOS] Adding callback to existing RFS Request: " + _filename);
					requestDetails.Callback += _callback;
				}
			}
			if (flag2)
			{
				EosHelpers.AssertMainThread("RFS.Get");
				this.TryProcessNextRequest();
			}
		}

		// Token: 0x0600DC52 RID: 56402 RVA: 0x004EF3F8 File Offset: 0x004ED5F8
		[PublicizedFrom(EAccessModifier.Private)]
		public void TryProcessNextRequest()
		{
			object obj = this.requestsLock;
			lock (obj)
			{
				while (this.activeRequests < 16 && this.requestQueue.Count > 0)
				{
					RemoteFileStorage.RequestDetails requestDetails = this.requestQueue.Dequeue();
					if (!requestDetails.Cancelled)
					{
						this.activeRequests++;
						this.getMetadata(requestDetails);
					}
				}
			}
		}

		// Token: 0x0600DC53 RID: 56403 RVA: 0x004EF478 File Offset: 0x004ED678
		public bool CancelGetFile(string fileName, string cancelReason)
		{
			object lockObject = this.requestsLock;
			RemoteFileStorage.RequestDetails requestDetails;
			lock (lockObject)
			{
				if (!this.requests.TryGetValue(fileName, out requestDetails))
				{
					return false;
				}
				requestDetails.Cancelled = true;
			}
			if (requestDetails.Transfer != null)
			{
				lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = requestDetails.Transfer.CancelRequest();
				}
				if (result != Result.Success && result != Result.NoChange)
				{
					Log.Warning("[EOS] CancelRequest (" + fileName + ") returned: " + result.ToStringCached<Result>());
				}
			}
			this.CompleteRequest(requestDetails, IRemoteFileStorage.EFileDownloadResult.Other, cancelReason, null);
			return true;
		}

		// Token: 0x0600DC54 RID: 56404 RVA: 0x004EF544 File Offset: 0x004ED744
		[PublicizedFrom(EAccessModifier.Private)]
		public void getMetadata(RemoteFileStorage.RequestDetails _details)
		{
			QueryFileOptions queryFileOptions = new QueryFileOptions
			{
				Filename = _details.Filename,
				LocalUserId = _details.LocalUserId
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.titleStorageInterface.QueryFile(ref queryFileOptions, _details, new OnQueryFileCompleteCallback(this.queryFileCallback));
			}
		}

		// Token: 0x0600DC55 RID: 56405 RVA: 0x004EF5C0 File Offset: 0x004ED7C0
		[PublicizedFrom(EAccessModifier.Private)]
		public void queryFileCallback(ref QueryFileCallbackInfo _callbackData)
		{
			RemoteFileStorage.RequestDetails requestDetails = (RemoteFileStorage.RequestDetails)_callbackData.ClientData;
			if (requestDetails.Cancelled)
			{
				return;
			}
			if (_callbackData.ResultCode != Result.Success)
			{
				Log.Error("[EOS] QueryFile (" + requestDetails.Filename + ") failed: " + _callbackData.ResultCode.ToStringCached<Result>());
				this.CompleteRequest(requestDetails, IRemoteFileStorage.EFileDownloadResult.FileNotFound, _callbackData.ResultCode.ToStringCached<Result>(), null);
				return;
			}
			CopyFileMetadataByFilenameOptions copyFileMetadataByFilenameOptions = new CopyFileMetadataByFilenameOptions
			{
				Filename = requestDetails.Filename,
				LocalUserId = requestDetails.LocalUserId
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				FileMetadata? fileMetadata;
				result = this.titleStorageInterface.CopyFileMetadataByFilename(ref copyFileMetadataByFilenameOptions, out fileMetadata);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS] CopyFileMetadataByFilename (" + requestDetails.Filename + ") failed: " + result.ToStringCached<Result>());
				this.CompleteRequest(requestDetails, IRemoteFileStorage.EFileDownloadResult.Other, result.ToStringCached<Result>(), null);
				return;
			}
			this.readFile(requestDetails);
		}

		// Token: 0x0600DC56 RID: 56406 RVA: 0x004EF6C8 File Offset: 0x004ED8C8
		[PublicizedFrom(EAccessModifier.Private)]
		public void readFile(RemoteFileStorage.RequestDetails _details)
		{
			ReadFileOptions readFileOptions = new ReadFileOptions
			{
				Filename = _details.Filename,
				LocalUserId = _details.LocalUserId,
				ReadFileDataCallback = new OnReadFileDataCallback(this.readFileDataCallback),
				ReadChunkLengthBytes = 524288U
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				_details.Transfer = this.titleStorageInterface.ReadFile(ref readFileOptions, _details, new OnReadFileCompleteCallback(this.readCompletedCallback));
			}
		}

		// Token: 0x0600DC57 RID: 56407 RVA: 0x004EF76C File Offset: 0x004ED96C
		[PublicizedFrom(EAccessModifier.Private)]
		public void fileTransferProgressCallback(ref FileTransferProgressCallbackInfo _callbackData)
		{
			RemoteFileStorage.RequestDetails requestDetails = (RemoteFileStorage.RequestDetails)_callbackData.ClientData;
			Log.Out(string.Format("[EOS] TransferProgress: {0}, {1} / {2}", _callbackData.Filename, _callbackData.BytesTransferred, _callbackData.TotalFileSizeBytes));
		}

		// Token: 0x0600DC58 RID: 56408 RVA: 0x004EF7A5 File Offset: 0x004ED9A5
		[PublicizedFrom(EAccessModifier.Private)]
		public ReadResult readFileDataCallback(ref ReadFileDataCallbackInfo _callbackData)
		{
			((RemoteFileStorage.RequestDetails)_callbackData.ClientData).Chunks.Add(_callbackData.DataChunk);
			return ReadResult.RrContinueReading;
		}

		// Token: 0x0600DC59 RID: 56409 RVA: 0x004EF7C4 File Offset: 0x004ED9C4
		[PublicizedFrom(EAccessModifier.Private)]
		public void readCompletedCallback(ref ReadFileCallbackInfo _callbackData)
		{
			RemoteFileStorage.RequestDetails requestDetails = (RemoteFileStorage.RequestDetails)_callbackData.ClientData;
			if (requestDetails.Cancelled)
			{
				return;
			}
			if (_callbackData.ResultCode == Result.Success)
			{
				int num = 0;
				foreach (ArraySegment<byte> arraySegment in requestDetails.Chunks)
				{
					num += arraySegment.Count;
				}
				byte[] array = new byte[num];
				int num2 = 0;
				for (int i = 0; i < requestDetails.Chunks.Count; i++)
				{
					Array.Copy(requestDetails.Chunks[i].Array, requestDetails.Chunks[i].Offset, array, num2, requestDetails.Chunks[i].Count);
					num2 += requestDetails.Chunks[i].Count;
				}
				Log.Out(string.Format("[EOS] Read ({0}) completed: {1}, received {2} bytes", _callbackData.Filename, _callbackData.ResultCode, num));
				this.cacheFile(requestDetails.Filename, array);
				this.CompleteRequest(requestDetails, IRemoteFileStorage.EFileDownloadResult.Ok, null, array);
				return;
			}
			if (_callbackData.ResultCode == Result.TooManyRequests)
			{
				Log.Error(string.Concat(new string[]
				{
					"[EOS] Read (",
					requestDetails.Filename,
					") failed: ",
					_callbackData.ResultCode.ToStringCached<Result>(),
					". Try lowering the MaxConcurrentReads value."
				}));
				this.CompleteRequest(requestDetails, IRemoteFileStorage.EFileDownloadResult.Other, _callbackData.ResultCode.ToStringCached<Result>(), null);
				return;
			}
			if (_callbackData.ResultCode != Result.OperationWillRetry)
			{
				Log.Error("[EOS] Read (" + requestDetails.Filename + ") failed: " + _callbackData.ResultCode.ToStringCached<Result>());
				this.CompleteRequest(requestDetails, IRemoteFileStorage.EFileDownloadResult.Other, _callbackData.ResultCode.ToStringCached<Result>(), null);
			}
		}

		// Token: 0x0600DC5A RID: 56410 RVA: 0x004EF9A4 File Offset: 0x004EDBA4
		[PublicizedFrom(EAccessModifier.Private)]
		public void CompleteRequest(RemoteFileStorage.RequestDetails _details, IRemoteFileStorage.EFileDownloadResult _result, string _errorName, byte[] _data)
		{
			object obj = this.requestsLock;
			lock (obj)
			{
				RemoteFileStorage.RequestDetails requestDetails;
				if (!this.requests.TryGetValue(_details.Filename, out requestDetails) || requestDetails != _details)
				{
					Log.Out(string.Concat(new string[]
					{
						"[EOS] Ignoring completion of RFS request no longer tracked: ",
						_details.Filename,
						" (",
						_result.ToStringCached<IRemoteFileStorage.EFileDownloadResult>(),
						")"
					}));
					return;
				}
				this.requests.Remove(_details.Filename);
				this.activeRequests = Math.Max(0, this.activeRequests - 1);
				this.TryProcessNextRequest();
			}
			_details.ExecuteCallback(_result, _errorName, _data);
		}

		// Token: 0x17001B76 RID: 7030
		// (get) Token: 0x0600DC5B RID: 56411 RVA: 0x004EFA6C File Offset: 0x004EDC6C
		public string CacheFilePrefix
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return "Rfs_";
			}
		}

		// Token: 0x17001B77 RID: 7031
		// (get) Token: 0x0600DC5C RID: 56412 RVA: 0x004EFA73 File Offset: 0x004EDC73
		public string CacheFolder
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return GameIO.GetDeviceLocalUserGameDataDir() + "/RfsCache";
			}
		}

		// Token: 0x0600DC5D RID: 56413 RVA: 0x004EFA84 File Offset: 0x004EDC84
		[PublicizedFrom(EAccessModifier.Private)]
		public void clearCache()
		{
			if (!SdDirectory.Exists(this.CacheFolder))
			{
				return;
			}
			foreach (string path in SdDirectory.GetFiles(this.CacheFolder))
			{
				if (Path.GetFileName(path).StartsWith(this.CacheFilePrefix))
				{
					SdFile.Delete(path);
				}
			}
		}

		// Token: 0x0600DC5E RID: 56414 RVA: 0x004EFAD6 File Offset: 0x004EDCD6
		[PublicizedFrom(EAccessModifier.Private)]
		public void cacheFile(string _filename, byte[] _data)
		{
			SdDirectory.CreateDirectory(this.CacheFolder);
			SdFile.WriteAllBytes(this.CacheFolder + "/" + this.CacheFilePrefix + _filename, _data);
		}

		// Token: 0x0600DC5F RID: 56415 RVA: 0x004EFB04 File Offset: 0x004EDD04
		public void GetCachedFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback)
		{
			if (_callback == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(_filename))
			{
				_callback(IRemoteFileStorage.EFileDownloadResult.EmptyFilename, null, null);
				return;
			}
			string path = this.CacheFolder + "/" + this.CacheFilePrefix + _filename;
			if (!SdFile.Exists(path))
			{
				_callback(IRemoteFileStorage.EFileDownloadResult.FileNotFound, "File not found", null);
				return;
			}
			byte[] array = SdFile.ReadAllBytes(path);
			Log.Out(string.Format("[EOS] Read cached ({0}) completed: {1} bytes", _filename, array.Length));
			_callback(IRemoteFileStorage.EFileDownloadResult.Ok, null, array);
		}

		// Token: 0x0400A6CC RID: 42700
		public const string CacheFolderName = "RfsCache";

		// Token: 0x0400A6CD RID: 42701
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MaxConcurrentReads = 16;

		// Token: 0x0400A6CE RID: 42702
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A6CF RID: 42703
		[PublicizedFrom(EAccessModifier.Private)]
		public TitleStorageInterface titleStorageInterface;

		// Token: 0x0400A6D0 RID: 42704
		[PublicizedFrom(EAccessModifier.Private)]
		public object requestsLock = new object();

		// Token: 0x0400A6D1 RID: 42705
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, RemoteFileStorage.RequestDetails> requests = new Dictionary<string, RemoteFileStorage.RequestDetails>();

		// Token: 0x0400A6D2 RID: 42706
		[PublicizedFrom(EAccessModifier.Private)]
		public Queue<RemoteFileStorage.RequestDetails> requestQueue = new Queue<RemoteFileStorage.RequestDetails>();

		// Token: 0x0400A6D3 RID: 42707
		[PublicizedFrom(EAccessModifier.Private)]
		public int activeRequests;

		// Token: 0x02001D06 RID: 7430
		[PublicizedFrom(EAccessModifier.Private)]
		public class RequestDetails
		{
			// Token: 0x14000141 RID: 321
			// (add) Token: 0x0600DC61 RID: 56417 RVA: 0x004EFBA8 File Offset: 0x004EDDA8
			// (remove) Token: 0x0600DC62 RID: 56418 RVA: 0x004EFBE0 File Offset: 0x004EDDE0
			public event IRemoteFileStorage.FileDownloadCompleteCallback Callback;

			// Token: 0x0600DC63 RID: 56419 RVA: 0x004EFC15 File Offset: 0x004EDE15
			public RequestDetails(string _filename, ProductUserId _localUserId, IRemoteFileStorage.FileDownloadCompleteCallback _callback)
			{
				this.Filename = _filename;
				this.LocalUserId = _localUserId;
				this.Callback = _callback;
			}

			// Token: 0x0600DC64 RID: 56420 RVA: 0x004EFC3D File Offset: 0x004EDE3D
			public void ExecuteCallback(IRemoteFileStorage.EFileDownloadResult _result, string _errorName, byte[] _data)
			{
				IRemoteFileStorage.FileDownloadCompleteCallback callback = this.Callback;
				if (callback == null)
				{
					return;
				}
				callback(_result, _errorName, _data);
			}

			// Token: 0x0400A6D4 RID: 42708
			public readonly string Filename;

			// Token: 0x0400A6D5 RID: 42709
			public readonly ProductUserId LocalUserId;

			// Token: 0x0400A6D7 RID: 42711
			public readonly List<ArraySegment<byte>> Chunks = new List<ArraySegment<byte>>();

			// Token: 0x0400A6D8 RID: 42712
			public TitleStorageFileTransferRequest Transfer;

			// Token: 0x0400A6D9 RID: 42713
			public bool Cancelled;
		}
	}
}
