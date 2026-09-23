using System;
using System.Collections.Generic;
using HorizonBasedAmbientOcclusion;
using PI.NGSS;
using TND.DLSS;
using TND.FSR;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02001340 RID: 4928
public class GameRenderManager
{
	// Token: 0x06009BAA RID: 39850 RVA: 0x003AC7C9 File Offset: 0x003AA9C9
	public static GameRenderManager Create(EntityPlayerLocal player)
	{
		GameRenderManager gameRenderManager = new GameRenderManager();
		gameRenderManager.player = player;
		gameRenderManager.Init();
		return gameRenderManager;
	}

	// Token: 0x06009BAB RID: 39851 RVA: 0x003AC7E0 File Offset: 0x003AA9E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init()
	{
		this.graphManager = GameGraphManager.Create(this.player);
		this.lightManager = GameLightManager.Create(this.player);
		this.reflectionManager = ReflectionManager.Create(this.player);
		this.PostProcessInit();
		this.DynamicResolutionInit();
	}

	// Token: 0x06009BAC RID: 39852 RVA: 0x003AC82C File Offset: 0x003AAA2C
	public void Destroy()
	{
		this.lightManager.Destroy();
		this.lightManager = null;
		this.reflectionManager.Destroy();
		this.reflectionManager = null;
		this.DynamicResolutionDestroyRT();
	}

	// Token: 0x06009BAD RID: 39853 RVA: 0x003AC858 File Offset: 0x003AAA58
	public void FrameUpdate()
	{
		this.lightManager.FrameUpdate();
		this.reflectionManager.FrameUpdate();
		this.DynamicResolutionUpdate();
	}

	// Token: 0x06009BAE RID: 39854 RVA: 0x003AC876 File Offset: 0x003AAA76
	[PublicizedFrom(EAccessModifier.Private)]
	public void PostProcessInit()
	{
		Camera playerCamera = this.player.playerCamera;
	}

	// Token: 0x1700127D RID: 4733
	// (get) Token: 0x06009BAF RID: 39855 RVA: 0x003AC884 File Offset: 0x003AAA84
	// (set) Token: 0x06009BB0 RID: 39856 RVA: 0x003AC88B File Offset: 0x003AAA8B
	public static int TextureMipmapLimit
	{
		get
		{
			return QualitySettings.globalTextureMipmapLimit;
		}
		set
		{
			QualitySettings.globalTextureMipmapLimit = value;
		}
	}

	// Token: 0x06009BB1 RID: 39857 RVA: 0x003AC894 File Offset: 0x003AAA94
	public static void ApplyCameraOptions(EntityPlayerLocal player)
	{
		if (GameManager.Instance.World == null)
		{
			return;
		}
		if (player)
		{
			player.renderManager.ApplyCameraOptions();
			return;
		}
		List<EntityPlayerLocal> localPlayers = GameManager.Instance.World.GetLocalPlayers();
		for (int i = 0; i < localPlayers.Count; i++)
		{
			localPlayers[i].renderManager.ApplyCameraOptions();
		}
	}

	// Token: 0x06009BB2 RID: 39858 RVA: 0x003AC8F4 File Offset: 0x003AAAF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyCameraOptions()
	{
		Camera playerCamera = this.player.playerCamera;
		this.layer = playerCamera.GetComponent<PostProcessLayer>();
		playerCamera.depthTextureMode = (DepthTextureMode.Depth | DepthTextureMode.DepthNormals | DepthTextureMode.MotionVectors);
		NGSS_FrustumShadows_7DTD component = playerCamera.GetComponent<NGSS_FrustumShadows_7DTD>();
		switch (GamePrefs.GetInt(EnumGamePrefs.OptionsGfxShadowQuality))
		{
		case 0:
		case 1:
		case 2:
			component.enabled = false;
			break;
		case 3:
			component.enabled = true;
			component.m_shadowsBlurIterations = 1;
			component.m_raySamples = 32;
			break;
		case 4:
			component.enabled = true;
			component.m_shadowsBlurIterations = 2;
			component.m_raySamples = 48;
			break;
		case 5:
			component.enabled = true;
			component.m_shadowsBlurIterations = 4;
			component.m_raySamples = 64;
			break;
		}
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxAA);
		float @float = GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxAASharpness);
		bool @bool = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxBloom);
		int int2 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxSSReflections);
		bool bool2 = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxSSAO);
		bool bool3 = GamePrefs.GetBool(EnumGamePrefs.OptionsGfxSunShafts);
		int num = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxMotionBlur);
		if (!GamePrefs.GetBool(EnumGamePrefs.OptionsGfxMotionBlurEnabled))
		{
			num = 0;
		}
		PostProcessVolume component2 = playerCamera.GetComponent<PostProcessVolume>();
		if (component2)
		{
			PostProcessProfile profile = component2.profile;
			if (profile)
			{
				component2.enabled = false;
				ScreenSpaceReflections setting = profile.GetSetting<ScreenSpaceReflections>();
				if (setting)
				{
					switch (int2)
					{
					case 1:
						setting.maximumIterationCount.Override(200);
						setting.resolution.Override(ScreenSpaceReflectionResolution.Downsampled);
						break;
					case 2:
						setting.maximumIterationCount.Override(120);
						setting.resolution.Override(ScreenSpaceReflectionResolution.FullSize);
						break;
					case 3:
						setting.maximumIterationCount.Override(250);
						setting.resolution.Override(ScreenSpaceReflectionResolution.FullSize);
						break;
					}
					setting.enabled.Override(int2 > 0);
				}
				MotionBlur setting2 = profile.GetSetting<MotionBlur>();
				setting2.enabled.Override(num != 0);
				if (num != 1)
				{
					if (num == 2)
					{
						setting2.shutterAngle.Override(270f);
						setting2.sampleCount.Override(10);
					}
				}
				else
				{
					setting2.shutterAngle.Override(135f);
					setting2.sampleCount.Override(5);
				}
				profile.GetSetting<Bloom>().enabled.Override(@bool);
				ColorGrading setting3 = profile.GetSetting<ColorGrading>();
				float num2 = 0.5f - GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxBrightness);
				if (num2 < 0f)
				{
					num2 *= 0.4f;
				}
				else
				{
					num2 = 0f;
				}
				setting3.toneCurveGamma.Override(1f + num2);
				SunShaftsEffect sunShaftsEffect;
				if (profile.TryGetSettings<SunShaftsEffect>(out sunShaftsEffect))
				{
					sunShaftsEffect.enabled.Override(bool3);
				}
				component2.enabled = true;
			}
		}
		HBAO component3 = playerCamera.GetComponent<HBAO>();
		if (component3)
		{
			GameOptionsPlatforms.GfxPreset int3 = (GameOptionsPlatforms.GfxPreset)GamePrefs.GetInt(EnumGamePrefs.OptionsGfxQualityPreset);
			switch (int3)
			{
			case GameOptionsPlatforms.GfxPreset.Lowest:
			case GameOptionsPlatforms.GfxPreset.Low:
			case GameOptionsPlatforms.GfxPreset.Medium:
				component3.SetQuality(HBAO.Quality.Low);
				goto IL_31B;
			case GameOptionsPlatforms.GfxPreset.High:
				component3.SetQuality(HBAO.Quality.Medium);
				goto IL_31B;
			case GameOptionsPlatforms.GfxPreset.Ultra:
				component3.SetQuality(HBAO.Quality.High);
				goto IL_31B;
			case GameOptionsPlatforms.GfxPreset.Custom:
			case GameOptionsPlatforms.GfxPreset.ConsolePerformance:
			case GameOptionsPlatforms.GfxPreset.LEGACY_ConsolePerformanceFSR:
			case GameOptionsPlatforms.GfxPreset.ConsoleQuality:
				break;
			default:
				if (int3 != GameOptionsPlatforms.GfxPreset.Simplified)
				{
				}
				break;
			}
			component3.SetQuality(HBAO.Quality.Medium);
			IL_31B:
			component3.enabled = bool2;
		}
		int num3 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxUpscalerMode);
		if (num3 == 2 && !FSR3.FSR3Supported())
		{
			num3 = 4;
			GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, num3);
		}
		if (num3 == 5 && (!FSR3.FSR3Supported() || !DLSS.DLSSSupported()))
		{
			num3 = 4;
			GamePrefs.Set(EnumGamePrefs.OptionsGfxUpscalerMode, num3);
		}
		int int4 = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxDynamicMinFPS);
		float num4;
		if (num3 != 3)
		{
			if (num3 != 4)
			{
				num4 = -1f;
			}
			else
			{
				num4 = GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxDynamicScale);
			}
		}
		else
		{
			num4 = 0f;
		}
		float scale = num4;
		this.SetDynamicResolution(scale, (float)int4, -1f);
		if (this.layer)
		{
			if (num3 == 5 || num3 == 2)
			{
				PostProcessLayer postProcessLayer = this.layer;
				PostProcessLayer.Antialiasing antialiasingMode;
				if (num3 == 5)
				{
					antialiasingMode = PostProcessLayer.Antialiasing.DLSS;
				}
				else
				{
					antialiasingMode = PostProcessLayer.Antialiasing.FSR3;
				}
				postProcessLayer.antialiasingMode = antialiasingMode;
				this.layer.fsr3.sharpness = @float;
				this.layer.dlss.sharpness = @float;
				this.UpscalingSetQuality(GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFSRPreset));
			}
			else
			{
				this.SetAntialiasing(@int, @float, this.layer);
			}
			Rect rect = playerCamera.rect;
			rect.x = ((this.layer.antialiasingMode == PostProcessLayer.Antialiasing.DLSS || this.layer.antialiasingMode == PostProcessLayer.Antialiasing.FSR3) ? 1E-07f : 0f);
			playerCamera.rect = rect;
		}
		this.reflectionManager.ApplyCameraOptions(playerCamera);
	}

	// Token: 0x06009BB3 RID: 39859 RVA: 0x003ACD7C File Offset: 0x003AAF7C
	public void SetAntialiasing(int aaQuality, float sharpness, PostProcessLayer mainLayer)
	{
		if (aaQuality == 0)
		{
			mainLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
			this.UpscalingSetQuality(-1);
			return;
		}
		if (aaQuality <= 3)
		{
			if (aaQuality == 1)
			{
				mainLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
				mainLayer.fastApproximateAntialiasing.fastMode = false;
			}
			else if (aaQuality == 2)
			{
				mainLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
				mainLayer.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Medium;
			}
			else
			{
				mainLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
				mainLayer.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.High;
			}
		}
		else if (aaQuality == 4)
		{
			mainLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
			mainLayer.temporalAntialiasing.jitterSpread = 0.35f;
			mainLayer.temporalAntialiasing.stationaryBlending = 0.8f;
			mainLayer.temporalAntialiasing.motionBlending = 0.75f;
			mainLayer.temporalAntialiasing.sharpness = sharpness * 0.2f;
		}
		else
		{
			Log.Error(string.Format("Unsupported aaQuality value \"{0}\".", aaQuality));
		}
		this.UpscalingSetQuality(-1);
	}

	// Token: 0x06009BB4 RID: 39860 RVA: 0x003ACE54 File Offset: 0x003AB054
	public void DynamicResolutionInit()
	{
		this.DynamicResolutionDestroyRT();
		Camera playerCamera = this.player.playerCamera;
		Camera finalCamera = this.player.finalCamera;
		bool flag = finalCamera != playerCamera;
		if (!GameRenderManager.dynamicIsEnabled)
		{
			if (flag)
			{
				UnityEngine.Object.Destroy(finalCamera.gameObject);
			}
			this.player.finalCamera = playerCamera;
			return;
		}
		if (!flag)
		{
			this.AddFinalCameraToPlayer();
		}
		this.DynamicResolutionAllocRTs();
	}

	// Token: 0x06009BB5 RID: 39861 RVA: 0x003ACEB8 File Offset: 0x003AB0B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddFinalCameraToPlayer()
	{
		GameObject gameObject = new GameObject("FinalCamera");
		gameObject.transform.SetParent(this.player.cameraTransform, false);
		Camera camera = gameObject.AddComponent<Camera>();
		this.player.finalCamera = camera;
		camera.clearFlags = CameraClearFlags.Nothing;
		camera.cullingMask = 0;
		camera.depth = -0.1f;
		gameObject.AddComponent<LocalPlayerFinalCamera>().entityPlayerLocal = this.player;
	}

	// Token: 0x06009BB6 RID: 39862 RVA: 0x003ACF24 File Offset: 0x003AB124
	public void DynamicResolutionUpdate()
	{
		if (GameManager.Instance.World == null)
		{
			return;
		}
		if (!GameRenderManager.dynamicIsEnabled)
		{
			return;
		}
		if (Screen.width != this.dynamicScreenW)
		{
			this.DynamicResolutionAllocRTs();
			return;
		}
		if (this.dynamicScaleOverride > 0f)
		{
			return;
		}
		if (this.dynamicUpdateDelay > 0f)
		{
			this.dynamicUpdateDelay -= Time.deltaTime;
			return;
		}
		float num = Time.deltaTime + 0.001f;
		this.dynamicFPS = this.dynamicFPS * 0.5f + 0.5f / num;
		float num2 = 0.1f * num;
		if (this.dynamicFPS < this.dynamicFPSTargetMin)
		{
			this.dynamicScaleTarget -= num2;
			if (this.dynamicScaleTarget < 0.4f)
			{
				this.dynamicScaleTarget = 0.4f;
			}
		}
		else
		{
			this.dynamicScaleTarget += num2 * 0.2f;
			if (this.dynamicFPS > this.dynamicFPSTargetMax)
			{
				this.dynamicScaleTarget += num2;
			}
			if (this.dynamicScaleTarget > 1f)
			{
				this.dynamicScaleTarget = 1f;
			}
		}
		if (this.dynamicScaleTarget < 1f || this.dynamicScale >= 1f)
		{
			float num3 = this.dynamicScaleTarget - this.dynamicScale;
			if (num3 > -0.049f && num3 < 0.049f)
			{
				return;
			}
		}
		this.dynamicScale = this.dynamicScaleTarget;
		RenderTexture y = null;
		for (int i = 0; i < this.dynamicRTs.Length; i++)
		{
			y = this.dynamicRTs[i];
			float num4 = (this.dynamicScales[i] + this.dynamicScales[i + 1]) * 0.5f;
			if (this.dynamicScale >= num4)
			{
				break;
			}
		}
		if (this.dynamicRT == y)
		{
			return;
		}
		this.dynamicRT = y;
	}

	// Token: 0x06009BB7 RID: 39863 RVA: 0x003AD0DC File Offset: 0x003AB2DC
	public bool DynamicResolutionUpdateGraph(ref float value)
	{
		if (this.dynamicRT != null)
		{
			float num = (float)this.dynamicRT.width / (float)Screen.width;
			if (num != value)
			{
				value = num;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06009BB8 RID: 39864 RVA: 0x003AD118 File Offset: 0x003AB318
	[PublicizedFrom(EAccessModifier.Private)]
	public void DynamicResolutionAllocRTs()
	{
		this.DynamicResolutionDestroyRT();
		this.dynamicScreenW = Screen.width;
		int num = (this.dynamicScaleOverride > 0f) ? 1 : 4;
		this.dynamicRTs = new RenderTexture[num];
		for (int i = 0; i < num; i++)
		{
			float scale = this.dynamicScales[i];
			if (this.dynamicScaleOverride > 0f)
			{
				scale = this.dynamicScaleOverride;
			}
			RenderTexture renderTexture = this.DynamicResolutionAllocRT(scale);
			this.dynamicRTs[i] = renderTexture;
		}
		this.dynamicRT = this.dynamicRTs[0];
		this.dynamicScale = 1f;
		this.dynamicScaleTarget = 1f;
		this.dynamicUpdateDelay = 18f;
	}

	// Token: 0x06009BB9 RID: 39865 RVA: 0x003AD1C0 File Offset: 0x003AB3C0
	public RenderTexture DynamicResolutionAllocRT(float scale)
	{
		int num = (int)((float)Screen.width * scale);
		int num2 = (int)((float)Screen.height * scale);
		RenderTexture renderTexture = new RenderTexture(num, num2, 24, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
		renderTexture.name = string.Format("DynRT{0}x{1}", num, num2);
		Log.Out("DynamicResolutionAllocRT scale {0}, Tex {1}x{2}", new object[]
		{
			scale,
			num,
			num2
		});
		return renderTexture;
	}

	// Token: 0x06009BBA RID: 39866 RVA: 0x003AD238 File Offset: 0x003AB438
	[PublicizedFrom(EAccessModifier.Private)]
	public void DynamicResolutionDestroyRT()
	{
		List<EntityPlayerLocal> localPlayers = GameManager.Instance.World.GetLocalPlayers();
		for (int i = 0; i < localPlayers.Count; i++)
		{
			localPlayers[i].playerCamera.targetTexture = null;
		}
		if (this.dynamicRTs != null)
		{
			for (int j = 0; j < this.dynamicRTs.Length; j++)
			{
				this.dynamicRTs[j].Release();
				UnityEngine.Object.Destroy(this.dynamicRTs[j]);
			}
			this.dynamicRTs = null;
		}
		this.dynamicRT = null;
	}

	// Token: 0x06009BBB RID: 39867 RVA: 0x003AD2BC File Offset: 0x003AB4BC
	public void SetDynamicResolution(float scale, float fpsMin, float fpsMax)
	{
		GameRenderManager.dynamicIsEnabled = (scale >= 0f);
		int @int = GamePrefs.GetInt(EnumGamePrefs.OptionsGfxVsync);
		this.dynamicFPSTargetMin = fpsMin;
		if (fpsMin < 0f)
		{
			this.dynamicFPSTargetMin = 30f;
		}
		if (@int > 0)
		{
			this.dynamicFPSTargetMin = Utils.FastMin(30f, this.dynamicFPSTargetMin);
		}
		if (@int > 1)
		{
			this.dynamicFPSTargetMin = Utils.FastMin(18f, this.dynamicFPSTargetMin);
		}
		this.dynamicFPSTargetMax = fpsMax;
		if (fpsMax < 0f)
		{
			this.dynamicFPSTargetMax = 64f;
			if (@int > 0)
			{
				this.dynamicFPSTargetMax = 55f;
			}
			if (@int > 1)
			{
				this.dynamicFPSTargetMax = 25f;
			}
		}
		this.dynamicScaleOverride = scale;
		if (this.dynamicScaleOverride > 0f)
		{
			this.dynamicScaleOverride = Mathf.Clamp(this.dynamicScaleOverride, 0.1f, 1f);
			this.dynamicScale = this.dynamicScaleOverride;
		}
		this.DynamicResolutionInit();
	}

	// Token: 0x06009BBC RID: 39868 RVA: 0x003AD3A8 File Offset: 0x003AB5A8
	public RenderTexture GetDynamicRenderTexture()
	{
		return this.dynamicRT;
	}

	// Token: 0x06009BBD RID: 39869 RVA: 0x003AD3B0 File Offset: 0x003AB5B0
	public void DynamicResolutionRender()
	{
		Graphics.Blit(this.GetDynamicRenderTexture(), null);
	}

	// Token: 0x06009BBE RID: 39870 RVA: 0x003AD3C0 File Offset: 0x003AB5C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpscalingSetQuality(int _quality)
	{
		if (_quality < 0)
		{
			if (this.upscalingEnabled)
			{
				this.upscalingEnabled = false;
				this.SetMipmapBias(0f);
			}
			return;
		}
		this.upscalingEnabled = true;
		this.mipmapTextureMem = 0UL;
		FSR3 fsr = this.layer.fsr3;
		FSR3_Quality qualityMode;
		switch (_quality)
		{
		case 0:
			qualityMode = FSR3_Quality.UltraPerformance;
			break;
		case 1:
			qualityMode = FSR3_Quality.Performance;
			break;
		case 2:
			qualityMode = FSR3_Quality.Balanced;
			break;
		case 3:
			qualityMode = FSR3_Quality.Quality;
			break;
		case 4:
			qualityMode = FSR3_Quality.UltraQuality;
			break;
		default:
			qualityMode = FSR3_Quality.NativeAA;
			break;
		}
		fsr.qualityMode = qualityMode;
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceVendor.ToLower().Contains("nvidia"))
		{
			this.layer.fsr3.exposureSource = FSR3.ExposureSource.Default;
		}
		DLSS dlss = this.layer.dlss;
		DLSS_Quality qualityMode2;
		switch (_quality)
		{
		case 0:
			qualityMode2 = DLSS_Quality.UltraPerformance;
			break;
		case 1:
			qualityMode2 = DLSS_Quality.Performance;
			break;
		case 2:
			qualityMode2 = DLSS_Quality.Balanced;
			break;
		case 3:
			qualityMode2 = DLSS_Quality.Quality;
			break;
		case 4:
			qualityMode2 = DLSS_Quality.UltraQuality;
			break;
		default:
			qualityMode2 = DLSS_Quality.NativeAA;
			break;
		}
		dlss.qualityMode = qualityMode2;
	}

	// Token: 0x06009BBF RID: 39871 RVA: 0x003AD4B8 File Offset: 0x003AB6B8
	public void UpscalingPreCull()
	{
		if (!this.upscalingEnabled)
		{
			return;
		}
		switch (this.layer.antialiasingMode)
		{
		case PostProcessLayer.Antialiasing.FSR1:
			this.UpdateMipmaps((float)this.layer.fsr1.renderSize.x / (float)Screen.width, 1f);
			return;
		case PostProcessLayer.Antialiasing.FSR3:
			this.UpdateMipmaps((float)this.layer.fsr3.renderSize.x / (float)Screen.width, 0.3f);
			return;
		case PostProcessLayer.Antialiasing.DLSS:
			this.UpdateMipmaps((float)this.layer.dlss.renderSize.x / (float)Screen.width, 1f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06009BC0 RID: 39872 RVA: 0x003AD56C File Offset: 0x003AB76C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateMipmaps(float _renderToScreenRatio, float biasStrength = 1f)
	{
		this.mipmapDelay -= Time.deltaTime;
		if (this.mipmapDelay <= 0f)
		{
			this.mipmapDelay = 2f;
			ulong currentTextureMemory = Texture.currentTextureMemory;
			if (this.mipmapTextureMem != currentTextureMemory)
			{
				this.mipmapTextureMem = currentTextureMemory;
				float mipmapBias = biasStrength * (Mathf.Log(_renderToScreenRatio, 2f) - 1f);
				this.SetMipmapBias(mipmapBias);
			}
		}
	}

	// Token: 0x06009BC1 RID: 39873 RVA: 0x003AD5D8 File Offset: 0x003AB7D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMipmapBias(float _bias)
	{
		Texture2D[] array = Resources.FindObjectsOfTypeAll(typeof(Texture2D)) as Texture2D[];
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mipMapBias = _bias;
		}
		Texture2DArray[] array2 = Resources.FindObjectsOfTypeAll(typeof(Texture2DArray)) as Texture2DArray[];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j].mipMapBias = _bias;
		}
	}

	// Token: 0x06009BC2 RID: 39874 RVA: 0x003AD63D File Offset: 0x003AB83D
	public void OnGUI()
	{
		this.graphManager.Draw();
	}

	// Token: 0x06009BC3 RID: 39875 RVA: 0x003AD64A File Offset: 0x003AB84A
	public bool FPSUpdateGraph(ref float value)
	{
		value = 1f / (Time.deltaTime + 0.001f);
		return true;
	}

	// Token: 0x06009BC4 RID: 39876 RVA: 0x003AD660 File Offset: 0x003AB860
	public bool SPFUpdateGraph(ref float value)
	{
		value = (Time.deltaTime + 0.0001f) * 1000f;
		return true;
	}

	// Token: 0x04007561 RID: 30049
	public GameGraphManager graphManager;

	// Token: 0x04007562 RID: 30050
	public GameLightManager lightManager;

	// Token: 0x04007563 RID: 30051
	public ReflectionManager reflectionManager;

	// Token: 0x04007564 RID: 30052
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x04007565 RID: 30053
	[PublicizedFrom(EAccessModifier.Private)]
	public PostProcessLayer layer;

	// Token: 0x04007566 RID: 30054
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDynamicUpdateDelay = 18f;

	// Token: 0x04007567 RID: 30055
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDynamicChangeSeconds = 5f;

	// Token: 0x04007568 RID: 30056
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDynamicFPSMin = 30f;

	// Token: 0x04007569 RID: 30057
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDynamicFPSMax = 64f;

	// Token: 0x0400756A RID: 30058
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDynamicFPSVSyncMin = 30f;

	// Token: 0x0400756B RID: 30059
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDynamicFPSVSyncMax = 55f;

	// Token: 0x0400756C RID: 30060
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDynamicScaleMin = 0.4f;

	// Token: 0x0400756D RID: 30061
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDynamicScaleThreshold = 0.049f;

	// Token: 0x0400756E RID: 30062
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cDynamicRTCount = 4;

	// Token: 0x0400756F RID: 30063
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly float[] dynamicScales = new float[]
	{
		1f,
		0.75f,
		0.62f,
		0.5f,
		0f
	};

	// Token: 0x04007570 RID: 30064
	public static bool dynamicIsEnabled;

	// Token: 0x04007571 RID: 30065
	[PublicizedFrom(EAccessModifier.Private)]
	public float dynamicUpdateDelay;

	// Token: 0x04007572 RID: 30066
	[PublicizedFrom(EAccessModifier.Private)]
	public float dynamicFPSTargetMin;

	// Token: 0x04007573 RID: 30067
	[PublicizedFrom(EAccessModifier.Private)]
	public float dynamicFPSTargetMax = 64f;

	// Token: 0x04007574 RID: 30068
	[PublicizedFrom(EAccessModifier.Private)]
	public float dynamicFPS = 60f;

	// Token: 0x04007575 RID: 30069
	[PublicizedFrom(EAccessModifier.Private)]
	public float dynamicScale;

	// Token: 0x04007576 RID: 30070
	[PublicizedFrom(EAccessModifier.Private)]
	public float dynamicScaleTarget;

	// Token: 0x04007577 RID: 30071
	[PublicizedFrom(EAccessModifier.Private)]
	public float dynamicScaleOverride;

	// Token: 0x04007578 RID: 30072
	[PublicizedFrom(EAccessModifier.Private)]
	public int dynamicScreenW;

	// Token: 0x04007579 RID: 30073
	[PublicizedFrom(EAccessModifier.Private)]
	public RenderTexture dynamicRT;

	// Token: 0x0400757A RID: 30074
	[PublicizedFrom(EAccessModifier.Private)]
	public RenderTexture[] dynamicRTs;

	// Token: 0x0400757B RID: 30075
	[PublicizedFrom(EAccessModifier.Private)]
	public bool upscalingEnabled;

	// Token: 0x0400757C RID: 30076
	[PublicizedFrom(EAccessModifier.Private)]
	public float mipmapDelay;

	// Token: 0x0400757D RID: 30077
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong mipmapTextureMem;
}
