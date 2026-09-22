using System;
using System.Runtime.CompilerServices;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL
{
	// Token: 0x02001C24 RID: 7204
	public class XblSandboxHelper
	{
		// Token: 0x17001A8F RID: 6799
		// (get) Token: 0x0600D5F0 RID: 54768 RVA: 0x004D3150 File Offset: 0x004D1350
		public string SandboxId
		{
			get
			{
				string text = this.sandboxId;
				if (text == null)
				{
					Log.Error("[XBL] XblSandboxHelper SandboxId has not finished refreshing");
					return null;
				}
				return text;
			}
		}

		// Token: 0x0600D5F1 RID: 54769 RVA: 0x004D3174 File Offset: 0x004D1374
		public void RefreshSandboxId()
		{
			this.sandboxId = null;
			ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(this.<RefreshSandboxId>g__GetSandboxTask|3_0), null, null, true);
		}

		// Token: 0x0600D5F2 RID: 54770 RVA: 0x004D3192 File Offset: 0x004D1392
		public static EMatchmakingGroup SandboxIdToMatchmakingGroup(string sandboxId)
		{
			if (sandboxId == "CERT" || sandboxId == "CERT.DEBUG")
			{
				return EMatchmakingGroup.CertQA;
			}
			if (!(sandboxId == "RETAIL"))
			{
				return EMatchmakingGroup.Dev;
			}
			return EMatchmakingGroup.Retail;
		}

		// Token: 0x0600D5F3 RID: 54771 RVA: 0x004D31C2 File Offset: 0x004D13C2
		public static DLCEnvironmentFlags SandboxIdToDLCEnvironment(string sandboxId)
		{
			if (sandboxId == "CERT" || sandboxId == "CERT.DEBUG")
			{
				return DLCEnvironmentFlags.Cert;
			}
			if (!(sandboxId == "RETAIL"))
			{
				return DLCEnvironmentFlags.Dev;
			}
			return DLCEnvironmentFlags.Retail;
		}

		// Token: 0x0600D5F5 RID: 54773 RVA: 0x004D31F4 File Offset: 0x004D13F4
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <RefreshSandboxId>g__GetSandboxTask|3_0(ThreadManager.TaskInfo taskInfo)
		{
			string str;
			int hr = SDK.XSystemGetXboxLiveSandboxId(out str);
			XblHelpers.LogHR(hr, "XSystemGetXboxLiveSandboxId", false);
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				Log.Out("[XBL] retrieved sandbox id: " + str);
				this.sandboxId = str;
			}
		}

		// Token: 0x0400A342 RID: 41794
		[PublicizedFrom(EAccessModifier.Private)]
		public string sandboxId;
	}
}
