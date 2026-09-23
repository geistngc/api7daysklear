using System;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class bleedingScale : MonoBehaviour
{
	// Token: 0x06000020 RID: 32 RVA: 0x00002BEC File Offset: 0x00000DEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		float x = this.parentObject.transform.lossyScale.x;
		base.gameObject.GetComponent<ParticleSystem>().main.startSize = new ParticleSystem.MinMaxCurve(this.minParticleScale * x, this.maxParticleScale * x)
		{
			mode = ParticleSystemCurveMode.TwoConstants
		};
	}

	// Token: 0x06000021 RID: 33 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
	}

	// Token: 0x04000031 RID: 49
	public GameObject parentObject;

	// Token: 0x04000032 RID: 50
	public float minParticleScale;

	// Token: 0x04000033 RID: 51
	public float maxParticleScale;
}
