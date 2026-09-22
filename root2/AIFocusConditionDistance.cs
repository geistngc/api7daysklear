using System;
using UnityEngine;

// Token: 0x02000440 RID: 1088
public readonly struct AIFocusConditionDistance
{
	// Token: 0x06002156 RID: 8534 RVA: 0x000C9279 File Offset: 0x000C7479
	public AIFocusConditionDistance(float distance, EntityAlive entityTransform)
	{
		this.ConditionalDistanceSq = distance * distance;
		this.EntityTransform = entityTransform;
		this.DistanceTransform = null;
		this.DistancePosition = Vector3.zero;
		this.HasDistancePosition = false;
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x000C92A4 File Offset: 0x000C74A4
	public AIFocusConditionDistance(float distance, Transform distanceTransform)
	{
		this.ConditionalDistanceSq = distance * distance;
		this.EntityTransform = null;
		this.DistanceTransform = distanceTransform;
		this.DistancePosition = Vector3.zero;
		this.HasDistancePosition = false;
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x000C92CF File Offset: 0x000C74CF
	public AIFocusConditionDistance(float distance, Vector3 distancePosition)
	{
		this.ConditionalDistanceSq = distance * distance;
		this.EntityTransform = null;
		this.DistanceTransform = null;
		this.DistancePosition = distancePosition;
		this.HasDistancePosition = true;
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x000C92F8 File Offset: 0x000C74F8
	public bool IsFocusDisabled(Entity theEntity)
	{
		if (this.ConditionalDistanceSq > 0f)
		{
			Vector3 a = Vector3.zero;
			if (this.HasDistancePosition)
			{
				a = this.DistancePosition;
			}
			else if (this.EntityTransform)
			{
				a = this.EntityTransform.position;
			}
			else
			{
				if (!this.DistanceTransform)
				{
					return false;
				}
				a = this.DistanceTransform.position;
			}
			Vector3 position = theEntity.position;
			if (Vector3.SqrMagnitude(a - position) > this.ConditionalDistanceSq)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040016E9 RID: 5865
	public readonly float ConditionalDistanceSq;

	// Token: 0x040016EA RID: 5866
	public readonly EntityAlive EntityTransform;

	// Token: 0x040016EB RID: 5867
	public readonly Transform DistanceTransform;

	// Token: 0x040016EC RID: 5868
	public readonly Vector3 DistancePosition;

	// Token: 0x040016ED RID: 5869
	public readonly bool HasDistancePosition;
}
