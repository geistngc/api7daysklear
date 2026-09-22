using System;
using System.Collections.Generic;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL
{
	// Token: 0x02001C35 RID: 7221
	public class DownloadableContentValidator : IEntitlementValidator
	{
		// Token: 0x0600D63A RID: 54842 RVA: 0x004D4810 File Offset: 0x004D2A10
		public void Init(IPlatform _owner)
		{
			PlatformManager.NativePlatform.User.UserLoggedIn += delegate(IPlatform _)
			{
				this.user = (User)_owner.User;
				foreach (EntitlementSetEnum dlcSet in DownloadableContentValidator.entitlementMap.Keys)
				{
					this.FetchEntitlement(dlcSet, null);
				}
			};
		}

		// Token: 0x0600D63B RID: 54843 RVA: 0x004D484C File Offset: 0x004D2A4C
		[PublicizedFrom(EAccessModifier.Private)]
		public void FetchEntitlement(EntitlementSetEnum _dlcSet, Action<EntitlementSetEnum> _onDlcFetched = null)
		{
			DownloadableContentValidator.<>c__DisplayClass7_0 CS$<>8__locals1 = new DownloadableContentValidator.<>c__DisplayClass7_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1._dlcSet = _dlcSet;
			CS$<>8__locals1._onDlcFetched = _onDlcFetched;
			if (CS$<>8__locals1._dlcSet == EntitlementSetEnum.None)
			{
				return;
			}
			if (!DownloadableContentValidator.entitlementMap.ContainsKey(CS$<>8__locals1._dlcSet))
			{
				Log.Warning(string.Format("[XBL] DLC map missing entry for DLC Set {0}", CS$<>8__locals1._dlcSet));
				return;
			}
			if (!this.StartStoreOperation())
			{
				return;
			}
			SDK.XStoreAcquireLicenseForDurablesAsync(this.storeContext, DownloadableContentValidator.entitlementMap[CS$<>8__locals1._dlcSet], new XStoreAcquireLicenseForDurablesCompleted(CS$<>8__locals1.<FetchEntitlement>g__licenseAcquired|0));
		}

		// Token: 0x0600D63C RID: 54844 RVA: 0x004D48DB File Offset: 0x004D2ADB
		public bool IsAvailableOnPlatform(EntitlementSetEnum _dlcSet)
		{
			return _dlcSet == EntitlementSetEnum.None || DownloadableContentValidator.entitlementMap.ContainsKey(_dlcSet);
		}

		// Token: 0x0600D63D RID: 54845 RVA: 0x004D48F0 File Offset: 0x004D2AF0
		public bool IsEntitlementPurchasable(EntitlementSetEnum _dlcSet)
		{
			DLCEnvironmentFlags dlcEnvironments = DLCEnvironmentFlags.None;
			string text = (this.user != null) ? this.user.SandboxHelper.SandboxId : null;
			if (text == null)
			{
				Log.Warning(string.Format("[XBL] {0} no sandbox id. Defaulting to {1}", "DLCEnvironmentFlags", DLCEnvironmentFlags.None));
			}
			else
			{
				dlcEnvironments = XblSandboxHelper.SandboxIdToDLCEnvironment(text);
			}
			return DLCTitleStorageManager.Instance.IsDLCPurchasable(_dlcSet, dlcEnvironments);
		}

		// Token: 0x0600D63E RID: 54846 RVA: 0x004D494D File Offset: 0x004D2B4D
		public string GetEntitlementSetId(EntitlementSetEnum _entitlementSet)
		{
			return DownloadableContentValidator.entitlementMap.GetValueOrDefault(_entitlementSet);
		}

		// Token: 0x0600D63F RID: 54847 RVA: 0x004D495C File Offset: 0x004D2B5C
		public DateTime? GetAcquiredDate(EntitlementSetEnum _entitlementSet)
		{
			return null;
		}

		// Token: 0x0600D640 RID: 54848 RVA: 0x004D4974 File Offset: 0x004D2B74
		public bool HasEntitlement(EntitlementSetEnum _dlcSet)
		{
			if (_dlcSet == EntitlementSetEnum.None)
			{
				return true;
			}
			if (!DownloadableContentValidator.entitlementMap.ContainsKey(_dlcSet))
			{
				return false;
			}
			object obj = this.lockObj;
			bool result;
			lock (obj)
			{
				result = this.ownedEntitlements.Contains(DownloadableContentValidator.entitlementMap[_dlcSet]);
			}
			return result;
		}

		// Token: 0x0600D641 RID: 54849 RVA: 0x004D49DC File Offset: 0x004D2BDC
		public bool OpenStore(EntitlementSetEnum _dlcSet, Action<EntitlementSetEnum> _onDlcPurchased)
		{
			DownloadableContentValidator.<>c__DisplayClass13_0 CS$<>8__locals1 = new DownloadableContentValidator.<>c__DisplayClass13_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1._onDlcPurchased = _onDlcPurchased;
			CS$<>8__locals1._dlcSet = _dlcSet;
			if (!DownloadableContentValidator.entitlementMap.ContainsKey(CS$<>8__locals1._dlcSet))
			{
				return false;
			}
			CS$<>8__locals1.dlcId = DownloadableContentValidator.entitlementMap[CS$<>8__locals1._dlcSet];
			if (!this.StartStoreOperation())
			{
				return true;
			}
			SDK.XStoreShowPurchaseUIAsync(this.storeContext, CS$<>8__locals1.dlcId, null, null, new XStoreShowPurchaseUICompleted(CS$<>8__locals1.<OpenStore>g__OnStoreClosed|0));
			return true;
		}

		// Token: 0x0600D642 RID: 54850 RVA: 0x004D4A5C File Offset: 0x004D2C5C
		[PublicizedFrom(EAccessModifier.Private)]
		public bool StartStoreOperation()
		{
			object obj = this.lockObj;
			bool result;
			lock (obj)
			{
				this.remainingStoreOperations++;
				if (this.storeContext == null)
				{
					int num = SDK.XStoreCreateContext(null, out this.storeContext);
					if (Unity.XGamingRuntime.Interop.HR.FAILED(num))
					{
						this.remainingStoreOperations--;
						Log.Error(string.Format("Failed to create store context with error {0}.", num));
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600D643 RID: 54851 RVA: 0x004D4AF4 File Offset: 0x004D2CF4
		[PublicizedFrom(EAccessModifier.Private)]
		public void CompleteStoreOperation()
		{
			object obj = this.lockObj;
			lock (obj)
			{
				if (this.remainingStoreOperations > 0)
				{
					this.remainingStoreOperations--;
				}
				if (this.remainingStoreOperations == 0 && this.storeContext != null)
				{
					SDK.XStoreCloseContextHandle(this.storeContext);
					this.storeContext = null;
				}
			}
		}

		// Token: 0x0400A378 RID: 41848
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<EntitlementSetEnum, string> entitlementMap = new Dictionary<EntitlementSetEnum, string>
		{
			{
				EntitlementSetEnum.MarauderCosmetic,
				"9P0P2QLB276Q"
			},
			{
				EntitlementSetEnum.HoarderCosmetic,
				"9NZ80ZC0SS1S"
			},
			{
				EntitlementSetEnum.DesertCosmetic,
				"9MZV1FWF2CGR"
			},
			{
				EntitlementSetEnum.ClassicSurvivorCosmetic,
				"9MWGC48ZGF76"
			},
			{
				EntitlementSetEnum.PirateCosmetic,
				"9P54R75H9N3Z"
			},
			{
				EntitlementSetEnum.ChristmasCosmetics,
				"9NB7W1P5CQ30"
			},
			{
				EntitlementSetEnum.HellreaverCosmetic,
				"9NJQPF9T4XLD"
			},
			{
				EntitlementSetEnum.ButcherCosmetic,
				"9N63SN1VPKR3"
			},
			{
				EntitlementSetEnum.BeachCosmetic,
				"9P1WF3JZN172"
			},
			{
				EntitlementSetEnum.WorkingStiffCosmetic,
				"9NT7PFJQ30S6"
			},
			{
				EntitlementSetEnum.HenpocalypseCosmetic,
				"9NL21ND88ZVM"
			}
		};

		// Token: 0x0400A379 RID: 41849
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HashSet<string> ownedEntitlements = new HashSet<string>();

		// Token: 0x0400A37A RID: 41850
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object lockObj = new object();

		// Token: 0x0400A37B RID: 41851
		[PublicizedFrom(EAccessModifier.Private)]
		public User user;

		// Token: 0x0400A37C RID: 41852
		[PublicizedFrom(EAccessModifier.Private)]
		public XStoreContext storeContext;

		// Token: 0x0400A37D RID: 41853
		[PublicizedFrom(EAccessModifier.Private)]
		public int remainingStoreOperations;
	}
}
