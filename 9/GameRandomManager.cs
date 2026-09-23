using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020009FA RID: 2554
public class GameRandomManager
{
	// Token: 0x17000822 RID: 2082
	// (get) Token: 0x06004C48 RID: 19528 RVA: 0x001D9161 File Offset: 0x001D7361
	public static GameRandomManager Instance
	{
		get
		{
			if (GameRandomManager.instance == null)
			{
				GameRandomManager.instance = new GameRandomManager();
				GameRandomManager.instance.SetBaseSeed((int)Stopwatch.GetTimestamp());
				GameRandomManager.instance.tempRandom = new GameRandom();
			}
			return GameRandomManager.instance;
		}
	}

	// Token: 0x06004C49 RID: 19529 RVA: 0x001D9198 File Offset: 0x001D7398
	public void SetBaseSeed(int _baseSeed)
	{
		this.baseSeed = _baseSeed;
	}

	// Token: 0x17000823 RID: 2083
	// (get) Token: 0x06004C4A RID: 19530 RVA: 0x001D91A1 File Offset: 0x001D73A1
	public int BaseSeed
	{
		get
		{
			return this.baseSeed;
		}
	}

	// Token: 0x06004C4B RID: 19531 RVA: 0x001D91A9 File Offset: 0x001D73A9
	public GameRandom CreateGameRandom()
	{
		return this.CreateGameRandom(this.baseSeed);
	}

	// Token: 0x06004C4C RID: 19532 RVA: 0x001D91B7 File Offset: 0x001D73B7
	public GameRandom CreateGameRandom(int _seed)
	{
		GameRandom gameRandom = this.pool.AllocSync(false);
		gameRandom.SetSeed(_seed);
		return gameRandom;
	}

	// Token: 0x06004C4D RID: 19533 RVA: 0x001D91CC File Offset: 0x001D73CC
	public void FreeGameRandom(GameRandom _gameRandom)
	{
		if (_gameRandom == null)
		{
			return;
		}
		this.pool.FreeSync(_gameRandom);
	}

	// Token: 0x06004C4E RID: 19534 RVA: 0x001D91DE File Offset: 0x001D73DE
	public GameRandom GetTempGameRandom(int _seed)
	{
		this.tempRandom.SetSeed(_seed);
		return this.tempRandom;
	}

	// Token: 0x06004C4F RID: 19535 RVA: 0x001D91F4 File Offset: 0x001D73F4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void log(string _format, params object[] _values)
	{
		float num = -1f;
		if (ThreadManager.IsMainThread())
		{
			num = Time.time;
			if (num != GameRandomManager.logTime)
			{
				if (GameRandomManager.logCount > 10)
				{
					Log.Warning("GameRandomManager {0} more...", new object[]
					{
						GameRandomManager.logCount - 10
					});
				}
				GameRandomManager.logTime = num;
				GameRandomManager.logCount = 0;
			}
			if (++GameRandomManager.logCount > 10)
			{
				return;
			}
		}
		Log.Warning(string.Format("{0} GameRandomManager ", num.ToCultureInvariantString()) + _format, _values);
	}

	// Token: 0x04003BD7 RID: 15319
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameRandomManager instance;

	// Token: 0x04003BD8 RID: 15320
	[PublicizedFrom(EAccessModifier.Private)]
	public int baseSeed;

	// Token: 0x04003BD9 RID: 15321
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryPooledObject<GameRandom> pool = new MemoryPooledObject<GameRandom>(30);

	// Token: 0x04003BDA RID: 15322
	[PublicizedFrom(EAccessModifier.Private)]
	public GameRandom tempRandom;

	// Token: 0x04003BDB RID: 15323
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cLogMax = 10;

	// Token: 0x04003BDC RID: 15324
	[PublicizedFrom(EAccessModifier.Private)]
	public static float logTime;

	// Token: 0x04003BDD RID: 15325
	[PublicizedFrom(EAccessModifier.Private)]
	public static int logCount;
}
