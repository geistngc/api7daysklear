using System;
using UnityEngine;

// Token: 0x02000444 RID: 1092
public readonly struct AIFocusAim : IFocusTarget
{
	// Token: 0x0600215F RID: 8543 RVA: 0x000C9472 File Offset: 0x000C7672
	public AIFocusAim(Entity target, AIAimFocusOffset targetOffset)
	{
		this.Target = target;
		this.TargetOffset = targetOffset;
		this.ConditionDistance = default(AIFocusConditionDistance);
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x000C9490 File Offset: 0x000C7690
	public static bool GetActiveFocus(Entity theEntity, AIFocus<AIFocusAim> focus, out Vector3 activeFocus)
	{
		Vector3 position = theEntity.position;
		for (int i = 0; i < focus.FocusTargets.Length; i++)
		{
			Entity target = focus.FocusTargets[i].Target;
			if (target && !focus.FocusTargets[i].ConditionDistance.IsFocusDisabled(theEntity))
			{
				Vector3 vector = target.position;
				switch (focus.FocusTargets[i].TargetOffset)
				{
				case AIAimFocusOffset.Belly:
					vector = target.getBellyPosition();
					goto IL_A7;
				case AIAimFocusOffset.Chest:
					vector = target.getChestPosition();
					goto IL_A7;
				case AIAimFocusOffset.Head:
					vector = target.getHeadPosition();
					goto IL_A7;
				}
				vector = focus.FocusTargets[i].Target.position;
				IL_A7:
				activeFocus = vector;
				return true;
			}
		}
		activeFocus = default(Vector3);
		return false;
	}

	// Token: 0x040016F7 RID: 5879
	public readonly Entity Target;

	// Token: 0x040016F8 RID: 5880
	public readonly AIAimFocusOffset TargetOffset;

	// Token: 0x040016F9 RID: 5881
	public readonly AIFocusConditionDistance ConditionDistance;
}
