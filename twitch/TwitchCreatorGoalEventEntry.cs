using System;

namespace Twitch
{
	// Token: 0x0200183A RID: 6202
	public class TwitchCreatorGoalEventEntry : BaseTwitchEventEntry
	{
		// Token: 0x0600BF4B RID: 48971 RVA: 0x0046C658 File Offset: 0x0046A858
		public override bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return name == this.GoalType;
		}

		// Token: 0x0400900D RID: 36877
		public string GoalType = "Subs";

		// Token: 0x0400900E RID: 36878
		public int RewardAmount = 100;

		// Token: 0x0400900F RID: 36879
		public TwitchAction.PointTypes RewardType;
	}
}
