using System;

namespace PrefabVolumes
{
	// Token: 0x020018C2 RID: 6338
	public class PrefabTeleportVolume : PrefabVolumeAbs<PrefabTeleportVolume>
	{
		// Token: 0x1700180B RID: 6155
		// (get) Token: 0x0600C39B RID: 50075 RVA: 0x0002003D File Offset: 0x0001E23D
		public override PrefabVolumeAbs.EVolumeType VolumeType
		{
			get
			{
				return PrefabVolumeAbs.EVolumeType.Teleport;
			}
		}

		// Token: 0x1700180C RID: 6156
		// (get) Token: 0x0600C39C RID: 50076 RVA: 0x0004AC66 File Offset: 0x00048E66
		public override int SerializedSize
		{
			get
			{
				return 25;
			}
		}
	}
}
