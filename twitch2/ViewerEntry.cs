using System;
using UnityEngine;

namespace Twitch
{
	// Token: 0x02001866 RID: 6246
	public class ViewerEntry
	{
		// Token: 0x1700179E RID: 6046
		// (get) Token: 0x0600C0F8 RID: 49400 RVA: 0x00479562 File Offset: 0x00477762
		public float CombinedPoints
		{
			get
			{
				return this.SpecialPoints + this.StandardPoints;
			}
		}

		// Token: 0x0600C0F9 RID: 49401 RVA: 0x00479574 File Offset: 0x00477774
		public void RemovePoints(float usedPoints, TwitchAction.PointTypes pointType, TwitchActionEntry entry)
		{
			if (pointType == TwitchAction.PointTypes.SP)
			{
				this.SpecialPoints -= usedPoints;
				entry.SpecialPointsUsed = (int)usedPoints;
				return;
			}
			if (pointType == TwitchAction.PointTypes.PP)
			{
				float num = Mathf.Min(usedPoints, this.StandardPoints);
				entry.StandardPointsUsed = (int)num;
				this.StandardPoints -= num;
				num = usedPoints - num;
				if (num > 0f)
				{
					this.SpecialPoints -= num;
					entry.SpecialPointsUsed = (int)num;
					return;
				}
			}
			else if (pointType == TwitchAction.PointTypes.Bits)
			{
				int num2 = Utils.FastMin((int)usedPoints, (ExtensionManager.Version == "2.0.1") ? this.BitCredits : TwitchAction.GetAdjustedBitPriceFloor(this.BitCredits));
				this.BitCredits -= num2;
				entry.CreditsUsed = num2;
				entry.BitsUsed = (int)usedPoints;
				TwitchLeaderboardStats leaderboardStats = TwitchManager.LeaderboardStats;
				int num3 = (ExtensionManager.Version == "2.0.1") ? (entry.BitsUsed - num2) : TwitchAction.GetAdjustedBitPriceCeil(entry.BitsUsed - num2);
				if (num3 > 0)
				{
					leaderboardStats.TotalBits += num3;
					leaderboardStats.CheckMostBitsSpent(leaderboardStats.AddBitsUsed(entry.UserName, this.UserColor, num3));
				}
			}
		}

		// Token: 0x040091CA RID: 37322
		public float SpecialPoints;

		// Token: 0x040091CB RID: 37323
		public float StandardPoints;

		// Token: 0x040091CC RID: 37324
		public int BitCredits;

		// Token: 0x040091CD RID: 37325
		public int UserID = -1;

		// Token: 0x040091CE RID: 37326
		public string UserColor = "FFFFFF";

		// Token: 0x040091CF RID: 37327
		public float LastAction = -1f;

		// Token: 0x040091D0 RID: 37328
		public float addPointsUntil;

		// Token: 0x040091D1 RID: 37329
		public bool IsActive;

		// Token: 0x040091D2 RID: 37330
		public bool IsSub;
	}
}
