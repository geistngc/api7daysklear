using System;

// Token: 0x020012F0 RID: 4848
public class CoroutineCancellationToken
{
	// Token: 0x060099A4 RID: 39332 RVA: 0x003A37F4 File Offset: 0x003A19F4
	public void Cancel()
	{
		this.cancelled = true;
	}

	// Token: 0x060099A5 RID: 39333 RVA: 0x003A37FD File Offset: 0x003A19FD
	public bool IsCancelled()
	{
		return this.cancelled;
	}

	// Token: 0x040073BD RID: 29629
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cancelled;
}
