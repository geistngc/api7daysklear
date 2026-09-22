using System;
using System.Collections.Generic;

// Token: 0x020013A5 RID: 5029
public sealed class BiDictionary<TKey, TValue>
{
	// Token: 0x06009E94 RID: 40596 RVA: 0x003BFCF4 File Offset: 0x003BDEF4
	public void Add(TKey key, TValue value)
	{
		if (this.m_keyToValue.ContainsKey(key))
		{
			throw new ArgumentException("Key already in dictionary.", "key");
		}
		if (this.m_valueToKey.ContainsKey(value))
		{
			throw new ArgumentException("Value already in dictionary.", "value");
		}
		this.m_keyToValue.Add(key, value);
		this.m_valueToKey.Add(value, key);
	}

	// Token: 0x06009E95 RID: 40597 RVA: 0x003BFD57 File Offset: 0x003BDF57
	public bool ContainsKey(TKey key)
	{
		return this.m_keyToValue.ContainsKey(key);
	}

	// Token: 0x06009E96 RID: 40598 RVA: 0x003BFD65 File Offset: 0x003BDF65
	public bool ContainsValue(TValue value)
	{
		return this.m_valueToKey.ContainsKey(value);
	}

	// Token: 0x06009E97 RID: 40599 RVA: 0x003BFD73 File Offset: 0x003BDF73
	public bool TryGetByKey(TKey key, out TValue value)
	{
		return this.m_keyToValue.TryGetValue(key, out value);
	}

	// Token: 0x06009E98 RID: 40600 RVA: 0x003BFD82 File Offset: 0x003BDF82
	public bool TryGetByValue(TValue value, out TKey key)
	{
		return this.m_valueToKey.TryGetValue(value, out key);
	}

	// Token: 0x06009E99 RID: 40601 RVA: 0x003BFD94 File Offset: 0x003BDF94
	public bool RemoveByKey(TKey key)
	{
		TValue key2;
		if (!this.m_keyToValue.Remove(key, out key2))
		{
			return false;
		}
		this.m_valueToKey.Remove(key2);
		return true;
	}

	// Token: 0x06009E9A RID: 40602 RVA: 0x003BFDC4 File Offset: 0x003BDFC4
	public bool RemoveByValue(TValue value)
	{
		TKey key;
		if (!this.m_valueToKey.Remove(value, out key))
		{
			return false;
		}
		this.m_keyToValue.Remove(key);
		return true;
	}

	// Token: 0x04007879 RID: 30841
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<TKey, TValue> m_keyToValue = new Dictionary<TKey, TValue>();

	// Token: 0x0400787A RID: 30842
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<TValue, TKey> m_valueToKey = new Dictionary<TValue, TKey>();
}
