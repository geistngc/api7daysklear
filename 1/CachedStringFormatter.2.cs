using System;
using System.Collections.Generic;

// Token: 0x020013B4 RID: 5044
public class CachedStringFormatter<T1, T2>
{
	// Token: 0x06009EE8 RID: 40680 RVA: 0x003C17A2 File Offset: 0x003BF9A2
	public CachedStringFormatter(Func<T1, T2, string> _formatterFunc)
	{
		this.formatter = _formatterFunc;
	}

	// Token: 0x06009EE9 RID: 40681 RVA: 0x003C17C8 File Offset: 0x003BF9C8
	public string Format(T1 _v1, T2 _v2)
	{
		bool flag;
		return this.Format(_v1, _v2, out flag);
	}

	// Token: 0x06009EEA RID: 40682 RVA: 0x003C17E0 File Offset: 0x003BF9E0
	public string Format(T1 _v1, T2 _v2, out bool _valueChanged)
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
		if (_valueChanged)
		{
			this.cachedResult = this.formatter(_v1, _v2);
		}
		return this.cachedResult;
	}

	// Token: 0x040078AB RID: 30891
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Func<T1, T2, string> formatter;

	// Token: 0x040078AC RID: 30892
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedResult;

	// Token: 0x040078AD RID: 30893
	[PublicizedFrom(EAccessModifier.Private)]
	public T1 oldValue1;

	// Token: 0x040078AE RID: 30894
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T1> comparer1 = EqualityComparer<T1>.Default;

	// Token: 0x040078AF RID: 30895
	[PublicizedFrom(EAccessModifier.Private)]
	public T2 oldValue2;

	// Token: 0x040078B0 RID: 30896
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T2> comparer2 = EqualityComparer<T2>.Default;
}
