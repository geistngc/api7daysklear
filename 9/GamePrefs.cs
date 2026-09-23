using System;
using System.Collections.Generic;
using System.Globalization;
using Platform;
using SandboxOptions;
using SDF;
using UnityEngine;

// Token: 0x020011ED RID: 4589
public class GamePrefs
{
	// Token: 0x0600927F RID: 37503 RVA: 0x00373934 File Offset: 0x00371B34
	[PublicizedFrom(EAccessModifier.Private)]
	public static void initPropertyDecl()
	{
		int num = 131072;
		if (!GameManager.IsDedicatedServer)
		{
			num = 524288;
		}
		GamePrefs.s_propertyList = new GamePrefs.PropertyDecl[]
		{
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsAmbientVolumeLevel, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsDynamicMusicEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsDynamicMusicDailyTime, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.45f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsPlayChanceFrequency, DeviceFlag.None, GamePrefs.EnumType.Float, 3f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsPlayChanceProbability, DeviceFlag.None, GamePrefs.EnumType.Float, 0.983f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsMusicVolumeLevel, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.6f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsMenuMusicVolumeLevel, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.7f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsMicVolumeLevel, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.75f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsVoiceVolumeLevel, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.75f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsOverallAudioVolumeLevel, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsVoiceChatEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsVoiceInputDevice, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsVoiceOutputDevice, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsMumblePositionalAudioSupport, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsAudioOcclusion, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxResetRevision, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControlsResetRevision, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxWaterQuality, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxViewDistance, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 6, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxShadowDistance, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxResolution, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxDynamicMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxDynamicMinFPS, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 30, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxDynamicScale, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxVsync, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxAA, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxAASharpness, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Float, 0f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxLODDistance, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Float, 0.5f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxFOV, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, Constants.cDefaultCameraFieldOfView, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxTexQuality, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxTexFilter, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxReflectQuality, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxStreamMipmaps, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxTerrainQuality, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 3, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxObjQuality, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 3, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxGrassDistance, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxQualityPreset, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 2, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxOcclusion, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxBloom, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxDOF, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxMotionBlur, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxSSAO, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxSSReflections, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxSunShafts, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxSignQuality, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 2, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxReflectShadows, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsHudSize, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsHudOpacity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsShowCrosshair, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsShowCompass, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsBackgroundGlobalOpacity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.95f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsForegroundGlobalOpacity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxBrightness, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.5f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxWaterPtlLimiter, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Float, 0.5f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsPOICulling, DeviceFlag.None, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsDisableChunkLODs, DeviceFlag.None, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.UNUSED_OptionsPlayerModel, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "playerMale", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsPlayerModelTexture, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "Player/Male/Player_male", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsLookSensitivity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.5f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsZoomSensitivity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.3f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsZoomAccel, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.5f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsVehicleLookSensitivity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.5f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsWeaponAiming, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsInvertMouse, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsAllowController, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsStabSpawnBlocksOnGround, DeviceFlag.None, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.GameName, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "My Game", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.GameNameClient, DeviceFlag.None, GamePrefs.EnumType.String, "My Game", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.GameMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, GameModeSurvival.TypeName, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.GameDifficulty, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.GameWorld, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, null, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.GameVersion, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, Constants.cVersionInformation.LongStringNoBuild, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerIP, DeviceFlag.None, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerPort, DeviceFlag.None, GamePrefs.EnumType.Int, Constants.cDefaultPort, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerMaxPlayerCount, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 8, null, null, new Dictionary<DeviceFlag, object>
			{
				{
					DeviceFlag.PS5,
					4
				},
				{
					DeviceFlag.XBoxSeriesX,
					4
				},
				{
					DeviceFlag.XBoxSeriesS,
					2
				}
			}),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerPasswordCache, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Binary, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerIsPublic, DeviceFlag.None, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerPassword, DeviceFlag.None, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerName, DeviceFlag.None, GamePrefs.EnumType.String, "Default Server", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerDescription, DeviceFlag.None, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerWebsiteURL, DeviceFlag.None, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerMaxWorldTransferSpeedKiBs, DeviceFlag.None, GamePrefs.EnumType.Int, 512, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerMaxAllowedViewDistance, DeviceFlag.None, GamePrefs.EnumType.Int, 12, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerAllowCrossplay, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ConnectToServerIP, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "127.0.0.1", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ConnectToServerPort, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, Constants.cDefaultPort, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.FavoriteServersList, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.UNUSED_ControlPanelPort, DeviceFlag.None, GamePrefs.EnumType.Int, 8080, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.UNUSED_ControlPanelPassword, DeviceFlag.None, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.CreateLevelName, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "My Level", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.CreateLevelDim, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "4096", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DebugMenuShowTasks, DeviceFlag.None, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DebugMenuEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DebugStopEnemiesMoving, DeviceFlag.None, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.CreativeMenuEnabled, DeviceFlag.None, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PlayerName, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "Player", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.UNUSED_PlayerId, DeviceFlag.None, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PlayerPassword, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Binary, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PlayerAutologin, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PlayerToken, DeviceFlag.None, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PlayerSafeZoneHours, DeviceFlag.None, GamePrefs.EnumType.Int, 7, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PlayerSafeZoneLevel, DeviceFlag.None, GamePrefs.EnumType.Int, 5, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DynamicSpawner, DeviceFlag.None, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PlayerKillingMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, EnumPlayerKillingMode.KillStrangersOnly, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.MatchLength, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 10, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.FragLimit, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 20, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.RebuildMap, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.JoiningOptions, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ZombiePlayers, DeviceFlag.None, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DayCount, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 3, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DayNightLength, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 60, 10, 120, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DayLightLength, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 18, 12, 18, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BloodMoonFrequency, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 7, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BloodMoonRange, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BloodMoonWarning, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 8, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ShowFriendPlayerOnMap, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.AdminFileName, DeviceFlag.None, GamePrefs.EnumType.String, "serveradmin.xml", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.UNUSED_ControlPanelEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.TelnetEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.TelnetPort, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 25003, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.TelnetPassword, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ZombieFeralSense, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ZombieMove, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ZombieMoveNight, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 3, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ZombieFeralMove, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 3, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ZombieBMMove, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 3, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DeathPenalty, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 1, 1, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DropOnDeath, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 1, 1, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DropOnQuit, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, 0, 1, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BloodMoonEnemyCount, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 8, 8, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.EnemySpawnMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, true, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.EnemyDifficulty, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BlockDamagePlayer, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 100, 100, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BlockDamageAI, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 100, 100, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BlockDamageAIBM, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 100, 100, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LootRespawnDays, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 7, 7, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LootAbundance, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 100, 100, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LandClaimCount, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 5, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LandClaimSize, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 41, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LandClaimDeadZone, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 30, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LandClaimExpiryTime, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 7, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LandClaimDecayMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LandClaimOnlineDurabilityModifier, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 4, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LandClaimOfflineDurabilityModifier, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 4, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LandClaimOfflineDelay, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.AirDropFrequency, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 3, 0, 6, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.AirDropMarker, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PartySharedKillRange, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 100, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.MaxSpawnedZombies, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 64, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.MaxSpawnedAnimals, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 50, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.AutopilotMode, DeviceFlag.None, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.SelectionOperationMode, DeviceFlag.None, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.SelectionContextMode, DeviceFlag.None, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.EACEnabled, DeviceFlag.None, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BuildCreate, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, false, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PersistentPlayerProfiles, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.XPMultiplier, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 100, 100, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LastGameResetRevision, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.NoGraphicsMode, DeviceFlag.None, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsTempCelsius, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsDisableXmlEvents, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerDisabledNetworkProtocols, DeviceFlag.None, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsScreenBoundsValue, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsUiFpsScaling, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsInterfaceSensitivity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.5f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerVibration, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerTriggerEffects, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, TriggerEffectManager.SettingDefaultValue(), null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.HideCommandExecutionLog, DeviceFlag.None, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.MaxUncoveredMapChunksPerPlayer, DeviceFlag.None, GamePrefs.EnumType.Int, num, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerReservedSlots, DeviceFlag.None, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerReservedSlotsPermission, DeviceFlag.None, GamePrefs.EnumType.Int, 100, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerAdminSlots, DeviceFlag.None, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerAdminSlotsPermission, DeviceFlag.None, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.GameGuidClient, DeviceFlag.None, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BedrollDeadZoneSize, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 15, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BedrollExpiryTime, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 45, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LastLoadedPrefab, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsJournalPopup, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsFilterProfanity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsQuestsAutoShare, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsQuestsAutoAccept, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsAutoPartyWithFriends, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.TelnetFailedLoginLimit, DeviceFlag.None, GamePrefs.EnumType.Int, 10, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.TelnetFailedLoginsBlocktime, DeviceFlag.None, GamePrefs.EnumType.Int, 10, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.TerminalWindowEnabled, DeviceFlag.None, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerVisibility, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 2, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerLoginConfirmationText, DeviceFlag.None, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.WorldGenSeed, DeviceFlag.None, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.WorldGenSize, DeviceFlag.None, GamePrefs.EnumType.Int, 8192, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxTreeDistance, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 4, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LastLoadingTipRead, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, -1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DynamicMeshEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DynamicMeshDistance, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 1000, 100, 3000, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DynamicMeshLandClaimOnly, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DynamicMeshLandClaimBuffer, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 3, 1, 5, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DynamicMeshUseImposters, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DynamicMeshMaxRegionCache, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 1, 1, 3, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DynamicMeshMaxItemCache, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 3, 1, 6, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.TwitchServerPermission, DeviceFlag.None, GamePrefs.EnumType.Int, 90, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.TwitchBloodMoonAllowed, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsSelectionBoxAlphaMultiplier, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.4f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsPoiVolumesSkipDeleteConfirmation, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.PlaytestBiome, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 3, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.Language, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.LanguageBrowser, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.Region, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerHistoryCache, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Binary, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.MaxChunkAge, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, -1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ResetUnprotectedChunks, DeviceFlag.None, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.SaveDataLimit, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, -1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsSubtitlesEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsIntroMovieEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.AllowSpawnNearBackpack, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.AllowSpawnNearFriend, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 2, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.WebDashboardEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.WebDashboardPort, DeviceFlag.None, GamePrefs.EnumType.Int, 8080, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.WebDashboardUrl, DeviceFlag.None, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.EnableMapRendering, DeviceFlag.None, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.MaxQueuedMeshLayers, DeviceFlag.None, GamePrefs.EnumType.Int, 40, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerSensitivityX, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.35f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerSensitivityY, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.25f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerLookInvert, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerJoystickLayout, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerLookAcceleration, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 4f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerZoomSensitivity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.5f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerLookAxisDeadzone, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerMoveAxisDeadzone, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerCursorSnap, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerCursorHoverSensitivity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.5f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerVehicleSensitivity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerWeaponAiming, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerAimAssists, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsChatCommunication, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsCrossplay, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControlsSprintLock, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DebugPanelsEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "-", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerVibrationStrength, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 2, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.EulaVersionAccepted, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, -1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.EulaLatestVersion, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxMotionBlurEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.IgnoreEOSSanctions, DeviceFlag.None, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.SkipSpawnButton, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsUiCompassUseEnglishCardinalDirections, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxShadowQuality, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.QuestProgressionDailyLimit, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 4, -1, 8, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControllerIconStyle, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsShowConsoleButton, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.SaveDataLimitType, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, SaveDataLimitType.Unlimited.ToStringCached<SaveDataLimitType>(), null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerEACPeerToPeer, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.BiomeProgression, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.StormFreq, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 100, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.JarRefund, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 60, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.SandboxPreset, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.SandboxCode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.ServerMatchmakingGroup, DeviceFlag.None, GamePrefs.EnumType.String, string.Empty, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxUpscalerMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, GameOptionsPlatforms.DefaultUpscalerMode, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxFSRPreset, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 2, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxFOV3P, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, Constants.cDefaultCameraFieldOfView, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxDefaultFirstPersonCamera, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfx3PCameraMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.CameraRestrictionMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxCameraDistance3P, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.25f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsGfxLimitFpsInGame, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX, GamePrefs.EnumType.Int, -1, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.AISmellMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 3, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsControlsDefaultQuickAction, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsBindingsResetRevision, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsCrosshairEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsCrosshairScale, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsCrosshairColor, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsCrosshairOpacity, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsCrosshairADS, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsCrosshairDot3P, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsCrosshairThickness, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 0.2f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.OptionsCrosshairRangedEnabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.GameSaveStorageType, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, UserDataStorageType.DeviceLocal, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.UserWorldStorageType, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, UserDataStorageType.DeviceLocal, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordFirstTimeInfoShown, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordDisabled, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordLastAccountType, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordAccessToken, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, null, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordRefreshToken, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, null, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordSelectedOutputDevice, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "default", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordSelectedInputDevice, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.String, "default", null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordOutputVolume, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordInputVolume, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Float, 1f, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordVoiceModePtt, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, false, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordVoiceVadModeAuto, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordVoiceVadThreshold, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, -60, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordDmPrivacyMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordMuteDmNotifications, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Bool, true, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.DiscordAutoJoinVoiceMode, DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5, GamePrefs.EnumType.Int, 0, null, null, null),
			new GamePrefs.PropertyDecl(EnumGamePrefs.GameWorldLocationType, DeviceFlag.None, GamePrefs.EnumType.Int, PathAbstractions.EAbstractedLocationType.None, null, null, null)
		};
	}

	// Token: 0x140000F9 RID: 249
	// (add) Token: 0x06009280 RID: 37504 RVA: 0x00375D64 File Offset: 0x00373F64
	// (remove) Token: 0x06009281 RID: 37505 RVA: 0x00375D98 File Offset: 0x00373F98
	public static event Action<EnumGamePrefs> OnGamePrefChanged;

	// Token: 0x1700117B RID: 4475
	// (get) Token: 0x06009282 RID: 37506 RVA: 0x00375DCB File Offset: 0x00373FCB
	public static GamePrefs Instance
	{
		get
		{
			if (GamePrefs.m_Instance == null)
			{
				throw new InvalidOperationException("GamePrefs is being accessed before it is ready.");
			}
			return GamePrefs.m_Instance;
		}
	}

	// Token: 0x06009283 RID: 37507 RVA: 0x00375DE4 File Offset: 0x00373FE4
	public static void InitPropertyDeclarations()
	{
		if (GamePrefs.s_propertyList != null)
		{
			throw new InvalidOperationException("GamePrefs' property declarations should only be initialized once.");
		}
		GamePrefs.initPropertyDecl();
	}

	// Token: 0x06009284 RID: 37508 RVA: 0x00375DFD File Offset: 0x00373FFD
	public static void InitPrefs()
	{
		if (GamePrefs.m_Instance != null)
		{
			throw new InvalidOperationException("GamePrefs should only be initialized and loaded once.");
		}
		GamePrefs.m_Instance = new GamePrefs();
		GamePrefs.m_Instance.Load();
	}

	// Token: 0x06009285 RID: 37509 RVA: 0x00375E25 File Offset: 0x00374025
	public static GamePrefs.PropertyDecl[] GetPropertyList()
	{
		return GamePrefs.s_propertyList;
	}

	// Token: 0x06009286 RID: 37510 RVA: 0x00375E2C File Offset: 0x0037402C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Load()
	{
		foreach (GamePrefs.PropertyDecl propertyDecl in GamePrefs.s_propertyList)
		{
			string key = propertyDecl.name.ToStringCached<EnumGamePrefs>();
			EnumGamePrefs name = propertyDecl.name;
			if (!propertyDecl.IsPersistent || !SdPlayerPrefs.HasKey(key))
			{
				this.SetObjectInternal(propertyDecl.name, propertyDecl.defaultValue);
			}
			else
			{
				switch (propertyDecl.type)
				{
				case GamePrefs.EnumType.Int:
					this.SetObjectInternal(propertyDecl.name, SdPlayerPrefs.GetInt(key));
					break;
				case GamePrefs.EnumType.Float:
					this.SetObjectInternal(propertyDecl.name, SdPlayerPrefs.GetFloat(key));
					break;
				case GamePrefs.EnumType.String:
				{
					string @string = SdPlayerPrefs.GetString(key);
					this.SetObjectInternal(propertyDecl.name, (@string != null) ? @string : propertyDecl.defaultValue);
					break;
				}
				case GamePrefs.EnumType.Bool:
					this.SetObjectInternal(propertyDecl.name, SdPlayerPrefs.GetInt(key) != 0);
					break;
				case GamePrefs.EnumType.Binary:
				{
					string string2 = SdPlayerPrefs.GetString(key);
					this.SetObjectInternal(propertyDecl.name, (string2 != null) ? Utils.FromBase64(string2) : propertyDecl.defaultValue);
					break;
				}
				}
			}
		}
	}

	// Token: 0x06009287 RID: 37511 RVA: 0x00375F60 File Offset: 0x00374160
	public void Load(string sdfFileName)
	{
		try
		{
			SdfFile sdfFile = new SdfFile(sdfFileName);
			sdfFile.Load();
			string[] storedGamePrefs = sdfFile.GetStoredGamePrefs();
			int i = 0;
			while (i < storedGamePrefs.Length)
			{
				string text = storedGamePrefs[i];
				EnumGamePrefs enumGamePrefs = EnumGamePrefs.Last;
				try
				{
					enumGamePrefs = EnumUtils.Parse<EnumGamePrefs>(text, false);
				}
				catch (ArgumentException)
				{
					Log.Warning("Savegame contains unknown option '{0}'. Probably an outdated savegame, ignoring this option!", new object[]
					{
						text
					});
					goto IL_14B;
				}
				goto IL_4B;
				IL_14B:
				i++;
				continue;
				IL_4B:
				int num = GamePrefs.find(enumGamePrefs);
				if (num == -1)
				{
					return;
				}
				switch (GamePrefs.s_propertyList[num].type)
				{
				case GamePrefs.EnumType.Int:
				{
					int? @int = sdfFile.GetInt(enumGamePrefs.ToStringCached<EnumGamePrefs>());
					if (@int != null)
					{
						GamePrefs.Set(enumGamePrefs, @int.Value);
						goto IL_14B;
					}
					goto IL_14B;
				}
				case GamePrefs.EnumType.Float:
				{
					float? @float = sdfFile.GetFloat(enumGamePrefs.ToStringCached<EnumGamePrefs>());
					if (@float != null)
					{
						GamePrefs.Set(enumGamePrefs, @float.Value);
						goto IL_14B;
					}
					goto IL_14B;
				}
				case GamePrefs.EnumType.String:
				{
					string @string = sdfFile.GetString(enumGamePrefs.ToStringCached<EnumGamePrefs>());
					if (@string != null)
					{
						GamePrefs.Set(enumGamePrefs, @string);
						goto IL_14B;
					}
					goto IL_14B;
				}
				case GamePrefs.EnumType.Bool:
				{
					bool? @bool = sdfFile.GetBool(enumGamePrefs.ToStringCached<EnumGamePrefs>());
					if (@bool != null)
					{
						GamePrefs.Set(enumGamePrefs, @bool.Value);
						goto IL_14B;
					}
					goto IL_14B;
				}
				case GamePrefs.EnumType.Binary:
				{
					string string2 = sdfFile.GetString(enumGamePrefs.ToStringCached<EnumGamePrefs>(), true);
					if (string2 != null)
					{
						GamePrefs.Set(enumGamePrefs, string2);
						goto IL_14B;
					}
					goto IL_14B;
				}
				default:
					goto IL_14B;
				}
			}
			if (GamePrefs.GetInt(EnumGamePrefs.MaxChunkAge) == 0)
			{
				GamePrefs.Set(EnumGamePrefs.MaxChunkAge, (int)GamePrefs.GetDefault(EnumGamePrefs.MaxChunkAge));
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex.Message + "\n" + ex.StackTrace);
		}
	}

	// Token: 0x06009288 RID: 37512 RVA: 0x00376144 File Offset: 0x00374344
	public void Save(string sdfFileName)
	{
		List<EnumGamePrefs> list = new List<EnumGamePrefs>();
		foreach (GamePrefs.PropertyDecl propertyDecl in GamePrefs.s_propertyList)
		{
			list.Add(propertyDecl.name);
		}
		this.Save(sdfFileName, list);
	}

	// Token: 0x06009289 RID: 37513 RVA: 0x00376188 File Offset: 0x00374388
	public void Save(string sdfFileName, List<EnumGamePrefs> prefsToSave)
	{
		try
		{
			SdfFile sdfFile = new SdfFile(sdfFileName);
			sdfFile.Load();
			foreach (GamePrefs.PropertyDecl propertyDecl in GamePrefs.s_propertyList)
			{
				if (prefsToSave.Contains(propertyDecl.name))
				{
					switch (propertyDecl.type)
					{
					case GamePrefs.EnumType.Int:
						sdfFile.Set(propertyDecl.name.ToStringCached<EnumGamePrefs>(), GamePrefs.GetInt(propertyDecl.name));
						break;
					case GamePrefs.EnumType.Float:
						sdfFile.Set(propertyDecl.name.ToStringCached<EnumGamePrefs>(), GamePrefs.GetFloat(propertyDecl.name));
						break;
					case GamePrefs.EnumType.String:
						sdfFile.Set(propertyDecl.name.ToStringCached<EnumGamePrefs>(), GamePrefs.GetString(propertyDecl.name));
						break;
					case GamePrefs.EnumType.Bool:
						sdfFile.Set(propertyDecl.name.ToStringCached<EnumGamePrefs>(), GamePrefs.GetBool(propertyDecl.name));
						break;
					case GamePrefs.EnumType.Binary:
						sdfFile.Set(propertyDecl.name.ToStringCached<EnumGamePrefs>(), GamePrefs.GetString(propertyDecl.name), true);
						break;
					}
				}
			}
			sdfFile.Save();
		}
		catch (Exception ex)
		{
			Log.Error(ex.Message + "\n" + ex.StackTrace);
		}
	}

	// Token: 0x0600928A RID: 37514 RVA: 0x003762D8 File Offset: 0x003744D8
	public void Save()
	{
		foreach (GamePrefs.PropertyDecl propertyDecl in GamePrefs.s_propertyList)
		{
			if (propertyDecl.IsPersistent)
			{
				switch (propertyDecl.type)
				{
				case GamePrefs.EnumType.Int:
					SdPlayerPrefs.SetInt(propertyDecl.name.ToStringCached<EnumGamePrefs>(), GamePrefs.GetInt(propertyDecl.name));
					break;
				case GamePrefs.EnumType.Float:
					SdPlayerPrefs.SetFloat(propertyDecl.name.ToStringCached<EnumGamePrefs>(), GamePrefs.GetFloat(propertyDecl.name));
					break;
				case GamePrefs.EnumType.String:
					SdPlayerPrefs.SetString(propertyDecl.name.ToStringCached<EnumGamePrefs>(), GamePrefs.GetString(propertyDecl.name));
					break;
				case GamePrefs.EnumType.Bool:
					SdPlayerPrefs.SetInt(propertyDecl.name.ToStringCached<EnumGamePrefs>(), GamePrefs.GetBool(propertyDecl.name) ? 1 : 0);
					break;
				case GamePrefs.EnumType.Binary:
					SdPlayerPrefs.SetString(propertyDecl.name.ToStringCached<EnumGamePrefs>(), Utils.ToBase64(GamePrefs.GetString(propertyDecl.name)));
					break;
				}
			}
		}
		SdPlayerPrefs.Save();
		SaveDataUtils.SaveDataManager.CommitAsync();
		Log.Out("Persistent GamePrefs saved");
	}

	// Token: 0x0600928B RID: 37515 RVA: 0x003763F0 File Offset: 0x003745F0
	public static object Parse(EnumGamePrefs _enum, string _val)
	{
		int num = GamePrefs.find(_enum);
		if (num == -1)
		{
			return null;
		}
		switch (GamePrefs.s_propertyList[num].type)
		{
		case GamePrefs.EnumType.Int:
		{
			int num2;
			if (!int.TryParse(_val, out num2))
			{
				num2 = 0;
			}
			return num2;
		}
		case GamePrefs.EnumType.Float:
			return StringParsers.ParseFloat(_val, 0, -1, NumberStyles.Any);
		case GamePrefs.EnumType.String:
			return _val;
		case GamePrefs.EnumType.Bool:
			return StringParsers.ParseBool(_val, 0, -1, true);
		case GamePrefs.EnumType.Binary:
			return _val;
		default:
			return null;
		}
	}

	// Token: 0x0600928C RID: 37516 RVA: 0x00376474 File Offset: 0x00374674
	public static string GetString(EnumGamePrefs _eProperty)
	{
		string result;
		try
		{
			result = (string)GamePrefs.GetObject(_eProperty);
		}
		catch (InvalidCastException e)
		{
			Log.Error("GetString: InvalidCastException " + _eProperty.ToStringCached<EnumGamePrefs>());
			Log.Exception(e);
			result = string.Empty;
		}
		return result;
	}

	// Token: 0x0600928D RID: 37517 RVA: 0x003764C4 File Offset: 0x003746C4
	public static float GetFloat(EnumGamePrefs _eProperty)
	{
		float result;
		try
		{
			BaseSandboxOption baseSandboxOption = GamePrefs.sandboxReferences[(int)_eProperty];
			if (baseSandboxOption != null)
			{
				result = baseSandboxOption.GetFloatValue();
			}
			else
			{
				object obj = GamePrefs.GetObject(_eProperty);
				if (obj != null)
				{
					result = (float)obj;
				}
				else
				{
					obj = GamePrefs.GetDefault(_eProperty);
					if (obj != null)
					{
						result = (float)obj;
					}
					else
					{
						Log.Error("GetFloat: GamePref {0}/{1} does not have a value/default", new object[]
						{
							(int)_eProperty,
							_eProperty.ToStringCached<EnumGamePrefs>()
						});
						result = 0f;
					}
				}
			}
		}
		catch (InvalidCastException e)
		{
			Log.Error("GetFloat: InvalidCastException " + _eProperty.ToStringCached<EnumGamePrefs>());
			Log.Exception(e);
			result = (float)GamePrefs.GetDefault(_eProperty);
		}
		return result;
	}

	// Token: 0x0600928E RID: 37518 RVA: 0x00376570 File Offset: 0x00374770
	public static int GetInt(EnumGamePrefs _eProperty)
	{
		int result;
		try
		{
			BaseSandboxOption baseSandboxOption = GamePrefs.sandboxReferences[(int)_eProperty];
			if (baseSandboxOption != null)
			{
				result = baseSandboxOption.GetIntValue();
			}
			else
			{
				object obj = GamePrefs.GetObject(_eProperty);
				if (obj != null)
				{
					result = (int)obj;
				}
				else
				{
					obj = GamePrefs.GetDefault(_eProperty);
					if (obj != null)
					{
						result = (int)obj;
					}
					else
					{
						Log.Error("GetInt: GamePref {0}/{1} does not have a value/default", new object[]
						{
							(int)_eProperty,
							_eProperty.ToStringCached<EnumGamePrefs>()
						});
						result = 0;
					}
				}
			}
		}
		catch (InvalidCastException e)
		{
			Log.Error("GetInt: InvalidCastException " + _eProperty.ToStringCached<EnumGamePrefs>());
			Log.Exception(e);
			result = 0;
		}
		return result;
	}

	// Token: 0x0600928F RID: 37519 RVA: 0x0037660C File Offset: 0x0037480C
	public static bool GetBool(EnumGamePrefs _eProperty)
	{
		bool result;
		try
		{
			BaseSandboxOption baseSandboxOption = GamePrefs.sandboxReferences[(int)_eProperty];
			if (baseSandboxOption != null)
			{
				result = baseSandboxOption.GetBoolValue();
			}
			else
			{
				object obj = GamePrefs.GetObject(_eProperty);
				if (obj != null)
				{
					result = (bool)obj;
				}
				else
				{
					obj = GamePrefs.GetDefault(_eProperty);
					if (obj != null)
					{
						result = (bool)obj;
					}
					else
					{
						Log.Error("GetBool: GamePref {0}/{1} does not have a value/default", new object[]
						{
							(int)_eProperty,
							_eProperty.ToStringCached<EnumGamePrefs>()
						});
						result = false;
					}
				}
			}
		}
		catch (InvalidCastException e)
		{
			Log.Error("GetBool: InvalidCastException " + _eProperty.ToStringCached<EnumGamePrefs>());
			Log.Exception(e);
			result = false;
		}
		return result;
	}

	// Token: 0x06009290 RID: 37520 RVA: 0x003766A8 File Offset: 0x003748A8
	public static void SetupSandboxReferences()
	{
		GamePrefs.sandboxReferences = new BaseSandboxOption[316];
		for (int i = 0; i < GamePrefs.s_propertyList.Length; i++)
		{
			GamePrefs.PropertyDecl propertyDecl = GamePrefs.s_propertyList[i];
			SandboxOptions optionType;
			if (Enum.TryParse<SandboxOptions>(propertyDecl.name.ToString(), out optionType))
			{
				GamePrefs.sandboxReferences[(int)propertyDecl.name] = SandboxOptionManager.GetOption(optionType);
			}
		}
	}

	// Token: 0x06009291 RID: 37521 RVA: 0x00376710 File Offset: 0x00374910
	public static object GetObject(EnumGamePrefs _eProperty)
	{
		int num = (int)_eProperty;
		if (num >= GamePrefs.Instance.propertyValues.Length)
		{
			Log.Error("GamePrefs: Trying to access non-existing pref " + num.ToString());
			return null;
		}
		return GamePrefs.Instance.propertyValues[(int)_eProperty];
	}

	// Token: 0x06009292 RID: 37522 RVA: 0x00376754 File Offset: 0x00374954
	[PublicizedFrom(EAccessModifier.Private)]
	public static int find(EnumGamePrefs _eProperty)
	{
		for (int i = 0; i < GamePrefs.s_propertyList.Length; i++)
		{
			if (GamePrefs.s_propertyList[i].name == _eProperty)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06009293 RID: 37523 RVA: 0x00376789 File Offset: 0x00374989
	public static bool Exists(EnumGamePrefs _eProperty)
	{
		return GamePrefs.find(_eProperty) != -1;
	}

	// Token: 0x06009294 RID: 37524 RVA: 0x00376798 File Offset: 0x00374998
	public static void SetPersistent(EnumGamePrefs _eProperty, bool _bPersistent)
	{
		int num = GamePrefs.find(_eProperty);
		if (num == -1)
		{
			Log.Error("Property value " + _eProperty.ToStringCached<EnumGamePrefs>() + " not found!");
			return;
		}
		if (_bPersistent)
		{
			GamePrefs.s_propertyList[num].bPersistent = (DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5);
			return;
		}
		GamePrefs.s_propertyList[num].bPersistent = DeviceFlag.None;
	}

	// Token: 0x06009295 RID: 37525 RVA: 0x003767F4 File Offset: 0x003749F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetObjectInternal(EnumGamePrefs _eProperty, object _value)
	{
		int num = (int)_eProperty;
		if (num >= this.propertyValues.Length)
		{
			Log.Error("GamePrefs: Trying to set non-existing pref " + num.ToString());
			return;
		}
		if (this.propertyValues[num] == null && _value == null)
		{
			return;
		}
		if (object.Equals(this.propertyValues[num], _value))
		{
			return;
		}
		this.propertyValues[num] = _value;
		GamePrefs.notifyListeners(_eProperty);
	}

	// Token: 0x06009296 RID: 37526 RVA: 0x00376853 File Offset: 0x00374A53
	public static void SetObject(EnumGamePrefs _eProperty, object _value)
	{
		GamePrefs.Instance.SetObjectInternal(_eProperty, _value);
	}

	// Token: 0x06009297 RID: 37527 RVA: 0x00376861 File Offset: 0x00374A61
	public static void Set(EnumGamePrefs _eProperty, int _value)
	{
		GamePrefs.SetObject(_eProperty, _value);
	}

	// Token: 0x06009298 RID: 37528 RVA: 0x0037686F File Offset: 0x00374A6F
	public static void Set(EnumGamePrefs _eProperty, float _value)
	{
		GamePrefs.SetObject(_eProperty, _value);
	}

	// Token: 0x06009299 RID: 37529 RVA: 0x0037687D File Offset: 0x00374A7D
	public static void Set(EnumGamePrefs _eProperty, string _value)
	{
		GamePrefs.SetObject(_eProperty, _value);
	}

	// Token: 0x0600929A RID: 37530 RVA: 0x00376886 File Offset: 0x00374A86
	public static void Set(EnumGamePrefs _eProperty, bool _value)
	{
		GamePrefs.SetObject(_eProperty, _value);
	}

	// Token: 0x0600929B RID: 37531 RVA: 0x00376894 File Offset: 0x00374A94
	public static object[] GetSettingsCopy()
	{
		object[] array = new object[GamePrefs.Instance.propertyValues.Length];
		Array.Copy(GamePrefs.Instance.propertyValues, array, array.Length);
		return array;
	}

	// Token: 0x0600929C RID: 37532 RVA: 0x003768C7 File Offset: 0x00374AC7
	public static void ApplySettingsCopy(object[] _settings)
	{
		Array.Copy(_settings, GamePrefs.Instance.propertyValues, _settings.Length);
	}

	// Token: 0x0600929D RID: 37533 RVA: 0x003768DC File Offset: 0x00374ADC
	public static bool IsDefault(EnumGamePrefs _eProperty)
	{
		int num = (int)_eProperty;
		if (num >= GamePrefs.Instance.propertyValues.Length)
		{
			Log.Error("GamePrefs: Trying to get default of non-existing pref " + num.ToString());
			return true;
		}
		return GamePrefs.Instance.propertyValues[num] != null && GamePrefs.Instance.propertyValues[num].Equals(GamePrefs.GetDefault(_eProperty));
	}

	// Token: 0x0600929E RID: 37534 RVA: 0x0037693C File Offset: 0x00374B3C
	public static object GetDefault(EnumGamePrefs _eProperty)
	{
		foreach (GamePrefs.PropertyDecl propertyDecl in GamePrefs.s_propertyList)
		{
			if (propertyDecl.name == _eProperty)
			{
				return propertyDecl.defaultValue;
			}
		}
		return null;
	}

	// Token: 0x0600929F RID: 37535 RVA: 0x00376978 File Offset: 0x00374B78
	public static GamePrefs.EnumType? GetPrefType(EnumGamePrefs _eProperty)
	{
		foreach (GamePrefs.PropertyDecl propertyDecl in GamePrefs.s_propertyList)
		{
			if (propertyDecl.name == _eProperty)
			{
				return new GamePrefs.EnumType?(propertyDecl.type);
			}
		}
		return null;
	}

	// Token: 0x060092A0 RID: 37536 RVA: 0x003769C0 File Offset: 0x00374BC0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PrintNonStockWarning(GamePrefs.PropertyDecl prop, object curVal, object allowedMin, object allowedMax = null)
	{
		if (allowedMax != null)
		{
			Log.Out(string.Format("Setting for '{0}' not within the default range (server will go to the modded category): current = {1}, default = {2} - {3}", new object[]
			{
				prop.name.ToStringCached<EnumGamePrefs>(),
				curVal.ToString(),
				allowedMin.ToString(),
				allowedMax.ToString()
			}));
			return;
		}
		Log.Out(string.Format("Setting for '{0}' does not match the default (server will go to the modded category): current = {1}, default = {2}", prop.name.ToStringCached<EnumGamePrefs>(), curVal.ToString(), allowedMin.ToString()));
	}

	// Token: 0x060092A1 RID: 37537 RVA: 0x00376A38 File Offset: 0x00374C38
	public static bool HasStockSettings()
	{
		bool result = true;
		foreach (GamePrefs.PropertyDecl propertyDecl in GamePrefs.s_propertyList)
		{
			if (propertyDecl.minStockValue != null)
			{
				switch (propertyDecl.type)
				{
				case GamePrefs.EnumType.Int:
				{
					int @int = GamePrefs.GetInt(propertyDecl.name);
					int num = (int)propertyDecl.minStockValue;
					if (propertyDecl.maxStockValue == null)
					{
						if (@int != num)
						{
							GamePrefs.PrintNonStockWarning(propertyDecl, @int, num, null);
							result = false;
						}
					}
					else
					{
						int num2 = (int)propertyDecl.maxStockValue;
						if (@int < num || @int > num2)
						{
							GamePrefs.PrintNonStockWarning(propertyDecl, @int, num, num2);
							result = false;
						}
					}
					break;
				}
				case GamePrefs.EnumType.Float:
				{
					float @float = GamePrefs.GetFloat(propertyDecl.name);
					float num3 = (float)propertyDecl.minStockValue;
					if (propertyDecl.maxStockValue == null)
					{
						if (!Mathf.Approximately(@float, num3))
						{
							GamePrefs.PrintNonStockWarning(propertyDecl, @float, num3, null);
							result = false;
						}
					}
					else
					{
						float num4 = (float)propertyDecl.maxStockValue;
						if (@float < num3 || @float > num4)
						{
							GamePrefs.PrintNonStockWarning(propertyDecl, @float, num3, num4);
							result = false;
						}
					}
					break;
				}
				case GamePrefs.EnumType.String:
				case GamePrefs.EnumType.Binary:
				{
					string @string = GamePrefs.GetString(propertyDecl.name);
					string text = (string)propertyDecl.minStockValue;
					if (!@string.Equals(text))
					{
						GamePrefs.PrintNonStockWarning(propertyDecl, @string, text, null);
						result = false;
					}
					break;
				}
				case GamePrefs.EnumType.Bool:
				{
					bool @bool = GamePrefs.GetBool(propertyDecl.name);
					bool flag = (bool)propertyDecl.minStockValue;
					if (@bool != flag)
					{
						GamePrefs.PrintNonStockWarning(propertyDecl, @bool, flag, null);
						result = false;
					}
					break;
				}
				}
			}
		}
		return result;
	}

	// Token: 0x060092A2 RID: 37538 RVA: 0x00376C14 File Offset: 0x00374E14
	public static void AddChangeListener(IGamePrefsChangedListener _listener)
	{
		GamePrefs.listeners.Add(_listener);
	}

	// Token: 0x060092A3 RID: 37539 RVA: 0x00376C21 File Offset: 0x00374E21
	public static void RemoveChangeListener(IGamePrefsChangedListener _listener)
	{
		GamePrefs.listeners.Remove(_listener);
	}

	// Token: 0x060092A4 RID: 37540 RVA: 0x00376C30 File Offset: 0x00374E30
	[PublicizedFrom(EAccessModifier.Private)]
	public static void notifyListeners(EnumGamePrefs _enum)
	{
		for (int i = 0; i < GamePrefs.listeners.Count; i++)
		{
			GamePrefs.listeners[i].OnGamePrefChanged(_enum);
		}
		Action<EnumGamePrefs> onGamePrefChanged = GamePrefs.OnGamePrefChanged;
		if (onGamePrefChanged == null)
		{
			return;
		}
		onGamePrefChanged(_enum);
	}

	// Token: 0x04006D85 RID: 28037
	[PublicizedFrom(EAccessModifier.Private)]
	public static GamePrefs.PropertyDecl[] s_propertyList;

	// Token: 0x04006D86 RID: 28038
	[PublicizedFrom(EAccessModifier.Private)]
	public object[] propertyValues = new object[316];

	// Token: 0x04006D87 RID: 28039
	[PublicizedFrom(EAccessModifier.Private)]
	public static BaseSandboxOption[] sandboxReferences;

	// Token: 0x04006D88 RID: 28040
	[PublicizedFrom(EAccessModifier.Private)]
	public static GamePrefs m_Instance;

	// Token: 0x04006D89 RID: 28041
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<IGamePrefsChangedListener> listeners = new List<IGamePrefsChangedListener>();

	// Token: 0x020011EE RID: 4590
	public enum EnumType
	{
		// Token: 0x04006D8C RID: 28044
		Int,
		// Token: 0x04006D8D RID: 28045
		Float,
		// Token: 0x04006D8E RID: 28046
		String,
		// Token: 0x04006D8F RID: 28047
		Bool,
		// Token: 0x04006D90 RID: 28048
		Binary
	}

	// Token: 0x020011EF RID: 4591
	public struct PropertyDecl
	{
		// Token: 0x1700117C RID: 4476
		// (get) Token: 0x060092A7 RID: 37543 RVA: 0x00376C97 File Offset: 0x00374E97
		public bool IsPersistent
		{
			get
			{
				return this.bPersistent.HasFlag(DeviceFlag.StandaloneWindows);
			}
		}

		// Token: 0x060092A8 RID: 37544 RVA: 0x00376CB0 File Offset: 0x00374EB0
		public PropertyDecl(EnumGamePrefs _name, DeviceFlag _bPersistent, GamePrefs.EnumType _type, object _defaultValue, object _minStockValue, object _maxStockValue, Dictionary<DeviceFlag, object> _deviceDefaults = null)
		{
			this.name = _name;
			this.type = _type;
			this.bPersistent = _bPersistent;
			this.minStockValue = _minStockValue;
			this.maxStockValue = _maxStockValue;
			if (_deviceDefaults != null && _deviceDefaults.ContainsKey(DeviceFlag.StandaloneWindows))
			{
				this.defaultValue = _deviceDefaults[DeviceFlag.StandaloneWindows];
				return;
			}
			this.defaultValue = _defaultValue;
		}

		// Token: 0x04006D91 RID: 28049
		public EnumGamePrefs name;

		// Token: 0x04006D92 RID: 28050
		public GamePrefs.EnumType type;

		// Token: 0x04006D93 RID: 28051
		public object defaultValue;

		// Token: 0x04006D94 RID: 28052
		public DeviceFlag bPersistent;

		// Token: 0x04006D95 RID: 28053
		public object minStockValue;

		// Token: 0x04006D96 RID: 28054
		public object maxStockValue;
	}
}
