using System;
using System.Collections;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020017B4 RID: 6068
	[PublicizedFrom(EAccessModifier.Internal)]
	public class Grouping<K, T> : IGrouping<K, T>, IEnumerable<!1>, IEnumerable
	{
		// Token: 0x0600BCC7 RID: 48327 RVA: 0x004620D7 File Offset: 0x004602D7
		public Grouping(K key, IEnumerable<T> group)
		{
			this.group = group;
			this.key = key;
		}

		// Token: 0x170016E6 RID: 5862
		// (get) Token: 0x0600BCC8 RID: 48328 RVA: 0x004620ED File Offset: 0x004602ED
		// (set) Token: 0x0600BCC9 RID: 48329 RVA: 0x004620F5 File Offset: 0x004602F5
		public K Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		// Token: 0x0600BCCA RID: 48330 RVA: 0x004620FE File Offset: 0x004602FE
		public IEnumerator<T> GetEnumerator()
		{
			return this.group.GetEnumerator();
		}

		// Token: 0x0600BCCB RID: 48331 RVA: 0x004620FE File Offset: 0x004602FE
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.group.GetEnumerator();
		}

		// Token: 0x04008D8D RID: 36237
		[PublicizedFrom(EAccessModifier.Private)]
		public K key;

		// Token: 0x04008D8E RID: 36238
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerable<T> group;
	}
}
