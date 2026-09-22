using System;
using System.Collections.Generic;

namespace Services.Analytics
{
	// Token: 0x02001682 RID: 5762
	public static class EventTypes
	{
		// Token: 0x040087B4 RID: 34740
		public static readonly Dictionary<string, int> VersionDictionary = new Dictionary<string, int>
		{
			{
				"login",
				3
			},
			{
				"heartbeat",
				5
			},
			{
				"hardware_info",
				2
			},
			{
				"server_start",
				3
			},
			{
				"player_join_server",
				2
			},
			{
				"challenge_claimed",
				1
			},
			{
				"challenge_completed",
				1
			},
			{
				"dlc_ownership",
				0
			},
			{
				"cosmetic_usage",
				0
			}
		};
	}
}
