using System;
using System.Collections.Generic;

// Token: 0x020013E6 RID: 5094
public class DictionaryNameId<T>
{
	// Token: 0x06009FD9 RID: 40921 RVA: 0x003C4F45 File Offset: 0x003C3145
	public DictionaryNameId(DictionaryNameIdMapping _mapping)
	{
		this.mapping = _mapping;
	}

	// Token: 0x06009FDA RID: 40922 RVA: 0x003C4F60 File Offset: 0x003C3160
	public void Add(string _name, T _value)
	{
		int key = this.mapping.Add(_name);
		this.idsToValues[key] = _value;
	}

	// Token: 0x170012DB RID: 4827
	// (get) Token: 0x06009FDB RID: 40923 RVA: 0x003C4F87 File Offset: 0x003C3187
	public int Count
	{
		get
		{
			return this.idsToValues.Count;
		}
	}

	// Token: 0x170012DC RID: 4828
	// (get) Token: 0x06009FDC RID: 40924 RVA: 0x003C4F94 File Offset: 0x003C3194
	public Dictionary<int, T> Dict
	{
		get
		{
			return this.idsToValues;
		}
	}

	// Token: 0x06009FDD RID: 40925 RVA: 0x003C4F9C File Offset: 0x003C319C
	public bool Contains(string _name)
	{
		int num = this.mapping.FindId(_name);
		return num != 0 && this.idsToValues.ContainsKey(num);
	}

	// Token: 0x06009FDE RID: 40926 RVA: 0x003C4FC8 File Offset: 0x003C31C8
	public T Get(int _id)
	{
		T result;
		this.idsToValues.TryGetValue(_id, out result);
		return result;
	}

	// Token: 0x06009FDF RID: 40927 RVA: 0x003C4FE8 File Offset: 0x003C31E8
	public T Get(string _name)
	{
		int num = this.mapping.FindId(_name);
		if (num == 0)
		{
			return default(T);
		}
		T result;
		this.idsToValues.TryGetValue(num, out result);
		return result;
	}

	// Token: 0x04007949 RID: 31049
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly DictionaryNameIdMapping mapping;

	// Token: 0x0400794A RID: 31050
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<int, T> idsToValues = new Dictionary<int, T>();
}
