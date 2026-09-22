using System;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class AnimationStateRandomBlend : StateMachineBehaviour
{
	// Token: 0x060002F9 RID: 761 RVA: 0x00016B78 File Offset: 0x00014D78
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int integer = animator.GetInteger("RandomSelector");
		if (this.ChoiceCount > 0)
		{
			animator.SetFloat("RandomVariationQuantized", (float)(integer % this.ChoiceCount));
			return;
		}
		animator.SetFloat("RandomVariationQuantized", 0f);
	}

	// Token: 0x04000393 RID: 915
	[Tooltip("The number of options to randomly select from")]
	public int ChoiceCount = 1;
}
