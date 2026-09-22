using System;
using System.Collections.Generic;

// Token: 0x020013E3 RID: 5091
public class DictionaryKeyValueList<T, S>
{
	// Token: 0x06009FC6 RID: 40902 RVA: 0x003C4C54 File Offset: 0x003C2E54
	public void Add(T _key, S _value)
	{
		this.dict.Add(_key, _value);
		this.keyList.Add(_key);
		this.valueList.Add(_value);
	}

	// Token: 0x06009FC7 RID: 40903 RVA: 0x003C4C7B File Offset: 0x003C2E7B
	public void Set(T _key, S _value)
	{
		if (this.dict.ContainsKey(_key))
		{
			this.Remove(_key);
		}
		this.Add(_key, _value);
	}

	// Token: 0x06009FC8 RID: 40904 RVA: 0x003C4C9C File Offset: 0x003C2E9C
	public void Remove(T _key)
	{
		int num = this.keyList.IndexOf(_key);
		if (num >= 0)
		{
			this.keyList.RemoveAt(num);
			this.valueList.RemoveAt(num);
			this.dict.Remove(_key);
		}
	}

	// Token: 0x06009FC9 RID: 40905 RVA: 0x003C4CDF File Offset: 0x003C2EDF
	public void Clear()
	{
		this.keyList.Clear();
		this.valueList.Clear();
		this.dict.Clear();
	}

	// Token: 0x04007941 RID: 31041
	public Dictionary<T, S> dict = new Dictionary<T, S>();

	// Token: 0x04007942 RID: 31042
	public List<S> valueList = new List<S>();

	// Token: 0x04007943 RID: 31043
	public List<T> keyList = new List<T>();
}
