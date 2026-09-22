using System;
using UnityEngine.Scripting;

// Token: 0x02000411 RID: 1041
[Preserve]
public class AIDirectorConstants
{
	// Token: 0x040015EC RID: 5612
	public static bool DebugOutput = true;

	// Token: 0x040015ED RID: 5613
	public const int kFileVersion = 10;

	// Token: 0x040015EE RID: 5614
	public const int kMaxSupplyCrates = 12;

	// Token: 0x040015EF RID: 5615
	public const float kStealthSightDistanceMultiplier = 0.8f;

	// Token: 0x040015F0 RID: 5616
	public const float kStealthNighttimeSightDistanceMultiplier = 0.5f;

	// Token: 0x040015F1 RID: 5617
	public const float kHordeMeterWarn1Threshold = 0.5f;

	// Token: 0x040015F2 RID: 5618
	public const float kHordeMeterWarn2Threshold = 0.8f;

	// Token: 0x040015F3 RID: 5619
	public const float kHordeMeterWarnResetThreshold = 0.2f;

	// Token: 0x040015F4 RID: 5620
	public const int kHordeDaySpawnRangeMin = 45;

	// Token: 0x040015F5 RID: 5621
	public const int kHordeDaySpawnRangeMax = 55;

	// Token: 0x040015F6 RID: 5622
	public const int kHordeNightSpawnRangeMin = 55;

	// Token: 0x040015F7 RID: 5623
	public const int kHordeNightSpawnRangeMax = 70;

	// Token: 0x040015F8 RID: 5624
	public const float kHordeMeterDecayDelay = 8f;

	// Token: 0x040015F9 RID: 5625
	public const float kHordeMeterDecayRate = 4f;

	// Token: 0x040015FA RID: 5626
	public const int kWanderingHordeGlobalStartTime = 28000;

	// Token: 0x040015FB RID: 5627
	public const int kSpawnWanderingHordeMin = 12000;

	// Token: 0x040015FC RID: 5628
	public const int kSpawnWanderingHordeMax = 24000;

	// Token: 0x040015FD RID: 5629
	public const int kWanderingHordeGroupSize = 6;

	// Token: 0x040015FE RID: 5630
	public const float kWanderingHordeSpawnDistance = 92f;

	// Token: 0x040015FF RID: 5631
	public const float kWanderingHordeSpawnMinDistance = 50f;

	// Token: 0x04001600 RID: 5632
	public const float kWanderingHordePlayerClusterSize = 30f;

	// Token: 0x04001601 RID: 5633
	public const int kSoundPriorityStart = 10;

	// Token: 0x04001602 RID: 5634
	public const int kSoundPriorityRange = 100;

	// Token: 0x04001603 RID: 5635
	public const int kScoutSpawnDistance = 80;

	// Token: 0x04001604 RID: 5636
	public const float kScoutScreamGraceTime = 2f;

	// Token: 0x04001605 RID: 5637
	public const float kScoutScreamAgainTime = 18f;

	// Token: 0x04001606 RID: 5638
	public const float kScoutSpawnAnotherScoutChance = 0.12f;

	// Token: 0x04001607 RID: 5639
	public const int kScoutSummonedPerScream = 5;

	// Token: 0x04001608 RID: 5640
	public const int kScoutSummonedTotal = 25;
}
