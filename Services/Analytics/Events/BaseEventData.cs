using System;
using System.ComponentModel;
using BhvrAnalyticsServices.Attributes;
using BhvrAnalyticsServices.Interfaces;
using Newtonsoft.Json;
using Platform;
using UnityEngine;

namespace Services.Analytics.Events
{
	// Token: 0x02001687 RID: 5767
	[JsonObject(MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public abstract class BaseEventData : IAnalyticsEventData
	{
		// Token: 0x170015AE RID: 5550
		// (get) Token: 0x0600B4A5 RID: 46245
		public abstract string EventType { get; }

		// Token: 0x170015AF RID: 5551
		// (get) Token: 0x0600B4A6 RID: 46246 RVA: 0x0043A4D8 File Offset: 0x004386D8
		[JsonProperty(PropertyName = "provider_user_id")]
		[Description("The unique player identifier based on the provider_id (anonymized)")]
		[KrakenIdTransform("$.provider_user_id")]
		public string ProviderUserId { get; }

		// Token: 0x170015B0 RID: 5552
		// (get) Token: 0x0600B4A7 RID: 46247 RVA: 0x0043A4E0 File Offset: 0x004386E0
		[JsonProperty(PropertyName = "session_build")]
		[Description("The name of the build used for the game session initiated (patch/CL number)")]
		public string SessionBuild
		{
			get
			{
				return Constants.cVersionInformation.Build.ToString();
			}
		}

		// Token: 0x170015B1 RID: 5553
		// (get) Token: 0x0600B4A8 RID: 46248 RVA: 0x0043A4FF File Offset: 0x004386FF
		[JsonProperty(PropertyName = "game_version")]
		[Description("The name of the game version (Major.Minor.Hotfix)")]
		public string GameVersion
		{
			get
			{
				return Constants.cVersionInformation.ShortString;
			}
		}

		// Token: 0x170015B2 RID: 5554
		// (get) Token: 0x0600B4A9 RID: 46249 RVA: 0x0043A50B File Offset: 0x0043870B
		[JsonProperty(PropertyName = "easy_anti_cheat_enabled")]
		[Description("TRUE if the player launched with EAC activated")]
		public bool EasyAntiCheatEnabled { get; }

		// Token: 0x170015B3 RID: 5555
		// (get) Token: 0x0600B4AA RID: 46250 RVA: 0x0043A513 File Offset: 0x00438713
		[JsonProperty(PropertyName = "steam_branch_name")]
		[Description("The name of the steam branch")]
		public string SteamBranchName
		{
			get
			{
				IPlatform nativePlatform = PlatformManager.NativePlatform;
				string text;
				if (nativePlatform == null)
				{
					text = null;
				}
				else
				{
					IUtils utils = nativePlatform.Utils;
					text = ((utils != null) ? utils.GetStoreBranchName() : null);
				}
				return text ?? "none";
			}
		}

		// Token: 0x170015B4 RID: 5556
		// (get) Token: 0x0600B4AB RID: 46251 RVA: 0x0043A53B File Offset: 0x0043873B
		[JsonProperty(PropertyName = "platform")]
		[Description("The platform that the game session was initiated on.")]
		public string Platform { get; }

		// Token: 0x170015B5 RID: 5557
		// (get) Token: 0x0600B4AC RID: 46252 RVA: 0x0043A543 File Offset: 0x00438743
		[JsonProperty(PropertyName = "provider")]
		[Description("Provider Used to launch the game (Steam, XBL, PSN)")]
		public string Provider { get; }

		// Token: 0x0600B4AD RID: 46253 RVA: 0x0043A54C File Offset: 0x0043874C
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseEventData()
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			object obj;
			if (nativePlatform == null)
			{
				obj = null;
			}
			else
			{
				IUserClient user = nativePlatform.User;
				if (user == null)
				{
					obj = null;
				}
				else
				{
					PlatformUserIdentifierAbs platformUserId = user.PlatformUserId;
					obj = ((platformUserId != null) ? platformUserId.ReadablePlatformUserIdentifier : null);
				}
			}
			this.ProviderUserId = obj;
			IPlatform multiPlatform = PlatformManager.MultiPlatform;
			bool? flag;
			if (multiPlatform == null)
			{
				flag = null;
			}
			else
			{
				IAntiCheatClient antiCheatClient = multiPlatform.AntiCheatClient;
				flag = ((antiCheatClient != null) ? new bool?(antiCheatClient.ClientAntiCheatEnabled()) : null);
			}
			bool? flag2 = flag;
			this.EasyAntiCheatEnabled = flag2.GetValueOrDefault();
			this.Platform = Application.platform.ToString();
			IPlatform nativePlatform2 = PlatformManager.NativePlatform;
			this.Provider = (((nativePlatform2 != null) ? nativePlatform2.PlatformIdentifier.ToString() : null) ?? "Unknown");
			base..ctor();
		}
	}
}
