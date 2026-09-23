using System;
using System.Diagnostics;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000437 RID: 1079
[Preserve]
public class EAIDestroyArea : EAIBase
{
	// Token: 0x06002117 RID: 8471 RVA: 0x000C7951 File Offset: 0x000C5B51
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 3;
		this.executeDelay = 0.9f + base.RandomFloat * 0.6f;
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x000C797C File Offset: 0x000C5B7C
	public override bool CanExecute()
	{
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		if (!moveHelper.CanBreakBlocks)
		{
			return false;
		}
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (!attackTarget)
		{
			return false;
		}
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		bool flag = this.isLookFar;
		if (moveHelper.IsDestroyAreaTryUnreachable)
		{
			moveHelper.IsDestroyAreaTryUnreachable = false;
			float num = moveHelper.UnreachablePercent;
			if (num > 0f)
			{
				if (base.RandomFloat < num)
				{
					flag = true;
					num = 0f;
				}
				moveHelper.UnreachablePercent = num * 0.5f;
			}
		}
		if (this.manager.pathCostScale < 0.65f)
		{
			float num2 = (1f - this.manager.pathCostScale * 1.5384616f) * 0.6f;
			if (base.RandomFloat < num2)
			{
				PathEntity path = this.theEntity.navigator.getPath();
				if (path != null && path.NodeCountRemaining() > 18 && (attackTarget.position - this.theEntity.position).sqrMagnitude <= 81f)
				{
					flag = true;
				}
			}
		}
		if (!flag && !moveHelper.IsUnreachableAbove)
		{
			return false;
		}
		moveHelper.IsDestroyAreaTryUnreachable = false;
		Vector3 position = this.theEntity.position;
		this.targetPos = (moveHelper.IsUnreachableSide ? moveHelper.UnreachablePos : attackTarget.position);
		this.targetPos.y = Utils.FastMoveTowards(position.y, this.targetPos.y, 2f);
		this.seekPos = Vector3.MoveTowards(position, this.targetPos, 5f);
		this.seekPos.x = this.seekPos.x + (float)(base.Random.RandomRange(11) - 5);
		this.seekPos.z = this.seekPos.z + (float)(base.Random.RandomRange(11) - 5);
		this.findDir = ((int)(Mathf.Atan2(this.targetPos.x - position.x, this.targetPos.z - position.z) * 0.6366198f + 4f) & 3);
		this.findCount = ((base.Random.RandomFloat < 0.1f) ? 4 : 5);
		this.state = EAIDestroyArea.eState.FindExistingPos;
		if (base.RandomFloat < 0.18f)
		{
			this.state = EAIDestroyArea.eState.FindPos;
		}
		return true;
	}

	// Token: 0x06002119 RID: 8473 RVA: 0x000C7BC2 File Offset: 0x000C5DC2
	public override void Start()
	{
		this.isAtPathEnd = false;
		this.delayTime = 3f;
		this.attackTimeout = 0;
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x000C7BE0 File Offset: 0x000C5DE0
	public void Stop()
	{
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		if (moveHelper != null)
		{
			moveHelper.IsDestroyAreaTryUnreachable = false;
		}
		this.delayTime = 0f;
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x000C7C10 File Offset: 0x000C5E10
	public override bool Continue()
	{
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		if (this.delayTime <= 0f)
		{
			return false;
		}
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		if (this.state == EAIDestroyArea.eState.FindExistingPos)
		{
			if (!moveHelper.FindExistingDestroyPos(ref this.seekPos))
			{
				this.state = EAIDestroyArea.eState.FindPos;
				return true;
			}
			this.state = EAIDestroyArea.eState.FindPath;
		}
		if (this.state == EAIDestroyArea.eState.FindPos)
		{
			Vector3 vector;
			if (this.findCount > 4)
			{
				this.findCount--;
				vector = this.targetPos;
				int destroyRadius = 4;
				if (base.RandomFloat < 0.65f)
				{
					vector.x += (float)(base.Random.RandomRange(5) - 2);
					vector.z += (float)(base.Random.RandomRange(5) - 2);
					destroyRadius = 7;
				}
				if (!moveHelper.FindDestroyPos(ref vector, destroyRadius, this.isLookFar))
				{
					return true;
				}
			}
			else
			{
				vector = this.seekPos;
				int num = this.findDir * 2;
				vector.x += EAIDestroyArea.quadrants[num];
				vector.z += EAIDestroyArea.quadrants[num + 1];
				if (!moveHelper.FindDestroyPos(ref vector, 8, this.isLookFar))
				{
					this.findDir = (this.findDir + 1 & 3);
					int num2 = this.findCount - 1;
					this.findCount = num2;
					if (num2 <= 0)
					{
						this.isLookFar = !this.isLookFar;
						return false;
					}
					return true;
				}
			}
			this.seekPos = vector;
			this.state = EAIDestroyArea.eState.FindPath;
		}
		if (this.state == EAIDestroyArea.eState.FindPath)
		{
			this.seekBlockPos = World.worldToBlockPos(this.seekPos);
			this.isLookFar = false;
			this.state = EAIDestroyArea.eState.WaitForPath;
			this.theEntity.navigator.clearPath();
			this.theEntity.FindPath(this.seekPos, this.theEntity.GetMoveSpeedAggro(), true, this);
			moveHelper.IsDestroyArea = true;
			return true;
		}
		if (this.state == EAIDestroyArea.eState.WaitForPath && this.theEntity.navigator.HasPath())
		{
			this.theEntity.navigator.ShortenEnd(0f);
			moveHelper.IsUnreachableAbove = true;
			this.state = EAIDestroyArea.eState.HasPath;
			this.delayTime = 15f;
		}
		if (this.state == EAIDestroyArea.eState.HasPath)
		{
			PathEntity path = this.theEntity.navigator.getPath();
			if (path != null && path.NodeCountRemaining() <= 1)
			{
				this.theEntity.navigator.clearPath();
				this.state = EAIDestroyArea.eState.EndPath;
				this.isAtPathEnd = true;
				this.delayTime = 5f + base.RandomFloat * 6f;
				moveHelper.BlockedFlags = 0;
				this.seekPos.y = Utils.FastMin(this.seekPos.y, this.theEntity.position.y + 2.1f);
				this.seekBlockPos.y = Utils.Fastfloor(this.seekPos.y);
				return true;
			}
		}
		if (this.state == EAIDestroyArea.eState.EndPath && moveHelper.BlockedFlags == 0)
		{
			if (!Voxel.BlockHit(this.hitInfo, this.seekBlockPos))
			{
				this.isLookFar = true;
				return false;
			}
			this.state = EAIDestroyArea.eState.Attack;
			this.theEntity.SeekYawToPos(this.seekPos, 10f);
		}
		return this.isAtPathEnd || !this.theEntity.navigator.noPathAndNotPlanningOne();
	}

	// Token: 0x0600211C RID: 8476 RVA: 0x000C7F54 File Offset: 0x000C6154
	public override void Update()
	{
		this.delayTime -= 0.05f;
		if (this.state == EAIDestroyArea.eState.Attack)
		{
			int num = this.attackTimeout - 1;
			this.attackTimeout = num;
			if (num <= 0)
			{
				ItemActionAttackData itemActionAttackData = this.theEntity.inventory.holdingItemData.actionData[0] as ItemActionAttackData;
				if (itemActionAttackData != null)
				{
					this.theEntity.SetLookPosition(Vector3.zero);
					if (this.theEntity.Attack(false))
					{
						this.attackTimeout = this.theEntity.GetAttackTimeoutTicks();
						itemActionAttackData.hitDelegate = new ItemActionAttackData.HitDelegate(this.GetHitInfo);
						this.theEntity.Attack(true);
						this.state = EAIDestroyArea.eState.EndPath;
					}
				}
			}
		}
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000C800C File Offset: 0x000C620C
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldRayHitInfo GetHitInfo(out float damageScale)
	{
		damageScale = 1f;
		return this.hitInfo;
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x000C801B File Offset: 0x000C621B
	public override void Reset()
	{
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		moveHelper.Stop();
		moveHelper.IsUnreachableAbove = false;
		moveHelper.IsDestroyArea = false;
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x000C803C File Offset: 0x000C623C
	public override string ToString()
	{
		return string.Format("{0}, {1}, delayTime {2}, findC {3}", new object[]
		{
			base.ToString(),
			this.state.ToStringCached<EAIDestroyArea.eState>(),
			this.delayTime.ToCultureInvariantString("0.00"),
			this.findCount
		});
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x000C8094 File Offset: 0x000C6294
	[Conditional("DEBUG_AIDESTROY")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogDestroy(string _format = "", params object[] _args)
	{
		_format = string.Format("{0} EAIDestroyArea {1} {2}, {3}", new object[]
		{
			GameManager.frameCount,
			this.theEntity.EntityName,
			this.theEntity.entityId,
			_format
		});
		Log.Warning(_format, _args);
	}

	// Token: 0x040016B3 RID: 5811
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDumbDistance = 9f;

	// Token: 0x040016B4 RID: 5812
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTowardsMeMaxDist = 5f;

	// Token: 0x040016B5 RID: 5813
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFindDirOffset = 8;

	// Token: 0x040016B6 RID: 5814
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float[] quadrants = new float[]
	{
		9f,
		9f,
		9f,
		-8f,
		-8f,
		-8f,
		-8f,
		9f
	};

	// Token: 0x040016B7 RID: 5815
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 targetPos;

	// Token: 0x040016B8 RID: 5816
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 seekPos;

	// Token: 0x040016B9 RID: 5817
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i seekBlockPos;

	// Token: 0x040016BA RID: 5818
	[PublicizedFrom(EAccessModifier.Private)]
	public int findDir;

	// Token: 0x040016BB RID: 5819
	[PublicizedFrom(EAccessModifier.Private)]
	public int findCount;

	// Token: 0x040016BC RID: 5820
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLookFar;

	// Token: 0x040016BD RID: 5821
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isAtPathEnd;

	// Token: 0x040016BE RID: 5822
	[PublicizedFrom(EAccessModifier.Private)]
	public float delayTime;

	// Token: 0x040016BF RID: 5823
	[PublicizedFrom(EAccessModifier.Private)]
	public int attackTimeout;

	// Token: 0x040016C0 RID: 5824
	[PublicizedFrom(EAccessModifier.Private)]
	public EAIDestroyArea.eState state;

	// Token: 0x040016C1 RID: 5825
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldRayHitInfo hitInfo = new WorldRayHitInfo();

	// Token: 0x02000438 RID: 1080
	[PublicizedFrom(EAccessModifier.Private)]
	public enum eState
	{
		// Token: 0x040016C3 RID: 5827
		FindExistingPos,
		// Token: 0x040016C4 RID: 5828
		FindPos,
		// Token: 0x040016C5 RID: 5829
		FindPath,
		// Token: 0x040016C6 RID: 5830
		WaitForPath,
		// Token: 0x040016C7 RID: 5831
		HasPath,
		// Token: 0x040016C8 RID: 5832
		EndPath,
		// Token: 0x040016C9 RID: 5833
		Attack
	}
}
