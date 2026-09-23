using System;

// Token: 0x020013A9 RID: 5033
public class BlendCycleTimer
{
	// Token: 0x06009EAD RID: 40621 RVA: 0x003C01B3 File Offset: 0x003BE3B3
	public BlendCycleTimer(float inTime, float holdTime, float outTime)
	{
		this.m_inTime = inTime;
		this.m_outTime = outTime;
		this.m_holdTime = holdTime;
		this.m_time = 0f;
		this.m_dir = BlendCycleTimer.Dir.Done;
	}

	// Token: 0x06009EAE RID: 40622 RVA: 0x003C01F4 File Offset: 0x003BE3F4
	public void Tick(float dt)
	{
		this.m_blendTimer.Tick(dt);
		switch (this.m_dir)
		{
		case BlendCycleTimer.Dir.In:
			this.m_time += dt;
			if (this.m_time >= this.m_inTime)
			{
				this.m_dir = BlendCycleTimer.Dir.Hold;
				this.m_time = 0f;
				return;
			}
			break;
		case BlendCycleTimer.Dir.Hold:
			if (this.m_holdTime != -1f)
			{
				this.m_time += dt;
				if (this.m_time >= this.m_holdTime)
				{
					this.m_dir = BlendCycleTimer.Dir.Out;
					this.m_time = 0f;
					this.m_blendTimer.BlendTo(0f, this.m_outTime);
				}
			}
			break;
		case BlendCycleTimer.Dir.Out:
			this.m_time += dt;
			if (this.m_time >= this.m_outTime)
			{
				this.m_dir = BlendCycleTimer.Dir.Done;
				return;
			}
			break;
		case BlendCycleTimer.Dir.Done:
			break;
		default:
			return;
		}
	}

	// Token: 0x06009EAF RID: 40623 RVA: 0x003C02D2 File Offset: 0x003BE4D2
	public void FadeIn()
	{
		this.m_dir = BlendCycleTimer.Dir.In;
		this.m_time = this.Value * this.m_inTime;
		this.m_blendTimer.BlendTo(1f, this.m_inTime);
	}

	// Token: 0x06009EB0 RID: 40624 RVA: 0x003C0304 File Offset: 0x003BE504
	public void FadeOut()
	{
		this.m_dir = BlendCycleTimer.Dir.Out;
		this.m_time = (1f - this.Value) * this.m_outTime;
		this.m_blendTimer.BlendTo(0f, this.m_outTime);
	}

	// Token: 0x06009EB1 RID: 40625 RVA: 0x003C033C File Offset: 0x003BE53C
	public void Restart()
	{
		this.m_time = 0f;
		this.m_dir = BlendCycleTimer.Dir.In;
		this.m_blendTimer.BlendTo(0f, 0f);
		this.m_blendTimer.BlendTo(1f, this.m_inTime);
	}

	// Token: 0x170012C4 RID: 4804
	// (get) Token: 0x06009EB2 RID: 40626 RVA: 0x003C037B File Offset: 0x003BE57B
	public BlendCycleTimer.Dir Direction
	{
		get
		{
			return this.m_dir;
		}
	}

	// Token: 0x170012C5 RID: 4805
	// (get) Token: 0x06009EB3 RID: 40627 RVA: 0x003C0383 File Offset: 0x003BE583
	public float Value
	{
		get
		{
			return this.m_blendTimer.Value;
		}
	}

	// Token: 0x0400787D RID: 30845
	[PublicizedFrom(EAccessModifier.Private)]
	public BlendTimer m_blendTimer = new BlendTimer(0f);

	// Token: 0x0400787E RID: 30846
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_inTime;

	// Token: 0x0400787F RID: 30847
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_outTime;

	// Token: 0x04007880 RID: 30848
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_holdTime;

	// Token: 0x04007881 RID: 30849
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_time;

	// Token: 0x04007882 RID: 30850
	[PublicizedFrom(EAccessModifier.Private)]
	public BlendCycleTimer.Dir m_dir;

	// Token: 0x020013AA RID: 5034
	public enum Dir
	{
		// Token: 0x04007884 RID: 30852
		In,
		// Token: 0x04007885 RID: 30853
		Hold,
		// Token: 0x04007886 RID: 30854
		Out,
		// Token: 0x04007887 RID: 30855
		Done
	}
}
