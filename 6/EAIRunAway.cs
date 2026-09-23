using System;
using System.Collections.Generic;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000450 RID: 1104
[Preserve]
public abstract class EAIRunAway : EAIBase
{
	// Token: 0x060021AF RID: 8623 RVA: 0x000CBD2F File Offset: 0x000C9F2F
	public EAIRunAway()
	{
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x000CBD3F File Offset: 0x000C9F3F
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		base.GetData(data, "fleeDistance", ref this.fleeDistance);
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x000CBD5A File Offset: 0x000C9F5A
	public override bool CanExecute()
	{
		return this.FindFleePos(this.GetFleeFromPos());
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x000CBD68 File Offset: 0x000C9F68
	public override void Start()
	{
		this.timeoutTicks = (30 + base.GetRandom(20)) * 20;
		PathFinderThread.Instance.RemovePathsFor(this.theEntity.entityId);
		string soundAlert = this.theEntity.GetSoundAlert();
		if (soundAlert != null)
		{
			this.theEntity.PlayOneShot(soundAlert, false, false, false, null, 1f);
		}
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000CBDC2 File Offset: 0x000C9FC2
	public override bool Continue()
	{
		return this.timeoutTicks > 0;
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x000CBDD0 File Offset: 0x000C9FD0
	public override void Update()
	{
		this.timeoutTicks--;
		PathEntity path = this.theEntity.navigator.getPath();
		if (!this.checkedPath && !PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
		{
			this.checkedPath = true;
			if (path != null)
			{
				Vector3 rawEndPos = path.rawEndPos;
				if (this.theEntity.GetDistanceSq(rawEndPos) < 1.21f)
				{
					this.FindRandomPos();
				}
			}
		}
		if (this.checkedPath && path != null && path.getCurrentPathLength() >= 2 && path.NodeCountRemaining() <= 2)
		{
			this.fleeTicks = 0;
		}
		int num = this.fleeTicks - 1;
		this.fleeTicks = num;
		if (num <= 0)
		{
			Vector3 fleeFromPos = this.GetFleeFromPos();
			this.FindFleePos(fleeFromPos);
		}
		num = this.pathTicks - 1;
		this.pathTicks = num;
		if (num <= 0 && !PathFinderThread.Instance.IsCalculatingPath(this.theEntity.entityId))
		{
			this.pathTicks = 60;
			this.theEntity.FindPath(this.targetPos, this.theEntity.GetMoveSpeed(), false, this);
			this.checkedPath = false;
		}
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x000CBEE5 File Offset: 0x000CA0E5
	public override void Reset()
	{
		this.enemy = null;
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000CBEF0 File Offset: 0x000CA0F0
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool FindFleePos(Vector3 fleeFromPos)
	{
		Vector3 dirV = this.theEntity.position - fleeFromPos;
		Vector3 vector = RandomPositionGenerator.CalcPositionInDirection(this.theEntity, this.theEntity.position, dirV, (float)this.fleeDistance, 80f);
		if (vector.y == 0f)
		{
			return false;
		}
		this.targetPos = vector;
		this.fleeTicks = 80;
		this.pathTicks = 0;
		this.checkedPath = false;
		return true;
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x000CBF60 File Offset: 0x000CA160
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool FindRandomPos()
	{
		Vector3 vector = RandomPositionGenerator.CalcAround(this.theEntity, this.fleeDistance, 0);
		if (vector.y == 0f)
		{
			this.fleeTicks = 0;
			return false;
		}
		this.targetPos = vector;
		this.fleeTicks = 80;
		this.pathTicks = 0;
		this.checkedPath = false;
		return true;
	}

	// Token: 0x060021B8 RID: 8632
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract Vector3 GetFleeFromPos();

	// Token: 0x060021B9 RID: 8633 RVA: 0x000CBFB4 File Offset: 0x000CA1B4
	public override string ToString()
	{
		return string.Format("{0}, flee {1}, timeout {2}, {3}", new object[]
		{
			base.ToString(),
			this.fleeTicks,
			this.timeoutTicks,
			(this.enemy != null) ? this.enemy.GetDebugName() : ""
		});
	}

	// Token: 0x04001748 RID: 5960
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityAlive enemy;

	// Token: 0x04001749 RID: 5961
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 targetPos;

	// Token: 0x0400174A RID: 5962
	[PublicizedFrom(EAccessModifier.Private)]
	public int timeoutTicks;

	// Token: 0x0400174B RID: 5963
	[PublicizedFrom(EAccessModifier.Private)]
	public int fleeTicks;

	// Token: 0x0400174C RID: 5964
	[PublicizedFrom(EAccessModifier.Private)]
	public int pathTicks;

	// Token: 0x0400174D RID: 5965
	[PublicizedFrom(EAccessModifier.Private)]
	public bool checkedPath;

	// Token: 0x0400174E RID: 5966
	[PublicizedFrom(EAccessModifier.Private)]
	public int fleeDistance = 20;
}
