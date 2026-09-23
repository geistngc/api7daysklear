using System;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200042F RID: 1071
[Preserve]
public class EAIApproachSpot : EAIBase
{
	// Token: 0x060020E8 RID: 8424 RVA: 0x000C5B47 File Offset: 0x000C3D47
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 3;
		this.executeDelay = 0.1f;
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x000C7008 File Offset: 0x000C5208
	public override bool CanExecute()
	{
		if (!this.theEntity.HasInvestigatePosition)
		{
			return false;
		}
		if (this.theEntity.IsSleeping)
		{
			return false;
		}
		this.investigatePos = this.theEntity.InvestigatePosition;
		this.seekPos = this.theEntity.world.FindSupportingBlockPos(this.investigatePos);
		return true;
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x000C7061 File Offset: 0x000C5261
	public override void Start()
	{
		this.hadPath = false;
		this.updatePath();
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x000C7070 File Offset: 0x000C5270
	public override bool Continue()
	{
		PathEntity path = this.theEntity.navigator.getPath();
		if (this.hadPath && path == null)
		{
			return false;
		}
		int num = this.investigateTicks + 1;
		this.investigateTicks = num;
		if (num > 40)
		{
			this.investigateTicks = 0;
			if (!this.theEntity.HasInvestigatePosition)
			{
				return false;
			}
			if ((this.investigatePos - this.theEntity.InvestigatePosition).sqrMagnitude >= 4f)
			{
				return false;
			}
		}
		if ((this.seekPos - this.theEntity.position).sqrMagnitude <= 4f || (path != null && path.isFinished()))
		{
			this.theEntity.ClearInvestigatePosition();
			return false;
		}
		return true;
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x000C712C File Offset: 0x000C532C
	public override void Update()
	{
		if (this.theEntity.navigator.getPath() != null)
		{
			this.hadPath = true;
			this.theEntity.moveHelper.CalcIfUnreachablePos();
		}
		Vector3 lookPosition = this.investigatePos;
		lookPosition.y += 0.8f;
		this.theEntity.SetLookPosition(lookPosition);
		int num = this.pathRecalculateTicks - 1;
		this.pathRecalculateTicks = num;
		if (num <= 0)
		{
			this.updatePath();
		}
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x000C71A0 File Offset: 0x000C53A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePath()
	{
		if (this.theEntity.IsScoutZombie)
		{
			AstarManager.Instance.AddLocationLine(this.theEntity.position, this.seekPos, 32);
		}
		if (PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
		{
			return;
		}
		this.pathRecalculateTicks = 40 + base.GetRandom(20);
		this.theEntity.FindPath(this.seekPos, this.theEntity.GetMoveSpeedAggro(), true, this);
	}

	// Token: 0x060020EE RID: 8430 RVA: 0x000C7220 File Offset: 0x000C5420
	public override void Reset()
	{
		this.theEntity.moveHelper.Stop();
		this.theEntity.SetLookPosition(Vector3.zero);
		this.manager.lookTime = 5f + base.RandomFloat * 3f;
		this.manager.interestDistance = 2f;
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x000C727C File Offset: 0x000C547C
	public override string ToString()
	{
		return string.Format("{0}, {1} dist{2}", base.ToString(), this.theEntity.navigator.noPathAndNotPlanningOne() ? "(-path)" : (this.theEntity.navigator.noPath() ? "(!path)" : ""), (this.theEntity.position - this.seekPos).magnitude.ToCultureInvariantString());
	}

	// Token: 0x04001691 RID: 5777
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cInvestigateChangeDist = 2f;

	// Token: 0x04001692 RID: 5778
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCloseDist = 2f;

	// Token: 0x04001693 RID: 5779
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cLookTimeMin = 5f;

	// Token: 0x04001694 RID: 5780
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cLookTimeMax = 8f;

	// Token: 0x04001695 RID: 5781
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 investigatePos;

	// Token: 0x04001696 RID: 5782
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 seekPos;

	// Token: 0x04001697 RID: 5783
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hadPath;

	// Token: 0x04001698 RID: 5784
	[PublicizedFrom(EAccessModifier.Private)]
	public int investigateTicks;

	// Token: 0x04001699 RID: 5785
	[PublicizedFrom(EAccessModifier.Private)]
	public int pathRecalculateTicks;
}
