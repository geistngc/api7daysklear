using System;

namespace Platform
{
	// Token: 0x02001BA1 RID: 7073
	public interface IRichPresence
	{
		// Token: 0x0600D33A RID: 54074
		void Init(IPlatform _owner);

		// Token: 0x0600D33B RID: 54075
		void UpdateRichPresence(IRichPresence.PresenceStates _state);

		// Token: 0x02001BA2 RID: 7074
		public enum PresenceStates
		{
			// Token: 0x0400A139 RID: 41273
			Menu,
			// Token: 0x0400A13A RID: 41274
			Loading,
			// Token: 0x0400A13B RID: 41275
			Connecting,
			// Token: 0x0400A13C RID: 41276
			InGame
		}
	}
}
