using System;

namespace Twitch
{
	// Token: 0x02001857 RID: 6231
	public class TwitchEventActionEntry
	{
		// Token: 0x0600C096 RID: 49302 RVA: 0x004774CD File Offset: 0x004756CD
		public bool HandleEvent(TwitchManager tm)
		{
			if (this.Event.HandleEvent(this.UserName, tm))
			{
				this.IsSent = true;
				return true;
			}
			return false;
		}

		// Token: 0x04009168 RID: 37224
		public string UserName;

		// Token: 0x04009169 RID: 37225
		public byte Tier;

		// Token: 0x0400916A RID: 37226
		public short Count;

		// Token: 0x0400916B RID: 37227
		public bool IsSent;

		// Token: 0x0400916C RID: 37228
		public bool IsRetry;

		// Token: 0x0400916D RID: 37229
		public bool ReadyForRemove;

		// Token: 0x0400916E RID: 37230
		public TwitchActionHistoryEntry HistoryEntry;

		// Token: 0x0400916F RID: 37231
		public BaseTwitchEventEntry Event;
	}
}
