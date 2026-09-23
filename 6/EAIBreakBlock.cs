using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000436 RID: 1078
[Preserve]
public class EAIBreakBlock : EAIBase
{
	// Token: 0x0600210E RID: 8462 RVA: 0x000C75C2 File Offset: 0x000C57C2
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 8;
		this.executeDelay = 0.15f;
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x000C75E0 File Offset: 0x000C57E0
	public override bool CanExecute()
	{
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		if (moveHelper.BlockedTime < 0.35f || !moveHelper.CanBreakBlocks)
		{
			return false;
		}
		if (this.theEntity.Jumping && !moveHelper.IsDestroyArea)
		{
			return false;
		}
		int num = (this.theEntity.crouchType == 0 && this.theEntity.physicsHeight >= 1f) ? 7 : 5;
		if ((moveHelper.BlockedFlags & num) > 0)
		{
			Vector3i blockPos = moveHelper.HitInfo.hit.blockPos;
			if (this.theEntity.world.GetBlock(blockPos).isair)
			{
				return false;
			}
			float num2 = moveHelper.CalcBlockedDistanceSq();
			float num3 = this.theEntity.m_characterController.GetRadius() + 0.7f;
			if (num2 <= num3 * num3)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002110 RID: 8464 RVA: 0x000C76C0 File Offset: 0x000C58C0
	public override void Start()
	{
		this.attackDelay = 1;
		Vector3i blockPos = this.theEntity.moveHelper.HitInfo.hit.blockPos;
		Block block = this.theEntity.world.GetBlock(blockPos).Block;
		if (block.HasTag(BlockTags.Door) || block.HasTag(BlockTags.ClosetDoor))
		{
			this.theEntity.IsBreakingDoors = true;
		}
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x000C772A File Offset: 0x000C592A
	public override bool Continue()
	{
		return (this.theEntity.onGround || this.theEntity.IsInElevator()) && this.CanExecute();
	}

	// Token: 0x06002112 RID: 8466 RVA: 0x000C774E File Offset: 0x000C594E
	public override void Update()
	{
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		if (this.attackDelay > 0)
		{
			this.attackDelay--;
		}
		if (this.attackDelay <= 0)
		{
			this.AttackBlock();
		}
	}

	// Token: 0x06002113 RID: 8467 RVA: 0x000C7782 File Offset: 0x000C5982
	public override void Reset()
	{
		this.theEntity.IsBreakingBlocks = false;
		this.theEntity.IsBreakingDoors = false;
	}

	// Token: 0x06002114 RID: 8468 RVA: 0x000C779C File Offset: 0x000C599C
	[PublicizedFrom(EAccessModifier.Private)]
	public void AttackBlock()
	{
		this.theEntity.SetLookPosition(Vector3.zero);
		ItemActionAttackData itemActionAttackData = this.theEntity.inventory.holdingItemData.actionData[0] as ItemActionAttackData;
		if (itemActionAttackData == null)
		{
			return;
		}
		this.damageBoostPercent = 0f;
		if (this.theEntity is EntityZombie)
		{
			Bounds bb = new Bounds(this.theEntity.position, new Vector3(1.7f, 1.5f, 1.7f));
			this.theEntity.world.GetEntitiesInBounds(typeof(EntityZombie), bb, this.allies);
			for (int i = this.allies.Count - 1; i >= 0; i--)
			{
				if ((EntityZombie)this.allies[i] != this.theEntity)
				{
					this.damageBoostPercent += 0.2f;
				}
			}
			this.allies.Clear();
		}
		if (this.theEntity.Attack(false))
		{
			this.theEntity.IsBreakingBlocks = true;
			float num = 0.25f + base.RandomFloat * 0.8f;
			if (this.theEntity.moveHelper.IsUnreachableAbove)
			{
				num *= 0.5f;
			}
			this.attackDelay = (int)((num + 0.75f) * 20f);
			itemActionAttackData.hitDelegate = new ItemActionAttackData.HitDelegate(this.GetHitInfo);
			this.theEntity.Attack(true);
		}
	}

	// Token: 0x06002115 RID: 8469 RVA: 0x000C7910 File Offset: 0x000C5B10
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldRayHitInfo GetHitInfo(out float damageScale)
	{
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		damageScale = moveHelper.DamageScale + this.damageBoostPercent;
		return moveHelper.HitInfo;
	}

	// Token: 0x040016AF RID: 5807
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDamageBoostPerAlly = 0.2f;

	// Token: 0x040016B0 RID: 5808
	[PublicizedFrom(EAccessModifier.Private)]
	public int attackDelay;

	// Token: 0x040016B1 RID: 5809
	[PublicizedFrom(EAccessModifier.Private)]
	public float damageBoostPercent;

	// Token: 0x040016B2 RID: 5810
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Entity> allies = new List<Entity>();
}
