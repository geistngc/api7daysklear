using System;
using System.Collections.Generic;

// Token: 0x020013B3 RID: 5043
public class CachedStringFormatter<T1>
{
	// Token: 0x06009EE5 RID: 40677 RVA: 0x003C171D File Offset: 0x003BF91D
	public CachedStringFormatter(Func<T1, string> _formatterFunc)
	{
		this.formatter = _formatterFunc;
	}

	// Token: 0x06009EE6 RID: 40678 RVA: 0x003C1738 File Offset: 0x003BF938
	public string Format(T1 _v1)
	{
		bool flag;
		return this.Format(_v1, out flag);
	}

	// Token: 0x06009EE7 RID: 40679 RVA: 0x003C1750 File Offset: 0x003BF950
	public string Format(T1 _v1, out bool _valueChanged)
	{
		_valueChanged = (this.cachedResult == null);
		if (!this.comparer1.Equals(this.oldValue1, _v1))
		{
			this.oldValue1 = _v1;
			_valueChanged = true;
		}
		if (_valueChanged)
		{
			this.cachedResult = this.formatter(_v1);
		}
		return this.cachedResult;
	}

	// Token: 0x040078A7 RID: 30887
	[PublicizedFrom(EAccessModifier.Protected)]
	public Func<T1, string> formatter;

	// Token: 0x040078A8 RID: 30888
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedResult;

	// Token: 0x040078A9 RID: 30889
	[PublicizedFrom(EAccessModifier.Private)]
	public T1 oldValue1;

	// Token: 0x040078AA RID: 30890
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T1> comparer1 = EqualityComparer<T1>.Default;
}
