using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AE9 RID: 2793
public sealed class GameStageDefinition
{
	// Token: 0x06005356 RID: 21334 RVA: 0x001FE2C7 File Offset: 0x001FC4C7
	public GameStageDefinition(string _name)
	{
		this.name = _name;
	}

	// Token: 0x06005357 RID: 21335 RVA: 0x001FE2E1 File Offset: 0x001FC4E1
	public static void AddGameStage(GameStageDefinition gameStage)
	{
		gameStage.SortStages();
		GameStageDefinition.gameStages.Add(gameStage.name, gameStage);
	}

	// Token: 0x06005358 RID: 21336 RVA: 0x001FE2FA File Offset: 0x001FC4FA
	public static GameStageDefinition GetGameStage(string name)
	{
		return GameStageDefinition.gameStages[name];
	}

	// Token: 0x06005359 RID: 21337 RVA: 0x001FE307 File Offset: 0x001FC507
	public static bool TryGetGameStage(string name, out GameStageDefinition definition)
	{
		return GameStageDefinition.gameStages.TryGetValue(name, out definition);
	}

	// Token: 0x0600535A RID: 21338 RVA: 0x001FE315 File Offset: 0x001FC515
	public static void Clear()
	{
		GameStageDefinition.gameStages.Clear();
	}

	// Token: 0x0600535B RID: 21339 RVA: 0x001FE321 File Offset: 0x001FC521
	public void AddStage(GameStageDefinition.Stage stage)
	{
		this.stages.Add(stage);
	}

	// Token: 0x0600535C RID: 21340 RVA: 0x001FE32F File Offset: 0x001FC52F
	public void SortStages()
	{
		this.stages.Sort((GameStageDefinition.Stage x, GameStageDefinition.Stage y) => x.stageNum.CompareTo(y.stageNum));
	}

	// Token: 0x0600535D RID: 21341 RVA: 0x001FE35C File Offset: 0x001FC55C
	public GameStageDefinition.Stage GetStage(int stage)
	{
		if (this.stages.Count < 1)
		{
			return null;
		}
		if (stage < this.stages[0].stageNum)
		{
			return null;
		}
		int num = GameStageDefinition.GetBoundIndex<GameStageDefinition.Stage>(this.stages, (GameStageDefinition.Stage s) => s.stageNum <= stage);
		num = Mathf.Clamp(num, 0, this.stages.Count - 1);
		return this.stages[num];
	}

	// Token: 0x0600535E RID: 21342 RVA: 0x001FE3DC File Offset: 0x001FC5DC
	public static int GetBoundIndex<T>(IList<T> sortedList, Func<T, bool> f)
	{
		int num = 0;
		int num2 = sortedList.Count;
		while (num + 1 < num2)
		{
			int num3 = (num + num2) / 2;
			if (f(sortedList[num3]))
			{
				num = num3;
			}
			else
			{
				num2 = num3;
			}
		}
		if (num2 < sortedList.Count && f(sortedList[num2]))
		{
			num = num2;
		}
		return num;
	}

	// Token: 0x0600535F RID: 21343 RVA: 0x001FE430 File Offset: 0x001FC630
	public static int CalcPartyLevel(List<int> playerGameStages)
	{
		float num = 0f;
		playerGameStages.Sort();
		float num2 = GameStageDefinition.StartingWeight;
		for (int i = playerGameStages.Count - 1; i >= 0; i--)
		{
			num += (float)playerGameStages[i] * num2;
			num2 *= GameStageDefinition.DiminishingReturns;
		}
		return Mathf.FloorToInt(num);
	}

	// Token: 0x06005360 RID: 21344 RVA: 0x001FE480 File Offset: 0x001FC680
	public static int CalcGameStageAround(EntityPlayer player)
	{
		List<EntityPlayer> list = new List<EntityPlayer>();
		player.world.GetPlayersAround(player.position, 100f, list);
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			EntityPlayer entityPlayer = list[i];
			if (entityPlayer.prefab == player.prefab)
			{
				list2.Add(entityPlayer.gameStage);
			}
		}
		return GameStageDefinition.CalcPartyLevel(list2);
	}

	// Token: 0x0400411D RID: 16669
	public static float DifficultyBonus = 1f;

	// Token: 0x0400411E RID: 16670
	public static float StartingWeight = 1f;

	// Token: 0x0400411F RID: 16671
	public static float DiminishingReturns = 0.5f;

	// Token: 0x04004120 RID: 16672
	public static long DaysAliveChangeWhenKilled = 2L;

	// Token: 0x04004121 RID: 16673
	public static int LootBonusEvery;

	// Token: 0x04004122 RID: 16674
	public static int LootBonusMaxCount;

	// Token: 0x04004123 RID: 16675
	public static float LootBonusScale;

	// Token: 0x04004124 RID: 16676
	public static int LootWanderingBonusEvery;

	// Token: 0x04004125 RID: 16677
	public static float LootWanderingBonusScale;

	// Token: 0x04004126 RID: 16678
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, GameStageDefinition> gameStages = new Dictionary<string, GameStageDefinition>();

	// Token: 0x04004127 RID: 16679
	public string name;

	// Token: 0x04004128 RID: 16680
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GameStageDefinition.Stage> stages = new List<GameStageDefinition.Stage>();

	// Token: 0x02000AEA RID: 2794
	public class Stage
	{
		// Token: 0x06005362 RID: 21346 RVA: 0x001FE51A File Offset: 0x001FC71A
		public Stage(int _stageNum)
		{
			this.stageNum = _stageNum;
		}

		// Token: 0x06005363 RID: 21347 RVA: 0x001FE534 File Offset: 0x001FC734
		public void AddSpawnGroup(GameStageDefinition.SpawnGroup spawn)
		{
			this.spawnGroups.Add(spawn);
		}

		// Token: 0x06005364 RID: 21348 RVA: 0x001FE542 File Offset: 0x001FC742
		public GameStageDefinition.SpawnGroup GetSpawnGroup(int index)
		{
			if (index >= 0 && index < this.spawnGroups.Count)
			{
				return this.spawnGroups[index];
			}
			return null;
		}

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x06005365 RID: 21349 RVA: 0x001FE564 File Offset: 0x001FC764
		public int Count
		{
			get
			{
				return this.spawnGroups.Count;
			}
		}

		// Token: 0x04004129 RID: 16681
		public readonly int stageNum;

		// Token: 0x0400412A RID: 16682
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<GameStageDefinition.SpawnGroup> spawnGroups = new List<GameStageDefinition.SpawnGroup>();
	}

	// Token: 0x02000AEB RID: 2795
	public class SpawnGroup
	{
		// Token: 0x06005366 RID: 21350 RVA: 0x001FE571 File Offset: 0x001FC771
		public SpawnGroup(string _groupName, int _spawnCount, int _maxAlive, int _interval, int _duration)
		{
			this.groupName = _groupName;
			this.spawnCount = (ushort)_spawnCount;
			this.maxAlive = (ushort)_maxAlive;
			this.interval = (ushort)_interval;
			this.duration = (ushort)_duration;
		}

		// Token: 0x0400412B RID: 16683
		public readonly string groupName;

		// Token: 0x0400412C RID: 16684
		public readonly ushort spawnCount;

		// Token: 0x0400412D RID: 16685
		public readonly ushort maxAlive;

		// Token: 0x0400412E RID: 16686
		public readonly ushort interval;

		// Token: 0x0400412F RID: 16687
		public readonly ushort duration;
	}
}
