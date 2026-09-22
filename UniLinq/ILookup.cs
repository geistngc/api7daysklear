using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020017B6 RID: 6070
	public interface ILookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>, IEnumerable
	{
		// Token: 0x170016E8 RID: 5864
		// (get) Token: 0x0600BCCD RID: 48333
		int Count { get; }

		// Token: 0x170016E9 RID: 5865
		IEnumerable<TElement> this[TKey key]
		{
			get;
		}

		// Token: 0x0600BCCF RID: 48335
		bool Contains(TKey key);
	}
}
