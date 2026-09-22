using System;
using System.Collections.Generic;

// Token: 0x02000BFC RID: 3068
public class BiomeLayer
{
	// Token: 0x06005DA9 RID: 23977 RVA: 0x002462BC File Offset: 0x002444BC
	public BiomeLayer(int _depth, BiomeBlockDecoration _bb)
	{
		this.m_Block = _bb;
		this.m_Depth = _depth;
		this.m_Resources = new List<BiomeBlockDecoration>();
		this.SumResourceProbs = new List<float>();
		this.MaxResourceProb = 0f;
	}

	// Token: 0x06005DAA RID: 23978 RVA: 0x002462F4 File Offset: 0x002444F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~BiomeLayer()
	{
	}

	// Token: 0x06005DAB RID: 23979 RVA: 0x0024631C File Offset: 0x0024451C
	public void AddResource(BiomeBlockDecoration _res)
	{
		this.m_Resources.Add(_res);
		this.MaxResourceProb = Utils.FastMax(_res.prob, this.MaxResourceProb);
		int count = this.SumResourceProbs.Count;
		this.SumResourceProbs.Add((count > 0) ? (this.SumResourceProbs[count - 1] + _res.prob) : _res.prob);
	}

	// Token: 0x04004876 RID: 18550
	public BiomeBlockDecoration m_Block;

	// Token: 0x04004877 RID: 18551
	public int m_Depth;

	// Token: 0x04004878 RID: 18552
	public List<BiomeBlockDecoration> m_Resources;

	// Token: 0x04004879 RID: 18553
	public List<float> SumResourceProbs;

	// Token: 0x0400487A RID: 18554
	public float MaxResourceProb;
}
