using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x020013D9 RID: 5081
public class EnumerableDebugWrapper<T> : EnumerableDebugWrapper, IEnumerable<!0>, IEnumerable
{
	// Token: 0x06009F9C RID: 40860 RVA: 0x003C4400 File Offset: 0x003C2600
	public EnumerableDebugWrapper(IEnumerable<T> enumerable) : this(null, enumerable)
	{
	}

	// Token: 0x06009F9D RID: 40861 RVA: 0x003C440A File Offset: 0x003C260A
	public EnumerableDebugWrapper(DebugWrapper parent, IEnumerable<T> enumerable) : base(parent, enumerable)
	{
		this.m_enumerable = enumerable;
	}

	// Token: 0x06009F9E RID: 40862 RVA: 0x003C441B File Offset: 0x003C261B
	public new IEnumerator<T> GetEnumerator()
	{
		return base.DebugEnumerator<T>(this.m_enumerable.GetEnumerator());
	}

	// Token: 0x04007931 RID: 31025
	[PublicizedFrom(EAccessModifier.Private)]
	public new readonly IEnumerable<T> m_enumerable;
}
