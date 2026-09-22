using System;
using System.IO;

namespace SDF
{
	// Token: 0x0200169E RID: 5790
	public abstract class SdfTag
	{
		// Token: 0x17001605 RID: 5637
		// (get) Token: 0x0600B589 RID: 46473 RVA: 0x0043B96D File Offset: 0x00439B6D
		// (set) Token: 0x0600B58A RID: 46474 RVA: 0x0043B975 File Offset: 0x00439B75
		public SdfTagType TagType { get; set; }

		// Token: 0x17001606 RID: 5638
		// (get) Token: 0x0600B58B RID: 46475 RVA: 0x0043B97E File Offset: 0x00439B7E
		// (set) Token: 0x0600B58C RID: 46476 RVA: 0x0043B986 File Offset: 0x00439B86
		public string Name { get; set; }

		// Token: 0x17001607 RID: 5639
		// (get) Token: 0x0600B58D RID: 46477 RVA: 0x0043B98F File Offset: 0x00439B8F
		// (set) Token: 0x0600B58E RID: 46478 RVA: 0x0043B997 File Offset: 0x00439B97
		public object Value { get; set; }

		// Token: 0x0600B58F RID: 46479
		public abstract void WritePayload(BinaryWriter bw);

		// Token: 0x0600B590 RID: 46480 RVA: 0x0000640C File Offset: 0x0000460C
		[PublicizedFrom(EAccessModifier.Protected)]
		public SdfTag()
		{
		}
	}
}
