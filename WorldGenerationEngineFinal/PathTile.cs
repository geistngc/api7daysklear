using System;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200171A RID: 5914
	public struct PathTile
	{
		// Token: 0x040089DE RID: 35294
		public PathTile.PathTileStates TileState;

		// Token: 0x0200171B RID: 5915
		public enum PathTileStates : byte
		{
			// Token: 0x040089E0 RID: 35296
			Free,
			// Token: 0x040089E1 RID: 35297
			Blocked,
			// Token: 0x040089E2 RID: 35298
			Highway,
			// Token: 0x040089E3 RID: 35299
			Country
		}
	}
}
