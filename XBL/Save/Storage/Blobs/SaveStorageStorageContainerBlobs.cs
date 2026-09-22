using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL.Save.Storage.Blobs
{
	// Token: 0x02001C58 RID: 7256
	public sealed class SaveStorageStorageContainerBlobs : ISaveStorageContainer, IDisposable
	{
		// Token: 0x0600D6FD RID: 55037 RVA: 0x004D8D4E File Offset: 0x004D6F4E
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogWarning(string text)
		{
			Log.Warning("[XBL: SaveStorageStorageContainerBlobs] " + text);
		}

		// Token: 0x0600D6FE RID: 55038 RVA: 0x004D8D60 File Offset: 0x004D6F60
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogError(string text)
		{
			Log.Error("[XBL: SaveStorageStorageContainerBlobs] " + text);
		}

		// Token: 0x0600D6FF RID: 55039 RVA: 0x004D8D72 File Offset: 0x004D6F72
		[Conditional("DEBUG_SAVE_DATA_MANAGER")]
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogTrace(string text)
		{
			Log.Out("[XBL: SaveStorageStorageContainerBlobs] " + text);
		}

		// Token: 0x0600D700 RID: 55040 RVA: 0x004D8D84 File Offset: 0x004D6F84
		public SaveStorageStorageContainerBlobs(string containerName, XGameSaveProviderHandle gameSaveProviderHandle, SingleThreadTaskScheduler taskScheduler)
		{
			using (this.m_lastAccessHelper.CreateScope())
			{
				bool flag = false;
				try
				{
					this.m_containerName = containerName;
					this.m_taskScheduler = taskScheduler;
					this.m_gameSaveProviderHandle = gameSaveProviderHandle;
					this.m_operationsInput = new ConcurrentQueue<SaveStorageStorageContainerBlobs.BlobOperation>();
					this.m_operationsInputLatest = new ConcurrentDictionary<string, SaveStorageStorageContainerBlobs.BlobOperation>();
					this.m_processOperationsToDo = new List<SaveStorageStorageContainerBlobs.BlobOperation>();
					this.m_processOperationsCurrent = new Dictionary<string, SaveStorageStorageContainerBlobs.BlobOperation>();
					this.m_processOperationsTaskWaitSkipTaskSource = new TaskCompletionSource<bool>();
					this.m_processOperationsTaskCancellationTokenProvider = new CancellationTokenSource();
					if (!Unity.XGamingRuntime.Interop.HR.FAILED(this.m_taskScheduler.ExecuteAndWait<int>(() => SDK.XGameSaveCreateContainer(this.m_gameSaveProviderHandle, this.m_containerName, out this.m_gameSaveContainerHandle))))
					{
						flag = true;
					}
				}
				finally
				{
					if (!flag)
					{
						this.Dispose();
					}
				}
			}
		}

		// Token: 0x0600D701 RID: 55041 RVA: 0x004D8E6C File Offset: 0x004D706C
		public void Dispose()
		{
			this.m_disposed = true;
			try
			{
				CancellationTokenSource processOperationsTaskCancellationTokenProvider = this.m_processOperationsTaskCancellationTokenProvider;
				if (processOperationsTaskCancellationTokenProvider != null)
				{
					processOperationsTaskCancellationTokenProvider.Cancel();
				}
			}
			catch (AggregateException arg)
			{
				SaveStorageStorageContainerBlobs.LogError(string.Format("Error while cancelling process operations task: {0}", arg));
			}
			TaskCompletionSource<bool> processOperationsTaskWaitSkipTaskSource = this.m_processOperationsTaskWaitSkipTaskSource;
			if (processOperationsTaskWaitSkipTaskSource != null)
			{
				processOperationsTaskWaitSkipTaskSource.SetResult(true);
			}
			this.m_processOperationsTaskWaitSkipTaskSource = null;
			object processOperationsTaskLock = this.m_processOperationsTaskLock;
			lock (processOperationsTaskLock)
			{
				if (this.m_processOperationsTask != null)
				{
					try
					{
						this.m_processOperationsTask.Wait();
					}
					catch (AggregateException arg2)
					{
						SaveStorageStorageContainerBlobs.LogError(string.Format("Error while waiting for process operations task to complete: {0}", arg2));
					}
					this.m_processOperationsTask = null;
				}
			}
			CancellationTokenSource processOperationsTaskCancellationTokenProvider2 = this.m_processOperationsTaskCancellationTokenProvider;
			if (processOperationsTaskCancellationTokenProvider2 != null)
			{
				processOperationsTaskCancellationTokenProvider2.Dispose();
			}
			this.m_processOperationsTaskCancellationTokenProvider = null;
			this.m_processOperationsCurrent = null;
			this.m_processOperationsToDo = null;
			this.m_operationsInputLatest = null;
			this.m_operationsInput = null;
			if (this.m_gameSaveContainerHandle != null)
			{
				this.m_taskScheduler.ExecuteAndWait(delegate()
				{
					SDK.XGameSaveCloseContainer(this.m_gameSaveContainerHandle);
				});
				this.m_gameSaveContainerHandle = null;
			}
			this.m_gameSaveProviderHandle = null;
			this.m_taskScheduler = null;
		}

		// Token: 0x17001AA0 RID: 6816
		// (get) Token: 0x0600D702 RID: 55042 RVA: 0x004D8FA4 File Offset: 0x004D71A4
		public bool IsDisposed
		{
			get
			{
				return this.m_disposed;
			}
		}

		// Token: 0x17001AA1 RID: 6817
		// (get) Token: 0x0600D703 RID: 55043 RVA: 0x004D8FAC File Offset: 0x004D71AC
		public string Name
		{
			get
			{
				return this.m_containerName;
			}
		}

		// Token: 0x17001AA2 RID: 6818
		// (get) Token: 0x0600D704 RID: 55044 RVA: 0x004D8FB4 File Offset: 0x004D71B4
		public DateTime LastAccessed
		{
			get
			{
				return this.m_lastAccessHelper.Time;
			}
		}

		// Token: 0x0600D705 RID: 55045 RVA: 0x004D8FC4 File Offset: 0x004D71C4
		public void Flush(bool waitForFlush)
		{
			Task task = this.StartProcessOperationsTask();
			if (task.IsCompleted)
			{
				return;
			}
			this.m_processOperationsTaskWaitSkipTaskSource.SetResult(true);
			this.m_processOperationsTaskWaitSkipTaskSource = new TaskCompletionSource<bool>();
			if (waitForFlush)
			{
				try
				{
					task.Wait();
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x0600D706 RID: 55046 RVA: 0x004D9018 File Offset: 0x004D7218
		public bool TryEnumerateBlobInfos(out Unity.XGamingRuntime.XGameSaveBlobInfo[] blobInfos)
		{
			bool result;
			using (this.m_lastAccessHelper.CreateScope())
			{
				Unity.XGamingRuntime.XGameSaveBlobInfo[] blobInfosTemp = null;
				int hr = this.m_taskScheduler.ExecuteAndWait<int>(() => SDK.XGameSaveEnumerateBlobInfo(this.m_gameSaveContainerHandle, out blobInfosTemp));
				if (this.m_operationsInputLatest.Count <= 0)
				{
					blobInfos = blobInfosTemp;
				}
				else
				{
					Dictionary<string, Unity.XGamingRuntime.XGameSaveBlobInfo> dictionary = blobInfosTemp.ToDictionary((Unity.XGamingRuntime.XGameSaveBlobInfo blobInfo) => blobInfo.Name);
					foreach (KeyValuePair<string, SaveStorageStorageContainerBlobs.BlobOperation> keyValuePair in this.m_operationsInputLatest)
					{
						string text;
						SaveStorageStorageContainerBlobs.BlobOperation blobOperation;
						keyValuePair.Deconstruct(out text, out blobOperation);
						string text2 = text;
						SaveStorageStorageContainerBlobs.BlobOperation blobOperation2 = blobOperation;
						using (RefCountedBuffer refCountedBuffer = blobOperation2.CreateRef())
						{
							if (refCountedBuffer == null)
							{
								if (!blobOperation2.IsDisposed)
								{
									dictionary.Remove(text2);
								}
							}
							else
							{
								uint length = (uint)refCountedBuffer.Length;
								if (!refCountedBuffer.IsDisposed)
								{
									dictionary[text2] = new Unity.XGamingRuntime.XGameSaveBlobInfo
									{
										Name = text2,
										Size = length
									};
								}
							}
						}
					}
					blobInfos = dictionary.Values.ToArray<Unity.XGamingRuntime.XGameSaveBlobInfo>();
				}
				result = Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr);
			}
			return result;
		}

		// Token: 0x0600D707 RID: 55047 RVA: 0x004D91C0 File Offset: 0x004D73C0
		public Unity.XGamingRuntime.XGameSaveBlobInfo GetBlobInfo(string blobName)
		{
			Unity.XGamingRuntime.XGameSaveBlobInfo result;
			using (this.m_lastAccessHelper.CreateScope())
			{
				SaveStorageStorageContainerBlobs.BlobOperation blobOperation;
				if (this.m_operationsInputLatest.TryGetValue(blobName, out blobOperation))
				{
					using (RefCountedBuffer refCountedBuffer = blobOperation.CreateRef())
					{
						if (refCountedBuffer == null)
						{
							if (blobOperation.IsDisposed)
							{
								return null;
							}
						}
						else
						{
							uint length = (uint)refCountedBuffer.Length;
							if (!refCountedBuffer.IsDisposed)
							{
								return new Unity.XGamingRuntime.XGameSaveBlobInfo
								{
									Name = blobName,
									Size = length
								};
							}
						}
					}
				}
				Unity.XGamingRuntime.XGameSaveBlobInfo[] blobInfos = null;
				int hr = this.m_taskScheduler.ExecuteAndWait<int>(() => SDK.XGameSaveEnumerateBlobInfoByName(this.m_gameSaveContainerHandle, blobName, out blobInfos));
				if (Unity.XGamingRuntime.Interop.HR.FAILED(hr))
				{
					GameCoreSaveHelpers.NonTraceLogHR(hr, string.Concat(new string[]
					{
						"Enumerate Blobs with prefix '",
						blobName,
						"' in Container '",
						this.m_containerName,
						"'."
					}));
					result = null;
				}
				else
				{
					foreach (Unity.XGamingRuntime.XGameSaveBlobInfo xgameSaveBlobInfo in blobInfos)
					{
						if (xgameSaveBlobInfo.Name == blobName)
						{
							return xgameSaveBlobInfo;
						}
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600D708 RID: 55048 RVA: 0x004D934C File Offset: 0x004D754C
		public RefCountedBuffer[] GetBlobs(string[] blobNames, StringSpan debugIdentifier)
		{
			RefCountedBuffer[] result;
			using (this.m_lastAccessHelper.CreateScope())
			{
				string text = null;
				string text2 = null;
				int num = 0;
				Dictionary<string, RefCountedBuffer> blobNamesToBuffer = new Dictionary<string, RefCountedBuffer>();
				bool flag = false;
				try
				{
					if (this.m_operationsInputLatest.Count > 0)
					{
						foreach (string text3 in blobNames)
						{
							SaveStorageStorageContainerBlobs.BlobOperation blobOperation;
							if (this.m_operationsInputLatest.TryGetValue(text3, out blobOperation))
							{
								if (blobOperation.Delete)
								{
									throw new IOException("Failed to read deleted blob named '" + text3 + "'.");
								}
								RefCountedBuffer refCountedBuffer = blobOperation.CreateRef();
								if (refCountedBuffer != null && !refCountedBuffer.IsDisposed)
								{
									blobNamesToBuffer[text3] = refCountedBuffer;
								}
							}
						}
					}
					int count = blobNamesToBuffer.Count;
					string[] blobNamesToRead = (from blobName in blobNames
					where !blobNamesToBuffer.ContainsKey(blobName)
					select blobName).ToArray<string>();
					if (blobNamesToRead.Length != 0)
					{
						Stopwatch stopwatch = new Stopwatch();
						XGameSaveBlob[] blobs = null;
						for (;;)
						{
							using (ManualResetEventSlim done = new ManualResetEventSlim())
							{
								int hr = -1;
								XGameSaveReadBlobDataCompleted <>9__3;
								this.m_taskScheduler.ExecuteAndWait<int>(delegate()
								{
									XGameSaveContainerHandle gameSaveContainerHandle = this.m_gameSaveContainerHandle;
									string[] blobNamesToRead = blobNamesToRead;
									XGameSaveReadBlobDataCompleted onCompleted;
									if ((onCompleted = <>9__3) == null)
									{
										onCompleted = (<>9__3 = delegate(int hresult, XGameSaveBlob[] saveBlobs)
										{
											hr = hresult;
											blobs = saveBlobs;
											done.Set();
										});
									}
									return SDK.XGameSaveReadBlobDataAsync(gameSaveContainerHandle, blobNamesToRead, onCompleted);
								});
								stopwatch.Start();
								while (!done.IsSet)
								{
									done.Wait(SaveStorageStorageContainerBlobs.GetBlobsWaitReportingInterval);
									TimeSpan elapsed = stopwatch.Elapsed;
									if (elapsed >= SaveStorageStorageContainerBlobs.GetBlobsWaitReportingInterval)
									{
										string format = "Read Blob(s) named '{0}' for '{1}' in Container '{2}' has been waiting for {3:F3} s.";
										object[] array = new object[4];
										int num2 = 0;
										string text4;
										if ((text4 = text) == null)
										{
											text4 = (text = string.Join("', '", blobNamesToRead));
										}
										array[num2] = text4;
										int num3 = 1;
										string text5;
										if ((text5 = text2) == null)
										{
											text5 = (text2 = debugIdentifier.ToString());
										}
										array[num3] = text5;
										array[2] = this.m_containerName;
										array[3] = elapsed.TotalSeconds;
										SaveStorageStorageContainerBlobs.LogWarning(string.Format(format, array));
									}
								}
								stopwatch.Stop();
								if (!Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
								{
									if (num >= 10 || (hr != -2147024638 && hr != -2147024882))
									{
										int hr2 = hr;
										string[] array2 = new string[7];
										array2[0] = "Read Blob(s) named '";
										int num4 = 1;
										string text6;
										if ((text6 = text) == null)
										{
											text6 = (text = string.Join("', '", blobNamesToRead));
										}
										array2[num4] = text6;
										array2[2] = "' for '";
										int num5 = 3;
										string text7;
										if ((text7 = text2) == null)
										{
											text7 = (text2 = debugIdentifier.ToString());
										}
										array2[num5] = text7;
										array2[4] = "' in Container '";
										array2[5] = this.m_containerName;
										array2[6] = "'.";
										GameCoreSaveHelpers.NonTraceLogHR(hr2, string.Concat(array2));
										throw new IOException("Failed to read blob(s).");
									}
									num++;
									continue;
								}
							}
							break;
						}
						foreach (XGameSaveBlob xgameSaveBlob in blobs)
						{
							blobNamesToBuffer[xgameSaveBlob.Info.Name] = RefCountedBuffer.CreateFromExisting(xgameSaveBlob.Data);
						}
					}
					RefCountedBuffer[] array3 = (from blobName in blobNames
					select blobNamesToBuffer[blobName]).ToArray<RefCountedBuffer>();
					flag = true;
					result = array3;
				}
				finally
				{
					if (!flag)
					{
						foreach (RefCountedBuffer refCountedBuffer2 in blobNamesToBuffer.Values)
						{
							if (refCountedBuffer2 != null)
							{
								refCountedBuffer2.Dispose();
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600D709 RID: 55049 RVA: 0x004D9768 File Offset: 0x004D7968
		public void SetBlob(string blobName, RefCountedBuffer blobData)
		{
			using (this.m_lastAccessHelper.CreateScope())
			{
				this.EnqueueBlobOperation(new SaveStorageStorageContainerBlobs.BlobOperation(blobName, blobData));
			}
		}

		// Token: 0x0600D70A RID: 55050 RVA: 0x004D97B0 File Offset: 0x004D79B0
		public void DeleteBlob(string blobName)
		{
			using (this.m_lastAccessHelper.CreateScope())
			{
				this.EnqueueBlobOperation(new SaveStorageStorageContainerBlobs.BlobOperation(blobName, null));
			}
		}

		// Token: 0x0600D70B RID: 55051 RVA: 0x004D97F8 File Offset: 0x004D79F8
		public int GetQueuedUsed()
		{
			int num = 0;
			foreach (SaveStorageStorageContainerBlobs.BlobOperation blobOperation in this.m_operationsInputLatest.Values)
			{
				using (RefCountedBuffer refCountedBuffer = blobOperation.CreateRef())
				{
					if (refCountedBuffer != null)
					{
						num += refCountedBuffer.Length;
					}
				}
			}
			return num;
		}

		// Token: 0x0600D70C RID: 55052 RVA: 0x004D9870 File Offset: 0x004D7A70
		[PublicizedFrom(EAccessModifier.Private)]
		public Task StartProcessOperationsTask()
		{
			object processOperationsTaskLock = this.m_processOperationsTaskLock;
			Task result;
			lock (processOperationsTaskLock)
			{
				Task processOperationsTask = this.m_processOperationsTask;
				if (processOperationsTask != null)
				{
					result = processOperationsTask;
				}
				else if (this.m_operationsInput.Count <= 0)
				{
					result = Task.CompletedTask;
				}
				else
				{
					CancellationToken cancellationToken = this.m_processOperationsTaskCancellationTokenProvider.Token;
					Task waitSkipTask = this.m_processOperationsTaskWaitSkipTaskSource.Task;
					Task task = this.m_taskScheduler.Factory.StartNew<Task>(() => this.ProcessOperations(cancellationToken, waitSkipTask), cancellationToken).Unwrap();
					this.m_processOperationsTask = task;
					result = task;
				}
			}
			return result;
		}

		// Token: 0x0600D70D RID: 55053 RVA: 0x004D9938 File Offset: 0x004D7B38
		[PublicizedFrom(EAccessModifier.Private)]
		public void EnqueueBlobOperation(SaveStorageStorageContainerBlobs.BlobOperation operation)
		{
			if (LaunchPrefs.GameCoreBlobOperationQueueMaxTotalSize.Value > 0 && this.m_operationsTotalSize > LaunchPrefs.GameCoreBlobOperationQueueMaxTotalSize.Value)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				TimeSpan t = TimeSpan.Zero;
				while (this.m_operationsTotalSize > LaunchPrefs.GameCoreBlobOperationQueueMaxTotalSize.Value)
				{
					Thread.Sleep(50);
					if (stopwatch.Elapsed > t + SaveStorageStorageContainerBlobs.EnqueueBlobOperationWaitReportingInterval)
					{
						t += SaveStorageStorageContainerBlobs.EnqueueBlobOperationWaitReportingInterval;
						SaveStorageStorageContainerBlobs.LogWarning(string.Format("Waiting for blob operations to be processed to free up space for {0:F3}s. Total size: {1}.", t.TotalSeconds, this.m_operationsTotalSize.FormatSize(true)));
					}
				}
			}
			Interlocked.Add(ref this.m_operationsTotalSize, operation.Size);
			this.m_operationsInputLatest[operation.Name] = operation;
			this.m_operationsInput.Enqueue(operation);
			this.StartProcessOperationsTask();
		}

		// Token: 0x0600D70E RID: 55054 RVA: 0x004D9A18 File Offset: 0x004D7C18
		[PublicizedFrom(EAccessModifier.Private)]
		public Task ProcessOperations(CancellationToken cancellationToken, Task waitSkipTask)
		{
			SaveStorageStorageContainerBlobs.<ProcessOperations>d__40 <ProcessOperations>d__;
			<ProcessOperations>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessOperations>d__.<>4__this = this;
			<ProcessOperations>d__.cancellationToken = cancellationToken;
			<ProcessOperations>d__.waitSkipTask = waitSkipTask;
			<ProcessOperations>d__.<>1__state = -1;
			<ProcessOperations>d__.<>t__builder.Start<SaveStorageStorageContainerBlobs.<ProcessOperations>d__40>(ref <ProcessOperations>d__);
			return <ProcessOperations>d__.<>t__builder.Task;
		}

		// Token: 0x0600D712 RID: 55058 RVA: 0x004D9ACC File Offset: 0x004D7CCC
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <ProcessOperations>g__DoSingleUpdate|40_0()
		{
			using (this.m_lastAccessHelper.CreateScope())
			{
				SaveStorageStorageContainerBlobs.BlobOperation item;
				while (this.m_operationsInput.TryDequeue(out item))
				{
					this.m_processOperationsToDo.Add(item);
				}
				this.m_processOperationsCurrent.Clear();
				int num = 0;
				int num2 = -1;
				for (int i = 0; i < this.m_processOperationsToDo.Count; i++)
				{
					SaveStorageStorageContainerBlobs.BlobOperation blobOperation = this.m_processOperationsToDo[i];
					using (RefCountedBuffer refCountedBuffer = blobOperation.CreateRef())
					{
						SaveStorageStorageContainerBlobs.BlobOperation valueOrDefault = this.m_processOperationsCurrent.GetValueOrDefault(blobOperation.Name);
						using (RefCountedBuffer refCountedBuffer2 = (valueOrDefault != null) ? valueOrDefault.CreateRef() : null)
						{
							if (refCountedBuffer2 != null)
							{
								num -= refCountedBuffer2.Length;
							}
							if (refCountedBuffer != null)
							{
								num += refCountedBuffer.Length;
							}
							if (num <= 16777216)
							{
								num2 = i;
							}
							this.m_processOperationsCurrent[blobOperation.Name] = blobOperation;
						}
					}
				}
				if (num2 >= 0)
				{
					this.m_processOperationsCurrent.Clear();
					for (int j = 0; j <= num2; j++)
					{
						SaveStorageStorageContainerBlobs.BlobOperation blobOperation2 = this.m_processOperationsToDo[j];
						this.m_processOperationsCurrent[blobOperation2.Name] = blobOperation2;
					}
					XGameSaveUpdateHandle updateHandle = null;
					int num3 = this.m_taskScheduler.ExecuteAndWait<int>(() => SDK.XGameSaveCreateUpdate(this.m_gameSaveContainerHandle, this.m_containerName, out updateHandle));
					if (Unity.XGamingRuntime.Interop.HR.FAILED(num3))
					{
						GameCoreSaveHelpers.NonTraceLogHR(num3, string.Format("Create Update Handle for Container '{0}' for {1} operation(s).", this.m_containerName, this.m_processOperationsCurrent.Count));
						throw new IOException(string.Format("Failed to create update handle. {0} (0x{1:X8})", XblHelpers.GetHRName(num3), num3));
					}
					try
					{
						using (Dictionary<string, SaveStorageStorageContainerBlobs.BlobOperation>.ValueCollection.Enumerator enumerator = this.m_processOperationsCurrent.Values.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								SaveStorageStorageContainerBlobs.BlobOperation op = enumerator.Current;
								if (op.Delete)
								{
									int num4 = this.m_taskScheduler.ExecuteAndWait<int>(() => SDK.XGameSaveSubmitBlobDelete(updateHandle, op.Name));
									if (Unity.XGamingRuntime.Interop.HR.FAILED(num4))
									{
										GameCoreSaveHelpers.NonTraceLogHR(num4, string.Concat(new string[]
										{
											"Delete Blob Data named '",
											op.Name,
											"' for Container '",
											this.m_containerName,
											"'."
										}));
										throw new IOException(string.Format("Failed to delete blob. {0} (0x{1:X8})", XblHelpers.GetHRName(num4), num4));
									}
								}
							}
						}
						foreach (SaveStorageStorageContainerBlobs.BlobOperation blobOperation3 in this.m_processOperationsCurrent.Values)
						{
							if (!blobOperation3.Delete)
							{
								using (RefCountedBuffer refCountedBuffer3 = blobOperation3.CreateRef())
								{
									if (refCountedBuffer3.Offset != 0)
									{
										throw new NotSupportedException("Non-zero offsets are currently not supported.");
									}
									int num5 = SDK.XGameSaveSubmitBlobWrite(updateHandle, blobOperation3.Name, refCountedBuffer3.BufferRaw, (ulong)((long)refCountedBuffer3.Length));
									if (Unity.XGamingRuntime.Interop.HR.FAILED(num5))
									{
										GameCoreSaveHelpers.NonTraceLogHR(num5, string.Concat(new string[]
										{
											"Write Blob Data named '",
											blobOperation3.Name,
											"' for Container '",
											this.m_containerName,
											"'."
										}));
										throw new IOException(string.Format("Failed to write blob data. {0} (0x{1:X8})", XblHelpers.GetHRName(num5), num5));
									}
								}
							}
						}
						int num6 = this.m_taskScheduler.ExecuteAndWait<int>(() => SDK.XGameSaveSubmitUpdate(updateHandle));
						if (Unity.XGamingRuntime.Interop.HR.FAILED(num6))
						{
							GameCoreSaveHelpers.NonTraceLogHR(num6, string.Format("Submit Update for Container '{0}' for {1} operation(s).", this.m_containerName, this.m_processOperationsCurrent.Count));
							throw new IOException(string.Format("Failed to submit update. {0} (0x{1:X8})", XblHelpers.GetHRName(num6), num6));
						}
					}
					finally
					{
						this.m_taskScheduler.ExecuteAndWait(delegate()
						{
							SDK.XGameSaveCloseUpdate(updateHandle);
						});
					}
					for (int k = 0; k <= num2; k++)
					{
						SaveStorageStorageContainerBlobs.BlobOperation blobOperation4 = this.m_processOperationsToDo[k];
						Interlocked.Add(ref this.m_operationsTotalSize, -blobOperation4.Size);
						blobOperation4.Dispose();
						((ICollection<KeyValuePair<string, SaveStorageStorageContainerBlobs.BlobOperation>>)this.m_operationsInputLatest).Remove(new KeyValuePair<string, SaveStorageStorageContainerBlobs.BlobOperation>(blobOperation4.Name, blobOperation4));
					}
					this.m_processOperationsToDo.RemoveRange(0, num2 + 1);
				}
			}
		}

		// Token: 0x0400A3EE RID: 41966
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MaxBlobSize = 16777216;

		// Token: 0x0400A3EF RID: 41967
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MaxUpdateSize = 16777216;

		// Token: 0x0400A3F0 RID: 41968
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan GetBlobsWaitReportingInterval = TimeSpan.FromSeconds(1.0);

		// Token: 0x0400A3F1 RID: 41969
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan EnqueueBlobOperationWaitReportingInterval = TimeSpan.FromSeconds(1.0);

		// Token: 0x0400A3F2 RID: 41970
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan BlobQueueDelay = TimeSpan.FromSeconds(5.0);

		// Token: 0x0400A3F3 RID: 41971
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly LastAccessHelper m_lastAccessHelper = new LastAccessHelper();

		// Token: 0x0400A3F4 RID: 41972
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_disposed;

		// Token: 0x0400A3F5 RID: 41973
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string m_containerName;

		// Token: 0x0400A3F6 RID: 41974
		[PublicizedFrom(EAccessModifier.Private)]
		public SingleThreadTaskScheduler m_taskScheduler;

		// Token: 0x0400A3F7 RID: 41975
		[PublicizedFrom(EAccessModifier.Private)]
		public XGameSaveProviderHandle m_gameSaveProviderHandle;

		// Token: 0x0400A3F8 RID: 41976
		[PublicizedFrom(EAccessModifier.Private)]
		public XGameSaveContainerHandle m_gameSaveContainerHandle;

		// Token: 0x0400A3F9 RID: 41977
		[PublicizedFrom(EAccessModifier.Private)]
		public ConcurrentQueue<SaveStorageStorageContainerBlobs.BlobOperation> m_operationsInput;

		// Token: 0x0400A3FA RID: 41978
		[PublicizedFrom(EAccessModifier.Private)]
		public ConcurrentDictionary<string, SaveStorageStorageContainerBlobs.BlobOperation> m_operationsInputLatest;

		// Token: 0x0400A3FB RID: 41979
		[PublicizedFrom(EAccessModifier.Private)]
		public int m_operationsTotalSize;

		// Token: 0x0400A3FC RID: 41980
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object m_processOperationsTaskLock = new object();

		// Token: 0x0400A3FD RID: 41981
		[PublicizedFrom(EAccessModifier.Private)]
		public List<SaveStorageStorageContainerBlobs.BlobOperation> m_processOperationsToDo;

		// Token: 0x0400A3FE RID: 41982
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, SaveStorageStorageContainerBlobs.BlobOperation> m_processOperationsCurrent;

		// Token: 0x0400A3FF RID: 41983
		[PublicizedFrom(EAccessModifier.Private)]
		public TaskCompletionSource<bool> m_processOperationsTaskWaitSkipTaskSource;

		// Token: 0x0400A400 RID: 41984
		[PublicizedFrom(EAccessModifier.Private)]
		public CancellationTokenSource m_processOperationsTaskCancellationTokenProvider;

		// Token: 0x0400A401 RID: 41985
		[PublicizedFrom(EAccessModifier.Private)]
		public Task m_processOperationsTask;

		// Token: 0x02001C59 RID: 7257
		[PublicizedFrom(EAccessModifier.Private)]
		public sealed class BlobOperation : IDisposable
		{
			// Token: 0x0600D713 RID: 55059 RVA: 0x004DA000 File Offset: 0x004D8200
			public BlobOperation(string name, RefCountedBuffer buffer)
			{
				this.m_name = name;
				this.m_delete = (buffer == null);
				this.m_buffer = ((buffer != null) ? buffer.CreateRef() : null);
				if (this.m_buffer != null && this.m_buffer.Offset != 0)
				{
					this.Dispose();
					throw new ArgumentException("Non-zero offsets are currently not supported.", "buffer");
				}
			}

			// Token: 0x0600D714 RID: 55060 RVA: 0x004DA061 File Offset: 0x004D8261
			[PublicizedFrom(EAccessModifier.Private)]
			public void Dispose(bool disposing)
			{
				if (!disposing)
				{
					Log.Error("BlobOperation is being finalized. It should be disposed properly.");
					return;
				}
				this.m_disposed = true;
				RefCountedBuffer buffer = this.m_buffer;
				if (buffer != null)
				{
					buffer.Dispose();
				}
				this.m_buffer = null;
			}

			// Token: 0x0600D715 RID: 55061 RVA: 0x004DA090 File Offset: 0x004D8290
			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			// Token: 0x0600D716 RID: 55062 RVA: 0x004DA0A0 File Offset: 0x004D82A0
			[PublicizedFrom(EAccessModifier.Protected)]
			public ~BlobOperation()
			{
				this.Dispose(false);
			}

			// Token: 0x17001AA3 RID: 6819
			// (get) Token: 0x0600D717 RID: 55063 RVA: 0x004DA0D0 File Offset: 0x004D82D0
			public bool IsDisposed
			{
				get
				{
					return this.m_disposed;
				}
			}

			// Token: 0x17001AA4 RID: 6820
			// (get) Token: 0x0600D718 RID: 55064 RVA: 0x004DA0D8 File Offset: 0x004D82D8
			public string Name
			{
				get
				{
					return this.m_name;
				}
			}

			// Token: 0x17001AA5 RID: 6821
			// (get) Token: 0x0600D719 RID: 55065 RVA: 0x004DA0E0 File Offset: 0x004D82E0
			public int Size
			{
				get
				{
					RefCountedBuffer buffer = this.m_buffer;
					if (buffer == null)
					{
						return 0;
					}
					return buffer.Length;
				}
			}

			// Token: 0x0600D71A RID: 55066 RVA: 0x004DA0F3 File Offset: 0x004D82F3
			public RefCountedBuffer CreateRef()
			{
				RefCountedBuffer buffer = this.m_buffer;
				if (buffer == null)
				{
					return null;
				}
				return buffer.CreateRef();
			}

			// Token: 0x17001AA6 RID: 6822
			// (get) Token: 0x0600D71B RID: 55067 RVA: 0x004DA106 File Offset: 0x004D8306
			public bool Delete
			{
				get
				{
					return this.m_delete;
				}
			}

			// Token: 0x0400A402 RID: 41986
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly string m_name;

			// Token: 0x0400A403 RID: 41987
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly bool m_delete;

			// Token: 0x0400A404 RID: 41988
			[PublicizedFrom(EAccessModifier.Private)]
			public RefCountedBuffer m_buffer;

			// Token: 0x0400A405 RID: 41989
			[PublicizedFrom(EAccessModifier.Private)]
			public bool m_disposed;
		}
	}
}
