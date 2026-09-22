using System;
using UnityEngine;

// Token: 0x02000097 RID: 151
public class AnimationStateRagdoll : StateMachineBehaviour
{
	// Token: 0x060002F6 RID: 758 RVA: 0x00016B00 File Offset: 0x00014D00
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent != null)
		{
			componentInParent.BeginDynamicRagdoll(this.RagdollFlags, this.StunTime);
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00016B30 File Offset: 0x00014D30
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent != null)
		{
			componentInParent.ActivateDynamicRagdoll();
		}
	}

	// Token: 0x04000391 RID: 913
	public DynamicRagdollFlags RagdollFlags = DynamicRagdollFlags.Active | DynamicRagdollFlags.RagdollOnFall | DynamicRagdollFlags.UseBoneVelocities;

	// Token: 0x04000392 RID: 914
	[Tooltip("Time period to stun")]
	public FloatRange StunTime = new FloatRange(1f, 1f);
}
