using System;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020017BF RID: 6079
	[PublicizedFrom(EAccessModifier.Internal)]
	public abstract class SortContext<TElement> : IComparer<int>
	{
		// Token: 0x0600BCFF RID: 48383 RVA: 0x0046288B File Offset: 0x00460A8B
		[PublicizedFrom(EAccessModifier.Protected)]
		public SortContext(SortDirection direction, SortContext<TElement> child_context)
		{
			this.direction = direction;
			this.child_context = child_context;
		}

		// Token: 0x0600BD00 RID: 48384
		public abstract void Initialize(TElement[] elements);

		// Token: 0x0600BD01 RID: 48385
		public abstract int Compare(int first_index, int second_index);

		// Token: 0x04008DAD RID: 36269
		[PublicizedFrom(EAccessModifier.Protected)]
		public SortDirection direction;

		// Token: 0x04008DAE RID: 36270
		[PublicizedFrom(EAccessModifier.Protected)]
		public SortContext<TElement> child_context;
	}
}
