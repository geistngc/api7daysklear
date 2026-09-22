using System;
using System.Collections.Generic;

namespace MusicUtils
{
	// Token: 0x02001A15 RID: 6677
	public static class SignalProcessing
	{
		// Token: 0x170018C5 RID: 6341
		// (get) Token: 0x0600CB1D RID: 51997 RVA: 0x004A87DF File Offset: 0x004A69DF
		public static float SuspenseRange
		{
			get
			{
				return SignalProcessing.CombatReadyThreshold - SignalProcessing.SuspenseThreshold;
			}
		}

		// Token: 0x170018C6 RID: 6342
		// (get) Token: 0x0600CB1E RID: 51998 RVA: 0x004A87EC File Offset: 0x004A69EC
		public static float SuspenseThreshold
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return 0.25f;
			}
		}

		// Token: 0x170018C7 RID: 6343
		// (get) Token: 0x0600CB1F RID: 51999 RVA: 0x0014D18B File Offset: 0x0014B38B
		public static float CombatReadyThreshold
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x04009A96 RID: 39574
		public const float cSilentVolume = -80f;

		// Token: 0x04009A97 RID: 39575
		public const float cMaxLPFCutoff = 22000f;

		// Token: 0x04009A98 RID: 39576
		public const float cPauseLowPassFilterCutoff = 500f;

		// Token: 0x04009A99 RID: 39577
		public const double cdBFullScaleBase = 1.12246204831;

		// Token: 0x04009A9A RID: 39578
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cExplorationLowPassCutoff = "ExpLPFCutOff";

		// Token: 0x04009A9B RID: 39579
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cExplorationReverbDryLevel = "ExpReverbDryLevel";

		// Token: 0x04009A9C RID: 39580
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cExplorationReverbRoom = "ExpReverbRoom";

		// Token: 0x04009A9D RID: 39581
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cExplorationReverbRoomHF = "ExpReverbRoomHF";

		// Token: 0x04009A9E RID: 39582
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cExplorationDecayHFRatio = "DecayHFRatio";

		// Token: 0x04009A9F RID: 39583
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cExplorationReflectDelay = "ExpReflectDelay";

		// Token: 0x04009AA0 RID: 39584
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cExplorationHFReference = "ExpHFReference";

		// Token: 0x04009AA1 RID: 39585
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cExplorationReverbRoomLF = "ExpReverbRoomLF";

		// Token: 0x04009AA2 RID: 39586
		public static readonly Dictionary<string, Curve> DspCurves = new Dictionary<string, Curve>
		{
			{
				"ExpLPFCutOff",
				new ExponentialCurve(2.0, 22000f, 750f, 0.25f, 0.5f)
			},
			{
				"ExpReverbDryLevel",
				new LogarithmicCurve(2.0, 600.0, 0f, -600f, 0.25f, 0.5f)
			},
			{
				"ExpReverbRoom",
				new LogarithmicCurve(2.0, 600.0, -10000f, -250f, 0.25f, 0.5f)
			},
			{
				"ExpReverbRoomHF",
				new LogarithmicCurve(2.0, 600.0, -600f, -250f, 0.25f, 0.5f)
			},
			{
				"DecayHFRatio",
				new LinearCurve(0.1f, 1f, 0.25f, 0.5f)
			},
			{
				"ExpReflectDelay",
				new LinearCurve(0.02f, 0.2f, 0.25f, 0.5f)
			},
			{
				"ExpHFReference",
				new ExponentialCurve(2.0, 1244.508f, 1174.659f, 0.25f, 0.5f)
			},
			{
				"ExpReverbRoomLF",
				new LogarithmicCurve(2.0, 600.0, -600f, 0f, 0.25f, 0.5f)
			}
		};
	}
}
