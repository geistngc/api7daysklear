using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001746 RID: 5958
	public static class WorldBuilderConstants
	{
		// Token: 0x04008ABD RID: 35517
		public static readonly Color32 forestCol = new Color32(0, 64, 0, byte.MaxValue);

		// Token: 0x04008ABE RID: 35518
		public static readonly Color32 burntForestCol = new Color32(186, 0, byte.MaxValue, byte.MaxValue);

		// Token: 0x04008ABF RID: 35519
		public static readonly Color32 desertCol = new Color32(byte.MaxValue, 228, 119, byte.MaxValue);

		// Token: 0x04008AC0 RID: 35520
		public static readonly Color32 snowCol = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		// Token: 0x04008AC1 RID: 35521
		public static readonly Color32 wastelandCol = new Color32(byte.MaxValue, 168, 0, byte.MaxValue);

		// Token: 0x04008AC2 RID: 35522
		public static readonly Color32 waterCol = new Color32(0, 0, 100, byte.MaxValue);

		// Token: 0x04008AC3 RID: 35523
		public static readonly List<Color32> biomeColorList = new List<Color32>
		{
			WorldBuilderConstants.forestCol,
			WorldBuilderConstants.burntForestCol,
			WorldBuilderConstants.desertCol,
			WorldBuilderConstants.snowCol,
			WorldBuilderConstants.wastelandCol
		};

		// Token: 0x04008AC4 RID: 35524
		public const int ForestBiomeWeightDefault = 13;

		// Token: 0x04008AC5 RID: 35525
		public const int BurntForestBiomeWeightDefault = 18;

		// Token: 0x04008AC6 RID: 35526
		public const int DesertBiomeWeightDefault = 22;

		// Token: 0x04008AC7 RID: 35527
		public const int SnowBiomeWeightDefault = 23;

		// Token: 0x04008AC8 RID: 35528
		public const int WastelandBiomeWeightDefault = 24;

		// Token: 0x04008AC9 RID: 35529
		public static readonly int[] BiomeWeightDefaults = new int[]
		{
			13,
			18,
			22,
			23,
			24
		};

		// Token: 0x04008ACA RID: 35530
		public const int PlainsWeightDefault = 4;

		// Token: 0x04008ACB RID: 35531
		public const int HillsWeightDefault = 4;

		// Token: 0x04008ACC RID: 35532
		public const int MountainsWeightDefault = 2;
	}
}
