using System;
using Platform.XBL;

namespace Platform.GameCore
{
	// Token: 0x02001C7C RID: 7292
	public class Api : XblPlatformApi
	{
		// Token: 0x17001AD0 RID: 6864
		// (get) Token: 0x0600D83B RID: 55355 RVA: 0x004DF1AE File Offset: 0x004DD3AE
		public override string SCID
		{
			get
			{
				return this.scid;
			}
		}

		// Token: 0x0600D83C RID: 55356 RVA: 0x004DF1B6 File Offset: 0x004DD3B6
		public override void Init(IPlatform owner)
		{
			Log.Out("[XBL] API Startup. SCID: " + this.SCID);
		}

		// Token: 0x0600D83D RID: 55357 RVA: 0x00010E62 File Offset: 0x0000F062
		public override bool InitServerApis()
		{
			return false;
		}

		// Token: 0x0600D83E RID: 55358 RVA: 0x000027FC File Offset: 0x000009FC
		public override void ServerApiLoaded()
		{
		}

		// Token: 0x0400A4A2 RID: 42146
		[PublicizedFrom(EAccessModifier.Private)]
		public string scid;
	}
}
