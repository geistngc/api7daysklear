using System;

namespace XMLData
{
	// Token: 0x0200163A RID: 5690
	public class Range<TValue>
	{
		// Token: 0x0600B29A RID: 45722 RVA: 0x0000640C File Offset: 0x0000460C
		public Range()
		{
		}

		// Token: 0x0600B29B RID: 45723 RVA: 0x0042B52B File Offset: 0x0042972B
		public Range(bool _hasMin, TValue _min, bool _hasMax, TValue _max)
		{
			this.hasMin = _hasMin;
			this.hasMax = _hasMax;
			this.min = _min;
			this.max = _max;
		}

		// Token: 0x0600B29C RID: 45724 RVA: 0x0042B550 File Offset: 0x00429750
		public override string ToString()
		{
			return string.Format("{0}-{1}", this.hasMin ? this.min.ToString() : "*", this.hasMax ? this.max.ToString() : "*");
		}

		// Token: 0x04008678 RID: 34424
		public bool hasMin;

		// Token: 0x04008679 RID: 34425
		public bool hasMax;

		// Token: 0x0400867A RID: 34426
		public TValue min;

		// Token: 0x0400867B RID: 34427
		public TValue max;
	}
}
