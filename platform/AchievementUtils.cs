using System;

namespace Platform
{
	// Token: 0x02001B57 RID: 6999
	public static class AchievementUtils
	{
		// Token: 0x0600D1C0 RID: 53696 RVA: 0x004C9E68 File Offset: 0x004C8068
		public static bool IsCreativeModeActive()
		{
			return GamePrefs.GetString(EnumGamePrefs.GameMode).Equals(GameModeCreative.TypeName) || GameStats.GetBool(EnumGameStats.IsCreativeMenuEnabled) || GamePrefs.GetBool(EnumGamePrefs.CreativeMenuEnabled) || GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled);
		}
	}
}
