using System;
using UnityEngine.Scripting;

namespace Platform.LAN
{
	// Token: 0x02001CD1 RID: 7377
	[Preserve]
	[PlatformFactory(EPlatformIdentifier.LAN)]
	public class Factory : AbsPlatform
	{
		// Token: 0x0600DB07 RID: 56071 RVA: 0x004E7749 File Offset: 0x004E5949
		public override void CreateInstances()
		{
			if (!base.AsServerOnly)
			{
				throw new NotSupportedException("This platform can only be used as a server platform.");
			}
			base.ServerListAnnouncer = new LANMasterServerAnnouncer();
		}
	}
}
