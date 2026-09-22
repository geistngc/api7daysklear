using System;

namespace Twitch
{
	// Token: 0x02001844 RID: 6212
	public class TwitchLeaderboardEntry
	{
		// Token: 0x0600BF87 RID: 49031 RVA: 0x0046D5F2 File Offset: 0x0046B7F2
		public TwitchLeaderboardEntry(string username, string usercolor, int kills)
		{
			this.UserName = username;
			this.UserColor = ((usercolor == null) ? "FFFFFF" : usercolor);
			this.Kills = kills;
		}

		// Token: 0x0400904F RID: 36943
		public string UserName;

		// Token: 0x04009050 RID: 36944
		public string UserColor;

		// Token: 0x04009051 RID: 36945
		public int Kills;
	}
}
