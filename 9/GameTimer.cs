using System;
using UnityEngine;

// Token: 0x02001412 RID: 5138
public class GameTimer
{
	// Token: 0x170012EF RID: 4847
	// (get) Token: 0x0600A16F RID: 41327 RVA: 0x003CB44B File Offset: 0x003C964B
	public static GameTimer Instance
	{
		get
		{
			if (GameTimer.m_Instance == null)
			{
				GameTimer.m_Instance = new GameTimer(20f);
			}
			return GameTimer.m_Instance;
		}
	}

	// Token: 0x0600A170 RID: 41328 RVA: 0x003CB468 File Offset: 0x003C9668
	public GameTimer(float _t)
	{
		this.ticksPerSecond = _t;
		this.ms = new MicroStopwatch();
		this.Reset(0UL);
	}

	// Token: 0x0600A171 RID: 41329 RVA: 0x003CB48A File Offset: 0x003C968A
	public void Reset(ulong _ticks = 0UL)
	{
		this.elapsedPartialTicks = 0f;
		this.ticks = _ticks;
		this.ticksSincePlayfieldLoaded = 0UL;
		this.elapsedTicksD = 0.0;
		this.lastMillis = 0L;
		this.ms.ResetAndRestart();
	}

	// Token: 0x0600A172 RID: 41330 RVA: 0x003CB4C8 File Offset: 0x003C96C8
	public void updateTimer(bool _bServerIsStopped)
	{
		if (_bServerIsStopped)
		{
			this.Reset(this.ticks);
			return;
		}
		long elapsedMilliseconds = this.ms.ElapsedMilliseconds;
		long num = elapsedMilliseconds - this.lastMillis;
		this.lastMillis = elapsedMilliseconds;
		this.elapsedTicksD += (double)(Time.timeScale * (float)num) / 1000.0 * (double)this.ticksPerSecond;
		this.elapsedTicks = (int)this.elapsedTicksD;
		this.elapsedPartialTicks = (float)(this.elapsedTicksD - (double)this.elapsedTicks);
		this.elapsedTicksD -= (double)this.elapsedTicks;
		this.ticks += (ulong)((long)this.elapsedTicks);
		this.ticksSincePlayfieldLoaded += (ulong)((long)this.elapsedTicks);
	}

	// Token: 0x040079CB RID: 31179
	public ulong ticks;

	// Token: 0x040079CC RID: 31180
	public ulong ticksSincePlayfieldLoaded;

	// Token: 0x040079CD RID: 31181
	public int elapsedTicks;

	// Token: 0x040079CE RID: 31182
	public float elapsedPartialTicks;

	// Token: 0x040079CF RID: 31183
	[PublicizedFrom(EAccessModifier.Private)]
	public double elapsedTicksD;

	// Token: 0x040079D0 RID: 31184
	[PublicizedFrom(EAccessModifier.Private)]
	public float ticksPerSecond;

	// Token: 0x040079D1 RID: 31185
	[PublicizedFrom(EAccessModifier.Private)]
	public long lastMillis;

	// Token: 0x040079D2 RID: 31186
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameTimer m_Instance;

	// Token: 0x040079D3 RID: 31187
	[PublicizedFrom(EAccessModifier.Private)]
	public MicroStopwatch ms;
}
