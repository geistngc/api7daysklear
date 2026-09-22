using System;

namespace Twitch
{
	// Token: 0x02001859 RID: 6233
	public class TwitchMessageEntry
	{
		// Token: 0x0600C099 RID: 49305 RVA: 0x00477507 File Offset: 0x00475707
		public TwitchMessageEntry(string msg, string sound)
		{
			this.Message = msg;
			this.Sound = sound;
		}

		// Token: 0x04009172 RID: 37234
		public string Message = "";

		// Token: 0x04009173 RID: 37235
		public string Sound = "";
	}
}
