using System;

// Token: 0x020013D7 RID: 5079
public class DebugWrapperException : Exception
{
	// Token: 0x06009F8E RID: 40846 RVA: 0x003C41FF File Offset: 0x003C23FF
	public DebugWrapperException()
	{
	}

	// Token: 0x06009F8F RID: 40847 RVA: 0x003C4207 File Offset: 0x003C2407
	public DebugWrapperException(string message) : base(message)
	{
	}

	// Token: 0x06009F90 RID: 40848 RVA: 0x003C4210 File Offset: 0x003C2410
	public DebugWrapperException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
