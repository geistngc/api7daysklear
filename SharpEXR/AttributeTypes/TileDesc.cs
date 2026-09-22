using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016EB RID: 5867
	public struct TileDesc
	{
		// Token: 0x0600B741 RID: 46913 RVA: 0x00442090 File Offset: 0x00440290
		public TileDesc(uint xSize, uint ySize, byte mode)
		{
			this.XSize = xSize;
			this.YSize = ySize;
			int roundingMode = (mode & 240) >> 4;
			int levelMode = (int)(mode & 15);
			this.RoundingMode = (RoundingMode)roundingMode;
			this.LevelMode = (LevelMode)levelMode;
		}

		// Token: 0x0600B742 RID: 46914 RVA: 0x004420C8 File Offset: 0x004402C8
		public override string ToString()
		{
			return string.Format("{0}: XSize={1}, YSize={2}", base.GetType().Name, this.XSize, this.YSize);
		}

		// Token: 0x0400893A RID: 35130
		public readonly uint XSize;

		// Token: 0x0400893B RID: 35131
		public readonly uint YSize;

		// Token: 0x0400893C RID: 35132
		public readonly LevelMode LevelMode;

		// Token: 0x0400893D RID: 35133
		public readonly RoundingMode RoundingMode;
	}
}
