using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ConcurrentCollections;
using UniLinq;
using UnityEngine;

// Token: 0x02000395 RID: 917
public class DynamicMeshRegion : DynamicMeshContainer
{
	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06001B39 RID: 6969 RVA: 0x000A1845 File Offset: 0x0009FA45
	// (set) Token: 0x06001B3A RID: 6970 RVA: 0x000A184D File Offset: 0x0009FA4D
	public Rect Rect { get; set; }

	// Token: 0x1700033A RID: 826
	// (get) Token: 0x06001B3B RID: 6971 RVA: 0x000A1856 File Offset: 0x0009FA56
	// (set) Token: 0x06001B3C RID: 6972 RVA: 0x000A185E File Offset: 0x0009FA5E
	public bool RegenRequired { get; set; }

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06001B3D RID: 6973 RVA: 0x000A1867 File Offset: 0x0009FA67
	// (set) Token: 0x06001B3E RID: 6974 RVA: 0x000A186F File Offset: 0x0009FA6F
	public int xIndex { get; set; }

	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06001B3F RID: 6975 RVA: 0x000A1878 File Offset: 0x0009FA78
	// (set) Token: 0x06001B40 RID: 6976 RVA: 0x000A1880 File Offset: 0x0009FA80
	public int zIndex { get; set; }

	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06001B41 RID: 6977 RVA: 0x000A1889 File Offset: 0x0009FA89
	// (set) Token: 0x06001B42 RID: 6978 RVA: 0x000A1891 File Offset: 0x0009FA91
	public List<PrefabInstance> Instances { get; set; }

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06001B43 RID: 6979 RVA: 0x000A189A File Offset: 0x0009FA9A
	// (set) Token: 0x06001B44 RID: 6980 RVA: 0x000A18A4 File Offset: 0x0009FAA4
	public GameObject RegionObject
	{
		get
		{
			return this._regionObject;
		}
		set
		{
			if (this._regionObject != null)
			{
				if (DynamicMeshManager.DoLog)
				{
					DynamicMeshManager.LogMsg(string.Concat(new string[]
					{
						"Removing old region mesh: ",
						base.ToDebugLocation(),
						" buff: ",
						this.InBuffer.ToString(),
						"  oldPos ",
						this._regionObject.transform.position.ToString(),
						" vs ",
						this._regionObject.transform.position.ToString()
					}));
				}
				DynamicMeshManager.MeshDestroy(this._regionObject);
			}
			this._regionObject = value;
		}
	}

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06001B45 RID: 6981 RVA: 0x000A1967 File Offset: 0x0009FB67
	// (set) Token: 0x06001B46 RID: 6982 RVA: 0x000A196F File Offset: 0x0009FB6F
	public bool IsMeshLoaded { get; set; }

	// Token: 0x06001B47 RID: 6983 RVA: 0x000A1978 File Offset: 0x0009FB78
	public DynamicMeshRegion(long key)
	{
		Vector3i vector3i = new Vector3i(WorldChunkCache.extractX(key) * 16, 0, WorldChunkCache.extractZ(key) * 16);
		this.WorldPosition = vector3i;
		this.Key = key;
		this.Rect = new Rect((float)vector3i.x, (float)vector3i.z, 160f, 160f);
		this.xIndex = (int)((double)vector3i.x / 160.0);
		this.zIndex = (int)((double)vector3i.z / 160.0);
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x000A1A68 File Offset: 0x0009FC68
	public DynamicMeshRegion(Vector3i worldPos)
	{
		this.WorldPosition = DynamicMeshUnity.GetRegionPositionFromWorldPosition(worldPos);
		this.Key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(this.WorldPosition.x), World.toChunkXZ(this.WorldPosition.z));
		this.Rect = new Rect((float)this.WorldPosition.x, (float)this.WorldPosition.z, 160f, 160f);
		this.xIndex = (int)((double)worldPos.x / 160.0);
		this.zIndex = (int)((double)worldPos.z / 160.0);
	}

	// Token: 0x06001B49 RID: 6985 RVA: 0x000A1B6E File Offset: 0x0009FD6E
	public void AddToLoadingQueue(DynamicMeshItem item)
	{
		this.OnLoadingQueue.Add(item);
		if (this.OnLoadingQueue.Count == 0)
		{
			this.SetVisibleNew(false, "LoadingQueueEmpty", true);
		}
	}

	// Token: 0x06001B4A RID: 6986 RVA: 0x000A1B97 File Offset: 0x0009FD97
	public void RemoveFromLoadingQueue(DynamicMeshItem item)
	{
		this.OnLoadingQueue.Remove(item);
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x000A1BA6 File Offset: 0x0009FDA6
	public override GameObject GetGameObject()
	{
		return this.RegionObject;
	}

	// Token: 0x06001B4C RID: 6988 RVA: 0x000A1BB0 File Offset: 0x0009FDB0
	public bool IsInBuffer()
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		return !(primaryPlayer == null) && DynamicMeshUnity.IsInBuffer(primaryPlayer.position.x, primaryPlayer.position.z, DynamicMeshRegion.BufferIndexSize, this.xIndex, this.zIndex);
	}

	// Token: 0x06001B4D RID: 6989 RVA: 0x000A1C04 File Offset: 0x0009FE04
	public static bool IsInBuffer(int x, int z)
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		return !(primaryPlayer == null) && Math.Abs((int)(primaryPlayer.position.x / 160f) - x / 160) <= DynamicMeshRegion.BufferIndexSize && Math.Abs((int)(primaryPlayer.position.x / 160f) - z / 160) <= DynamicMeshRegion.BufferIndexSize;
	}

	// Token: 0x06001B4E RID: 6990 RVA: 0x000A1C80 File Offset: 0x0009FE80
	public bool IsInItemLoad()
	{
		if (GameManager.Instance == null)
		{
			return false;
		}
		if (GameManager.Instance.World == null)
		{
			return false;
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		return !(primaryPlayer == null) && DynamicMeshUnity.IsInBuffer(primaryPlayer.position.x, primaryPlayer.position.z, DynamicMeshRegion.ItemLoadIndex, this.xIndex, this.zIndex);
	}

	// Token: 0x06001B4F RID: 6991 RVA: 0x000A1CF1 File Offset: 0x0009FEF1
	public bool IsInItemLoad(float x, float z)
	{
		return DynamicMeshUnity.IsInBuffer(x, z, DynamicMeshRegion.ItemLoadIndex, this.xIndex, this.zIndex);
	}

	// Token: 0x06001B50 RID: 6992 RVA: 0x000A1D0C File Offset: 0x0009FF0C
	public bool IsInItemUnload()
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		return !(primaryPlayer == null) && !DynamicMeshUnity.IsInBuffer(primaryPlayer.position.x, primaryPlayer.position.z, DynamicMeshRegion.ItemUnloadIndex, this.xIndex, this.zIndex);
	}

	// Token: 0x06001B51 RID: 6993 RVA: 0x000A1D63 File Offset: 0x0009FF63
	public bool FileExists()
	{
		return SdFile.Exists(this.Path);
	}

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06001B52 RID: 6994 RVA: 0x000A1D70 File Offset: 0x0009FF70
	public string Path
	{
		get
		{
			return DynamicMeshFile.MeshLocation + this.Key.ToString() + ".group";
		}
	}

	// Token: 0x06001B53 RID: 6995 RVA: 0x000A1D8C File Offset: 0x0009FF8C
	public static DynamicMeshRegion GetRegionFromWorldPosition(Vector3i worldPos)
	{
		return DynamicMeshManager.Instance.GetRegion(worldPos);
	}

	// Token: 0x06001B54 RID: 6996 RVA: 0x000A1D99 File Offset: 0x0009FF99
	public static DynamicMeshRegion GetRegionFromWorldPosition(float worldX, float worldZ)
	{
		return DynamicMeshRegion.GetRegionFromWorldPosition((int)worldX, (int)worldZ);
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x000A1DA4 File Offset: 0x0009FFA4
	public static DynamicMeshRegion GetRegionFromWorldPosition(int worldX, int worldZ)
	{
		long regionKeyFromWorldPosition = DynamicMeshUnity.GetRegionKeyFromWorldPosition(worldX, worldZ);
		DynamicMeshRegion result;
		DynamicMeshRegion.Regions.TryGetValue(regionKeyFromWorldPosition, out result);
		return result;
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x000A1DC8 File Offset: 0x0009FFC8
	public bool AddItemToLoadedList(DynamicMeshItem item)
	{
		if (item == null)
		{
			return false;
		}
		for (int i = 0; i < this.UnloadedItems.Count; i++)
		{
			DynamicMeshItem dynamicMeshItem = this.UnloadedItems[i];
			long? num = (dynamicMeshItem != null) ? new long?(dynamicMeshItem.Key) : null;
			long key = item.Key;
			if (num.GetValueOrDefault() == key & num != null)
			{
				this.UnloadedItems.RemoveAt(i);
				this.LoadedItems.Add(item);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B57 RID: 6999 RVA: 0x000A1E4C File Offset: 0x000A004C
	public bool HideIfAllLoaded()
	{
		if (this.RegionObject == null)
		{
			return false;
		}
		if (this.UnloadedItems.Count > 0)
		{
			if (this.LoadedItems.Count <= 0 || this.UnloadedItems.Count != 1 || this.UnloadedItems[0].WorldPosition.x != this.WorldPosition.x || this.UnloadedItems[0].WorldPosition.z != this.WorldPosition.z)
			{
				return false;
			}
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Override for single item " + base.ToDebugLocation());
			}
		}
		else
		{
			if (!this.RegionObject.activeSelf)
			{
				return false;
			}
			if (this.LoadedItems.Count == 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001B58 RID: 7000 RVA: 0x000A1F1B File Offset: 0x000A011B
	public bool IsVisible()
	{
		return this.RegionObject != null && this.RegionObject.activeSelf;
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x000A1F38 File Offset: 0x000A0138
	public bool AddChunk(int x, int z)
	{
		if (this.HasChunk(x, z))
		{
			return false;
		}
		this.LoadedChunks.Add(new Vector3i(x, 0, z));
		return true;
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x000A1F5A File Offset: 0x000A015A
	public bool AddChunk(Vector3i chunk)
	{
		if (this.HasChunk(chunk.x, chunk.z))
		{
			return false;
		}
		this.LoadedChunks.Add(chunk);
		return true;
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x000A1F7F File Offset: 0x000A017F
	public bool AddThreadedChunk(int x, int z)
	{
		this.AddChunksThreaded.Enqueue(new Vector3i(x, 0, z));
		return true;
	}

	// Token: 0x06001B5C RID: 7004 RVA: 0x000A1F98 File Offset: 0x000A0198
	public bool HasChunk(int x, int z)
	{
		for (int i = 0; i < this.LoadedChunks.Count; i++)
		{
			if (this.LoadedChunks[i].x == x && this.LoadedChunks[i].z == z)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B5D RID: 7005 RVA: 0x000A1FE8 File Offset: 0x000A01E8
	public bool HasChunkAny(int x, int z)
	{
		for (int i = 0; i < this.LoadedItems.Count; i++)
		{
			Vector3i worldPosition = this.LoadedItems[i].WorldPosition;
			if (worldPosition.x == x && worldPosition.z == z)
			{
				return true;
			}
		}
		for (int j = 0; j < this.UnloadedItems.Count; j++)
		{
			Vector3i worldPosition2 = this.UnloadedItems[j].WorldPosition;
			if (worldPosition2.x == x && worldPosition2.z == z)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B5E RID: 7006 RVA: 0x000A2070 File Offset: 0x000A0270
	public bool IsRegionLoadedAndActive(bool doDebug)
	{
		if (DynamicMeshManager.Instance.PrefabCheck == PrefabCheckState.Run)
		{
			return true;
		}
		if (!(this.RegionObject == null) && this.LoadedItems.Count != 0)
		{
			if (!this.LoadedItems.Any((DynamicMeshItem d) => d.State != DynamicItemState.Loaded && d.State != DynamicItemState.ReadyToDelete && d.State != DynamicItemState.Empty))
			{
				return true;
			}
		}
		if (this.UnloadedItems.Count > 0)
		{
			this.LoadItems(true, !this.IsVisible(), true, "IsRegionLoadedAndActive");
			if (this.UnloadedItems.Count == 1)
			{
				DynamicMeshItem dynamicMeshItem = this.UnloadedItems[0];
				if (dynamicMeshItem.WorldPosition.x == this.WorldPosition.x && dynamicMeshItem.WorldPosition.z == this.WorldPosition.z)
				{
					return true;
				}
			}
		}
		if (doDebug)
		{
			if (this.RegionObject == null)
			{
				DynamicMeshRegion.LogMsg("LoadedAndActive Failed: regionObject null on " + base.ToDebugLocation());
			}
			if (this.UnloadedItems.Count > 0)
			{
				DynamicMeshRegion.LogMsg("LoadedAndActive Failed: unloaded items on " + base.ToDebugLocation());
			}
			if (this.LoadedItems.Count == 0)
			{
				DynamicMeshRegion.LogMsg("LoadedAndActive Failed: no loaded items on " + base.ToDebugLocation());
			}
			if (this.LoadedItems.Any((DynamicMeshItem d) => d.State != DynamicItemState.Loaded && d.State != DynamicItemState.Empty))
			{
				DynamicMeshRegion.LogMsg("LoadedAndActive Failed: loaded or empty");
			}
		}
		return false;
	}

	// Token: 0x06001B5F RID: 7007 RVA: 0x000A21F0 File Offset: 0x000A03F0
	public bool ContainsPrefab(PrefabInstance p)
	{
		return this.Intersects(p.boundingBoxPosition.x, p.boundingBoxPosition.z, p.boundingBoxPosition.x + p.boundingBoxSize.x, p.boundingBoxPosition.z + p.boundingBoxSize.z) || this.Intersects(p.boundingBoxPosition.x, p.boundingBoxPosition.z + p.boundingBoxSize.z, p.boundingBoxPosition.x + p.boundingBoxSize.x, p.boundingBoxPosition.z);
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x000A2298 File Offset: 0x000A0498
	public bool Intersects(int x1, int y1, int x2, int y2)
	{
		int num = Math.Min(x1, x2);
		int num2 = Math.Max(x1, x2);
		int num3 = Math.Min(y1, y2);
		int num4 = Math.Max(y1, y2);
		if (this.Rect.xMin > (float)num2 || this.Rect.xMax < (float)num)
		{
			return false;
		}
		if (this.Rect.yMin > (float)num4 || this.Rect.yMax < (float)num3)
		{
			return false;
		}
		if (this.Rect.xMin < (float)num && (float)num2 < this.Rect.xMax)
		{
			return true;
		}
		if (this.Rect.yMin < (float)num3 && (float)num4 < this.Rect.yMax)
		{
			return true;
		}
		Func<float, float> func = (float x) => (float)y1 - (x - (float)x1) * (float)((y1 - y2) / (x2 - x1));
		float num5 = func(this.Rect.xMin);
		float num6 = func(this.Rect.xMax);
		return (this.Rect.yMax >= num5 || this.Rect.yMax >= num6) && (this.Rect.yMin <= num5 || this.Rect.yMin <= num6);
	}

	// Token: 0x06001B61 RID: 7009 RVA: 0x000A243D File Offset: 0x000A063D
	public void OnChunkVisible(DynamicMeshItem item)
	{
		this.VisibleChunks += 1;
		this.ShowItems();
		this.HideRegion("onChunkVisible");
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x000A2460 File Offset: 0x000A0660
	public void OnChunkUnloaded(DynamicMeshItem item)
	{
		if (this.VisibleChunks > 0)
		{
			this.VisibleChunks -= 1;
		}
		if (this.VisibleChunks == 0)
		{
			bool active = true;
			string str = "All chunks unloaded on ";
			Vector3i worldPosition = this.WorldPosition;
			this.SetVisibleNew(active, str + worldPosition.ToString(), true);
			this.HideItems();
		}
	}

	// Token: 0x06001B63 RID: 7011 RVA: 0x000A24BC File Offset: 0x000A06BC
	public void SetVisibleNew(bool active, string reason, bool updateItems = true)
	{
		if (this.RegionObject != null && active != this.RegionObject.activeSelf)
		{
			if (active && this.IsPlayerInRegion())
			{
				return;
			}
			if (DynamicMeshManager.DoLog)
			{
				Log.Out(string.Concat(new string[]
				{
					"Changing view state for ",
					base.ToDebugLocation(),
					" to visible: ",
					active.ToString(),
					"      Reason: ",
					reason
				}));
			}
			this.RegionObject.SetActive(active);
			if (DynamicMeshManager.DebugItemPositions)
			{
				this.RegionObject.name = base.ToDebugLocation() + ": " + reason;
			}
		}
	}

	// Token: 0x06001B64 RID: 7012 RVA: 0x000A256C File Offset: 0x000A076C
	public void HideItems()
	{
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			dynamicMeshItem.SetVisible(false, "Region hide");
		}
	}

	// Token: 0x06001B65 RID: 7013 RVA: 0x000A25C4 File Offset: 0x000A07C4
	public void ShowItems()
	{
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			dynamicMeshItem.SetVisible(!dynamicMeshItem.IsChunkInGame, "Region show");
		}
		if (this.OnLoadingQueue.Count == 0)
		{
			this.SetVisibleNew(false, "ShowItems HideRegion", true);
		}
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x000A263C File Offset: 0x000A083C
	public bool LoadItems(bool urgent, bool visible, bool includeUnloaded, string reason)
	{
		if (!this.IsInItemLoad())
		{
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Load items ignore as outside " + base.ToDebugLocation());
			}
			this.HideItems();
			return false;
		}
		bool flag = false;
		for (int i = 0; i < this.LoadedItems.Count; i++)
		{
			DynamicMeshItem dynamicMeshItem = this.LoadedItems[i];
			if (dynamicMeshItem != null)
			{
				flag = (dynamicMeshItem.LoadIfEmpty("region load '" + reason + "'", urgent, this.InBuffer) || flag);
			}
		}
		if (includeUnloaded)
		{
			for (int j = 0; j < this.UnloadedItems.Count; j++)
			{
				DynamicMeshItem dynamicMeshItem2 = this.UnloadedItems[j];
				if (dynamicMeshItem2 != null)
				{
					flag = (dynamicMeshItem2.LoadIfEmpty("region load unloaded", urgent, this.InBuffer) || flag);
				}
			}
		}
		return this.UnloadedItems.Count > 0 || flag;
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x000A2714 File Offset: 0x000A0914
	public void ShowDebug()
	{
		if (DynamicMeshManager.DoLog)
		{
			string str = "Region: ";
			Vector3i worldPosition = this.WorldPosition;
			DynamicMeshManager.LogMsg(str + worldPosition.ToString() + "  Object: " + ((this.RegionObject == null) ? "null" : this.RegionObject.activeSelf.ToString()));
		}
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("Chunks: " + this.LoadedChunks.Count.ToString());
		}
		foreach (Vector3i vector3i in this.LoadedChunks)
		{
			Log.Out(vector3i.ToString());
		}
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("Items: " + this.LoadedItems.Count.ToString() + " vs Unloaded: " + this.UnloadedItems.Count.ToString());
		}
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			if (DynamicMeshManager.DoLog)
			{
				string[] array = new string[5];
				int num = 0;
				Vector3i worldPosition = dynamicMeshItem.WorldPosition;
				array[num] = worldPosition.ToString();
				array[1] = "  Object: ";
				array[2] = ((dynamicMeshItem.ChunkObject == null) ? "null" : dynamicMeshItem.ChunkObject.activeSelf.ToString());
				array[3] = "  State: ";
				array[4] = dynamicMeshItem.State.ToString();
				DynamicMeshManager.LogMsg(string.Concat(array));
			}
		}
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("--unloaded--");
		}
		foreach (DynamicMeshItem dynamicMeshItem2 in this.UnloadedItems)
		{
			if (DynamicMeshManager.DoLog)
			{
				string[] array2 = new string[5];
				int num2 = 0;
				Vector3i worldPosition = dynamicMeshItem2.WorldPosition;
				array2[num2] = worldPosition.ToString();
				array2[1] = "  Object: ";
				array2[2] = ((dynamicMeshItem2.ChunkObject == null) ? "null" : dynamicMeshItem2.ChunkObject.activeSelf.ToString());
				array2[3] = "  State: ";
				array2[4] = dynamicMeshItem2.State.ToString();
				DynamicMeshManager.LogMsg(string.Concat(array2));
			}
		}
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x000A29D0 File Offset: 0x000A0BD0
	public void OnCorrupted()
	{
		if (DynamicMeshManager.DoLog)
		{
			string str = "Corrupted region. Adding for regen ";
			Vector3i worldPosition = this.WorldPosition;
			DynamicMeshManager.LogMsg(str + worldPosition.ToString());
		}
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			DynamicMeshManager.Instance.AddChunk(dynamicMeshItem.WorldPosition, true);
		}
	}

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06001B69 RID: 7017 RVA: 0x000A2A58 File Offset: 0x000A0C58
	public int Triangles
	{
		get
		{
			int num = 0;
			if (this.RegionObject != null && this.RegionObject.GetComponent<MeshFilter>().mesh.isReadable)
			{
				num += this.RegionObject.GetComponent<MeshFilter>().mesh.triangles.Length;
				foreach (object obj in this.RegionObject.transform)
				{
					Transform transform = (Transform)obj;
					num += transform.gameObject.GetComponent<MeshFilter>().mesh.triangles.Length;
				}
			}
			return num;
		}
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06001B6A RID: 7018 RVA: 0x000A2B10 File Offset: 0x000A0D10
	public int Vertices
	{
		get
		{
			int num = 0;
			if (this.RegionObject != null && this.RegionObject.GetComponent<MeshFilter>().mesh.isReadable)
			{
				num += this.RegionObject.GetComponent<MeshFilter>().mesh.vertexCount;
				foreach (object obj in this.RegionObject.transform)
				{
					Transform transform = (Transform)obj;
					num += transform.gameObject.GetComponent<MeshFilter>().mesh.vertexCount;
				}
			}
			return num;
		}
	}

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06001B6B RID: 7019 RVA: 0x000A2BC4 File Offset: 0x000A0DC4
	public int RegionObjects
	{
		get
		{
			int result = 0;
			if (this.RegionObject != null)
			{
				result = this.RegionObject.transform.childCount + 1;
			}
			return result;
		}
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x000A2BF8 File Offset: 0x000A0DF8
	public void SetPosition()
	{
		if (this.RegionObject != null)
		{
			Vector3 vector = this.WorldPosition.ToVector3() - Origin.position;
			if (this.RegionObject.transform.position != vector)
			{
				this.RegionObject.transform.position = vector;
			}
		}
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x000A2C52 File Offset: 0x000A0E52
	public void HideRegion(string debugReason)
	{
		this.SetVisibleNew(false, debugReason, true);
		this.ShowItems();
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x000A2C64 File Offset: 0x000A0E64
	public void SetViewStats(bool inBuffer, bool shouldLoadItems, bool shouldUnloadItems, bool isOutsideMaxRegionArea)
	{
		this.OutsideLoadArea = isOutsideMaxRegionArea;
		if (shouldLoadItems)
		{
			this.LoadItems(false, true, true, "setViewStatsShouldLoadItem");
		}
		if (this.IsPlayerInRegion())
		{
			this.HideRegion("set view stats all items loaded");
		}
		if (!inBuffer && !isOutsideMaxRegionArea)
		{
			this.SetVisibleNew(true, "SetViewStats visible", true);
		}
		if (this.RegionObject == null && this.FileExists())
		{
			if (isOutsideMaxRegionArea)
			{
				this.SetState(DynamicRegionState.Unloaded, false);
			}
			else if (this.State != DynamicRegionState.StartLoad)
			{
				this.SetState(DynamicRegionState.StartLoad, false);
				DynamicMeshManager.AddRegionLoadMeshes(this.Key);
			}
		}
		if (inBuffer != this.InBuffer)
		{
			this.InBuffer = inBuffer;
			if (inBuffer)
			{
				if (!this.LoadItems(true, true, true, "setViewInBuffer"))
				{
					this.SetState(DynamicRegionState.Loaded, false);
				}
			}
			else
			{
				this.SetVisibleNew(true, "SetViewStats leftBuffer", true);
			}
		}
		if (shouldUnloadItems && this.LoadedItems.Count > 0)
		{
			this.ClearItems();
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06001B6F RID: 7023 RVA: 0x000A2D44 File Offset: 0x000A0F44
	public EntityPlayer GetPlayer
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (GameManager.Instance == null)
			{
				return null;
			}
			if (GameManager.Instance.World == null)
			{
				return null;
			}
			if (!GameManager.IsDedicatedServer)
			{
				return GameManager.Instance.World.GetPrimaryPlayer();
			}
			if (GameManager.Instance.World.Players.Count <= 0)
			{
				return null;
			}
			return GameManager.Instance.World.Players.list[0];
		}
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x000A2DB8 File Offset: 0x000A0FB8
	public float DistanceToPlayer()
	{
		EntityPlayer getPlayer = this.GetPlayer;
		if (getPlayer == null)
		{
			return 999999f;
		}
		Vector3 position = getPlayer.position;
		int num = 80;
		return Math.Abs(Mathf.Sqrt(Mathf.Pow(position.x - (float)(this.WorldPosition.x + num), 2f) + Mathf.Pow(position.z - (float)(this.WorldPosition.z + num), 2f)));
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x000A2E2E File Offset: 0x000A102E
	public void SetState(DynamicRegionState newState, bool forceChange)
	{
		if (!forceChange && newState == DynamicRegionState.Unloading && this.State == DynamicRegionState.Unloaded)
		{
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Can't change state from unloading to unloaded");
			}
			return;
		}
		this.State = newState;
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x000A2E58 File Offset: 0x000A1058
	public void RemoveChunk(int x, int z, string reason, bool removedFromWorld)
	{
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(string.Concat(new string[]
			{
				"Removing chunk ",
				x.ToString(),
				",",
				z.ToString(),
				": ",
				reason
			}));
		}
		DynamicMeshThread.ChunkDataQueue.MarkForDeletion(DynamicMeshUnity.GetRegionKeyFromWorldPosition(x, z));
		int i = 0;
		while (i < this.UnloadedItems.Count)
		{
			DynamicMeshItem dynamicMeshItem = this.UnloadedItems[i];
			if (dynamicMeshItem != null && dynamicMeshItem.WorldPosition.x == x && dynamicMeshItem.WorldPosition.z == z)
			{
				dynamicMeshItem.DestroyChunk();
				if (removedFromWorld)
				{
					this.UnloadedItems.RemoveAt(i);
					break;
				}
				dynamicMeshItem.State = DynamicItemState.ReadyToDelete;
				break;
			}
			else
			{
				i++;
			}
		}
		int j = 0;
		while (j < this.LoadedItems.Count)
		{
			DynamicMeshItem dynamicMeshItem2 = this.LoadedItems[j];
			if (dynamicMeshItem2 != null && dynamicMeshItem2.WorldPosition.x == x && dynamicMeshItem2.WorldPosition.z == z)
			{
				dynamicMeshItem2.DestroyChunk();
				if (removedFromWorld)
				{
					this.LoadedItems.RemoveAt(j);
					break;
				}
				dynamicMeshItem2.State = DynamicItemState.ReadyToDelete;
				break;
			}
			else
			{
				j++;
			}
		}
		this.LoadedChunks.Remove(new Vector3i(x, 0, z));
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000A2F9C File Offset: 0x000A119C
	public void AddItem(DynamicMeshItem item)
	{
		int num = 0;
		int num2 = 0;
		try
		{
			num = 1;
			num2 = ((item == null) ? 1 : 0);
			if (item == null)
			{
				Log.Error("null item tried to be added");
			}
			else
			{
				num = 2;
				bool flag = false;
				for (int i = 0; i < this.LoadedItems.Count; i++)
				{
					DynamicMeshItem dynamicMeshItem = this.LoadedItems[i];
					num = 3;
					if (dynamicMeshItem != null)
					{
						num = 4;
						num2 = ((item == null) ? 1 : 0) + ((dynamicMeshItem == null) ? 10 : 0);
						if (dynamicMeshItem.Key == item.Key)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					num = 5;
					for (int j = 0; j < this.UnloadedItems.Count; j++)
					{
						num = 6;
						DynamicMeshItem dynamicMeshItem2 = this.UnloadedItems[j];
						num2 = ((item == null) ? 1 : 0);
						if (dynamicMeshItem2 != null)
						{
							num = 7;
							num2 = ((item == null) ? 1 : 0) + ((dynamicMeshItem2 == null) ? 10 : 0);
							if (dynamicMeshItem2.Key == item.Key)
							{
								flag = true;
								break;
							}
						}
					}
				}
				if (!flag)
				{
					num = 8;
					if (GameManager.IsDedicatedServer)
					{
						num = 9;
						this.LoadedItems.Add(item);
					}
					else
					{
						num = 10;
						this.UnloadedItems.Add(item);
					}
				}
				num = 11;
			}
		}
		catch (Exception)
		{
			Log.Error("Add Item error at stage: " + num.ToString() + " nulls: " + num2.ToString());
		}
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x000A30F4 File Offset: 0x000A12F4
	public int GetStreamLength()
	{
		int val = 8 + this.LoadedChunks.Distinct<Vector3i>().Count<Vector3i>() * 8 + 8 + 12 + 1 + 4;
		return Math.Max(10240, val);
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x000A312A File Offset: 0x000A132A
	public void CleanUp()
	{
		if (this.RegionObject != null)
		{
			DynamicMeshManager.MeshDestroy(this.RegionObject);
			this.RegionObject = null;
		}
		this.ClearMeshes();
		this.State = DynamicRegionState.Unloaded;
		this.IsMeshLoaded = false;
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x000A3160 File Offset: 0x000A1360
	public bool IsPlayerInRegion()
	{
		EntityPlayerLocal player = DynamicMeshManager.player;
		return !(player == null) && DynamicMeshManager.Instance.GetRegion((int)player.position.x, (int)player.position.z).WorldPosition == this.WorldPosition;
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x000A31B0 File Offset: 0x000A13B0
	public void DistanceChecks()
	{
		float num = this.DistanceToPlayer();
		bool inBuffer = this.IsInBuffer();
		bool shouldLoadItems = this.IsInItemLoad();
		bool shouldUnloadItems = this.IsInItemUnload();
		bool flag = num >= (float)DynamicMeshSettings.MaxViewDistance || DynamicMeshManager.IsOutsideDistantTerrain(this);
		this.SetPosition();
		this.SetViewStats(inBuffer, shouldLoadItems, shouldUnloadItems, flag);
		if (this.State == DynamicRegionState.Unloaded && num < (float)DynamicMeshSettings.MaxViewDistance && this.RegionObject == null)
		{
			bool outsideLoadArea = this.OutsideLoadArea;
		}
		if (!(this.RegionObject == null) || this.State == DynamicRegionState.Unloaded)
		{
		}
		if (this.RegionObject != null && flag && DynamicMeshFile.CurrentlyLoadingRegionPosition != this.WorldPosition)
		{
			this.CleanUp();
		}
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x000A3267 File Offset: 0x000A1467
	public static void LogMsg(string msg)
	{
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(msg);
		}
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x000A3278 File Offset: 0x000A1478
	public void ClearItems()
	{
		foreach (DynamicMeshItem dynamicMeshItem in this.LoadedItems)
		{
			dynamicMeshItem.CleanUp();
			this.UnloadedItems.Add(dynamicMeshItem);
		}
		this.LoadedItems.Clear();
		this.SetVisibleNew(true, "itemsUnloaded", true);
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x000027FC File Offset: 0x000009FC
	public void ClearMeshes()
	{
	}

	// Token: 0x04001189 RID: 4489
	public static ConcurrentDictionary<long, DynamicMeshRegion> Regions = new ConcurrentDictionary<long, DynamicMeshRegion>();

	// Token: 0x0400118A RID: 4490
	public static int BufferIndexSize = 1;

	// Token: 0x0400118B RID: 4491
	public static int ItemLoadIndex = 3;

	// Token: 0x0400118C RID: 4492
	public static int ItemUnloadIndex = DynamicMeshRegion.ItemLoadIndex + 1;

	// Token: 0x04001191 RID: 4497
	public byte VisibleChunks;

	// Token: 0x04001193 RID: 4499
	public List<DynamicMeshItem> LoadedItems = new List<DynamicMeshItem>();

	// Token: 0x04001194 RID: 4500
	public List<DynamicMeshItem> UnloadedItems = new List<DynamicMeshItem>();

	// Token: 0x04001195 RID: 4501
	public HashSet<DynamicMeshItem> OnLoadingQueue = new HashSet<DynamicMeshItem>();

	// Token: 0x04001196 RID: 4502
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject _regionObject;

	// Token: 0x04001197 RID: 4503
	public ConcurrentQueue<Vector3i> AddChunksThreaded = new ConcurrentQueue<Vector3i>();

	// Token: 0x04001198 RID: 4504
	public ConcurrentHashSet<Vector3i> LoadedChunksThreaded = new ConcurrentHashSet<Vector3i>();

	// Token: 0x04001199 RID: 4505
	public List<Vector3i> LoadedChunks = new List<Vector3i>();

	// Token: 0x0400119B RID: 4507
	public DateTime CreateDate = DateTime.Now;

	// Token: 0x0400119C RID: 4508
	public DateTime NextLoadTime = DateTime.Now;

	// Token: 0x0400119D RID: 4509
	public DynamicRegionState State;

	// Token: 0x0400119E RID: 4510
	public bool InBuffer;

	// Token: 0x0400119F RID: 4511
	public bool FastTrackLoaded;

	// Token: 0x040011A0 RID: 4512
	public bool MarkedForDeletion;

	// Token: 0x040011A1 RID: 4513
	public bool OutsideLoadArea = true;

	// Token: 0x040011A2 RID: 4514
	public bool IsThreadedRegion;
}
