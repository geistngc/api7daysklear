using System;
using System.Collections.Generic;
using MemoryPack;

// Token: 0x020013EF RID: 5103
public class DataItem<T> : IDataItem
{
	// Token: 0x14000102 RID: 258
	// (add) Token: 0x0600A002 RID: 40962 RVA: 0x003C5430 File Offset: 0x003C3630
	// (remove) Token: 0x0600A003 RID: 40963 RVA: 0x003C5468 File Offset: 0x003C3668
	public event DataItem<T>.OnChangeDelegate OnChangeDelegates;

	// Token: 0x170012E1 RID: 4833
	// (get) Token: 0x0600A004 RID: 40964 RVA: 0x003C549D File Offset: 0x003C369D
	public string Name
	{
		get
		{
			return this.name;
		}
	}

	// Token: 0x0600A005 RID: 40965 RVA: 0x003C54A8 File Offset: 0x003C36A8
	[MemoryPackConstructor]
	public DataItem() : this(default(T))
	{
	}

	// Token: 0x0600A006 RID: 40966 RVA: 0x003C54C4 File Offset: 0x003C36C4
	public DataItem(T _startValue) : this(null, _startValue)
	{
	}

	// Token: 0x0600A007 RID: 40967 RVA: 0x003C54CE File Offset: 0x003C36CE
	public DataItem(string _name, T _startValue)
	{
		this.name = _name;
		this.internalValue = _startValue;
	}

	// Token: 0x170012E2 RID: 4834
	// (get) Token: 0x0600A009 RID: 40969 RVA: 0x003C5514 File Offset: 0x003C3714
	// (set) Token: 0x0600A008 RID: 40968 RVA: 0x003C54E4 File Offset: 0x003C36E4
	public T Value
	{
		get
		{
			return this.internalValue;
		}
		set
		{
			T oldValue = this.internalValue;
			this.internalValue = value;
			if (this.OnChangeDelegates != null)
			{
				this.OnChangeDelegates(oldValue, value);
			}
		}
	}

	// Token: 0x0600A00A RID: 40970 RVA: 0x003C551C File Offset: 0x003C371C
	public override string ToString()
	{
		if (this.Formatter != null)
		{
			return this.Formatter.ToString(this.internalValue);
		}
		if (this.internalValue == null)
		{
			return "null";
		}
		return this.internalValue.ToString();
	}

	// Token: 0x0600A00B RID: 40971 RVA: 0x003C556C File Offset: 0x003C376C
	public static bool operator ==(DataItem<T> v1, T v2)
	{
		if (v1 != null)
		{
			return EqualityComparer<T>.Default.Equals(v1.internalValue, v2);
		}
		return v2 == null;
	}

	// Token: 0x0600A00C RID: 40972 RVA: 0x003C558C File Offset: 0x003C378C
	public static bool operator !=(DataItem<T> v1, T v2)
	{
		if (v1 != null)
		{
			return !EqualityComparer<T>.Default.Equals(v1.internalValue, v2);
		}
		return v2 != null;
	}

	// Token: 0x0600A00D RID: 40973 RVA: 0x003C55AF File Offset: 0x003C37AF
	public override bool Equals(object obj)
	{
		return obj != null && this.internalValue.Equals(obj);
	}

	// Token: 0x0600A00E RID: 40974 RVA: 0x003C55C8 File Offset: 0x003C37C8
	public override int GetHashCode()
	{
		return this.internalValue.GetHashCode();
	}

	// Token: 0x04007952 RID: 31058
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;

	// Token: 0x04007953 RID: 31059
	[PublicizedFrom(EAccessModifier.Private)]
	public T internalValue;

	// Token: 0x04007954 RID: 31060
	public IDataItemFormatter Formatter;

	// Token: 0x020013F0 RID: 5104
	// (Invoke) Token: 0x0600A010 RID: 40976
	public delegate void OnChangeDelegate(T _oldValue, T _newValue);
}
