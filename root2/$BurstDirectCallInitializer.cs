using System;
using UnityEngine;
using WorldGenerationEngineFinal;

// Token: 0x02001D99 RID: 7577
[PublicizedFrom(EAccessModifier.Internal)]
public static class $BurstDirectCallInitializer
{
	// Token: 0x0600DE8F RID: 56975 RVA: 0x004FDEFC File Offset: 0x004FC0FC
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Initialize()
	{
		ChunkProviderGenerateWorldFromRaw.RoadSmooth_000056FA$BurstDirectCall.Initialize();
		PathingUtils.FindDetailedPath_0000B7B3$BurstDirectCall.Initialize();
		PathingUtils.FindDetailedPath_0000B7B5$BurstDirectCall.Initialize();
		PathingUtils.CalcPathBounds_0000B7B6$BurstDirectCall.Initialize();
		PathingUtils.FindClosestPathPoint_0000B7B7$BurstDirectCall.Initialize();
		PathingUtils.FindClosestPathPoint_0000B7B8$BurstDirectCall.Initialize();
		PathingUtils.IsPointOnPath_0000B7B9$BurstDirectCall.Initialize();
		RawStamp.SmoothAlpha_0000B802$BurstDirectCall.Initialize();
		StampManager.DrawStamp_0000B825$BurstDirectCall.Initialize();
		StampManager.DrawWaterStamp_0000B828$BurstDirectCall.Initialize();
		WorldBuilder.ClearWaterUnderTerrain_0000B8E9$BurstDirectCall.Initialize();
		WorldBuilder.FinalizeWater_0000B8EC$BurstDirectCall.Initialize();
		WorldBuilder.SmoothRoadTerrainTask_0000B8F3$BurstDirectCall.Initialize();
		WorldBuilder.AdjustHeights_0000B906$BurstDirectCall.Initialize();
	}
}
