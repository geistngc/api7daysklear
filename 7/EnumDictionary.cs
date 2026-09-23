using System;
using System.Collections.Generic;

// Token: 0x020013F7 RID: 5111
public class EnumDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : struct, IConvertible
{
	// Token: 0x0600A072 RID: 41074 RVA: 0x003C73D9 File Offset: 0x003C55D9
	public EnumDictionary() : base(new FastEnumIntEqualityComparer<TKey>())
	{
	}

	// Token: 0x0600A073 RID: 41075 RVA: 0x003C73E6 File Offset: 0x003C55E6
	public EnumDictionary(int capacity) : base(capacity, new FastEnumIntEqualityComparer<TKey>())
	{
	}

	// Token: 0x0600A074 RID: 41076 RVA: 0x003C73F4 File Offset: 0x003C55F4
	public EnumDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary, new FastEnumIntEqualityComparer<TKey>())
	{
	}

	// Token: 0x0600A075 RID: 41077 RVA: 0x003C7402 File Offset: 0x003C5602
	[Obsolete("EnumDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public EnumDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
	{
	}

	// Token: 0x0600A076 RID: 41078 RVA: 0x003C740B File Offset: 0x003C560B
	[Obsolete("EnumDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public EnumDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
	{
	}

	// Token: 0x0600A077 RID: 41079 RVA: 0x003C7415 File Offset: 0x003C5615
	[Obsolete("EnumDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public EnumDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
	{
	}
}
