using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001331 RID: 4913
public class CharacterMatrixOverride : MonoBehaviour
{
	// Token: 0x06009B1D RID: 39709 RVA: 0x003A9B94 File Offset: 0x003A7D94
	public void Init(EntityPlayerLocal epl)
	{
		this.referenceCamera = epl.playerCamera;
	}

	// Token: 0x06009B1E RID: 39710 RVA: 0x003A9BA4 File Offset: 0x003A7DA4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(this.OnPreCullCallback));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(this.OnPreRenderCallback));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(this.OnPostRenderCallback));
	}

	// Token: 0x06009B1F RID: 39711 RVA: 0x003A9C14 File Offset: 0x003A7E14
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(this.OnPreCullCallback));
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(this.OnPreRenderCallback));
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(this.OnPostRenderCallback));
	}

	// Token: 0x06009B20 RID: 39712 RVA: 0x003A9C81 File Offset: 0x003A7E81
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreCullCallback(Camera camera)
	{
		if (this.Active && camera == this.referenceCamera)
		{
			this.UpdateRendererList();
		}
	}

	// Token: 0x06009B21 RID: 39713 RVA: 0x003A9CA0 File Offset: 0x003A7EA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateRendererList()
	{
		this.overriddenRenderers.Clear();
		base.GetComponentsInChildren<Renderer>(this.overriddenRenderers);
		foreach (Renderer key in this.overriddenRenderers)
		{
			CharacterMatrixOverride.RendererSettings rendererSettings;
			if (!this.rendererSettingsMap.TryGetValue(key, out rendererSettings))
			{
				rendererSettings = new CharacterMatrixOverride.RendererSettings();
				rendererSettings.originalProperties = new MaterialPropertyBlock();
				rendererSettings.overriddenProperties = new MaterialPropertyBlock();
				this.rendererSettingsMap[key] = rendererSettings;
			}
		}
	}

	// Token: 0x06009B22 RID: 39714 RVA: 0x003A9D3C File Offset: 0x003A7F3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreRenderCallback(Camera camera)
	{
		if (!this.Active || camera != this.referenceCamera)
		{
			return;
		}
		Matrix4x4 value = Matrix4x4.Perspective(90f, 1f, 10000f, 10000.1f) * this.referenceCamera.worldToCameraMatrix;
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			CharacterMatrixOverride.RendererSettings rendererSettings;
			if (!this.rendererSettingsMap.TryGetValue(renderer, out rendererSettings))
			{
				Log.Error("[CharacterMatrixOverride] Failed to retrieve RendererSettings for overridden renderer");
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

	// Token: 0x06009B23 RID: 39715 RVA: 0x003A9E1C File Offset: 0x003A801C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPostRenderCallback(Camera camera)
	{
		if (camera != this.referenceCamera)
		{
			return;
		}
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			CharacterMatrixOverride.RendererSettings rendererSettings;
			if (!(renderer == null) && this.rendererSettingsMap.TryGetValue(renderer, out rendererSettings))
			{
				renderer.SetPropertyBlock(rendererSettings.originalProperties);
			}
		}
	}

	// Token: 0x040074F7 RID: 29943
	public bool Active;

	// Token: 0x040074F8 RID: 29944
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera referenceCamera;

	// Token: 0x040074F9 RID: 29945
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Renderer> overriddenRenderers = new List<Renderer>();

	// Token: 0x040074FA RID: 29946
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Renderer, CharacterMatrixOverride.RendererSettings> rendererSettingsMap = new Dictionary<Renderer, CharacterMatrixOverride.RendererSettings>();

	// Token: 0x02001332 RID: 4914
	[PublicizedFrom(EAccessModifier.Private)]
	public class RendererSettings
	{
		// Token: 0x040074FB RID: 29947
		public MaterialPropertyBlock originalProperties;

		// Token: 0x040074FC RID: 29948
		public MaterialPropertyBlock overriddenProperties;
	}
}
