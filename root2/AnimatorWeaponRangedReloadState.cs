using System;
using Audio;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class AnimatorWeaponRangedReloadState : StateMachineBehaviour
{
	// Token: 0x0600034A RID: 842 RVA: 0x00018624 File Offset: 0x00016824
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetBool("Reload", false);
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		this.actionData = ((componentInParent != null) ? (componentInParent.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged) : null);
		this.actionData.isWeaponReloading = true;
		this.actionData.wasWeaponReloadCancelled = false;
		if (this.actionData.invData.item.Properties.GetValue(ItemClass.PropSoundIdle) != null)
		{
			Manager.Stop(this.actionData.invData.holdingEntity.entityId, this.actionData.invData.item.Properties.GetValue(ItemClass.PropSoundIdle));
		}
	}

	// Token: 0x0600034B RID: 843 RVA: 0x000186E4 File Offset: 0x000168E4
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.actionData != null && this.actionData.isWeaponReloadCancelled && !this.actionData.wasWeaponReloadCancelled)
		{
			this.actionData.wasWeaponReloadCancelled = true;
			animator.Play(0, -1, 1f);
			animator.Update(0f);
		}
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00018737 File Offset: 0x00016937
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.actionData.isWeaponReloading = false;
		this.actionData.isWeaponReloadCancelled = false;
		animator.speed = 1f;
	}

	// Token: 0x0600034D RID: 845 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x0600034E RID: 846 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x0600034F RID: 847 RVA: 0x0001875C File Offset: 0x0001695C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (this.actionData != null)
		{
			this.actionData.isWeaponReloading = false;
			this.actionData.isWeaponReloadCancelled = false;
		}
	}

	// Token: 0x040003C7 RID: 967
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemActionRanged.ItemActionDataRanged actionData;
}
