using System;

namespace KinematicCharacterController
{
	// Token: 0x02001D55 RID: 7509
	public enum MovementSweepState
	{
		// Token: 0x0400A851 RID: 43089
		Initial,
		// Token: 0x0400A852 RID: 43090
		AfterFirstHit,
		// Token: 0x0400A853 RID: 43091
		FoundBlockingCrease,
		// Token: 0x0400A854 RID: 43092
		FoundBlockingCorner
	}
}
