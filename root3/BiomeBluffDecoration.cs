using System;

// Token: 0x02000BFF RID: 3071
public class BiomeBluffDecoration
{
	// Token: 0x06005DAF RID: 23983 RVA: 0x002464A7 File Offset: 0x002446A7
	public BiomeBluffDecoration(string _name, float _prob, float _minScale, float _maxScale)
	{
		this.m_sName = _name;
		this.m_Prob = _prob;
		this.m_MinScale = _minScale;
		this.m_MaxScale = _maxScale;
	}

	// Token: 0x04004884 RID: 18564
	public string m_sName;

	// Token: 0x04004885 RID: 18565
	public float m_Prob;

	// Token: 0x04004886 RID: 18566
	public float m_MinScale;

	// Token: 0x04004887 RID: 18567
	public float m_MaxScale;
}
