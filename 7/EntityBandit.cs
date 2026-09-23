using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200048A RID: 1162
[Preserve]
public class EntityBandit : EntityHuman
{
	// Token: 0x0600241C RID: 9244 RVA: 0x000DAD98 File Offset: 0x000D8F98
	public override void PostInit()
	{
		string @string = EntityClass.list[this.entityClass].Properties.GetString(EntityClass.PropHandItem);
		if (@string.Length > 0)
		{
			base.SetInventorySlots(@string);
		}
		if (this.inventory.GetItem(0).IsEmpty())
		{
			ItemValue bareHandItemValue = this.inventory.GetBareHandItemValue();
			bareHandItemValue.Quality = (ushort)this.rand.RandomRange(1, 3);
			bareHandItemValue.UseTimes = (float)bareHandItemValue.MaxUseTimes * 0.7f - 1f;
			this.inventory.SetItem(0, bareHandItemValue, 1, true);
		}
		int num = 1;
		if (!this.inventory.GetItem(num).IsEmpty())
		{
			this.inventory.SetHoldingItemIdx(num);
		}
		if (this.moveHelper != null)
		{
			this.moveHelper.CanOpenDoors = true;
		}
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x000DAE65 File Offset: 0x000D9065
	public override void OnAddedToWorld()
	{
		base.OnAddedToWorld();
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000DAE6D File Offset: 0x000D906D
	public override void SetupHandItem()
	{
		this.ShowHoldingItem(true);
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x000DAE78 File Offset: 0x000D9078
	public override bool UseHoldingItem(int _actionIndex, bool _isReleased)
	{
		if (!_isReleased)
		{
			ItemActionAttackData itemActionAttackData = this.inventory.holdingItemData.actionData[0] as ItemActionAttackData;
			if (itemActionAttackData != null)
			{
				ItemValue itemValue = itemActionAttackData.invData.itemValue;
				itemValue.UseTimes = (float)itemValue.MaxUseTimes * 0.8f - 1f;
				if (itemActionAttackData is ItemActionRanged.ItemActionDataRanged)
				{
					itemValue.Meta = 2;
				}
			}
		}
		return base.UseHoldingItem(_actionIndex, _isReleased);
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x000DAEE4 File Offset: 0x000D90E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTasks()
	{
		base.updateTasks();
		Vector3 lookPosition;
		if (AIFocusAim.GetActiveFocus(this, this.focusAim, out lookPosition))
		{
			base.SetLookPosition(lookPosition);
		}
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x000DAF0E File Offset: 0x000D910E
	public override bool GetAimTarget(out Vector3 aimTarget)
	{
		return AIFocusAim.GetActiveFocus(this, this.focusAim, out aimTarget) || base.GetAimTarget(out aimTarget);
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x000DAF28 File Offset: 0x000D9128
	public override bool GetHeadLookTarget(out Vector3 lookTarget)
	{
		return AIFocusAim.GetActiveFocus(this, this.focusAim, out lookTarget) || base.GetHeadLookTarget(out lookTarget);
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x000DAF44 File Offset: 0x000D9144
	public override bool CalcStrafeYawOffset(float _moveX, float _moveZ, ref float _desiredyaw, ref float _yawOffset)
	{
		float num;
		if (AIFocusBody.GetActiveFocus(this, this.focusBody, out num))
		{
			float num2 = Mathf.Atan2(_moveX, _moveZ) * 57.29578f;
			_desiredyaw = num;
			_yawOffset = MathUtils.NormalizeAxis(num2 - _desiredyaw);
			return true;
		}
		return false;
	}

	// Token: 0x04001997 RID: 6551
	public readonly AIFocus<AIFocusBody> focusBody = new AIFocus<AIFocusBody>(true);

	// Token: 0x04001998 RID: 6552
	public readonly AIFocus<AIFocusAim> focusAim = new AIFocus<AIFocusAim>(true);

	// Token: 0x0200048B RID: 1163
	[PublicizedFrom(EAccessModifier.Private)]
	public enum InvSlots
	{
		// Token: 0x0400199A RID: 6554
		Melee,
		// Token: 0x0400199B RID: 6555
		Ranged,
		// Token: 0x0400199C RID: 6556
		Thrown,
		// Token: 0x0400199D RID: 6557
		Heal
	}
}
