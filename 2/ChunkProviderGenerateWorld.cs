using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

// Token: 0x02000B30 RID: 2864
public abstract class ChunkProviderGenerateWorld : ChunkProviderAbstract
{
	// Token: 0x060056B6 RID: 22198 RVA: 0x002147CC File Offset: 0x002129CC
	public ChunkProviderGenerateWorld(string _levelName, PathAbstractions.AbstractedLocation _worldLocation, bool _bClientMode = false)
	{
		this.bClientMode = _bClientMode;
		this.levelName = _levelName;
		this.worldLocation = _worldLocation;
		this.bDecorationsEnabled = true;
	}

	// Token: 0x060056B7 RID: 22199 RVA: 0x00214833 File Offset: 0x00212A33
	public override DynamicPrefabDecorator GetDynamicPrefabDecorator()
	{
		return this.prefabDecorator;
	}

	// Token: 0x060056B8 RID: 22200 RVA: 0x0021483B File Offset: 0x00212A3B
	public override EventPrefabs GetEventPrefabs()
	{
		return this.eventPrefabs;
	}

	// Token: 0x060056B9 RID: 22201 RVA: 0x00214843 File Offset: 0x00212A43
	public override IEnumerator Init(World _world)
	{
		this.world = _world;
		this.prefabDecorator = new DynamicPrefabDecorator(_world.m_PrefabCache);
		yield return this.prefabDecorator.Load(this.worldLocation.FullPath, false);
		yield return null;
		this.spawnPointManager = new SpawnPointManager(true);
		if (!this.bClientMode)
		{
			yield return null;
			this.spawnPointManager.Load(this.worldLocation.FullPath);
		}
		if (!this.bClientMode)
		{
			this.threadInfo = ThreadManager.StartThread("GenerateChunks", null, new ThreadManager.ThreadFunctionLoopDelegate(this.GenerateChunksThread), null, null, null, true, false);
		}
		yield return null;
		yield break;
	}

	// Token: 0x060056BA RID: 22202 RVA: 0x0021485C File Offset: 0x00212A5C
	public override void SaveAll()
	{
		if (this.bClientMode)
		{
			return;
		}
		if (this.world.IsEditor())
		{
			this.GetDynamicPrefabDecorator().Save(this.worldLocation.FullPath);
			this.spawnPointManager.Save(this.worldLocation.FullPath);
			return;
		}
		if (this.m_RegionFileManager != null)
		{
			this.m_RegionFileManager.MakePersistent(this.world.ChunkCache, false);
			this.m_RegionFileManager.WaitSaveDone();
		}
		EventPrefabs eventPrefabs = this.GetEventPrefabs();
		if (eventPrefabs == null)
		{
			return;
		}
		eventPrefabs.Save(true);
	}

	// Token: 0x060056BB RID: 22203 RVA: 0x002148EC File Offset: 0x00212AEC
	public override void SaveRandomChunks(int count, ulong _curWorldTimeInTicks, ArraySegment<long> _activeChunkSet)
	{
		if (this.bClientMode)
		{
			return;
		}
		GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
		int i = _activeChunkSet.Offset;
		while (i < _activeChunkSet.Count)
		{
			Chunk chunkSync = this.world.ChunkCache.GetChunkSync(_activeChunkSet.Array[i]);
			if (chunkSync != null && chunkSync.NeedsSaving && !chunkSync.NeedsDecoration && !chunkSync.InProgressDecorating && !chunkSync.NeedsLightCalculation && !chunkSync.InProgressLighting && _curWorldTimeInTicks - chunkSync.SavedInWorldTicks > 400UL && gameRandom.RandomFloat < 0.3f)
			{
				Chunk obj = chunkSync;
				lock (obj)
				{
					if (chunkSync.IsLocked)
					{
						goto IL_E5;
					}
					chunkSync.InProgressSaving = true;
				}
				this.m_RegionFileManager.SaveChunkSnapshot(chunkSync, false);
				count--;
				chunkSync.InProgressSaving = false;
				goto IL_E1;
			}
			goto IL_E1;
			IL_E5:
			i++;
			continue;
			IL_E1:
			if (count > 0)
			{
				goto IL_E5;
			}
			break;
		}
	}

	// Token: 0x060056BC RID: 22204 RVA: 0x00214A00 File Offset: 0x00212C00
	public override void ClearCaches()
	{
		if (this.bClientMode)
		{
			return;
		}
		this.m_RegionFileManager.ClearCaches();
	}

	// Token: 0x060056BD RID: 22205 RVA: 0x00214A16 File Offset: 0x00212C16
	public HashSetLong ResetAllChunks(ChunkProtectionLevel excludedProtectionLevels, EnumResetUnprotectedChunksGroupingMode _groupingMode = EnumResetUnprotectedChunksGroupingMode.GroupedPOIs)
	{
		return this.m_RegionFileManager.ResetAllChunks(excludedProtectionLevels, _groupingMode);
	}

	// Token: 0x060056BE RID: 22206 RVA: 0x00214A25 File Offset: 0x00212C25
	public HashSetLong ResetRegion(int _regionX, int _regionZ, ChunkProtectionLevel excludedProtectionLevels, EnumResetUnprotectedChunksGroupingMode _groupingMode = EnumResetUnprotectedChunksGroupingMode.GroupedPOIs)
	{
		return this.m_RegionFileManager.ResetRegion(_regionX, _regionZ, excludedProtectionLevels, _groupingMode);
	}

	// Token: 0x060056BF RID: 22207 RVA: 0x00214A37 File Offset: 0x00212C37
	public void RequestChunkReset(long _chunkKey)
	{
		this.m_RegionFileManager.RequestChunkReset(_chunkKey);
	}

	// Token: 0x060056C0 RID: 22208 RVA: 0x00214A45 File Offset: 0x00212C45
	public void MainThreadCacheProtectedPositions()
	{
		this.m_RegionFileManager.MainThreadCacheProtectedPositions();
	}

	// Token: 0x060056C1 RID: 22209 RVA: 0x00214A52 File Offset: 0x00212C52
	public void SaveChunkAgeDebugTexture(float rangeInDays)
	{
		this.m_RegionFileManager.SaveChunkAgeDebugTexture(rangeInDays);
	}

	// Token: 0x060056C2 RID: 22210 RVA: 0x00214A60 File Offset: 0x00212C60
	public void IterateChunkExpiryTimes(Action<long, ulong> action)
	{
		this.m_RegionFileManager.IterateChunkExpiryTimes(action);
	}

	// Token: 0x17000933 RID: 2355
	// (get) Token: 0x060056C3 RID: 22211 RVA: 0x00214A6E File Offset: 0x00212C6E
	public IReadOnlyCollection<LongSetGroups.Group> ChunkGroups
	{
		get
		{
			return this.m_RegionFileManager.ChunkGroups;
		}
	}

	// Token: 0x060056C4 RID: 22212 RVA: 0x00214A7C File Offset: 0x00212C7C
	public override void RequestChunk(int _x, int _y)
	{
		if (this.bClientMode)
		{
			return;
		}
		object syncRoot = ((ICollection)this.m_ChunkQueue.list).SyncRoot;
		lock (syncRoot)
		{
			long num = WorldChunkCache.MakeChunkKey(_x, _y);
			if (this.m_ChunkQueue.hashSet.Contains(num))
			{
				return;
			}
			this.m_ChunkQueue.Add(num);
		}
		this.m_WaitHandle.Set();
	}

	// Token: 0x060056C5 RID: 22213 RVA: 0x00214B00 File Offset: 0x00212D00
	public override HashSetList<long> GetRequestedChunks()
	{
		return this.m_ChunkQueue;
	}

	// Token: 0x060056C6 RID: 22214 RVA: 0x00214B08 File Offset: 0x00212D08
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void generateTerrain(World _world, Chunk _chunk, GameRandom _random)
	{
		this.m_TerrainGenerator.GenerateTerrain(_world, _chunk, _random, Vector3i.zero, Vector3i.zero, false, false);
	}

	// Token: 0x060056C7 RID: 22215 RVA: 0x00214B24 File Offset: 0x00212D24
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void generateTerrain(World _world, Chunk _chunk, GameRandom _random, Vector3i _areaStart, Vector3i _areaSize, bool _bFillEmptyBlocks, bool _isReset)
	{
		this.m_TerrainGenerator.GenerateTerrain(_world, _chunk, _random, _areaStart, _areaSize, _bFillEmptyBlocks, _isReset);
	}

	// Token: 0x060056C8 RID: 22216 RVA: 0x00214B3C File Offset: 0x00212D3C
	public bool GenerateSingleChunk(ChunkCluster cc, long key, bool _forceRebuild = false)
	{
		this.currentGeneratingChunk = key;
		this.chunkGenerationTimer.Restart();
		if (!_forceRebuild && cc.ContainsChunkSync(key))
		{
			this.currentGeneratingChunk = 0L;
			return false;
		}
		Chunk chunk = null;
		if (this.m_RegionFileManager.ContainsChunkSync(key))
		{
			chunk = this.m_RegionFileManager.GetChunkSync(key);
			this.m_RegionFileManager.RemoveChunkSync(key);
		}
		if (_forceRebuild)
		{
			chunk = cc.GetChunkSync(key);
			if (chunk != null)
			{
				chunk.RemoveBlockEntityTransforms();
				chunk.Reset();
			}
		}
		if (_forceRebuild || chunk == null)
		{
			int x = WorldChunkCache.extractX(key);
			int num = WorldChunkCache.extractZ(key);
			if (chunk == null)
			{
				chunk = MemoryPools.PoolChunks.AllocSync(true);
			}
			if (chunk != null)
			{
				chunk.X = x;
				chunk.Z = num;
				GameRandom gameRandom = Utils.RandomFromSeedOnPos(x, num, this.world.Seed);
				this.generateTerrain(this.world, chunk, gameRandom);
				GameRandomManager.Instance.FreeGameRandom(gameRandom);
				if (!this.bDecorationsEnabled)
				{
					chunk.NeedsDecoration = false;
					chunk.NeedsLightCalculation = false;
					chunk.NeedsRegeneration = true;
				}
				if (this.bDecorationsEnabled)
				{
					chunk.NeedsDecoration = true;
					chunk.NeedsLightCalculation = true;
					if (this.GetDynamicPrefabDecorator() != null)
					{
						this.GetDynamicPrefabDecorator().DecorateChunk(this.world, chunk);
					}
				}
			}
		}
		bool flag = false;
		if (chunk != null)
		{
			if (!_forceRebuild)
			{
				flag = cc.AddChunkSync(chunk, false);
			}
			else
			{
				ReaderWriterLockSlim syncRoot = cc.GetSyncRoot();
				syncRoot.EnterUpgradeableReadLock();
				if (cc.ContainsChunkSync(key))
				{
					cc.RemoveChunkSync(key);
				}
				flag = cc.AddChunkSync(chunk, false);
				syncRoot.ExitUpgradeableReadLock();
			}
			if (flag)
			{
				if (!chunk.NeedsDecoration)
				{
					ChunkProviderGenerateWorld.OnChunkSyncedAndDecorated(chunk);
				}
				this.updateDecorationsWherePossible(chunk);
				if (_forceRebuild)
				{
					chunk.isModified = true;
				}
			}
			else
			{
				MemoryPools.PoolChunks.FreeSync(chunk);
			}
		}
		return flag;
	}

	// Token: 0x060056C9 RID: 22217 RVA: 0x00214CE1 File Offset: 0x00212EE1
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OnChunkSyncedAndDecorated(Chunk chunk)
	{
		WaterSimulationNative.Instance.InitializeChunk(chunk);
	}

	// Token: 0x060056CA RID: 22218 RVA: 0x00214CF0 File Offset: 0x00212EF0
	[PublicizedFrom(EAccessModifier.Private)]
	public int GenerateChunksThread(ThreadManager.ThreadInfo _threadInfo)
	{
		if (_threadInfo.TerminationRequested())
		{
			return -1;
		}
		if (this.m_RegionFileManager == null)
		{
			return 15;
		}
		long num = this.world.GetNextChunkToProvide();
		if (num == 9223372036854775807L)
		{
			num = DynamicMeshThread.GetNextChunkToLoad();
			if (num == 9223372036854775807L)
			{
				return 15;
			}
		}
		ChunkCluster chunkCache = this.world.ChunkCache;
		this.GenerateSingleChunk(chunkCache, num, false);
		return 0;
	}

	// Token: 0x060056CB RID: 22219 RVA: 0x00214D56 File Offset: 0x00212F56
	[PublicizedFrom(EAccessModifier.Private)]
	public void tryToDecorate(Chunk _chunk)
	{
		if (_chunk == null)
		{
			return;
		}
		if (!_chunk.NeedsDecoration)
		{
			return;
		}
		if (_chunk.IsLocked)
		{
			return;
		}
		this.decorate(_chunk);
	}

	// Token: 0x060056CC RID: 22220 RVA: 0x00214D78 File Offset: 0x00212F78
	[PublicizedFrom(EAccessModifier.Private)]
	public void decorate(Chunk _chunk)
	{
		int x = _chunk.X;
		int z = _chunk.Z;
		Chunk chunk;
		Chunk chunk2;
		Chunk chunk3;
		if ((chunk = (Chunk)this.world.GetChunkSync(x + 1, z + 1)) == null || (chunk2 = (Chunk)this.world.GetChunkSync(x, z + 1)) == null || (chunk3 = (Chunk)this.world.GetChunkSync(x + 1, z)) == null)
		{
			return;
		}
		chunk.InProgressDecorating = true;
		chunk2.InProgressDecorating = true;
		chunk3.InProgressDecorating = true;
		_chunk.InProgressDecorating = true;
		this.updateDecosAllowedForChunk(_chunk, chunk3, chunk2);
		for (int i = 0; i < this.m_Decorators.Count; i++)
		{
			this.m_Decorators[i].DecorateChunkOverlapping(this.world, _chunk, chunk3, chunk2, chunk, this.world.Seed);
		}
		_chunk.OnDecorated();
		_chunk.ResetStability();
		_chunk.RefreshSunlight();
		_chunk.NeedsDecoration = false;
		_chunk.NeedsLightCalculation = true;
		chunk.InProgressDecorating = false;
		chunk2.InProgressDecorating = false;
		chunk3.InProgressDecorating = false;
		_chunk.InProgressDecorating = false;
		ChunkProviderGenerateWorld.OnChunkSyncedAndDecorated(_chunk);
	}

	// Token: 0x060056CD RID: 22221 RVA: 0x00214EA0 File Offset: 0x002130A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateDecosAllowedForChunk(Chunk _chunk, Chunk _c10, Chunk _c01)
	{
		Vector3 lhs = new Vector3(0f, 0f, 1f);
		Vector3 rhs = new Vector3(1f, 0f, 0f);
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				int terrainHeight = (int)_chunk.GetTerrainHeight(j, i);
				int num = (int)((j < 15) ? _chunk.GetTerrainHeight(j + 1, i) : _c10.GetTerrainHeight(0, i));
				int num2 = (int)((i < 15) ? _chunk.GetTerrainHeight(j, i + 1) : _c01.GetTerrainHeight(j, 0));
				if (terrainHeight >= 253 || num >= 253 || num2 >= 253)
				{
					_chunk.SetDecoAllowedAt(j, i, EnumDecoAllowed.Nothing);
				}
				else
				{
					float num3 = (float)_chunk.GetDensity(j, terrainHeight, i) / -128f;
					float num4 = (j < 15) ? ((float)_chunk.GetDensity(j + 1, num, i) / -128f) : ((float)_c10.GetDensity(0, num, i) / -128f);
					float num5 = (i < 15) ? ((float)_chunk.GetDensity(j, num2, i + 1) / -128f) : ((float)_c01.GetDensity(j, num2, 0) / -128f);
					float num6 = (float)_chunk.GetDensity(j, terrainHeight + 1, i) / 127f;
					float num7 = (j < 15) ? ((float)_chunk.GetDensity(j + 1, num + 1, i) / 127f) : ((float)_c10.GetDensity(0, num + 1, i) / 127f);
					float num8 = (i < 15) ? ((float)_chunk.GetDensity(j, num2 + 1, i + 1) / 127f) : ((float)_c01.GetDensity(j, num2 + 1, 0) / 127f);
					if (num3 > 0.999f && num6 > 0.999f)
					{
						num3 = 0.5f;
					}
					if (num4 > 0.999f && num7 > 0.999f)
					{
						num4 = 0.5f;
					}
					if (num5 > 0.999f && num8 > 0.999f)
					{
						num5 = 0.5f;
					}
					float num9 = (float)terrainHeight + num3;
					float num10 = (float)num + num4;
					float num11 = (float)num2 + num5;
					lhs.y = num11 - num9;
					rhs.y = num10 - num9;
					Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
					_chunk.SetTerrainNormal(j, i, normalized);
					if (normalized.y < 0.55f)
					{
						_chunk.SetDecoAllowedSlopeAt(j, i, EnumDecoAllowedSlope.Steep);
					}
					else if (normalized.y < 0.65f)
					{
						_chunk.SetDecoAllowedSlopeAt(j, i, EnumDecoAllowedSlope.Sloped);
					}
					if (terrainHeight <= 1 || terrainHeight >= 255 || _chunk.IsWater(j, terrainHeight, i) || _chunk.IsWater(j, terrainHeight + 1, i))
					{
						_chunk.SetDecoAllowedAt(j, i, EnumDecoAllowed.Nothing);
					}
				}
			}
		}
	}

	// Token: 0x060056CE RID: 22222 RVA: 0x0021514C File Offset: 0x0021334C
	public void UpdateDecorations(Chunk _chunk)
	{
		this.decorate(_chunk);
	}

	// Token: 0x060056CF RID: 22223 RVA: 0x00215158 File Offset: 0x00213358
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateDecorationsWherePossible(Chunk chunk)
	{
		World world = this.world;
		int x = chunk.X;
		int z = chunk.Z;
		this.tryToDecorate(chunk);
		this.tryToDecorate((Chunk)world.GetChunkSync(x - 1, z));
		this.tryToDecorate((Chunk)world.GetChunkSync(x, z - 1));
		this.tryToDecorate((Chunk)world.GetChunkSync(x - 1, z - 1));
	}

	// Token: 0x060056D0 RID: 22224 RVA: 0x002151C2 File Offset: 0x002133C2
	public override void UnloadChunk(Chunk _chunk)
	{
		if (this.bClientMode)
		{
			MemoryPools.PoolChunks.FreeSync(_chunk);
			return;
		}
		this.m_RegionFileManager.AddChunkSync(_chunk, false);
	}

	// Token: 0x060056D1 RID: 22225 RVA: 0x002151E8 File Offset: 0x002133E8
	public override void ReloadAllChunks()
	{
		object syncRoot = ((ICollection)this.m_ChunkQueue.list).SyncRoot;
		lock (syncRoot)
		{
			this.m_ChunkQueue.Clear();
		}
		this.m_RegionFileManager.Clear();
	}

	// Token: 0x060056D2 RID: 22226 RVA: 0x00215244 File Offset: 0x00213444
	public List<ChunkProviderParameter> GetParameters()
	{
		return this.m_Parameters;
	}

	// Token: 0x060056D3 RID: 22227 RVA: 0x0021524C File Offset: 0x0021344C
	public override void Update()
	{
		RegionFileManager regionFileManager = this.m_RegionFileManager;
		if (regionFileManager == null)
		{
			return;
		}
		regionFileManager.Update();
	}

	// Token: 0x060056D4 RID: 22228 RVA: 0x0021525E File Offset: 0x0021345E
	public override void StopUpdate()
	{
		if (this.threadInfo != null)
		{
			this.threadInfo.WaitForEnd(30);
			this.threadInfo = null;
		}
	}

	// Token: 0x060056D5 RID: 22229 RVA: 0x0021527C File Offset: 0x0021347C
	public override void Cleanup()
	{
		this.StopUpdate();
		if (this.spawnPointManager != null)
		{
			this.spawnPointManager.Cleanup();
		}
		if (this.m_RegionFileManager != null)
		{
			this.m_RegionFileManager.Cleanup();
		}
		MultiBlockManager.Instance.Cleanup();
	}

	// Token: 0x060056D6 RID: 22230 RVA: 0x002152B4 File Offset: 0x002134B4
	public override bool GetOverviewMap(Vector2i _startPos, Vector2i _size, Color[] mapColors)
	{
		this.m_RegionFileManager.SetCacheSize(1000);
		new TerrainMapGenerator().GenerateTerrain(this);
		this.m_RegionFileManager.SetCacheSize(0);
		return true;
	}

	// Token: 0x060056D7 RID: 22231 RVA: 0x002152DE File Offset: 0x002134DE
	public override IBiomeProvider GetBiomeProvider()
	{
		return this.m_BiomeProvider;
	}

	// Token: 0x060056D8 RID: 22232 RVA: 0x002152E6 File Offset: 0x002134E6
	public override ITerrainGenerator GetTerrainGenerator()
	{
		return this.m_TerrainGenerator;
	}

	// Token: 0x060056D9 RID: 22233 RVA: 0x002152EE File Offset: 0x002134EE
	public override SpawnPointList GetSpawnPointList()
	{
		return this.spawnPointManager.spawnPointList;
	}

	// Token: 0x060056DA RID: 22234 RVA: 0x002152FB File Offset: 0x002134FB
	public override void SetSpawnPointList(SpawnPointList _spawnPointList)
	{
		this.spawnPointManager.spawnPointList = _spawnPointList;
	}

	// Token: 0x060056DB RID: 22235 RVA: 0x0021530C File Offset: 0x0021350C
	public override void RebuildTerrain(HashSetLong _chunks, Vector3i _areaStart, Vector3i _areaSize, bool _isStopStabilityCalc, bool _isRegenChunk, bool _isFillEmptyBlocks, bool _isReset)
	{
		ChunkCluster chunkCache = this.world.ChunkCache;
		foreach (long key in _chunks)
		{
			Chunk chunkSync = chunkCache.GetChunkSync(key);
			if (chunkSync != null)
			{
				GameRandom gameRandom = Utils.RandomFromSeedOnPos(chunkSync.X, chunkSync.Z, this.world.Seed);
				chunkSync.StopStabilityCalculation = _isStopStabilityCalc;
				this.generateTerrain(this.world, chunkSync, gameRandom, _areaStart, _areaSize, _isFillEmptyBlocks, _isReset);
				GameRandomManager.Instance.FreeGameRandom(gameRandom);
				if (_isRegenChunk)
				{
					chunkSync.NeedsRegeneration = true;
				}
			}
		}
	}

	// Token: 0x060056DC RID: 22236 RVA: 0x002153BC File Offset: 0x002135BC
	public void RemoveChunks(HashSetLong _chunks)
	{
		if (this.m_RegionFileManager != null)
		{
			this.m_RegionFileManager.RemoveChunks(_chunks, true, true);
		}
	}

	// Token: 0x060056DD RID: 22237 RVA: 0x002153D4 File Offset: 0x002135D4
	public override ChunkProtectionLevel GetChunkProtectionLevel(Vector3i worldPos)
	{
		return this.m_RegionFileManager.GetChunkProtectionLevelForWorldPos(worldPos);
	}

	// Token: 0x060056DE RID: 22238 RVA: 0x002153E2 File Offset: 0x002135E2
	public void CheckPersistentData()
	{
		this.m_RegionFileManager.CheckPersistentData();
	}

	// Token: 0x060056DF RID: 22239 RVA: 0x002153F0 File Offset: 0x002135F0
	public void LogCurrentChunkGeneration()
	{
		Log.Error(string.Format("ChunkProvider Currently Generating Data for Chunk - Key: {0}, Pos: {1}/{2}, Duration {3}", new object[]
		{
			this.currentGeneratingChunk,
			WorldChunkCache.extractX(this.currentGeneratingChunk) << 4,
			WorldChunkCache.extractZ(this.currentGeneratingChunk) << 4,
			this.chunkGenerationTimer.Elapsed.TotalSeconds
		}));
	}

	// Token: 0x0400432D RID: 17197
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int cCacheChunks = 0;

	// Token: 0x0400432E RID: 17198
	[PublicizedFrom(EAccessModifier.Protected)]
	public World world;

	// Token: 0x0400432F RID: 17199
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly PathAbstractions.AbstractedLocation worldLocation;

	// Token: 0x04004330 RID: 17200
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly string levelName;

	// Token: 0x04004331 RID: 17201
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly string gameName;

	// Token: 0x04004332 RID: 17202
	[PublicizedFrom(EAccessModifier.Protected)]
	public DynamicPrefabDecorator prefabDecorator;

	// Token: 0x04004333 RID: 17203
	[PublicizedFrom(EAccessModifier.Protected)]
	public EventPrefabs eventPrefabs;

	// Token: 0x04004334 RID: 17204
	[PublicizedFrom(EAccessModifier.Protected)]
	public SpawnPointManager spawnPointManager;

	// Token: 0x04004335 RID: 17205
	[PublicizedFrom(EAccessModifier.Protected)]
	public RegionFileManager m_RegionFileManager;

	// Token: 0x04004336 RID: 17206
	[PublicizedFrom(EAccessModifier.Protected)]
	public IBiomeProvider m_BiomeProvider;

	// Token: 0x04004337 RID: 17207
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<ChunkProviderParameter> m_Parameters = new List<ChunkProviderParameter>();

	// Token: 0x04004338 RID: 17208
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<IWorldDecorator> m_Decorators = new List<IWorldDecorator>();

	// Token: 0x04004339 RID: 17209
	[PublicizedFrom(EAccessModifier.Protected)]
	public ITerrainGenerator m_TerrainGenerator;

	// Token: 0x0400433A RID: 17210
	public HashSetList<long> m_ChunkQueue = new HashSetList<long>();

	// Token: 0x0400433B RID: 17211
	[PublicizedFrom(EAccessModifier.Private)]
	public AutoResetEvent m_WaitHandle = new AutoResetEvent(false);

	// Token: 0x0400433C RID: 17212
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo threadInfo;

	// Token: 0x0400433D RID: 17213
	[PublicizedFrom(EAccessModifier.Private)]
	public Stopwatch chunkGenerationTimer = new Stopwatch();

	// Token: 0x0400433E RID: 17214
	[PublicizedFrom(EAccessModifier.Private)]
	public long currentGeneratingChunk;

	// Token: 0x0400433F RID: 17215
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bClientMode;
}
