using System;
using System.Threading;
using Unity.Profiling;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;
using UnityEngine;
using UnityEngine.Profiling;

namespace Platform.XBL
{
	// Token: 0x02001C22 RID: 7202
	public abstract class XblPlatformApi : IPlatformApi
	{
		// Token: 0x17001A8D RID: 6797
		// (get) Token: 0x0600D5DE RID: 54750
		public abstract string SCID { get; }

		// Token: 0x17001A8E RID: 6798
		// (get) Token: 0x0600D5DF RID: 54751 RVA: 0x004D2E46 File Offset: 0x004D1046
		// (set) Token: 0x0600D5E0 RID: 54752 RVA: 0x004D2E4E File Offset: 0x004D104E
		public EApiStatus ClientApiStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EApiStatus.Uninitialized;

		// Token: 0x14000130 RID: 304
		// (add) Token: 0x0600D5E1 RID: 54753 RVA: 0x004D2E58 File Offset: 0x004D1058
		// (remove) Token: 0x0600D5E2 RID: 54754 RVA: 0x004D2EB4 File Offset: 0x004D10B4
		public event Action ClientApiInitialized
		{
			add
			{
				lock (this)
				{
					this.m_clientApiInitialized = (Action)Delegate.Combine(this.m_clientApiInitialized, value);
					if (this.ClientApiStatus == EApiStatus.Ok)
					{
						value();
					}
				}
			}
			remove
			{
				lock (this)
				{
					this.m_clientApiInitialized = (Action)Delegate.Remove(this.m_clientApiInitialized, value);
				}
			}
		}

		// Token: 0x0600D5E3 RID: 54755
		public abstract void Init(IPlatform owner);

		// Token: 0x0600D5E4 RID: 54756 RVA: 0x004D2F00 File Offset: 0x004D1100
		public bool InitClientApis()
		{
			if (this.ClientApiStatus == EApiStatus.Ok)
			{
				return true;
			}
			if (Application.isEditor)
			{
				this.ClientApiStatus = EApiStatus.Ok;
				Action clientApiInitialized = this.m_clientApiInitialized;
				if (clientApiInitialized != null)
				{
					clientApiInitialized();
				}
				return true;
			}
			int hr = SDK.XGameRuntimeInitialize();
			XblHelpers.LogHR(hr, "Initialize Gaming Runtime.", false);
			if (Unity.XGamingRuntime.Interop.HR.FAILED(hr))
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				return false;
			}
			if (Unity.XGamingRuntime.Interop.HR.FAILED(SDK.CreateDefaultTaskQueue()))
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				return false;
			}
			this.m_dispatchGXDKTaskQueueRunning = true;
			this.m_dispatchGXDKTaskQueueThread = new Thread(delegate()
			{
				Log.Out("[XBL] Started Thread: " + Thread.CurrentThread.Name);
				try
				{
					while (this.m_dispatchGXDKTaskQueueRunning)
					{
						try
						{
							SDK.XTaskQueueDispatch(32U);
						}
						catch (ThreadInterruptedException)
						{
							break;
						}
						catch (Exception e)
						{
							Log.Exception(e);
						}
						finally
						{
						}
					}
				}
				finally
				{
					Profiler.EndThreadProfiling();
					Log.Out("[XBL] Stopped Thread: " + Thread.CurrentThread.Name);
				}
			})
			{
				Name = "GXDK Task Queue Dispatch Completion",
				IsBackground = true
			};
			this.m_dispatchGXDKTaskQueueThread.Start();
			int hr2 = SDK.XBL.XblInitialize(this.SCID);
			XblHelpers.LogHR(hr2, "Initialize Xbox Live.", false);
			if (Unity.XGamingRuntime.Interop.HR.FAILED(hr2))
			{
				return false;
			}
			Log.Out("[XBL] API loaded.");
			this.ClientApiStatus = EApiStatus.Ok;
			Action clientApiInitialized2 = this.m_clientApiInitialized;
			if (clientApiInitialized2 != null)
			{
				clientApiInitialized2();
			}
			return true;
		}

		// Token: 0x0600D5E5 RID: 54757
		public abstract bool InitServerApis();

		// Token: 0x0600D5E6 RID: 54758
		public abstract void ServerApiLoaded();

		// Token: 0x0600D5E7 RID: 54759 RVA: 0x000027FC File Offset: 0x000009FC
		public void Update()
		{
		}

		// Token: 0x0600D5E8 RID: 54760 RVA: 0x004D2FEC File Offset: 0x004D11EC
		public void Destroy()
		{
			if (Application.isEditor)
			{
				return;
			}
			this.m_dispatchGXDKTaskQueueRunning = false;
			SDK.CloseDefaultXTaskQueue();
			SDK.XBL.XblCleanup(delegate(int hr)
			{
				XblHelpers.LogHR(hr, "Uninitialize Xbox Live.", false);
			});
			Thread dispatchGXDKTaskQueueThread = this.m_dispatchGXDKTaskQueueThread;
			if (dispatchGXDKTaskQueueThread != null)
			{
				dispatchGXDKTaskQueueThread.Interrupt();
			}
			Thread dispatchGXDKTaskQueueThread2 = this.m_dispatchGXDKTaskQueueThread;
			if (dispatchGXDKTaskQueueThread2 != null)
			{
				dispatchGXDKTaskQueueThread2.Join();
			}
			this.m_dispatchGXDKTaskQueueThread = null;
			SDK.XGameRuntimeUninitialize();
			XblHelpers.LogHR(0, "Uninitialize Gaming Runtime.", false);
			Log.Out("[XBL] API Destroyed.");
		}

		// Token: 0x0600D5E9 RID: 54761 RVA: 0x00040FA0 File Offset: 0x0003F1A0
		public float GetScreenBoundsValueFromSystem()
		{
			return 1f;
		}

		// Token: 0x0600D5EA RID: 54762 RVA: 0x004D3075 File Offset: 0x004D1275
		[PublicizedFrom(EAccessModifier.Protected)]
		public XblPlatformApi()
		{
		}

		// Token: 0x0400A33C RID: 41788
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ProfilerMarker s_TaskQueueDispatchMarker = new ProfilerMarker("XTaskQueueDispatch");

		// Token: 0x0400A33D RID: 41789
		[PublicizedFrom(EAccessModifier.Private)]
		public Thread m_dispatchGXDKTaskQueueThread;

		// Token: 0x0400A33E RID: 41790
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_dispatchGXDKTaskQueueRunning;

		// Token: 0x0400A33F RID: 41791
		[PublicizedFrom(EAccessModifier.Private)]
		public Action m_clientApiInitialized;
	}
}
