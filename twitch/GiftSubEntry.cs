using System;

namespace Twitch
{
	// Token: 0x02001867 RID: 6247
	public class GiftSubEntry
	{
		// Token: 0x0600C0FB RID: 49403 RVA: 0x004796B8 File Offset: 0x004778B8
		public GiftSubEntry(string userName, int userID, TwitchSubEventEntry.SubTierTypes tier, int subCount)
		{
			this.UserName = userName;
			this.UserID = userID;
			this.TimeRemaining = 1f;
			this.SubCount = subCount;
			this.Tier = tier;
		}

		// Token: 0x0600C0FC RID: 49404 RVA: 0x00479717 File Offset: 0x00477917
		public void AddSub()
		{
			this.SubCount++;
			this.TimeRemaining = 1f;
		}

		// Token: 0x0600C0FD RID: 49405 RVA: 0x00479732 File Offset: 0x00477932
		public bool Update(float deltaTime)
		{
			this.TimeRemaining -= deltaTime;
			return this.TimeRemaining <= 0f;
		}

		// Token: 0x040091D3 RID: 37331
		public float TimeRemaining = 1f;

		// Token: 0x040091D4 RID: 37332
		public string UserName = "";

		// Token: 0x040091D5 RID: 37333
		public int UserID = -1;

		// Token: 0x040091D6 RID: 37334
		public int SubCount;

		// Token: 0x040091D7 RID: 37335
		public TwitchSubEventEntry.SubTierTypes Tier = TwitchSubEventEntry.SubTierTypes.Tier1;
	}
}
