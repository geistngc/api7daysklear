using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PI.NGSS
{
	// Token: 0x02001B2A RID: 6954
	[RequireComponent(typeof(Light))]
	[ExecuteInEditMode]
	public class NGSS_Directional : MonoBehaviour
	{
		// Token: 0x17001998 RID: 6552
		// (get) Token: 0x0600D0AE RID: 53422 RVA: 0x004C4661 File Offset: 0x004C2861
		public Light DirLight
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				if (this._DirLight == null)
				{
					this._DirLight = base.GetComponent<Light>();
				}
				return this._DirLight;
			}
		}

		// Token: 0x17001999 RID: 6553
		// (get) Token: 0x0600D0B0 RID: 53424 RVA: 0x004C468C File Offset: 0x004C288C
		// (set) Token: 0x0600D0AF RID: 53423 RVA: 0x004C4683 File Offset: 0x004C2883
		public Material mMaterial
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				if (this._mMaterial)
				{
					return this._mMaterial;
				}
				if (this.denoiserShader == null)
				{
					this.denoiserShader = Shader.Find("Hidden/NGSS_DenoiseShader");
				}
				this._mMaterial = new Material(this.denoiserShader);
				if (this._mMaterial)
				{
					return this._mMaterial;
				}
				Debug.LogWarning("NGSS Warning: can't find NGSS_DenoiseShader shader, make sure it's on your project.", this);
				base.enabled = false;
				return this._mMaterial;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this._mMaterial = value;
			}
		}

		// Token: 0x0600D0B1 RID: 53425 RVA: 0x004C4708 File Offset: 0x004C2908
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			this.isInitialized = false;
			if (this.NGSS_KEEP_ONDISABLE)
			{
				return;
			}
			if (this.isGraphicSet)
			{
				this.isGraphicSet = false;
				GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, Shader.Find("Hidden/Internal-ScreenSpaceShadows"));
				GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseBuiltin);
			}
			if (this.mMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.mMaterial);
				this.mMaterial = null;
			}
			this.RemoveCommandBuffer();
		}

		// Token: 0x0600D0B2 RID: 53426 RVA: 0x004C4770 File Offset: 0x004C2970
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveCommandBuffer()
		{
			if (this._computeDenoiseCB == null || this._DirLight == null)
			{
				return;
			}
			foreach (CommandBuffer commandBuffer in this._DirLight.GetCommandBuffers(LightEvent.AfterScreenspaceMask))
			{
				if (!(commandBuffer.name != this._computeDenoiseCB.name))
				{
					this._DirLight.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, commandBuffer);
					break;
				}
			}
			this._computeDenoiseCB = null;
			foreach (CommandBuffer commandBuffer2 in this._DirLight.GetCommandBuffers(LightEvent.AfterScreenspaceMask))
			{
				if (!(commandBuffer2.name != this._blendDenoiseCB.name))
				{
					this._DirLight.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, commandBuffer2);
					break;
				}
			}
			this._blendDenoiseCB = null;
		}

		// Token: 0x0600D0B3 RID: 53427 RVA: 0x004C482E File Offset: 0x004C2A2E
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnEnable()
		{
			if (this.IsNotSupported())
			{
				Debug.LogWarning("Unsupported graphics API, NGSS requires at least SM3.0 or higher and DX9 is not supported.", this);
				base.enabled = false;
				return;
			}
			this.Init();
		}

		// Token: 0x0600D0B4 RID: 53428 RVA: 0x004C4854 File Offset: 0x004C2A54
		[PublicizedFrom(EAccessModifier.Private)]
		public void Init()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (!this.isGraphicSet)
			{
				GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseCustom);
				GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, Shader.Find("Hidden/NGSS_Directional"));
				this.isGraphicSet = true;
			}
			if (this.NGSS_NOISE_TEXTURE == null)
			{
				this.NGSS_NOISE_TEXTURE = Resources.Load<Texture2D>("BlueNoise_R8_8");
			}
			Shader.SetGlobalTexture("_BlueNoiseTextureDir", this.NGSS_NOISE_TEXTURE);
			bool flag = false;
			if (this.NGSS_DENOISER_ENABLED && !flag)
			{
				this.AddCommandBuffer();
			}
			this.isInitialized = true;
		}

		// Token: 0x0600D0B5 RID: 53429 RVA: 0x004C48D8 File Offset: 0x004C2AD8
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddCommandBuffer()
		{
			if (this._DirLight == null)
			{
				return;
			}
			this._computeDenoiseCB = new CommandBuffer();
			this._blendDenoiseCB = new CommandBuffer();
			this._computeDenoiseCB.name = "NGSS_Directional Denoiser Computation";
			this._blendDenoiseCB.name = "NGSS_Directional Denoiser Blending";
			int nameID = Shader.PropertyToID("NGSS_DenoiseTexture1");
			int nameID2 = Shader.PropertyToID("NGSS_DenoiseTexture2");
			this._blendDenoiseCB.Blit(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CurrentActive, this.mMaterial, 1);
			this._computeDenoiseCB.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
			this._computeDenoiseCB.GetTemporaryRT(nameID2, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
			for (int i = 1; i <= this.NGSS_DENOISER_PASSES; i++)
			{
				this._computeDenoiseCB.SetGlobalVector(this._denoiseKernelID, new Vector2(0f, 1f));
				if (i == 1)
				{
					this._computeDenoiseCB.Blit(BuiltinRenderTextureType.CurrentActive, nameID2, this.mMaterial, 0);
				}
				else
				{
					this._computeDenoiseCB.Blit(nameID, nameID2, this.mMaterial, 0);
				}
				this._computeDenoiseCB.SetGlobalVector(this._denoiseKernelID, new Vector2(1f, 0f));
				this._computeDenoiseCB.Blit(nameID2, nameID, this.mMaterial, 0);
			}
			this._computeDenoiseCB.SetGlobalTexture(this._denoiseTextureID, nameID);
			this._computeDenoiseCB.ReleaseTemporaryRT(nameID);
			this._computeDenoiseCB.ReleaseTemporaryRT(nameID2);
			this.mMaterial.SetFloat(this._denoiseEdgeToleranceID, this.NGSS_DENOISER_EDGE_SOFTNESS);
			this.mMaterial.SetFloat(this._denoiseSoftnessID, this.NGSS_DENOISER_SOFTNESS);
			bool flag = true;
			CommandBuffer[] commandBuffers = this._DirLight.GetCommandBuffers(LightEvent.AfterScreenspaceMask);
			for (int j = 0; j < commandBuffers.Length; j++)
			{
				if (!(commandBuffers[j].name != this._computeDenoiseCB.name))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this._DirLight.AddCommandBuffer(LightEvent.AfterScreenspaceMask, this._computeDenoiseCB);
			}
			flag = true;
			commandBuffers = this._DirLight.GetCommandBuffers(LightEvent.AfterScreenspaceMask);
			for (int j = 0; j < commandBuffers.Length; j++)
			{
				if (!(commandBuffers[j].name != this._blendDenoiseCB.name))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this._DirLight.AddCommandBuffer(LightEvent.AfterScreenspaceMask, this._blendDenoiseCB);
			}
		}

		// Token: 0x0600D0B6 RID: 53430 RVA: 0x004C4B52 File Offset: 0x004C2D52
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsNotSupported()
		{
			return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2;
		}

		// Token: 0x0600D0B7 RID: 53431 RVA: 0x004C4B5C File Offset: 0x004C2D5C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			if (Application.isPlaying && this.NGSS_NO_UPDATE_ON_PLAY && this.isSetup)
			{
				return;
			}
			if (this.DirLight.shadows == LightShadows.None || this.DirLight.type != LightType.Directional)
			{
				return;
			}
			Shader.SetGlobalFloat(this._ngssDirSamplingDistanceid, this.NGSS_SAMPLING_DISTANCE);
			this.NGSS_SAMPLING_TEST = Mathf.Clamp(this.NGSS_SAMPLING_TEST, 4, this.NGSS_SAMPLING_FILTER);
			Shader.SetGlobalFloat(this._ngssTestSamplersDirid, (float)this.NGSS_SAMPLING_TEST);
			Shader.SetGlobalFloat(this._ngssFilterSamplersDirid, (float)this.NGSS_SAMPLING_FILTER);
			Shader.SetGlobalFloat(this._ngssGlobalSoftnessid, (QualitySettings.shadowProjection == ShadowProjection.CloseFit) ? this.NGSS_SHADOWS_SOFTNESS : (this.NGSS_SHADOWS_SOFTNESS * 2f / (QualitySettings.shadowDistance * 0.66f) * ((QualitySettings.shadowCascades == 2) ? 1.5f : ((QualitySettings.shadowCascades == 4) ? 1f : 0.25f))));
			Shader.SetGlobalFloat(this._ngssGlobalSoftnessOptimizedid, this.NGSS_SHADOWS_SOFTNESS / QualitySettings.shadowDistance);
			int num = (int)Mathf.Sqrt((float)this.NGSS_SAMPLING_FILTER);
			Shader.SetGlobalInt(this._ngssOptimizedIterationsid, (num % 2 == 0) ? (num + 1) : num);
			Shader.SetGlobalInt(this._ngssOptimizedSamplersid, this.NGSS_SAMPLING_FILTER);
			if (this._DENOISER_ENABLED != this.NGSS_DENOISER_ENABLED)
			{
				this._DENOISER_ENABLED = this.NGSS_DENOISER_ENABLED;
				this.RemoveCommandBuffer();
				if (this.NGSS_DENOISER_ENABLED)
				{
					this.AddCommandBuffer();
				}
			}
			if (this._DENOISER_PASSES != this.NGSS_DENOISER_PASSES)
			{
				this._DENOISER_PASSES = this.NGSS_DENOISER_PASSES;
				this.RemoveCommandBuffer();
				if (this.NGSS_DENOISER_ENABLED)
				{
					this.AddCommandBuffer();
				}
			}
			if (this.NGSS_DENOISER_ENABLED)
			{
				this.mMaterial.SetFloat(this._denoiseEdgeToleranceID, this.NGSS_DENOISER_EDGE_SOFTNESS);
				this.mMaterial.SetFloat(this._denoiseSoftnessID, this.NGSS_DENOISER_SOFTNESS);
			}
			if (this.NGSS_RECEIVER_PLANE_BIAS)
			{
				Shader.EnableKeyword("NGSS_USE_RECEIVER_PLANE_BIAS");
			}
			else
			{
				Shader.DisableKeyword("NGSS_USE_RECEIVER_PLANE_BIAS");
			}
			Shader.SetGlobalFloat(this._ngssNoiseToDitheringScaleDirid, (float)this.NGSS_NOISE_TO_DITHERING_SCALE);
			if (this.NGSS_PCSS_ENABLED)
			{
				float num2 = this.NGSS_PCSS_SOFTNESS_NEAR * 0.25f;
				float num3 = this.NGSS_PCSS_SOFTNESS_FAR * 0.25f;
				Shader.SetGlobalFloat(this._ngssPcssFilterDirMinid, (num2 > num3) ? num3 : num2);
				Shader.SetGlobalFloat(this._ngssPcssFilterDirMaxid, (num3 < num2) ? num2 : num3);
				Shader.EnableKeyword("NGSS_PCSS_FILTER_DIR");
			}
			else
			{
				Shader.DisableKeyword("NGSS_PCSS_FILTER_DIR");
			}
			if (this.NGSS_SHADOWS_RESOLUTION != NGSS_Directional.ShadowMapResolution.UseQualitySettings)
			{
				this.DirLight.shadowCustomResolution = (int)this.NGSS_SHADOWS_RESOLUTION;
			}
			else
			{
				this.DirLight.shadowCustomResolution = 0;
				this.DirLight.shadowResolution = LightShadowResolution.FromQualitySettings;
			}
			if (QualitySettings.shadowCascades > 1)
			{
				Shader.SetGlobalFloat(this._ngssCascadesSoftnessNormalizationid, this.NGSS_CASCADES_SOFTNESS_NORMALIZATION);
				Shader.SetGlobalFloat(this._ngssCascadesCountid, (float)QualitySettings.shadowCascades);
				Shader.SetGlobalVector(this._ngssCascadesSplitsid, (QualitySettings.shadowCascades == 2) ? new Vector4(QualitySettings.shadowCascade2Split, 1f, 1f, 1f) : new Vector4(QualitySettings.shadowCascade4Split.x, QualitySettings.shadowCascade4Split.y, QualitySettings.shadowCascade4Split.z, 1f));
			}
			if (this.NGSS_CASCADES_BLENDING && QualitySettings.shadowCascades > 1)
			{
				Shader.EnableKeyword("NGSS_USE_CASCADE_BLENDING");
				Shader.SetGlobalFloat(this._ngssCascadeBlendDistanceid, this.NGSS_CASCADES_BLENDING_VALUE * 0.125f);
			}
			else
			{
				Shader.DisableKeyword("NGSS_USE_CASCADE_BLENDING");
			}
			this.isSetup = true;
		}

		// Token: 0x04009F45 RID: 40773
		[Header("MAIN SETTINGS")]
		public Shader denoiserShader;

		// Token: 0x04009F46 RID: 40774
		[Tooltip("If disabled, NGSS Directional shadows replacement will be removed from Graphics settings when OnDisable is called in this component.")]
		public bool NGSS_KEEP_ONDISABLE = true;

		// Token: 0x04009F47 RID: 40775
		[Tooltip("Check this option if you don't need to update shadows variables at runtime, only once when scene loads.\nUseful to save some CPU cycles.")]
		public bool NGSS_NO_UPDATE_ON_PLAY;

		// Token: 0x04009F48 RID: 40776
		[Tooltip("Shadows resolution.\nUseQualitySettings = From Quality Settings, SuperLow = 512, Low = 1024, Med = 2048, High = 4096, Ultra = 8192, Mega = 16384.")]
		public NGSS_Directional.ShadowMapResolution NGSS_SHADOWS_RESOLUTION = NGSS_Directional.ShadowMapResolution.UseQualitySettings;

		// Token: 0x04009F49 RID: 40777
		[Header("BASE SAMPLING")]
		[Tooltip("Used to test blocker search and early bail out algorithms. Keep it as low as possible, might lead to white noise if too low.\nRecommended values: Mobile = 8, Consoles & VR = 16, Desktop = 24")]
		[Range(4f, 32f)]
		public int NGSS_SAMPLING_TEST = 16;

		// Token: 0x04009F4A RID: 40778
		[Tooltip("Number of samplers per pixel used for PCF and PCSS shadows algorithms.\nRecommended values: Mobile = 16, Consoles & VR = 32, Desktop Med = 48, Desktop High = 64, Desktop Ultra = 128")]
		[Range(8f, 128f)]
		public int NGSS_SAMPLING_FILTER = 48;

		// Token: 0x04009F4B RID: 40779
		[Tooltip("New optimization that reduces sampling over distance. Interpolates current sampling set (TEST and FILTER) down to 4spp when reaching this distance.")]
		[Range(0f, 500f)]
		public float NGSS_SAMPLING_DISTANCE = 75f;

		// Token: 0x04009F4C RID: 40780
		[Header("SHADOW SOFTNESS")]
		[Tooltip("Overall shadows softness.")]
		[Range(0f, 3f)]
		public float NGSS_SHADOWS_SOFTNESS = 1f;

		// Token: 0x04009F4D RID: 40781
		[Header("PCSS")]
		[Tooltip("PCSS Requires inline sampling and SM3.5.\nProvides Area Light soft-shadows.\nDisable it if you are looking for PCF filtering (uniform soft-shadows) which runs with SM3.0.")]
		public bool NGSS_PCSS_ENABLED;

		// Token: 0x04009F4E RID: 40782
		[Tooltip("How soft shadows are when close to caster.")]
		[Range(0f, 2f)]
		public float NGSS_PCSS_SOFTNESS_NEAR = 0.125f;

		// Token: 0x04009F4F RID: 40783
		[Tooltip("How soft shadows are when far from caster.")]
		[Range(0f, 2f)]
		public float NGSS_PCSS_SOFTNESS_FAR = 1f;

		// Token: 0x04009F50 RID: 40784
		[Header("NOISE")]
		[Tooltip("If zero = 100% noise.\nIf one = 100% dithering.\nUseful when fighting banding.")]
		[Range(0f, 1f)]
		public int NGSS_NOISE_TO_DITHERING_SCALE;

		// Token: 0x04009F51 RID: 40785
		[Tooltip("If you set the noise scale value to something less than 1 you need to input a noise texture.\nRecommended noise textures are blue noise signals.")]
		public Texture2D NGSS_NOISE_TEXTURE;

		// Token: 0x04009F52 RID: 40786
		[Header("DENOISER")]
		[Tooltip("Separable low pass filter that help fight artifacts and noise in shadows.\nRequires Cascaded Shadows to be enabled in the Editor Graphics Settings.")]
		public bool NGSS_DENOISER_ENABLED = true;

		// Token: 0x04009F53 RID: 40787
		[Tooltip("How many iterations the Denoiser algorithm should do.")]
		[Range(1f, 4f)]
		public int NGSS_DENOISER_PASSES = 1;

		// Token: 0x04009F54 RID: 40788
		[Tooltip("Overall Denoiser softness.")]
		[Range(0.01f, 1f)]
		public float NGSS_DENOISER_SOFTNESS = 1f;

		// Token: 0x04009F55 RID: 40789
		[Tooltip("The amount of shadow edges the Denoiser can tolerate during denoising.")]
		[Range(0.01f, 1f)]
		public float NGSS_DENOISER_EDGE_SOFTNESS = 1f;

		// Token: 0x04009F56 RID: 40790
		[Header("BIAS")]
		[Tooltip("This estimates receiver slope using derivatives and tries to tilt the filtering kernel along it.\nHowever, when doing it in screenspace from the depth texture can leads to shadow artifacts.\nThus it is disabled by default.")]
		public bool NGSS_RECEIVER_PLANE_BIAS;

		// Token: 0x04009F57 RID: 40791
		[Header("CASCADES")]
		[Tooltip("Blends cascades at seams intersection.\nAdditional overhead required for this option.")]
		public bool NGSS_CASCADES_BLENDING = true;

		// Token: 0x04009F58 RID: 40792
		[Tooltip("Tweak this value to adjust the blending transition between cascades.")]
		[Range(0f, 2f)]
		public float NGSS_CASCADES_BLENDING_VALUE = 1f;

		// Token: 0x04009F59 RID: 40793
		[Range(0f, 1f)]
		[Tooltip("If one, softness across cascades will be matched using splits distribution, resulting in realistic soft-ness over distance.\nIf zero the softness distribution will be based on cascade index, resulting in blurrier shadows over distance thus less realistic.")]
		public float NGSS_CASCADES_SOFTNESS_NORMALIZATION = 1f;

		// Token: 0x04009F5A RID: 40794
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssDirSamplingDistanceid = Shader.PropertyToID("NGSS_DIR_SAMPLING_DISTANCE");

		// Token: 0x04009F5B RID: 40795
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssTestSamplersDirid = Shader.PropertyToID("NGSS_TEST_SAMPLERS_DIR");

		// Token: 0x04009F5C RID: 40796
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssFilterSamplersDirid = Shader.PropertyToID("NGSS_FILTER_SAMPLERS_DIR");

		// Token: 0x04009F5D RID: 40797
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssGlobalSoftnessid = Shader.PropertyToID("NGSS_GLOBAL_SOFTNESS");

		// Token: 0x04009F5E RID: 40798
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssGlobalSoftnessOptimizedid = Shader.PropertyToID("NGSS_GLOBAL_SOFTNESS_OPTIMIZED");

		// Token: 0x04009F5F RID: 40799
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssOptimizedIterationsid = Shader.PropertyToID("NGSS_OPTIMIZED_ITERATIONS");

		// Token: 0x04009F60 RID: 40800
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssOptimizedSamplersid = Shader.PropertyToID("NGSS_OPTIMIZED_SAMPLERS");

		// Token: 0x04009F61 RID: 40801
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssNoiseToDitheringScaleDirid = Shader.PropertyToID("NGSS_NOISE_TO_DITHERING_SCALE_DIR");

		// Token: 0x04009F62 RID: 40802
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssPcssFilterDirMinid = Shader.PropertyToID("NGSS_PCSS_FILTER_DIR_MIN");

		// Token: 0x04009F63 RID: 40803
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssPcssFilterDirMaxid = Shader.PropertyToID("NGSS_PCSS_FILTER_DIR_MAX");

		// Token: 0x04009F64 RID: 40804
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssCascadesSoftnessNormalizationid = Shader.PropertyToID("NGSS_CASCADES_SOFTNESS_NORMALIZATION");

		// Token: 0x04009F65 RID: 40805
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssCascadesCountid = Shader.PropertyToID("NGSS_CASCADES_COUNT");

		// Token: 0x04009F66 RID: 40806
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssCascadesSplitsid = Shader.PropertyToID("NGSS_CASCADES_SPLITS");

		// Token: 0x04009F67 RID: 40807
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _ngssCascadeBlendDistanceid = Shader.PropertyToID("NGSS_CASCADE_BLEND_DISTANCE");

		// Token: 0x04009F68 RID: 40808
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _denoiseKernelID = Shader.PropertyToID("DenoiseKernel");

		// Token: 0x04009F69 RID: 40809
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _denoiseSoftnessID = Shader.PropertyToID("DenoiseSoftness");

		// Token: 0x04009F6A RID: 40810
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _denoiseTextureID = Shader.PropertyToID("NGSS_DenoiseTexture");

		// Token: 0x04009F6B RID: 40811
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _denoiseEdgeToleranceID = Shader.PropertyToID("DenoiseEdgeTolerance");

		// Token: 0x04009F6C RID: 40812
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public CommandBuffer _computeDenoiseCB;

		// Token: 0x04009F6D RID: 40813
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public CommandBuffer _blendDenoiseCB;

		// Token: 0x04009F6E RID: 40814
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _DENOISER_ENABLED;

		// Token: 0x04009F6F RID: 40815
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _DENOISER_PASSES = 1;

		// Token: 0x04009F70 RID: 40816
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool isSetup;

		// Token: 0x04009F71 RID: 40817
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool isInitialized;

		// Token: 0x04009F72 RID: 40818
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool isGraphicSet;

		// Token: 0x04009F73 RID: 40819
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Light _DirLight;

		// Token: 0x04009F74 RID: 40820
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Material _mMaterial;

		// Token: 0x02001B2B RID: 6955
		public enum ShadowMapResolution
		{
			// Token: 0x04009F76 RID: 40822
			UseQualitySettings = 256,
			// Token: 0x04009F77 RID: 40823
			VeryLow = 512,
			// Token: 0x04009F78 RID: 40824
			Low = 1024,
			// Token: 0x04009F79 RID: 40825
			Med = 2048,
			// Token: 0x04009F7A RID: 40826
			High = 4096,
			// Token: 0x04009F7B RID: 40827
			Ultra = 8192,
			// Token: 0x04009F7C RID: 40828
			Mega = 16384
		}
	}
}
