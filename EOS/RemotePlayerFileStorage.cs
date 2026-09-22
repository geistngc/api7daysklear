using System;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.PlayerDataStorage;

namespace Platform.EOS
{
	// Token: 0x02001D07 RID: 7431
	public class RemotePlayerFileStorage : IRemotePlayerFileStorage
	{
		// Token: 0x17001B78 RID: 7032
		// (get) Token: 0x0600DC65 RID: 56421 RVA: 0x004EFC54 File Offset: 0x004EDE54
		public bool IsReady
		{
			get
			{
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

		// Token: 0x0600DC66 RID: 56422 RVA: 0x004EFCA0 File Offset: 0x004EDEA0
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += this.OnClientApiInitialized;
		}

		// Token: 0x0600DC67 RID: 56423 RVA: 0x004EFCC5 File Offset: 0x004EDEC5
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnClientApiInitialized()
		{
			this.playerDataStorage = ((Api)this.owner.Api).PlatformInterface.GetPlayerDataStorageInterface();
		}

		// Token: 0x0600DC68 RID: 56424 RVA: 0x004EFCE8 File Offset: 0x004EDEE8
		[PublicizedFrom(EAccessModifier.Private)]
		public void ProcessNextOperation()
		{
			object obj = this.queueLock;
			lock (obj)
			{
				if (!this.queueProcessing)
				{
					RemotePlayerFileStorage.StorageOperation storageOperation;
					if (this.operationQueue.TryDequeue(out storageOperation))
					{
						this.queueProcessing = true;
						if (storageOperation.ToRead)
						{
							this.ProcessReadOperation(storageOperation.Filename, storageOperation.OverwriteCache, storageOperation.ReadCallback);
						}
						else
						{
							this.ProcessWriteOperation(storageOperation.Filename, storageOperation.Data, storageOperation.OverwriteCache, storageOperation.WriteCallback);
						}
					}
				}
			}
		}

		// Token: 0x0600DC69 RID: 56425 RVA: 0x004EFD84 File Offset: 0x004EDF84
		[PublicizedFrom(EAccessModifier.Private)]
		public void CompleteActiveOperation()
		{
			object obj = this.queueLock;
			lock (obj)
			{
				this.queueProcessing = false;
				this.ProcessNextOperation();
			}
		}

		// Token: 0x0600DC6A RID: 56426 RVA: 0x004EFDCC File Offset: 0x004EDFCC
		public void ReadRemoteData(string _filename, bool _overwriteCache, IRemotePlayerFileStorage.FileReadCompleteCallback _callback)
		{
			object obj = this.queueLock;
			lock (obj)
			{
				this.operationQueue.Enqueue(new RemotePlayerFileStorage.StorageOperation(_filename, _overwriteCache, _callback));
				this.ProcessNextOperation();
			}
		}

		// Token: 0x0600DC6B RID: 56427 RVA: 0x004EFE20 File Offset: 0x004EE020
		[PublicizedFrom(EAccessModifier.Private)]
		public void ProcessReadOperation(string _filename, bool _overwriteCache, IRemotePlayerFileStorage.FileReadCompleteCallback _callback)
		{
			if (_callback == null)
			{
				Log.Warning("[EOS] PlayerDataStorage Read Operation failed as no callback supplied.");
				this.CompleteActiveOperation();
				return;
			}
			if (!this.IsReady)
			{
				Log.Warning("[EOS] Tried to read from PlayerDataStorage user is not logged in.");
				_callback(IRemotePlayerFileStorage.CallbackResult.NoConnection, null);
				this.CompleteActiveOperation();
				return;
			}
			if (this.playerDataStorage == null)
			{
				Log.Warning("[EOS] Tried to read from PlayerDataStorage but it was null.");
				_callback(IRemotePlayerFileStorage.CallbackResult.NoConnection, null);
				this.CompleteActiveOperation();
				return;
			}
			if (string.IsNullOrEmpty(_filename))
			{
				Log.Warning("[EOS] Supplied filename was null or empty.");
				_callback(IRemotePlayerFileStorage.CallbackResult.FileNotFound, null);
				this.CompleteActiveOperation();
				return;
			}
			ProductUserId productUserId = ((UserIdentifierEos)PlatformManager.CrossplatformPlatform.User.PlatformUserId).ProductUserId;
			RemotePlayerFileStorage.ReadRequestDetails clientData = new RemotePlayerFileStorage.ReadRequestDetails(_callback, _overwriteCache);
			ReadFileOptions readFileOptions = new ReadFileOptions
			{
				Filename = _filename,
				LocalUserId = productUserId,
				ReadChunkLengthBytes = 524288U,
				ReadFileDataCallback = new OnReadFileDataCallback(this.ReadChunkCallback)
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.playerDataStorage.ReadFile(ref readFileOptions, clientData, new OnReadFileCompleteCallback(this.ReadFileCompleteCallback));
			}
		}

		// Token: 0x0600DC6C RID: 56428 RVA: 0x004EFF58 File Offset: 0x004EE158
		[PublicizedFrom(EAccessModifier.Private)]
		public ReadResult ReadChunkCallback(ref ReadFileDataCallbackInfo _callbackData)
		{
			((RemotePlayerFileStorage.ReadRequestDetails)_callbackData.ClientData).Chunks.Add(_callbackData.DataChunk);
			return ReadResult.ContinueReading;
		}

		// Token: 0x0600DC6D RID: 56429 RVA: 0x004EFF78 File Offset: 0x004EE178
		[PublicizedFrom(EAccessModifier.Private)]
		public void ReadFileCompleteCallback(ref ReadFileCallbackInfo _callbackData)
		{
			try
			{
				RemotePlayerFileStorage.ReadRequestDetails readRequestDetails = (RemotePlayerFileStorage.ReadRequestDetails)_callbackData.ClientData;
				if (_callbackData.ResultCode != Result.Success)
				{
					if (_callbackData.ResultCode == Result.NotFound)
					{
						readRequestDetails.Callback(IRemotePlayerFileStorage.CallbackResult.FileNotFound, null);
					}
					else if (_callbackData.ResultCode != Result.OperationWillRetry)
					{
						Log.Warning(string.Format("[EOS] Read from PlayerDataStorage failed ({0}): {1}", _callbackData.Filename, _callbackData.ResultCode.ToStringCached<Result>()));
						readRequestDetails.Callback(IRemotePlayerFileStorage.CallbackResult.Other, null);
					}
				}
				else
				{
					int num = 0;
					foreach (ArraySegment<byte> arraySegment in readRequestDetails.Chunks)
					{
						num += arraySegment.Count;
					}
					byte[] array = new byte[num];
					int num2 = 0;
					for (int i = 0; i < readRequestDetails.Chunks.Count; i++)
					{
						Array.Copy(readRequestDetails.Chunks[i].Array, readRequestDetails.Chunks[i].Offset, array, num2, readRequestDetails.Chunks[i].Count);
						num2 += readRequestDetails.Chunks[i].Count;
					}
					if (readRequestDetails.OverwriteCache)
					{
						IRemotePlayerFileStorage.WriteCachedObject(this.owner.User, _callbackData.Filename, array);
					}
					Log.Out(string.Format("[EOS] Read ({0}) completed: {1}, received {2} bytes", _callbackData.Filename, _callbackData.ResultCode, num));
					readRequestDetails.Callback(IRemotePlayerFileStorage.CallbackResult.Success, array);
				}
			}
			finally
			{
				this.CompleteActiveOperation();
			}
		}

		// Token: 0x0600DC6E RID: 56430 RVA: 0x004F0148 File Offset: 0x004EE348
		public void WriteRemoteData(string _filename, byte[] _data, bool _overwriteCache, IRemotePlayerFileStorage.FileWriteCompleteCallback _callback)
		{
			object obj = this.queueLock;
			lock (obj)
			{
				this.operationQueue.Enqueue(new RemotePlayerFileStorage.StorageOperation(_filename, _data, _overwriteCache, _callback));
				this.ProcessNextOperation();
			}
		}

		// Token: 0x0600DC6F RID: 56431 RVA: 0x004F01A0 File Offset: 0x004EE3A0
		public void ProcessWriteOperation(string _filename, byte[] _data, bool _overwriteCache, IRemotePlayerFileStorage.FileWriteCompleteCallback _callback)
		{
			if (_callback == null)
			{
				Log.Warning("[EOS] PlayerDataStorage Write Operation failed as no callback supplied.");
				this.CompleteActiveOperation();
				return;
			}
			if (!this.IsReady)
			{
				Log.Warning("[EOS] Tried to write to PlayerDataStorage user is not logged in.");
				this.CompleteActiveOperation();
				_callback(IRemotePlayerFileStorage.CallbackResult.NoConnection);
				return;
			}
			if (this.playerDataStorage == null)
			{
				Log.Warning("[EOS] Tried to write to PlayerDataStorage but it was null.");
				this.CompleteActiveOperation();
				_callback(IRemotePlayerFileStorage.CallbackResult.NoConnection);
				return;
			}
			if (string.IsNullOrEmpty(_filename))
			{
				Log.Warning("[EOS] Supplied filename was null or empty.");
				this.CompleteActiveOperation();
				_callback(IRemotePlayerFileStorage.CallbackResult.FileNotFound);
				return;
			}
			if (_data == null)
			{
				Log.Warning("[EOS] Supplied data to store was null.");
				this.CompleteActiveOperation();
				_callback(IRemotePlayerFileStorage.CallbackResult.MalformedData);
				return;
			}
			RemotePlayerFileStorage.WriteRequestDetails clientData = new RemotePlayerFileStorage.WriteRequestDetails(_overwriteCache, _data, _callback);
			ProductUserId productUserId = ((UserIdentifierEos)PlatformManager.CrossplatformPlatform.User.PlatformUserId).ProductUserId;
			WriteFileOptions writeFileOptions = new WriteFileOptions
			{
				Filename = _filename,
				LocalUserId = productUserId,
				ChunkLengthBytes = 524288U,
				WriteFileDataCallback = new OnWriteFileDataCallback(this.WriteFileChunkCallback)
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.playerDataStorage.WriteFile(ref writeFileOptions, clientData, new OnWriteFileCompleteCallback(this.WriteFileCompleteCallback));
			}
		}

		// Token: 0x0600DC70 RID: 56432 RVA: 0x004F02F8 File Offset: 0x004EE4F8
		[PublicizedFrom(EAccessModifier.Private)]
		public WriteResult WriteFileChunkCallback(ref WriteFileDataCallbackInfo _callbackData, out ArraySegment<byte> _outDataBuffer)
		{
			RemotePlayerFileStorage.WriteRequestDetails writeRequestDetails = (RemotePlayerFileStorage.WriteRequestDetails)_callbackData.ClientData;
			_outDataBuffer = (writeRequestDetails.GetNextChunk() ?? null);
			if (writeRequestDetails.HasNextChunk())
			{
				return WriteResult.ContinueWriting;
			}
			return WriteResult.CompleteRequest;
		}

		// Token: 0x0600DC71 RID: 56433 RVA: 0x004F0344 File Offset: 0x004EE544
		[PublicizedFrom(EAccessModifier.Private)]
		public void WriteFileCompleteCallback(ref WriteFileCallbackInfo _callbackData)
		{
			try
			{
				RemotePlayerFileStorage.WriteRequestDetails writeRequestDetails = (RemotePlayerFileStorage.WriteRequestDetails)_callbackData.ClientData;
				IRemotePlayerFileStorage.CallbackResult result = IRemotePlayerFileStorage.CallbackResult.Success;
				if (_callbackData.ResultCode != Result.Success && _callbackData.ResultCode != Result.OperationWillRetry)
				{
					Log.Warning(string.Format("[EOS] Write to PlayerDataStorage failed ({0}): {1}", _callbackData.Filename, _callbackData.ResultCode.ToStringCached<Result>()));
					result = IRemotePlayerFileStorage.CallbackResult.Other;
				}
				if (writeRequestDetails.WriteToCache && !IRemotePlayerFileStorage.WriteCachedObject(this.owner.User, _callbackData.Filename, writeRequestDetails.Data))
				{
					Log.Warning(string.Format("[EOS] Write to PlayerDataStorage succeeded ({0}), but failed while saving to local cache.", _callbackData.Filename));
				}
				IRemotePlayerFileStorage.FileWriteCompleteCallback callback = writeRequestDetails.Callback;
				if (callback != null)
				{
					callback(result);
				}
			}
			finally
			{
				this.CompleteActiveOperation();
			}
		}

		// Token: 0x0400A6DA RID: 42714
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cReadWriteByteLimit = 524288;

		// Token: 0x0400A6DB RID: 42715
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A6DC RID: 42716
		[PublicizedFrom(EAccessModifier.Private)]
		public PlayerDataStorageInterface playerDataStorage;

		// Token: 0x0400A6DD RID: 42717
		[PublicizedFrom(EAccessModifier.Private)]
		public Queue<RemotePlayerFileStorage.StorageOperation> operationQueue = new Queue<RemotePlayerFileStorage.StorageOperation>();

		// Token: 0x0400A6DE RID: 42718
		[PublicizedFrom(EAccessModifier.Private)]
		public object queueLock = new object();

		// Token: 0x0400A6DF RID: 42719
		[PublicizedFrom(EAccessModifier.Private)]
		public bool queueProcessing;

		// Token: 0x02001D08 RID: 7432
		[PublicizedFrom(EAccessModifier.Private)]
		public struct StorageOperation
		{
			// Token: 0x0600DC73 RID: 56435 RVA: 0x004F041E File Offset: 0x004EE61E
			public StorageOperation(string _filename, bool _overwriteCache, IRemotePlayerFileStorage.FileReadCompleteCallback _callback)
			{
				this.ToRead = true;
				this.Filename = _filename;
				this.OverwriteCache = _overwriteCache;
				this.Data = null;
				this.ReadCallback = _callback;
				this.WriteCallback = null;
			}

			// Token: 0x0600DC74 RID: 56436 RVA: 0x004F044A File Offset: 0x004EE64A
			public StorageOperation(string _filename, byte[] _data, bool _overwriteCache, IRemotePlayerFileStorage.FileWriteCompleteCallback _callback)
			{
				this.ToRead = false;
				this.Filename = _filename;
				this.Data = _data;
				this.OverwriteCache = _overwriteCache;
				this.ReadCallback = null;
				this.WriteCallback = _callback;
			}

			// Token: 0x0400A6E0 RID: 42720
			public bool ToRead;

			// Token: 0x0400A6E1 RID: 42721
			public string Filename;

			// Token: 0x0400A6E2 RID: 42722
			public bool OverwriteCache;

			// Token: 0x0400A6E3 RID: 42723
			public byte[] Data;

			// Token: 0x0400A6E4 RID: 42724
			public IRemotePlayerFileStorage.FileReadCompleteCallback ReadCallback;

			// Token: 0x0400A6E5 RID: 42725
			public IRemotePlayerFileStorage.FileWriteCompleteCallback WriteCallback;
		}

		// Token: 0x02001D09 RID: 7433
		[PublicizedFrom(EAccessModifier.Private)]
		public class ReadRequestDetails
		{
			// Token: 0x0600DC75 RID: 56437 RVA: 0x004F0477 File Offset: 0x004EE677
			public ReadRequestDetails(IRemotePlayerFileStorage.FileReadCompleteCallback _callback, bool _overwriteCache)
			{
				this.Callback = _callback;
				this.OverwriteCache = _overwriteCache;
				this.Chunks = new List<ArraySegment<byte>>();
			}

			// Token: 0x0400A6E6 RID: 42726
			public readonly IRemotePlayerFileStorage.FileReadCompleteCallback Callback;

			// Token: 0x0400A6E7 RID: 42727
			public List<ArraySegment<byte>> Chunks;

			// Token: 0x0400A6E8 RID: 42728
			public bool OverwriteCache;
		}

		// Token: 0x02001D0A RID: 7434
		[PublicizedFrom(EAccessModifier.Private)]
		public class WriteRequestDetails
		{
			// Token: 0x0600DC76 RID: 56438 RVA: 0x004F0498 File Offset: 0x004EE698
			public WriteRequestDetails(bool _writeToCache, byte[] _data, IRemotePlayerFileStorage.FileWriteCompleteCallback _callback)
			{
				this.WriteToCache = _writeToCache;
				this.Data = _data;
				this.Callback = _callback;
			}

			// Token: 0x0600DC77 RID: 56439 RVA: 0x004F04B8 File Offset: 0x004EE6B8
			public ArraySegment<byte>? GetNextChunk()
			{
				if (this.DataPointer >= this.Data.Length)
				{
					return null;
				}
				int val = this.Data.Length - this.DataPointer;
				int num = Math.Min(524288, val);
				ArraySegment<byte> value = new ArraySegment<byte>(this.Data, this.DataPointer, num);
				this.DataPointer += num;
				return new ArraySegment<byte>?(value);
			}

			// Token: 0x0600DC78 RID: 56440 RVA: 0x004F0520 File Offset: 0x004EE720
			public bool HasNextChunk()
			{
				return this.DataPointer < this.Data.Length;
			}

			// Token: 0x0400A6E9 RID: 42729
			public bool WriteToCache;

			// Token: 0x0400A6EA RID: 42730
			public byte[] Data;

			// Token: 0x0400A6EB RID: 42731
			public readonly IRemotePlayerFileStorage.FileWriteCompleteCallback Callback;

			// Token: 0x0400A6EC RID: 42732
			[PublicizedFrom(EAccessModifier.Private)]
			public int DataPointer;
		}
	}
}
