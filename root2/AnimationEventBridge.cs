using System;
using Audio;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class AnimationEventBridge : RootTransformRefEntity
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000356 RID: 854 RVA: 0x0001882C File Offset: 0x00016A2C
	public EntityAlive entity
	{
		get
		{
			if (!this._entity && this.RootTransform)
			{
				this._entity = this.RootTransform.GetComponent<EntityAlive>();
			}
			return this._entity;
		}
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0001885F File Offset: 0x00016A5F
	public void PlaySound(AnimationEvent ae)
	{
		this.playSound(ae.stringParameter, ae);
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00018870 File Offset: 0x00016A70
	public void playSound(string name, AnimationEvent ae = null)
	{
		if (this.entity != null)
		{
			EntityPlayer entityPlayer = this.entity as EntityPlayer;
			if (entityPlayer != null && entityPlayer.IsReloadCancelled())
			{
				return;
			}
			this.entity.PlayOneShot(name, false, true, false, ae, 1f);
		}
	}

	// Token: 0x06000359 RID: 857 RVA: 0x000188C0 File Offset: 0x00016AC0
	public void PlayWeaponSound(AnimationEvent ae)
	{
		EntityPlayer entityPlayer = this.entity as EntityPlayer;
		if (entityPlayer != null && entityPlayer.inventory.holdingItem.HoldType.Value == ae.intParameter)
		{
			this.playSound(ae.stringParameter, ae);
		}
	}

	// Token: 0x0600035A RID: 858 RVA: 0x00018906 File Offset: 0x00016B06
	public void PlayLocalSound(string name)
	{
		this.playSound(name, null);
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00018910 File Offset: 0x00016B10
	public void PlayStepSound(AnimationEvent _animEvent)
	{
		if (this.entity && _animEvent.animatorClipInfo.weight >= 0.3f)
		{
			float time = Time.time;
			if (time - this.lastTimeStepPlayed >= 0.1f)
			{
				this.lastTimeStepPlayed = time;
				float num = _animEvent.floatParameter;
				if (num == 0f)
				{
					num = 1f;
				}
				this.entity.PlayStepSound(num);
			}
		}
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0001897C File Offset: 0x00016B7C
	public void DeathImpactLight()
	{
		if (this.entity != null && this.entity.IsDead())
		{
			Manager.Play(this.entity, "impactbodylight", 1f, false);
		}
	}

	// Token: 0x0600035D RID: 861 RVA: 0x000189B0 File Offset: 0x00016BB0
	public void DeathImpactHeavy()
	{
		if (this.entity != null && this.entity.IsDead())
		{
			Manager.Play(this.entity, "impactbodyheavy", 1f, false);
		}
	}

	// Token: 0x0600035E RID: 862 RVA: 0x000189E4 File Offset: 0x00016BE4
	public void HideHoldingItem()
	{
		EntityAlive entity = this.entity;
		if (entity && entity.emodel)
		{
			Transform holdingItemTransform = entity.inventory.GetHoldingItemTransform();
			if (holdingItemTransform)
			{
				holdingItemTransform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00018A30 File Offset: 0x00016C30
	public void ShowHoldingItem()
	{
		EntityAlive entity = this.entity;
		if (entity && entity.emodel)
		{
			Transform holdingItemTransform = entity.inventory.GetHoldingItemTransform();
			if (holdingItemTransform)
			{
				holdingItemTransform.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06000360 RID: 864 RVA: 0x00018A7C File Offset: 0x00016C7C
	public void Hit()
	{
		Entity entity = this.entity;
		if (entity && entity.emodel && entity.emodel.avatarController)
		{
			entity.emodel.avatarController.SetAttackImpact();
		}
	}

	// Token: 0x06000361 RID: 865 RVA: 0x00018AC8 File Offset: 0x00016CC8
	public void ConsumeComplete()
	{
		EntityPlayerLocal entityPlayerLocal = this.entity as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			ItemClass holdingItem = entityPlayerLocal.inventory.holdingItem;
			for (int i = 0; i < holdingItem.Actions.Length; i++)
			{
				ItemActionEat itemActionEat = holdingItem.Actions[i] as ItemActionEat;
				if (itemActionEat != null && itemActionEat.PercentDone(entityPlayerLocal.inventory.holdingItemData.actionData[i]) > 0f)
				{
					itemActionEat.Completed(entityPlayerLocal.inventory.holdingItemData.actionData[i]);
					return;
				}
			}
		}
	}

	// Token: 0x06000362 RID: 866 RVA: 0x00018B5C File Offset: 0x00016D5C
	public void HeldEntityStress()
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			ItemClassHeldEntity itemClassHeldEntity = primaryPlayer.inventory.holdingItem as ItemClassHeldEntity;
			if (itemClassHeldEntity != null)
			{
				itemClassHeldEntity.PlayStressSound();
			}
		}
	}

	// Token: 0x06000363 RID: 867 RVA: 0x00018B9C File Offset: 0x00016D9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		EntityAlive entity = this.entity;
		if (entity != null && entity.emodel != null)
		{
			entity.emodel.avatarController.SetCrouching(entity.IsCrouching);
			entity.SetVehiclePoseMode(entity.vehiclePoseMode);
		}
	}

	// Token: 0x040003CB RID: 971
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive _entity;

	// Token: 0x040003CC RID: 972
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeStepPlayed;
}
