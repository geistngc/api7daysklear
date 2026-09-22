using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ConcurrentCollections;
using UniLinq;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200037D RID: 893
public class DynamicMeshManager : MonoBehaviour
{
	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06001A7B RID: 6779 RVA: 0x0009C197 File Offset: 0x0009A397
	public static Transform ParentTransform
	{
		get
		{
			if (!(DynamicMeshManager.Instance == null))
			{
				return DynamicMeshManager.Instance.transform;
			}
			return null;
		}
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x0009C1B2 File Offset: 0x0009A3B2
	public void ForceOrphanChecks()
	{
		base.StartCoroutine(this.DoubleOrphanCheck());
	}

	// Token: 0x06001A7D RID: 6781 RVA: 0x0009C1C1 File Offset: 0x0009A3C1
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator DoubleOrphanCheck()
	{
		yield return base.StartCoroutine(this.CheckForOrphans(false));
		yield return base.StartCoroutine(this.CheckForOrphans(false));
		yield break;
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x0009C1D0 File Offset: 0x0009A3D0
	public IEnumerator CheckForOrphans(bool debugLogOnly)
	{
		int returns = 0;
		DateTime t = DateTime.Now.AddMilliseconds(2.0);
		this.checkForOrphans.Clear();
		for (int i = 0; i < DynamicMeshManager.ParentTransform.childCount; i++)
		{
			Transform child = DynamicMeshManager.ParentTransform.GetChild(i);
			this.checkForOrphans.Add(child.gameObject);
		}
		foreach (DynamicMeshItem dynamicMeshItem in this.ItemsDictionary.Values)
		{
			if (!(dynamicMeshItem.ChunkObject == null))
			{
				this.checkForOrphans.Remove(dynamicMeshItem.ChunkObject);
				this.potentialOrphans.Remove(dynamicMeshItem.ChunkObject);
				if (t < DateTime.Now)
				{
					int num = returns;
					returns = num + 1;
					yield return null;
					t = DateTime.Now.AddMilliseconds(2.0);
				}
			}
		}
		IEnumerator<DynamicMeshItem> enumerator = null;
		foreach (DynamicMeshRegion dynamicMeshRegion in DynamicMeshRegion.Regions.Values)
		{
			if (!(dynamicMeshRegion.RegionObject == null))
			{
				this.checkForOrphans.Remove(dynamicMeshRegion.RegionObject);
				this.potentialOrphans.Remove(dynamicMeshRegion.RegionObject);
				if (t < DateTime.Now)
				{
					int num = returns;
					returns = num + 1;
					yield return null;
					t = DateTime.Now.AddMilliseconds(2.0);
				}
			}
		}
		IEnumerator<DynamicMeshRegion> enumerator2 = null;
		using (HashSet<GameObject>.Enumerator enumerator3 = this.checkForOrphans.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				GameObject gameObject = enumerator3.Current;
				if (this.potentialOrphans.Contains(gameObject))
				{
					Log.Warning("Found orphaned mesh in dymesh parent " + gameObject.name);
					if (!debugLogOnly)
					{
						UnityEngine.Object.Destroy(gameObject);
					}
					this.potentialOrphans.Remove(gameObject);
				}
				else
				{
					this.potentialOrphans.Add(gameObject);
				}
			}
			yield break;
		}
		yield break;
		yield break;
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x0009C1E6 File Offset: 0x0009A3E6
	public static void EnabledChanged(bool newvalue)
	{
		if (DynamicMeshManager.CONTENT_ENABLED == newvalue)
		{
			return;
		}
		if (GameManager.Instance == null)
		{
			return;
		}
		if (GameManager.Instance.World == null)
		{
			return;
		}
		DynamicMeshManager.OnWorldUnload();
		if (newvalue)
		{
			DynamicMeshManager.Init();
		}
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x0009C219 File Offset: 0x0009A419
	public void AddObjectForDestruction(GameObject go)
	{
		this.ToBeDestroyed.Enqueue(go);
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x0009C228 File Offset: 0x0009A428
	public static void AddRegionLoadMeshes(long key)
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		using (LinkedList<long>.Enumerator enumerator = DynamicMeshManager.Instance.RegionsAvailableToLoad.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == key)
				{
					return;
				}
			}
		}
		DynamicMeshManager.Instance.RegionsAvailableToLoad.AddLast(key);
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x0009C298 File Offset: 0x0009A498
	public void AddChunkLoadData(DynamicMeshVoxelLoad loadData)
	{
		object chunkMeshDataLock = this._chunkMeshDataLock;
		lock (chunkMeshDataLock)
		{
			DynamicMeshManager.Instance.ChunkMeshData.AddLast(loadData);
		}
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x0009C2E4 File Offset: 0x0009A4E4
	[PublicizedFrom(EAccessModifier.Private)]
	static DynamicMeshManager()
	{
		GamePrefs.OnGamePrefChanged += DynamicMeshManager.OnGamePrefChanged;
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x0009C3D3 File Offset: 0x0009A5D3
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OnGamePrefChanged(EnumGamePrefs _pref)
	{
		if (_pref == EnumGamePrefs.DebugMenuEnabled)
		{
			DynamicMeshManager.EnabledChanged(GamePrefs.GetBool(_pref));
		}
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x0009C3E8 File Offset: 0x0009A5E8
	public static bool IsOutsideDistantTerrain(DynamicMeshRegion region)
	{
		return DynamicMeshManager.CONTENT_ENABLED && !(DynamicMeshManager.Instance == null) && (region.Rect.xMin < (float)DynamicMeshManager.dtMinX || region.Rect.xMax > (float)DynamicMeshManager.dtMaxX || region.Rect.yMin < (float)DynamicMeshManager.dtMinZ || region.Rect.yMax > (float)DynamicMeshManager.dtMaxZ);
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x0009C465 File Offset: 0x0009A665
	public static bool IsOutsideDistantTerrain(float minx, float maxx, float minz, float maxz)
	{
		return DynamicMeshManager.CONTENT_ENABLED && !(DynamicMeshManager.Instance == null) && (minx < (float)DynamicMeshManager.dtMinX || maxx > (float)DynamicMeshManager.dtMaxX || minz < (float)DynamicMeshManager.dtMinZ || maxz > (float)DynamicMeshManager.dtMaxZ);
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x0009C4A4 File Offset: 0x0009A6A4
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void UpdateDistantTerrainBounds(TileArea<UnityDistantTerrain.TerrainAndWater> data, UnityDistantTerrain.Config terrainConfig)
	{
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		int num = int.MaxValue;
		int num2 = int.MaxValue;
		int num3 = int.MinValue;
		int num4 = int.MinValue;
		if (data.Data.Count == 0)
		{
			DynamicMeshManager.dtMinX = int.MinValue;
			DynamicMeshManager.dtMinZ = int.MinValue;
			DynamicMeshManager.dtMaxX = -2147482624;
			DynamicMeshManager.dtMaxZ = -2147482624;
			return;
		}
		foreach (uint key in data.Data.Keys)
		{
			int tileXPos = TileAreaUtils.GetTileXPos(key);
			int tileZPos = TileAreaUtils.GetTileZPos(key);
			num = Math.Min(num, tileXPos);
			num2 = Math.Min(num2, tileZPos);
			num3 = Math.Max(num3, tileXPos);
			num4 = Math.Max(num4, tileZPos);
		}
		int dataTileSize = terrainConfig.DataTileSize;
		num *= dataTileSize;
		num2 *= dataTileSize;
		num3 *= dataTileSize;
		num4 *= dataTileSize;
		num3 += dataTileSize;
		num4 += dataTileSize;
		if (num != DynamicMeshManager.dtMinX || num3 != DynamicMeshManager.dtMaxX || DynamicMeshManager.dtMinZ != num2 || DynamicMeshManager.dtMaxZ != num4)
		{
			DynamicMeshManager.dtMinX = num;
			DynamicMeshManager.dtMaxX = num3;
			DynamicMeshManager.dtMinZ = num2;
			DynamicMeshManager.dtMaxZ = num4;
			DynamicMeshManager.Instance.ShowOrHidePrefabs();
			GameManager.Instance.prefabLODManager.TriggerUpdate();
		}
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x0009C5FC File Offset: 0x0009A7FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void DestroyObjects()
	{
		GameObject gameObject;
		while (this.ToBeDestroyed.TryDequeue(out gameObject))
		{
			if (!(gameObject == null))
			{
				DynamicMeshContainer currentlyLoadingItem = DynamicMeshFile.CurrentlyLoadingItem;
				if (((currentlyLoadingItem != null) ? currentlyLoadingItem.GetGameObject() : null) == gameObject)
				{
					Log.Warning("Object in use when destroying... delaying");
					this.ToBeDestroyed.Enqueue(gameObject);
					return;
				}
				gameObject.SetActive(false);
				DynamicMeshManager.MeshDestroy(gameObject);
			}
		}
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x000027FC File Offset: 0x000009FC
	public static void WarmUp()
	{
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x0009C660 File Offset: 0x0009A860
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SendClientMessage()
	{
		while (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			yield return new WaitForSeconds(1f);
		}
		this.ClientMessage = DynamicMeshServerStatus.ClientMessageSent;
		NetPackageDynamicClientArrive package = NetPackageManager.GetPackage<NetPackageDynamicClientArrive>();
		package.BuildData();
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
		DynamicMeshManager.LogMsg("Sending client arrive message. Items: " + package.Items.Count.ToString());
		yield break;
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x0009C66F File Offset: 0x0009A86F
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ProcessChunkRegionRequests()
	{
		this.ProcessRegionReady = false;
		this.ForceNextRegion = DateTime.Now.AddSeconds(60.0);
		DyMeshRegionLoadRequest load = null;
		if (this.RegionFileLoadRequests.Count != 0 && this.RegionFileLoadRequests.TryDequeue(out load))
		{
			if (this.PrefabCheck == PrefabCheckState.Waiting)
			{
				this.PrefabCheck = PrefabCheckState.Ready;
			}
			DynamicMeshRegion region = this.GetRegion(load.Key);
			if (DynamicMeshManager.DoLog)
			{
				Log.Out("Loading region go-process " + region.ToDebugLocation());
			}
			if (region != null)
			{
				if (region.OutsideLoadArea)
				{
					region.SetState(DynamicRegionState.Unloaded, false);
				}
				else
				{
					yield return base.StartCoroutine(load.CreateMeshCoroutine(region));
					if (region.RegionObject != null)
					{
						region.SetState(DynamicRegionState.Loaded, false);
						bool flag = !region.InBuffer && region.LoadedItems.Count > 0 && region.UnloadedItems.Count == 0 && region.RegionObject != null && region.RegionObject.activeSelf;
						region.LoadItems(false, !flag, false, "regionLoadRequest");
						this.UpdateDynamicPrefabDecoratorRegions(region);
					}
					region = null;
				}
			}
		}
		if (load != null)
		{
			MeshLists.ReturnList(load.OpaqueMesh);
			MeshLists.ReturnList(load.TerrainMesh);
		}
		this.AvailableRegionLoadRequests = Math.Min(DynamicMeshSettings.MaxRegionMeshData, this.AvailableRegionLoadRequests + 1);
		this.ProcessRegionReady = true;
		yield break;
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x0009C67E File Offset: 0x0009A87E
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ProcessItemMeshGeneration()
	{
		this.ProcessItemLoadReady = false;
		this.ForceNextItemLoad = DateTime.Now.AddSeconds(30.0);
		DynamicMeshVoxelLoad voxelData = null;
		if (this.ChunkMeshData.Count != 0)
		{
			bool flag = false;
			Monitor.TryEnter(this._chunkMeshDataLock, 1, ref flag);
			if (flag)
			{
				float num = 9999999f;
				foreach (DynamicMeshVoxelLoad dynamicMeshVoxelLoad in this.ChunkMeshData)
				{
					float num2 = dynamicMeshVoxelLoad.Item.DistanceToPlayer();
					if (num2 < num)
					{
						voxelData = dynamicMeshVoxelLoad;
						num = num2;
						if (num2 < 50f)
						{
							break;
						}
					}
				}
				if (voxelData != null)
				{
					this.ChunkMeshData.Remove(voxelData);
				}
				Monitor.Exit(this._chunkMeshDataLock);
				if (voxelData != null)
				{
					DynamicMeshItem item = this.GetItemOrNull(voxelData.Item.Key);
					if (item != null)
					{
						DynamicMeshRegion region = this.GetRegion(item);
						region.RemoveFromLoadingQueue(item);
						if (!region.IsInItemLoad())
						{
							item.State = DynamicItemState.Waiting;
						}
						else
						{
							item.State = DynamicItemState.Loading;
							if (DynamicMeshManager.DoLog)
							{
								Log.Out("Item " + item.ToDebugLocation() + " loading request");
							}
							DynamicMeshManager.ItemLoadDistance = item.DistanceToPlayer();
							yield return base.StartCoroutine(item.CreateMeshFromVoxelCoroutine(false, DynamicMeshManager.MeshLoadStop, voxelData));
							region.AddChunk(item.WorldPosition.x, item.WorldPosition.z);
							if (region.WorldPosition.x != DynamicMeshUnity.RoundRegion(item.WorldPosition.x))
							{
								string[] array = new string[6];
								array[0] = "Region mismatch: ";
								int num3 = 1;
								Vector3i worldPosition = region.WorldPosition;
								array[num3] = worldPosition.ToString();
								array[2] = " vs ";
								array[3] = (item.WorldPosition.x / 16 * 16).ToString();
								array[4] = " for  ";
								int num4 = 5;
								worldPosition = item.WorldPosition;
								array[num4] = worldPosition.ToString();
								Log.Error(string.Concat(array));
							}
							if (item.State != DynamicItemState.ReadyToDelete)
							{
								region.AddItemToLoadedList(item);
								bool flag2 = region.IsVisible();
								item.SetVisible(!flag2 && !item.IsChunkInView, "loadItemRequest");
								item.State = DynamicItemState.Loaded;
							}
						}
					}
				}
			}
		}
		this.ProcessItemLoadReady = true;
		if (voxelData != null)
		{
			voxelData.DisposeMeshes();
		}
		yield break;
	}

	// Token: 0x06001A8D RID: 6797 RVA: 0x0009C690 File Offset: 0x0009A890
	public void ForceLoadDataAroundPosition(Vector3i pos, int regionRadius)
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		DateTime now = DateTime.Now;
		DynamicMeshManager.CONTENT_ENABLED = false;
		foreach (DynamicMeshRegion dynamicMeshRegion in DynamicMeshRegion.Regions.Values)
		{
			if (dynamicMeshRegion != null)
			{
				dynamicMeshRegion.DistanceChecks();
			}
		}
		DynamicMeshManager.CONTENT_ENABLED = true;
		DyMeshRegionLoadRequest dyMeshRegionLoadRequest = DyMeshRegionLoadRequest.Create(0L);
		foreach (long key in this.RegionsAvailableToLoad)
		{
			dyMeshRegionLoadRequest.Key = key;
			DynamicMeshRegion region = this.GetRegion(dyMeshRegionLoadRequest.Key);
			if (region != null)
			{
				if (region.OutsideLoadArea || region.DistanceToPlayer() > 1000f)
				{
					region.SetState(DynamicRegionState.Unloaded, false);
				}
				else
				{
					dyMeshRegionLoadRequest.OpaqueMesh.Reset();
					dyMeshRegionLoadRequest.TerrainMesh.Reset();
					DynamicMeshThread.RegionStorage.LoadRegion(dyMeshRegionLoadRequest);
					dyMeshRegionLoadRequest.CreateMeshSync(region);
					if (region.RegionObject != null)
					{
						region.SetState(DynamicRegionState.Loaded, false);
						bool flag = !region.InBuffer && region.LoadedItems.Count > 0 && region.UnloadedItems.Count == 0 && region.RegionObject != null && region.RegionObject.activeSelf;
						region.LoadItems(false, !flag, false, "regionLoadRequest");
						this.UpdateDynamicPrefabDecoratorRegions(region);
					}
				}
			}
		}
		MeshLists.ReturnList(dyMeshRegionLoadRequest.OpaqueMesh);
		MeshLists.ReturnList(dyMeshRegionLoadRequest.TerrainMesh);
		this.RegionsAvailableToLoad.Clear();
		foreach (ChunkGameObject chunkGameObject in GameManager.Instance.World.ChunkCache.DisplayedChunkGameObjects.Dict.Values)
		{
			if (!DynamicMeshManager.ChunkGameObjects.Contains(chunkGameObject.chunk.Key))
			{
				DynamicMeshThread.AddChunkGameObject(chunkGameObject.chunk);
			}
		}
		Log.Out("Force load took " + ((int)(DateTime.Now - now).TotalSeconds).ToString() + " seconds");
	}

	// Token: 0x06001A8E RID: 6798 RVA: 0x0009C920 File Offset: 0x0009AB20
	public void ClearPrefabs()
	{
		if (base.gameObject != null)
		{
			foreach (object obj in base.gameObject.transform)
			{
				UnityEngine.Object.Destroy(((Transform)obj).gameObject);
			}
		}
		if (this.ItemsDictionary != null)
		{
			this.ItemsDictionary.Clear();
			DynamicMeshRegion.Regions.Clear();
		}
	}

	// Token: 0x06001A8F RID: 6799 RVA: 0x0009C9AC File Offset: 0x0009ABAC
	public void Cleanup()
	{
		DynamicMeshThread.StopThreadRequest();
		this.ClearPrefabs();
		base.StopAllCoroutines();
		Vector2i vector2i;
		while (this.ChunksToRemove.TryDequeue(out vector2i))
		{
		}
		foreach (DynamicMeshRegion dynamicMeshRegion in DynamicMeshRegion.Regions.Values)
		{
			dynamicMeshRegion.CleanUp();
		}
		this.ItemsDictionary.Clear();
		this.UpdateData.Clear();
		this.ChunkMeshData.Clear();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			this.ToBeDestroyed.Enqueue(transform.gameObject);
		}
		this.DestroyObjects();
		this.disabledImposterChunkManager.Dispose();
	}

	// Token: 0x06001A90 RID: 6800 RVA: 0x0009CAA4 File Offset: 0x0009ACA4
	public bool HalEventChunkChanged(object chunkObject)
	{
		if (chunkObject == null || !(chunkObject is Chunk))
		{
			return false;
		}
		DynamicMeshManager.ChunkChanged(((Chunk)chunkObject).GetWorldPos(), -1, 1);
		return true;
	}

	// Token: 0x06001A91 RID: 6801 RVA: 0x0009CAC8 File Offset: 0x0009ACC8
	public static bool IsValidGameMode()
	{
		GameManager instance = GameManager.Instance;
		if (((instance != null) ? instance.World : null) == null)
		{
			Log.Out("GM or World is not initialised yet for dynamic mesh");
			return false;
		}
		EnumGameMode gameMode = (EnumGameMode)GameManager.Instance.World.GetGameMode();
		return !GameManager.Instance.IsEditMode() && !GameUtils.IsPlaytesting() && gameMode != EnumGameMode.Creative && gameMode != EnumGameMode.EditWorld;
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x0009CB24 File Offset: 0x0009AD24
	public void Awake()
	{
		DynamicMeshManager.LogMsg("Awake");
		DynamicMeshManager.CONTENT_ENABLED = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshEnabled);
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			Log.Out("Dynamic mesh disabled");
			return;
		}
		if (DynamicMeshManager.backgroundTexture == null)
		{
			DynamicMeshManager.backgroundTexture = Texture2D.blackTexture;
		}
		DynamicMeshFile.TerrainSharedMaterials.Clear();
		if (DynamicMeshManager.Instance != null)
		{
			DynamicMeshManager.Instance.StopAllCoroutines();
			this.ClearPrefabs();
			if (DynamicMeshManager.Instance != this)
			{
				UnityEngine.Object.Destroy(DynamicMeshManager.Instance);
			}
			DynamicMeshManager.Instance = null;
		}
		if (!DynamicMeshManager.IsValidGameMode())
		{
			Log.Out("Dynamic Mesh will not run in this game mode");
			return;
		}
		DynamicMeshManager.Instance = this;
		DynamicMeshFile.MeshLocation = (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? (GameIO.GetSaveGameDir() + "/DynamicMeshes/") : (GameIO.GetSaveGameLocalDir() + "/DynamicMeshes/"));
		DynamicMeshManager.LogMsg("Mesh location: " + DynamicMeshFile.MeshLocation);
		this.BufferRegionLoadRequests.Clear();
		this.AvailableRegionLoadRequests = DynamicMeshSettings.MaxRegionMeshData;
		DyMeshData.ActiveItems = 0;
		this.ChunkMeshData.Clear();
		this.ChunkMeshLoadRequests.Clear();
		ConnectionManager.OnClientDisconnected -= DynamicMeshServer.OnClientDisconnect;
		ConnectionManager.OnClientDisconnected += DynamicMeshServer.OnClientDisconnect;
		this.UpdateData.Clear();
		this.ItemsDictionary = new ConcurrentDictionary<long, DynamicMeshItem>();
		DynamicMeshThread.StopThreadForce();
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			DynamicMeshManager.LogMsg("Disabled");
			return;
		}
		DateTime now = DateTime.Now;
		this.LoadItemsDedicated();
		DynamicMeshManager.LogMsg("Loading all items took: " + (DateTime.Now - now).TotalSeconds.ToString() + " seconds.");
		this.disabledImposterChunkManager = new DynamicMeshManager.DisabledImposterChunkManager(this);
		if (!GameManager.IsDedicatedServer)
		{
			if (DynamicMeshManager.player != null)
			{
				DynamicMeshThread.PlayerPositionX = DynamicMeshManager.player.position.x;
				DynamicMeshThread.PlayerPositionZ = DynamicMeshManager.player.position.z;
				this.ForceLoadDataAroundPosition(new Vector3i(DynamicMeshManager.player.position), DynamicMeshSettings.MaxViewDistance / 2);
			}
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				if (DynamicMeshManager.DoLog)
				{
					DynamicMeshManager.LogMsg("Saying hello to server. Regions: " + DynamicMeshRegion.Regions.Values.Count.ToString());
				}
				this.ShowOrHidePrefabs();
			}
		}
		DynamicMeshThread.StartThread();
		this.PrefabCheck = (SdFile.Exists(DynamicMeshFile.MeshLocation + "!!ChunksChecked.info") ? PrefabCheckState.Run : PrefabCheckState.Waiting);
		if (this.PrefabCheck != PrefabCheckState.Run)
		{
			this.CheckPrefabs("thread Started", false);
		}
		MeshDescription.meshes[0].bTextureArray = true;
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x0009CDC0 File Offset: 0x0009AFC0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ShowErrorStackTraces(string _msg, string _trace, LogType _type)
	{
		if (_type == LogType.Error || _type == LogType.Exception)
		{
			Log.Out(string.Concat(new string[]
			{
				"Callback ",
				_type.ToString(),
				":",
				_msg,
				" | ",
				_trace
			}));
		}
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x0009CE14 File Offset: 0x0009B014
	public static void OnWorldUnload()
	{
		DynamicMeshThread.StopThreadRequest();
		DateTime t = DateTime.Now.AddSeconds(1.0);
		while (DynamicMeshThread.RequestThreadStop && DateTime.Now < t)
		{
			Thread.Sleep(100);
		}
		DynamicMeshThread.CleanUp();
		DynamicMeshManager.ChunkGameObjects.Clear();
		if (DynamicMeshManager.Instance != null)
		{
			DynamicMeshManager.Instance.Cleanup();
		}
		DynamicMeshFile.CleanUp();
		DynamicMeshFile.MeshLocation = null;
	}

	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06001A95 RID: 6805 RVA: 0x0009CE8B File Offset: 0x0009B08B
	public static bool IsServer
	{
		get
		{
			return SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer;
		}
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x0009CE98 File Offset: 0x0009B098
	public void CheckPrefabsInRegion(int x, int z)
	{
		int xMin = DynamicMeshUnity.RoundRegion(x);
		int zMin = DynamicMeshUnity.RoundRegion(z);
		int xMax = xMin + 160;
		int zMax = zMin + 160;
		DynamicMeshManager.LogMsg("Checking prefabs in region");
		this.PrefabCheck = PrefabCheckState.Warming;
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
		EntityPlayerLocal player = DynamicMeshManager.player;
		if (dynamicPrefabDecorator == null)
		{
			return;
		}
		if (player == null)
		{
			return;
		}
		Vector3 playerPos = new Vector3((float)x, 0f, (float)z);
		this.dynamicPrefabsBuffer.Clear();
		dynamicPrefabDecorator.GetAllPrefabs(this.dynamicPrefabsBuffer);
		List<PrefabInstance> list = (from d in this.dynamicPrefabsBuffer
		where d.boundingBoxPosition.x >= xMin && d.boundingBoxPosition.x + d.boundingBoxSize.x < xMax && d.boundingBoxPosition.z >= zMin && d.boundingBoxPosition.z + d.boundingBoxSize.z < zMax
		orderby Math.Abs(Vector3.Distance(playerPos, d.boundingBoxPosition.ToVector3()))
		select d).ToList<PrefabInstance>();
		DynamicMeshManager.LogMsg("Found prefabs: " + list.Count.ToString() + "   Already loaded: " + this.ItemsDictionary.Count.ToString());
		Dictionary<Vector3i, List<Vector3i>> dictionary = new Dictionary<Vector3i, List<Vector3i>>();
		foreach (PrefabInstance p2 in list)
		{
			DynamicMeshManager.Instance.CheckPrefab(p2, dictionary);
		}
		List<Vector3i> list2 = (from d in dictionary.Keys
		orderby DynamicMeshUnity.Distance(d, playerPos)
		select d).ToList<Vector3i>();
		DynamicMeshThread.RegionsToCheck = new List<List<Vector3i>>();
		int num = 0;
		foreach (Vector3i key in list2)
		{
			using (List<Vector3i>.Enumerator enumerator3 = dictionary[key].GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Vector3i p = enumerator3.Current;
					DynamicMeshRegion regionFromWorldPosition = DynamicMeshRegion.GetRegionFromWorldPosition(p.x, p.z);
					if (regionFromWorldPosition == null || !regionFromWorldPosition.LoadedChunks.Any((Vector3i d) => d.x == p.x && d.z == p.z))
					{
						num++;
						this.AddChunk(p, false);
					}
				}
			}
		}
		DynamicMeshManager.LogMsg("Keys: " + list2.Count.ToString() + " loaded: " + num.ToString());
		this.PrefabCheck = PrefabCheckState.WaitingForCompleteCheck;
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x0009D144 File Offset: 0x0009B344
	public void CheckPrefabs(string source, bool forceRegen = false)
	{
		if (!DynamicMeshManager.IsServer)
		{
			return;
		}
		if (!forceRegen && (DynamicMeshSettings.OnlyPlayerAreas || !DynamicMeshSettings.NewWorldFullRegen))
		{
			return;
		}
		DynamicMeshManager.LogMsg("Checking prefabs " + source);
		if (GameManager.Instance.World.ChunkCache == null)
		{
			DynamicMeshManager.LogMsg("ChunkCache null");
			this.PrefabCheck = PrefabCheckState.WaitingForCompleteCheck;
			return;
		}
		this.PrefabCheck = PrefabCheckState.Warming;
		DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.World.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
		EntityPlayerLocal player = DynamicMeshManager.player;
		if (dynamicPrefabDecorator == null)
		{
			DynamicMeshManager.LogMsg("No deco found");
			return;
		}
		Vector3 playerPos = DynamicMeshManager.IsServer ? Vector3.zero : DynamicMeshManager.player.GetPosition();
		this.dynamicPrefabsBuffer.Clear();
		dynamicPrefabDecorator.GetAllPrefabs(this.dynamicPrefabsBuffer);
		List<PrefabInstance> list = (from d in this.dynamicPrefabsBuffer
		orderby Math.Abs(Vector3.Distance(playerPos, d.boundingBoxPosition.ToVector3()))
		select d).ToList<PrefabInstance>();
		DynamicMeshManager.LogMsg("Prefabs: " + list.Count.ToString());
		DynamicMeshManager.LogMsg("Found prefabs: " + list.Count.ToString() + "   Already loaded: " + this.ItemsDictionary.Count.ToString());
		Dictionary<Vector3i, List<Vector3i>> dictionary = new Dictionary<Vector3i, List<Vector3i>>();
		foreach (PrefabInstance p2 in list)
		{
			DynamicMeshManager.Instance.CheckPrefab(p2, dictionary);
		}
		List<Vector3i> list2 = (from d in dictionary.Keys
		orderby DynamicMeshUnity.Distance(d, playerPos)
		select d).ToList<Vector3i>();
		DynamicMeshThread.RegionsToCheck = new List<List<Vector3i>>();
		int num = 0;
		foreach (Vector3i key in list2)
		{
			using (List<Vector3i>.Enumerator enumerator3 = dictionary[key].GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Vector3i p = enumerator3.Current;
					DynamicMeshRegion regionFromWorldPosition = DynamicMeshRegion.GetRegionFromWorldPosition(p.x, p.z);
					if (regionFromWorldPosition == null || !regionFromWorldPosition.LoadedChunks.Any((Vector3i d) => d.x == p.x && d.z == p.z))
					{
						num++;
						this.AddChunk(p, false);
					}
				}
			}
		}
		DynamicMeshManager.LogMsg("Keys: " + list2.Count.ToString() + " loaded: " + num.ToString());
		this.PrefabCheck = PrefabCheckState.WaitingForCompleteCheck;
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x0009D40C File Offset: 0x0009B60C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckPrefab(PrefabInstance p, Dictionary<Vector3i, List<Vector3i>> positions)
	{
		Vector3i boundingBoxPosition = p.boundingBoxPosition;
		Vector3i boundingBoxSize = p.boundingBoxSize;
		int itemPosition = DynamicMeshUnity.GetItemPosition(boundingBoxPosition.x);
		int num = DynamicMeshUnity.GetItemPosition(boundingBoxPosition.x + boundingBoxSize.x) + 16;
		int itemPosition2 = DynamicMeshUnity.GetItemPosition(boundingBoxPosition.z);
		int num2 = DynamicMeshUnity.GetItemPosition(boundingBoxPosition.z + boundingBoxSize.z) + 16;
		for (int i = itemPosition; i <= num; i += 16)
		{
			for (int j = itemPosition2; j <= num2; j += 16)
			{
				long itemKey = DynamicMeshUnity.GetItemKey(i, j);
				DynamicMeshItem dynamicMeshItem;
				this.ItemsDictionary.TryGetValue(itemKey, out dynamicMeshItem);
				if (dynamicMeshItem == null || !dynamicMeshItem.FileExists())
				{
					Vector3i regionPositionFromWorldPosition = DynamicMeshUnity.GetRegionPositionFromWorldPosition(i, j);
					if (!positions.ContainsKey(regionPositionFromWorldPosition))
					{
						positions.Add(regionPositionFromWorldPosition, new List<Vector3i>());
					}
					List<Vector3i> list = positions[regionPositionFromWorldPosition];
					bool flag = false;
					for (int k = 0; k < list.Count; k++)
					{
						if (list[k].x == i && list[k].z == j)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						Vector3i item = new Vector3i(i, 0, j);
						if (DynamicMeshManager.DoLog)
						{
							DynamicMeshManager.LogMsg("Adding chunk " + i.ToString() + "," + j.ToString());
						}
						list.Add(item);
					}
				}
			}
		}
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x0009D574 File Offset: 0x0009B774
	public void RefreshAll()
	{
		foreach (DynamicMeshItem item in this.ItemsDictionary.Values)
		{
			DynamicMeshThread.ToGenerate.Enqueue(item);
		}
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x0009D5CC File Offset: 0x0009B7CC
	public void AddChunkStub(Vector3i worldPos, DynamicMeshRegion region)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldPos.x), World.toChunkXZ(worldPos.z));
		this.AddChunk(key, false, false, region);
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x0009D600 File Offset: 0x0009B800
	public static void AddDataFromServer(int x, int z)
	{
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		Vector3i worldPos = new Vector3i(x, 0, z);
		DynamicMeshRegion region = DynamicMeshManager.Instance.GetRegion(x, z);
		bool flag = region.IsInItemLoad();
		DynamicMeshManager.Instance.AddChunkStub(worldPos, region);
		DynamicMeshItem itemFromWorldPosition = DynamicMeshManager.Instance.GetItemFromWorldPosition(x, z);
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(string.Concat(new string[]
			{
				"Add data from server ",
				x.ToString(),
				",",
				z.ToString(),
				" itemLoad: ",
				flag.ToString(),
				"  state: ",
				itemFromWorldPosition.State.ToString()
			}));
		}
		itemFromWorldPosition.State = DynamicItemState.UpdateRequired;
		if (flag)
		{
			itemFromWorldPosition.Load("data from server", true, region.InBuffer);
		}
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x0009D6D8 File Offset: 0x0009B8D8
	public DynamicMeshItem AddChunk(Vector3i worldPos, bool primary)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldPos.x), World.toChunkXZ(worldPos.z));
		return this.AddChunk(key, true, primary, null);
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x0009D70C File Offset: 0x0009B90C
	public DynamicMeshItem AddChunk(long key, bool addToThread, bool primary, DynamicMeshRegion region)
	{
		Vector3i vector3i = new Vector3i(WorldChunkCache.extractX(key) * 16, 0, WorldChunkCache.extractZ(key) * 16);
		DynamicMeshThread.AddRegionChunk(vector3i.x, vector3i.z, key);
		DynamicMeshItem dynamicMeshItem;
		if (!this.ItemsDictionary.TryGetValue(key, out dynamicMeshItem))
		{
			dynamicMeshItem = new DynamicMeshItem(vector3i);
			if (region == null)
			{
				region = this.GetRegion(dynamicMeshItem);
			}
			if (region == null)
			{
				return dynamicMeshItem;
			}
			region.AddItem(dynamicMeshItem);
			this.disabledImposterChunksDirty |= this.ItemsDictionary.TryAdd(key, dynamicMeshItem);
		}
		DynamicMeshUnity.AddDisabledImposterChunk(dynamicMeshItem.Key);
		if (addToThread)
		{
			if (primary)
			{
				DynamicMeshThread.RequestPrimaryQueue(dynamicMeshItem);
			}
			else
			{
				DynamicMeshThread.RequestSecondaryQueue(dynamicMeshItem);
			}
		}
		return dynamicMeshItem;
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x0009D7B1 File Offset: 0x0009B9B1
	public static void LogMsg(string msg)
	{
		Log.Out("Dymesh: {0}", new object[]
		{
			msg
		});
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x0009D7C8 File Offset: 0x0009B9C8
	public static void MeshDestroy(GameObject go)
	{
		MeshFilter component = go.GetComponent<MeshFilter>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component.sharedMesh);
			component.sharedMesh = null;
		}
		MeshRenderer component2 = go.GetComponent<MeshRenderer>();
		if (component2 != null)
		{
			component2.sharedMaterial = null;
			UnityEngine.Object.Destroy(component2);
		}
		foreach (object obj in go.transform)
		{
			DynamicMeshManager.MeshDestroy(((Transform)obj).gameObject);
		}
		UnityEngine.Object.Destroy(go);
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x0009D868 File Offset: 0x0009BA68
	public void UpdateDynamicPrefabDecoratorRegions(DynamicMeshRegion region)
	{
		this.UpdateDynamicPrefabDecoratorRegion(region);
		DynamicMeshRegion region2 = this.GetRegion(region.WorldPosition + new Vector3i(160, 0, -160));
		this.UpdateDynamicPrefabDecoratorRegion(region2);
		region2 = this.GetRegion(region.WorldPosition + new Vector3i(-160, 0, 160));
		this.UpdateDynamicPrefabDecoratorRegion(region2);
		region2 = this.GetRegion(region.WorldPosition + new Vector3i(160, 0, 160));
		this.UpdateDynamicPrefabDecoratorRegion(region2);
		region2 = this.GetRegion(region.WorldPosition + new Vector3i(-160, 0, -160));
		this.UpdateDynamicPrefabDecoratorRegion(region2);
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x0009D920 File Offset: 0x0009BB20
	public void UpdateDynamicPrefabDecoratorRegion(DynamicMeshRegion region)
	{
		if (DynamicMeshManager.DisableLOD)
		{
			return;
		}
		if (region == null)
		{
			return;
		}
		if (region.RegionObject == null)
		{
			return;
		}
		if (region.LoadedItems.Any((DynamicMeshItem d) => d.State != DynamicItemState.Loaded && d.State != DynamicItemState.Empty && d.State != DynamicItemState.ReadyToDelete))
		{
			return;
		}
		DateTime now = DateTime.Now;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = null;
			if (GameManager.Instance == null)
			{
				DynamicMeshManager.LogMsg("GM Null");
			}
			else if (GameManager.Instance.World == null)
			{
				DynamicMeshManager.LogMsg("world Null");
			}
			else if (GameManager.Instance.World.ChunkCache == null)
			{
				DynamicMeshManager.LogMsg("ChunkCache Null");
			}
			else if (GameManager.Instance.World.ChunkCache.ChunkProvider == null)
			{
				DynamicMeshManager.LogMsg("Provider Null");
			}
			else
			{
				dynamicPrefabDecorator = GameManager.Instance.World.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator();
			}
			if (dynamicPrefabDecorator == null)
			{
				DynamicMeshManager.LogMsg("dec Null");
				return;
			}
			if (region.Instances == null)
			{
				region.Instances = new List<PrefabInstance>();
				this.dynamicPrefabsBuffer.Clear();
				dynamicPrefabDecorator.GetAllPrefabs(this.dynamicPrefabsBuffer);
				if (this.dynamicPrefabsBuffer.Count == 0)
				{
					DynamicMeshManager.LogMsg("prefabs Null or empty");
					return;
				}
				for (int i = this.dynamicPrefabsBuffer.Count - 1; i >= 0; i--)
				{
					PrefabInstance prefabInstance = this.dynamicPrefabsBuffer[i];
					if (region.ContainsPrefab(prefabInstance))
					{
						region.Instances.Add(prefabInstance);
					}
				}
			}
		}
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x0009DAA0 File Offset: 0x0009BCA0
	public bool PointInRectAndRegionExists(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, int x, int y)
	{
		bool flag = x >= topLeftX && x <= bottomRightX && y >= bottomRightY && y <= topLeftY;
		if (flag)
		{
			DynamicMeshRegion regionFromWorldPosition = DynamicMeshRegion.GetRegionFromWorldPosition(x, y);
			if (regionFromWorldPosition == null || regionFromWorldPosition.RegionObject == null)
			{
				return false;
			}
		}
		return flag;
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x0009DAEC File Offset: 0x0009BCEC
	public bool IsRegionLoadedAndActive(int x, int y, bool doDebug)
	{
		if (DynamicMeshManager.Instance.PrefabCheck == PrefabCheckState.Run)
		{
			return true;
		}
		DynamicMeshRegion regionFromWorldPosition = DynamicMeshRegion.GetRegionFromWorldPosition(x, y);
		if (regionFromWorldPosition != null)
		{
			return regionFromWorldPosition.IsRegionLoadedAndActive(doDebug);
		}
		if (doDebug)
		{
			Log.Out("LoadedAndActive Failed: region is null");
		}
		return false;
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x0009DB29 File Offset: 0x0009BD29
	public void ArrangeChunkRemoval(int x, int z)
	{
		this.ChunksToRemove.Enqueue(new Vector2i(x, z));
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x0009DB40 File Offset: 0x0009BD40
	public void DisableLodGO(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(go.GetComponent<MeshRenderer>());
		UnityEngine.Object.Destroy(go.GetComponent<MeshFilter>());
		foreach (object obj in go.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
	}

	// Token: 0x06001AA6 RID: 6822 RVA: 0x0009DBBC File Offset: 0x0009BDBC
	public bool StartObserver(Vector3 pos, Vector3 next)
	{
		this.ObserverRequestInfo = ObserverRequest.Start;
		this.ObserverPos = pos;
		this.ObserverPosNext = next;
		return true;
	}

	// Token: 0x06001AA7 RID: 6823 RVA: 0x0009DBD4 File Offset: 0x0009BDD4
	public void StopObserver()
	{
		if (this.Observer != null && this.Observer.Observer != null && this.ObserverRequestInfo != ObserverRequest.Stop)
		{
			this.ObserverRequestInfo = ObserverRequest.Stop;
		}
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x0009DBFC File Offset: 0x0009BDFC
	public void HideChunk(Vector3i worldPos)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldPos.x), World.toChunkXZ(worldPos.z));
		DynamicMeshItem dynamicMeshItem;
		if (this.ItemsDictionary.TryGetValue(key, out dynamicMeshItem))
		{
			dynamicMeshItem.SetVisible(false, "Hide chunk world pos");
		}
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x0009DC44 File Offset: 0x0009BE44
	public void HideChunk(long key)
	{
		DynamicMeshItem dynamicMeshItem;
		if (this.ItemsDictionary.TryGetValue(key, out dynamicMeshItem))
		{
			dynamicMeshItem.SetVisible(false, "Hide chunk world pos");
		}
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x0009DC70 File Offset: 0x0009BE70
	public void ShowChunk(long key)
	{
		DynamicMeshItem dynamicMeshItem;
		if (this.ItemsDictionary.TryGetValue(key, out dynamicMeshItem))
		{
			dynamicMeshItem.GetRegion().OnChunkUnloaded(dynamicMeshItem);
			if (dynamicMeshItem.ChunkObject != null)
			{
				dynamicMeshItem.SetVisible(true, "Show chunk force");
			}
		}
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x0009DCB4 File Offset: 0x0009BEB4
	public void LoadItemsDedicated()
	{
		DynamicMeshManager.LogMsg("Loading Items: " + DynamicMeshFile.MeshLocation);
		string[] files = SdDirectory.GetFiles(DynamicMeshFile.MeshLocation);
		SdFileInfo[] array = (from fi in new SdDirectoryInfo(DynamicMeshFile.MeshLocation).GetFiles("*.*")
		where fi.Length == 0L
		select fi).ToArray<SdFileInfo>();
		if (array.Length != 0)
		{
			DynamicMeshManager.LogMsg("Found " + array.Length.ToString() + " files @ zero size. Deleting...");
			SdFileInfo[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Delete();
			}
		}
		foreach (string text in files)
		{
			if (text.EndsWith(".update"))
			{
				long key = long.Parse(Path.GetFileNameWithoutExtension(text));
				DynamicMeshItem dynamicMeshItem = this.AddChunk(key, false, false, null);
				dynamicMeshItem.UpdateTime = dynamicMeshItem.ReadUpdateTimeFromFile();
			}
		}
		DynamicMeshManager.LogMsg("Loaded Items: " + this.ItemsDictionary.Count.ToString());
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			base.StartCoroutine(this.SendClientMessage());
		}
		if (files.Length == 0)
		{
			this.CheckPrefabs(" (load items dedicated)", false);
		}
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x0009DDF0 File Offset: 0x0009BFF0
	public void LoadItemsChunksDedicated()
	{
		DynamicMeshManager.LogMsg("Loading Items: " + DynamicMeshFile.MeshLocation);
		string[] files = SdDirectory.GetFiles(DynamicMeshFile.MeshLocation, "*.chunk");
		new Dictionary<string, Vector3>();
		string[] array = files;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = Path.GetFileNameWithoutExtension(array[i]).Split(',', StringSplitOptions.None);
			float x = float.Parse(array2[0]);
			float z = float.Parse(array2[1]);
			this.AddChunkStub(new Vector3i(x, 0f, z), null);
		}
		if (this.ItemsDictionary.Count == 0)
		{
			this.CheckPrefabs("LoadItemsChunksDedicated", false);
		}
		DynamicMeshManager.LogMsg("Loaded Items: " + this.ItemsDictionary.Count.ToString());
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x0009DEA4 File Offset: 0x0009C0A4
	public static int GetViewSize(EntityPlayer _player)
	{
		if (_player.ChunkObserver == null)
		{
			throw new Exception();
		}
		return DynamicMeshManager.player.ChunkObserver.viewDim * 16;
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x0009DEC6 File Offset: 0x0009C0C6
	public static int GetBufferSize(EntityPlayer _player)
	{
		return ((_player.ChunkObserver != null) ? DynamicMeshManager.player.ChunkObserver.viewDim : GamePrefs.GetInt(EnumGamePrefs.OptionsGfxViewDistance)) * 16 * 3;
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x0009DEEC File Offset: 0x0009C0EC
	public static bool RectContainsRect(Rect r1, Rect r2)
	{
		float xMin = r1.xMin;
		float xMax = r1.xMax;
		float yMin = r1.yMin;
		float yMax = r1.yMax;
		float xMin2 = r2.xMin;
		float xMax2 = r2.xMax;
		float yMin2 = r2.yMin;
		float yMax2 = r2.yMax;
		return xMax2 >= xMin && xMin2 <= xMax && yMax2 >= yMin && yMin2 <= yMax;
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x0009DF54 File Offset: 0x0009C154
	public static bool RectContainsPoint(Rect r1, int x, int z)
	{
		float xMin = r1.xMin;
		float xMax = r1.xMax;
		float yMin = r1.yMin;
		float yMax = r1.yMax;
		return (float)x >= xMin && (float)x <= xMax && (float)z >= yMin && (float)z <= yMax;
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x0009DF9C File Offset: 0x0009C19C
	public static void OriginUpdate()
	{
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		DynamicMeshManager.Instance.ShowOrHidePrefabs();
		DynamicMeshManager.Instance.SetItemPositions();
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x0009DFC0 File Offset: 0x0009C1C0
	public void SetItemPositions()
	{
		foreach (long key in this.ItemsDictionary.Keys.ToArray<long>())
		{
			this.ItemsDictionary[key].SetPosition();
		}
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x0009E004 File Offset: 0x0009C204
	public void ShowOrHidePrefabs()
	{
		if (DynamicMeshManager.player == null)
		{
			return;
		}
		if (DynamicMeshRegion.Regions == null)
		{
			return;
		}
		if (DynamicMeshManager.DebugReport)
		{
			DynamicMeshManager.LogMsg("Regions to process: " + DynamicMeshRegion.Regions.Count.ToString());
			string str = "Player Position: ";
			Vector3 position = DynamicMeshManager.player.position;
			DynamicMeshManager.LogMsg(str + position.ToString());
		}
		Vector3i vector3i = new Vector3i(DynamicMeshManager.player.position);
		IChunk chunkFromWorldPos = GameManager.Instance.World.GetChunkFromWorldPos(vector3i);
		if (chunkFromWorldPos == null)
		{
			return;
		}
		Vector3i worldPos = chunkFromWorldPos.GetWorldPos();
		int viewSize = DynamicMeshManager.GetViewSize(DynamicMeshManager.player);
		GameManager instance = GameManager.Instance;
		DictionarySave<long, ChunkGameObject> dictionarySave = (instance != null) ? instance.World.ChunkCache.DisplayedChunkGameObjects : null;
		if (dictionarySave == null)
		{
			return;
		}
		for (int i = worldPos.x - viewSize; i <= worldPos.x + viewSize; i += 16)
		{
			for (int j = worldPos.z - viewSize; j <= worldPos.z + viewSize; j += 16)
			{
				Vector3i vector3i2 = new Vector3i(i, 0, j);
				DynamicMeshItem itemOrNull = this.GetItemOrNull(vector3i2);
				if (itemOrNull != null && dictionarySave.ContainsKey(itemOrNull.Key))
				{
					itemOrNull.SetVisible(false, "showHide");
				}
				if (DynamicMeshManager.DebugReport)
				{
					string str2 = "World pos: ";
					Vector3i vector3i3 = vector3i2;
					DynamicMeshManager.LogMsg(str2 + vector3i3.ToString());
					DynamicMeshManager.LogMsg("Chunk Item: " + ((itemOrNull == null) ? "null" : itemOrNull.IsVisible.ToString()));
				}
			}
		}
		DynamicMeshItem itemOrNull2 = this.GetItemOrNull(vector3i);
		DynamicMeshRegion dynamicMeshRegion = (itemOrNull2 != null) ? itemOrNull2.GetRegion() : null;
		if (dynamicMeshRegion != null)
		{
			dynamicMeshRegion.SetVisibleNew(false, "showHide", true);
			foreach (DynamicMeshItem dynamicMeshItem in dynamicMeshRegion.LoadedItems)
			{
				dynamicMeshItem.SetVisible(!dynamicMeshItem.IsChunkInGame, "Region updateItems");
			}
		}
		foreach (DynamicMeshRegion dynamicMeshRegion2 in DynamicMeshRegion.Regions.Values)
		{
			if (dynamicMeshRegion2 != null)
			{
				dynamicMeshRegion2.DistanceChecks();
			}
		}
		DynamicMeshManager.DebugReport = false;
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x0009E270 File Offset: 0x0009C470
	public bool AddRegionChecks()
	{
		if (DynamicMeshThread.RegionsToCheck == null)
		{
			return false;
		}
		if (DynamicMeshThread.RegionsToCheck.Count == 0)
		{
			return false;
		}
		DynamicMeshThread.AddRegionChecks = false;
		List<Vector3i> list = DynamicMeshThread.RegionsToCheck[0];
		DynamicMeshThread.RegionsToCheck.RemoveAt(0);
		DynamicMeshManager.LogMsg(string.Concat(new string[]
		{
			"Adding new region to check: ",
			list.Count.ToString(),
			"  at ",
			Time.time.ToString(),
			"  remaining: ",
			DynamicMeshThread.RegionsToCheck.Count.ToString()
		}));
		foreach (Vector3i worldPos in list)
		{
			this.AddChunk(worldPos, false);
		}
		if (DynamicMeshThread.RegionsToCheck.Count == 0)
		{
			DynamicMeshThread.RegionsToCheck = null;
		}
		return true;
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x0009E368 File Offset: 0x0009C568
	public void OnGUI()
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		if (!DynamicMeshManager.ShowGui || GameManager.Instance == null || GameManager.Instance.World == null)
		{
			return;
		}
		if (DynamicMeshManager.player == null)
		{
			return;
		}
		Vector3i regionPositionFromWorldPosition = DynamicMeshUnity.GetRegionPositionFromWorldPosition(DynamicMeshManager.player.position);
		Vector3i vector3i = new Vector3i(World.toChunkXZ((int)DynamicMeshManager.player.position.x) * 16, 0, World.toChunkXZ((int)DynamicMeshManager.player.position.z) * 16);
		string text = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? "SP /p2p Host" : "Dedi / p2p";
		try
		{
			string[] array = new string[71];
			array[0] = "p: ";
			array[1] = DynamicMeshManager.player.position.ToString();
			array[2] = " - ";
			array[3] = DynamicMeshManager.player.transform.position.ToString();
			array[4] = "\nc: ";
			array[5] = vector3i.x.ToString();
			array[6] = ",";
			array[7] = vector3i.z.ToString();
			array[8] = "\nr:";
			array[9] = regionPositionFromWorldPosition.x.ToString();
			array[10] = ",";
			array[11] = regionPositionFromWorldPosition.z.ToString();
			array[12] = "\n Buff : ";
			array[13] = this.BufferRegionLoadRequests.Count.ToString();
			array[14] = "\n Items: ";
			array[15] = this.ChunkMeshData.Count.ToString();
			array[16] = "\n ItemGen: ";
			array[17] = DynamicMeshThread.MeshGenCount.ToString();
			array[18] = "\n Reg  : ";
			array[19] = this.RegionsAvailableToLoad.Count.ToString();
			array[20] = string.Format(" ({0})", this.AvailableRegionLoadRequests);
			array[21] = "\n Server : ";
			array[22] = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer.ToString();
			array[23] = " / ";
			array[24] = SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient.ToString();
			array[25] = "\n Thread P/S : ";
			array[26] = DynamicMeshThread.PrimaryQueue.Count.ToString();
			array[27] = " / ";
			array[28] = DynamicMeshThread.SecondaryQueue.Count.ToString();
			array[29] = "\n Game: ";
			array[30] = text;
			array[31] = "\n Packets: ";
			array[32] = NetPackageDynamicMesh.Count.ToString();
			array[33] = "\n ThreadDistance: ";
			array[34] = DynamicMeshManager.ThreadDistance.ToString();
			array[35] = "\n ObserverDistance: ";
			array[36] = DynamicMeshManager.ObserverDistance.ToString();
			array[37] = "\n ItemLoadDistance: ";
			array[38] = DynamicMeshManager.ItemLoadDistance.ToString();
			array[39] = "\n ItemCache: ";
			array[40] = DynamicMeshThread.ChunkDataQueue.ChunkData.Count.ToString();
			array[41] = " (";
			array[42] = DynamicMeshThread.ChunkDataQueue.LiveItems.ToString();
			array[43] = " live)\n WorldChunks: ";
			array[44] = GameManager.Instance.World.ChunkCache.chunks.list.Count.ToString();
			array[45] = "\n ThreadNext: ";
			array[46] = DynamicMeshThread.nextChunks.Count.ToString();
			array[47] = "\n ThreadQueue: ";
			array[48] = DynamicMeshThread.Queue;
			array[49] = "\n RegionUpdates: ";
			array[50] = DynamicMeshThread.RegionUpdates.Count.ToString();
			array[51] = "\n RegionUpdatesDebug: ";
			array[52] = DynamicMeshThread.RegionUpdatesDebug;
			array[53] = "\n SyncPackets: ";
			array[54] = DynamicMeshServer.SyncRequests.Count.ToString();
			array[55] = " (";
			array[56] = DynamicMeshServer.ActiveSyncs.Count.ToString();
			array[57] = ")\n DataSaveQueue: ";
			array[58] = DynamicMeshThread.ChunkDataQueue.ChunkData.Count.ToString();
			array[59] = "\n DataCache: ";
			array[60] = DynamicMeshChunkData.ActiveDataItems.ToString();
			array[61] = "/";
			array[62] = DynamicMeshChunkData.Cache.Count.ToString();
			array[63] = " (";
			array[64] = DynamicMeshChunkData.Cache.Sum((DynamicMeshChunkData d) => (double)d.GetStreamSize() / 1024.0 / 1024.0).ToString();
			array[65] = "MB)\n vMeshCache: ";
			array[66] = DynamicMeshVoxelLoad.LayerCache.Count.ToString();
			array[67] = "\n DymeshData: ";
			array[68] = DyMeshData.ActiveItems.ToString();
			array[69] = "/";
			array[70] = DyMeshData.TotalItems.ToString();
			string text2 = string.Concat(array);
			foreach (DynamicMeshChunkProcessor dynamicMeshChunkProcessor in DynamicMeshThread.BuilderManager.BuilderThreads.ToList<DynamicMeshChunkProcessor>())
			{
				if (dynamicMeshChunkProcessor != null)
				{
					string[] array2 = new string[9];
					array2[0] = text2;
					array2[1] = "\n Thread: ";
					int num = 2;
					DynamicMeshItem item = dynamicMeshChunkProcessor.Item;
					array2[num] = ((item != null) ? item.ToDebugLocation() : null);
					array2[3] = " Data ";
					array2[4] = ((int)dynamicMeshChunkProcessor.MeshDataTime).ToString();
					array2[5] = "ms Mesh: ";
					array2[6] = ((int)dynamicMeshChunkProcessor.ExportTime).ToString();
					array2[7] = " Inactive: ";
					array2[8] = ((int)dynamicMeshChunkProcessor.InactiveTime).ToString();
					text2 = string.Concat(array2);
				}
			}
			Color contentColor = GUI.contentColor;
			Color color = GUI.color;
			int depth = GUI.depth;
			GUI.depth = 0;
			GUI.DrawTexture(new Rect(0f, (float)Screen.height, 500f, 500f), Texture2D.blackTexture, ScaleMode.StretchToFill);
			GUI.color = DynamicMeshManager.DebugStyle.normal.textColor;
			GUI.contentColor = DynamicMeshManager.DebugStyle.normal.textColor;
			GUI.Label(new Rect(0f, (float)DynamicMeshManager.GuiY, 500f, 500f), text2, DynamicMeshManager.DebugStyle);
			GUI.contentColor = contentColor;
			GUI.color = color;
			GUI.depth = depth;
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x0009E9EC File Offset: 0x0009CBEC
	public DynamicMeshRegion GetNearestUnloadedRegion()
	{
		if (this.PrimaryLocation != null)
		{
			this.NearestRegionWithUnloaded = this.GetRegion(this.PrimaryLocation.Value);
		}
		if (this.NearestRegionWithUnloaded == null)
		{
			DynamicMeshRegion nearestRegionWithUnloaded = (from d in DynamicMeshRegion.Regions.Values
			where !d.FastTrackLoaded && d.UnloadedItems.Any<DynamicMeshItem>()
			orderby d.DistanceToPlayer()
			select d).ToList<DynamicMeshRegion>().FirstOrDefault<DynamicMeshRegion>();
			(from d in DynamicMeshRegion.Regions.Values
			where d.UnloadedItems.Any<DynamicMeshItem>()
			orderby d.DistanceToPlayer()
			select d).FirstOrDefault<DynamicMeshRegion>();
			this.NearestRegionWithUnloaded = nearestRegionWithUnloaded;
			if (this.NearestRegionWithUnloaded != null)
			{
				this.NearestRegionWithUnloaded.FastTrackLoaded = true;
			}
		}
		this.PrimaryLocation = null;
		this.FindNearestUnloadedItems = false;
		return this.NearestRegionWithUnloaded;
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x0009EB14 File Offset: 0x0009CD14
	[PublicizedFrom(EAccessModifier.Private)]
	public void MonitorGC()
	{
		for (int i = 0; i < 3; i++)
		{
			int num = GC.CollectionCount(i);
			if (num != this.gcGenCount[i])
			{
				this.gcGenCount[i] = num;
				Log.Out(string.Format("Gen {0} has fired: ", i) + num.ToString());
			}
		}
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x0009EB6C File Offset: 0x0009CD6C
	public void Update()
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		this.time = Time.time;
		if (this.FindNearestUnloadedItems)
		{
			this.GetNearestUnloadedRegion();
		}
		while (this.ChunksToRemove.Count > 0)
		{
			Vector2i vector2i;
			if (this.ChunksToRemove.TryDequeue(out vector2i))
			{
				if (DynamicMeshManager.DoLog)
				{
					DynamicMeshManager.LogMsg("Remove chunk from region " + vector2i.x.ToString() + "," + vector2i.y.ToString());
				}
				DynamicMeshItem itemFromWorldPosition = this.GetItemFromWorldPosition(vector2i.x, vector2i.y);
				this.RemoveItem(itemFromWorldPosition, true);
			}
		}
		if (this.ProcessItemLoadReady || this.ForceNextItemLoad < DateTime.Now)
		{
			if (this.ForceNextItemLoad < DateTime.Now)
			{
				Log.Warning("Forcing mesh processing after large delay");
			}
			base.StartCoroutine(this.ProcessItemMeshGeneration());
		}
		if (this.RegionFileLoadRequests.Count > 0)
		{
			base.StartCoroutine(this.ProcessChunkRegionRequests());
		}
		this.DestroyObjects();
		this.CheckFallingObservers();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			DynamicMeshServer.Update();
		}
		if (this.Observer.StopTime < Time.time)
		{
			this.Observer.Stop();
			this.ObserverPrep.Stop();
		}
		if (this.nextShowHide < Time.time)
		{
			this.ShowOrHidePrefabs();
			this.nextShowHide = Time.time + (float)(DynamicMeshManager.ShowHideCheckTime / 1000);
			this.CheckGameObjects();
		}
		if (this.ObserverRequestInfo != ObserverRequest.None)
		{
			if (this.ObserverRequestInfo == ObserverRequest.Start)
			{
				this.Observer.Start(this.ObserverPos);
			}
			else if (this.ObserverRequestInfo == ObserverRequest.Stop)
			{
				this.Observer.StopTime = Time.time + 3f;
			}
			this.ObserverRequestInfo = ObserverRequest.None;
		}
		while (DynamicMeshThread.ReadyForCollection.Count > 0)
		{
			DynamicMeshData dynamicMeshData;
			if (DynamicMeshThread.ReadyForCollection.TryDequeue(out dynamicMeshData))
			{
				DynamicMeshItem itemFromWorldPosition2 = this.GetItemFromWorldPosition(dynamicMeshData.X, dynamicMeshData.Z);
				DynamicMeshRegion region = itemFromWorldPosition2.GetRegion();
				if (itemFromWorldPosition2.ChunkObject != null || (region != null && region.IsInItemLoad()))
				{
					this.AddItemLoadRequest(itemFromWorldPosition2, false);
				}
			}
		}
		while (DynamicMeshThread.ChunkReadyForCollection.Count > 0)
		{
			Vector2i vector2i2;
			if (DynamicMeshThread.ChunkReadyForCollection.TryRemoveFirst(out vector2i2))
			{
				DynamicMeshItem itemFromWorldPosition3 = this.GetItemFromWorldPosition(vector2i2.x, vector2i2.y);
				DynamicMeshRegion region = itemFromWorldPosition3.GetRegion();
				if (itemFromWorldPosition3.ChunkObject != null || (region != null && region.IsInItemLoad()))
				{
					DynamicMeshThread.AddChunkGenerationRequest(itemFromWorldPosition3);
				}
			}
		}
		while (this.AvailableRegionLoadRequests > 0 && this.RegionsAvailableToLoad.Count > 0)
		{
			LinkedListNode<long> linkedListNode = null;
			float num = 99999f;
			for (LinkedListNode<long> linkedListNode2 = this.RegionsAvailableToLoad.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
			{
				long value = linkedListNode2.Value;
				DynamicMeshRegion region = this.GetRegion(value);
				if (region.OutsideLoadArea)
				{
					if (linkedListNode == linkedListNode2)
					{
						linkedListNode = null;
					}
					this.RegionsAvailableToLoad.Remove(linkedListNode2);
				}
				else
				{
					float num2 = region.DistanceToPlayer();
					if (num2 < num)
					{
						linkedListNode = linkedListNode2;
						num = num2;
						if (num < 100f)
						{
							break;
						}
					}
				}
			}
			if (linkedListNode == null)
			{
				break;
			}
			long value2 = linkedListNode.Value;
			this.RegionsAvailableToLoad.Remove(linkedListNode);
			DynamicMeshThread.AddRegionLoadRequest(DyMeshRegionLoadRequest.Create(value2));
			this.AvailableRegionLoadRequests--;
		}
		List<DynamicMeshUpdateData> list = new List<DynamicMeshUpdateData>();
		for (int i = 0; i < this.UpdateData.Count; i++)
		{
			DynamicMeshUpdateData dynamicMeshUpdateData = this.UpdateData[i];
			if (dynamicMeshUpdateData.UpdateTime < Time.time || dynamicMeshUpdateData.IsUrgent || dynamicMeshUpdateData.MaxTime < Time.time)
			{
				this.AddChunk(dynamicMeshUpdateData.Key, dynamicMeshUpdateData.AddToThread, true, null);
				list.Add(dynamicMeshUpdateData);
			}
		}
		foreach (DynamicMeshUpdateData item in list)
		{
			this.UpdateData.Remove(item);
		}
		if (this.nextUpdate > Time.time)
		{
			return;
		}
		if (DynamicMeshThread.RegionsToCheck != null && DynamicMeshThread.AddRegionChecks)
		{
			this.AddRegionChecks();
		}
		this.nextUpdate = Time.time + 1f;
		EntityPlayerLocal player = DynamicMeshManager.player;
		if (player != null)
		{
			DynamicMeshThread.PlayerPositionX = player.position.x;
			DynamicMeshThread.PlayerPositionZ = player.position.z;
		}
		if (DynamicMeshChunkProcessor.DebugOnMainThread)
		{
			DynamicMeshThread.BuilderManager.MainThreadRunJobs();
		}
		if (this.nextOrphanCheck < DateTime.Now)
		{
			this.nextOrphanCheck = DateTime.Now.AddSeconds(10.0);
			base.StartCoroutine(this.CheckForOrphans(true));
		}
		if (this.disabledImposterChunksDirty)
		{
			this.disabledImposterChunkManager.Update();
			this.disabledImposterChunksDirty = false;
		}
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x0009F044 File Offset: 0x0009D244
	public void AddItemLoadRequest(DynamicMeshItem item, bool urgent)
	{
		item.GetRegion().AddToLoadingQueue(item);
		DynamicMeshThread.AddChunkGenerationRequest(item);
	}

	// Token: 0x06001ABA RID: 6842 RVA: 0x0009F058 File Offset: 0x0009D258
	public bool IsInLoadableArea(long key)
	{
		return !DynamicMeshSettings.OnlyPlayerAreas || this.HandlePlayerOnlyAreas(key, DynamicMeshUnity.GetWorldPosFromKey(key));
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x0009F078 File Offset: 0x0009D278
	public DynamicMeshItem GetItemOrNull(Vector3i worldPos)
	{
		long itemKey = DynamicMeshUnity.GetItemKey(worldPos.x, worldPos.z);
		DynamicMeshItem result;
		if (this.ItemsDictionary.TryGetValue(itemKey, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x0009F0AC File Offset: 0x0009D2AC
	public DynamicMeshItem GetItemOrNull(long key)
	{
		DynamicMeshItem result;
		if (this.ItemsDictionary.TryGetValue(key, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x0009F0CC File Offset: 0x0009D2CC
	public DynamicMeshItem GetItemFromWorldPosition(int x, int z)
	{
		long itemKey = DynamicMeshUnity.GetItemKey(x, z);
		DynamicMeshItem result;
		if (this.ItemsDictionary.TryGetValue(itemKey, out result))
		{
			return result;
		}
		result = this.AddChunk(itemKey, false, false, null);
		return result;
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x0009F0FF File Offset: 0x0009D2FF
	public DynamicMeshRegion GetRegion(int x, int z)
	{
		return this.GetRegion(DynamicMeshUnity.GetRegionKeyFromWorldPosition(x, z));
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x0009F110 File Offset: 0x0009D310
	public DynamicMeshRegion GetRegion(BlockValueRef bvRef)
	{
		switch (bvRef.Type)
		{
		case BlockValueRefType.None:
			return null;
		case BlockValueRefType.Block:
			return this.GetRegion(bvRef.BlockPosition);
		case BlockValueRefType.Prop:
		{
			Vector2i vector2i = World.toChunkXZ(bvRef.PropReference.ChunkPos);
			return this.GetRegion(new Vector3i(vector2i.x, 0, vector2i.y));
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x0009F176 File Offset: 0x0009D376
	public DynamicMeshRegion GetRegion(Vector3i worldPos)
	{
		return this.GetRegion(DynamicMeshUnity.GetRegionKeyFromWorldPosition(worldPos.x, worldPos.z));
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x0009F18F File Offset: 0x0009D38F
	public DynamicMeshRegion GetRegion(DynamicMeshItem item)
	{
		return this.GetRegion(item.GetRegionKey());
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x0009F1A0 File Offset: 0x0009D3A0
	public DynamicMeshRegion GetRegion(long key)
	{
		DynamicMeshRegion dynamicMeshRegion;
		DynamicMeshRegion.Regions.TryGetValue(key, out dynamicMeshRegion);
		if (dynamicMeshRegion == null)
		{
			dynamicMeshRegion = new DynamicMeshRegion(key);
			if (!DynamicMeshRegion.Regions.TryAdd(key, dynamicMeshRegion))
			{
				DynamicMeshRegion.Regions.TryGetValue(key, out dynamicMeshRegion);
			}
		}
		return dynamicMeshRegion;
	}

	// Token: 0x06001AC3 RID: 6851 RVA: 0x0009F1E2 File Offset: 0x0009D3E2
	public static void Init()
	{
		DynamicMeshManager.CONTENT_ENABLED = GamePrefs.GetBool(EnumGamePrefs.DynamicMeshEnabled);
		DynamicMeshManager.DisabledImposterChunkManager.DisableShaderKeyword();
		GameManager.Instance.StopCoroutine(DynamicMeshManager.DelayStartForWorldLoad());
		GameManager.Instance.StartCoroutine(DynamicMeshManager.DelayStartForWorldLoad());
	}

	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001AC4 RID: 6852 RVA: 0x0009BFB0 File Offset: 0x0009A1B0
	public static EntityPlayerLocal player
	{
		get
		{
			if (GameManager.Instance.World != null)
			{
				return GameManager.Instance.World.GetPrimaryPlayer();
			}
			return null;
		}
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x0009F217 File Offset: 0x0009D417
	public static void EnableErrorCallstackLogs()
	{
		Log.LogCallbacks -= DynamicMeshManager.ShowErrorStackTraces;
		Log.LogCallbacks += DynamicMeshManager.ShowErrorStackTraces;
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x0009F23B File Offset: 0x0009D43B
	public static void DisableErrorCallstackLogs()
	{
		Log.LogCallbacks -= DynamicMeshManager.ShowErrorStackTraces;
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x0009F24E File Offset: 0x0009D44E
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator DelayStartForWorldLoad()
	{
		while (GameManager.Instance == null || GameManager.Instance.World == null)
		{
			Log.Out("Dynamic mesh waiting for world");
			yield return DynamicMeshFile.WaitOne;
		}
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			Log.Out("Dynamic mesh disabled on world start");
			yield break;
		}
		DynamicMeshManager.LogMsg("Prepping dynamic mesh. Resend Default: " + DynamicMeshServer.ResendPackages.ToString());
		DynamicMeshFile.MeshLocation = (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? (GameIO.GetSaveGameDir() + "/DynamicMeshes/") : (GameIO.GetSaveGameLocalDir() + "/DynamicMeshes/"));
		DynamicMeshManager.LogMsg("Mesh location: " + DynamicMeshFile.MeshLocation);
		if (!SdDirectory.Exists(DynamicMeshFile.MeshLocation))
		{
			SdDirectory.CreateDirectory(DynamicMeshFile.MeshLocation);
		}
		yield return new WaitForSeconds(1f);
		if (DynamicMeshManager.Parent != null)
		{
			UnityEngine.Object.Destroy(DynamicMeshManager.Parent);
			DynamicMeshManager.Parent = null;
		}
		DynamicMeshManager.DebugStyle.fontSize = ((Screen.width > 3000) ? 30 : 14);
		DynamicMeshManager.DebugStyle.normal.textColor = Color.magenta;
		if (DynamicMeshManager.Instance)
		{
			DynamicMeshManager.OnWorldUnload();
			DynamicMeshManager.Instance.ClearPrefabs();
		}
		DynamicMeshManager.LogMsg("Warming dynamic mesh");
		if (DynamicMeshManager.Instance == null || DynamicMeshManager.Parent == null)
		{
			DynamicMeshManager.LogMsg("Creating dynamic mesh manager");
			DynamicMeshManager.Parent = new GameObject("DynamicMeshes");
			DynamicMeshManager.Parent.AddComponent<DynamicMeshManager>();
		}
		DynamicMeshBlockSwap.Init();
		DynamicMeshSettings.Validate();
		if (!GameManager.IsDedicatedServer && Application.isEditor && SdDirectory.Exists("D:\\7DaysToDie\\trunkCode"))
		{
			DynamicMeshConsoleCmd.DebugAll();
		}
		yield break;
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x0009F258 File Offset: 0x0009D458
	public void ReorderGameObjects()
	{
		List<Transform> regions = new List<Transform>();
		List<Transform> list = new List<Transform>();
		foreach (object obj in DynamicMeshManager.Parent.transform)
		{
			Transform item = (Transform)obj;
			list.Add(item);
		}
		regions = (from d in list
		where !d.gameObject.name.StartsWith("C")
		orderby d.gameObject.name
		select d).ToList<Transform>();
		list.RemoveAll((Transform d) => regions.Contains(d));
		int num = 0;
		foreach (Transform transform in regions)
		{
			transform.SetSiblingIndex(num++);
			if (!(transform.gameObject.name == string.Empty))
			{
				string[] array = transform.gameObject.name.Replace("(sync)", "").Replace("R ", "").Split(',', StringSplitOptions.None);
				int num2 = int.Parse(array[0]);
				int num3 = int.Parse(array[1]);
				foreach (Transform transform2 in list)
				{
					string[] array2 = transform2.gameObject.name.Substring(2, transform2.gameObject.name.IndexOf(":") - 2).Split(',', StringSplitOptions.None);
					int num4 = int.Parse(array2[0]);
					int num5 = int.Parse(array2[1]);
					if (num4 >= num2 && num5 >= num3 && num4 < num2 + 160 && num5 < num3 + 160)
					{
						transform2.SetSiblingIndex(num++);
					}
				}
			}
		}
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x0009F4C0 File Offset: 0x0009D6C0
	public void RemoveItem(DynamicMeshItem item, bool removedFromWorld)
	{
		if (item == null)
		{
			return;
		}
		this.GetRegion(item).RemoveChunk(item.WorldPosition.x, item.WorldPosition.z, "removeItem", removedFromWorld);
		if (removedFromWorld && !GameManager.IsDedicatedServer)
		{
			DynamicMeshItem dynamicMeshItem;
			this.disabledImposterChunksDirty |= this.ItemsDictionary.TryRemove(item.Key, out dynamicMeshItem);
		}
		DynamicMeshThread.RemoveRegionChunk(item.WorldPosition.x, item.WorldPosition.z, item.Key);
		DynamicMeshUnity.RemoveDisabledImposterChunk(item.Key);
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("Item removed: " + item.ToDebugLocation());
		}
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x0009F56B File Offset: 0x0009D76B
	public static void ChunkChanged(Vector3i blockPos, int entityId, int blockType)
	{
		DynamicMeshManager.ChunkChanged(new BlockValueRef(blockPos), entityId, blockType);
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x0009F57C File Offset: 0x0009D77C
	public static void ChunkChanged(BlockValueRef bvRef, int entityId, int blockType)
	{
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		if (blockType != -1 && !DynamicMeshBlockSwap.IsValidBlock(blockType))
		{
			return;
		}
		if (ThreadManager.IsMainThread() && DynamicMeshManager.player != null && DynamicMeshManager.player.entityId == entityId)
		{
			DynamicMeshRegion region = DynamicMeshManager.Instance.GetRegion(bvRef);
			if (region.IsInItemLoad())
			{
				region.SetVisibleNew(false, DynamicMeshManager.ChunkChangedInItemLoad, true);
			}
		}
		Vector3i vector3i;
		if (!DynamicMeshManager.IsServer || !bvRef.TryGetBlockPos(out vector3i))
		{
			return;
		}
		DynamicMeshManager.Instance.AddUpdateData(vector3i, false, true);
		int num = vector3i.x & 15;
		if (num == 0)
		{
			DynamicMeshManager.Instance.AddUpdateData(vector3i + new Vector3i(-16, 0, 0), false, true);
		}
		else if (num == 15)
		{
			DynamicMeshManager.Instance.AddUpdateData(vector3i + new Vector3i(16, 0, 0), false, true);
		}
		if ((vector3i.z & 15) == 0)
		{
			DynamicMeshManager.Instance.AddUpdateData(vector3i + new Vector3i(0, 0, -16), false, true);
			return;
		}
		if (num == 15)
		{
			DynamicMeshManager.Instance.AddUpdateData(vector3i + new Vector3i(0, 0, 16), false, true);
		}
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x0009F69C File Offset: 0x0009D89C
	public bool AddUpdateData(Vector3i worldPos, bool isUrgent, bool addToThread)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldPos.x), World.toChunkXZ(worldPos.z));
		return this.AddUpdateData(key, isUrgent, addToThread, true, 1);
	}

	// Token: 0x06001ACD RID: 6861 RVA: 0x0009F6D0 File Offset: 0x0009D8D0
	public bool AddUpdateData(int worldX, int worldZ, bool isUrgent, bool addToThread)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldX), World.toChunkXZ(worldZ));
		return this.AddUpdateData(key, isUrgent, addToThread, true, 1);
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x0009F6FC File Offset: 0x0009D8FC
	public bool AddUpdateData(long key, bool isUrgent, bool addToThread, bool checkPlayerArea, int delayScale = 1)
	{
		if (DynamicMeshThread.ChunkDataQueue == null || DynamicMeshManager.Instance == null)
		{
			return false;
		}
		DynamicMeshUpdateData dynamicMeshUpdateData = null;
		Vector3i chunkPosition = new Vector3i(WorldChunkCache.extractX(key) * 16, 0, WorldChunkCache.extractZ(key) * 16);
		Vector3 position = chunkPosition.ToVector3();
		if (checkPlayerArea && !this.HandlePlayerOnlyAreas(key, position))
		{
			return false;
		}
		int i = 0;
		while (i < this.UpdateData.Count)
		{
			DynamicMeshUpdateData dynamicMeshUpdateData2 = this.UpdateData[i++];
			if (dynamicMeshUpdateData2.Key == key)
			{
				dynamicMeshUpdateData = dynamicMeshUpdateData2;
				break;
			}
		}
		if (dynamicMeshUpdateData == null)
		{
			dynamicMeshUpdateData = new DynamicMeshUpdateData();
			dynamicMeshUpdateData.ChunkPosition = chunkPosition;
			dynamicMeshUpdateData.Key = key;
			dynamicMeshUpdateData.MaxTime = this.time + DynamicMeshManager.MaxRebuildTime;
			dynamicMeshUpdateData.AddToThread = addToThread;
			this.UpdateData.Add(dynamicMeshUpdateData);
		}
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(string.Concat(new string[]
			{
				"Adding update ",
				key.ToString(),
				"  Time: ",
				dynamicMeshUpdateData.UpdateTime.ToString(),
				" pos: ",
				dynamicMeshUpdateData.ToDebugLocation()
			}));
		}
		dynamicMeshUpdateData.UpdateTime = this.time + (float)(this.QueueDelay * delayScale);
		dynamicMeshUpdateData.IsUrgent = (dynamicMeshUpdateData.IsUrgent || isUrgent);
		dynamicMeshUpdateData.AddToThread = (dynamicMeshUpdateData.AddToThread || addToThread);
		return true;
	}

	// Token: 0x06001ACF RID: 6863 RVA: 0x0009F846 File Offset: 0x0009DA46
	public bool HandlePlayerOnlyAreas(long key, Vector3 position)
	{
		return !DynamicMeshSettings.OnlyPlayerAreas || this.IsPositionInRange(position);
	}

	// Token: 0x06001AD0 RID: 6864 RVA: 0x0009F860 File Offset: 0x0009DA60
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsPositionInRange(Vector3 _position)
	{
		float x = _position.x;
		float z = _position.z;
		int num = Math.Max(16, (DynamicMeshSettings.PlayerAreaChunkBuffer - 1) / 2 * 16);
		DictionaryList<int, EntityPlayer> players = GameManager.Instance.World.Players;
		for (int i = 0; i < players.list.Count; i++)
		{
			for (int j = 0; j < players.list[i].SpawnPoints.Count; j++)
			{
				Vector3i vector3i = players.list[i].SpawnPoints[j];
				int num2 = vector3i.x - num;
				int num3 = vector3i.x + num + 16;
				int num4 = vector3i.z - num;
				int num5 = vector3i.z + num + 16;
				if (x >= (float)num2 && x < (float)num3 && z >= (float)num4 && z < (float)num5)
				{
					return true;
				}
			}
		}
		foreach (PersistentPlayerData persistentPlayerData in GameManager.Instance.persistentPlayers.m_lpBlockMap.Values)
		{
			for (int k = 0; k < persistentPlayerData.LPBlocks.Count; k++)
			{
				Vector3i vector3i2 = persistentPlayerData.LPBlocks[k];
				int num2 = DynamicMeshUnity.GetItemPosition(vector3i2.x) - num;
				int num3 = DynamicMeshUnity.GetItemPosition(vector3i2.x) + num + 16;
				int num4 = DynamicMeshUnity.GetItemPosition(vector3i2.z) - num;
				int num5 = DynamicMeshUnity.GetItemPosition(vector3i2.z) + num + 16;
				if (x >= (float)num2 && x < (float)num3 && z >= (float)num4 && z < (float)num5)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x0009FA30 File Offset: 0x0009DC30
	public static void ImportVox(string name, Vector3 pos, int blockId)
	{
		if (blockId == 0)
		{
			blockId = 502;
		}
		if (DynamicMeshManager.Instance == null || GameManager.Instance == null || GameManager.Instance.World == null)
		{
			return;
		}
		GameManager.bPhysicsActive = false;
		string text = DynamicMeshFile.MeshLocation + name + ".vox";
		if (!SdFile.Exists(text))
		{
			Log.Out("File " + text + " does not exist. Cancelling import");
			return;
		}
		byte[] buffer = Convert.FromBase64String(SdFile.ReadAllText(text));
		BlockValue blockValue = default(BlockValue);
		blockValue.type = blockId;
		BlockValue blockValue2 = default(BlockValue);
		blockValue2.type = 1;
		HashSet<Vector3i> hashSet = new HashSet<Vector3i>();
		int num;
		int num3;
		List<BlockChangeInfo> list;
		using (MemoryStream memoryStream = new MemoryStream(buffer))
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(memoryStream);
				num = (int)pooledBinaryReader.ReadByte();
				int num2 = (int)pooledBinaryReader.ReadByte();
				num3 = (int)pooledBinaryReader.ReadByte();
				list = new List<BlockChangeInfo>(num * num2 * num3 / 2);
				while (pooledBinaryReader.BaseStream.Position != pooledBinaryReader.BaseStream.Length)
				{
					byte b = pooledBinaryReader.ReadByte();
					byte b2 = pooledBinaryReader.ReadByte();
					byte b3 = pooledBinaryReader.ReadByte();
					int num4 = (int)(pos.x + (float)b);
					int y = (int)(pos.y + (float)b2);
					int num5 = (int)(pos.z + (float)b3);
					BlockChangeInfo blockChangeInfo = new BlockChangeInfo(new BlockValueRef(num4, y, num5), blockValue, 0);
					if (!hashSet.Contains(blockChangeInfo.blockValueRef.BlockPosition))
					{
						list.Add(blockChangeInfo);
						hashSet.Add(blockChangeInfo.blockValueRef.BlockPosition);
					}
					blockChangeInfo = new BlockChangeInfo(new BlockValueRef(num4 + 1, y, num5), blockValue2, 0);
					if (!hashSet.Contains(blockChangeInfo.blockValueRef.BlockPosition))
					{
						list.Add(blockChangeInfo);
					}
					blockChangeInfo = new BlockChangeInfo(new BlockValueRef(num4 - 1, y, num5), blockValue2, 0);
					if (!hashSet.Contains(blockChangeInfo.blockValueRef.BlockPosition))
					{
						list.Add(blockChangeInfo);
					}
					blockChangeInfo = new BlockChangeInfo(new BlockValueRef(num4, y, num5 + 1), blockValue2, 0);
					if (!hashSet.Contains(blockChangeInfo.blockValueRef.BlockPosition))
					{
						list.Add(blockChangeInfo);
					}
					blockChangeInfo = new BlockChangeInfo(new BlockValueRef(num4, y, num5 - 1), blockValue2, 0);
					if (!hashSet.Contains(blockChangeInfo.blockValueRef.BlockPosition))
					{
						list.Add(blockChangeInfo);
					}
				}
			}
		}
		Log.Out("Setting " + list.Count.ToString() + " blocks");
		GameManager.Instance.ChangeBlocks(null, list);
		Log.Out(name + " imported");
		GameManager.bPhysicsActive = true;
		int num6 = (num + 32) / 2;
		int num7 = (num3 + 32) / 2;
		int num8 = (int)pos.x - num6;
		while ((float)num8 < pos.x + (float)num6)
		{
			int num9 = (int)pos.z - num7;
			while ((float)num9 < pos.z + (float)num7)
			{
				DynamicMeshManager.Instance.AddChunk(new Vector3i(num8, 0, num9), true);
				num9 += 16;
			}
			num8 += 16;
		}
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x0009FD90 File Offset: 0x0009DF90
	public static void AddFallingBlockObserver(Vector3i pos)
	{
		if (DynamicMeshManager.Instance == null)
		{
			return;
		}
		if (!DynamicMeshManager.IsServer)
		{
			return;
		}
		using (List<DynamicObserver>.Enumerator enumerator = DynamicMeshManager.Instance.Observers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.ContainsPoint(pos))
				{
					return;
				}
			}
		}
		DynamicObserver dynamicObserver = new DynamicObserver();
		dynamicObserver.Start(pos.ToVector3());
		DynamicMeshManager.Instance.Observers.Add(dynamicObserver);
		DynamicMeshManager.Instance.NextFallingCheck = DateTime.Now.AddSeconds(5.0);
	}

	// Token: 0x06001AD3 RID: 6867 RVA: 0x0009FE44 File Offset: 0x0009E044
	public void CheckGameObjects()
	{
		int num = 0;
		foreach (object obj in DynamicMeshManager.ParentTransform)
		{
			GameObject gameObject = ((Transform)obj).gameObject;
			if (gameObject.name.StartsWith("C ") && !(gameObject.name == ""))
			{
				string[] array = gameObject.name.Substring(2, gameObject.name.IndexOf(" ", 3) - 3).Split(',', StringSplitOptions.None);
				int x = int.Parse(array[0]);
				int z = int.Parse(array[1]);
				DynamicMeshItem itemFromWorldPosition = this.GetItemFromWorldPosition(x, z);
				if (itemFromWorldPosition.ChunkObject == null)
				{
					this.AddObjectForDestruction(gameObject);
					num++;
				}
				if (!itemFromWorldPosition.GetRegion().IsInItemLoad())
				{
					num++;
					this.AddObjectForDestruction(gameObject);
				}
				if (itemFromWorldPosition.ChunkObject != gameObject)
				{
					num++;
					this.AddObjectForDestruction(gameObject);
				}
			}
		}
	}

	// Token: 0x06001AD4 RID: 6868 RVA: 0x0009FF5C File Offset: 0x0009E15C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckFallingObservers()
	{
		if (this.NextFallingCheck > DateTime.Now)
		{
			return;
		}
		this.NextFallingCheck = DateTime.Now.AddSeconds(5.0);
		for (int i = this.Observers.Count - 1; i >= 0; i--)
		{
			DynamicObserver dynamicObserver = this.Observers[i];
			if (!dynamicObserver.HasFallingBlocks())
			{
				dynamicObserver.Stop();
				this.Observers.RemoveAt(i);
			}
		}
	}

	// Token: 0x040010DD RID: 4317
	public int DebugOption;

	// Token: 0x040010DE RID: 4318
	public float DebugY = -0.7f;

	// Token: 0x040010DF RID: 4319
	public float TestZ = -1f;

	// Token: 0x040010E0 RID: 4320
	public static int DebugX = 1840;

	// Token: 0x040010E1 RID: 4321
	public static int DebugZ = 672;

	// Token: 0x040010E2 RID: 4322
	public static bool DebugItemPositions = false;

	// Token: 0x040010E3 RID: 4323
	public static bool DebugReleases = false;

	// Token: 0x040010E4 RID: 4324
	public int QueueDelay = 5;

	// Token: 0x040010E5 RID: 4325
	public static bool ForceMeshGeneration;

	// Token: 0x040010E6 RID: 4326
	public static string FileMissing = "FM";

	// Token: 0x040010E7 RID: 4327
	public static bool Allow32BitMeshes = true;

	// Token: 0x040010E8 RID: 4328
	public static GUIStyle DebugStyle = new GUIStyle();

	// Token: 0x040010E9 RID: 4329
	public static bool CONTENT_ENABLED = true;

	// Token: 0x040010EA RID: 4330
	public static bool CompressFiles = true;

	// Token: 0x040010EB RID: 4331
	public static bool DisableScopeTexture = true;

	// Token: 0x040010EC RID: 4332
	public static int GuiY = 0;

	// Token: 0x040010ED RID: 4333
	public static float MaxRebuildTime = 300f;

	// Token: 0x040010EE RID: 4334
	public PrefabCheckState PrefabCheck;

	// Token: 0x040010EF RID: 4335
	public static Rect ViewRect = default(Rect);

	// Token: 0x040010F0 RID: 4336
	public int QuadSize = 10000;

	// Token: 0x040010F1 RID: 4337
	public ConcurrentDictionary<long, DynamicMeshItem> ItemsDictionary;

	// Token: 0x040010F2 RID: 4338
	public static bool DisableLOD = false;

	// Token: 0x040010F3 RID: 4339
	public DynamicObserver Observer = new DynamicObserver();

	// Token: 0x040010F4 RID: 4340
	public DynamicObserver ObserverPrep = new DynamicObserver();

	// Token: 0x040010F5 RID: 4341
	public Vector3 ObserverPos;

	// Token: 0x040010F6 RID: 4342
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 ObserverPosNext;

	// Token: 0x040010F7 RID: 4343
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ObserverRequest ObserverRequestInfo;

	// Token: 0x040010F8 RID: 4344
	public static DynamicMeshManager Instance;

	// Token: 0x040010F9 RID: 4345
	public static bool DisableMoveVerts = false;

	// Token: 0x040010FA RID: 4346
	public static bool ShowDebug = true;

	// Token: 0x040010FB RID: 4347
	public Queue ToRemove = new Queue();

	// Token: 0x040010FC RID: 4348
	public static int CombineType = 1;

	// Token: 0x040010FD RID: 4349
	public static HashSet<long> ChunkGameObjects = new HashSet<long>();

	// Token: 0x040010FE RID: 4350
	public List<DynamicObserver> Observers = new List<DynamicObserver>();

	// Token: 0x040010FF RID: 4351
	public DynamicMeshServerStatus ClientMessage;

	// Token: 0x04001100 RID: 4352
	public ConcurrentHashSet<DynamicMeshVoxelLoad> BufferRegionLoadRequests = new ConcurrentHashSet<DynamicMeshVoxelLoad>();

	// Token: 0x04001101 RID: 4353
	public LinkedList<DynamicMeshItem> ChunkMeshLoadRequests = new LinkedList<DynamicMeshItem>();

	// Token: 0x04001102 RID: 4354
	public object _chunkMeshDataLock = new object();

	// Token: 0x04001103 RID: 4355
	public LinkedList<DynamicMeshVoxelLoad> ChunkMeshData = new LinkedList<DynamicMeshVoxelLoad>();

	// Token: 0x04001104 RID: 4356
	public LinkedList<long> RegionsAvailableToLoad = new LinkedList<long>();

	// Token: 0x04001105 RID: 4357
	public int AvailableRegionLoadRequests;

	// Token: 0x04001106 RID: 4358
	public ConcurrentQueue<DyMeshRegionLoadRequest> RegionFileLoadRequests = new ConcurrentQueue<DyMeshRegionLoadRequest>();

	// Token: 0x04001107 RID: 4359
	public static MicroStopwatch MeshLoadStop = new MicroStopwatch();

	// Token: 0x04001108 RID: 4360
	public int LongestRegionLoad;

	// Token: 0x04001109 RID: 4361
	public static bool DebugReport = false;

	// Token: 0x0400110A RID: 4362
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float nextUpdate;

	// Token: 0x0400110B RID: 4363
	public static bool ShowGui = false;

	// Token: 0x0400110C RID: 4364
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float nextShowHide;

	// Token: 0x0400110D RID: 4365
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ProcessItemLoadReady = true;

	// Token: 0x0400110E RID: 4366
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DateTime ForceNextItemLoad = DateTime.Now.AddDays(1.0);

	// Token: 0x0400110F RID: 4367
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool ProcessRegionReady = true;

	// Token: 0x04001110 RID: 4368
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DateTime ForceNextRegion = DateTime.Now.AddDays(1.0);

	// Token: 0x04001111 RID: 4369
	public static bool testMessage = true;

	// Token: 0x04001112 RID: 4370
	public static GameObject Parent;

	// Token: 0x04001113 RID: 4371
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LinkedListNode<DynamicMeshItem> CachedItem;

	// Token: 0x04001114 RID: 4372
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<PrefabInstance> dynamicPrefabsBuffer = new List<PrefabInstance>();

	// Token: 0x04001115 RID: 4373
	public float time;

	// Token: 0x04001116 RID: 4374
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Texture2D backgroundTexture = null;

	// Token: 0x04001117 RID: 4375
	public static float ThreadDistance;

	// Token: 0x04001118 RID: 4376
	public static float ObserverDistance;

	// Token: 0x04001119 RID: 4377
	public static float ItemLoadDistance;

	// Token: 0x0400111A RID: 4378
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DynamicMeshManager.DisabledImposterChunkManager disabledImposterChunkManager;

	// Token: 0x0400111B RID: 4379
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool disabledImposterChunksDirty;

	// Token: 0x0400111C RID: 4380
	public List<DynamicMeshUpdateData> UpdateData = new List<DynamicMeshUpdateData>(10);

	// Token: 0x0400111D RID: 4381
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly int PointerSize = 8;

	// Token: 0x0400111E RID: 4382
	public ConcurrentQueue<GameObject> ToBeDestroyed = new ConcurrentQueue<GameObject>();

	// Token: 0x0400111F RID: 4383
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DateTime nextOrphanCheck;

	// Token: 0x04001120 RID: 4384
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public HashSet<GameObject> potentialOrphans = new HashSet<GameObject>();

	// Token: 0x04001121 RID: 4385
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public HashSet<GameObject> checkForOrphans = new HashSet<GameObject>();

	// Token: 0x04001122 RID: 4386
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int dtMinX;

	// Token: 0x04001123 RID: 4387
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int dtMaxX;

	// Token: 0x04001124 RID: 4388
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int dtMinZ;

	// Token: 0x04001125 RID: 4389
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int dtMaxZ;

	// Token: 0x04001126 RID: 4390
	public static bool DoLog = false;

	// Token: 0x04001127 RID: 4391
	public static bool DoLogNet = false;

	// Token: 0x04001128 RID: 4392
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ConcurrentQueue<Vector2i> ChunksToRemove = new ConcurrentQueue<Vector2i>();

	// Token: 0x04001129 RID: 4393
	public DynamicMeshRegion NearestRegionWithUnloaded;

	// Token: 0x0400112A RID: 4394
	public bool FindNearestUnloadedItems;

	// Token: 0x0400112B RID: 4395
	public Vector3i? PrimaryLocation;

	// Token: 0x0400112C RID: 4396
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<long, HashSet<long>> RequestKeys = new Dictionary<long, HashSet<long>>();

	// Token: 0x0400112D RID: 4397
	public static int ShowHideCheckTime = 3000;

	// Token: 0x0400112E RID: 4398
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] gcGenCount = new int[3];

	// Token: 0x0400112F RID: 4399
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static string ChunkChangedInItemLoad = "ChunkChangedInItemLoad";

	// Token: 0x04001130 RID: 4400
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DateTime NextFallingCheck = DateTime.Now;

	// Token: 0x0200037E RID: 894
	[PublicizedFrom(EAccessModifier.Private)]
	public class DisabledImposterChunkManager : IDisposable
	{
		// Token: 0x06001AD6 RID: 6870 RVA: 0x000A0130 File Offset: 0x0009E330
		public DisabledImposterChunkManager(DynamicMeshManager dynamicMeshManager)
		{
			this.dynamicMeshManager = dynamicMeshManager;
			Vector2i worldSize = GameManager.Instance.World.ChunkCache.ChunkProvider.GetWorldSize();
			this.worldChunkDimensions = worldSize.x / 16;
			int num = this.worldChunkDimensions * this.worldChunkDimensions;
			this.chunkClipValues = new ComputeBuffer(num, 4);
			this.chunkClipValuesArray = new int[num];
			Shader.SetGlobalInteger(DynamicMeshManager.DisabledImposterChunkManager.chunkDimensionsID, this.worldChunkDimensions);
			Shader.EnableKeyword(DynamicMeshManager.DisabledImposterChunkManager.featureKeyword);
			this.Update();
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x000A01BC File Offset: 0x0009E3BC
		public void Update()
		{
			if (this.chunkClipValuesArray != null)
			{
				int num = this.worldChunkDimensions / 2;
				for (int i = 0; i < this.chunkClipValuesArray.Length; i++)
				{
					int x = i % this.worldChunkDimensions - num;
					int y = i / this.worldChunkDimensions - num;
					long key = WorldChunkCache.MakeChunkKey(x, y);
					if (this.dynamicMeshManager.ItemsDictionary.ContainsKey(key))
					{
						this.chunkClipValuesArray[i] = -1;
					}
					else
					{
						this.chunkClipValuesArray[i] = 1;
					}
				}
				this.chunkClipValues.SetData(this.chunkClipValuesArray);
				Shader.SetGlobalBuffer(DynamicMeshManager.DisabledImposterChunkManager.chunkBufferID, this.chunkClipValues);
			}
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x000A0254 File Offset: 0x0009E454
		public static void DisableShaderKeyword()
		{
			Shader.DisableKeyword(DynamicMeshManager.DisabledImposterChunkManager.featureKeyword);
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x000A0260 File Offset: 0x0009E460
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					DynamicMeshManager.DisabledImposterChunkManager.DisableShaderKeyword();
					this.chunkClipValues.Dispose();
				}
				this.chunkClipValuesArray = null;
				this.disposedValue = true;
			}
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x000A028B File Offset: 0x0009E48B
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x04001131 RID: 4401
		[PublicizedFrom(EAccessModifier.Private)]
		public const string ClippingOnKeyword = "PREFAB_CLIPPING_ON";

		// Token: 0x04001132 RID: 4402
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly int chunkBufferID = Shader.PropertyToID("_ChunkClipValues");

		// Token: 0x04001133 RID: 4403
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly int chunkDimensionsID = Shader.PropertyToID("_WorldChunkDimensions");

		// Token: 0x04001134 RID: 4404
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly GlobalKeyword featureKeyword = GlobalKeyword.Create("PREFAB_CLIPPING_ON");

		// Token: 0x04001135 RID: 4405
		[PublicizedFrom(EAccessModifier.Private)]
		public DynamicMeshManager dynamicMeshManager;

		// Token: 0x04001136 RID: 4406
		[PublicizedFrom(EAccessModifier.Private)]
		public ComputeBuffer chunkClipValues;

		// Token: 0x04001137 RID: 4407
		[PublicizedFrom(EAccessModifier.Private)]
		public int[] chunkClipValuesArray;

		// Token: 0x04001138 RID: 4408
		[PublicizedFrom(EAccessModifier.Private)]
		public int worldChunkDimensions;

		// Token: 0x04001139 RID: 4409
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disposedValue;
	}
}
