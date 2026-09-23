using System;
using UnityEngine.Scripting;

// Token: 0x020004F0 RID: 1264
[Preserve]
public class EntityVGyroCopter : EntityDriveable
{
	// Token: 0x0600296E RID: 10606 RVA: 0x000FFB68 File Offset: 0x000FDD68
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateWheelsSteering()
	{
		this.wheels[0].wheelC.steerAngle = this.wheelDir;
	}
}
