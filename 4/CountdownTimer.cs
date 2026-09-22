using System;

// Token: 0x020013C3 RID: 5059
public class CountdownTimer
{
	// Token: 0x170012C6 RID: 4806
	// (get) Token: 0x06009F1B RID: 40731 RVA: 0x003C21E3 File Offset: 0x003C03E3
	// (set) Token: 0x06009F1C RID: 40732 RVA: 0x000027FC File Offset: 0x000009FC
	public long ElapsedMilliseconds
	{
		get
		{
			return (long)this.Elapsed.TotalMilliseconds;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
		}
	}

	// Token: 0x06009F1D RID: 40733 RVA: 0x003C21F1 File Offset: 0x003C03F1
	public CountdownTimer(float _seconds, bool _start = true)
	{
		this.ms = (long)((int)(_seconds * 1000f));
		this.IsRunning = _start;
		this.offset = 0L;
		if (this.IsRunning)
		{
			this.ResetAndRestart();
			return;
		}
		this.Reset();
	}

	// Token: 0x06009F1E RID: 40734 RVA: 0x003C222C File Offset: 0x003C042C
	public void SetTimeout(float _seconds)
	{
		this.ms = (long)((int)(_seconds * 1000f));
	}

	// Token: 0x06009F1F RID: 40735 RVA: 0x003C2240 File Offset: 0x003C0440
	public bool HasPassed()
	{
		bool flag = false;
		if (this.IsRunning)
		{
			this.Update();
			flag = ((this.offset == 0L) ? (this.ElapsedMilliseconds > this.ms) : (this.ElapsedMilliseconds + this.offset > this.ms));
			if (flag)
			{
				this.offset = 0L;
			}
		}
		return flag;
	}

	// Token: 0x06009F20 RID: 40736 RVA: 0x003C2297 File Offset: 0x003C0497
	public void SetPassedIn(float _seconds)
	{
		this.offset = (long)((float)this.ms - _seconds * 1000f) - this.ElapsedMilliseconds;
	}

	// Token: 0x06009F21 RID: 40737 RVA: 0x003C22B8 File Offset: 0x003C04B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.Elapsed = DateTime.Now.Subtract(this.StartTime);
	}

	// Token: 0x06009F22 RID: 40738 RVA: 0x003C22DE File Offset: 0x003C04DE
	public void Reset()
	{
		this.Elapsed = TimeSpan.Zero;
		this.StartTime = DateTime.Now;
		this.IsRunning = false;
	}

	// Token: 0x06009F23 RID: 40739 RVA: 0x003C22FD File Offset: 0x003C04FD
	public void ResetAndRestart()
	{
		this.Reset();
		this.IsRunning = true;
	}

	// Token: 0x06009F24 RID: 40740 RVA: 0x003C230C File Offset: 0x003C050C
	public void Stop()
	{
		this.IsRunning = false;
	}

	// Token: 0x040078D4 RID: 30932
	[PublicizedFrom(EAccessModifier.Private)]
	public long ms;

	// Token: 0x040078D5 RID: 30933
	[PublicizedFrom(EAccessModifier.Private)]
	public long offset;

	// Token: 0x040078D6 RID: 30934
	public TimeSpan Elapsed;

	// Token: 0x040078D7 RID: 30935
	public bool IsRunning;

	// Token: 0x040078D8 RID: 30936
	[PublicizedFrom(EAccessModifier.Private)]
	public DateTime StartTime;
}
