using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001922 RID: 6434
	[Preserve]
	public class TrackingEntry
	{
		// Token: 0x0600C6B1 RID: 50865 RVA: 0x00493100 File Offset: 0x00491300
		public void AddHooks()
		{
			if (this.TrackingHelper != null)
			{
				this.TrackingHelper.AddTrackingEntry(this);
			}
			QuestEventManager.Current.BlockChange -= this.Current_BlockChange;
			QuestEventManager.Current.BlockChange += this.Current_BlockChange;
		}

		// Token: 0x0600C6B2 RID: 50866 RVA: 0x00493150 File Offset: 0x00491350
		public void RemoveHooks()
		{
			if (this.TrackingHelper != null)
			{
				this.TrackingHelper.RemoveTrackingEntry(this);
			}
			QuestEventManager.Current.BlockChange -= this.Current_BlockChange;
			NavObjectManager instance = NavObjectManager.Instance;
			for (int i = this.TrackedBlocks.Count - 1; i >= 0; i--)
			{
				instance.UnRegisterNavObject(this.TrackedBlocks[i].NavObject);
				this.TrackedBlocks.RemoveAt(i);
			}
		}

		// Token: 0x0600C6B3 RID: 50867 RVA: 0x004931C8 File Offset: 0x004913C8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BlockChange(Block blockOld, Block blockNew, Vector3i blockPos)
		{
			if (blockOld.IndexName == this.blockIndexName)
			{
				for (int i = 0; i < this.TrackedBlocks.Count; i++)
				{
					if (this.TrackedBlocks[i].WorldPos == blockPos)
					{
						NavObjectManager.Instance.UnRegisterNavObject(this.TrackedBlocks[i].NavObject);
						this.TrackedBlocks.RemoveAt(i);
						return;
					}
				}
			}
		}

		// Token: 0x0600C6B4 RID: 50868 RVA: 0x00493240 File Offset: 0x00491440
		public void StartUpdate()
		{
			if (this.localPlayer == null)
			{
				this.localPlayer = this.Owner.Owner.Owner.Player;
			}
			for (int i = 0; i < this.TrackedBlocks.Count; i++)
			{
				this.TrackedBlocks[i].KeepAlive = false;
			}
		}

		// Token: 0x0600C6B5 RID: 50869 RVA: 0x004932A0 File Offset: 0x004914A0
		public void HandleTrack(Chunk c)
		{
			List<Vector3i> list;
			if (c.IndexedBlocks.TryGetValue(this.blockIndexName, out list))
			{
				foreach (Vector3i pos in list)
				{
					Vector3i vector3i = c.ToWorldPos(pos);
					if (!c.GetBlock(pos).ischild && Vector3.Distance(vector3i, this.localPlayer.position) < this.trackDistance)
					{
						this.HandleAddTrackedBlock(vector3i);
					}
				}
			}
		}

		// Token: 0x0600C6B6 RID: 50870 RVA: 0x0049333C File Offset: 0x0049153C
		public void EndUpdate()
		{
			NavObjectManager instance = NavObjectManager.Instance;
			for (int i = this.TrackedBlocks.Count - 1; i >= 0; i--)
			{
				if (!this.TrackedBlocks[i].KeepAlive)
				{
					instance.UnRegisterNavObject(this.TrackedBlocks[i].NavObject);
					this.TrackedBlocks.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600C6B7 RID: 50871 RVA: 0x004933A0 File Offset: 0x004915A0
		[PublicizedFrom(EAccessModifier.Protected)]
		public void HandleAddTrackedBlock(Vector3i pos)
		{
			for (int i = 0; i < this.TrackedBlocks.Count; i++)
			{
				if (pos == this.TrackedBlocks[i].WorldPos)
				{
					this.TrackedBlocks[i].KeepAlive = true;
				}
			}
			this.TrackedBlocks.Add(new TrackingEntry.TrackedBlock(pos, this.navObjectName));
		}

		// Token: 0x040095FB RID: 38395
		public float trackDistance = 20f;

		// Token: 0x040095FC RID: 38396
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal localPlayer;

		// Token: 0x040095FD RID: 38397
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<TrackingEntry.TrackedBlock> TrackedBlocks = new List<TrackingEntry.TrackedBlock>();

		// Token: 0x040095FE RID: 38398
		public ItemClass TrackedItem;

		// Token: 0x040095FF RID: 38399
		public BaseChallengeObjective Owner;

		// Token: 0x04009600 RID: 38400
		public ChallengeTrackingHandler TrackingHelper;

		// Token: 0x04009601 RID: 38401
		public string blockIndexName = "quest_wood";

		// Token: 0x04009602 RID: 38402
		public string navObjectName = "quest_resource";

		// Token: 0x02001923 RID: 6435
		public class TrackedBlock
		{
			// Token: 0x0600C6B9 RID: 50873 RVA: 0x00493439 File Offset: 0x00491639
			public TrackedBlock(Vector3i worldPos, string NavObjectName)
			{
				this.WorldPos = worldPos;
				this.NavObject = NavObjectManager.Instance.RegisterNavObject(NavObjectName, this.WorldPos.ToVector3Center(), "", false, -1, null);
				this.KeepAlive = true;
			}

			// Token: 0x04009603 RID: 38403
			public Vector3i WorldPos;

			// Token: 0x04009604 RID: 38404
			public NavObject NavObject;

			// Token: 0x04009605 RID: 38405
			public bool KeepAlive;
		}
	}
}
