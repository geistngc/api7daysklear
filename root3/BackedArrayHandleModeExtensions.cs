using System;

// Token: 0x0200138E RID: 5006
public static class BackedArrayHandleModeExtensions
{
	// Token: 0x06009DDD RID: 40413 RVA: 0x003BC6C8 File Offset: 0x003BA8C8
	public static bool CanRead(this BackedArrayHandleMode mode)
	{
		bool result;
		if (mode != BackedArrayHandleMode.ReadOnly)
		{
			if (mode != BackedArrayHandleMode.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("mode", mode, string.Format("Unknown mode: {0}", mode));
			}
			result = true;
		}
		else
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06009DDE RID: 40414 RVA: 0x003BC708 File Offset: 0x003BA908
	public static bool CanWrite(this BackedArrayHandleMode mode)
	{
		bool result;
		if (mode != BackedArrayHandleMode.ReadOnly)
		{
			if (mode != BackedArrayHandleMode.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("mode", mode, string.Format("Unknown mode: {0}", mode));
			}
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}
}
