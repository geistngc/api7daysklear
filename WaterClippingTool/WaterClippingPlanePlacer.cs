using System;
using UnityEngine;

namespace WaterClippingTool
{
	// Token: 0x02001665 RID: 5733
	public class WaterClippingPlanePlacer : MonoBehaviour
	{
		// Token: 0x0600B40B RID: 46091 RVA: 0x0043710D File Offset: 0x0043530D
		public Plane GetPlane()
		{
			return new Plane(base.transform.forward, base.transform.position);
		}

		// Token: 0x04008748 RID: 34632
		public static readonly Plane DisabledPlane = new Plane(Vector3.up, 1000f);

		// Token: 0x04008749 RID: 34633
		public static readonly Vector4 DisabledPlaneVec = new Vector4(0f, 1f, 0f, 1000f);

		// Token: 0x0400874A RID: 34634
		public static readonly Vector3 DefaultModelOffset = new Vector3(1f, 0f, 1f);

		// Token: 0x0400874B RID: 34635
		public ShapeSettings liveSettings;
	}
}
