using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// Token: 0x02000721 RID: 1825
public readonly struct FastTags<TTagGroup> where TTagGroup : TagGroup.TagsGroupAbs, new()
{
	// Token: 0x1700056D RID: 1389
	// (get) Token: 0x06003724 RID: 14116 RVA: 0x0016C85C File Offset: 0x0016AA5C
	public static FastTags<TTagGroup> all
	{
		get
		{
			return FastTags<TTagGroup>.allInternal;
		}
	}

	// Token: 0x06003725 RID: 14117 RVA: 0x0016C863 File Offset: 0x0016AA63
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags(ulong[] _bits)
	{
		this.singleBit = 0;
		this.bits = _bits;
	}

	// Token: 0x06003726 RID: 14118 RVA: 0x0016C873 File Offset: 0x0016AA73
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags(int _singleBit)
	{
		this.singleBit = _singleBit;
		this.bits = null;
	}

	// Token: 0x06003727 RID: 14119 RVA: 0x0016C884 File Offset: 0x0016AA84
	public FastTags(FastTags<TTagGroup> _ft)
	{
		if (_ft.bits != null)
		{
			this.bits = new ulong[_ft.bits.Length];
			_ft.bits.CopyTo(this.bits, 0);
		}
		else
		{
			this.bits = null;
		}
		this.singleBit = _ft.singleBit;
	}

	// Token: 0x06003728 RID: 14120 RVA: 0x0016C8D4 File Offset: 0x0016AAD4
	public static FastTags<TTagGroup> Parse(string _str)
	{
		FastTags<TTagGroup> result;
		if (_str.IndexOf(',') < 0)
		{
			result = FastTags<TTagGroup>.GetTag(_str);
			return result;
		}
		List<ulong> obj = FastTags<TTagGroup>.maskList;
		lock (obj)
		{
			string[] array = _str.Split(FastTags<TTagGroup>.tagSeparator);
			for (int i = 0; i < array.Length; i++)
			{
				int bit = FastTags<TTagGroup>.GetBit(array[i]);
				int num = bit >> 6;
				while (FastTags<TTagGroup>.maskList.Count <= num)
				{
					FastTags<TTagGroup>.maskList.Add(0UL);
				}
				List<ulong> list = FastTags<TTagGroup>.maskList;
				int index = num;
				list[index] |= 1UL << bit;
			}
			ulong[] array2 = (FastTags<TTagGroup>.maskList.Count > 0) ? FastTags<TTagGroup>.maskList.ToArray() : null;
			result = new FastTags<TTagGroup>(array2);
			FastTags<TTagGroup>.maskList.Clear();
		}
		return result;
	}

	// Token: 0x06003729 RID: 14121 RVA: 0x0016C9C4 File Offset: 0x0016ABC4
	public static int GetBit(string _tag)
	{
		_tag = _tag.Trim();
		int num;
		if (FastTags<TTagGroup>.tags.TryGetValue(_tag, out num))
		{
			return num;
		}
		num = Interlocked.Increment(ref FastTags<TTagGroup>.next);
		FastTags<TTagGroup>.tags.Add(_tag, num);
		FastTags<TTagGroup>.bitTags.Add(num, _tag);
		int num2 = (num >> 6) + 1;
		if (num2 > FastTags<TTagGroup>.allInternal.bits.Length)
		{
			ulong[] array = new ulong[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = ulong.MaxValue;
			}
			FastTags<TTagGroup>.allInternal = new FastTags<TTagGroup>(array);
		}
		return num;
	}

	// Token: 0x0600372A RID: 14122 RVA: 0x0016CA48 File Offset: 0x0016AC48
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetBit(int _bit, ulong[] _extended)
	{
		int num = _bit >> 6;
		_extended[num] |= 1UL << _bit;
	}

	// Token: 0x0600372B RID: 14123 RVA: 0x0016CA6B File Offset: 0x0016AC6B
	public static FastTags<TTagGroup> GetTag(string _tag)
	{
		return new FastTags<TTagGroup>(FastTags<TTagGroup>.GetBit(_tag));
	}

	// Token: 0x0600372C RID: 14124 RVA: 0x0016CA78 File Offset: 0x0016AC78
	public static FastTags<TTagGroup> CombineTags(FastTags<TTagGroup> _tags1, FastTags<TTagGroup> _tags2)
	{
		return _tags1 | _tags2;
	}

	// Token: 0x0600372D RID: 14125 RVA: 0x0016CA84 File Offset: 0x0016AC84
	public static FastTags<TTagGroup> CombineTags(FastTags<TTagGroup> _tags1, FastTags<TTagGroup> _tags2, FastTags<TTagGroup> _tags3)
	{
		ulong[] array = _tags1.bits;
		int num = (array != null) ? array.Length : 0;
		ulong[] array2 = _tags2.bits;
		int num2 = (array2 != null) ? array2.Length : 0;
		ulong[] array3 = _tags3.bits;
		int num3 = (array3 != null) ? array3.Length : 0;
		int num4 = (_tags1.singleBit > 0) ? (_tags1.singleBit >> 6) : -1;
		int num5 = (_tags2.singleBit > 0) ? (_tags2.singleBit >> 6) : -1;
		int num6 = (_tags3.singleBit > 0) ? (_tags3.singleBit >> 6) : -1;
		int num7 = MathUtils.Max(num, num2, num3);
		num7 = MathUtils.Max(num7, num4 + 1, num5 + 1, num6 + 1);
		ulong[] array4 = (num7 > 0) ? new ulong[num7] : null;
		if (num4 >= 0)
		{
			array4[num4] |= 1UL << _tags1.singleBit;
		}
		if (num5 >= 0)
		{
			array4[num5] |= 1UL << _tags2.singleBit;
		}
		if (num6 >= 0)
		{
			array4[num6] |= 1UL << _tags3.singleBit;
		}
		for (int i = 0; i < num7; i++)
		{
			if (i < num)
			{
				array4[i] |= _tags1.bits[i];
			}
			if (i < num2)
			{
				array4[i] |= _tags2.bits[i];
			}
			if (i < num3)
			{
				array4[i] |= _tags3.bits[i];
			}
		}
		return new FastTags<TTagGroup>(array4);
	}

	// Token: 0x0600372E RID: 14126 RVA: 0x0016CBF4 File Offset: 0x0016ADF4
	public static FastTags<TTagGroup> CombineTags(FastTags<TTagGroup> _tags1, FastTags<TTagGroup> _tags2, FastTags<TTagGroup> _tags3, FastTags<TTagGroup> _tags4)
	{
		ulong[] array = _tags1.bits;
		int num = (array != null) ? array.Length : 0;
		ulong[] array2 = _tags2.bits;
		int num2 = (array2 != null) ? array2.Length : 0;
		ulong[] array3 = _tags3.bits;
		int num3 = (array3 != null) ? array3.Length : 0;
		ulong[] array4 = _tags4.bits;
		int num4 = (array4 != null) ? array4.Length : 0;
		int num5 = (_tags1.singleBit > 0) ? (_tags1.singleBit >> 6) : -1;
		int num6 = (_tags2.singleBit > 0) ? (_tags2.singleBit >> 6) : -1;
		int num7 = (_tags3.singleBit > 0) ? (_tags3.singleBit >> 6) : -1;
		int num8 = (_tags4.singleBit > 0) ? (_tags4.singleBit >> 6) : -1;
		int num9 = MathUtils.Max(num, num2, num3, num4);
		num9 = MathUtils.Max(num9, num5 + 1, num6 + 1, num7 + 1);
		num9 = MathUtils.Max(num9, num8 + 1);
		ulong[] array5 = (num9 > 0) ? new ulong[num9] : null;
		if (num5 >= 0)
		{
			array5[num5] |= 1UL << _tags1.singleBit;
		}
		if (num6 >= 0)
		{
			array5[num6] |= 1UL << _tags2.singleBit;
		}
		if (num7 >= 0)
		{
			array5[num7] |= 1UL << _tags3.singleBit;
		}
		if (num8 >= 0)
		{
			array5[num8] |= 1UL << _tags4.singleBit;
		}
		for (int i = 0; i < num9; i++)
		{
			if (i < num)
			{
				array5[i] |= _tags1.bits[i];
			}
			if (i < num2)
			{
				array5[i] |= _tags2.bits[i];
			}
			if (i < num3)
			{
				array5[i] |= _tags3.bits[i];
			}
			if (i < num4)
			{
				array5[i] |= _tags4.bits[i];
			}
		}
		return new FastTags<TTagGroup>(array5);
	}

	// Token: 0x0600372F RID: 14127 RVA: 0x0016CDD4 File Offset: 0x0016AFD4
	public static void CombineTags(FastTags<TTagGroup> _tags1, FastTags<TTagGroup> _tags2, FastTags<TTagGroup> _tags3, FastTags<TTagGroup> _tags4, ref FastTags<TTagGroup> _outTag)
	{
		ulong[] array = _tags1.bits;
		int num = (array != null) ? array.Length : 0;
		ulong[] array2 = _tags2.bits;
		int num2 = (array2 != null) ? array2.Length : 0;
		ulong[] array3 = _tags3.bits;
		int num3 = (array3 != null) ? array3.Length : 0;
		ulong[] array4 = _tags4.bits;
		int num4 = (array4 != null) ? array4.Length : 0;
		int num5 = (_tags1.singleBit > 0) ? (_tags1.singleBit >> 6) : -1;
		int num6 = (_tags2.singleBit > 0) ? (_tags2.singleBit >> 6) : -1;
		int num7 = (_tags3.singleBit > 0) ? (_tags3.singleBit >> 6) : -1;
		int num8 = (_tags4.singleBit > 0) ? (_tags4.singleBit >> 6) : -1;
		int num9 = MathUtils.Max(num, num2, num3, num4);
		num9 = MathUtils.Max(num9, num5 + 1, num6 + 1, num7 + 1);
		num9 = MathUtils.Max(num9, num8 + 1);
		ulong[] array5;
		if (_outTag.bits != null && _outTag.bits.Length == num9)
		{
			array5 = _outTag.bits;
			for (int i = 0; i < num9; i++)
			{
				array5[i] = 0UL;
			}
		}
		else
		{
			array5 = ((num9 > 0) ? new ulong[num9] : null);
		}
		if (num5 >= 0)
		{
			array5[num5] |= 1UL << _tags1.singleBit;
		}
		if (num6 >= 0)
		{
			array5[num6] |= 1UL << _tags2.singleBit;
		}
		if (num7 >= 0)
		{
			array5[num7] |= 1UL << _tags3.singleBit;
		}
		if (num8 >= 0)
		{
			array5[num8] |= 1UL << _tags4.singleBit;
		}
		for (int j = 0; j < num9; j++)
		{
			if (j < num)
			{
				array5[j] |= _tags1.bits[j];
			}
			if (j < num2)
			{
				array5[j] |= _tags2.bits[j];
			}
			if (j < num3)
			{
				array5[j] |= _tags3.bits[j];
			}
			if (j < num4)
			{
				array5[j] |= _tags4.bits[j];
			}
		}
		_outTag = new FastTags<TTagGroup>(array5);
	}

	// Token: 0x06003730 RID: 14128 RVA: 0x0016CFF4 File Offset: 0x0016B1F4
	public List<string> GetTagNames()
	{
		List<string> list = new List<string>();
		if (this.singleBit > 0)
		{
			string item;
			if (FastTags<TTagGroup>.bitTags.TryGetValue(this.singleBit, out item))
			{
				list.Add(item);
			}
			return list;
		}
		if (this.bits == null)
		{
			return list;
		}
		int num = 0;
		foreach (ulong num2 in this.bits)
		{
			for (int j = 0; j < 64; j++)
			{
				string item2;
				if ((num2 & 1UL << j) != 0UL && FastTags<TTagGroup>.bitTags.TryGetValue(num + j, out item2))
				{
					list.Add(item2);
				}
			}
			num += 64;
		}
		return list;
	}

	// Token: 0x06003731 RID: 14129 RVA: 0x0016D098 File Offset: 0x0016B298
	public bool Test_AnySet(FastTags<TTagGroup> _other)
	{
		if (_other.IsEmpty)
		{
			return this.IsEmpty;
		}
		if (_other.singleBit > 0)
		{
			return this.Test_Bit(_other.singleBit);
		}
		if (this.singleBit > 0)
		{
			return _other.Test_Bit(this.singleBit);
		}
		ulong[] array = this.bits;
		int a = (array != null) ? array.Length : 0;
		ulong[] array2 = _other.bits;
		int num = Mathf.Min(a, (array2 != null) ? array2.Length : 0);
		for (int i = 0; i < num; i++)
		{
			if ((this.bits[i] & _other.bits[i]) != 0UL)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003732 RID: 14130 RVA: 0x0016D128 File Offset: 0x0016B328
	public bool Test_AllSet(FastTags<TTagGroup> _other)
	{
		if (_other.singleBit > 0)
		{
			if (this.singleBit > 0)
			{
				return this.singleBit == _other.singleBit;
			}
			return this.Test_Bit(_other.singleBit);
		}
		else
		{
			if (this.singleBit > 0)
			{
				return _other.Test_IsOnlyBit(this.singleBit);
			}
			ulong[] array = this.bits;
			int num = (array != null) ? array.Length : 0;
			ulong[] array2 = _other.bits;
			int num2 = (array2 != null) ? array2.Length : 0;
			int num3 = Mathf.Min(num, num2);
			for (int i = 0; i < num3; i++)
			{
				ulong num4 = _other.bits[i];
				if ((this.bits[i] & num4) != num4)
				{
					return false;
				}
			}
			if (num2 > num)
			{
				for (int j = num; j < num2; j++)
				{
					if (_other.bits[j] != 0UL)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	// Token: 0x06003733 RID: 14131 RVA: 0x0016D1EC File Offset: 0x0016B3EC
	public bool Test_Bit(int _bitNum)
	{
		if (this.IsEmpty)
		{
			return false;
		}
		if (this.singleBit > 0)
		{
			return _bitNum == this.singleBit;
		}
		if (this.bits == null)
		{
			return false;
		}
		int num = _bitNum >> 6;
		return num < this.bits.Length && (this.bits[num] & 1UL << _bitNum) > 0UL;
	}

	// Token: 0x06003734 RID: 14132 RVA: 0x0016D248 File Offset: 0x0016B448
	[PublicizedFrom(EAccessModifier.Private)]
	public bool Test_IsOnlyBit(int _bitNum)
	{
		if (this.IsEmpty)
		{
			return false;
		}
		if (this.singleBit > 0)
		{
			return this.singleBit == _bitNum;
		}
		if (this.bits == null)
		{
			return false;
		}
		int num = _bitNum >> 6;
		ulong num2 = 1UL << _bitNum;
		if (num >= this.bits.Length)
		{
			return false;
		}
		if (this.bits[num] != num2)
		{
			return false;
		}
		for (int i = 0; i < this.bits.Length; i++)
		{
			if (i != num && this.bits[i] != 0UL)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003735 RID: 14133 RVA: 0x0016D2C8 File Offset: 0x0016B4C8
	public bool Equals(FastTags<TTagGroup> _other)
	{
		if (this.singleBit > 0 && _other.singleBit > 0)
		{
			return this.singleBit == _other.singleBit;
		}
		if (this.singleBit > 0)
		{
			return _other.Equals(this);
		}
		if (_other.singleBit > 0)
		{
			return this.Test_IsOnlyBit(_other.singleBit);
		}
		ulong[] array = this.bits;
		int num = (array != null) ? array.Length : 0;
		ulong[] array2 = _other.bits;
		int num2 = (array2 != null) ? array2.Length : 0;
		int num3 = Mathf.Min(num, num2);
		for (int i = 0; i < num3; i++)
		{
			if (this.bits[i] != _other.bits[i])
			{
				return false;
			}
		}
		if (num > num3)
		{
			for (int j = num3; j < num; j++)
			{
				if (this.bits[j] != 0UL)
				{
					return false;
				}
			}
		}
		else if (num2 > num3)
		{
			for (int k = num3; k < num2; k++)
			{
				if (_other.bits[k] != 0UL)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06003736 RID: 14134 RVA: 0x0016D3B0 File Offset: 0x0016B5B0
	public FastTags<TTagGroup> Remove(FastTags<TTagGroup> _tagsToRemove)
	{
		if (_tagsToRemove.singleBit > 0)
		{
			if (!this.Test_Bit(_tagsToRemove.singleBit))
			{
				return this;
			}
			if (this.Test_IsOnlyBit(_tagsToRemove.singleBit))
			{
				return FastTags<TTagGroup>.none;
			}
		}
		if (this.singleBit <= 0)
		{
			ulong[] array = this.bits;
			int num = (array != null) ? array.Length : 0;
			ulong[] array2 = null;
			if (num > 0)
			{
				array2 = new ulong[num];
				ulong[] array3 = _tagsToRemove.bits;
				int num2 = (array3 != null) ? array3.Length : 0;
				int num3 = (_tagsToRemove.singleBit > 0) ? (_tagsToRemove.singleBit >> 6) : -1;
				ulong num4 = (_tagsToRemove.singleBit > 0) ? (1UL << _tagsToRemove.singleBit) : 0UL;
				for (int i = 0; i < num; i++)
				{
					array2[i] = ((i < num2) ? (this.bits[i] & ~_tagsToRemove.bits[i]) : this.bits[i]);
					if (num3 >= 0 && i == num3)
					{
						array2[i] &= ~num4;
					}
				}
			}
			return new FastTags<TTagGroup>(array2);
		}
		if (_tagsToRemove.Test_Bit(this.singleBit))
		{
			return FastTags<TTagGroup>.none;
		}
		return this;
	}

	// Token: 0x06003737 RID: 14135 RVA: 0x0016D4CC File Offset: 0x0016B6CC
	public static FastTags<TTagGroup>operator |(FastTags<TTagGroup> _a, FastTags<TTagGroup> _b)
	{
		if (_b.singleBit > 0)
		{
			if (_a.Test_Bit(_b.singleBit))
			{
				return _a;
			}
			int b = (_b.singleBit >> 6) + 1;
			int a;
			if (_a.singleBit > 0)
			{
				a = (_a.singleBit >> 6) + 1;
			}
			else
			{
				ulong[] array = _a.bits;
				a = ((array != null) ? array.Length : 0);
			}
			int num = Mathf.Max(a, b);
			ulong[] array2 = (num > 0) ? new ulong[num] : null;
			if (_a.singleBit > 0)
			{
				FastTags<TTagGroup>.SetBit(_a.singleBit, array2);
			}
			else if (_a.bits != null)
			{
				for (int i = 0; i < _a.bits.Length; i++)
				{
					array2[i] = _a.bits[i];
				}
			}
			FastTags<TTagGroup>.SetBit(_b.singleBit, array2);
			return new FastTags<TTagGroup>(array2);
		}
		else
		{
			if (_a.singleBit > 0)
			{
				return _b | _a;
			}
			ulong[] array3 = _a.bits;
			int num2 = (array3 != null) ? array3.Length : 0;
			ulong[] array4 = _b.bits;
			int num3 = (array4 != null) ? array4.Length : 0;
			int num4 = Mathf.Min(num2, num3);
			int num5 = Mathf.Max(num2, num3);
			ulong[] array5 = (num5 > 0) ? new ulong[num5] : null;
			for (int j = 0; j < num4; j++)
			{
				array5[j] = (_a.bits[j] | _b.bits[j]);
			}
			if (num2 > num4)
			{
				for (int k = num4; k < num2; k++)
				{
					array5[k] = _a.bits[k];
				}
			}
			else if (num3 > num4)
			{
				for (int l = num4; l < num3; l++)
				{
					array5[l] = _b.bits[l];
				}
			}
			return new FastTags<TTagGroup>(array5);
		}
	}

	// Token: 0x06003738 RID: 14136 RVA: 0x0016D66C File Offset: 0x0016B86C
	public static FastTags<TTagGroup>operator &(FastTags<TTagGroup> _a, FastTags<TTagGroup> _b)
	{
		if (_b.singleBit > 0)
		{
			if (_a.Test_Bit(_b.singleBit))
			{
				return _b;
			}
			return FastTags<TTagGroup>.none;
		}
		else
		{
			if (_a.singleBit > 0)
			{
				return _b & _a;
			}
			ulong[] array = _a.bits;
			int a = (array != null) ? array.Length : 0;
			ulong[] array2 = _b.bits;
			int b = (array2 != null) ? array2.Length : 0;
			int num = Mathf.Min(a, b);
			ulong[] array3 = null;
			for (int i = num - 1; i >= 0; i--)
			{
				ulong num2 = _a.bits[i] & _b.bits[i];
				if (num2 != 0UL)
				{
					if (array3 == null)
					{
						array3 = new ulong[i + 1];
					}
					array3[i] = num2;
				}
			}
			return new FastTags<TTagGroup>(array3);
		}
	}

	// Token: 0x1700056E RID: 1390
	// (get) Token: 0x06003739 RID: 14137 RVA: 0x0016D70C File Offset: 0x0016B90C
	public bool IsEmpty
	{
		get
		{
			if (this.singleBit > 0)
			{
				return false;
			}
			if (this.bits == null)
			{
				return true;
			}
			ulong[] array = this.bits;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != 0UL)
				{
					return false;
				}
			}
			return true;
		}
	}

	// Token: 0x0600373A RID: 14138 RVA: 0x0016D74C File Offset: 0x0016B94C
	public override string ToString()
	{
		string text = string.Empty;
		List<string> tagNames = this.GetTagNames();
		for (int i = 0; i < tagNames.Count; i++)
		{
			text += tagNames[i];
			if (i < tagNames.Count - 1)
			{
				text += ", ";
			}
		}
		return text;
	}

	// Token: 0x04002CAB RID: 11435
	public static readonly FastTags<TTagGroup> none = new FastTags<TTagGroup>(null);

	// Token: 0x04002CAC RID: 11436
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TTagGroup> allInternal = new FastTags<TTagGroup>(new ulong[]
	{
		ulong.MaxValue,
		ulong.MaxValue,
		ulong.MaxValue,
		ulong.MaxValue,
		ulong.MaxValue,
		ulong.MaxValue
	});

	// Token: 0x04002CAD RID: 11437
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cNumBitsPerField = 64;

	// Token: 0x04002CAE RID: 11438
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFieldShift = 6;

	// Token: 0x04002CAF RID: 11439
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly CaseInsensitiveStringDictionary<int> tags = new CaseInsensitiveStringDictionary<int>();

	// Token: 0x04002CB0 RID: 11440
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<int, string> bitTags = new Dictionary<int, string>();

	// Token: 0x04002CB1 RID: 11441
	[PublicizedFrom(EAccessModifier.Private)]
	public static int next;

	// Token: 0x04002CB2 RID: 11442
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int singleBit;

	// Token: 0x04002CB3 RID: 11443
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ulong[] bits;

	// Token: 0x04002CB4 RID: 11444
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<ulong> maskList = new List<ulong>();

	// Token: 0x04002CB5 RID: 11445
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] tagSeparator = new char[]
	{
		','
	};
}
