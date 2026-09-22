using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class AnimatorThrownWeaponHoldState : StateMachineBehaviour
{
	// Token: 0x06000342 RID: 834 RVA: 0x000185DC File Offset: 0x000167DC
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityPlayerLocal componentInParent = animator.GetComponentInParent<EntityPlayerLocal>();
		if (componentInParent != null)
		{
			componentInParent.HolsterWeapon(false);
		}
	}

	// Token: 0x06000343 RID: 835 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06000344 RID: 836 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
