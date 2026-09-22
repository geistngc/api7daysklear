using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016EE RID: 5870
	public struct V2I
	{
		// Token: 0x0600B748 RID: 46920 RVA: 0x00442166 File Offset: 0x00440366
		public V2I(int v0, int v1)
		{
			this.V0 = v0;
			this.V1 = v1;
		}

		// Token: 0x0600B749 RID: 46921 RVA: 0x00442176 File Offset: 0x00440376
		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}", base.GetType().Name, this.V0, this.V1);
		}

		// Token: 0x1700165C RID: 5724
		// (get) Token: 0x0600B74A RID: 46922 RVA: 0x004421AD File Offset: 0x004403AD
		public int X
		{
			get
			{
				return this.V0;
			}
		}

		// Token: 0x1700165D RID: 5725
		// (get) Token: 0x0600B74B RID: 46923 RVA: 0x004421B5 File Offset: 0x004403B5
		public int Y
		{
			get
			{
				return this.V1;
			}
		}

		// Token: 0x04008942 RID: 35138
		public int V0;

		// Token: 0x04008943 RID: 35139
		public int V1;
	}
}
