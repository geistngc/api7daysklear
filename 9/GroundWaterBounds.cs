using System;
using Unity.Mathematics;

// Token: 0x02000CCB RID: 3275
public struct GroundWaterBounds
{
	// Token: 0x17000A7E RID: 2686
	// (get) Token: 0x06006486 RID: 25734 RVA: 0x002756F5 File Offset: 0x002738F5
	public bool IsGroundWater
	{
		get
		{
			return this.state > 0;
		}
	}

	// Token: 0x06006487 RID: 25735 RVA: 0x00275700 File Offset: 0x00273900
	public GroundWaterBounds(int _groundHeight, int _waterHeight)
	{
		this.state = 1;
		this.waterHeight = (byte)math.clamp(_waterHeight, 0, 255);
		this.bottom = (byte)math.clamp(_groundHeight, 0, (int)this.waterHeight);
	}

	// Token: 0x04004E30 RID: 20016
	[PublicizedFrom(EAccessModifier.Private)]
	public byte state;

	// Token: 0x04004E31 RID: 20017
	public byte waterHeight;

	// Token: 0x04004E32 RID: 20018
	public byte bottom;
}
