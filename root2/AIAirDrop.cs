using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003FE RID: 1022
[Preserve]
public class AIAirDrop
{
	// Token: 0x06001F9D RID: 8093 RVA: 0x000BF11E File Offset: 0x000BD31E
	public AIAirDrop(AIDirectorAirDropComponent _airDropController, World _world, List<EntityPlayer> _players)
	{
		this.controller = _airDropController;
		this.world = _world;
		this.numPlayers = _players.Count;
		this.MakePlayerClusters(_players);
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x000BF148 File Offset: 0x000BD348
	public bool Tick(float dt)
	{
		if (this.flightPaths == null)
		{
			this.CreateFlightPaths();
			Log.Out("AIAirDrop: Computed flight paths for " + this.flightPaths.Count.ToString() + " aircraft.");
			Log.Out("AIAirDrop: Waiting for supply crate chunk locations to load...");
		}
		if (!this.spawningCrates)
		{
			bool flag = true;
			for (int i = 0; i < this.flightPaths.Count; i++)
			{
				AIAirDrop.FlightPath flightPath = this.flightPaths[i];
				for (int j = 0; j < flightPath.Crates.Count; j++)
				{
					AIAirDrop.SupplyCrateSpawn supplyCrateSpawn = flightPath.Crates[j];
					if ((Chunk)this.world.GetChunkFromWorldPos(World.worldToBlockPos(supplyCrateSpawn.SpawnPos)) == null)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			this.spawningCrates = flag;
		}
		if (this.spawningCrates)
		{
			int k = 0;
			while (k < this.flightPaths.Count)
			{
				AIAirDrop.FlightPath flightPath2 = this.flightPaths[k];
				flightPath2.Delay -= dt;
				if (flightPath2.Delay <= 0f)
				{
					if (!flightPath2.Spawned)
					{
						this.SpawnPlane(flightPath2);
						flightPath2.Spawned = true;
					}
					int l = 0;
					while (l < flightPath2.Crates.Count)
					{
						AIAirDrop.SupplyCrateSpawn supplyCrateSpawn2 = flightPath2.Crates[l];
						supplyCrateSpawn2.Delay -= dt;
						if (supplyCrateSpawn2.Delay <= 0f)
						{
							this.controller.SpawnSupplyCrate(supplyCrateSpawn2.SpawnPos, supplyCrateSpawn2.ChunkRef);
							Log.Out("AIAirDrop: Spawned supply crate at " + supplyCrateSpawn2.SpawnPos.ToCultureInvariantString() + ", plane is at " + ((this.eSupplyPlane != null) ? this.eSupplyPlane.position : Vector3.zero).ToString());
							flightPath2.Crates.RemoveAt(l);
						}
						else
						{
							l++;
						}
					}
				}
				if (flightPath2.Crates.Count == 0)
				{
					this.flightPaths.RemoveAt(k);
				}
				else
				{
					k++;
				}
			}
			if (this.flightPaths.Count == 0)
			{
				this.flightPaths = null;
			}
		}
		return this.flightPaths == null;
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x000BF38C File Offset: 0x000BD58C
	public static float Angle(Vector2 p_vector2)
	{
		if (p_vector2.x < 0f)
		{
			return 360f - Mathf.Atan2(p_vector2.x, p_vector2.y) * 57.29578f * -1f;
		}
		return Mathf.Atan2(p_vector2.x, p_vector2.y) * 57.29578f;
	}

	// Token: 0x06001FA0 RID: 8096 RVA: 0x000BF3E4 File Offset: 0x000BD5E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnPlane(AIAirDrop.FlightPath _fp)
	{
		Vector3 vector = _fp.End - _fp.Start;
		Vector3 normalized = vector.normalized;
		Vector2 vector2 = new Vector2(normalized.x, normalized.z);
		EntitySupplyPlane entitySupplyPlane = (EntitySupplyPlane)EntityFactory.CreateEntity(EntityClass.FromString("supplyPlane"), _fp.Start, new Vector3(0f, AIAirDrop.Angle(vector2), 0f));
		this.eSupplyPlane = entitySupplyPlane;
		entitySupplyPlane.SetDirectionToFly(normalized, (int)(20f * (vector.magnitude / 120f + 10f)));
		this.world.SpawnEntityInWorld(entitySupplyPlane);
		Log.Out(string.Concat(new string[]
		{
			"AIAirDrop: Spawned aircraft at (",
			_fp.Start.ToCultureInvariantString(),
			"), heading (",
			vector2.ToCultureInvariantString(),
			")"
		}));
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x000BF4C4 File Offset: 0x000BD6C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateFlightPaths()
	{
		this.flightPaths = new List<AIAirDrop.FlightPath>();
		int i;
		int num;
		this.CalcSupplyDropMetrics(this.numPlayers, this.clusters.Count, out i, out num);
		int num2 = Math.Max(1, i / num);
		int num3 = i - num * num2;
		HashSet<int> hashSet = new HashSet<int>();
		GameRandom random = this.controller.Random;
		while (i > 0)
		{
			int num4 = Mathf.Min(i, num2 + num3);
			i -= num4;
			num3 = 0;
			int num5;
			do
			{
				num5 = random.RandomRange(0, this.clusters.Count);
			}
			while (hashSet.Contains(num5));
			AIAirDrop.PlayerCluster playerCluster = this.clusters[num5];
			hashSet.Add(num5);
			if (hashSet.Count == this.clusters.Count)
			{
				hashSet.Clear();
			}
			float num6 = playerCluster.Players[random.RandomRange(0, playerCluster.Players.Count)].position.y + 180f;
			num6 = Utils.FastMin(num6, 276f);
			Vector2 vector = random.RandomOnUnitCircle;
			Vector2 a = playerCluster.XZCenter + vector * random.RandomRange(30f, 750f);
			float num7 = random.RandomRange(150f, 700f);
			float num8 = num7 / 2f;
			float x = vector.x;
			vector.x = -vector.y;
			vector.y = x;
			float num9 = random.RandomRange(1500f, 2000f) / 2f;
			Vector2 vector2 = a + -vector * (num8 + num9);
			Vector2 vector3 = a + vector * (num8 + num9);
			vector2 = this.FindSafePoint(vector2, -vector, 25f, 600f);
			vector3 = this.FindSafePoint(vector3, vector, 25f, 600f);
			float num10 = num7 / (float)num4;
			float num11 = -num10 * Math.Max(1f, ((float)num4 - 1f) / 2f);
			AIAirDrop.FlightPath flightPath = new AIAirDrop.FlightPath();
			flightPath.Start = new Vector3(vector2.x, num6, vector2.y);
			flightPath.End = new Vector3(vector3.x, num6, vector3.y);
			float magnitude = (flightPath.End - flightPath.Start).magnitude;
			for (int j = 0; j < num4; j++)
			{
				Vector2 vector4 = a + (num11 + (float)j * num10) * vector;
				AIAirDrop.SupplyCrateSpawn supplyCrateSpawn = new AIAirDrop.SupplyCrateSpawn();
				float num12 = num6 - 10f;
				if (GameManager.Instance != null && GameManager.Instance.World != null)
				{
					float num13 = (float)GameManager.Instance.World.GetHeight((int)vector4.x, (int)vector4.y);
					if (num12 <= num13 + 15f)
					{
						num12 = num13 + 15f;
					}
				}
				supplyCrateSpawn.SpawnPos = this.ClampToMapExtents(new Vector3(vector4.x, num12, vector4.y), vector, 25f);
				if (j == 0)
				{
					vector = new Vector2(supplyCrateSpawn.SpawnPos.x, supplyCrateSpawn.SpawnPos.z) - new Vector2(flightPath.Start.x, flightPath.Start.z);
					vector.Normalize();
					flightPath.End = flightPath.Start + new Vector3(vector.x, 0f, vector.y) * magnitude;
				}
				supplyCrateSpawn.Delay = (vector2 - vector4).magnitude / 120f;
				supplyCrateSpawn.ChunkRef = this.world.GetGameManager().AddChunkObserver(supplyCrateSpawn.SpawnPos, false, 3, -1);
				flightPath.Crates.Add(supplyCrateSpawn);
			}
			flightPath.Delay = playerCluster.Delay + random.RandomRange(0f, 15f);
			playerCluster.Delay += random.RandomRange(25f, 120f);
			this.flightPaths.Add(flightPath);
		}
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x000BF90C File Offset: 0x000BDB0C
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 FindSafePoint(Vector2 _point, Vector2 _dir, float _stepSize, float _range)
	{
		_range *= _range;
		for (;;)
		{
			bool flag = true;
			for (int i = 0; i < this.clusters.Count; i++)
			{
				AIAirDrop.PlayerCluster playerCluster = this.clusters[i];
				for (int j = 0; j < playerCluster.Players.Count; j++)
				{
					EntityPlayer entityPlayer = playerCluster.Players[j];
					if ((_point - new Vector2(entityPlayer.position.x, entityPlayer.position.z)).sqrMagnitude < _range)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			if (flag)
			{
				break;
			}
			_point += _dir * _stepSize;
		}
		return _point;
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x000BF9B8 File Offset: 0x000BDBB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void MakePlayerClusters(List<EntityPlayer> _players)
	{
		this.clusters = new List<AIAirDrop.PlayerCluster>();
		foreach (EntityPlayer entityPlayer in _players)
		{
			bool flag = true;
			for (int i = 0; i < this.clusters.Count; i++)
			{
				AIAirDrop.PlayerCluster cluster = this.clusters[i];
				if (this.TryAddPlayerToCluster(entityPlayer, cluster))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				AIAirDrop.PlayerCluster playerCluster = new AIAirDrop.PlayerCluster();
				playerCluster.Radius = 30f;
				playerCluster.XZCenter = new Vector2(entityPlayer.position.x, entityPlayer.position.z);
				playerCluster.Players.Add(entityPlayer);
				this.clusters.Add(playerCluster);
			}
		}
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x000BFA98 File Offset: 0x000BDC98
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryAddPlayerToCluster(EntityPlayer _player, AIAirDrop.PlayerCluster _cluster)
	{
		Vector2 vector = _cluster.XZCenter + new Vector2(_player.position.x, _player.position.z);
		vector.Scale(new Vector2(0.5f, 0.5f));
		float num = this.GetPlayerDistanceSq(_player, vector);
		if (num > 70f)
		{
			return false;
		}
		for (int i = 0; i < _cluster.Players.Count; i++)
		{
			EntityPlayer player = _cluster.Players[i];
			num = Mathf.Max(num, this.GetPlayerDistanceSq(player, vector));
			if (num > 70f)
			{
				return false;
			}
		}
		_cluster.XZCenter = vector;
		_cluster.Radius = Mathf.Max(num, 30f);
		_cluster.Players.Add(_player);
		return true;
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x000BFB58 File Offset: 0x000BDD58
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetPlayerDistanceSq(EntityPlayer _player, Vector2 _xzPos)
	{
		return (_xzPos - new Vector2(_player.position.x, _player.position.z)).sqrMagnitude;
	}

	// Token: 0x06001FA6 RID: 8102 RVA: 0x000BFB8E File Offset: 0x000BDD8E
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SelectCrateCount(int _numPlayers, out int _min, out int _max)
	{
		_min = 1;
		_max = 1;
	}

	// Token: 0x06001FA7 RID: 8103 RVA: 0x000BFB98 File Offset: 0x000BDD98
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcSupplyDropMetrics(int _numPlayers, int _numClusters, out int _numCrates, out int _numPlanes)
	{
		int min;
		int num;
		AIAirDrop.SelectCrateCount(_numPlayers, out min, out num);
		_numCrates = this.controller.Random.RandomRange(min, num + 1);
		_numPlanes = Math.Max(1, Math.Min(_numClusters + this.controller.Random.RandomRange(0, 2), 4));
		if (_numCrates / _numPlanes < 1 || _numCrates / _numPlanes > 3)
		{
			_numPlanes = Math.Min(1, Mathf.CeilToInt((float)_numCrates / 3f));
		}
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x000BFC11 File Offset: 0x000BDE11
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 ClampToMapExtents(Vector3 _pos, Vector2 _dir, float _step)
	{
		return this.world.ClampToValidWorldPos(_pos);
	}

	// Token: 0x04001550 RID: 5456
	public const float cPlaneMetersPerSecond = 120f;

	// Token: 0x04001551 RID: 5457
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMaxPlayerClusterRadius = 70f;

	// Token: 0x04001552 RID: 5458
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMinPlayerClusterRadius = 30f;

	// Token: 0x04001553 RID: 5459
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMaxPlaneTangentPointRadius = 750f;

	// Token: 0x04001554 RID: 5460
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMinPlaneTangentPointRadius = 30f;

	// Token: 0x04001555 RID: 5461
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMinPlaneFlightVector = 1500f;

	// Token: 0x04001556 RID: 5462
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMaxPlaneFlightVector = 2000f;

	// Token: 0x04001557 RID: 5463
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMinDropRange = 150f;

	// Token: 0x04001558 RID: 5464
	[PublicizedFrom(EAccessModifier.Private)]
	public const float kMaxDropRange = 700f;

	// Token: 0x04001559 RID: 5465
	[PublicizedFrom(EAccessModifier.Private)]
	public const int kMaxDropsPerPlane = 3;

	// Token: 0x0400155A RID: 5466
	[PublicizedFrom(EAccessModifier.Private)]
	public const int kSpawnYUp = 180;

	// Token: 0x0400155B RID: 5467
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorAirDropComponent controller;

	// Token: 0x0400155C RID: 5468
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x0400155D RID: 5469
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIAirDrop.PlayerCluster> clusters;

	// Token: 0x0400155E RID: 5470
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIAirDrop.FlightPath> flightPaths;

	// Token: 0x0400155F RID: 5471
	[PublicizedFrom(EAccessModifier.Private)]
	public bool spawningCrates;

	// Token: 0x04001560 RID: 5472
	[PublicizedFrom(EAccessModifier.Private)]
	public int numPlayers;

	// Token: 0x04001561 RID: 5473
	[PublicizedFrom(EAccessModifier.Private)]
	public Entity eSupplyPlane;

	// Token: 0x020003FF RID: 1023
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class SupplyCrateSpawn
	{
		// Token: 0x04001562 RID: 5474
		public float Delay;

		// Token: 0x04001563 RID: 5475
		public Vector3 SpawnPos;

		// Token: 0x04001564 RID: 5476
		public ChunkManager.ChunkObserver ChunkRef;
	}

	// Token: 0x02000400 RID: 1024
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class PlayerCluster
	{
		// Token: 0x04001565 RID: 5477
		public Vector2 XZCenter;

		// Token: 0x04001566 RID: 5478
		public float Radius;

		// Token: 0x04001567 RID: 5479
		public List<EntityPlayer> Players = new List<EntityPlayer>();

		// Token: 0x04001568 RID: 5480
		public float Delay;
	}

	// Token: 0x02000401 RID: 1025
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class FlightPath
	{
		// Token: 0x04001569 RID: 5481
		public List<AIAirDrop.SupplyCrateSpawn> Crates = new List<AIAirDrop.SupplyCrateSpawn>();

		// Token: 0x0400156A RID: 5482
		public Vector3 Start;

		// Token: 0x0400156B RID: 5483
		public Vector3 End;

		// Token: 0x0400156C RID: 5484
		public float Delay;

		// Token: 0x0400156D RID: 5485
		public bool Spawned;
	}
}
