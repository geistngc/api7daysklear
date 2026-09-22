using System;

// Token: 0x02000445 RID: 1093
public readonly struct AIFocus<T> where T : struct, IFocusTarget
{
	// Token: 0x06002161 RID: 8545 RVA: 0x000C9567 File Offset: 0x000C7767
	public AIFocus(bool thisIsDumb)
	{
		this.FocusTargets = new T[4];
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x000C9575 File Offset: 0x000C7775
	public void SetFocus(FocusPriority priority, T newTarget)
	{
		this.FocusTargets[(int)priority] = newTarget;
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x000C9584 File Offset: 0x000C7784
	public void ClearFocus(FocusPriority priority)
	{
		this.FocusTargets[(int)priority] = Activator.CreateInstance<T>();
	}

	// Token: 0x040016FA RID: 5882
	public readonly T[] FocusTargets;
}
