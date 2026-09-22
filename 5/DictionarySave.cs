using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

// Token: 0x020013E8 RID: 5096
[MemoryPackable(GenerateType.Object)]
public class DictionarySave<T1, T2> : IMemoryPackable<DictionarySave<T1, T2>>, IMemoryPackFormatterRegister where T2 : class
{
	// Token: 0x06009FE4 RID: 40932 RVA: 0x003C50A4 File Offset: 0x003C32A4
	[MemoryPackConstructor]
	public DictionarySave()
	{
	}

	// Token: 0x170012DD RID: 4829
	public virtual T2 this[T1 _v]
	{
		get
		{
			if (!DictionarySave<T1, T2>.KeyIsValuetype && _v == null)
			{
				return default(T2);
			}
			T2 result;
			if (this.dic.TryGetValue(_v, out result))
			{
				return result;
			}
			return default(T2);
		}
		set
		{
			this.dic[_v] = value;
		}
	}

	// Token: 0x170012DE RID: 4830
	// (get) Token: 0x06009FE7 RID: 40935 RVA: 0x003C5114 File Offset: 0x003C3314
	public Dictionary<T1, T2> Dict
	{
		get
		{
			return this.dic;
		}
	}

	// Token: 0x06009FE8 RID: 40936 RVA: 0x003C511C File Offset: 0x003C331C
	public bool ContainsKey(T1 _key)
	{
		return this.dic.ContainsKey(_key);
	}

	// Token: 0x06009FE9 RID: 40937 RVA: 0x003C512A File Offset: 0x003C332A
	public bool TryGetValue(T1 _key, out T2 _value)
	{
		return this.dic.TryGetValue(_key, out _value);
	}

	// Token: 0x06009FEA RID: 40938 RVA: 0x003C5139 File Offset: 0x003C3339
	public void Add(T1 _key, T2 _value)
	{
		this.dic.Add(_key, _value);
	}

	// Token: 0x06009FEB RID: 40939 RVA: 0x003C5148 File Offset: 0x003C3348
	public void Remove(T1 _key)
	{
		this.dic.Remove(_key);
	}

	// Token: 0x06009FEC RID: 40940 RVA: 0x003C5157 File Offset: 0x003C3357
	public void Clear()
	{
		this.dic.Clear();
	}

	// Token: 0x06009FED RID: 40941 RVA: 0x003C5164 File Offset: 0x003C3364
	public void MarkToRemove(T1 _v)
	{
		this.toRemove.Add(_v);
	}

	// Token: 0x06009FEE RID: 40942 RVA: 0x003C5174 File Offset: 0x003C3374
	public void RemoveAllMarked(DictionarySave<T1, T2>.DictionaryRemoveCallback _callback)
	{
		foreach (T1 o in this.toRemove)
		{
			_callback(o);
		}
		this.toRemove.Clear();
	}

	// Token: 0x170012DF RID: 4831
	// (get) Token: 0x06009FEF RID: 40943 RVA: 0x003C51D4 File Offset: 0x003C33D4
	public int Count
	{
		get
		{
			return this.dic.Count;
		}
	}

	// Token: 0x06009FF0 RID: 40944 RVA: 0x003C51E1 File Offset: 0x003C33E1
	[PublicizedFrom(EAccessModifier.Private)]
	static DictionarySave()
	{
		DictionarySave<T1, T2>.RegisterFormatter();
	}

	// Token: 0x06009FF1 RID: 40945 RVA: 0x003C51FC File Offset: 0x003C33FC
	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DictionarySave<T1, T2>>())
		{
			MemoryPackFormatterProvider.Register<DictionarySave<T1, T2>>(new DictionarySave<T1, T2>.DictionarySaveFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DictionarySave<T1, T2>[]>())
		{
			MemoryPackFormatterProvider.Register<DictionarySave<T1, T2>[]>(new ArrayFormatter<DictionarySave<T1, T2>>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<T1, T2>>())
		{
			MemoryPackFormatterProvider.Register<Dictionary<T1, T2>>(new DictionaryFormatter<T1, T2>());
		}
	}

	// Token: 0x06009FF2 RID: 40946 RVA: 0x003C5234 File Offset: 0x003C3434
	[Preserve]
	public static void Serialize(ref MemoryPackWriter writer, [Nullable(new byte[]
	{
		2,
		1,
		1
	})] ref DictionarySave<T1, T2> value)
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteObjectHeader(3);
		writer.WriteValue<Dictionary<T1, T2>>(value.dic);
		Dictionary<T1, T2> dict = value.Dict;
		writer.WriteValue<Dictionary<T1, T2>>(dict);
		int count = value.Count;
		writer.WriteUnmanaged<int>(count);
	}

	// Token: 0x06009FF3 RID: 40947 RVA: 0x003C5280 File Offset: 0x003C3480
	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, [Nullable(new byte[]
	{
		2,
		1,
		1
	})] ref DictionarySave<T1, T2> value)
	{
		byte b;
		if (!reader.TryReadObjectHeader(out b))
		{
			value = null;
			return;
		}
		Dictionary<T1, T2> dictionary;
		if (b == 3)
		{
			Dictionary<T1, T2> dictionary2;
			int num;
			if (value == null)
			{
				dictionary = reader.ReadValue<Dictionary<T1, T2>>();
				dictionary2 = reader.ReadValue<Dictionary<T1, T2>>();
				reader.ReadUnmanaged<int>(out num);
				goto IL_D1;
			}
			dictionary = value.dic;
			dictionary2 = value.Dict;
			num = value.Count;
			reader.ReadValue<Dictionary<T1, T2>>(ref dictionary);
			reader.ReadValue<Dictionary<T1, T2>>(ref dictionary2);
			reader.ReadUnmanaged<int>(out num);
		}
		else
		{
			if (b > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DictionarySave<T1, T2>), 3, b);
				return;
			}
			Dictionary<T1, T2> dictionary2;
			if (value == null)
			{
				dictionary = null;
				dictionary2 = null;
				int num = 0;
			}
			else
			{
				dictionary = value.dic;
				dictionary2 = value.Dict;
				int num = value.Count;
			}
			if (b != 0)
			{
				reader.ReadValue<Dictionary<T1, T2>>(ref dictionary);
				if (b != 1)
				{
					reader.ReadValue<Dictionary<T1, T2>>(ref dictionary2);
					if (b != 2)
					{
						int num;
						reader.ReadUnmanaged<int>(out num);
					}
				}
			}
			if (value == null)
			{
				goto IL_D1;
			}
		}
		value.dic = dictionary;
		return;
		IL_D1:
		value = new DictionarySave<T1, T2>
		{
			dic = dictionary
		};
	}

	// Token: 0x0400794E RID: 31054
	[MemoryPackInclude]
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<T1, T2> dic = new Dictionary<T1, T2>();

	// Token: 0x0400794F RID: 31055
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T1> toRemove = new List<T1>();

	// Token: 0x04007950 RID: 31056
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly bool KeyIsValuetype = typeof(T1).IsValueType;

	// Token: 0x020013E9 RID: 5097
	// (Invoke) Token: 0x06009FF5 RID: 40949
	public delegate void DictionaryRemoveCallback(T1 _o);

	// Token: 0x020013EA RID: 5098
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1,
		1,
		1
	})]
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class DictionarySaveFormatter : MemoryPackFormatter<DictionarySave<T1, T2>>
	{
		// Token: 0x06009FF8 RID: 40952 RVA: 0x003C536C File Offset: 0x003C356C
		[Preserve]
		public override void Serialize(ref MemoryPackWriter writer, ref DictionarySave<T1, T2> value)
		{
			DictionarySave<T1, T2>.Serialize(ref writer, ref value);
		}

		// Token: 0x06009FF9 RID: 40953 RVA: 0x003C5375 File Offset: 0x003C3575
		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DictionarySave<T1, T2> value)
		{
			DictionarySave<T1, T2>.Deserialize(ref reader, ref value);
		}
	}
}
