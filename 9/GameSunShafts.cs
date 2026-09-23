using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x0200006A RID: 106
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Rendering/Game Sun Shafts")]
public class GameSunShafts : PostEffectsBase
{
	// Token: 0x06000224 RID: 548 RVA: 0x00011DB4 File Offset: 0x0000FFB4
	public override bool CheckResources()
	{
		base.CheckSupport(this.useDepthTexture);
		this.sunShaftsMaterial = base.CheckShaderAndCreateMaterial(this.sunShaftsShader, this.sunShaftsMaterial);
		this.simpleClearMaterial = base.CheckShaderAndCreateMaterial(this.simpleClearShader, this.simpleClearMaterial);
		if (!this.isSupported)
		{
			base.ReportAutoDisable();
		}
		return this.isSupported;
	}

	// Token: 0x06000225 RID: 549 RVA: 0x00011E14 File Offset: 0x00010014
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (this.useDepthTexture)
		{
			base.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		}
		int num = 4;
		if (this.resolution == GameSunShafts.SunShaftsResolution.Normal)
		{
			num = 2;
		}
		else if (this.resolution == GameSunShafts.SunShaftsResolution.High)
		{
			num = 1;
		}
		Vector3 vector = base.GetComponent<Camera>().WorldToViewportPoint(this.sunPosition);
		int width = source.width / num;
		int height = source.height / num;
		RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
		this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(1f, 1f, 0f, 0f) * this.sunShaftBlurRadius);
		this.sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, this.maxRadius));
		this.sunShaftsMaterial.SetVector("_SunThreshold", this.sunThreshold);
		if (!this.useDepthTexture)
		{
			RenderTextureFormat format = base.GetComponent<Camera>().allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, 0, format);
			RenderTexture.active = temporary2;
			GL.ClearWithSkybox(false, base.GetComponent<Camera>());
			this.sunShaftsMaterial.SetTexture("_Skybox", temporary2);
			Graphics.Blit(source, temporary, this.sunShaftsMaterial, 3);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			Graphics.Blit(source, temporary, this.sunShaftsMaterial, 2);
		}
		base.DrawBorder(temporary, this.simpleClearMaterial);
		this.radialBlurIterations = Mathf.Clamp(this.radialBlurIterations, 1, 4);
		float num2 = this.sunShaftBlurRadius * 0.0013020834f;
		this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, 0f, 0f));
		this.sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, this.maxRadius));
		for (int i = 0; i < this.radialBlurIterations; i++)
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(width, height, 0);
			Graphics.Blit(temporary, temporary3, this.sunShaftsMaterial, 1);
			RenderTexture.ReleaseTemporary(temporary);
			num2 = this.sunShaftBlurRadius * (((float)i * 2f + 1f) * 6f) / 768f;
			this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, 0f, 0f));
			temporary = RenderTexture.GetTemporary(width, height, 0);
			Graphics.Blit(temporary3, temporary, this.sunShaftsMaterial, 1);
			RenderTexture.ReleaseTemporary(temporary3);
			num2 = this.sunShaftBlurRadius * (((float)i * 2f + 2f) * 6f) / 768f;
			this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, 0f, 0f));
		}
		if (vector.z >= 0f)
		{
			this.sunShaftsMaterial.SetVector("_SunColor", new Vector4(this.sunColor.r, this.sunColor.g, this.sunColor.b, this.sunColor.a) * this.sunShaftIntensity);
		}
		else
		{
			this.sunShaftsMaterial.SetVector("_SunColor", Vector4.zero);
		}
		this.sunShaftsMaterial.SetTexture("_ColorBuffer", temporary);
		Graphics.Blit(source, destination, this.sunShaftsMaterial, (this.screenBlendMode == GameSunShafts.ShaftsScreenBlendMode.Screen) ? 0 : 4);
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x040002C0 RID: 704
	public GameSunShafts.SunShaftsResolution resolution = GameSunShafts.SunShaftsResolution.Normal;

	// Token: 0x040002C1 RID: 705
	public GameSunShafts.ShaftsScreenBlendMode screenBlendMode;

	// Token: 0x040002C2 RID: 706
	public Vector3 sunPosition;

	// Token: 0x040002C3 RID: 707
	public int radialBlurIterations = 2;

	// Token: 0x040002C4 RID: 708
	public Color sunColor = Color.white;

	// Token: 0x040002C5 RID: 709
	public Color sunThreshold = new Color(0.87f, 0.74f, 0.65f);

	// Token: 0x040002C6 RID: 710
	public float sunShaftBlurRadius = 2.5f;

	// Token: 0x040002C7 RID: 711
	public float sunShaftIntensity = 1.15f;

	// Token: 0x040002C8 RID: 712
	public float maxRadius = 0.75f;

	// Token: 0x040002C9 RID: 713
	public bool useDepthTexture = true;

	// Token: 0x040002CA RID: 714
	public Shader sunShaftsShader;

	// Token: 0x040002CB RID: 715
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material sunShaftsMaterial;

	// Token: 0x040002CC RID: 716
	public Shader simpleClearShader;

	// Token: 0x040002CD RID: 717
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material simpleClearMaterial;

	// Token: 0x0200006B RID: 107
	public enum SunShaftsResolution
	{
		// Token: 0x040002CF RID: 719
		Low,
		// Token: 0x040002D0 RID: 720
		Normal,
		// Token: 0x040002D1 RID: 721
		High
	}

	// Token: 0x0200006C RID: 108
	public enum ShaftsScreenBlendMode
	{
		// Token: 0x040002D3 RID: 723
		Screen,
		// Token: 0x040002D4 RID: 724
		Add
	}
}
