using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x020013D8 RID: 5080
public sealed class DictionaryDebugWrapper<TKey, TValue> : CollectionDebugWrapper<KeyValuePair<TKey, TValue>>, IDictionary<!0, !1>, ICollection<KeyValuePair<!0, !1>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	// Token: 0x06009F91 RID: 40849 RVA: 0x003C421A File Offset: 0x003C241A
	public DictionaryDebugWrapper() : this(new Dictionary<TKey, TValue>())
	{
	}

	// Token: 0x06009F92 RID: 40850 RVA: 0x003C4227 File Offset: 0x003C2427
	public DictionaryDebugWrapper(IDictionary<TKey, TValue> dictionary) : this(null, dictionary)
	{
	}

	// Token: 0x06009F93 RID: 40851 RVA: 0x003C4231 File Offset: 0x003C2431
	public DictionaryDebugWrapper(DebugWrapper parent, IDictionary<TKey, TValue> dictionary) : base(parent, dictionary)
	{
		this.m_dictionary = dictionary;
		this.Keys = new CollectionDebugWrapper<TKey>(this, this.m_dictionary.Keys);
		this.Values = new CollectionDebugWrapper<TValue>(this, this.m_dictionary.Values);
	}

	// Token: 0x06009F94 RID: 40852 RVA: 0x003C4270 File Offset: 0x003C2470
	public void Add(TKey key, TValue value)
	{
		using (base.DebugReadWriteScope())
		{
			this.m_dictionary.Add(key, value);
		}
	}

	// Token: 0x06009F95 RID: 40853 RVA: 0x003C42B0 File Offset: 0x003C24B0
	public bool ContainsKey(TKey key)
	{
		bool result;
		using (base.DebugReadScope())
		{
			result = this.m_dictionary.ContainsKey(key);
		}
		return result;
	}

	// Token: 0x06009F96 RID: 40854 RVA: 0x003C42F0 File Offset: 0x003C24F0
	public bool Remove(TKey key)
	{
		bool result;
		using (base.DebugReadWriteScope())
		{
			result = this.m_dictionary.Remove(key);
		}
		return result;
	}

	// Token: 0x06009F97 RID: 40855 RVA: 0x003C4330 File Offset: 0x003C2530
	public bool TryGetValue(TKey key, out TValue value)
	{
		bool result;
		using (base.DebugReadScope())
		{
			result = this.m_dictionary.TryGetValue(key, out value);
		}
		return result;
	}

	// Token: 0x170012D5 RID: 4821
	public TValue this[TKey key]
	{
		get
		{
			TValue result;
			using (base.DebugReadScope())
			{
				result = this.m_dictionary[key];
			}
			return result;
		}
		set
		{
			using (base.DebugReadWriteScope())
			{
				this.m_dictionary[key] = value;
			}
		}
	}

	// Token: 0x170012D6 RID: 4822
	// (get) Token: 0x06009F9A RID: 40858 RVA: 0x003C43F0 File Offset: 0x003C25F0
	public ICollection<TKey> Keys { get; }

	// Token: 0x170012D7 RID: 4823
	// (get) Token: 0x06009F9B RID: 40859 RVA: 0x003C43F8 File Offset: 0x003C25F8
	public ICollection<TValue> Values { get; }

	// Token: 0x0400792E RID: 31022
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IDictionary<TKey, TValue> m_dictionary;
}
