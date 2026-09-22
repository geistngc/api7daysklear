using System;
using System.Text;

namespace Platform
{
	// Token: 0x02001B81 RID: 7041
	// (Invoke) Token: 0x0600D29D RID: 53917
	public delegate void PlatformMemoryRenderDelta<in T>(StringBuilder builder, T current, T last);
}
