using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001745 RID: 5957
	public static class WorldBuilderStatic
	{
		// Token: 0x04008ABA RID: 35514
		public static readonly Dictionary<string, DynamicProperties> Properties = new Dictionary<string, DynamicProperties>();

		// Token: 0x04008ABB RID: 35515
		public static readonly Dictionary<string, Vector2i> WorldSizeMapper = new Dictionary<string, Vector2i>();

		// Token: 0x04008ABC RID: 35516
		public static readonly Dictionary<int, TownshipData> idToTownshipData = new Dictionary<int, TownshipData>();
	}
}
