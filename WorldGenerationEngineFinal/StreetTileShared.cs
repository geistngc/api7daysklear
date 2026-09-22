using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001732 RID: 5938
	public class StreetTileShared
	{
		// Token: 0x0600B8A5 RID: 47269 RVA: 0x0044AF18 File Offset: 0x00449118
		public StreetTileShared(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
			for (int i = 0; i < this.RoadShapeExits.Length; i++)
			{
				int num = this.RoadShapeExits[i];
				int[] array = new int[4];
				for (int j = 0; j < 4; j++)
				{
					array[j] = num;
					num = ((num << 1 & 15) | num >> 3);
				}
				this.RoadShapeExitsPerRotation.Add(array);
			}
		}

		// Token: 0x04008A39 RID: 35385
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04008A3A RID: 35386
		public readonly FastTags<TagGroup.Poi> traderTag = FastTags<TagGroup.Poi>.Parse("trader");

		// Token: 0x04008A3B RID: 35387
		public readonly FastTags<TagGroup.Poi> wildernessTag = FastTags<TagGroup.Poi>.Parse("wilderness");

		// Token: 0x04008A3C RID: 35388
		public readonly string[] RoadShapes = new string[]
		{
			"rwg_tile_straight",
			"rwg_tile_t",
			"rwg_tile_intersection",
			"rwg_tile_cap",
			"rwg_tile_corner"
		};

		// Token: 0x04008A3D RID: 35389
		public readonly string[] RoadShapesDistrict = new string[]
		{
			"rwg_tile_{0}straight",
			"rwg_tile_{0}t",
			"rwg_tile_{0}intersection",
			"rwg_tile_{0}cap",
			"rwg_tile_{0}corner"
		};

		// Token: 0x04008A3E RID: 35390
		[PublicizedFrom(EAccessModifier.Private)]
		public int[] RoadShapeExits = new int[]
		{
			5,
			7,
			15,
			4,
			6
		};

		// Token: 0x04008A3F RID: 35391
		public readonly List<int> RoadShapeExitCounts = new List<int>
		{
			2,
			3,
			4,
			1,
			2
		};

		// Token: 0x04008A40 RID: 35392
		public readonly List<int[]> RoadShapeExitsPerRotation = new List<int[]>();

		// Token: 0x04008A41 RID: 35393
		public readonly Vector2i[] dir4way = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 0),
			new Vector2i(0, -1),
			new Vector2i(-1, 0)
		};

		// Token: 0x04008A42 RID: 35394
		public readonly Vector2i[] dirDiagonal = new Vector2i[]
		{
			new Vector2i(1, 1),
			new Vector2i(1, -1),
			new Vector2i(-1, -1),
			new Vector2i(-1, 1)
		};

		// Token: 0x04008A43 RID: 35395
		public readonly Vector2i[] dir8way = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 1),
			new Vector2i(1, 0),
			new Vector2i(1, -1),
			new Vector2i(0, -1),
			new Vector2i(-1, -1),
			new Vector2i(-1, 0),
			new Vector2i(-1, 1)
		};

		// Token: 0x04008A44 RID: 35396
		public readonly Vector2i[] dir9way = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 1),
			new Vector2i(1, 0),
			new Vector2i(1, -1),
			new Vector2i(0, 0),
			new Vector2i(0, -1),
			new Vector2i(-1, -1),
			new Vector2i(-1, 0),
			new Vector2i(-1, 1)
		};
	}
}
