using System;

// Token: 0x02000B0A RID: 2826
public class CBCLayer : IMemoryPoolableObject
{
	// Token: 0x06005512 RID: 21778 RVA: 0x002090AC File Offset: 0x002072AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~CBCLayer()
	{
	}

	// Token: 0x06005513 RID: 21779 RVA: 0x000027FC File Offset: 0x000009FC
	public void Reset()
	{
	}

	// Token: 0x06005514 RID: 21780 RVA: 0x000027FC File Offset: 0x000009FC
	public void Cleanup()
	{
	}

	// Token: 0x06005515 RID: 21781 RVA: 0x002090D4 File Offset: 0x002072D4
	public void CopyFrom(CBCLayer _other)
	{
		Array.Copy(_other.data, this.data, this.data.Length);
	}

	// Token: 0x04004225 RID: 16933
	public readonly byte[] data = new byte[1024];

	// Token: 0x04004226 RID: 16934
	public static int InstanceCount;
}
