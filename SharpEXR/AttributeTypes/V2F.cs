using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016ED RID: 5869
	public struct V2F
	{
		// Token: 0x0600B744 RID: 46916 RVA: 0x0044210F File Offset: 0x0044030F
		public V2F(float v0, float v1)
		{
			this.V0 = v0;
			this.V1 = v1;
		}

		// Token: 0x0600B745 RID: 46917 RVA: 0x0044211F File Offset: 0x0044031F
		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}", base.GetType().Name, this.V0, this.V1);
		}

		// Token: 0x1700165A RID: 5722
		// (get) Token: 0x0600B746 RID: 46918 RVA: 0x00442156 File Offset: 0x00440356
		public float X
		{
			get
			{
				return this.V0;
			}
		}

		// Token: 0x1700165B RID: 5723
		// (get) Token: 0x0600B747 RID: 46919 RVA: 0x0044215E File Offset: 0x0044035E
		public float Y
		{
			get
			{
				return this.V1;
			}
		}

		// Token: 0x04008940 RID: 35136
		public float V0;

		// Token: 0x04008941 RID: 35137
		public float V1;
	}
}
