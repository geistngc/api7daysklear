using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpEXR
{
	// Token: 0x020016DA RID: 5850
	public class OffsetTable : IEnumerable<uint>, IEnumerable
	{
		// Token: 0x17001653 RID: 5715
		// (get) Token: 0x0600B710 RID: 46864 RVA: 0x00441362 File Offset: 0x0043F562
		// (set) Token: 0x0600B711 RID: 46865 RVA: 0x0044136A File Offset: 0x0043F56A
		public List<uint> Offsets { get; set; }

		// Token: 0x0600B712 RID: 46866 RVA: 0x00441373 File Offset: 0x0043F573
		public OffsetTable()
		{
			this.Offsets = new List<uint>();
		}

		// Token: 0x0600B713 RID: 46867 RVA: 0x00441386 File Offset: 0x0043F586
		public OffsetTable(int capacity)
		{
			this.Offsets = new List<uint>(capacity);
		}

		// Token: 0x0600B714 RID: 46868 RVA: 0x0044139C File Offset: 0x0043F59C
		public void Read(IEXRReader reader, int count)
		{
			for (int i = 0; i < count; i++)
			{
				this.Offsets.Add(reader.ReadUInt32());
				reader.ReadUInt32();
			}
		}

		// Token: 0x0600B715 RID: 46869 RVA: 0x004413CD File Offset: 0x0043F5CD
		public IEnumerator<uint> GetEnumerator()
		{
			return this.Offsets.GetEnumerator();
		}

		// Token: 0x0600B716 RID: 46870 RVA: 0x004413DF File Offset: 0x0043F5DF
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
