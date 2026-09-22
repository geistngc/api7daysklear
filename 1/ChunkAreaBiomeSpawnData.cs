using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x02000B08 RID: 2824
public class ChunkAreaBiomeSpawnData
{
	// Token: 0x06005501 RID: 21761 RVA: 0x002086C8 File Offset: 0x002068C8
	public ChunkAreaBiomeSpawnData(Chunk _chunk, byte _biomeId, ChunkCustomData _ccd)
	{
		this.biomeId = _biomeId;
		this.area = new Rect((float)(_chunk.X * 16), (float)(_chunk.Z * 16), 80f, 80f);
		this.chunk = _chunk;
		this.ccd = _ccd;
		this.ccd.TriggerWriteDataDelegate = new ChunkCustomData.TriggerWriteData(this.BeforeWrite);
		if (this.ccd.data != null)
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(new MemoryStream(this.ccd.data));
				this.read(pooledBinaryReader);
			}
		}
	}

	// Token: 0x06005502 RID: 21762 RVA: 0x00208790 File Offset: 0x00206990
	public bool IsSpawnNeeded(WorldBiomes _worldBiomes, ulong _worldTime)
	{
		BiomeDefinition biome = _worldBiomes.GetBiome(this.biomeId);
		if (biome == null)
		{
			return false;
		}
		BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
		if (biomeSpawnEntityGroupList == null)
		{
			return false;
		}
		for (int i = 0; i < biomeSpawnEntityGroupList.list.Count; i++)
		{
			BiomeSpawnEntityGroupData biomeSpawnEntityGroupData = biomeSpawnEntityGroupList.list[i];
			ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
			if (!this.entitesSpawned.TryGetValue(biomeSpawnEntityGroupData.idHash, out countsAndTime))
			{
				return true;
			}
			if (countsAndTime.count < countsAndTime.maxCount || _worldTime > countsAndTime.delayWorldTime)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005503 RID: 21763 RVA: 0x0020881C File Offset: 0x00206A1C
	public bool CanSpawn(int _idHash)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
		return this.entitesSpawned.TryGetValue(_idHash, out countsAndTime) && countsAndTime.count < countsAndTime.maxCount;
	}

	// Token: 0x06005504 RID: 21764 RVA: 0x0020884C File Offset: 0x00206A4C
	public void SetCounts(int _idHash, int _count, int _maxCount)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime value;
		this.entitesSpawned.TryGetValue(_idHash, out value);
		value.count = _count;
		value.maxCount = _maxCount;
		this.entitesSpawned[_idHash] = value;
	}

	// Token: 0x06005505 RID: 21765 RVA: 0x00208888 File Offset: 0x00206A88
	public void IncCount(int _idHash)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime value;
		if (!this.entitesSpawned.TryGetValue(_idHash, out value))
		{
			value.count = 1;
		}
		else
		{
			value.count++;
		}
		this.entitesSpawned[_idHash] = value;
		this.chunk.isModified = true;
	}

	// Token: 0x06005506 RID: 21766 RVA: 0x002088D4 File Offset: 0x00206AD4
	public void DecCount(int _idHash, bool _killed)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
		if (this.entitesSpawned.TryGetValue(_idHash, out countsAndTime))
		{
			countsAndTime.count = Utils.FastMax(countsAndTime.count - 1, 0);
			if (_killed)
			{
				countsAndTime.maxCount = Utils.FastMax(0, countsAndTime.maxCount - 1);
			}
			this.entitesSpawned[_idHash] = countsAndTime;
			this.chunk.isModified = true;
		}
	}

	// Token: 0x06005507 RID: 21767 RVA: 0x00208938 File Offset: 0x00206B38
	public void DecMaxCount(int _idHash)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
		if (this.entitesSpawned.TryGetValue(_idHash, out countsAndTime))
		{
			countsAndTime.maxCount = Utils.FastMax(0, countsAndTime.maxCount - 1);
			this.entitesSpawned[_idHash] = countsAndTime;
			this.chunk.isModified = true;
		}
	}

	// Token: 0x06005508 RID: 21768 RVA: 0x00208984 File Offset: 0x00206B84
	public ulong GetDelayWorldTime(int _idHash)
	{
		ChunkAreaBiomeSpawnData.CountsAndTime countsAndTime;
		this.entitesSpawned.TryGetValue(_idHash, out countsAndTime);
		return countsAndTime.delayWorldTime;
	}

	// Token: 0x06005509 RID: 21769 RVA: 0x002089A8 File Offset: 0x00206BA8
	public void ResetRespawn(int _idHash, World _world, int _maxCount)
	{
		BiomeDefinition biome = _world.Biomes.GetBiome(this.biomeId);
		if (biome == null)
		{
			return;
		}
		BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
		if (biomeSpawnEntityGroupList == null)
		{
			return;
		}
		BiomeSpawnEntityGroupData biomeSpawnEntityGroupData = biomeSpawnEntityGroupList.Find(_idHash);
		if (biomeSpawnEntityGroupData == null)
		{
			return;
		}
		ChunkAreaBiomeSpawnData.CountsAndTime value;
		this.entitesSpawned.TryGetValue(_idHash, out value);
		int num = 0;
		bool flag = _world.IsDaytime();
		if (biomeSpawnEntityGroupData.respawnDelayInWorldTime.Length > 1)
		{
			if (biomeSpawnEntityGroupData.type == BiomeSpawnEntityGroupData.eType.Animal)
			{
				num = (flag ? ChunkAreaBiomeSpawnData.RespawnDayDelayIndexAnimals : ChunkAreaBiomeSpawnData.RespawnNightDelayIndexAnimals);
			}
			else
			{
				num = (flag ? ChunkAreaBiomeSpawnData.RespawnDayDelayIndexEnemies : ChunkAreaBiomeSpawnData.RespawnNightDelayIndexEnemies);
			}
		}
		if (_maxCount > 0)
		{
			if (flag)
			{
				if (ChunkAreaBiomeSpawnData.RespawnDayEnemyCountOverride == -1)
				{
					_maxCount = 0;
				}
				else if (biomeSpawnEntityGroupData.type != BiomeSpawnEntityGroupData.eType.Rare)
				{
					if (ChunkAreaBiomeSpawnData.RespawnDayEnemyCountOverride > 0 && EntityGroups.IsEnemyGroup(biomeSpawnEntityGroupData.entityGroupName))
					{
						_maxCount = ChunkAreaBiomeSpawnData.RespawnDayEnemyCountOverride;
					}
					else if (ChunkAreaBiomeSpawnData.RespawnDayAnimalCountOverride > 0 && !EntityGroups.IsEnemyGroup(biomeSpawnEntityGroupData.entityGroupName))
					{
						_maxCount = ChunkAreaBiomeSpawnData.RespawnDayAnimalCountOverride;
					}
				}
			}
			else if (ChunkAreaBiomeSpawnData.RespawnNightEnemyCountOverride == -1)
			{
				_maxCount = 0;
			}
			else if (ChunkAreaBiomeSpawnData.RespawnNightEnemyCountOverride > 0 && EntityGroups.IsEnemyGroup(biomeSpawnEntityGroupData.entityGroupName))
			{
				_maxCount = ChunkAreaBiomeSpawnData.RespawnNightEnemyCountOverride;
			}
			else if (ChunkAreaBiomeSpawnData.RespawnNightAnimalCountOverride > 0 && !EntityGroups.IsEnemyGroup(biomeSpawnEntityGroupData.entityGroupName))
			{
				_maxCount = ChunkAreaBiomeSpawnData.RespawnNightAnimalCountOverride;
			}
		}
		value.delayWorldTime = _world.worldTime + (ulong)((float)biomeSpawnEntityGroupData.respawnDelayInWorldTime[num] * _world.RandomRange(0.9f, 1.1f));
		value.maxCount = _maxCount;
		this.entitesSpawned[_idHash] = value;
		this.chunk.isModified = true;
	}

	// Token: 0x0600550A RID: 21770 RVA: 0x00208B38 File Offset: 0x00206D38
	public bool DelayAllEnemySpawningUntil(ulong _worldTime, WorldBiomes _worldBiomes)
	{
		bool result = false;
		BiomeDefinition biome = _worldBiomes.GetBiome(this.biomeId);
		if (biome == null)
		{
			return false;
		}
		BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
		if (biomeSpawnEntityGroupList == null)
		{
			return false;
		}
		Dictionary<int, ChunkAreaBiomeSpawnData.CountsAndTime> dictionary = new Dictionary<int, ChunkAreaBiomeSpawnData.CountsAndTime>();
		foreach (KeyValuePair<int, ChunkAreaBiomeSpawnData.CountsAndTime> keyValuePair in this.entitesSpawned)
		{
			BiomeSpawnEntityGroupData biomeSpawnEntityGroupData = biomeSpawnEntityGroupList.Find(keyValuePair.Key);
			if (biomeSpawnEntityGroupData != null && EntityGroups.IsEnemyGroup(biomeSpawnEntityGroupData.entityGroupName))
			{
				ChunkAreaBiomeSpawnData.CountsAndTime value = keyValuePair.Value;
				bool flag = false;
				if (value.delayWorldTime < _worldTime)
				{
					value.delayWorldTime = _worldTime;
					flag = true;
				}
				if (value.maxCount > 0)
				{
					value.maxCount = 0;
					flag = true;
				}
				if (flag)
				{
					dictionary[keyValuePair.Key] = value;
					result = true;
				}
			}
		}
		foreach (KeyValuePair<int, ChunkAreaBiomeSpawnData.CountsAndTime> keyValuePair2 in dictionary)
		{
			this.entitesSpawned[keyValuePair2.Key] = keyValuePair2.Value;
		}
		for (int i = 0; i < biomeSpawnEntityGroupList.list.Count; i++)
		{
			BiomeSpawnEntityGroupData biomeSpawnEntityGroupData2 = biomeSpawnEntityGroupList.list[i];
			if (EntityGroups.IsEnemyGroup(biomeSpawnEntityGroupData2.entityGroupName) && !this.entitesSpawned.ContainsKey(biomeSpawnEntityGroupData2.idHash))
			{
				this.entitesSpawned[biomeSpawnEntityGroupData2.idHash] = new ChunkAreaBiomeSpawnData.CountsAndTime(0, 0, _worldTime);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600550B RID: 21771 RVA: 0x00208CDC File Offset: 0x00206EDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void read(BinaryReader _br)
	{
		int num = (int)_br.ReadByte();
		this.entitesSpawned.Clear();
		int num2 = (int)_br.ReadByte();
		for (int i = 0; i < num2; i++)
		{
			if (num <= 1)
			{
				_br.ReadString();
				_br.ReadUInt16();
				_br.ReadUInt64();
			}
			else
			{
				int key = _br.ReadInt32();
				int num3 = (int)_br.ReadUInt16();
				ChunkAreaBiomeSpawnData.CountsAndTime value;
				value.count = (num3 & 255);
				value.maxCount = num3 >> 8;
				value.delayWorldTime = _br.ReadUInt64();
				this.entitesSpawned[key] = value;
			}
		}
	}

	// Token: 0x0600550C RID: 21772 RVA: 0x00208D70 File Offset: 0x00206F70
	public void BeforeWrite()
	{
		using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
		{
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.write(pooledBinaryWriter);
			}
			this.ccd.data = pooledExpandableMemoryStream.ToArray();
		}
	}

	// Token: 0x0600550D RID: 21773 RVA: 0x00208DE8 File Offset: 0x00206FE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void write(BinaryWriter _bw)
	{
		_bw.Write(2);
		int num = 0;
		int num2 = Utils.FastMin(this.entitesSpawned.Count, 255);
		_bw.Write((byte)num2);
		foreach (KeyValuePair<int, ChunkAreaBiomeSpawnData.CountsAndTime> keyValuePair in this.entitesSpawned)
		{
			_bw.Write(keyValuePair.Key);
			_bw.Write((ushort)(keyValuePair.Value.maxCount << 8 | keyValuePair.Value.count));
			_bw.Write(keyValuePair.Value.delayWorldTime);
			if (++num >= num2)
			{
				break;
			}
		}
	}

	// Token: 0x0600550E RID: 21774 RVA: 0x00208EA8 File Offset: 0x002070A8
	public override string ToString()
	{
		World world = GameManager.Instance.World;
		ulong worldTime = world.worldTime;
		BiomeDefinition biome = world.Biomes.GetBiome(this.biomeId);
		if (biome == null)
		{
			return "biome? " + this.biomeId.ToString();
		}
		BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<int, ChunkAreaBiomeSpawnData.CountsAndTime> keyValuePair in this.entitesSpawned)
		{
			string text = "?";
			if (biomeSpawnEntityGroupList != null)
			{
				BiomeSpawnEntityGroupData biomeSpawnEntityGroupData = biomeSpawnEntityGroupList.Find(keyValuePair.Key);
				if (biomeSpawnEntityGroupData != null)
				{
					text = biomeSpawnEntityGroupData.entityGroupName + " " + biomeSpawnEntityGroupData.daytime.ToString();
				}
			}
			ulong num = keyValuePair.Value.delayWorldTime - worldTime;
			if (num < 0UL)
			{
				num = 0UL;
			}
			stringBuilder.Append(string.Format("{0} #{1}/{2} {3}, ", new object[]
			{
				text,
				keyValuePair.Value.count,
				keyValuePair.Value.maxCount,
				GameUtils.WorldTimeDeltaToString(num)
			}));
		}
		return string.Format("biomeId {0}, XZ {1} {2}: {3}", new object[]
		{
			this.biomeId,
			this.area.x.ToCultureInvariantString("0"),
			this.area.y.ToCultureInvariantString("0"),
			stringBuilder.ToString()
		});
	}

	// Token: 0x04004211 RID: 16913
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCurrentSaveVersion = 2;

	// Token: 0x04004212 RID: 16914
	public byte biomeId;

	// Token: 0x04004213 RID: 16915
	public Rect area;

	// Token: 0x04004214 RID: 16916
	public Chunk chunk;

	// Token: 0x04004215 RID: 16917
	public bool checkedPOITags;

	// Token: 0x04004216 RID: 16918
	public FastTags<TagGroup.Poi> poiTags;

	// Token: 0x04004217 RID: 16919
	public int groupsEnabledFlags;

	// Token: 0x04004218 RID: 16920
	public static int RespawnDayDelayIndexEnemies;

	// Token: 0x04004219 RID: 16921
	public static int RespawnDayDelayIndexAnimals;

	// Token: 0x0400421A RID: 16922
	public static int RespawnDayEnemyCountOverride;

	// Token: 0x0400421B RID: 16923
	public static int RespawnDayAnimalCountOverride;

	// Token: 0x0400421C RID: 16924
	public static int RespawnNightDelayIndexEnemies;

	// Token: 0x0400421D RID: 16925
	public static int RespawnNightDelayIndexAnimals;

	// Token: 0x0400421E RID: 16926
	public static int RespawnNightEnemyCountOverride;

	// Token: 0x0400421F RID: 16927
	public static int RespawnNightAnimalCountOverride;

	// Token: 0x04004220 RID: 16928
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkCustomData ccd;

	// Token: 0x04004221 RID: 16929
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<int, ChunkAreaBiomeSpawnData.CountsAndTime> entitesSpawned = new Dictionary<int, ChunkAreaBiomeSpawnData.CountsAndTime>();

	// Token: 0x02000B09 RID: 2825
	[PublicizedFrom(EAccessModifier.Private)]
	public struct CountsAndTime
	{
		// Token: 0x0600550F RID: 21775 RVA: 0x00209050 File Offset: 0x00207250
		public CountsAndTime(int _count, int _maxCount, ulong _delayWorldTime)
		{
			this.count = _count;
			this.maxCount = _maxCount;
			this.delayWorldTime = _delayWorldTime;
		}

		// Token: 0x06005510 RID: 21776 RVA: 0x00209067 File Offset: 0x00207267
		public override string ToString()
		{
			return string.Format("cnt {0}, maxCnt {1}, wtime {2}", this.count, this.maxCount, this.delayWorldTime);
		}

		// Token: 0x04004222 RID: 16930
		public int count;

		// Token: 0x04004223 RID: 16931
		public int maxCount;

		// Token: 0x04004224 RID: 16932
		public ulong delayWorldTime;
	}
}
