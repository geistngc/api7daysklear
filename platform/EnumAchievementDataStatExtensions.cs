using System;

namespace Platform
{
	// Token: 0x02001B33 RID: 6963
	public static class EnumAchievementDataStatExtensions
	{
		// Token: 0x0600D0DF RID: 53471 RVA: 0x004C659F File Offset: 0x004C479F
		public static bool IsSupported(this EnumAchievementDataStat _stat)
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			return achievementManager == null || achievementManager.IsAchievementStatSupported(_stat);
		}
	}
}
