using System;
using UnityEngine;

// Token: 0x020013A8 RID: 5032
public class BlendTimer
{
	// Token: 0x06009EA6 RID: 40614 RVA: 0x003C0014 File Offset: 0x003BE214
	public BlendTimer() : this(1f)
	{
	}

	// Token: 0x06009EA7 RID: 40615 RVA: 0x003C0024 File Offset: 0x003BE224
	public BlendTimer(float initialValue)
	{
		this.m_time[0] = (this.m_time[1] = 0f);
		float[] value = this.m_value;
		int num = 0;
		float[] value2 = this.m_value;
		int num2 = 1;
		this.m_value[2] = initialValue;
		value[num] = (value2[num2] = initialValue);
	}

	// Token: 0x06009EA8 RID: 40616 RVA: 0x003C0088 File Offset: 0x003BE288
	public void Tick(float dt)
	{
		if (this.m_time[1] != 0f)
		{
			this.m_time[0] += dt;
			if (this.m_time[0] >= this.m_time[1])
			{
				this.m_value[0] = this.m_value[2];
				this.m_time[1] = 0f;
				return;
			}
			this.m_value[0] = Mathf.Lerp(this.m_value[1], this.m_value[2], this.m_time[0] / this.m_time[1]);
		}
	}

	// Token: 0x06009EA9 RID: 40617 RVA: 0x003C0114 File Offset: 0x003BE314
	public void BlendTo(float value, float time)
	{
		if (time > 0f)
		{
			this.m_value[1] = this.m_value[0];
			this.m_value[2] = value;
			this.m_time[0] = 0f;
			this.m_time[1] = time;
			return;
		}
		float[] value2 = this.m_value;
		int num = 0;
		float[] value3 = this.m_value;
		int num2 = 1;
		this.m_value[2] = value;
		value2[num] = (value3[num2] = value);
		this.m_time[1] = 0f;
	}

	// Token: 0x06009EAA RID: 40618 RVA: 0x003C0185 File Offset: 0x003BE385
	public void BlendToRate(float value, float unitsPerSecond)
	{
		this.BlendTo(value, Mathf.Abs(value - this.m_value[0]) / unitsPerSecond);
	}

	// Token: 0x170012C2 RID: 4802
	// (get) Token: 0x06009EAB RID: 40619 RVA: 0x003C019F File Offset: 0x003BE39F
	public float Value
	{
		get
		{
			return this.m_value[0];
		}
	}

	// Token: 0x170012C3 RID: 4803
	// (get) Token: 0x06009EAC RID: 40620 RVA: 0x003C01A9 File Offset: 0x003BE3A9
	public float Target
	{
		get
		{
			return this.m_value[2];
		}
	}

	// Token: 0x0400787B RID: 30843
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] m_value = new float[3];

	// Token: 0x0400787C RID: 30844
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] m_time = new float[2];
}
