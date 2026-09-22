using System;

namespace Twitch
{
	// Token: 0x02001854 RID: 6228
	public class TwitchSpawnedEntityEntry
	{
		// Token: 0x04009154 RID: 37204
		public Entity SpawnedEntity;

		// Token: 0x04009155 RID: 37205
		public int SpawnedEntityID = -1;

		// Token: 0x04009156 RID: 37206
		public TwitchActionEntry Action;

		// Token: 0x04009157 RID: 37207
		public TwitchEventActionEntry Event;

		// Token: 0x04009158 RID: 37208
		public TwitchVoteEntry Vote;

		// Token: 0x04009159 RID: 37209
		public TwitchRespawnEntry RespawnEntry;
	}
}
