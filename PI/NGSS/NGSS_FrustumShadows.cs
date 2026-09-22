using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace PI.NGSS
{
	// Token: 0x02001B2C RID: 6956
	[ImageEffectAllowedInSceneView]
	[ExecuteInEditMode]
	public class NGSS_FrustumShadows : MonoBehaviour
	{
		// Token: 0x0600D0B9 RID: 53433 RVA: 0x004C4B52 File Offset: 0x004C2D52
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsNotSupported()
		{
			return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2;
		}

		// Token: 0x1700199A RID: 6554
		// (get) Token: 0x0600D0BA RID: 53434 RVA: 0x004C5074 File Offset: 0x004C3274
		public Camera mCamera
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				if (this._mCamera == null)
				{
					this._mCamera = base.GetComponent<Camera>();
					if (this._mCamera == null)
					{
						this._mCamera = Camera.main;
					}
					if (this._mCamera == null)
					{
						Debug.LogError("NGSS Error: No MainCamera found, please provide one.", this);
						base.enabled = false;
					}
				}
				return this._mCamera;
			}
		}

		// Token: 0x1700199B RID: 6555
		// (get) Token: 0x0600D0BC RID: 53436 RVA: 0x004C50E4 File Offset: 0x004C32E4
		// (set) Token: 0x0600D0BB RID: 53435 RVA: 0x004C50DA File Offset: 0x004C32DA
		public Material mMaterial
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				if (this._mMaterial == null)
				{
					if (this.frustumShadowsShader == null)
					{
						this.frustumShadowsShader = Shader.Find("Hidden/NGSS_FrustumShadows");
					}
					this._mMaterial = new Material(this.frustumShadowsShader);
					if (this._mMaterial == null)
					{
						Debug.LogWarning("NGSS Warning: can't find NGSS_FrustumShadows shader, make sure it's on your project.", this);
						base.enabled = false;
					}
				}
				return this._mMaterial;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this._mMaterial = value;
			}
		}

		// Token: 0x0600D0BD RID: 53437 RVA: 0x004C5154 File Offset: 0x004C3354
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddCommandBuffers()
		{
			if (this.computeShadowsCB == null)
			{
				this.computeShadowsCB = new CommandBuffer
				{
					name = "NGSS FrustumShadows: Compute"
				};
			}
			else
			{
				this.computeShadowsCB.Clear();
			}
			bool flag = true;
			if (this.mCamera)
			{
				CommandBuffer[] commandBuffers = this.mCamera.GetCommandBuffers((this.mCamera.actualRenderingPath == RenderingPath.DeferredShading) ? CameraEvent.BeforeLighting : CameraEvent.AfterDepthTexture);
				for (int i = 0; i < commandBuffers.Length; i++)
				{
					if (!(commandBuffers[i].name != this.computeShadowsCB.name))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					this.mCamera.AddCommandBuffer((this.mCamera.actualRenderingPath == RenderingPath.DeferredShading) ? CameraEvent.BeforeLighting : CameraEvent.AfterDepthTexture, this.computeShadowsCB);
				}
			}
		}

		// Token: 0x0600D0BE RID: 53438 RVA: 0x004C520C File Offset: 0x004C340C
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveCommandBuffers()
		{
			this._mMaterial = null;
			if (this.mCamera)
			{
				this.mCamera.RemoveCommandBuffer(CameraEvent.BeforeLighting, this.computeShadowsCB);
				this.mCamera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, this.computeShadowsCB);
			}
			this._isInit = false;
		}

		// Token: 0x0600D0BF RID: 53439 RVA: 0x004C5258 File Offset: 0x004C3458
		[PublicizedFrom(EAccessModifier.Private)]
		public void Init()
		{
			int scaledPixelWidth = this.mCamera.scaledPixelWidth;
			int scaledPixelHeight = this.mCamera.scaledPixelHeight;
			this.m_shadowsBlurIterations = (this.m_fastBlur ? 1 : this.m_shadowsBlurIterations);
			if (this._iterations == this.m_shadowsBlurIterations && this._downGrade == this.m_shadowsDownGrade && this._width == scaledPixelWidth && this._height == scaledPixelHeight && (this._isInit || this.mainShadowsLight == null))
			{
				return;
			}
			if (this.mCamera.actualRenderingPath == RenderingPath.VertexLit)
			{
				Debug.LogWarning("Vertex Lit Rendering Path is not supported by NGSS Contact Shadows. Please set the Rendering Path in your game camera or Graphics Settings to something else than Vertex Lit.", this);
				base.enabled = false;
				return;
			}
			if (this.mCamera.actualRenderingPath == RenderingPath.Forward)
			{
				this.mCamera.depthTextureMode |= DepthTextureMode.Depth;
			}
			this.AddCommandBuffers();
			this._width = scaledPixelWidth;
			this._height = scaledPixelHeight;
			this._downGrade = this.m_shadowsDownGrade;
			int nameID = Shader.PropertyToID("NGSS_ContactShadowRT1");
			int nameID2 = Shader.PropertyToID("NGSS_ContactShadowRT2");
			this.computeShadowsCB.GetTemporaryRT(nameID, scaledPixelWidth / this._downGrade, scaledPixelHeight / this._downGrade, 0, FilterMode.Bilinear, RenderTextureFormat.RG16, RenderTextureReadWrite.Linear);
			this.computeShadowsCB.GetTemporaryRT(nameID2, scaledPixelWidth / this._downGrade, scaledPixelHeight / this._downGrade, 0, FilterMode.Bilinear, RenderTextureFormat.RG16, RenderTextureReadWrite.Linear);
			this.computeShadowsCB.Blit(null, nameID, this.mMaterial, 0);
			this._iterations = this.m_shadowsBlurIterations;
			for (int i = 1; i <= this._iterations; i++)
			{
				this.computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(0f, (float)i));
				this.computeShadowsCB.Blit(nameID, nameID2, this.mMaterial, 1);
				this.computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2((float)i, 0f));
				this.computeShadowsCB.Blit(nameID2, nameID, this.mMaterial, 1);
			}
			this.computeShadowsCB.SetGlobalTexture("NGSS_FrustumShadowsTexture", nameID);
			this.computeShadowsCB.ReleaseTemporaryRT(nameID);
			this.computeShadowsCB.ReleaseTemporaryRT(nameID2);
			this._isInit = true;
		}

		// Token: 0x0600D0C0 RID: 53440 RVA: 0x004C5487 File Offset: 0x004C3687
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

		// Token: 0x0600D0C1 RID: 53441 RVA: 0x004C54AA File Offset: 0x004C36AA
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			Shader.SetGlobalFloat("NGSS_FRUSTUM_SHADOWS_ENABLED", 0f);
			if (this._isInit)
			{
				this.RemoveCommandBuffers();
			}
			if (this.mMaterial != null)
			{
				UnityEngine.Object.DestroyImmediate(this.mMaterial);
				this.mMaterial = null;
			}
		}

		// Token: 0x0600D0C2 RID: 53442 RVA: 0x004C54E9 File Offset: 0x004C36E9
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnApplicationQuit()
		{
			if (this._isInit)
			{
				this.RemoveCommandBuffers();
			}
		}

		// Token: 0x0600D0C3 RID: 53443 RVA: 0x004C54FC File Offset: 0x004C36FC
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnPreRender()
		{
			this.Init();
			if (!this._isInit || this.mainShadowsLight == null)
			{
				return;
			}
			if (this._currentRenderingPath != this.mCamera.actualRenderingPath)
			{
				this._currentRenderingPath = this.mCamera.actualRenderingPath;
				this.RemoveCommandBuffers();
				this.AddCommandBuffers();
			}
			Shader.SetGlobalFloat("NGSS_FRUSTUM_SHADOWS_ENABLED", 1f);
			Shader.SetGlobalFloat("NGSS_FRUSTUM_SHADOWS_OPACITY", 1f - this.mainShadowsLight.shadowStrength);
			if (this.m_Temporal)
			{
				this.m_temporalJitter = (this.m_temporalJitter + 1) % 8;
				this.mMaterial.SetFloat("TemporalJitter", (float)this.m_temporalJitter * this.m_JitterScale * 0.0002f);
			}
			else
			{
				this.mMaterial.SetFloat("TemporalJitter", 0f);
			}
			if (QualitySettings.shadowProjection == ShadowProjection.StableFit)
			{
				this.mMaterial.EnableKeyword("SHADOWS_SPLIT_SPHERES");
			}
			else
			{
				this.mMaterial.DisableKeyword("SHADOWS_SPLIT_SPHERES");
			}
			this.mMaterial.SetMatrix("WorldToView", this.mCamera.worldToCameraMatrix);
			this.mMaterial.SetVector("LightDir", this.mCamera.transform.InverseTransformDirection(-this.mainShadowsLight.transform.forward));
			this.mMaterial.SetVector("LightPosRange", new Vector4(this.mainShadowsLight.transform.position.x, this.mainShadowsLight.transform.position.y, this.mainShadowsLight.transform.position.z, this.mainShadowsLight.range * this.mainShadowsLight.range));
			this.mMaterial.SetVector("LightDirWorld", -this.mainShadowsLight.transform.forward);
			this.mMaterial.SetFloat("ShadowsEdgeTolerance", this.m_shadowsEdgeBlur);
			this.mMaterial.SetFloat("ShadowsSoftness", this.m_shadowsBlur);
			this.mMaterial.SetFloat("RayScale", this.m_rayScale);
			this.mMaterial.SetFloat("ShadowsBias", this.m_shadowsBias * 0.02f);
			this.mMaterial.SetFloat("ShadowsDistanceStart", this.m_shadowsDistanceStart - 10f);
			this.mMaterial.SetFloat("RayThickness", this.m_rayThickness);
			this.mMaterial.SetFloat("RaySamples", (float)this.m_raySamples);
			if (this.m_deferredBackfaceOptimization && this.mCamera.actualRenderingPath == RenderingPath.DeferredShading)
			{
				this.mMaterial.EnableKeyword("NGSS_DEFERRED_OPTIMIZATION");
				this.mMaterial.SetFloat("BackfaceOpacity", this.m_deferredBackfaceTranslucency);
			}
			else
			{
				this.mMaterial.DisableKeyword("NGSS_DEFERRED_OPTIMIZATION");
			}
			if (this.m_dithering)
			{
				this.mMaterial.EnableKeyword("NGSS_USE_DITHERING");
			}
			else
			{
				this.mMaterial.DisableKeyword("NGSS_USE_DITHERING");
			}
			if (this.m_fastBlur)
			{
				this.mMaterial.EnableKeyword("NGSS_FAST_BLUR");
			}
			else
			{
				this.mMaterial.DisableKeyword("NGSS_FAST_BLUR");
			}
			if (this.mainShadowsLight.type != LightType.Directional)
			{
				this.mMaterial.EnableKeyword("NGSS_USE_LOCAL_SHADOWS");
			}
			else
			{
				this.mMaterial.DisableKeyword("NGSS_USE_LOCAL_SHADOWS");
			}
			this.mMaterial.SetFloat("RayScreenScale", this.m_rayScreenScale ? 1f : 0f);
		}

		// Token: 0x0600D0C4 RID: 53444 RVA: 0x004C5884 File Offset: 0x004C3A84
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnPostRender()
		{
			Shader.SetGlobalFloat("NGSS_FRUSTUM_SHADOWS_ENABLED", 0f);
		}

		// Token: 0x0600D0C5 RID: 53445 RVA: 0x004C5895 File Offset: 0x004C3A95
		[PublicizedFrom(EAccessModifier.Private)]
		public void BlitXR(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, Material mat, int pass)
		{
			cmd.SetRenderTarget(dest, 0, CubemapFace.Unknown, -1);
			cmd.ClearRenderTarget(true, true, Color.clear);
			cmd.DrawMesh(this.FullScreenTriangle, Matrix4x4.identity, mat, pass);
		}

		// Token: 0x1700199C RID: 6556
		// (get) Token: 0x0600D0C6 RID: 53446 RVA: 0x004C58C4 File Offset: 0x004C3AC4
		public Mesh FullScreenTriangle
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				if (this._fullScreenTriangle)
				{
					return this._fullScreenTriangle;
				}
				this._fullScreenTriangle = new Mesh
				{
					name = "Full-Screen Triangle",
					vertices = new Vector3[]
					{
						new Vector3(-1f, -1f, 0f),
						new Vector3(-1f, 3f, 0f),
						new Vector3(3f, -1f, 0f)
					},
					triangles = new int[]
					{
						0,
						1,
						2
					}
				};
				this._fullScreenTriangle.UploadMeshData(true);
				return this._fullScreenTriangle;
			}
		}

		// Token: 0x04009F7D RID: 40829
		[Header("REFERENCES")]
		public Light mainShadowsLight;

		// Token: 0x04009F7E RID: 40830
		public Shader frustumShadowsShader;

		// Token: 0x04009F7F RID: 40831
		[Header("SHADOWS SETTINGS")]
		[Tooltip("Poisson Noise. Randomize samples to remove repeated patterns.")]
		public bool m_dithering;

		// Token: 0x04009F80 RID: 40832
		[Tooltip("If enabled a faster separable blur will be used.\nIf disabled a slower depth aware blur will be used.")]
		public bool m_fastBlur = true;

		// Token: 0x04009F81 RID: 40833
		[Tooltip("If enabled, backfaced lit fragments will be skipped increasing performance. Requires GBuffer normals.")]
		public bool m_deferredBackfaceOptimization;

		// Token: 0x04009F82 RID: 40834
		[Range(0f, 1f)]
		[Tooltip("Set how backfaced lit fragments are shaded. Requires DeferredBackfaceOptimization to be enabled.")]
		public float m_deferredBackfaceTranslucency;

		// Token: 0x04009F83 RID: 40835
		[Tooltip("Tweak this value to remove soft-shadows leaking around edges.")]
		[Range(0.01f, 1f)]
		public float m_shadowsEdgeBlur = 0.25f;

		// Token: 0x04009F84 RID: 40836
		[Tooltip("Overall softness of the shadows.")]
		[Range(0.01f, 1f)]
		public float m_shadowsBlur = 0.5f;

		// Token: 0x04009F85 RID: 40837
		[Tooltip("Overall softness of the shadows. Higher values than 1 wont work well if FastBlur is enabled.")]
		[Range(1f, 4f)]
		public int m_shadowsBlurIterations = 1;

		// Token: 0x04009F86 RID: 40838
		[Tooltip("Rising this value will make shadows more blurry but also lower in resolution.")]
		[Range(1f, 4f)]
		public int m_shadowsDownGrade = 1;

		// Token: 0x04009F87 RID: 40839
		[Tooltip("Tweak this value if your objects display backface shadows.")]
		[Range(0f, 1f)]
		public float m_shadowsBias = 0.05f;

		// Token: 0x04009F88 RID: 40840
		[Tooltip("The distance in metters from camera where shadows start to shown.")]
		public float m_shadowsDistanceStart;

		// Token: 0x04009F89 RID: 40841
		[Header("RAY SETTINGS")]
		[Tooltip("If enabled the ray length will be scaled at screen space instead of world space. Keep it enabled for an infinite view shadows coverage. Disable it for a ContactShadows like effect. Adjust the Ray Scale property accordingly.")]
		public bool m_rayScreenScale = true;

		// Token: 0x04009F8A RID: 40842
		[Tooltip("Number of samplers between each step. The higher values produces less gaps between shadows but is more costly.")]
		[Range(16f, 128f)]
		public int m_raySamples = 64;

		// Token: 0x04009F8B RID: 40843
		[Tooltip("The higher the value, the larger the shadows ray will be.")]
		[Range(0.01f, 1f)]
		public float m_rayScale = 0.25f;

		// Token: 0x04009F8C RID: 40844
		[Tooltip("The higher the value, the ticker the shadows will look.")]
		[Range(0f, 1f)]
		public float m_rayThickness = 0.01f;

		// Token: 0x04009F8D RID: 40845
		[Header("TEMPORAL SETTINGS")]
		[Tooltip("Enable this option if you use temporal anti-aliasing in your project. Works better when Dithering is enabled.")]
		public bool m_Temporal;

		// Token: 0x04009F8E RID: 40846
		[Range(0f, 1f)]
		public float m_JitterScale = 0.5f;

		// Token: 0x04009F8F RID: 40847
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int m_temporalJitter;

		// Token: 0x04009F90 RID: 40848
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _iterations = 1;

		// Token: 0x04009F91 RID: 40849
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _downGrade = 1;

		// Token: 0x04009F92 RID: 40850
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _width;

		// Token: 0x04009F93 RID: 40851
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int _height;

		// Token: 0x04009F94 RID: 40852
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public RenderingPath _currentRenderingPath;

		// Token: 0x04009F95 RID: 40853
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public CommandBuffer computeShadowsCB;

		// Token: 0x04009F96 RID: 40854
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _isInit;

		// Token: 0x04009F97 RID: 40855
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Camera _mCamera;

		// Token: 0x04009F98 RID: 40856
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Material _mMaterial;

		// Token: 0x04009F99 RID: 40857
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Mesh _fullScreenTriangle;
	}
}
