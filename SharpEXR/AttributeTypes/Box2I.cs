using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016E5 RID: 5861
	public struct Box2I
	{
		// Token: 0x0600B736 RID: 46902 RVA: 0x00441DEF File Offset: 0x0043FFEF
		public Box2I(int xMin, int yMin, int xMax, int yMax)
		{
			this.XMin = xMin;
			this.YMin = yMin;
			this.XMax = xMax;
			this.YMax = yMax;
		}

		// Token: 0x0600B737 RID: 46903 RVA: 0x00441E10 File Offset: 0x00440010
		public override string ToString()
		{
			return string.Format("{0}: ({1}, {2})-({3}, {4})", new object[]
			{
				base.GetType().Name,
				this.XMin,
				this.YMin,
				this.XMax,
				this.YMax
			});
		}

		// Token: 0x17001657 RID: 5719
		// (get) Token: 0x0600B738 RID: 46904 RVA: 0x00441E7D File Offset: 0x0044007D
		public int Width
		{
			get
			{
				return this.XMax - this.XMin + 1;
			}
		}

		// Token: 0x17001658 RID: 5720
		// (get) Token: 0x0600B739 RID: 46905 RVA: 0x00441E8E File Offset: 0x0044008E
		public int Height
		{
			get
			{
				return this.YMax - this.YMin + 1;
			}
		}

		// Token: 0x04008923 RID: 35107
		public readonly int XMin;

		// Token: 0x04008924 RID: 35108
		public readonly int YMin;

		// Token: 0x04008925 RID: 35109
		public readonly int XMax;

		// Token: 0x04008926 RID: 35110
		public readonly int YMax;
	}
}
