using System;
using System.IO;

namespace SDF
{
	// Token: 0x0200169B RID: 5787
	public class SdfBool : SdfTag
	{
		// Token: 0x0600B583 RID: 46467 RVA: 0x0043B8A4 File Offset: 0x00439AA4
		public SdfBool(string _name, bool _value)
		{
			base.TagType = SdfTagType.Bool;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x0600B584 RID: 46468 RVA: 0x0043B8C6 File Offset: 0x00439AC6
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write((bool)base.Value);
		}
	}
}
