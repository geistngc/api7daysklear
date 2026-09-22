using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016E4 RID: 5860
	public struct Box2F
	{
		// Token: 0x0600B732 RID: 46898 RVA: 0x00441D37 File Offset: 0x0043FF37
		public Box2F(float xMin, float yMin, float xMax, float yMax)
		{
			this.XMin = xMin;
			this.YMin = yMin;
			this.XMax = xMax;
			this.YMax = yMax;
		}

		// Token: 0x0600B733 RID: 46899 RVA: 0x00441D58 File Offset: 0x0043FF58
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

		// Token: 0x17001655 RID: 5717
		// (get) Token: 0x0600B734 RID: 46900 RVA: 0x00441DC5 File Offset: 0x0043FFC5
		public float Width
		{
			get
			{
				return this.XMax - this.XMin + 1f;
			}
		}

		// Token: 0x17001656 RID: 5718
		// (get) Token: 0x0600B735 RID: 46901 RVA: 0x00441DDA File Offset: 0x0043FFDA
		public float Height
		{
			get
			{
				return this.YMax - this.YMin + 1f;
			}
		}

		// Token: 0x0400891F RID: 35103
		public readonly float XMin;

		// Token: 0x04008920 RID: 35104
		public readonly float YMin;

		// Token: 0x04008921 RID: 35105
		public readonly float XMax;

		// Token: 0x04008922 RID: 35106
		public readonly float YMax;
	}
}
