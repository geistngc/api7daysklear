using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000421 RID: 1057
[Preserve]
public class AIScoutHordeSpawner
{
	// Token: 0x060020A8 RID: 8360 RVA: 0x000C4E9F File Offset: 0x000C309F
	public AIScoutHordeSpawner(EntitySpawner _spawner, Vector3 _startPos, Vector3 _endPos, bool _isBloodMoon)
	{
		this.spawner = _spawner;
		this.startPos = _startPos;
		this.endPos = _endPos;
		this.isBloodMoon = _isBloodMoon;
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x000C4EDA File Offset: 0x000C30DA
	public bool Update(World world, float dt)
	{
		if (world.GetPlayers().Count == 0)
		{
			return true;
		}
		if (this.SpawnUpdate(world) && this.hordeList.Count == 0)
		{
			return true;
		}
		this.UpdateHorde(world, dt);
		return false;
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x000C4F0C File Offset: 0x000C310C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool SpawnUpdate(World world)
	{
		if (!AIDirector.CanSpawn(1f) || this.spawner.CurrentWave > 0)
		{
			return true;
		}
		this.spawner.SpawnManually(world, GameUtils.WorldTimeToDays(world.worldTime), true, delegate(EntitySpawner _es, out EntityPlayer _outPlayerToAttack)
		{
			_outPlayerToAttack = null;
			return true;
		}, delegate(EntitySpawner _es, EntityPlayer _inPlayerToAttack, out EntityPlayer _outPlayerToAttack, out Vector3 _pos)
		{
			_outPlayerToAttack = null;
			return world.GetMobRandomSpawnPosWithWater(this.startPos, 0, 8, 10, true, out _pos);
		}, null, this.spawnedList);
		for (int i = 0; i < this.spawnedList.Count; i++)
		{
			EntityEnemy entityEnemy = this.spawnedList[i] as EntityEnemy;
			if (entityEnemy != null)
			{
				entityEnemy.IsHordeZombie = true;
				entityEnemy.IsScoutZombie = true;
				entityEnemy.IsBloodMoon = this.isBloodMoon;
				entityEnemy.bIsChunkObserver = true;
				AIScoutHordeSpawner.ZombieCommand zombieCommand = new AIScoutHordeSpawner.ZombieCommand();
				zombieCommand.Zombie = entityEnemy;
				zombieCommand.TargetPos = AIScoutHordeSpawner.CalcRandomPos(world.aiDirector, this.endPos, 6f);
				zombieCommand.Wandering = false;
				zombieCommand.AttackDelay = 2f;
				entityEnemy.SetInvestigatePosition(zombieCommand.TargetPos, 6000, true);
				this.hordeList.Add(zombieCommand);
				string str = "scout horde spawned '";
				EntityEnemy entityEnemy2 = entityEnemy;
				AIDirector.LogAI(str + ((entityEnemy2 != null) ? entityEnemy2.ToString() : null) + "'. Moving to point of interest", Array.Empty<object>());
			}
		}
		this.spawnedList.Clear();
		return this.spawner.CurrentWave > 0;
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x000C5098 File Offset: 0x000C3298
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateHorde(World world, float deltaTime)
	{
		int i = 0;
		while (i < this.hordeList.Count)
		{
			bool flag = false;
			AIScoutHordeSpawner.ZombieCommand zombieCommand = this.hordeList[i];
			EntityEnemy zombie = zombieCommand.Zombie;
			if (zombie.IsDead())
			{
				flag = true;
			}
			else
			{
				EntityAlive attackTarget = zombie.GetAttackTarget();
				bool flag2 = attackTarget is EntityPlayer;
				if (zombieCommand.Horde != null)
				{
					if (attackTarget && !attackTarget.IsDead() && flag2)
					{
						zombieCommand.Horde.SetSpawnPos(attackTarget.GetPosition());
					}
					else
					{
						zombieCommand.Horde.SetSpawnPos(zombie.GetPosition());
					}
				}
				if (zombieCommand.Attacking)
				{
					if (!zombieCommand.Zombie.HasInvestigatePosition && (attackTarget == null || attackTarget.IsDead() || !flag2))
					{
						zombieCommand.Wandering = true;
						zombieCommand.WorldExpiryTime = world.worldTime + 2000UL;
					}
					else
					{
						zombieCommand.AttackDelay -= deltaTime;
						if (zombieCommand.AttackDelay <= 0f && zombie.bodyDamage.CurrentStun == EnumEntityStunType.None)
						{
							if (zombie.HasInvestigatePosition || (flag2 && !attackTarget.IsDead()))
							{
								Vector3 target = attackTarget ? attackTarget.GetPosition() : zombieCommand.Zombie.InvestigatePosition;
								if (this.spawnHordeNear(world, zombieCommand, target))
								{
									zombieCommand.AttackDelay = 18f;
								}
								else
								{
									flag = true;
								}
							}
							else
							{
								zombieCommand.Wandering = true;
								zombieCommand.WorldExpiryTime = world.worldTime + 2000UL;
							}
						}
					}
				}
				else if (attackTarget)
				{
					if (flag2)
					{
						zombieCommand.Attacking = true;
					}
				}
				else if (zombieCommand.Wandering)
				{
					if (world.worldTime >= zombieCommand.WorldExpiryTime)
					{
						flag = true;
					}
				}
				else if (zombieCommand.Zombie.HasInvestigatePosition)
				{
					if (zombieCommand.Zombie.InvestigatePosition == zombieCommand.TargetPos)
					{
						zombieCommand.Zombie.SetInvestigatePosition(zombieCommand.TargetPos, 6000, true);
					}
				}
				else
				{
					zombieCommand.Wandering = true;
					zombieCommand.WorldExpiryTime = world.worldTime + 2000UL;
				}
			}
			if (flag)
			{
				if (zombieCommand.Horde != null)
				{
					zombieCommand.Horde.Destroy();
				}
				string str = "scout horde '";
				EntityEnemy zombie2 = zombieCommand.Zombie;
				AIDirector.LogAIExtra(str + ((zombie2 != null) ? zombie2.ToString() : null) + "' removed from control", Array.Empty<object>());
				zombieCommand.Zombie.IsHordeZombie = false;
				zombieCommand.Zombie.bIsChunkObserver = false;
				this.hordeList.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x000C5324 File Offset: 0x000C3524
	public void Cleanup()
	{
		for (int i = 0; i < this.hordeList.Count; i++)
		{
			AIScoutHordeSpawner.ZombieCommand zombieCommand = this.hordeList[i];
			zombieCommand.Zombie.IsHordeZombie = false;
			zombieCommand.Zombie.bIsChunkObserver = false;
		}
		this.hordeList.Clear();
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x000C5378 File Offset: 0x000C3578
	[PublicizedFrom(EAccessModifier.Private)]
	public bool spawnHordeNear(World world, AIScoutHordeSpawner.ZombieCommand command, Vector3 target)
	{
		AIDirector.LogAI("Scout spawned a zombie horde", Array.Empty<object>());
		if (command.Horde == null)
		{
			AIDirectorChunkEventComponent component = world.GetAIDirector().GetComponent<AIDirectorChunkEventComponent>();
			command.Horde = component.CreateHorde(target);
		}
		if (command.Horde.canSpawnMore)
		{
			int num = 5;
			if (world.aiDirector.random.RandomFloat < 0.12f)
			{
				num--;
				if (this.spawner.CurrentWave > 0)
				{
					Vector3 vector = this.endPos;
					this.spawner.ResetSpawner();
					this.spawner.numberToSpawnThisWave = 1;
					this.endPos = target;
					this.SpawnUpdate(world);
					this.endPos = vector;
				}
				else
				{
					this.spawner.numberToSpawnThisWave++;
				}
			}
			command.Horde.SpawnMore(num);
			command.Zombie.PlayOneShot(command.Zombie.GetSoundAlert(), false, false, false, null, 1f);
		}
		command.Horde.SetSpawnPos(target);
		return command.Horde.canSpawnMore || command.Horde.isSpawning;
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x000C548C File Offset: 0x000C368C
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector3 CalcRandomPos(AIDirector director, Vector3 target, float radius)
	{
		Vector2 vector = director.random.RandomOnUnitCircle * radius;
		return target + new Vector3(vector.x, 0f, vector.y);
	}

	// Token: 0x04001651 RID: 5713
	[PublicizedFrom(EAccessModifier.Private)]
	public EntitySpawner spawner;

	// Token: 0x04001652 RID: 5714
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startPos;

	// Token: 0x04001653 RID: 5715
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 endPos;

	// Token: 0x04001654 RID: 5716
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBloodMoon;

	// Token: 0x04001655 RID: 5717
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Entity> spawnedList = new List<Entity>();

	// Token: 0x04001656 RID: 5718
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIScoutHordeSpawner.ZombieCommand> hordeList = new List<AIScoutHordeSpawner.ZombieCommand>();

	// Token: 0x02000422 RID: 1058
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class ZombieCommand
	{
		// Token: 0x04001657 RID: 5719
		public EntityEnemy Zombie;

		// Token: 0x04001658 RID: 5720
		public ulong WorldExpiryTime;

		// Token: 0x04001659 RID: 5721
		public Vector3 TargetPos;

		// Token: 0x0400165A RID: 5722
		public bool Wandering;

		// Token: 0x0400165B RID: 5723
		public bool Attacking;

		// Token: 0x0400165C RID: 5724
		public float AttackDelay;

		// Token: 0x0400165D RID: 5725
		public AIScoutHordeSpawner.IHorde Horde;
	}

	// Token: 0x02000423 RID: 1059
	public interface IHorde
	{
		// Token: 0x060020B0 RID: 8368
		void SpawnMore(int size);

		// Token: 0x060020B1 RID: 8369
		void SetSpawnPos(Vector3 pos);

		// Token: 0x060020B2 RID: 8370
		void Destroy();

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x060020B3 RID: 8371
		bool canSpawnMore { get; }

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x060020B4 RID: 8372
		bool isSpawning { get; }
	}
}
