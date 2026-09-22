using System;
using System.Collections.Generic;
using Platform.LAN;
using Platform.Shared;
using UnityEngine.Scripting;

namespace Platform.Local
{
	// Token: 0x02001CCC RID: 7372
	[Preserve]
	[PlatformFactory(EPlatformIdentifier.Local)]
	public class Factory : AbsPlatform
	{
		// Token: 0x0600DADD RID: 56029 RVA: 0x004E7470 File Offset: 0x004E5670
		public override void CreateInstances()
		{
			base.Api = new Api();
			if (!base.AsServerOnly)
			{
				base.User = new User();
				LocalServerDetect localServerDetect = new LocalServerDetect();
				base.ServerListInterfaces = new List<IServerListInterface>
				{
					localServerDetect,
					new FavoriteServers(),
					new LANServerList()
				};
				base.ServerLookupInterface = localServerDetect;
				base.Utils = new Platform.Shared.Utils();
				base.Input = new PlayerInputManager();
				base.RemoteFileStorage = new RemoteFileStorage();
				base.EntitlementValidators = new List<IEntitlementValidator>
				{
					new DownloadableContentValidator()
				};
			}
		}
	}
}
