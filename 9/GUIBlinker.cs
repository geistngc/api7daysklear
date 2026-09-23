using System;

// Token: 0x0200122A RID: 4650
public class GUIBlinker
{
	// Token: 0x06009481 RID: 38017 RVA: 0x0038295D File Offset: 0x00380B5D
	public GUIBlinker(float _ms)
	{
		this.ms = _ms;
	}

	// Token: 0x06009482 RID: 38018 RVA: 0x00382973 File Offset: 0x00380B73
	public bool Draw(float _curTime)
	{
		if (_curTime - this.lastBlinkTime > this.ms)
		{
			this.lastBlinkTime = _curTime;
			this.bResult = !this.bResult;
		}
		return this.bResult;
	}

	// Token: 0x04006F3F RID: 28479
	[PublicizedFrom(EAccessModifier.Private)]
	public float ms;

	// Token: 0x04006F40 RID: 28480
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastBlinkTime;

	// Token: 0x04006F41 RID: 28481
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bResult = true;
}
