using System;

namespace Platform.EOS
{
	// Token: 0x02001CE5 RID: 7397
	public static class AntiCheatCommon
	{
		// Token: 0x0600DB77 RID: 56183 RVA: 0x004E96B0 File Offset: 0x004E78B0
		public static void Init()
		{
			if (AntiCheatCommon.initialized)
			{
				return;
			}
			string launchArgument = GameUtils.GetLaunchArgument("debugeac");
			AntiCheatCommon.DebugEacVerbose = (launchArgument != null && launchArgument == "verbose");
			AntiCheatCommon.NoEacCmdLine = (GameUtils.GetLaunchArgument("noeac") != null);
			AntiCheatCommon.initialized = true;
		}

		// Token: 0x0600DB78 RID: 56184 RVA: 0x004E9700 File Offset: 0x004E7900
		public static ClientInfo IntPtrToClientInfo(IntPtr _ptr, string _messageIfNull = null)
		{
			int num = _ptr.ToInt32();
			ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForClientNumber(num);
			if (clientInfo == null && _messageIfNull != null)
			{
				Log.Error(_messageIfNull, new object[]
				{
					num
				});
			}
			return clientInfo;
		}

		// Token: 0x0600DB79 RID: 56185 RVA: 0x004E9742 File Offset: 0x004E7942
		public static IntPtr ClientInfoToIntPtr(ClientInfo _clientInfo)
		{
			return new IntPtr(_clientInfo.ClientNumber);
		}

		// Token: 0x0400A630 RID: 42544
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool initialized;

		// Token: 0x0400A631 RID: 42545
		public static bool NoEacCmdLine;

		// Token: 0x0400A632 RID: 42546
		public static bool DebugEacVerbose;

		// Token: 0x0400A633 RID: 42547
		public static readonly object LockObject = new object();
	}
}
