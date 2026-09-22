using System;
using System.Collections.Generic;

// Token: 0x020013BA RID: 5050
public class CachedStringFormatter<T1, T2, T3, T4, T5>
{
	// Token: 0x06009EFD RID: 40701 RVA: 0x003C1A40 File Offset: 0x003BFC40
	public CachedStringFormatter(global::Func<T1, T2, T3, T4, T5, string> _formatterFunc)
	{
		this.formatter = _formatterFunc;
	}

	// Token: 0x06009EFE RID: 40702 RVA: 0x003C1A94 File Offset: 0x003BFC94
	public string Format(T1 _v1, T2 _v2, T3 _v3, T4 _v4, T5 _v5)
	{
		bool flag;
		return this.Format(_v1, _v2, _v3, _v4, _v5, out flag);
	}

	// Token: 0x06009EFF RID: 40703 RVA: 0x003C1AB0 File Offset: 0x003BFCB0
	public string Format(T1 _v1, T2 _v2, T3 _v3, T4 _v4, T5 _v5, out bool _valueChanged)
	{
		_valueChanged = (this.cachedResult == null);
		if (!this.comparer1.Equals(this.oldValue1, _v1))
		{
			this.oldValue1 = _v1;
			_valueChanged = true;
		}
		if (!this.comparer2.Equals(this.oldValue2, _v2))
		{
			this.oldValue2 = _v2;
			_valueChanged = true;
		}
		if (!this.comparer3.Equals(this.oldValue3, _v3))
		{
			this.oldValue3 = _v3;
			_valueChanged = true;
		}
		if (!this.comparer4.Equals(this.oldValue4, _v4))
		{
			this.oldValue4 = _v4;
			_valueChanged = true;
		}
		if (!this.comparer5.Equals(this.oldValue5, _v5))
		{
			this.oldValue5 = _v5;
			_valueChanged = true;
		}
		if (_valueChanged)
		{
			this.cachedResult = this.formatter(_v1, _v2, _v3, _v4, _v5);
		}
		return this.cachedResult;
	}

	// Token: 0x040078C3 RID: 30915
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly global::Func<T1, T2, T3, T4, T5, string> formatter;

	// Token: 0x040078C4 RID: 30916
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedResult;

	// Token: 0x040078C5 RID: 30917
	[PublicizedFrom(EAccessModifier.Private)]
	public T1 oldValue1;

	// Token: 0x040078C6 RID: 30918
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T1> comparer1 = EqualityComparer<T1>.Default;

	// Token: 0x040078C7 RID: 30919
	[PublicizedFrom(EAccessModifier.Private)]
	public T2 oldValue2;

	// Token: 0x040078C8 RID: 30920
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T2> comparer2 = EqualityComparer<T2>.Default;

	// Token: 0x040078C9 RID: 30921
	[PublicizedFrom(EAccessModifier.Private)]
	public T3 oldValue3;

	// Token: 0x040078CA RID: 30922
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T3> comparer3 = EqualityComparer<T3>.Default;

	// Token: 0x040078CB RID: 30923
	[PublicizedFrom(EAccessModifier.Private)]
	public T4 oldValue4;

	// Token: 0x040078CC RID: 30924
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T4> comparer4 = EqualityComparer<T4>.Default;

	// Token: 0x040078CD RID: 30925
	[PublicizedFrom(EAccessModifier.Private)]
	public T5 oldValue5;

	// Token: 0x040078CE RID: 30926
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T5> comparer5 = EqualityComparer<T5>.Default;
}
