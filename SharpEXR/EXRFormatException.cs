using System;

namespace SharpEXR
{
	// Token: 0x020016CA RID: 5834
	public class EXRFormatException : Exception
	{
		// Token: 0x0600B648 RID: 46664 RVA: 0x003C41FF File Offset: 0x003C23FF
		public EXRFormatException()
		{
		}

		// Token: 0x0600B649 RID: 46665 RVA: 0x003C4207 File Offset: 0x003C2407
		public EXRFormatException(string message) : base(message)
		{
		}

		// Token: 0x0600B64A RID: 46666 RVA: 0x003C4210 File Offset: 0x003C2410
		public EXRFormatException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
