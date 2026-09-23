using System;

// Token: 0x02001421 RID: 5153
public struct HsvColor
{
	// Token: 0x170012F6 RID: 4854
	// (get) Token: 0x0600A1E4 RID: 41444 RVA: 0x003CE7E0 File Offset: 0x003CC9E0
	// (set) Token: 0x0600A1E5 RID: 41445 RVA: 0x003CE7EF File Offset: 0x003CC9EF
	public float normalizedH
	{
		get
		{
			return (float)this.H / 360f;
		}
		set
		{
			this.H = (double)value * 360.0;
		}
	}

	// Token: 0x170012F7 RID: 4855
	// (get) Token: 0x0600A1E6 RID: 41446 RVA: 0x003CE803 File Offset: 0x003CCA03
	// (set) Token: 0x0600A1E7 RID: 41447 RVA: 0x003CE80C File Offset: 0x003CCA0C
	public float normalizedS
	{
		get
		{
			return (float)this.S;
		}
		set
		{
			this.S = (double)value;
		}
	}

	// Token: 0x170012F8 RID: 4856
	// (get) Token: 0x0600A1E8 RID: 41448 RVA: 0x003CE816 File Offset: 0x003CCA16
	// (set) Token: 0x0600A1E9 RID: 41449 RVA: 0x003CE81F File Offset: 0x003CCA1F
	public float normalizedV
	{
		get
		{
			return (float)this.V;
		}
		set
		{
			this.V = (double)value;
		}
	}

	// Token: 0x0600A1EA RID: 41450 RVA: 0x003CE829 File Offset: 0x003CCA29
	public HsvColor(double h, double s, double v)
	{
		this.H = h;
		this.S = s;
		this.V = v;
	}

	// Token: 0x0600A1EB RID: 41451 RVA: 0x003CE840 File Offset: 0x003CCA40
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"{",
			this.H.ToCultureInvariantString("f2"),
			",",
			this.S.ToCultureInvariantString("f2"),
			",",
			this.V.ToCultureInvariantString("f2"),
			"}"
		});
	}

	// Token: 0x04007A07 RID: 31239
	public double H;

	// Token: 0x04007A08 RID: 31240
	public double S;

	// Token: 0x04007A09 RID: 31241
	public double V;
}
