using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001200 RID: 4608
public class GameStateManager
{
	// Token: 0x0600932F RID: 37679 RVA: 0x00379BF7 File Offset: 0x00377DF7
	public GameStateManager(GameManager _gameManager)
	{
		this.bGameStarted = false;
		this.gameManager = _gameManager;
	}

	// Token: 0x06009330 RID: 37680 RVA: 0x00379C20 File Offset: 0x00377E20
	public void InitGame(bool _bServer)
	{
		this.bServer = _bServer;
		GameStats.Set(EnumGameStats.GameState, 1);
		Type type = Type.GetType(GamePrefs.GetString(EnumGamePrefs.GameMode));
		if (type == null)
		{
			type = Type.GetType((string)GamePrefs.GetDefault(EnumGamePrefs.GameMode));
		}
		this.currentGameMode = (GameMode)Activator.CreateInstance(type);
		GameStats.Set(EnumGameStats.GameModeId, this.currentGameMode.GetID());
		if (this.bServer)
		{
			GameStats.Set(EnumGameStats.CurrentRoundIx, 0);
			this.timeRoundStarted = Time.time;
			this.currentGameMode.Init();
			this.currentGameMode.StartRound(GameStats.GetInt(EnumGameStats.CurrentRoundIx));
			this.bDirty = true;
		}
	}

	// Token: 0x06009331 RID: 37681 RVA: 0x00379CC4 File Offset: 0x00377EC4
	public void StartGame()
	{
		this.bGameStarted = true;
		BacktraceUtils.StartStatisticsUpdate();
	}

	// Token: 0x06009332 RID: 37682 RVA: 0x00379CD2 File Offset: 0x00377ED2
	public bool IsGameStarted()
	{
		return this.bGameStarted;
	}

	// Token: 0x06009333 RID: 37683 RVA: 0x00379CDA File Offset: 0x00377EDA
	public void EndGame()
	{
		GameStats.Set(EnumGameStats.GameState, 0);
		this.bDirty = true;
		this.bGameStarted = false;
		this.bServer = false;
	}

	// Token: 0x06009334 RID: 37684 RVA: 0x00379CF8 File Offset: 0x00377EF8
	public void SetBloodMoonDay(int day)
	{
		int @int = GameStats.GetInt(EnumGameStats.BloodMoonDay);
		if (day != @int)
		{
			GameStats.Set(EnumGameStats.BloodMoonDay, day);
			this.bDirty = true;
		}
	}

	// Token: 0x06009335 RID: 37685 RVA: 0x00379D20 File Offset: 0x00377F20
	[PublicizedFrom(EAccessModifier.Private)]
	public int nextRound()
	{
		int num = GameStats.GetInt(EnumGameStats.CurrentRoundIx);
		this.currentGameMode.EndRound(num);
		if (++num >= this.currentGameMode.GetRoundCount())
		{
			num = 0;
		}
		GameStats.Set(EnumGameStats.CurrentRoundIx, num);
		this.bDirty = true;
		this.timeRoundStarted = Time.time;
		return num;
	}

	// Token: 0x06009336 RID: 37686 RVA: 0x00379D70 File Offset: 0x00377F70
	public bool OnUpdateTick()
	{
		if (this.bServer)
		{
			if (GameStats.GetBool(EnumGameStats.TimeLimitActive))
			{
				int num = GameStats.GetInt(EnumGameStats.TimeLimitThisRound);
				if (num >= 0 && Time.time - this.timeRoundStarted >= 1f)
				{
					int num2 = (int)(Time.time - this.timeRoundStarted);
					this.timeRoundStarted = Time.time;
					num -= num2;
					GameStats.Set(EnumGameStats.TimeLimitThisRound, num);
					if (num < 0)
					{
						this.currentGameMode.StartRound(this.nextRound());
					}
					this.bDirty = true;
				}
			}
			if (GameStats.GetBool(EnumGameStats.DayLimitActive) && GameUtils.WorldTimeToDays(this.gameManager.World.worldTime) > GameStats.GetInt(EnumGameStats.DayLimitThisRound))
			{
				this.currentGameMode.StartRound(this.nextRound());
			}
			if (GameStats.GetBool(EnumGameStats.FragLimitActive))
			{
				int num3 = this.fragLimitCounter + 1;
				this.fragLimitCounter = num3;
				if (num3 > 40)
				{
					this.fragLimitCounter = 0;
					int @int = GameStats.GetInt(EnumGameStats.FragLimitThisRound);
					for (int i = 0; i < this.gameManager.World.Players.list.Count; i++)
					{
						if (this.gameManager.World.Players.list[i].KilledPlayers >= @int)
						{
							this.currentGameMode.StartRound(this.nextRound());
							break;
						}
					}
				}
			}
			if (GameTimer.Instance.ticks % 20UL == 0UL)
			{
				int num4 = 0;
				int num5 = 0;
				List<Entity> list = this.gameManager.World.Entities.list;
				for (int j = list.Count - 1; j >= 0; j--)
				{
					Entity entity = list[j];
					if (!entity.IsDead())
					{
						EntityClass entityClass = EntityClass.list[entity.entityClass];
						if (entityClass.bIsEnemyEntity)
						{
							num4++;
						}
						else if (entityClass.bIsAnimalEntity)
						{
							num5++;
						}
					}
				}
				GameStats.Set(EnumGameStats.EnemyCount, num4);
				GameStats.Set(EnumGameStats.AnimalCount, num5);
			}
			if (this.bDirty)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameStats>().Setup(GameStats.Instance), true, -1, -1, -1, null, 192, false);
				this.bDirty = false;
			}
		}
		return true;
	}

	// Token: 0x06009337 RID: 37687 RVA: 0x00379F94 File Offset: 0x00378194
	public string GetModeName()
	{
		if (this.currentGameMode == null)
		{
			return string.Empty;
		}
		if (this.currentGameMode.GetID() != this.lastGameModeID)
		{
			this.lastGameModeString = Localization.Get(this.currentGameMode.GetName(), false, null);
			this.lastGameModeID = this.currentGameMode.GetID();
		}
		return this.lastGameModeString;
	}

	// Token: 0x06009338 RID: 37688 RVA: 0x00379FF1 File Offset: 0x003781F1
	public GameMode GetGameMode()
	{
		return this.currentGameMode;
	}

	// Token: 0x04006E09 RID: 28169
	[PublicizedFrom(EAccessModifier.Private)]
	public float timeRoundStarted;

	// Token: 0x04006E0A RID: 28170
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bDirty;

	// Token: 0x04006E0B RID: 28171
	[PublicizedFrom(EAccessModifier.Private)]
	public GameMode currentGameMode;

	// Token: 0x04006E0C RID: 28172
	[PublicizedFrom(EAccessModifier.Private)]
	public GameManager gameManager;

	// Token: 0x04006E0D RID: 28173
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bServer;

	// Token: 0x04006E0E RID: 28174
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bGameStarted;

	// Token: 0x04006E0F RID: 28175
	[PublicizedFrom(EAccessModifier.Private)]
	public int fragLimitCounter;

	// Token: 0x04006E10 RID: 28176
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastGameModeID = -1;

	// Token: 0x04006E11 RID: 28177
	[PublicizedFrom(EAccessModifier.Private)]
	public string lastGameModeString = string.Empty;
}
