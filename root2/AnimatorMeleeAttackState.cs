using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class AnimatorMeleeAttackState : StateMachineBehaviour
{
	// Token: 0x0600030F RID: 783 RVA: 0x0001753C File Offset: 0x0001573C
	public AnimatorMeleeAttackState()
	{
		AnimatorMeleeAttackState.FistHoldHash = Animator.StringToHash("fistHold");
		AnimatorMeleeAttackState.FpvFistHoldHash = Animator.StringToHash("fpvFistHold");
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00017590 File Offset: 0x00015790
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.playingImpact)
		{
			return;
		}
		this.hasFired = false;
		this.actionIndex = animator.GetInteger(AvatarController.itemActionIndexHash);
		AnimationEventBridge component = animator.GetComponent<AnimationEventBridge>();
		this.entity = component.entity;
		if (this.actionIndex < 0 || this.actionIndex >= this.entity.inventory.holdingItemData.actionData.Count || this.entity.inventory.holdingItemData.actionData[this.actionIndex] == null)
		{
			return;
		}
		AnimatorClipInfo[] array = animator.GetNextAnimatorClipInfo(layerIndex);
		if (array.Length == 0)
		{
			array = animator.GetCurrentAnimatorClipInfo(layerIndex);
			if (array.Length == 0)
			{
				return;
			}
		}
		AnimationClip clip = array[0].clip;
		float length = clip.length;
		this.attacksPerMinute = (float)((int)(60f / length));
		FastTags<TagGroup.Global> fastTags = (this.actionIndex == 0) ? ItemActionAttack.PrimaryTag : ItemActionAttack.SecondaryTag;
		ItemValue holdingItemItemValue = this.entity.inventory.holdingItemItemValue;
		ItemClass itemClass = holdingItemItemValue.ItemClass;
		if (itemClass != null)
		{
			fastTags |= itemClass.ItemTags;
		}
		this.originalMeleeAttackSpeed = EffectManager.GetValue(PassiveEffects.AttacksPerMinute, holdingItemItemValue, this.attacksPerMinute, this.entity, null, fastTags, true, true, true, true, true, 1, true, false) / 60f * length;
		animator.SetFloat("MeleeAttackSpeed", this.originalMeleeAttackSpeed);
		ItemClass holdingItem = this.entity.inventory.holdingItem;
		holdingItem.Properties.ParseFloat((this.actionIndex == 0) ? "Action0" : "Action1", "RaycastTime", ref this.RaycastTime);
		float num = -1f;
		holdingItem.Properties.ParseFloat((this.actionIndex == 0) ? "Action0" : "Action1", "ImpactDuration", ref num);
		if (num >= 0f)
		{
			this.ImpactDuration = num * this.originalMeleeAttackSpeed;
		}
		holdingItem.Properties.ParseFloat((this.actionIndex == 0) ? "Action0" : "Action1", "ImpactPlaybackSpeed", ref this.ImpactPlaybackSpeed);
		if (this.originalMeleeAttackSpeed != 0f)
		{
			this.calculatedRaycastTime = this.RaycastTime / this.originalMeleeAttackSpeed;
			this.calculatedImpactDuration = this.ImpactDuration / this.originalMeleeAttackSpeed;
			this.calculatedImpactPlaybackSpeed = this.ImpactPlaybackSpeed / this.originalMeleeAttackSpeed;
		}
		else
		{
			this.calculatedRaycastTime = 0.001f;
			this.calculatedImpactDuration = 0.001f;
			this.calculatedImpactPlaybackSpeed = 0.001f;
		}
		this.iData = (this.entity.inventory.holdingItemData.actionData[this.actionIndex] as ItemActionDynamicMelee.ItemActionDynamicMeleeData);
		if (this.iData != null)
		{
			this.iData.HasFinished = false;
		}
		GameManager.Instance.StartCoroutine(this.impactStart(animator, animator.GetNextAnimatorStateInfo(layerIndex), clip, layerIndex));
	}

	// Token: 0x06000311 RID: 785 RVA: 0x0001784A File Offset: 0x00015A4A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator impactStart(Animator animator, AnimatorStateInfo stateInfo, AnimationClip clip, int layerIndex)
	{
		yield return new WaitForSeconds(this.calculatedRaycastTime);
		if (!this.hasFired)
		{
			this.hasFired = true;
			if (this.entity != null && !this.entity.isEntityRemote && this.actionIndex >= 0)
			{
				ItemActionDynamicMelee.ItemActionDynamicMeleeData itemActionDynamicMeleeData = this.entity.inventory.holdingItemData.actionData[this.actionIndex] as ItemActionDynamicMelee.ItemActionDynamicMeleeData;
				if (itemActionDynamicMeleeData != null && itemActionDynamicMeleeData.Equals(this.iData) && (this.entity.inventory.holdingItem.Actions[this.actionIndex] as ItemActionDynamicMelee).Raycast(itemActionDynamicMeleeData))
				{
					animator.SetFloat("MeleeAttackSpeed", this.calculatedImpactPlaybackSpeed);
					this.playingImpact = true;
					GameManager.Instance.StartCoroutine(this.impactStop(animator, stateInfo, clip, layerIndex));
				}
			}
		}
		yield break;
	}

	// Token: 0x06000312 RID: 786 RVA: 0x00017876 File Offset: 0x00015A76
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator impactStop(Animator animator, AnimatorStateInfo stateInfo, AnimationClip clip, int layerIndex)
	{
		animator.Play(0, layerIndex, Mathf.Min(1f, this.calculatedRaycastTime * this.originalMeleeAttackSpeed / clip.length));
		yield return new WaitForSeconds(this.calculatedImpactDuration);
		animator.SetFloat("MeleeAttackSpeed", this.originalMeleeAttackSpeed);
		this.playingImpact = false;
		yield break;
	}

	// Token: 0x06000313 RID: 787 RVA: 0x0001789C File Offset: 0x00015A9C
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.entity != null && !this.entity.isEntityRemote && this.actionIndex >= 0)
		{
			ItemActionDynamicMelee.ItemActionDynamicMeleeData itemActionDynamicMeleeData = this.entity.inventory.holdingItemData.actionData[this.actionIndex] as ItemActionDynamicMelee.ItemActionDynamicMeleeData;
			if (itemActionDynamicMeleeData != null)
			{
				itemActionDynamicMeleeData.HasFinished = true;
				animator.SetFloat("MeleeAttackSpeed", this.originalMeleeAttackSpeed);
			}
		}
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00017910 File Offset: 0x00015B10
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		float normalizedTime = stateInfo.normalizedTime;
		if (float.IsInfinity(normalizedTime) || float.IsNaN(normalizedTime))
		{
			if (animator.HasState(layerIndex, AnimatorMeleeAttackState.FistHoldHash))
			{
				animator.Play(AnimatorMeleeAttackState.FistHoldHash, layerIndex, 1f);
				return;
			}
			if (animator.HasState(layerIndex, AnimatorMeleeAttackState.FpvFistHoldHash))
			{
				animator.Play(AnimatorMeleeAttackState.FpvFistHoldHash, layerIndex, 1f);
				return;
			}
			animator.Play(animator.GetNextAnimatorStateInfo(layerIndex).shortNameHash, layerIndex);
		}
	}

	// Token: 0x040003A6 RID: 934
	public float RaycastTime = 0.3f;

	// Token: 0x040003A7 RID: 935
	public float ImpactDuration = 0.01f;

	// Token: 0x040003A8 RID: 936
	public float ImpactPlaybackSpeed = 1f;

	// Token: 0x040003A9 RID: 937
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float calculatedRaycastTime;

	// Token: 0x040003AA RID: 938
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float calculatedImpactDuration;

	// Token: 0x040003AB RID: 939
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float calculatedImpactPlaybackSpeed;

	// Token: 0x040003AC RID: 940
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasFired;

	// Token: 0x040003AD RID: 941
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int actionIndex;

	// Token: 0x040003AE RID: 942
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float originalMeleeAttackSpeed;

	// Token: 0x040003AF RID: 943
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool playingImpact;

	// Token: 0x040003B0 RID: 944
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive entity;

	// Token: 0x040003B1 RID: 945
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float attacksPerMinute;

	// Token: 0x040003B2 RID: 946
	public static int FistHoldHash = Animator.StringToHash("fistHold");

	// Token: 0x040003B3 RID: 947
	public static int FpvFistHoldHash = Animator.StringToHash("fpvFistHold");

	// Token: 0x040003B4 RID: 948
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemActionDynamicMelee.ItemActionDynamicMeleeData iData;
}
