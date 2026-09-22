using System;
using System.IO;

namespace SDF
{
	// Token: 0x0200169A RID: 5786
	public class SdfString : SdfTag
	{
		// Token: 0x0600B581 RID: 46465 RVA: 0x0043B82A File Offset: 0x00439A2A
		public SdfString(string _name, string _value)
		{
			base.TagType = SdfTagType.String;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x0600B582 RID: 46466 RVA: 0x0043B848 File Offset: 0x00439A48
		public override void WritePayload(BinaryWriter bw)
		{
			if (base.Value == null)
			{
				Log.Error("Null value: " + base.Name);
			}
			bw.Write((short)Utils.ToBase64(base.Value.ToString()).Length);
			bw.Write(Utils.ToBase64(base.Value.ToString()));
		}
	}
}
