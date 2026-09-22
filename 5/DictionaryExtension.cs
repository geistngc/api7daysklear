using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020013DF RID: 5087
public static class DictionaryExtension
{
	// Token: 0x06009FB4 RID: 40884 RVA: 0x003C47D4 File Offset: 0x003C29D4
	public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<TValue, bool> predicate)
	{
		foreach (TKey key in (from k in dic.Keys
		where predicate(dic[k])
		select k).ToList<TKey>())
		{
			dic.Remove(key);
		}
	}

	// Token: 0x06009FB5 RID: 40885 RVA: 0x003C485C File Offset: 0x003C2A5C
	public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<TKey, bool> predicate)
	{
		foreach (TKey key in (from k in dic.Keys
		where predicate(k)
		select k).ToList<TKey>())
		{
			dic.Remove(key);
		}
	}

	// Token: 0x06009FB6 RID: 40886 RVA: 0x003C48D4 File Offset: 0x003C2AD4
	public static void CopyTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, IDictionary<TKey, TValue> _dest, bool _overwriteExisting = false)
	{
		if (_overwriteExisting)
		{
			using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = _src.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> keyValuePair = enumerator.Current;
					_dest[keyValuePair.Key] = keyValuePair.Value;
				}
				return;
			}
		}
		foreach (KeyValuePair<TKey, TValue> keyValuePair2 in _src)
		{
			_dest.Add(keyValuePair2.Key, keyValuePair2.Value);
		}
	}

	// Token: 0x06009FB7 RID: 40887 RVA: 0x003C4970 File Offset: 0x003C2B70
	public static void CopyKeysTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, ICollection<TKey> _dest)
	{
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _src)
		{
			_dest.Add(keyValuePair.Key);
		}
	}

	// Token: 0x06009FB8 RID: 40888 RVA: 0x003C49C0 File Offset: 0x003C2BC0
	public static void CopyKeysTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, TKey[] _dest)
	{
		if (_dest.Length != _src.Count)
		{
			throw new ArgumentOutOfRangeException("_dest", "Target array does not have the same size as the dictionary");
		}
		int num = 0;
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _src)
		{
			_dest[num++] = keyValuePair.Key;
		}
	}

	// Token: 0x06009FB9 RID: 40889 RVA: 0x003C4A30 File Offset: 0x003C2C30
	public static void CopyValuesTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, IList<TValue> _dest)
	{
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _src)
		{
			_dest.Add(keyValuePair.Value);
		}
	}

	// Token: 0x06009FBA RID: 40890 RVA: 0x003C4A80 File Offset: 0x003C2C80
	public static void CopyValuesTo<TKey, TValue>(this IDictionary<TKey, TValue> _src, TValue[] _dest)
	{
		if (_dest.Length != _src.Count)
		{
			throw new ArgumentOutOfRangeException("_dest", "Target array does not have the same size as the dictionary");
		}
		int num = 0;
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _src)
		{
			_dest[num++] = keyValuePair.Value;
		}
	}

	// Token: 0x06009FBB RID: 40891 RVA: 0x003C4AF0 File Offset: 0x003C2CF0
	public static bool ValuesEquals<TKey, TValue>(this IDictionary<TKey, TValue> _src, IDictionary<TKey, TValue> _other)
	{
		if (_src.Count != _other.Count)
		{
			return false;
		}
		foreach (KeyValuePair<TKey, TValue> keyValuePair in _src)
		{
			TValue tvalue;
			if (!_other.TryGetValue(keyValuePair.Key, out tvalue))
			{
				return false;
			}
			if (!tvalue.Equals(keyValuePair.Value))
			{
				return false;
			}
		}
		return true;
	}
}
