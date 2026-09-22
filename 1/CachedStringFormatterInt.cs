using System;

// Token: 0x020013BB RID: 5051
public class CachedStringFormatterInt : CachedStringFormatter<int>
{
	// Token: 0x06009F00 RID: 40704 RVA: 0x003C1B8B File Offset: 0x003BFD8B
	public CachedStringFormatterInt() : base(new Func<int, string>(CachedStringFormatterInt.formatterFunc))
	{
	}

	// Token: 0x06009F01 RID: 40705 RVA: 0x003C1B9F File Offset: 0x003BFD9F
	[PublicizedFrom(EAccessModifier.Private)]
	public static string formatterFunc(int _i)
	{
		return _i.ToString();
	}
}
