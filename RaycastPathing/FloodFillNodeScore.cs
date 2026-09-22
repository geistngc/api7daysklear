using System;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020018D3 RID: 6355
	[Preserve]
	public class FloodFillNodeScore
	{
		// Token: 0x1700183C RID: 6204
		// (get) Token: 0x0600C411 RID: 50193 RVA: 0x004879F8 File Offset: 0x00485BF8
		public float F
		{
			get
			{
				return this.G + this.H;
			}
		}

		// Token: 0x04009497 RID: 38039
		public float G;

		// Token: 0x04009498 RID: 38040
		public float H;
	}
}
