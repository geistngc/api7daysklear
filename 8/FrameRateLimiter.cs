using System;
using System.Threading;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class FrameRateLimiter : MonoBehaviour
{
	// Token: 0x0600010A RID: 266 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
	}

	// Token: 0x0600010B RID: 267 RVA: 0x0000BFA4 File Offset: 0x0000A1A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.MaxFrames < 4f)
		{
			this.MaxFrames = 4f;
		}
		if (this.MaxFrames < 60f)
		{
			int num = (int)(1000.0 / (double)this.MaxFrames - (double)(Time.deltaTime * 1000f));
			if (num > 0)
			{
				Thread.Sleep(num);
			}
		}
	}

	// Token: 0x04000128 RID: 296
	public float MaxFrames = 9999f;
}
