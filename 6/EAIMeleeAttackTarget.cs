using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200044B RID: 1099
[Preserve]
public class EAIMeleeAttackTarget : EAIBase
{
	// Token: 0x06002194 RID: 8596 RVA: 0x000CAF3C File Offset: 0x000C913C
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 11;
		this.cooldown = 3f;
		this.attackDuration = 20f;
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x000CAF64 File Offset: 0x000C9164
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		base.GetData(data, "slot", ref this.inventorySlot);
		base.GetData(data, "itemType", ref this.itemActionType);
		base.GetData(data, "startAnimType", ref this.startAnimType);
		base.GetData(data, "releaseDelay", ref this.releaseDelay);
		base.GetData(data, "cooldown", ref this.baseCooldown);
		base.GetData(data, "duration", ref this.attackDuration);
		base.GetData(data, "minRange", ref this.minRange);
		base.GetData(data, "maxRange", ref this.maxRange);
		base.GetData(data, "unreachableRange", ref this.unreachableRange);
		data.TryGetValue("sndStart", out this.soundStartName);
		data.TryGetValue("sndRelease", out this.soundReleaseName);
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x000CB040 File Offset: 0x000C9240
	public override bool CanExecute()
	{
		if (this.theEntity.IsDancing)
		{
			return false;
		}
		if (this.cooldown > 0f)
		{
			this.cooldown -= this.executeWaitTime;
			return false;
		}
		if (!this.theEntity.IsAttackValid())
		{
			return false;
		}
		this.entityTarget = this.theEntity.GetAttackTarget();
		return this.entityTarget && !this.entityTarget.IsDead() && !this.theEntity.bodyDamage.IsAnyLegMissing && (this.startAnimType < 0 || !this.theEntity.bodyDamage.IsAnyArmOrLegMissing) && this.InRange() && this.theEntity.CanSee(this.entityTarget);
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x000CB108 File Offset: 0x000C9308
	public override void Start()
	{
		this.theEntity.emodel.avatarController.hitWeightMax = 0.5f;
		this.painHitsFelt = this.theEntity.painHitsFelt;
		this.elapsedTime = 0f;
		this.state = EAIMeleeAttackTarget.State.Attack;
		this.stateTime = 0f;
		if (this.startAnimType >= 0)
		{
			this.state = EAIMeleeAttackTarget.State.StartAnim;
			this.theEntity.StartAnimAction(this.startAnimType + 3000);
		}
		if (!string.IsNullOrEmpty(this.soundStartName))
		{
			Manager.BroadcastPlay(this.theEntity, this.soundStartName, false, 1f);
		}
		this.theEntity.inventory.SetHoldingItemIdx(this.inventorySlot);
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000CB1C0 File Offset: 0x000C93C0
	public override bool Continue()
	{
		return this.entityTarget && !this.entityTarget.IsDead() && this.elapsedTime < this.attackDuration && this.theEntity.bodyDamage.CurrentStun == EnumEntityStunType.None && !this.theEntity.Electrocuted && (this.startAnimType < 0 || !this.theEntity.bodyDamage.IsAnyArmOrLegMissing) && this.InRange() && this.theEntity.painHitsFelt - this.painHitsFelt < 1f;
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x000CB258 File Offset: 0x000C9458
	public override void Update()
	{
		this.elapsedTime += 0.05f;
		if (this.elapsedTime < this.attackDuration * 0.5f)
		{
			Vector3 headPosition = this.entityTarget.getHeadPosition();
			if (this.theEntity.IsInFrontOfMe(headPosition))
			{
				this.theEntity.SetLookPosition(headPosition);
			}
			this.theEntity.SeekYawToPos(this.entityTarget.position, 30f);
		}
		this.stateTime += 0.05f;
		if (this.state == EAIMeleeAttackTarget.State.StartAnim)
		{
			if (this.theEntity.GetAnimActionState() != AvatarController.ActionState.Ready)
			{
				return;
			}
			this.theEntity.ContinueAnimAction(this.startAnimType + 1 + 3000);
			this.state = EAIMeleeAttackTarget.State.ReleaseAnim;
			this.stateTime = 0f;
			if (!string.IsNullOrEmpty(this.soundReleaseName))
			{
				Manager.BroadcastPlay(this.theEntity, this.soundReleaseName, false, 1f);
			}
		}
		if (this.state == EAIMeleeAttackTarget.State.ReleaseAnim)
		{
			if (this.stateTime < this.releaseDelay)
			{
				return;
			}
			this.state = EAIMeleeAttackTarget.State.Attack;
			this.stateTime = 0f;
		}
		this.theEntity.UseHoldingItem(this.itemActionType, false);
		if (!this.theEntity.IsHoldingItemInUse(this.itemActionType))
		{
			this.elapsedTime = float.MaxValue;
		}
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000CB3A0 File Offset: 0x000C95A0
	public override void Reset()
	{
		this.theEntity.UseHoldingItem(this.itemActionType, true);
		this.theEntity.StartAnimAction(9999);
		this.theEntity.SetLookPosition(Vector3.zero);
		this.theEntity.emodel.avatarController.hitWeightMax = 1f;
		this.entityTarget = null;
		this.cooldown = this.baseCooldown + this.baseCooldown * 0.5f * base.RandomFloat;
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000CB424 File Offset: 0x000C9624
	[PublicizedFrom(EAccessModifier.Private)]
	public bool InRange()
	{
		float distanceSq = this.entityTarget.GetDistanceSq(this.theEntity);
		if (this.unreachableRange > 0f)
		{
			EntityMoveHelper moveHelper = this.theEntity.moveHelper;
			if (moveHelper.IsUnreachableAbove || moveHelper.IsUnreachableSide)
			{
				return distanceSq <= this.unreachableRange * this.unreachableRange;
			}
		}
		return distanceSq >= this.minRange * this.minRange && distanceSq <= this.maxRange * this.maxRange;
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000CB4A8 File Offset: 0x000C96A8
	public override string ToString()
	{
		bool flag = this.entityTarget && this.InRange();
		return string.Format("{0} {1}, inRange{2}, Time {3}", new object[]
		{
			base.ToString(),
			this.entityTarget ? this.entityTarget.EntityName : "",
			flag,
			this.elapsedTime.ToCultureInvariantString("0.00")
		});
	}

	// Token: 0x04001717 RID: 5911
	[PublicizedFrom(EAccessModifier.Private)]
	public int inventorySlot;

	// Token: 0x04001718 RID: 5912
	[PublicizedFrom(EAccessModifier.Private)]
	public int itemActionType;

	// Token: 0x04001719 RID: 5913
	[PublicizedFrom(EAccessModifier.Private)]
	public int startAnimType = -1;

	// Token: 0x0400171A RID: 5914
	[PublicizedFrom(EAccessModifier.Private)]
	public float releaseDelay = 0.5f;

	// Token: 0x0400171B RID: 5915
	[PublicizedFrom(EAccessModifier.Private)]
	public float baseCooldown;

	// Token: 0x0400171C RID: 5916
	[PublicizedFrom(EAccessModifier.Private)]
	public float cooldown;

	// Token: 0x0400171D RID: 5917
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entityTarget;

	// Token: 0x0400171E RID: 5918
	[PublicizedFrom(EAccessModifier.Private)]
	public float attackDuration;

	// Token: 0x0400171F RID: 5919
	[PublicizedFrom(EAccessModifier.Private)]
	public float elapsedTime;

	// Token: 0x04001720 RID: 5920
	[PublicizedFrom(EAccessModifier.Private)]
	public float minRange = 4f;

	// Token: 0x04001721 RID: 5921
	[PublicizedFrom(EAccessModifier.Private)]
	public float maxRange = 25f;

	// Token: 0x04001722 RID: 5922
	[PublicizedFrom(EAccessModifier.Private)]
	public float unreachableRange;

	// Token: 0x04001723 RID: 5923
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundStartName;

	// Token: 0x04001724 RID: 5924
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundReleaseName;

	// Token: 0x04001725 RID: 5925
	[PublicizedFrom(EAccessModifier.Private)]
	public EAIMeleeAttackTarget.State state;

	// Token: 0x04001726 RID: 5926
	[PublicizedFrom(EAccessModifier.Private)]
	public float stateTime;

	// Token: 0x04001727 RID: 5927
	[PublicizedFrom(EAccessModifier.Private)]
	public float painHitsFelt;

	// Token: 0x0200044C RID: 1100
	[PublicizedFrom(EAccessModifier.Private)]
	public enum State
	{
		// Token: 0x04001729 RID: 5929
		StartAnim,
		// Token: 0x0400172A RID: 5930
		ReleaseAnim,
		// Token: 0x0400172B RID: 5931
		Attack,
		// Token: 0x0400172C RID: 5932
		Release
	}
}
