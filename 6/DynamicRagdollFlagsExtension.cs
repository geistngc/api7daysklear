using System;

// Token: 0x02000471 RID: 1137
public static class DynamicRagdollFlagsExtension
{
	// Token: 0x06002232 RID: 8754 RVA: 0x000CE82B File Offset: 0x000CCA2B
	public static bool HasFlag(this DynamicRagdollFlags flag, DynamicRagdollFlags checkFlag)
	{
		return (flag & checkFlag) == checkFlag;
	}
}
