using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D5A RID: 7514
	public struct HitStabilityReport
	{
		// Token: 0x0400A86E RID: 43118
		public bool IsStable;

		// Token: 0x0400A86F RID: 43119
		public Vector3 InnerNormal;

		// Token: 0x0400A870 RID: 43120
		public Vector3 OuterNormal;

		// Token: 0x0400A871 RID: 43121
		public bool ValidStepDetected;

		// Token: 0x0400A872 RID: 43122
		public Collider SteppedCollider;

		// Token: 0x0400A873 RID: 43123
		public bool LedgeDetected;

		// Token: 0x0400A874 RID: 43124
		public bool IsOnEmptySideOfLedge;

		// Token: 0x0400A875 RID: 43125
		public float DistanceFromLedge;

		// Token: 0x0400A876 RID: 43126
		public bool IsMovingTowardsEmptySideOfLedge;

		// Token: 0x0400A877 RID: 43127
		public Vector3 LedgeGroundNormal;

		// Token: 0x0400A878 RID: 43128
		public Vector3 LedgeRightDirection;

		// Token: 0x0400A879 RID: 43129
		public Vector3 LedgeFacingDirection;
	}
}
