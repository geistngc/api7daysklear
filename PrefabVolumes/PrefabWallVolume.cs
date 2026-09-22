using System;

namespace PrefabVolumes
{
	// Token: 0x020018C4 RID: 6340
	public class PrefabWallVolume : PrefabVolumeAbs<PrefabWallVolume>
	{
		// Token: 0x1700180F RID: 6159
		// (get) Token: 0x0600C3A1 RID: 50081 RVA: 0x00046EF6 File Offset: 0x000450F6
		public override PrefabVolumeAbs.EVolumeType VolumeType
		{
			get
			{
				return PrefabVolumeAbs.EVolumeType.Wall;
			}
		}

		// Token: 0x17001810 RID: 6160
		// (get) Token: 0x0600C3A2 RID: 50082 RVA: 0x0004AC66 File Offset: 0x00048E66
		public override int SerializedSize
		{
			get
			{
				return 25;
			}
		}
	}
}
