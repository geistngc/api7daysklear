using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x02000378 RID: 888
public class DynamicMeshItem : DynamicMeshContainer, IEquatable<DynamicMeshItem>
{
	// Token: 0x1700031C RID: 796
	// (get) Token: 0x06001A4A RID: 6730 RVA: 0x0009B2ED File Offset: 0x000994ED
	// (set) Token: 0x06001A4B RID: 6731 RVA: 0x0009B2F5 File Offset: 0x000994F5
	public Rect Rect { get; set; }

	// Token: 0x06001A4C RID: 6732 RVA: 0x0009B2FE File Offset: 0x000994FE
	public override GameObject GetGameObject()
	{
		return this.ChunkObject;
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x0009B308 File Offset: 0x00099508
	public DynamicMeshItem(Vector3i pos)
	{
		this.WorldPosition = pos;
		this.Rect = new Rect((float)pos.x, (float)pos.z, 16f, 16f);
		this.Key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(pos.x), World.toChunkXZ(pos.z));
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x0009B36D File Offset: 0x0009956D
	public static void AddToMeshPool(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		DynamicMeshManager.MeshDestroy(go);
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x0009B380 File Offset: 0x00099580
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool AddToPoolInternal(GameObject go)
	{
		using (HashSet<GameObject>.Enumerator enumerator = DynamicMeshItem.MeshPool.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GetInstanceID() == go.GetInstanceID())
				{
					Log.Warning("Duplicate pool add. Name: " + go.name);
					return false;
				}
			}
		}
		DynamicMeshItem.MeshPool.Add(go);
		go.transform.parent = null;
		go.SetActive(false);
		go.GetComponent<MeshFilter>().mesh.Clear(false);
		return true;
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x0009B424 File Offset: 0x00099624
	public static GameObject GetItemMeshRendererFromPool()
	{
		GameObject gameObject;
		if (DynamicMeshItem.MeshPool.Count > 0)
		{
			gameObject = DynamicMeshItem.MeshPool.Last<GameObject>();
			DynamicMeshItem.MeshPool.Remove(gameObject);
		}
		else
		{
			gameObject = DynamicMeshFile.CreateMeshObject(string.Empty, false);
		}
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.parent = DynamicMeshManager.ParentTransform;
		return gameObject;
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x0009B484 File Offset: 0x00099684
	public static GameObject GetRegionMeshRendererFromPool()
	{
		GameObject gameObject;
		if (DynamicMeshItem.MeshPool.Count > 0)
		{
			gameObject = DynamicMeshItem.MeshPool.Last<GameObject>();
			DynamicMeshItem.MeshPool.Remove(gameObject);
		}
		else
		{
			gameObject = DynamicMeshFile.CreateMeshObject(string.Empty, true);
		}
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.parent = DynamicMeshManager.ParentTransform;
		return gameObject;
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x0009B4E4 File Offset: 0x000996E4
	public static GameObject GetTerrainMeshRendererFromPool()
	{
		GameObject gameObject;
		if (DynamicMeshItem.TerrainMeshPool.Count > 0)
		{
			gameObject = DynamicMeshItem.TerrainMeshPool.Last<GameObject>();
			DynamicMeshItem.TerrainMeshPool.Remove(gameObject);
		}
		else
		{
			gameObject = DynamicMeshFile.CreateTerrainMeshObject(string.Empty);
		}
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.parent = DynamicMeshManager.ParentTransform;
		return gameObject;
	}

	// Token: 0x06001A53 RID: 6739 RVA: 0x0009B543 File Offset: 0x00099743
	public void CleanUp()
	{
		if (this.ChunkObject != null)
		{
			DynamicMeshManager.MeshDestroy(this.ChunkObject);
		}
		this.State = DynamicItemState.UpdateRequired;
	}

	// Token: 0x1700031D RID: 797
	// (get) Token: 0x06001A54 RID: 6740 RVA: 0x0009B568 File Offset: 0x00099768
	public int Triangles
	{
		get
		{
			int num = 0;
			if (this.ChunkObject != null)
			{
				num += this.ChunkObject.GetComponent<MeshFilter>().mesh.triangles.Length;
				foreach (object obj in this.ChunkObject.transform)
				{
					Transform transform = (Transform)obj;
					num += transform.gameObject.GetComponent<MeshFilter>().mesh.triangles.Length;
				}
			}
			return num;
		}
	}

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x06001A55 RID: 6741 RVA: 0x0009B604 File Offset: 0x00099804
	public int Vertices
	{
		get
		{
			int num = 0;
			if (this.ChunkObject != null)
			{
				num += this.ChunkObject.GetComponent<MeshFilter>().mesh.vertexCount;
				foreach (object obj in this.ChunkObject.transform)
				{
					Transform transform = (Transform)obj;
					num += transform.gameObject.GetComponent<MeshFilter>().mesh.vertexCount;
				}
			}
			return num;
		}
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x0009B69C File Offset: 0x0009989C
	public Vector3i GetRegionLocation()
	{
		return DynamicMeshUnity.GetRegionPositionFromWorldPosition(this.WorldPosition);
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x0009B6A9 File Offset: 0x000998A9
	public long GetRegionKey()
	{
		return WorldChunkCache.MakeChunkKey(World.toChunkXZ(DynamicMeshUnity.RoundRegion(this.WorldPosition.x)), World.toChunkXZ(DynamicMeshUnity.RoundRegion(this.WorldPosition.z)));
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x0009B6DC File Offset: 0x000998DC
	public int ReadUpdateTimeFromFile()
	{
		int result;
		using (Stream stream = SdFile.OpenRead(this.Path))
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				pooledBinaryReader.SetBaseStream(stream);
				result = pooledBinaryReader.ReadInt32();
			}
		}
		return result;
	}

	// Token: 0x06001A59 RID: 6745 RVA: 0x0009B744 File Offset: 0x00099944
	public DynamicMeshRegion GetRegion()
	{
		DynamicMeshRegion.GetRegionFromWorldPosition(this.WorldPosition);
		DynamicMeshRegion result;
		DynamicMeshRegion.Regions.TryGetValue(this.GetRegionKey(), out result);
		return result;
	}

	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06001A5A RID: 6746 RVA: 0x0009B771 File Offset: 0x00099971
	public bool IsVisible
	{
		get
		{
			return this.ChunkObject != null && this.ChunkObject.activeSelf;
		}
	}

	// Token: 0x17000320 RID: 800
	// (get) Token: 0x06001A5B RID: 6747 RVA: 0x0009B790 File Offset: 0x00099990
	public bool IsChunkInView
	{
		get
		{
			if (GameManager.IsDedicatedServer)
			{
				return false;
			}
			if (DynamicMeshManager.Instance == null)
			{
				return false;
			}
			Vector3 position = DynamicMeshItem.player.position;
			int viewSize = DynamicMeshManager.GetViewSize(DynamicMeshItem.player);
			int num = World.toChunkXZ(Utils.Fastfloor(position.x)) * 16;
			int num2 = World.toChunkXZ(Utils.Fastfloor(position.z)) * 16;
			int num3 = num - viewSize;
			int num4 = num + viewSize;
			int num5 = num2 - viewSize;
			int num6 = num2 + viewSize;
			return this.WorldPosition.x > num3 && this.WorldPosition.x <= num4 && this.WorldPosition.z > num5 && this.WorldPosition.z <= num6;
		}
	}

	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06001A5C RID: 6748 RVA: 0x0009B842 File Offset: 0x00099A42
	public bool IsChunkInGame
	{
		get
		{
			return !GameManager.IsDedicatedServer && DynamicMeshManager.ChunkGameObjects.Contains(this.Key);
		}
	}

	// Token: 0x06001A5D RID: 6749 RVA: 0x0009B860 File Offset: 0x00099A60
	public void SetVisible(bool active, string reason)
	{
		if (this.ChunkObject == null)
		{
			return;
		}
		if (active != this.ChunkObject.activeSelf)
		{
			if (DynamicMeshManager.DebugItemPositions)
			{
				this.ChunkObject.name = string.Concat(new string[]
				{
					"C ",
					base.ToDebugLocation(),
					": ",
					reason,
					" (",
					active.ToString(),
					")"
				});
			}
			this.ChunkObject.SetActive(active);
			if (DynamicMeshManager.DoLog)
			{
				Log.Out(string.Concat(new string[]
				{
					"Chunk ",
					this.WorldPosition.x.ToString(),
					",",
					this.WorldPosition.z.ToString(),
					" Visible: ",
					active.ToString(),
					" reason: ",
					reason,
					" inview: ",
					this.IsChunkInView.ToString()
				}));
			}
		}
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x0009B974 File Offset: 0x00099B74
	public void ForceHide()
	{
		if (this.ChunkObject == null || !this.ChunkObject.activeSelf)
		{
			return;
		}
		if (DynamicMeshManager.DebugItemPositions)
		{
			this.ChunkObject.name = "C " + base.ToDebugLocation() + ": forceHide";
		}
		this.ChunkObject.SetActive(false);
		if (DynamicMeshManager.DoLog)
		{
			Log.Out(string.Concat(new string[]
			{
				"Chunk ",
				this.WorldPosition.x.ToString(),
				",",
				this.WorldPosition.z.ToString(),
				" ForceHide"
			}));
		}
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x0009BA28 File Offset: 0x00099C28
	public void OnCorrupted()
	{
		if (DynamicMeshManager.DoLog)
		{
			string str = "Corrupted item. Adding for regen ";
			Vector3i worldPosition = this.WorldPosition;
			DynamicMeshManager.LogMsg(str + worldPosition.ToString());
		}
		DynamicMeshManager.Instance.AddChunk(this.WorldPosition, true);
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x0009BA74 File Offset: 0x00099C74
	public bool LoadIfEmpty(string caller, bool urgentLoad, bool regionInBuffer)
	{
		if (this.ChunkObject != null)
		{
			return false;
		}
		if (this.State == DynamicItemState.ReadyToDelete)
		{
			return false;
		}
		if (this.State == DynamicItemState.Empty)
		{
			return false;
		}
		if (this.State == DynamicItemState.LoadRequested)
		{
			return false;
		}
		if (!DynamicMeshManager.Instance.IsInLoadableArea(this.Key))
		{
			return false;
		}
		this.State = DynamicItemState.LoadRequested;
		DynamicMeshManager.Instance.AddItemLoadRequest(this, urgentLoad);
		return true;
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x0009BADA File Offset: 0x00099CDA
	public bool Load(string caller, bool urgentLoad, bool regionInBuffer)
	{
		if (this.State == DynamicItemState.ReadyToDelete)
		{
			return false;
		}
		if (this.State == DynamicItemState.LoadRequested)
		{
			return false;
		}
		this.State = DynamicItemState.LoadRequested;
		DynamicMeshManager.Instance.AddItemLoadRequest(this, urgentLoad);
		return true;
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x0009BB08 File Offset: 0x00099D08
	public float DistanceToPlayer(Vector3i playerPos)
	{
		return Math.Abs(Mathf.Sqrt(Mathf.Pow((float)(playerPos.x - this.WorldPosition.x), 2f) + Mathf.Pow((float)(playerPos.z - this.WorldPosition.z), 2f)));
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x0009BB5C File Offset: 0x00099D5C
	public float DistanceToPlayer()
	{
		EntityPlayerLocal player = DynamicMeshItem.player;
		Vector3 vector = (player == null) ? Vector3.zero : player.position;
		return Math.Abs(Mathf.Sqrt(Mathf.Pow(vector.x - (float)this.WorldPosition.x, 2f) + Mathf.Pow(vector.z - (float)this.WorldPosition.z, 2f)));
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x0009BBCB File Offset: 0x00099DCB
	public float DistanceToPlayer(float x, float z)
	{
		return Math.Abs(Mathf.Sqrt(Mathf.Pow(x - (float)this.WorldPosition.x, 2f) + Mathf.Pow(z - (float)this.WorldPosition.z, 2f)));
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x0009BC08 File Offset: 0x00099E08
	public bool DestroyChunk()
	{
		bool result = false;
		if (this.ChunkObject != null)
		{
			result = true;
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Destroying chunk " + base.ToDebugLocation());
			}
			DynamicMeshManager.MeshDestroy(this.ChunkObject);
		}
		return result;
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x0009BC4F File Offset: 0x00099E4F
	public void DestroyMesh()
	{
		DynamicMeshManager.Instance.AddObjectForDestruction(this.ChunkObject);
		this.ChunkObject = null;
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x0009BC68 File Offset: 0x00099E68
	public bool CreateMeshSync(bool isVisible)
	{
		if (this.ChunkObject != null)
		{
			this.SetVisible(isVisible, "Create mesh exists");
			return false;
		}
		string path = this.Path;
		if (!SdFile.Exists(path))
		{
			this.State = DynamicItemState.Empty;
			return false;
		}
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			using (Stream readStream = DynamicMeshFile.GetReadStream(path))
			{
				pooledBinaryReader.SetBaseStream(readStream);
				if (pooledBinaryReader.BaseStream.Position == pooledBinaryReader.BaseStream.Length)
				{
					this.State = DynamicItemState.Empty;
				}
				else
				{
					DynamicMeshFile.ReadItemMesh(pooledBinaryReader, this, isVisible);
				}
			}
		}
		if (this.ChunkObject != null)
		{
			this.SetVisible(isVisible, "create mesh complete");
			this.SetPosition();
			Quaternion identity = Quaternion.identity;
			this.ChunkObject.transform.parent = DynamicMeshManager.ParentTransform;
			this.ChunkObject.transform.rotation = identity;
		}
		this.PackageLength = this.GetStreamLength();
		this.State = DynamicItemState.Loaded;
		return true;
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x0009BD80 File Offset: 0x00099F80
	public void SetPosition()
	{
		if (this.ChunkObject == null)
		{
			return;
		}
		this.ChunkObject.transform.position = this.WorldPosition.ToVector3() - Origin.position;
	}

	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06001A69 RID: 6761 RVA: 0x0009BDB6 File Offset: 0x00099FB6
	public string Path
	{
		get
		{
			return DynamicMeshUnity.GetItemPath(this.Key);
		}
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x0009BDC3 File Offset: 0x00099FC3
	public bool FileExists()
	{
		return SdFile.Exists(this.Path);
	}

	// Token: 0x06001A6B RID: 6763 RVA: 0x0009BDD0 File Offset: 0x00099FD0
	public IEnumerator CreateMeshFromVoxelCoroutine(bool isVisible, MicroStopwatch stop, DynamicMeshVoxelLoad data)
	{
		GameObject oldMesh = this.ChunkObject;
		this.ChunkObject = null;
		this.State = DynamicItemState.Loading;
		yield return GameManager.Instance.StartCoroutine(data.CreateMeshCoroutine(this));
		if (this.ChunkObject != null)
		{
			Quaternion identity = Quaternion.identity;
			this.ChunkObject.transform.parent = DynamicMeshManager.ParentTransform;
			this.ChunkObject.transform.rotation = identity;
			this.SetVisible(isVisible, "create mesh complete coroutine");
			this.SetPosition();
			this.State = DynamicItemState.Loaded;
		}
		else
		{
			this.State = DynamicItemState.Empty;
		}
		if (oldMesh != null)
		{
			DynamicMeshItem.AddToMeshPool(oldMesh);
		}
		yield break;
	}

	// Token: 0x06001A6C RID: 6764 RVA: 0x0009BDED File Offset: 0x00099FED
	public override int GetHashCode()
	{
		return this.Key.GetHashCode();
	}

	// Token: 0x06001A6D RID: 6765 RVA: 0x0009BDFC File Offset: 0x00099FFC
	public override bool Equals(object obj)
	{
		DynamicMeshItem dynamicMeshItem = obj as DynamicMeshItem;
		return dynamicMeshItem != null && dynamicMeshItem.Key == this.Key;
	}

	// Token: 0x06001A6E RID: 6766 RVA: 0x0009BE24 File Offset: 0x0009A024
	public int GetStreamLength()
	{
		int num = 20;
		if (this.ChunkObject == null)
		{
			return num;
		}
		MeshFilter component = this.ChunkObject.GetComponent<MeshFilter>();
		num += 12 + component.mesh.vertexCount * 6 + component.mesh.vertexCount * 8 + component.mesh.triangles.Length * 2;
		foreach (object obj in this.ChunkObject.transform)
		{
			component = ((Transform)obj).gameObject.GetComponent<MeshFilter>();
			num += 12 + component.mesh.vertexCount * 6 + component.mesh.vertexCount * 8 + component.mesh.triangles.Length * 2;
		}
		return num;
	}

	// Token: 0x06001A6F RID: 6767 RVA: 0x0009BF08 File Offset: 0x0009A108
	public int GetStreamLength(List<VoxelMesh> meshes, List<VoxelMeshTerrain> terrainMeshes)
	{
		int num = 20;
		foreach (VoxelMesh m in meshes)
		{
			num += m.GetByteLength();
		}
		foreach (VoxelMeshTerrain m2 in terrainMeshes)
		{
			num += m2.GetByteLength();
		}
		return num;
	}

	// Token: 0x06001A70 RID: 6768 RVA: 0x0009BFA0 File Offset: 0x0009A1A0
	public bool Equals(DynamicMeshItem other)
	{
		return other.Key == this.Key;
	}

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06001A71 RID: 6769 RVA: 0x0009BFB0 File Offset: 0x0009A1B0
	public static EntityPlayerLocal player
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (GameManager.Instance.World != null)
			{
				return GameManager.Instance.World.GetPrimaryPlayer();
			}
			return null;
		}
	}

	// Token: 0x040010C5 RID: 4293
	public static HashSet<GameObject> MeshPool = new HashSet<GameObject>();

	// Token: 0x040010C6 RID: 4294
	public static HashSet<GameObject> TerrainMeshPool = new HashSet<GameObject>();

	// Token: 0x040010C7 RID: 4295
	[PublicizedFrom(EAccessModifier.Private)]
	public static int cacheId = 0;

	// Token: 0x040010C9 RID: 4297
	public GameObject ChunkObject;

	// Token: 0x040010CA RID: 4298
	public int UpdateTime;

	// Token: 0x040010CB RID: 4299
	public int PackageLength;

	// Token: 0x040010CC RID: 4300
	public DynamicItemState State = DynamicItemState.UpdateRequired;
}
