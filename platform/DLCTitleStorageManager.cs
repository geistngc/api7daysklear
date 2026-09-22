using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platform
{
	// Token: 0x02001B4A RID: 6986
	public class DLCTitleStorageManager
	{
		// Token: 0x170019AD RID: 6573
		// (get) Token: 0x0600D149 RID: 53577 RVA: 0x004C8CFC File Offset: 0x004C6EFC
		public static DLCTitleStorageManager Instance
		{
			get
			{
				DLCTitleStorageManager result;
				if ((result = DLCTitleStorageManager.instance) == null)
				{
					result = (DLCTitleStorageManager.instance = new DLCTitleStorageManager());
				}
				return result;
			}
		}

		// Token: 0x0600D14A RID: 53578 RVA: 0x004C8D12 File Offset: 0x004C6F12
		[PublicizedFrom(EAccessModifier.Private)]
		public DLCTitleStorageManager()
		{
		}

		// Token: 0x0600D14B RID: 53579 RVA: 0x004C8D28 File Offset: 0x004C6F28
		public bool IsDLCPurchasable(EntitlementSetEnum _dlcSet, DLCEnvironmentFlags _dlcEnvironments)
		{
			DLCTitleStorageManager.CatalogEntry catalogEntry;
			return _dlcSet != EntitlementSetEnum.None && this.dlcPurchasability.TryGetValue(_dlcSet, out catalogEntry) && (catalogEntry.environments & _dlcEnvironments) != DLCEnvironmentFlags.None && (!_dlcEnvironments.HasFlag(DLCEnvironmentFlags.Retail) || catalogEntry.retailDate == null || !(catalogEntry.retailDate.Value > DateTime.Now));
		}

		// Token: 0x0600D14C RID: 53580 RVA: 0x004C8D94 File Offset: 0x004C6F94
		public void FetchFromSource()
		{
			if (!this.fetchAttempted)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				if (crossplatformPlatform != null && crossplatformPlatform.PlatformIdentifier == EPlatformIdentifier.EOS)
				{
					this.fetchAttempted = true;
					PlatformManager.NativePlatform.User.UserLoggedIn += delegate(IPlatform _)
					{
						ThreadManager.StartCoroutine(this.RequestDataCo());
					};
					return;
				}
			}
		}

		// Token: 0x0600D14D RID: 53581 RVA: 0x004C8DE5 File Offset: 0x004C6FE5
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator RequestDataCo()
		{
			DLCTitleStorageManager.<>c__DisplayClass10_0 CS$<>8__locals1 = new DLCTitleStorageManager.<>c__DisplayClass10_0();
			CS$<>8__locals1.<>4__this = this;
			IRemoteFileStorage storage = PlatformManager.MultiPlatform.RemoteFileStorage;
			if (storage == null)
			{
				Log.Warning("[DLCTitleStorageManager] No remote file storage implementation available.");
				yield break;
			}
			bool loggedSlow = false;
			float startTime = Time.time;
			while (!storage.IsReady)
			{
				if (storage.Unavailable)
				{
					Log.Warning("[DLCTitleStorageManager] Remote Storage is unavailable");
					yield break;
				}
				yield return null;
				if (!loggedSlow && Time.time > startTime + 30f)
				{
					loggedSlow = true;
					Log.Warning("[DLCTitleStorageManager] Waiting for DLC configuration from remote storage exceeded 30s");
				}
			}
			CS$<>8__locals1.fileDownloadComplete = false;
			storage.GetFile("DLCConfiguration", new IRemoteFileStorage.FileDownloadCompleteCallback(CS$<>8__locals1.<RequestDataCo>g__fileDownloadedCallback|0));
			while (!CS$<>8__locals1.fileDownloadComplete)
			{
				yield return null;
			}
			yield break;
		}

		// Token: 0x0600D14E RID: 53582 RVA: 0x004C8DF4 File Offset: 0x004C6FF4
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetLocalPlatformNetworkString()
		{
			if ((DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent())
			{
				if (PlatformManager.NativePlatform.PlatformIdentifier == EPlatformIdentifier.Steam)
				{
					return "Standalone_Steam";
				}
				if (PlatformManager.NativePlatform.PlatformIdentifier == EPlatformIdentifier.XBL)
				{
					return "Standalone_XBL";
				}
			}
			else
			{
				if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent())
				{
					return "XboxSeries_XBL";
				}
				if (DeviceFlag.PS5.IsCurrent())
				{
					return "PS5_PSN";
				}
			}
			return null;
		}

		// Token: 0x0400A080 RID: 41088
		public const string RFSUri = "DLCConfiguration";

		// Token: 0x0400A081 RID: 41089
		[PublicizedFrom(EAccessModifier.Private)]
		public static DLCTitleStorageManager instance;

		// Token: 0x0400A082 RID: 41090
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<EntitlementSetEnum, DLCTitleStorageManager.CatalogEntry> dlcPurchasability = new Dictionary<EntitlementSetEnum, DLCTitleStorageManager.CatalogEntry>();

		// Token: 0x0400A083 RID: 41091
		[PublicizedFrom(EAccessModifier.Private)]
		public bool fetchAttempted;

		// Token: 0x02001B4B RID: 6987
		public struct CatalogEntry
		{
			// Token: 0x0400A084 RID: 41092
			public DLCEnvironmentFlags environments;

			// Token: 0x0400A085 RID: 41093
			public DateTime? retailDate;
		}
	}
}
