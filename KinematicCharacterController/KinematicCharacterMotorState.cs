using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D56 RID: 7510
	[Serializable]
	public struct KinematicCharacterMotorState
	{
		// Token: 0x0400A855 RID: 43093
		public Vector3 Position;

		// Token: 0x0400A856 RID: 43094
		public Quaternion Rotation;

		// Token: 0x0400A857 RID: 43095
		public Vector3 BaseVelocity;

		// Token: 0x0400A858 RID: 43096
		public bool MustUnground;

		// Token: 0x0400A859 RID: 43097
		public float MustUngroundTime;

		// Token: 0x0400A85A RID: 43098
		public bool LastMovementIterationFoundAnyGround;

		// Token: 0x0400A85B RID: 43099
		public CharacterTransientGroundingReport GroundingStatus;

		// Token: 0x0400A85C RID: 43100
		public Rigidbody AttachedRigidbody;

		// Token: 0x0400A85D RID: 43101
		public Vector3 AttachedRigidbodyVelocity;
	}
}
