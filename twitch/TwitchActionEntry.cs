using System;

namespace Twitch
{
	// Token: 0x02001801 RID: 6145
	public class TwitchActionEntry
	{
		// Token: 0x17001717 RID: 5911
		// (get) Token: 0x0600BE37 RID: 48695 RVA: 0x00468769 File Offset: 0x00466969
		public int ActionCost
		{
			get
			{
				return this.Action.CurrentCost;
			}
		}

		// Token: 0x0600BE38 RID: 48696 RVA: 0x00468778 File Offset: 0x00466978
		public TwitchActionHistoryEntry SetupHistoryEntry(ViewerEntry viewerEntry)
		{
			string target = (this.Target != null) ? this.Target.EntityName : "";
			this.HistoryEntry = new TwitchActionHistoryEntry(this.UserName, viewerEntry.UserColor, this.Action, null, null)
			{
				UserID = viewerEntry.UserID,
				PointsSpent = this.Action.CurrentCost,
				Target = target
			};
			this.HistoryEntry.ActionEntry = this;
			return this.HistoryEntry;
		}

		// Token: 0x04008F5A RID: 36698
		public string UserName;

		// Token: 0x04008F5B RID: 36699
		public EntityPlayer Target;

		// Token: 0x04008F5C RID: 36700
		public bool ReadyForRemove;

		// Token: 0x04008F5D RID: 36701
		public TwitchVoteEntry VoteEntry;

		// Token: 0x04008F5E RID: 36702
		public TwitchAction Action;

		// Token: 0x04008F5F RID: 36703
		public bool IsSent;

		// Token: 0x04008F60 RID: 36704
		public bool ChannelNotify = true;

		// Token: 0x04008F61 RID: 36705
		public bool IsBitAction;

		// Token: 0x04008F62 RID: 36706
		public bool IsReRun;

		// Token: 0x04008F63 RID: 36707
		public bool IsRespawn;

		// Token: 0x04008F64 RID: 36708
		public int SpecialPointsUsed;

		// Token: 0x04008F65 RID: 36709
		public int StandardPointsUsed;

		// Token: 0x04008F66 RID: 36710
		public int BitsUsed;

		// Token: 0x04008F67 RID: 36711
		public int CreditsUsed;

		// Token: 0x04008F68 RID: 36712
		public TwitchActionHistoryEntry HistoryEntry;
	}
}
