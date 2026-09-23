using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004D9 RID: 1241
[Preserve]
public class EntitySwarm : EntityVulture
{
	// Token: 0x06002866 RID: 10342 RVA: 0x000FADDC File Offset: 0x000F8FDC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Init()
	{
		base.Init();
		this.targetAttackHealthPercent = 1f;
		this.ignoreTargetAttached = true;
		this.wanderHeightRange.x = 1f;
		this.wanderHeightRange.y = 8f;
		this.dissipateDelay = 24f;
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000FAE2C File Offset: 0x000F902C
	public override void OnUpdateLive()
	{
		base.OnUpdateLive();
		if (!this.IsDead())
		{
			this.dissipateDelay -= 0.05f;
			if (this.dissipateDelay <= 2f && this.state != EntityVulture.State.Home)
			{
				base.StartHome(this.position + new Vector3(0f, 50f, 0f));
			}
			if (this.dissipateDelay <= 0f)
			{
				this.Kill(DamageResponse.New(true));
			}
		}
	}

	// Token: 0x04001E56 RID: 7766
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dissipateDelay;
}
