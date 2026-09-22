using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001901 RID: 6401
	[Preserve]
	public class ChallengeObjectiveChallengeStatAwarded : BaseChallengeObjective
	{
		// Token: 0x17001860 RID: 6240
		// (get) Token: 0x0600C571 RID: 50545 RVA: 0x0048DCB1 File Offset: 0x0048BEB1
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.ChallengeStatAwarded;
			}
		}

		// Token: 0x17001861 RID: 6241
		// (get) Token: 0x0600C572 RID: 50546 RVA: 0x0048DCB5 File Offset: 0x0048BEB5
		public override string DescriptionText
		{
			get
			{
				return Localization.Get(this.statText, false, null);
			}
		}

		// Token: 0x0600C573 RID: 50547 RVA: 0x0048DCC4 File Offset: 0x0048BEC4
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.ChallengeAwardCredit += this.Current_ChallengeAwardCredit;
			if (this.trackerIndexName != null && this.trackingEntry == null)
			{
				this.trackingEntry = new TrackingEntry
				{
					Owner = this,
					blockIndexName = this.trackerIndexName,
					navObjectName = ((this.trackerNavObjectName != null) ? this.trackerNavObjectName : "quest_resource"),
					trackDistance = this.trackDistance
				};
				this.trackingEntry.TrackingHelper = this.Owner.GetTrackingHelper();
			}
		}

		// Token: 0x0600C574 RID: 50548 RVA: 0x0048DD52 File Offset: 0x0048BF52
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.ChallengeAwardCredit -= this.Current_ChallengeAwardCredit;
		}

		// Token: 0x0600C575 RID: 50549 RVA: 0x0048DD6C File Offset: 0x0048BF6C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_ChallengeAwardCredit(string stat, int awardCount)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.challengeStat.EqualsCaseInsensitive(stat))
			{
				base.Current += awardCount;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600C576 RID: 50550 RVA: 0x0048DDC0 File Offset: 0x0048BFC0
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

		// Token: 0x0600C577 RID: 50551 RVA: 0x0048DE0D File Offset: 0x0048C00D
		public override void HandleTrackingEnded()
		{
			base.HandleTrackingEnded();
			if (this.trackingEntry != null)
			{
				this.trackingEntry.RemoveHooks();
				this.Owner.RemoveTrackingEntry(this.trackingEntry);
			}
		}

		// Token: 0x0600C578 RID: 50552 RVA: 0x0048DE3C File Offset: 0x0048C03C
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("challenge_stat"))
			{
				this.challengeStat = e.GetAttribute("challenge_stat");
			}
			if (e.HasAttribute("stat_text_key"))
			{
				this.statText = Localization.Get(e.GetAttribute("stat_text_key"), false, null);
			}
			else if (e.HasAttribute("stat_text"))
			{
				this.statText = e.GetAttribute("stat_text");
			}
			if (e.HasAttribute("tracker_index"))
			{
				this.trackerIndexName = e.GetAttribute("tracker_index");
			}
			if (e.HasAttribute("tracker_nav_object"))
			{
				this.trackerNavObjectName = e.GetAttribute("tracker_nav_object");
			}
			if (e.HasAttribute("track_distance"))
			{
				this.trackDistance = StringParsers.ParseFloat(e.GetAttribute("track_distance"), 0, -1, NumberStyles.Any);
			}
		}

		// Token: 0x0600C579 RID: 50553 RVA: 0x0048DF58 File Offset: 0x0048C158
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveChallengeStatAwarded
			{
				challengeStat = this.challengeStat,
				statText = this.statText,
				trackerIndexName = this.trackerIndexName,
				trackerNavObjectName = this.trackerNavObjectName,
				trackDistance = this.trackDistance
			};
		}

		// Token: 0x04009588 RID: 38280
		[PublicizedFrom(EAccessModifier.Private)]
		public string challengeStat = "";

		// Token: 0x04009589 RID: 38281
		[PublicizedFrom(EAccessModifier.Private)]
		public string statText = "";

		// Token: 0x0400958A RID: 38282
		[PublicizedFrom(EAccessModifier.Protected)]
		public string trackerIndexName;

		// Token: 0x0400958B RID: 38283
		[PublicizedFrom(EAccessModifier.Protected)]
		public string trackerNavObjectName;

		// Token: 0x0400958C RID: 38284
		[PublicizedFrom(EAccessModifier.Protected)]
		public float trackDistance = 20f;

		// Token: 0x0400958D RID: 38285
		public TrackingEntry trackingEntry;
	}
}
