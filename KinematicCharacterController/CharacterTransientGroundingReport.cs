using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001D59 RID: 7513
	public struct CharacterTransientGroundingReport
	{
		// Token: 0x0600DDE5 RID: 56805 RVA: 0x004F9554 File Offset: 0x004F7754
		public void CopyFrom(CharacterGroundingReport groundingReport)
		{
			this.FoundAnyGround = groundingReport.FoundAnyGround;
			this.IsStableOnGround = groundingReport.IsStableOnGround;
			this.SnappingPrevented = groundingReport.SnappingPrevented;
			this.GroundNormal = groundingReport.GroundNormal;
			this.InnerGroundNormal = groundingReport.InnerGroundNormal;
			this.OuterGroundNormal = groundingReport.OuterGroundNormal;
		}

		// Token: 0x0400A868 RID: 43112
		public bool FoundAnyGround;

		// Token: 0x0400A869 RID: 43113
		public bool IsStableOnGround;

		// Token: 0x0400A86A RID: 43114
		public bool SnappingPrevented;

		// Token: 0x0400A86B RID: 43115
		public Vector3 GroundNormal;

		// Token: 0x0400A86C RID: 43116
		public Vector3 InnerGroundNormal;

		// Token: 0x0400A86D RID: 43117
		public Vector3 OuterGroundNormal;
	}
}
