using System;

// Token: 0x02001361 RID: 4961
public class BoundaryProjectorTreasure : BoundaryProjector
{
	// Token: 0x1700128F RID: 4751
	// (get) Token: 0x06009C5E RID: 40030 RVA: 0x003B1140 File Offset: 0x003AF340
	// (set) Token: 0x06009C5F RID: 40031 RVA: 0x003B1148 File Offset: 0x003AF348
	public bool WithinRadius
	{
		get
		{
			return this.withinRadius;
		}
		set
		{
			if (this.withinRadius != value)
			{
				this.withinRadius = value;
				this.HandleWithinRadiusChange();
			}
		}
	}

	// Token: 0x17001290 RID: 4752
	// (get) Token: 0x06009C60 RID: 40032 RVA: 0x003B1160 File Offset: 0x003AF360
	public float CurrentRadius
	{
		get
		{
			return this.ProjectorList[0].Projector.orthographicSize;
		}
	}

	// Token: 0x06009C61 RID: 40033 RVA: 0x003B1178 File Offset: 0x003AF378
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetupProjectors()
	{
		base.SetAlpha(0, 1f);
		base.SetAutoRotate(0, true, 2f);
	}

	// Token: 0x06009C62 RID: 40034 RVA: 0x003B1193 File Offset: 0x003AF393
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleWithinRadiusChange()
	{
		base.SetGlow(0, this.withinRadius);
	}

	// Token: 0x04007618 RID: 30232
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool withinRadius;
}
