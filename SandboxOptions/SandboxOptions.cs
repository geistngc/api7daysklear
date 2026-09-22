using System;

namespace SandboxOptions
{
	// Token: 0x0200189D RID: 6301
	public enum SandboxOptions
	{
		// Token: 0x0400933A RID: 37690
		RangedDamage,
		// Token: 0x0400933B RID: 37691
		MeleeDamage,
		// Token: 0x0400933C RID: 37692
		BlockDamage,
		// Token: 0x0400933D RID: 37693
		TerrainDamage,
		// Token: 0x0400933E RID: 37694
		HeadshotMultiplier,
		// Token: 0x0400933F RID: 37695
		CrouchSpeed,
		// Token: 0x04009340 RID: 37696
		WalkSpeed,
		// Token: 0x04009341 RID: 37697
		RunSpeed,
		// Token: 0x04009342 RID: 37698
		JumpStrength,
		// Token: 0x04009343 RID: 37699
		StaminaUsage,
		// Token: 0x04009344 RID: 37700
		StaminaRegen,
		// Token: 0x04009345 RID: 37701
		PlayerLevelBonusApplied,
		// Token: 0x04009346 RID: 37702
		JarRefund,
		// Token: 0x04009347 RID: 37703
		ShowHealthBars,
		// Token: 0x04009348 RID: 37704
		ShowEnemyDamage,
		// Token: 0x04009349 RID: 37705
		NewbieCoat,
		// Token: 0x0400934A RID: 37706
		HeadshotMode,
		// Token: 0x0400934B RID: 37707
		IncomingDamage,
		// Token: 0x0400934C RID: 37708
		XPMultiplier,
		// Token: 0x0400934D RID: 37709
		ShowXP,
		// Token: 0x0400934E RID: 37710
		EncumbranceModifier,
		// Token: 0x0400934F RID: 37711
		ItemDegradation,
		// Token: 0x04009350 RID: 37712
		LoseItemsOnDeathType,
		// Token: 0x04009351 RID: 37713
		LoseItemsOnDeathCount,
		// Token: 0x04009352 RID: 37714
		DegradeItemsOnDeath,
		// Token: 0x04009353 RID: 37715
		DegradeAmountOnDeath,
		// Token: 0x04009354 RID: 37716
		DeathPenalty,
		// Token: 0x04009355 RID: 37717
		DropOnDeath,
		// Token: 0x04009356 RID: 37718
		DropOnQuit,
		// Token: 0x04009357 RID: 37719
		InfectionRate,
		// Token: 0x04009358 RID: 37720
		EnemySpawnMode,
		// Token: 0x04009359 RID: 37721
		EntityDamage,
		// Token: 0x0400935A RID: 37722
		BlockDamageAI,
		// Token: 0x0400935B RID: 37723
		BlockDamageAIBM,
		// Token: 0x0400935C RID: 37724
		ZombieMove,
		// Token: 0x0400935D RID: 37725
		ZombieMoveNight,
		// Token: 0x0400935E RID: 37726
		ZombieFeralMove,
		// Token: 0x0400935F RID: 37727
		ZombieBMMove,
		// Token: 0x04009360 RID: 37728
		ZombieFeralSense,
		// Token: 0x04009361 RID: 37729
		AISmellMode,
		// Token: 0x04009362 RID: 37730
		AllowZombieDigging,
		// Token: 0x04009363 RID: 37731
		ZombieRageChance,
		// Token: 0x04009364 RID: 37732
		EntityIncomingDamage,
		// Token: 0x04009365 RID: 37733
		MaxEnemyTier,
		// Token: 0x04009366 RID: 37734
		BiomeDayZombieRespawn,
		// Token: 0x04009367 RID: 37735
		BiomeDayAnimalRespawn,
		// Token: 0x04009368 RID: 37736
		BiomeDayEnemyDensity,
		// Token: 0x04009369 RID: 37737
		ZombiesEatAnimals,
		// Token: 0x0400936A RID: 37738
		BloodMoonFrequency,
		// Token: 0x0400936B RID: 37739
		BloodMoonRange,
		// Token: 0x0400936C RID: 37740
		BloodMoonWarning,
		// Token: 0x0400936D RID: 37741
		BloodMoonEnemyCount,
		// Token: 0x0400936E RID: 37742
		AirDropFrequency,
		// Token: 0x0400936F RID: 37743
		AirDropMarker,
		// Token: 0x04009370 RID: 37744
		AirDropRandomTime,
		// Token: 0x04009371 RID: 37745
		BiomeProgression,
		// Token: 0x04009372 RID: 37746
		TemperatureSurvival,
		// Token: 0x04009373 RID: 37747
		StormFreq,
		// Token: 0x04009374 RID: 37748
		StormWarning,
		// Token: 0x04009375 RID: 37749
		HeatMapSensitivity,
		// Token: 0x04009376 RID: 37750
		GlobalGSModifier,
		// Token: 0x04009377 RID: 37751
		BiomeGSModifier,
		// Token: 0x04009378 RID: 37752
		GlobalLSModifier,
		// Token: 0x04009379 RID: 37753
		BiomeLSModifier,
		// Token: 0x0400937A RID: 37754
		POITierLSModifier,
		// Token: 0x0400937B RID: 37755
		GlobalTSModifier,
		// Token: 0x0400937C RID: 37756
		DayNightLength,
		// Token: 0x0400937D RID: 37757
		DayLightLength,
		// Token: 0x0400937E RID: 37758
		AllowMap,
		// Token: 0x0400937F RID: 37759
		AllowCompass,
		// Token: 0x04009380 RID: 37760
		AllowScreenMarkers,
		// Token: 0x04009381 RID: 37761
		ShowLocationInfo,
		// Token: 0x04009382 RID: 37762
		ShowDayTime,
		// Token: 0x04009383 RID: 37763
		WorkstationsInTheWild,
		// Token: 0x04009384 RID: 37764
		MaxTechType,
		// Token: 0x04009385 RID: 37765
		LootRespawnDays,
		// Token: 0x04009386 RID: 37766
		LootTimer,
		// Token: 0x04009387 RID: 37767
		LootMaxTier,
		// Token: 0x04009388 RID: 37768
		GlobalLootCount,
		// Token: 0x04009389 RID: 37769
		FoodLootCount,
		// Token: 0x0400938A RID: 37770
		DrinkLootCount,
		// Token: 0x0400938B RID: 37771
		MedicalLootCount,
		// Token: 0x0400938C RID: 37772
		AmmoLootCount,
		// Token: 0x0400938D RID: 37773
		ResourceLootCount,
		// Token: 0x0400938E RID: 37774
		ArmorLootCount,
		// Token: 0x0400938F RID: 37775
		MeleeLootCount,
		// Token: 0x04009390 RID: 37776
		RangedLootCount,
		// Token: 0x04009391 RID: 37777
		DukesLootCount,
		// Token: 0x04009392 RID: 37778
		CraftingMagazinesLootCount,
		// Token: 0x04009393 RID: 37779
		TreasureMapChance,
		// Token: 0x04009394 RID: 37780
		LootBagChance,
		// Token: 0x04009395 RID: 37781
		CropOutput,
		// Token: 0x04009396 RID: 37782
		SeedDropOutput,
		// Token: 0x04009397 RID: 37783
		CropGrowthSpeed,
		// Token: 0x04009398 RID: 37784
		BackpackCrafting,
		// Token: 0x04009399 RID: 37785
		WorkstationCrafting,
		// Token: 0x0400939A RID: 37786
		CraftingProgression,
		// Token: 0x0400939B RID: 37787
		CraftingTime,
		// Token: 0x0400939C RID: 37788
		CraftingInput,
		// Token: 0x0400939D RID: 37789
		CraftingOutput,
		// Token: 0x0400939E RID: 37790
		CraftingMaxTier,
		// Token: 0x0400939F RID: 37791
		MiningOutput,
		// Token: 0x040093A0 RID: 37792
		HarvestingOutput,
		// Token: 0x040093A1 RID: 37793
		ScrappingOutput,
		// Token: 0x040093A2 RID: 37794
		SmeltingType,
		// Token: 0x040093A3 RID: 37795
		DewCollectorTime,
		// Token: 0x040093A4 RID: 37796
		DewCollectorOutput,
		// Token: 0x040093A5 RID: 37797
		DewCollectorInput,
		// Token: 0x040093A6 RID: 37798
		ApiaryTime,
		// Token: 0x040093A7 RID: 37799
		ApiaryOutput,
		// Token: 0x040093A8 RID: 37800
		ApiaryInput,
		// Token: 0x040093A9 RID: 37801
		BookLootCount,
		// Token: 0x040093AA RID: 37802
		CrouchRunSpeed,
		// Token: 0x040093AB RID: 37803
		RepairTypes,
		// Token: 0x040093AC RID: 37804
		MaxDegradationAmount,
		// Token: 0x040093AD RID: 37805
		PointsPerMagazine,
		// Token: 0x040093AE RID: 37806
		SkillGainRate,
		// Token: 0x040093AF RID: 37807
		SkillPointsPerLevel,
		// Token: 0x040093B0 RID: 37808
		QuestsEnabled,
		// Token: 0x040093B1 RID: 37809
		IntroQuestEnabled,
		// Token: 0x040093B2 RID: 37810
		TraderToTraderQuestsEnabled,
		// Token: 0x040093B3 RID: 37811
		StarterSkillPoints,
		// Token: 0x040093B4 RID: 37812
		QuestsPerTier,
		// Token: 0x040093B5 RID: 37813
		QuestProgressionDailyLimit,
		// Token: 0x040093B6 RID: 37814
		BuriedQuestsEnabled,
		// Token: 0x040093B7 RID: 37815
		POIQuestsEnabled,
		// Token: 0x040093B8 RID: 37816
		TraderDialog,
		// Token: 0x040093B9 RID: 37817
		TraderHours,
		// Token: 0x040093BA RID: 37818
		TradersEnabled,
		// Token: 0x040093BB RID: 37819
		VendingEnabled,
		// Token: 0x040093BC RID: 37820
		TraderSellPrices,
		// Token: 0x040093BD RID: 37821
		TraderBuyPrices,
		// Token: 0x040093BE RID: 37822
		TraderProtection,
		// Token: 0x040093BF RID: 37823
		TraderResetInterval,
		// Token: 0x040093C0 RID: 37824
		TraderItemAbundance,
		// Token: 0x040093C1 RID: 37825
		TraderBuyLimit,
		// Token: 0x040093C2 RID: 37826
		TraderMaxTier,
		// Token: 0x040093C3 RID: 37827
		VendingResetInterval,
		// Token: 0x040093C4 RID: 37828
		VendingItemAbundance,
		// Token: 0x040093C5 RID: 37829
		ChallengesEnabled,
		// Token: 0x040093C6 RID: 37830
		IntroChallengesEnabled,
		// Token: 0x040093C7 RID: 37831
		VehicleFuelUsage,
		// Token: 0x040093C8 RID: 37832
		VehicleEntityDamage,
		// Token: 0x040093C9 RID: 37833
		VehicleBlockDamage,
		// Token: 0x040093CA RID: 37834
		VehicleSelfDamage,
		// Token: 0x040093CB RID: 37835
		ElectricalOutput,
		// Token: 0x040093CC RID: 37836
		SillyCelebrate,
		// Token: 0x040093CD RID: 37837
		SillyBigHeads,
		// Token: 0x040093CE RID: 37838
		SillyTinyZombies,
		// Token: 0x040093CF RID: 37839
		SillySounds,
		// Token: 0x040093D0 RID: 37840
		SillyLowGravity,
		// Token: 0x040093D1 RID: 37841
		SillyBlackandWhite,
		// Token: 0x040093D2 RID: 37842
		ChickenCoopTime,
		// Token: 0x040093D3 RID: 37843
		ChickenCoopOutput,
		// Token: 0x040093D4 RID: 37844
		ChickenCoopInput,
		// Token: 0x040093D5 RID: 37845
		BiomeDayAnimalDensity,
		// Token: 0x040093D6 RID: 37846
		BiomeNightZombieRespawn,
		// Token: 0x040093D7 RID: 37847
		BiomeNightAnimalRespawn,
		// Token: 0x040093D8 RID: 37848
		BiomeNightEnemyDensity,
		// Token: 0x040093D9 RID: 37849
		BiomeNightAnimalDensity,
		// Token: 0x040093DA RID: 37850
		InfectionChance,
		// Token: 0x040093DB RID: 37851
		HungerMultiplier,
		// Token: 0x040093DC RID: 37852
		ThirstMultiplier,
		// Token: 0x040093DD RID: 37853
		StackSizeMultiplier,
		// Token: 0x040093DE RID: 37854
		FullChickenStressEvent,
		// Token: 0x040093DF RID: 37855
		Max
	}
}
