using System;
using System.Collections.Generic;

namespace UniLinq
{
	// Token: 0x020017BD RID: 6077
	[PublicizedFrom(EAccessModifier.Internal)]
	public class QuickSort<TElement>
	{
		// Token: 0x0600BCF3 RID: 48371 RVA: 0x0046266C File Offset: 0x0046086C
		[PublicizedFrom(EAccessModifier.Private)]
		public QuickSort(IEnumerable<TElement> source, SortContext<TElement> context)
		{
			List<TElement> list = new List<TElement>();
			foreach (TElement item in source)
			{
				list.Add(item);
			}
			this.elements = list.ToArray();
			this.indexes = QuickSort<TElement>.CreateIndexes(this.elements.Length);
			this.context = context;
		}

		// Token: 0x0600BCF4 RID: 48372 RVA: 0x004626E8 File Offset: 0x004608E8
		[PublicizedFrom(EAccessModifier.Private)]
		public static int[] CreateIndexes(int length)
		{
			int[] array = new int[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = i;
			}
			return array;
		}

		// Token: 0x0600BCF5 RID: 48373 RVA: 0x0046270D File Offset: 0x0046090D
		[PublicizedFrom(EAccessModifier.Private)]
		public void PerformSort()
		{
			if (this.elements.Length <= 1)
			{
				return;
			}
			this.context.Initialize(this.elements);
			Array.Sort<int>(this.indexes, this.context);
		}

		// Token: 0x0600BCF6 RID: 48374 RVA: 0x0046273D File Offset: 0x0046093D
		public static IEnumerable<TElement> Sort(IEnumerable<TElement> source, SortContext<TElement> context)
		{
			QuickSort<TElement> sorter = new QuickSort<TElement>(source, context);
			sorter.PerformSort();
			int num;
			for (int i = 0; i < sorter.elements.Length; i = num + 1)
			{
				yield return sorter.elements[sorter.indexes[i]];
				num = i;
			}
			yield break;
		}

		// Token: 0x04008DA1 RID: 36257
		[PublicizedFrom(EAccessModifier.Private)]
		public TElement[] elements;

		// Token: 0x04008DA2 RID: 36258
		[PublicizedFrom(EAccessModifier.Private)]
		public int[] indexes;

		// Token: 0x04008DA3 RID: 36259
		[PublicizedFrom(EAccessModifier.Private)]
		public SortContext<TElement> context;
	}
}
