using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016EA RID: 5866
	public struct Rational
	{
		// Token: 0x0600B73E RID: 46910 RVA: 0x0044204A File Offset: 0x0044024A
		public Rational(int numerator, uint denominator)
		{
			this.Numerator = numerator;
			this.Denominator = denominator;
		}

		// Token: 0x0600B73F RID: 46911 RVA: 0x0044205A File Offset: 0x0044025A
		public override string ToString()
		{
			return string.Format("{0}/{1}", this.Numerator, this.Denominator);
		}

		// Token: 0x17001659 RID: 5721
		// (get) Token: 0x0600B740 RID: 46912 RVA: 0x0044207C File Offset: 0x0044027C
		public double Value
		{
			get
			{
				return (double)this.Numerator / this.Denominator;
			}
		}

		// Token: 0x04008938 RID: 35128
		public readonly int Numerator;

		// Token: 0x04008939 RID: 35129
		public readonly uint Denominator;
	}
}
