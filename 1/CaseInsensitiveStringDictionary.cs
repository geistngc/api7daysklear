using System;
using System.Collections.Generic;

// Token: 0x020013BE RID: 5054
public class CaseInsensitiveStringDictionary<TValue> : Dictionary<string, TValue>
{
	// Token: 0x06009F06 RID: 40710 RVA: 0x003C1BFF File Offset: 0x003BFDFF
	public CaseInsensitiveStringDictionary() : base(StringComparer.OrdinalIgnoreCase)
	{
	}

	// Token: 0x06009F07 RID: 40711 RVA: 0x003C1C0C File Offset: 0x003BFE0C
	public CaseInsensitiveStringDictionary(int _capacity) : base(_capacity, StringComparer.OrdinalIgnoreCase)
	{
	}

	// Token: 0x06009F08 RID: 40712 RVA: 0x003C1C1A File Offset: 0x003BFE1A
	public CaseInsensitiveStringDictionary(IDictionary<string, TValue> _dictionary) : base(_dictionary, StringComparer.OrdinalIgnoreCase)
	{
	}

	// Token: 0x06009F09 RID: 40713 RVA: 0x003C1C28 File Offset: 0x003BFE28
	[Obsolete("CaseInsensitiveStringDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public CaseInsensitiveStringDictionary(IEqualityComparer<string> _comparer) : base(_comparer)
	{
	}

	// Token: 0x06009F0A RID: 40714 RVA: 0x003C1C31 File Offset: 0x003BFE31
	[Obsolete("CaseInsensitiveStringDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public CaseInsensitiveStringDictionary(int _capacity, IEqualityComparer<string> _comparer) : base(_capacity, _comparer)
	{
	}

	// Token: 0x06009F0B RID: 40715 RVA: 0x003C1C3B File Offset: 0x003BFE3B
	[Obsolete("CaseInsensitiveStringDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public CaseInsensitiveStringDictionary(IDictionary<string, TValue> _dictionary, IEqualityComparer<string> _comparer) : base(_dictionary, _comparer)
	{
	}
}
