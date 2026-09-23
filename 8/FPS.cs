using System;
using UnityEngine;

// Token: 0x02001405 RID: 5125
public class FPS
{
	// Token: 0x0600A0EA RID: 41194 RVA: 0x003C9059 File Offset: 0x003C7259
	public FPS(float _restartTime)
	{
		this.restartTime = _restartTime;
		this.timeleft = _restartTime;
	}

	// Token: 0x0600A0EB RID: 41195 RVA: 0x003C907C File Offset: 0x003C727C
	public bool Update()
	{
		this.timeleft -= Time.unscaledDeltaTime;
		this.accum += Time.unscaledDeltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			this.Counter = (float)this.frames / this.accum;
			this.timeleft = this.restartTime;
			this.accum = 0f;
			this.frames = 0;
			return true;
		}
		return false;
	}

	// Token: 0x0400798A RID: 31114
	[PublicizedFrom(EAccessModifier.Private)]
	public float accum;

	// Token: 0x0400798B RID: 31115
	[PublicizedFrom(EAccessModifier.Private)]
	public int frames;

	// Token: 0x0400798C RID: 31116
	[PublicizedFrom(EAccessModifier.Private)]
	public float timeleft;

	// Token: 0x0400798D RID: 31117
	[PublicizedFrom(EAccessModifier.Private)]
	public float restartTime = 0.5f;

	// Token: 0x0400798E RID: 31118
	public float Counter;
}
