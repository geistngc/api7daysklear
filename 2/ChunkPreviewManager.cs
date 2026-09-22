using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class ChunkPreviewManager
{
	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x060018D1 RID: 6353 RVA: 0x0008C304 File Offset: 0x0008A504
	public Prefab Prefab
	{
		get
		{
			return this.PreviewData.PrefabData;
		}
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x060018D2 RID: 6354 RVA: 0x0008C311 File Offset: 0x0008A511
	public Vector3i WorldPosition
	{
		get
		{
			return this.PreviewData.WorldPosition;
		}
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x0008C320 File Offset: 0x0008A520
	public ChunkPreviewManager()
	{
		ChunkPreviewManager.Instance = this;
		this.PreviewData = new ChunkPreviewData();
		this.PreviewChunksContainer = GameObject.Find("PreviewChunks");
		if (this.PreviewChunksContainer == null)
		{
			this.PreviewChunksContainer = new GameObject("PreviewChunks");
		}
		DynamicMeshThread.SetDefaultThreads();
		this.ThreadData = new DynamicMeshPrefabPreviewThread();
		this.ThreadData.PreviewData = this.PreviewData;
		this.ThreadData.StartThread();
		GameManager.Instance.StartCoroutine(this.LoadPreviewMesh());
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x0008C3D0 File Offset: 0x0008A5D0
	public void AddChunkPreviewLoadData(DynamicMeshVoxelLoad loadData)
	{
		this.ChunkPreviewMeshData.Enqueue(loadData);
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x0008C3DE File Offset: 0x0008A5DE
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator LoadPreviewMesh()
	{
		while (!this.StopRequested)
		{
			DynamicMeshVoxelLoad voxelData;
			if (!this.ChunkPreviewMeshData.TryDequeue(out voxelData))
			{
				yield return null;
			}
			else
			{
				while (voxelData.Item.State == DynamicItemState.Loading)
				{
					Log.Out("delaying load");
					yield return null;
				}
				voxelData.Item.DestroyChunk();
				yield return GameManager.Instance.StartCoroutine(voxelData.Item.CreateMeshFromVoxelCoroutine(true, null, voxelData));
				this.AddPreviewChunk(voxelData);
				if (voxelData.Item.ChunkObject != null)
				{
					voxelData.Item.ChunkObject.transform.parent = this.PreviewChunksContainer.transform;
				}
				voxelData.DisposeMeshes();
				voxelData = null;
			}
		}
		yield break;
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x0008C3F0 File Offset: 0x0008A5F0
	public void ClearAll()
	{
		foreach (DynamicMeshItem dynamicMeshItem in this.Items)
		{
			dynamicMeshItem.DestroyChunk();
		}
		this.Items.Clear();
	}

	// Token: 0x060018D7 RID: 6359 RVA: 0x0008C44C File Offset: 0x0008A64C
	public void AddPreviewChunk(DynamicMeshVoxelLoad loadData)
	{
		DynamicMeshItem item = loadData.Item;
		if (this.IsPositionInArea(loadData.Item.WorldPosition))
		{
			this.SetChunkGoVisiblity(item.Key, false);
		}
		else
		{
			this.SetChunkGoVisiblity(item.Key, true);
			loadData.Item.DestroyChunk();
		}
		this.CheckItems();
	}

	// Token: 0x060018D8 RID: 6360 RVA: 0x0008C4A1 File Offset: 0x0008A6A1
	public bool IsPositionInArea(Vector3 pos)
	{
		return this.IsPositionInArea(new Vector3i(pos));
	}

	// Token: 0x060018D9 RID: 6361 RVA: 0x0008C4B0 File Offset: 0x0008A6B0
	public void ActivationChanged(PrefabInstance pi)
	{
		if (pi == null)
		{
			this.SetWorldPosition(new Vector3i(this.WorldPosition.x, -512, this.WorldPosition.z));
			this.CheckItems();
			return;
		}
		this.SetPrefab(pi.prefab, pi, pi.boundingBoxPosition);
	}

	// Token: 0x060018DA RID: 6362 RVA: 0x0008C500 File Offset: 0x0008A700
	public bool IsActivePrefab(PrefabInstance pi)
	{
		return pi.prefab == this.PreviewData.PrefabData && this.IsPositionInArea(pi.boundingBoxPosition);
	}

	// Token: 0x060018DB RID: 6363 RVA: 0x0008C528 File Offset: 0x0008A728
	public bool IsPositionInArea(Vector3i fullposition)
	{
		if (this.WorldPosition.y < -256)
		{
			return false;
		}
		Vector3i chunkPositionFromWorldPosition = DynamicMeshUnity.GetChunkPositionFromWorldPosition(fullposition);
		Vector3i chunkPositionFromWorldPosition2 = DynamicMeshUnity.GetChunkPositionFromWorldPosition(this.PreviewData.WorldPosition);
		Vector3i chunkPositionFromWorldPosition3 = DynamicMeshUnity.GetChunkPositionFromWorldPosition(this.PreviewData.WorldPosition + this.PreviewData.PrefabData.size);
		int x = chunkPositionFromWorldPosition2.x;
		int x2 = chunkPositionFromWorldPosition3.x;
		int z = chunkPositionFromWorldPosition2.z;
		int z2 = chunkPositionFromWorldPosition3.z;
		return chunkPositionFromWorldPosition.x >= x && chunkPositionFromWorldPosition.x <= x2 && chunkPositionFromWorldPosition.z >= z && chunkPositionFromWorldPosition.z <= z2;
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x0008C5CC File Offset: 0x0008A7CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckItems()
	{
		for (int i = this.Items.Count - 1; i >= 0; i--)
		{
			DynamicMeshItem dynamicMeshItem = this.Items[i];
			if (!this.IsPositionInArea(dynamicMeshItem.WorldPosition))
			{
				this.SetChunkGoVisiblity(dynamicMeshItem.Key, true);
				dynamicMeshItem.DestroyChunk();
			}
		}
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x0008C620 File Offset: 0x0008A820
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetChunkGoVisiblity(long chunkKey, bool isVisible)
	{
		string b = string.Format("Chunk_{0},{1}", DynamicMeshUnity.GetChunkSectionX(chunkKey), DynamicMeshUnity.GetChunkSectionZ(chunkKey));
		foreach (ChunkGameObject chunkGameObject in GameManager.Instance.World.m_ChunkManager.GetUsedChunkGameObjects())
		{
			Transform transform = chunkGameObject.transform;
			if (transform.name == b)
			{
				if (isVisible)
				{
					transform.gameObject.transform.localScale = Vector3.one;
				}
				else
				{
					transform.gameObject.transform.localScale = Vector3.zero;
				}
			}
		}
	}

	// Token: 0x060018DE RID: 6366 RVA: 0x0008C6E0 File Offset: 0x0008A8E0
	public void CleanUp()
	{
		DynamicMeshPrefabPreviewThread threadData = this.ThreadData;
		if (threadData == null)
		{
			return;
		}
		threadData.StopThread();
	}

	// Token: 0x060018DF RID: 6367 RVA: 0x0008C6F4 File Offset: 0x0008A8F4
	public void SetWorldPosition(Vector3i worldPosition)
	{
		this.PreviewData.WorldPosition = worldPosition;
		if (this.Prefab != null)
		{
			int num = DynamicMeshUnity.RoundChunk(worldPosition.x + this.Prefab.size.x) + 16;
			int num2 = DynamicMeshUnity.RoundChunk(worldPosition.z + this.Prefab.size.z) + 16;
			for (int i = worldPosition.x; i <= num; i += 16)
			{
				for (int j = worldPosition.z; j <= num2; j += 16)
				{
					this.StartChunkPreview(new Vector3i(i, 0, j));
				}
			}
		}
		this.CheckItems();
	}

	// Token: 0x060018E0 RID: 6368 RVA: 0x0008C790 File Offset: 0x0008A990
	public void StartChunkPreview(Vector3i chunkPos)
	{
		DynamicMeshItem item = this.Get(chunkPos);
		this.ThreadData.AddChunk(item);
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x0008C7B1 File Offset: 0x0008A9B1
	public void SetPrefab(PrefabInstance prefab)
	{
		this.SetPrefab(prefab.prefab, prefab, this.WorldPosition);
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x0008C7C6 File Offset: 0x0008A9C6
	public void SetPrefab(Prefab prefab)
	{
		this.SetPrefab(prefab, null, this.WorldPosition);
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x0008C7D8 File Offset: 0x0008A9D8
	public void SetPrefab(Prefab prefab, PrefabInstance instance, Vector3i worldPosition)
	{
		PrefabInstance prefabInstance = this.PreviewData.PrefabInstance;
		if (prefabInstance != null)
		{
			PrefabLODManager.PrefabGameObject instance2 = GameManager.Instance.prefabLODManager.GetInstance(prefabInstance.id);
			if (instance2 != null)
			{
				instance2.go.SetActive(true);
			}
		}
		this.PreviewData.PrefabData = prefab;
		this.PreviewData.PrefabInstance = instance;
		if (instance != null)
		{
			PrefabLODManager.PrefabGameObject instance3 = GameManager.Instance.prefabLODManager.GetInstance(instance.id);
			if (instance3 != null)
			{
				instance3.go.SetActive(false);
			}
		}
		this.SetWorldPosition(worldPosition);
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x0008C860 File Offset: 0x0008AA60
	public DynamicMeshItem Get(Vector3i worldPos)
	{
		worldPos = DynamicMeshUnity.GetChunkPositionFromWorldPosition(worldPos);
		foreach (DynamicMeshItem dynamicMeshItem in this.Items)
		{
			if (dynamicMeshItem.WorldPosition.x == worldPos.x && dynamicMeshItem.WorldPosition.z == worldPos.z)
			{
				return dynamicMeshItem;
			}
		}
		DynamicMeshItem dynamicMeshItem2 = new DynamicMeshItem(worldPos);
		this.Items.Add(dynamicMeshItem2);
		return dynamicMeshItem2;
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x0008C8F4 File Offset: 0x0008AAF4
	public void Update()
	{
		if (this.NextUpdate > DateTime.Now)
		{
			return;
		}
		this.NextUpdate = DateTime.Now.AddSeconds(1.0);
		foreach (DynamicMeshItem dynamicMeshItem in this.Items)
		{
			this.SetChunkGoVisiblity(dynamicMeshItem.Key, dynamicMeshItem.ChunkObject == null || !this.IsPositionInArea(dynamicMeshItem.WorldPosition));
		}
	}

	// Token: 0x04000FD1 RID: 4049
	public static ChunkPreviewManager Instance;

	// Token: 0x04000FD2 RID: 4050
	public List<DynamicMeshItem> Items = new List<DynamicMeshItem>();

	// Token: 0x04000FD3 RID: 4051
	public ChunkPreviewData PreviewData;

	// Token: 0x04000FD4 RID: 4052
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicMeshPrefabPreviewThread ThreadData;

	// Token: 0x04000FD5 RID: 4053
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject PreviewChunksContainer;

	// Token: 0x04000FD6 RID: 4054
	public ConcurrentQueue<DynamicMeshVoxelLoad> ChunkPreviewMeshData = new ConcurrentQueue<DynamicMeshVoxelLoad>();

	// Token: 0x04000FD7 RID: 4055
	[PublicizedFrom(EAccessModifier.Private)]
	public bool StopRequested;

	// Token: 0x04000FD8 RID: 4056
	[PublicizedFrom(EAccessModifier.Private)]
	public DateTime NextUpdate = DateTime.Now;
}
