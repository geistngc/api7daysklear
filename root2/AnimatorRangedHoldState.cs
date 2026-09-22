using System;
using Audio;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public class AnimatorRangedHoldState : StateMachineBehaviour
{
	// Token: 0x06000326 RID: 806 RVA: 0x00017BE8 File Offset: 0x00015DE8
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent == null)
		{
			return;
		}
		componentInParent.emodel.avatarController.UpdateInt("CurrentAnim", 0, true);
		this.actionData = (componentInParent.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged);
		if (this.actionData != null)
		{
			this.itemClass = this.actionData.invData.itemValue.ItemClass;
			if (this.itemClass != null && this.itemClass.Properties.Values.ContainsKey(ItemClass.PropSoundIdle))
			{
				if (this.actionData.invData.itemValue.Meta > 0)
				{
					Manager.Play(this.actionData.invData.holdingEntity, this.itemClass.Properties.Values[ItemClass.PropSoundIdle], 1f, false);
					this.actionData.invData.holdingEntitySoundID = 0;
					return;
				}
				Manager.Stop(this.actionData.invData.holdingEntity.entityId, this.itemClass.Properties.Values[ItemClass.PropSoundIdle]);
				this.actionData.invData.holdingEntitySoundID = -1;
			}
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00017D33 File Offset: 0x00015F33
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.actionData = null;
		this.itemClass = null;
	}

	// Token: 0x06000328 RID: 808 RVA: 0x00017D44 File Offset: 0x00015F44
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.itemClass != null && this.actionData != null && this.actionData.invData.holdingEntity != null && this.itemClass.Properties.Values.ContainsKey(ItemClass.PropSoundIdle))
		{
			if (this.actionData.invData.holdingEntitySoundID != -1 && this.actionData.invData.itemValue.Meta == 0)
			{
				Manager.Stop(this.actionData.invData.holdingEntity.entityId, this.itemClass.Properties.Values[ItemClass.PropSoundIdle]);
				this.actionData.invData.holdingEntitySoundID = -1;
				return;
			}
			if (this.actionData.invData.holdingEntitySoundID == -1 && this.actionData.invData.itemValue.Meta > 0)
			{
				Manager.Play(this.actionData.invData.holdingEntity, this.itemClass.Properties.Values[ItemClass.PropSoundIdle], 1f, false);
				this.actionData.invData.holdingEntitySoundID = 0;
			}
		}
	}

	// Token: 0x040003C2 RID: 962
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemActionRanged.ItemActionDataRanged actionData;

	// Token: 0x040003C3 RID: 963
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemClass itemClass;
}
