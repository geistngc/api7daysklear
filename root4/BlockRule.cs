using System;

// Token: 0x020001C4 RID: 452
public class BlockRule<O, M>
{
	// Token: 0x06000DE2 RID: 3554 RVA: 0x0000640C File Offset: 0x0000460C
	public BlockRule()
	{
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x0005BB33 File Offset: 0x00059D33
	public BlockRule(O _output, M[] _mask)
	{
		this.Output = _output;
		this.Mask = _mask;
	}

	// Token: 0x04000BF6 RID: 3062
	public O Output;

	// Token: 0x04000BF7 RID: 3063
	public M[] Mask;
}
