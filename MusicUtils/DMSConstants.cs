using System;
using System.Collections.Generic;
using MusicUtils.Enums;

namespace MusicUtils
{
	// Token: 0x02001A0F RID: 6671
	public static class DMSConstants
	{
		// Token: 0x04009A7E RID: 39550
		public static EnumDictionary<SectionType, string> SectionAbbrvs = new EnumDictionary<SectionType, string>
		{
			{
				SectionType.Exploration,
				"Exp"
			},
			{
				SectionType.Suspense,
				"Sus"
			},
			{
				SectionType.Combat,
				"Cbt"
			},
			{
				SectionType.Bloodmoon,
				"Bld"
			}
		};

		// Token: 0x04009A7F RID: 39551
		public static EnumDictionary<LayerType, string> LayerAbbrvs = new EnumDictionary<LayerType, string>
		{
			{
				LayerType.Primary,
				"Pri"
			},
			{
				LayerType.PrimaryPairable1,
				"Pp1"
			},
			{
				LayerType.PrimarySupporting,
				"Psp"
			},
			{
				LayerType.Secondary,
				"Sec"
			},
			{
				LayerType.LongEffects,
				"Lfx"
			},
			{
				LayerType.ShortEffects,
				"Sfx"
			}
		};

		// Token: 0x04009A80 RID: 39552
		public static EnumDictionary<PlacementType, string> PlacementAbbrv = new EnumDictionary<PlacementType, string>
		{
			{
				PlacementType.Begin,
				"a"
			},
			{
				PlacementType.Loop,
				"b"
			},
			{
				PlacementType.End,
				"c"
			}
		};

		// Token: 0x04009A81 RID: 39553
		public const int cChannels = 2;

		// Token: 0x04009A82 RID: 39554
		public const int cFrequency = 44100;

		// Token: 0x04009A83 RID: 39555
		public static List<SectionType> LayeredSections = new List<SectionType>
		{
			SectionType.Exploration,
			SectionType.Suspense,
			SectionType.Combat,
			SectionType.Bloodmoon
		};
	}
}
