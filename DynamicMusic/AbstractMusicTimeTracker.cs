using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001A2A RID: 6698
	public abstract class AbstractMusicTimeTracker : AbstractFilter, IFilter<SectionType>
	{
		// Token: 0x0600CB41 RID: 52033 RVA: 0x004A8D13 File Offset: 0x004A6F13
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbstractMusicTimeTracker()
		{
		}

		// Token: 0x04009AF2 RID: 39666
		[PublicizedFrom(EAccessModifier.Protected)]
		public float dailyAllottedPlayTime;

		// Token: 0x04009AF3 RID: 39667
		[PublicizedFrom(EAccessModifier.Protected)]
		public float dailyPlayTimeUsed;

		// Token: 0x04009AF4 RID: 39668
		[PublicizedFrom(EAccessModifier.Protected)]
		public float musicStartTime;

		// Token: 0x04009AF5 RID: 39669
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool IsMusicPlaying;

		// Token: 0x04009AF6 RID: 39670
		[PublicizedFrom(EAccessModifier.Protected)]
		public float pauseDuration;

		// Token: 0x04009AF7 RID: 39671
		[PublicizedFrom(EAccessModifier.Protected)]
		public float pauseStartTime;
	}
}
