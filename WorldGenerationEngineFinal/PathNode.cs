using System;
using Unity.Burst;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001718 RID: 5912
	[BurstCompile(CompileSynchronously = true)]
	public struct PathNode
	{
		// Token: 0x0600B821 RID: 47137 RVA: 0x00447EE7 File Offset: 0x004460E7
		public PathNode(Vector2i _position, float _travelledCost, float _totalCost, int _next)
		{
			this.position = _position;
			this.travelledCost = _travelledCost;
			this.totalCost = _totalCost;
			this.pathNext = _next;
			this.listNext = -1;
		}

		// Token: 0x0600B822 RID: 47138 RVA: 0x00447F0D File Offset: 0x0044610D
		public void Set(Vector2i _position, float _travelledCost, float _totalCost, int _next)
		{
			this.position = _position;
			this.travelledCost = _travelledCost;
			this.totalCost = _totalCost;
			this.pathNext = _next;
		}

		// Token: 0x040089D7 RID: 35287
		public Vector2i position;

		// Token: 0x040089D8 RID: 35288
		public float travelledCost;

		// Token: 0x040089D9 RID: 35289
		public float totalCost;

		// Token: 0x040089DA RID: 35290
		public int pathNext;

		// Token: 0x040089DB RID: 35291
		public int listNext;
	}
}
