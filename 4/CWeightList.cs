using System;
using System.Collections;
using UnityEngine;

// Token: 0x020012CA RID: 4810
[PublicizedFrom(EAccessModifier.Internal)]
public class CWeightList
{
	// Token: 0x0600987B RID: 39035 RVA: 0x0039EC70 File Offset: 0x0039CE70
	public CWeightList()
	{
		this.weights = new ArrayList();
	}

	// Token: 0x040072B0 RID: 29360
	public Transform transform;

	// Token: 0x040072B1 RID: 29361
	public ArrayList weights;
}
