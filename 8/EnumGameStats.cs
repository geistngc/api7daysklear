using System;

// Token: 0x02001201 RID: 4609
public enum EnumGameStats
{
	// Token: 0x04006E13 RID: 28179
	GameState,
	// Token: 0x04006E14 RID: 28180
	GameModeId,
	// Token: 0x04006E15 RID: 28181
	TimeLimitActive,
	// Token: 0x04006E16 RID: 28182
	TimeLimitThisRound,
	// Token: 0x04006E17 RID: 28183
	DayLimitActive,
	// Token: 0x04006E18 RID: 28184
	DayLimitThisRound,
	// Token: 0x04006E19 RID: 28185
	FragLimitActive,
	// Token: 0x04006E1A RID: 28186
	FragLimitThisRound,
	// Token: 0x04006E1B RID: 28187
	ShowWindow,
	// Token: 0x04006E1C RID: 28188
	LoadScene,
	// Token: 0x04006E1D RID: 28189
	CurrentRoundIx,
	// Token: 0x04006E1E RID: 28190
	TimeOfDayIncPerSec,
	// Token: 0x04006E1F RID: 28191
	EnemyCount,
	// Token: 0x04006E20 RID: 28192
	AnimalCount,
	// Token: 0x04006E21 RID: 28193
	ShowAllPlayersOnMap,
	// Token: 0x04006E22 RID: 28194
	ShowFriendPlayerOnMap,
	// Token: 0x04006E23 RID: 28195
	ShowSpawnWindow,
	// Token: 0x04006E24 RID: 28196
	UNUSED_ShowZombieCounter,
	// Token: 0x04006E25 RID: 28197
	IsCreativeMenuEnabled,
	// Token: 0x04006E26 RID: 28198
	IsTeleportEnabled,
	// Token: 0x04006E27 RID: 28199
	IsFlyingEnabled,
	// Token: 0x04006E28 RID: 28200
	IsPlayerDamageEnabled,
	// Token: 0x04006E29 RID: 28201
	IsPlayerCollisionEnabled,
	// Token: 0x04006E2A RID: 28202
	PlayerKillingMode,
	// Token: 0x04006E2B RID: 28203
	IsSpawnEnemies,
	// Token: 0x04006E2C RID: 28204
	IsSpawnNearOtherPlayer,
	// Token: 0x04006E2D RID: 28205
	IsSaveSupplyCrates,
	// Token: 0x04006E2E RID: 28206
	IsResetMapOnRestart,
	// Token: 0x04006E2F RID: 28207
	ScoreZombieKillMultiplier,
	// Token: 0x04006E30 RID: 28208
	ScorePlayerKillMultiplier,
	// Token: 0x04006E31 RID: 28209
	ScoreDiedMultiplier,
	// Token: 0x04006E32 RID: 28210
	IsVersionCheckDone,
	// Token: 0x04006E33 RID: 28211
	ZombieHordeMeter,
	// Token: 0x04006E34 RID: 28212
	DropOnDeath,
	// Token: 0x04006E35 RID: 28213
	DropOnQuit,
	// Token: 0x04006E36 RID: 28214
	DeathPenalty,
	// Token: 0x04006E37 RID: 28215
	LootTimer,
	// Token: 0x04006E38 RID: 28216
	GameDifficulty,
	// Token: 0x04006E39 RID: 28217
	UNUSED_GameDifficultyBonus,
	// Token: 0x04006E3A RID: 28218
	BloodMoonEnemyCount,
	// Token: 0x04006E3B RID: 28219
	EnemySpawnMode,
	// Token: 0x04006E3C RID: 28220
	EnemyDifficulty,
	// Token: 0x04006E3D RID: 28221
	DayLightLength,
	// Token: 0x04006E3E RID: 28222
	LandClaimCount,
	// Token: 0x04006E3F RID: 28223
	LandClaimSize,
	// Token: 0x04006E40 RID: 28224
	LandClaimDeadZone,
	// Token: 0x04006E41 RID: 28225
	LandClaimExpiryTime,
	// Token: 0x04006E42 RID: 28226
	LandClaimDecayMode,
	// Token: 0x04006E43 RID: 28227
	LandClaimOnlineDurabilityModifier,
	// Token: 0x04006E44 RID: 28228
	LandClaimOfflineDurabilityModifier,
	// Token: 0x04006E45 RID: 28229
	LandClaimOfflineDelay,
	// Token: 0x04006E46 RID: 28230
	AirDropFrequency,
	// Token: 0x04006E47 RID: 28231
	GlobalMessageToShow,
	// Token: 0x04006E48 RID: 28232
	AirDropMarker,
	// Token: 0x04006E49 RID: 28233
	PartySharedKillRange,
	// Token: 0x04006E4A RID: 28234
	ChunkStabilityEnabled,
	// Token: 0x04006E4B RID: 28235
	AutoParty,
	// Token: 0x04006E4C RID: 28236
	OptionsPOICulling,
	// Token: 0x04006E4D RID: 28237
	BloodMoonDay,
	// Token: 0x04006E4E RID: 28238
	BlockDamagePlayer,
	// Token: 0x04006E4F RID: 28239
	XPMultiplier,
	// Token: 0x04006E50 RID: 28240
	BloodMoonWarning,
	// Token: 0x04006E51 RID: 28241
	AllowedViewDistance,
	// Token: 0x04006E52 RID: 28242
	BedrollExpiryTime,
	// Token: 0x04006E53 RID: 28243
	TwitchBloodMoonAllowed,
	// Token: 0x04006E54 RID: 28244
	QuestProgressionDailyLimit,
	// Token: 0x04006E55 RID: 28245
	BiomeProgression,
	// Token: 0x04006E56 RID: 28246
	StormFreq,
	// Token: 0x04006E57 RID: 28247
	CameraRestrictionMode,
	// Token: 0x04006E58 RID: 28248
	JarRefund,
	// Token: 0x04006E59 RID: 28249
	SandboxPreset,
	// Token: 0x04006E5A RID: 28250
	SandboxCode,
	// Token: 0x04006E5B RID: 28251
	DayNightLength,
	// Token: 0x04006E5C RID: 28252
	BlockDamageAI,
	// Token: 0x04006E5D RID: 28253
	BlockDamageAIBM,
	// Token: 0x04006E5E RID: 28254
	LootAbundance,
	// Token: 0x04006E5F RID: 28255
	LootRespawnDays,
	// Token: 0x04006E60 RID: 28256
	GlobalGSModifier,
	// Token: 0x04006E61 RID: 28257
	BiomeGSModifier,
	// Token: 0x04006E62 RID: 28258
	GlobalLSModifier,
	// Token: 0x04006E63 RID: 28259
	BiomeLSModifier,
	// Token: 0x04006E64 RID: 28260
	Last
}
