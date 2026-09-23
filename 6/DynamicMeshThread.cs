using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConcurrentCollections;
using UnityEngine;

// Token: 0x020003A7 RID: 935
public class DynamicMeshThread
{
	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06001BD9 RID: 7129 RVA: 0x000A52E1 File Offset: 0x000A34E1
	public static int MeshGenCount
	{
		get
		{
			return DynamicMeshThread.ChunkMeshGenRequests.Count + DynamicMeshThread.TempChunkMeshGenRequests.Count;
		}
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06001BDA RID: 7130 RVA: 0x000A52F8 File Offset: 0x000A34F8
	public static float time
	{
		get
		{
			return (float)(DateTime.Now - DynamicMeshThread.StartTime).TotalSeconds;
		}
	}

	// Token: 0x17000351 RID: 849
	// (get) Token: 0x06001BDC RID: 7132 RVA: 0x0009CE8B File Offset: 0x0009B08B
	public static bool IsServer
	{
		get
		{
			return SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		}
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x000A5320 File Offset: 0x000A3520
	public static void AddChunkGameObject(Chunk chunk)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (!chunk.NeedsOnlyCollisionMesh)
		{
			DynamicMeshManager.ChunkGameObjects.Add(chunk.Key);
			if (DynamicMeshManager.Instance != null)
			{
				DynamicMeshItem itemOrNull = DynamicMeshManager.Instance.GetItemOrNull(chunk.GetWorldPos());
				if (itemOrNull != null)
				{
					itemOrNull.ForceHide();
					itemOrNull.GetRegion().OnChunkVisible(itemOrNull);
				}
			}
		}
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x000A5383 File Offset: 0x000A3583
	public static void RemoveChunkGameObject(long key)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (DynamicMeshManager.Instance != null)
		{
			DynamicMeshManager.ChunkGameObjects.Remove(key);
			DynamicMeshManager.Instance.ShowChunk(key);
		}
	}

	// Token: 0x06001BDF RID: 7135 RVA: 0x000A53B4 File Offset: 0x000A35B4
	public static void CleanUp()
	{
		DynamicMeshThread.ChunksToProcess.Clear();
		DynamicMeshThread.ChunksToLoad.Clear();
		DynamicMeshThread.RequestThreadStop = true;
		DynamicMeshThread.BuilderManager.StopThreads(true);
		DynamicMeshServer.CleanUp();
		DynamicMeshThread.ServerUpdates.Clear();
		ConcurrentDictionary<long, DynamicMeshUpdateData> regionUpdates = DynamicMeshThread.RegionUpdates;
		if (regionUpdates != null)
		{
			regionUpdates.Clear();
		}
		DynamicMeshThread.ClearData();
		Queue<DynamicMeshItem> toGenerate = DynamicMeshThread.ToGenerate;
		if (toGenerate != null)
		{
			toGenerate.Clear();
		}
		LinkedList<DynamicMeshItem> needObservers = DynamicMeshThread.NeedObservers;
		if (needObservers != null)
		{
			needObservers.Clear();
		}
		LinkedList<DynamicMeshItem> ignoredChunks = DynamicMeshThread.IgnoredChunks;
		if (ignoredChunks != null)
		{
			ignoredChunks.Clear();
		}
		ConcurrentDictionary<long, DynamicMeshItem> primaryQueue = DynamicMeshThread.PrimaryQueue;
		if (primaryQueue != null)
		{
			primaryQueue.Clear();
		}
		ConcurrentDictionary<long, DynamicMeshItem> secondaryQueue = DynamicMeshThread.SecondaryQueue;
		if (secondaryQueue != null)
		{
			secondaryQueue.Clear();
		}
		List<ChunkGameObject> loadedGos = DynamicMeshThread.LoadedGos;
		if (loadedGos != null)
		{
			loadedGos.Clear();
		}
		Queue<ChunkGameObject> toRemoveGos = DynamicMeshThread.ToRemoveGos;
		if (toRemoveGos != null)
		{
			toRemoveGos.Clear();
		}
		ConcurrentDictionary<long, DynamicMeshThread.ThreadRegion> concurrentDictionary = DynamicMeshThread.threadRegions;
		if (concurrentDictionary != null)
		{
			concurrentDictionary.Clear();
		}
		DynamicMeshThread.nextChunks = new ConcurrentQueue<long>();
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x000A5494 File Offset: 0x000A3694
	public static bool AddChunkUpdateFromServer(DynamicMeshServerUpdates data)
	{
		DynamicMeshThread.ServerUpdates.Enqueue(data);
		DynamicMeshManager.Instance.AddChunkStub(new Vector3i(data.ChunkX, data.StartY, data.ChunkZ), null);
		return true;
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x000A54C4 File Offset: 0x000A36C4
	public static void AddRegionChunk(int worldX, int worldZ, long key)
	{
		DynamicMeshThread.GetThreadRegion(worldX, worldZ).AddLoadedChunk(key);
	}

	// Token: 0x06001BE2 RID: 7138 RVA: 0x000A54D3 File Offset: 0x000A36D3
	public static void RemoveRegionChunk(int worldX, int worldZ, long key)
	{
		if (!DynamicMeshThread.GetThreadRegion(worldX, worldZ).RemoveLoadedChunk(key) && DynamicMeshManager.DoLog)
		{
			Log.Warning("Failed to remove threaded chunk");
		}
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x000A54F8 File Offset: 0x000A36F8
	public static bool AddRegionUpdateData(int worldX, int worldZ, bool isUrgent)
	{
		if (GameManager.IsDedicatedServer)
		{
			return false;
		}
		DynamicMeshThread.ThreadRegion threadRegion = DynamicMeshThread.GetThreadRegion(worldX, worldZ);
		DynamicMeshUpdateData dynamicMeshUpdateData;
		DynamicMeshThread.RegionUpdates.TryGetValue(threadRegion.Key, out dynamicMeshUpdateData);
		if (dynamicMeshUpdateData == null)
		{
			dynamicMeshUpdateData = new DynamicMeshUpdateData();
			dynamicMeshUpdateData.ChunkPosition.x = threadRegion.X;
			dynamicMeshUpdateData.ChunkPosition.z = threadRegion.Z;
			dynamicMeshUpdateData.Key = threadRegion.Key;
			dynamicMeshUpdateData.IsUrgent = false;
			DynamicMeshThread.RegionUpdates.TryAdd(dynamicMeshUpdateData.Key, dynamicMeshUpdateData);
		}
		dynamicMeshUpdateData.UpdateTime = DynamicMeshThread.time + (float)(isUrgent ? 0 : 3);
		dynamicMeshUpdateData.IsUrgent = (dynamicMeshUpdateData.IsUrgent || isUrgent);
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(string.Concat(new string[]
			{
				"Adding thread region update ",
				threadRegion.ToDebugLocation(),
				" Time: ",
				dynamicMeshUpdateData.UpdateTime.ToString(),
				" urgent: ",
				isUrgent.ToString()
			}));
		}
		return true;
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x000A55EC File Offset: 0x000A37EC
	public static DynamicMeshThread.ThreadRegion GetThreadRegion(Vector3i worldPos)
	{
		return DynamicMeshThread.GetThreadRegionInternal(DynamicMeshUnity.GetRegionKeyFromWorldPosition(worldPos));
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x000A55F9 File Offset: 0x000A37F9
	public static DynamicMeshThread.ThreadRegion GetThreadRegion(int worldX, int worldZ)
	{
		return DynamicMeshThread.GetThreadRegionInternal(DynamicMeshUnity.GetRegionKeyFromWorldPosition(worldX, worldZ));
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x000A5607 File Offset: 0x000A3807
	public static DynamicMeshThread.ThreadRegion GetThreadRegion(long key)
	{
		return DynamicMeshThread.GetThreadRegionInternal(DynamicMeshUnity.GetRegionKeyFromItemKey(key));
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x000A5614 File Offset: 0x000A3814
	[PublicizedFrom(EAccessModifier.Private)]
	public static DynamicMeshThread.ThreadRegion GetThreadRegionInternal(long key)
	{
		DynamicMeshThread.ThreadRegion threadRegion;
		if (!DynamicMeshThread.threadRegions.TryGetValue(key, out threadRegion))
		{
			threadRegion = new DynamicMeshThread.ThreadRegion(key);
			if (!DynamicMeshThread.threadRegions.TryAdd(key, threadRegion))
			{
				DynamicMeshThread.threadRegions.TryGetValue(key, out threadRegion);
			}
		}
		return threadRegion;
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x000A5654 File Offset: 0x000A3854
	public static void SetNextChunksFromQueues()
	{
		DynamicMeshThread.QueueUpdateOverride = true;
		foreach (KeyValuePair<long, DynamicMeshItem> keyValuePair in DynamicMeshThread.PrimaryQueue)
		{
			DynamicMeshThread.nextChunks.Enqueue(keyValuePair.Key);
		}
		foreach (KeyValuePair<long, DynamicMeshItem> keyValuePair2 in DynamicMeshThread.SecondaryQueue)
		{
			DynamicMeshThread.nextChunks.Enqueue(keyValuePair2.Key);
		}
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x000A56F8 File Offset: 0x000A38F8
	public static void SetNextChunks(long key)
	{
		if (DynamicMeshThread.nextChunks.Count > 512)
		{
			return;
		}
		int num = WorldChunkCache.extractX(key);
		int num2 = WorldChunkCache.extractZ(key);
		long item = WorldChunkCache.MakeChunkKey(num, num2 + 1);
		long item2 = WorldChunkCache.MakeChunkKey(num, num2 - 1);
		long item3 = WorldChunkCache.MakeChunkKey(num + 1, num2);
		long item4 = WorldChunkCache.MakeChunkKey(num + 1, num2 + 1);
		long item5 = WorldChunkCache.MakeChunkKey(num + 1, num2 - 1);
		long item6 = WorldChunkCache.MakeChunkKey(num - 1, num2);
		long item7 = WorldChunkCache.MakeChunkKey(num - 1, num2 + 1);
		long item8 = WorldChunkCache.MakeChunkKey(num - 1, num2 - 1);
		DynamicMeshThread.nextChunks.Enqueue(key);
		DynamicMeshThread.nextChunks.Enqueue(item);
		DynamicMeshThread.nextChunks.Enqueue(item2);
		DynamicMeshThread.nextChunks.Enqueue(item3);
		DynamicMeshThread.nextChunks.Enqueue(item4);
		DynamicMeshThread.nextChunks.Enqueue(item5);
		DynamicMeshThread.nextChunks.Enqueue(item6);
		DynamicMeshThread.nextChunks.Enqueue(item7);
		DynamicMeshThread.nextChunks.Enqueue(item8);
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x000A57E8 File Offset: 0x000A39E8
	public static void RequestChunk(long key)
	{
		DynamicMeshThread.nextChunks.Enqueue(key);
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x000A57F8 File Offset: 0x000A39F8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetNextChunkToLoad()
	{
		if (DynamicMeshThread.nextChunks.Count > 0)
		{
			return;
		}
		if (DynamicMeshThread.PrimaryQueue.Count == 0 && DynamicMeshThread.SecondaryQueue.Count == 0)
		{
			return;
		}
		DynamicMeshManager instance = DynamicMeshManager.Instance;
		if (instance == null)
		{
			return;
		}
		if (instance.NearestRegionWithUnloaded == null)
		{
			if (!instance.FindNearestUnloadedItems)
			{
				instance.FindNearestUnloadedItems = true;
				if (DynamicMeshThread.PrimaryQueue.Count > 0)
				{
					DynamicMeshItem dynamicMeshItem = (from d in DynamicMeshThread.PrimaryQueue
					where d.Value.WorldPosition != d.Value.GetRegionLocation()
					select d.Value).FirstOrDefault<DynamicMeshItem>();
					instance.PrimaryLocation = ((dynamicMeshItem != null) ? new Vector3i?(dynamicMeshItem.WorldPosition) : null);
					return;
				}
				if (instance.PrimaryLocation == null && DynamicMeshThread.SecondaryQueue.Count > 0)
				{
					DynamicMeshItem dynamicMeshItem2 = (from d in DynamicMeshThread.SecondaryQueue
					where d.Value.WorldPosition != d.Value.GetRegionLocation()
					select d.Value).FirstOrDefault<DynamicMeshItem>();
					instance.PrimaryLocation = ((dynamicMeshItem2 != null) ? new Vector3i?(dynamicMeshItem2.WorldPosition) : null);
				}
			}
			return;
		}
		DynamicMeshRegion nearestRegionWithUnloaded = instance.NearestRegionWithUnloaded;
		List<DynamicMeshItem> list = nearestRegionWithUnloaded.UnloadedItems;
		if (!DynamicMeshThread.QueueUpdateOverride && GameManager.Instance.World.ChunkCache.chunks.Count > 600)
		{
			GameManager.Instance.World.m_ChunkManager.ForceUpdate();
		}
		instance.NearestRegionWithUnloaded = null;
		for (int i = 0; i < list.Count; i++)
		{
			DynamicMeshItem dynamicMeshItem3 = list[i];
			if (dynamicMeshItem3 != null)
			{
				long key = dynamicMeshItem3.Key;
				if (DynamicMeshThread.SecondaryQueue.ContainsKey(dynamicMeshItem3.Key))
				{
					DynamicMeshThread.RequestPrimaryQueue(dynamicMeshItem3);
				}
				DynamicMeshThread.SetNextChunks(key);
			}
		}
		list = nearestRegionWithUnloaded.LoadedItems;
		for (int i = 0; i < list.Count; i++)
		{
			DynamicMeshItem dynamicMeshItem4 = list[i];
			long key2 = dynamicMeshItem4.Key;
			if (DynamicMeshThread.SecondaryQueue.ContainsKey(dynamicMeshItem4.Key))
			{
				DynamicMeshThread.RequestPrimaryQueue(dynamicMeshItem4);
			}
			DynamicMeshThread.SetNextChunks(key2);
		}
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x000A5A4C File Offset: 0x000A3C4C
	public static ConcurrentDictionary<long, DynamicMeshItem> GetQueue(bool isPrimary)
	{
		if (!isPrimary)
		{
			return DynamicMeshThread.SecondaryQueue;
		}
		return DynamicMeshThread.PrimaryQueue;
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x000A5A5C File Offset: 0x000A3C5C
	public static long GetNextChunkToLoad()
	{
		if (DynamicMeshThread.RequestThreadStop)
		{
			return long.MaxValue;
		}
		if (DynamicMeshThread.nextChunks.Count <= 0)
		{
			return long.MaxValue;
		}
		long result;
		if (!DynamicMeshThread.nextChunks.TryDequeue(out result))
		{
			return long.MaxValue;
		}
		return result;
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x000A5AAC File Offset: 0x000A3CAC
	public static void RequestSecondaryQueue(DynamicMeshItem item)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			return;
		}
		long key = item.Key;
		if (DynamicMeshThread.PrimaryQueue.ContainsKey(key))
		{
			return;
		}
		if (!DynamicMeshThread.SecondaryQueue.ContainsKey(key))
		{
			DynamicMeshThread.GetThreadRegion(item.WorldPosition);
			DynamicMeshThread.SecondaryQueue.TryAdd(key, item);
			DynamicMeshThread.ChunksToLoad.Add(key);
			DynamicMeshThread.ChunksToProcess.Add(key);
		}
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x000A5B18 File Offset: 0x000A3D18
	public static void RequestPrimaryQueue(DynamicMeshItem item)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			return;
		}
		long key = item.Key;
		DynamicMeshThread.GetThreadRegion(item.WorldPosition);
		if (DynamicMeshThread.SecondaryQueue.ContainsKey(key))
		{
			DynamicMeshItem dynamicMeshItem;
			DynamicMeshThread.SecondaryQueue.TryRemove(key, out dynamicMeshItem);
		}
		if (DynamicMeshManager.Instance.PrefabCheck != PrefabCheckState.Run)
		{
			Vector3i regionLocation = item.GetRegionLocation();
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation, item.WorldPosition);
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation + new Vector3i(160, 0, 160), item.WorldPosition);
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation + new Vector3i(160, 0, -160), item.WorldPosition);
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation + new Vector3i(-160, 0, 160), item.WorldPosition);
			DynamicMeshThread.CheckSecondaryQueueForRegion(regionLocation + new Vector3i(-160, 0, -160), item.WorldPosition);
		}
		if (!DynamicMeshThread.PrimaryQueue.ContainsKey(key))
		{
			DynamicMeshThread.PrimaryQueue.TryAdd(key, item);
			DynamicMeshThread.ChunksToLoad.Add(key);
			DynamicMeshThread.ChunksToProcess.Add(key);
		}
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x000A5C38 File Offset: 0x000A3E38
	public static void CheckSecondaryQueueForRegion(Vector3i regionPos, Vector3i itemPos)
	{
		for (int i = regionPos.x; i < regionPos.x + 160; i += 16)
		{
			for (int j = regionPos.z; j < regionPos.z + 160; j += 16)
			{
				long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(i), World.toChunkXZ(j));
				DynamicMeshItem value;
				if (DynamicMeshThread.SecondaryQueue.TryRemove(key, out value))
				{
					DynamicMeshThread.PrimaryQueue.TryAdd(key, value);
				}
			}
		}
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x000A5CAD File Offset: 0x000A3EAD
	public static void SetDefaultThreads()
	{
		DynamicMeshBuilderManager.MaxBuilderThreads = Math.Min(Math.Min(8, Math.Max(SystemInfo.processorCount - 2, 1)), DynamicMeshSettings.MaxDyMeshData + 1);
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x000A5CD4 File Offset: 0x000A3ED4
	public static void StartThread()
	{
		DynamicMeshThread.StopThreadForce();
		DynamicMeshThread.ClearData();
		DynamicMeshThread.RequestThreadStop = false;
		DynamicMeshThread.ChunkDataQueue = new DynamicMeshChunkDataStorage<DynamicMeshItem>(DynamicMeshThread.CachePurgeInterval);
		if (DynamicMeshThread.ChunkDataQueue.MaxAllowedItems == 0)
		{
			DynamicMeshThread.ChunkDataQueue.MaxAllowedItems = 300;
		}
		DynamicMeshThread.ChunksToLoad.Clear();
		DynamicMeshThread.ChunksToProcess.Clear();
		DynamicMeshThread.SetDefaultThreads();
		DynamicMeshThread.StartTime = DateTime.Now;
		DynamicMeshThread.MeshThread = new Thread(delegate()
		{
			while (GameManager.Instance == null || GameManager.Instance.World == null)
			{
				Thread.Sleep(100);
			}
			Log.Out("Dynamic thread starting");
			DynamicMeshThread.RequestThreadStop = false;
			if (DynamicMeshManager.IsValidGameMode())
			{
				while (!DynamicMeshThread.RequestThreadStop)
				{
					DynamicMeshThread.GenerationThread();
				}
			}
			if (!DynamicMeshThread.RequestThreadStop)
			{
				Log.Error("Dynamic thread stopped");
			}
			DynamicMeshThread.ClearData();
		});
		DynamicMeshThread.MeshThread.Start();
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x000A5D74 File Offset: 0x000A3F74
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ClearData()
	{
		DynamicMeshData dynamicMeshData;
		while (DynamicMeshThread.ReadyForCollection.TryDequeue(out dynamicMeshData))
		{
		}
		Vector2i vector2i;
		while (DynamicMeshThread.ChunkReadyForCollection.TryRemoveFirst(out vector2i))
		{
		}
		DynamicMeshThread.RegionStorage.ClearQueues();
		while (DynamicMeshThread.ToGenerate.Count != 0 && DynamicMeshThread.ToGenerate.Dequeue() != null)
		{
		}
		DynamicMeshThread.PrimaryQueue.Clear();
		DynamicMeshThread.SecondaryQueue.Clear();
		DynamicMeshThread.NeedObservers.Clear();
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x000A5DDD File Offset: 0x000A3FDD
	public static void StopThreadRequest()
	{
		DynamicMeshThread.RequestThreadStop = true;
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x000A5DE8 File Offset: 0x000A3FE8
	public static void StopThreadForce()
	{
		if (DynamicMeshThread.MeshThread != null)
		{
			try
			{
				DynamicMeshThread.ChunkDataQueue.ClearQueues();
				DynamicMeshThread.MeshThread.Abort();
			}
			catch (Exception ex)
			{
				if (DynamicMeshManager.DoLog)
				{
					DynamicMeshManager.LogMsg("Dynamic Mesh Thread abort error " + ex.Message);
				}
			}
		}
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x000A5E44 File Offset: 0x000A4044
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GetKeys(WorldChunkCache cache)
	{
		DynamicMeshThread.Keys.Clear();
		ReaderWriterLockSlim syncRoot = cache.GetSyncRoot();
		syncRoot.EnterReadLock();
		try
		{
			foreach (long item in cache.chunkKeys)
			{
				DynamicMeshThread.Keys.Add(item);
			}
		}
		finally
		{
			syncRoot.ExitReadLock();
		}
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x000A5EC8 File Offset: 0x000A40C8
	public static void RemoveFromQueues(long key)
	{
		DynamicMeshItem dynamicMeshItem;
		DynamicMeshThread.PrimaryQueue.TryRemove(key, out dynamicMeshItem);
		DynamicMeshThread.SecondaryQueue.TryRemove(key, out dynamicMeshItem);
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x000A5EF4 File Offset: 0x000A40F4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GenerationThread()
	{
		try
		{
			if (DateTime.Now < DynamicMeshThread.NextRun)
			{
				Thread.Sleep((int)Math.Max(1.0, (DynamicMeshThread.NextRun - DateTime.Now).TotalMilliseconds));
			}
			else if (DynamicMeshThread.Paused || DynamicMeshManager.Instance == null)
			{
				DynamicMeshThread.NextRun = DateTime.Now.AddMilliseconds(500.0);
			}
			else
			{
				if (DynamicMeshThread.ChunkDataQueue.ChunkData.Count < DynamicMeshThread.ChunkDataQueue.LiveItems)
				{
					DynamicMeshThread.ChunkDataQueue.LiveItems = DynamicMeshThread.ChunkDataQueue.ChunkData.Count;
				}
				while (DynamicMeshThread.NewlyLoadedGos.Count > 0)
				{
					DynamicMeshThread.LoadedGos.Add(DynamicMeshThread.NewlyLoadedGos.Dequeue());
				}
				while (DynamicMeshThread.ToRemoveGos.Count > 0)
				{
					DynamicMeshThread.LoadedGos.Remove(DynamicMeshThread.ToRemoveGos.Dequeue());
				}
				DynamicMeshThread.HandleRegionLoads();
				DynamicMeshThread.BuilderManager.CheckBuilders();
				DynamicMeshThread.AddRegionChecks = (DynamicMeshThread.PrimaryQueue.Count == 0 && DynamicMeshThread.SecondaryQueue.Count == 0);
				bool hasThreadAvailable = DynamicMeshThread.BuilderManager.HasThreadAvailable;
				if ((DynamicMeshThread.ChunkMeshGenRequests.Count == 0 && DynamicMeshThread.RegionUpdates.Count == 0 && DynamicMeshThread.PrimaryQueue.Count == 0 && DynamicMeshThread.SecondaryQueue.Count == 0 && DynamicMeshThread.ChunkMeshGenRequests.Count == 0) || !hasThreadAvailable)
				{
					if (DynamicMeshSettings.NewWorldFullRegen && hasThreadAvailable && DynamicMeshManager.Instance.PrefabCheck == PrefabCheckState.WaitingForCompleteCheck)
					{
						DynamicMeshThread.WriteChecksComplete();
						DynamicMeshServer.ProcessDelayedPackages();
					}
					DynamicMeshThread.NextRun = DateTime.Now.AddMilliseconds(300.0);
				}
				else
				{
					DynamicMeshThread.SetNextChunkToLoad();
					DynamicMeshThread.ProcessRegionRegenRequests();
					DynamicMeshThread.ProcessMeshGenerationRequests();
					bool flag = false;
					DynamicMeshThread.GetKeys(GameManager.Instance.World.ChunkCache);
					DynamicMeshThread.Queue = DynamicMeshThread.QueuePrimary;
					if (DynamicMeshThread.PrimaryQueue.Count > 0)
					{
						flag = DynamicMeshThread.ProcessQueue(DynamicMeshThread.PrimaryQueue);
					}
					if (!flag)
					{
						if (DynamicMeshThread.PrimaryQueue.Count > 0)
						{
							if (DynamicMeshManager.DoLog)
							{
								Log.Out("Setting chunks");
							}
							foreach (long num in DynamicMeshThread.PrimaryQueue.Keys)
							{
								if (!DynamicMeshThread.Keys.Contains(num) && !DynamicMeshThread.nextChunks.Contains(num))
								{
									DynamicMeshThread.SetNextChunks(num);
								}
							}
						}
						if (DynamicMeshThread.SecondaryQueue.Count > 0)
						{
							DynamicMeshThread.Queue = DynamicMeshThread.QueueSecondary;
							flag = DynamicMeshThread.ProcessQueue(DynamicMeshThread.SecondaryQueue);
						}
					}
					DynamicMeshThread.Queue = DynamicMeshThread.QueueNone;
				}
			}
		}
		catch (Exception ex)
		{
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Process requests " + ex.Message + "\n" + ex.StackTrace);
			}
		}
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x000A61FC File Offset: 0x000A43FC
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ProcessQueue(ConcurrentDictionary<long, DynamicMeshItem> queue)
	{
		if (DynamicMeshThread.RequestThreadStop)
		{
			return false;
		}
		if (!DynamicMeshThread.BuilderManager.HasThreadAvailable)
		{
			return false;
		}
		bool result = false;
		int num = 0;
		int num2 = 0;
		bool flag = queue == DynamicMeshThread.PrimaryQueue;
		if (DynamicMeshManager.DoLog)
		{
			Log.Out(string.Format("Checking {0} keys", DynamicMeshThread.Keys.Count));
		}
		foreach (long num3 in DynamicMeshThread.Keys)
		{
			if (DynamicMeshThread.RequestThreadStop)
			{
				return false;
			}
			if (DynamicMeshThread.PrimaryQueue.Count > 0 && !flag)
			{
				break;
			}
			DynamicMeshItem dynamicMeshItem;
			if (queue.TryGetValue(num3, out dynamicMeshItem))
			{
				Chunk chunkSync = GameManager.Instance.World.ChunkCache.GetChunkSync(num3);
				if (chunkSync == null || !DynamicMeshChunkProcessor.IsChunkLoaded(chunkSync))
				{
					if (DynamicMeshManager.DoLog)
					{
						Log.Out(dynamicMeshItem.ToDebugLocation() + " not in world cache");
						DynamicMeshThread.nextChunks.Enqueue(num3);
					}
					num++;
				}
				else if (!GameManager.Instance.World.ChunkCache.HasNeighborChunks(chunkSync))
				{
					num2++;
					DynamicMeshThread.SetNextChunks(chunkSync.Key);
					if (DynamicMeshManager.DoLog)
					{
						Log.Out(dynamicMeshItem.ToDebugLocation() + " no neighbours");
					}
				}
				else
				{
					result = true;
					int num4 = DynamicMeshThread.BuilderManager.AddItemForExport(dynamicMeshItem, flag);
					if (num4 == 1)
					{
						DynamicMeshItem dynamicMeshItem2;
						queue.TryRemove(num3, out dynamicMeshItem2);
					}
					if (num4 != -1 && !DynamicMeshThread.BuilderManager.HasThreadAvailable)
					{
						break;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x000A63A8 File Offset: 0x000A45A8
	public static void WriteChecksComplete()
	{
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("All chunks checked. Disabling future checks");
		}
		SdFile.WriteAllText(DynamicMeshFile.MeshLocation + "!!ChunksChecked.info", DynamicMeshThread.time.ToString());
		DynamicMeshManager.Instance.PrefabCheck = PrefabCheckState.Run;
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x000A63F2 File Offset: 0x000A45F2
	public static void AddChunkGenerationRequest(DynamicMeshItem item)
	{
		DynamicMeshThread.AddRegionChunk(item.WorldPosition.x, item.WorldPosition.z, item.Key);
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		DynamicMeshThread.ChunkMeshGenRequests.Enqueue(item);
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x000A6428 File Offset: 0x000A4628
	public static void AddRegionLoadRequest(DyMeshRegionLoadRequest request)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (DynamicMeshManager.DoLog)
		{
			Log.Out("Loading region go " + DynamicMeshUnity.GetDebugPositionFromKey(request.Key));
		}
		DynamicMeshThread.RegionFileLoadRequests.Enqueue(request);
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x000A6460 File Offset: 0x000A4660
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ProcessMeshGenerationRequests()
	{
		DynamicMeshItem dynamicMeshItem;
		if (!DynamicMeshThread.ChunkMeshGenRequests.TryDequeue(out dynamicMeshItem))
		{
			return;
		}
		DynamicMeshThread.TempChunkMeshGenRequests.Enqueue(dynamicMeshItem);
		DynamicMeshItem dynamicMeshItem2 = dynamicMeshItem;
		float num = dynamicMeshItem.DistanceToPlayer(DynamicMeshThread.PlayerPositionX, DynamicMeshThread.PlayerPositionZ);
		DynamicMeshItem dynamicMeshItem3;
		while (DynamicMeshThread.ChunkMeshGenRequests.TryDequeue(out dynamicMeshItem3))
		{
			if ((DynamicMeshThread.PlayerPositionX != 0f || DynamicMeshThread.PlayerPositionZ != 0f) && !DynamicMeshUnity.IsInBuffer(DynamicMeshThread.PlayerPositionX, DynamicMeshThread.PlayerPositionZ, DynamicMeshRegion.ItemLoadIndex, dynamicMeshItem3.WorldPosition.x / 160, dynamicMeshItem3.WorldPosition.z / 160))
			{
				dynamicMeshItem3.State = DynamicItemState.Waiting;
			}
			else
			{
				DynamicMeshThread.TempChunkMeshGenRequests.Enqueue(dynamicMeshItem3);
				float num2 = dynamicMeshItem3.DistanceToPlayer(DynamicMeshThread.PlayerPositionX, DynamicMeshThread.PlayerPositionZ);
				if (num2 < num)
				{
					num = num2;
					dynamicMeshItem2 = dynamicMeshItem3;
				}
			}
		}
		DynamicMeshItem dynamicMeshItem4;
		while (DynamicMeshThread.TempChunkMeshGenRequests.TryDequeue(out dynamicMeshItem4))
		{
			if (dynamicMeshItem4 != dynamicMeshItem2)
			{
				DynamicMeshThread.ChunkMeshGenRequests.Enqueue(dynamicMeshItem4);
			}
		}
		DynamicMeshThread.GetThreadRegion(dynamicMeshItem2.Key);
		if (!dynamicMeshItem2.FileExists())
		{
			dynamicMeshItem2.State = DynamicItemState.Empty;
			return;
		}
		if (DynamicMeshThread.BuilderManager.AddItemForMeshGeneration(dynamicMeshItem2, false) != 1)
		{
			DynamicMeshThread.ChunkMeshGenRequests.Enqueue(dynamicMeshItem2);
		}
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x000A658C File Offset: 0x000A478C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ProcessRegionRegenRequests()
	{
		bool useAllThreads = false;
		foreach (KeyValuePair<long, DynamicMeshUpdateData> keyValuePair in DynamicMeshThread.RegionUpdates)
		{
			if (DynamicMeshThread.RequestThreadStop)
			{
				break;
			}
			DynamicMeshUpdateData value = keyValuePair.Value;
			DynamicMeshThread.ThreadRegion threadRegionInternal = DynamicMeshThread.GetThreadRegionInternal(value.Key);
			if (threadRegionInternal.LoadedChunkCount > 0 && value.UpdateTime < DynamicMeshThread.time)
			{
				if (DynamicMeshThread.BuilderManager.RegenerateRegion(threadRegionInternal, useAllThreads) == 1)
				{
					DynamicMeshThread.RegionUpdates.TryRemove(keyValuePair.Key, out value);
					break;
				}
				break;
			}
		}
	}

	// Token: 0x06001BFF RID: 7167 RVA: 0x000A6630 File Offset: 0x000A4830
	[PublicizedFrom(EAccessModifier.Private)]
	public static void HandleRegionLoads()
	{
		DyMeshRegionLoadRequest dyMeshRegionLoadRequest;
		while (DynamicMeshThread.RegionFileLoadRequests.TryDequeue(out dyMeshRegionLoadRequest))
		{
			if (DynamicMeshManager.DoLog)
			{
				Log.Out("Loading region from storage " + DynamicMeshUnity.GetDebugPositionFromKey(dyMeshRegionLoadRequest.Key));
			}
			DynamicMeshThread.RegionStorage.LoadRegion(dyMeshRegionLoadRequest);
			DynamicMeshManager.Instance.RegionFileLoadRequests.Enqueue(dyMeshRegionLoadRequest);
		}
	}

	// Token: 0x040011FB RID: 4603
	public static bool Paused = false;

	// Token: 0x040011FC RID: 4604
	public static bool NoProcessing = false;

	// Token: 0x040011FD RID: 4605
	public static bool LockMeshesAfterGenerating = true;

	// Token: 0x040011FE RID: 4606
	[PublicizedFrom(EAccessModifier.Private)]
	public static string QueuePrimary = "Primary";

	// Token: 0x040011FF RID: 4607
	[PublicizedFrom(EAccessModifier.Private)]
	public static string QueueSecondary = "Secondary";

	// Token: 0x04001200 RID: 4608
	[PublicizedFrom(EAccessModifier.Private)]
	public static string QueueNone = "None";

	// Token: 0x04001201 RID: 4609
	public static string Queue;

	// Token: 0x04001202 RID: 4610
	public static string Processed;

	// Token: 0x04001203 RID: 4611
	public static ChunkQueue ChunksToLoad = new ChunkQueue();

	// Token: 0x04001204 RID: 4612
	public static ConcurrentHashSet<long> ChunksToProcess = new ConcurrentHashSet<long>();

	// Token: 0x04001205 RID: 4613
	public static ConcurrentQueue<long> nextChunks = new ConcurrentQueue<long>();

	// Token: 0x04001206 RID: 4614
	public static bool QueueUpdateOverride;

	// Token: 0x04001207 RID: 4615
	public static bool RequestThreadStop = false;

	// Token: 0x04001208 RID: 4616
	public static bool AddRegionChecks = false;

	// Token: 0x04001209 RID: 4617
	public static int CachePurgeInterval = 8;

	// Token: 0x0400120A RID: 4618
	public static List<List<Vector3i>> RegionsToCheck = null;

	// Token: 0x0400120B RID: 4619
	public static DynamicMeshRegionDataStorage RegionStorage = new DynamicMeshRegionDataStorage();

	// Token: 0x0400120C RID: 4620
	public static DynamicMeshChunkDataStorage<DynamicMeshItem> ChunkDataQueue = new DynamicMeshChunkDataStorage<DynamicMeshItem>(DynamicMeshThread.CachePurgeInterval);

	// Token: 0x0400120D RID: 4621
	public static ConcurrentQueue<DynamicMeshItem> ChunkMeshGenRequests = new ConcurrentQueue<DynamicMeshItem>();

	// Token: 0x0400120E RID: 4622
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConcurrentQueue<DynamicMeshItem> TempChunkMeshGenRequests = new ConcurrentQueue<DynamicMeshItem>();

	// Token: 0x0400120F RID: 4623
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConcurrentQueue<DyMeshRegionLoadRequest> RegionFileLoadRequests = new ConcurrentQueue<DyMeshRegionLoadRequest>();

	// Token: 0x04001210 RID: 4624
	public static DynamicMeshBuilderManager BuilderManager = DynamicMeshBuilderManager.GetOrCreate();

	// Token: 0x04001211 RID: 4625
	public static string RegionUpdatesDebug = "";

	// Token: 0x04001212 RID: 4626
	public static ConcurrentDictionary<long, DynamicMeshUpdateData> RegionUpdates = new ConcurrentDictionary<long, DynamicMeshUpdateData>();

	// Token: 0x04001213 RID: 4627
	public static ConcurrentQueue<DynamicMeshData> ReadyForCollection = new ConcurrentQueue<DynamicMeshData>();

	// Token: 0x04001214 RID: 4628
	public static ConcurrentHashSet<Vector2i> ChunkReadyForCollection = new ConcurrentHashSet<Vector2i>();

	// Token: 0x04001215 RID: 4629
	public static float PlayerPositionX;

	// Token: 0x04001216 RID: 4630
	public static float PlayerPositionZ;

	// Token: 0x04001217 RID: 4631
	public static Queue<DynamicMeshItem> ToGenerate = new Queue<DynamicMeshItem>();

	// Token: 0x04001218 RID: 4632
	[PublicizedFrom(EAccessModifier.Private)]
	public static LinkedList<DynamicMeshItem> NeedObservers = new LinkedList<DynamicMeshItem>();

	// Token: 0x04001219 RID: 4633
	[PublicizedFrom(EAccessModifier.Private)]
	public static LinkedList<DynamicMeshItem> IgnoredChunks = new LinkedList<DynamicMeshItem>();

	// Token: 0x0400121A RID: 4634
	public static ConcurrentDictionary<long, DynamicMeshItem> PrimaryQueue = new ConcurrentDictionary<long, DynamicMeshItem>();

	// Token: 0x0400121B RID: 4635
	public static ConcurrentDictionary<long, DynamicMeshItem> SecondaryQueue = new ConcurrentDictionary<long, DynamicMeshItem>();

	// Token: 0x0400121C RID: 4636
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<ChunkGameObject> LoadedGos = new List<ChunkGameObject>();

	// Token: 0x0400121D RID: 4637
	[PublicizedFrom(EAccessModifier.Private)]
	public static Queue<ChunkGameObject> NewlyLoadedGos = new Queue<ChunkGameObject>();

	// Token: 0x0400121E RID: 4638
	[PublicizedFrom(EAccessModifier.Private)]
	public static Queue<ChunkGameObject> ToRemoveGos = new Queue<ChunkGameObject>();

	// Token: 0x0400121F RID: 4639
	public static ConcurrentDictionary<long, DynamicMeshThread.ThreadRegion> threadRegions = new ConcurrentDictionary<long, DynamicMeshThread.ThreadRegion>();

	// Token: 0x04001220 RID: 4640
	public static Queue<DynamicMeshServerUpdates> ServerUpdates = new Queue<DynamicMeshServerUpdates>(20);

	// Token: 0x04001221 RID: 4641
	[PublicizedFrom(EAccessModifier.Private)]
	public static Thread MeshThread;

	// Token: 0x04001222 RID: 4642
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<long> Keys = new List<long>(50);

	// Token: 0x04001223 RID: 4643
	[PublicizedFrom(EAccessModifier.Private)]
	public static DateTime NextRun = DateTime.Now;

	// Token: 0x04001224 RID: 4644
	[PublicizedFrom(EAccessModifier.Private)]
	public static DateTime StartTime = DateTime.Now;

	// Token: 0x020003A8 RID: 936
	public class ThreadRegion
	{
		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06001C01 RID: 7169 RVA: 0x000A67EE File Offset: 0x000A49EE
		public int LoadedChunkCount
		{
			get
			{
				return this.LoadedChunks.Count;
			}
		}

		// Token: 0x06001C02 RID: 7170 RVA: 0x000A67FC File Offset: 0x000A49FC
		public void AddLoadedChunk(long key)
		{
			object obj = this.chunkListLock;
			lock (obj)
			{
				this.LoadedChunks.Add(key);
			}
		}

		// Token: 0x06001C03 RID: 7171 RVA: 0x000A6844 File Offset: 0x000A4A44
		public bool RemoveLoadedChunk(long key)
		{
			object obj = this.chunkListLock;
			bool result;
			lock (obj)
			{
				result = this.LoadedChunks.TryRemove(key);
			}
			return result;
		}

		// Token: 0x06001C04 RID: 7172 RVA: 0x000A688C File Offset: 0x000A4A8C
		public void CopyLoadedChunks(List<long> chunks)
		{
			chunks.Clear();
			object obj = this.chunkListLock;
			lock (obj)
			{
				chunks.AddRange(this.LoadedChunks);
			}
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x000A68D8 File Offset: 0x000A4AD8
		public ThreadRegion(long key)
		{
			this.Key = key;
			this.xIndex = WorldChunkCache.extractX(key);
			this.zIndex = WorldChunkCache.extractZ(key);
			this.X = this.xIndex * 16;
			this.Z = this.zIndex * 16;
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x000A6949 File Offset: 0x000A4B49
		public Vector3i ToWorldPosition()
		{
			return new Vector3i(this.X, 0, this.Z);
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x000A695D File Offset: 0x000A4B5D
		public string ToDebugLocation()
		{
			return string.Format("R:{0} {1}", this.X, this.Z);
		}

		// Token: 0x06001C08 RID: 7176 RVA: 0x000A6980 File Offset: 0x000A4B80
		public bool IsInItemLoad(float playerX, float playerZ)
		{
			return !(GameManager.Instance == null) && GameManager.Instance.World != null && !(GameManager.Instance.World.GetPrimaryPlayer() == null) && DynamicMeshUnity.IsInBuffer(playerX, playerZ, DynamicMeshRegion.ItemLoadIndex, this.xIndex, this.zIndex);
		}

		// Token: 0x04001225 RID: 4645
		public int X;

		// Token: 0x04001226 RID: 4646
		public int Z;

		// Token: 0x04001227 RID: 4647
		public long Key;

		// Token: 0x04001228 RID: 4648
		[PublicizedFrom(EAccessModifier.Private)]
		public ConcurrentHashSet<long> LoadedChunks = new ConcurrentHashSet<long>();

		// Token: 0x04001229 RID: 4649
		[PublicizedFrom(EAccessModifier.Private)]
		public int xIndex;

		// Token: 0x0400122A RID: 4650
		[PublicizedFrom(EAccessModifier.Private)]
		public int zIndex;

		// Token: 0x0400122B RID: 4651
		public bool IsRegerating;

		// Token: 0x0400122C RID: 4652
		public float UpdateTime = float.MaxValue;

		// Token: 0x0400122D RID: 4653
		[PublicizedFrom(EAccessModifier.Private)]
		public object chunkListLock = new object();
	}
}
