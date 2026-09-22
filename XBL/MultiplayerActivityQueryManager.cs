using System;
using System.Collections.Generic;
using System.Threading;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL
{
	// Token: 0x02001BFB RID: 7163
	public class MultiplayerActivityQueryManager
	{
		// Token: 0x0600D4D6 RID: 54486 RVA: 0x004CF42D File Offset: 0x004CD62D
		public MultiplayerActivityQueryManager(Unity.XGamingRuntime.XblContextHandle xblContextHandle)
		{
			this.xblContextHandle = xblContextHandle;
		}

		// Token: 0x0600D4D7 RID: 54487 RVA: 0x004CF468 File Offset: 0x004CD668
		public void GetActivityAsync(ulong[] xuids, MultiplayerActivityQueryManager.OnGetActivityComplete callback)
		{
			int num = (xuids.Length + 30 - 1) / 30;
			MultiplayerActivityQueryManager.Request request = new MultiplayerActivityQueryManager.Request(xuids, num, callback);
			object obj = this.pendingLock;
			lock (obj)
			{
				if (!this.pendingRequests.TryAdd(request.id, request))
				{
					Log.Error("[XBL] could not start GetActivityAsync as request could not be enqueued");
					return;
				}
			}
			if (num == 1)
			{
				this.StartBatch(request, xuids);
				return;
			}
			for (int i = 0; i < num; i++)
			{
				int num2 = i * 30;
				int num3 = Math.Min(xuids.Length - num2, 30);
				ulong[] array = new ulong[num3];
				Array.Copy(xuids, num2, array, 0, num3);
				this.StartBatch(request, array);
			}
		}

		// Token: 0x0600D4D8 RID: 54488 RVA: 0x004CF52C File Offset: 0x004CD72C
		[PublicizedFrom(EAccessModifier.Private)]
		public void StartBatch(MultiplayerActivityQueryManager.Request request, ulong[] xuids)
		{
			SDK.XBL.XblMultiplayerActivityGetActivityAsync(this.xblContextHandle, xuids, delegate(int hresult, XblMultiplayerActivityInfo[] results)
			{
				if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hresult))
				{
					this.CompleteBatch(request, results);
					return;
				}
				if (hresult == -2145844819)
				{
					object obj = this.retryLock;
					lock (obj)
					{
						this.retryQueue.Enqueue(new MultiplayerActivityQueryManager.RequestBatch(xuids, request));
						if (this.retryTask == null)
						{
							this.retryTask = ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(this.RetryBatchesTask), null, new ThreadManager.ExitCallbackTask(this.RetryExitHandler), true);
						}
					}
					return;
				}
				XblHelpers.LogHR(hresult, "XblMultiplayerActivityGetActivityAsync", false);
				this.CompleteBatch(request, null);
			});
		}

		// Token: 0x0600D4D9 RID: 54489 RVA: 0x004CF574 File Offset: 0x004CD774
		[PublicizedFrom(EAccessModifier.Private)]
		public void CompleteBatch(MultiplayerActivityQueryManager.Request request, XblMultiplayerActivityInfo[] batchResults)
		{
			if (batchResults != null && batchResults.Length != 0)
			{
				List<XblMultiplayerActivityInfo> results = request.results;
				lock (results)
				{
					request.results.AddRange(batchResults);
				}
			}
			if (Interlocked.Decrement(ref request.batchesPending) == 0)
			{
				request.callback(request.xuids, request.results);
				object obj = this.pendingLock;
				lock (obj)
				{
					MultiplayerActivityQueryManager.Request request2;
					this.pendingRequests.Remove(request.id, out request2);
				}
			}
		}

		// Token: 0x0600D4DA RID: 54490 RVA: 0x004CF624 File Offset: 0x004CD824
		[PublicizedFrom(EAccessModifier.Private)]
		public void RetryBatchesTask(ThreadManager.TaskInfo taskInfo)
		{
			for (;;)
			{
				Log.Warning(string.Format("[XBL] too many multiplayer activity requests, will try again in {0}s", 20));
				Thread.Sleep(20000);
				object obj = this.retryLock;
				lock (obj)
				{
					int num = 0;
					MultiplayerActivityQueryManager.RequestBatch requestBatch;
					while (this.retryQueue.TryDequeue(out requestBatch) && num < 20)
					{
						this.StartBatch(requestBatch.request, requestBatch.xuids);
						num++;
					}
					if (this.retryQueue.Count != 0)
					{
						continue;
					}
				}
				break;
			}
		}

		// Token: 0x0600D4DB RID: 54491 RVA: 0x004CF6BC File Offset: 0x004CD8BC
		[PublicizedFrom(EAccessModifier.Private)]
		public void RetryExitHandler(ThreadManager.TaskInfo _ti, Exception _e)
		{
			object obj = this.retryLock;
			lock (obj)
			{
				this.retryTask = null;
			}
		}

		// Token: 0x0400A232 RID: 41522
		[PublicizedFrom(EAccessModifier.Private)]
		public const int batchMax = 30;

		// Token: 0x0400A233 RID: 41523
		[PublicizedFrom(EAccessModifier.Private)]
		public const int backoffSeconds = 20;

		// Token: 0x0400A234 RID: 41524
		[PublicizedFrom(EAccessModifier.Private)]
		public const int burstLimit = 20;

		// Token: 0x0400A235 RID: 41525
		[PublicizedFrom(EAccessModifier.Private)]
		public static int nextRequestId;

		// Token: 0x0400A236 RID: 41526
		[PublicizedFrom(EAccessModifier.Private)]
		public Unity.XGamingRuntime.XblContextHandle xblContextHandle;

		// Token: 0x0400A237 RID: 41527
		[PublicizedFrom(EAccessModifier.Private)]
		public object pendingLock = new object();

		// Token: 0x0400A238 RID: 41528
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, MultiplayerActivityQueryManager.Request> pendingRequests = new Dictionary<int, MultiplayerActivityQueryManager.Request>();

		// Token: 0x0400A239 RID: 41529
		[PublicizedFrom(EAccessModifier.Private)]
		public object retryLock = new object();

		// Token: 0x0400A23A RID: 41530
		[PublicizedFrom(EAccessModifier.Private)]
		public Queue<MultiplayerActivityQueryManager.RequestBatch> retryQueue = new Queue<MultiplayerActivityQueryManager.RequestBatch>();

		// Token: 0x0400A23B RID: 41531
		[PublicizedFrom(EAccessModifier.Private)]
		public ThreadManager.TaskInfo retryTask;

		// Token: 0x02001BFC RID: 7164
		[PublicizedFrom(EAccessModifier.Private)]
		public class Request
		{
			// Token: 0x0600D4DC RID: 54492 RVA: 0x004CF700 File Offset: 0x004CD900
			public Request(ulong[] xuids, int batchCount, MultiplayerActivityQueryManager.OnGetActivityComplete callback)
			{
				this.id = Interlocked.Increment(ref MultiplayerActivityQueryManager.nextRequestId);
				this.xuids = xuids;
				this.batchesPending = batchCount;
				this.callback = callback;
			}

			// Token: 0x0400A23C RID: 41532
			public int id;

			// Token: 0x0400A23D RID: 41533
			public ulong[] xuids;

			// Token: 0x0400A23E RID: 41534
			public MultiplayerActivityQueryManager.OnGetActivityComplete callback;

			// Token: 0x0400A23F RID: 41535
			public List<XblMultiplayerActivityInfo> results = new List<XblMultiplayerActivityInfo>();

			// Token: 0x0400A240 RID: 41536
			public int batchesPending;
		}

		// Token: 0x02001BFD RID: 7165
		[PublicizedFrom(EAccessModifier.Private)]
		public struct RequestBatch
		{
			// Token: 0x0600D4DD RID: 54493 RVA: 0x004CF738 File Offset: 0x004CD938
			public RequestBatch(ulong[] xuidBatch, MultiplayerActivityQueryManager.Request request)
			{
				this.xuids = xuidBatch;
				this.request = request;
			}

			// Token: 0x0400A241 RID: 41537
			public ulong[] xuids;

			// Token: 0x0400A242 RID: 41538
			public MultiplayerActivityQueryManager.Request request;
		}

		// Token: 0x02001BFE RID: 7166
		// (Invoke) Token: 0x0600D4DF RID: 54495
		public delegate void OnGetActivityComplete(ulong[] requestedXuids, List<XblMultiplayerActivityInfo> results);
	}
}
