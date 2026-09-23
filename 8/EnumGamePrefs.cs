using System;

// Token: 0x020011E3 RID: 4579
public enum EnumGamePrefs
{
	// Token: 0x04006C26 RID: 27686
	CreateLevelName,
	// Token: 0x04006C27 RID: 27687
	CreateLevelDim,
	// Token: 0x04006C28 RID: 27688
	OptionsAmbientVolumeLevel,
	// Token: 0x04006C29 RID: 27689
	OptionsMusicVolumeLevel,
	// Token: 0x04006C2A RID: 27690
	OptionsMenuMusicVolumeLevel,
	// Token: 0x04006C2B RID: 27691
	OptionsOverallAudioVolumeLevel,
	// Token: 0x04006C2C RID: 27692
	OptionsGfxAASharpness,
	// Token: 0x04006C2D RID: 27693
	OptionsGfxWaterQuality,
	// Token: 0x04006C2E RID: 27694
	OptionsGfxViewDistance,
	// Token: 0x04006C2F RID: 27695
	OptionsGfxShadowDistance,
	// Token: 0x04006C30 RID: 27696
	UNUSED_OptionsPlayerModel,
	// Token: 0x04006C31 RID: 27697
	OptionsPlayerModelTexture,
	// Token: 0x04006C32 RID: 27698
	OptionsGfxAA,
	// Token: 0x04006C33 RID: 27699
	OptionsLookSensitivity,
	// Token: 0x04006C34 RID: 27700
	OptionsZoomSensitivity,
	// Token: 0x04006C35 RID: 27701
	OptionsInvertMouse,
	// Token: 0x04006C36 RID: 27702
	OptionsGfxFOV,
	// Token: 0x04006C37 RID: 27703
	UNUSED_OptionsFieldOfViewNew,
	// Token: 0x04006C38 RID: 27704
	ServerPort,
	// Token: 0x04006C39 RID: 27705
	ServerIP,
	// Token: 0x04006C3A RID: 27706
	ServerPassword,
	// Token: 0x04006C3B RID: 27707
	ServerName,
	// Token: 0x04006C3C RID: 27708
	ServerDescription,
	// Token: 0x04006C3D RID: 27709
	ServerWebsiteURL,
	// Token: 0x04006C3E RID: 27710
	ServerPasswordCache,
	// Token: 0x04006C3F RID: 27711
	ServerIsPublic,
	// Token: 0x04006C40 RID: 27712
	ServerMaxPlayerCount,
	// Token: 0x04006C41 RID: 27713
	ServerAllowCrossplay,
	// Token: 0x04006C42 RID: 27714
	ServerEACPeerToPeer,
	// Token: 0x04006C43 RID: 27715
	GameMode,
	// Token: 0x04006C44 RID: 27716
	GameDifficulty,
	// Token: 0x04006C45 RID: 27717
	GameName,
	// Token: 0x04006C46 RID: 27718
	GameNameClient,
	// Token: 0x04006C47 RID: 27719
	GameWorld,
	// Token: 0x04006C48 RID: 27720
	GameVersion,
	// Token: 0x04006C49 RID: 27721
	ConnectToServerIP,
	// Token: 0x04006C4A RID: 27722
	ConnectToServerPort,
	// Token: 0x04006C4B RID: 27723
	PlayerName,
	// Token: 0x04006C4C RID: 27724
	UNUSED_PlayerId,
	// Token: 0x04006C4D RID: 27725
	PlayerPassword,
	// Token: 0x04006C4E RID: 27726
	PlayerAutologin,
	// Token: 0x04006C4F RID: 27727
	PlayerToken,
	// Token: 0x04006C50 RID: 27728
	PlayerSafeZoneHours,
	// Token: 0x04006C51 RID: 27729
	PlayerSafeZoneLevel,
	// Token: 0x04006C52 RID: 27730
	DebugMenuShowTasks,
	// Token: 0x04006C53 RID: 27731
	DebugMenuEnabled,
	// Token: 0x04006C54 RID: 27732
	DebugStopEnemiesMoving,
	// Token: 0x04006C55 RID: 27733
	CreativeMenuEnabled,
	// Token: 0x04006C56 RID: 27734
	FavoriteServersList,
	// Token: 0x04006C57 RID: 27735
	UNUSED_ControlPanelPort,
	// Token: 0x04006C58 RID: 27736
	UNUSED_ControlPanelPassword,
	// Token: 0x04006C59 RID: 27737
	DynamicSpawner,
	// Token: 0x04006C5A RID: 27738
	PlayerKillingMode,
	// Token: 0x04006C5B RID: 27739
	MatchLength,
	// Token: 0x04006C5C RID: 27740
	FragLimit,
	// Token: 0x04006C5D RID: 27741
	RebuildMap,
	// Token: 0x04006C5E RID: 27742
	JoiningOptions,
	// Token: 0x04006C5F RID: 27743
	ZombiePlayers,
	// Token: 0x04006C60 RID: 27744
	BuildCreate,
	// Token: 0x04006C61 RID: 27745
	DayCount,
	// Token: 0x04006C62 RID: 27746
	DayNightLength,
	// Token: 0x04006C63 RID: 27747
	DayLightLength,
	// Token: 0x04006C64 RID: 27748
	BloodMoonFrequency,
	// Token: 0x04006C65 RID: 27749
	BloodMoonRange,
	// Token: 0x04006C66 RID: 27750
	BloodMoonWarning,
	// Token: 0x04006C67 RID: 27751
	ShowFriendPlayerOnMap,
	// Token: 0x04006C68 RID: 27752
	AdminFileName,
	// Token: 0x04006C69 RID: 27753
	UNUSED_ControlPanelEnabled,
	// Token: 0x04006C6A RID: 27754
	TelnetEnabled,
	// Token: 0x04006C6B RID: 27755
	TelnetPort,
	// Token: 0x04006C6C RID: 27756
	ZombieFeralSense,
	// Token: 0x04006C6D RID: 27757
	UNUSED_OptionsSSAO,
	// Token: 0x04006C6E RID: 27758
	ZombieMove,
	// Token: 0x04006C6F RID: 27759
	ZombieMoveNight,
	// Token: 0x04006C70 RID: 27760
	ZombieFeralMove,
	// Token: 0x04006C71 RID: 27761
	ZombieBMMove,
	// Token: 0x04006C72 RID: 27762
	OptionsGfxLODDistance,
	// Token: 0x04006C73 RID: 27763
	DropOnDeath,
	// Token: 0x04006C74 RID: 27764
	DropOnQuit,
	// Token: 0x04006C75 RID: 27765
	DeathPenalty,
	// Token: 0x04006C76 RID: 27766
	LootTimer,
	// Token: 0x04006C77 RID: 27767
	BloodMoonEnemyCount,
	// Token: 0x04006C78 RID: 27768
	EnemySpawnMode,
	// Token: 0x04006C79 RID: 27769
	EnemyDifficulty,
	// Token: 0x04006C7A RID: 27770
	BlockDamagePlayer,
	// Token: 0x04006C7B RID: 27771
	BlockDamageAI,
	// Token: 0x04006C7C RID: 27772
	BlockDamageAIBM,
	// Token: 0x04006C7D RID: 27773
	LootAbundance,
	// Token: 0x04006C7E RID: 27774
	LootRespawnDays,
	// Token: 0x04006C7F RID: 27775
	TelnetPassword,
	// Token: 0x04006C80 RID: 27776
	LandClaimCount,
	// Token: 0x04006C81 RID: 27777
	LandClaimSize,
	// Token: 0x04006C82 RID: 27778
	LandClaimDeadZone,
	// Token: 0x04006C83 RID: 27779
	LandClaimExpiryTime,
	// Token: 0x04006C84 RID: 27780
	LandClaimDecayMode,
	// Token: 0x04006C85 RID: 27781
	LandClaimOnlineDurabilityModifier,
	// Token: 0x04006C86 RID: 27782
	LandClaimOfflineDurabilityModifier,
	// Token: 0x04006C87 RID: 27783
	LandClaimOfflineDelay,
	// Token: 0x04006C88 RID: 27784
	AirDropFrequency,
	// Token: 0x04006C89 RID: 27785
	MaxSpawnedZombies,
	// Token: 0x04006C8A RID: 27786
	PartySharedKillRange,
	// Token: 0x04006C8B RID: 27787
	UNUSED_SaveGameFolder,
	// Token: 0x04006C8C RID: 27788
	OptionsMicVolumeLevel,
	// Token: 0x04006C8D RID: 27789
	OptionsVoiceVolumeLevel,
	// Token: 0x04006C8E RID: 27790
	OptionsVoiceChatEnabled,
	// Token: 0x04006C8F RID: 27791
	OptionsGfxTexQuality,
	// Token: 0x04006C90 RID: 27792
	AutopilotMode,
	// Token: 0x04006C91 RID: 27793
	SelectionOperationMode,
	// Token: 0x04006C92 RID: 27794
	SelectionContextMode,
	// Token: 0x04006C93 RID: 27795
	EACEnabled,
	// Token: 0x04006C94 RID: 27796
	PersistentPlayerProfiles,
	// Token: 0x04006C95 RID: 27797
	XPMultiplier,
	// Token: 0x04006C96 RID: 27798
	OptionsAudioOcclusion,
	// Token: 0x04006C97 RID: 27799
	LastGameResetRevision,
	// Token: 0x04006C98 RID: 27800
	OptionsGfxResolution,
	// Token: 0x04006C99 RID: 27801
	OptionsGfxVsync,
	// Token: 0x04006C9A RID: 27802
	OptionsGfxReflectQuality,
	// Token: 0x04006C9B RID: 27803
	OptionsGfxResetRevision,
	// Token: 0x04006C9C RID: 27804
	OptionsGfxSignQuality,
	// Token: 0x04006C9D RID: 27805
	UNUSED_OptionsReflectionCullList,
	// Token: 0x04006C9E RID: 27806
	UNUSED_OptionsReflectionFarClip,
	// Token: 0x04006C9F RID: 27807
	UNUSED_OptionsReflectionShadowDistance,
	// Token: 0x04006CA0 RID: 27808
	UNUSED_OptionsReflectionBounces,
	// Token: 0x04006CA1 RID: 27809
	UNUSED_OptionsReflectionTimeSlicingMode,
	// Token: 0x04006CA2 RID: 27810
	UNUSED_OptionsReflectionRefreshMode,
	// Token: 0x04006CA3 RID: 27811
	OptionsGfxObjQuality,
	// Token: 0x04006CA4 RID: 27812
	OptionsGfxGrassDistance,
	// Token: 0x04006CA5 RID: 27813
	UNUSED_OptionsMotionBlur,
	// Token: 0x04006CA6 RID: 27814
	UNUSED_OptionsObjectBlur,
	// Token: 0x04006CA7 RID: 27815
	MaxSpawnedAnimals,
	// Token: 0x04006CA8 RID: 27816
	UNUSED_OptionsBloom,
	// Token: 0x04006CA9 RID: 27817
	UNUSED_OptionsSunShafts,
	// Token: 0x04006CAA RID: 27818
	UNUSED_OptionsDOF,
	// Token: 0x04006CAB RID: 27819
	OptionsGfxReflectShadows,
	// Token: 0x04006CAC RID: 27820
	OptionsAllowController,
	// Token: 0x04006CAD RID: 27821
	OptionsGfxQualityPreset,
	// Token: 0x04006CAE RID: 27822
	OptionsScreenBoundsValue,
	// Token: 0x04006CAF RID: 27823
	OptionsInterfaceSensitivity,
	// Token: 0x04006CB0 RID: 27824
	OptionsControllerVibration,
	// Token: 0x04006CB1 RID: 27825
	NoGraphicsMode,
	// Token: 0x04006CB2 RID: 27826
	OptionsHudSize,
	// Token: 0x04006CB3 RID: 27827
	OptionsHudOpacity,
	// Token: 0x04006CB4 RID: 27828
	OptionsShowCrosshair,
	// Token: 0x04006CB5 RID: 27829
	OptionsShowCompass,
	// Token: 0x04006CB6 RID: 27830
	ServerDisabledNetworkProtocols,
	// Token: 0x04006CB7 RID: 27831
	OptionsBackgroundGlobalOpacity,
	// Token: 0x04006CB8 RID: 27832
	OptionsForegroundGlobalOpacity,
	// Token: 0x04006CB9 RID: 27833
	UNUSED_OptionsGamma,
	// Token: 0x04006CBA RID: 27834
	OptionsStabSpawnBlocksOnGround,
	// Token: 0x04006CBB RID: 27835
	OptionsTempCelsius,
	// Token: 0x04006CBC RID: 27836
	AirDropMarker,
	// Token: 0x04006CBD RID: 27837
	OptionsGfxWaterPtlLimiter,
	// Token: 0x04006CBE RID: 27838
	UNUSED_OptionsGfxUMATexQuality,
	// Token: 0x04006CBF RID: 27839
	HideCommandExecutionLog,
	// Token: 0x04006CC0 RID: 27840
	MaxUncoveredMapChunksPerPlayer,
	// Token: 0x04006CC1 RID: 27841
	ServerReservedSlots,
	// Token: 0x04006CC2 RID: 27842
	ServerReservedSlotsPermission,
	// Token: 0x04006CC3 RID: 27843
	ServerAdminSlots,
	// Token: 0x04006CC4 RID: 27844
	ServerAdminSlotsPermission,
	// Token: 0x04006CC5 RID: 27845
	GameGuidClient,
	// Token: 0x04006CC6 RID: 27846
	BedrollDeadZoneSize,
	// Token: 0x04006CC7 RID: 27847
	LastLoadedPrefab,
	// Token: 0x04006CC8 RID: 27848
	UNUSED_LastLoadedPrefabSize,
	// Token: 0x04006CC9 RID: 27849
	OptionsJournalPopup,
	// Token: 0x04006CCA RID: 27850
	OptionsFilterProfanity,
	// Token: 0x04006CCB RID: 27851
	TelnetFailedLoginLimit,
	// Token: 0x04006CCC RID: 27852
	TelnetFailedLoginsBlocktime,
	// Token: 0x04006CCD RID: 27853
	TerminalWindowEnabled,
	// Token: 0x04006CCE RID: 27854
	ServerEnabled,
	// Token: 0x04006CCF RID: 27855
	ServerVisibility,
	// Token: 0x04006CD0 RID: 27856
	ServerLoginConfirmationText,
	// Token: 0x04006CD1 RID: 27857
	WorldGenSeed,
	// Token: 0x04006CD2 RID: 27858
	WorldGenSize,
	// Token: 0x04006CD3 RID: 27859
	OptionsGfxTreeDistance,
	// Token: 0x04006CD4 RID: 27860
	OptionsPOICulling,
	// Token: 0x04006CD5 RID: 27861
	OptionsDynamicMusicEnabled,
	// Token: 0x04006CD6 RID: 27862
	OptionsDynamicMusicDailyTime,
	// Token: 0x04006CD7 RID: 27863
	OptionsPlayChanceFrequency,
	// Token: 0x04006CD8 RID: 27864
	OptionsPlayChanceProbability,
	// Token: 0x04006CD9 RID: 27865
	UNUSED_UserDataFolder,
	// Token: 0x04006CDA RID: 27866
	OptionsGfxStreamMipmaps,
	// Token: 0x04006CDB RID: 27867
	UNUSED_OptionsStreamingMipmapsBudget,
	// Token: 0x04006CDC RID: 27868
	OptionsGfxBloom,
	// Token: 0x04006CDD RID: 27869
	OptionsGfxDOF,
	// Token: 0x04006CDE RID: 27870
	OptionsGfxMotionBlur,
	// Token: 0x04006CDF RID: 27871
	OptionsGfxSSAO,
	// Token: 0x04006CE0 RID: 27872
	OptionsGfxSSReflections,
	// Token: 0x04006CE1 RID: 27873
	OptionsGfxSunShafts,
	// Token: 0x04006CE2 RID: 27874
	OptionsDisableChunkLODs,
	// Token: 0x04006CE3 RID: 27875
	ServerMaxWorldTransferSpeedKiBs,
	// Token: 0x04006CE4 RID: 27876
	ServerMaxAllowedViewDistance,
	// Token: 0x04006CE5 RID: 27877
	OptionsGfxOcclusion,
	// Token: 0x04006CE6 RID: 27878
	BedrollExpiryTime,
	// Token: 0x04006CE7 RID: 27879
	OptionsGfxTexFilter,
	// Token: 0x04006CE8 RID: 27880
	OptionsGfxTerrainQuality,
	// Token: 0x04006CE9 RID: 27881
	OptionsGfxBrightness,
	// Token: 0x04006CEA RID: 27882
	LastLoadingTipRead,
	// Token: 0x04006CEB RID: 27883
	OptionsGfxDynamicMode,
	// Token: 0x04006CEC RID: 27884
	OptionsGfxDynamicMinFPS,
	// Token: 0x04006CED RID: 27885
	OptionsGfxDynamicScale,
	// Token: 0x04006CEE RID: 27886
	OptionsUiFpsScaling,
	// Token: 0x04006CEF RID: 27887
	OptionsControlsResetRevision,
	// Token: 0x04006CF0 RID: 27888
	OptionsWeaponAiming,
	// Token: 0x04006CF1 RID: 27889
	DynamicMeshEnabled,
	// Token: 0x04006CF2 RID: 27890
	DynamicMeshDistance,
	// Token: 0x04006CF3 RID: 27891
	DynamicMeshLandClaimOnly,
	// Token: 0x04006CF4 RID: 27892
	DynamicMeshLandClaimBuffer,
	// Token: 0x04006CF5 RID: 27893
	DynamicMeshUseImposters,
	// Token: 0x04006CF6 RID: 27894
	DynamicMeshMaxRegionCache,
	// Token: 0x04006CF7 RID: 27895
	DynamicMeshMaxItemCache,
	// Token: 0x04006CF8 RID: 27896
	TwitchBloodMoonAllowed,
	// Token: 0x04006CF9 RID: 27897
	TwitchServerPermission,
	// Token: 0x04006CFA RID: 27898
	OptionsVehicleLookSensitivity,
	// Token: 0x04006CFB RID: 27899
	OptionsSelectionBoxAlphaMultiplier,
	// Token: 0x04006CFC RID: 27900
	PlaytestBiome,
	// Token: 0x04006CFD RID: 27901
	Language,
	// Token: 0x04006CFE RID: 27902
	LanguageBrowser,
	// Token: 0x04006CFF RID: 27903
	Region,
	// Token: 0x04006D00 RID: 27904
	ServerHistoryCache,
	// Token: 0x04006D01 RID: 27905
	OptionsVoiceInputDevice,
	// Token: 0x04006D02 RID: 27906
	OptionsVoiceOutputDevice,
	// Token: 0x04006D03 RID: 27907
	MaxChunkAge,
	// Token: 0x04006D04 RID: 27908
	SaveDataLimit,
	// Token: 0x04006D05 RID: 27909
	OptionsSubtitlesEnabled,
	// Token: 0x04006D06 RID: 27910
	OptionsIntroMovieEnabled,
	// Token: 0x04006D07 RID: 27911
	AllowSpawnNearBackpack,
	// Token: 0x04006D08 RID: 27912
	OptionsZoomAccel,
	// Token: 0x04006D09 RID: 27913
	UNUSED_NewGameSetDefaults,
	// Token: 0x04006D0A RID: 27914
	UNUSED_OptionsGfxGameplayResolutionWidth,
	// Token: 0x04006D0B RID: 27915
	UNUSED_OptionsGfxGameplayResolutionHeight,
	// Token: 0x04006D0C RID: 27916
	OptionsMumblePositionalAudioSupport,
	// Token: 0x04006D0D RID: 27917
	WebDashboardEnabled,
	// Token: 0x04006D0E RID: 27918
	WebDashboardPort,
	// Token: 0x04006D0F RID: 27919
	WebDashboardUrl,
	// Token: 0x04006D10 RID: 27920
	EnableMapRendering,
	// Token: 0x04006D11 RID: 27921
	OptionsAutoPartyWithFriends,
	// Token: 0x04006D12 RID: 27922
	OptionsQuestsAutoShare,
	// Token: 0x04006D13 RID: 27923
	OptionsQuestsAutoAccept,
	// Token: 0x04006D14 RID: 27924
	OptionsControllerTriggerEffects,
	// Token: 0x04006D15 RID: 27925
	MaxQueuedMeshLayers,
	// Token: 0x04006D16 RID: 27926
	OptionsControllerSensitivityX,
	// Token: 0x04006D17 RID: 27927
	OptionsControllerSensitivityY,
	// Token: 0x04006D18 RID: 27928
	OptionsControllerLookInvert,
	// Token: 0x04006D19 RID: 27929
	OptionsControllerJoystickLayout,
	// Token: 0x04006D1A RID: 27930
	OptionsControllerLookAcceleration,
	// Token: 0x04006D1B RID: 27931
	OptionsControllerZoomSensitivity,
	// Token: 0x04006D1C RID: 27932
	OptionsControllerLookAxisDeadzone,
	// Token: 0x04006D1D RID: 27933
	OptionsControllerMoveAxisDeadzone,
	// Token: 0x04006D1E RID: 27934
	OptionsControllerCursorSnap,
	// Token: 0x04006D1F RID: 27935
	OptionsControllerCursorHoverSensitivity,
	// Token: 0x04006D20 RID: 27936
	OptionsControllerVehicleSensitivity,
	// Token: 0x04006D21 RID: 27937
	OptionsControllerWeaponAiming,
	// Token: 0x04006D22 RID: 27938
	OptionsControllerAimAssists,
	// Token: 0x04006D23 RID: 27939
	OptionsChatCommunication,
	// Token: 0x04006D24 RID: 27940
	OptionsControlsSprintLock,
	// Token: 0x04006D25 RID: 27941
	OptionsDisableXmlEvents,
	// Token: 0x04006D26 RID: 27942
	DebugPanelsEnabled,
	// Token: 0x04006D27 RID: 27943
	OptionsControllerVibrationStrength,
	// Token: 0x04006D28 RID: 27944
	EulaVersionAccepted,
	// Token: 0x04006D29 RID: 27945
	EulaLatestVersion,
	// Token: 0x04006D2A RID: 27946
	OptionsGfxMotionBlurEnabled,
	// Token: 0x04006D2B RID: 27947
	IgnoreEOSSanctions,
	// Token: 0x04006D2C RID: 27948
	SkipSpawnButton,
	// Token: 0x04006D2D RID: 27949
	OptionsUiCompassUseEnglishCardinalDirections,
	// Token: 0x04006D2E RID: 27950
	OptionsGfxShadowQuality,
	// Token: 0x04006D2F RID: 27951
	QuestProgressionDailyLimit,
	// Token: 0x04006D30 RID: 27952
	OptionsControllerIconStyle,
	// Token: 0x04006D31 RID: 27953
	OptionsShowConsoleButton,
	// Token: 0x04006D32 RID: 27954
	SaveDataLimitType,
	// Token: 0x04006D33 RID: 27955
	OptionsCrossplay,
	// Token: 0x04006D34 RID: 27956
	AllowSpawnNearFriend,
	// Token: 0x04006D35 RID: 27957
	BiomeProgression,
	// Token: 0x04006D36 RID: 27958
	ServerMatchmakingGroup,
	// Token: 0x04006D37 RID: 27959
	OptionsGfxUpscalerMode,
	// Token: 0x04006D38 RID: 27960
	OptionsGfxFSRPreset,
	// Token: 0x04006D39 RID: 27961
	StormFreq,
	// Token: 0x04006D3A RID: 27962
	OptionsGfxFOV3P,
	// Token: 0x04006D3B RID: 27963
	OptionsGfxDefaultFirstPersonCamera,
	// Token: 0x04006D3C RID: 27964
	OptionsGfx3PCameraMode,
	// Token: 0x04006D3D RID: 27965
	OptionsPoiVolumesSkipDeleteConfirmation,
	// Token: 0x04006D3E RID: 27966
	CameraRestrictionMode,
	// Token: 0x04006D3F RID: 27967
	OptionsGfxCameraDistance3P,
	// Token: 0x04006D40 RID: 27968
	AISmellMode,
	// Token: 0x04006D41 RID: 27969
	OptionsControlsDefaultQuickAction,
	// Token: 0x04006D42 RID: 27970
	OptionsBindingsResetRevision,
	// Token: 0x04006D43 RID: 27971
	JarRefund,
	// Token: 0x04006D44 RID: 27972
	OptionsCrosshairEnabled,
	// Token: 0x04006D45 RID: 27973
	OptionsCrosshairScale,
	// Token: 0x04006D46 RID: 27974
	OptionsCrosshairColor,
	// Token: 0x04006D47 RID: 27975
	OptionsCrosshairOpacity,
	// Token: 0x04006D48 RID: 27976
	OptionsCrosshairADS,
	// Token: 0x04006D49 RID: 27977
	OptionsCrosshairDot3P,
	// Token: 0x04006D4A RID: 27978
	OptionsCrosshairThickness,
	// Token: 0x04006D4B RID: 27979
	OptionsCrosshairRangedEnabled,
	// Token: 0x04006D4C RID: 27980
	GameSaveStorageType,
	// Token: 0x04006D4D RID: 27981
	SandboxPreset,
	// Token: 0x04006D4E RID: 27982
	SandboxCode,
	// Token: 0x04006D4F RID: 27983
	UserWorldStorageType,
	// Token: 0x04006D50 RID: 27984
	DiscordFirstTimeInfoShown,
	// Token: 0x04006D51 RID: 27985
	DiscordDisabled,
	// Token: 0x04006D52 RID: 27986
	DiscordLastAccountType,
	// Token: 0x04006D53 RID: 27987
	DiscordAccessToken,
	// Token: 0x04006D54 RID: 27988
	DiscordRefreshToken,
	// Token: 0x04006D55 RID: 27989
	DiscordSelectedOutputDevice,
	// Token: 0x04006D56 RID: 27990
	DiscordSelectedInputDevice,
	// Token: 0x04006D57 RID: 27991
	DiscordOutputVolume,
	// Token: 0x04006D58 RID: 27992
	DiscordInputVolume,
	// Token: 0x04006D59 RID: 27993
	DiscordVoiceModePtt,
	// Token: 0x04006D5A RID: 27994
	DiscordVoiceVadModeAuto,
	// Token: 0x04006D5B RID: 27995
	DiscordVoiceVadThreshold,
	// Token: 0x04006D5C RID: 27996
	DiscordDmPrivacyMode,
	// Token: 0x04006D5D RID: 27997
	DiscordAutoJoinVoiceMode,
	// Token: 0x04006D5E RID: 27998
	ResetUnprotectedChunks,
	// Token: 0x04006D5F RID: 27999
	GameWorldLocationType,
	// Token: 0x04006D60 RID: 28000
	OptionsGfxLimitFpsInGame,
	// Token: 0x04006D61 RID: 28001
	DiscordMuteDmNotifications,
	// Token: 0x04006D62 RID: 28002
	Last
}
