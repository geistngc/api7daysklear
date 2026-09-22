using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B23 RID: 2851
public class ChunkGameObjectLayer : IMemoryPoolableObject
{
	// Token: 0x06005632 RID: 22066 RVA: 0x0020FB0C File Offset: 0x0020DD0C
	public ChunkGameObjectLayer()
	{
		int num = MeshDescription.meshes.Length;
		this.m_MeshFilter = new MeshFilter[num];
		this.m_MeshRenderer = new MeshRenderer[num];
		this.m_MeshCollider = new MeshCollider[num];
		this.m_MeshesGO = new GameObject[num];
		this.m_ParentGO = new GameObject("CLayer");
		Transform transform = this.m_ParentGO.transform;
		for (int i = 0; i < num; i++)
		{
			MeshDescription meshDescription = MeshDescription.meshes[i];
			GameObject gameObject = new GameObject(meshDescription.Name);
			this.m_MeshesGO[i] = gameObject;
			gameObject.transform.SetParent(transform, false);
			VoxelMesh.CreateMeshFilter(i, 0, gameObject, meshDescription.Tag, true, out this.m_MeshFilter[i], out this.m_MeshRenderer[i], out this.m_MeshCollider[i]);
		}
		if (OcclusionManager.Instance.cullChunkLayers)
		{
			Occludee.Add(this.m_ParentGO);
		}
		this.m_ParentGO.SetActive(false);
	}

	// Token: 0x06005633 RID: 22067 RVA: 0x0020FC04 File Offset: 0x0020DE04
	public void Init(int _chunkLayerIdx, IReadOnlyDictionary<string, int> _layerMappingTable, Transform _chunkT, bool _bStatic)
	{
		for (int i = 0; i < MeshDescription.meshes.Length; i++)
		{
			MeshDescription meshDescription = MeshDescription.meshes[i];
			GameObject gameObject = this.m_MeshesGO[i];
			gameObject.isStatic = _bStatic;
			if (!string.IsNullOrEmpty(meshDescription.MeshLayerName))
			{
				gameObject.layer = _layerMappingTable[meshDescription.MeshLayerName];
			}
			else
			{
				gameObject.layer = 0;
			}
			MeshCollider meshCollider = this.m_MeshCollider[i];
			if (meshCollider)
			{
				if (meshCollider.sharedMesh)
				{
					Log.Warning("ChunkGameObjectLayer Init collider '{0}' should be null", new object[]
					{
						meshCollider.sharedMesh.name
					});
				}
				GameObject gameObject2 = meshCollider.gameObject;
				gameObject2.isStatic = _bStatic;
				gameObject2.layer = _layerMappingTable[meshDescription.ColliderLayerName];
			}
		}
		this.m_ParentGO.name = "CLayer" + _chunkLayerIdx.ToString("00");
		this.m_ParentGO.transform.SetParent(_chunkT, false);
	}

	// Token: 0x06005634 RID: 22068 RVA: 0x0020FCF8 File Offset: 0x0020DEF8
	public void Reset()
	{
		foreach (MeshFilter meshFilter2 in this.m_MeshFilter)
		{
			if (meshFilter2)
			{
				Mesh sharedMesh = meshFilter2.sharedMesh;
				if (sharedMesh)
				{
					meshFilter2.sharedMesh = null;
					VoxelMesh.AddPooledMesh(sharedMesh);
				}
			}
		}
		foreach (MeshCollider meshCollider2 in this.m_MeshCollider)
		{
			if (meshCollider2)
			{
				Mesh sharedMesh2 = meshCollider2.sharedMesh;
				if (sharedMesh2)
				{
					meshCollider2.sharedMesh = null;
					UnityEngine.Object.Destroy(sharedMesh2);
				}
			}
		}
	}

	// Token: 0x06005635 RID: 22069 RVA: 0x0020FD8C File Offset: 0x0020DF8C
	public void Cleanup()
	{
		Span<MeshFilter> span = this.m_MeshFilter.AsSpan<MeshFilter>();
		for (int i = 0; i < span.Length; i++)
		{
			ref MeshFilter ptr = ref span[i];
			if (ptr)
			{
				Mesh sharedMesh = ptr.sharedMesh;
				if (sharedMesh)
				{
					ptr.sharedMesh = null;
					UnityEngine.Object.Destroy(sharedMesh);
				}
				GameUtils.NullThenDestroy<MeshFilter>(ref ptr);
			}
		}
		Span<MeshRenderer> span2 = this.m_MeshRenderer.AsSpan<MeshRenderer>();
		for (int i = 0; i < span2.Length; i++)
		{
			ref MeshRenderer ptr2 = ref span2[i];
			if (ptr2)
			{
				GameUtils.NullThenDestroy<MeshRenderer>(ref ptr2);
			}
		}
		Span<MeshCollider> span3 = this.m_MeshCollider.AsSpan<MeshCollider>();
		for (int i = 0; i < span3.Length; i++)
		{
			ref MeshCollider ptr3 = ref span3[i];
			if (ptr3)
			{
				Mesh sharedMesh2 = ptr3.sharedMesh;
				if (sharedMesh2)
				{
					ptr3.sharedMesh = null;
					UnityEngine.Object.Destroy(sharedMesh2);
				}
				GameUtils.NullThenDestroy<MeshCollider>(ref ptr3);
			}
		}
		Span<GameObject> span4 = this.m_MeshesGO.AsSpan<GameObject>();
		for (int i = 0; i < span4.Length; i++)
		{
			ref GameObject ptr4 = ref span4[i];
			if (ptr4)
			{
				GameUtils.NullThenDestroy<GameObject>(ref ptr4);
			}
		}
		GameUtils.NullThenDestroy<GameObject>(ref this.m_ParentGO);
	}

	// Token: 0x0400429F RID: 17055
	public GameObject m_ParentGO;

	// Token: 0x040042A0 RID: 17056
	public MeshFilter[] m_MeshFilter;

	// Token: 0x040042A1 RID: 17057
	public MeshRenderer[] m_MeshRenderer;

	// Token: 0x040042A2 RID: 17058
	public MeshCollider[] m_MeshCollider;

	// Token: 0x040042A3 RID: 17059
	public GameObject[] m_MeshesGO;

	// Token: 0x040042A4 RID: 17060
	public bool isGrassCastShadows;

	// Token: 0x040042A5 RID: 17061
	public static int InstanceCount;
}
