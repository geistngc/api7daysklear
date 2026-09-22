using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200040E RID: 1038
[Preserve]
public class AIDirectorChunkEventComponent : AIDirectorHordeComponent
{
	// Token: 0x0600201C RID: 8220 RVA: 0x000C2A39 File Offset: 0x000C0C39
	public void Clear()
	{
		this.activeChunks.Clear();
		this.checkChunks.Clear();
	}

	// Token: 0x0600201D RID: 8221 RVA: 0x000C2A54 File Offset: 0x000C0C54
	public override void Tick(double _dt)
	{
		base.Tick(_dt);
		float num = (float)_dt;
		this.spawnDelay -= num;
		if (this.spawnDelay <= 0f)
		{
			this.spawnDelay = 5f;
			this.CheckToSpawn();
			foreach (KeyValuePair<long, AIDirectorChunkData> keyValuePair in this.activeChunks)
			{
				if (!keyValuePair.Value.Tick(5f))
				{
					this.removeChunks.Add(keyValuePair.Key);
				}
			}
			if (this.removeChunks.Count > 0)
			{
				for (int i = 0; i < this.removeChunks.Count; i++)
				{
					this.activeChunks.Remove(this.removeChunks[i]);
				}
				this.removeChunks.Clear();
			}
		}
		this.TickActiveSpawns(num);
	}

	// Token: 0x0600201E RID: 8222 RVA: 0x000C2B4C File Offset: 0x000C0D4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickActiveSpawns(float dt)
	{
		for (int i = this.scoutSpawnList.Count - 1; i >= 0; i--)
		{
			if (this.scoutSpawnList[i].Update(this.Director.World, dt))
			{
				AIDirector.LogAIExtra("Scout horde spawn finished (all mobs spawned)", Array.Empty<object>());
				this.scoutSpawnList[i].Cleanup();
				this.scoutSpawnList.RemoveAt(i);
			}
		}
		for (int j = this.hordeSpawnList.Count - 1; j >= 0; j--)
		{
			if (this.hordeSpawnList[j].Tick((double)dt))
			{
				AIDirector.LogAIExtra("Scout triggered horde finished (all mobs spawned)", Array.Empty<object>());
				this.hordeSpawnList.RemoveAt(j);
			}
		}
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x000C2C04 File Offset: 0x000C0E04
	public override void Read(BinaryReader _stream, int _outerVersion)
	{
		if (_outerVersion >= 5)
		{
			this.activeChunks.Clear();
			int outerVersion = _stream.ReadInt32();
			int num = _stream.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				long key = _stream.ReadInt64();
				AIDirectorChunkData aidirectorChunkData = new AIDirectorChunkData();
				aidirectorChunkData.Read(_stream, outerVersion);
				this.activeChunks[key] = aidirectorChunkData;
			}
		}
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x000C2C60 File Offset: 0x000C0E60
	public override void Write(BinaryWriter _stream)
	{
		_stream.Write(1);
		_stream.Write(this.activeChunks.Count);
		foreach (KeyValuePair<long, AIDirectorChunkData> keyValuePair in this.activeChunks)
		{
			_stream.Write(keyValuePair.Key);
			keyValuePair.Value.Write(_stream);
		}
	}

	// Token: 0x06002021 RID: 8225 RVA: 0x000C2CE0 File Offset: 0x000C0EE0
	public int GetActiveCount()
	{
		return this.activeChunks.Count;
	}

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x06002022 RID: 8226 RVA: 0x000C2CED File Offset: 0x000C0EED
	public bool HasAnySpawns
	{
		get
		{
			return this.hordeSpawnList.Count != 0;
		}
	}

	// Token: 0x06002023 RID: 8227 RVA: 0x000C2D00 File Offset: 0x000C0F00
	public AIDirectorChunkData GetChunkDataFromPosition(Vector3i _position, bool _createIfNeeded)
	{
		int x = World.toChunkXZ(_position.x) / 5;
		int y = World.toChunkXZ(_position.z) / 5;
		long key = WorldChunkCache.MakeChunkKey(x, y);
		AIDirectorChunkData aidirectorChunkData;
		if (this.activeChunks.TryGetValue(key, out aidirectorChunkData))
		{
			return aidirectorChunkData;
		}
		if (_createIfNeeded)
		{
			aidirectorChunkData = new AIDirectorChunkData();
			this.activeChunks[key] = aidirectorChunkData;
		}
		return aidirectorChunkData;
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x000C2D58 File Offset: 0x000C0F58
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartCooldownOnNeighbors(Vector3i _position, bool _isLong)
	{
		int num = World.toChunkXZ(_position.x) / 5;
		int num2 = World.toChunkXZ(_position.z) / 5;
		for (int i = 0; i < AIDirectorChunkEventComponent.neighbors.Length; i += 2)
		{
			long key = WorldChunkCache.MakeChunkKey(num + AIDirectorChunkEventComponent.neighbors[i], num2 + AIDirectorChunkEventComponent.neighbors[i + 1]);
			AIDirectorChunkData aidirectorChunkData;
			if (!this.activeChunks.TryGetValue(key, out aidirectorChunkData))
			{
				aidirectorChunkData = new AIDirectorChunkData();
				this.activeChunks[key] = aidirectorChunkData;
			}
			aidirectorChunkData.StartNeighborCooldown(_isLong);
		}
	}

	// Token: 0x06002025 RID: 8229 RVA: 0x000C2DDC File Offset: 0x000C0FDC
	public void NotifyEvent(AIDirectorChunkEvent _chunkEvent)
	{
		AIDirectorChunkData chunkDataFromPosition = this.GetChunkDataFromPosition(_chunkEvent.Position, true);
		if (chunkDataFromPosition.IsReady)
		{
			chunkDataFromPosition.AddEvent(_chunkEvent);
			if (!this.checkChunks.Contains(chunkDataFromPosition))
			{
				this.checkChunks.Add(chunkDataFromPosition);
			}
		}
	}

	// Token: 0x06002026 RID: 8230 RVA: 0x000C2E20 File Offset: 0x000C1020
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckToSpawn()
	{
		if (this.checkChunks.Count > 0)
		{
			AIDirectorChunkData chunkData = this.checkChunks[0];
			this.checkChunks.RemoveAt(0);
			this.CheckToSpawn(chunkData);
		}
	}

	// Token: 0x06002027 RID: 8231 RVA: 0x000C2E5C File Offset: 0x000C105C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckToSpawn(AIDirectorChunkData _chunkData)
	{
		if (GameStats.GetBool(EnumGameStats.ZombieHordeMeter) && GameStats.GetBool(EnumGameStats.IsSpawnEnemies) && _chunkData.ActivityLevel >= 25f)
		{
			AIDirectorChunkEvent aidirectorChunkEvent = _chunkData.FindBestEventAndReset();
			if (aidirectorChunkEvent != null)
			{
				bool flag = this.Director.random.RandomFloat < 0.2f && !GameUtils.IsPlaytesting();
				this.StartCooldownOnNeighbors(aidirectorChunkEvent.Position, flag);
				if (flag)
				{
					_chunkData.SetLongDelay();
					this.SpawnScouts(aidirectorChunkEvent.Position.ToVector3());
					return;
				}
			}
			else
			{
				AIDirector.LogAI("Chunk event not found!", Array.Empty<object>());
			}
		}
	}

	// Token: 0x06002028 RID: 8232 RVA: 0x000C2EEC File Offset: 0x000C10EC
	public void SpawnScouts(Vector3 targetPos)
	{
		Vector3 vector;
		if (base.FindScoutStartPos(targetPos, out vector))
		{
			EntityPlayer closestPlayer = this.Director.World.GetClosestPlayer(targetPos, 120f, false);
			if (closestPlayer)
			{
				int num = GameStageDefinition.CalcGameStageAround(closestPlayer);
				string text = "ScoutsRadiated";
				if (num < 45)
				{
					text = "Scouts1";
				}
				else if (num < 85)
				{
					text = "Scouts2";
				}
				else if (num < 125)
				{
					text = "ScoutsFeral";
				}
				EntitySpawner spawner = new EntitySpawner(text, Vector3i.zero, Vector3i.zero, 0, null);
				this.scoutSpawnList.Add(new AIScoutHordeSpawner(spawner, vector, targetPos, false));
				AIDirector.LogAI("Spawning {0} at {1}, to {2}", new object[]
				{
					text,
					vector.ToCultureInvariantString(),
					targetPos.ToCultureInvariantString()
				});
				return;
			}
		}
		else
		{
			AIDirector.LogAI("Scout spawning failed", Array.Empty<object>());
		}
	}

	// Token: 0x06002029 RID: 8233 RVA: 0x000C2FBC File Offset: 0x000C11BC
	public AIScoutHordeSpawner.IHorde CreateHorde(Vector3 startPos)
	{
		AIDirectorChunkEventComponent.Horde horde = new AIDirectorChunkEventComponent.Horde(this, startPos);
		this.hordeSpawnList.Add(horde);
		return horde;
	}

	// Token: 0x040015DA RID: 5594
	public const int cVersion = 1;

	// Token: 0x040015DB RID: 5595
	public const int cChunksPerArea = 5;

	// Token: 0x040015DC RID: 5596
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cEventDelay = 5f;

	// Token: 0x040015DD RID: 5597
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cActivityLevelToSpawn = 25f;

	// Token: 0x040015DE RID: 5598
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSpawnChance = 0.2f;

	// Token: 0x040015DF RID: 5599
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<long, AIDirectorChunkData> activeChunks = new Dictionary<long, AIDirectorChunkData>();

	// Token: 0x040015E0 RID: 5600
	[PublicizedFrom(EAccessModifier.Private)]
	public List<long> removeChunks = new List<long>();

	// Token: 0x040015E1 RID: 5601
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIScoutHordeSpawner> scoutSpawnList = new List<AIScoutHordeSpawner>();

	// Token: 0x040015E2 RID: 5602
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIDirectorChunkEventComponent.Horde> hordeSpawnList = new List<AIDirectorChunkEventComponent.Horde>();

	// Token: 0x040015E3 RID: 5603
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIDirectorChunkData> checkChunks = new List<AIDirectorChunkData>();

	// Token: 0x040015E4 RID: 5604
	[PublicizedFrom(EAccessModifier.Private)]
	public float spawnDelay;

	// Token: 0x040015E5 RID: 5605
	[PublicizedFrom(EAccessModifier.Private)]
	public static int[] neighbors = new int[]
	{
		-1,
		0,
		1,
		0,
		0,
		-1,
		0,
		1,
		-1,
		-1,
		1,
		-1,
		-1,
		1,
		1,
		1
	};

	// Token: 0x0200040F RID: 1039
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class Horde : AIScoutHordeSpawner.IHorde
	{
		// Token: 0x0600202C RID: 8236 RVA: 0x000C3036 File Offset: 0x000C1236
		public Horde(AIDirectorChunkEventComponent outer, Vector3 pos)
		{
			this._outer = outer;
			this._pos = pos;
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x000C304C File Offset: 0x000C124C
		public void SpawnMore(int size)
		{
			int num = this._numSpawned + size;
			int num2 = num - this._numSpawned;
			this._numSpawned = num;
			if (this._horde != null)
			{
				this._horde.numToSpawn += num2;
				return;
			}
			this._horde = new AIHordeSpawner(this._outer.Director.World, "ScoutGSList", this._pos, 30f);
			this._horde.numToSpawn = num2;
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x000C30C5 File Offset: 0x000C12C5
		public void SetSpawnPos(Vector3 pos)
		{
			if (this._horde != null)
			{
				this._horde.targetPos = pos;
			}
			this._pos = pos;
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x000C30E2 File Offset: 0x000C12E2
		public void Destroy()
		{
			if (this._horde != null)
			{
				this._horde.Cleanup();
			}
			this._horde = null;
			this._destroy = true;
		}

		// Token: 0x06002030 RID: 8240 RVA: 0x000C3105 File Offset: 0x000C1305
		public bool Tick(double dt)
		{
			if (this._destroy)
			{
				return true;
			}
			if (this._horde != null && this._horde.Tick(dt))
			{
				this._horde.Cleanup();
				this._horde = null;
			}
			return false;
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06002031 RID: 8241 RVA: 0x000C313A File Offset: 0x000C133A
		public bool canSpawnMore
		{
			get
			{
				return this._numSpawned < 25;
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06002032 RID: 8242 RVA: 0x000C3146 File Offset: 0x000C1346
		public bool isSpawning
		{
			get
			{
				return this._horde != null && this._horde.isSpawning;
			}
		}

		// Token: 0x040015E6 RID: 5606
		[PublicizedFrom(EAccessModifier.Private)]
		public AIDirectorChunkEventComponent _outer;

		// Token: 0x040015E7 RID: 5607
		[PublicizedFrom(EAccessModifier.Private)]
		public AIHordeSpawner _horde;

		// Token: 0x040015E8 RID: 5608
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 _pos;

		// Token: 0x040015E9 RID: 5609
		[PublicizedFrom(EAccessModifier.Private)]
		public int _numSpawned;

		// Token: 0x040015EA RID: 5610
		[PublicizedFrom(EAccessModifier.Private)]
		public bool _destroy;
	}
}
