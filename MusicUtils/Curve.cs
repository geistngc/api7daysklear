using System;

namespace MusicUtils
{
	// Token: 0x02001A16 RID: 6678
	public abstract class Curve
	{
		// Token: 0x0600CB21 RID: 52001 RVA: 0x004A8985 File Offset: 0x004A6B85
		public Curve(float _start, float _end, float _startX, float _endX)
		{
			this.rate = (_end - _start) / (_endX - _startX);
			this.linearStart = _start;
			this.linearEnd = _end;
			this.startX = _startX;
		}

		// Token: 0x0600CB22 RID: 52002
		public abstract float GetMixerValue(float _param);

		// Token: 0x04009AA3 RID: 39587
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly float rate;

		// Token: 0x04009AA4 RID: 39588
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly float linearStart;

		// Token: 0x04009AA5 RID: 39589
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly float linearEnd;

		// Token: 0x04009AA6 RID: 39590
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly float startX;
	}
}
