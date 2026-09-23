using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200043B RID: 1083
public class EAIDroneItemModStunWeapon : EAIDroneItemTask
{
	// Token: 0x06002138 RID: 8504 RVA: 0x000C84A4 File Offset: 0x000C66A4
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
	}

	// Token: 0x06002139 RID: 8505 RVA: 0x000C84AD File Offset: 0x000C66AD
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x000C87E0 File Offset: 0x000C69E0
	public override bool CanExecute()
	{
		if (!base.CanExecute())
		{
			return false;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return false;
		}
		if (this.drone.IsOnTeleportCooldown())
		{
			if (this.currentPath.Count > 0)
			{
				this.OnAttackExit();
			}
			return false;
		}
		if (!this.drone.CanAttack)
		{
			return false;
		}
		if (!this.drone.CanInterruptFollow)
		{
			return false;
		}
		EntityAlive owner = this.drone.Owner;
		if (owner && (this.drone.IsAttachedToVehicle(owner) || LockManager.Instance.IsLockedServer(this.drone, 0)))
		{
			return false;
		}
		if (this.drone.IsOwnerSneaking())
		{
			return false;
		}
		EntityAlive nearestEnemyInRange = this.drone.GetNearestEnemyInRange(owner ? owner.position : this.drone.position);
		if (!nearestEnemyInRange)
		{
			return false;
		}
		DroneWeapons.Weapon installedWeapon = this.drone.GetInstalledWeapon(this.ItemKey);
		if (installedWeapon.canFire())
		{
			this.drone.SetAttackTarget(nearestEnemyInRange, 1200);
			if (this.drone.GetAttackTarget())
			{
				this.drone.SetActiveWeapon(installedWeapon);
				this.drone.SetState(EntityDrone.State.Attack, true);
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600213B RID: 8507 RVA: 0x000C8918 File Offset: 0x000C6B18
	public override bool Continue()
	{
		if (!this.drone)
		{
			return false;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return false;
		}
		EntityAlive attackTarget = this.drone.GetAttackTarget();
		if (!attackTarget || attackTarget.IsDead())
		{
			this.OnAttackExit();
			return false;
		}
		if (this.drone.WakeupAnimTime > 0f)
		{
			return false;
		}
		if (!this.drone.CanInterruptFollow)
		{
			return false;
		}
		EntityAlive owner = this.drone.Owner;
		if (owner && (this.drone.IsAttachedToVehicle(owner) || LockManager.Instance.IsLockedServer(this.drone, 0)))
		{
			this.OnAttackExit();
			return false;
		}
		DroneWeapons.Weapon installedWeapon = this.drone.GetInstalledWeapon(this.ItemKey);
		if (installedWeapon != null && this.attackEnterTimer <= 0f)
		{
			Vector3 position = this.drone.position;
			Transform transform = this.drone.transform;
			Utils.DrawCircleLinesHorzontal(position - Origin.position, this.drone.EnemyDetectionRadius, Color.white, Color.yellow, 24, 0.05f);
			float pointRadius = 0.1f;
			float duration = 10f;
			if (!this.weaponDischarged)
			{
				Vector3 chestPosition = attackTarget.getChestPosition();
				if (base.DoMoveIntoAtkPos(attackTarget, installedWeapon.Range, transform.forward, pointRadius, true, duration) && installedWeapon.canFire())
				{
					Utils.DrawCircleLinesHorzontal(position - Origin.position, installedWeapon.Range, Color.white, Color.red, 24, 5f);
					Utils.DrawLine(position - Origin.position, chestPosition - Origin.position, Color.red, Color.red, 1, 5f);
					installedWeapon.Fire(attackTarget);
					this.weaponDischarged = true;
					base.ClearPath();
				}
			}
			else if (this.actionTimer <= 0f && base.DoFleeFromTargetEntity(attackTarget, 1.414f, pointRadius, true, duration))
			{
				this.OnAttackExit();
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x000C879D File Offset: 0x000C699D
	public override void Update()
	{
		base.Update();
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x000C87A5 File Offset: 0x000C69A5
	public override void Reset()
	{
		base.Reset();
		this.OnAttackExit();
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x000C87B3 File Offset: 0x000C69B3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnAttackEnter()
	{
		base.OnAttackEnter();
	}

	// Token: 0x0600213F RID: 8511 RVA: 0x000C87BB File Offset: 0x000C69BB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnAttackAction()
	{
		base.OnAttackAction();
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x000C87C3 File Offset: 0x000C69C3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnAttackExit()
	{
		base.OnAttackExit();
	}
}
