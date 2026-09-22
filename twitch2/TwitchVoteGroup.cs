using System;
using System.Collections.Generic;
using UnityEngine;

namespace Twitch
{
	// Token: 0x0200186B RID: 6251
	public class TwitchVoteGroup
	{
		// Token: 0x0600C109 RID: 49417 RVA: 0x00479FCB File Offset: 0x004781CB
		public TwitchVoteGroup(string name)
		{
			this.Name = name;
		}

		// Token: 0x0600C10A RID: 49418 RVA: 0x00479FF0 File Offset: 0x004781F0
		public TwitchVoteType GetNextVoteType()
		{
			this.index++;
			if (this.index >= this.VoteTypes.Count)
			{
				this.index = 0;
			}
			return this.VoteTypes[this.index];
		}

		// Token: 0x0600C10B RID: 49419 RVA: 0x0047A02C File Offset: 0x0047822C
		public void ShuffleVoteTypes()
		{
			for (int i = 0; i <= this.VoteTypes.Count * this.VoteTypes.Count; i++)
			{
				int num = UnityEngine.Random.Range(0, this.VoteTypes.Count);
				int num2 = UnityEngine.Random.Range(0, this.VoteTypes.Count);
				if (num != num2)
				{
					TwitchVoteType value = this.VoteTypes[num];
					this.VoteTypes[num] = this.VoteTypes[num2];
					this.VoteTypes[num2] = value;
				}
			}
		}

		// Token: 0x0400921F RID: 37407
		public string Name = "";

		// Token: 0x04009220 RID: 37408
		public List<TwitchVoteType> VoteTypes = new List<TwitchVoteType>();

		// Token: 0x04009221 RID: 37409
		[PublicizedFrom(EAccessModifier.Private)]
		public int index;

		// Token: 0x04009222 RID: 37410
		public bool SkippedThisVote;
	}
}
