using System;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200042E RID: 1070
[Preserve]
public class EAIApproachDistraction : EAIBase
{
	// Token: 0x060020E1 RID: 8417 RVA: 0x000C6CAD File Offset: 0x000C4EAD
	public EAIApproachDistraction()
	{
		this.MutexBits = 3;
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000C6CBC File Offset: 0x000C4EBC
	public override bool CanExecute()
	{
		EntityItem pendingDistraction = this.theEntity.pendingDistraction;
		if (!pendingDistraction || pendingDistraction.itemClass == null)
		{
			return false;
		}
		if (this.theEntity.GetAttackTarget())
		{
			if (!pendingDistraction.itemClass.IsEatDistraction)
			{
				this.theEntity.pendingDistraction = null;
			}
			return false;
		}
		if ((this.theEntity.position - pendingDistraction.position).sqrMagnitude < 2.25f && !pendingDistraction.itemClass.IsEatDistraction)
		{
			this.theEntity.pendingDistraction = null;
			return false;
		}
		return true;
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000C6D58 File Offset: 0x000C4F58
	public override void Start()
	{
		this.theEntity.SetAttackTarget(null, 0);
		this.theEntity.IsEating = false;
		this.theEntity.distraction = this.theEntity.pendingDistraction;
		this.theEntity.pendingDistraction = null;
		this.hadPath = false;
		this.updatePath();
	}

	// Token: 0x060020E4 RID: 8420 RVA: 0x000C6DB0 File Offset: 0x000C4FB0
	public override bool Continue()
	{
		PathEntity path = this.theEntity.navigator.getPath();
		if (this.hadPath && path == null)
		{
			return false;
		}
		EntityItem distraction = this.theEntity.distraction;
		return !(distraction == null) && distraction.itemClass != null && (((this.theEntity.position - distraction.position).sqrMagnitude > 2.25f && (path == null || !path.isFinished())) || (distraction.itemClass.IsEatDistraction && distraction.IsDistractionActive));
	}

	// Token: 0x060020E5 RID: 8421 RVA: 0x000C6E44 File Offset: 0x000C5044
	public override void Update()
	{
		EntityItem distraction = this.theEntity.distraction;
		if (!distraction)
		{
			return;
		}
		PathEntity path = this.theEntity.getNavigator().getPath();
		if (path != null)
		{
			this.hadPath = true;
		}
		bool flag = false;
		if (path != null && !path.isFinished() && !this.theEntity.isCollidedHorizontally)
		{
			flag = true;
		}
		if (this.theEntity.IsSwimming())
		{
			flag = true;
		}
		if (Mathf.Abs(this.theEntity.speedForward) > 0.01f || Mathf.Abs(this.theEntity.speedStrafe) > 0.01f)
		{
			flag = true;
		}
		if (flag)
		{
			this.theEntity.SetLookPosition(distraction.position);
		}
		if ((this.theEntity.GetPosition() - distraction.position).sqrMagnitude <= 2.25f)
		{
			this.theEntity.IsEating = true;
			distraction.distractionEatTicks--;
			return;
		}
		int num = this.pathRecalculateTicks - 1;
		this.pathRecalculateTicks = num;
		if (num <= 0)
		{
			this.updatePath();
		}
	}

	// Token: 0x060020E6 RID: 8422 RVA: 0x000C6F50 File Offset: 0x000C5150
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePath()
	{
		if (PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
		{
			return;
		}
		this.pathRecalculateTicks = 20 + base.GetRandom(20);
		this.theEntity.FindPath(this.theEntity.distraction.position, this.theEntity.GetMoveSpeedAggro(), true, this);
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x000C6FB0 File Offset: 0x000C51B0
	public override void Reset()
	{
		this.theEntity.moveHelper.Stop();
		this.theEntity.SetLookPosition(Vector3.zero);
		this.theEntity.IsEating = false;
		this.theEntity.distraction = null;
		this.manager.lookTime = 2f;
	}

	// Token: 0x0400168D RID: 5773
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCloseDist = 1.5f;

	// Token: 0x0400168E RID: 5774
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cLookTime = 2f;

	// Token: 0x0400168F RID: 5775
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hadPath;

	// Token: 0x04001690 RID: 5776
	[PublicizedFrom(EAccessModifier.Private)]
	public int pathRecalculateTicks;
}
