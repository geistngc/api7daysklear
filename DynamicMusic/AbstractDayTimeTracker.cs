using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001A27 RID: 6695
	public abstract class AbstractDayTimeTracker : AbstractFilter, IFilter<SectionType>
	{
		// Token: 0x0600CB3C RID: 52028
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract int GetCurrentDay();

		// Token: 0x0600CB3D RID: 52029
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract float GetCurrentTime();

		// Token: 0x0600CB3E RID: 52030 RVA: 0x004A8D13 File Offset: 0x004A6F13
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbstractDayTimeTracker()
		{
		}

		// Token: 0x04009AE7 RID: 39655
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbstractDayTimeTracker.DayPeriodType dayPeriod;

		// Token: 0x04009AE8 RID: 39656
		[PublicizedFrom(EAccessModifier.Protected)]
		public int currentDay;

		// Token: 0x04009AE9 RID: 39657
		[PublicizedFrom(EAccessModifier.Protected)]
		public float currentTime;

		// Token: 0x04009AEA RID: 39658
		[PublicizedFrom(EAccessModifier.Protected)]
		public float dawnTime;

		// Token: 0x04009AEB RID: 39659
		[PublicizedFrom(EAccessModifier.Protected)]
		public float duskTime;

		// Token: 0x02001A28 RID: 6696
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum DayPeriodType : byte
		{
			// Token: 0x04009AED RID: 39661
			Morning,
			// Token: 0x04009AEE RID: 39662
			Day,
			// Token: 0x04009AEF RID: 39663
			Night,
			// Token: 0x04009AF0 RID: 39664
			Dusk,
			// Token: 0x04009AF1 RID: 39665
			Dawn
		}
	}
}
