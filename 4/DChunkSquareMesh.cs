using System;
using UnityEngine;

// Token: 0x02000C25 RID: 3109
public class DChunkSquareMesh
{
	// Token: 0x06005ECE RID: 24270 RVA: 0x0025010C File Offset: 0x0024E30C
	public DChunkSquareMesh(DistantChunkMap DCMap, int LODLevel)
	{
		DistantChunkMapInfo distantChunkMapInfo = DCMap.ChunkMapInfoArray[LODLevel];
		this.Init(distantChunkMapInfo.BaseMesh.Vertices.Length, LODLevel, distantChunkMapInfo.ChunkResolution, distantChunkMapInfo.ColliderResolution, DCMap.NbResLevel, distantChunkMapInfo.BaseMesh.Triangles.Length);
	}

	// Token: 0x06005ECF RID: 24271 RVA: 0x0025016C File Offset: 0x0024E36C
	public void Init(int NbVertices, int ResLevel, int Resolution, int ColliderResolution, int MaxNbResLevel, int NbTriangles)
	{
		this.Normals = new Vector3[NbVertices];
		this.Tangents = new Vector4[NbVertices];
		this.EdgeCorNormals = new Vector3[Resolution * 4];
		this.Colors = new Color[NbVertices];
		this.TextureId = new int[NbVertices];
		this.IsWater = new bool[NbVertices];
		this.ChunkBound = default(Bounds);
		this.ColVertices = new Vector3[ColliderResolution * ColliderResolution];
		this.ColVerticesHeight = new float[ColliderResolution * ColliderResolution];
	}

	// Token: 0x040049B5 RID: 18869
	public Vector3[] Normals;

	// Token: 0x040049B6 RID: 18870
	public Vector4[] Tangents;

	// Token: 0x040049B7 RID: 18871
	public Vector3[] EdgeCorNormals;

	// Token: 0x040049B8 RID: 18872
	public Color[] Colors;

	// Token: 0x040049B9 RID: 18873
	public int[] TextureId;

	// Token: 0x040049BA RID: 18874
	public Bounds ChunkBound;

	// Token: 0x040049BB RID: 18875
	public int WaterPlaneBlockId;

	// Token: 0x040049BC RID: 18876
	public Vector3[] ColVertices;

	// Token: 0x040049BD RID: 18877
	public float[] ColVerticesHeight;

	// Token: 0x040049BE RID: 18878
	public VoxelMeshTerrain VoxelMesh = new VoxelMeshTerrain(0, 500);

	// Token: 0x040049BF RID: 18879
	public bool[] IsWater;
}
