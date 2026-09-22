using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D58 RID: 7512
	public struct CharacterGroundingReport
	{
		// Token: 0x0600DDE4 RID: 56804 RVA: 0x004F94EC File Offset: 0x004F76EC
		public void CopyFrom(CharacterTransientGroundingReport transientGroundingReport)
		{
			this.FoundAnyGround = transientGroundingReport.FoundAnyGround;
			this.IsStableOnGround = transientGroundingReport.IsStableOnGround;
			this.SnappingPrevented = transientGroundingReport.SnappingPrevented;
			this.GroundNormal = transientGroundingReport.GroundNormal;
			this.InnerGroundNormal = transientGroundingReport.InnerGroundNormal;
			this.OuterGroundNormal = transientGroundingReport.OuterGroundNormal;
			this.GroundCollider = null;
			this.GroundPoint = Vector3.zero;
		}

		// Token: 0x0400A860 RID: 43104
		public bool FoundAnyGround;

		// Token: 0x0400A861 RID: 43105
		public bool IsStableOnGround;

		// Token: 0x0400A862 RID: 43106
		public bool SnappingPrevented;

		// Token: 0x0400A863 RID: 43107
		public Vector3 GroundNormal;

		// Token: 0x0400A864 RID: 43108
		public Vector3 InnerGroundNormal;

		// Token: 0x0400A865 RID: 43109
		public Vector3 OuterGroundNormal;

		// Token: 0x0400A866 RID: 43110
		public Collider GroundCollider;

		// Token: 0x0400A867 RID: 43111
		public Vector3 GroundPoint;
	}
}
