using System;
using UnityEngine;

namespace MapRendering
{
	// Token: 0x02001AB0 RID: 6832
	public static class Constants
	{
		// Token: 0x17001965 RID: 6501
		// (get) Token: 0x0600CE17 RID: 52759 RVA: 0x004B0B55 File Offset: 0x004AED55
		public static int MAP_BLOCK_TO_CHUNK_DIV
		{
			get
			{
				return Constants.MapBlockSize / 16;
			}
		}

		// Token: 0x17001966 RID: 6502
		// (get) Token: 0x0600CE18 RID: 52760 RVA: 0x001845A5 File Offset: 0x001827A5
		public static int MAP_REGION_TO_CHUNK_DIV
		{
			get
			{
				return 32;
			}
		}

		// Token: 0x04009C66 RID: 40038
		public static readonly TextureFormat DefaultTextureFormat = TextureFormat.ARGB32;

		// Token: 0x04009C67 RID: 40039
		public static int MapBlockSize = 128;

		// Token: 0x04009C68 RID: 40040
		public const int MapChunkSize = 16;

		// Token: 0x04009C69 RID: 40041
		public const int MapRegionSize = 512;

		// Token: 0x04009C6A RID: 40042
		public static int Zoomlevels = 5;

		// Token: 0x04009C6B RID: 40043
		public static string MapDirectory = string.Empty;
	}
}
