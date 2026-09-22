using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016F0 RID: 5872
	public struct V3I
	{
		// Token: 0x0600B751 RID: 46929 RVA: 0x0044224B File Offset: 0x0044044B
		public V3I(int v0, int v1, int v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		// Token: 0x0600B752 RID: 46930 RVA: 0x00442264 File Offset: 0x00440464
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

		// Token: 0x17001661 RID: 5729
		// (get) Token: 0x0600B753 RID: 46931 RVA: 0x004422C3 File Offset: 0x004404C3
		public int X
		{
			get
			{
				return this.V0;
			}
		}

		// Token: 0x17001662 RID: 5730
		// (get) Token: 0x0600B754 RID: 46932 RVA: 0x004422CB File Offset: 0x004404CB
		public int Y
		{
			get
			{
				return this.V1;
			}
		}

		// Token: 0x17001663 RID: 5731
		// (get) Token: 0x0600B755 RID: 46933 RVA: 0x004422D3 File Offset: 0x004404D3
		public int Z
		{
			get
			{
				return this.V2;
			}
		}

		// Token: 0x04008947 RID: 35143
		public int V0;

		// Token: 0x04008948 RID: 35144
		public int V1;

		// Token: 0x04008949 RID: 35145
		public int V2;
	}
}
