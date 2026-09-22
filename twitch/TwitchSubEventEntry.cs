using System;

namespace Twitch
{
	// Token: 0x0200183F RID: 6207
	public class TwitchSubEventEntry : TwitchEventEntry
	{
		// Token: 0x0600BF72 RID: 49010 RVA: 0x0046CD64 File Offset: 0x0046AF64
		public TwitchSubEventEntry()
		{
			this.RewardsBitPot = true;
		}

		// Token: 0x0600BF73 RID: 49011 RVA: 0x0046CD73 File Offset: 0x0046AF73
		public override bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return (this.StartAmount == -1 || amount >= this.StartAmount) && (this.EndAmount == -1 || amount <= this.EndAmount) && (this.SubTier == TwitchSubEventEntry.SubTierTypes.Any || this.SubTier == subTier);
		}

		// Token: 0x0600BF74 RID: 49012 RVA: 0x0046CDAE File Offset: 0x0046AFAE
		public override string Description(TwitchEventActionEntry entry)
		{
			return this.EventTitle;
		}

		// Token: 0x0600BF75 RID: 49013 RVA: 0x0046CDB6 File Offset: 0x0046AFB6
		public static TwitchSubEventEntry.SubTierTypes GetSubTier(string subPlan)
		{
			if (subPlan == "1000")
			{
				return TwitchSubEventEntry.SubTierTypes.Tier1;
			}
			if (subPlan == "2000")
			{
				return TwitchSubEventEntry.SubTierTypes.Tier2;
			}
			if (!(subPlan == "3000"))
			{
				return TwitchSubEventEntry.SubTierTypes.Prime;
			}
			return TwitchSubEventEntry.SubTierTypes.Tier3;
		}

		// Token: 0x04009026 RID: 36902
		public TwitchSubEventEntry.SubTierTypes SubTier;

		// Token: 0x02001840 RID: 6208
		public enum SubTierTypes
		{
			// Token: 0x04009028 RID: 36904
			Any,
			// Token: 0x04009029 RID: 36905
			Prime,
			// Token: 0x0400902A RID: 36906
			Tier1,
			// Token: 0x0400902B RID: 36907
			Tier2,
			// Token: 0x0400902C RID: 36908
			Tier3
		}
	}
}
