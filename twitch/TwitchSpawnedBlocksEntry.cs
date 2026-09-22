using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001855 RID: 6229
	public class TwitchSpawnedBlocksEntry
	{
		// Token: 0x0600C091 RID: 49297 RVA: 0x004772F4 File Offset: 0x004754F4
		public bool CheckPos(Vector3i pos)
		{
			for (int i = 0; i < this.blocks.Count; i++)
			{
				if (this.blocks[i] == pos)
				{
					return true;
				}
			}
			if (this.recentlyRemoved != null)
			{
				for (int j = 0; j < this.recentlyRemoved.Count; j++)
				{
					if (this.recentlyRemoved[j] == pos)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600C092 RID: 49298 RVA: 0x00477364 File Offset: 0x00475564
		[PublicizedFrom(EAccessModifier.Internal)]
		public bool RemoveBlock(Vector3i blockRemoved)
		{
			for (int i = this.blocks.Count - 1; i >= 0; i--)
			{
				if (this.blocks[i] == blockRemoved)
				{
					if (this.recentlyRemoved == null)
					{
						this.recentlyRemoved = new List<Vector3i>();
					}
					this.recentlyRemoved.Add(this.blocks[i]);
					this.blocks.RemoveAt(i);
				}
			}
			return this.blocks.Count == 0;
		}

		// Token: 0x0600C093 RID: 49299 RVA: 0x004773E4 File Offset: 0x004755E4
		[PublicizedFrom(EAccessModifier.Internal)]
		public bool RemoveBlocks(List<Vector3i> blocksRemoved)
		{
			for (int i = this.blocks.Count - 1; i >= 0; i--)
			{
				for (int j = 0; j < blocksRemoved.Count; j++)
				{
					if (this.blocks[i] == blocksRemoved[j])
					{
						this.blocks.RemoveAt(i);
						break;
					}
				}
			}
			return this.blocks.Count == 0;
		}

		// Token: 0x0400915A RID: 37210
		public List<Vector3i> blocks;

		// Token: 0x0400915B RID: 37211
		public List<Vector3i> recentlyRemoved;

		// Token: 0x0400915C RID: 37212
		public TwitchActionEntry Action;

		// Token: 0x0400915D RID: 37213
		public TwitchEventActionEntry Event;

		// Token: 0x0400915E RID: 37214
		public TwitchVoteEntry Vote;

		// Token: 0x0400915F RID: 37215
		public int BlockGroupID = -1;

		// Token: 0x04009160 RID: 37216
		public float TimeRemaining = -1f;

		// Token: 0x04009161 RID: 37217
		public TwitchRespawnEntry RespawnEntry;
	}
}
