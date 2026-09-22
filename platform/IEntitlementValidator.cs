using System;

namespace Platform
{
	// Token: 0x02001B69 RID: 7017
	public interface IEntitlementValidator
	{
		// Token: 0x0600D20C RID: 53772
		void Init(IPlatform _owner);

		// Token: 0x0600D20D RID: 53773
		bool HasEntitlement(EntitlementSetEnum _entitlementSet);

		// Token: 0x0600D20E RID: 53774
		bool IsAvailableOnPlatform(EntitlementSetEnum _entitlementSet);

		// Token: 0x0600D20F RID: 53775
		bool IsEntitlementPurchasable(EntitlementSetEnum _entitlementSet);

		// Token: 0x0600D210 RID: 53776
		string GetEntitlementSetId(EntitlementSetEnum _entitlementSet);

		// Token: 0x0600D211 RID: 53777
		DateTime? GetAcquiredDate(EntitlementSetEnum _entitlementSet);

		// Token: 0x0600D212 RID: 53778
		bool OpenStore(EntitlementSetEnum _entitlementSet, Action<EntitlementSetEnum> _onPurchased);
	}
}
