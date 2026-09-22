using System;
using Audio;
using UnityEngine;

// Token: 0x0200009A RID: 154
public class Animator3PRangedReloadState : StateMachineBehaviour
{
	// Token: 0x060002FC RID: 764 RVA: 0x00016BD8 File Offset: 0x00014DD8
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent == null)
		{
			return;
		}
		this.actionData = (componentInParent.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged);
		if (this.actionData == null)
		{
			return;
		}
		this.actionRanged = (ItemActionRanged)componentInParent.inventory.holdingItem.Actions[0];
		if (this.actionData.invData.item.Properties.GetValue(ItemClass.PropSoundIdle) != null)
		{
			Manager.Stop(this.actionData.invData.holdingEntity.entityId, this.actionData.invData.item.Properties.GetValue(ItemClass.PropSoundIdle));
		}
		if (animator.GetCurrentAnimatorClipInfo(0).Length != 0 && animator.GetCurrentAnimatorClipInfo(0)[0].clip.events.Length == 0)
		{
			if (this.actionRanged.SoundReload != null)
			{
				componentInParent.PlayOneShot(this.actionRanged.SoundReload.Value, false, false, false, null, 1f);
			}
		}
		else if (animator.GetNextAnimatorClipInfo(0).Length != 0 && animator.GetNextAnimatorClipInfo(0)[0].clip.events.Length == 0 && this.actionRanged.SoundReload != null)
		{
			componentInParent.PlayOneShot(this.actionRanged.SoundReload.Value, false, false, false, null, 1f);
		}
		int num = (int)EffectManager.GetValue(PassiveEffects.MagazineSize, this.actionData.invData.itemValue, (float)this.actionRanged.BulletsPerMagazine, this.actionData.invData.holdingEntity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		ItemActionLauncher itemActionLauncher = this.actionRanged as ItemActionLauncher;
		if (itemActionLauncher != null && this.actionData.invData.itemValue.Meta < num)
		{
			ItemValue itemValue = this.actionData.invData.itemValue;
			ItemValue item = ItemClass.GetItem(this.actionRanged.MagazineItemNames[(int)itemValue.SelectedAmmoTypeIndex], false);
			ItemActionLauncher.ItemActionDataLauncher itemActionDataLauncher = this.actionData as ItemActionLauncher.ItemActionDataLauncher;
			int num2 = itemActionDataLauncher.projectileTs.Count;
			if (item != itemActionLauncher.LastProjectileType)
			{
				itemActionLauncher.DeleteProjectiles(this.actionData);
				itemActionDataLauncher.isChangingAmmoType = false;
				num2 = 0;
			}
			int num3 = 1;
			if (!this.actionData.invData.holdingEntity.isEntityRemote)
			{
				num3 = (itemActionLauncher.HasInfiniteAmmo(this.actionData) ? num : this.GetAmmoCount(this.actionData.invData.holdingEntity, item, num));
			}
			for (int i = num2; i < num3; i++)
			{
				itemActionDataLauncher.projectileTs.Add(itemActionLauncher.instantiateProjectile(this.actionData, new Vector3(0f, (float)i * 0.005f, 0f)));
			}
		}
		this.actionData.isReloading = true;
		this.actionData.isWeaponReloading = true;
		this.actionData.invData.holdingEntity.MinEventContext.ItemActionData = this.actionData;
		this.actionData.invData.holdingEntity.FireEvent(MinEventTypes.onReloadStart, true);
	}

	// Token: 0x060002FD RID: 765 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x060002FE RID: 766 RVA: 0x000027FC File Offset: 0x000009FC
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00016EFA File Offset: 0x000150FA
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetAmmoCount(EntityAlive ea, ItemValue ammo, int modifiedMagazineSize)
	{
		return Mathf.Min(ea.bag.GetItemCount(ammo, -1, -1, true) + ea.inventory.GetItemCount(ammo, false, -1, -1, true), modifiedMagazineSize);
	}

	// Token: 0x04000394 RID: 916
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float MultiProjectileOffset = 0.005f;

	// Token: 0x04000395 RID: 917
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemActionRanged.ItemActionDataRanged actionData;

	// Token: 0x04000396 RID: 918
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemActionRanged actionRanged;
}
