using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000408 RID: 1032
[Preserve]
public class AIDirectorBloodMoonParty
{
	// Token: 0x06001FF7 RID: 8183 RVA: 0x000C1B1C File Offset: 0x000BFD1C
	public AIDirectorBloodMoonParty(EntityPlayer _initialPlayer, AIDirectorBloodMoonComponent _controller, int _bloodMoonCountUNUSED)
	{
		this.spawnWorld = _initialPlayer.world;
		this.spawnBasePos = _initialPlayer.position;
		this.controller = _controller;
		this.partySpawner = new AIDirectorGameStagePartySpawner(_controller.Director.World, "BloodMoonHorde");
		this.partySpawner.AddMember(_initialPlayer);
		_initialPlayer.bloodMoonParty = this;
		this.spawnBaseDir = _controller.Random.RandomRange(0, 360);
		this.groupIndex = -1;
	}

	// Token: 0x06001FF8 RID: 8184 RVA: 0x000C1BA8 File Offset: 0x000BFDA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcBestDir(Vector3 basePos)
	{
		int[] array = new int[16];
		int num = 0;
		for (int i = 0; i < 16; i++)
		{
			float num2 = (float)i * 22.5f;
			this.spawnDirectionV = Quaternion.AngleAxis(num2, Vector3.up) * Vector3.forward * 40f;
			int num3 = 0;
			for (int j = 0; j < 9; j++)
			{
				Vector3 vector;
				if (this.spawnWorld.GetRandomSpawnPositionMinMaxToPosition(basePos + this.spawnDirectionV, 0, 10, 30, false, out vector, -1, true, 30, false, EnumLandClaimOwner.None, false))
				{
					num3++;
				}
			}
			if (num3 > 0)
			{
				num3 = (num3 + 2) / 3;
				if (Utils.FastAbs(Mathf.DeltaAngle(num2, (float)this.spawnBaseDir)) <= 60f)
				{
					num3 *= 3;
				}
			}
			array[i] = num3;
			num = Utils.FastMax(num, num3);
		}
		int num4 = 0;
		for (int k = 0; k < 16; k++)
		{
			if (array[k] == num)
			{
				num4++;
			}
		}
		int num5 = 0;
		int num6 = this.controller.Random.RandomRange(0, num4);
		for (int l = 0; l < 16; l++)
		{
			if (array[l] >= num && --num6 < 0)
			{
				num5 = l;
				break;
			}
		}
		this.spawnDirectionV = Quaternion.AngleAxis((float)num5 * 22.5f, Vector3.up) * Vector3.forward * 40f;
	}

	// Token: 0x06001FF9 RID: 8185 RVA: 0x000C1D0C File Offset: 0x000BFF0C
	public bool Tick(World _world, double _dt, bool _canSpawn)
	{
		if (this.partySpawner.partyLevel < 0)
		{
			this.InitParty();
		}
		for (int i = this.zombies.Count - 1; i >= 0; i--)
		{
			AIDirectorBloodMoonParty.ManagedZombie managedZombie = this.zombies[i];
			managedZombie.updateDelay -= (float)_dt;
			if (managedZombie.updateDelay <= 0f)
			{
				managedZombie.updateDelay = 1.8f;
				if (!this.SeekTarget(managedZombie))
				{
					this.zombies.RemoveAt(i);
				}
			}
		}
		this.partySpawner.Tick(_dt);
		bool result = false;
		if (_canSpawn)
		{
			if (!this.partySpawner.canSpawn || this.partySpawner.partyMembers.Count == 0)
			{
				return true;
			}
			if (AIDirector.CanSpawn(1.9f))
			{
				int num = this.partySpawner.groupIndex;
				if (num != this.groupIndex)
				{
					this.groupIndex = num;
					this.spawnBaseDir += 120;
					this.CalcBestDir(this.spawnBasePos);
				}
				result = true;
				int count = this.partySpawner.partyMembers.Count;
				int num2 = Utils.FastMin(this.partySpawner.maxAlive, this.enemyActiveMax);
				if (this.zombies.Count < num2)
				{
					for (int j = Utils.FastMin(count, 3); j > 0; j--)
					{
						if (this.nextPlayer >= count)
						{
							this.nextPlayer = 0;
						}
						EntityPlayer entityPlayer = this.partySpawner.partyMembers[this.nextPlayer];
						bool flag = false;
						if (this.IsPlayerATarget(entityPlayer))
						{
							flag = this.SpawnZombie(_world, entityPlayer, entityPlayer.position, this.spawnDirectionV);
						}
						this.nextPlayer++;
						if (flag)
						{
							break;
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x000C1EBD File Offset: 0x000C00BD
	public void PlayerLoggedOut(EntityPlayer _player)
	{
		this.partySpawner.RemoveMember(_player, false);
		if (this.nextPlayer >= this.partySpawner.partyMembers.Count)
		{
			this.nextPlayer = 0;
		}
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x000C1EEC File Offset: 0x000C00EC
	public void KillPartyZombies()
	{
		int count = this.zombies.Count;
		if (count > 0)
		{
			this.partySpawner.DecSpawnCount(count);
			for (int i = 0; i < count; i++)
			{
				EntityEnemy zombie = this.zombies[i].zombie;
				if (zombie && !zombie.IsDead() && !zombie.IsDespawned && zombie.gameObject)
				{
					zombie.Kill(DamageResponse.New(true));
				}
			}
			this.zombies.Clear();
		}
	}

	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x06001FFC RID: 8188 RVA: 0x000C1F6F File Offset: 0x000C016F
	public bool IsEmpty
	{
		get
		{
			return this.partySpawner.partyMembers.Count <= 0;
		}
	}

	// Token: 0x06001FFD RID: 8189 RVA: 0x000C1F87 File Offset: 0x000C0187
	public bool IsMemberOfParty(int _entityID)
	{
		return this.partySpawner.IsMemberOfParty(_entityID);
	}

	// Token: 0x06001FFE RID: 8190 RVA: 0x000C1F98 File Offset: 0x000C0198
	public bool TryAddPlayer(EntityPlayer _player)
	{
		for (int i = 0; i < this.partySpawner.partyMembers.Count; i++)
		{
			if ((this.partySpawner.partyMembers[i].GetPosition() - _player.GetPosition()).sqrMagnitude <= 6400f)
			{
				this.AddPlayer(_player);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x000C1FFA File Offset: 0x000C01FA
	public void AddPlayer(EntityPlayer _player)
	{
		this.partySpawner.AddMember(_player);
		_player.bloodMoonParty = this;
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x000C2010 File Offset: 0x000C0210
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitParty()
	{
		int num = this.partySpawner.CalcPartyLevel();
		int num2 = AIDirectorBloodMoonComponent.BloodMoonEnemyCount * this.partySpawner.partyMembers.Count;
		this.enemyActiveMax = Utils.FastMin(30, num2);
		float num3 = Utils.FastMax(1f, (float)num2 / (float)this.enemyActiveMax);
		num3 = Utils.FastLerp(1f, num3, (float)num / 60f);
		this.partySpawner.SetScaling(num3);
		this.partySpawner.SetPartyLevel(num);
		this.bonusLootSpawnCount = this.partySpawner.bonusLootEvery / 2;
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x000C20A4 File Offset: 0x000C02A4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool SpawnZombie(World _world, EntityPlayer _target, Vector3 _focusPos, Vector3 _radiusV)
	{
		Vector3 vector;
		if (!this.CalcSpawnPos(_world, _focusPos, _radiusV, out vector))
		{
			return false;
		}
		bool flag = true;
		int num = EntityGroups.GetRandomEntityFromGroupMaxTier(this.partySpawner.spawnGroupName, EntityFactory.MaxEntityTier, ref this.lastClassId, true, false, null);
		if (_target.AttachedToEntity && this.controller.Random.RandomFloat < 0.5f)
		{
			flag = false;
			num = EntityClass.FromString("animalZombieVultureRadiated");
		}
		if (num == -1)
		{
			Log.Warning(string.Format("Could not spawn an entity from group {0} within Sandbox Options Max Tier Limit {1}.", this.partySpawner.spawnGroupName, EntityFactory.MaxEntityTier));
			return false;
		}
		EntityEnemy entityEnemy = (EntityEnemy)EntityFactory.CreateEntity(num, vector);
		entityEnemy.SetSpawnerSource(EnumSpawnerSource.Dynamic);
		_world.SpawnEntityInWorld(entityEnemy);
		entityEnemy.IsHordeZombie = true;
		entityEnemy.IsBloodMoon = true;
		entityEnemy.bIsChunkObserver = true;
		entityEnemy.timeStayAfterDeath /= 3;
		if (flag)
		{
			int num2 = this.bonusLootSpawnCount + 1;
			this.bonusLootSpawnCount = num2;
			if (num2 >= this.partySpawner.bonusLootEvery)
			{
				this.bonusLootSpawnCount = 0;
				entityEnemy.lootDropProb *= GameStageDefinition.LootBonusScale;
			}
		}
		AIDirectorBloodMoonParty.ManagedZombie managedZombie = new AIDirectorBloodMoonParty.ManagedZombie(entityEnemy, _target);
		this.zombies.Add(managedZombie);
		this.SeekTarget(managedZombie);
		this.partySpawner.IncSpawnCount();
		AstarManager.Instance.AddLocation(vector, 40);
		ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(_world.worldTime);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		Log.Out("BloodMoonParty: SpawnZombie grp {0}, cnt {1}, {2}, loot {3}, at player {4}, day/time {5} {6:D2}:{7:D2}", new object[]
		{
			this.partySpawner.ToString(),
			this.zombies.Count,
			entityEnemy.EntityName,
			entityEnemy.lootDropProb,
			_target.entityId,
			item,
			item2,
			item3
		});
		return true;
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000C2288 File Offset: 0x000C0488
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CalcSpawnPos(World _world, Vector3 _focusPos, Vector3 _radiusV, out Vector3 spawnPos)
	{
		_radiusV = Quaternion.AngleAxis((this.controller.Random.RandomFloat - 0.5f) * 90f, Vector3.up) * _radiusV;
		return _world.GetMobRandomSpawnPosWithWater(_focusPos + _radiusV, 0, 10, 30, false, out spawnPos);
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x000C22E0 File Offset: 0x000C04E0
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer FindPartyTarget(Vector3 fromPos)
	{
		float num = float.MaxValue;
		EntityPlayer result = null;
		for (int i = this.partySpawner.partyMembers.Count - 1; i >= 0; i--)
		{
			EntityPlayer entityPlayer = this.partySpawner.partyMembers[i];
			if (this.IsPlayerATarget(entityPlayer))
			{
				float sqrMagnitude = (fromPos - entityPlayer.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = entityPlayer;
				}
			}
		}
		return result;
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x000C2350 File Offset: 0x000C0550
	[PublicizedFrom(EAccessModifier.Private)]
	public bool SeekTarget(AIDirectorBloodMoonParty.ManagedZombie mz)
	{
		EntityAlive zombie = mz.zombie;
		if (!zombie || zombie.IsDead() || zombie.IsDespawned || !zombie.gameObject)
		{
			return false;
		}
		EntityPlayer entityPlayer = zombie.GetAttackTarget() as EntityPlayer;
		if (entityPlayer)
		{
			mz.player = entityPlayer;
		}
		if (!mz.player || !this.IsPlayerATarget(mz.player))
		{
			mz.player = this.FindPartyTarget(zombie.position);
		}
		if (mz.player)
		{
			Vector3 vector = zombie.position - mz.player.position;
			float sqrMagnitude = vector.sqrMagnitude;
			vector.y = 0f;
			Vector3 vector2;
			if (vector.sqrMagnitude >= 22500f && this.CalcSpawnPos(zombie.world, mz.player.position, this.spawnDirectionV, out vector2) && !zombie.world.IsPlayerAliveAndNear(zombie.position, 70f))
			{
				if (this.controller.Random.RandomFloat < 0.5f)
				{
					this.partySpawner.DecSpawnCount(1);
					zombie.lootDropProb = 0f;
					zombie.Kill(DamageResponse.New(true));
					return false;
				}
				zombie.SetPosition(vector2, true);
				zombie.moveHelper.Stop();
				Log.Warning("SeekTarget {0}, far, move {1}", new object[]
				{
					zombie.GetDebugName(),
					vector2
				});
			}
			if (sqrMagnitude <= 10000f || entityPlayer != mz.player)
			{
				zombie.SetAttackTarget(mz.player, 1200);
			}
			else
			{
				if (entityPlayer)
				{
					zombie.SetAttackTarget(null, 0);
				}
				zombie.SetInvestigatePosition(mz.player.position, 1200, true);
			}
			return true;
		}
		if (!zombie.world.IsPlayerAliveAndNear(zombie.position, 60f))
		{
			zombie.Kill(DamageResponse.New(true));
			return false;
		}
		return true;
	}

	// Token: 0x06002005 RID: 8197 RVA: 0x000C2544 File Offset: 0x000C0744
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsPlayerATarget(EntityPlayer player)
	{
		return !player.IsDead() && player.IsSpawned() && player.entityId != -1 && !player.IsIgnoredByAI() && player.Progression.Level > 1 && !player.IsBloodMoonDead;
	}

	// Token: 0x170003BA RID: 954
	// (get) Token: 0x06002006 RID: 8198 RVA: 0x000C2584 File Offset: 0x000C0784
	public bool BloodmoonZombiesRemain
	{
		get
		{
			return this.zombies.Count > 0;
		}
	}

	// Token: 0x040015A9 RID: 5545
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cPartyJoinDistance = 80f;

	// Token: 0x040015AA RID: 5546
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cPartyJoinDistanceSq = 6400f;

	// Token: 0x040015AB RID: 5547
	public const float cSightDist = 100f;

	// Token: 0x040015AC RID: 5548
	public const float cSightDistSq = 10000f;

	// Token: 0x040015AD RID: 5549
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTeleportDist = 150f;

	// Token: 0x040015AE RID: 5550
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTeleportDistSq = 22500f;

	// Token: 0x040015AF RID: 5551
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSpawnPreferredArc = 120;

	// Token: 0x040015B0 RID: 5552
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSpawnAngle = 90f;

	// Token: 0x040015B1 RID: 5553
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSpawnDistance = 40f;

	// Token: 0x040015B2 RID: 5554
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSpawnMinRandDistance = 0;

	// Token: 0x040015B3 RID: 5555
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSpawnMaxRandDistance = 10;

	// Token: 0x040015B4 RID: 5556
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSpawnMinPlayerDistance = 30;

	// Token: 0x040015B5 RID: 5557
	public AIDirectorGameStagePartySpawner partySpawner;

	// Token: 0x040015B6 RID: 5558
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastClassId;

	// Token: 0x040015B7 RID: 5559
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIDirectorBloodMoonParty.ManagedZombie> zombies = new List<AIDirectorBloodMoonParty.ManagedZombie>();

	// Token: 0x040015B8 RID: 5560
	[PublicizedFrom(EAccessModifier.Private)]
	public World spawnWorld;

	// Token: 0x040015B9 RID: 5561
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 spawnBasePos;

	// Token: 0x040015BA RID: 5562
	[PublicizedFrom(EAccessModifier.Private)]
	public int spawnBaseDir;

	// Token: 0x040015BB RID: 5563
	[PublicizedFrom(EAccessModifier.Private)]
	public int enemyActiveMax;

	// Token: 0x040015BC RID: 5564
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorBloodMoonComponent controller;

	// Token: 0x040015BD RID: 5565
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 spawnDirectionV;

	// Token: 0x040015BE RID: 5566
	[PublicizedFrom(EAccessModifier.Private)]
	public int nextPlayer;

	// Token: 0x040015BF RID: 5567
	[PublicizedFrom(EAccessModifier.Private)]
	public int groupIndex;

	// Token: 0x040015C0 RID: 5568
	[PublicizedFrom(EAccessModifier.Private)]
	public int bonusLootSpawnCount;

	// Token: 0x02000409 RID: 1033
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class ManagedZombie
	{
		// Token: 0x06002007 RID: 8199 RVA: 0x000C2594 File Offset: 0x000C0794
		public ManagedZombie(EntityEnemy _zombie, EntityPlayer _player)
		{
			this.zombie = _zombie;
			this.player = _player;
		}

		// Token: 0x040015C1 RID: 5569
		public EntityPlayer player;

		// Token: 0x040015C2 RID: 5570
		public EntityEnemy zombie;

		// Token: 0x040015C3 RID: 5571
		public float updateDelay;
	}
}
