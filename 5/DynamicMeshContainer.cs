using System;
using UnityEngine;

// Token: 0x02000377 RID: 887
public abstract class DynamicMeshContainer
{
	// Token: 0x06001A47 RID: 6727 RVA: 0x0009B2C1 File Offset: 0x000994C1
	public string ToDebugLocation()
	{
		return this.WorldPosition.x.ToString() + " " + this.WorldPosition.z.ToString();
	}

	// Token: 0x06001A48 RID: 6728
	public abstract GameObject GetGameObject();

	// Token: 0x06001A49 RID: 6729 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Protected)]
	public DynamicMeshContainer()
	{
	}

	// Token: 0x040010C3 RID: 4291
	public Vector3i WorldPosition;

	// Token: 0x040010C4 RID: 4292
	public long Key;
}
