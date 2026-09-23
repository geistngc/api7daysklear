using System;
using System.Collections;

// Token: 0x020013DA RID: 5082
public class EnumerableDebugWrapper : DebugWrapper, IEnumerable
{
	// Token: 0x06009F9F RID: 40863 RVA: 0x003C442E File Offset: 0x003C262E
	public EnumerableDebugWrapper(IEnumerable enumerable) : this(null, enumerable)
	{
	}

	// Token: 0x06009FA0 RID: 40864 RVA: 0x003C4438 File Offset: 0x003C2638
	public EnumerableDebugWrapper(DebugWrapper parent, IEnumerable enumerable) : base(parent)
	{
		this.m_enumerable = enumerable;
	}

	// Token: 0x06009FA1 RID: 40865 RVA: 0x003C4448 File Offset: 0x003C2648
	public IEnumerator GetEnumerator()
	{
		return base.DebugEnumerator(this.m_enumerable.GetEnumerator());
	}

	// Token: 0x04007932 RID: 31026
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IEnumerable m_enumerable;
}
