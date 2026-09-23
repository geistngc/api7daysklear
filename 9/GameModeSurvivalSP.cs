using System;
using UnityEngine.Scripting;

// Token: 0x020011D3 RID: 4563
[Preserve]
public class GameModeSurvivalSP : GameModeAbstract
{
	// Token: 0x06009205 RID: 37381 RVA: 0x00370D09 File Offset: 0x0036EF09
	public override string GetName()
	{
		return "gmSurvivalSP";
	}

	// Token: 0x06009206 RID: 37382 RVA: 0x00370D10 File Offset: 0x0036EF10
	public override string GetDescription()
	{
		return "gmSurvivalSPDesc";
	}

	// Token: 0x06009207 RID: 37383 RVA: 0x00081B10 File Offset: 0x0007FD10
	public override int GetID()
	{
		return 6;
	}

	// Token: 0x06009208 RID: 37384 RVA: 0x00370D18 File Offset: 0x0036EF18
	public override GameMode.ModeGamePref[] GetSupportedGamePrefsInfo()
	{
		return new GameMode.ModeGamePref[]
		{
			new GameMode.ModeGamePref(EnumGamePrefs.GameDifficulty, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayNightLength, GamePrefs.EnumType.Int, 60, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayLightLength, GamePrefs.EnumType.Int, 18, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LootAbundance, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LootRespawnDays, GamePrefs.EnumType.Int, 7, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DropOnDeath, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BloodMoonEnemyCount, GamePrefs.EnumType.Int, 8, null),
			new GameMode.ModeGamePref(EnumGamePrefs.EnemySpawnMode, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.EnemyDifficulty, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.AirDropFrequency, GamePrefs.EnumType.Int, 3, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BuildCreate, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.PersistentPlayerProfiles, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.AirDropMarker, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BedrollDeadZoneSize, GamePrefs.EnumType.Int, 15, null),
			new GameMode.ModeGamePref(EnumGamePrefs.MaxChunkAge, GamePrefs.EnumType.Int, -1, null)
		};
	}

	// Token: 0x06009209 RID: 37385 RVA: 0x00370E8C File Offset: 0x0036F08C
	public override void Init()
	{
		base.Init();
		GameStats.Set(EnumGameStats.ShowSpawnWindow, false);
		GameStats.Set(EnumGameStats.TimeLimitActive, false);
		GameStats.Set(EnumGameStats.DayLimitActive, false);
		GameStats.Set(EnumGameStats.ShowWindow, "");
		GameStats.Set(EnumGameStats.IsSpawnEnemies, GamePrefs.GetBool(EnumGamePrefs.EnemySpawnMode));
		GameStats.Set(EnumGameStats.ScorePlayerKillMultiplier, 0);
		GameStats.Set(EnumGameStats.ScoreZombieKillMultiplier, 1);
		GameStats.Set(EnumGameStats.ScoreDiedMultiplier, -5);
		GameStats.Set(EnumGameStats.IsSpawnNearOtherPlayer, false);
		GameStats.Set(EnumGameStats.ZombieHordeMeter, true);
		GameStats.Set(EnumGameStats.DropOnQuit, 0);
		GamePrefs.Set(EnumGamePrefs.ServerMaxPlayerCount, 1);
		GamePrefs.Set(EnumGamePrefs.ServerIsPublic, false);
		GamePrefs.Set(EnumGamePrefs.ServerPort, Constants.cDefaultPort);
		GameStats.Set(EnumGameStats.IsFlyingEnabled, GamePrefs.GetBool(EnumGamePrefs.BuildCreate));
	}

	// Token: 0x0600920A RID: 37386 RVA: 0x0002003D File Offset: 0x0001E23D
	public override int GetRoundCount()
	{
		return 1;
	}

	// Token: 0x0600920B RID: 37387 RVA: 0x0036FA24 File Offset: 0x0036DC24
	public override void StartRound(int _idx)
	{
		GameStats.Set(EnumGameStats.GameState, 1);
	}

	// Token: 0x0600920C RID: 37388 RVA: 0x000027FC File Offset: 0x000009FC
	public override void EndRound(int _idx)
	{
	}

	// Token: 0x04006BF6 RID: 27638
	public static readonly string TypeName = typeof(GameModeSurvivalSP).Name;
}
