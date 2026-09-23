using System;

// Token: 0x020014B7 RID: 5303
public readonly struct FloatRange
{
	// Token: 0x0600A687 RID: 42631 RVA: 0x003E2681 File Offset: 0x003E0881
	public FloatRange(float _min, float _max)
	{
		this.min = _min;
		this.max = _max;
	}

	// Token: 0x0600A688 RID: 42632 RVA: 0x003E2691 File Offset: 0x003E0891
	public bool IsSet()
	{
		return this.min != 0f || this.max != 0f;
	}

	// Token: 0x0600A689 RID: 42633 RVA: 0x003E26B2 File Offset: 0x003E08B2
	public float Random(GameRandom _rnd)
	{
		return _rnd.RandomRange(this.min, this.max);
	}

	// Token: 0x0600A68A RID: 42634 RVA: 0x003E26C8 File Offset: 0x003E08C8
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"(",
			this.min.ToCultureInvariantString(),
			"-",
			this.max.ToCultureInvariantString(),
			")"
		});
	}

	// Token: 0x04007C13 RID: 31763
	public readonly float min;

	// Token: 0x04007C14 RID: 31764
	public readonly float max;
}
