using System;
using System.Collections.Generic;
using GamePath;
using RaycastPathing;
using UnityEngine;

// Token: 0x0200043C RID: 1084
public class EAIDroneItemTask : EAIItemTask
{
	// Token: 0x06002142 RID: 8514 RVA: 0x000C8B17 File Offset: 0x000C6D17
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.drone = (_theEntity as EntityDrone);
	}

	// Token: 0x06002143 RID: 8515 RVA: 0x000C8B2C File Offset: 0x000C6D2C
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		base.GetData(data, "enterTime", ref this.attackEnterTime);
		base.GetData(data, "actionTime", ref this.actionTime);
		base.GetData(data, "exitTime", ref this.attackExitTime);
		this.attackEnterTimer = this.attackEnterTime;
		base.GetData(data, "fleeDist", ref this.fleeDist);
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x000C8B94 File Offset: 0x000C6D94
	public override bool CanExecute()
	{
		return this.drone && PathFinderThread.Instance != null && base.CanExecute();
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x000C8BB4 File Offset: 0x000C6DB4
	public override bool Continue()
	{
		return this.drone && base.Continue();
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x000C8BCC File Offset: 0x000C6DCC
	public override void Update()
	{
		if (!this.drone)
		{
			return;
		}
		base.Update();
		if (this.attackEnterTimer > 0f)
		{
			this.attackEnterTimer -= 0.05f;
			if (this.attackEnterTimer <= 0f)
			{
				this.OnAttackEnter();
			}
		}
		if (this.actionTimer > 0f)
		{
			this.actionTimer -= 0.05f;
			if (this.actionTimer <= 0f)
			{
				this.OnAttackAction();
			}
		}
		if (this.attackExitTimer > 0f)
		{
			this.attackExitTimer -= 0.05f;
			if (this.attackExitTimer <= 0f)
			{
				this.OnAttackExit();
			}
		}
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x000C8C83 File Offset: 0x000C6E83
	public override void Reset()
	{
		base.Reset();
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x000C8C8B File Offset: 0x000C6E8B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnAttackEnter()
	{
		this.actionTimer = this.actionTime;
		this.drone.GetInstalledWeapon(this.ItemKey).PlayBark();
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000C8CAF File Offset: 0x000C6EAF
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnAttackAction()
	{
		this.attackExitTimer = this.attackExitTime;
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x000C8CC0 File Offset: 0x000C6EC0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnAttackExit()
	{
		this.attackEnterTimer = this.attackEnterTime;
		this.actionTimer = 0f;
		this.attackExitTimer = 0f;
		this.drone.SetRevengeTarget(null);
		this.drone.SetAttackTarget(null, 0);
		this.weaponDischarged = false;
		this.ClearPath();
		this.drone.SetState((this.drone.OrderState == EntityDrone.Orders.Stay) ? EntityDrone.State.Sentry : EntityDrone.State.Idle, true);
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x000C8D34 File Offset: 0x000C6F34
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool GetPath(Vector3 seekPos, float seekDist, float pointRadius, bool debugDraw = false, float duration = 0f)
	{
		return EntityDrone.GetPath(this.currentPath, this.drone, this.drone.position, seekPos, this.drone.SpeedFlying, this, seekDist, pointRadius, debugDraw, duration);
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x000C8D75 File Offset: 0x000C6F75
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3[] GetGroupPositions(EntityAlive target, float dist, bool debugDraw = false, float duration = 0f)
	{
		return EntityDrone.GetGroupPositions(target, dist, debugDraw, duration);
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x000C8D81 File Offset: 0x000C6F81
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool IsPositionBlocked(Vector3 start, Vector3 end, int layerMask = 1073807360, bool debugDraw = false, float duration = 0f)
	{
		return EntityDrone.IsPositionBlocked(start, end, layerMask, debugDraw, 0f);
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x000C8D92 File Offset: 0x000C6F92
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ClearPath()
	{
		this.currentPath.Clear();
		this.drone.OnPathInterupted();
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x000C8DAC File Offset: 0x000C6FAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void FollowPlannedPath(float speed, float pointRadius = 0.1f, bool debugDraw = false, float duration = 0f)
	{
		if (this.currentPath.Count > 0)
		{
			this.currentPathTarget = this.currentPath[0];
			this.drone.RotateTo((this.currentPathTarget - this.drone.position).normalized);
			this.drone.Move(this.currentPathTarget, pointRadius);
			if (this.drone.IsInRange(this.currentPathTarget, pointRadius))
			{
				this.currentPath.RemoveAt(0);
				return;
			}
			if (debugDraw && this.currentPath.Count > 1)
			{
				RaycastPathUtils.DrawLine(this.currentPath[0], this.currentPath[1], Color.green, 1f);
			}
			if (this.pathTracker.IsStuck(this.drone.position, this.currentPath[0], 0.5f, false))
			{
				if (this.currentPath.Count > 1)
				{
					this.drone.TeleportToPosition(this.currentPath[1]);
					this.currentPath.RemoveRange(0, 2);
					return;
				}
				this.drone.TeleportToPosition(this.currentPath[0]);
				this.currentPath.RemoveAt(0);
				return;
			}
		}
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x000C8EF4 File Offset: 0x000C70F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool DoMoveIntoAtkPos(EntityAlive avaliableTarget, float seekDist, Vector3 seekForward, float pointRadius, bool debugDraw = false, float duration = 0f)
	{
		Vector3 position = this.drone.position;
		Vector3 chestPosition = avaliableTarget.getChestPosition();
		Vector3 normalized = (chestPosition - position).normalized;
		bool flag = Vector3.Dot(seekForward, normalized) >= 0.9f;
		float magnitude = (chestPosition - position).magnitude;
		Utils.DrawCircleLinesHorzontal(position - Origin.position, seekDist, Color.white, Color.red, 24, 0.05f);
		if (this.currentPath.Count == 0)
		{
			Vector3[] groupPositions = this.GetGroupPositions(avaliableTarget, 1.414f, false, 0f);
			Array.Sort<Vector3>(groupPositions, (Vector3 x, Vector3 y) => Vector3.Distance(position, x).CompareTo(Vector3.Distance(position, y)));
			Vector3 seekPos = groupPositions[0];
			this.GetPath(seekPos, seekDist, pointRadius, debugDraw, duration);
			if (this.currentPath.Count > 0)
			{
				this.currentPathDest = this.currentPath[this.currentPath.Count - 1];
			}
			else
			{
				this.currentPathDest = seekPos;
			}
		}
		else
		{
			if ((chestPosition - this.currentPathDest).magnitude > seekDist)
			{
				this.ClearPath();
			}
			if (this.IsPositionBlocked(position, chestPosition, 1073807360, false, 0f) || (float)this.currentPath.Count > seekDist + 1f || (this.currentPath.Count > 0 && magnitude > seekDist + 1.414f))
			{
				this.FollowPlannedPath(this.drone.SpeedFlying, pointRadius, false, 0f);
			}
			else if (magnitude >= seekDist)
			{
				this.drone.RotateTo(normalized);
				this.drone.Move(chestPosition, pointRadius);
			}
		}
		if (!this.IsPositionBlocked(position, chestPosition, 1073807360, false, 0f) && magnitude <= seekDist)
		{
			this.drone.RotateTo(normalized);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x000C90E0 File Offset: 0x000C72E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool DoFleeFromTargetEntity(EntityAlive avaliableTarget, float seekDist, float pointRadius, bool debugDraw = false, float duration = 0f)
	{
		Vector3 position = this.drone.position;
		avaliableTarget.getChestPosition();
		if (this.currentPath.Count == 0)
		{
			Vector3[] groupPositions = this.GetGroupPositions(avaliableTarget, (this.fleeDist > 0f) ? this.fleeDist : 10f, false, 0f);
			Vector3 seekPos = groupPositions[this.drone.rand.RandomRange(0, groupPositions.Length)];
			this.GetPath(seekPos, seekDist, pointRadius, debugDraw, duration);
			if (this.currentPath.Count > 0)
			{
				this.currentPathDest = this.currentPath[this.currentPath.Count - 1];
			}
			else
			{
				this.currentPathDest = seekPos;
			}
		}
		else
		{
			bool flag = !EntityDrone.IsPositionBlocked(position, this.currentPathDest, 1073807360, false, 0f);
			float magnitude = (this.currentPathDest - position).magnitude;
			if (magnitude > 5f || !flag)
			{
				this.FollowPlannedPath(this.drone.SpeedFlying, pointRadius, false, 0f);
			}
			else
			{
				this.drone.RotateTo((this.currentPathDest - position).normalized);
				this.drone.Move(this.currentPathDest, pointRadius);
			}
			if (magnitude <= seekDist)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040016D6 RID: 5846
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityDrone drone;

	// Token: 0x040016D7 RID: 5847
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<Vector3> currentPath = new List<Vector3>();

	// Token: 0x040016D8 RID: 5848
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 currentPathTarget;

	// Token: 0x040016D9 RID: 5849
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 currentPathDest;

	// Token: 0x040016DA RID: 5850
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool weaponDischarged;

	// Token: 0x040016DB RID: 5851
	[PublicizedFrom(EAccessModifier.Protected)]
	public float attackEnterTime;

	// Token: 0x040016DC RID: 5852
	[PublicizedFrom(EAccessModifier.Protected)]
	public float actionTime;

	// Token: 0x040016DD RID: 5853
	[PublicizedFrom(EAccessModifier.Protected)]
	public float attackExitTime;

	// Token: 0x040016DE RID: 5854
	[PublicizedFrom(EAccessModifier.Protected)]
	public float attackEnterTimer;

	// Token: 0x040016DF RID: 5855
	[PublicizedFrom(EAccessModifier.Protected)]
	public float actionTimer;

	// Token: 0x040016E0 RID: 5856
	[PublicizedFrom(EAccessModifier.Protected)]
	public float attackExitTimer;

	// Token: 0x040016E1 RID: 5857
	[PublicizedFrom(EAccessModifier.Private)]
	public float fleeDist;

	// Token: 0x040016E2 RID: 5858
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityDrone.PathTracker pathTracker = new EntityDrone.PathTracker();
}
