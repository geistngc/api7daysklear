using System;
using UnityEngine;

// Token: 0x0200118D RID: 4493
public class Constants
{
	// Token: 0x04006958 RID: 26968
	public static bool IsWebPlayer = false;

	// Token: 0x04006959 RID: 26969
	public static bool Is32BitOs = IntPtr.Size == 4;

	// Token: 0x0400695A RID: 26970
	public const int cMinShadowDistance = 20;

	// Token: 0x0400695B RID: 26971
	public const string cOptionsIni = "UserOptions.ini";

	// Token: 0x0400695C RID: 26972
	public const string cProduct = "7 Days To Die";

	// Token: 0x0400695D RID: 26973
	public const string cProductAbbrev = "7DTD";

	// Token: 0x0400695E RID: 26974
	public const VersionInformation.EGameReleaseType cReleaseType = VersionInformation.EGameReleaseType.V;

	// Token: 0x0400695F RID: 26975
	public const int cVersionMajor = 3;

	// Token: 0x04006960 RID: 26976
	public const int cVersionMinor = 20;

	// Token: 0x04006961 RID: 26977
	public const int cVersionBuild = 10;

	// Token: 0x04006962 RID: 26978
	public static readonly VersionInformation cVersionInformation = new VersionInformation(VersionInformation.EGameReleaseType.V, 3, 20, 10);

	// Token: 0x04006963 RID: 26979
	public const int cGameResetRevision = 13;

	// Token: 0x04006964 RID: 26980
	public const int cGraphicsResetRevision = 4;

	// Token: 0x04006965 RID: 26981
	public const int cControlsResetRevision = 7;

	// Token: 0x04006966 RID: 26982
	public const int cBindingsResetRevision = 1;

	// Token: 0x04006967 RID: 26983
	public const string cCopyright = "Copyright (c) 2014-2026 The Fun Pimps LLC All Rights Reserved.";

	// Token: 0x04006968 RID: 26984
	public const int cMaxMPPlayers = 8;

	// Token: 0x04006969 RID: 26985
	public const int cMaxCrossplayMPPlayers = 8;

	// Token: 0x0400696A RID: 26986
	public const int cDefaultUserPermissionLevel = 1000;

	// Token: 0x0400696B RID: 26987
	public const string cDefaultPlayerName = "Player";

	// Token: 0x0400696C RID: 26988
	public const string cDirWorlds = "Data/Worlds";

	// Token: 0x0400696D RID: 26989
	public const string cDirPrefabs = "Data/Prefabs";

	// Token: 0x0400696E RID: 26990
	public const string cDirBluff = "Data/Bluffs";

	// Token: 0x0400696F RID: 26991
	public const string cDirBlocks = "Data/Config";

	// Token: 0x04006970 RID: 26992
	public const string cDirGroups = "Data/Config/Groups";

	// Token: 0x04006971 RID: 26993
	public const string cDirItems = "Data/Config";

	// Token: 0x04006972 RID: 26994
	public const string cDirWorldCreation = "Data/Config";

	// Token: 0x04006973 RID: 26995
	public const string cDirConfig = "Data/Config";

	// Token: 0x04006974 RID: 26996
	public const string cDirPresets = "Data/Presets";

	// Token: 0x04006975 RID: 26997
	public const string cDirHeightmaps = "Data/Heightmaps";

	// Token: 0x04006976 RID: 26998
	public const string cDirBackgroundImage = "Data/Textures/misc/background.jpg";

	// Token: 0x04006977 RID: 26999
	public const string cDirAssetBundles = "Data/Bundles/Standalone";

	// Token: 0x04006978 RID: 27000
	public const string cDirPrefabParts = "Data/Prefabs/Parts";

	// Token: 0x04006979 RID: 27001
	public const string cFolderResourcesSounds = "Sounds";

	// Token: 0x0400697A RID: 27002
	public const string cFolderResourcesConfig = "Data/Config";

	// Token: 0x0400697B RID: 27003
	public const string cFolderResourcesTextures = "Textures";

	// Token: 0x0400697C RID: 27004
	public const string cFolderResourcesEnvironment = "Textures/Environment";

	// Token: 0x0400697D RID: 27005
	public const string cFolderResourcesTerrainTextures = "Textures/Terrain";

	// Token: 0x0400697E RID: 27006
	public const string cFolderResourcesLocalizationEnglish = "GUI/Localization/English";

	// Token: 0x0400697F RID: 27007
	public const string cFolderSaveGame = "Saves";

	// Token: 0x04006980 RID: 27008
	public const string cFolderSaveGameLocal = "SavesLocal";

	// Token: 0x04006981 RID: 27009
	public const string cFolderSaveRegion = "Region";

	// Token: 0x04006982 RID: 27010
	public const string cFolderSavePlayer = "Player";

	// Token: 0x04006983 RID: 27011
	public const string cFolderGeneratedWorlds = "GeneratedWorlds";

	// Token: 0x04006984 RID: 27012
	public const string cFolderRemoteWorld = "World";

	// Token: 0x04006985 RID: 27013
	public const string cFolderLocalPrefabs = "LocalPrefabs";

	// Token: 0x04006986 RID: 27014
	public const string cFolderSaveTwitch = "Twitch";

	// Token: 0x04006987 RID: 27015
	public const string cFolderSavePresets = "Presets";

	// Token: 0x04006988 RID: 27016
	public const string cDirConfigInternal = "DataInternal/Config";

	// Token: 0x04006989 RID: 27017
	public const string cExtLevels = ".ttw";

	// Token: 0x0400698A RID: 27018
	public const string cExtPrefabs = ".tts";

	// Token: 0x0400698B RID: 27019
	public const string cExtPrefabImposters = ".mesh";

	// Token: 0x0400698C RID: 27020
	public const string cExtIdNameMappings = ".nim";

	// Token: 0x0400698D RID: 27021
	public const string cExtPlayedLevel = ".played";

	// Token: 0x0400698E RID: 27022
	public const string cExtSdf = ".sdf";

	// Token: 0x0400698F RID: 27023
	public const string cExtFlag = ".flag";

	// Token: 0x04006990 RID: 27024
	public const string cFileMainTTW = "main.ttw";

	// Token: 0x04006991 RID: 27025
	public const string cFileWorldChecksums = "checksums.txt";

	// Token: 0x04006992 RID: 27026
	public static readonly string cFileBlockMappings = "blockmappings.nim";

	// Token: 0x04006993 RID: 27027
	public static readonly string cFileItemMappings = "itemmappings.nim";

	// Token: 0x04006994 RID: 27028
	public const string cFileDecos = "decoration.7dt";

	// Token: 0x04006995 RID: 27029
	public const string cFileMultiBlocks = "multiblocks.7dt";

	// Token: 0x04006996 RID: 27030
	public const string cGameNameDefault = "Region";

	// Token: 0x04006997 RID: 27031
	public const string cSdfFileName = "gameOptions.sdf";

	// Token: 0x04006998 RID: 27032
	public const string cPersistentPlayersFileName = "players.xml";

	// Token: 0x04006999 RID: 27033
	public const string cAchievementFilename = "achievements.bin";

	// Token: 0x0400699A RID: 27034
	public const string cNewGameSdfFileName = "newGameOptions.sdf";

	// Token: 0x0400699B RID: 27035
	public const string cFileArchivedFlag = "archived.flag";

	// Token: 0x0400699C RID: 27036
	public static string cPrefixAtlas = "ta_";

	// Token: 0x0400699D RID: 27037
	public const string cLevelPrefab = "prefabs";

	// Token: 0x0400699E RID: 27038
	public const string cSpawnPoints = "spawnpoints";

	// Token: 0x0400699F RID: 27039
	public const string cExtTexturePack = ".xml";

	// Token: 0x040069A0 RID: 27040
	public static string cArgDedicatedServer = "-dedicated";

	// Token: 0x040069A1 RID: 27041
	public static string cArgSubmissionBuild = "-submission";

	// Token: 0x040069A2 RID: 27042
	public const int cMaxBiomes = 50;

	// Token: 0x040069A3 RID: 27043
	public const string cTagLargeEntityBlocker = "LargeEntityBlocker";

	// Token: 0x040069A4 RID: 27044
	public const string cTagPhysics = "Physics";

	// Token: 0x040069A5 RID: 27045
	public const int cLayerDefault = 0;

	// Token: 0x040069A6 RID: 27046
	public const int cLayerTransparentFx = 1;

	// Token: 0x040069A7 RID: 27047
	public const int cLayerIgnoreRaycast = 2;

	// Token: 0x040069A8 RID: 27048
	public const int cLayerRemotePlayerCCPhysics = 3;

	// Token: 0x040069A9 RID: 27049
	public const int cLayerWater = 4;

	// Token: 0x040069AA RID: 27050
	public const int cLayerNoShadow = 8;

	// Token: 0x040069AB RID: 27051
	public const int cLayerBackgroundImage = 9;

	// Token: 0x040069AC RID: 27052
	public const int cLayerHoldingItem = 10;

	// Token: 0x040069AD RID: 27053
	public const int cLayerRenderInTexture = 11;

	// Token: 0x040069AE RID: 27054
	public const int cLayerNGUI = 12;

	// Token: 0x040069AF RID: 27055
	public const int cLayerItems = 13;

	// Token: 0x040069B0 RID: 27056
	public const int cLayerNoCollision = 14;

	// Token: 0x040069B1 RID: 27057
	public const int cLayerCCPhysics = 15;

	// Token: 0x040069B2 RID: 27058
	public const int cLayerTerrainCollision = 16;

	// Token: 0x040069B3 RID: 27059
	public const int cLayerPhysicsDead = 17;

	// Token: 0x040069B4 RID: 27060
	public const int cLayerGrass = 18;

	// Token: 0x040069B5 RID: 27061
	public const int cLayerLargeEntityBlocker = 19;

	// Token: 0x040069B6 RID: 27062
	public const int cLayerLocalCCPhysics = 20;

	// Token: 0x040069B7 RID: 27063
	public const int cLayerPhysics = 21;

	// Token: 0x040069B8 RID: 27064
	public const int cLayerUnderwaterEffects = 22;

	// Token: 0x040069B9 RID: 27065
	public const int cLayerTrees = 23;

	// Token: 0x040069BA RID: 27066
	public const int cLayerLocalPlayer = 24;

	// Token: 0x040069BB RID: 27067
	public const int cLayerPlayerRagdollsOLD = 27;

	// Token: 0x040069BC RID: 27068
	public const int cLayerTerrain = 28;

	// Token: 0x040069BD RID: 27069
	public const int cLayerWires = 29;

	// Token: 0x040069BE RID: 27070
	public const int cLayerGlass = 30;

	// Token: 0x040069BF RID: 27071
	public const int cLayerVolumes = 31;

	// Token: 0x040069C0 RID: 27072
	public const int cLayerMaskItems = 8192;

	// Token: 0x040069C1 RID: 27073
	public const int cLayerMaskIgnoreRayCast = 538480652;

	// Token: 0x040069C2 RID: 27074
	public const int cLayerMaskGrass = 262144;

	// Token: 0x040069C3 RID: 27075
	public const int cLayerMaskWater = 16;

	// Token: 0x040069C4 RID: 27076
	public const int cLayerMaskLocalPlayer = 17825792;

	// Token: 0x040069C5 RID: 27077
	public const int cLayerMaskAllLayers = -538480653;

	// Token: 0x040069C6 RID: 27078
	public const int cLayerMaskNoItems = -538488845;

	// Token: 0x040069C7 RID: 27079
	public const int cLayerMaskOnlyItemsAndCollision = 73728;

	// Token: 0x040069C8 RID: 27080
	public const int cLayerMaskNoItemsNoGrass = -538750989;

	// Token: 0x040069C9 RID: 27081
	public const int cLayerMaskNoItemsNoGrassNoWater = -538751005;

	// Token: 0x040069CA RID: 27082
	public const int cLayerMaskNoItemsNoLocalPlayer = -555266061;

	// Token: 0x040069CB RID: 27083
	public const int cLayerMaskAttackingBlocksMask = 1073807360;

	// Token: 0x040069CC RID: 27084
	public const int cLayerMaskSight = -1612492829;

	// Token: 0x040069CD RID: 27085
	public const int cLayerMaskTrajectory = 1082195968;

	// Token: 0x040069CE RID: 27086
	public static int cDistanceRandomDisplayUpdates = 30;

	// Token: 0x040069CF RID: 27087
	public static float cRunningFOVMultiplier = 1.05f;

	// Token: 0x040069D0 RID: 27088
	public static float cRunningFOVSpeedDown = 3f;

	// Token: 0x040069D1 RID: 27089
	public static float cRunningFOVSpeedUp = 1f;

	// Token: 0x040069D2 RID: 27090
	public static int cDefaultCameraFieldOfView = 65;

	// Token: 0x040069D3 RID: 27091
	public static int cMinCameraFieldOfView = 50;

	// Token: 0x040069D4 RID: 27092
	public static int cMaxCameraFieldOfView = 85;

	// Token: 0x040069D5 RID: 27093
	public static readonly Vector3 cDefaultCameraPlayerOffset = new Vector3(0f, 1.6f, 0f);

	// Token: 0x040069D6 RID: 27094
	public const int cMaxVertices = 786432;

	// Token: 0x040069D7 RID: 27095
	public static float cMinGlobalBackgroundOpacity = 0.55f;

	// Token: 0x040069D8 RID: 27096
	public static float cMinGlobalForegroundOpacity = 0.75f;

	// Token: 0x040069D9 RID: 27097
	public const int cTicksPerSecond = 20;

	// Token: 0x040069DA RID: 27098
	public const float cTickDuration = 0.05f;

	// Token: 0x040069DB RID: 27099
	public const float cPhysicsTicksPerSecond = 50f;

	// Token: 0x040069DC RID: 27100
	public const float cPhysicsTickDuration = 0.02f;

	// Token: 0x040069DD RID: 27101
	public static float cDefaultDistortionFactor = 1f;

	// Token: 0x040069DE RID: 27102
	public static int cMaxEntitiesPerMobSpawner = 8;

	// Token: 0x040069DF RID: 27103
	public const byte cMaxLightValue = 15;

	// Token: 0x040069E0 RID: 27104
	public static float cSizePlanesAround = 250f;

	// Token: 0x040069E1 RID: 27105
	public static int cDefaultPort = 26900;

	// Token: 0x040069E2 RID: 27106
	public static int cLevelServerPort = 6789;

	// Token: 0x040069E3 RID: 27107
	public static int cRandomSpawnPointsToPlace = 4;

	// Token: 0x040069E4 RID: 27108
	public static int cStartTeamTickets = 0;

	// Token: 0x040069E5 RID: 27109
	public static int cTeamTicketsAlarm = 10;

	// Token: 0x040069E6 RID: 27110
	public static float cDecreaseOneTicketTime = 5f;

	// Token: 0x040069E7 RID: 27111
	public static float cTimeGameOverButWaitSeconds = 5f;

	// Token: 0x040069E8 RID: 27112
	public static float cDigAndBuildDistance = 4f;

	// Token: 0x040069E9 RID: 27113
	public static float cCollectItemDistance = 2f;

	// Token: 0x040069EA RID: 27114
	public static float cPlayerInteractDistance = 5f;

	// Token: 0x040069EB RID: 27115
	public static float cRespawnAfterDeathTime = 3f;

	// Token: 0x040069EC RID: 27116
	public static float cRespawnEnterGameTime = 0f;

	// Token: 0x040069ED RID: 27117
	public static float cRespawnAfterFallenDown = 3f;

	// Token: 0x040069EE RID: 27118
	public static float cHitColorDuration = 0.15f;

	// Token: 0x040069EF RID: 27119
	public static float cItemDroppedOnDeathLifetime = 300f;

	// Token: 0x040069F0 RID: 27120
	public const float cItemDroppedLifetime = 60f;

	// Token: 0x040069F1 RID: 27121
	public static float cItemExplosionLifetime = 30f;

	// Token: 0x040069F2 RID: 27122
	public static float cItemHealthDroppedLifetime = 60f;

	// Token: 0x040069F3 RID: 27123
	public static float cItemSpawnPointLifetime = float.MaxValue;

	// Token: 0x040069F4 RID: 27124
	public static float cItemPortalLifetime = float.MaxValue;

	// Token: 0x040069F5 RID: 27125
	public static float cItemItemSpawnerLifetime = float.MaxValue;

	// Token: 0x040069F6 RID: 27126
	public static int cHardenBrickTime = 20;

	// Token: 0x040069F7 RID: 27127
	public const int cItemQualityTierVariations = 1;

	// Token: 0x040069F8 RID: 27128
	public const ushort cItemMaxQuality = 6;

	// Token: 0x040069F9 RID: 27129
	public static float cMinHolsterTime = 0.1f;

	// Token: 0x040069FA RID: 27130
	public static float cMinUnHolsterTime = 0.1f;

	// Token: 0x040069FB RID: 27131
	public static float cEnergyJetpackPerBlock = 10f;

	// Token: 0x040069FC RID: 27132
	public static int cHealthPotionAdd = 30;

	// Token: 0x040069FD RID: 27133
	public static int cMaxPlayerFood = 100;

	// Token: 0x040069FE RID: 27134
	public static int cFoodOversaturate = 100;

	// Token: 0x040069FF RID: 27135
	public static int cMaxPlayerDrink = 100;

	// Token: 0x04006A00 RID: 27136
	public static int cDrinkOversaturate = 100;

	// Token: 0x04006A01 RID: 27137
	public static int cItemDropCountWhenDead = 3;

	// Token: 0x04006A02 RID: 27138
	public static float cBuildIntervall = 0.5f;

	// Token: 0x04006A03 RID: 27139
	public static float cSendWorldTickTimeToClients = 1.5f;

	// Token: 0x04006A04 RID: 27140
	public static float cCheckGameState = 0.5f;

	// Token: 0x04006A05 RID: 27141
	public static float cSneakDamageMultiplier = 2f;

	// Token: 0x04006A06 RID: 27142
	public static int cNumberOfTeams = 2;

	// Token: 0x04006A07 RID: 27143
	public static Color[] cTeamColors = new Color[]
	{
		Color.white,
		new Color(0f, 0.8f, 1f),
		Color.red
	};

	// Token: 0x04006A08 RID: 27144
	public static string[] cTeamName = new string[]
	{
		"No",
		"BLUE",
		"RED"
	};

	// Token: 0x04006A09 RID: 27145
	public static string[] cTeamSkinName = new string[]
	{
		"Soldier Blue",
		"Soldier Blue",
		"Soldier Red"
	};

	// Token: 0x04006A0A RID: 27146
	public static float cDarkAtNightSubtraction = 12f;

	// Token: 0x04006A0B RID: 27147
	public static float cDefaultMonsterSeeDistance = 48f;

	// Token: 0x04006A0C RID: 27148
	public static float cPlayerSpeedModifierRunning = 1.6f;

	// Token: 0x04006A0D RID: 27149
	public static float cPlayerSpeedModifierWalking = 0.8f;

	// Token: 0x04006A0E RID: 27150
	public static float cPlayerSpeedModifierCrouching = 0.4f;

	// Token: 0x04006A0F RID: 27151
	public static int cPosInventoryYSub = 70;

	// Token: 0x04006A10 RID: 27152
	public static int cPosMinimapY = 10;

	// Token: 0x04006A11 RID: 27153
	public static int cPosMinimapSubRight = 20;

	// Token: 0x04006A12 RID: 27154
	public static Color cColorBlood = new Color(0.8f, 0f, 0.08f);

	// Token: 0x04006A13 RID: 27155
	public static Color cColorBorderBox = new Color(0.8f, 0f, 0f, 0.5f);

	// Token: 0x04006A14 RID: 27156
	public static Vector3 cStartPositionPlayerInLevel = new Vector3(0f, 200f, 0f);

	// Token: 0x04006A15 RID: 27157
	public static Vector3 cStartRotationPlayerInLevel = new Vector3(0f, 0f, 0f);

	// Token: 0x04006A16 RID: 27158
	public static BlockValue cTerrainBlockValue = new BlockValue(1U);

	// Token: 0x04006A17 RID: 27159
	public static string cTerrainFillerBlockName = "terrainFiller";

	// Token: 0x04006A18 RID: 27160
	public static string cTerrainFiller2BlockName = "terrainFillerAdaptive";

	// Token: 0x04006A19 RID: 27161
	public static string cPOIFillerBlock = "poiFillerBlock";

	// Token: 0x04006A1A RID: 27162
	public static string cQuestLootFetchContainerIndexName = "FetchContainer";

	// Token: 0x04006A1B RID: 27163
	public static string cQuestRestorePowerIndexName = "QuestRestorePower";

	// Token: 0x04006A1C RID: 27164
	public static Color[] TrackedFriendColors = new Color[]
	{
		Color.green,
		Color.blue,
		Color.yellow,
		new Color(1f, 0f, 1f),
		new Color(0.5f, 0.25f, 0f),
		new Color(1f, 0.5f, 0f),
		new Color32(56, 35, 16, byte.MaxValue),
		new Color32(42, 59, 0, byte.MaxValue)
	};

	// Token: 0x04006A1D RID: 27165
	public const int cMaxViewDistanceOptions = 7;

	// Token: 0x04006A1E RID: 27166
	public const int cMinViewDistance = 4;

	// Token: 0x04006A1F RID: 27167
	public const int cMaxViewDistance = 12;

	// Token: 0x04006A20 RID: 27168
	public const float cBlockDamageLosesPaint = 1f;

	// Token: 0x04006A21 RID: 27169
	public static int cEnemySenseMemory = 60;

	// Token: 0x04006A22 RID: 27170
	public const int ChunkCompressionLevel = 3;

	// Token: 0x04006A23 RID: 27171
	public const int NetworkCompressionLevel = 3;

	// Token: 0x04006A24 RID: 27172
	public const string cSpecialWorldName_Empty = "Empty";

	// Token: 0x04006A25 RID: 27173
	public const string cSpecialWorldName_Playtesting = "Playtesting";

	// Token: 0x04006A26 RID: 27174
	public const string cSpecialWorldName_Navezgane = "Navezgane";

	// Token: 0x04006A27 RID: 27175
	public const float cMouseSensitivityMin = 0.01f;

	// Token: 0x04006A28 RID: 27176
	public const float cMouseSensitivityMax = 1.5f;

	// Token: 0x04006A29 RID: 27177
	public const float cControllerSensitivityMin = 0.05f;

	// Token: 0x04006A2A RID: 27178
	public const float cControllerSensitivityMax = 1f;

	// Token: 0x04006A2B RID: 27179
	public const float cControllerModifierSensitivityMax = 2f;

	// Token: 0x04006A2C RID: 27180
	public const int cPlayTestingSpawnOffset = 10;

	// Token: 0x04006A2D RID: 27181
	public const float cAimAssistMaxDistance = 50f;

	// Token: 0x04006A2E RID: 27182
	public const float cAimAssistSlowDownEntity = 0.5f;

	// Token: 0x04006A2F RID: 27183
	public const float cAimAssistSlowDownItem = 0.6f;

	// Token: 0x04006A30 RID: 27184
	public const float cAimAssistSlowDownItemDistance = 10f;

	// Token: 0x04006A31 RID: 27185
	public const float cAimAssistSlowThreatLevelThreshold = 0.75f;

	// Token: 0x04006A32 RID: 27186
	public const float cAimAssistSnapScreenDistance = 0.15f;

	// Token: 0x04006A33 RID: 27187
	public const float cCameraSnapTime = 0.3f;

	// Token: 0x04006A34 RID: 27188
	public const float cAimAssistSnapMaximumAngle = 15f;

	// Token: 0x04006A35 RID: 27189
	public const float cAimAssistZoomSnapSpeed = 1f;

	// Token: 0x04006A36 RID: 27190
	public const float cAimAssistMeleeSnapAngle = 20f;

	// Token: 0x04006A37 RID: 27191
	public const float cAimAssistMeleeSnapSpeed = 1.5f;

	// Token: 0x04006A38 RID: 27192
	public const float cAimAssistMeleeHitSnapAngle = 30f;

	// Token: 0x04006A39 RID: 27193
	public const float cRapidTriggerFireDelay = 0.25f;

	// Token: 0x04006A3A RID: 27194
	public const float cMapViewControllerSpeed = 500f;

	// Token: 0x04006A3B RID: 27195
	public const float cRunToggleHoldTime = 0.2f;

	// Token: 0x04006A3C RID: 27196
	public const float cRecoveryPositionAttemptTime = 30f;

	// Token: 0x04006A3D RID: 27197
	public const float cRecoveryPositionMinSqrMagnitude = 10000f;

	// Token: 0x04006A3E RID: 27198
	public const int cMaxRecoveryPositions = 5;

	// Token: 0x04006A3F RID: 27199
	public const float cCursorSensitivityMin = 0.1f;

	// Token: 0x04006A40 RID: 27200
	public const float cCursorSensitivityMax = 1f;

	// Token: 0x04006A41 RID: 27201
	public const int cNavWorldSizeX = 6144;

	// Token: 0x04006A42 RID: 27202
	public const int cNavWorldSizeZ = 6144;

	// Token: 0x04006A43 RID: 27203
	public const float cPartyActivationRange = 15f;

	// Token: 0x04006A44 RID: 27204
	public const int cMaxPartySize = 8;

	// Token: 0x04006A45 RID: 27205
	public const int cDummyWaterTexId = 5000;

	// Token: 0x04006A46 RID: 27206
	public static int cMaxLoadTimePixelsPerTest = 4096;

	// Token: 0x04006A47 RID: 27207
	public static int cMaxLoadTimePerFrameMillis = 50;

	// Token: 0x04006A48 RID: 27208
	public const float cInputRepeatDelay = 0.1f;

	// Token: 0x04006A49 RID: 27209
	public const float cInputInitialRepeatDelay = 0.35f;

	// Token: 0x04006A4A RID: 27210
	public const int cConsoleMaxPersistentPlayerDataEntries = 100;

	// Token: 0x04006A4B RID: 27211
	public const int cVirtualKeyboardDefaultCharacterLimit = 200;

	// Token: 0x0200118E RID: 4494
	public enum EBiomePoiMap : byte
	{
		// Token: 0x04006A4D RID: 27213
		CityAsphalt = 1,
		// Token: 0x04006A4E RID: 27214
		CountryRoadAsphalt,
		// Token: 0x04006A4F RID: 27215
		RoadGravel,
		// Token: 0x04006A50 RID: 27216
		Sand,
		// Token: 0x04006A51 RID: 27217
		Free
	}
}
