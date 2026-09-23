using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000AE0 RID: 2784
public class EntitySpawner
{
	// Token: 0x17000907 RID: 2311
	// (get) Token: 0x06005330 RID: 21296 RVA: 0x001FCF7B File Offset: 0x001FB17B
	public int CurrentWave
	{
		get
		{
			return this.currentWave;
		}
	}

	// Token: 0x06005331 RID: 21297 RVA: 0x001FCF83 File Offset: 0x001FB183
	public static int ModifySpawnCountByGameDifficulty(int count)
	{
		if (!EntityFactory.EnemySpawnMode)
		{
			return 0;
		}
		return count;
	}

	// Token: 0x06005332 RID: 21298 RVA: 0x001FCF90 File Offset: 0x001FB190
	public EntitySpawner()
	{
		this.position = Vector3i.zero;
		this.size = Vector3i.one;
		this.totalSpawnedThisWave = 0;
		this.timeDelayToNextWave = 0f;
		this.timeDelayBetweenSpawns = 0f;
		this.currentWave = 0;
		this.numberToSpawnThisWave = 0;
		this.lastDaySpawnCalled = -1;
		this.bCaveSpawn = false;
		PList<int> plist = new PList<int>();
		plist.writeElement = delegate(BinaryWriter _bw, int _el)
		{
			_bw.Write(_el);
		};
		plist.readElement = ((BinaryReader _br, uint _version) => _br.ReadInt32());
		this.entityIdSpawned = plist;
	}

	// Token: 0x06005333 RID: 21299 RVA: 0x001FD048 File Offset: 0x001FB248
	public EntitySpawner(string _esClassname, Vector3i _position, Vector3i _size, int _triggerDiameter, ICollection<int> _entityIdsAlreadySpawned = null) : this()
	{
		this.position = _position;
		this.size = _size;
		this.triggerDiameter = _triggerDiameter;
		this.entitySpawnerClassName = _esClassname;
		if (_entityIdsAlreadySpawned != null)
		{
			this.entityIdSpawned.AddRange(_entityIdsAlreadySpawned);
		}
		this.timeDelayBetweenSpawns = (EntitySpawnerClass.list.ContainsKey(this.entitySpawnerClassName) ? EntitySpawnerClass.list[this.entitySpawnerClassName].Day(0).delayBetweenSpawns : 0f);
	}

	// Token: 0x06005334 RID: 21300 RVA: 0x001FD0C3 File Offset: 0x001FB2C3
	public ICollection<int> GetEntityIdsSpaned()
	{
		return this.entityIdSpawned;
	}

	// Token: 0x06005335 RID: 21301 RVA: 0x001FD0CC File Offset: 0x001FB2CC
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(3);
		_bw.Write(this.position.x);
		_bw.Write(this.position.y);
		_bw.Write(this.position.z);
		_bw.Write((short)this.size.x);
		_bw.Write((short)this.size.y);
		_bw.Write((short)this.size.z);
		_bw.Write((ushort)this.triggerDiameter);
		_bw.Write(this.entitySpawnerClassName);
		_bw.Write((short)this.totalSpawnedThisWave);
		_bw.Write(this.timeDelayToNextWave);
		_bw.Write(this.timeDelayBetweenSpawns);
		this.entityIdSpawned.Write(_bw);
		_bw.Write((short)this.currentWave);
		_bw.Write(this.lastDaySpawnCalled);
		_bw.Write(this.numberToSpawnThisWave);
		_bw.Write(this.worldTimeNextWave);
		_bw.Write(this.bCaveSpawn);
	}

	// Token: 0x06005336 RID: 21302 RVA: 0x001FD1D4 File Offset: 0x001FB3D4
	public void Read(BinaryReader _br)
	{
		byte b = _br.ReadByte();
		this.position = new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32());
		this.size = new Vector3i((int)_br.ReadInt16(), (int)_br.ReadInt16(), (int)_br.ReadInt16());
		this.triggerDiameter = (int)_br.ReadUInt16();
		this.entitySpawnerClassName = _br.ReadString();
		if (!EntitySpawnerClass.list.ContainsKey(this.entitySpawnerClassName))
		{
			string[] array = new string[5];
			array[0] = "Entity spawner at pos ";
			int num = 1;
			Vector3i vector3i = this.position;
			array[num] = vector3i.ToString();
			array[2] = " contains invalid spawner class reference '";
			array[3] = this.entitySpawnerClassName;
			array[4] = "'";
			Log.Warning(string.Concat(array));
			this.entitySpawnerClassName = EntitySpawnerClass.DefaultClassName.name;
		}
		this.totalSpawnedThisWave = (int)_br.ReadInt16();
		this.timeDelayToNextWave = _br.ReadSingle();
		this.timeDelayBetweenSpawns = _br.ReadSingle();
		this.entityIdSpawned.Read(_br);
		this.currentWave = (int)_br.ReadInt16();
		this.lastDaySpawnCalled = _br.ReadInt32();
		this.numberToSpawnThisWave = _br.ReadInt32();
		if (b > 1)
		{
			this.worldTimeNextWave = _br.ReadUInt64();
		}
		if (b > 2)
		{
			this.bCaveSpawn = _br.ReadBoolean();
		}
	}

	// Token: 0x06005337 RID: 21303 RVA: 0x001FD318 File Offset: 0x001FB518
	public void Spawn(World _world, int _day, bool _bSpawnEnemies)
	{
		if (!AIDirector.CanSpawn(1f))
		{
			return;
		}
		this.SpawnManually(_world, _day, _bSpawnEnemies, delegate(EntitySpawner _es, out EntityPlayer _outPlayerToAttack)
		{
			_outPlayerToAttack = null;
			EntitySpawnerClass entitySpawnerClass = EntitySpawnerClass.list[_es.entitySpawnerClassName].Day(_day);
			if (entitySpawnerClass.bIgnoreTrigger)
			{
				if (entitySpawnerClass.bAttackPlayerImmediately)
				{
					_outPlayerToAttack = _world.GetClosestPlayer((float)this.position.x, (float)this.position.y, (float)this.position.z, 0, 160.0);
				}
				return true;
			}
			for (int i = 0; i < _world.Players.list.Count; i++)
			{
				if (_outPlayerToAttack == null)
				{
					Vector3 vector = _world.Players.list[i].GetPosition();
					if (Mathf.Abs(vector.x - (float)this.position.x) <= (float)(_es.triggerDiameter / 2) && Mathf.Abs(vector.y - (float)this.position.y) <= (float)(_es.triggerDiameter / 2) && Mathf.Abs(vector.z - (float)this.position.z) <= (float)(_es.triggerDiameter / 2))
					{
						_outPlayerToAttack = _world.Players.list[i];
					}
				}
				for (int j = 0; j < _world.Players.list[i].SpawnPoints.Count; j++)
				{
					Vector3 vector2 = _world.Players.list[i].SpawnPoints[j].ToVector3();
					if (Mathf.Abs(vector2.x - (float)this.position.x) <= (float)(_es.triggerDiameter / 2) && Mathf.Abs(vector2.y - (float)this.position.y) <= (float)(_es.triggerDiameter / 2) && Mathf.Abs(vector2.z - (float)this.position.z) <= (float)(_es.triggerDiameter / 2))
					{
						_outPlayerToAttack = null;
						return false;
					}
				}
			}
			return _outPlayerToAttack != null;
		}, delegate(EntitySpawner _es, EntityPlayer _inPlayerToAttack, out EntityPlayer _outPlayerToAttack, out Vector3 _pos)
		{
			_outPlayerToAttack = _inPlayerToAttack;
			int num;
			int num2;
			int num3;
			if (this.bCaveSpawn)
			{
				if (!_world.FindRandomSpawnPointNearPositionUnderground(_es.position.ToVector3(), 16, out num, out num2, out num3, _es.size.ToVector3()))
				{
					_pos = Vector3.zero;
					return false;
				}
			}
			else
			{
				bool bSpawnOnGround = EntitySpawnerClass.list[_es.entitySpawnerClassName].Day(_day).bSpawnOnGround;
				if (!_world.FindRandomSpawnPointNearPosition(_es.position.ToVector3(), 16, out num, out num2, out num3, _es.size.ToVector3(), bSpawnOnGround, true))
				{
					_pos = Vector3.zero;
					return false;
				}
			}
			_pos = new Vector3((float)num, (float)num2, (float)num3);
			return true;
		}, null, null);
	}

	// Token: 0x06005338 RID: 21304 RVA: 0x001FD37A File Offset: 0x001FB57A
	public void ResetSpawner()
	{
		this.resetRuntimeVariables();
	}

	// Token: 0x06005339 RID: 21305 RVA: 0x001FD382 File Offset: 0x001FB582
	[PublicizedFrom(EAccessModifier.Private)]
	public void resetRuntimeVariables()
	{
		this.totalSpawnedThisWave = 0;
		this.timeDelayToNextWave = 0f;
		this.timeDelayBetweenSpawns = 0f;
		this.entityIdSpawned.Clear();
		this.currentWave = 0;
		this.numberToSpawnThisWave = 0;
	}

	// Token: 0x0600533A RID: 21306 RVA: 0x001FD3BC File Offset: 0x001FB5BC
	public void SpawnManually(World _world, int _day, bool _bSpawnEnemyEntities, EntitySpawner.ES_CheckSpawnPrecondition _checkSpawnPrecondition, EntitySpawner.ES_GetSpawnPosition _getSpawnPosition, EntitySpawner.ES_ModifySpawnCount _checkSpawnCount, List<Entity> _spawned)
	{
		if (this.entitySpawnerClassName == null)
		{
			return;
		}
		EntitySpawnerClassForDay entitySpawnerClassForDay = EntitySpawnerClass.list[this.entitySpawnerClassName];
		if (entitySpawnerClassForDay == null)
		{
			return;
		}
		EntitySpawnerClass entitySpawnerClass = entitySpawnerClassForDay.Day(_day);
		if (this.lastDaySpawnCalled != -1 && this.lastDaySpawnCalled != _day && entitySpawnerClass.bPropResetToday)
		{
			this.resetRuntimeVariables();
		}
		this.lastDaySpawnCalled = _day;
		if (entitySpawnerClass.numberOfWaves > 0 && this.currentWave >= entitySpawnerClass.numberOfWaves)
		{
			return;
		}
		if (entitySpawnerClass.spawnAtTimeOfDay != EDaytime.Any && !(_world.IsDaytime() ? (entitySpawnerClass.spawnAtTimeOfDay == EDaytime.Day) : (entitySpawnerClass.spawnAtTimeOfDay == EDaytime.Night)))
		{
			return;
		}
		float num = Time.time - this.lastTimeSpawnCalled;
		this.lastTimeSpawnCalled = Time.time;
		if (this.timeDelayToNextWave > 0f)
		{
			this.timeDelayToNextWave -= num;
			return;
		}
		if (this.timeDelayBetweenSpawns > 0f)
		{
			this.timeDelayBetweenSpawns -= num;
			if (this.timeDelayBetweenSpawns > 0f)
			{
				return;
			}
		}
		EntityPlayer entityPlayer;
		bool flag = _checkSpawnPrecondition(this, out entityPlayer);
		if (entityPlayer != null && entitySpawnerClass.daysToRespawnIfPlayerLeft != 0 && this.worldTimeNextWave != 0UL && this.worldTimeNextWave < _world.worldTime)
		{
			this.worldTimeNextWave = _world.worldTime + (ulong)((long)entitySpawnerClass.daysToRespawnIfPlayerLeft * 24000L);
		}
		if (this.worldTimeNextWave != 0UL && this.worldTimeNextWave >= _world.worldTime)
		{
			return;
		}
		this.worldTimeNextWave = 0UL;
		if (!flag)
		{
			return;
		}
		for (int i = 0; i < this.entityIdSpawned.Count; i++)
		{
			Entity entity;
			if ((entity = _world.GetEntity(this.entityIdSpawned[i])) == null || entity.IsDead())
			{
				this.entityIdSpawned.MarkToRemove(this.entityIdSpawned[i]);
			}
		}
		this.entityIdSpawned.RemoveAllMarked();
		if (this.numberToSpawnThisWave == 0)
		{
			this.numberToSpawnThisWave = _world.GetGameRandom().RandomRange(EntitySpawner.ModifySpawnCountByGameDifficulty(entitySpawnerClass.totalPerWaveMin), EntitySpawner.ModifySpawnCountByGameDifficulty(entitySpawnerClass.totalPerWaveMax + 1));
			if (_checkSpawnCount != null)
			{
				this.numberToSpawnThisWave = _checkSpawnCount(this, this.numberToSpawnThisWave);
			}
			Log.Out("Spawning this wave: " + this.numberToSpawnThisWave.ToString());
		}
		if (this.totalSpawnedThisWave >= this.numberToSpawnThisWave)
		{
			if ((float)this.entityIdSpawned.Count <= Utils.FastMax((float)this.numberToSpawnThisWave * 0.2f, 1f))
			{
				this.timeDelayToNextWave = entitySpawnerClass.delayToNextWave;
				if (entitySpawnerClass.daysToRespawnIfPlayerLeft > 0)
				{
					this.worldTimeNextWave = _world.worldTime + (ulong)((long)entitySpawnerClass.daysToRespawnIfPlayerLeft * 24000L);
				}
				Log.Out(string.Concat(new string[]
				{
					"Start a new wave '",
					this.entitySpawnerClassName,
					"'. timeout=",
					this.timeDelayToNextWave.ToCultureInvariantString(),
					"s. worldtime=",
					this.worldTimeNextWave.ToString()
				}));
				this.totalSpawnedThisWave = 0;
				this.numberToSpawnThisWave = 0;
				this.currentWave++;
			}
			return;
		}
		if (this.entityIdSpawned.Count >= entitySpawnerClass.totalAlive)
		{
			return;
		}
		int num2 = 1;
		if (entitySpawnerClass.delayBetweenSpawns == 0f)
		{
			num2 = Utils.FastMin(this.numberToSpawnThisWave, entitySpawnerClass.totalAlive - this.entityIdSpawned.Count);
		}
		for (int j = 0; j < num2; j++)
		{
			Vector3 vector;
			if (_getSpawnPosition(this, entityPlayer, out entityPlayer, out vector))
			{
				int randomFromGroup = EntityGroups.GetRandomFromGroup(entitySpawnerClass.entityGroupName, ref this.lastClassId, null);
				if (_bSpawnEnemyEntities || !EntityClass.list[randomFromGroup].bIsEnemyEntity)
				{
					Entity entity2 = EntityFactory.CreateEntity(randomFromGroup, vector, new Vector3(0f, _world.GetGameRandom().RandomFloat * 360f, 0f));
					entity2.SetSpawnerSource(entitySpawnerClassForDay.bDynamicSpawner ? EnumSpawnerSource.Dynamic : EnumSpawnerSource.StaticSpawner);
					_world.SpawnEntityInWorld(entity2);
					this.entityIdSpawned.Add(entity2.entityId);
					if (_spawned != null)
					{
						_spawned.Add(entity2);
					}
					if (this.totalSpawnedThisWave == 0 && entitySpawnerClass.startSound != null && entitySpawnerClass.startSound.Length > 0)
					{
						_world.GetGameManager().PlaySoundAtPositionServer(this.position.ToVector3(), entitySpawnerClass.startSound, AudioRolloffMode.Custom, 300, 1f);
					}
					this.totalSpawnedThisWave++;
					EntityAlive entityAlive = entity2 as EntityAlive;
					if (entityAlive)
					{
						if (entitySpawnerClass.bAttackPlayerImmediately && entityPlayer != null)
						{
							entityAlive.SetRevengeTarget(entityPlayer);
						}
						if (entitySpawnerClass.bTerritorial)
						{
							entityAlive.setHomeArea(new Vector3i(vector), entitySpawnerClass.territorialRange);
						}
					}
					string[] array = new string[10];
					array[0] = "Spawned ";
					int num3 = 1;
					Entity entity3 = entity2;
					array[num3] = ((entity3 != null) ? entity3.ToString() : null);
					array[2] = " at ";
					array[3] = vector.ToCultureInvariantString();
					array[4] = " Day=";
					array[5] = _day.ToString();
					array[6] = " TotalInWave=";
					array[7] = this.totalSpawnedThisWave.ToString();
					array[8] = " CurrentWave=";
					array[9] = (this.currentWave + 1).ToString();
					Log.Out(string.Concat(array));
					_world.DebugAddSpawnedEntity(entity2);
					this.timeDelayBetweenSpawns = entitySpawnerClass.delayBetweenSpawns;
				}
			}
		}
	}

	// Token: 0x040040D8 RID: 16600
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFileVersion = 3;

	// Token: 0x040040D9 RID: 16601
	public Vector3i position;

	// Token: 0x040040DA RID: 16602
	public Vector3i size;

	// Token: 0x040040DB RID: 16603
	public int triggerDiameter;

	// Token: 0x040040DC RID: 16604
	public string entitySpawnerClassName;

	// Token: 0x040040DD RID: 16605
	public bool bCaveSpawn;

	// Token: 0x040040DE RID: 16606
	[PublicizedFrom(EAccessModifier.Protected)]
	public int totalSpawnedThisWave;

	// Token: 0x040040DF RID: 16607
	[PublicizedFrom(EAccessModifier.Protected)]
	public float timeDelayToNextWave;

	// Token: 0x040040E0 RID: 16608
	[PublicizedFrom(EAccessModifier.Protected)]
	public float timeDelayBetweenSpawns;

	// Token: 0x040040E1 RID: 16609
	[PublicizedFrom(EAccessModifier.Protected)]
	public ulong worldTimeNextWave;

	// Token: 0x040040E2 RID: 16610
	[PublicizedFrom(EAccessModifier.Protected)]
	public PList<int> entityIdSpawned;

	// Token: 0x040040E3 RID: 16611
	[PublicizedFrom(EAccessModifier.Protected)]
	public int currentWave;

	// Token: 0x040040E4 RID: 16612
	[PublicizedFrom(EAccessModifier.Protected)]
	public int lastDaySpawnCalled;

	// Token: 0x040040E5 RID: 16613
	public int numberToSpawnThisWave;

	// Token: 0x040040E6 RID: 16614
	[PublicizedFrom(EAccessModifier.Protected)]
	public float lastTimeSpawnCalled;

	// Token: 0x040040E7 RID: 16615
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastClassId;

	// Token: 0x02000AE1 RID: 2785
	// (Invoke) Token: 0x0600533C RID: 21308
	public delegate bool ES_CheckSpawnPrecondition(EntitySpawner _es, out EntityPlayer _outPlayerToAttack);

	// Token: 0x02000AE2 RID: 2786
	// (Invoke) Token: 0x06005340 RID: 21312
	public delegate bool ES_GetSpawnPosition(EntitySpawner _es, EntityPlayer _inPlayerToAttack, out EntityPlayer _outPlayerToAttack, out Vector3 _pos);

	// Token: 0x02000AE3 RID: 2787
	// (Invoke) Token: 0x06005344 RID: 21316
	public delegate int ES_ModifySpawnCount(EntitySpawner _es, int spawnCount);
}
