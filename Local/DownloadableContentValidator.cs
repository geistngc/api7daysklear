using System;

namespace Platform.Local
{
	// Token: 0x02001CCB RID: 7371
	public class DownloadableContentValidator : IEntitlementValidator
	{
		// Token: 0x0600DAD5 RID: 56021 RVA: 0x000027FC File Offset: 0x000009FC
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600DAD6 RID: 56022 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsAvailableOnPlatform(EntitlementSetEnum _dlcSet)
		{
			return false;
		}

		// Token: 0x0600DAD7 RID: 56023 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool HasEntitlement(EntitlementSetEnum _dlcSet)
		{
			return false;
		}

		// Token: 0x0600DAD8 RID: 56024 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool IsEntitlementPurchasable(EntitlementSetEnum _dlcSet)
		{
			return false;
		}

		// Token: 0x0600DAD9 RID: 56025 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public string GetEntitlementSetId(EntitlementSetEnum _entitlementSet)
		{
			return null;
		}

		// Token: 0x0600DADA RID: 56026 RVA: 0x004E7458 File Offset: 0x004E5658
		public DateTime? GetAcquiredDate(EntitlementSetEnum _entitlementSet)
		{
			return null;
		}

		// Token: 0x0600DADB RID: 56027 RVA: 0x00010E62 File Offset: 0x0000F062
		public bool OpenStore(EntitlementSetEnum _dlcSet, Action<EntitlementSetEnum> _onDlcPurchased)
		{
			return false;
		}
	}
}
