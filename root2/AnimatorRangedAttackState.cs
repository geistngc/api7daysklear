using System;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public class AnimatorRangedAttackState : StateMachineBehaviour
{
	// Token: 0x06000322 RID: 802 RVA: 0x00017BB0 File Offset: 0x00015DB0
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent == null)
		{
			return;
		}
		componentInParent.emodel.avatarController.UpdateInt("CurrentAnim", 2, true);
	}

	// Token: 0x06000323 RID: 803 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06000324 RID: 804 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
