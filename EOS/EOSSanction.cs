using System;
using Epic.OnlineServices;

namespace Platform.EOS
{
	// Token: 0x02001D10 RID: 7440
	[PublicizedFrom(EAccessModifier.Internal)]
	public struct EOSSanction
	{
		// Token: 0x17001B7D RID: 7037
		// (get) Token: 0x0600DC8E RID: 56462 RVA: 0x004F0BB3 File Offset: 0x004EEDB3
		public readonly string ReferenceId { get; }

		// Token: 0x0600DC8F RID: 56463 RVA: 0x004F0BBB File Offset: 0x004EEDBB
		public EOSSanction(DateTime? expiryDate, Utf8String referenceId)
		{
			this.ReferenceId = referenceId;
			this.expiry = expiryDate.GetValueOrDefault();
		}

		// Token: 0x0400A704 RID: 42756
		public DateTime expiry;
	}
}
