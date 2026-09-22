using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class CharacterShaderLODControl : MonoBehaviour
{
	// Token: 0x06000100 RID: 256 RVA: 0x0000B9A0 File Offset: 0x00009BA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		this.materials = new List<Material>();
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (Material material in array[i].materials)
			{
				if (material.shader.name.Contains("Game/SDCS/"))
				{
					this.materials.Add(material);
				}
			}
		}
	}

	// Token: 0x06000101 RID: 257 RVA: 0x0000BA10 File Offset: 0x00009C10
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (Camera.main == null)
		{
			return;
		}
		int maximumLOD;
		if (Vector3.Distance(Camera.main.transform.position, base.transform.position) <= this.transitionDistance)
		{
			maximumLOD = 200;
		}
		else
		{
			maximumLOD = 100;
		}
		foreach (Material material in this.materials)
		{
			material.shader.maximumLOD = maximumLOD;
		}
	}

	// Token: 0x04000113 RID: 275
	public float transitionDistance = 5f;

	// Token: 0x04000114 RID: 276
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Material> materials;
}
