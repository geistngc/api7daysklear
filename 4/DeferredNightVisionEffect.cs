using System;
using UnityEngine;

// Token: 0x02000075 RID: 117
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DeferredNightVisionEffect : MonoBehaviour
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000233 RID: 563 RVA: 0x00012A12 File Offset: 0x00010C12
	public Shader NightVisionShader
	{
		get
		{
			return this.m_Shader;
		}
	}

	// Token: 0x06000234 RID: 564 RVA: 0x00012A1A File Offset: 0x00010C1A
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyMaterial(Material mat)
	{
		if (mat)
		{
			UnityEngine.Object.DestroyImmediate(mat);
			mat = null;
		}
	}

	// Token: 0x06000235 RID: 565 RVA: 0x00012A30 File Offset: 0x00010C30
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateMaterials()
	{
		if (this.m_Shader == null)
		{
			this.m_Shader = Shader.Find("Custom/DeferredNightVisionShader");
		}
		if (this.m_Material == null && this.m_Shader != null && this.m_Shader.isSupported)
		{
			this.m_Material = this.CreateMaterial(this.m_Shader);
		}
	}

	// Token: 0x06000236 RID: 566 RVA: 0x00012A96 File Offset: 0x00010C96
	[PublicizedFrom(EAccessModifier.Private)]
	public Material CreateMaterial(Shader shader)
	{
		if (!shader)
		{
			return null;
		}
		return new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00012AB0 File Offset: 0x00010CB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.DestroyMaterial(this.m_Material);
		this.m_Material = null;
		this.m_Shader = null;
	}

	// Token: 0x06000238 RID: 568 RVA: 0x00012ACC File Offset: 0x00010CCC
	[ContextMenu("UpdateShaderValues")]
	public void UpdateShaderValues()
	{
		if (this.m_Material == null)
		{
			return;
		}
		this.m_Material.SetVector("_NVColor", this.m_NVColor);
		this.m_Material.SetVector("_TargetWhiteColor", this.m_TargetBleachColor);
		this.m_Material.SetFloat("_BaseLightingContribution", this.m_baseLightingContribution);
		this.m_Material.SetFloat("_LightSensitivityMultiplier", this.m_LightSensitivityMultiplier);
		this.m_Material.shaderKeywords = null;
		if (this.useVignetting)
		{
			Shader.EnableKeyword("USE_VIGNETTE");
			return;
		}
		Shader.DisableKeyword("USE_VIGNETTE");
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00012B73 File Offset: 0x00010D73
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.CreateMaterials();
		this.UpdateShaderValues();
	}

	// Token: 0x0600023A RID: 570 RVA: 0x00012B81 File Offset: 0x00010D81
	public void ReloadShaders()
	{
		this.OnDisable();
	}

	// Token: 0x0600023B RID: 571 RVA: 0x00012B89 File Offset: 0x00010D89
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.UpdateShaderValues();
		this.CreateMaterials();
		Graphics.Blit(source, destination, this.m_Material);
	}

	// Token: 0x040002EF RID: 751
	[SerializeField]
	[Tooltip("The main color of the NV effect")]
	public Color m_NVColor = new Color(0f, 1f, 0.1724138f, 0f);

	// Token: 0x040002F0 RID: 752
	[SerializeField]
	[Tooltip("The color that the NV effect will 'bleach' towards (white = default)")]
	public Color m_TargetBleachColor = new Color(1f, 1f, 1f, 0f);

	// Token: 0x040002F1 RID: 753
	[Range(0f, 1f)]
	[Tooltip("How much base lighting does the NV effect pick up")]
	public float m_baseLightingContribution = 0.025f;

	// Token: 0x040002F2 RID: 754
	[Range(0f, 128f)]
	[Tooltip("The higher this value, the more bright areas will get 'bleached out'")]
	public float m_LightSensitivityMultiplier = 100f;

	// Token: 0x040002F3 RID: 755
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material m_Material;

	// Token: 0x040002F4 RID: 756
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Shader m_Shader;

	// Token: 0x040002F5 RID: 757
	[Tooltip("Do we want to apply a vignette to the edges of the screen?")]
	public bool useVignetting = true;
}
