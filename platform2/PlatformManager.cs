using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Platform.MultiPlatform;
using UnityEngine;

namespace Platform
{
	// Token: 0x02001BD6 RID: 7126
	public static class PlatformManager
	{
		// Token: 0x17001A3A RID: 6714
		// (get) Token: 0x0600D405 RID: 54277 RVA: 0x004CC030 File Offset: 0x004CA230
		// (set) Token: 0x0600D406 RID: 54278 RVA: 0x004CC037 File Offset: 0x004CA237
		public static EDeviceType DeviceType { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A3B RID: 6715
		// (get) Token: 0x0600D407 RID: 54279 RVA: 0x004CC03F File Offset: 0x004CA23F
		// (set) Token: 0x0600D408 RID: 54280 RVA: 0x004CC046 File Offset: 0x004CA246
		public static IPlatform MultiPlatform { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A3C RID: 6716
		// (get) Token: 0x0600D409 RID: 54281 RVA: 0x004CC04E File Offset: 0x004CA24E
		// (set) Token: 0x0600D40A RID: 54282 RVA: 0x004CC055 File Offset: 0x004CA255
		public static IPlatform NativePlatform { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A3D RID: 6717
		// (get) Token: 0x0600D40B RID: 54283 RVA: 0x004CC05D File Offset: 0x004CA25D
		// (set) Token: 0x0600D40C RID: 54284 RVA: 0x004CC064 File Offset: 0x004CA264
		public static IPlatform CrossplatformPlatform { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A3E RID: 6718
		// (get) Token: 0x0600D40D RID: 54285 RVA: 0x004CC06C File Offset: 0x004CA26C
		// (set) Token: 0x0600D40E RID: 54286 RVA: 0x004CC073 File Offset: 0x004CA273
		public static ClientLobbyManager ClientLobbyManager { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001A3F RID: 6719
		// (get) Token: 0x0600D40F RID: 54287 RVA: 0x004CC07B File Offset: 0x004CA27B
		public static PlatformUserIdentifierAbs InternalLocalUserIdentifier
		{
			get
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				PlatformUserIdentifierAbs platformUserIdentifierAbs;
				if (crossplatformPlatform == null)
				{
					platformUserIdentifierAbs = null;
				}
				else
				{
					IUserClient user = crossplatformPlatform.User;
					platformUserIdentifierAbs = ((user != null) ? user.PlatformUserId : null);
				}
				return platformUserIdentifierAbs ?? PlatformManager.NativePlatform.User.PlatformUserId;
			}
		}

		// Token: 0x0600D410 RID: 54288 RVA: 0x004CC0B0 File Offset: 0x004CA2B0
		public static bool Init()
		{
			if (PlatformManager.initialized)
			{
				return true;
			}
			PlatformManager.DeviceType = EDeviceType.PC;
			try
			{
				PlatformManager.initialized = true;
				Log.Out("[Platform] Init");
				PlatformManager.FindSupportedPlatforms();
				PlatformConfiguration platformConfiguration = PlatformManager.DetectPlatform();
				PlatformManager.GetCommandLineOverrides(platformConfiguration);
				IPlatform platform;
				PlatformManager.initPlatformFromIdentifier(platformConfiguration.NativePlatform, "Native", out platform);
				PlatformManager.NativePlatform = platform;
				if (platformConfiguration.CrossPlatform != EPlatformIdentifier.None)
				{
					PlatformManager.initPlatformFromIdentifier(platformConfiguration.CrossPlatform, "Cross", out platform);
					platform.IsCrossplatform = true;
					PlatformManager.CrossplatformPlatform = platform;
				}
				PlatformManager.MultiPlatform = new Factory();
				using (List<EPlatformIdentifier>.Enumerator enumerator = platformConfiguration.ServerPlatforms.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (PlatformManager.initPlatformFromIdentifier(enumerator.Current, "Server", out platform))
						{
							platform.AsServerOnly = true;
						}
					}
				}
				PlatformManager.ClientLobbyManager = new ClientLobbyManager();
				PlatformManager.NativePlatform.CreateInstances();
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				if (crossplatformPlatform != null)
				{
					crossplatformPlatform.CreateInstances();
				}
				PlatformManager.MultiPlatform.CreateInstances();
				List<EPlatformIdentifier> list = new List<EPlatformIdentifier>();
				foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.serverPlatforms)
				{
					if (keyValuePair.Value.AsServerOnly)
					{
						try
						{
							keyValuePair.Value.CreateInstances();
						}
						catch (NotSupportedException ex)
						{
							Log.Error(string.Format("[Platform] Platform {0} Errored on init, removing from the list of server platforms. Error: {1}.", keyValuePair.Key, ex.Message));
							list.Add(keyValuePair.Key);
						}
					}
				}
				foreach (EPlatformIdentifier eplatformIdentifier in list)
				{
					IPlatform platform2;
					if (PlatformManager.serverPlatforms.TryGetValue(eplatformIdentifier, out platform2))
					{
						platformConfiguration.ServerPlatforms.Remove(eplatformIdentifier);
						platform2.Destroy();
						PlatformManager.serverPlatforms.Remove(eplatformIdentifier);
					}
				}
				list.Clear();
				IPlatform crossplatformPlatform2 = PlatformManager.CrossplatformPlatform;
				if (((crossplatformPlatform2 != null) ? crossplatformPlatform2.User : null) != null)
				{
					PlatformManager.CrossplatformPlatform.User.UserLoggedIn += BacktraceUtils.BacktraceUserLoggedIn;
				}
				else if (PlatformManager.NativePlatform.User != null)
				{
					PlatformManager.NativePlatform.User.UserLoggedIn += BacktraceUtils.BacktraceUserLoggedIn;
				}
				PlatformManager.NativePlatform.Init();
				IPlatform crossplatformPlatform3 = PlatformManager.CrossplatformPlatform;
				if (crossplatformPlatform3 != null)
				{
					crossplatformPlatform3.Init();
				}
				PlatformManager.MultiPlatform.Init();
				foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair2 in PlatformManager.serverPlatforms)
				{
					if (keyValuePair2.Value.AsServerOnly)
					{
						keyValuePair2.Value.Init();
					}
				}
				PlatformUserManager.Init();
			}
			catch (Exception e)
			{
				Log.Error("[Platform] Error while initializing platform code, shutting down.");
				Log.Exception(e);
				Application.Quit(1);
				return false;
			}
			return true;
		}

		// Token: 0x0600D411 RID: 54289 RVA: 0x004CC410 File Offset: 0x004CA610
		public static void Update()
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (nativePlatform != null)
			{
				nativePlatform.Update();
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform != null)
			{
				crossplatformPlatform.Update();
			}
			IPlatform multiPlatform = PlatformManager.MultiPlatform;
			if (multiPlatform != null)
			{
				multiPlatform.Update();
			}
			foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.serverPlatforms)
			{
				if (keyValuePair.Value.AsServerOnly)
				{
					keyValuePair.Value.Update();
				}
			}
			PlatformUserManager.Update();
		}

		// Token: 0x0600D412 RID: 54290 RVA: 0x004CC4AC File Offset: 0x004CA6AC
		public static void LateUpdate()
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (nativePlatform != null)
			{
				nativePlatform.LateUpdate();
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform != null)
			{
				crossplatformPlatform.LateUpdate();
			}
			IPlatform multiPlatform = PlatformManager.MultiPlatform;
			if (multiPlatform != null)
			{
				multiPlatform.LateUpdate();
			}
			foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.serverPlatforms)
			{
				if (keyValuePair.Value.AsServerOnly)
				{
					keyValuePair.Value.LateUpdate();
				}
			}
		}

		// Token: 0x0600D413 RID: 54291 RVA: 0x004CC544 File Offset: 0x004CA744
		public static void Destroy()
		{
			PlatformUserManager.Destroy();
			foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.serverPlatforms)
			{
				if (keyValuePair.Value.AsServerOnly)
				{
					keyValuePair.Value.Destroy();
				}
			}
			IPlatform multiPlatform = PlatformManager.MultiPlatform;
			if (multiPlatform != null)
			{
				multiPlatform.Destroy();
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform != null)
			{
				crossplatformPlatform.Destroy();
			}
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (nativePlatform != null)
			{
				nativePlatform.Destroy();
			}
			PlatformManager.serverPlatforms.Clear();
			PlatformManager.MultiPlatform = null;
			PlatformManager.CrossplatformPlatform = null;
			PlatformManager.NativePlatform = null;
		}

		// Token: 0x0600D414 RID: 54292 RVA: 0x004CC5FC File Offset: 0x004CA7FC
		public static string PlatformStringFromEnum(EPlatformIdentifier _platformIdentifier)
		{
			return _platformIdentifier.ToStringCached<EPlatformIdentifier>();
		}

		// Token: 0x0600D415 RID: 54293 RVA: 0x004CC604 File Offset: 0x004CA804
		public static bool TryPlatformIdentifierFromString(string _platformName, out EPlatformIdentifier _platformIdentifier)
		{
			return EnumUtils.TryParse<EPlatformIdentifier>(_platformName, out _platformIdentifier, true);
		}

		// Token: 0x0600D416 RID: 54294 RVA: 0x004CC610 File Offset: 0x004CA810
		public static IPlatform InstanceForPlatformIdentifier(EPlatformIdentifier _platformIdentifier)
		{
			IPlatform result;
			if (!PlatformManager.serverPlatforms.TryGetValue(_platformIdentifier, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x0600D417 RID: 54295 RVA: 0x004CC62F File Offset: 0x004CA82F
		public static bool IsPlatformLoaded(EPlatformIdentifier _platformIdentifier)
		{
			return PlatformManager.serverPlatforms.ContainsKey(_platformIdentifier);
		}

		// Token: 0x0600D418 RID: 54296 RVA: 0x004CC63C File Offset: 0x004CA83C
		public static string GetPlatformDisplayName(EPlatformIdentifier _platformIdentifier)
		{
			return Localization.Get("platformName" + _platformIdentifier.ToStringCached<EPlatformIdentifier>(), false, null);
		}

		// Token: 0x0600D419 RID: 54297 RVA: 0x004CC658 File Offset: 0x004CA858
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool initPlatformFromIdentifier(EPlatformIdentifier _platformIdentifier, string _logName, out IPlatform _target)
		{
			Type type;
			if (!PlatformManager.supportedPlatforms.TryGetValue(_platformIdentifier, out type))
			{
				throw new NotSupportedException(string.Concat(new string[]
				{
					"[Platform] ",
					_logName,
					" platform ",
					_platformIdentifier.ToStringCached<EPlatformIdentifier>(),
					" not supported. Supported: ",
					PlatformManager.supportedPlatformsString
				}));
			}
			Log.Out("[Platform] Using " + _logName.ToLowerInvariant() + " platform: " + _platformIdentifier.ToStringCached<EPlatformIdentifier>());
			if (PlatformManager.serverPlatforms.ContainsKey(_platformIdentifier))
			{
				_target = null;
				return false;
			}
			_target = ReflectionHelpers.Instantiate<IPlatform>(type);
			PlatformManager.serverPlatforms.Add(_target.PlatformIdentifier, _target);
			return true;
		}

		// Token: 0x0600D41A RID: 54298 RVA: 0x004CC700 File Offset: 0x004CA900
		[PublicizedFrom(EAccessModifier.Private)]
		public static void FindSupportedPlatforms()
		{
			PlatformManager.supportedPlatforms.Clear();
			PlatformManager.supportedPlatformsString = "";
			Type typeFromHandle = typeof(IPlatform);
			Type attrType = typeof(PlatformFactoryAttribute);
			ReflectionHelpers.FindTypesImplementingBase(typeFromHandle, delegate(Type _type)
			{
				object[] customAttributes = _type.GetCustomAttributes(attrType, false);
				if (customAttributes.Length != 1)
				{
					return;
				}
				PlatformFactoryAttribute platformFactoryAttribute = (PlatformFactoryAttribute)customAttributes[0];
				Type type;
				if (PlatformManager.supportedPlatforms.TryGetValue(platformFactoryAttribute.TargetPlatform, out type))
				{
					Log.Error(string.Concat(new string[]
					{
						"[Platform] Multiple platform providers for platform ",
						platformFactoryAttribute.TargetPlatform.ToStringCached<EPlatformIdentifier>(),
						": Loaded '",
						type.FullName,
						"', found '",
						_type.FullName,
						"'"
					}));
					return;
				}
				PlatformManager.supportedPlatforms.Add(platformFactoryAttribute.TargetPlatform, _type);
				if (PlatformManager.supportedPlatformsString.Length > 0)
				{
					PlatformManager.supportedPlatformsString += ", ";
				}
				PlatformManager.supportedPlatformsString += platformFactoryAttribute.TargetPlatform.ToStringCached<EPlatformIdentifier>();
			}, false);
			PlatformManager.UserIdentifierFactories.Clear();
			Type typeFromHandle2 = typeof(AbsUserIdentifierFactory);
			Type attrType2 = typeof(UserIdentifierFactoryAttribute);
			ReflectionHelpers.FindTypesImplementingBase(typeFromHandle2, delegate(Type _type)
			{
				object[] customAttributes = _type.GetCustomAttributes(attrType2, false);
				if (customAttributes.Length != 1)
				{
					return;
				}
				UserIdentifierFactoryAttribute userIdentifierFactoryAttribute = (UserIdentifierFactoryAttribute)customAttributes[0];
				AbsUserIdentifierFactory absUserIdentifierFactory;
				if (PlatformManager.UserIdentifierFactories.TryGetValue(userIdentifierFactoryAttribute.TargetPlatform, out absUserIdentifierFactory))
				{
					Log.Error(string.Concat(new string[]
					{
						"[Platform] Multiple user identifier factories for platform ",
						userIdentifierFactoryAttribute.TargetPlatform.ToStringCached<EPlatformIdentifier>(),
						": Loaded '",
						absUserIdentifierFactory.GetType().FullName,
						"', found '",
						_type.FullName,
						"'"
					}));
					return;
				}
				AbsUserIdentifierFactory absUserIdentifierFactory2 = ReflectionHelpers.Instantiate<AbsUserIdentifierFactory>(_type);
				if (absUserIdentifierFactory2 == null)
				{
					return;
				}
				PlatformManager.UserIdentifierFactories.Add(userIdentifierFactoryAttribute.TargetPlatform, absUserIdentifierFactory2);
			}, false);
		}

		// Token: 0x0600D41B RID: 54299 RVA: 0x004CC78C File Offset: 0x004CA98C
		[PublicizedFrom(EAccessModifier.Private)]
		public static void GetCommandLineOverrides(PlatformConfiguration _platforms)
		{
			string launchArgument = GameUtils.GetLaunchArgument("platform");
			if (!string.IsNullOrEmpty(launchArgument))
			{
				_platforms.ParsePlatform("platform", launchArgument);
			}
			launchArgument = GameUtils.GetLaunchArgument("crossplatform");
			if (!string.IsNullOrEmpty(launchArgument))
			{
				_platforms.ParsePlatform("crossplatform", launchArgument);
			}
			launchArgument = GameUtils.GetLaunchArgument("serverplatforms");
			if (!string.IsNullOrEmpty(launchArgument))
			{
				_platforms.ParsePlatform("serverplatforms", launchArgument);
			}
		}

		// Token: 0x0600D41C RID: 54300 RVA: 0x004CC7FC File Offset: 0x004CA9FC
		[PublicizedFrom(EAccessModifier.Private)]
		public static PlatformConfiguration DetectPlatform()
		{
			PlatformConfiguration result = null;
			if (PlatformConfiguration.ReadFile(ref result, null))
			{
				return result;
			}
			PlatformConfiguration platformConfiguration = new PlatformConfiguration();
			Log.Warning(string.Format("[Platform] No platform config file ({0}) found, defaulting to {1} / {2} without additional server platforms.", "platform.cfg", platformConfiguration.NativePlatform, platformConfiguration.CrossPlatform));
			return platformConfiguration;
		}

		// Token: 0x0400A1A8 RID: 41384
		public const string PlatformConfigFileName = "platform.cfg";

		// Token: 0x0400A1AE RID: 41390
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<EPlatformIdentifier, IPlatform> serverPlatforms = new EnumDictionary<EPlatformIdentifier, IPlatform>();

		// Token: 0x0400A1AF RID: 41391
		public static readonly ReadOnlyDictionary<EPlatformIdentifier, IPlatform> ServerPlatforms = new ReadOnlyDictionary<EPlatformIdentifier, IPlatform>(PlatformManager.serverPlatforms);

		// Token: 0x0400A1B0 RID: 41392
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool initialized;

		// Token: 0x0400A1B1 RID: 41393
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<EPlatformIdentifier, Type> supportedPlatforms = new EnumDictionary<EPlatformIdentifier, Type>();

		// Token: 0x0400A1B2 RID: 41394
		[PublicizedFrom(EAccessModifier.Private)]
		public static string supportedPlatformsString;

		// Token: 0x0400A1B3 RID: 41395
		public static readonly Dictionary<EPlatformIdentifier, AbsUserIdentifierFactory> UserIdentifierFactories = new EnumDictionary<EPlatformIdentifier, AbsUserIdentifierFactory>();
	}
}
