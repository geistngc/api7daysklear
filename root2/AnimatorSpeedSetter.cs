using System;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class AnimatorSpeedSetter : StateMachineBehaviour
{
	// Token: 0x06000013 RID: 19 RVA: 0x000027D0 File Offset: 0x000009D0
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.applyOnStateEnter)
		{
			animator.speed = this.animatorSpeed;
		}
	}

	// Token: 0x06000014 RID: 20 RVA: 0x000027E6 File Offset: 0x000009E6
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.applyOnStateExit)
		{
			animator.speed = this.animatorSpeed;
		}
	}

	// Token: 0x06000015 RID: 21 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06000016 RID: 22 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06000017 RID: 23 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x0400001D RID: 29
	public bool applyOnStateEnter = true;

	// Token: 0x0400001E RID: 30
	public bool applyOnStateExit;

	// Token: 0x0400001F RID: 31
	public float animatorSpeed = 1f;
}
