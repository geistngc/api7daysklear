using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BhvrAnalyticsServices.Interfaces;
using Platform;
using Platform.EOS;
using SandboxOptions;
using Services.Analytics.Events;
using UnityEngine.Networking;

namespace Services.Analytics
{
	// Token: 0x02001683 RID: 5763
	public static class Helper
	{
		// Token: 0x0600B48B RID: 46219 RVA: 0x00439EE4 File Offset: 0x004380E4
		public static string GetServerId()
		{
			GameServerInfo currentGameServerInfoServerOrClient = SingletonMonoBehaviour<ConnectionManager>.Instance.CurrentGameServerInfoServerOrClient;
			string text = (currentGameServerInfoServerOrClient != null) ? currentGameServerInfoServerOrClient.GetValue(GameInfoString.UniqueId) : null;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		// Token: 0x0600B48C RID: 46220 RVA: 0x00439F18 File Offset: 0x00438118
		public static int? GetServerPlayerCount()
		{
			World world = GameManager.Instance.World;
			if (world == null)
			{
				return null;
			}
			if (GameManager.IsDedicatedServer)
			{
				return new int?(world.Players.dict.Values.Count);
			}
			if (world.Players.dict.Values.Count <= 0)
			{
				return null;
			}
			return new int?(world.Players.dict.Values.Count);
		}

		// Token: 0x0600B48D RID: 46221 RVA: 0x00439F9B File Offset: 0x0043819B
		public static string GetSaveID()
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				return GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
			}
			World world = GameManager.Instance.World;
			if (world == null)
			{
				return null;
			}
			return world.Guid;
		}

		// Token: 0x0600B48E RID: 46222 RVA: 0x00439FC9 File Offset: 0x004381C9
		public static IList<Mod> GetTruncatedLoadedMods(int modCountLimit)
		{
			return ModManager.GetLoadedMods().GetRange(0, Math.Min(ModManager.GetLoadedMods().Count, modCountLimit));
		}

		// Token: 0x0600B48F RID: 46223 RVA: 0x00439FE8 File Offset: 0x004381E8
		public static Dictionary<string, string> GetSandboxSettingsDelta(SandboxOptionPreset preset)
		{
			return preset.PresetValues.ToDictionary((KeyValuePair<SandboxOptions, int> kvp) => SandboxOptionManager.Current.SandboxOptionsDict[kvp.Key].OptionName, (KeyValuePair<SandboxOptions, int> kvp) => SandboxOptionManager.Current.SandboxOptionsDict[kvp.Key].GetValueTextFromIndex(kvp.Value, "english"));
		}

		// Token: 0x0600B490 RID: 46224 RVA: 0x0043A03E File Offset: 0x0043823E
		public static IEnumerator LoginEventAnalyticCoroutine(bool isNewLogin)
		{
			string ipAddress = "Unknown";
			using (UnityWebRequest webRequest = UnityWebRequest.Get("https://api.ipify.org"))
			{
				webRequest.timeout = 10;
				yield return webRequest.SendWebRequest();
				if (webRequest.result == UnityWebRequest.Result.Success)
				{
					ipAddress = webRequest.downloadHandler.text;
				}
				else
				{
					Log.Out("[Kraken] Error: Failed to retrieve IP address: " + webRequest.error);
				}
			}
			UnityWebRequest webRequest = null;
			LoginEventData loginEventData = new LoginEventData();
			loginEventData.SessionStartTimeStamp = DateTime.UtcNow.ToString("O");
			loginEventData.IP = ipAddress;
			loginEventData.CrossplayEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsCrossplay);
			LoginEventData loginEventData2 = loginEventData;
			EUserAccountState userAccountState = EosHelpers.UserAccountState;
			bool? isFirstLaunchEos;
			if (userAccountState != EUserAccountState.NewUser)
			{
				if (userAccountState == EUserAccountState.Unknown)
				{
					isFirstLaunchEos = null;
				}
				else
				{
					isFirstLaunchEos = new bool?(false);
				}
			}
			else
			{
				isFirstLaunchEos = new bool?(true);
			}
			loginEventData2.IsFirstLaunchEos = isFirstLaunchEos;
			loginEventData.IsNewLogin = isNewLogin;
			LoginEventData analyticsEventData = loginEventData;
			IAnalyticsService analyticsService = ServiceProvider.Instance.Get<IAnalyticsService>();
			analyticsService.LogEvent(analyticsEventData);
			SingletonMonoBehaviour<ConnectionManager>.Instance.BeginHeartbeat((float)analyticsService.Configuration.HeartbeatInSeconds);
			if (isNewLogin)
			{
				analyticsService.LogEvent(new HardwareInfoEventData());
				if (PlatformManager.NativePlatform.Api.ClientApiStatus != EApiStatus.Ok)
				{
					PlatformManager.NativePlatform.Api.ClientApiInitialized += Helper.<LoginEventAnalyticCoroutine>g__OnClientApiInitialized|6_0;
				}
				else
				{
					Helper.LogDlcEntitlementsAnalytic();
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x0600B491 RID: 46225 RVA: 0x0043A050 File Offset: 0x00438250
		[PublicizedFrom(EAccessModifier.Private)]
		public static void LogDlcEntitlementsAnalytic()
		{
			List<DlcOwnershipEventData.EntitlementData> list = (from entitlement in EnumUtils.Values<EntitlementSetEnum>()
			where entitlement > EntitlementSetEnum.None
			where EntitlementManager.Instance.HasEntitlement(entitlement)
			select entitlement).Select(delegate(EntitlementSetEnum entitlement)
			{
				DateTime? dateTime;
				return new DlcOwnershipEventData.EntitlementData
				{
					Name = entitlement.ToStringCached<EntitlementSetEnum>(),
					ID = EntitlementManager.Instance.GetEntitlementSetId(entitlement),
					AcquiredDate = ((EntitlementManager.Instance.GetAcquiredDate(entitlement) != null) ? dateTime.GetValueOrDefault().ToUniversalTime().ToString("o") : null)
				};
			}).ToList<DlcOwnershipEventData.EntitlementData>();
			IAnalyticsService analyticsService = ServiceProvider.Instance.Get<IAnalyticsService>();
			DlcOwnershipEventData dlcOwnershipEventData = new DlcOwnershipEventData();
			dlcOwnershipEventData.DlcOwned = ((list.Count > 0) ? list : null);
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			bool? isFamilyShare;
			if (nativePlatform == null)
			{
				isFamilyShare = null;
			}
			else
			{
				IUtils utils = nativePlatform.Utils;
				isFamilyShare = ((utils != null) ? utils.IsFamilyShare() : null);
			}
			dlcOwnershipEventData.IsFamilyShare = isFamilyShare;
			analyticsService.LogEvent(dlcOwnershipEventData);
		}

		// Token: 0x0600B492 RID: 46226 RVA: 0x0043A12E File Offset: 0x0043832E
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void <LoginEventAnalyticCoroutine>g__OnClientApiInitialized|6_0()
		{
			PlatformManager.NativePlatform.Api.ClientApiInitialized -= Helper.<LoginEventAnalyticCoroutine>g__OnClientApiInitialized|6_0;
			Helper.LogDlcEntitlementsAnalytic();
		}

		// Token: 0x040087B5 RID: 34741
		public const int ModCountLimit = 100;
	}
}
