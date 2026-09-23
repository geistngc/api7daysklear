using System;
using System.Collections.Generic;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000447 RID: 1095
[Preserve]
public class EAILeap : EAIBase
{
	// Token: 0x0600216B RID: 8555 RVA: 0x000C95D1 File Offset: 0x000C77D1
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 3;
		this.executeDelay = 1f + base.RandomFloat;
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x000C95F3 File Offset: 0x000C77F3
	public override void SetData(Dictionary<string, string> data)
	{
		base.SetData(data);
		base.GetData(data, "legs", ref this.legCount);
	}

	// Token: 0x0600216D RID: 8557 RVA: 0x000C9610 File Offset: 0x000C7810
	public override bool CanExecute()
	{
		if (this.theEntity.IsDancing)
		{
			return false;
		}
		if (!this.theEntity.GetAttackTarget())
		{
			return false;
		}
		if (this.theEntity.Jumping)
		{
			return false;
		}
		if ((this.legCount <= 2) ? this.theEntity.bodyDamage.IsAnyLegMissing : this.theEntity.bodyDamage.IsAnyArmOrLegMissing)
		{
			return false;
		}
		if (this.theEntity.moveHelper.BlockedFlags > 0)
		{
			return false;
		}
		PathEntity path = this.theEntity.navigator.getPath();
		if (path == null)
		{
			return false;
		}
		float jumpMaxDistance = this.theEntity.jumpMaxDistance;
		this.leapV = path.GetEndPos() - this.theEntity.position;
		if (this.leapV.y < -5f || this.leapV.y > 0.5f + jumpMaxDistance * 0.5f)
		{
			return false;
		}
		this.leapDist = Mathf.Sqrt(this.leapV.x * this.leapV.x + this.leapV.z * this.leapV.z);
		if (this.leapDist < 2.8f || this.leapDist > jumpMaxDistance)
		{
			return false;
		}
		Vector3 position = this.theEntity.position;
		position.y += 1.5f;
		RaycastHit raycastHit;
		return !Physics.Raycast(position - Origin.position, this.leapV, out raycastHit, this.leapDist - 0.5f, 1082195968);
	}

	// Token: 0x0600216E RID: 8558 RVA: 0x000C979C File Offset: 0x000C799C
	public override void Start()
	{
		this.abortTime = 5f;
		this.theEntity.moveHelper.Stop();
		this.leapYaw = Mathf.Atan2(this.leapV.x, this.leapV.z) * 57.29578f;
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x000C97EC File Offset: 0x000C79EC
	public override bool Continue()
	{
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		if (this.abortTime <= 0f)
		{
			return false;
		}
		EntityMoveHelper moveHelper = this.theEntity.moveHelper;
		this.theEntity.SeekYaw(this.leapYaw, 0f, 10f);
		if (Utils.FastAbs(Mathf.DeltaAngle(this.theEntity.rotation.y, this.leapYaw)) < 1f)
		{
			moveHelper.StartJump(false, this.leapDist, this.leapV.y);
			return false;
		}
		return true;
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x000C9886 File Offset: 0x000C7A86
	public override void Update()
	{
		this.abortTime -= 0.05f;
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x000027FC File Offset: 0x000009FC
	public override void Reset()
	{
	}

	// Token: 0x040016FC RID: 5884
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCollisionMask = 1082195968;

	// Token: 0x040016FD RID: 5885
	[PublicizedFrom(EAccessModifier.Private)]
	public int legCount = 2;

	// Token: 0x040016FE RID: 5886
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 leapV;

	// Token: 0x040016FF RID: 5887
	[PublicizedFrom(EAccessModifier.Private)]
	public float leapDist;

	// Token: 0x04001700 RID: 5888
	[PublicizedFrom(EAccessModifier.Private)]
	public float leapYaw;

	// Token: 0x04001701 RID: 5889
	[PublicizedFrom(EAccessModifier.Private)]
	public float abortTime;
}
