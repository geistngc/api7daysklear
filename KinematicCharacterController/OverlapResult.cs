using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D57 RID: 7511
	public struct OverlapResult
	{
		// Token: 0x0600DDE3 RID: 56803 RVA: 0x004F94DB File Offset: 0x004F76DB
		public OverlapResult(Vector3 normal, Collider collider)
		{
			this.Normal = normal;
			this.Collider = collider;
		}

		// Token: 0x0400A85E RID: 43102
		public Vector3 Normal;

		// Token: 0x0400A85F RID: 43103
		public Collider Collider;
	}
}
