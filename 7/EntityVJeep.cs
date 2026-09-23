using System;
using UnityEngine.Scripting;

// Token: 0x020004F2 RID: 1266
[Preserve]
public class EntityVJeep : EntityDriveable
{
	// Token: 0x06002976 RID: 10614 RVA: 0x00104A21 File Offset: 0x00102C21
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateWheelsSteering()
	{
		this.wheels[0].wheelC.steerAngle = this.wheelDir;
		this.wheels[1].wheelC.steerAngle = this.wheelDir;
	}
}
