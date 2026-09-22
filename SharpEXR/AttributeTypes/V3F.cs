using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016EF RID: 5871
	public struct V3F
	{
		// Token: 0x0600B74C RID: 46924 RVA: 0x004421BD File Offset: 0x004403BD
		public V3F(float v0, float v1, float v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		// Token: 0x0600B74D RID: 46925 RVA: 0x004421D4 File Offset: 0x004403D4
		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}, {3}", new object[]
			{
				base.GetType().Name,
				this.V0,
				this.V1,
				this.V2
			});
		}

		// Token: 0x1700165E RID: 5726
		// (get) Token: 0x0600B74E RID: 46926 RVA: 0x00442233 File Offset: 0x00440433
		public float X
		{
			get
			{
				return this.V0;
			}
		}

		// Token: 0x1700165F RID: 5727
		// (get) Token: 0x0600B74F RID: 46927 RVA: 0x0044223B File Offset: 0x0044043B
		public float Y
		{
			get
			{
				return this.V1;
			}
		}

		// Token: 0x17001660 RID: 5728
		// (get) Token: 0x0600B750 RID: 46928 RVA: 0x00442243 File Offset: 0x00440443
		public float Z
		{
			get
			{
				return this.V2;
			}
		}

		// Token: 0x04008944 RID: 35140
		public float V0;

		// Token: 0x04008945 RID: 35141
		public float V1;

		// Token: 0x04008946 RID: 35142
		public float V2;
	}
}
