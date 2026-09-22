using System;
using System.IO;

namespace SDF
{
	// Token: 0x02001699 RID: 5785
	public class SdfInt : SdfTag
	{
		// Token: 0x0600B57F RID: 46463 RVA: 0x0043B7F5 File Offset: 0x004399F5
		public SdfInt(string _name, int _value)
		{
			base.TagType = SdfTagType.Int;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x0600B580 RID: 46464 RVA: 0x0043B817 File Offset: 0x00439A17
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write((int)base.Value);
		}
	}
}
