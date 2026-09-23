using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020011D0 RID: 4560
[Preserve]
public class GameModeSurvival : GameModeAbstract
{
	// Token: 0x060091E7 RID: 37351 RVA: 0x0036FF0C File Offset: 0x0036E10C
	public override string GetName()
	{
		return "gmSurvival";
	}

	// Token: 0x060091E8 RID: 37352 RVA: 0x0036FF13 File Offset: 0x0036E113
	public override string GetDescription()
	{
		return "gmSurvivalDesc";
	}

	// Token: 0x060091E9 RID: 37353 RVA: 0x0002003D File Offset: 0x0001E23D
	public override int GetID()
	{
		return 1;
	}

	// Token: 0x060091EA RID: 37354 RVA: 0x0036FF1C File Offset: 0x0036E11C
	public override GameMode.ModeGamePref[] GetSupportedGamePrefsInfo()
	{
		return new GameMode.ModeGamePref[]
		{
			new GameMode.ModeGamePref(EnumGamePrefs.GameDifficulty, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayNightLength, GamePrefs.EnumType.Int, 60, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DayLightLength, GamePrefs.EnumType.Int, 18, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BloodMoonFrequency, GamePrefs.EnumType.Int, 7, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BloodMoonRange, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BloodMoonWarning, GamePrefs.EnumType.Int, 8, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ZombieFeralSense, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ZombieMove, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ZombieMoveNight, GamePrefs.EnumType.Int, 3, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ZombieFeralMove, GamePrefs.EnumType.Int, 3, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ZombieBMMove, GamePrefs.EnumType.Int, 3, null),
			new GameMode.ModeGamePref(EnumGamePrefs.MaxSpawnedZombies, GamePrefs.EnumType.Int, 64, null),
			new GameMode.ModeGamePref(EnumGamePrefs.MaxSpawnedAnimals, GamePrefs.EnumType.Int, 50, null),
			new GameMode.ModeGamePref(EnumGamePrefs.PersistentPlayerProfiles, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.XPMultiplier, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BlockDamagePlayer, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BlockDamageAI, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BlockDamageAIBM, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LootAbundance, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LootRespawnDays, GamePrefs.EnumType.Int, 7, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DeathPenalty, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DropOnDeath, GamePrefs.EnumType.Int, 1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.DropOnQuit, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BloodMoonEnemyCount, GamePrefs.EnumType.Int, 8, null),
			new GameMode.ModeGamePref(EnumGamePrefs.EnemySpawnMode, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.EnemyDifficulty, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerEnabled, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerIsPublic, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerVisibility, GamePrefs.EnumType.Int, 2, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerMaxPlayerCount, GamePrefs.EnumType.Int, 8, new Dictionary<DeviceFlag, object>
			{
				{
					DeviceFlag.PS5,
					(GameModeSurvival.OverrideMaxPlayerCount == -1) ? 4 : GameModeSurvival.OverrideMaxPlayerCount
				},
				{
					DeviceFlag.XBoxSeriesX,
					(GameModeSurvival.OverrideMaxPlayerCount == -1) ? 4 : GameModeSurvival.OverrideMaxPlayerCount
				},
				{
					DeviceFlag.XBoxSeriesS,
					(GameModeSurvival.OverrideMaxPlayerCount == -1) ? 2 : GameModeSurvival.OverrideMaxPlayerCount
				}
			}),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerPassword, GamePrefs.EnumType.String, "", null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerPort, GamePrefs.EnumType.Int, Constants.cDefaultPort, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerAllowCrossplay, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ServerEACPeerToPeer, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.PlayerKillingMode, GamePrefs.EnumType.Int, EnumPlayerKillingMode.KillStrangersOnly, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimCount, GamePrefs.EnumType.Int, 5, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimSize, GamePrefs.EnumType.Int, 41, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimDeadZone, GamePrefs.EnumType.Int, 30, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimExpiryTime, GamePrefs.EnumType.Int, 7, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimDecayMode, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimOnlineDurabilityModifier, GamePrefs.EnumType.Int, 4, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimOfflineDurabilityModifier, GamePrefs.EnumType.Int, 4, null),
			new GameMode.ModeGamePref(EnumGamePrefs.LandClaimOfflineDelay, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BedrollDeadZoneSize, GamePrefs.EnumType.Int, 15, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BedrollExpiryTime, GamePrefs.EnumType.Int, 45, null),
			new GameMode.ModeGamePref(EnumGamePrefs.AirDropFrequency, GamePrefs.EnumType.Int, 3, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BuildCreate, GamePrefs.EnumType.Bool, false, null),
			new GameMode.ModeGamePref(EnumGamePrefs.AirDropMarker, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.PartySharedKillRange, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ShowFriendPlayerOnMap, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.MaxChunkAge, GamePrefs.EnumType.Int, -1, null),
			new GameMode.ModeGamePref(EnumGamePrefs.ResetUnprotectedChunks, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.QuestProgressionDailyLimit, GamePrefs.EnumType.Int, 4, null),
			new GameMode.ModeGamePref(EnumGamePrefs.BiomeProgression, GamePrefs.EnumType.Bool, true, null),
			new GameMode.ModeGamePref(EnumGamePrefs.StormFreq, GamePrefs.EnumType.Int, 100, null),
			new GameMode.ModeGamePref(EnumGamePrefs.CameraRestrictionMode, GamePrefs.EnumType.Int, 0, null),
			new GameMode.ModeGamePref(EnumGamePrefs.AISmellMode, GamePrefs.EnumType.Int, 3, null),
			new GameMode.ModeGamePref(EnumGamePrefs.JarRefund, GamePrefs.EnumType.Int, 60, null),
			new GameMode.ModeGamePref(EnumGamePrefs.SandboxPreset, GamePrefs.EnumType.String, "", null),
			new GameMode.ModeGamePref(EnumGamePrefs.SandboxCode, GamePrefs.EnumType.String, "", null)
		};
	}

	// Token: 0x060091EB RID: 37355 RVA: 0x00370528 File Offset: 0x0036E728
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
		GameStats.Set(EnumGameStats.IsFlyingEnabled, GamePrefs.GetBool(EnumGamePrefs.BuildCreate));
		GameStats.Set(EnumGameStats.AutoParty, false);
	}

	// Token: 0x060091EC RID: 37356 RVA: 0x0002003D File Offset: 0x0001E23D
	public override int GetRoundCount()
	{
		return 1;
	}

	// Token: 0x060091ED RID: 37357 RVA: 0x0036FA24 File Offset: 0x0036DC24
	public override void StartRound(int _idx)
	{
		GameStats.Set(EnumGameStats.GameState, 1);
	}

	// Token: 0x060091EE RID: 37358 RVA: 0x000027FC File Offset: 0x000009FC
	public override void EndRound(int _idx)
	{
	}

	// Token: 0x04006BF2 RID: 27634
	public static readonly string TypeName = typeof(GameModeSurvival).Name;

	// Token: 0x04006BF3 RID: 27635
	public static int OverrideMaxPlayerCount = -1;
}
