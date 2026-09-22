using System;
using System.Text;

namespace Platform
{
	// Token: 0x02001B7E RID: 7038
	public interface IPlatformMemoryStat
	{
		// Token: 0x17001A0E RID: 6670
		// (get) Token: 0x0600D291 RID: 53905
		string Name { get; }

		// Token: 0x0600D292 RID: 53906
		void RenderColumn(StringBuilder builder, MemoryStatColumn column, bool delta);

		// Token: 0x0600D293 RID: 53907
		void UpdateLast();
	}
}
