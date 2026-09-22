using System;

namespace PrefabVolumes
{
	// Token: 0x020018C3 RID: 6339
	public class PrefabInfoVolume : PrefabVolumeAbs<PrefabInfoVolume>
	{
		// Token: 0x1700180D RID: 6157
		// (get) Token: 0x0600C39E RID: 50078 RVA: 0x0002F184 File Offset: 0x0002D384
		public override PrefabVolumeAbs.EVolumeType VolumeType
		{
			get
			{
				return PrefabVolumeAbs.EVolumeType.Info;
			}
		}

		// Token: 0x1700180E RID: 6158
		// (get) Token: 0x0600C39F RID: 50079 RVA: 0x0004AC66 File Offset: 0x00048E66
		public override int SerializedSize
		{
			get
			{
				return 25;
			}
		}
	}
}
