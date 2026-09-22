using System;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020017BC RID: 6076
	[PublicizedFrom(EAccessModifier.Internal)]
	public class OrderedSequence<TElement, TKey> : OrderedEnumerable<TElement>
	{
		// Token: 0x0600BCEE RID: 48366 RVA: 0x004625DB File Offset: 0x004607DB
		[PublicizedFrom(EAccessModifier.Internal)]
		public OrderedSequence(IEnumerable<TElement> source, Func<TElement, TKey> key_selector, IComparer<TKey> comparer, SortDirection direction) : base(source)
		{
			this.selector = key_selector;
			this.comparer = (comparer ?? Comparer<TKey>.Default);
			this.direction = direction;
		}

		// Token: 0x0600BCEF RID: 48367 RVA: 0x00462603 File Offset: 0x00460803
		[PublicizedFrom(EAccessModifier.Internal)]
		public OrderedSequence(OrderedEnumerable<TElement> parent, IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, SortDirection direction) : this(source, keySelector, comparer, direction)
		{
			this.parent = parent;
		}

		// Token: 0x0600BCF0 RID: 48368 RVA: 0x00462618 File Offset: 0x00460818
		public override IEnumerator<TElement> GetEnumerator()
		{
			return base.GetEnumerator();
		}

		// Token: 0x0600BCF1 RID: 48369 RVA: 0x00462620 File Offset: 0x00460820
		public override SortContext<TElement> CreateContext(SortContext<TElement> current)
		{
			SortContext<TElement> sortContext = new SortSequenceContext<TElement, TKey>(this.selector, this.comparer, this.direction, current);
			if (this.parent != null)
			{
				return this.parent.CreateContext(sortContext);
			}
			return sortContext;
		}

		// Token: 0x0600BCF2 RID: 48370 RVA: 0x0046265C File Offset: 0x0046085C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IEnumerable<TElement> Sort(IEnumerable<TElement> source)
		{
			return QuickSort<TElement>.Sort(source, this.CreateContext(null));
		}

		// Token: 0x04008D9D RID: 36253
		[PublicizedFrom(EAccessModifier.Private)]
		public OrderedEnumerable<TElement> parent;

		// Token: 0x04008D9E RID: 36254
		[PublicizedFrom(EAccessModifier.Private)]
		public Func<TElement, TKey> selector;

		// Token: 0x04008D9F RID: 36255
		[PublicizedFrom(EAccessModifier.Private)]
		public IComparer<TKey> comparer;

		// Token: 0x04008DA0 RID: 36256
		[PublicizedFrom(EAccessModifier.Private)]
		public SortDirection direction;
	}
}
