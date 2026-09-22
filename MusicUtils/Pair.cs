using System;

namespace MusicUtils
{
	// Token: 0x02001A1A RID: 6682
	public struct Pair<T1, T2> where T1 : IComparable<T1> where T2 : IComparable<T2>
	{
		// Token: 0x0600CB2A RID: 52010 RVA: 0x004A8B02 File Offset: 0x004A6D02
		public Pair(T1 _item1, T2 _item2)
		{
			this.item1 = _item1;
			this.item2 = _item2;
		}

		// Token: 0x04009AAD RID: 39597
		public T1 item1;

		// Token: 0x04009AAE RID: 39598
		public T2 item2;
	}
}
