using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001B94 RID: 7060
	public interface IPlayerInteractionsRecorder
	{
		// Token: 0x0600D308 RID: 54024
		void Init(IPlatform owner);

		// Token: 0x0600D309 RID: 54025
		void RecordPlayerInteraction(PlayerInteraction interaction);

		// Token: 0x0600D30A RID: 54026
		void RecordPlayerInteractions(IEnumerable<PlayerInteraction> interactions);

		// Token: 0x0600D30B RID: 54027
		void Destroy();
	}
}
