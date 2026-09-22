using System;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200173E RID: 5950
	public class TownshipShared
	{
		// Token: 0x0600B912 RID: 47378 RVA: 0x0044FA70 File Offset: 0x0044DC70
		public TownshipShared(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x04008A96 RID: 35478
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04008A97 RID: 35479
		public int NextId;

		// Token: 0x04008A98 RID: 35480
		public readonly Vector2i[] dir4way = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 0),
			new Vector2i(0, -1),
			new Vector2i(-1, 0)
		};
	}
}
