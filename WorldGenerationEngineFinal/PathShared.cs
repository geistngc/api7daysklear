using System;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001706 RID: 5894
	public class PathShared
	{
		// Token: 0x0600B79E RID: 47006 RVA: 0x0044502C File Offset: 0x0044322C
		public PathShared(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
			this.IdToColor = new Color32[]
			{
				default(Color32),
				this.CountryColor,
				this.HighwayColor,
				this.CountryColor,
				this.WaterColor
			};
		}

		// Token: 0x0600B79F RID: 47007 RVA: 0x004450CC File Offset: 0x004432CC
		public void ConvertIdsToColors(byte[] ids, Color32[] dest)
		{
			for (int i = 0; i < ids.Length; i++)
			{
				int num = (int)ids[i];
				dest[i] = this.IdToColor[num & 15];
			}
		}

		// Token: 0x0400898F RID: 35215
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04008990 RID: 35216
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Color32 CountryColor = new Color32(0, byte.MaxValue, 0, byte.MaxValue);

		// Token: 0x04008991 RID: 35217
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Color32 HighwayColor = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

		// Token: 0x04008992 RID: 35218
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Color32 WaterColor = new Color32(0, 0, byte.MaxValue, byte.MaxValue);

		// Token: 0x04008993 RID: 35219
		public readonly Color32[] IdToColor;
	}
}
