using System;
using UnityEngine;

// Token: 0x020013BD RID: 5053
public class CachedStringFormatterXuiRgbaColor : CachedStringFormatter<Color32>
{
	// Token: 0x06009F04 RID: 40708 RVA: 0x003C1BD8 File Offset: 0x003BFDD8
	public CachedStringFormatterXuiRgbaColor() : base(new Func<Color32, string>(CachedStringFormatterXuiRgbaColor.formatterFunc))
	{
		this.comparer1 = Color32EqualityComparer.Instance;
	}

	// Token: 0x06009F05 RID: 40709 RVA: 0x003C1BF7 File Offset: 0x003BFDF7
	[PublicizedFrom(EAccessModifier.Private)]
	public static string formatterFunc(Color32 _color)
	{
		return _color.ToXuiColorString();
	}
}
