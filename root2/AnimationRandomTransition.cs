using System;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class AnimationRandomTransition : StateMachineBehaviour
{
	// Token: 0x060002F4 RID: 756 RVA: 0x00016ABC File Offset: 0x00014CBC
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.numberOfAnimations > 0)
		{
			int value = UnityEngine.Random.Range(0, this.numberOfAnimations);
			animator.SetInteger(this.animationParameter, value);
		}
	}

	// Token: 0x0400038F RID: 911
	public string animationParameter = "RandomIndex";

	// Token: 0x04000390 RID: 912
	public int numberOfAnimations;
}
