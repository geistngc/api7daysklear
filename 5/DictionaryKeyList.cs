using System;
using System.Collections.Generic;

// Token: 0x020013E2 RID: 5090
public class DictionaryKeyList<T, S>
{
	// Token: 0x06009FC1 RID: 40897 RVA: 0x003C4BBD File Offset: 0x003C2DBD
	public void Add(T _key, S _value)
	{
		this.dict.Add(_key, _value);
		this.list.Add(_key);
	}

	// Token: 0x06009FC2 RID: 40898 RVA: 0x003C4BD8 File Offset: 0x003C2DD8
	public void Remove(T _key)
	{
		this.list.Remove(_key);
		this.dict.Remove(_key);
	}

	// Token: 0x06009FC3 RID: 40899 RVA: 0x003C4BF4 File Offset: 0x003C2DF4
	public void Replace(T _key, S _value)
	{
		if (this.dict.ContainsKey(_key))
		{
			this.Remove(_key);
		}
		this.Add(_key, _value);
	}

	// Token: 0x06009FC4 RID: 40900 RVA: 0x003C4C13 File Offset: 0x003C2E13
	public void Clear()
	{
		this.list.Clear();
		this.dict.Clear();
	}

	// Token: 0x0400793F RID: 31039
	public Dictionary<T, S> dict = new Dictionary<T, S>();

	// Token: 0x04007940 RID: 31040
	public List<T> list = new List<T>();
}
