using System;

// Token: 0x020013BC RID: 5052
public class CachedStringFormatterFloat : CachedStringFormatter<float>
{
	// Token: 0x06009F02 RID: 40706 RVA: 0x003C1BA8 File Offset: 0x003BFDA8
	public CachedStringFormatterFloat(string _format = null) : base(null)
	{
		this.formatter = new Func<float, string>(this.formatterFunc);
		this.format = _format;
	}

	// Token: 0x06009F03 RID: 40707 RVA: 0x003C1BCA File Offset: 0x003BFDCA
	[PublicizedFrom(EAccessModifier.Private)]
	public string formatterFunc(float _f)
	{
		return _f.ToCultureInvariantString(this.format);
	}

	// Token: 0x040078CF RID: 30927
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string format;
}
