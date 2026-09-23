using System;

// Token: 0x020011D2 RID: 4562
public class GameModeSurvivalPvP : GameModeAbstract
{
	// Token: 0x060091FB RID: 37371 RVA: 0x003709E7 File Offset: 0x0036EBE7
	public override string GetName()
	{
		return "gmSurvivalPvP";
	}

	// Token: 0x060091FC RID: 37372 RVA: 0x003709EE File Offset: 0x0036EBEE
	public override string GetDescription()
	{
		return "gmSurvivalPvPDesc";
	}

	// Token: 0x060091FD RID: 37373 RVA: 0x00080A2D File Offset: 0x0007EC2D
	public override int GetID()
	{
		return 5;
	}

	// Token: 0x060091FE RID: 37374 RVA: 0x003709F8 File Offset: 0x0036EBF8
	public override GameMode.ModeGamePref[] GetSupportedGamePrefsInfo()
	{
		return new GameMode.ModeGamePref[]
		{
			new GameMode.ModeGamePref(EnumGamePrefs.GameDifficulty, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayNightLength, GamePrefs.EnumType.Int, 60, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayLightLength, GamePrefs.EnumType.Int, 18, null),
			new GameMode.ModeGamePref(EnumGamePrefs.PlayerKillingMode, GamePrefs.EnumType.Int, EnumPlayerKillingMode.KillEveryone, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ShowFriendPlayerOnMap, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LootAbundance, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LootRespawnDays, GamePrefs.EnumType.Int, 7, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DropOnDeath, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DropOnQuit, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BloodMoonEnemyCount, GamePrefs.EnumType.Int, 8, null),
			new GameMode.ModeGamePref(EnumGamePrefs.EnemySpawnMode, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BuildCreate, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerIsPublic, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerMaxPlayerCount, GamePrefs.EnumType.Int, 4, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerPassword, GamePrefs.EnumType.String, "", null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerPort, GamePrefs.EnumType.Int, Constants.cDefaultPort, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimCount, GamePrefs.EnumType.Int, 5, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimSize, GamePrefs.EnumType.Int, 41, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimDeadZone, GamePrefs.EnumType.Int, 30, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimExpiryTime, GamePrefs.EnumType.Int, 3, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimDecayMode, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimOnlineDurabilityModifier, GamePrefs.EnumType.Int, 32, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimOfflineDurabilityModifier, GamePrefs.EnumType.Int, 32, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimOfflineDelay, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BedrollDeadZoneSize, GamePrefs.EnumType.Int, 15, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BedrollExpiryTime, GamePrefs.EnumType.Int, 45, null),
			new GameMode.ModeGamePref(EnumGamePrefs.MaxChunkAge, GamePrefs.EnumType.Int, -1, null)
		};
	}

	// Token: 0x060091FF RID: 37375 RVA: 0x00370C88 File Offset: 0x0036EE88
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
	}

	// Token: 0x06009200 RID: 37376 RVA: 0x0002003D File Offset: 0x0001E23D
	public override int GetRoundCount()
	{
		return 1;
	}

	// Token: 0x06009201 RID: 37377 RVA: 0x0036FA24 File Offset: 0x0036DC24
	public override void StartRound(int _idx)
	{
		GameStats.Set(EnumGameStats.GameState, 1);
	}

	// Token: 0x06009202 RID: 37378 RVA: 0x000027FC File Offset: 0x000009FC
	public override void EndRound(int _idx)
	{
	}

	// Token: 0x04006BF5 RID: 27637
	public static readonly string TypeName = typeof(GameModeSurvivalPvP).Name;
}
