using System;
using System.Runtime.CompilerServices;

// Token: 0x02001398 RID: 5016
public interface IBackedArrayView<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x170012B2 RID: 4786
	// (get) Token: 0x06009E2B RID: 40491
	int Length { get; }

	// Token: 0x170012B3 RID: 4787
	// (get) Token: 0x06009E2C RID: 40492
	BackedArrayHandleMode Mode { get; }

	// Token: 0x170012B4 RID: 4788
	T this[int i]
	{
		get;
		set;
	}

	// Token: 0x06009E2F RID: 40495
	void Flush();
}
