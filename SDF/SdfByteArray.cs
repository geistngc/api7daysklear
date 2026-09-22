using System;
using System.IO;

namespace SDF
{
	// Token: 0x0200169D RID: 5789
	public class SdfByteArray : SdfTag
	{
		// Token: 0x0600B587 RID: 46471 RVA: 0x0043B92A File Offset: 0x00439B2A
		public SdfByteArray(string _name, byte[] _value)
		{
			base.TagType = SdfTagType.ByteArray;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x0600B588 RID: 46472 RVA: 0x0043B947 File Offset: 0x00439B47
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write(((byte[])base.Value).Length);
			bw.Write((byte[])base.Value);
		}
	}
}
