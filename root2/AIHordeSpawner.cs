using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000420 RID: 1056
[Preserve]
public class AIHordeSpawner
{
	// Token: 0x060020A4 RID: 8356 RVA: 0x000C4B82 File Offset: 0x000C2D82
	public AIHordeSpawner(World _world, string _spawnerDefinition, Vector3 _targetPos, float _playerSearchBounds)
	{
		this.world = _world;
		this.spawner = new AIDirectorGameStagePartySpawner(_world, _spawnerDefinition);
		this.playerSearchBounds = _playerSearchBounds;
		this.targetPos = _targetPos;
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x000C4BB8 File Offset: 0x000C2DB8
	public bool Tick(double _dt)
	{
		if (this.world.GetPlayers().Count == 0 || !AIDirector.CanSpawn(1f))
		{
			return true;
		}
		if (!this.isInited)
		{
			List<Entity> entitiesInBounds = this.world.GetEntitiesInBounds(typeof(EntityPlayer), BoundsUtils.BoundsForMinMax(this.targetPos.x - this.playerSearchBounds, this.targetPos.y - this.playerSearchBounds, this.targetPos.z - this.playerSearchBounds, this.targetPos.x + this.playerSearchBounds, this.targetPos.y + this.playerSearchBounds, this.targetPos.z + this.playerSearchBounds), new List<Entity>());
			for (int i = 0; i < entitiesInBounds.Count; i++)
			{
				EntityPlayer entityPlayer = (EntityPlayer)entitiesInBounds[i];
				if (!entityPlayer.IsIgnoredByAI())
				{
					this.spawner.AddMember(entityPlayer);
				}
			}
			if (this.spawner.partyMembers.Count == 0)
			{
				return false;
			}
			this.isInited = true;
			this.spawner.ResetPartyLevel(0);
			this.spawner.ClearMembers();
		}
		if (!this.spawner.Tick(_dt))
		{
			return true;
		}
		if (!this.spawner.canSpawn || this.numSpawned >= this.numToSpawn)
		{
			return false;
		}
		Vector3 transformPos;
		if (this.world.IsDaytime())
		{
			if (!this.world.GetMobRandomSpawnPosWithWater(this.targetPos, 45, 55, 45, true, out transformPos))
			{
				return false;
			}
		}
		else if (!this.world.GetMobRandomSpawnPosWithWater(this.targetPos, 55, 70, 55, true, out transformPos))
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
		Log.Out("Screamer spawned {0} from {1}", new object[]
		{
			entityEnemy.EntityName,
			this.spawner.spawnGroupName
		});
		entityEnemy.SetSpawnerSource(EnumSpawnerSource.Dynamic);
		this.world.SpawnEntityInWorld(entityEnemy);
		entityEnemy.IsHordeZombie = true;
		entityEnemy.bIsChunkObserver = true;
		entityEnemy.SetInvestigatePosition(AIWanderingHordeSpawner.RandomPos(this.world.aiDirector, this.targetPos, 3f), 2400, true);
		this.hordeList.Add(entityEnemy);
		this.spawner.IncSpawnCount();
		this.numSpawned++;
		return false;
	}

	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x060020A6 RID: 8358 RVA: 0x000C4E49 File Offset: 0x000C3049
	public bool isSpawning
	{
		get
		{
			return this.spawner.canSpawn;
		}
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x000C4E58 File Offset: 0x000C3058
	public void Cleanup()
	{
		for (int i = 0; i < this.hordeList.Count; i++)
		{
			EntityEnemy entityEnemy = this.hordeList[i];
			entityEnemy.IsHordeZombie = false;
			entityEnemy.bIsChunkObserver = false;
		}
		this.hordeList.Clear();
	}

	// Token: 0x04001648 RID: 5704
	public Vector3 targetPos;

	// Token: 0x04001649 RID: 5705
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorGameStagePartySpawner spawner;

	// Token: 0x0400164A RID: 5706
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastClassId;

	// Token: 0x0400164B RID: 5707
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityEnemy> hordeList = new List<EntityEnemy>();

	// Token: 0x0400164C RID: 5708
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x0400164D RID: 5709
	[PublicizedFrom(EAccessModifier.Private)]
	public float playerSearchBounds;

	// Token: 0x0400164E RID: 5710
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isInited;

	// Token: 0x0400164F RID: 5711
	public int numToSpawn;

	// Token: 0x04001650 RID: 5712
	[PublicizedFrom(EAccessModifier.Private)]
	public int numSpawned;
}
