using System;
using UnityEngine;

namespace CoverClippingTool
{
	// Token: 0x02001680 RID: 5760
	[Serializable]
	public class BlockSettings
	{
		// Token: 0x0600B481 RID: 46209 RVA: 0x00439DD2 File Offset: 0x00437FD2
		public BlockSettings()
		{
			this.ResetToDefault();
		}

		// Token: 0x0600B482 RID: 46210 RVA: 0x00439DE0 File Offset: 0x00437FE0
		public void ResetToDefault()
		{
			this.shapeModel = null;
			this.savedShapeInfo = null;
		}

		// Token: 0x040087B0 RID: 34736
		public GameObject shapeModel;

		// Token: 0x040087B1 RID: 34737
		public BlockShapeInfo savedShapeInfo;
	}
}
