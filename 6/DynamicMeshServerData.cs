using System;

// Token: 0x0200038F RID: 911
public abstract class DynamicMeshServerData : NetPackage
{
	// Token: 0x06001B22 RID: 6946
	public abstract bool Prechecks();

	// Token: 0x06001B23 RID: 6947 RVA: 0x0005F8B4 File Offset: 0x0005DAB4
	[PublicizedFrom(EAccessModifier.Protected)]
	public DynamicMeshServerData()
	{
	}

	// Token: 0x0400117B RID: 4475
	public int X;

	// Token: 0x0400117C RID: 4476
	public int Z;
}
