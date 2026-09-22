using System;
using UnityEngine;

// Token: 0x02000442 RID: 1090
public struct AIFocusBody : IFocusTarget
{
	// Token: 0x0600215A RID: 8538 RVA: 0x000C937F File Offset: 0x000C757F
	public AIFocusBody(float target)
	{
		this.TargetYaw = target;
		this.TargetYawEntity = null;
		this.HasTargetYaw = true;
		this.ConditionDistance = default(AIFocusConditionDistance);
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x000C93A2 File Offset: 0x000C75A2
	public AIFocusBody(EntityAlive target)
	{
		this.TargetYaw = 0f;
		this.TargetYawEntity = target;
		this.HasTargetYaw = false;
		this.ConditionDistance = default(AIFocusConditionDistance);
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000C93C9 File Offset: 0x000C75C9
	public bool TryGetValue(EntityAlive theEntity, out float value)
	{
		if (this.HasTargetYaw)
		{
			value = this.TargetYaw;
			return true;
		}
		if (this.TargetYawEntity != null)
		{
			value = theEntity.YawForTarget(this.TargetYawEntity);
			return true;
		}
		value = 0f;
		return false;
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x000C9403 File Offset: 0x000C7603
	public static bool GetActiveFocusForPriority(EntityAlive theEntity, FocusPriority priority, AIFocus<AIFocusBody> focus, out float focusForPriority)
	{
		return focus.FocusTargets[(int)priority].TryGetValue(theEntity, out focusForPriority);
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x000C9418 File Offset: 0x000C7618
	public static bool GetActiveFocus(EntityAlive theEntity, AIFocus<AIFocusBody> focus, out float activeFocus)
	{
		Vector3 position = theEntity.position;
		for (int i = 0; i < focus.FocusTargets.Length; i++)
		{
			ref AIFocusBody ptr = ref focus.FocusTargets[i];
			float num;
			if (ptr.TryGetValue(theEntity, out num) && !ptr.ConditionDistance.IsFocusDisabled(theEntity))
			{
				activeFocus = num;
				return true;
			}
		}
		activeFocus = 0f;
		return false;
	}

	// Token: 0x040016EE RID: 5870
	public readonly EntityAlive TargetYawEntity;

	// Token: 0x040016EF RID: 5871
	public readonly float TargetYaw;

	// Token: 0x040016F0 RID: 5872
	public readonly bool HasTargetYaw;

	// Token: 0x040016F1 RID: 5873
	public AIFocusConditionDistance ConditionDistance;
}
