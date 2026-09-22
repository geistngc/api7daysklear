using System;
using System.Text;

// Token: 0x020013B2 RID: 5042
public static class ByteLengthUtils
{
	// Token: 0x06009EE3 RID: 40675 RVA: 0x003C16D8 File Offset: 0x003BF8D8
	[PublicizedFrom(EAccessModifier.Private)]
	public static int GetBinaryWriter7BitEncodedIntLength(int value)
	{
		int num = 0;
		for (uint num2 = (uint)value; num2 >= 128U; num2 >>= 7)
		{
			num++;
		}
		return num + 1;
	}

	// Token: 0x06009EE4 RID: 40676 RVA: 0x003C1700 File Offset: 0x003BF900
	public static int GetBinaryWriterLength(this string text, Encoding encoding)
	{
		int byteCount = encoding.GetByteCount(text);
		return ByteLengthUtils.GetBinaryWriter7BitEncodedIntLength(byteCount) + byteCount;
	}
}
