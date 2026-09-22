using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016E8 RID: 5864
	public struct M33F
	{
		// Token: 0x0600B73C RID: 46908 RVA: 0x00441F18 File Offset: 0x00440118
		public M33F(float v0, float v1, float v2, float v3, float v4, float v5, float v6, float v7, float v8)
		{
			this.Values = new float[9];
			this.Values[0] = v0;
			this.Values[1] = v1;
			this.Values[2] = v2;
			this.Values[3] = v3;
			this.Values[4] = v4;
			this.Values[5] = v5;
			this.Values[6] = v6;
			this.Values[7] = v7;
			this.Values[8] = v8;
		}

		// Token: 0x04008936 RID: 35126
		public readonly float[] Values;
	}
}
