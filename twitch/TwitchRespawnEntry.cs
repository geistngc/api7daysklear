using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001864 RID: 6244
	public class TwitchRespawnEntry
	{
		// Token: 0x0600C0C8 RID: 49352 RVA: 0x00477A62 File Offset: 0x00475C62
		public TwitchRespawnEntry(string username, int respawnsLeft, EntityPlayer target, TwitchAction action)
		{
			this.UserName = username;
			this.Target = target;
			this.Action = action;
			this.RespawnsLeft = respawnsLeft;
		}

		// Token: 0x0600C0C9 RID: 49353 RVA: 0x00477A9D File Offset: 0x00475C9D
		public bool CheckRespawn(string username, EntityPlayer target, TwitchAction action)
		{
			return this.UserName == username && this.Target == target && this.Action == action;
		}

		// Token: 0x0600C0CA RID: 49354 RVA: 0x00477AC8 File Offset: 0x00475CC8
		public bool RemoveSpawnedEntry(int entityID, bool checkForRemove)
		{
			bool result = false;
			for (int i = this.SpawnedEntities.Count - 1; i >= 0; i--)
			{
				if (this.SpawnedEntities[i] == entityID)
				{
					result = true;
					this.SpawnedEntities.RemoveAt(i);
				}
			}
			if (checkForRemove)
			{
				this.CheckReadyForRemove();
			}
			return result;
		}

		// Token: 0x0600C0CB RID: 49355 RVA: 0x00477B18 File Offset: 0x00475D18
		public bool RemoveSpawnedBlock(Vector3i pos, bool checkForRemove)
		{
			bool result = false;
			for (int i = this.SpawnedBlocks.Count - 1; i >= 0; i--)
			{
				if (this.SpawnedBlocks[i] == pos)
				{
					result = true;
					this.SpawnedBlocks.RemoveAt(i);
				}
			}
			if (checkForRemove)
			{
				this.CheckReadyForRemove();
			}
			return result;
		}

		// Token: 0x0600C0CC RID: 49356 RVA: 0x00477B6C File Offset: 0x00475D6C
		public bool RemoveAllSpawnedBlock(bool checkForRemove)
		{
			bool result = false;
			if (this.SpawnedBlocks.Count > 0)
			{
				result = true;
				this.SpawnedBlocks.Clear();
			}
			if (checkForRemove)
			{
				this.CheckReadyForRemove();
			}
			return result;
		}

		// Token: 0x0600C0CD RID: 49357 RVA: 0x00477BA0 File Offset: 0x00475DA0
		public void CheckReadyForRemove()
		{
			TwitchAction.RespawnCountTypes respawnCountType = this.Action.RespawnCountType;
			if (respawnCountType == TwitchAction.RespawnCountTypes.SpawnsOnly)
			{
				this.ReadyForRemove = (this.SpawnedEntities.Count == 0);
				return;
			}
			if (respawnCountType == TwitchAction.RespawnCountTypes.BlocksOnly)
			{
				this.ReadyForRemove = (this.SpawnedBlocks.Count == 0);
				return;
			}
			this.ReadyForRemove = (this.SpawnedEntities.Count == 0 && this.SpawnedBlocks.Count == 0);
		}

		// Token: 0x0600C0CE RID: 49358 RVA: 0x00477C10 File Offset: 0x00475E10
		public TwitchActionEntry RespawnAction()
		{
			TwitchActionEntry twitchActionEntry = this.Action.SetupActionEntry();
			twitchActionEntry.UserName = this.UserName;
			twitchActionEntry.Target = this.Target;
			twitchActionEntry.Action = this.Action;
			twitchActionEntry.IsRespawn = true;
			twitchActionEntry.IsBitAction = (this.Action.PointType == TwitchAction.PointTypes.Bits);
			this.RespawnsLeft--;
			this.NeedsRespawn = false;
			if (this.RespawnsLeft <= 0)
			{
				this.ReadyForRemove = true;
			}
			return twitchActionEntry;
		}

		// Token: 0x0600C0CF RID: 49359 RVA: 0x00477C8C File Offset: 0x00475E8C
		public bool CanRespawn(TwitchManager tm)
		{
			return this.NeedsRespawn && tm.CheckCanRespawnEvent(this.Target);
		}

		// Token: 0x040091A9 RID: 37289
		public string UserName;

		// Token: 0x040091AA RID: 37290
		public EntityPlayer Target;

		// Token: 0x040091AB RID: 37291
		public TwitchAction Action;

		// Token: 0x040091AC RID: 37292
		public int RespawnsLeft;

		// Token: 0x040091AD RID: 37293
		public bool NeedsRespawn;

		// Token: 0x040091AE RID: 37294
		public bool ReadyForRemove;

		// Token: 0x040091AF RID: 37295
		public List<int> SpawnedEntities = new List<int>();

		// Token: 0x040091B0 RID: 37296
		public List<Vector3i> SpawnedBlocks = new List<Vector3i>();
	}
}
