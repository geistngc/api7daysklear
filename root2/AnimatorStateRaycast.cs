using System;
using UnityEngine;

// Token: 0x020000A6 RID: 166
[SharedBetweenAnimators]
public class AnimatorStateRaycast : StateMachineBehaviour
{
	// Token: 0x06000338 RID: 824 RVA: 0x00018510 File Offset: 0x00016710
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent != null && !componentInParent.isEntityRemote)
		{
			int integer = animator.GetInteger(AvatarController.itemActionIndexHash);
			if (integer >= 0 && integer < componentInParent.inventory.holdingItem.Actions.Length && componentInParent.inventory.holdingItemData.actionData[integer] is ItemActionDynamicMelee.ItemActionDynamicMeleeData)
			{
				(componentInParent.inventory.holdingItem.Actions[integer] as ItemActionDynamicMelee).Raycast(componentInParent.inventory.holdingItemData.actionData[integer] as ItemActionDynamic.ItemActionDynamicData);
			}
		}
	}
}
