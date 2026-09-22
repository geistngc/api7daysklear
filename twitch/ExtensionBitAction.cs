using System;

namespace Twitch
{
	// Token: 0x020017D3 RID: 6099
	[Serializable]
	public class ExtensionBitAction : ExtensionAction
	{
		// Token: 0x04008DEA RID: 36330
		public string txn_id;

		// Token: 0x04008DEB RID: 36331
		public long time_created;

		// Token: 0x04008DEC RID: 36332
		public int cost;
	}
}
