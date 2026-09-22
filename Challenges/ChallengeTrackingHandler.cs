using System;
using System.Collections.Generic;
using UnityEngine;

namespace Challenges
{
	// Token: 0x02001924 RID: 6436
	public class ChallengeTrackingHandler
	{
		// Token: 0x0600C6BA RID: 50874 RVA: 0x00493474 File Offset: 0x00491674
		public bool Update(float deltaTime)
		{
			if (this.LocalPlayer == null)
			{
				return true;
			}
			if (this.Owner == null || !this.Owner.IsActive)
			{
				return false;
			}
			if (this.LocalPlayer.IsInTrader != this.lastInTrader)
			{
				this.lastInTrader = this.LocalPlayer.IsInTrader;
				this.NeedsRefresh = true;
			}
			if (Vector3.Distance(this.LastCheckedPosition, this.LocalPlayer.position) > this.RefreshDistance || this.NeedsRefresh)
			{
				this.LastCheckedPosition = this.LocalPlayer.position;
				this.HandleTracking();
				this.NeedsRefresh = false;
			}
			return true;
		}

		// Token: 0x0600C6BB RID: 50875 RVA: 0x00493518 File Offset: 0x00491718
		public void AddTrackingEntry(TrackingEntry track)
		{
			if (!this.trackingEntries.Contains(track))
			{
				this.trackingEntries.Add(track);
			}
			QuestEventManager.Current.AddTrackerToBeUpdated(this);
			this.NeedsRefresh = true;
		}

		// Token: 0x0600C6BC RID: 50876 RVA: 0x00493546 File Offset: 0x00491746
		public void RemoveTrackingEntry(TrackingEntry track)
		{
			if (this.trackingEntries.Contains(track))
			{
				this.trackingEntries.Remove(track);
				this.NeedsRefresh = true;
			}
			if (this.trackingEntries.Count == 0)
			{
				QuestEventManager.Current.RemoveTrackerToBeUpdated(this);
			}
		}

		// Token: 0x0600C6BD RID: 50877 RVA: 0x00493584 File Offset: 0x00491784
		[PublicizedFrom(EAccessModifier.Protected)]
		public void HandleTracking()
		{
			NavObjectManager instance = NavObjectManager.Instance;
			List<Chunk> chunkArrayCopySync = GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync();
			for (int i = 0; i < this.trackingEntries.Count; i++)
			{
				this.trackingEntries[i].StartUpdate();
			}
			if (!this.LocalPlayer.IsInTrader)
			{
				foreach (Chunk c in chunkArrayCopySync)
				{
					for (int j = 0; j < this.trackingEntries.Count; j++)
					{
						this.trackingEntries[j].HandleTrack(c);
					}
				}
			}
			for (int k = 0; k < this.trackingEntries.Count; k++)
			{
				this.trackingEntries[k].EndUpdate();
			}
		}

		// Token: 0x04009606 RID: 38406
		public Challenge Owner;

		// Token: 0x04009607 RID: 38407
		public EntityPlayerLocal LocalPlayer;

		// Token: 0x04009608 RID: 38408
		public List<TrackingEntry> trackingEntries = new List<TrackingEntry>();

		// Token: 0x04009609 RID: 38409
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3 LastCheckedPosition = new Vector3(0f, 9999f, 0f);

		// Token: 0x0400960A RID: 38410
		public float RefreshDistance = 5f;

		// Token: 0x0400960B RID: 38411
		public bool NeedsRefresh;

		// Token: 0x0400960C RID: 38412
		[PublicizedFrom(EAccessModifier.Private)]
		public bool lastInTrader;
	}
}
