using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000430 RID: 1072
[Preserve]
public abstract class EAIBase
{
	// Token: 0x060020F1 RID: 8433 RVA: 0x000C72F3 File Offset: 0x000C54F3
	public virtual void Init(EntityAlive _theEntity)
	{
		this.executeDelay = 0.5f;
		this.manager = _theEntity.aiManager;
		this.theEntity = _theEntity;
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetData(Dictionary<string, string> data)
	{
	}

	// Token: 0x060020F3 RID: 8435 RVA: 0x000C7314 File Offset: 0x000C5514
	[PublicizedFrom(EAccessModifier.Protected)]
	public void GetData(Dictionary<string, string> data, string name, ref float value)
	{
		string input;
		float num;
		if (data.TryGetValue(name, out input) && StringParsers.TryParseFloat(input, out num))
		{
			value = num;
		}
	}

	// Token: 0x060020F4 RID: 8436 RVA: 0x000C733C File Offset: 0x000C553C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void GetData(Dictionary<string, string> data, string name, ref int value)
	{
		string input;
		int num;
		if (data.TryGetValue(name, out input) && StringParsers.TryParseSInt32(input, out num))
		{
			value = num;
		}
	}

	// Token: 0x060020F5 RID: 8437
	public abstract bool CanExecute();

	// Token: 0x060020F6 RID: 8438 RVA: 0x000C7361 File Offset: 0x000C5561
	public virtual bool Continue()
	{
		return this.CanExecute();
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x0002003D File Offset: 0x0001E23D
	public virtual bool IsContinuous()
	{
		return true;
	}

	// Token: 0x060020F8 RID: 8440 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Start()
	{
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Reset()
	{
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void Update()
	{
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x00010E62 File Offset: 0x0000F062
	public virtual bool IsPathUsageBlocked(PathEntity _path)
	{
		return false;
	}

	// Token: 0x060020FC RID: 8444 RVA: 0x000C7369 File Offset: 0x000C5569
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector3 GetTargetPos(EntityAlive theEntity)
	{
		if (theEntity.GetAttackTarget() != null)
		{
			return theEntity.GetAttackTarget().position;
		}
		return theEntity.InvestigatePosition;
	}

	// Token: 0x060020FD RID: 8445 RVA: 0x000C738B File Offset: 0x000C558B
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool EntityHasTarget(EntityAlive theEntity)
	{
		return theEntity.GetAttackTarget() != null || theEntity.HasInvestigatePosition;
	}

	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x060020FE RID: 8446 RVA: 0x000C73A3 File Offset: 0x000C55A3
	public GameRandom Random
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.manager.random;
		}
	}

	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x060020FF RID: 8447 RVA: 0x000C73B0 File Offset: 0x000C55B0
	public float RandomFloat
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.manager.random.RandomFloat;
		}
	}

	// Token: 0x06002100 RID: 8448 RVA: 0x000C73C2 File Offset: 0x000C55C2
	[PublicizedFrom(EAccessModifier.Protected)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetRandom(int maxExclusive)
	{
		return this.manager.random.RandomRange(maxExclusive);
	}

	// Token: 0x06002101 RID: 8449 RVA: 0x000C73D5 File Offset: 0x000C55D5
	public override string ToString()
	{
		if (this.shortedTypeName == null)
		{
			this.shortedTypeName = this.GetTypeName().Substring(3);
		}
		return this.shortedTypeName;
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x000C73F7 File Offset: 0x000C55F7
	public string GetTypeName()
	{
		if (this.cachedTypeName == null)
		{
			this.cachedTypeName = base.GetType().Name;
		}
		return this.cachedTypeName;
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x0000640C File Offset: 0x0000460C
	[PublicizedFrom(EAccessModifier.Protected)]
	public EAIBase()
	{
	}

	// Token: 0x0400169A RID: 5786
	public EAIManager manager;

	// Token: 0x0400169B RID: 5787
	public EntityAlive theEntity;

	// Token: 0x0400169C RID: 5788
	public float executeWaitTime;

	// Token: 0x0400169D RID: 5789
	public float executeDelay;

	// Token: 0x0400169E RID: 5790
	public int MutexBits;

	// Token: 0x0400169F RID: 5791
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedTypeName;

	// Token: 0x040016A0 RID: 5792
	[PublicizedFrom(EAccessModifier.Private)]
	public string shortedTypeName;
}
