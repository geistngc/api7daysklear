using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020003E5 RID: 997
[Preserve]
public class DroneRunningLightMan
{
	// Token: 0x06001E41 RID: 7745 RVA: 0x000B80D5 File Offset: 0x000B62D5
	public DroneRunningLightMan()
	{
		DroneRunningLightMan.instance = this;
		this.runningLights = new List<DroneRunningLight>();
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x000B80EE File Offset: 0x000B62EE
	public void AddLight(DroneRunningLight _light)
	{
		this.runningLights.Add(_light);
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x000027FC File Offset: 0x000009FC
	public void QueueLight(DroneRunningLight _light)
	{
	}

	// Token: 0x04001433 RID: 5171
	[PublicizedFrom(EAccessModifier.Private)]
	public List<DroneRunningLight> runningLights;

	// Token: 0x04001434 RID: 5172
	public static DroneRunningLightMan instance;
}
