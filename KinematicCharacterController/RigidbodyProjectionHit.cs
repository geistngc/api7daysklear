using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D5B RID: 7515
	public struct RigidbodyProjectionHit
	{
		// Token: 0x0400A87A RID: 43130
		public Rigidbody Rigidbody;

		// Token: 0x0400A87B RID: 43131
		public Vector3 HitPoint;

		// Token: 0x0400A87C RID: 43132
		public Vector3 EffectiveHitNormal;

		// Token: 0x0400A87D RID: 43133
		public Vector3 HitVelocity;

		// Token: 0x0400A87E RID: 43134
		public bool StableOnHit;
	}
}
