using System;

namespace Platform
{
	// Token: 0x02001B82 RID: 7042
	public interface IPlatformMemoryStat<T> : IPlatformMemoryStat
	{
		// Token: 0x14000123 RID: 291
		// (add) Token: 0x0600D2A0 RID: 53920
		// (remove) Token: 0x0600D2A1 RID: 53921
		event PlatformMemoryColumnChangedHandler<T> ColumnSetAfter;

		// Token: 0x17001A0F RID: 6671
		// (get) Token: 0x0600D2A2 RID: 53922
		// (set) Token: 0x0600D2A3 RID: 53923
		PlatformMemoryRenderValue<T> RenderValue { get; set; }

		// Token: 0x17001A10 RID: 6672
		// (get) Token: 0x0600D2A4 RID: 53924
		// (set) Token: 0x0600D2A5 RID: 53925
		PlatformMemoryRenderDelta<T> RenderDelta { get; set; }

		// Token: 0x0600D2A6 RID: 53926
		void Set(MemoryStatColumn column, T value);

		// Token: 0x0600D2A7 RID: 53927
		bool TryGet(MemoryStatColumn column, out T value);

		// Token: 0x0600D2A8 RID: 53928
		bool TryGetLast(MemoryStatColumn column, out T value);
	}
}
