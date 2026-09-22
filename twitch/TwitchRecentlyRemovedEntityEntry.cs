using System;

namespace Twitch
{
	// Token: 0x02001856 RID: 6230
	public class TwitchRecentlyRemovedEntityEntry
	{
		// Token: 0x0600C095 RID: 49301 RVA: 0x0047746C File Offset: 0x0047566C
		public TwitchRecentlyRemovedEntityEntry(TwitchSpawnedEntityEntry entry)
		{
			this.SpawnedEntity = entry.SpawnedEntity;
			this.SpawnedEntityID = entry.SpawnedEntityID;
			this.Action = entry.Action;
			this.Event = entry.Event;
			this.Vote = entry.Vote;
			this.TimeRemaining = 60f;
		}

		// Token: 0x04009162 RID: 37218
		public Entity SpawnedEntity;

		// Token: 0x04009163 RID: 37219
		public int SpawnedEntityID = -1;

		// Token: 0x04009164 RID: 37220
		public TwitchActionEntry Action;

		// Token: 0x04009165 RID: 37221
		public TwitchEventActionEntry Event;

		// Token: 0x04009166 RID: 37222
		public TwitchVoteEntry Vote;

		// Token: 0x04009167 RID: 37223
		public float TimeRemaining;
	}
}
