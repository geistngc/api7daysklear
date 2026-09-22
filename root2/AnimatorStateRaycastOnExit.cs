using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
[SharedBetweenAnimators]
public class AnimatorStateRaycastOnExit : StateMachineBehaviour
{
	// Token: 0x0600033A RID: 826 RVA: 0x000185B2 File Offset: 0x000167B2
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("WeaponFire");
		animator.ResetTrigger("PowerAttack");
	}
}
