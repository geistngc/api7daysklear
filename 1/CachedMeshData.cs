using System;
using UnityEngine;

// Token: 0x020013F5 RID: 5109
[Serializable]
public class CachedMeshData
{
	// Token: 0x0600A06C RID: 41068 RVA: 0x003C7288 File Offset: 0x003C5488
	public bool ApproximatelyEquals(CachedMeshData other)
	{
		return this.name.Equals(other.name) && Mathf.Abs(this.vertexCount - other.vertexCount) < 10 && Mathf.Abs(this.triCount - other.triCount) < 10;
	}

	// Token: 0x0400796B RID: 31083
	public string name;

	// Token: 0x0400796C RID: 31084
	public int vertexCount;

	// Token: 0x0400796D RID: 31085
	public int triCount;
}
