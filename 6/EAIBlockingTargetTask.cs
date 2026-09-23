using System;
using UnityEngine.Scripting;

// Token: 0x02000435 RID: 1077
[Preserve]
public class EAIBlockingTargetTask : EAIBase
{
	// Token: 0x0600210A RID: 8458 RVA: 0x000C7418 File Offset: 0x000C5618
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 1;
	}

	// Token: 0x0600210B RID: 8459 RVA: 0x000C75BA File Offset: 0x000C57BA
	public override bool CanExecute()
	{
		return this.canExecute;
	}

	// Token: 0x0600210C RID: 8460 RVA: 0x000C75BA File Offset: 0x000C57BA
	public override bool Continue()
	{
		return this.canExecute;
	}

	// Token: 0x040016AE RID: 5806
	public bool canExecute;
}
