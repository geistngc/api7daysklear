using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000458 RID: 1112
[Preserve]
public class EAISetNearestEntityAsTargetSorter : IComparer<Entity>
{
	// Token: 0x060021E9 RID: 8681 RVA: 0x000CD571 File Offset: 0x000CB771
	public EAISetNearestEntityAsTargetSorter(Entity _entity)
	{
		this.theEntity = _entity;
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x000CD580 File Offset: 0x000CB780
	public int Compare(Entity _e, Entity _e2)
	{
		float distanceSq = this.theEntity.GetDistanceSq(_e);
		float distanceSq2 = this.theEntity.GetDistanceSq(_e2);
		if (distanceSq < distanceSq2)
		{
			return -1;
		}
		if (distanceSq > distanceSq2)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x04001770 RID: 6000
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Entity theEntity;
}
