using System;
using System.IO;

namespace SDF
{
	// Token: 0x0200169C RID: 5788
	public class SdfBinary : SdfTag
	{
		// Token: 0x0600B585 RID: 46469 RVA: 0x0043B8D9 File Offset: 0x00439AD9
		public SdfBinary(string _name, string _value)
		{
			base.TagType = SdfTagType.Binary;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x0600B586 RID: 46470 RVA: 0x0043B8F6 File Offset: 0x00439AF6
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write((short)Utils.ToBase64(base.Value.ToString()).Length);
			bw.Write(Utils.ToBase64((string)base.Value));
		}
	}
}
