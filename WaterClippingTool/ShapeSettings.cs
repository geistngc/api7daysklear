using System;
using UnityEngine;

namespace WaterClippingTool
{
	// Token: 0x02001666 RID: 5734
	[Serializable]
	public class ShapeSettings : IEquatable<ShapeSettings>
	{
		// Token: 0x17001592 RID: 5522
		// (get) Token: 0x0600B40E RID: 46094 RVA: 0x00437184 File Offset: 0x00435384
		public bool hasPlane
		{
			get
			{
				return this.plane != WaterClippingPlanePlacer.DisabledPlaneVec;
			}
		}

		// Token: 0x0600B40F RID: 46095 RVA: 0x00437196 File Offset: 0x00435396
		public ShapeSettings()
		{
			this.ResetToDefault();
		}

		// Token: 0x0600B410 RID: 46096 RVA: 0x004371AC File Offset: 0x004353AC
		public void ResetToDefault()
		{
			this.shapeName = string.Empty;
			this.shapeModel = null;
			this.modelOffset = WaterClippingPlanePlacer.DefaultModelOffset;
			this.plane = WaterClippingPlanePlacer.DisabledPlaneVec;
			this.waterFlowMask = BlockFaceFlag.All;
		}

		// Token: 0x0600B411 RID: 46097 RVA: 0x004371DE File Offset: 0x004353DE
		public void CopyFrom(ShapeSettings other)
		{
			this.shapeName = other.shapeName;
			this.shapeModel = other.shapeModel;
			this.modelOffset = other.modelOffset;
			this.plane = other.plane;
			this.waterFlowMask = other.waterFlowMask;
		}

		// Token: 0x0600B412 RID: 46098 RVA: 0x0043721C File Offset: 0x0043541C
		public bool Equals(ShapeSettings other)
		{
			return !(this.plane != other.plane) && !(this.shapeName != other.shapeName) && !(this.shapeModel != other.shapeModel) && !(this.modelOffset != other.modelOffset) && this.waterFlowMask == other.waterFlowMask;
		}

		// Token: 0x0400874C RID: 34636
		public string shapeName;

		// Token: 0x0400874D RID: 34637
		public GameObject shapeModel;

		// Token: 0x0400874E RID: 34638
		public Vector3 modelOffset;

		// Token: 0x0400874F RID: 34639
		public Vector4 plane;

		// Token: 0x04008750 RID: 34640
		public BlockFaceFlag waterFlowMask = BlockFaceFlag.All;
	}
}
