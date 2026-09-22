using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D5E RID: 7518
	[Serializable]
	public struct PhysicsMoverState
	{
		// Token: 0x0400A8E5 RID: 43237
		public Vector3 Position;

		// Token: 0x0400A8E6 RID: 43238
		public Quaternion Rotation;

		// Token: 0x0400A8E7 RID: 43239
		public Vector3 Velocity;

		// Token: 0x0400A8E8 RID: 43240
		public Vector3 AngularVelocity;
	}
}
