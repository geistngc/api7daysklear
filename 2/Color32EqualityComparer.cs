using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020013FD RID: 5117
public class Color32EqualityComparer : IEqualityComparer<Color32>
{
	// Token: 0x0600A092 RID: 41106 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32EqualityComparer()
	{
	}

	// Token: 0x0600A093 RID: 41107 RVA: 0x003C7BDA File Offset: 0x003C5DDA
	public bool Equals(Color32 _a, Color32 _b)
	{
		return _a.r == _b.r && _a.g == _b.g && _a.b == _b.b && _a.a == _b.a;
	}

	// Token: 0x0600A094 RID: 41108 RVA: 0x003C7C18 File Offset: 0x003C5E18
	public int GetHashCode(Color32 _a)
	{
		return ((_a.r.GetHashCode() * 397 ^ _a.g.GetHashCode()) * 397 ^ _a.b.GetHashCode()) * 397 ^ _a.a.GetHashCode();
	}

	// Token: 0x0400797B RID: 31099
	public static readonly Color32EqualityComparer Instance = new Color32EqualityComparer();
}
