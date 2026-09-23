using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020003B6 RID: 950
public class DynamicMeshVoxelLoad
{
	// Token: 0x1700035C RID: 860
	// (get) Token: 0x06001C89 RID: 7305 RVA: 0x000AA8C8 File Offset: 0x000A8AC8
	// (set) Token: 0x06001C8A RID: 7306 RVA: 0x000AA8D0 File Offset: 0x000A8AD0
	public DynamicMeshItem Item { get; set; }

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x06001C8B RID: 7307 RVA: 0x000AA8D9 File Offset: 0x000A8AD9
	// (set) Token: 0x06001C8C RID: 7308 RVA: 0x000AA8E1 File Offset: 0x000A8AE1
	public DyMeshData Data { get; set; }

	// Token: 0x06001C8D RID: 7309 RVA: 0x000AA8EA File Offset: 0x000A8AEA
	public void DisposeMeshes()
	{
		this.Data = DyMeshData.AddToCache(this.Data);
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x000AA8FD File Offset: 0x000A8AFD
	public IEnumerator CreateMeshCoroutine(DynamicMeshItem item)
	{
		DateTime start = DateTime.Now;
		if (item.Key != this.Item.Key)
		{
			Log.Error("mismatching items in Mesh creation");
		}
		GameObject child = DynamicMeshItem.GetItemMeshRendererFromPool();
		child.SetActive(false);
		bool hasMeshes = false;
		VoxelMesh opaqueMesh = this.Data.OpaqueMesh;
		if (opaqueMesh.Vertices.Count > 0)
		{
			hasMeshes = true;
			MeshFilter component = child.GetComponent<MeshFilter>();
			MeshRenderer component2 = child.GetComponent<MeshRenderer>();
			Mesh mesh;
			if (component.sharedMesh == null)
			{
				mesh = new Mesh();
			}
			else
			{
				mesh = component.sharedMesh;
			}
			mesh.indexFormat = ((opaqueMesh.Vertices.Count > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
			component.sharedMesh = mesh;
			opaqueMesh.CopyToMesh(component, component2, 0, null);
			if ((DateTime.Now - start).TotalMilliseconds > 3.0)
			{
				start = DateTime.Now;
				yield return null;
			}
		}
		if ((DateTime.Now - start).TotalMilliseconds > 3.0)
		{
			start = DateTime.Now;
			yield return null;
		}
		VoxelMeshTerrain terrainMesh = this.Data.TerrainMesh;
		if (terrainMesh != null && terrainMesh.Vertices != null && terrainMesh.Vertices.Count != 0)
		{
			hasMeshes = true;
			GameObject terrainMeshRendererFromPool = DynamicMeshItem.GetTerrainMeshRendererFromPool();
			terrainMeshRendererFromPool.SetActive(true);
			terrainMeshRendererFromPool.transform.parent = child.transform;
			MeshFilter component3 = terrainMeshRendererFromPool.GetComponent<MeshFilter>();
			Mesh mesh2;
			if (component3.sharedMesh == null)
			{
				mesh2 = new Mesh();
			}
			else
			{
				mesh2 = component3.sharedMesh;
			}
			mesh2.indexFormat = ((mesh2.vertexCount > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
			component3.sharedMesh = mesh2;
			DynamicMeshVoxelLoad.CopyTerrain(terrainMesh, mesh2, component3, new MeshTiming(), this.Item);
			if ((DateTime.Now - start).TotalMilliseconds > 3.0)
			{
				start = DateTime.Now;
				yield return null;
			}
		}
		if (hasMeshes)
		{
			item.ChunkObject = child;
		}
		else
		{
			DynamicMeshManager.MeshDestroy(child);
		}
		yield break;
	}

	// Token: 0x06001C8F RID: 7311 RVA: 0x000AA914 File Offset: 0x000A8B14
	public static void CopyTerrain(VoxelMeshTerrain terrain, Mesh mesh, MeshFilter filter, MeshTiming time, DynamicMeshItem item)
	{
		time.Reset();
		MeshUnsafeCopyHelper.CopyVertices(terrain.Vertices, mesh);
		time.CopyVerts = time.GetTime();
		time.Reset();
		MeshUnsafeCopyHelper.CopyUV(terrain.Uvs, mesh);
		time.CopyUv = time.time;
		time.Reset();
		MeshUnsafeCopyHelper.CopyUV2(terrain.UvsCrack, mesh);
		time.CopyUv2 = time.time;
		if (((terrain != null) ? terrain.Uvs3.Items : null) != null)
		{
			time.Reset();
			MeshUnsafeCopyHelper.CopyUV3(terrain.Uvs3, mesh);
			time.CopyUv3 = time.time;
		}
		if (((terrain != null) ? terrain.Uvs4.Items : null) != null)
		{
			time.Reset();
			MeshUnsafeCopyHelper.CopyUV4(terrain.Uvs4, mesh);
			time.CopyUv4 = time.time;
		}
		time.Reset();
		MeshUnsafeCopyHelper.CopyColors(terrain.ColorVertices, mesh);
		time.CopyColours = time.time;
		time.Reset();
		if (terrain.Indices.Count > 0)
		{
			MeshUnsafeCopyHelper.CopyTriangles(terrain.Indices, mesh);
		}
		else
		{
			mesh.subMeshCount = ((terrain.Indices.Count > 0) ? 1 : terrain.submeshes.Count);
			for (int i = 0; i < terrain.submeshes.Count; i++)
			{
				MeshUnsafeCopyHelper.CopyTriangles(terrain.submeshes[i].triangles, mesh, i);
			}
		}
		time.CopyTriangles = time.time;
		if (terrain.Normals.Count == 0)
		{
			time.Reset();
			mesh.RecalculateNormals();
			time.NormalRecalc = time.time;
		}
		else
		{
			time.Reset();
			MeshUnsafeCopyHelper.CopyNormals(terrain.Normals, mesh);
			time.CopyNormals = time.time;
		}
		time.Reset();
		time.CopyTangents = time.time;
		time.Reset();
		GameUtils.SetMeshVertexAttributes(mesh, true);
		mesh.UploadMeshData(false);
		time.UploadMesh = time.time;
		Renderer component = filter.GetComponent<Renderer>();
		int num = component.sharedMaterials.Length;
		if (!DynamicMeshFile.TerrainSharedMaterials.ContainsKey(num))
		{
			Material[] array = new Material[num];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = MeshDescription.meshes[5].material;
			}
			DynamicMeshFile.TerrainSharedMaterials.Add(num, array);
		}
		component.sharedMaterials = DynamicMeshFile.TerrainSharedMaterials[num];
	}

	// Token: 0x06001C90 RID: 7312 RVA: 0x000AAB53 File Offset: 0x000A8D53
	public static DynamicMeshVoxelLoad Create(DynamicMeshItem item, DyMeshData data)
	{
		return new DynamicMeshVoxelLoad
		{
			Item = item,
			Data = data
		};
	}

	// Token: 0x04001288 RID: 4744
	public static ConcurrentQueue<VoxelMeshLayer> LayerCache = new ConcurrentQueue<VoxelMeshLayer>();
}
