using System;
using System.Collections.Generic;

// Token: 0x020013E4 RID: 5092
public class DictionaryLinkedList<T, S>
{
	// Token: 0x06009FCA RID: 40906 RVA: 0x003C4D02 File Offset: 0x003C2F02
	public DictionaryLinkedList()
	{
		this.dict = new Dictionary<T, S>();
	}

	// Token: 0x06009FCB RID: 40907 RVA: 0x003C4D2B File Offset: 0x003C2F2B
	public DictionaryLinkedList(IEqualityComparer<T> _comparer)
	{
		this.dict = new Dictionary<T, S>(_comparer);
	}

	// Token: 0x06009FCC RID: 40908 RVA: 0x003C4D58 File Offset: 0x003C2F58
	public void Add(T _key, S _value)
	{
		this.dict.Add(_key, _value);
		LinkedListNode<S> value = this.list.AddLast(_value);
		this.indices.Add(_key, value);
	}

	// Token: 0x06009FCD RID: 40909 RVA: 0x003C4D8C File Offset: 0x003C2F8C
	public void Set(T _key, S _value)
	{
		if (this.dict.ContainsKey(_key))
		{
			this.Remove(_key);
		}
		this.Add(_key, _value);
	}

	// Token: 0x06009FCE RID: 40910 RVA: 0x003C4DAC File Offset: 0x003C2FAC
	public void Remove(T _key)
	{
		if (this.dict.ContainsKey(_key))
		{
			S s = this.dict[_key];
			this.dict.Remove(_key);
			LinkedListNode<S> node = this.indices[_key];
			this.list.Remove(node);
			this.indices.Remove(_key);
		}
	}

	// Token: 0x06009FCF RID: 40911 RVA: 0x003C4E07 File Offset: 0x003C3007
	public void Clear()
	{
		this.list.Clear();
		this.dict.Clear();
		this.indices.Clear();
	}

	// Token: 0x170012D9 RID: 4825
	// (get) Token: 0x06009FD0 RID: 40912 RVA: 0x003C4E2A File Offset: 0x003C302A
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x04007944 RID: 31044
	public Dictionary<T, S> dict;

	// Token: 0x04007945 RID: 31045
	public LinkedList<S> list = new LinkedList<S>();

	// Token: 0x04007946 RID: 31046
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<T, LinkedListNode<S>> indices = new Dictionary<T, LinkedListNode<S>>();
}
