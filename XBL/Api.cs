using System;

namespace Platform.XBL
{
	// Token: 0x02001C33 RID: 7219
	public class Api : XblPlatformApi
	{
		// Token: 0x17001A90 RID: 6800
		// (get) Token: 0x0600D62B RID: 54827 RVA: 0x004D471E File Offset: 0x004D291E
		public override string SCID
		{
			get
			{
				return "00000000-0000-0000-0000-0000680ee616";
			}
		}

		// Token: 0x0600D62C RID: 54828 RVA: 0x000027FC File Offset: 0x000009FC
		public override void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600D62D RID: 54829 RVA: 0x0002003D File Offset: 0x0001E23D
		public override bool InitServerApis()
		{
			return true;
		}

		// Token: 0x0600D62E RID: 54830 RVA: 0x000027FC File Offset: 0x000009FC
		public override void ServerApiLoaded()
		{
		}

		// Token: 0x0400A374 RID: 41844
		public const string s_scid = "00000000-0000-0000-0000-0000680ee616";

		// Token: 0x0400A375 RID: 41845
		public const int TitleId = 1745806870;
	}
}
