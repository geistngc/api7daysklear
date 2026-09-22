using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200132C RID: 4908
public class CameraMatrixOverride : MonoBehaviour
{
	// Token: 0x06009B12 RID: 39698 RVA: 0x003A942C File Offset: 0x003A762C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		if (this.referenceCamera == null && !base.TryGetComponent<Camera>(out this.referenceCamera))
		{
			Debug.LogError("Failed to get Camera. The CameraMatrixOverride script must be attached to a GameObject with a Camera component.");
			base.enabled = false;
			return;
		}
		this.originalNearClip = this.referenceCamera.nearClipPlane;
	}

	// Token: 0x06009B13 RID: 39699 RVA: 0x003A9478 File Offset: 0x003A7678
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (this.referenceCamera == null)
		{
			return;
		}
		this.referenceCamera.nearClipPlane = this.originalNearClip;
		this.RestoreChildSettings();
	}

	// Token: 0x06009B14 RID: 39700 RVA: 0x003A94A0 File Offset: 0x003A76A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateRendererList()
	{
		bool flag = this.advancedSettings.enableBoundsPadding && this.referenceCamera.fieldOfView < this.fov;
		this.renderersToRestore.Clear();
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			if (renderer != null)
			{
				this.renderersToRestore.Add(renderer);
			}
		}
		this.overriddenRenderers.Clear();
		base.GetComponentsInChildren<Renderer>(this.overriddenRenderers);
		Vector3 b = new Vector3(this.advancedSettings.boundsPadding, this.advancedSettings.boundsPadding, this.advancedSettings.boundsPadding);
		foreach (Renderer renderer2 in this.overriddenRenderers)
		{
			this.renderersToRestore.Remove(renderer2);
			CameraMatrixOverride.RendererSettings rendererSettings;
			if (!this.rendererSettingsMap.TryGetValue(renderer2, out rendererSettings))
			{
				rendererSettings = new CameraMatrixOverride.RendererSettings();
				rendererSettings.originalShadowCastingMode = renderer2.shadowCastingMode;
				rendererSettings.originalProperties = new MaterialPropertyBlock();
				rendererSettings.overriddenProperties = new MaterialPropertyBlock();
				this.rendererSettingsMap[renderer2] = rendererSettings;
			}
			if (this.advancedSettings.enableChildShadows && rendererSettings.shadowModeDirty)
			{
				renderer2.shadowCastingMode = rendererSettings.originalShadowCastingMode;
				rendererSettings.shadowModeDirty = false;
			}
			else if (!this.advancedSettings.enableChildShadows && !rendererSettings.shadowModeDirty)
			{
				renderer2.shadowCastingMode = ShadowCastingMode.Off;
				rendererSettings.shadowModeDirty = true;
			}
			if (flag && !(renderer2 is ParticleSystemRenderer))
			{
				renderer2.ResetBounds();
				Bounds bounds = renderer2.bounds;
				bounds.extents += b;
				renderer2.bounds = bounds;
				rendererSettings.boundsDirty = true;
			}
			else if (rendererSettings.boundsDirty)
			{
				renderer2.ResetBounds();
				rendererSettings.boundsDirty = false;
			}
		}
		foreach (Renderer renderer3 in this.renderersToRestore)
		{
			CameraMatrixOverride.RendererSettings rendererSettings2;
			if (this.rendererSettingsMap.TryGetValue(renderer3, out rendererSettings2))
			{
				if (rendererSettings2.shadowModeDirty)
				{
					renderer3.shadowCastingMode = rendererSettings2.originalShadowCastingMode;
					rendererSettings2.shadowModeDirty = false;
				}
				if (rendererSettings2.boundsDirty)
				{
					renderer3.ResetBounds();
					rendererSettings2.boundsDirty = false;
				}
			}
		}
		this.renderersToRestore.Clear();
	}

	// Token: 0x06009B15 RID: 39701 RVA: 0x003A9770 File Offset: 0x003A7970
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.advancedSettings.updateTiming == CameraMatrixOverride.UpdateTiming.LateUpdate)
		{
			this.UpdateRendererList();
		}
	}

	// Token: 0x06009B16 RID: 39702 RVA: 0x003A9785 File Offset: 0x003A7985
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreCull()
	{
		if (this.advancedSettings.updateTiming == CameraMatrixOverride.UpdateTiming.OnPreCull)
		{
			this.UpdateRendererList();
		}
	}

	// Token: 0x06009B17 RID: 39703 RVA: 0x003A979C File Offset: 0x003A799C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreRender()
	{
		if (this.advancedSettings.updateTiming == CameraMatrixOverride.UpdateTiming.OnPreRender)
		{
			this.UpdateRendererList();
		}
		this.referenceCamera.nearClipPlane = (this.advancedSettings.enableNearClipOverride ? this.nearClipOverride : this.originalNearClip);
		Matrix4x4 projectionMatrix = this.referenceCamera.projectionMatrix;
		Matrix4x4 matrix4x = Matrix4x4.Perspective(this.fov, this.referenceCamera.aspect, this.referenceCamera.nearClipPlane * this.nearClipFactor, this.referenceCamera.farClipPlane * this.advancedSettings.farClipFactor);
		ref Matrix4x4 ptr = ref matrix4x;
		ptr[0, 2] = ptr[0, 2] + this.advancedSettings.jitterFactor * (projectionMatrix[0, 2] - matrix4x[0, 2]);
		ptr = ref matrix4x;
		ptr[1, 2] = ptr[1, 2] + this.advancedSettings.jitterFactor * (projectionMatrix[1, 2] - matrix4x[1, 2]);
		Matrix4x4 matrix4x2;
		switch (this.advancedSettings.projectionMode)
		{
		case CameraMatrixOverride.ProjectionMode.Custom:
			matrix4x2 = matrix4x;
			break;
		case CameraMatrixOverride.ProjectionMode.Reference:
			matrix4x2 = projectionMatrix;
			break;
		case CameraMatrixOverride.ProjectionMode.ReferenceNonJittered:
			matrix4x2 = this.referenceCamera.nonJitteredProjectionMatrix;
			break;
		default:
			matrix4x2 = projectionMatrix;
			break;
		}
		Matrix4x4 matrix4x3 = matrix4x2;
		if (this.advancedSettings.depthScaleFactor != 1f)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m22 = this.advancedSettings.depthScaleFactor;
			matrix4x3 = identity * matrix4x3;
		}
		matrix4x3 = GL.GetGPUProjectionMatrix(matrix4x3, true);
		Matrix4x4 worldToCameraMatrix = this.referenceCamera.worldToCameraMatrix;
		Matrix4x4 value = matrix4x3 * worldToCameraMatrix;
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			CameraMatrixOverride.RendererSettings rendererSettings;
			if (!this.rendererSettingsMap.TryGetValue(renderer, out rendererSettings))
			{
				Debug.LogError("[CMO] Failed to retrieve RendererSettings for overridden renderer");
			}
			else
			{
				renderer.GetPropertyBlock(rendererSettings.originalProperties);
				renderer.GetPropertyBlock(rendererSettings.overriddenProperties);
				MaterialPropertyBlock overriddenProperties = rendererSettings.overriddenProperties;
				overriddenProperties.SetMatrix("unity_MatrixVP", value);
				renderer.SetPropertyBlock(overriddenProperties);
			}
		}
	}

	// Token: 0x06009B18 RID: 39704 RVA: 0x003A99C8 File Offset: 0x003A7BC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void RestoreChildSettings()
	{
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			CameraMatrixOverride.RendererSettings rendererSettings;
			if (renderer != null && this.rendererSettingsMap.TryGetValue(renderer, out rendererSettings))
			{
				if (rendererSettings.shadowModeDirty)
				{
					renderer.shadowCastingMode = rendererSettings.originalShadowCastingMode;
					rendererSettings.shadowModeDirty = false;
				}
				if (rendererSettings.boundsDirty)
				{
					renderer.ResetBounds();
					rendererSettings.boundsDirty = false;
				}
			}
		}
	}

	// Token: 0x06009B19 RID: 39705 RVA: 0x003A9A60 File Offset: 0x003A7C60
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPostRender()
	{
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			CameraMatrixOverride.RendererSettings rendererSettings;
			if (!(renderer == null) && this.rendererSettingsMap.TryGetValue(renderer, out rendererSettings))
			{
				renderer.SetPropertyBlock(rendererSettings.originalProperties);
			}
		}
	}

	// Token: 0x040074D7 RID: 29911
	[Tooltip("The overridden FoV to use when rendering any child Renderers in the hierarchy beneath the Camera this script is attached to.")]
	public float fov = 45f;

	// Token: 0x040074D8 RID: 29912
	[Range(0.01f, 1f)]
	[Tooltip("The overridden near-clip distance to use when this script is enabled. Note this applies to the Camera as a whole, rather than specifically targeting child Renderers.")]
	public float nearClipOverride = 0.01f;

	// Token: 0x040074D9 RID: 29913
	[Range(1E-45f, 8f)]
	[Tooltip("A value of 1 results in normal rendering behaviour. Higher values effectively squash the depth of child Renderers towards the camera; this reduces the likelihood of clipping into environment geometry, but can distort certain screen effects such as reflections. A value of 2 seems to provide a good balance between reducing clipping and minimising distortion of screen effects.")]
	public float nearClipFactor = 2f;

	// Token: 0x040074DA RID: 29914
	[Tooltip("An assortment of parameters left over from earlier prototyping. They remain exposed for debug purposes if ever required; otherwise it is not recommended to change them away from their default values.")]
	public CameraMatrixOverride.AdvancedSettings advancedSettings = new CameraMatrixOverride.AdvancedSettings();

	// Token: 0x040074DB RID: 29915
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera referenceCamera;

	// Token: 0x040074DC RID: 29916
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Renderer, CameraMatrixOverride.RendererSettings> rendererSettingsMap = new Dictionary<Renderer, CameraMatrixOverride.RendererSettings>();

	// Token: 0x040074DD RID: 29917
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Renderer> overriddenRenderers = new List<Renderer>();

	// Token: 0x040074DE RID: 29918
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public HashSet<Renderer> renderersToRestore = new HashSet<Renderer>();

	// Token: 0x040074DF RID: 29919
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float originalNearClip = 0.0751f;

	// Token: 0x0200132D RID: 4909
	public enum ProjectionMode
	{
		// Token: 0x040074E1 RID: 29921
		Custom,
		// Token: 0x040074E2 RID: 29922
		Reference,
		// Token: 0x040074E3 RID: 29923
		ReferenceNonJittered
	}

	// Token: 0x0200132E RID: 4910
	public enum UpdateTiming
	{
		// Token: 0x040074E5 RID: 29925
		LateUpdate,
		// Token: 0x040074E6 RID: 29926
		OnPreCull,
		// Token: 0x040074E7 RID: 29927
		OnPreRender,
		// Token: 0x040074E8 RID: 29928
		None
	}

	// Token: 0x0200132F RID: 4911
	[Serializable]
	public class AdvancedSettings
	{
		// Token: 0x040074E9 RID: 29929
		public bool enableNearClipOverride = true;

		// Token: 0x040074EA RID: 29930
		public bool enableChildShadows;

		// Token: 0x040074EB RID: 29931
		public bool enableBoundsPadding = true;

		// Token: 0x040074EC RID: 29932
		public CameraMatrixOverride.ProjectionMode projectionMode;

		// Token: 0x040074ED RID: 29933
		public CameraMatrixOverride.UpdateTiming updateTiming = CameraMatrixOverride.UpdateTiming.OnPreCull;

		// Token: 0x040074EE RID: 29934
		[Range(1E-45f, 2f)]
		public float depthScaleFactor = 1f;

		// Token: 0x040074EF RID: 29935
		public float farClipFactor = 1f;

		// Token: 0x040074F0 RID: 29936
		public float jitterFactor = 1f;

		// Token: 0x040074F1 RID: 29937
		public float boundsPadding = 1f;
	}

	// Token: 0x02001330 RID: 4912
	[PublicizedFrom(EAccessModifier.Private)]
	public class RendererSettings
	{
		// Token: 0x040074F2 RID: 29938
		public ShadowCastingMode originalShadowCastingMode;

		// Token: 0x040074F3 RID: 29939
		public MaterialPropertyBlock originalProperties;

		// Token: 0x040074F4 RID: 29940
		public MaterialPropertyBlock overriddenProperties;

		// Token: 0x040074F5 RID: 29941
		public bool boundsDirty;

		// Token: 0x040074F6 RID: 29942
		public bool shadowModeDirty;
	}
}
