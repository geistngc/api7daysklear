using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200043A RID: 1082
public class EAIDroneItemModHealWeapon : EAIDroneItemTask
{
	// Token: 0x0600212E RID: 8494 RVA: 0x000C84A4 File Offset: 0x000C66A4
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
	}

	// Token: 0x0600212F RID: 8495 RVA: 0x000C84AD File Offset: 0x000C66AD
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
	}

	// Token: 0x06002130 RID: 8496 RVA: 0x000C84B8 File Offset: 0x000C66B8
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
		if (this.drone.Owner && LockManager.Instance.IsLockedServer(this.drone, 0))
		{
			return false;
		}
		EntityAlive nearestHealTargetInRange = this.drone.GetNearestHealTargetInRange(this.healTargetRange);
		if (!nearestHealTargetInRange)
		{
			return false;
		}
		if (!this.drone.TargetCanBeHealed(nearestHealTargetInRange))
		{
			return false;
		}
		if (!this.drone.IsTargetInNeedOfMedical(nearestHealTargetInRange))
		{
			return false;
		}
		DroneWeapons.Weapon installedWeapon = this.drone.GetInstalledWeapon(this.ItemKey);
		if (installedWeapon.canFire())
		{
			this.drone.SetAttackTarget(nearestHealTargetInRange, 1200);
			if (this.drone.GetAttackTarget())
			{
				this.drone.SetActiveWeapon(installedWeapon);
				this.drone.SetState(EntityDrone.State.Heal, true);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002131 RID: 8497 RVA: 0x000C85CC File Offset: 0x000C67CC
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
		DroneWeapons.Weapon installedWeapon = this.drone.GetInstalledWeapon(this.ItemKey);
		EntityAlive attackTarget2 = this.drone.GetAttackTarget();
		if (attackTarget2 && attackTarget2.AttachedToEntity as EntityVehicle != null)
		{
			if (installedWeapon != null)
			{
				installedWeapon.Fire(attackTarget);
			}
			this.OnAttackExit();
			return false;
		}
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
			else if (this.actionTimer <= 0f)
			{
				this.OnAttackExit();
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x000C879D File Offset: 0x000C699D
	public override void Update()
	{
		base.Update();
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x000C87A5 File Offset: 0x000C69A5
	public override void Reset()
	{
		base.Reset();
		this.OnAttackExit();
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000C87B3 File Offset: 0x000C69B3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnAttackEnter()
	{
		base.OnAttackEnter();
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x000C87BB File Offset: 0x000C69BB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnAttackAction()
	{
		base.OnAttackAction();
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x000C87C3 File Offset: 0x000C69C3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnAttackExit()
	{
		base.OnAttackExit();
	}

	// Token: 0x040016D5 RID: 5845
	[PublicizedFrom(EAccessModifier.Private)]
	public float healTargetRange = 15f;
}
