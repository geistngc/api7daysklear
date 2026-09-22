using System;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class AnimatorRangedSpecialAttackState : StateMachineBehaviour
{
	// Token: 0x0600032F RID: 815 RVA: 0x0001842C File Offset: 0x0001662C
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent == null)
		{
			return;
		}
		componentInParent.emodel.avatarController.UpdateInt("CurrentAnim", 1, true);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06000331 RID: 817 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
