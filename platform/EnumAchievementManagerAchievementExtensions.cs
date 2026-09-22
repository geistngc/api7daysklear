using System;

namespace Platform
{
	// Token: 0x02001B30 RID: 6960
	public static class EnumAchievementManagerAchievementExtensions
	{
		// Token: 0x0600D0DE RID: 53470 RVA: 0x004C6592 File Offset: 0x004C4792
		public static bool IsSupported(this EnumAchievementManagerAchievement _achievement)
		{
			return AchievementData.GetStat(_achievement).IsSupported();
		}
	}
}
