using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x020016E7 RID: 5863
	public struct KeyCode
	{
		// Token: 0x0600B73B RID: 46907 RVA: 0x00441EDE File Offset: 0x004400DE
		public KeyCode(int filmMfcCode, int filmType, int prefix, int count, int perfOffset, int perfsPerFrame, int perfsPerCount)
		{
			this.FilmMfcCode = filmMfcCode;
			this.FilmType = filmType;
			this.Prefix = prefix;
			this.Count = count;
			this.PerfOffset = perfOffset;
			this.PerfsPerFrame = perfsPerFrame;
			this.PerfsPerCount = perfsPerCount;
		}

		// Token: 0x0400892F RID: 35119
		public readonly int FilmMfcCode;

		// Token: 0x04008930 RID: 35120
		public readonly int FilmType;

		// Token: 0x04008931 RID: 35121
		public readonly int Prefix;

		// Token: 0x04008932 RID: 35122
		public readonly int Count;

		// Token: 0x04008933 RID: 35123
		public readonly int PerfOffset;

		// Token: 0x04008934 RID: 35124
		public readonly int PerfsPerFrame;

		// Token: 0x04008935 RID: 35125
		public readonly int PerfsPerCount;
	}
}
