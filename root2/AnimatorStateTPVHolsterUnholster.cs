using System;
using UnityEngine;

// Token: 0x020000A9 RID: 169
[PublicizedFrom(EAccessModifier.Internal)]
public class AnimatorStateTPVHolsterUnholster : StateMachineBehaviour
{
	// Token: 0x0600033D RID: 829 RVA: 0x000185CA File Offset: 0x000167CA
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponentInParent<EntityAlive>() != null;
	}
}
