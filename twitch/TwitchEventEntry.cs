using System;

namespace Twitch
{
	// Token: 0x0200183B RID: 6203
	public class TwitchEventEntry : BaseTwitchEventEntry
	{
		// Token: 0x0600BF4D RID: 48973 RVA: 0x0046C681 File Offset: 0x0046A881
		public override bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return (this.StartAmount == -1 || amount >= this.StartAmount) && (this.EndAmount == -1 || amount <= this.EndAmount);
		}

		// Token: 0x04009010 RID: 36880
		public int StartAmount = -1;

		// Token: 0x04009011 RID: 36881
		public int EndAmount = -1;
	}
}
