using System;
using System.Collections.Generic;

// Token: 0x020013E5 RID: 5093
public class DictionaryList<T, S>
{
	// Token: 0x06009FD1 RID: 40913 RVA: 0x003C4E37 File Offset: 0x003C3037
	public DictionaryList()
	{
		this.dict = new Dictionary<T, S>();
	}

	// Token: 0x06009FD2 RID: 40914 RVA: 0x003C4E55 File Offset: 0x003C3055
	public DictionaryList(IEqualityComparer<T> _comparer)
	{
		this.dict = new Dictionary<T, S>(_comparer);
	}

	// Token: 0x06009FD3 RID: 40915 RVA: 0x003C4E74 File Offset: 0x003C3074
	public void Add(T _key, S _value)
	{
		this.dict.Add(_key, _value);
		this.list.Add(_value);
	}

	// Token: 0x06009FD4 RID: 40916 RVA: 0x003C4E8F File Offset: 0x003C308F
	public void Set(T _key, S _value)
	{
		if (this.dict.ContainsKey(_key))
		{
			this.Remove(_key);
		}
		this.Add(_key, _value);
	}

	// Token: 0x06009FD5 RID: 40917 RVA: 0x003C4EB0 File Offset: 0x003C30B0
	public bool Remove(T _key)
	{
		if (this.dict.ContainsKey(_key))
		{
			S item = this.dict[_key];
			this.dict.Remove(_key);
			this.list.Remove(item);
			return true;
		}
		return false;
	}

	// Token: 0x06009FD6 RID: 40918 RVA: 0x003C4EF8 File Offset: 0x003C30F8
	public S Get(T _key)
	{
		S result;
		if (this.dict.TryGetValue(_key, out result))
		{
			return result;
		}
		return default(S);
	}

	// Token: 0x06009FD7 RID: 40919 RVA: 0x003C4F20 File Offset: 0x003C3120
	public void Clear()
	{
		this.list.Clear();
		this.dict.Clear();
	}

	// Token: 0x170012DA RID: 4826
	// (get) Token: 0x06009FD8 RID: 40920 RVA: 0x003C4F38 File Offset: 0x003C3138
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x04007947 RID: 31047
	public Dictionary<T, S> dict;

	// Token: 0x04007948 RID: 31048
	public List<S> list = new List<S>();
}
