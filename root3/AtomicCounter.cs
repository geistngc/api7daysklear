using System;
using System.Threading;

// Token: 0x020013CF RID: 5071
public sealed class AtomicCounter
{
	// Token: 0x170012CC RID: 4812
	// (get) Token: 0x06009F61 RID: 40801 RVA: 0x003C3BDA File Offset: 0x003C1DDA
	public int Value
	{
		get
		{
			return this.m_counter;
		}
	}

	// Token: 0x06009F62 RID: 40802 RVA: 0x003C3BE2 File Offset: 0x003C1DE2
	public int Increment()
	{
		return Interlocked.Increment(ref this.m_counter);
	}

	// Token: 0x06009F63 RID: 40803 RVA: 0x003C3BEF File Offset: 0x003C1DEF
	public int Decrement()
	{
		return Interlocked.Decrement(ref this.m_counter);
	}

	// Token: 0x0400791D RID: 31005
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_counter;
}
