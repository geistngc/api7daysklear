using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020018FC RID: 6396
	[Preserve]
	public class ChallengeBaseTrackedItemObjective : BaseChallengeObjective
	{
		// Token: 0x0600C53E RID: 50494 RVA: 0x0048D0E2 File Offset: 0x0048B2E2
		public override void Init()
		{
			this.expectedItem = ItemClass.GetItem(this.itemClassID, false);
			this.expectedItemClass = ItemClass.GetItemClass(this.itemClassID, false);
		}

		// Token: 0x0600C53F RID: 50495 RVA: 0x0048D108 File Offset: 0x0048B308
		public void SetupItem(string itemID)
		{
			this.itemClassID = itemID;
		}

		// Token: 0x0600C540 RID: 50496 RVA: 0x0048D114 File Offset: 0x0048B314
		public override void HandleAddHooks()
		{
			if (this.expectedItemClass != null)
			{
				string text = (this.overrideTrackerIndexName != null) ? this.overrideTrackerIndexName : this.expectedItemClass.TrackerIndexName;
				if (text != null && this.trackingEntry == null && !this.disableTracking)
				{
					this.trackingEntry = new TrackingEntry
					{
						TrackedItem = this.expectedItemClass,
						Owner = this,
						blockIndexName = text,
						navObjectName = ((this.expectedItemClass.TrackerNavObject != null) ? this.expectedItemClass.TrackerNavObject : "quest_resource"),
						trackDistance = this.trackDistance
					};
					this.trackingEntry.TrackingHelper = this.Owner.GetTrackingHelper();
				}
			}
			base.HandleAddHooks();
		}

		// Token: 0x0600C541 RID: 50497 RVA: 0x0048D1D0 File Offset: 0x0048B3D0
		public override void HandleTrackingStarted()
		{
			base.HandleTrackingStarted();
			if (this.trackingEntry != null)
			{
				this.Owner.AddTrackingEntry(this.trackingEntry);
				this.trackingEntry.TrackingHelper = this.Owner.TrackingHandler;
				this.trackingEntry.AddHooks();
			}
		}

		// Token: 0x0600C542 RID: 50498 RVA: 0x0048D21D File Offset: 0x0048B41D
		public override void HandleTrackingEnded()
		{
			base.HandleTrackingEnded();
			if (this.trackingEntry != null)
			{
				this.trackingEntry.RemoveHooks();
				this.Owner.RemoveTrackingEntry(this.trackingEntry);
			}
		}

		// Token: 0x0600C543 RID: 50499 RVA: 0x0048D24C File Offset: 0x0048B44C
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.itemClassID = e.GetAttribute("item");
			}
			if (e.HasAttribute("override_tracker_index"))
			{
				this.overrideTrackerIndexName = e.GetAttribute("override_tracker_index");
			}
			if (e.HasAttribute("track_distance"))
			{
				this.trackDistance = StringParsers.ParseFloat(e.GetAttribute("track_distance"), 0, -1, NumberStyles.Any);
			}
			if (e.HasAttribute("disable_tracking"))
			{
				this.disableTracking = StringParsers.ParseBool(e.GetAttribute("disable_tracking"), 0, -1, true);
			}
		}

		// Token: 0x0600C544 RID: 50500 RVA: 0x0048D314 File Offset: 0x0048B514
		public override void CopyValues(BaseChallengeObjective obj, BaseChallengeObjective objFromClass)
		{
			base.CopyValues(obj, objFromClass);
			ChallengeBaseTrackedItemObjective challengeBaseTrackedItemObjective = objFromClass as ChallengeBaseTrackedItemObjective;
			if (challengeBaseTrackedItemObjective != null)
			{
				this.itemClassID = challengeBaseTrackedItemObjective.itemClassID;
				this.overrideTrackerIndexName = challengeBaseTrackedItemObjective.overrideTrackerIndexName;
				this.trackDistance = challengeBaseTrackedItemObjective.trackDistance;
				this.disableTracking = challengeBaseTrackedItemObjective.disableTracking;
			}
		}

		// Token: 0x0600C545 RID: 50501 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		public override BaseChallengeObjective Clone()
		{
			return null;
		}

		// Token: 0x0600C546 RID: 50502 RVA: 0x0048D363 File Offset: 0x0048B563
		public override void CompleteObjective(bool handleComplete = true)
		{
			base.Current = this.MaxCount;
			base.Complete = true;
			if (handleComplete)
			{
				this.Owner.HandleComplete(true, false);
			}
		}

		// Token: 0x04009578 RID: 38264
		[PublicizedFrom(EAccessModifier.Protected)]
		public ItemValue expectedItem = ItemValue.None;

		// Token: 0x04009579 RID: 38265
		[PublicizedFrom(EAccessModifier.Protected)]
		public ItemClass expectedItemClass;

		// Token: 0x0400957A RID: 38266
		[PublicizedFrom(EAccessModifier.Protected)]
		public string itemClassID = "";

		// Token: 0x0400957B RID: 38267
		[PublicizedFrom(EAccessModifier.Protected)]
		public string overrideTrackerIndexName;

		// Token: 0x0400957C RID: 38268
		[PublicizedFrom(EAccessModifier.Protected)]
		public float trackDistance = 20f;

		// Token: 0x0400957D RID: 38269
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool disableTracking;

		// Token: 0x0400957E RID: 38270
		public TrackingEntry trackingEntry;
	}
}
