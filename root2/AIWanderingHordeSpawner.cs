using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000426 RID: 1062
[Preserve]
public class AIWanderingHordeSpawner
{
	// Token: 0x060020BA RID: 8378 RVA: 0x000C54FC File Offset: 0x000C36FC
	public AIWanderingHordeSpawner(AIDirector _director, AIWanderingHordeSpawner.SpawnType _spawnType, AIWanderingHordeSpawner.HordeArrivedDelegate _arrivedEvent, List<AIDirectorPlayerState> _targets, ulong _endTime, Vector3 _startPos, Vector3 _pitStopPos, Vector3 _endPos)
	{
		this.director = _director;
		this.startPos = _startPos;
		this.pitStopPos = _pitStopPos;
		this.endPos = _endPos;
		this.endTime = _endTime;
		this.arrivedCallback = _arrivedEvent;
		this.spawnType = _spawnType;
		AIWanderingHordeSpawner.SpawnType spawnType = this.spawnType;
		string gameStageName;
		int mod;
		if (spawnType != AIWanderingHordeSpawner.SpawnType.Bandits)
		{
			if (spawnType != AIWanderingHordeSpawner.SpawnType.Horde)
			{
			}
			gameStageName = "WanderingHorde";
			mod = 50;
		}
		else
		{
			gameStageName = "WanderingBandits";
			mod = 0;
		}
		this.spawner = new AIDirectorGameStagePartySpawner(_director.World, gameStageName);
		for (int i = 0; i < _targets.Count; i++)
		{
			this.spawner.AddMember(_targets[i].Player);
		}
		this.spawner.ResetPartyLevel(mod);
		this.spawner.ClearMembers();
	}

	// Token: 0x060020BB RID: 8379 RVA: 0x000C55C8 File Offset: 0x000C37C8
	public bool Update(World world, float _deltaTime)
	{
		if (world.GetPlayers().Count == 0)
		{
			return true;
		}
		if (world.worldTime >= this.endTime)
		{
			if (this.arrivedCallback != null)
			{
				this.arrivedCallback();
			}
			return true;
		}
		bool flag = this.UpdateSpawn(world, _deltaTime);
		if (flag && this.commandList.Count == 0)
		{
			if (this.arrivedCallback != null)
			{
				this.arrivedCallback();
			}
			return true;
		}
		if (!flag)
		{
			AstarManager.Instance.AddLocationLine(this.startPos, this.endPos, 64);
		}
		else
		{
			Vector3 vector = Vector3.zero;
			int num = 0;
			for (int i = 0; i < this.commandList.Count; i++)
			{
				Entity enemy = this.commandList[i].Enemy;
				if (!enemy.IsDead())
				{
					vector += enemy.position;
					num++;
				}
			}
			if (num > 0)
			{
				vector *= 1f / (float)num;
				AstarManager.Instance.AddLocation(vector, 64);
			}
		}
		this.UpdateHorde(_deltaTime);
		return false;
	}

	// Token: 0x060020BC RID: 8380 RVA: 0x000C56C8 File Offset: 0x000C38C8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UpdateSpawn(World _world, float _deltaTime)
	{
		if (!AIDirector.CanSpawn(1f))
		{
			return true;
		}
		if (!this.spawner.Tick((double)_deltaTime))
		{
			return true;
		}
		this.spawnDelay -= _deltaTime;
		if (this.spawnDelay >= 0f)
		{
			return false;
		}
		this.spawnDelay = 1f;
		if (!this.spawner.canSpawn)
		{
			return false;
		}
		Vector3 transformPos;
		if (!_world.GetMobRandomSpawnPosWithWater(this.startPos, 1, 6, 15, true, out transformPos))
		{
			return false;
		}
		int randomEntityFromGroupMaxTier = EntityGroups.GetRandomEntityFromGroupMaxTier(this.spawner.spawnGroupName, EntityFactory.MaxEntityTier, ref this.lastClassId, true, false, null);
		if (randomEntityFromGroupMaxTier == -1)
		{
			Log.Warning(string.Format("Could not spawn an entity from group {0} within Sandbox Options Max Tier Limit {1}.", this.spawner.spawnGroupName, EntityFactory.MaxEntityTier));
			return false;
		}
		EntityEnemy entityEnemy = (EntityEnemy)EntityFactory.CreateEntity(randomEntityFromGroupMaxTier, transformPos);
		entityEnemy.SetSpawnerSource(EnumSpawnerSource.Dynamic);
		_world.SpawnEntityInWorld(entityEnemy);
		entityEnemy.IsHordeZombie = true;
		entityEnemy.bIsChunkObserver = true;
		entityEnemy.IsHordeZombie = true;
		entityEnemy.bIsChunkObserver = true;
		int num = this.bonusLootSpawnCount + 1;
		this.bonusLootSpawnCount = num;
		if (num >= GameStageDefinition.LootWanderingBonusEvery)
		{
			this.bonusLootSpawnCount = 0;
			entityEnemy.lootDropProb *= GameStageDefinition.LootWanderingBonusScale;
		}
		AIWanderingHordeSpawner.ZombieCommand zombieCommand = new AIWanderingHordeSpawner.ZombieCommand();
		zombieCommand.Enemy = entityEnemy;
		zombieCommand.TargetPos = AIWanderingHordeSpawner.RandomPos(this.director, this.endPos, 6f);
		zombieCommand.Command = AIWanderingHordeSpawner.ECommand.EndPos;
		this.commandList.Add(zombieCommand);
		entityEnemy.SetInvestigatePosition(zombieCommand.TargetPos, 6000, false);
		AIDirector.LogAI("Spawned wandering horde (group {0}, zombie {1})", new object[]
		{
			this.spawner.spawnGroupName,
			entityEnemy
		});
		this.spawner.IncSpawnCount();
		return false;
	}

	// Token: 0x060020BD RID: 8381 RVA: 0x000C5874 File Offset: 0x000C3A74
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateHorde(float dt)
	{
		int i = 0;
		while (i < this.commandList.Count)
		{
			AIWanderingHordeSpawner.ZombieCommand zombieCommand = this.commandList[i];
			bool flag = zombieCommand.Enemy.IsDead() || zombieCommand.Enemy.GetAttackTarget() != null;
			if (!flag)
			{
				if (zombieCommand.Command == AIWanderingHordeSpawner.ECommand.PitStop || zombieCommand.Command == AIWanderingHordeSpawner.ECommand.EndPos)
				{
					if (zombieCommand.Enemy.HasInvestigatePosition)
					{
						if (zombieCommand.Enemy.InvestigatePosition != zombieCommand.TargetPos)
						{
							flag = true;
							string str = "Wandering horde zombie '";
							EntityEnemy enemy = zombieCommand.Enemy;
							AIDirector.LogAIExtra(str + ((enemy != null) ? enemy.ToString() : null) + "' removed from horde control. Was killed or investigating", Array.Empty<object>());
						}
						else
						{
							zombieCommand.Enemy.SetInvestigatePosition(zombieCommand.TargetPos, 6000, false);
						}
					}
					else if (zombieCommand.Command == AIWanderingHordeSpawner.ECommand.PitStop)
					{
						string str2 = "Wandering horde zombie '";
						EntityEnemy enemy2 = zombieCommand.Enemy;
						AIDirector.LogAIExtra(str2 + ((enemy2 != null) ? enemy2.ToString() : null) + "' reached pitstop. Wander around for awhile", Array.Empty<object>());
						zombieCommand.WanderTime = 90f + this.director.random.RandomFloat * 4f;
						zombieCommand.Command = AIWanderingHordeSpawner.ECommand.Wander;
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					zombieCommand.WanderTime -= dt;
					zombieCommand.Enemy.ResetDespawnTime();
					if (zombieCommand.WanderTime <= 0f && zombieCommand.Enemy.GetAttackTarget() == null)
					{
						string str3 = "Wandering horde zombie '";
						EntityEnemy enemy3 = zombieCommand.Enemy;
						AIDirector.LogAIExtra(str3 + ((enemy3 != null) ? enemy3.ToString() : null) + "' wandered long enough. Going to endstop", Array.Empty<object>());
						zombieCommand.Command = AIWanderingHordeSpawner.ECommand.EndPos;
						zombieCommand.TargetPos = AIWanderingHordeSpawner.RandomPos(this.director, this.endPos, 6f);
						zombieCommand.Enemy.SetInvestigatePosition(zombieCommand.TargetPos, 6000, false);
						zombieCommand.Enemy.IsHordeZombie = false;
					}
				}
			}
			if (flag)
			{
				string str4 = "Wandering horde zombie '";
				EntityEnemy enemy4 = zombieCommand.Enemy;
				AIDirector.LogAIExtra(str4 + ((enemy4 != null) ? enemy4.ToString() : null) + "' removed from control", Array.Empty<object>());
				zombieCommand.Enemy.IsHordeZombie = false;
				zombieCommand.Enemy.bIsChunkObserver = false;
				this.commandList.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060020BE RID: 8382 RVA: 0x000C5AC4 File Offset: 0x000C3CC4
	public void Cleanup()
	{
		for (int i = 0; i < this.commandList.Count; i++)
		{
			AIWanderingHordeSpawner.ZombieCommand zombieCommand = this.commandList[i];
			zombieCommand.Enemy.IsHordeZombie = false;
			zombieCommand.Enemy.bIsChunkObserver = false;
		}
	}

	// Token: 0x060020BF RID: 8383 RVA: 0x000C5B0C File Offset: 0x000C3D0C
	public static Vector3 RandomPos(AIDirector director, Vector3 target, float radius)
	{
		Vector2 vector = director.random.RandomOnUnitCircle * radius;
		return target + new Vector3(vector.x, 0f, vector.y);
	}

	// Token: 0x04001662 RID: 5730
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cInvestigateTime = 6000;

	// Token: 0x04001663 RID: 5731
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirector director;

	// Token: 0x04001664 RID: 5732
	[PublicizedFrom(EAccessModifier.Private)]
	public AIWanderingHordeSpawner.HordeArrivedDelegate arrivedCallback;

	// Token: 0x04001665 RID: 5733
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startPos;

	// Token: 0x04001666 RID: 5734
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 pitStopPos;

	// Token: 0x04001667 RID: 5735
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 endPos;

	// Token: 0x04001668 RID: 5736
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong endTime;

	// Token: 0x04001669 RID: 5737
	public AIWanderingHordeSpawner.SpawnType spawnType;

	// Token: 0x0400166A RID: 5738
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorGameStagePartySpawner spawner;

	// Token: 0x0400166B RID: 5739
	[PublicizedFrom(EAccessModifier.Private)]
	public float spawnDelay;

	// Token: 0x0400166C RID: 5740
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastClassId;

	// Token: 0x0400166D RID: 5741
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIWanderingHordeSpawner.ZombieCommand> commandList = new List<AIWanderingHordeSpawner.ZombieCommand>();

	// Token: 0x0400166E RID: 5742
	[PublicizedFrom(EAccessModifier.Private)]
	public int bonusLootSpawnCount;

	// Token: 0x02000427 RID: 1063
	// (Invoke) Token: 0x060020C1 RID: 8385
	public delegate void HordeArrivedDelegate();

	// Token: 0x02000428 RID: 1064
	public enum SpawnType
	{
		// Token: 0x04001670 RID: 5744
		Bandits,
		// Token: 0x04001671 RID: 5745
		Horde
	}

	// Token: 0x02000429 RID: 1065
	[PublicizedFrom(EAccessModifier.Private)]
	public enum ECommand
	{
		// Token: 0x04001673 RID: 5747
		PitStop,
		// Token: 0x04001674 RID: 5748
		Wander,
		// Token: 0x04001675 RID: 5749
		EndPos
	}

	// Token: 0x0200042A RID: 1066
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class ZombieCommand
	{
		// Token: 0x04001676 RID: 5750
		public AIWanderingHordeSpawner.ECommand Command;

		// Token: 0x04001677 RID: 5751
		public EntityEnemy Enemy;

		// Token: 0x04001678 RID: 5752
		public float WanderTime;

		// Token: 0x04001679 RID: 5753
		public Vector3 TargetPos;
	}
}
