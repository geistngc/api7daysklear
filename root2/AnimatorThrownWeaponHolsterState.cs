using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class AnimatorThrownWeaponHolsterState : StateMachineBehaviour
{
	// Token: 0x06000346 RID: 838 RVA: 0x00018600 File Offset: 0x00016800
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityPlayerLocal componentInParent = animator.GetComponentInParent<EntityPlayerLocal>();
		if (componentInParent != null)
		{
			componentInParent.HolsterWeapon(true);
		}
	}

	// Token: 0x06000347 RID: 839 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06000348 RID: 840 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
