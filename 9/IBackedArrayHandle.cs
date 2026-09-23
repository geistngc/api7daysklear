using System;

// Token: 0x02001397 RID: 5015
public interface IBackedArrayHandle : IDisposable
{
	// Token: 0x170012B1 RID: 4785
	// (get) Token: 0x06009E29 RID: 40489
	BackedArrayHandleMode Mode { get; }

	// Token: 0x06009E2A RID: 40490
	void Flush();
}
