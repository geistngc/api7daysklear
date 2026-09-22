using System;
using UnityEngine;

namespace JBooth.MicroSplat
{
	// Token: 0x02001D3D RID: 7485
	[ExecuteInEditMode]
	[RequireComponent(typeof(Light))]
	public class GlitterLight : MonoBehaviour
	{
		// Token: 0x0600DDB3 RID: 56755 RVA: 0x004F7F14 File Offset: 0x004F6114
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnEnable()
		{
			this.lght = base.GetComponent<Light>();
		}

		// Token: 0x0600DDB4 RID: 56756 RVA: 0x004F7F14 File Offset: 0x004F6114
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			this.lght = base.GetComponent<Light>();
		}

		// Token: 0x0600DDB5 RID: 56757 RVA: 0x004F7F24 File Offset: 0x004F6124
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			Shader.SetGlobalVector("_gGlitterLightDir", -base.transform.forward);
			Shader.SetGlobalVector("_gGlitterLightWorldPos", base.transform.position);
			if (this.lght != null)
			{
				Shader.SetGlobalColor("_gGlitterLightColor", this.lght.color);
			}
		}

		// Token: 0x0400A7BE RID: 42942
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Light lght;
	}
}
