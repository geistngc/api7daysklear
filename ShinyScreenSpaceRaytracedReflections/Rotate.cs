using System;
using UnityEngine;

namespace ShinyScreenSpaceRaytracedReflections
{
	// Token: 0x02001624 RID: 5668
	public class Rotate : MonoBehaviour
	{
		// Token: 0x0600B232 RID: 45618 RVA: 0x0042A482 File Offset: 0x00428682
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			base.transform.Rotate(this.axis * (Time.deltaTime * this.speed));
		}

		// Token: 0x04008649 RID: 34377
		public Vector3 axis = Vector3.up;

		// Token: 0x0400864A RID: 34378
		public float speed = 60f;
	}
}
