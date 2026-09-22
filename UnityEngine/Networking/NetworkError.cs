using System;

namespace UnityEngine.Networking
{
	// Token: 0x020018ED RID: 6381
	public enum NetworkError
	{
		// Token: 0x040094EE RID: 38126
		Ok,
		// Token: 0x040094EF RID: 38127
		WrongHost,
		// Token: 0x040094F0 RID: 38128
		WrongConnection,
		// Token: 0x040094F1 RID: 38129
		WrongChannel,
		// Token: 0x040094F2 RID: 38130
		NoResources,
		// Token: 0x040094F3 RID: 38131
		BadMessage,
		// Token: 0x040094F4 RID: 38132
		Timeout,
		// Token: 0x040094F5 RID: 38133
		MessageToLong,
		// Token: 0x040094F6 RID: 38134
		WrongOperation,
		// Token: 0x040094F7 RID: 38135
		VersionMismatch,
		// Token: 0x040094F8 RID: 38136
		CRCMismatch,
		// Token: 0x040094F9 RID: 38137
		DNSFailure,
		// Token: 0x040094FA RID: 38138
		UsageError
	}
}
