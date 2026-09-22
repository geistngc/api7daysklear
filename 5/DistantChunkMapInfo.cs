using System;
using UnityEngine;

// Token: 0x02000C2B RID: 3115
public class DistantChunkMapInfo
{
	// Token: 0x04004A1C RID: 18972
	public Vector4[] ChunkTriggerArea;

	// Token: 0x04004A1D RID: 18973
	public Vector2i[][] ChunkToDelete;

	// Token: 0x04004A1E RID: 18974
	public Vector2i[][] ChunkToAdd;

	// Token: 0x04004A1F RID: 18975
	public Vector2i[][] ChunkToConvDel;

	// Token: 0x04004A20 RID: 18976
	public Vector2i[][] ChunkToConvAdd;

	// Token: 0x04004A21 RID: 18977
	public Vector4[][] ChunkToConvAddEdgeFactor;

	// Token: 0x04004A22 RID: 18978
	public Vector4[][] ChunkToAddEdgeFactor;

	// Token: 0x04004A23 RID: 18979
	public Vector2i[][][] ChunkEdgeToOwnResLevel;

	// Token: 0x04004A24 RID: 18980
	public int[][][] ChunkEdgeToOwnRLEdgeId;

	// Token: 0x04004A25 RID: 18981
	public Vector2i[][][] ChunkEdgeToNextResLevel;

	// Token: 0x04004A26 RID: 18982
	public int[][][] ChunkEdgeToNextRLEdgeId;

	// Token: 0x04004A27 RID: 18983
	public Vector2i[] ChunkLLIntPos;

	// Token: 0x04004A28 RID: 18984
	public Vector2[] ChunkLLPos;

	// Token: 0x04004A29 RID: 18985
	public int[][] NeighbResLevel;

	// Token: 0x04004A2A RID: 18986
	public float[][] EdgeResFactor;

	// Token: 0x04004A2B RID: 18987
	public float NextResLevelEdgeFactor;

	// Token: 0x04004A2C RID: 18988
	public int NbChunk;

	// Token: 0x04004A2D RID: 18989
	public int NbCurChunkInOneNextLevelChunk;

	// Token: 0x04004A2E RID: 18990
	public int ResLevel;

	// Token: 0x04004A2F RID: 18991
	public int LayerId = 28;

	// Token: 0x04004A30 RID: 18992
	public int ChunkDataListResLevel;

	// Token: 0x04004A31 RID: 18993
	public int ChunkResolution;

	// Token: 0x04004A32 RID: 18994
	public int ColliderResolution;

	// Token: 0x04004A33 RID: 18995
	public bool IsColliderEnabled;

	// Token: 0x04004A34 RID: 18996
	public float ResRadius;

	// Token: 0x04004A35 RID: 18997
	public int IntResRadius;

	// Token: 0x04004A36 RID: 18998
	public Vector2i LLIntArea;

	// Token: 0x04004A37 RID: 18999
	public float ChunkWidth;

	// Token: 0x04004A38 RID: 19000
	public float UnitStep;

	// Token: 0x04004A39 RID: 19001
	public Vector2 ShiftVec;

	// Token: 0x04004A3A RID: 19002
	public Vector3 ChunkExtraShiftVector;

	// Token: 0x04004A3B RID: 19003
	public DistantChunkBasicMesh BaseMesh;

	// Token: 0x04004A3C RID: 19004
	public int[][] EdgeMap;

	// Token: 0x04004A3D RID: 19005
	public int[] SouthMap;

	// Token: 0x04004A3E RID: 19006
	public int[] WestMap;

	// Token: 0x04004A3F RID: 19007
	public int[] NorthMap;

	// Token: 0x04004A40 RID: 19008
	public int[] EastMap;
}
