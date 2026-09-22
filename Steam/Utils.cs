using System;
using InControl;
using Platform.Shared;
using Steamworks;

namespace Platform.Steam
{
	// Token: 0x02001CA8 RID: 7336
	public class Utils : IUtils
	{
		// Token: 0x0600D996 RID: 55702 RVA: 0x004E48E2 File Offset: 0x004E2AE2
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += delegate()
			{
				this.ResolveSteamBranchName();
			};
		}

		// Token: 0x0600D997 RID: 55703 RVA: 0x004E4908 File Offset: 0x004E2B08
		[PublicizedFrom(EAccessModifier.Private)]
		public void ResolveSteamBranchName()
		{
			if (this.owner.Api.ClientApiStatus != EApiStatus.Ok)
			{
				return;
			}
			string value;
			if (SteamApps.GetCurrentBetaName(out value, 128) && !string.IsNullOrEmpty(value))
			{
				this.branchName = value;
				return;
			}
			this.branchName = "public";
		}

		// Token: 0x0600D998 RID: 55704 RVA: 0x004E4951 File Offset: 0x004E2B51
		public bool OpenBrowser(string _url)
		{
			if (global::Utils.IsValidWebUrl(ref _url))
			{
				SteamFriends.ActivateGameOverlayToWebPage(_url, EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
			}
			return true;
		}

		// Token: 0x0600D999 RID: 55705 RVA: 0x004E4964 File Offset: 0x004E2B64
		public string GetPlatformLanguage()
		{
			if (GameManager.IsDedicatedServer)
			{
				return "english";
			}
			if (this.owner.Api.ClientApiStatus != EApiStatus.Ok)
			{
				Log.Warning("[Steam] Unable to get platform language, Steam not initialized");
				return "english";
			}
			string text = SteamUtils.GetSteamUILanguage().ToLower();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "english";
		}

		// Token: 0x0600D99A RID: 55706 RVA: 0x004E49BC File Offset: 0x004E2BBC
		public string GetAppLanguage()
		{
			if (GameManager.IsDedicatedServer)
			{
				return "english";
			}
			if (this.owner.Api.ClientApiStatus != EApiStatus.Ok)
			{
				Log.Warning("[Steam] Unable to get app language, Steam not initialized");
				return "english";
			}
			string text = SteamApps.GetCurrentGameLanguage().ToLower();
			if (text == "latam")
			{
				text = "spanish";
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "english";
		}

		// Token: 0x0600D99B RID: 55707 RVA: 0x004E4A25 File Offset: 0x004E2C25
		public string GetCountry()
		{
			if (GameManager.IsDedicatedServer)
			{
				return "??";
			}
			if (this.owner.Api.ClientApiStatus == EApiStatus.Ok)
			{
				return SteamUtils.GetIPCountry();
			}
			Log.Warning("[Steam] Unable to get country, Steam not initialized");
			return "??";
		}

		// Token: 0x0600D99C RID: 55708 RVA: 0x004E4A5B File Offset: 0x004E2C5B
		public string GetStoreBranchName()
		{
			return this.branchName;
		}

		// Token: 0x0600D99D RID: 55709 RVA: 0x004E4A63 File Offset: 0x004E2C63
		public void ClearTempFiles()
		{
			Platform.Shared.Utils.TryDeleteTempCacheContents();
		}

		// Token: 0x0600D99E RID: 55710 RVA: 0x004E4A6A File Offset: 0x004E2C6A
		public string GetTempFileName(string prefix = "", string suffix = "")
		{
			return Platform.Shared.Utils.GetRandomTempCacheFileName(prefix, suffix);
		}

		// Token: 0x0600D99F RID: 55711 RVA: 0x004E4A74 File Offset: 0x004E2C74
		public bool? IsFamilyShare()
		{
			bool? result;
			try
			{
				result = new bool?(SteamApps.BIsSubscribedFromFamilySharing());
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600D9A0 RID: 55712 RVA: 0x000027FC File Offset: 0x000009FC
		public void ControllerDisconnected(InputDevice inputDevice)
		{
		}

		// Token: 0x0600D9A1 RID: 55713 RVA: 0x004E4AAC File Offset: 0x004E2CAC
		public string GetCrossplayPlayerIcon(EPlayGroup _playGroup, bool _fetchGenericIcons, EPlatformIdentifier _nativePlatform = EPlatformIdentifier.None)
		{
			switch (_playGroup)
			{
			case EPlayGroup.Standalone:
				if (_fetchGenericIcons)
				{
					return "ui_platform_pc";
				}
				break;
			case EPlayGroup.XBS:
				if (_fetchGenericIcons)
				{
					return "ui_platform_console";
				}
				break;
			case EPlayGroup.PS5:
				if (_fetchGenericIcons)
				{
					return "ui_platform_console";
				}
				break;
			}
			return string.Empty;
		}

		// Token: 0x0400A555 RID: 42325
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400A556 RID: 42326
		[PublicizedFrom(EAccessModifier.Private)]
		public string branchName = "none";
	}
}
