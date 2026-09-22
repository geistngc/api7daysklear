using System;

namespace Platform
{
	// Token: 0x02001B56 RID: 6998
	public interface IAchievementManager
	{
		// Token: 0x0600D1B8 RID: 53688
		void Init(IPlatform _owner);

		// Token: 0x0600D1B9 RID: 53689
		void ShowAchievementsUi();

		// Token: 0x0600D1BA RID: 53690
		bool IsAchievementStatSupported(EnumAchievementDataStat _stat);

		// Token: 0x0600D1BB RID: 53691
		void SetAchievementStat(EnumAchievementDataStat _stat, int _value);

		// Token: 0x0600D1BC RID: 53692
		void SetAchievementStat(EnumAchievementDataStat _stat, float _value);

		// Token: 0x0600D1BD RID: 53693
		void ResetStats(bool _andAchievements);

		// Token: 0x0600D1BE RID: 53694
		void UnlockAllAchievements();

		// Token: 0x0600D1BF RID: 53695
		void Destroy();
	}
}
