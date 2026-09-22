using System;
using System.Diagnostics;

namespace Platform.XBL.Save
{
	// Token: 0x02001C41 RID: 7233
	public static class GameCoreSaveHelpers
	{
		// Token: 0x0600D674 RID: 54900 RVA: 0x004D5B9C File Offset: 0x004D3D9C
		[Conditional("NEVER_DEFINED")]
		public static void TraceLogHR(int hr, string identifier)
		{
			XblHelpers.LogHR(hr, identifier, false);
		}

		// Token: 0x0600D675 RID: 54901 RVA: 0x004D5B9C File Offset: 0x004D3D9C
		public static void NonTraceLogHR(int hr, string identifier)
		{
			XblHelpers.LogHR(hr, identifier, false);
		}
	}
}
