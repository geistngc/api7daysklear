using System;
using UnityEngine;

// Token: 0x02000022 RID: 34
[ExecuteInEditMode]
public class DangerRoomEnvironmentSim : MonoBehaviour
{
	// Token: 0x06000103 RID: 259 RVA: 0x0000BABC File Offset: 0x00009CBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.simulateWind)
		{
			Shader.SetGlobalVector("_Wind", new Vector4(this.wind, 0f, 0f, 0f));
			return;
		}
		Shader.SetGlobalVector("_Wind", new Vector4(0f, 0f, 0f, 0f));
	}

	// Token: 0x04000115 RID: 277
	public bool simulateWind = true;

	// Token: 0x04000116 RID: 278
	[Range(0f, 100f)]
	public float wind = 100f;
}
