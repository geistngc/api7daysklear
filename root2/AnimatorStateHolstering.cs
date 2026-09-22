using System;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class AnimatorStateHolstering : StateMachineBehaviour
{
	// Token: 0x06000335 RID: 821 RVA: 0x00018464 File Offset: 0x00016664
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent != null && componentInParent.emodel != null && componentInParent.emodel.avatarController != null)
		{
			componentInParent.emodel.avatarController.CancelEvent("WeaponFire");
			componentInParent.emodel.avatarController.CancelEvent("PowerAttack");
			componentInParent.emodel.avatarController.CancelEvent("UseItem");
			componentInParent.emodel.avatarController.UpdateBool("ItemUse", false, true);
			componentInParent.emodel.avatarController.CancelEvent("Reload");
		}
	}

	// Token: 0x06000336 RID: 822 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
