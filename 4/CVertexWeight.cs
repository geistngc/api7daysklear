using System;
using UnityEngine;

// Token: 0x020012C9 RID: 4809
[PublicizedFrom(EAccessModifier.Internal)]
public class CVertexWeight
{
	// Token: 0x0600987A RID: 39034 RVA: 0x0039EC53 File Offset: 0x0039CE53
	public CVertexWeight(int i, Vector3 p, float w)
	{
		this.index = i;
		this.localPosition = p;
		this.weight = w;
	}

	// Token: 0x040072AD RID: 29357
	public int index;

	// Token: 0x040072AE RID: 29358
	public Vector3 localPosition;

	// Token: 0x040072AF RID: 29359
	public float weight;
}
