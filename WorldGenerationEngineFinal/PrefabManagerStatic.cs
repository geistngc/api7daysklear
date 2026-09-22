using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200171D RID: 5917
	public static class PrefabManagerStatic
	{
		// Token: 0x040089E5 RID: 35301
		public static readonly Dictionary<string, Vector2i> TileMinMaxCounts = new Dictionary<string, Vector2i>();

		// Token: 0x040089E6 RID: 35302
		public static readonly Dictionary<string, float> TileMaxDensityScore = new Dictionary<string, float>();

		// Token: 0x040089E7 RID: 35303
		public static readonly List<PrefabManager.POIWeightData> prefabWeightData = new List<PrefabManager.POIWeightData>();
	}
}
