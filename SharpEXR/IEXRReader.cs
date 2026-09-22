using System;

namespace SharpEXR
{
	// Token: 0x020016CF RID: 5839
	public interface IEXRReader : IDisposable
	{
		// Token: 0x0600B67A RID: 46714
		byte ReadByte();

		// Token: 0x0600B67B RID: 46715
		int ReadInt32();

		// Token: 0x0600B67C RID: 46716
		uint ReadUInt32();

		// Token: 0x0600B67D RID: 46717
		Half ReadHalf();

		// Token: 0x0600B67E RID: 46718
		float ReadSingle();

		// Token: 0x0600B67F RID: 46719
		double ReadDouble();

		// Token: 0x0600B680 RID: 46720
		string ReadNullTerminatedString(int maxLength);

		// Token: 0x0600B681 RID: 46721
		string ReadString(int length);

		// Token: 0x0600B682 RID: 46722
		string ReadString();

		// Token: 0x0600B683 RID: 46723
		byte[] ReadBytes(int count);

		// Token: 0x0600B684 RID: 46724
		void CopyBytes(byte[] dest, int offset, int count);

		// Token: 0x1700164B RID: 5707
		// (get) Token: 0x0600B685 RID: 46725
		// (set) Token: 0x0600B686 RID: 46726
		int Position { get; set; }
	}
}
