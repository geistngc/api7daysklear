using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x020017D1 RID: 6097
	[Serializable]
	public class ExtensionActionResponse
	{
		// Token: 0x04008DE5 RID: 36325
		public List<ExtensionAction> standardActions;

		// Token: 0x04008DE6 RID: 36326
		public List<ExtensionBitAction> bitActions;
	}
}
