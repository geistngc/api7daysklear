using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200041D RID: 1053
[Preserve]
public sealed class AIDirectorSmellMarker : IAIDirectorMarker, IMemoryPoolableObject
{
	// Token: 0x0600207B RID: 8315 RVA: 0x000C4418 File Offset: 0x000C2618
	public void Reference()
	{
		this.m_refCount++;
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x000C4428 File Offset: 0x000C2628
	public bool Release()
	{
		int num = this.m_refCount - 1;
		this.m_refCount = num;
		if (num == 0)
		{
			this.Reset();
			AIDirectorSmellMarker.s_pool.Free(this);
			return true;
		}
		return false;
	}

	// Token: 0x0600207D RID: 8317 RVA: 0x000C445C File Offset: 0x000C265C
	public void Reset()
	{
		this.m_playerState = null;
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x000027FC File Offset: 0x000009FC
	public void Cleanup()
	{
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x000C4468 File Offset: 0x000C2668
	public void Tick(double dt)
	{
		this.m_ttl -= dt;
		if (this.m_ttl < 0.0)
		{
			this.m_ttl = 0.0;
		}
		this.m_validTime -= dt;
		if (this.m_validTime < 0.0)
		{
			this.m_validTime = 0.0;
		}
		this.m_time += dt;
		if (this.m_time > this.m_lifetime)
		{
			this.m_time = this.m_lifetime;
		}
		this.m_effectiveRadius = ((this.m_speed > 0.0) ? Math.Min(this.m_radius, this.m_speed * this.m_time) : this.m_radius);
		this.m_effectiveStrength = this.m_strength * (1.0 - this.m_time / this.m_lifetime);
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x06002080 RID: 8320 RVA: 0x000C4554 File Offset: 0x000C2754
	public EntityPlayer Player
	{
		get
		{
			if (this.m_playerState != null)
			{
				return this.m_playerState.Player;
			}
			return null;
		}
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x000C456C File Offset: 0x000C276C
	public double IntensityForPosition(Vector3 position)
	{
		double num = (double)(this.m_pos - position).magnitude;
		if (num > this.m_effectiveRadius)
		{
			return 0.0;
		}
		double num2 = 1.0;
		if (num > 0.0)
		{
			num2 /= num * num;
		}
		return this.m_effectiveStrength * num2;
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x06002082 RID: 8322 RVA: 0x000C45C6 File Offset: 0x000C27C6
	public Vector3 Position
	{
		get
		{
			return this.m_pos;
		}
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x06002083 RID: 8323 RVA: 0x000C45CE File Offset: 0x000C27CE
	public Vector3 TargetPosition
	{
		get
		{
			return this.m_targetPos;
		}
	}

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x06002084 RID: 8324 RVA: 0x000C45D6 File Offset: 0x000C27D6
	public bool Valid
	{
		get
		{
			return this.m_validTime > 0.0 && (this.Player == null || !this.Player.IsDead());
		}
	}

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x06002085 RID: 8325 RVA: 0x000C4609 File Offset: 0x000C2809
	public float MaxRadius
	{
		get
		{
			return (float)this.m_radius;
		}
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x06002086 RID: 8326 RVA: 0x000C4612 File Offset: 0x000C2812
	public float Radius
	{
		get
		{
			return (float)this.m_effectiveRadius;
		}
	}

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x06002087 RID: 8327 RVA: 0x000C461B File Offset: 0x000C281B
	public float TimeToLive
	{
		get
		{
			return (float)this.m_ttl;
		}
	}

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x06002088 RID: 8328 RVA: 0x000C4624 File Offset: 0x000C2824
	public float ValidTime
	{
		get
		{
			return (float)this.m_validTime;
		}
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x06002089 RID: 8329 RVA: 0x000C462D File Offset: 0x000C282D
	public float Speed
	{
		get
		{
			return (float)this.m_speed;
		}
	}

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x0600208A RID: 8330 RVA: 0x000C4636 File Offset: 0x000C2836
	public int Priority
	{
		get
		{
			return this.m_priority;
		}
	}

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x0600208B RID: 8331 RVA: 0x000C463E File Offset: 0x000C283E
	public bool InterruptsNonPlayerAttack
	{
		get
		{
			return this.m_interruptsNonPlayerAttack;
		}
	}

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x0600208C RID: 8332 RVA: 0x000C4646 File Offset: 0x000C2846
	public bool IsDistraction
	{
		get
		{
			return this.m_isDistraction;
		}
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x0001FFFE File Offset: 0x0001E1FE
	public static AIDirectorSmellMarker Allocate(AIDirectorPlayerState ps, Vector3 position, Vector3 targetPosition, double radius, double strength, double speed, int priority, double ttl, bool interruptsNonPlayerAttack, bool isDistraction)
	{
		return null;
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x000C4650 File Offset: 0x000C2850
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorSmellMarker Construct(AIDirectorPlayerState ps, Vector3 position, Vector3 targetPosition, double radius, double strength, double speed, int priority, double ttl, bool interruptsNonPlayerAttack, bool isDistraction)
	{
		this.m_refCount = 1;
		this.m_playerState = ps;
		this.m_pos = position;
		this.m_targetPos = targetPosition;
		this.m_radius = radius;
		this.m_strength = strength;
		this.m_speed = speed;
		this.m_priority = priority;
		this.m_validTime = ttl;
		this.m_lifetime = ttl;
		this.m_time = 0.0;
		this.m_effectiveRadius = 0.0;
		this.m_effectiveStrength = strength;
		this.m_interruptsNonPlayerAttack = interruptsNonPlayerAttack;
		this.m_isDistraction = isDistraction;
		if (isDistraction)
		{
			this.m_ttl = (double)Mathf.Max((float)ttl, 20f);
		}
		else
		{
			this.m_ttl = (double)Constants.cEnemySenseMemory;
		}
		return this;
	}

	// Token: 0x04001630 RID: 5680
	public const int kMax = 256;

	// Token: 0x04001631 RID: 5681
	[PublicizedFrom(EAccessModifier.Private)]
	public static MemoryPooledObject<AIDirectorSmellMarker> s_pool = new MemoryPooledObject<AIDirectorSmellMarker>(256);

	// Token: 0x04001632 RID: 5682
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_radius;

	// Token: 0x04001633 RID: 5683
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_strength;

	// Token: 0x04001634 RID: 5684
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_speed;

	// Token: 0x04001635 RID: 5685
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_ttl;

	// Token: 0x04001636 RID: 5686
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_validTime;

	// Token: 0x04001637 RID: 5687
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_time;

	// Token: 0x04001638 RID: 5688
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_lifetime;

	// Token: 0x04001639 RID: 5689
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_effectiveRadius;

	// Token: 0x0400163A RID: 5690
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_effectiveStrength;

	// Token: 0x0400163B RID: 5691
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_priority;

	// Token: 0x0400163C RID: 5692
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_refCount;

	// Token: 0x0400163D RID: 5693
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 m_pos;

	// Token: 0x0400163E RID: 5694
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 m_targetPos;

	// Token: 0x0400163F RID: 5695
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorPlayerState m_playerState;

	// Token: 0x04001640 RID: 5696
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_interruptsNonPlayerAttack;

	// Token: 0x04001641 RID: 5697
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_isDistraction;
}
