using System;
using System.IO;
using Epic.OnlineServices;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.Sanctions;
using UnityEngine;

namespace Platform.EOS
{
	// Token: 0x02001CE8 RID: 7400
	public class Api : IPlatformApi
	{
		// Token: 0x0600DBA5 RID: 56229 RVA: 0x004EAA98 File Offset: 0x004E8C98
		[PublicizedFrom(EAccessModifier.Private)]
		static Api()
		{
			string launchArgument = GameUtils.GetLaunchArgument("debugeos");
			if (launchArgument != null)
			{
				if (launchArgument == "verbose")
				{
					Api.DebugLevel = Api.EDebugLevel.Verbose;
					return;
				}
				Api.DebugLevel = Api.EDebugLevel.Normal;
			}
		}

		// Token: 0x0600DBA6 RID: 56230 RVA: 0x004EAAD3 File Offset: 0x004E8CD3
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x0600DBA7 RID: 56231 RVA: 0x004EAADC File Offset: 0x004E8CDC
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnApplicationStateChanged(ApplicationState _applicationState)
		{
			if (this.PlatformInterface == null)
			{
				return;
			}
			ApplicationStatus applicationStatus;
			if (_applicationState != ApplicationState.Foreground)
			{
				if (_applicationState != ApplicationState.Suspended)
				{
					throw new ArgumentOutOfRangeException("_applicationState", _applicationState, "[EOS] OnApplicationStateChanged: ApplicationState is missing a conversion to a EOS.ApplicationStatus");
				}
				applicationStatus = ApplicationStatus.BackgroundSuspended;
			}
			else
			{
				applicationStatus = ApplicationStatus.Foreground;
			}
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.PlatformInterface.SetApplicationStatus(applicationStatus);
			}
		}

		// Token: 0x17001B67 RID: 7015
		// (get) Token: 0x0600DBA8 RID: 56232 RVA: 0x004EAB58 File Offset: 0x004E8D58
		// (set) Token: 0x0600DBA9 RID: 56233 RVA: 0x004EAB60 File Offset: 0x004E8D60
		public EApiStatus ClientApiStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EApiStatus.Uninitialized;

		// Token: 0x14000140 RID: 320
		// (add) Token: 0x0600DBAA RID: 56234 RVA: 0x004EAB6C File Offset: 0x004E8D6C
		// (remove) Token: 0x0600DBAB RID: 56235 RVA: 0x004EABC8 File Offset: 0x004E8DC8
		public event Action ClientApiInitialized
		{
			add
			{
				lock (this)
				{
					this.clientApiInitialized = (Action)Delegate.Combine(this.clientApiInitialized, value);
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
					this.clientApiInitialized = (Action)Delegate.Remove(this.clientApiInitialized, value);
				}
			}
		}

		// Token: 0x0600DBAC RID: 56236 RVA: 0x004EAC14 File Offset: 0x004E8E14
		public bool InitClientApis()
		{
			if (this.ClientApiStatus == EApiStatus.Ok)
			{
				return true;
			}
			EosCreds eosCreds = GameManager.IsDedicatedServer ? EosCreds.ServerDeviceIdCredentials : EosCreds.ClientCredentials;
			this.initPlatform(eosCreds, eosCreds.ServerMode);
			return this.ClientApiStatus == EApiStatus.Ok;
		}

		// Token: 0x0600DBAD RID: 56237 RVA: 0x004EAC55 File Offset: 0x004E8E55
		public bool InitServerApis()
		{
			return this.InitClientApis();
		}

		// Token: 0x0600DBAE RID: 56238 RVA: 0x000027FC File Offset: 0x000009FC
		public void ServerApiLoaded()
		{
		}

		// Token: 0x0600DBAF RID: 56239 RVA: 0x004EAC60 File Offset: 0x004E8E60
		public void Update()
		{
			if (this.ClientApiStatus != EApiStatus.Ok)
			{
				return;
			}
			this.tickDurationStopwatch.Restart();
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.PlatformInterface.Tick();
			}
			long num = this.tickDurationStopwatch.ElapsedMicroseconds / 1000L;
			if (Api.DebugLevel != Api.EDebugLevel.Off && num > 5L)
			{
				Log.Warning(string.Format("[EOS] Tick took exceptionally long: {0} ms", num));
			}
		}

		// Token: 0x0600DBB0 RID: 56240 RVA: 0x004EACEC File Offset: 0x004E8EEC
		public void Destroy()
		{
			if (this.ClientApiStatus != EApiStatus.Ok)
			{
				return;
			}
			this.ConnectInterface = null;
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.PlatformInterface.Release();
			}
			this.PlatformInterface = null;
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				PlatformInterface.Shutdown();
			}
		}

		// Token: 0x0600DBB1 RID: 56241 RVA: 0x00040FA0 File Offset: 0x0003F1A0
		public float GetScreenBoundsValueFromSystem()
		{
			return 1f;
		}

		// Token: 0x0600DBB2 RID: 56242 RVA: 0x004EAD78 File Offset: 0x004E8F78
		[PublicizedFrom(EAccessModifier.Private)]
		public void initPlatform(EosCreds _creds, bool _serverMode)
		{
			InitializeOptions initializeOptions = new InitializeOptions
			{
				ProductName = "7 Days To Die",
				ProductVersion = Constants.cVersionInformation.SerializableString
			};
			Result result = Result.NotFound;
			object lockObject;
			try
			{
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					result = PlatformInterface.Initialize(ref initializeOptions);
				}
			}
			catch (DllNotFoundException e)
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[EOS] Native library or one of its dependencies not found (e.g. no Microsoft Visual C Redistributables 2022)");
				Log.Exception(e);
				Application.Quit(1);
			}
			Log.Out(string.Format("[EOS] Initialize: {0}", result));
			LogLevel logLevel;
			switch (Api.DebugLevel)
			{
			case Api.EDebugLevel.Off:
				logLevel = LogLevel.Warning;
				break;
			case Api.EDebugLevel.Normal:
				logLevel = LogLevel.Info;
				break;
			case Api.EDebugLevel.Verbose:
				logLevel = LogLevel.VeryVerbose;
				break;
			default:
				throw new ArgumentOutOfRangeException("DebugLevel");
			}
			LogLevel logLevel2 = logLevel;
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				LoggingInterface.SetLogLevel(LogCategory.AllCategories, logLevel2);
				string launchArgument = GameUtils.GetLaunchArgument("debugeac");
				if (launchArgument != null)
				{
					LoggingInterface.SetLogLevel(LogCategory.AntiCheat, (launchArgument == "verbose") ? LogLevel.Verbose : LogLevel.Info);
				}
				else
				{
					LoggingInterface.SetLogLevel(LogCategory.AntiCheat, LogLevel.Warning);
				}
				if (logLevel2 == LogLevel.VeryVerbose)
				{
					LoggingInterface.SetLogLevel(LogCategory.Http, LogLevel.Verbose);
				}
				LoggingInterface.SetLogLevel(LogCategory.Analytics, LogLevel.Error);
				LoggingInterface.SetLogLevel(LogCategory.Messaging, LogLevel.Warning);
				LoggingInterface.SetLogLevel(LogCategory.Ecom, LogLevel.Error);
				LoggingInterface.SetLogLevel(LogCategory.Auth, LogLevel.Error);
				LoggingInterface.SetLogLevel(LogCategory.Presence, LogLevel.Warning);
				LoggingInterface.SetLogLevel(LogCategory.Overlay, LogLevel.Warning);
				LoggingInterface.SetLogLevel(LogCategory.Ui, LogLevel.Warning);
				LoggingInterface.SetCallback(new LogMessageFunc(this.logCallback));
			}
			this.PlatformInterface = this.createPlatformInterface(_creds, _serverMode);
			if (this.PlatformInterface == null)
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[EOS] Failed to create platform");
				return;
			}
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (((nativePlatform != null) ? nativePlatform.ApplicationState : null) != null)
			{
				PlatformManager.NativePlatform.ApplicationState.OnApplicationStateChanged += this.OnApplicationStateChanged;
			}
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.ConnectInterface = this.PlatformInterface.GetConnectInterface();
			}
			if (this.ConnectInterface == null)
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[EOS] Failed to get connect interface");
				return;
			}
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.SanctionsInterface = this.PlatformInterface.GetSanctionsInterface();
			}
			if (this.SanctionsInterface == null)
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[EOS] Failed to get sanctions interface");
				return;
			}
			this.ClientApiStatus = EApiStatus.Ok;
			Action action = this.clientApiInitialized;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600DBB3 RID: 56243 RVA: 0x004EB0A0 File Offset: 0x004E92A0
		[PublicizedFrom(EAccessModifier.Private)]
		public PlatformInterface createPlatformInterface(EosCreds _creds, bool _serverMode)
		{
			WindowsOptions windowsOptions = default(WindowsOptions);
			windowsOptions.ProductId = _creds.ProductId;
			windowsOptions.SandboxId = _creds.SandboxId;
			windowsOptions.ClientCredentials = new ClientCredentials
			{
				ClientId = _creds.ClientId,
				ClientSecret = _creds.ClientSecret
			};
			windowsOptions.DeploymentId = _creds.DeploymentId;
			windowsOptions.EncryptionKey = "0000000000000000000000000000000000000000000000000000000000000000";
			windowsOptions.IsServer = _serverMode;
			windowsOptions.Flags = PlatformFlags.DisableOverlay;
			windowsOptions.Flags |= PlatformFlags.DisableSocialOverlay;
			windowsOptions.RTCOptions = null;
			windowsOptions.RTCOptions = new WindowsRTCOptions?(new WindowsRTCOptions
			{
				PlatformSpecificOptions = new WindowsRTCOptionsPlatformSpecificOptions?(new WindowsRTCOptionsPlatformSpecificOptions
				{
					XAudio29DllPath = GameIO.GetGameDir("7DaysToDie_Data/Plugins/x86_64/xaudio2_9redist.dll")
				})
			});
			windowsOptions.CacheDirectory = GameIO.GetDeviceLocalUserGameDataDir();
			if (!Directory.Exists(windowsOptions.CacheDirectory))
			{
				Directory.CreateDirectory(windowsOptions.CacheDirectory);
			}
			object lockObject = AntiCheatCommon.LockObject;
			PlatformInterface result;
			lock (lockObject)
			{
				result = PlatformInterface.Create(ref windowsOptions);
			}
			return result;
		}

		// Token: 0x0600DBB4 RID: 56244 RVA: 0x004EB214 File Offset: 0x004E9414
		[PublicizedFrom(EAccessModifier.Private)]
		public void logCallback(ref LogMessage _message)
		{
			if (_message.Level == LogLevel.Warning && _message.Category == "LogHttp")
			{
				this.httpWarningCount++;
				if (this.httpWarningCount == 50)
				{
					this.httpNextTime = Time.unscaledTime + 600f;
					return;
				}
				if (this.httpWarningCount > 50)
				{
					if (Time.unscaledTime < this.httpNextTime)
					{
						return;
					}
					Log.Out(string.Format("[EOS] [LogHttp - Warning] Skipped {0} warnings within the last {1} seconds!", this.httpWarningCount - 50, 600f));
					this.httpWarningCount = 0;
				}
			}
			if (Api.DebugLevel == Api.EDebugLevel.Off && _message.Level == LogLevel.Warning && _message.Category == "LogEOSRTC" && _message.Message.StartsWith("TickTracker Ticks have been delayed.", StringComparison.Ordinal))
			{
				return;
			}
			string txt = string.Format("[EOS] [{0} - {1}] {2}", _message.Category, _message.Level.ToStringCached<LogLevel>(), _message.Message);
			LogLevel level = _message.Level;
			if (level > LogLevel.Error)
			{
				if (level <= LogLevel.Info)
				{
					if (level == LogLevel.Warning)
					{
						Log.Warning(txt);
						return;
					}
					if (level != LogLevel.Info)
					{
						goto IL_16D;
					}
				}
				else if (level != LogLevel.Verbose && level != LogLevel.VeryVerbose)
				{
					goto IL_16D;
				}
				Log.Out(txt);
				return;
			}
			if (level == LogLevel.Off)
			{
				Log.Error(txt);
				throw new ArgumentOutOfRangeException();
			}
			if (level == LogLevel.Fatal || level == LogLevel.Error)
			{
				Log.Error(txt);
				return;
			}
			IL_16D:
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x0400A644 RID: 42564
		public static readonly Api.EDebugLevel DebugLevel = Api.EDebugLevel.Off;

		// Token: 0x0400A645 RID: 42565
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A646 RID: 42566
		public PlatformInterface PlatformInterface;

		// Token: 0x0400A647 RID: 42567
		public ConnectInterface ConnectInterface;

		// Token: 0x0400A648 RID: 42568
		public SanctionsInterface SanctionsInterface;

		// Token: 0x0400A649 RID: 42569
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MicroStopwatch tickDurationStopwatch = new MicroStopwatch(false);

		// Token: 0x0400A64A RID: 42570
		[PublicizedFrom(EAccessModifier.Internal)]
		public readonly SanctionsCheck eosSanctionsCheck = new SanctionsCheck();

		// Token: 0x0400A64C RID: 42572
		[PublicizedFrom(EAccessModifier.Private)]
		public Action clientApiInitialized;

		// Token: 0x0400A64D RID: 42573
		[PublicizedFrom(EAccessModifier.Private)]
		public const int httpWarningLimit = 50;

		// Token: 0x0400A64E RID: 42574
		[PublicizedFrom(EAccessModifier.Private)]
		public const float httpWarningTimeout = 600f;

		// Token: 0x0400A64F RID: 42575
		[PublicizedFrom(EAccessModifier.Private)]
		public int httpWarningCount;

		// Token: 0x0400A650 RID: 42576
		[PublicizedFrom(EAccessModifier.Private)]
		public float httpNextTime;

		// Token: 0x02001CE9 RID: 7401
		public enum EDebugLevel
		{
			// Token: 0x0400A652 RID: 42578
			Off,
			// Token: 0x0400A653 RID: 42579
			Normal,
			// Token: 0x0400A654 RID: 42580
			Verbose
		}
	}
}
