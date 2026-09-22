using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200186A RID: 6250
	public class TwitchVoteEntry
	{
		// Token: 0x0600C108 RID: 49416 RVA: 0x00479F91 File Offset: 0x00478191
		public TwitchVoteEntry(string voteCommand, TwitchVote voteClass)
		{
			this.VoteCommand = voteCommand;
			this.VoteClass = voteClass;
		}

		// Token: 0x04009216 RID: 37398
		public TwitchVote VoteClass;

		// Token: 0x04009217 RID: 37399
		public string VoteCommand;

		// Token: 0x04009218 RID: 37400
		public TwitchVotingManager Owner;

		// Token: 0x04009219 RID: 37401
		public int VoteCount;

		// Token: 0x0400921A RID: 37402
		public int Index = -1;

		// Token: 0x0400921B RID: 37403
		public bool UIDirty = true;

		// Token: 0x0400921C RID: 37404
		public bool Complete;

		// Token: 0x0400921D RID: 37405
		public List<string> VoterNames = new List<string>();

		// Token: 0x0400921E RID: 37406
		public List<int> ActiveSpawns = new List<int>();
	}
}
