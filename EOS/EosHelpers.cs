using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatCommon;

namespace Platform.EOS
{
	// Token: 0x02001CF0 RID: 7408
	public static class EosHelpers
	{
		// Token: 0x0600DBCF RID: 56271 RVA: 0x004EBF30 File Offset: 0x004EA130
		[PublicizedFrom(EAccessModifier.Private)]
		static EosHelpers()
		{
			EnumDictionary<EPlatformIdentifier, ExternalAccountType> enumDictionary = new EnumDictionary<EPlatformIdentifier, ExternalAccountType>();
			foreach (KeyValuePair<ExternalAccountType, EPlatformIdentifier> keyValuePair in EosHelpers.accountTypeMappings)
			{
				enumDictionary.Add(keyValuePair.Value, keyValuePair.Key);
			}
			EosHelpers.PlatformIdentifierMappings = new ReadOnlyDictionary<EPlatformIdentifier, ExternalAccountType>(enumDictionary);
		}

		// Token: 0x0600DBD0 RID: 56272 RVA: 0x004EC028 File Offset: 0x004EA228
		public static void TestEosConnection(Action<bool> _callback)
		{
			ThreadManager.StartThread("TestEosConnection", new ThreadManager.ThreadFunctionDelegate(EosHelpers.<TestEosConnection>g__workerFunc|10_0), new EosHelpers.EosConnectionTestInfo
			{
				Callback = _callback
			}, null, false, true);
		}

		// Token: 0x0600DBD1 RID: 56273 RVA: 0x004EC050 File Offset: 0x004EA250
		public static void AssertMainThread(string _id)
		{
			if (!ThreadManager.IsMainThread())
			{
				Log.Warning("[EOSH] Called EOS code from secondary thread: " + _id);
			}
		}

		// Token: 0x0600DBD2 RID: 56274 RVA: 0x004EC06C File Offset: 0x004EA26C
		public static ClientInfo.EDeviceType GetDeviceTypeFromPlatform(string platform)
		{
			if (platform == "other" || platform == "steam")
			{
				return ClientInfo.EDeviceType.Unknown;
			}
			if (platform == "playstation")
			{
				return ClientInfo.EDeviceType.PlayStation;
			}
			if (!(platform == "xbox"))
			{
				Log.Error("[EOS] [Auth] GetDeviceTypeFromPlatform: Unknown platform: " + platform);
				return ClientInfo.EDeviceType.Unknown;
			}
			return ClientInfo.EDeviceType.Xbox;
		}

		// Token: 0x0600DBD3 RID: 56275 RVA: 0x004EC0C6 File Offset: 0x004EA2C6
		public static bool RequiresAntiCheat(this ClientInfo.EDeviceType deviceType)
		{
			return deviceType != ClientInfo.EDeviceType.PlayStation && deviceType != ClientInfo.EDeviceType.Xbox;
		}

		// Token: 0x0600DBD4 RID: 56276 RVA: 0x004EC0D8 File Offset: 0x004EA2D8
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <TestEosConnection>g__workerFunc|10_0(ThreadManager.ThreadInfo _info)
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.epicgames.dev/sdk/v1/default");
				httpWebRequest.Timeout = 5000;
				httpWebRequest.KeepAlive = false;
				using ((HttpWebResponse)httpWebRequest.GetResponse())
				{
					((EosHelpers.EosConnectionTestInfo)_info.parameter).Result = true;
				}
			}
			catch (Exception ex)
			{
				Log.Out("[EOS] Connection test failed: " + ex.Message);
				((EosHelpers.EosConnectionTestInfo)_info.parameter).Result = false;
			}
			ThreadManager.AddSingleTaskMainThread("TestEosConnectionResult", new ThreadManager.MainThreadTaskFunctionDelegate(EosHelpers.<TestEosConnection>g__mainThreadSyncFunc|10_1), _info.parameter);
		}

		// Token: 0x0600DBD5 RID: 56277 RVA: 0x004EC194 File Offset: 0x004EA394
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <TestEosConnection>g__mainThreadSyncFunc|10_1(object _parameter)
		{
			EosHelpers.EosConnectionTestInfo eosConnectionTestInfo = (EosHelpers.EosConnectionTestInfo)_parameter;
			eosConnectionTestInfo.Callback(eosConnectionTestInfo.Result);
		}

		// Token: 0x0400A66D RID: 42605
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<ExternalAccountType, EPlatformIdentifier> accountTypeMappings = new EnumDictionary<ExternalAccountType, EPlatformIdentifier>
		{
			{
				ExternalAccountType.Epic,
				EPlatformIdentifier.EGS
			},
			{
				ExternalAccountType.Psn,
				EPlatformIdentifier.PSN
			},
			{
				ExternalAccountType.Steam,
				EPlatformIdentifier.Steam
			},
			{
				ExternalAccountType.Xbl,
				EPlatformIdentifier.XBL
			}
		};

		// Token: 0x0400A66E RID: 42606
		public static readonly ReadOnlyDictionary<ExternalAccountType, EPlatformIdentifier> AccountTypeMappings = new ReadOnlyDictionary<ExternalAccountType, EPlatformIdentifier>(EosHelpers.accountTypeMappings);

		// Token: 0x0400A66F RID: 42607
		public static readonly ReadOnlyDictionary<EPlatformIdentifier, ExternalAccountType> PlatformIdentifierMappings;

		// Token: 0x0400A670 RID: 42608
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<ClientInfo.EDeviceType, AntiCheatCommonClientPlatform> deviceTypeToAntiCheatPlatformMappings = new EnumDictionary<ClientInfo.EDeviceType, AntiCheatCommonClientPlatform>
		{
			{
				ClientInfo.EDeviceType.Unknown,
				AntiCheatCommonClientPlatform.Unknown
			},
			{
				ClientInfo.EDeviceType.Linux,
				AntiCheatCommonClientPlatform.Linux
			},
			{
				ClientInfo.EDeviceType.Mac,
				AntiCheatCommonClientPlatform.Mac
			},
			{
				ClientInfo.EDeviceType.Windows,
				AntiCheatCommonClientPlatform.Windows
			},
			{
				ClientInfo.EDeviceType.PlayStation,
				AntiCheatCommonClientPlatform.PlayStation
			},
			{
				ClientInfo.EDeviceType.Xbox,
				AntiCheatCommonClientPlatform.Xbox
			}
		};

		// Token: 0x0400A671 RID: 42609
		public static readonly ReadOnlyDictionary<ClientInfo.EDeviceType, AntiCheatCommonClientPlatform> DeviceTypeToAntiCheatPlatformMappings = new ReadOnlyDictionary<ClientInfo.EDeviceType, AntiCheatCommonClientPlatform>(EosHelpers.deviceTypeToAntiCheatPlatformMappings);

		// Token: 0x0400A672 RID: 42610
		[PublicizedFrom(EAccessModifier.Private)]
		public const string eosApiUrl = "https://api.epicgames.dev/sdk/v1/default";

		// Token: 0x0400A673 RID: 42611
		[PublicizedFrom(EAccessModifier.Private)]
		public const int eosApiTestTimeout = 5000;

		// Token: 0x0400A674 RID: 42612
		public static EUserAccountState UserAccountState = EUserAccountState.Unknown;

		// Token: 0x02001CF1 RID: 7409
		[PublicizedFrom(EAccessModifier.Private)]
		public class EosConnectionTestInfo
		{
			// Token: 0x0400A675 RID: 42613
			public Action<bool> Callback;

			// Token: 0x0400A676 RID: 42614
			public bool Result;
		}
	}
}
