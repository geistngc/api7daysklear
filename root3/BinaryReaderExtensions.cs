using System;
using System.IO;

// Token: 0x020013A6 RID: 5030
public static class BinaryReaderExtensions
{
	// Token: 0x06009E9C RID: 40604 RVA: 0x003BFE10 File Offset: 0x003BE010
	public static bool TryReadAllBytes(this BinaryReader reader, Span<byte> dest)
	{
		int num;
		return reader.TryReadAllBytes(dest, out num);
	}

	// Token: 0x06009E9D RID: 40605 RVA: 0x003BFE28 File Offset: 0x003BE028
	public static bool TryReadAllBytes(this BinaryReader reader, Span<byte> dest, out int totalBytesRead)
	{
		int num2;
		for (totalBytesRead = 0; totalBytesRead < dest.Length; totalBytesRead += num2)
		{
			Span<byte> span = dest;
			int num = totalBytesRead;
			num2 = reader.Read(span.Slice(num, span.Length - num));
			if (num2 <= 0)
			{
				return false;
			}
		}
		return true;
	}
}
