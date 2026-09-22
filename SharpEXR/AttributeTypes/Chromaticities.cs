using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016E6 RID: 5862
	public struct Chromaticities
	{
		// Token: 0x0600B73A RID: 46906 RVA: 0x00441E9F File Offset: 0x0044009F
		public Chromaticities(float redX, float redY, float greenX, float greenY, float blueX, float blueY, float whiteX, float whiteY)
		{
			this.RedX = redX;
			this.RedY = redY;
			this.GreenX = greenX;
			this.GreenY = greenY;
			this.BlueX = blueX;
			this.BlueY = blueY;
			this.WhiteX = whiteX;
			this.WhiteY = whiteY;
		}

		// Token: 0x04008927 RID: 35111
		public readonly float RedX;

		// Token: 0x04008928 RID: 35112
		public readonly float RedY;

		// Token: 0x04008929 RID: 35113
		public readonly float GreenX;

		// Token: 0x0400892A RID: 35114
		public readonly float GreenY;

		// Token: 0x0400892B RID: 35115
		public readonly float BlueX;

		// Token: 0x0400892C RID: 35116
		public readonly float BlueY;

		// Token: 0x0400892D RID: 35117
		public readonly float WhiteX;

		// Token: 0x0400892E RID: 35118
		public readonly float WhiteY;
	}
}
