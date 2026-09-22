using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020017B5 RID: 6069
	public interface IGrouping<TKey, TElement> : IEnumerable<!1>, IEnumerable
	{
		// Token: 0x170016E7 RID: 5863
		// (get) Token: 0x0600BCCC RID: 48332
		TKey Key { get; }
	}
}
