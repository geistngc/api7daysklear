using System;

namespace Platform.XBL
{
	// Token: 0x02001C2D RID: 7213
	public class XuidResolveRequest
	{
		// Token: 0x0600D61A RID: 54810 RVA: 0x004D4288 File Offset: 0x004D2488
		public XuidResolveRequest(PlatformUserIdentifierAbs id)
		{
			this.Id = id;
		}

		// Token: 0x0400A361 RID: 41825
		public readonly PlatformUserIdentifierAbs Id;

		// Token: 0x0400A362 RID: 41826
		public bool IsSuccess;

		// Token: 0x0400A363 RID: 41827
		public ulong Xuid;
	}
}
