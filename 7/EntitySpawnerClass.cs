using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000AE8 RID: 2792
public class EntitySpawnerClass
{
	// Token: 0x06005352 RID: 21330 RVA: 0x001FDD74 File Offset: 0x001FBF74
	public void Init()
	{
		if (!this.Properties.Values.ContainsKey(EntitySpawnerClass.PropEntityGroupName))
		{
			throw new Exception(string.Concat(new string[]
			{
				"Mandatory property '",
				EntitySpawnerClass.PropEntityGroupName,
				"' missing in entityspawnerclass '",
				this.name,
				"'"
			}));
		}
		this.entityGroupName = this.Properties.Values[EntitySpawnerClass.PropEntityGroupName];
		if (!EntityGroups.list.ContainsKey(this.entityGroupName))
		{
			throw new Exception("Entity spawner '" + this.name + "' contains invalid group " + this.entityGroupName);
		}
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropStartSound))
		{
			this.startSound = this.Properties.Values[EntitySpawnerClass.PropStartSound];
		}
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropStartText))
		{
			this.startText = this.Properties.Values[EntitySpawnerClass.PropStartText];
		}
		this.spawnAtTimeOfDay = EDaytime.Any;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTime))
		{
			this.spawnAtTimeOfDay = EnumUtils.Parse<EDaytime>(this.Properties.Values[EntitySpawnerClass.PropTime], false);
		}
		this.delayBetweenSpawns = 0f;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropDelayBetweenSpawns))
		{
			this.delayBetweenSpawns = StringParsers.ParseFloat(this.Properties.Values[EntitySpawnerClass.PropDelayBetweenSpawns], 0, -1, NumberStyles.Any);
		}
		this.totalAlive = 1;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTotalAlive))
		{
			this.totalAlive = int.Parse(this.Properties.Values[EntitySpawnerClass.PropTotalAlive]);
		}
		this.totalPerWaveMin = 1;
		this.totalPerWaveMax = 1;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTotalPerWave))
		{
			StringParsers.ParseMinMaxCount(this.Properties.Values[EntitySpawnerClass.PropTotalPerWave], out this.totalPerWaveMin, out this.totalPerWaveMax);
		}
		this.delayToNextWave = 1f;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropDelayToNextWave))
		{
			this.delayToNextWave = StringParsers.ParseFloat(this.Properties.Values[EntitySpawnerClass.PropDelayToNextWave], 0, -1, NumberStyles.Any);
		}
		this.bAttackPlayerImmediately = false;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropAttackPlayerAtOnce))
		{
			this.bAttackPlayerImmediately = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropAttackPlayerAtOnce], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropNumberOfWaves))
		{
			this.numberOfWaves = int.Parse(this.Properties.Values[EntitySpawnerClass.PropNumberOfWaves]);
		}
		this.bTerritorial = false;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTerritorial))
		{
			this.bTerritorial = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropTerritorial], 0, -1, true);
		}
		this.territorialRange = 10;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTerritorialRange))
		{
			this.territorialRange = int.Parse(this.Properties.Values[EntitySpawnerClass.PropTerritorialRange]);
		}
		this.bSpawnOnGround = true;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropSpawnOnGround))
		{
			this.bSpawnOnGround = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropSpawnOnGround], 0, -1, true);
		}
		this.bIgnoreTrigger = false;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropIgnoreTrigger))
		{
			this.bIgnoreTrigger = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropIgnoreTrigger], 0, -1, true);
		}
		this.bPropResetToday = true;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropResetToday))
		{
			this.bPropResetToday = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropResetToday], 0, -1, true);
		}
		this.daysToRespawnIfPlayerLeft = 0;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropDaysToRespawnIfPlayerLeft))
		{
			this.daysToRespawnIfPlayerLeft = Mathf.RoundToInt(StringParsers.ParseFloat(this.Properties.Values[EntitySpawnerClass.PropDaysToRespawnIfPlayerLeft], 0, -1, NumberStyles.Any));
		}
		if (EntitySpawnerClass.DefaultClassName == null)
		{
			EntitySpawnerClass.DefaultClassName = this;
		}
	}

	// Token: 0x06005353 RID: 21331 RVA: 0x001FE1F0 File Offset: 0x001FC3F0
	public static void Cleanup()
	{
		EntitySpawnerClass.list.Clear();
	}

	// Token: 0x040040F8 RID: 16632
	public static string PropStartSound = "StartSound";

	// Token: 0x040040F9 RID: 16633
	public static string PropStartText = "StartText";

	// Token: 0x040040FA RID: 16634
	public static string PropEntityGroupName = "EntityGroupName";

	// Token: 0x040040FB RID: 16635
	public static string PropTime = "Time";

	// Token: 0x040040FC RID: 16636
	public static string PropDelayBetweenSpawns = "DelayBetweenSpawns";

	// Token: 0x040040FD RID: 16637
	public static string PropTotalAlive = "TotalAlive";

	// Token: 0x040040FE RID: 16638
	public static string PropTotalPerWave = "TotalPerWave";

	// Token: 0x040040FF RID: 16639
	public static string PropDelayToNextWave = "DelayToNextWave";

	// Token: 0x04004100 RID: 16640
	public static string PropAttackPlayerAtOnce = "AttackPlayerAtOnce";

	// Token: 0x04004101 RID: 16641
	public static string PropNumberOfWaves = "NumberOfWaves";

	// Token: 0x04004102 RID: 16642
	public static string PropTerritorial = "Territorial";

	// Token: 0x04004103 RID: 16643
	public static string PropTerritorialRange = "TerritorialRange";

	// Token: 0x04004104 RID: 16644
	public static string PropSpawnOnGround = "SpawnOnGround";

	// Token: 0x04004105 RID: 16645
	public static string PropIgnoreTrigger = "IgnoreTrigger";

	// Token: 0x04004106 RID: 16646
	public static string PropResetToday = "ResetToday";

	// Token: 0x04004107 RID: 16647
	public static string PropDaysToRespawnIfPlayerLeft = "DaysToRespawnIfPlayerLeft";

	// Token: 0x04004108 RID: 16648
	public static DictionarySave<string, EntitySpawnerClassForDay> list = new DictionarySave<string, EntitySpawnerClassForDay>();

	// Token: 0x04004109 RID: 16649
	public static EntitySpawnerClass DefaultClassName;

	// Token: 0x0400410A RID: 16650
	public DynamicProperties Properties = new DynamicProperties();

	// Token: 0x0400410B RID: 16651
	public string name;

	// Token: 0x0400410C RID: 16652
	public string entityGroupName;

	// Token: 0x0400410D RID: 16653
	public EDaytime spawnAtTimeOfDay;

	// Token: 0x0400410E RID: 16654
	public float delayBetweenSpawns;

	// Token: 0x0400410F RID: 16655
	public int totalAlive;

	// Token: 0x04004110 RID: 16656
	public float delayToNextWave;

	// Token: 0x04004111 RID: 16657
	public int totalPerWaveMin;

	// Token: 0x04004112 RID: 16658
	public int totalPerWaveMax;

	// Token: 0x04004113 RID: 16659
	public int numberOfWaves;

	// Token: 0x04004114 RID: 16660
	public bool bAttackPlayerImmediately;

	// Token: 0x04004115 RID: 16661
	public bool bSpawnOnGround;

	// Token: 0x04004116 RID: 16662
	public bool bIgnoreTrigger;

	// Token: 0x04004117 RID: 16663
	public bool bTerritorial;

	// Token: 0x04004118 RID: 16664
	public int territorialRange;

	// Token: 0x04004119 RID: 16665
	public bool bPropResetToday;

	// Token: 0x0400411A RID: 16666
	public int daysToRespawnIfPlayerLeft;

	// Token: 0x0400411B RID: 16667
	public string startSound;

	// Token: 0x0400411C RID: 16668
	public string startText;
}
