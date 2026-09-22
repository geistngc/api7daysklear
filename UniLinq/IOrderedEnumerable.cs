using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020017B7 RID: 6071
	public interface IOrderedEnumerable<TElement> : IEnumerable<!0>, IEnumerable
	{
		// Token: 0x0600BCD0 RID: 48336
		IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
	}
}
