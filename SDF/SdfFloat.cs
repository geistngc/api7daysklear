using System;
using System.IO;

namespace SDF
{
	// Token: 0x02001698 RID: 5784
	public class SdfFloat : SdfTag
	{
		// Token: 0x0600B57D RID: 46461 RVA: 0x0043B7C0 File Offset: 0x004399C0
		public SdfFloat(string _name, float _value)
		{
			base.TagType = SdfTagType.Float;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x0600B57E RID: 46462 RVA: 0x0043B7E2 File Offset: 0x004399E2
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write((float)base.Value);
		}
	}
}
