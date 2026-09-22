using System;

// Token: 0x02000E8C RID: 3724
public abstract class XUiListEntry<T> : IComparable<T> where T : XUiListEntry<T>
{
	// Token: 0x06007396 RID: 29590
	public abstract int CompareTo(T _otherEntry);

	// Token: 0x06007397 RID: 29591 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool MatchesSearch(string _searchString)
	{
		return true;
	}

	// Token: 0x17000D6B RID: 3435
	// (get) Token: 0x06007398 RID: 29592 RVA: 0x002BE3B9 File Offset: 0x002BC5B9
	// (set) Token: 0x06007399 RID: 29593 RVA: 0x002BE3C1 File Offset: 0x002BC5C1
	public virtual bool UiDirty { get; set; }

	// Token: 0x0600739A RID: 29594 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiListEntry()
	{
	}
}
