using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x020011D7 RID: 4567
public static class GameOptionsManager
{
	// Token: 0x140000F4 RID: 244
	// (add) Token: 0x0600922A RID: 37418 RVA: 0x003717A0 File Offset: 0x0036F9A0
	// (remove) Token: 0x0600922B RID: 37419 RVA: 0x003717D4 File Offset: 0x0036F9D4
	public static event Action<int, int> ResolutionChanged;

	// Token: 0x140000F5 RID: 245
	// (add) Token: 0x0600922C RID: 37420 RVA: 0x00371808 File Offset: 0x0036FA08
	// (remove) Token: 0x0600922D RID: 37421 RVA: 0x0037183C File Offset: 0x0036FA3C
	public static event Action<int> TextureQualityChanged;

	// Token: 0x140000F6 RID: 246
	// (add) Token: 0x0600922E RID: 37422 RVA: 0x00371870 File Offset: 0x0036FA70
	// (remove) Token: 0x0600922F RID: 37423 RVA: 0x003718A4 File Offset: 0x0036FAA4
	public static event Action<int> TextureFilterChanged;

	// Token: 0x140000F7 RID: 247
	// (add) Token: 0x06009230 RID: 37424 RVA: 0x003718D8 File Offset: 0x0036FAD8
	// (remove) Token: 0x06009231 RID: 37425 RVA: 0x0037190C File Offset: 0x0036FB0C
	public static event Action<int> ShadowDistanceChanged;

	// Token: 0x140000F8 RID: 248
	// (add) Token: 0x06009232 RID: 37426 RVA: 0x00371940 File Offset: 0x0036FB40
	// (remove) Token: 0x06009233 RID: 37427 RVA: 0x00371974 File Offset: 0x0036FB74
	public static event Action OnGameOptionsApplied;

	// Token: 0x06009234 RID: 37428 RVA: 0x003719A8 File Offset: 0x0036FBA8
	[PublicizedFrom(EAccessModifier.Private)]
	static GameOptionsManager()
	{
		GameOptionsManager.initQualityPresets();
	}

	// Token: 0x06009235 RID: 37429 RVA: 0x00371A83 File Offset: 0x0036FC83
	public static void ValidateGamePrefs()
	{
		GamePrefs.OnGamePrefChanged += GameOptionsManager.OnGamePrefChanged;
		GameOptionsManager.ValidateFoV();
		GameOptionsManager.ValidateFoV3P();
		GameOptionsManager.ValidateTreeDistance();
		GameOptionsManager.ValidateHudSize();
		GameOptionsManager.ValidateShadowDistance();
	}

	// Token: 0x06009236 RID: 37430 RVA: 0x00371AB0 File Offset: 0x0036FCB0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OnGamePrefChanged(EnumGamePrefs _pref)
	{
		if (_pref <= EnumGamePrefs.OptionsHudSize)
		{
			if (_pref <= EnumGamePrefs.OptionsGfxShadowDistance)
			{
				if (_pref == EnumGamePrefs.OptionsOverallAudioVolumeLevel)
				{
					AudioListener.volume = GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel);
					return;
				}
				if (_pref != EnumGamePrefs.OptionsGfxShadowDistance)
				{
					return;
				}
				GameOptionsManager.ValidateShadowDistance();
				return;
			}
			else
			{
				if (_pref == EnumGamePrefs.OptionsGfxFOV)
				{
					GameOptionsManager.ValidateFoV();
					return;
				}
				if (_pref != EnumGamePrefs.OptionsHudSize)
				{
					return;
				}
				GameOptionsManager.ValidateHudSize();
				return;
			}
		}
		else if (_pref <= EnumGamePrefs.OptionsGfxTreeDistance)
		{
			if (_pref == EnumGamePrefs.OptionsGfxWaterPtlLimiter)
			{
				WaterSplashCubes.particleLimiter = GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxWaterPtlLimiter);
				return;
			}
			if (_pref != EnumGamePrefs.OptionsGfxTreeDistance)
			{
				return;
			}
			GameOptionsManager.ValidateTreeDistance();
			return;
		}
		else
		{
			if (_pref == EnumGamePrefs.OptionsMumblePositionalAudioSupport)
			{
				GameOptionsManager.UpdateMumblePositionalAudioState();
				return;
			}
			if (_pref != EnumGamePrefs.OptionsGfxFOV3P)
			{
				return;
			}
			GameOptionsManager.ValidateFoV3P();
			return;
		}
	}

	// Token: 0x06009237 RID: 37431 RVA: 0x00371B4C File Offset: 0x0036FD4C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateFoV()
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
		if (@int < Constants.cMinCameraFieldOfView)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, Constants.cMinCameraFieldOfView);
			return;
		}
		if (@int > Constants.cMaxCameraFieldOfView)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV, Constants.cMaxCameraFieldOfView);
		}
	}

	// Token: 0x06009238 RID: 37432 RVA: 0x00371B8C File Offset: 0x0036FD8C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateFoV3P()
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV3P);
		if (@int < Constants.cMinCameraFieldOfView)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV3P, Constants.cMinCameraFieldOfView);
			return;
		}
		if (@int > Constants.cMaxCameraFieldOfView)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxFOV3P, Constants.cMaxCameraFieldOfView);
		}
	}

	// Token: 0x06009239 RID: 37433 RVA: 0x00371BD3 File Offset: 0x0036FDD3
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateTreeDistance()
	{
		if (GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTreeDistance) < 2)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxTreeDistance, 2);
		}
	}

	// Token: 0x0600923A RID: 37434 RVA: 0x00371BF0 File Offset: 0x0036FDF0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateHudSize()
	{
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsHudSize);
		if ((double)@float < 0.01)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsHudSize, 1f);
			return;
		}
		if ((double)@float < 0.5)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsHudSize, 0.5f);
		}
	}

	// Token: 0x0600923B RID: 37435 RVA: 0x00371C41 File Offset: 0x0036FE41
	[PublicizedFrom(EAccessModifier.Private)]
	public static void UpdateMumblePositionalAudioState()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.OptionsMumblePositionalAudioSupport))
		{
			MumblePositionalAudio.Init();
			return;
		}
		MumblePositionalAudio.Destroy();
	}

	// Token: 0x0600923C RID: 37436 RVA: 0x00371C5C File Offset: 0x0036FE5C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateShadowDistance()
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowDistance);
		if (@int >= 5 && @int < 20)
		{
			GamePrefs.Set(EnumGamePrefs.OptionsGfxShadowDistance, 20);
		}
	}

	// Token: 0x0600923D RID: 37437 RVA: 0x00371C84 File Offset: 0x0036FE84
	public static void ApplyAllOptions(LocalPlayerUI _playerUi)
	{
		QualitySettings.antiAliasing = 0;
		float streamingMipmapBudget = GameOptionsPlatforms.GetStreamingMipmapBudget();
		QualitySettings.streamingMipmapsMemoryBudget = streamingMipmapBudget;
		Log.Out("ApplyAllOptions streaming budget {0} MB", new object[]
		{
			streamingMipmapBudget
		});
		QualitySettings.softParticles = (GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxWaterPtlLimiter) >= 0.51f);
		GameOptionsManager.ApplyScreenResolution();
		AudioListener.volume = Math.Min(GamePrefs.GetFloat(EnumGamePrefs.OptionsOverallAudioVolumeLevel), 1f);
		GameOptionsManager.ApplyShadowQuality();
		GameOptionsManager.ApplyTextureQuality(-1);
		GameOptionsManager.ApplyTextureFilter();
		GameOptionsManager.ApplyCameraOptions(null);
		Shader.globalMaximumLOD = 400 + GamePrefs.GetInt(EnumGamePrefs.OptionsGfxObjQuality) * 100;
		QualitySettings.lodBias = GameOptionsManager.GetLODBias();
		GameOptionsManager.ApplyTerrainOptions();
		MeshDescription.SetGrassQuality();
		MeshDescription.SetWaterQuality();
		Action onGameOptionsApplied = GameOptionsManager.OnGameOptionsApplied;
		if (onGameOptionsApplied == null)
		{
			return;
		}
		onGameOptionsApplied();
	}

	// Token: 0x0600923E RID: 37438 RVA: 0x00371D44 File Offset: 0x0036FF44
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ApplyScreenResolution()
	{
		ValueTuple<int, int, FullScreenMode> screenOptions = PlatformApplicationManager.Application.ScreenOptions;
		int item = screenOptions.Item1;
		int item2 = screenOptions.Item2;
		FullScreenMode item3 = screenOptions.Item3;
		Resolution currentResolution = PlatformApplicationManager.Application.GetCurrentResolution();
		Log.Out("ApplyAllOptions current screen {0} x {1}, {2}hz, window {3} x {4}, mode {5}", new object[]
		{
			currentResolution.width,
			currentResolution.height,
			currentResolution.refreshRateRatio.value,
			Screen.width,
			Screen.height,
			Screen.fullScreenMode
		});
		GameOptionsManager.SetResolution(item, item2, item3);
	}

	// Token: 0x0600923F RID: 37439 RVA: 0x00371DF0 File Offset: 0x0036FFF0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ApplyShadowQuality()
	{
		Vector3 vector = new Vector3(0.06f, 0.15f, 0.35f);
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowDistance);
		switch (@int)
		{
		case 0:
			QualitySettings.shadowDistance = 35f;
			QualitySettings.shadowCascades = 2;
			QualitySettings.shadowCascade2Split = 0.33f;
			goto IL_C8;
		case 1:
			QualitySettings.shadowDistance = 80f;
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowCascade4Split = vector;
			goto IL_C8;
		case 2:
			QualitySettings.shadowDistance = 120f;
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowCascade4Split = vector;
			goto IL_C8;
		case 3:
			QualitySettings.shadowDistance = 200f;
			QualitySettings.shadowCascades = 4;
			QualitySettings.shadowCascade4Split = vector * 0.8f;
			goto IL_C8;
		}
		QualitySettings.shadowDistance = 300f;
		QualitySettings.shadowCascades = 4;
		QualitySettings.shadowCascade4Split = vector * 0.6f;
		IL_C8:
		if (GameOptionsManager.ShadowDistanceChanged != null)
		{
			GameOptionsManager.ShadowDistanceChanged(@int);
		}
		switch (GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowQuality))
		{
		case 0:
			QualitySettings.shadows = ShadowQuality.Disable;
			return;
		case 1:
			QualitySettings.shadows = ShadowQuality.HardOnly;
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			return;
		case 2:
			QualitySettings.shadows = ShadowQuality.All;
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			return;
		case 3:
			QualitySettings.shadows = ShadowQuality.All;
			QualitySettings.shadowResolution = ShadowResolution.High;
			return;
		case 4:
			QualitySettings.shadows = ShadowQuality.All;
			QualitySettings.shadowResolution = ShadowResolution.High;
			return;
		case 5:
			QualitySettings.shadows = ShadowQuality.All;
			QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
			return;
		default:
			return;
		}
	}

	// Token: 0x06009240 RID: 37440 RVA: 0x00371F48 File Offset: 0x00370148
	public static float GetLODBias()
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxObjQuality);
		switch (@int)
		{
		case 0:
			return 0.5f;
		case 1:
			return 0.65f;
		case 2:
			return 0.8f;
		case 3:
			return 1.2f;
		case 4:
			return 1.7f;
		default:
			return (float)@int / 100f;
		}
	}

	// Token: 0x06009241 RID: 37441 RVA: 0x00371FA0 File Offset: 0x003701A0
	public static void CheckResolution()
	{
		if (GameOptionsManager.screenExclusiveCheckDelay > 0 && --GameOptionsManager.screenExclusiveCheckDelay == 0)
		{
			GameOptionsManager.screenExclusiveCheckDelay = 10;
			if (Screen.width != GameOptionsManager.screenWidth || Screen.height != GameOptionsManager.screenHeight)
			{
				Log.Warning("Fullscreen Exclusive failed! Reverting to {0} x {1}", new object[]
				{
					GameOptionsManager.screenWidth,
					GameOptionsManager.screenHeight
				});
				GameOptionsManager.SetResolution(GameOptionsManager.screenWidth, GameOptionsManager.screenHeight, FullScreenMode.Windowed);
			}
		}
	}

	// Token: 0x06009242 RID: 37442 RVA: 0x00372020 File Offset: 0x00370220
	public static void SetResolution(int _width, int _height, FullScreenMode _fullscreen = FullScreenMode.Windowed)
	{
		if (Screen.width == _width && Screen.height == _height && Screen.fullScreenMode == _fullscreen)
		{
			return;
		}
		Resolution currentResolution = PlatformApplicationManager.Application.GetCurrentResolution();
		Log.Out("SetResolution was screen {0} x {1}, {2}hz, window {3} x {4}, mode {5}", new object[]
		{
			currentResolution.width,
			currentResolution.height,
			currentResolution.refreshRateRatio.value,
			Screen.width,
			Screen.height,
			Screen.fullScreenMode
		});
		Log.Out("SetResolution to {0} x {1}, mode {2}", new object[]
		{
			_width,
			_height,
			_fullscreen
		});
		GameOptionsManager.screenWidth = _width;
		GameOptionsManager.screenHeight = _height;
		GameOptionsManager.screenExclusiveCheckDelay = ((_fullscreen == FullScreenMode.ExclusiveFullScreen) ? 10 : 0);
		PlatformApplicationManager.Application.SetResolution(_width, _height, _fullscreen);
		if (GameOptionsManager.ResolutionChanged != null)
		{
			GameOptionsManager.ResolutionChanged(_width, _height);
		}
	}

	// Token: 0x06009243 RID: 37443 RVA: 0x00372124 File Offset: 0x00370324
	public static int GetTextureQuality(int _overrideValue = -1)
	{
		if (_overrideValue != -1)
		{
			return _overrideValue;
		}
		int num = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTexQuality);
		if (Constants.Is32BitOs && num < 2)
		{
			num = 2;
		}
		return Utils.FastMax(num, GameOptionsManager.CalcTextureQualityMin());
	}

	// Token: 0x06009244 RID: 37444 RVA: 0x00372159 File Offset: 0x00370359
	public static int CalcTextureQualityMin()
	{
		return GameOptionsPlatforms.CalcTextureQualityMin();
	}

	// Token: 0x06009245 RID: 37445 RVA: 0x00372160 File Offset: 0x00370360
	public static void ApplyTextureQuality(int _overrideValue = -1)
	{
		int textureQuality = GameOptionsManager.GetTextureQuality(_overrideValue);
		QualitySettings.streamingMipmapsActive = true;
		QualitySettings.streamingMipmapsMaxLevelReduction = Math.Max(3, GameRenderManager.TextureMipmapLimit);
		GameRenderManager.TextureMipmapLimit = textureQuality;
		float value = 0.6776996f;
		if (textureQuality > 0)
		{
			if (textureQuality == 1)
			{
				value = 0.6f;
			}
			else if (textureQuality == 2)
			{
				value = 0.5f;
			}
			else if (textureQuality == 3)
			{
				value = 0.4f;
			}
		}
		Shader.SetGlobalFloat("_MipSlope", value);
		if (GameOptionsManager.TextureQualityChanged != null)
		{
			GameOptionsManager.TextureQualityChanged(textureQuality);
		}
		Log.Out("Texture quality is set to " + GameRenderManager.TextureMipmapLimit.ToString());
	}

	// Token: 0x06009246 RID: 37446 RVA: 0x003721F5 File Offset: 0x003703F5
	public static int GetTextureFilter()
	{
		return GameOptionsPlatforms.ApplyTextureFilterLimit(GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTexFilter));
	}

	// Token: 0x06009247 RID: 37447 RVA: 0x00372208 File Offset: 0x00370408
	public static void ApplyTextureFilter()
	{
		int textureFilter = GameOptionsManager.GetTextureFilter();
		QualitySettings.anisotropicFiltering = ((textureFilter == 0) ? AnisotropicFiltering.Disable : ((textureFilter <= 3) ? AnisotropicFiltering.Enable : AnisotropicFiltering.ForceEnable));
		if (GameOptionsManager.TextureFilterChanged != null)
		{
			GameOptionsManager.TextureFilterChanged(textureFilter);
		}
		Log.Out("ApplyTextureFilter {0}, AF {1}", new object[]
		{
			textureFilter,
			QualitySettings.anisotropicFiltering
		});
	}

	// Token: 0x06009248 RID: 37448 RVA: 0x00372268 File Offset: 0x00370468
	public static void ApplyCameraOptions(EntityPlayerLocal _playerLocal = null)
	{
		bool enabled = false;
		GameRenderManager.ApplyCameraOptions(_playerLocal);
		Camera[] array;
		if (_playerLocal)
		{
			array = _playerLocal.GetComponentsInChildren<Camera>();
		}
		else
		{
			array = Camera.allCameras;
		}
		foreach (Camera camera in array)
		{
			if ((camera.cullingMask & 4096) == 0)
			{
				DepthOfField depthOfField;
				if (camera.TryGetComponent<DepthOfField>(out depthOfField))
				{
					depthOfField.enabled = enabled;
				}
				camera.allowHDR = true;
				int num = GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) ? GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV) : Constants.cDefaultCameraFieldOfView;
				camera.fieldOfView = (float)num;
				camera.renderingPath = RenderingPath.DeferredShading;
				if (QualitySettings.antiAliasing != 0 && camera.actualRenderingPath == RenderingPath.DeferredShading)
				{
					Log.Warning("QualitySettings antialiasing has been enabled but the rendering path is set to deferred. This is incompatible and wastes memory and so will be disabled");
					QualitySettings.antiAliasing = 0;
				}
				camera.farClipPlane = 2000f;
			}
		}
	}

	// Token: 0x06009249 RID: 37449 RVA: 0x00372328 File Offset: 0x00370528
	public static void ApplyTerrainOptions()
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxTerrainQuality);
		if (@int <= 1)
		{
			Shader.EnableKeyword("GAME_TERRAINLOWQ");
		}
		else
		{
			Shader.DisableKeyword("GAME_TERRAINLOWQ");
		}
		if (@int == 0)
		{
			Shader.DisableKeyword("_MAX3LAYER");
			Shader.EnableKeyword("_MAX2LAYER");
		}
		else if (@int <= 1)
		{
			Shader.DisableKeyword("_MAX2LAYER");
			Shader.EnableKeyword("_MAX3LAYER");
		}
		else
		{
			Shader.DisableKeyword("_MAX2LAYER");
			Shader.DisableKeyword("_MAX3LAYER");
		}
		Log.Out("ApplyTerrainOptions {0}", new object[]
		{
			@int
		});
	}

	// Token: 0x0600924A RID: 37450 RVA: 0x003723BC File Offset: 0x003705BC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void initQualityPresets()
	{
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxAA] = new List<object>
		{
			0,
			1,
			2,
			4,
			4,
			null,
			3,
			null,
			3
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxAASharpness] = new List<object>
		{
			0f,
			0f,
			0f,
			0.5f,
			0.6f,
			null,
			0.85f,
			null,
			0.85f
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxMotionBlur] = new List<object>
		{
			0,
			0,
			1,
			1,
			2,
			null,
			2,
			null,
			2
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxTexQuality] = new List<object>
		{
			3,
			2,
			1,
			0,
			0,
			null,
			DeviceFlag.XBoxSeriesS.IsCurrent() ? 1 : 0,
			null,
			DeviceFlag.XBoxSeriesS.IsCurrent() ? 1 : 0
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxTexFilter] = new List<object>
		{
			0,
			0,
			1,
			2,
			3,
			null,
			3,
			null,
			3
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxReflectQuality] = new List<object>
		{
			0,
			0,
			1,
			2,
			3,
			null,
			3,
			null,
			3
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxReflectShadows] = new List<object>
		{
			false,
			false,
			false,
			false,
			true,
			null,
			true,
			null,
			true
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxShadowDistance] = new List<object>
		{
			0,
			0,
			1,
			2,
			3,
			null,
			1,
			null,
			3
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxShadowQuality] = new List<object>
		{
			0,
			1,
			2,
			3,
			4,
			null,
			2,
			null,
			DeviceFlag.XBoxSeriesS.IsCurrent() ? 2 : 3
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxLODDistance] = new List<object>
		{
			0f,
			0.25f,
			0.5f,
			0.75f,
			1f,
			null,
			0.75f,
			null,
			0.75f
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxTerrainQuality] = new List<object>
		{
			0,
			1,
			2,
			3,
			4,
			null,
			2,
			null,
			4
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxObjQuality] = new List<object>
		{
			0,
			1,
			2,
			3,
			4,
			null,
			2,
			null,
			DeviceFlag.XBoxSeriesS.IsCurrent() ? 3 : 4
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxGrassDistance] = new List<object>
		{
			0,
			1,
			2,
			3,
			3,
			null,
			2,
			null,
			3
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxBloom] = new List<object>
		{
			false,
			false,
			true,
			true,
			true,
			null,
			true,
			null,
			true
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxDOF] = new List<object>
		{
			false,
			false,
			false,
			false,
			false,
			null,
			false,
			null,
			false
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxSSAO] = new List<object>
		{
			false,
			false,
			true,
			true,
			true,
			null,
			true,
			null,
			true
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxSSReflections] = new List<object>
		{
			0,
			1,
			1,
			2,
			3,
			null,
			1,
			null,
			1
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxSunShafts] = new List<object>
		{
			false,
			false,
			true,
			true,
			true,
			null,
			true,
			null,
			true
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxWaterQuality] = new List<object>
		{
			0,
			0,
			1,
			1,
			1,
			null,
			1,
			null,
			1
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxWaterPtlLimiter] = new List<object>
		{
			0f,
			0.2f,
			0.5f,
			0.75f,
			1f,
			null,
			0.75f,
			null,
			0.75f
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxSignQuality] = new List<object>
		{
			0,
			1,
			2,
			3,
			4,
			null,
			2,
			null,
			2
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxOcclusion] = new List<object>
		{
			false,
			true,
			true,
			true,
			true,
			null,
			true,
			null,
			true
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxViewDistance] = new List<object>
		{
			5,
			5,
			6,
			6,
			7,
			null,
			6,
			null,
			6
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxDynamicScale] = new List<object>
		{
			null,
			null,
			null,
			null,
			null,
			null,
			DeviceFlag.XBoxSeriesS.IsCurrent() ? 0.6f : 0.5f,
			null,
			0.75f
		};
		GameOptionsManager.QualityPresets[EnumGamePrefs.OptionsGfxFSRPreset] = new List<object>
		{
			null,
			null,
			null,
			null,
			null,
			null,
			0,
			null,
			2
		};
	}

	// Token: 0x0600924B RID: 37451 RVA: 0x00372F90 File Offset: 0x00371190
	public static void SetGraphicsQuality()
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset);
		if (@int < 0)
		{
			Log.Warning(string.Format("SetGraphicsQuality: Selected preset is negative ({0})", @int));
			return;
		}
		foreach (KeyValuePair<EnumGamePrefs, List<object>> keyValuePair in GameOptionsManager.QualityPresets)
		{
			EnumGamePrefs enumGamePrefs;
			List<object> list;
			keyValuePair.Deconstruct(out enumGamePrefs, out list);
			EnumGamePrefs enumGamePrefs2 = enumGamePrefs;
			List<object> list2 = list;
			if (@int >= list2.Count)
			{
				Log.Warning(string.Format("SetGraphicsQuality: Skipping GamePref {0}, selected preset ({1}) outside of defined values ({2})", enumGamePrefs2.ToStringCached<EnumGamePrefs>(), @int, list2.Count));
			}
			else
			{
				object obj = list2[@int];
				if (obj == null)
				{
					Log.Warning(string.Format("SetGraphicsQuality: Skipping GamePref {0}, selected preset ({1}) does not have a value defined for this setting", enumGamePrefs2.ToStringCached<EnumGamePrefs>(), @int));
				}
				else
				{
					GamePrefs.SetObject(enumGamePrefs2, obj);
				}
			}
		}
	}

	// Token: 0x0600924C RID: 37452 RVA: 0x00373078 File Offset: 0x00371278
	public static double GetUiSizeLimit(double _aspectRation)
	{
		int num = 0;
		while (_aspectRation > GameOptionsManager.uiScaleLimits[num].Item1)
		{
			num++;
		}
		return GameOptionsManager.uiScaleLimits[num].Item2;
	}

	// Token: 0x0600924D RID: 37453 RVA: 0x003730B0 File Offset: 0x003712B0
	public static double GetUiSizeLimit()
	{
		Vector2i currentScreenSize = GameOptionsManager.CurrentScreenSize;
		return GameOptionsManager.GetUiSizeLimit((double)currentScreenSize.x / (double)currentScreenSize.y);
	}

	// Token: 0x0600924E RID: 37454 RVA: 0x003730D8 File Offset: 0x003712D8
	public static float GetActiveUiScale()
	{
		float v = (float)GameOptionsManager.GetUiSizeLimit();
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsHudSize);
		return Utils.FastMin(v, @float);
	}

	// Token: 0x17001172 RID: 4466
	// (get) Token: 0x0600924F RID: 37455 RVA: 0x003730FC File Offset: 0x003712FC
	public static Vector2i CurrentScreenSize
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return new Vector2i(Screen.width, Screen.height);
		}
	}

	// Token: 0x04006C03 RID: 27651
	[PublicizedFrom(EAccessModifier.Private)]
	public static int screenWidth;

	// Token: 0x04006C04 RID: 27652
	[PublicizedFrom(EAccessModifier.Private)]
	public static int screenHeight;

	// Token: 0x04006C05 RID: 27653
	[PublicizedFrom(EAccessModifier.Private)]
	public static int screenExclusiveCheckDelay;

	// Token: 0x04006C06 RID: 27654
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cScreenExclusiveFrameWait = 10;

	// Token: 0x04006C08 RID: 27656
	public static readonly EnumDictionary<EnumGamePrefs, List<object>> QualityPresets = new EnumDictionary<EnumGamePrefs, List<object>>();

	// Token: 0x04006C09 RID: 27657
	[TupleElementNames(new string[]
	{
		"aspectLimit",
		"scaleLimit"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ValueTuple<double, double>[] uiScaleLimits = new ValueTuple<double, double>[]
	{
		new ValueTuple<double, double>(1.2, 0.65),
		new ValueTuple<double, double>(1.26, 0.7),
		new ValueTuple<double, double>(1.34, 0.75),
		new ValueTuple<double, double>(1.51, 0.85),
		new ValueTuple<double, double>(1.61, 0.9),
		new ValueTuple<double, double>(1000.0, 1.0)
	};
}
