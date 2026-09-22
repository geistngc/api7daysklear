using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x020017E8 RID: 6120
	[Serializable]
	public class PubSubStatusMessage
	{
		// Token: 0x04008E64 RID: 36452
		public string opaqueId;

		// Token: 0x04008E65 RID: 36453
		public string displayName;

		// Token: 0x04008E66 RID: 36454
		public string status;

		// Token: 0x04008E67 RID: 36455
		public List<string> party;

		// Token: 0x04008E68 RID: 36456
		public string commands;

		// Token: 0x04008E69 RID: 36457
		public string language;

		// Token: 0x04008E6A RID: 36458
		public List<string> actionTypes;
	}
}
