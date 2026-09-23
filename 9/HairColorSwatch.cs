using System;
using UnityEngine;

// Token: 0x02001341 RID: 4929
[CreateAssetMenu(fileName = "HairColorSwatch", menuName = "Hair Color Management/Hair Color Swatch", order = 1)]
public class HairColorSwatch : ScriptableObject
{
	// Token: 0x06009BC6 RID: 39878 RVA: 0x003AD6AC File Offset: 0x003AB8AC
	public void ApplyToMaterial(Material material)
	{
		if (material.GetFloat("_HoldColor") > 0.5f)
		{
			return;
		}
		material.SetColor("_Tint1", this.tint1);
		material.SetColor("_Tint2", this.tint2);
		material.SetColor("_Tint3", this.tint3);
		material.SetFloat("_TintSharpness", this.tintSharpness);
		material.SetFloat("_IDMapStrength", this.idMapStrength);
		material.SetFloat("_RootDarkening", this.rootDarkening);
		material.SetFloat("_Metallic", this.metallic);
		material.SetColor("_CuticleSpecularColor", this.cuticleSpecularColor);
		material.SetColor("_CortexSpecularColor", this.cortexSpecularColor);
		material.SetFloat("_SmoothnessMultiplier", this.smoothnessMultiplier);
		material.SetColor("_SubsurfaceAmbient", this.subsurfaceAmbient);
		material.SetColor("_SubsurfaceColor", this.subsurfaceColor);
	}

	// Token: 0x06009BC7 RID: 39879 RVA: 0x003AD798 File Offset: 0x003AB998
	public void ApplySwatchToGameObject(GameObject targetGameObject)
	{
		Shader y = Shader.Find("Game/SDCS/Hair");
		if (targetGameObject != null)
		{
			foreach (Renderer renderer in targetGameObject.GetComponentsInChildren<Renderer>(true))
			{
				Material[] array;
				if (Application.isPlaying)
				{
					array = renderer.materials;
				}
				else
				{
					array = renderer.sharedMaterials;
				}
				foreach (Material material in array)
				{
					if (material.shader == y)
					{
						this.ApplyToMaterial(material);
					}
				}
			}
			return;
		}
		Debug.LogWarning("No target GameObject selected.");
	}

	// Token: 0x0400757E RID: 30078
	[ColorUsage(false, false)]
	public Color tint1 = Color.red;

	// Token: 0x0400757F RID: 30079
	[ColorUsage(false, false)]
	public Color tint2 = Color.green;

	// Token: 0x04007580 RID: 30080
	[ColorUsage(false, false)]
	public Color tint3 = Color.blue;

	// Token: 0x04007581 RID: 30081
	[Range(0f, 1f)]
	public float tintSharpness = 0.5f;

	// Token: 0x04007582 RID: 30082
	[Range(0f, 1f)]
	public float idMapStrength;

	// Token: 0x04007583 RID: 30083
	[Range(0f, 1f)]
	public float rootDarkening;

	// Token: 0x04007584 RID: 30084
	[Range(0f, 1f)]
	public float metallic;

	// Token: 0x04007585 RID: 30085
	[ColorUsage(false, true)]
	public Color cuticleSpecularColor = Color.white;

	// Token: 0x04007586 RID: 30086
	[ColorUsage(false, true)]
	public Color cortexSpecularColor = Color.white;

	// Token: 0x04007587 RID: 30087
	[Range(0f, 10f)]
	public float smoothnessMultiplier = 1f;

	// Token: 0x04007588 RID: 30088
	[ColorUsage(false, false)]
	public Color subsurfaceAmbient = Color.white;

	// Token: 0x04007589 RID: 30089
	[ColorUsage(false, false)]
	public Color subsurfaceColor = Color.white;
}
